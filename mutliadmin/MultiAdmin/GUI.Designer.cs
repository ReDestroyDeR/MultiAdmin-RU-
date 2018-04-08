using System.Drawing;

namespace MultiAdmin.MultiAdmin
{
    partial class GUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GUI));
            ServerName = new System.Windows.Forms.Label();
            PlayerCount = new System.Windows.Forms.Label();
            UniqueLogins = new System.Windows.Forms.Label();
            Uptime = new System.Windows.Forms.Label();
            ServerIP = new System.Windows.Forms.Label();
            ServerMod_Version = new System.Windows.Forms.Label();
            MultiAdmin_Version = new System.Windows.Forms.Label();
            ActionType = new System.Windows.Forms.ComboBox();
            Action = new System.Windows.Forms.ComboBox();
            Argument = new System.Windows.Forms.ComboBox();
            Username = new System.Windows.Forms.TextBox();
            SendButton = new System.Windows.Forms.Button();
            Shell = new System.Windows.Forms.TextBox();
            ModeratorLog = new System.Windows.Forms.TextBox();
            InputLine = new System.Windows.Forms.TextBox();
            NoMonitor = new System.Windows.Forms.Label();
            SCPSLM_V = new System.Windows.Forms.Label();
            OpenMonitorConfig = new System.Windows.Forms.Button();
            StopMonitor = new System.Windows.Forms.Button();
            StartMonitor = new System.Windows.Forms.Button();
            RetryButton = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // ServerName
            // 
            resources.ApplyResources(ServerName, "ServerName");
            ServerName.Name = "ServerName";
            // 
            // PlayerCount
            // 
            resources.ApplyResources(PlayerCount, "PlayerCount");
            PlayerCount.Name = "PlayerCount";
            // 
            // UniqueLogins
            // 
            resources.ApplyResources(UniqueLogins, "UniqueLogins");
            UniqueLogins.Name = "UniqueLogins";
            // 
            // Uptime
            // 
            resources.ApplyResources(Uptime, "Uptime");
            Uptime.Name = "Uptime";
            // 
            // ServerIP
            // 
            resources.ApplyResources(ServerIP, "ServerIP");
            ServerIP.Name = "ServerIP";
            // 
            // ServerMod_Version
            // 
            resources.ApplyResources(ServerMod_Version, "ServerMod_Version");
            ServerMod_Version.Name = "ServerMod_Version";
            // 
            // MultiAdmin_Version
            // 
            resources.ApplyResources(MultiAdmin_Version, "MultiAdmin_Version");
            MultiAdmin_Version.Name = "MultiAdmin_Version";
            // 
            // ActionType
            // 
            ActionType.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(ActionType, "ActionType");
            ActionType.FormattingEnabled = true;
            ActionType.Name = "ActionType";
            ActionType.SelectedIndexChanged += new System.EventHandler(ActionType_SelectedIndexChanged);
            // 
            // Action
            // 
            Action.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(Action, "Action");
            Action.FormattingEnabled = true;
            Action.Name = "Action";
            Action.SelectedIndexChanged += new System.EventHandler(Action_SelectedIndexChanged);
            // 
            // Argument
            // 
            Argument.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(Argument, "Argument");
            Argument.FormattingEnabled = true;
            Argument.Name = "Argument";
            Argument.SelectedIndexChanged += new System.EventHandler(Argument_SelectedIndexChanged);
            // 
            // Username
            // 
            Username.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(Username, "Username");
            Username.Name = "Username";
            Username.ReadOnly = true;
            // 
            // SendButton
            // 
            SendButton.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(SendButton, "SendButton");
            SendButton.Name = "SendButton";
            SendButton.UseVisualStyleBackColor = true;
            SendButton.Click += new System.EventHandler(SendButton_Click);
            // 
            // Shell
            // 
            Shell.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            Shell.Cursor = System.Windows.Forms.Cursors.Arrow;
            resources.ApplyResources(Shell, "Shell");
            Shell.ForeColor = System.Drawing.Color.MediumAquamarine;
            Shell.Name = "Shell";
            Shell.ReadOnly = true;
            // 
            // ModeratorLog
            // 
            ModeratorLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            ModeratorLog.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(ModeratorLog, "ModeratorLog");
            ModeratorLog.ForeColor = System.Drawing.Color.SpringGreen;
            ModeratorLog.Name = "ModeratorLog";
            ModeratorLog.ReadOnly = true;
            // 
            // InputLine
            // 
            InputLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            InputLine.ForeColor = System.Drawing.Color.SpringGreen;
            resources.ApplyResources(InputLine, "InputLine");
            InputLine.Name = "InputLine";
            InputLine.TextChanged += new System.EventHandler(InputLine_TextChanged);
            // 
            // NoMonitor
            // 
            resources.ApplyResources(NoMonitor, "NoMonitor");
            NoMonitor.BackColor = System.Drawing.Color.White;
            NoMonitor.Name = "NoMonitor";
            // 
            // SCPSLM_V
            // 
            resources.ApplyResources(SCPSLM_V, "SCPSLM_V");
            SCPSLM_V.Name = "SCPSLM_V";
            // 
            // OpenMonitorConfig
            // 
            resources.ApplyResources(OpenMonitorConfig, "OpenMonitorConfig");
            OpenMonitorConfig.Name = "OpenMonitorConfig";
            OpenMonitorConfig.UseVisualStyleBackColor = true;
            OpenMonitorConfig.Click += new System.EventHandler(OpenMonitorConfig_Click);
            // 
            // StopMonitor
            // 
            resources.ApplyResources(StopMonitor, "StopMonitor");
            StopMonitor.Name = "StopMonitor";
            StopMonitor.UseVisualStyleBackColor = true;
            StopMonitor.Click += new System.EventHandler(StopMonitor_Click);
            // 
            // StartMonitor
            // 
            resources.ApplyResources(StartMonitor, "StartMonitor");
            StartMonitor.Name = "StartMonitor";
            StartMonitor.UseVisualStyleBackColor = true;
            StartMonitor.Click += new System.EventHandler(StartMonitor_Click);
            // 
            // RetryButton
            // 
            resources.ApplyResources(RetryButton, "RetryButton");
            RetryButton.Name = "RetryButton";
            RetryButton.UseVisualStyleBackColor = true;
            RetryButton.Click += new System.EventHandler(RetryButton_Click);
            // 
            // GUI
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            Controls.Add(RetryButton);
            Controls.Add(StartMonitor);
            Controls.Add(StopMonitor);
            Controls.Add(OpenMonitorConfig);
            Controls.Add(SCPSLM_V);
            Controls.Add(InputLine);
            Controls.Add(ModeratorLog);
            Controls.Add(Shell);
            Controls.Add(SendButton);
            Controls.Add(Username);
            Controls.Add(Argument);
            Controls.Add(Action);
            Controls.Add(ActionType);
            Controls.Add(MultiAdmin_Version);
            Controls.Add(ServerMod_Version);
            Controls.Add(ServerIP);
            Controls.Add(Uptime);
            Controls.Add(UniqueLogins);
            Controls.Add(PlayerCount);
            Controls.Add(ServerName);
            Controls.Add(NoMonitor);
            DoubleBuffered = true;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            HelpButton = true;
            Name = "GUI";
            TransparencyKey = System.Drawing.Color.Coral;
            Load += new System.EventHandler(GUI_Load);
            FormClosing += new System.Windows.Forms.FormClosingEventHandler(GUI_FormClosing);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        public static System.Windows.Forms.Label NoMonitor;
        public static System.Windows.Forms.Label SCPSLM_V;
        public static System.Windows.Forms.Button OpenMonitorConfig;
        public static System.Windows.Forms.Button StopMonitor;
        public static System.Windows.Forms.Button StartMonitor;
        public static System.Windows.Forms.Label ServerName;
        public static System.Windows.Forms.Label PlayerCount;
        public static System.Windows.Forms.Label UniqueLogins;
        public static System.Windows.Forms.Label Uptime;
        public static System.Windows.Forms.Label ServerIP;
        public static System.Windows.Forms.Label ServerMod_Version;
        public static System.Windows.Forms.Label MultiAdmin_Version;
        public static System.Windows.Forms.ComboBox ActionType;
        public static System.Windows.Forms.ComboBox Action;
        public static System.Windows.Forms.ComboBox Argument;
        public static System.Windows.Forms.TextBox Username;
        public static System.Windows.Forms.Button SendButton;
        public static System.Windows.Forms.TextBox Shell;
        public static System.Windows.Forms.TextBox ModeratorLog;
        public static System.Windows.Forms.TextBox InputLine;
        public static System.Windows.Forms.Button RetryButton;
    }
}