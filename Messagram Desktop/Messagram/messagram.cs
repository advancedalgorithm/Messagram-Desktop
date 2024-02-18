using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Runtime.Remoting.Messaging;
using System.Diagnostics.Eventing.Reader;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Security.Cryptography;
using System.Management;
using Microsoft.Win32;

namespace Messagram_Desktop.Messagram
{
    /*
     * 
     *          The Official Messagram Client Library
     * 
     * Quick Start Example:
     * 
     * Form Open:
     * 
     *     - Set a Messagram Property For The Main Client Form
     *     - Add a Messagram Function Argument to the Form's Constructor
     *     
     *          messagram m = new messagram("CLIENT_NAME", "CLIENT_VERSION");
     *          
     * Upon Login: 
     *          
     *          messaResponse r = m.ConnectnAuthorize(textBox1.Text, textBox2.Text);
     *          if (r.resp_t != Resp_T.SOCKET_CONNECTED)
     *          {
     *              MessageBox.Show("Messagram server is down!");
     *              Environment.Exit(0);
     *          } else if(r.cmd == Cmd_T.SUCCESSFUL_LOGIN)
     *          {
     *              MessageBox.Show($"Welcome {username} to Messagram!");
     *              ClientForm c = new ClientForm(m) // YOUR CLIENT FORM HERE
     *              this.close();
     *              c.ShowDialog();
     *          }
     */
    public class messagram
    {
        /* Client Application Information */
        public string CLIENT_NAME = "official_messagram_client_v.0.0.1";
        public string CLIENT_VERSION = "0.0.1";

        /* Client's Current User Information */
        public string Username;
        public string sessionID;
        public string HWID;

        /* Client's Current Opened Chat */
        public bool seen;
        public bool listen_to_chat;
        public string listen_to;
        public Msg_T currentChat_T;
        public MessagramMessage CurrentChats;

        /* Messagram Server Information & Connections */
        private string MESSAGRAM_BACKEND    = "195.133.52.252";
        private int MESSAGRAM_PORT          = 666;

        public TcpClient MessagramServer;
        public NetworkStream Messagram_IO;

        public bool terminate = false;

        public Thread listener;

        public string ServerLogs = string.Empty;

        public string Messages = string.Empty;
        public messagram(string client_name, string client_v)
        {
            this.CLIENT_NAME = client_name;
            this.CLIENT_VERSION = client_v;
            this.retrieveHardwareInfo();
        }

        public void disableChat()
        { this.listen_to_chat = false; this.currentChat_T = Msg_T.NULL; this.listen_to = string.Empty; }

        public void ChangeChat(Msg_T c, string username_or_com)
        { this.listen_to_chat = true; this.currentChat_T = c; this.listen_to = username_or_com; }
        
        public string get_logs() { return this.ServerLogs; }

        private void retrieveHardwareInfo()
        {
            // Get Hardware Info And Addresses
            this.HWID = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\IDConfigDB\Hardware Profiles\0001", "HwProfileGuid", null).ToString();
        }

        public string[] getInfo()
        {
            return new string[] { this.Username, this.sessionID, this.HWID, this.CLIENT_NAME, $"{this.CLIENT_VERSION}" };
        }

        /*
            Connecting & Authenicating User.

            Messagram server drops the connection when rejected
        */
        public messaResponse ConnectnAuthorize(string username, string password)
        {
            try {
                this.retrieveHardwareInfo();

                this.MessagramServer = new TcpClient(this.MESSAGRAM_BACKEND, this.MESSAGRAM_PORT);

                this.Messagram_IO = this.MessagramServer.GetStream();
                this.ServerLogs += $"{this.Messagram_IO.ReadTimeout}";

                /* Request Login API Endpoint For Session ID */
                this.sessionID = new WebClient().DownloadString($"https://api.yomarket.info/auth?username={username}&password={password}&hwid={this.HWID}");

                /* Invalid Login Information Provided */
                if (this.sessionID == String.Empty)
                    return (new messaResponse("Unable to connect to Messagram....!", true, Resp_T.INVALID_CONNECTION, Cmd_T.NULL));

                /* BUILD AUTH COMMAND */
                this.Username = username;
                messaResponse newCMD = this.BuildMessagramCmd(Cmd_T.CLIENT_AUTHENTICATION);
                newCMD.BuildCmd(new string[] { });

                /* Send Auth Information and Receive Auth Response */
                this.SendCmd(newCMD);


                // RUN ACCOUNT INFORMATION PARSER
                // parse_account_info(); // RECEIVE ALL DMS, COMMUNITES, and ACCOUNT SETTINGS

                /* Start listener */
                this.listener = new Thread(Listener);
                this.listener.Start();

                /* Return a Success Signal For Developers */
                return (new messaResponse("Client connected to Messagram Server", true, Resp_T.SOCKET_CONNECTED, Cmd_T.SUCCESSFUL_LOGIN));
            } catch(Exception e)
            {
                MessageBox.Show($"{e}");
                return (new messaResponse("Invalid connection", true, Resp_T.INVALID_CONNECTION, Cmd_T.NULL));
            }
        }

