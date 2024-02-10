using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Messagram_Desktop.Messagram
{

    public class messaResponse
    {
        public string serverData;

        public bool status;
        public Resp_T resp_t;
        public Cmd_T cmd;
        public string data;

        public messaResponse(string d, bool build = false, Resp_T r = Resp_T.NULL, Cmd_T c = Cmd_T.NULL, string[] args = null)
        {
            /* Build a new Cmd for Server */
            if(build)
            {
                this.cmd = c;
                this.resp_t = r;
                this.data = this.BuildCmd(args);
                return;
            }
            
            /* This will always fall in here when a response a received from server */
            if (!d.StartsWith("{") && !d.StartsWith("}"))
            {
                this.resp_t = Resp_T.NULL;
                return;
            }

            this.serverData = d;
            this.customJSONParse();
        }

        public void customJSONParse()
        {
            /* 
             * Only key/value JSON structures only, no extra structures
             * 
             * 2 Types Of JSON can be received here: Cmd Response && Mass or Single Messagram User Events
             * 
             * For more information read: https://github.com/advancedalgorithm/Messagram/blob/main/api.md
            */

            string[] info = this.serverData.Replace("{\"", "").Replace("\"}", "").Replace("\": \"", ":").Split(',');

            string status = info[0].Replace("status:", "").Replace(" ", ""); // This should always be true/false

            if (status == "true")
                this.status = bool.Parse(status);
            else
                return;

            /* resp_t Should never have spaces */
            this.resp_t = Response.resp2type(info[1].Replace("resp_t:", "").Replace(" ", ""));

            /* NOTHING TO DO HERE */
            if (this.resp_t == Resp_T.NULL)
                return;

            Resp_T[] user_cmds = { Resp_T.USER_RESP,
                                   Resp_T.INVALID_CONNECTION,
                                   Resp_T.SOCKET_REJECTED };

            if(user_cmds.Contains(this.resp_t))
            {
                this.data = info[2].Replace("data:", "");
            } else if(this.resp_t == Resp_T.PUSH_EVENT || this.resp_t == Resp_T.MASS_EVENT)
            {
                //this.cmd = info[2].Replace("cmd:", "").Replace(" ", "");
                this.data = info[3].Replace("data:", "");
            }
        }

        public string BuildCmd(string[] args)
        {
            string _new = "{";
            switch(this.cmd)
            {
                case Cmd_T.CLIENT_AUTHENICATION:
                    // Only Accepting the following array format
                    // username, sessionID, hwid, client_name, client_version
                    if (args.Length != 5)
                    {
                        this.cmd = Cmd_T.INVALID_PARAMETERS;
                        return "Invalid parameters";
                    }

                    _new += build_client_auth_keys(args);
                    break;

                case Cmd_T.SEND_FRIEND_REQ:
                    // Only Accepting the following array format
                    // username, sessionID, hwid, to_username, client_name, client_version
                    if(args.Length != 6)
                    {
                        this.cmd = Cmd_T.INVALID_PARAMETERS;
                        return "Invalid parameters";
                    }

                    _new += build_friend_req_cmd_key(args);
                    break;

                    // FILL THE REST HERE
                case Cmd_T.SEND_DM_MSG:
                    // Only Accepting the following array format
                    // username, sessionID, hwid, to_username, data, client_name, client_version
                    if (args.Length != 7)
                    {
                        this.cmd = Cmd_T.INVALID_PARAMETERS;
                        return "Invalid parameters";
                    }

                    _new += build_send_dm_msg_cmd_key(args);
                    break;
            }

            _new += "}";
            return _new;
        }

        public string build_client_auth_keys(string[] args)
        {
            string json_keys = "\"cmd\": \"client_authenication\", ";

            // Assuming the arrow is in order
            int c = 0;
            foreach (string arg in args)
            {
                switch (c)
                {
                    case 0:
                        json_keys += $"\"username\": \"{arg}\", ";
                        break;
                    case 1:
                        json_keys += $"\"sessionID\": \"{arg}\", ";
                        break;
                    case 2:
                        json_keys += $"\"hwid\": \"{arg}\", ";
                        break;
                    case 3:
                        json_keys += $"\"client_name\": \"{arg}\", ";
                        break;
                    case 4:
                        json_keys += $"\"client_version\": \"{arg}\" ";
                        break;
                }
                c++;
            }

            return json_keys;
        }

        public string build_friend_req_cmd_key(string[] args)
        {

            string json_keys = "\"cmd\": \"user_friend_request\", ";

            // Assuming the arrow is in order
            int c = 0;
            foreach (string arg in args)
            {
                switch (c)
                {
                    case 0:
                        json_keys += $"\"username\": \"{arg}\", ";
                        break;
                    case 1:
                        json_keys += $"\"sessionID\": \"{arg}\", ";
                        break;
                    case 2:
                        json_keys += $"\"hwid\": \"{arg}\", ";
                        break;
                    case 3:
                        json_keys += $"\to_username\": \"{arg}\", ";
                        break;
                    case 4:
                        json_keys += $"\"client_name\": \"{arg}\", ";
                        break;
                    case 5:
                        json_keys += $"\"client_version\": \"{arg}\" ";
                        break;
                }
                c++;
            }

            return json_keys;
        }



        public string build_send_dm_msg_cmd_key(string[] args)
        {

            string json_keys = "\"cmd\": \"send_dm_msg\", ";

            // Assuming the arrow is in order
            int c = 0;
            foreach (string arg in args)
            {
                switch (c)
                {
                    case 0:
                        json_keys += $"\"username\": \"{arg}\", ";
                        break;
                    case 1:
                        json_keys += $"\"sessionID\": \"{arg}\", ";
                        break;
                    case 2:
                        json_keys += $"\"hwid\": \"{arg}\", ";
                        break;
                    case 3:
                        json_keys += $"\to_username\": \"{arg}\", ";
                        break;
                    case 4:
                        json_keys += $"\"data\": \"{arg}\", ";
                        break;
                    case 5:
                        json_keys += $"\"client_name\": \"{arg}\", ";
                        break;
                    case 6:
                        json_keys += $"\"client_version\": \"{arg}\" ";
                        break;
                }
                c++;
            }

            return json_keys;
        }
    }
}
