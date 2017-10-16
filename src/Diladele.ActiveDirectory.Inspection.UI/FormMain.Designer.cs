namespace Diladele.ActiveDirectory.Inspection.UI
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
            this.components = new System.ComponentModel.Container();
            this._lstStorage = new System.Windows.Forms.ListView();
            this._timerRefresh = new System.Windows.Forms.Timer(this.components);
            this._statusStrip = new System.Windows.Forms.StatusStrip();
            this._statusLabelCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._menuRefreshList = new System.Windows.Forms.ToolStripMenuItem();
            this.IP = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._statusStrip.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _lstStorage
            // 
            this._lstStorage.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.IP,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this._lstStorage.Dock = System.Windows.Forms.DockStyle.Fill;
            this._lstStorage.Location = new System.Drawing.Point(0, 0);
            this._lstStorage.Name = "_lstStorage";
            this._lstStorage.Size = new System.Drawing.Size(2257, 1225);
            this._lstStorage.TabIndex = 0;
            this._lstStorage.UseCompatibleStateImageBehavior = false;
            this._lstStorage.View = System.Windows.Forms.View.Details;
            // 
            // _timerRefresh
            // 
            this._timerRefresh.Interval = 1000;
            this._timerRefresh.Tick += new System.EventHandler(this._timerRefresh_Tick);
            // 
            // _statusStrip
            // 
            this._statusStrip.ImageScalingSize = new System.Drawing.Size(36, 36);
            this._statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._statusLabelCount});
            this._statusStrip.Location = new System.Drawing.Point(0, 1270);
            this._statusStrip.Name = "_statusStrip";
            this._statusStrip.Size = new System.Drawing.Size(2257, 42);
            this._statusStrip.TabIndex = 1;
            this._statusStrip.Text = "statusStrip1";
            // 
            // _statusLabelCount
            // 
            this._statusLabelCount.Name = "_statusLabelCount";
            this._statusLabelCount.Size = new System.Drawing.Size(265, 37);
            this._statusLabelCount.Text = "Address List Count: 0";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(36, 36);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(2257, 45);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._menuRefreshList});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(90, 41);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // _menuRefreshList
            // 
            this._menuRefreshList.Name = "_menuRefreshList";
            this._menuRefreshList.Size = new System.Drawing.Size(297, 42);
            this._menuRefreshList.Text = "Refresh List";
            this._menuRefreshList.Click += new System.EventHandler(this._menuRefreshList_Click);
            // 
            // IP
            // 
            this.IP.Text = "IP";
            this.IP.Width = 160;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this._lstStorage);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 45);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(2257, 1225);
            this.panel1.TabIndex = 3;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "DNS";
            this.columnHeader1.Width = 260;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "User Count";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "User Name (0)";
            this.columnHeader3.Width = 160;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "User Name (1)";
            this.columnHeader4.Width = 160;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2257, 1312);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this._statusStrip);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.Text = "Storage View Test";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this._statusStrip.ResumeLayout(false);
            this._statusStrip.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView _lstStorage;
        private System.Windows.Forms.Timer _timerRefresh;
        private System.Windows.Forms.StatusStrip _statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel _statusLabelCount;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _menuRefreshList;
        private System.Windows.Forms.ColumnHeader IP;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
    }
}

