import * as React from 'react';
import { connect } from 'react-redux';
import { ApplicationState } from '../store/index';
import * as AuthState from '../store/AuthStore';
import * as NotificationsState from '../store/NotificationsStore';
import { bindActionCreators } from "redux";
import * as BanksState from '../store/bankState';
import * as CorporateState from '../store/corporateState';
import * as AccountsState from '../store/accountState';
import * as DocumentState from '../store/documentState';
import * as RolesState from '../store/rolesStore';
import * as UserState from '../store/usersStore';
import * as GroupState from '../store/groupState';
import * as ToastsState from '../store/toastsState';


type accountActions = typeof AccountsState.actionCreators
type bankactions =  typeof BanksState.actionCreators
type corporateActions =  typeof CorporateState.actionCreators
type documentActions =  typeof DocumentState.actionCreators
type groupActions =  typeof GroupState.actionCreators
type rolesActions =  typeof RolesState.actionCreators
type userActions = typeof UserState.actionCreators
type toastActions = typeof ToastsState.actionsCreators

// At runtime, Redux will merge together...
export type ReduxProps =
    {// ... state we've requested from the Redux store
        auth: AuthState.AuthState,
        notifications: NotificationsState.NotificationsState,
        banks: BanksState.BanksState,
        accounts: AccountsState.AccountState,
        documents: DocumentState.DocumentState,
        roles: RolesState.RolesState,
        corporates: CorporateState.CorporatesState,
        users: UserState.UsersState,
        groups: GroupState.GroupsState,
        toasts: ToastsState.ToastsState
    } &
    { // actions creators from redux
        actions:
        accountActions & bankactions & corporateActions & groupActions
        & documentActions & groupActions & rolesActions & userActions & toastActions
    } &
    {
        // ... plus incoming routing parameters
        name: string,
        type: string;
        params: any;
    };

export const withRedux = (unConnectedComponent) => {

    const mergeProps = (state, actions, ownProps) => ({
        ...state,
        ...actions,
        ...ownProps
    }); 

    return connect(
        (state: ApplicationState) => {
            const { auth, notifications, banks, accounts, documents,
                corporates, groups, roles, users, toasts } = state;
            return {
                auth, notifications, banks, accounts, documents,
                corporates, groups, roles, users, toasts
            };
        },           // Selects which state properties are merged into the component's props
        (dispatch) => {
            return {
                actions: bindActionCreators(Object.assign({},
                    AccountsState.actionCreators,
                    BanksState.actionCreators,
                    DocumentState.actionCreators,
                    CorporateState.actionCreators,
                    GroupState.actionCreators,
                    RolesState.actionCreators,
                    UserState.actionCreators,
                    ToastsState.actionsCreators
                ), dispatch)
            };
        },
        mergeProps              
    )(unConnectedComponent);
}


