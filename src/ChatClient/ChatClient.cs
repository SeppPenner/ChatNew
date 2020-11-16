using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Chat;
using ChatClient.Properties;

namespace ChatClient
{
    public delegate void ClientAction();

    public partial class ChatClient : Form, IChatClient
    {
        public static string MediaPlayerDirectory = "";
        private readonly Font _cfont;
        private readonly ChatStream _chatstream;
        private readonly string _currentpath;
        private readonly Hashtable _memberColor;
        private ClientAction _action;
        private Form _authForm;
        private bool _formActivated;
        private int _mediaId;
        private int _nextcolorindex;
        private string _nickname = "";
        private bool _pauseListening;
        private int _piclastx;
        private Point _pt1;
        private string _responseData;

        public ChatClient(string host, int port = 1300)
        {
            _currentpath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            try
            {
                var tcpc = Connect(host, port);
                var stream = tcpc.GetStream();
                _chatstream = new ChatStream(stream);
            }
            catch
            {
                Environment.Exit(0);
            }
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            _cfont = new Font("Arial", 14);
            _memberColor = new Hashtable();
            var tl = new Thread(Listen);
            tl.SetApartmentState(ApartmentState.STA);
            tl.Start();
        }

        public TcpClient Connect(string address, int port)
        {
            return new TcpClient(address, port);
        }


        public bool Authenticate()
        {
            _authForm = new FormAuthentication();
            ((FormAuthentication) _authForm).Chatstream = _chatstream;
            var dr = _authForm.ShowDialog();
            return dr == DialogResult.OK;
        }

