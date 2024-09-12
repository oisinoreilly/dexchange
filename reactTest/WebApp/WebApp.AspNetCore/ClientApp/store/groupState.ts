import { Action, Reducer, ActionCreator } from 'redux';
import { AppThunkAction } from './';
import { BackendClientSingleton } from '../BackendClientSingleton';
import { JsonServiceClient } from 'servicestack-client';
import * as backendTypes from '../backendTypes';
import { RoleInfo, Group } from '../backendTypes';
import { ApplicationState } from '../store/index';

export class GroupsState {
    groupsList: Group[];
    selectedGroup: Group;
}

interface ListAllGroupsResponse {
    type: "LIST_ALL_GROUPS_RESPONSE";
    groups: Group[];
}

interface CreateGroupResponse {
    type: "CREATE_GROUP_RESPONSE";
    groupToAdd: Group;
}

interface DeleteGroupResponse {
    type: "DELETE_GROUP_RESPONSE";
    groupId: string;
}

interface AddUserToGroupResponse {
    type: "ADD_USER_TO_GROUP_RESPONSE";
    userId: number;
    groupId: string;
}

interface AddUsersToGroupResponse {
    type: "ADD_USERS_TO_GROUP_RESPONSE";
    userIds: number[];
    groupId: string;
}

interface RemoveUserFromGroupResponse {
    type: "REMOVE_USER_FROM_GROUP_RESPONSE";
    userId: number;
    groupId: string;
}

interface AddRolesToGroupResponse {
    type: "ADD_ROLES_TO_GROUP_RESPONSE";
    roleIds: string[];
    groupId: string;
}

interface RemoveRoleFromGroupResponse {
    type: "REMOVE_ROLE_FROM_GROUP_RESPONSE";
    roleId: string;
    groupId: string;
}

interface SelectGroup {
    type: "SELECT_GROUP";
    group: Group;
}

//Initial state for the reducer.
const unloadedState: GroupsState = {
    groupsList: [],
    selectedGroup: new Group()
}

export type KnownGroupAction =
    ListAllGroupsResponse |
    DeleteGroupResponse |
    CreateGroupResponse |
    CreateGroupResponse |
    AddRolesToGroupResponse |
    AddUserToGroupResponse |
    RemoveRoleFromGroupResponse |
    RemoveUserFromGroupResponse |
    SelectGroup |
    AddUsersToGroupResponse;

export const actionCreators = {
    listAllGroups: (): AppThunkAction<KnownGroupAction> => (dispatch, getState) => {
        const client = BackendClientSingleton.getClient();
        const request = new backendTypes.GroupsListGet();
        client.get(request)
            .then(response => {
                dispatch({
                    type: 'LIST_ALL_GROUPS_RESPONSE',
                    groups: response
                });
            })
    },
    createGroup: (groupToAdd: Group): AppThunkAction<KnownGroupAction> => (dispatch, getState) => {
        const client = BackendClientSingleton.getClient();
        const request = new backendTypes.GroupCreate();
        request.Group = groupToAdd;
        client.post(request)
            .then(response => {
                dispatch({
                    type: 'CREATE_GROUP_RESPONSE',
                    groupToAdd
                });
            })
            .catch(error => {
                //TODO: Handle error here
            });
        
    },
    deleteGroup: (groupId: string): AppThunkAction<KnownGroupAction> => (dispatch, getState) => {
        const client = BackendClientSingleton.getClient();
        const request = new backendTypes.GroupDelete();
        request.GroupId = groupId;
        client.delete(request)
            .then(response => {
                dispatch({
                    type: 'DELETE_GROUP_RESPONSE',
                    groupId
                });
            })
            .catch(error => {
                //TODO: Handle error here
            });
    },
    addUsersToGroup: (userIds: number[], groupId): AppThunkAction<KnownGroupAction> => (dispatch, getState) => {
        const client = BackendClientSingleton.getClient();
        const request = new backendTypes.AddUsersToGroup();
        request.GroupId = groupId;
        request.UserIds = userIds;
        client.put(request)
            .then(response => {
                dispatch({
                    type: 'ADD_USERS_TO_GROUP_RESPONSE',
                    userIds,
                    groupId
                });
            })
            .catch(error => {
                //TODO: Handle error here
            });
    },
    removeUserFromGroup: (userId: number, groupId): AppThunkAction<KnownGroupAction> => (dispatch, getState) => {
        const client = BackendClientSingleton.getClient();
        const request = new backendTypes.DeleteUserFromGroup();
        request.GroupId = groupId;
        request.UserId = userId;
        client.delete(request)
            .then(response => {
                dispatch({
                    type: 'REMOVE_USER_FROM_GROUP_RESPONSE',
                    userId,
                    groupId
                });
            })
            .catch(error => {
                //TODO: Handle error here
            });
    },
    addRolesToGroup: (roleIds: string[], groupId): AppThunkAction<KnownGroupAction> => (dispatch, getState) => {
        const client = BackendClientSingleton.getClient();
        const request = new backendTypes.AddRolesToGroup();
        request.GroupId = groupId;
        request.RoleIds = roleIds;
        client.put(request)
            .then(response => {
                dispatch({
                    type: 'ADD_ROLES_TO_GROUP_RESPONSE',
                    roleIds,
                    groupId
                });
            })
            .catch(error => {
                //TODO: Handle error here
            });
    },
    removeRoleFromGroup: (roleId: string, groupId: string): AppThunkAction<KnownGroupAction> => (dispatch, getState) => {
        const client = BackendClientSingleton.getClient();
        const request = new backendTypes.RemoveRoleInfoFromGroup();
        request.GroupId = groupId;
        request.RoleId = roleId;
        client.delete(request)
            .then(response => {
                dispatch({
                    type: 'REMOVE_ROLE_FROM_GROUP_RESPONSE',
                    roleId,
                    groupId
                });
            })
            .catch(error => {
                //TODO: Handle error here
            });
    },
    selectGroup: (group: Group): AppThunkAction<KnownGroupAction> => (dispatch, getState) => {
        dispatch({
            type: 'SELECT_GROUP',
            group
        });
    }
};

