namespace StxClientTest
{
    partial class FormMain
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
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Requests", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Log", System.Windows.Forms.HorizontalAlignment.Left);
            this.textBoxRequest = new System.Windows.Forms.TextBox();
            this.buttonRequest = new System.Windows.Forms.Button();
            this.groupBoxRequest = new System.Windows.Forms.GroupBox();
            this.labelCount = new System.Windows.Forms.Label();
            this.numericSendCount = new System.Windows.Forms.NumericUpDown();
            this.buttonSend = new System.Windows.Forms.Button();
            this.listBoxData = new System.Windows.Forms.ListBox();
            this.labelClientID = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonAddField = new System.Windows.Forms.Button();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.buttonClear = new System.Windows.Forms.Button();
            this.groupBoxActions = new System.Windows.Forms.GroupBox();
            this.buttonReady = new System.Windows.Forms.Button();
            this.buttonLeaveRoom = new System.Windows.Forms.Button();
            this.buttonChat = new System.Windows.Forms.Button();
            this.checkBoxAnswerPing = new System.Windows.Forms.CheckBox();
            this.buttonDisconnect = new System.Windows.Forms.Button();
            this.listRequests = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.comboBoxFieldType = new System.Windows.Forms.ComboBox();
            this.textBoxFieldKey = new System.Windows.Forms.TextBox();
            this.buttonMatchmaking = new System.Windows.Forms.Button();
            this.buttonReconnect = new System.Windows.Forms.Button();
            this.buttonSetName = new System.Windows.Forms.Button();
            this.buttonSetAvatar = new System.Windows.Forms.Button();
            this.buttonShowAvatar = new System.Windows.Forms.Button();
            this.groupBoxRequest.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericSendCount)).BeginInit();
            this.groupBoxActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxRequest
            // 
            this.textBoxRequest.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxRequest.Location = new System.Drawing.Point(6, 19);
            this.textBoxRequest.Name = "textBoxRequest";
            this.textBoxRequest.Size = new System.Drawing.Size(247, 20);
            this.textBoxRequest.TabIndex = 2;
            this.textBoxRequest.Text = "Matchmaking";
            this.textBoxRequest.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxRequest_KeyDown);
            // 
            // buttonRequest
            // 
            this.buttonRequest.Location = new System.Drawing.Point(259, 14);
            this.buttonRequest.Name = "buttonRequest";
            this.buttonRequest.Size = new System.Drawing.Size(134, 27);
            this.buttonRequest.TabIndex = 3;
            this.buttonRequest.Text = "Send as &RequestPacket\r\n";
            this.buttonRequest.UseVisualStyleBackColor = true;
            this.buttonRequest.Click += new System.EventHandler(this.buttonRequest_Click);
            // 
            // groupBoxRequest
            // 
            this.groupBoxRequest.Controls.Add(this.textBoxRequest);
            this.groupBoxRequest.Controls.Add(this.buttonRequest);
            this.groupBoxRequest.Location = new System.Drawing.Point(11, 452);
            this.groupBoxRequest.Name = "groupBoxRequest";
            this.groupBoxRequest.Size = new System.Drawing.Size(399, 52);
            this.groupBoxRequest.TabIndex = 4;
            this.groupBoxRequest.TabStop = false;
            this.groupBoxRequest.Text = "Request";
            // 
            // labelCount
            // 
            this.labelCount.AutoSize = true;
            this.labelCount.Location = new System.Drawing.Point(14, 516);
            this.labelCount.Name = "labelCount";
            this.labelCount.Size = new System.Drawing.Size(65, 13);
            this.labelCount.TabIndex = 5;
            this.labelCount.Text = "Send count:";
            // 
            // numericSendCount
            // 
            this.numericSendCount.Location = new System.Drawing.Point(85, 514);
            this.numericSendCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericSendCount.Name = "numericSendCount";
            this.numericSendCount.Size = new System.Drawing.Size(46, 20);
            this.numericSendCount.TabIndex = 4;
            this.numericSendCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // buttonSend
            // 
            this.buttonSend.Location = new System.Drawing.Point(305, 509);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(99, 27);
            this.buttonSend.TabIndex = 5;
            this.buttonSend.Text = "Send as Packet";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // listBoxData
            // 
            this.listBoxData.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxData.ForeColor = System.Drawing.SystemColors.GrayText;
            this.listBoxData.FormattingEnabled = true;
            this.listBoxData.Location = new System.Drawing.Point(12, 325);
            this.listBoxData.Name = "listBoxData";
            this.listBoxData.Size = new System.Drawing.Size(398, 121);
            this.listBoxData.TabIndex = 6;
            // 
            // labelClientID
            // 
            this.labelClientID.AutoSize = true;
            this.labelClientID.Location = new System.Drawing.Point(467, 516);
            this.labelClientID.Name = "labelClientID";
            this.labelClientID.Size = new System.Drawing.Size(72, 13);
            this.labelClientID.TabIndex = 7;
            this.labelClientID.Text = "Your ClientID:";
            this.labelClientID.Click += new System.EventHandler(this.label2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 283);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Data to send:";
            // 
            // buttonAddField
            // 
            this.buttonAddField.Location = new System.Drawing.Point(354, 298);
            this.buttonAddField.Name = "buttonAddField";
            this.buttonAddField.Size = new System.Drawing.Size(56, 23);
            this.buttonAddField.TabIndex = 9;
            this.buttonAddField.Text = "Add field";
            this.buttonAddField.UseVisualStyleBackColor = true;
            this.buttonAddField.Click += new System.EventHandler(this.buttonAddField_Click);
            // 
            // buttonRemove
            // 
            this.buttonRemove.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.buttonRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRemove.Location = new System.Drawing.Point(387, 423);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(23, 23);
            this.buttonRemove.TabIndex = 10;
            this.buttonRemove.Text = "-";
            this.buttonRemove.UseVisualStyleBackColor = false;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // buttonClear
            // 
            this.buttonClear.BackColor = System.Drawing.Color.Red;
            this.buttonClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonClear.Location = new System.Drawing.Point(720, 278);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(97, 23);
            this.buttonClear.TabIndex = 11;
            this.buttonClear.Text = "Clear screen";
            this.buttonClear.UseVisualStyleBackColor = false;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // groupBoxActions
            // 
            this.groupBoxActions.Controls.Add(this.buttonReady);
            this.groupBoxActions.Controls.Add(this.buttonLeaveRoom);
            this.groupBoxActions.Location = new System.Drawing.Point(464, 299);
            this.groupBoxActions.Name = "groupBoxActions";
            this.groupBoxActions.Size = new System.Drawing.Size(124, 83);
            this.groupBoxActions.TabIndex = 15;
            this.groupBoxActions.TabStop = false;
            this.groupBoxActions.Text = "Current room";
            // 
            // buttonReady
            // 
            this.buttonReady.ForeColor = System.Drawing.Color.Red;
            this.buttonReady.Location = new System.Drawing.Point(6, 48);
            this.buttonReady.Name = "buttonReady";
            this.buttonReady.Size = new System.Drawing.Size(112, 23);
            this.buttonReady.TabIndex = 19;
            this.buttonReady.Text = "Not Ready.";
            this.buttonReady.UseVisualStyleBackColor = true;
            this.buttonReady.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonLeaveRoom
            // 
            this.buttonLeaveRoom.Location = new System.Drawing.Point(6, 19);
            this.buttonLeaveRoom.Name = "buttonLeaveRoom";
            this.buttonLeaveRoom.Size = new System.Drawing.Size(112, 23);
            this.buttonLeaveRoom.TabIndex = 16;
            this.buttonLeaveRoom.Text = "Leave Room";
            this.buttonLeaveRoom.UseVisualStyleBackColor = true;
            this.buttonLeaveRoom.Click += new System.EventHandler(this.buttonLeaveRoom_Click);
            // 
            // buttonChat
            // 
            this.buttonChat.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonChat.Location = new System.Drawing.Point(657, 347);
            this.buttonChat.Name = "buttonChat";
            this.buttonChat.Size = new System.Drawing.Size(112, 23);
            this.buttonChat.TabIndex = 20;
            this.buttonChat.Text = "Chat...";
            this.buttonChat.UseVisualStyleBackColor = true;
            this.buttonChat.Click += new System.EventHandler(this.buttonChat_Click);
            // 
            // checkBoxAnswerPing
            // 
            this.checkBoxAnswerPing.AutoSize = true;
            this.checkBoxAnswerPing.Checked = true;
            this.checkBoxAnswerPing.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAnswerPing.Location = new System.Drawing.Point(673, 398);
            this.checkBoxAnswerPing.Name = "checkBoxAnswerPing";
            this.checkBoxAnswerPing.Size = new System.Drawing.Size(84, 17);
            this.checkBoxAnswerPing.TabIndex = 18;
            this.checkBoxAnswerPing.Text = "Answer ping";
            this.checkBoxAnswerPing.UseVisualStyleBackColor = true;
            this.checkBoxAnswerPing.CheckedChanged += new System.EventHandler(this.checkBoxAnswerPing_CheckedChanged);
            // 
            // buttonDisconnect
            // 
            this.buttonDisconnect.Enabled = false;
            this.buttonDisconnect.Location = new System.Drawing.Point(657, 423);
            this.buttonDisconnect.Name = "buttonDisconnect";
            this.buttonDisconnect.Size = new System.Drawing.Size(112, 23);
            this.buttonDisconnect.TabIndex = 17;
            this.buttonDisconnect.Text = "Disconnect";
            this.buttonDisconnect.UseVisualStyleBackColor = true;
            this.buttonDisconnect.Click += new System.EventHandler(this.buttonDisconnect_Click);
            // 
            // listRequests
            // 
            this.listRequests.AllowColumnReorder = true;
            this.listRequests.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader6,
            this.columnHeader3,
            this.columnHeader5});
            this.listRequests.FullRowSelect = true;
            this.listRequests.GridLines = true;
            listViewGroup1.Header = "Requests";
            listViewGroup1.Name = "listViewGroup1";
            listViewGroup1.Tag = "Requests";
            listViewGroup2.Header = "Log";
            listViewGroup2.Name = "listViewGroup2";
            listViewGroup2.Tag = "Log";
            this.listRequests.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2});
            this.listRequests.Location = new System.Drawing.Point(12, 12);
            this.listRequests.Name = "listRequests";
            this.listRequests.Size = new System.Drawing.Size(805, 268);
            this.listRequests.TabIndex = 16;
            this.listRequests.UseCompatibleStateImageBehavior = false;
            this.listRequests.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Tag = "Requests";
            this.columnHeader4.Text = "ID";
            this.columnHeader4.Width = 30;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Tag = "Requests";
            this.columnHeader1.Text = "Request ID";
            this.columnHeader1.Width = 80;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Tag = "Requests";
            this.columnHeader2.Text = "Request Item";
            this.columnHeader2.Width = 115;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Tag = "Requests";
            this.columnHeader6.Text = "Time";
            this.columnHeader6.Width = 65;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Tag = "Requests";
            this.columnHeader3.Text = "Status";
            this.columnHeader3.Width = 90;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Tag = "Requests";
            this.columnHeader5.Text = "Response";
            this.columnHeader5.Width = 800;
            // 
            // comboBoxFieldType
            // 
            this.comboBoxFieldType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFieldType.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxFieldType.FormattingEnabled = true;
            this.comboBoxFieldType.Items.AddRange(new object[] {
            "string",
            "int",
            "MatchmakingQuery",
            "ClientRoomStatus",
            "RoomTemplate"});
            this.comboBoxFieldType.Location = new System.Drawing.Point(192, 299);
            this.comboBoxFieldType.Name = "comboBoxFieldType";
            this.comboBoxFieldType.Size = new System.Drawing.Size(156, 21);
            this.comboBoxFieldType.TabIndex = 17;
            // 
            // textBoxFieldKey
            // 
            this.textBoxFieldKey.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxFieldKey.Location = new System.Drawing.Point(12, 299);
            this.textBoxFieldKey.Name = "textBoxFieldKey";
            this.textBoxFieldKey.Size = new System.Drawing.Size(174, 20);
            this.textBoxFieldKey.TabIndex = 4;
            this.textBoxFieldKey.Text = "Query";
            this.textBoxFieldKey.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxFieldKey_KeyDown);
            // 
            // buttonMatchmaking
            // 
            this.buttonMatchmaking.Location = new System.Drawing.Point(657, 318);
            this.buttonMatchmaking.Name = "buttonMatchmaking";
            this.buttonMatchmaking.Size = new System.Drawing.Size(112, 23);
            this.buttonMatchmaking.TabIndex = 21;
            this.buttonMatchmaking.Text = "Matchmaking...";
            this.buttonMatchmaking.UseVisualStyleBackColor = true;
            this.buttonMatchmaking.Click += new System.EventHandler(this.buttonMatchmaking_Click);
            // 
            // buttonReconnect
            // 
            this.buttonReconnect.Location = new System.Drawing.Point(657, 452);
            this.buttonReconnect.Name = "buttonReconnect";
            this.buttonReconnect.Size = new System.Drawing.Size(112, 23);
            this.buttonReconnect.TabIndex = 22;
            this.buttonReconnect.Text = "Reconnect...";
            this.buttonReconnect.UseVisualStyleBackColor = true;
            this.buttonReconnect.Click += new System.EventHandler(this.buttonReconnect_Click);
            // 
            // buttonSetName
            // 
            this.buttonSetName.Location = new System.Drawing.Point(470, 388);
            this.buttonSetName.Name = "buttonSetName";
            this.buttonSetName.Size = new System.Drawing.Size(112, 23);
            this.buttonSetName.TabIndex = 20;
            this.buttonSetName.Text = "Set Name";
            this.buttonSetName.UseVisualStyleBackColor = true;
            this.buttonSetName.Click += new System.EventHandler(this.buttonSetName_Click);
            // 
            // buttonSetAvatar
            // 
            this.buttonSetAvatar.Location = new System.Drawing.Point(470, 417);
            this.buttonSetAvatar.Name = "buttonSetAvatar";
            this.buttonSetAvatar.Size = new System.Drawing.Size(112, 23);
            this.buttonSetAvatar.TabIndex = 23;
            this.buttonSetAvatar.Text = "Set Avatar";
            this.buttonSetAvatar.UseVisualStyleBackColor = true;
            this.buttonSetAvatar.Click += new System.EventHandler(this.buttonSetAvatar_Click);
            // 
            // buttonShowAvatar
            // 
            this.buttonShowAvatar.Location = new System.Drawing.Point(470, 446);
            this.buttonShowAvatar.Name = "buttonShowAvatar";
            this.buttonShowAvatar.Size = new System.Drawing.Size(112, 23);
            this.buttonShowAvatar.TabIndex = 24;
            this.buttonShowAvatar.Text = "Show Avatar";
            this.buttonShowAvatar.UseVisualStyleBackColor = true;
            this.buttonShowAvatar.Click += new System.EventHandler(this.buttonShowAvatar_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(829, 547);
            this.Controls.Add(this.buttonShowAvatar);
            this.Controls.Add(this.buttonSetAvatar);
            this.Controls.Add(this.buttonSetName);
            this.Controls.Add(this.buttonReconnect);
            this.Controls.Add(this.buttonChat);
            this.Controls.Add(this.checkBoxAnswerPing);
            this.Controls.Add(this.buttonMatchmaking);
            this.Controls.Add(this.buttonDisconnect);
            this.Controls.Add(this.textBoxFieldKey);
            this.Controls.Add(this.comboBoxFieldType);
            this.Controls.Add(this.groupBoxRequest);
            this.Controls.Add(this.labelCount);
            this.Controls.Add(this.buttonSend);
            this.Controls.Add(this.numericSendCount);
            this.Controls.Add(this.listRequests);
            this.Controls.Add(this.groupBoxActions);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.buttonRemove);
            this.Controls.Add(this.buttonAddField);
            this.Controls.Add(this.listBoxData);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.labelClientID);
            this.Name = "FormMain";
            this.Text = "StxClient Control Panel";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.groupBoxRequest.ResumeLayout(false);
            this.groupBoxRequest.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericSendCount)).EndInit();
            this.groupBoxActions.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox textBoxRequest;
        private System.Windows.Forms.Button buttonRequest;
        private System.Windows.Forms.GroupBox groupBoxRequest;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.Label labelClientID;
        private System.Windows.Forms.ListBox listBoxData;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonAddField;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.GroupBox groupBoxActions;
        private System.Windows.Forms.Button buttonLeaveRoom;
        private System.Windows.Forms.ListView listRequests;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Button buttonDisconnect;
        private System.Windows.Forms.Label labelCount;
        private System.Windows.Forms.NumericUpDown numericSendCount;
        private System.Windows.Forms.CheckBox checkBoxAnswerPing;
        private System.Windows.Forms.ComboBox comboBoxFieldType;
        private System.Windows.Forms.TextBox textBoxFieldKey;
        private System.Windows.Forms.Button buttonReady;
        private System.Windows.Forms.Button buttonChat;
        private System.Windows.Forms.Button buttonMatchmaking;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.Button buttonReconnect;
        private System.Windows.Forms.Button buttonSetName;
        private System.Windows.Forms.Button buttonSetAvatar;
        private System.Windows.Forms.Button buttonShowAvatar;
    }
}

