import { BackendClientSingleton } from '../BackendClientSingleton';
import * as backendTypes from '../backendTypes';
import * as Immutable from "seamless-immutable";
//The state that this store exposes
export class UsersState {
}
//Dispatch method helpers to allow clients interact with the store
export const actionCreators = {
    listUsers: () => (dispatch, getState) => {
        var client = BackendClientSingleton.getClient();
        let request = new backendTypes.UsersListGet();
        client.get(request)
            .then(response => {
            dispatch({
                type: "LIST_USERS_RESPONSE",
                users: response
            });
        })
            .catch(error => {
            //TODO: Handle error here
        });
    },
    createUser: (userToCreate, password, userConfig) => (dispatch, getState) => {
        //Get backend connection
        var client = BackendClientSingleton.getClient();
        //Get a list of the roles
        const request = new backendTypes.UserInfoPost();
        request.Password = password;
        request.UserToCreate = userToCreate;
        request.Config = userConfig;
        client.post(request)
            .then(response => {
            //Dispatch the information to the reducer
            dispatch({
                type: "CREATE_USER_RESPONSE",
                userToCreate,
                userConfig
            });
        })
            .catch(error => {
            //TODO: Handle error here
        });
    },
    deleteUser: (userName) => (dispatch, getState) => {
        var client = BackendClientSingleton.getClient();
        //delete a user by username
        const request = new backendTypes.UserInfoDelete;
        request.UserToDelete = userName;
        client.delete(request)
            .then(response => {
            dispatch({
                type: "DELETE_USER_RESPONSE",
                userToDelete: userName
            });
        })
            .catch(error => {
            //TODO: Handle error here
            console.log(error);
        });
    },
    editUser: (user) => (dispatch, getState) => {
        console.dir("edited user " + user.UserName);
        //Get backend connection
        var client = BackendClientSingleton.getClient();
        //Get a list of the roles
        let request = new backendTypes.UserInfoPut();
        request.UserToUpdate = new backendTypes.UserAuth();
        request.UserToUpdate = user;
        client.put(request)
            .then(response => {
            //Dispatch the information to the reducer
            dispatch({
                type: "EDIT_USER_RESPONSE",
                userToUpdate: user
            });
        })
            .catch(error => {
            console.log(error);
        });
    },
    listUsersForgroup: (groupId) => (dispatch, getState) => {
        const client = BackendClientSingleton.getClient();
        let request = new backendTypes.GetUsersForGroup();
        request.Group = groupId;
        client.get(request)
            .then(response => {
            const res = response;
            dispatch({
                type: "LIST_USERS_FOR_GROUP",
                users: res
            });
        }).catch(error => {
            //TODO: Handle error here
        });
    }
};
//Initial state for the reducer.
const unloadedState = {
    userList: []
};
export const reducer = (state, action) => {
    switch (action.type) {
        case "LIST_USERS_RESPONSE":
            var newState = new UsersState();
            //Add the users list and merge it with the current state
            newState.userList = action.users;
            return Immutable.from(state).merge(newState);
        case "CREATE_USER_RESPONSE":
            //Make a copy of the staet, with deep copy to copy recursively
            var newState = Immutable.from(state).asMutable({ deep: true });
            //Add the users list and merge it with the current state
            newState.userList.push(action.userToCreate);
            return Immutable.from(state).merge(newState);
        case "EDIT_USER_RESPONSE":
            //Update the edited user in the list
            var newState = Immutable.from(state).asMutable({ deep: true });
            var users = newState.userList.map((user) => {
                if (user.UserName == action.userToUpdate.UserName) {
                    return action.userToUpdate;
                }
                else {
                    return user;
                }
            });
            newState.userList = users;
            return Immutable.from(state).merge(newState);
        case "DELETE_USER_RESPONSE":
            {
                //Remove the user from the list
                var newState = Immutable.from(state).asMutable({ deep: true });
                var users = newState.userList.filter(user => user.UserName != action.userToDelete);
                newState.userList = users;
                return Immutable.from(state).merge(newState);
            }
        case "LIST_USERS_FOR_GROUP":
            {
                var newState = new UsersState();
                newState.userList = [...action.users];
                return newState;
            }
    }
    return state || unloadedState;
};
//# sourceMappingURL=usersStore.js.map