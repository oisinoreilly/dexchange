import * as Auth from './authStore';
import * as Roles from './rolesStore';
import * as Users from './usersStore';
;
import * as Notifications from './notificationsStore';
import * as Banks from './bankState';
import * as Accounts from './accountState';
import * as Documents from './documentState';
import * as Corporates from './corporateState';
import * as Groups from './groupState';
import * as Toasts from './toastsState';
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
//# sourceMappingURL=index.js.map