using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Net;
using ICSharpCode.SharpZipLib.Zip.Compression;
using System.Net.WebSockets;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordDAVECalling.Networking
{
    class WebSocket
    {
        // Discord's WebSocket / Gateway URL
        private string gatewayUrl;

        // The Discord token used by the user
        private string DscToken;

        // Used for sending the first payload required
        private string identifyPayloadJson;
        private string voicePayloadJson;

        // Used for the heartbeat payloads
        private readonly string heartbeatPayloadJson = JsonSerializer.Serialize(new { op = 1, d = (object)null });
        private Task heartbeatTask;
        private CancellationTokenSource heartbeatCts;

        // The interval Discord sends back to us from WebSocket
        private int heartbeatInterval;

        public ClientWebSocket WSClient { get; private set; }

        // Reusable buffers for memory efficiency
        private readonly byte[] _receiveBuffer = new byte[8192];
        private readonly ArraySegment<byte> _heartbeatBuffer;
        private readonly ArraySegment<byte> _identifyBuffer;

        private CancellationTokenSource _receiveCts;

        // Voice call properties
        public string userId;
        public string sessionId;
        public string voiceEndpoint;
        public string voiceToken;
        public event Action VoiceServerUpdateCompleted;

        public WebSocket(string token, string channelId)
        {
            DscToken = token;
            var config = new ConfigMgr();

            gatewayUrl = "wss://gateway.discord.gg/?encoding=json&v=9&compress=zlib-stream";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            identifyPayloadJson = JsonSerializer.Serialize(new
            {
                op = 2,
                d = new
                {
                    token = token,
                    properties = new
                    {
                        os = config.OperatingSystem,
                        browser = config.BrowserName,
                        device = string.Empty,
                        system_locale = config.SystemLocale,
                        has_client_mods = config.HasClientMods,
                        browser_user_agent = config.BrowserUA,
                        browser_version = config.BrowserVer,
                        os_version = config.OSVersion,
                        referrer = config.DCReferrer,
                        referring_domain = config.DCReferringDomain,
                        referrer_current = config.DCReferringCurrent,
                        referring_domain_current = config.DCReferringCurrentDomain,
                        release_channel = config.DCClientState,
                        client_event_source = config.DCClientEvtSrc,
                        client_launch_id = config.ClientLaunchId,
                        is_fast_connect = true
                    }
                },
                client_state = new { guild_versions = new { } }
            });

            voicePayloadJson = JsonSerializer.Serialize(new
            {
                op = 4,
                d = new
                {
                    guild_id = (string)null,
                    channel_id = channelId,
                    self_mute = false,
                    self_deaf = false,
                    self_video = false,
                    flags = 2
                }
            });

            _heartbeatBuffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(heartbeatPayloadJson));
            _identifyBuffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(identifyPayloadJson));

            ConnectAsync();
        }

        public async Task ConnectAsync()
        {
            await InitWS();
        }

        private async Task InitWS()
        {
            WSClient = new ClientWebSocket();
            WSClient.Options.KeepAliveInterval = TimeSpan.FromSeconds(20);
            _inflater = new Inflater();

            var uri = new Uri(gatewayUrl);
            await WSClient.ConnectAsync(uri, CancellationToken.None).ConfigureAwait(false);

            _receiveCts = new CancellationTokenSource();
            _ = Task.Run(() => ReceiveLoop(_receiveCts.Token));
        }

        internal async Task SendPayload(string payload = null)
        {
            if (WSClient?.State != WebSocketState.Open) return;

            if (payload == null)
            {
                await WSClient.SendAsync(_identifyBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
                return;
            }

            var byteCount = Encoding.UTF8.GetByteCount(payload);
            byte[] buffer = ArrayPool<byte>.Shared.Rent(byteCount);

            try
            {
                int bytesWritten = Encoding.UTF8.GetBytes(payload, 0, payload.Length, buffer, 0);
                await WSClient.SendAsync(new ArraySegment<byte>(buffer, 0, bytesWritten), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        private async Task ReceiveLoop(CancellationToken cancellationToken)
        {
            using var ms = new MemoryStream();
            try
            {
                while (WSClient.State == WebSocketState.Open)
                {
                    var result = await WSClient.ReceiveAsync(new ArraySegment<byte>(_receiveBuffer), cancellationToken);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Debug.WriteLine($"Server closed connection: {result.CloseStatus}");
                        await ReconnectWithDelay(1);
                        return;
                    }

                    if (result.Count > 0)
                    {
                        ms.Write(_receiveBuffer, 0, result.Count);
                    }

                    if (result.EndOfMessage)
                    {
                        byte[] data = ms.ToArray();
                        string message = DecodeZStream(data);
                        ms.SetLength(0);
                        HandleMessage(message);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected during shutdown
            }
            catch (WebSocketException ex)
            {
                Debug.WriteLine($"WebSocket error: {ex.Message}");
                await ReconnectWithDelay();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"WebSocket error: {ex.Message}");
                await ReconnectWithDelay();
            }
        }

        private Inflater _inflater;

        public string DecodeZStream(byte[] compressed)
        {
            if (!EndsWithFlushSuffix(compressed)) return null;
            _inflater.SetInput(compressed);
            using (var output = new MemoryStream())
            {
                byte[] buf = new byte[4096];
                int read;
                while ((read = _inflater.Inflate(buf)) > 0)
                    output.Write(buf, 0, read);
                return Encoding.UTF8.GetString(output.ToArray());
            }
        }

        private bool EndsWithFlushSuffix(byte[] data)
        {
            if (data.Length < 4) return false;
            return data[data.Length - 4] == 0x00 && data[data.Length - 3] == 0x00 &&
                   data[data.Length - 2] == 0xFF && data[data.Length - 1] == 0xFF;
        }

        private void HandleVoiceStateUpdate(JsonNode data)
        {
            if (data is null) return;
            userId = data["user_id"]?.GetValue<string>();
            sessionId = data["session_id"]?.GetValue<string>();
        }

        private void HandleVoiceServerUpdate(JsonNode data)
        {
            if (data is null) return;
            voiceToken = data["token"]?.GetValue<string>();
            voiceEndpoint = data["endpoint"]?.GetValue<string>();
            VoiceServerUpdateCompleted?.Invoke();
        } 

        private async void HandleMessage(string data)
        {
            try
            {
                var json = JsonNode.Parse(data);
                int opCode = json["op"]?.GetValue<int>() ?? -1;

                switch (opCode)
                {
                    case 0:
                        string eventType = json["t"]?.GetValue<string>() ?? "";

                        switch (eventType)
                        {
                            case "READY":
                                // Send the voice payload that we generated
                                await SendPayload(voicePayloadJson);
                                Debug.WriteLine("Sent the voice payload over to Discord.");
                                break;
                            case "VOICE_STATE_UPDATE":
                                HandleVoiceStateUpdate(json["d"]);
                                break;
                            case "VOICE_SERVER_UPDATE":
                                HandleVoiceServerUpdate(json["d"]);
                                break;
                        }
                        break;
                    case 10: // Hello from the gateway (Op 10)
                        Debug.WriteLine("Discord has said hello to us from the gateway.");
                        heartbeatInterval = json["d"]?["heartbeat_interval"]?.GetValue<int>() ?? 41250;
                        StartHeartbeat();
                        await SendPayload();
                        break;
                    default:
                        Debug.WriteLine($"Unhandled op code: {opCode}, with the data: {data}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error processing message: {ex.Message}");
            }
        }

        private void StartHeartbeat()
        {
            StopHeartbeat();
            heartbeatCts = new CancellationTokenSource();
            heartbeatTask = Task.Run(async () =>
            {
                var token = heartbeatCts.Token;
                while (!token.IsCancellationRequested && WSClient.State == WebSocketState.Open)
                {
                    await Task.Delay(heartbeatInterval, token);
                    if (WSClient.State == WebSocketState.Open)
                        await WSClient.SendAsync(_heartbeatBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            });
        }

        private void StopHeartbeat()
        {
            heartbeatCts?.Cancel();
            heartbeatCts?.Dispose();
            heartbeatCts = null;
        }

        private async Task ReconnectWithDelay(int attempt = 1)
        {
            WSDispose();

            int delayMs = Math.Min(1000 * (int)Math.Pow(2, attempt), 30000);
            await Task.Delay(delayMs);

            try
            {
                await InitWS();
            }
            catch
            {
                _ = ReconnectWithDelay(attempt + 1);
            }
        }

        public void WSDispose()
        {
            StopHeartbeat();
            _receiveCts?.Cancel();
            _receiveCts?.Dispose();
            try
            {
                WSClient?.Abort();
            }
            catch { /* This ignores any abort errors */ }
            WSClient?.Dispose();
        }
    }
}