export const reducer: Reducer<GroupsState> = (state: GroupsState, action: KnownGroupAction) => {
    switch (action.type) {
        case 'LIST_ALL_GROUPS_RESPONSE':
            {
                const newState = new GroupsState();
                newState.groupsList = [ ...action.groups ];
                newState.selectedGroup = { ...state.selectedGroup };
                return newState;
            }
        case 'CREATE_GROUP_RESPONSE':
            {
                const newState = new GroupsState();
                newState.groupsList = state.groupsList.concat(action.groupToAdd);
                newState.selectedGroup = { ...state.selectedGroup };
                return newState;
            }
        case 'DELETE_GROUP_RESPONSE':
            {
                const newState = new GroupsState();
                newState.groupsList = state.groupsList.filter(group => group.Id !== action.groupId);
                newState.selectedGroup = { ...state.selectedGroup };
                return newState;
            }
        case 'ADD_USER_TO_GROUP_RESPONSE':
            {
                const newState = new GroupsState();
                newState.groupsList = state.groupsList.map((group) => {
                    if (group.Id === action.groupId) {
                        group.UserAuthIds = group.UserAuthIds.concat(action.userId);
                        return group;
                    }
                    return group;
                });
                newState.selectedGroup = newState.groupsList.find(grp => grp.Id === state.selectedGroup.Id);
                return newState;
            }
        case 'ADD_USERS_TO_GROUP_RESPONSE':
            {
                const newState = new GroupsState();
                newState.groupsList = state.groupsList.map((group) => {
                    if (group.Id === action.groupId) {
                        group.UserAuthIds = group.UserAuthIds.concat(action.userIds);
                        return group;
                    }
                    return group;
                });
                newState.selectedGroup = newState.groupsList.find(grp => grp.Id === state.selectedGroup.Id);
                return newState;
            }
        case 'ADD_ROLES_TO_GROUP_RESPONSE':
            {
                const newState = new GroupsState();
                newState.groupsList = state.groupsList.map((group) => {
                    if (group.Id === action.groupId) {
                        group.Roles = group.Roles.concat(action.roleIds);
                        return group;
                    }
                    return group;
                });
                newState.selectedGroup = newState.groupsList.find(grp => grp.Id === state.selectedGroup.Id);
                return newState;
            }
        case 'REMOVE_ROLE_FROM_GROUP_RESPONSE':
            {
                const newState = new GroupsState();
                newState.groupsList = state.groupsList.map((group) => {
                    if (group.Id === action.groupId) {
                        group.Roles = group.Roles.filter(id => id !== action.roleId);
                        return group;
                    }
                    return group;
                });
                newState.selectedGroup = newState.groupsList.find(grp => grp.Id === state.selectedGroup.Id);
                return newState;
            }
        case 'REMOVE_USER_FROM_GROUP_RESPONSE':
            {  
                const newState = new GroupsState();
                newState.groupsList = state.groupsList.map((group) => {
                    if (group.Id === action.groupId) {
                        group.UserAuthIds = group.UserAuthIds.filter(id => id !== action.userId);
                        return group;
                    }
                    return group;
                });
                newState.selectedGroup = newState.groupsList.find(grp => grp.Id === state.selectedGroup.Id);
                return newState;
            }
        case 'SELECT_GROUP':
            {
                const newState = new GroupsState();
                newState.groupsList = [...state.groupsList];
                newState.selectedGroup = { ...action.group };
                return newState;
            }
    }
    return state || unloadedState;
}

