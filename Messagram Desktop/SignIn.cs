using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Messagram_Desktop;
using Messagram_Desktop.Messagram;

namespace Messagram_Desktop
{
    public partial class SignIn : Form
    {
        public messagram m;
        public string username;
        public SignIn()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void SignIn_Load(object sender, EventArgs e)
        {
            this.m = new messagram("official_client", "0.0.1");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            messaResponse r = this.m.ConnectnAuthorize(textBox1.Text, textBox2.Text);
            if (r.resp_t != Resp_T.SOCKET_CONNECTED)
            {
                MessageBox.Show("Messagram server is down!");
                Environment.Exit(0);
            }

            if (checkBox1.Checked)
            {
                // TO-DO: do save password shit here
            }

            this.username = textBox1.Text;

            if (r.cmd == Cmd_T.ACCOUNT_PERM_BAN || r.cmd == Cmd_T.ACCOUNT_TEMP_BAN)
            {
                MessageBox.Show("You're account is temp or perm banned...!\nCheck your email!", "Error");
                Environment.Exit(0);
            } else if (r.cmd == Cmd_T.INVALID_CMD)
            {
                MessageBox.Show("Invalid username or password provided!", "Sign In Error");
                return;
            } else if (r.cmd == Cmd_T.FORCE_CONFIRM_EMAIL)
            {
                // Force user to confirm email or they cant use the account (Just exit app)
                Environment.Exit(0);
            } else if (r.cmd == Cmd_T.FORCE_DEVICE_TRUST)
            {
                // Force am email confirmation to trust device
            } else if (r.cmd == Cmd_T.FORCE_ADD_PHONE_NUMBER_REQUEST)
            {
                // Force user to add a phone number and verify it
                Environment.Exit(0);
            } else if (r.cmd == Cmd_T.VERIFY_PIN_CODE)
            {
                // Verify account pin code
                Environment.Exit(0);
            } else if (r.cmd == Cmd_T.VERIFY_SMS_CODE)
            {
                // Verify SMS Code
                Environment.Exit(0);
            }

            this.Close();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            // TO-DO: do forgot password
        }
    }
}
