import * as backendTypes from '../backendTypes';
import { BackendClientSingleton } from '../BackendClientSingleton';
export class AccountState {
}
export class AccountUi {
}
export const actionCreators = {
    listAccounts: (bankId, corporateId) => (dispatch, getState) => {
        const request = new backendTypes.AccountList();
        request.BankID = bankId;
        request.CorporateID = corporateId;
        BackendClientSingleton.getClient().get(request)
            .then(response => {
            let accountsList = new Array();
            for (let account of response) {
                var accountEntry = new AccountUi();
                accountEntry.title = account.Name;
                if (account.Detail != null) {
                    accountEntry.description = account.Detail.Description;
                }
                accountEntry.id = account.Id;
                accountEntry.status = account.Status;
                accountEntry.documents = account.Documents;
                accountEntry.parentId = bankId === "" ? account.CorporateId : account.ParentID;
                accountsList.push(accountEntry);
            }
            dispatch({ type: 'LIST_ACCOUNTS_RESPONSE', accounts: accountsList });
        });
    },
    listAllAccounts: () => (dispatch, getState) => {
        const request = new backendTypes.AccountListAll();
        BackendClientSingleton.getClient().get(request)
            .then(response => {
            let accountsList = new Array();
            for (let account of response) {
                const accountEntry = new AccountUi();
                accountEntry.title = account.Name;
                if (account.Detail != null) {
                    accountEntry.description = account.Detail.Description;
                }
                accountEntry.id = account.Id;
                accountEntry.status = account.Status;
                accountEntry.documents = account.Documents;
                accountEntry.bankId = account.ParentID;
                accountEntry.corporateId = account.CorporateId;
                accountsList.push(accountEntry);
            }
            dispatch({ type: 'LIST_ACCOUNTS_RESPONSE', accounts: accountsList });
        });
    },
    addAccount: (bankId, corporateId, account, fillDocs) => (dispatch, getState) => {
        const request = new backendTypes.AccountCreate();
        request.BankID = bankId;
        let now = new Date();
        let dattime = now.toLocaleString();
        request.Account = new backendTypes.Account();
        request.Account.Name = account.title;
        request.Account.Creation = dattime;
        request.Account.Id = account.id;
        request.Account.ParentID = bankId;
        request.Account.CorporateId = corporateId;
        request.Account.Documents = [];
        request.Account.Detail = new backendTypes.AccountDetail();
        request.Account.Detail.Description = account.description;
        request.Account.AccountType = account.accountType;
        request.PrefillDocuments = fillDocs;
        BackendClientSingleton.getClient().post(request).then(response => {
            dispatch({
                type: 'ADD_ACCOUNT',
                bankId,
                corporateId,
                account
            });
        });
    },
    deleteAccount: (accountId) => (dispatch, getState) => {
        const request = new backendTypes.AccountDelete();
        request.ID = accountId;
        BackendClientSingleton.getClient().delete(request).then(response => {
            dispatch({ type: 'DELETE_ACCOUNT', accountId: accountId });
        });
    },
    selectAccount: (accountId) => (dispatch, getState) => {
        dispatch({
            type: 'SELECT_ACCOUNT',
            accountId
        });
    },
    clearAccounts: () => (dispatch, getState) => {
        dispatch({
            type: 'CLEAR_ACCOUNTS'
        });
    },
    retrieveAccountTypes: (bankId) => (dispatch, getState) => {
        const request = new backendTypes.AccountTypeReadAll();
        request.BankID = bankId;
        BackendClientSingleton.getClient().get(request)
            .then(response => {
            dispatch({
                type: 'LIST_ACCOUNT_TYPES_RESPONSE',
                accountTypes: response
            });
        });
    }
};
const initialState = {
    accountList: [],
    accountTypeList: [],
    selectedAccount: new AccountUi()
};
export const reducer = (state, action) => {
    switch (action.type) {
        case 'LIST_ACCOUNTS_RESPONSE':
            {
                const newState = new AccountState();
                newState.accountList = [...action.accounts];
                newState.selectedAccount = Object.assign({}, state.selectedAccount);
                newState.accountTypeList = [...state.accountTypeList];
                return newState;
            }
        case 'ADD_ACCOUNT':
            {
                const newState = new AccountState();
                newState.accountList = state.accountList.concat(action.account);
                newState.selectedAccount = Object.assign({}, state.selectedAccount);
                newState.accountTypeList = [...state.accountTypeList];
                return newState;
            }
        case 'SELECT_ACCOUNT':
            {
                const newState = new AccountState();
                newState.accountList = [...state.accountList];
                newState.selectedAccount = state.accountList.filter(account => account.id === action.accountId)[0];
                newState.accountTypeList = [...state.accountTypeList];
                return newState;
            }
        case 'DELETE_ACCOUNT':
            {
                const newState = new AccountState();
                newState.accountList = state.accountList.filter(account => account.id !== action.accountId);
                newState.selectedAccount = Object.assign({}, state.selectedAccount);
                newState.accountTypeList = [...state.accountTypeList];
                return newState;
            }
        case 'UPDATE_STATUS_MESSAGE':
            {
                const newState = new AccountState();
                newState.accountList = state.accountList.map(account => {
                    if (account.id === action.account) {
                        const newStatus = new backendTypes.StatusEx();
                        newStatus.Status = action.accountstatus;
                        account.status = newStatus;
                        return account;
                    }
                    return account;
                });
                newState.selectedAccount = Object.assign({}, state.selectedAccount);
                newState.accountTypeList = [...state.accountTypeList];
                return newState;
            }
        case 'LIST_ACCOUNT_TYPES_RESPONSE':
            {
                const newState = new AccountState();
                newState.accountList = [...state.accountList];
                newState.selectedAccount = Object.assign({}, state.selectedAccount);
                newState.accountTypeList = action.accountTypes;
                return newState;
            }
        case 'CLEAR_ACCOUNTS':
            {
                const newState = new AccountState();
                newState.accountTypeList = [...state.accountTypeList];
                newState.accountList = [];
                newState.selectedAccount = new AccountUi();
                return newState;
            }
    }
    return state || initialState;
};
//# sourceMappingURL=accountState.js.map