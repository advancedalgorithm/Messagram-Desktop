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

namespace Messagram_Desktop.Messagram
{
    public class messagram
    {
        /* Client Application Information */
        public string CLIENT_NAME = "official_messagram_client_v.0.0.1";
        public string CLIENT_VERSION = "0.0.1";

        /* Client's Current User Information */
        public string Username;
        private string sessionID;
        private string HWID;

        /* Client's Current Opened Chat */
        private bool chat_opened; // this must be enabled in-order to 
        private bool dm; // dm on true, community chat on false
        private string chat_name; // chat_name to listen to

        /* Messagram Server Information & Connections */
        private string MESSAGRAM_BACKEND    = "50.114.177.31";
        private int MESSAGRAM_PORT          = 666;

        private TcpClient MessagramServer;
        private NetworkStream Messagram_IO;

        public bool terminate = false;

        public Thread listener;
        public messagram(string client_name, string client_v)
        {
            this.CLIENT_NAME = client_name;
            this.CLIENT_VERSION = client_v;
            this.retrieveHardwareInfo();
        }

        public void disableChat()
        { this.chat_opened = false; }

        public void ChangeChat(string chat_n, bool dm)
        {
            this.chat_opened = true;
            this.chat_name = chat_n;
            this.dm = dm;

            // Send CMD
        }

        private void retrieveHardwareInfo()
        {
            // Get Hardware Info And Addresses
        }

        /*
            Connecting & Authenicating User.

            Messagram server drops the connection when rejected
        */
        public messaResponse ConnectnAuthorize(string username, string password)
        {
            try {

                this.MessagramServer = new TcpClient(this.MESSAGRAM_BACKEND, this.MESSAGRAM_PORT);

                this.Messagram_IO = this.MessagramServer.GetStream();

                /* Request Login API Endpoint For Session ID */
                //this.sessionID = new WebClient().DownloadString($"https://api.messagram.io/auth?username={username}&password={password}&hwid={this.HWID}");

                /* Invalid Login Information Provided */
                //if (this.sessionID == String.Empty)
                //    return (new messaResponse("", true, Resp_T.NULL, Cmd_T.INVALID_LOGIN_INFO));

                /* BUILD AUTH COMMAND */
                string[] cmd_info = { username, this.sessionID, this.HWID, this.CLIENT_NAME, $"{this.CLIENT_VERSION}" };
                messaResponse newCMD = this.BuildMessagramCmd(Cmd_T.CLIENT_AUTHENICATION, cmd_info);

                /* Send Auth Information and Receive Auth Response */
                messaResponse r = this.SendCmd(newCMD);
                Resp_T rtype = r.resp_t;

                /* Detect for bans and//or socket rejections */
                switch (rtype)
                {
                    case Resp_T.DEVICE_BANNED:
                        return (new messaResponse("This device has been banned from Messagram's Network!", true, Resp_T.DEVICE_BANNED, Cmd_T.NULL));
                    case Resp_T.SOCKET_REJECTED:
                        return (new messaResponse("Messagram Server has rejected the connection!", true, Resp_T.SOCKET_REJECTED, Cmd_T.NULL));
                }

                // RUN ACCOUNT INFORMATION PARSER
                // parse_account_info(); // RECEIVE ALL DMS, COMMUNITES, and ACCOUNT SETTINGS

                /* Start listener */
                this.listener = new Thread(Listener);
                this.listener.Start();

                /* Return a Success Signal For Developers */
                return (new messaResponse("Client connected to Messagram Server", true, Resp_T.SOCKET_CONNECTED, Cmd_T.SUCCESSFUL_LOGIN));
            } catch
            {
                return (new messaResponse("Invalid connection", true, Resp_T.INVALID_CONNECTION, Cmd_T.NULL));
            }

        }

        /*
         * Listens to random events sent to the CLIENT upon connecting
         * 
         * Server Resp_T: PUSH_EVENTS && MASS_EVENTS are caught in HERE!
         * User Command Request Resp_T are usually USER_RESP
        */
        public void Listener()
        {
            while(true)
            {
                if (terminate)
                    Environment.Exit(0);

                Byte[] data = new Byte[256];
                Int32 bytes = this.Messagram_IO.Read(data, 0, data.Length);
                string server_data = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

                messaResponse r = new messaResponse(server_data);
                Resp_T res_t = r.resp_t;

                if (res_t == Resp_T.NULL)
                    continue;

                switch(res_t)
                {
                    case Resp_T.PUSH_EVENT:
                        // do something
                        break;
                    case Resp_T.MASS_EVENT:
                        // do something
                        break;
                }
            }
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
        public void handle_mass_events()
        {

        }

        /*
         *  This takes a new Built messaResponse() Class with data to send
         *  
         *  and generate a new messaResponse() Class with data received
         */
        public messaResponse SendCmd(messaResponse r)
        {
            Byte[] data = System.Text.Encoding.ASCII.GetBytes($"{r.data}\n");
            this.Messagram_IO.Write(data, 0, data.Length);

            data = new Byte[256];

            Int32 bytes = this.Messagram_IO.Read(data, 0, data.Length);
            string resp = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            return (new messaResponse(resp, false));
        }

        public messaResponse BuildMessagramCmd(Cmd_T c, string[] args = null)
        {
            return (new messaResponse("", true, Resp_T.NULL, c, args));
        }
    }
}
