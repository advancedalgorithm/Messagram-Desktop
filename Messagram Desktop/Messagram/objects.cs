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
         * !!! The Server will not respond with a neat message to output for users !!!
         */


        /* REQUEST ERRORS */
        INVALID_CMD,
        INVALID_PARAMETERS,                 // INVALID PARAMETERS SENT TO SERVER
        INVALID_PERM,                       // INVALID PERMS (USED FOR COMMUNITY ROLES ETCS)
        INVALID_OPERATION,                  // A reason will be sent why. This is more for the developer using the API

        /* 
         * LOGIN CMDS FOR SERVER 
        */
        CLIENT_AUTHENTICATION,               // Sending login information


        /* 
         * LOGIN CMD RESPONSES FROM SERVER 
        */
        ADD_SMS_AUTH,                       // SEND NEW PHONE NUMBER FOR VERIFICATION
        ADD_NEW_EMAIL,                      // CHANGE CURRENT EMAIL FOR VERIFICATION
        SEND_PIN_VERIFICATION_CODE,         // SEND PIN VERIFICATION CODE
        SEND_SMS_VERIFICATION_CODE,         // SEND SMS VERIFICATION CODE
        SEND_EMAIL_VERIFICATION_CODE,       // SEND EMAIL VERIFICATION CODE

        /* SUCCESS OPERATIONS */
        SUCCESSFUL_LOGIN,                   // LOGIN SUCCESSFUL
        NEW_EMAIL_ADDED,                    // NEW EMAIL HAS BEEN ADDED
        TRUST_CONFIRMED,                    // EMAIL HAS BEEN VERIFIED
        SMS_VERIFIED,                       // SMS CODE HAS BEEN VERIFIED
        PIN_VERIFIED,                       // PIN CODE HAS BEEN VERIFIED
        NUMBER_ADDED,                       // PHONE NUMBER HAS BEEN ADDED & SMS SENT

        /* FAIL OPERATIONS */
        INVALID_LOGIN_INFO,                 // INVALID USERNAME OR PASSWORD
        ACCOUNT_PERM_BAN,                   // ACCOUNT PERM BANNED
        ACCOUNT_TEMP_BAN,                   // ACCOUNT TEMP BANNED
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

        /* OPERATION RESPONSES */
        DM_SENT,                            // DM SUCCESSFULLY SENT
        DM_FAILED,                          // DM FAILED

        /*
         * COMMUNITY CMD FOR SERVER
        */
        /* FAILED OPERATIONS */
        INVALID_ROLE_PERMS,                 // ROLE DOES NOT CONTAIN PERMISSION FOR OPERATION REQUESTED

        /* COMMUNITY CREATION & SETTING EDITING */
        CREATE_COMMUNITY,                   // CREATE A COMMUNITY (LIKE A DISCORD SERVER)
        EDIT_COMMUNITY,                     // Edit Community Info/Settings (EDIT A COMMUNITY SETTINGS OR INFO)
        INVO_TOGGLE,                        // Enable/Disable Community Invites (EDIT THE INVITE TOGGLE)
        KICK_USER,                          // Kick a user from the community
        BAN_USER,                           // Ban a user from the community
        DEL_MSG,                            // Delete a message from the community chat


        /* Roles */
        CREATE_COMMUNITY_ROLE,              // CREATE A ROLE
        EDIT_COMMUNITY_ROLES,               // EDIT A ROLE (Perms, Color, Rank Level)
        DEL_COMMUNITY_ROLE,                 // DELETE A ROLE
        
        /* Chats */
        CREATE_COMMUNITY_CHAT,              // CREATE A NEW CHAT
        EDIT_COMMUNITY_CHAT,                // EDIT THE CHAT SETTINGS (Perms, Name, Desc)
        DEL_COMMUNITY_CHAT,                 // DELETE THE CHAT

        /*
         * 
         *          EVENT NOTIFICATIONS
         * 
         * Commands from server, Actions from/by other users
         * 
         * Mainly used for receiving commands from server which is
         * usually when a user requests or msg another user
         */

        ACCOUNT_BAN,                        // ACCOUNT BANNED
        FRIEND_REQ_RECEIVED,                // USER FRIEND REQUEST HAS BEEN RECEIVED
        DM_MSG_RECEIVED,                    // USER DM MESSAGE HAS BEEN RECEIVED
        COMMUNITY_MSG_RECEIVED              // COMMUNITY MESSAGED RECEIVED
    }

    public class objects
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


        public static Cmd_T cmd2type(string cmd)
        {
            switch(cmd)
            {
                /* Upon Connecting Cmd Responses */
                case "invalid_cmd":
                    return Cmd_T.INVALID_CMD;
                case "invalid_parameters":
                    return Cmd_T.INVALID_PARAMETERS;
                case "invalid_perm":
                    return Cmd_T.INVALID_PERM;
                case "invalid_operation":
                    return Cmd_T.INVALID_OPERATION;

                    /* Login Cmd Responses */
                case "client_authentication":
                    return Cmd_T.CLIENT_AUTHENTICATION;
                case "successful_login":
                    return Cmd_T.SUCCESSFUL_LOGIN;
                case "add_sms_auth":
                    return Cmd_T.ADD_SMS_AUTH;
                case "add_new_email":
                    return Cmd_T.ADD_NEW_EMAIL;
                case "send_pin_verification_code":
                    return Cmd_T.SEND_PIN_VERIFICATION_CODE;
                case "send_sms_verification_code":
                    return Cmd_T.SEND_SMS_VERIFICATION_CODE;
                case "send_email_verification_code":
                    return Cmd_T.SEND_EMAIL_VERIFICATION_CODE;
                case "new_email_added":
                    return Cmd_T.NEW_EMAIL_ADDED;
                case "trust_confirmed":
                    return Cmd_T.TRUST_CONFIRMED;
                case "sms_verified":
                    return Cmd_T.SMS_VERIFIED;
                case "pin_verified":
                    return Cmd_T.PIN_VERIFIED;
                case "number_added":
                    return Cmd_T.NUMBER_ADDED;
                case "invalid_login_info":
                    return Cmd_T.INVALID_LOGIN_INFO;
                case "account_perm_ban":
                    return Cmd_T.ACCOUNT_PERM_BAN;
                case "account_temp_ban":
                    return Cmd_T.ACCOUNT_TEMP_BAN;
                case "force_confirm_email":
                    return Cmd_T.FORCE_CONFIRM_EMAIL;
                case "force_device_trust":
                    return Cmd_T.FORCE_DEVICE_TRUST;
                case "force_add_phone_number_request":
                    return Cmd_T.FORCE_ADD_PHONE_NUMBER_REQUEST;
                case "verify_pin_code":
                    return Cmd_T.VERIFY_PIN_CODE;
                case "verify_sms_code":
                    return Cmd_T.VERIFY_SMS_CODE;

                /* User Friend Request Cmd Responses */
                case "send_friend_req":
                    return Cmd_T.SEND_FRIEND_REQ;
                case "cancel_friend_req":
                    return Cmd_T.CANCEL_FRIEND_REQ;
                case "friend_request_sent":
                    return Cmd_T.FRIEND_REQUEST_SENT;
                case "failed_to_send_friend_request":
                    return Cmd_T.FAILED_TO_SEND_FRIEND_REQUEST;
                case "blocked_by_user":
                    return Cmd_T.BLOCKED_BY_USER;

                /* User Dm Cmd Responses */
                case "send_dm_msg":
                    return Cmd_T.SEND_DM_MSG;
                case "send_dm_msg_rm":
                    return Cmd_T.SEND_DM_MSG_RM;
                case "send_dm_reaction":
                    return Cmd_T.SEND_DM_RACTION;
                case "dm_sent":
                    return Cmd_T.DM_SENT;
                case "dm_failed":
                    return Cmd_T.DM_FAILED;
                case "dm_msg_received":
                    return Cmd_T.DM_MSG_RECEIVED;
            }
            return Cmd_T.NULL;
        }
    }
}
