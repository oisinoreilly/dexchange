import * as backendTypes from '../backendTypes';
import { BackendClientSingleton } from '../BackendClientSingleton';
export class BanksState {
}
export class BankUi {
}
export const actionCreators = {
    addBank: (bank) => (dispatch, getState) => {
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
    deleteBank: (bankId) => (dispatch, getState) => {
        const request = new backendTypes.BankDelete();
        request.BankID = bankId;
        BackendClientSingleton.getClient().delete(request).then(response => {
            dispatch({ type: 'DELETE_BANK_RESPONSE', bankId: bankId });
        });
    },
    listBanks: () => (dispatch, getState) => {
        const request = new backendTypes.BankList();
        BackendClientSingleton.getClient().get(request)
            .then(response => {
            let banksList = new Array();
            for (let bank of response) {
                var bankEntry = new BankUi();
                bankEntry.title = bank.Name;
                bankEntry.icon = bank.IconBase64;
                bankEntry.id = bank.Id;
                bankEntry.accountTypes = bank.accountTypes;
                let status = new backendTypes.StatusEx();
                if (null == bank.Status)
                    status.Status = "Approved_e";
                else
                    status.Status = bank.Status.Status;
                bankEntry.status = status;
                bankEntry.accounts = [];
                banksList.push(bankEntry);
            }
            dispatch({ type: 'LIST_BANKS_RESPONSE', banks: banksList });
        });
    },
    selectBank: (id) => (dispatch, getState) => {
        dispatch({
            type: 'SELECT_BANK',
            bankId: id
        });
    },
    clearBanks: () => (dispatch, getState) => {
        dispatch({
            type: 'CLEAR_BANKS'
        });
    },
};
const intialState = {
    bankList: [],
    selectedBank: new BankUi()
};
export const reducer = (state, action) => {
    switch (action.type) {
        case 'LIST_BANKS_RESPONSE':
            {
                const newState = new BanksState();
                newState.bankList = [...action.banks];
                newState.selectedBank = Object.assign({}, state.selectedBank);
                return newState;
            }
        case 'ADD_BANK_RESPONSE':
            {
                const newState = new BanksState();
                newState.bankList = state.bankList.concat(action.bank);
                newState.selectedBank = Object.assign({}, state.selectedBank);
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
                newState.selectedBank = Object.assign({}, state.selectedBank);
                return newState;
            }
        case 'UPDATE_STATUS_MESSAGE':
            {
                if (action.bankstatus === null || action.bank === null)
                    return state;
                const newState = new BanksState();
                newState.bankList = state.bankList.map(bank => {
                    if (bank.id === action.bank) {
                        const newStatus = new backendTypes.StatusEx();
                        newStatus.Status = action.bankstatus;
                        bank.status = newStatus;
                        return bank;
                    }
                    return bank;
                });
                newState.selectedBank = Object.assign({}, state.selectedBank);
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
};
//# sourceMappingURL=bankState.js.map