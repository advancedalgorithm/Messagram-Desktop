using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messagram_Desktop.Messagram
{
    public enum Msg_T
    {
        NULL,
        DM,
        COMMUNITY,
    }
    public class MessagramMessage
    {
        public string from_username;
        public string to_username;
        public string data;

        public MessagramMessage(string from_username, string to_username, string data)
        {
            this.from_username = from_username;
            this.to_username = to_username;
            this.data = data;
        }

        public override string ToString()
        {
            return $"{from_username}: {data}";
        }
    }
}
