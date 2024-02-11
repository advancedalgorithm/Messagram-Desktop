using Messagram_Desktop.Messagram;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Messagram_Desktop
{
    public partial class Main : Form
    {
        messagram m;
        public Main()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SignIn login = new SignIn();
            login.ShowDialog();


        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            m.terminate = true;
        }
    }
}