        public void Listen()
        {
            while (!_formActivated) Thread.Sleep(0);
            try
            {
                while (true)
                {
                    _action = null;
                    _responseData = _chatstream.Read();
                    while (_pauseListening) Thread.Sleep(0);
                    if (_responseData == "")
                        continue;
                    if (WindowState == FormWindowState.Minimized)
                        FlashWindow(Handle.ToInt32(), true);
                    if (_responseData != "")
                        _action = action_message;
                    if (checkbox.Checked)
                        if (
                            _responseData.IndexOf(ChatProtocolValues.PIC_FROM_MSG("", _nickname),
                                StringComparison.Ordinal) == 0)
                            _action = action_auto_get_pic;
                    if (checkbox.Checked)
                        if (
                            _responseData.IndexOf(ChatProtocolValues.MEDIA_FROM_MSG("", _nickname),
                                StringComparison.Ordinal) == 0)
                            _action = action_auto_get_media;
                    if (_responseData.IndexOf(ChatProtocolValues.QuitMsg, StringComparison.Ordinal) == 0)
                        _action = action_server_says_quit;
                    if (_responseData.IndexOf(ChatProtocolValues.ConnectionHeaderMsg, StringComparison.Ordinal) == 0)
                        _action = action_connection;
                    if (_responseData == ChatProtocolValues.GetPicMsg)
                        _action = action_server_get_pic;
                    if (_responseData == ChatProtocolValues.SendPicMsg)
                        _action = action_server_send_pic;
                    if (_responseData == ChatProtocolValues.GetMediaMsg)
                        _action = action_server_get_media;
                    if (_responseData == ChatProtocolValues.SendMediaMsg)
                        _action = action_server_send_media;
                    _action?.Invoke();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                _action = action_handle_error;
                _action();
            }
        }


        [DllImport("user32", EntryPoint = "FlashWindow", SetLastError = true,
            CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern bool FlashWindow(int hwnd, bool bInvert);

        private Color GetColor(int next)
        {
            switch (next % 10)
            {
                case 0:
                    return Color.Black;
                case 1:
                    return Color.Brown;
                case 2:
                    return Color.Red;
                case 3:
                    return Color.DarkBlue;
                case 4:
                    return Color.DarkMagenta;
                case 5:
                    return Color.DeepSkyBlue;
                case 6:
                    return Color.Yellow;
                case 7:
                    return Color.DarkCyan;
                case 8:
                    return Color.DarkGreen;
                case 9:
                    return Color.ForestGreen;
                default:
                    return Color.Black;
            }
        }

        private void action_handle_error()
        {
            Console.WriteLine(Resources.PossibilyNetworkError);
            if (MessageBox.Show(this,
                    Resources.ServerConnectionError,
                    Resources.ISS_Chat,
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var filename = DateTime.Now.ToString("yyyymmddhhmmss") + ".rtf";
                var directory = Application.StartupPath;
                if (!directory.EndsWith(@"\")) directory += @"\";
                filename = directory + filename;
                rtb.SaveFile(filename);
                MessageBox.Show(this,
                    Resources.DisplayMessages +
                    filename + Resources.NewLine +
                    Resources.ViewItUsingWinWord +
                    Resources.ThatSupportRtfFormat,
                    Resources.ISS_Chat);
            }
            Application.Exit();
            Environment.Exit(0);
        }

        private void action_auto_get_media()
        {
            var s = _responseData.Split(' ');
            var sender = "";
            if (s.Length >= 5)
                sender = s[s.Length - 1];
            action_message();
            var ext = shp1.Text.Substring(shp1.Text.Length - 3);
            _chatstream.Write(":get media:" + sender + ":" + ext);
        }


        private void action_auto_get_pic()
        {
            var s = _responseData.Split(' ');
            var sender = "";
            if (s.Length >= 5)
                sender = s[s.Length - 1];
            action_message();
            _chatstream.Write(":get pic:" + sender);
        }

        private void action_message()
        {
            var data = _responseData.Split('>');
            var member = data[0].ToUpper();
            Color colr;
            if (_memberColor[member] == null)
            {
                colr = GetColor(_nextcolorindex++);
                _memberColor.Add(member, colr);
            }
            else
            {
                colr = (Color) _memberColor[member];
            }
            WriteRtb("\n", colr, rtb.Font);
            var chars = _responseData.ToCharArray();
            var sA = "";
            var sN = "";
            foreach (var c in chars)
                if (c < 256)
                {
                    sA = sA + new string(c, 1);
                    if (sN == "") continue;
                    WriteRtb(sN, colr, _cfont);
                    sN = "";
                }
                else
                {
                    sN = sN + new string(c, 1);
                    if (sA == "") continue;
                    WriteRtb(sA, colr, rtb.Font);
                    sA = "";
                }
            if (sN != "")
                WriteRtb(sN, colr, _cfont);
            if (sA != "")
                WriteRtb(sA, colr, rtb.Font);
        }

        private void WriteRtb(string s, Color colr, Font f)
        {
            rtb.SelectionStart = rtb.Text.Length;
            rtb.SelectionLength = 0;
            rtb.SelectionColor = colr;
            rtb.SelectionFont = f;
            rtb.SelectedText = s;
        }

        private void action_server_send_media()
        {
            if (shp1.Text.Equals("Empty"))
            {
                _chatstream.Write("" + 0);
                return;
            }
            var ext = shp1.Text.Substring(shp1.Text.Length - 3);
            _chatstream.Write(ext);
            var fi = new FileInfo(_currentpath + "\\" + _nickname + "." + ext);
            var f = new FileStream(_currentpath + "\\" + _nickname + "." + ext, FileMode.Open);
            var b = new byte[fi.Length];
            f.Read(b, 0, b.Length);
            f.Close();
            _chatstream.Write("" + b.Length);
            _chatstream.WriteBinary(b);
        }

        private void action_server_get_media()
        {
            var ext = _chatstream.Read();
            var snumbytes = _chatstream.Read();
            var numbytes = int.Parse(snumbytes);
            var readbytes = _chatstream.ReadBinary(numbytes);
            if (readbytes == null)
            {
                _responseData = "server> Error getting picture";
                action_message();
                return;
            }
            var f = new FileStream(_currentpath + "\\" + _nickname + "_received." + ext, FileMode.Create);
            f.Write(readbytes, 0, numbytes);
            f.Close();
            rtb.SelectionStart = rtb.Text.Length;
            _mediaId++;
            var cCurrentpath = _currentpath.Replace(" ", "@");
            rtb.SelectedText = "\nfile:///" + cCurrentpath + "\\" + _nickname + "_received" + _mediaId + "." + ext;
            File.Copy(_currentpath + "\\" + _nickname + "_received." + ext,
                _currentpath + "\\" + _nickname + "_received" + _mediaId + "." + ext, true);
        }

        private void action_server_send_pic()
        {
            var ms = new MemoryStream();
            pic.Image.Save(ms, ImageFormat.Jpeg);
            var buf = ms.GetBuffer();
            ms.Close();
            _chatstream.Write("" + buf.Length);
            _chatstream.WriteBinary(buf);
        }

        private void action_server_get_pic()
        {
            var snumbytes = _chatstream.Read();
            var numbytes = int.Parse(snumbytes);
            var readbytes = _chatstream.ReadBinary(numbytes);
            if (readbytes == null)
            {
                _responseData = "server> Error getting picture";
                action_message();
                return;
            }
            var ms = new MemoryStream(readbytes);
            var img = Image.FromStream(ms);
            ms.Close();
            PastePictureToRtb(img);
        }

        private void action_server_says_quit()
        {
            Application.Exit();
            Environment.Exit(0);
        }

        private void action_connection()
        {
            var s1 = _responseData.Split(':');
            Text = Resources.ChatUser + s1[1];
            _nickname = s1[1];
            WindowState = FormWindowState.Normal;
            ClearPicture(null, null);
        }


        private void textbox_SizeChanged(object sender, EventArgs e)
        {
            rtb.Width = Width - 20;
        }

        private void CommandPopUp(object sender, EventArgs e)
        {
            _pauseListening = true;
            Thread.Sleep(0);
            _responseData = "";
            _chatstream.Write(":list all");
            long delay = 0;
            while (_responseData == "" && delay < 2000)
            {
                Thread.Sleep(200);
                delay += 200;
            }
            if (_responseData == "")
            {
                _pauseListening = false;
                return;
            }
            var s = _responseData.Split('\n');
            var arrlist = new ArrayList();
            foreach (var s1 in s)
                if (s1.Trim() != "")
                {
                    var s2 = s1.Trim().Split(':');

                    if (s2.Length == 2 && s2[0].IndexOf("server>", StringComparison.Ordinal) < 0)
                        arrlist.Add(s2[0].Trim());
                }
            p_smItemS1.DropDownItems.Clear();
            p_smItemS2.DropDownItems.Clear();
            p_smItem5.DropDownItems.Clear();
            p_smItem6.DropDownItems.Clear();
            p_smItem7.DropDownItems.Clear();
            foreach (string name in arrlist)
            {
                p_smItemS2.DropDownItems.Add(name, null, Param1Command);
                if (!shp1.Text.Equals("Empty"))
                    p_smItemS1.DropDownItems.Add(name, null, Param1Command);
                p_smItem5.DropDownItems.Add(name, null, Param1Command);
                p_smItem6.DropDownItems.Add(name, null, Param1Command);
                p_smItem7.DropDownItems.Add(name + ":", null, Param1Command);
            }
            _responseData = "";
            _pauseListening = false;
            Thread.Sleep(0);
        }

        private void Param1Command(object sender, EventArgs e)
        {
            var p = ((ToolStripMenuItem) ((ToolStripMenuItem) sender).OwnerItem).Text.Trim();
            var s = ((ToolStripMenuItem) sender).Text.Trim();
            if (p.ToUpper() == ":PRIVATE:")
            {
                textbox.Text = p + s + Resources.KeyInMessage;
                textbox.SelectionStart = textbox.Text.Length - 16;
                textbox.SelectionLength = 16;
            }
            else
            {
                _chatstream.Write(p + s);
            }
        }

        private void Commands(object sender, EventArgs e)
        {
            var s = ((ToolStripMenuItem) sender).Text;
            _chatstream.Write(s);
            if (s != ":change room") return;
            textbox.Text = Resources.EnterRoomNumber;
            textbox.SelectionStart = 0;
            textbox.SelectionLength = textbox.Text.Length;
        }

        private void PlayMediaFile(object sender, EventArgs e)
        {
            try
            {
                if (!((ToolStripMenuItem) sender).Text.Equals("Play Media File")) return;
                if (shp1.Text.Equals("Empty")) return;
                var ext = shp1.Text.Substring(shp1.Text.Length - 3);
                WinMediaPlayer.Play(rtb.Handle, _currentpath + "\\" + _nickname + "_for_testing." + ext);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private void LoadMedia(object sender, EventArgs e)
        {
            var fd = new OpenFileDialog
            {
                Multiselect = false,
                Filter =
                    Resources.MediaFilesFilter
            };
            if (fd.ShowDialog(this) != DialogResult.OK) return;
            try
            {
                var ext = fd.FileName.Substring(fd.FileName.Length - 3);

                File.Copy(fd.FileName, _currentpath + "\\" + _nickname + "." + ext, true);
                File.Copy(fd.FileName, _currentpath + "\\" + _nickname + "_for_testing." + ext, true);
                shp1.Text = Path.GetFileName(fd.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ClearPicture(object sender, EventArgs e)
        {
            var g = Graphics.FromImage(pic.Image);
            g.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, pic.Width, pic.Height));
            pic.Refresh();
            _piclastx = 0;
        }

        private void LoadPictureFile(object sender, EventArgs e)
        {
            var fd = new OpenFileDialog
            {
                Multiselect = true,
                Filter = Resources.ImageFilesFilter
            };
            if (fd.ShowDialog(this) != DialogResult.OK) return;
            try
            {
                foreach (var s in fd.FileNames)
                {
                    var fs = new FileStream(s, FileMode.Open);
                    var img = Image.FromStream(fs);
                    fs.Close();
                    PastePicture(img);
                }
            }
            catch
            {
                MessageBox.Show(Resources.InvalidPicture);
            }
        }

        private void PastePictureToRtb(Image img)
        {
            var bM = new Bitmap(img);
            try
            {
                Clipboard.SetDataObject(bM, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            var bmp = DataFormats.GetFormat(DataFormats.Bitmap);
            rtb.SelectionStart = rtb.Text.Length;
            rtb.SelectedText = "\n";
            rtb.SelectionStart = rtb.Text.Length;
            rtb.ReadOnly = false;
            if (rtb.CanPaste(bmp))
                rtb.Paste(bmp);
            rtb.ReadOnly = true;
        }

        private void PastePicture(Image img)
        {
            var g = Graphics.FromImage(pic.Image);
            var scaleRatio = pic.Image.Height / (float) img.Height;
            if (_piclastx + (int) (img.Width * scaleRatio) >= pic.Width)
                _piclastx = 0;
            g.DrawImage(img,
                new Rectangle(_piclastx, 0, (int) (img.Width * scaleRatio), (int) (img.Height * scaleRatio)),
                new Rectangle(0, 0, img.Width, img.Height),
                GraphicsUnit.Pixel
            );
            _piclastx = _piclastx + (int) (img.Width * scaleRatio);
            pic.Refresh();
        }

        private void button_Click(object sender, EventArgs e)
        {
            rtb.Text = "";
            rtb.Refresh();
        }

        private void pic_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            var pt = new Point(e.X, e.Y);
            var g = Graphics.FromImage(pic.Image);
            g.DrawLine(new Pen(Color.Black), _pt1, pt);
            _pt1 = pt;
            pic.Refresh();
        }

        private void pic_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                _pt1 = new Point(e.X, e.Y);
        }

        private void form_Activated(object sender, EventArgs e)
        {
            if (!_formActivated)
            {
                if (!Authenticate())
                {
                    Close();
                    Application.Exit();
                    Environment.Exit(0);
                }
                if (!WinMediaPlayer.GetMediaPlayerDirectory().Equals(""))
                    MediaPlayerDirectory = WinMediaPlayer.GetMediaPlayerDirectory();
                pic.Image = new Bitmap(pic.Width, pic.Height, PixelFormat.Format32bppArgb);
            }
            _formActivated = true;
            TopMost = false;
        }

        private void textbox_KeyPressed(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                e.Handled = true;
                return;
            }
            if (e.KeyChar != ' ' || textbox.SelectionStart < 6) return;
            textbox.SelectedText = "";
            var s = textbox.Text.Substring(textbox.SelectionStart - 6, 6);
            if (s.Substring(0, 2).ToUpper() != "\\U") return;
            var s1 = s.Substring(2, 4);
            int d;
            try
            {
                d = Convert.ToInt32(s1, 16);
            }
            catch
            {
                e.Handled = true;
                return;
            }
            var b1 = (byte) (d >> 8);
            var b0 = (byte) (d % 256);
            var bytes = new byte[2];
            bytes[0] = b0;
            bytes[1] = b1;
            var u = new UnicodeEncoding();
            var s2 = u.GetString(bytes);
            textbox.SelectionStart = textbox.SelectionStart - 6;
            textbox.SelectionLength = 6;
            textbox.SelectedText = s2;
            e.Handled = true;
        }

        private void rtb_TextChanged(object sender, EventArgs e)
        {
            rtb.SelectionStart = rtb.Text.Length;
            rtb.Focus();
            rtb.ScrollToCaret();
            textbox.Focus();
        }

        private void rtb_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            var s = e.LinkText.Substring(8);
            var v = s.Replace("@", " ");
            WinMediaPlayer.Play(rtb.Handle, v);
        }

        private void form_Closing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
            Environment.Exit(0);
        }

        private void textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            var s = textbox.Text;
            s = s.Replace("\r\n", "");
            textbox.Clear();
            textbox.Text = "";
            textbox.Select(0, 0);
            textbox.Refresh();
            if (s.Length <= 0) return;
            var s1 = s.Split(':');
            var toretain = "";
            if (s1.Length == 4)
                if (s1[1].ToUpper() == "PRIVATE")
                    toretain = ":" + s1[1] + ":" + s1[2] + ":";
            if (s1.Length == 3)
                if (s1[1].ToUpper() == "SEND PIC")
                    toretain = s;
            textbox.Text = toretain;
            textbox.Select(toretain.Length, 0);
            textbox.Refresh();
            _chatstream.Write(s);
        }
    }
}