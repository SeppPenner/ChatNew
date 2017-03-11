namespace Chat
{
    partial class ChatClient
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
            this.button = new System.Windows.Forms.Button();
            this.rtb = new System.Windows.Forms.RichTextBox();
            this.pic = new System.Windows.Forms.PictureBox();
            this.contextmenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.clearPicturToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shp1 = new ShapeControl.ShapeControl();
            this.contextmenu_shp1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.loadMediaFileToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.playMediaFileToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.textbox = new System.Windows.Forms.TextBox();
            this.checkbox = new System.Windows.Forms.CheckBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeRoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.whichRoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.p_smItemS1 = new System.Windows.Forms.ToolStripMenuItem();
            this.p_smItemS2 = new System.Windows.Forms.ToolStripMenuItem();
            this.p_smItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.p_smItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.p_smItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearPictureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mediaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadMediaFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playMediaFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pic)).BeginInit();
            this.contextmenu.SuspendLayout();
            this.contextmenu_shp1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button
            // 
            this.button.Location = new System.Drawing.Point(349, 321);
            this.button.Name = "button";
            this.button.Size = new System.Drawing.Size(114, 20);
            this.button.TabIndex = 0;
            this.button.Text = "Clear Messages";
            this.button.UseVisualStyleBackColor = true;
            this.button.Click += new System.EventHandler(this.button_Click);
            // 
            // rtb
            // 
            this.rtb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtb.Location = new System.Drawing.Point(3, 27);
            this.rtb.Name = "rtb";
            this.rtb.Size = new System.Drawing.Size(460, 288);
            this.rtb.TabIndex = 1;
            this.rtb.Text = "";
            this.rtb.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtb_LinkClicked);
            this.rtb.TextChanged += new System.EventHandler(this.rtb_TextChanged);
            // 
            // pic
            // 
            this.pic.BackColor = System.Drawing.Color.White;
            this.pic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pic.ContextMenuStrip = this.contextmenu;
            this.pic.Location = new System.Drawing.Point(3, 363);
            this.pic.Name = "pic";
            this.pic.Size = new System.Drawing.Size(460, 129);
            this.pic.TabIndex = 2;
            this.pic.TabStop = false;
            this.pic.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pic_MouseMove);
            this.pic.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pic_MouseDown);
            // 
            // contextmenu
            // 
            this.contextmenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearPicturToolStripMenuItem,
            this.eBoxToolStripMenuItem});
            this.contextmenu.Name = "contextmenu";
            this.contextmenu.Size = new System.Drawing.Size(164, 48);
            // 
            // clearPicturToolStripMenuItem
            // 
            this.clearPicturToolStripMenuItem.Name = "clearPicturToolStripMenuItem";
            this.clearPicturToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.clearPicturToolStripMenuItem.Text = "Clear Picture Box";
            this.clearPicturToolStripMenuItem.Click += new System.EventHandler(this.ClearPicture);
            // 
            // eBoxToolStripMenuItem
            // 
            this.eBoxToolStripMenuItem.Name = "eBoxToolStripMenuItem";
            this.eBoxToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.eBoxToolStripMenuItem.Text = "Load Picture(s)";
            this.eBoxToolStripMenuItem.Click += new System.EventHandler(this.LoadPictureFile);
            // 
            // shp1
            // 
            this.shp1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(145)))), ((int)(((byte)(255)))), ((int)(((byte)(160)))));
            this.shp1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(255)))), ((int)(((byte)(118)))));
            this.shp1.BorderStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            this.shp1.BorderWidth = 3;
            this.shp1.CenterColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.shp1.ContextMenuStrip = this.contextmenu_shp1;
            this.shp1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.shp1.Location = new System.Drawing.Point(3, 509);
            this.shp1.Name = "shp1";
            this.shp1.Shape = ShapeControl.ShapeType.RoundedRectangle;
            this.shp1.Size = new System.Drawing.Size(178, 48);
            this.shp1.SurroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.shp1.TabIndex = 3;
            this.shp1.Text = "Empty";
            this.shp1.UseGradient = false;
            // 
            // contextmenu_shp1
            // 
            this.contextmenu_shp1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadMediaFileToolStripMenuItem1,
            this.playMediaFileToolStripMenuItem1});
            this.contextmenu_shp1.Name = "contextmenu_shp1";
            this.contextmenu_shp1.Size = new System.Drawing.Size(158, 48);
            // 
            // loadMediaFileToolStripMenuItem1
            // 
            this.loadMediaFileToolStripMenuItem1.Name = "loadMediaFileToolStripMenuItem1";
            this.loadMediaFileToolStripMenuItem1.Size = new System.Drawing.Size(157, 22);
            this.loadMediaFileToolStripMenuItem1.Text = "Load Media File";
            this.loadMediaFileToolStripMenuItem1.Click += new System.EventHandler(this.LoadMedia);
            // 
            // playMediaFileToolStripMenuItem1
            // 
            this.playMediaFileToolStripMenuItem1.Name = "playMediaFileToolStripMenuItem1";
            this.playMediaFileToolStripMenuItem1.Size = new System.Drawing.Size(157, 22);
            this.playMediaFileToolStripMenuItem1.Text = "Play Media File";
            this.playMediaFileToolStripMenuItem1.Click += new System.EventHandler(this.PlayMediaFile);
            // 
            // textbox
            // 
            this.textbox.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textbox.Location = new System.Drawing.Point(3, 573);
            this.textbox.MaxLength = 200;
            this.textbox.Multiline = true;
            this.textbox.Name = "textbox";
            this.textbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textbox.Size = new System.Drawing.Size(460, 74);
            this.textbox.TabIndex = 4;
            this.textbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textbox_KeyDown);
            this.textbox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textbox_KeyPressed);
            // 
            // checkbox
            // 
            this.checkbox.AutoSize = true;
            this.checkbox.Checked = true;
            this.checkbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkbox.Location = new System.Drawing.Point(3, 321);
            this.checkbox.Name = "checkbox";
            this.checkbox.Size = new System.Drawing.Size(194, 20);
            this.checkbox.TabIndex = 5;
            this.checkbox.Text = "Auto Retrieve Picture/Media";
            this.checkbox.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.pToolStripMenuItem,
            this.mediaToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(481, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem,
            this.listAllToolStripMenuItem,
            this.changeRoomToolStripMenuItem,
            this.whichRoomToolStripMenuItem,
            this.p_smItemS1,
            this.p_smItemS2,
            this.p_smItem5,
            this.p_smItem6,
            this.p_smItem7,
            this.quitToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(76, 20);
            this.toolStripMenuItem1.Text = "Command";
            this.toolStripMenuItem1.DropDownOpening += new System.EventHandler(this.CommandPopUp);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.helpToolStripMenuItem.Text = ":help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.Commands);
            // 
            // listAllToolStripMenuItem
            // 
            this.listAllToolStripMenuItem.Name = "listAllToolStripMenuItem";
            this.listAllToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.listAllToolStripMenuItem.Text = ":list all";
            this.listAllToolStripMenuItem.Click += new System.EventHandler(this.Commands);
            // 
            // changeRoomToolStripMenuItem
            // 
            this.changeRoomToolStripMenuItem.Name = "changeRoomToolStripMenuItem";
            this.changeRoomToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.changeRoomToolStripMenuItem.Text = ":change room";
            this.changeRoomToolStripMenuItem.Click += new System.EventHandler(this.Commands);
            // 
            // whichRoomToolStripMenuItem
            // 
            this.whichRoomToolStripMenuItem.Name = "whichRoomToolStripMenuItem";
            this.whichRoomToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.whichRoomToolStripMenuItem.Text = ":which room";
            this.whichRoomToolStripMenuItem.Click += new System.EventHandler(this.Commands);
            // 
            // p_smItemS1
            // 
            this.p_smItemS1.Name = "p_smItemS1";
            this.p_smItemS1.Size = new System.Drawing.Size(148, 22);
            this.p_smItemS1.Text = ":send media:";
            // 
            // p_smItemS2
            // 
            this.p_smItemS2.Name = "p_smItemS2";
            this.p_smItemS2.Size = new System.Drawing.Size(148, 22);
            this.p_smItemS2.Text = ":get media:";
            // 
            // p_smItem5
            // 
            this.p_smItem5.Name = "p_smItem5";
            this.p_smItem5.Size = new System.Drawing.Size(148, 22);
            this.p_smItem5.Text = ":send pic:";
            // 
            // p_smItem6
            // 
            this.p_smItem6.Name = "p_smItem6";
            this.p_smItem6.Size = new System.Drawing.Size(148, 22);
            this.p_smItem6.Text = ":get pic:";
            // 
            // p_smItem7
            // 
            this.p_smItem7.Name = "p_smItem7";
            this.p_smItem7.Size = new System.Drawing.Size(148, 22);
            this.p_smItem7.Text = ":private:";
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.quitToolStripMenuItem.Text = ":quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.Commands);
            // 
            // pToolStripMenuItem
            // 
            this.pToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearPictureToolStripMenuItem,
            this.loadToolStripMenuItem});
            this.pToolStripMenuItem.Name = "pToolStripMenuItem";
            this.pToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.pToolStripMenuItem.Text = "Picture";
            // 
            // clearPictureToolStripMenuItem
            // 
            this.clearPictureToolStripMenuItem.Name = "clearPictureToolStripMenuItem";
            this.clearPictureToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.clearPictureToolStripMenuItem.Text = "Clear Picture";
            this.clearPictureToolStripMenuItem.Click += new System.EventHandler(this.ClearPicture);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.loadToolStripMenuItem.Text = "Load Picture(s)";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.LoadPictureFile);
            // 
            // mediaToolStripMenuItem
            // 
            this.mediaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadMediaFileToolStripMenuItem,
            this.playMediaFileToolStripMenuItem});
            this.mediaToolStripMenuItem.Name = "mediaToolStripMenuItem";
            this.mediaToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.mediaToolStripMenuItem.Text = "Media";
            // 
            // loadMediaFileToolStripMenuItem
            // 
            this.loadMediaFileToolStripMenuItem.Name = "loadMediaFileToolStripMenuItem";
            this.loadMediaFileToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.loadMediaFileToolStripMenuItem.Text = "Load Media File";
            this.loadMediaFileToolStripMenuItem.Click += new System.EventHandler(this.LoadMedia);
            // 
            // playMediaFileToolStripMenuItem
            // 
            this.playMediaFileToolStripMenuItem.Name = "playMediaFileToolStripMenuItem";
            this.playMediaFileToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.playMediaFileToolStripMenuItem.Text = "Play Media File";
            this.playMediaFileToolStripMenuItem.Click += new System.EventHandler(this.PlayMediaFile);
            // 
            // ChatClient
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(481, 659);
            this.Controls.Add(this.shp1);
            this.Controls.Add(this.pic);
            this.Controls.Add(this.textbox);
            this.Controls.Add(this.rtb);
            this.Controls.Add(this.checkbox);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.button);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ChatClient";
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.SizeChanged += new System.EventHandler(this.textbox_SizeChanged);
            this.Activated += new System.EventHandler(this.form_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.form_Closing);
            ((System.ComponentModel.ISupportInitialize)(this.pic)).EndInit();
            this.contextmenu.ResumeLayout(false);
            this.contextmenu_shp1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button;
        private System.Windows.Forms.RichTextBox rtb;
        private System.Windows.Forms.PictureBox pic;
        private ShapeControl.ShapeControl shp1;
        private System.Windows.Forms.TextBox textbox;
        private System.Windows.Forms.CheckBox checkbox;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem pToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem whichRoomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem p_smItemS1;
        private System.Windows.Forms.ToolStripMenuItem p_smItemS2;
        private System.Windows.Forms.ToolStripMenuItem p_smItem5;
        private System.Windows.Forms.ToolStripMenuItem p_smItem6;
        private System.Windows.Forms.ToolStripMenuItem p_smItem7;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearPictureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mediaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadMediaFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem playMediaFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeRoomToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextmenu;
        private System.Windows.Forms.ContextMenuStrip contextmenu_shp1;
        private System.Windows.Forms.ToolStripMenuItem loadMediaFileToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem playMediaFileToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem clearPicturToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem eBoxToolStripMenuItem;
    }
}

