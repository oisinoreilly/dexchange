import { Action, Reducer, ActionCreator } from 'redux';
import { AppThunkAction } from './';
import * as backendTypes from '../backendTypes';
import { Entity } from '../frontendTypes';
import { BackendClientSingleton } from '../BackendClientSingleton';

export class BanksState {
    bankList: BankUi[];
    selectedBank: BankUi;
}

export class BankUi implements Entity  {
    id: string;
    title: string;
    icon: string;
    status: backendTypes.StatusEx;
    accounts: string[];
    accountTypes: backendTypes.AccountType[];
}

interface ReceiveBanksAction {
    type: 'LIST_BANKS_RESPONSE';
    banks: BankUi[];
}

interface AddBanksAction {
    type: 'ADD_BANK_RESPONSE';
    bank: BankUi;
}

interface SelectBankAction {
    type: 'SELECT_BANK';
    bankId: string;
}

interface DeleteBankAction {
    type: 'DELETE_BANK_RESPONSE';
    bankId: string;
}

interface DeleteBankAction {
    type: 'DELETE_BANK_RESPONSE';
    bankId: string;
}

interface ClearBanks {
    type: 'CLEAR_BANKS',
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


export type KnownBankAction =
    ReceiveBanksAction |
    AddBanksAction |
    SelectBankAction |
    DeleteBankAction |
    UpdateStatusAction |
    ClearBanks;

export const actionCreators = {
    addBank: (bank: BankUi): AppThunkAction<KnownBankAction> => (dispatch, getState) => {

        const request = new backendTypes.BankCreate();
        request.Bank = new backendTypes.Bank();
        request.Bank.Name = bank.title;
        request.Bank.Id = bank.id.toString();
        request.Bank.IconBase64 = bank.icon;
        request.Bank.Accounts = [];
        BackendClientSingleton.getClient().post(request).then(response => {
            dispatch({ type: 'ADD_BANK_RESPONSE', bank: bank });
        });
    },
    deleteBank: (bankId: string): AppThunkAction<KnownBankAction> => (dispatch, getState) => {

        const request = new backendTypes.BankDelete();
        request.BankID = bankId;
        BackendClientSingleton.getClient().delete(request).then(response => {
            dispatch({ type: 'DELETE_BANK_RESPONSE', bankId: bankId });
        });
    },
    listBanks: (): AppThunkAction<KnownBankAction> => (dispatch, getState) => {
        const request = new backendTypes.BankList();

        BackendClientSingleton.getClient().get(request)
            .then(response => {
                let banksList = new Array<BankUi>();
                for (let bank of response) {
                    var bankEntry = new BankUi();
                    bankEntry.title = bank.Name;
                    bankEntry.icon = bank.IconBase64;
                    bankEntry.id = bank.Id;
                    bankEntry.accountTypes = bank.accountTypes;

                    let status = new backendTypes.StatusEx();
                    if (null == bank.Status)
                        status.Status = "Approved_e";
                    else status.Status = bank.Status.Status;
                    bankEntry.status = status;

                    bankEntry.accounts = [];
                    banksList.push(bankEntry);
                }
                dispatch({ type: 'LIST_BANKS_RESPONSE', banks: banksList });
            });
    },
    selectBank: (id: string): AppThunkAction<KnownBankAction> => (dispatch, getState) => {
        dispatch({
            type: 'SELECT_BANK',
            bankId: id
        });
    },
    clearBanks: (): AppThunkAction<KnownBankAction> => (dispatch, getState) => {
        dispatch({
            type: 'CLEAR_BANKS'
        });
    },
}

const intialState: BanksState = {
    bankList: [],
    selectedBank: new BankUi()
}

export const reducer: Reducer<BanksState> = (state: BanksState, action: KnownBankAction) => {
    switch (action.type) {
        case 'LIST_BANKS_RESPONSE':
            {
                const newState = new BanksState();
                newState.bankList =  [ ...action.banks];
                newState.selectedBank =  {...state.selectedBank}
                return newState;
            }
        case 'ADD_BANK_RESPONSE':
            {
                const newState = new BanksState();
                newState.bankList = state.bankList.concat(action.bank);
                newState.selectedBank = { ...state.selectedBank };
                return newState;
            }
        case 'SELECT_BANK':
            {
                const newState = new BanksState();
                newState.bankList = [...state.bankList];
                newState.selectedBank = state.bankList.filter(bank => bank.id === action.bankId)[0];
                return newState;
            }
        case 'DELETE_BANK_RESPONSE':
            {
                const newState = new BanksState();
                newState.bankList = state.bankList.filter(bank => bank.id !== action.bankId);
                newState.selectedBank = { ...state.selectedBank };
                return newState;
            }
        case 'UPDATE_STATUS_MESSAGE':
            {
                if (action.bankstatus === null || action.bank === null)
                    return state;

                const newState = new BanksState()
                newState.bankList = state.bankList.map(bank => {
                    if (bank.id === action.bank) {
                        const newStatus = new backendTypes.StatusEx();
                        newStatus.Status = action.bankstatus;
                        bank.status = newStatus;
                        return bank;
                    }
                    return bank;
                });
                newState.selectedBank = { ...state.selectedBank };
                return newState;
            }
        case 'CLEAR_BANKS':
            {
                const newState = new BanksState();
                newState.bankList = [];
                newState.selectedBank = new BankUi();
                return newState;
            }
    }
    return state || intialState;
}
