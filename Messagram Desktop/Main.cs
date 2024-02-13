using Messagram_Desktop.Messagram;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Messagram_Desktop
{
    public partial class Main : Form
    {
        public messagram m;
        public Thread log_listener;

        public ListBox friendsBox;
        public ListBox[] servers = new ListBox[] { }; // LIST OF SERVERS
        public Main()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SignIn login = new SignIn();
            login.ShowDialog();
            this.m = login.m;

            richTextBox1.Parent = this;
            richTextBox1.BringToFront();

            this.log_listener = new Thread(get_logs);
            this.log_listener.Start();


            /* Create new listbox for friends list */
            this.friendsBox = new ListBox();
            this.friendsBox.Parent = MessageTabContainer;

            /* Setting Size and Location */
            this.friendsBox.Font = new Font("Microsoft Sans Serif", 15);
            this.friendsBox.Location = new Point(0, 60);
            this.friendsBox.Size = new Size(378, 198);
            this.friendsBox.BackColor = Color.FromArgb(42, 42, 42);
            this.friendsBox.BorderStyle = BorderStyle.None;
            this.friendsBox.ForeColor = Color.Cyan;
            this.friendsBox.Dock = DockStyle.Fill;

            for (int i = 0; i <= 5; i++)
                this.friendsBox.Items.Add($"{i} NEW FRIEND");
        }

        /*
         * Spawns a new community/chatbox 
         * 
         * Used for user joining a community or creating one
         */
        public void spawn_new_server()
        {
            /* Create the chat box */
            ListBox new_server = new ListBox();
            new_server.Parent = MessageTabContainer;

            /* Setting Size and Location */
            new_server.Font = new Font("Microsoft Sans Serif", 15);
            new_server.Location = new Point(0, 60);
            new_server.Size = new Size(378, 198);
            new_server.BackColor = Color.FromArgb(42, 42, 42);
            new_server.BorderStyle = BorderStyle.None;
            new_server.ForeColor = Color.Cyan;
            new_server.Dock = DockStyle.Fill;
            new_server.BringToFront();

            new_server.Items.Add($"Welcome to {textBox1.Text}'s Server....!");

            ListBox[] n = new ListBox[] { new_server };

            //this.servers = (ListBox[])n.Clone();
            this.chat_list.Items.Add(textBox1.Text);
        }

        public void get_logs()
        {
            while(true)
            {
                richTextBox1.Text = this.m.MessagramLogs;
                Thread.Sleep(1000); // milliseconds
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.m.die();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.log_listener.Abort();
            this.m.die();
            Environment.Exit(0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"{this.servers.Length}");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.spawn_new_server();
        }

        private void richTextBox1_MouseHover(object sender, EventArgs e)
        {

        }

        private void richTextBox1_MouseLeave(object sender, EventArgs e)
        {
            richTextBox1.Visible = false;
        }

        private void panel1_MouseHover(object sender, EventArgs e)
        {
            richTextBox1.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.friendsBox.BringToFront();
        }

        private void chat_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = this.chat_list.SelectedIndex;
            MessageBox.Show($"Chats In ChatLIST:{this.chat_list.Items.Count}\nChats in 'this.servers': {this.servers.Length}\nCurrent Server Selected: {Convert.ToInt32(i)}");
            this.servers[0].BringToFront();
        }
    }
}
