namespace StxClientTest
{
    partial class FormMatchmakingQuery
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
            this.checkBoxOnlyNotFull = new System.Windows.Forms.CheckBox();
            this.checkBoxOnlyUnlocked = new System.Windows.Forms.CheckBox();
            this.comboBoxState = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxID = new System.Windows.Forms.TextBox();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.textBoxRoomCode = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxTag = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOkay = new System.Windows.Forms.Button();
            this.numericPage = new System.Windows.Forms.NumericUpDown();
            this.numericResults = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericPage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericResults)).BeginInit();
            this.SuspendLayout();
            // 
            // checkBoxOnlyNotFull
            // 
            this.checkBoxOnlyNotFull.AutoSize = true;
            this.checkBoxOnlyNotFull.Location = new System.Drawing.Point(15, 12);
            this.checkBoxOnlyNotFull.Name = "checkBoxOnlyNotFull";
            this.checkBoxOnlyNotFull.Size = new System.Drawing.Size(81, 17);
            this.checkBoxOnlyNotFull.TabIndex = 0;
            this.checkBoxOnlyNotFull.Text = "Only not full";
            this.checkBoxOnlyNotFull.UseVisualStyleBackColor = true;
            // 
            // checkBoxOnlyUnlocked
            // 
            this.checkBoxOnlyUnlocked.AutoSize = true;
            this.checkBoxOnlyUnlocked.Location = new System.Drawing.Point(102, 12);
            this.checkBoxOnlyUnlocked.Name = "checkBoxOnlyUnlocked";
            this.checkBoxOnlyUnlocked.Size = new System.Drawing.Size(94, 17);
            this.checkBoxOnlyUnlocked.TabIndex = 1;
            this.checkBoxOnlyUnlocked.Text = "Only unlocked";
            this.checkBoxOnlyUnlocked.UseVisualStyleBackColor = true;
            // 
            // comboBoxState
            // 
            this.comboBoxState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxState.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxState.FormattingEnabled = true;
            this.comboBoxState.Items.AddRange(new object[] {
            "Any",
            "InLobby",
            "InGame"});
            this.comboBoxState.Location = new System.Drawing.Point(84, 35);
            this.comboBoxState.Name = "comboBoxState";
            this.comboBoxState.Size = new System.Drawing.Size(121, 21);
            this.comboBoxState.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Game state:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Name:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "ID:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 125);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Room code:";
            // 
            // textBoxID
            // 
            this.textBoxID.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxID.Location = new System.Drawing.Point(84, 88);
            this.textBoxID.Name = "textBoxID";
            this.textBoxID.Size = new System.Drawing.Size(216, 20);
            this.textBoxID.TabIndex = 8;
            // 
            // textBoxName
            // 
            this.textBoxName.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxName.Location = new System.Drawing.Point(84, 62);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(216, 20);
            this.textBoxName.TabIndex = 9;
            // 
            // textBoxRoomCode
            // 
            this.textBoxRoomCode.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxRoomCode.Location = new System.Drawing.Point(84, 114);
            this.textBoxRoomCode.MaxLength = 4;
            this.textBoxRoomCode.Name = "textBoxRoomCode";
            this.textBoxRoomCode.Size = new System.Drawing.Size(65, 32);
            this.textBoxRoomCode.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 218);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(295, 26);
            this.label5.TabIndex = 11;
            this.label5.Text = "This empty query is the default query and will match anything.\r\nEmpty fields will" +
    " not get compared!\r\n";
            // 
            // textBoxTag
            // 
            this.textBoxTag.Location = new System.Drawing.Point(84, 152);
            this.textBoxTag.Name = "textBoxTag";
            this.textBoxTag.Size = new System.Drawing.Size(121, 20);
            this.textBoxTag.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 155);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Tag:";
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(12, 256);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 32);
            this.buttonCancel.TabIndex = 14;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOkay
            // 
            this.buttonOkay.Location = new System.Drawing.Point(225, 256);
            this.buttonOkay.Name = "buttonOkay";
            this.buttonOkay.Size = new System.Drawing.Size(75, 32);
            this.buttonOkay.TabIndex = 15;
            this.buttonOkay.Text = "Okay";
            this.buttonOkay.UseVisualStyleBackColor = true;
            this.buttonOkay.Click += new System.EventHandler(this.buttonOkay_Click);
            // 
            // numericPage
            // 
            this.numericPage.Location = new System.Drawing.Point(212, 178);
            this.numericPage.Name = "numericPage";
            this.numericPage.Size = new System.Drawing.Size(49, 20);
            this.numericPage.TabIndex = 16;
            // 
            // numericResults
            // 
            this.numericResults.Location = new System.Drawing.Point(84, 178);
            this.numericResults.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numericResults.Name = "numericResults";
            this.numericResults.Size = new System.Drawing.Size(49, 20);
            this.numericResults.TabIndex = 17;
            this.numericResults.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 180);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Max results:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(171, 180);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "Page:";
            // 
            // FormMatchmakingQuery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(312, 300);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.numericResults);
            this.Controls.Add(this.numericPage);
            this.Controls.Add(this.buttonOkay);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBoxTag);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxRoomCode);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.textBoxID);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxState);
            this.Controls.Add(this.checkBoxOnlyUnlocked);
            this.Controls.Add(this.checkBoxOnlyNotFull);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormMatchmakingQuery";
            this.Text = "Create Matchmaking Query";
            this.Load += new System.EventHandler(this.FormMatchmakingQuery_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericPage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericResults)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxOnlyNotFull;
        private System.Windows.Forms.CheckBox checkBoxOnlyUnlocked;
        private System.Windows.Forms.ComboBox comboBoxState;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxID;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.TextBox textBoxRoomCode;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxTag;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOkay;
        private System.Windows.Forms.NumericUpDown numericPage;
        private System.Windows.Forms.NumericUpDown numericResults;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
    }
}