using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Chat
{
    public partial class FormAuthentication : Form
    {
        //private bool _authenticated = false;
        private bool _passwordattempted = false;
        public ChatStream _chatstream;
        public string responseData;

        public FormAuthentication()
        {
            InitializeComponent();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void FormAuthentication_Activated(object sender, EventArgs e)
        {
            tb_username.Focus();
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
          //  _authenticated = false;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
           // authForm.Close();

        }

        public bool Authenticate()
        {

            //if we had attempted a password before
            //do not get new response
            //just process the old response ( which is password prompt)
            if (!_passwordattempted)
                responseData = _chatstream.Read();


            //Console.Write("**" +responseData+"**");			


            //step 1
            if (responseData.IndexOf(AuthenticationProtocolValues.COMMAND_PROMPT) >= 0)
            {
                if (rb_register.Checked) _chatstream.Write(rb_register.Text);
                if (rb_login.Checked) _chatstream.Write(rb_login.Text);

                responseData = _chatstream.Read();
                //Console._chatstream.Write(responseData);


            }


            //step 2
            if (responseData.IndexOf(AuthenticationProtocolValues.USERID_PROMPT) >= 0)
            {
                rb_register.Enabled = false;
                rb_login.Enabled = false;
                _chatstream.Write(tb_username.Text);
                responseData = _chatstream.Read();
                //Console.Write(responseData);			   
            }



            //step 3 .. up to 3 tries
            if (responseData.IndexOf(AuthenticationProtocolValues.PASSWORD_PROMPT) >= 0)
            {
                //we are processing the password,
                // do not allow user to change userid
                tb_username.Enabled = false;
                _chatstream.Write(tb_userpassword.Text);
                responseData = _chatstream.Read();
                //Console.Write(responseData);
                //password verification has been attempted
                if (rb_login.Checked) _passwordattempted = true;
            }

            //show the message from server on results of
            //the authenication
            MessageBox.Show(FormatMessage(responseData));

            if (responseData.IndexOf(AuthenticationProtocolValues.QUIT_MSG) == 0)
            {
               // _authenticated = false;
                this.DialogResult = DialogResult.Abort;
                this.Close();
               // authForm.Close();
            }

            if (responseData.IndexOf(AuthenticationProtocolValues.AUTENTICATED_MSG) == 0)
            {
               // _authenticated = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
               // authForm.Close();
            }

            if (responseData.IndexOf(AuthenticationProtocolValues.USER_ALREADY_LOGGED_IN) == 0)
            {
               // _authenticated = false;
                this.DialogResult = DialogResult.No;
                this.Close();
                //authForm.Close();
            }



            return true; //dummy return			

        }

        private string FormatMessage(string message)
        {

            if (message.Equals(AuthenticationProtocolValues.PASSWORD_PROMPT))
                return "Incorrect passport";
            else
            {
                int marker = message.IndexOf("> ") + 2;
                return message.Substring(marker);
            }



        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            Authenticate();
        }


    }
}
