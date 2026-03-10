using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using DiscordDAVECalling.Networking;
using System.Diagnostics;

namespace DiscordDAVECalling
{
    public partial class CallingUI : Form
    {
        public CallingUI()
        {
            InitializeComponent();
            if (string.IsNullOrEmpty(Properties.Settings.Default.dscToken))
            {
                // Do nothing
            }
            else
            {
                tokenBox.Text = Properties.Settings.Default.dscToken;
                channelIdBox.Text = Properties.Settings.Default.channelId;

                enterLabel.Text = "Logged into Discord using your token, you may now ring someone.";
                loginButton.Enabled = false;
            }
        }

        private async Task InitializeCall()
        {
            var tcs = new TaskCompletionSource<bool>();

            // The average Discord gateway WebSocket
            WebSocket normalSocket = new WebSocket(Properties.Settings.Default.dscToken, Properties.Settings.Default.channelId);

            // The socket used to call people on Discord
            normalSocket.VoiceServerUpdateCompleted += () =>
            {
                CallSocket callSocket = new CallSocket(normalSocket.voiceEndpoint, normalSocket.voiceToken, normalSocket.sessionId, normalSocket.userId, Properties.Settings.Default.channelId);
                tcs.TrySetResult(true);
            };
            // Await the event for the CallSocket to start up
            await tcs.Task;
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tokenBox.Text))
            {
                MessageBox.Show("Hey jackass, you didn't enter a token, don't think I will let you get away with this! You will be punished for life.", "Why did you do this to me?", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            enterLabel.Text = "Logged into Discord using your token, you may now ring someone.";
            Properties.Settings.Default.dscToken = tokenBox.Text;
            Properties.Settings.Default.Save();
        }

        private async void callButton_Click(object sender, EventArgs e)
        {
            // Save the setting properties to storage so we can use them when the app restarts
            Properties.Settings.Default.channelId = channelIdBox.Text;
            Properties.Settings.Default.Save();

            await InitializeCall();
        }
    }
}