namespace ChatClient
{
    partial class FormAuthentication
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
            this.btn_ok = new System.Windows.Forms.Button();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.lbl_username = new System.Windows.Forms.Label();
            this.lbl_userpassword = new System.Windows.Forms.Label();
            this.tb_username = new System.Windows.Forms.TextBox();
            this.tb_userpassword = new System.Windows.Forms.TextBox();
            this.rb_login = new System.Windows.Forms.RadioButton();
            this.rb_register = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // btn_ok
            // 
            this.btn_ok.Location = new System.Drawing.Point(37, 181);
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.Size = new System.Drawing.Size(48, 29);
            this.btn_ok.TabIndex = 0;
            this.btn_ok.Text = "OK";
            this.btn_ok.UseVisualStyleBackColor = true;
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            // 
            // btn_cancel
            // 
            this.btn_cancel.Location = new System.Drawing.Point(204, 181);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(48, 29);
            this.btn_cancel.TabIndex = 1;
            this.btn_cancel.Text = "Cancel";
            this.btn_cancel.UseVisualStyleBackColor = true;
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // lbl_username
            // 
            this.lbl_username.AutoSize = true;
            this.lbl_username.Location = new System.Drawing.Point(34, 77);
            this.lbl_username.Name = "lbl_username";
            this.lbl_username.Size = new System.Drawing.Size(43, 13);
            this.lbl_username.TabIndex = 2;
            this.lbl_username.Text = "User ID";
            // 
            // lbl_userpassword
            // 
            this.lbl_userpassword.AutoSize = true;
            this.lbl_userpassword.Location = new System.Drawing.Point(34, 114);
            this.lbl_userpassword.Name = "lbl_userpassword";
            this.lbl_userpassword.Size = new System.Drawing.Size(53, 13);
            this.lbl_userpassword.TabIndex = 3;
            this.lbl_userpassword.Text = "Password";
            // 
            // tb_username
            // 
            this.tb_username.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_username.Location = new System.Drawing.Point(88, 77);
            this.tb_username.Name = "tb_username";
            this.tb_username.Size = new System.Drawing.Size(100, 22);
            this.tb_username.TabIndex = 4;
            // 
            // tb_userpassword
            // 
            this.tb_userpassword.AcceptsReturn = true;
            this.tb_userpassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_userpassword.Location = new System.Drawing.Point(88, 114);
            this.tb_userpassword.Name = "tb_userpassword";
            this.tb_userpassword.PasswordChar = '*';
            this.tb_userpassword.Size = new System.Drawing.Size(100, 22);
            this.tb_userpassword.TabIndex = 5;
            this.tb_userpassword.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // rb_login
            // 
            this.rb_login.AutoSize = true;
            this.rb_login.Checked = true;
            this.rb_login.Location = new System.Drawing.Point(37, 25);
            this.rb_login.Name = "rb_login";
            this.rb_login.Size = new System.Drawing.Size(47, 17);
            this.rb_login.TabIndex = 6;
            this.rb_login.TabStop = true;
            this.rb_login.Text = "login";
            this.rb_login.UseVisualStyleBackColor = true;
            // 
            // rb_register
            // 
            this.rb_register.AutoSize = true;
            this.rb_register.Location = new System.Drawing.Point(175, 25);
            this.rb_register.Name = "rb_register";
            this.rb_register.Size = new System.Drawing.Size(59, 17);
            this.rb_register.TabIndex = 7;
            this.rb_register.Text = "register";
            this.rb_register.UseVisualStyleBackColor = true;
            // 
            // FormAuthentication
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(288, 246);
            this.Controls.Add(this.rb_register);
            this.Controls.Add(this.rb_login);
            this.Controls.Add(this.tb_userpassword);
            this.Controls.Add(this.tb_username);
            this.Controls.Add(this.lbl_userpassword);
            this.Controls.Add(this.lbl_username);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_ok);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAuthentication";
            this.Text = "Authentication";
            this.Activated += new System.EventHandler(this.FormAuthentication_Activated);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_ok;
        private System.Windows.Forms.Button btn_cancel;
        private System.Windows.Forms.Label lbl_username;
        private System.Windows.Forms.Label lbl_userpassword;
        private System.Windows.Forms.TextBox tb_username;
        private System.Windows.Forms.TextBox tb_userpassword;
        private System.Windows.Forms.RadioButton rb_login;
        private System.Windows.Forms.RadioButton rb_register;
    }
}