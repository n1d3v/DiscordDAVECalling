
namespace DiscordDAVECalling
{
    partial class CallingUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CallingUI));
            this.enterLabel = new System.Windows.Forms.Label();
            this.tokenBox = new System.Windows.Forms.TextBox();
            this.loginButton = new System.Windows.Forms.Button();
            this.callButton = new System.Windows.Forms.Button();
            this.noticeLabel = new System.Windows.Forms.Label();
            this.channelIdBox = new System.Windows.Forms.TextBox();
            this.cidLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // enterLabel
            // 
            this.enterLabel.AutoSize = true;
            this.enterLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.enterLabel.Location = new System.Drawing.Point(12, 9);
            this.enterLabel.Name = "enterLabel";
            this.enterLabel.Size = new System.Drawing.Size(330, 15);
            this.enterLabel.TabIndex = 0;
            this.enterLabel.Text = "Enter your Discord token to get your DMs list to call someone";
            // 
            // tokenBox
            // 
            this.tokenBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tokenBox.Location = new System.Drawing.Point(15, 32);
            this.tokenBox.Name = "tokenBox";
            this.tokenBox.Size = new System.Drawing.Size(652, 23);
            this.tokenBox.TabIndex = 1;
            this.tokenBox.UseSystemPasswordChar = true;
            // 
            // loginButton
            // 
            this.loginButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.loginButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loginButton.Location = new System.Drawing.Point(435, 64);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(232, 23);
            this.loginButton.TabIndex = 2;
            this.loginButton.Text = "Log into Discord to start the program!";
            this.loginButton.UseVisualStyleBackColor = true;
            this.loginButton.Click += new System.EventHandler(this.loginButton_Click);
            // 
            // callButton
            // 
            this.callButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.callButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.callButton.Location = new System.Drawing.Point(579, 132);
            this.callButton.Name = "callButton";
            this.callButton.Size = new System.Drawing.Size(88, 23);
            this.callButton.TabIndex = 4;
            this.callButton.Text = "Ring it up!";
            this.callButton.UseVisualStyleBackColor = true;
            this.callButton.Click += new System.EventHandler(this.callButton_Click);
            // 
            // noticeLabel
            // 
            this.noticeLabel.AutoSize = true;
            this.noticeLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.noticeLabel.Location = new System.Drawing.Point(12, 68);
            this.noticeLabel.Name = "noticeLabel";
            this.noticeLabel.Size = new System.Drawing.Size(384, 15);
            this.noticeLabel.TabIndex = 5;
            this.noticeLabel.Text = "Enter the user ID and the channel ID of the user / group you want to call";
            // 
            // channelIdBox
            // 
            this.channelIdBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.channelIdBox.Location = new System.Drawing.Point(45, 100);
            this.channelIdBox.Name = "channelIdBox";
            this.channelIdBox.Size = new System.Drawing.Size(622, 23);
            this.channelIdBox.TabIndex = 11;
            // 
            // cidLabel
            // 
            this.cidLabel.AutoSize = true;
            this.cidLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cidLabel.Location = new System.Drawing.Point(13, 103);
            this.cidLabel.Name = "cidLabel";
            this.cidLabel.Size = new System.Drawing.Size(26, 15);
            this.cidLabel.TabIndex = 10;
            this.cidLabel.Text = "CID";
            // 
            // CallingUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(679, 167);
            this.Controls.Add(this.channelIdBox);
            this.Controls.Add(this.cidLabel);
            this.Controls.Add(this.noticeLabel);
            this.Controls.Add(this.callButton);
            this.Controls.Add(this.loginButton);
            this.Controls.Add(this.tokenBox);
            this.Controls.Add(this.enterLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CallingUI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Discord DAVE testing environment";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label enterLabel;
        private System.Windows.Forms.TextBox tokenBox;
        private System.Windows.Forms.Button loginButton;
        private System.Windows.Forms.Button callButton;
        private System.Windows.Forms.Label noticeLabel;
        private System.Windows.Forms.TextBox channelIdBox;
        private System.Windows.Forms.Label cidLabel;
    }
}