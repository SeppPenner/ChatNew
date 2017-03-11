using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Chat
{

	public delegate void ClientAction(); 

	//public class ChatClient:ChatStream,IChatClient
    public partial class ChatClient : Form, IChatClient
    {


        //external
        [DllImport("user32", EntryPoint = "FlashWindow", SetLastError = true,
         CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern bool FlashWindow(int hwnd, bool bInvert);

        private ChatStream _chatstream;
        //delegate
        private ClientAction action;

        //media_id
        private int _media_id = 0;
        //nickname
        private string nickname = "";
        //the socket client	
        private TcpClient tcpc = null;
        //netwoek stream for client
        private NetworkStream stream = null;
        //listener thread
        private Thread tl = null;


        private string _currentpath = "";
        public static string MediaPlayerDirectory = "";
        private Form authForm = null;
        private string host = null;
        //default to port 1300
        private int port = 1300;
        //used by pic contol
        //start point for mouse down
        private Point pt1;
        //last x pos to draw new picture
        private int piclastx = 0;
        //data from server
        private string responseData;

        //Hash table to keep all member color to use for
        //displaying text for that member
        private Hashtable memberColor;
        //A auto increment number to keep track of
        //last color assigned
        private int nextcolorindex = 0;
        //form_activated
        private bool form_activated = false;

        //pause_listening
        private bool pause_listening = false;

        //Chinese fonts
        private Font cfont;

        //Color picking from 10 preselected colors
        private Color getColor(int next)
        {
            switch (next % 10)
            {
                case 0: return Color.Black;
                case 1: return Color.Brown;
                case 2: return Color.Red;
                case 3: return Color.DarkBlue;
                case 4: return Color.DarkMagenta;
                case 5: return Color.DeepSkyBlue;
                case 6: return Color.Yellow;
                case 7: return Color.DarkCyan;
                case 8: return Color.DarkGreen;
                case 9: return Color.ForestGreen;
                default: return Color.Black;
            }
        }



        //All the Actions**********************
        //HANDLE ERROR
        private void action_handle_error()
        {
            //handle abrupt network error
            Console.WriteLine("Error:Possibily Network Error or Server has shut down");
            //Provide the user a way to save the content of the message
            //to a rich text file
            if (MessageBox.Show(this,
                "Server/Connection Error\n" +
                "Do you want to Save Display Messages?",
                "ISS Chat",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string filename = DateTime.Now.ToString("yyyymmddhhmmss") + ".rtf";
                string directory = Application.StartupPath;
                if (!directory.EndsWith(@"\")) directory += @"\";
                filename = directory + filename;
                rtb.SaveFile(filename);
                MessageBox.Show(this,
                    "Display Messages saved in\n" +
                    filename + "\n" +
                    "You can view it using WinWord or any Editor/Reader\n" +
                    "that support rtf format",
                    "ISS Chat");
            }
            Application.Exit();
            Environment.Exit(0);
        }



        //AUTOMATICALLY INDICATE TO SERVER TO GET MEDIA
        private void action_auto_get_media()
        {
            string[] s = responseData.Split(' ');
            string sender = "";

            //the triggering special message is
            //"server> <target> picture from <sender>
            //note that there are at least 4 spaces
            //and thus at least 5 subtexts
            //the 5th subtext is expected to be the sender name
            if (s.Length >= 5)
                sender = s[s.Length - 1];

            action_message();

            string ext = shp1.Text.Substring(shp1.Text.Length - 3);
            //automatically send command to the server
            _chatstream.Write(":get media:" + sender + ":" + ext);
        }


        //AUTOMATICALLY INDICATE TO SERVER TO GET PIC
        private void action_auto_get_pic()
        {
            string[] s = responseData.Split(' ');
            string sender = "";

            //the triggering special message is
            //"server> <target> picture from <sender>
            //note that there are at least 4 spaces
            //and thus at least 5 subtexts
            //the 5th subtext is expected to be the sender name
            if (s.Length >= 5)
                sender = s[s.Length - 1];

            action_message();

            //automatically send command to the server
            _chatstream.Write(":get pic:" + sender);
        }

        //MESSAGE
        //Display message from server to the rich text box
        private void action_message()
        {
            //get the color for the member
            //string format is alway "user> message"
            string[] data = responseData.Split('>');
            string member = data[0].ToUpper();

            //Default to empty color
            Color colr = new Color();

            if (memberColor[member] == null)
            {
                colr = getColor(nextcolorindex++);
                memberColor.Add(member, colr);
            }
            else
            {
                colr = (Color)memberColor[member];
            }

            //Write to the Rich Text Box 
            /*
            rtb.SelectionStart=rtb.Text.Length;
            rtb.SelectionLength=0;  
            rtb.SelectedText="\n";
            */

            WriteRTB("\n", colr, rtb.Font);

            char[] chars = responseData.ToCharArray();

            string s_a = ""; //ASCII
            string s_n = ""; //NON_ASCII

            foreach (char c in chars)
            {
                if (((int)c) < 256)
                {
                    //rtb.SelectionFont=rtb.Font;
                    s_a = s_a + new string(c, 1);
                    if (s_n != "")
                    {
                        WriteRTB(s_n, colr, cfont);
                        s_n = "";
                    }
                }
                else
                {
                    //rtb.SelectionFont=cfont;
                    s_n = s_n + new string(c, 1);
                    if (s_a != "")
                    {
                        WriteRTB(s_a, colr, rtb.Font);
                        s_a = "";
                    }
                }

                // rtb.SelectionStart=rtb.Text.Length;
                // rtb.SelectionLength=0;
                // rtb.SelectionColor=colr;
                // rtb.SelectedText=new string(c,1);

            }

            //Write the rest    
            if (s_n != "")
                WriteRTB(s_n, colr, cfont);

            if (s_a != "")
                WriteRTB(s_a, colr, rtb.Font);


        }

        private void WriteRTB(string s, Color colr, Font f)
        {
            rtb.SelectionStart = rtb.Text.Length;
            rtb.SelectionLength = 0;
            rtb.SelectionColor = colr;
            rtb.SelectionFont = f;
            rtb.SelectedText = s;

            //just for fun. Test Thai
            //rtb.SelectedText=new string('\u0E20',1)+s;
            //textbox.Text=new string(new char[]{'\u0E20','\u0E30'});
        }

        //SEND MEDIA
        //Sending picture to the server
        private void action_server_send_media()
        {
            //To store the data in a media file
            //name convention is <sender>_<target>.ext

            if (shp1.Text.Equals("Empty"))
            {
                _chatstream.Write("" + 0);
                return;
            }

            String ext = shp1.Text.Substring(shp1.Text.Length - 3);
            _chatstream.Write(ext);

            FileInfo fi = new FileInfo(_currentpath + "\\" + nickname + "." + ext);
            FileStream f = new FileStream(_currentpath + "\\" + nickname + "." + ext, FileMode.Open);
            byte[] b = new byte[fi.Length];



            f.Read(b, 0, b.Length);
            f.Close();


            _chatstream.Write("" + b.Length);
            //Thread.Sleep(500);
            _chatstream.WriteBinary(b);

            //Console.WriteLine("Send: {0} bytes", b.Length);			


        }

        //GET MEDIA
        //Geting picture from server
        private void action_server_get_media()
        {
            string ext = _chatstream.Read();

            string snumbytes = _chatstream.Read();
            int numbytes = int.Parse(snumbytes);
            //int numbytes=0;
            byte[] readbytes = _chatstream.ReadBinary(numbytes);

            if (readbytes == null)
            {
                //Console.WriteLine("Error getting picture");
                responseData = "server> Error getting picture";
                action_message();
                return;

            }

            FileStream f = new FileStream(_currentpath + "\\" + nickname + "_received." + ext, FileMode.Create);
            f.Write(readbytes, 0, numbytes);
            f.Close();

            // shpR.Text=""+(numbytes/1000)+"KB";

            rtb.SelectionStart = rtb.Text.Length;
            _media_id++;

            string c_currentpath = _currentpath.Replace(" ", "@");

            rtb.SelectedText = "\nfile:///" + c_currentpath + "\\" + nickname + "_received" + _media_id + "." + ext;

            File.Copy(_currentpath + "\\" + nickname + "_received." + ext,
                      _currentpath + "\\" + nickname + "_received" + _media_id + "." + ext, true);

            //Play the sound
            //MediaPlayer.Play(this.rtb.Handle,_currentpath +"\\"+nickname+"_received.ext");			
        }


        //SEND PIC
        //Sending picture to the server
        private void action_server_send_pic()
        {
            //Console.WriteLine("server> send pic");	
            //Save the picture box image to the memory	
            MemoryStream ms = new MemoryStream();
            pic.Image.Save(ms, ImageFormat.Jpeg);
            //Get the memory buffer
            byte[] buf = ms.GetBuffer();
            ms.Close();

            _chatstream.Write("" + buf.Length);
            //Thread.Sleep(500);
            _chatstream.WriteBinary(buf);

            //Console.WriteLine("Send: {0} bytes", buf.Length);		
        }

        //GET PIC
        //Geting picture from server
        private void action_server_get_pic()
        {

            string snumbytes = _chatstream.Read();
            int numbytes = int.Parse(snumbytes);
            //int numbytes=0;
            byte[] readbytes = _chatstream.ReadBinary(numbytes);

            if (readbytes == null)
            {
                //Console.WriteLine("Error getting picture");
                responseData = "server> Error getting picture";
                action_message();
                return;

            }

            //Create the image from memory
            MemoryStream ms = new MemoryStream(readbytes);
            Image img = Image.FromStream(ms);
            ms.Close();

            //Paste picture to the rich text box
            PastePictureToRTB(img);


        }

        //QUIT
        private void action_server_says_quit()
        {
            Application.Exit();
            Environment.Exit(0);
        }

        //CONNECTION
        private void action_connection()
        {
            //Console.WriteLine("action_connection");
            string[] s1 = responseData.Split(':');
            this.Text = "Chat User: " + s1[1];
            nickname = s1[1];

            this.WindowState = FormWindowState.Normal;

            //form has been loaded.
            //Clear the picture box
            ClearPicture(null, null);

        }


    
        private void textbox_SizeChanged(object sender, System.EventArgs e)
        {
            rtb.Width = this.Width - 20;
        }

        private void CommandPopUp(object sender, System.EventArgs e)
        {

            pause_listening = true;
            Thread.Sleep(0);
            responseData = "";
            _chatstream.Write(":list all");

            //if no data had been read after the write
            //attempt to read for 2 seconds
            long delay = 0;

            while (responseData == "" && delay < 2000)
            {
                Thread.Sleep(200);
                delay += 200;
            }

            //no data has been read so just return
            if (responseData == "")
            {
                pause_listening = false;
                return;
            }

            string[] s = responseData.Split('\n');

            ArrayList arrlist = new ArrayList();
            foreach (string s1 in s)
                if (s1.Trim() != "")
                {
                    string[] s2 = s1.Trim().Split(':');

                    if ((s2.Length == 2) && (s2[0].IndexOf("server>") < 0))
                        arrlist.Add(s2[0].Trim());
                }

            //Console.WriteLine(arrlist.Count);


            p_smItemS1.DropDownItems.Clear();
            p_smItemS2.DropDownItems.Clear();
            p_smItem5.DropDownItems.Clear();
            p_smItem6.DropDownItems.Clear();
            p_smItem7.DropDownItems.Clear();

            foreach (string name in arrlist)
            {
                //Console.WriteLine(name);
                p_smItemS2.DropDownItems.Add(name,null, new EventHandler(Param1Command));
                if (!shp1.Text.Equals("Empty"))
                    p_smItemS1.DropDownItems.Add(name, null,new EventHandler(Param1Command));
                p_smItem5.DropDownItems.Add(name,null, new EventHandler(Param1Command));
                p_smItem6.DropDownItems.Add(name,null, new EventHandler(Param1Command));
                p_smItem7.DropDownItems.Add(name + ":",null, new EventHandler(Param1Command));
            }

            responseData = "";
            pause_listening = false;
            Thread.Sleep(0);
        }

        private void Param1Command(object sender, System.EventArgs e)
        {
            string p = ((ToolStripMenuItem)((ToolStripMenuItem)sender).OwnerItem).Text.Trim();
            string s = ((ToolStripMenuItem)sender).Text.Trim();
            if (p.ToUpper() == ":PRIVATE:")
            {
                textbox.Text = p + s + "<Key in Message>";
                textbox.SelectionStart = textbox.Text.Length - 16;
                textbox.SelectionLength = 16;
            }
            else
                _chatstream.Write(p + s);
        }

        private void Commands(object sender, System.EventArgs e)
        {
            string s = ((ToolStripMenuItem)sender).Text;
            //Console.WriteLine(s);
            _chatstream.Write(s);
            if (s == ":change room")
            {
                textbox.Text = "<Enter Room Number>";
                textbox.SelectionStart = 0;
                textbox.SelectionLength = textbox.Text.Length;

            }


        }

        private void PlayMediaFile(object sender, System.EventArgs e)
        {

            try
            {
                if (((ToolStripMenuItem)sender).Text.Equals("Play Media File"))
                    if (!shp1.Text.Equals("Empty"))
                    {
                        String ext = shp1.Text.Substring(shp1.Text.Length - 3);
                        WinMediaPlayer.Play(rtb.Handle, _currentpath + "\\" + nickname + "_for_testing." + ext);

                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }



        private void LoadMedia(object sender, System.EventArgs e)
        {
            //let user choose the file
            OpenFileDialog fd = new OpenFileDialog();
            fd.Multiselect = false;

            //Filter for supported picture file format
            fd.Filter =
                "Media Files(*.WAV;*.MPG;*.WMV;*.WMA;*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF;*.WAV;*.MPG;*.WMV;*.WMA)";
            //				"Wave Files(*.WAV)|*.WAV|Windows Media Video(*.WMV)|*.WMV|" +
            //				"MPEG Files(*.MPG)|*.MPG|GIF Files(*.GIF)|*.GIF|" +
            //				"JPEG Files(*.JPG)|*.JPG";

            //if file choosen
            if (fd.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    String ext = fd.FileName.Substring(fd.FileName.Length - 3);

                    File.Copy(fd.FileName, _currentpath + "\\" + nickname + "." + ext, true);
                    File.Copy(fd.FileName, _currentpath + "\\" + nickname + "_for_testing." + ext, true);
                    shp1.Text = Path.GetFileName(fd.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());

                }

            }
        }


        //Clear picture of picture box
        private void ClearPicture(object sender, System.EventArgs e)
        {
            Graphics g = Graphics.FromImage(pic.Image);
            g.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, pic.Width, pic.Height));
            pic.Refresh();
            piclastx = 0;
        }


        //Load picture into picture box
        private void LoadPictureFile(object sender, System.EventArgs e)
        {
            //let user choose the file
            OpenFileDialog fd = new OpenFileDialog();
            fd.Multiselect = true;

            //Filter for supported picture file format
            fd.Filter =
                "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";

            //if file choosen
            if (fd.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    foreach (string s in fd.FileNames)
                    {
                        FileStream fs = new FileStream(s, FileMode.Open);
                        Image img = Image.FromStream(fs);
                        fs.Close();
                        PastePicture(img);
                    }
                }
                catch
                {
                    MessageBox.Show("Invalid Picture");
                }
            }
        }

        //Paste picture to the Rich Text Box via the Clipboard
        private void PastePictureToRTB(Image img)
        {

            //Create a bitmap from the image
            Bitmap b_m = new Bitmap(img);
            //Copy bitmap to cliboard
            try
            {
                Clipboard.SetDataObject(b_m, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            //Get the data format for bitmap
            DataFormats.Format bmp = DataFormats.GetFormat(DataFormats.Bitmap);

            //Set the insertion point
            rtb.SelectionStart = rtb.Text.Length;
            rtb.SelectedText = "\n";
            rtb.SelectionStart = rtb.Text.Length;

            //Set the flag so that pasting is allowed    
            rtb.ReadOnly = false;
            //Check if can paste
            if (rtb.CanPaste(bmp))
            {
                rtb.Paste(bmp);
                //Console.WriteLine("Picture Pasted");  
            }
            else
            {
                //Console.WriteLine("Picture Not Pasted");    	
            }
            //Set back to readonly
            rtb.ReadOnly = true;

        }

        //Paste Picture to PictureBox
        private void PastePicture(Image img)
        {
            Graphics g = Graphics.FromImage(pic.Image);
            float scale_ratio = ((float)pic.Image.Height) / ((float)img.Height);
            if ((piclastx + (int)(img.Width * scale_ratio)) >= pic.Width)
                piclastx = 0;

            g.DrawImage(img,
                new Rectangle(piclastx, 0, (int)(img.Width * scale_ratio), (int)(img.Height * scale_ratio)),
                new Rectangle(0, 0, img.Width, img.Height),
                GraphicsUnit.Pixel
                );

            piclastx = piclastx + (int)(img.Width * scale_ratio);

            pic.Refresh();
        }



        //******Events Handlers*********
        //Reset the messages display
        private void button_Click(object sender, EventArgs e)
        {
            rtb.Text = "";
            rtb.Refresh();
        }


        //draw doodles
        private void pic_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            Point pt = new Point(e.X, e.Y);
            Graphics g = Graphics.FromImage(pic.Image);
            g.DrawLine(new Pen(Color.Black), pt1, pt);
            pt1 = pt;
            pic.Refresh();

        }


        //mark start point of mouse down for
        //purpose of doole drawing
        private void pic_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
                pt1 = new Point(e.X, e.Y);

        }

        //form activated
        private void form_Activated(Object sender, System.EventArgs e)
        {


            if (!form_activated)
            {
  

                if(!Authenticate())
                {
                    this.Close();
                    Application.Exit();
                    Environment.Exit(0);

                }

              if (!WinMediaPlayer.GetMediaPlayerDirectory().Equals(""))
                ChatClient.MediaPlayerDirectory = WinMediaPlayer.GetMediaPlayerDirectory();


              pic.Image = new System.Drawing.Bitmap(pic.Width, pic.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
  
            }
            form_activated = true;
            //MessageBox.Show(this,"Directory " +MediaPlayer.GetMediaPlayerDirectory());
            this.TopMost = false;
           
        }



        //handle key entry in text box
        private void textbox_KeyPressed(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar == '\r')
            {
                e.Handled = true;
                return;
            }
           

            //System.Diagnostics.Debug.WriteLine(textbox.Text + e.KeyChar + " " + textbox. );
            string s = "";
            if (e.KeyChar == ' ' && textbox.SelectionStart >= 6)
            {
                textbox.SelectedText = "";
                s = textbox.Text.Substring(textbox.SelectionStart - 6, 6);
                //  System.Diagnostics.Debug.WriteLine(s);
                if (s.Substring(0, 2).ToUpper() == "\\U")
                {
                    string s1 = s.Substring(2, 4);
                    //     System.Diagnostics.Debug.WriteLine(s1);
                    int d = Convert.ToInt32("0020", 16);
                    try
                    {
                        d = Convert.ToInt32(s1, 16);
                    }
                    catch
                    {

                        e.Handled = true;
                        return;
                    };

                    byte b1 = (byte)(d >> 8);
                    byte b0 = (byte)(d % 256);
                    byte[] bytes = new byte[2];
                    bytes[0] = b0; bytes[1] = b1;
                    UnicodeEncoding u = new UnicodeEncoding();

                    string s2 = u.GetString(bytes);
                    // System.Diagnostics.Debug.WriteLine(s2);
                    textbox.SelectionStart = textbox.SelectionStart - 6;
                    textbox.SelectionLength = 6;
                    textbox.SelectedText = s2;
                    e.Handled = true;
                }
            }



        }

        private void rtb_TextChanged(object sender, System.EventArgs e)
        {
            //move to last line
            rtb.SelectionStart = rtb.Text.Length;
            rtb.Focus();
            rtb.ScrollToCaret();

            //focus passed to input box
            textbox.Focus();
        }

        //*****Constructor*****
        //1 parameter constructor
        public ChatClient(string _host)
            : this(_host, 1300)
        {
        }

        //2 parameter constructor
        public ChatClient(string _host, int _port)
        {

            _currentpath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);

            //store the host string
            host = _host;

            //store the port number
            port = _port;
           
           
            //Get the connection
            try
            {
                tcpc = Connect(host, port);
                stream = tcpc.GetStream();
                _chatstream = new ChatStream(stream);

               // _chatstream.Stream = stream;
            }
            catch//(Exception e)
            {
                //Console.WriteLine(e);
                Environment.Exit(0);
            }




            //Initialize the GUI
            Control.CheckForIllegalCrossThreadCalls = false;

            InitializeComponent();
            // init_components();
            cfont = new Font("Arial", 14);
            memberColor = new Hashtable();

            //Start Listening
            tl = new Thread(new ThreadStart(Listen));
            tl.SetApartmentState(ApartmentState.STA);
            tl.Start();



        }

		#region IChatClient Members

		//connect to the server
		public TcpClient Connect(string address,int port)
		{
			return new TcpClient(address,port);
		}
		
		
		
 

        public bool Authenticate()
        {
            authForm = new FormAuthentication();
            ((FormAuthentication)authForm)._chatstream = this._chatstream;
            DialogResult dr = authForm.ShowDialog();
            if (!(dr == DialogResult.OK))
                return false;
            else 
                return true;
        }


		
		//read and response to mesaages
		public void Listen()
		{
			//To ensure that the form is activated
			while(!form_activated)Thread.Sleep(0);

			try
			{
				while (true)
				{
					action=null;

					//waiting to read
					responseData=_chatstream.Read();
					
					//if asked to pause	
					//while paused data may have been used by another method						
					while(pause_listening)Thread.Sleep(0);	
								
					//check if data had been cleared																	
					if (responseData=="") 
					{
					  continue;
				    }
					
					if(this.WindowState==FormWindowState.Minimized)
					{
						FlashWindow(this.Handle.ToInt32(),true);
					}    
    
					
					//default asction
					if (responseData!="")
					  action=new ClientAction(action_message);

					//Perform this only if checkbox is checked
					if(checkbox.Checked)
						if(responseData.IndexOf(ChatProtocolValues.PIC_FROM_MSG("",nickname))==0)
							action=new ClientAction(action_auto_get_pic);

					if(checkbox.Checked)
						if(responseData.IndexOf(ChatProtocolValues.MEDIA_FROM_MSG("",nickname))==0)
							action=new ClientAction(action_auto_get_media);					

					//Special messages from server 
					//signal to quit from server	        
					if(responseData.IndexOf(ChatProtocolValues.QUIT_MSG)==0)
						action=new ClientAction(action_server_says_quit);		       	
	    
					//Connection established
					if(responseData.IndexOf(ChatProtocolValues.CONNECTION_HEADER_MSG)==0)
						action=new ClientAction(action_connection);
	        
					//signal to get picture
					if(responseData==ChatProtocolValues.GET_PIC_MSG)
						action=new ClientAction(action_server_get_pic);
	    
					//Signal to send picture
					if(responseData==ChatProtocolValues.SEND_PIC_MSG) 
						action=new ClientAction(action_server_send_pic);
						
					//signal to get media
					if(responseData==ChatProtocolValues.GET_MEDIA_MSG)
						action=new ClientAction(action_server_get_media);
	    
					//Signal to send media
					if(responseData==ChatProtocolValues.SEND_MEDIA_MSG) 
						action=new ClientAction(action_server_send_media);						
						
	    
					//perform the action	    
					if (action!=null)
					  action();	    
				}  
			}
			catch (Exception ex)
			{
                MessageBox.Show(ex.ToString());

				action=new ClientAction(action_handle_error);
				action();
			}     
		}//LISTEN

		#endregion

		private void rtb_LinkClicked(object sender, LinkClickedEventArgs e)
		{
		
		
			string s=e.LinkText.Substring(8);
			string v=s.Replace("@"," ");
			WinMediaPlayer.Play(rtb.Handle,v);


        }

        private void form_Closing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
            Environment.Exit(0);
        }


        private void textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter )
            {
               
                string s = textbox.Text;

                s = s.Replace("\r\n", "");
                textbox.Clear();
                textbox.Text = "";
                textbox.Select(0, 0);
                textbox.Refresh();
                if (s.Length <= 0) return;


                //save the user time to type in private message header	  
                string[] s1 = s.Split(':');
                string toretain = "";
                if (s1.Length == 4)
                    if (s1[1].ToUpper() == "PRIVATE")
                        toretain = ":" + s1[1] + ":" + s1[2] + ":";

                //save the user time to send pic header		  
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
	
	
	internal class Helpers 
	{

		public const int SW_MINIMIZE=6;
		public const int SW_NORMAL=1;
		public const int SW_FORCEMINIMIZE  = 11;
		public const int IDC_CROSS = 32515;
		public const int IDC_WAIT = 32514;
		public const int IDC_ARROW = 32512;
		public const int KEY_QUERY_VALUE = 0x1;


		
		[DllImport("shell32.dll")]
		public static extern System.IntPtr  ShellExecute(
			IntPtr hwnd,
			string lpVerb,
			string lpFile,
			string lpParameter,
			string lpDirectory,
			int nShowCmd
		);	
		
	}

	

	public class WinMediaPlayer 
	{
		public static string GetMediaPlayerDirectory()
		{
		 
			try
			{
				Microsoft.Win32.RegistryKey localmachineregkey=Microsoft.Win32.Registry.LocalMachine;
				Microsoft.Win32.RegistryKey mediaplayerkey=localmachineregkey.OpenSubKey(@"SOFTWARE\Microsoft\MediaPlayer");
				return (string)mediaplayerkey.GetValue("Installation Directory");
			}
			catch
			{
               return "";
			}
		  
		 
		  
		   
		}

		public static void Play(IntPtr hwnd,string strFileName)
		{
          if(!ChatClient.MediaPlayerDirectory.Equals(""))
		  	Helpers.ShellExecute(hwnd,"open","wmplayer","\""+strFileName+"\"",ChatClient.MediaPlayerDirectory ,Helpers.SW_NORMAL);

		  
		}
    }



    }





