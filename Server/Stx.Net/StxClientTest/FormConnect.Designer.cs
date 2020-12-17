namespace StxClientTest
{
    partial class FormConnect
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
            this.textBoxHost = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxConnect = new System.Windows.Forms.GroupBox();
            this.buttonRandomClient = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.checkBoxUseName = new System.Windows.Forms.CheckBox();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.groupBoxVersioning = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxAppKey = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxAppVersion = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxAppName = new System.Windows.Forms.TextBox();
            this.comboBoxAuthToken = new System.Windows.Forms.ComboBox();
            this.comboBoxClientID = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBoxConnect.SuspendLayout();
            this.groupBoxVersioning.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxHost
            // 
            this.textBoxHost.Location = new System.Drawing.Point(123, 19);
            this.textBoxHost.Name = "textBoxHost";
            this.textBoxHost.Size = new System.Drawing.Size(182, 20);
            this.textBoxHost.TabIndex = 0;
            this.textBoxHost.Text = "localhost";
            this.textBoxHost.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Host:";
            // 
            // groupBoxConnect
            // 
            this.groupBoxConnect.Controls.Add(this.buttonRandomClient);
            this.groupBoxConnect.Controls.Add(this.label6);
            this.groupBoxConnect.Controls.Add(this.textBoxPort);
            this.groupBoxConnect.Controls.Add(this.checkBoxUseName);
            this.groupBoxConnect.Controls.Add(this.textBoxName);
            this.groupBoxConnect.Controls.Add(this.groupBoxVersioning);
            this.groupBoxConnect.Controls.Add(this.comboBoxAuthToken);
            this.groupBoxConnect.Controls.Add(this.comboBoxClientID);
            this.groupBoxConnect.Controls.Add(this.label3);
            this.groupBoxConnect.Controls.Add(this.label2);
            this.groupBoxConnect.Controls.Add(this.label1);
            this.groupBoxConnect.Controls.Add(this.textBoxHost);
            this.groupBoxConnect.Location = new System.Drawing.Point(12, 12);
            this.groupBoxConnect.Name = "groupBoxConnect";
            this.groupBoxConnect.Size = new System.Drawing.Size(461, 233);
            this.groupBoxConnect.TabIndex = 2;
            this.groupBoxConnect.TabStop = false;
            this.groupBoxConnect.Text = "Connect";
            // 
            // buttonRandomClient
            // 
            this.buttonRandomClient.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRandomClient.Location = new System.Drawing.Point(388, 51);
            this.buttonRandomClient.Name = "buttonRandomClient";
            this.buttonRandomClient.Size = new System.Drawing.Size(66, 74);
            this.buttonRandomClient.TabIndex = 5;
            this.buttonRandomClient.Text = "Random";
            this.buttonRandomClient.UseVisualStyleBackColor = true;
            this.buttonRandomClient.Click += new System.EventHandler(this.buttonRandomClient_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(311, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(10, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = ":";
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(327, 19);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(55, 20);
            this.textBoxPort.TabIndex = 12;
            this.textBoxPort.Text = "1987";
            this.textBoxPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // checkBoxUseName
            // 
            this.checkBoxUseName.AutoSize = true;
            this.checkBoxUseName.Location = new System.Drawing.Point(9, 107);
            this.checkBoxUseName.Name = "checkBoxUseName";
            this.checkBoxUseName.Size = new System.Drawing.Size(77, 17);
            this.checkBoxUseName.TabIndex = 11;
            this.checkBoxUseName.Text = "Use name:";
            this.checkBoxUseName.UseVisualStyleBackColor = true;
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(123, 105);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(259, 20);
            this.textBoxName.TabIndex = 9;
            this.textBoxName.Text = "ThisIsMyName";
            this.textBoxName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // groupBoxVersioning
            // 
            this.groupBoxVersioning.Controls.Add(this.label7);
            this.groupBoxVersioning.Controls.Add(this.textBoxAppKey);
            this.groupBoxVersioning.Controls.Add(this.label5);
            this.groupBoxVersioning.Controls.Add(this.textBoxAppVersion);
            this.groupBoxVersioning.Controls.Add(this.label4);
            this.groupBoxVersioning.Controls.Add(this.textBoxAppName);
            this.groupBoxVersioning.Location = new System.Drawing.Point(6, 137);
            this.groupBoxVersioning.Name = "groupBoxVersioning";
            this.groupBoxVersioning.Size = new System.Drawing.Size(372, 90);
            this.groupBoxVersioning.TabIndex = 8;
            this.groupBoxVersioning.TabStop = false;
            this.groupBoxVersioning.Text = "Versioning";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 16);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(83, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Application Key:";
            // 
            // textBoxAppKey
            // 
            this.textBoxAppKey.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxAppKey.Location = new System.Drawing.Point(114, 13);
            this.textBoxAppKey.Name = "textBoxAppKey";
            this.textBoxAppKey.Size = new System.Drawing.Size(253, 20);
            this.textBoxAppKey.TabIndex = 13;
            this.textBoxAppKey.Text = "ffffffff-ffff-ffff-ffff-ffffffffffff\r\n";
            this.textBoxAppKey.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 66);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Application Version:";
            // 
            // textBoxAppVersion
            // 
            this.textBoxAppVersion.Location = new System.Drawing.Point(114, 63);
            this.textBoxAppVersion.Name = "textBoxAppVersion";
            this.textBoxAppVersion.Size = new System.Drawing.Size(182, 20);
            this.textBoxAppVersion.TabIndex = 11;
            this.textBoxAppVersion.Text = "1.0";
            this.textBoxAppVersion.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Application Name:";
            // 
            // textBoxAppName
            // 
            this.textBoxAppName.Location = new System.Drawing.Point(114, 39);
            this.textBoxAppName.Name = "textBoxAppName";
            this.textBoxAppName.Size = new System.Drawing.Size(182, 20);
            this.textBoxAppName.TabIndex = 9;
            this.textBoxAppName.Text = "Application";
            this.textBoxAppName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // comboBoxAuthToken
            // 
            this.comboBoxAuthToken.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxAuthToken.FormattingEnabled = true;
            this.comboBoxAuthToken.Location = new System.Drawing.Point(123, 77);
            this.comboBoxAuthToken.Name = "comboBoxAuthToken";
            this.comboBoxAuthToken.Size = new System.Drawing.Size(259, 21);
            this.comboBoxAuthToken.TabIndex = 7;
            this.comboBoxAuthToken.Text = "ffffffff-ffff-ffff-ffff-ffffffffffff";
            // 
            // comboBoxClientID
            // 
            this.comboBoxClientID.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxClientID.FormattingEnabled = true;
            this.comboBoxClientID.Location = new System.Drawing.Point(123, 51);
            this.comboBoxClientID.Name = "comboBoxClientID";
            this.comboBoxClientID.Size = new System.Drawing.Size(259, 21);
            this.comboBoxClientID.TabIndex = 6;
            this.comboBoxClientID.Text = "ffffffff-ffff-ffff-ffff-ffffffffffff";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Auth Token:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "ClientID:";
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(349, 251);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(124, 31);
            this.buttonConnect.TabIndex = 3;
            this.buttonConnect.Text = "&Connect...";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(12, 251);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(124, 31);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // FormConnect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(485, 294);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.groupBoxConnect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConnect";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Connect...";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FormConnect_Load);
            this.groupBoxConnect.ResumeLayout(false);
            this.groupBoxConnect.PerformLayout();
            this.groupBoxVersioning.ResumeLayout(false);
            this.groupBoxVersioning.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxHost;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBoxConnect;
        private System.Windows.Forms.GroupBox groupBoxVersioning;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxAppVersion;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxAppName;
        private System.Windows.Forms.ComboBox comboBoxAuthToken;
        private System.Windows.Forms.ComboBox comboBoxClientID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxUseName;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxAppKey;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonRandomClient;
    }
}