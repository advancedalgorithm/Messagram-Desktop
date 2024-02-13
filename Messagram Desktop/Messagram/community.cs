using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Messagram_Desktop.Messagram
{
    public class community
    {
        public int community_id;
        public string community_server;
        public string[] messages;

        public community(int id, string name, string[] msg = null)
        {
            if(msg != null)
            {
                this.messages = msg;
            }
        }

        public void add_msg(string data)
        {
            this.messages.Append(data);
        }
    }
}
