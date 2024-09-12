import { BackendClientSingleton } from '../BackendClientSingleton';
import * as backendTypes from '../backendTypes';
import * as Immutable from "seamless-immutable";
export class Notification {
}
//The state that this store exposes
export class NotificationsState {
}
export const actionCreators = {
    requestNotificationsForCorporate: (corporateID) => (dispatch, getState) => {
        // Specify corporate ID in request.
        const request = new backendTypes.CorporateNotifications();
        request.CorporateID = corporateID;
        // Get maximum of 20 notifications.
        request.MaximumCount = 10;
        // OOR TODO: Use current user, rather than user passed in. This user is the user set when the channel was opened.
        BackendClientSingleton.getClient().get(request).then(response => {
            //Convert each bank response to a frontend bank type
            let notificationsList = new Array();
            for (let notification of response) {
                let documentID = null;
                let status = null;
                if (null != notification.StatusUpdate.DocumentUpdates) {
                    status = notification.StatusUpdate.DocumentUpdates[0].Status;
                    documentID = notification.StatusUpdate.DocumentUpdates[0].ID;
                }
                let accountID = null;
                if (null != notification.StatusUpdate.AccountUpdates) {
                    accountID = notification.StatusUpdate.AccountUpdates[0].ID;
                }
                let bankID = null;
                if (null != notification.StatusUpdate.BankUpdates) {
                    bankID = notification.StatusUpdate.BankUpdates[0].ID;
                }
                var notificationEntry = new Notification();
                notificationEntry.type = notification.Type;
                notificationEntry.account = accountID;
                notificationEntry.document = documentID;
                notificationEntry.status = status;
                notificationEntry.bankname = notification.BankName;
                notificationEntry.bankid = notification.BankID;
                notificationEntry.corporateid = corporateID;
                notificationEntry.corporatename = notification.CorporateName;
                notificationEntry.username = notification.UserName;
                notificationEntry.message = notification.Message;
                notificationEntry.dateCreated = notification.CreationDate;
                // Flag indicates whether notification has been processed or not.
                notificationEntry.processed = notification.Processed;
                notificationsList.push(notificationEntry);
            }
            dispatch({ type: 'RECEIVE_NOTIFICATIONS', notifications: notificationsList });
        });
    },
    requestNotificationsForBank: (bankID) => (dispatch, getState) => {
        const request = new backendTypes.BankNotifications();
        request.BankID = bankID;
        // Get maximum of 20 notifications.
        request.MaximumCount = 10;
        // OOR TODO: Use current user, rather than user passed in. This user is the user set when the channel was opened.
        BackendClientSingleton.getClient().get(request).then(response => {
            //Convert each bank response to a frontend bank type
            let notificationsList = new Array();
            for (let notification of response) {
                let documentID = null;
                let status = null;
                if (null != notification.StatusUpdate.DocumentUpdates) {
                    status = notification.StatusUpdate.DocumentUpdates[0].Status;
                    documentID = notification.StatusUpdate.DocumentUpdates[0].ID;
                }
                let accountID = null;
                if (null != notification.StatusUpdate.AccountUpdates) {
                    accountID = notification.StatusUpdate.AccountUpdates[0].ID;
                }
                let corporateID = null;
                if (null != notification.StatusUpdate.CorporateUpdates) {
                    corporateID = notification.StatusUpdate.CorporateUpdates[0].ID;
                }
                var notificationEntry = new Notification();
                notificationEntry.type = notification.Type;
                notificationEntry.account = accountID;
                notificationEntry.document = documentID;
                notificationEntry.status = status;
                notificationEntry.bankname = notification.BankName;
                notificationEntry.bankid = notification.BankID;
                notificationEntry.corporateid = corporateID;
                notificationEntry.corporatename = notification.CorporateName;
                notificationEntry.username = notification.UserName;
                notificationEntry.message = notification.Message;
                notificationEntry.dateCreated = notification.CreationDate;
                // Flag indicates whether notification has been processed or not.
                notificationEntry.processed = notification.Processed;
                notificationsList.push(notificationEntry);
            }
            dispatch({ type: 'RECEIVE_NOTIFICATIONS', notifications: notificationsList });
        });
    },
    createNotification: (notification) => (dispatch, getState) => {
        console.dir("Created notification" + notification.message);
        //Get backend connection
        var client = BackendClientSingleton.getClient();
        //Get a list of the roles
        // let request = new backendTypes.CreateUser();
        // client.get(request)
        //     .then(response => {
        //         //Dispatch the information to the reducer
        //         dispatch({
        //             type: "ADD_NOTIFICATION",
        //         })
        //     })
        //     .catch(error => {
        //         //TODO: Handle error here
        //     });
    },
    deleteNotification: (notificationID) => (dispatch, getState) => {
        console.dir("Deleted notification " + notificationID);
        //Get backend connection
        // var client = BackendClientSingleton.getClient();
        // //Get a list of the roles
        // let request = new backendTypes.DeleteUser();
        // request.notification = notificationToDelete;
        // client.post(request)
        //     .then(response => {
        //         //Dispatch the information to the reducer
        //         dispatch({
        //             type: "DELETE_NOTIFICATION"
        //         })
        //     })
        //     .catch(error => {
        //         //TODO: Handle error here
        //     });
    }
};
//Initial state for the reducer.
const unloadedState = {
    notificationsList: []
};
export const reducer = (state, action) => {
    switch (action.type) {
        case "RECEIVE_NOTIFICATIONS":
            var newState = new NotificationsState();
            //Add the users list and merge it with the current state
            newState.notificationsList = action.notifications;
            return Immutable.from(state).merge(newState);
        case "ADD_NOTIFICATION":
            //Make a copy of the staet, with deep copy to copy recursively
            var newState = Immutable.from(state).asMutable({ deep: true });
            //Add the users list and merge it with the current state
            newState.notificationsList.push(action.notificationToAdd);
            return Immutable.from(state).merge(newState);
        case "DELETE_NOTIFICATION":
            var newState = Immutable.from(state).asMutable({ deep: true });
            var notifications = newState.notificationsList.filter((notification) => notification.id != action.notificationID);
            newState.notificationsList = notifications;
            return Immutable.from(state).merge(newState);
    }
    return state || unloadedState;
};
//# sourceMappingURL=notificationsStore.js.map