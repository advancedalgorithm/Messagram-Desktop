using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Messagram_Desktop.Messagram
{

    public enum Resp_T
    {
        NULL,

        /* CONNECTION SUCCESS */
        SOCKET_CONNECTED,

        /* CONNECTION FAILURE */
        INVALID_CONNECTION,
        SOCKET_REJECTED,
        DEVICE_BANNED,

        /* ACTION EVENTS */
        USER_RESP,
        PUSH_EVENT,
        MASS_EVENT,
    }

    public enum Cmd_T
    {
        NULL,
        /*
         *                  SERVER COMMANDS & SERVER COMMAND RESPONSES
         * 
         * The developer using this client library only needs to know what these objects
         * are used for in-order to build a better response message for the user on client
         * 
         * !!! Server response message is NOT for USER output !!!
         */


        /* REQUEST ERRORS */
        INVALID_CMD,
        INVALID_PARAMETERS,
        INVALID_OPERATION,                  // A reason will be sent why. This is more for the developer using the API

        /* 
         * LOGIN CMDS FOR SERVER 
        */
        CLIENT_AUTHENICATION,               // Sending login information


        /* 
         * LOGIN CMD RESPONSES FROM SERVER 
        */

        /* SUCCESS OPERATIONS */
        SUCCESSFUL_LOGIN,                   // LOGIN SUCCESSFUL
        NEW_EMAIL_ADDED,                    // NEW EMAIL HAS BEEN ADDED
        TRUST_CONFIRMED,                    // EMAIL HAS BEEN VERIFIED
        SMS_VERIFIED,                       // SMS CODE HAS BEEN VERIFIED
        PIN_VERIFIED,                       // PIN CODE HAS BEEN VERIFIED
        NUMBER_ADDED,                       // PHONE NUMBER HAS BEEN ADDED & SMS SENT

        /* FAIL OPERATIONS */
        INVALID_LOGIN_INFO,                 // INVALID USERNAME OR PASSWORD
        FORCE_CONFIRM_EMAIL,                // FORCE USER TO VERIFY EMAIL TO USE THE ACCOUNT
        FORCE_DEVICE_TRUST,                 // UNKNOWN DEVICE, TRUST CONFIRMATION EMAIL SENT
        FORCE_ADD_PHONE_NUMBER_REQUEST,     // FORCE USER TO ADD A PHONE NUMBER
        VERIFY_PIN_CODE,                    // REQUEST USER FOR PIN CODE
        VERIFY_SMS_CODE,                    // REQUEST USER FOR SMS CODE

        /*
         * FRIEND REQUEST CMD FOR SERVER
         * 
         * Due to signals in the response type with a small message
         * objects is not needed for server response. 
         * 
         * The client will only be receiving the following CMD Types on failure:
         *      - INVALID_CMD
         *      - INVALID_PARAMETERS
         *      - INVALID_OPERATION
         *      
         * A better message can be built for response on failure
         */
        SEND_FRIEND_REQ,                    // SEND A FRIEND REQUEST
        CANCEL_FRIEND_REQ,                  // CANCEL A FRIEND REQUEST

        /* SUCCESS OPERATIONS */
        FRIEND_REQUEST_SENT,                // FRIEND REQUEST SENT
        
        /* FAILED OPERATIONS */
        FAILED_TO_SEND_FRIEND_REQUEST,      // FAILED TO SEND FRIEND REQUEST
        BLOCKED_BY_USER,                    // FRIEND REQUESTED USER HAS THE CURRENT CLIENT USER BLOCKED

        /* 
         * DM CMD FOR SERVER 
         * 
         * Due to signals in the response type with a small message
         * objects is not needed for server response. 
         * 
         * The client will only be receiving the following CMD Types on failure:
         *      - INVALID_CMD
         *      - INVALID_PARAMETERS
         *      - INVALID_OPERATION
         *      
         * A better message can be built for response on failure
        */
        SEND_DM_MSG,                        // SEND DM MESSAGE
        SEND_DM_MSG_RM,                     // SEND DM MESSAGE REMOVAL
        SEND_DM_RACTION,                    // SEND DM REACTION
        SEND_DM_REACTION_RM,                // SEND DM REACTION REMOVAL

        /*
         * COMMUNITY CMD FOR SERVER
        */
        CREATE_COMMUNITY,                   // CREATE A COMMUNITY (LIKE A DISCORD SERVER)
        EDIT_COMMUNITY,                     // Edit Community Info/Settings (EDIT A COMMUNITY SETTINGS OR INFO)
        INVO_TOGGLE,                        // Enable/Disable Community Invites (EDIT THE INVITE TOGGLE)

        /* Roles */
        CREATE_COMMUNITY_ROLE,              // CREATE A ROLE
        EDIT_COMMUNITY_ROLES,               // EDIT A ROLE (Perms, Color, Rank Level)
        DEL_COMMUNITY_ROLE,                 // DELETE A ROLE
        
        /* Chats */
        CREATE_COMMUNITY_CHAT,              // CREATE A NEW CHAT
        EDIT_COMMUNITY_CHAT,                // EDIT THE CHAT SETTINGS (Perms, Name, Desc)
        DEL_COMMUNITY_CHAT                  // DELETE THE CHAT
    }

    public class Response
    {
        public static Resp_T resp2type(string resp)
        {
            switch(resp)
            {
                case "user_resp":
                    return Resp_T.USER_RESP;
                case "push_event":
                    return Resp_T.PUSH_EVENT;
                case "mass_event":
                    return Resp_T.MASS_EVENT;
                case "socket_rejected":
                    return Resp_T.SOCKET_REJECTED;
                case "device_banned":
                    return Resp_T.DEVICE_BANNED;
            }

            return Resp_T.NULL;
        }
    }
}