        public void die()
        {
            this.MessagramServer.Close();
            this.Messagram_IO.Close();
            this.listener.Abort();
        }

        public string getLastMessage()
        {
            string[] messages_arr = this.Messages.Split('\n');
            if (!this.seen)
            {
                this.seen = true;
                return messages_arr[messages_arr.Count() - 1];
            }

            return "";
        }

        /*
         * Listens to random events sent to the CLIENT upon connecting
         * 
         * Server Resp_T: PUSH_EVENTS && MASS_EVENTS are caught in HERE!
         * User Command Request Resp_T are usually USER_RESP
        */
        public void Listener()
        {
            //try
            //{
                while (true)
                {
                    if (terminate)
                        Environment.Exit(0);

                    Byte[] data = new Byte[256];
                    Int32 bytes = this.Messagram_IO.Read(data, 0, data.Length);
                    string server_data = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

                    messaResponse r = new messaResponse(server_data);

                    if (r.cmd == Cmd_T.NULL)
                        continue;

                    this.ServerLogs += $"[@RECEIVED-FROM-SERVER]\n{server_data}\n\n";

                    switch (r.resp_t)
                        {
                        case Resp_T.USER_RESP:
                            if (r.cmd == Cmd_T.SEND_DM_MSG || r.cmd == Cmd_T.DM_MSG_RECEIVED)
                            {
                                string str = $"{r.from_username}: {r.msg}";
                                this.Messages += $"{str}\n";
                                this.ServerLogs += $"Messages @ {this.Messages.Count()}\n";
                            }
                            continue;
                        case Resp_T.PUSH_EVENT:
                            this.handle_push_events(r);
                            continue;
                        case Resp_T.MASS_EVENT:
                            this.handle_mass_events(r);
                            // do something
                            continue;
                        default:
                            break;
                    }
                }
            //} catch(Exception e)
            //{
            //    MessageBox.Show("Messagram Server has went down!");
            //    //Environment.Exit(0);
            //}
        }

        /* HANDLE PUSH EVENTS (Force Client To Lock, Ban, Modify Restrictions Etc */
        public void handle_push_events(messaResponse r)
        {
            Cmd_T cmd = r.cmd;

            //switch(cmd)
            //{
            //    case "force_update_app":
            //        update_app();
            //        break;
            //}
        }

        /* HANDLE MASS EVENTS (Daily/Weekly/Monthly Update Announcements, Community Chats) */
        public void handle_mass_events(messaResponse r)
        {

        }

        /*
         *  This takes a new Built messaResponse() Class with data to send
         *  
         *  and generate a new messaResponse() Class with data received
         */
        public void SendCmd(messaResponse r)
        {
            this.ServerLogs += $"[@SEND-TO-SERVER]\n${r.data}\n\n";
            Byte[] data = System.Text.Encoding.ASCII.GetBytes($"{r.data}\n".Replace(", }", "}"));
            this.Messagram_IO.Write(data, 0, data.Length);
        }

        public messaResponse BuildMessagramCmd(Cmd_T c, string[] args = null)
        {
            return (new messaResponse("", 
                                    true, 
                                    Resp_T.NULL, 
                                    c,
                                    new string[] { this.Username, this.sessionID, this.HWID, this.CLIENT_NAME, $"{this.CLIENT_VERSION}" }, args));
        }
    }
}
