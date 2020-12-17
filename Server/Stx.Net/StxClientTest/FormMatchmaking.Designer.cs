namespace StxClientTest
{
    partial class FormMatchmaking
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
            this.buttonChangeQuery = new System.Windows.Forms.Button();
            this.labelQuery = new System.Windows.Forms.Label();
            this.listRooms = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonJoin = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.labelResults = new System.Windows.Forms.Label();
            this.buttonNextPage = new System.Windows.Forms.Button();
            this.buttonPreviousPage = new System.Windows.Forms.Button();
            this.labelPage = new System.Windows.Forms.Label();
            this.buttonJoinRandom = new System.Windows.Forms.Button();
            this.buttonJoinCode = new System.Windows.Forms.Button();
            this.buttonJoinRandomOrNew = new System.Windows.Forms.Button();
            this.buttonJoinNewRoom = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonChangeQuery
            // 
            this.buttonChangeQuery.Location = new System.Drawing.Point(12, 12);
            this.buttonChangeQuery.Name = "buttonChangeQuery";
            this.buttonChangeQuery.Size = new System.Drawing.Size(115, 28);
            this.buttonChangeQuery.TabIndex = 0;
            this.buttonChangeQuery.Text = "Change query...";
            this.buttonChangeQuery.UseVisualStyleBackColor = true;
            this.buttonChangeQuery.Click += new System.EventHandler(this.button1_Click);
            // 
            // labelQuery
            // 
            this.labelQuery.AutoSize = true;
            this.labelQuery.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelQuery.Location = new System.Drawing.Point(12, 49);
            this.labelQuery.Name = "labelQuery";
            this.labelQuery.Size = new System.Drawing.Size(91, 13);
            this.labelQuery.TabIndex = 1;
            this.labelQuery.Text = "Current query:";
            // 
            // listRooms
            // 
            this.listRooms.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader5,
            this.columnHeader6});
            this.listRooms.Location = new System.Drawing.Point(12, 73);
            this.listRooms.Name = "listRooms";
            this.listRooms.Size = new System.Drawing.Size(432, 312);
            this.listRooms.TabIndex = 17;
            this.listRooms.UseCompatibleStateImageBehavior = false;
            this.listRooms.View = System.Windows.Forms.View.Details;
            this.listRooms.SelectedIndexChanged += new System.EventHandler(this.listRooms_SelectedIndexChanged);
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "ID";
            this.columnHeader4.Width = 70;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 140;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Players";
            this.columnHeader2.Width = 50;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Locked";
            this.columnHeader3.Width = 50;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Code";
            this.columnHeader5.Width = 50;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "State";
            // 
            // buttonJoin
            // 
            this.buttonJoin.Enabled = false;
            this.buttonJoin.Location = new System.Drawing.Point(450, 293);
            this.buttonJoin.Name = "buttonJoin";
            this.buttonJoin.Size = new System.Drawing.Size(79, 43);
            this.buttonJoin.TabIndex = 18;
            this.buttonJoin.Text = "Join selected";
            this.buttonJoin.UseVisualStyleBackColor = true;
            this.buttonJoin.Click += new System.EventHandler(this.buttonJoin_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(450, 342);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(79, 43);
            this.buttonCancel.TabIndex = 19;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Location = new System.Drawing.Point(133, 12);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(104, 28);
            this.buttonRefresh.TabIndex = 20;
            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // labelResults
            // 
            this.labelResults.AutoSize = true;
            this.labelResults.Location = new System.Drawing.Point(12, 388);
            this.labelResults.Name = "labelResults";
            this.labelResults.Size = new System.Drawing.Size(134, 13);
            this.labelResults.TabIndex = 21;
            this.labelResults.Text = "Results are displayed here.\r\n";
            // 
            // buttonNextPage
            // 
            this.buttonNextPage.Enabled = false;
            this.buttonNextPage.Location = new System.Drawing.Point(412, 12);
            this.buttonNextPage.Name = "buttonNextPage";
            this.buttonNextPage.Size = new System.Drawing.Size(34, 28);
            this.buttonNextPage.TabIndex = 22;
            this.buttonNextPage.Text = ">";
            this.buttonNextPage.UseVisualStyleBackColor = true;
            this.buttonNextPage.Click += new System.EventHandler(this.buttonNextPage_Click);
            // 
            // buttonPreviousPage
            // 
            this.buttonPreviousPage.Enabled = false;
            this.buttonPreviousPage.Location = new System.Drawing.Point(372, 12);
            this.buttonPreviousPage.Name = "buttonPreviousPage";
            this.buttonPreviousPage.Size = new System.Drawing.Size(34, 28);
            this.buttonPreviousPage.TabIndex = 23;
            this.buttonPreviousPage.Text = "<";
            this.buttonPreviousPage.UseVisualStyleBackColor = true;
            this.buttonPreviousPage.Click += new System.EventHandler(this.buttonPreviousPage_Click);
            // 
            // labelPage
            // 
            this.labelPage.AutoSize = true;
            this.labelPage.Location = new System.Drawing.Point(389, 49);
            this.labelPage.Name = "labelPage";
            this.labelPage.Size = new System.Drawing.Size(0, 13);
            this.labelPage.TabIndex = 24;
            this.labelPage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonJoinRandom
            // 
            this.buttonJoinRandom.Location = new System.Drawing.Point(450, 73);
            this.buttonJoinRandom.Name = "buttonJoinRandom";
            this.buttonJoinRandom.Size = new System.Drawing.Size(79, 43);
            this.buttonJoinRandom.TabIndex = 25;
            this.buttonJoinRandom.Text = "Join random";
            this.buttonJoinRandom.UseVisualStyleBackColor = true;
            this.buttonJoinRandom.Click += new System.EventHandler(this.buttonJoinRandom_Click);
            // 
            // buttonJoinCode
            // 
            this.buttonJoinCode.Location = new System.Drawing.Point(450, 171);
            this.buttonJoinCode.Name = "buttonJoinCode";
            this.buttonJoinCode.Size = new System.Drawing.Size(79, 43);
            this.buttonJoinCode.TabIndex = 26;
            this.buttonJoinCode.Text = "Join with code";
            this.buttonJoinCode.UseVisualStyleBackColor = true;
            this.buttonJoinCode.Click += new System.EventHandler(this.buttonJoinCode_Click);
            // 
            // buttonJoinRandomOrNew
            // 
            this.buttonJoinRandomOrNew.Location = new System.Drawing.Point(450, 122);
            this.buttonJoinRandomOrNew.Name = "buttonJoinRandomOrNew";
            this.buttonJoinRandomOrNew.Size = new System.Drawing.Size(79, 43);
            this.buttonJoinRandomOrNew.TabIndex = 27;
            this.buttonJoinRandomOrNew.Text = "Join random or new";
            this.buttonJoinRandomOrNew.UseVisualStyleBackColor = true;
            this.buttonJoinRandomOrNew.Click += new System.EventHandler(this.buttonJoinRandomOrNew_Click);
            // 
            // buttonJoinNewRoom
            // 
            this.buttonJoinNewRoom.Location = new System.Drawing.Point(450, 232);
            this.buttonJoinNewRoom.Name = "buttonJoinNewRoom";
            this.buttonJoinNewRoom.Size = new System.Drawing.Size(79, 43);
            this.buttonJoinNewRoom.TabIndex = 28;
            this.buttonJoinNewRoom.Text = "Create new room and join\r\n";
            this.buttonJoinNewRoom.UseVisualStyleBackColor = true;
            this.buttonJoinNewRoom.Click += new System.EventHandler(this.buttonJoinNewRoom_Click);
            // 
            // FormMatchmaking
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(537, 432);
            this.Controls.Add(this.buttonJoinNewRoom);
            this.Controls.Add(this.buttonJoinRandomOrNew);
            this.Controls.Add(this.buttonJoinCode);
            this.Controls.Add(this.buttonJoinRandom);
            this.Controls.Add(this.labelPage);
            this.Controls.Add(this.buttonPreviousPage);
            this.Controls.Add(this.buttonNextPage);
            this.Controls.Add(this.labelResults);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonJoin);
            this.Controls.Add(this.listRooms);
            this.Controls.Add(this.labelQuery);
            this.Controls.Add(this.buttonChangeQuery);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormMatchmaking";
            this.Text = "Matchmaking";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonChangeQuery;
        private System.Windows.Forms.Label labelQuery;
        private System.Windows.Forms.ListView listRooms;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.Button buttonJoin;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.Label labelResults;
        private System.Windows.Forms.Button buttonNextPage;
        private System.Windows.Forms.Button buttonPreviousPage;
        private System.Windows.Forms.Label labelPage;
        private System.Windows.Forms.Button buttonJoinRandom;
        private System.Windows.Forms.Button buttonJoinCode;
        private System.Windows.Forms.Button buttonJoinRandomOrNew;
        private System.Windows.Forms.Button buttonJoinNewRoom;
    }
}