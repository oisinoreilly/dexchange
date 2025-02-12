import { Action, Reducer, ActionCreator } from 'redux';
import { AppThunkAction } from './';
import * as backendTypes from '../backendTypes';
import { BackendClientSingleton } from '../BackendClientSingleton';

export class AccountState {
    accountList: AccountUi[];
    selectedAccount: AccountUi;
    accountTypeList: backendTypes.AccountType[];
}

export class AccountUi {
    id: string;
    parentId?: string;
    bankId?: string;
    corporateId?: string;
    title: string;
    icon: string;
    description: string;
    status: backendTypes.StatusEx;
    documents: string[];
    accountType: string;
}

interface ReceiveAccountsAction {
    type: 'LIST_ACCOUNTS_RESPONSE';
    accounts: AccountUi[];
}

interface SelectAccountAction {
    type: 'SELECT_ACCOUNT';
    accountId: string;
}

interface AddAccountAction {
    type: 'ADD_ACCOUNT';
    bankId: string;
    corporateId: string;
    account: AccountUi;
}

interface DeleteAccountAction {
    type: 'DELETE_ACCOUNT';
    accountId: string;
}

interface AccountTypeListResponse {
    type: 'LIST_ACCOUNT_TYPES_RESPONSE',
    accountTypes: backendTypes.AccountType[]
}

interface ClearAccounts {
    type: 'CLEAR_ACCOUNTS',
}

export interface UpdateStatusAction {
    type: 'UPDATE_STATUS_MESSAGE';
    account: string,
    document: string,
    corporate: string,
    bank: string,
    documentstatus: backendTypes.Status,
    accountstatus: backendTypes.Status,
    corporatestatus: backendTypes.Status,
    bankstatus: backendTypes.Status,
}

export type KnownAccountAction =
    ReceiveAccountsAction | 
    AddAccountAction |
    SelectAccountAction |
    DeleteAccountAction |
    UpdateStatusAction |
    AccountTypeListResponse |
    ClearAccounts;

export const actionCreators = {
    listAccounts: (bankId: string, corporateId: string): AppThunkAction<KnownAccountAction> => (dispatch, getState) => {
        const request = new backendTypes.AccountList();
        request.BankID = bankId;
        request.CorporateID = corporateId;
        BackendClientSingleton.getClient().get(request)
            .then(response => {
                let accountsList = new Array<AccountUi>();
                for (let account of response as backendTypes.Account[]) {
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
    listAllAccounts: (): AppThunkAction<KnownAccountAction> => (dispatch, getState) => {
        const request = new backendTypes.AccountListAll();
        BackendClientSingleton.getClient().get(request)
            .then(response => {
                let accountsList = new Array<AccountUi>();
                for (let account of response as backendTypes.Account[]) {
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
    addAccount: (bankId: string, corporateId: string, account: AccountUi, fillDocs: boolean): AppThunkAction<KnownAccountAction> => (dispatch, getState) => {
        
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
    deleteAccount: (accountId: string): AppThunkAction<KnownAccountAction> => (dispatch, getState) => {
        const request = new backendTypes.AccountDelete();
        request.ID = accountId;
        BackendClientSingleton.getClient().delete(request).then(response => {
            dispatch({ type: 'DELETE_ACCOUNT', accountId: accountId });
        });
    },
    selectAccount: (accountId: string): AppThunkAction<KnownAccountAction> => (dispatch, getState) => {
            dispatch({
                type: 'SELECT_ACCOUNT',
                accountId
            });
    },
    clearAccounts: (): AppThunkAction<KnownAccountAction> => (dispatch, getState) => {
        dispatch({
            type: 'CLEAR_ACCOUNTS'
        });
    },
    retrieveAccountTypes: (bankId: string): AppThunkAction<KnownAccountAction> => (dispatch, getState) => {
        const request = new backendTypes.AccountTypeReadAll();
        request.BankID = bankId;
        BackendClientSingleton.getClient().get(request)
            .then(response => {
                dispatch({
                    type: 'LIST_ACCOUNT_TYPES_RESPONSE',
                    accountTypes: response
                });
            })
    }
}

const initialState: AccountState = {
    accountList: [],
    accountTypeList: [],
    selectedAccount: new AccountUi()
}

export const reducer: Reducer<AccountState> = (state: AccountState, action: KnownAccountAction) => {
    switch (action.type) {
        case 'LIST_ACCOUNTS_RESPONSE':
            {
                const newState = new AccountState();
                newState.accountList = [ ...action.accounts ];
                newState.selectedAccount = { ...state.selectedAccount };
                newState.accountTypeList = [...state.accountTypeList];
                return newState;
            }
        case 'ADD_ACCOUNT':
            {
                const newState = new AccountState();
                newState.accountList = state.accountList.concat(action.account);
                newState.selectedAccount = { ...state.selectedAccount };
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
                newState.selectedAccount = { ...state.selectedAccount };
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
                newState.selectedAccount = { ...state.selectedAccount };
                newState.accountTypeList = [...state.accountTypeList];
                return newState;
            }
        case 'LIST_ACCOUNT_TYPES_RESPONSE':
            {
                const newState = new AccountState();
                newState.accountList = [...state.accountList];
                newState.selectedAccount = { ...state.selectedAccount };
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
}
