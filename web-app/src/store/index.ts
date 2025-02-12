import * as Auth from './authStore';
import * as Roles from './rolesStore';
import * as Users from './usersStore';;
import * as Notifications from './notificationsStore';
import * as Banks from './bankState';
import * as Accounts from './accountState';
import * as Documents from './documentState';
import * as Corporates from './corporateState';
import * as Groups from './groupState';
import * as Toasts from './toastsState';


// The top-level state object
export interface ApplicationState {
    banks: Banks.BanksState,
    auth: Auth.AuthState,
    roles: Roles.RolesState,
    users: Users.UsersState,
    groups: Groups.GroupsState,
    notifications: Notifications.NotificationsState,
    accounts: Accounts.AccountState,
    documents: Documents.DocumentState,
    corporates: Corporates.CorporatesState,
    toasts: Toasts.ToastsState
}

// Whenever an action is dispatched, Redux will update each top-level application state property using
// the reducer with the matching name. It's important that the names match exactly, and that the reducer
// acts on the corresponding ApplicationState property type.
export const reducers = {
    banks: Banks.reducer,
    auth: Auth.reducer,
    roles: Roles.reducer,
    groups: Groups.reducer,
    users: Users.reducer,
    notifications: Notifications.reducer,
    accounts: Accounts.reducer,
    documents: Documents.reducer,
    corporates: Corporates.reducer,
    toasts: Toasts.reducer
};

// This type can be used as a hint on action creators so that its 'dispatch' and 'getState' params are
// correctly typed to match your store.
export interface AppThunkAction<TAction> {
    (dispatch: (action: TAction) => void, getState: () => ApplicationState): void;
}
