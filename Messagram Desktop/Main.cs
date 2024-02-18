using Messagram_Desktop.Messagram;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Messagram_Desktop
{
    public partial class Main : Form
    {
        /*
         * [Required Properties]
         * Messagram Server && Library Connectivity
         */
        public messagram Messagram;
        public Thread messaListener;

        public Main()
        {
            InitializeComponent();
        }

        /* Start of Messagram Client Application */
        private void Form1_Load(object sender, EventArgs e)
        {
            /* Set Original Size Of Form To Load */
            community_list.Items.Add("Messagram");
            this.Size = new Size(801, 450);
            panel6.Location = new Point(89, 406);

            /* Send user to login and Grab Account/Connection Info */
            SignIn login = new SignIn();
            login.ShowDialog();
            this.Messagram = login.m;

            /* Fix Both ChatBoxes For Community & DM */
            this.fix_friendlist_box();
            this.fix_chat_box();

            /* Bring Chatbox Up Front */
            this.big_chat_box.BringToFront();

            this.messaListener = new Thread(get_logs);
            this.messaListener.Start();
        }
       
        public void fix_friendlist_box()
        {
            big_friendlist_box.Font = new Font("Microsoft Sans Serif", 15);
            big_friendlist_box.Location = new Point(0, 60);
            big_friendlist_box.Size = new Size(720, 320);
        }

        public void fix_chat_box()
        {
            big_chat_box.Parent = MessageTabContainer;
            big_chat_box.Font = new Font("Microsoft Sans Serif", 15);
            big_chat_box.Location = new Point(1, 1);
            big_chat_box.Size = new Size(720, 315);
        }

        public void AddMessage(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AddMessage), new object[] { value });
                return;
            }
            big_chat_box.Items.Add(value);
            
            /* Scroll To The Bottom If MessageBox is Filled */
            if(big_chat_box.Items.Count > 12) { 
                int visibleItems = big_chat_box.ClientSize.Height / big_chat_box.ItemHeight;
                big_chat_box.TopIndex = Math.Max(big_chat_box.Items.Count - visibleItems + 1, 0);
            }
        }


        /* Enable a Message Listener */
        public void get_logs()
        {
            this.Messagram.ServerLogs += $"Messagram Message Listener has started....!\n";
            this.AddMessage("Welcome to Messagram Server...!");
            int last_count = 0;
            while(this.Messagram.terminate != true)
            {
                string[] msgs = this.Messagram.Messages.Split('\n');
                if (msgs.Count() != last_count)
                {
                    label8.Text = $"Messages @ {last_count}/{msgs.Count()}";
                    string gg = ($"{msgs[msgs.Count() - 1]}" ?? $"{msgs[0]}");
                    if(msgs.Count() == 0)
                        this.AddMessage($"{gg}");
                    else 
                        this.AddMessage($"{gg}");

                    last_count = msgs.Count();
                }
                Thread.Sleep(1000); // milliseconds
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Messagram.die();
        }
        
        private void label1_Click(object sender, EventArgs e)
        {
            this.messaListener.Abort();
            this.Messagram.die();
            Environment.Exit(0);
        }

        /* Minimize Form Button */
        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        /* Log Display & Log Controls [DEVKIT] */
        private void pictureBox7_Click(object sender, EventArgs e)
        {
            this.Size = new Size(1017, 631);
            richTextBox1.Text = $"[@DEBUG LOGS]:\n\n{this.Messagram.ServerLogs}";
            panel6.Location = new Point(89, 406);
        }

        private void richTextBox1_MouseLeave(object sender, EventArgs e)
        {
            this.Size = new Size(801, 450);
            panel6.Location = new Point(89, 406);
        }

        /*
         * Friend List Image 
         */
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.dm_list.BringToFront();
            this.big_friendlist_box.BringToFront();
        }

        private void chat_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            string listen2 = dm_list.SelectedItem.ToString();
            this.Messagram.ChangeChat(Msg_T.DM, listen2);

            label5.Text = $"{this.Messagram.listen_to}";
            big_chat_box.BringToFront();
            big_chat_box.Items.Clear();
        }

        private void community_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            string listen2 = community_list.SelectedItem.ToString();
            this.Messagram.ChangeChat(Msg_T.COMMUNITY, listen2);

            label5.Text = $"{this.Messagram.listen_to}";
            big_chat_box.BringToFront();
            big_chat_box.Items.Clear();
        }

        /*
        *  Community List
        */
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            community_list.BringToFront();
            big_chat_box.BringToFront();
        }

        /* Settings Icon */
        private void pictureBox5_Click(object sender, EventArgs e)
        {
            
        }

        /* Send message icon */
        private void pictureBox6_Click(object sender, EventArgs e)
        {
            messaResponse r = new messaResponse("", true, Resp_T.NULL, Cmd_T.SEND_DM_MSG, this.Messagram.getInfo());
            r.BuildCmd(new string[] { this.Messagram.Username, textBox1.Text, richTextBox2.Text });
            this.Messagram.SendCmd(r);
        }

        /*
         *  Developer Kit // Used for testing purposes 
         */
        private void button3_Click_1(object sender, EventArgs e)
        {
            try
            {
                this.Messagram.listener.Abort();
            }
            catch { }
            this.messaListener = new Thread(get_logs);
            this.Messagram.ConnectnAuthorize("Jeff", "testpw123");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            messaResponse r = new messaResponse("", true, Resp_T.NULL, Cmd_T.SEND_DM_MSG, this.Messagram.getInfo());
            r.BuildCmd(new string[] { this.Messagram.Username, textBox1.Text, richTextBox2.Text });
            this.Messagram.SendCmd(r);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Messagram.listener.Abort();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            this.messaListener.Abort();
        }

        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            panel6.Location = new Point(89, 406);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            label8.Text = $"Message Count: {this.Messagram.Messages.Count()}";

            foreach (string s in this.Messagram.Messages.Split('\n'))
            {
                richTextBox3.Text = $"{s}\n";
                big_chat_box.Items.Add($"{s}");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            messaResponse r = new messaResponse("", true, Resp_T.NULL, Cmd_T.SEND_FRIEND_REQ, this.Messagram.getInfo());
            r.BuildCmd(new string[] { this.Messagram.Username, textBox1.Text, richTextBox2.Text });
            this.Messagram.ServerLogs += $"{r.data}";
            //this.Messagram.SendCmd(r);
            if (textBox1.Text.Length < 12)
                dm_list.Items.Add($"{textBox1.Text}");
            else
                dm_list.Items.Add($"{textBox1.Text.Substring(0, 10)}...");
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }
    }
}
