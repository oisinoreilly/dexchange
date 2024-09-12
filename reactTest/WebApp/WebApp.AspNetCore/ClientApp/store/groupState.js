import { BackendClientSingleton } from '../BackendClientSingleton';
import * as backendTypes from '../backendTypes';
import { Group } from '../backendTypes';
export class GroupsState {
}
//Initial state for the reducer.
const unloadedState = {
    groupsList: [],
    selectedGroup: new Group()
};
export const actionCreators = {
    listAllGroups: () => (dispatch, getState) => {
        const client = BackendClientSingleton.getClient();
        const request = new backendTypes.GroupsListGet();
        client.get(request)
            .then(response => {
            dispatch({
                type: 'LIST_ALL_GROUPS_RESPONSE',
                groups: response
            });
        });
    },
    createGroup: (groupToAdd) => (dispatch, getState) => {
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
    deleteGroup: (groupId) => (dispatch, getState) => {
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
    addUsersToGroup: (userIds, groupId) => (dispatch, getState) => {
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
    removeUserFromGroup: (userId, groupId) => (dispatch, getState) => {
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
    addRolesToGroup: (roleIds, groupId) => (dispatch, getState) => {
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
    removeRoleFromGroup: (roleId, groupId) => (dispatch, getState) => {
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
    selectGroup: (group) => (dispatch, getState) => {
        dispatch({
            type: 'SELECT_GROUP',
            group
        });
    }
};
export const reducer = (state, action) => {
    switch (action.type) {
        case 'LIST_ALL_GROUPS_RESPONSE':
            {
                const newState = new GroupsState();
                newState.groupsList = [...action.groups];
                newState.selectedGroup = Object.assign({}, state.selectedGroup);
                return newState;
            }
        case 'CREATE_GROUP_RESPONSE':
            {
                const newState = new GroupsState();
                newState.groupsList = state.groupsList.concat(action.groupToAdd);
                newState.selectedGroup = Object.assign({}, state.selectedGroup);
                return newState;
            }
        case 'DELETE_GROUP_RESPONSE':
            {
                const newState = new GroupsState();
                newState.groupsList = state.groupsList.filter(group => group.Id !== action.groupId);
                newState.selectedGroup = Object.assign({}, state.selectedGroup);
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
                newState.selectedGroup = Object.assign({}, action.group);
                return newState;
            }
    }
    return state || unloadedState;
};
//# sourceMappingURL=groupState.js.map