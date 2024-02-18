using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Messagram_Desktop.Messagram
{

    public class messaResponse
    {
        /*
         * keys that the JSON always need 
         */

        /* JSON Keys to Receive */
        public bool status;
        public string resp;
        public Resp_T resp_t;


        /* JSON TO SEND */
        public Cmd_T cmd;
        public string cmd__;
        public string username;
        public string from_username;
        public string to_username;
        public string to_community;
        public string data;

        public string[] raw_json_info;
        public string raw_json;
        public string msg;
        public string[] info;

        public messaResponse(string d, bool build = false, Resp_T r = Resp_T.NULL, Cmd_T c = Cmd_T.NULL, string[] info = null, string[] args = null)
        {
            this.data = d.Trim();
            /* Build a new Cmd for Server */
            if(build)
            {
                this.cmd = c;
                this.resp_t = r;
                this.info = info;
                return;
            }

            this.customJSONParse();
        }

        public override string ToString()
        {
            return $"{this.data}";
        }

        /*
         * 
         * 
         *      PARSING SERVER COMMAND RESPONSES BELOW
         * 
         * 
         */

        public void customJSONParse()
        {
            /* 
             * Only key/value JSON structures only, no extra structures
             * 
             * 2 Types Of JSON can be received here: Cmd Response && Mass or Single Messagram User Events
             * 
             * For more information read: https://github.com/advancedalgorithm/Messagram/blob/main/api.md
            */

            this.raw_json_info = this.data.Replace("{", "").Replace("}", "").Replace(": ", ":").Replace("\"", "").Replace("'", "").Split(',');

            if(this.data.Trim().Contains("\n"))
                this.raw_json_info = this.data.Replace("{", "").Replace("}", "").Replace(": ", ":").Replace("\"", "").Replace("'", "").Split('\n');

            /* Retrieving and setting Status // This will always be false if never true */
            string status = this.raw_json_info[0].Replace("status:", "").Replace(" ", ""); // This should always be true/false
            if (status == "true") this.status = bool.Parse(status);


            /* /* Set Resp_T && Cmd_T // resp_t Should never have spaces */
            if (this.raw_json_info.Count() < 3) return;
            this.resp_t = objects.resp2type(this.raw_json_info[1].Replace("resp_t:", "").Trim());
            this.cmd = objects.cmd2type(this.raw_json_info[2].Replace("cmd_t:", "").Replace(" ", "").Trim());

            if (this.data.Contains("\"data\":") || this.data.Contains("'data':"))
            {
                string[] json_info = this.data.Replace("{", "").Replace("}", "").Replace("\", \"", "\\").Replace("', '", "\\").Replace("\"", "").Replace("'", "").Split('\\');
                this.from_username = this.raw_json_info[3].Replace("from_username:", "").Trim(); ;
                this.to_username = this.raw_json_info[4].Replace("to_username:", "").Trim();
                this.msg = json_info[json_info.Count() - 1].Replace("data:", "").Trim();
            }
        }

        /*
         * 
         * 
         *          COMMAND BUILDING FUNCTIONS BELOW
         * 
         * 
         * 
         */

        public void BuildCmd(string[] args)
        {
            // Only Accepting the following array formats
            /*
            	"cmd_t":"client_authentication",
	            "username":"Jeff",
	            "sid":"70144081426107300636451535365233",
	            "hwid":"{6d9c0696-ba90-11ee-94f1-806e6f6e6963}",
	            "client_name":"official_client",
	            "client_version":"0.0.1"
            */

            /* Formatting the beginning of the JSON data */
            string _new = "{\"cmd_t\":\"" + $"{this.cmd}".ToLower() + "\", \"username\":\"{NEW_USERNAME}\", \"sid\":\"{NEW_SID}\", \"hwid\":\"{NEW_HWID}\", \"client_name\":\"{NEW_CLIENT}\", \"client_version\":\"{NEW_CVERSION}\", ";
            _new = _new.Replace("{NEW_USERNAME}", this.info[0]).Replace("{NEW_SID}", this.info[1]).Replace("{NEW_HWID}", this.info[2]).Replace("{NEW_CLIENT}", this.info[3]).Replace("{NEW_CVERSION}", this.info[4]);


            if (args.Length == 0) 
            { 
                this.data += _new + "}".Replace(",}", "").Replace(", }", "");
                return;
            }

            switch (this.cmd)
            {
                case Cmd_T.SEND_FRIEND_REQ:
                    _new += build_friend_req_cmd_key(args);
                    break;
                    
                case Cmd_T.SEND_DM_MSG:

                    _new += build_send_dm_msg_cmd_key(args);
                    break;
            }

            _new += "}";
            this.data += _new.Replace(", }", "}"); // Should never end like this unless its used for CLIENT_AUTHENTICATION
        }
        
        public string build_friend_req_cmd_key(string[] args)
        {
            if(args.Length == 1)
                return $"\to_username\": \"{args[0]}\"";

            this.cmd = Cmd_T.INVALID_PARAMETERS;

            return "";
        }



        public string build_send_dm_msg_cmd_key(string[] args)
        {
            // Assuming the arrow is in order
            // username, sessionID, hwid, to_username, data, client_name, client_version
            if(args.Length == 3)
                return $"\"from_username\": \"{args[0]}\", \"to_username\": \"{args[1]}\", \"data\": \"{args[2]}\", ";

            this.cmd = Cmd_T.INVALID_PARAMETERS;
            return "";
        }
    }
}
