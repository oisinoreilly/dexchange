//EventSource Polyfill for IE
require('eventsource-polyfill');
import * as Store from '../store';
import * as backendTypes from '../backendTypes';
import * as notifications from './notificationsStore';

import {
    JsonServiceClient,
    ServerEventsClient,
    ErrorResponse,
    appendQueryString,
    ServerEventConnect,
    ServerEventJoin,
    ServerEventLeave,
    ServerEventUpdate,
    ServerEventMessage,
    ServerEventHeartbeat,
    ServerEventUser,
    SingletonInstanceResolver,
    GetEventSubscribers
} from 'servicestack-client';

const backendServerUrl = "http://localhost:5001";
//TODO: UNCOMMENT FOR DEPLOYMENT
//const backendServerUrl = "http://ec2-54-202-32-215.us-west-2.compute.amazonaws.com:5001";

const client = new JsonServiceClient(backendServerUrl);
const sseClient = new ServerEventsClient(backendServerUrl, ['*'], {
    handlers: {
       
        onConnect: (sub: ServerEventConnect) => {
           // debugger;
            // Successful SSE connection
            console.log("You've connected! welcome " + sub.displayName);
        },
        onJoin: (msg: ServerEventJoin) => {
            //debugger;
            // User has joined subscribed channel
            console.log("Welcome, " + msg.displayName);
        },
        onLeave: (msg: ServerEventLeave) => {
            //debugger;
            // User has left subscribed channel
            console.log(msg.displayName + " has left the building");
            // Terminate chat at back end.
            const request = new backendTypes.DocumentChatCancelSubscription();
            request.SubscriptionID = msg.displayName;
            client.put(request);
            // Need to notify here as well, we may need to store completed chat.
        },
        onUpdate: (msg: ServerEventUpdate) => {    // User channel subscription was changed
            console.log(msg.displayName + " channels subscription were updated");
        },
        onMessage: (msg: ServerEventMessage) => {
           // debugger;
            // Need to call method here as well to append to each chat.
            let msgData = JSON.parse(msg.json);

            // Now check if this is a notification or a chat message. 
             //Get a handle to the store from the window (global scope)
            let store = (window as any).store;
            if (msgData.UserName == null) {
                //Get a handle to the store from the window (global scope)
                //TODO: Do this with connect instead. Maybe we need to make this a component...
              
                store.dispatch({
                    type: 'ADD_CHAT_MESSAGE',
                    documentIndex: msgData.DocumentID,
                    fromUser: msgData.From,
                    chatMessage: msgData.Message
                });
            }
            else
            {  
                // Dispatch notification message so that structs can be updated.
                let documentstatus = null;
                let accountstatus = null;
                let corporatestatus = null;
                let bankstatus = null;
                let documentID = null;
                if (null != msgData.StatusUpdate.DocumentUpdates)
                {
                    documentID = msgData.StatusUpdate.DocumentUpdates[0].ID;
                    documentstatus = msgData.StatusUpdate.DocumentUpdates[0].Status;
                }
                let accountID = null;
                if (null != msgData.StatusUpdate.AccountUpdates) {
                    accountID = msgData.StatusUpdate.AccountUpdates[0].ID;
                    accountstatus = msgData.StatusUpdate.AccountUpdates[0].Status;
                }
                let corporateID = null;
                if (null != msgData.StatusUpdate.CorporateUpdates) {
                    corporateID = msgData.StatusUpdate.CorporateUpdates[0].ID;
                    corporatestatus = msgData.StatusUpdate.CorporateUpdates[0].Status;
                }

                let bankID = null;
                if (null != msgData.StatusUpdate.BankUpdates) {
                    bankID = msgData.StatusUpdate.BankUpdates[0].ID;
                    bankstatus = msgData.StatusUpdate.BankUpdates[0].Status;
                }
                else if (null != msgData.BankID)
                {
                    bankID = msgData.BankID;
                }


                let notification: notifications.Notification = {
                    id: msgData.ID, 
                    type: msgData.Type,
                    account: accountID,
                    document: documentID,
                    status: documentstatus,
                    bankname: msgData.BankName,
                    bankid: bankID,
                    corporateid: corporateID,
                    corporatename: msgData.CorporateName, 
                    username: msgData.UserName,
                    message: msgData.Message,
                    dateCreated: msgData.CreationDate,
                    processed: false
                };
                 
                store.dispatch({
                    type: 'ADD_NOTIFICATION',
                    notificationToAdd: notification
                });

                store.dispatch({
                    type: 'UPDATE_STATUS_MESSAGE',                   
                    account: accountID,
                    accountstatus: accountstatus,
                    document: documentID,
                    documentstatus: documentstatus,
                    corporate: corporateID,
                    corporatestatus: corporatestatus,
                    bank: bankID,
                    bankstatus: bankstatus
                  
                });
                
            }
        } // Invoked for each other message
    },
    onException: (e: Error) => { },                 // Invoked on each Error
    onReconnect: (e: Error) => { }                  // Invoked after each auto-reconnect
})
    //.addListener("theEvent", (e: ServerEventMessage) => { }) // Add listener for pub/sub event trigger
    .start();


//Middleware to handle all SSE event modification before sending to the reducer.
export const sseEventsMiddleware = store => next => action => {
    //Intercept all chat messages and redispatch as a chat message to be added to the UI.
    //if (action.type == "ADD_CHAT_MESSAGE"){
    //    let actionTyped = action as Banks.AddChatMessageAction;
    //    store.dispatch(actionTyped);
    //}
    console.log('dispatching', action)
    let result = next(action);
    return result;
}