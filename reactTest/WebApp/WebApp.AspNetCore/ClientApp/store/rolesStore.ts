import { Action, Reducer, ActionCreator } from 'redux';
import { AppThunkAction } from './';
import { BackendClientSingleton } from '../BackendClientSingleton';
import { JsonServiceClient } from 'servicestack-client';
import * as backendTypes from '../backendTypes';
import * as Immutable from "seamless-immutable";

//The state that this store exposes
export class RolesState {
    rolesList: backendTypes.RoleInfo[];
    selectedRole: backendTypes.RoleInfo;
}
//Interfaces for the allowed action responses, to be used by the reducer
interface RolesListResponse {
    type: "ROLES_LIST_RESPONSE_ACTION";
    rolesList?: backendTypes.RoleInfo[];
}

interface GetRoleResponse {
    type: "ROLE_GET_RESPONSE_ACTION";
    role?: backendTypes.RoleInfo;
}

interface AddRoleResponse {
    type: "ROLE_ADD_RESPONSE_ACTION";
    newRole?: backendTypes.RoleInfo;
}

interface DeleteRoleResponse {
    type: "DELETE_ROLE";
    roleId: string;
}

interface SelectRole {
    type: "SELECT_ROLE";
    roleId: string;
}

//Known actions
export type KnownRoleAction = RolesListResponse |
    GetRoleResponse |
    AddRoleResponse | 
    SelectRole |
    DeleteRoleResponse;

//Dispatch method helpers to allow clients interact with the store
export const actionCreators = {
    requestRolesList: (): AppThunkAction<KnownRoleAction> => (dispatch, getState) => {
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
                })
            })
            .catch(error => {
                //TODO: Handle error here
            });
    },
    requestAddRole: (roleToAdd: backendTypes.RoleInfo): AppThunkAction<KnownRoleAction> => (dispatch, getState) => {
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
                })
            })
            .catch(error => {
                //TODO: Handle error here
            });
    },
    requestRoleInfo: (roleName: string): AppThunkAction<KnownRoleAction> => (dispatch, getState) => {
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
                })
            })
            .catch(error => {
                //TODO: Handle error here
            });
    },
    selectRole: (roleId: string): AppThunkAction<KnownRoleAction> => (dispatch, getState) => {
        dispatch({
            type: "SELECT_ROLE",
            roleId
        })
    },
    deleteRole: (roleId: string): AppThunkAction<KnownRoleAction> => (dispatch, getState) => {
        var client = BackendClientSingleton.getClient();

        let request = new backendTypes.RoleInfoDelete();
        request.RoleId = roleId;
        client.delete(request)
            .then(response => {
                dispatch({
                    type: "DELETE_ROLE",
                    roleId
                })
            })
            .catch(error => {
                //TODO: Handle error here
            });
    },
};

//Initial state for the reducer.
const unloadedState: RolesState = {
    rolesList: [],
    selectedRole: new backendTypes.RoleInfo
};

export const reducer: Reducer<RolesState> = (state: RolesState, action: KnownRoleAction) => {
    switch (action.type) {
        case "ROLES_LIST_RESPONSE_ACTION":
            {
                var newState = new RolesState();
                //Add the roles list and merge it with the current state
                newState.rolesList = action.rolesList;
                //Clear the current role, as none is selected at this point
                newState.selectedRole = { ...state.selectedRole };
                return Immutable.from(state).merge(newState);
            }
        case "ROLE_ADD_RESPONSE_ACTION":
            {
                var newState = Immutable.from(state).asMutable({ deep: true });
                newState.rolesList.push(action.newRole);
                newState.selectedRole = { ...state.selectedRole };
                return Immutable.from(state).merge(newState);
            }
        case "DELETE_ROLE":
            {
                const newState = Immutable.from(state).asMutable({ deep: true });
                newState.rolesList = state.rolesList.filter(role => role.Id !== action.roleId);
                newState.selectedRole = { ...state.selectedRole };
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