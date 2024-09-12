import { connect } from 'react-redux';
import { bindActionCreators } from "redux";
import * as BanksState from '../store/bankState';
import * as CorporateState from '../store/corporateState';
import * as AccountsState from '../store/accountState';
import * as DocumentState from '../store/documentState';
import * as RolesState from '../store/rolesStore';
import * as UserState from '../store/usersStore';
import * as GroupState from '../store/groupState';
import * as ToastsState from '../store/toastsState';
export const withRedux = (unConnectedComponent) => {
    const mergeProps = (state, actions, ownProps) => (Object.assign({}, state, actions, ownProps));
    return connect((state) => {
        const { auth, notifications, banks, accounts, documents, corporates, groups, roles, users, toasts } = state;
        return {
            auth, notifications, banks, accounts, documents,
            corporates, groups, roles, users, toasts
        };
    }, // Selects which state properties are merged into the component's props
    (dispatch) => {
        return {
            actions: bindActionCreators(Object.assign({}, AccountsState.actionCreators, BanksState.actionCreators, DocumentState.actionCreators, CorporateState.actionCreators, GroupState.actionCreators, RolesState.actionCreators, UserState.actionCreators, ToastsState.actionsCreators), dispatch)
        };
    }, mergeProps)(unConnectedComponent);
};
//# sourceMappingURL=withReduxStore.jsx.map