import { BackendClientSingleton } from '../BackendClientSingleton';
import * as backendTypes from '../backendTypes';
import * as Immutable from "seamless-immutable";
//The state that this store exposes
export class RolesState {
}
//Dispatch method helpers to allow clients interact with the store
export const actionCreators = {
    requestRolesList: () => (dispatch, getState) => {
        //Get backend connection
        var client = BackendClientSingleton.getClient();
        //Get a list of the roles
        let request = new backendTypes.RoleListGet();
        client.get(request)
            .then(response => {
            //Dispatch the information to the reducer
            dispatch({
                type: "ROLES_LIST_RESPONSE_ACTION",
                rolesList: response
            });
        })
            .catch(error => {
            //TODO: Handle error here
        });
    },
    requestAddRole: (roleToAdd) => (dispatch, getState) => {
        //Get backend connection
        var client = BackendClientSingleton.getClient();
        //Get a list of the roles
        let request = new backendTypes.RoleInfoInsert();
        request.Role = roleToAdd;
        client.post(request)
            .then(response => {
            //Dispatch the information to the reducer
            roleToAdd.Id = response.Id;
            dispatch({
                type: "ROLE_ADD_RESPONSE_ACTION",
                newRole: roleToAdd
            });
        })
            .catch(error => {
            //TODO: Handle error here
        });
    },
    requestRoleInfo: (roleName) => (dispatch, getState) => {
        //Get backend connection
        var client = BackendClientSingleton.getClient();
        //Get a list of the roles
        let request = new backendTypes.RoleInfoGet();
        request.RoleName = roleName;
        client.get(request)
            .then(response => {
            //Dispatch the information to the reducer
            dispatch({
                type: "ROLE_GET_RESPONSE_ACTION",
                role: response
            });
        })
            .catch(error => {
            //TODO: Handle error here
        });
    },
    selectRole: (roleId) => (dispatch, getState) => {
        dispatch({
            type: "SELECT_ROLE",
            roleId
        });
    },
    deleteRole: (roleId) => (dispatch, getState) => {
        var client = BackendClientSingleton.getClient();
        let request = new backendTypes.RoleInfoDelete();
        request.RoleId = roleId;
        client.delete(request)
            .then(response => {
            dispatch({
                type: "DELETE_ROLE",
                roleId
            });
        })
            .catch(error => {
            //TODO: Handle error here
        });
    },
};
//Initial state for the reducer.
const unloadedState = {
    rolesList: [],
    selectedRole: new backendTypes.RoleInfo
};
export const reducer = (state, action) => {
    switch (action.type) {
        case "ROLES_LIST_RESPONSE_ACTION":
            {
                var newState = new RolesState();
                //Add the roles list and merge it with the current state
                newState.rolesList = action.rolesList;
                //Clear the current role, as none is selected at this point
                newState.selectedRole = Object.assign({}, state.selectedRole);
                return Immutable.from(state).merge(newState);
            }
        case "ROLE_ADD_RESPONSE_ACTION":
            {
                var newState = Immutable.from(state).asMutable({ deep: true });
                newState.rolesList.push(action.newRole);
                newState.selectedRole = Object.assign({}, state.selectedRole);
                return Immutable.from(state).merge(newState);
            }
        case "DELETE_ROLE":
            {
                const newState = Immutable.from(state).asMutable({ deep: true });
                newState.rolesList = state.rolesList.filter(role => role.Id !== action.roleId);
                newState.selectedRole = Object.assign({}, state.selectedRole);
                return Immutable.from(state).merge(newState);
            }
        case "ROLE_GET_RESPONSE_ACTION":
            {
                //Return the role data fetched
                var newState = new RolesState();
                newState.selectedRole = action.role;
                return Immutable.from(state).merge(newState);
            }
        case "SELECT_ROLE":
            {
                const copy = new RolesState();
                copy.selectedRole = state.rolesList.find(role => role.Id === action.roleId);
                copy.rolesList = [...state.rolesList];
                return copy;
            }
    }
    return state || unloadedState;
};
//# sourceMappingURL=rolesStore.js.map