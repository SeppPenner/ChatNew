using System;
using System.Windows.Forms;
using Chat;

namespace ChatClient
{
    public partial class FormAuthentication : Form
    {
        private bool _passwordattempted;
        private string _responseData;
        public ChatStream Chatstream;

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
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void Authenticate()
        {
            if (!_passwordattempted)
                _responseData = Chatstream.Read();
            if (_responseData.IndexOf(AuthenticationProtocolValues.CommandPrompt, StringComparison.Ordinal) >= 0)
            {
                if (rb_register.Checked) Chatstream.Write(rb_register.Text);
                if (rb_login.Checked) Chatstream.Write(rb_login.Text);
                _responseData = Chatstream.Read();
            }

            if (_responseData.IndexOf(AuthenticationProtocolValues.UseridPrompt, StringComparison.Ordinal) >= 0)
            {
                rb_register.Enabled = false;
                rb_login.Enabled = false;
                Chatstream.Write(tb_username.Text);
                _responseData = Chatstream.Read();
            }

            if (_responseData.IndexOf(AuthenticationProtocolValues.PasswordPrompt, StringComparison.Ordinal) >= 0)
            {
                tb_username.Enabled = false;
                Chatstream.Write(tb_userpassword.Text);
                _responseData = Chatstream.Read();
                if (rb_login.Checked) _passwordattempted = true;
            }

            MessageBox.Show(FormatMessage(_responseData));

            if (_responseData.IndexOf(AuthenticationProtocolValues.QuitMsg, StringComparison.Ordinal) == 0)
            {
                DialogResult = DialogResult.Abort;
                Close();
            }

            if (_responseData.IndexOf(AuthenticationProtocolValues.AutenticatedMsg, StringComparison.Ordinal) == 0)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            if (_responseData.IndexOf(AuthenticationProtocolValues.UserAlreadyLoggedIn, StringComparison.Ordinal) != 0)
                return;
            DialogResult = DialogResult.No;
            Close();
        }

        private string FormatMessage(string message)
        {
            if (message.Equals(AuthenticationProtocolValues.PasswordPrompt))
                return "Incorrect passport";
            var marker = message.IndexOf("> ", StringComparison.Ordinal) + 2;
            return message.Substring(marker);
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            Authenticate();
        }
    }
}