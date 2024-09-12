import * as backendTypes from '../backendTypes';
import { BackendClientSingleton } from '../BackendClientSingleton';
export class CorporatesState {
}
export class CorporateUi {
}
export const actionCreators = {
    addCorporate: (corporate) => (dispatch, getState) => {
        const request = new backendTypes.CorporateCreate();
        request.Corporate = new backendTypes.Corporate();
        request.Corporate.Detail = new backendTypes.CorporateDetail();
        request.Corporate.Detail.Name = corporate.title;
        request.Corporate.Id = corporate.id.toString();
        request.Corporate.Icon = corporate.icon;
        request.Corporate.Accounts = [];
        request.Corporate.ParentID = corporate.parentId;
        BackendClientSingleton.getClient().post(request).then(response => {
            dispatch({ type: 'ADD_CORPORATE_RESPONSE', corporate: corporate });
        });
    },
    deleteCorporate: (corporateId) => (dispatch, getState) => {
        const request = new backendTypes.CorporateDelete();
        request.CorporateID = corporateId;
        BackendClientSingleton.getClient().delete(request).then(response => {
            dispatch({ type: 'DELETE_CORPORATE_RESPONSE', corporateId: corporateId });
        });
    },
    updateFieldDefinitions: (corporateId, fieldDefinitions) => (dispatch, getState) => {
        const request = new backendTypes.CorporateAllFieldDefinitionsUpdate();
        request.CorporateID = corporateId;
        request.FieldDefinitions = fieldDefinitions;
        BackendClientSingleton.getClient().put(request).then(response => {
            const id = getState().toasts.nextId;
            dispatch({
                type: 'UPDATE_DEFINITIONS',
                definitions: fieldDefinitions,
            });
            dispatch({
                type: 'ADD_TOAST',
                message: 'Corporate Successfully Updated',
                id
            });
            setTimeout(() => {
                dispatch({
                    type: 'REMOVE_TOAST',
                    id
                });
            }, 5000);
        });
    },
    getCorporate: (corporateId) => (dispatch, getState) => {
        const request = new backendTypes.CorporateRead();
        request.CorporateID = corporateId;
        BackendClientSingleton.getClient().get(request).then(response => {
            var corporateEntry = new CorporateUi();
            corporateEntry.title = response.Detail.Name;
            corporateEntry.icon = response.Icon;
            corporateEntry.id = response.Id;
            corporateEntry.subsids = response.Children;
            corporateEntry.parentId = response.ParentID;
            corporateEntry.fields = response.Fields;
            let status = new backendTypes.StatusEx();
            if (null == response.Status)
                status.Status = "Approved_e";
            else
                status.Status = response.Status.Status;
            corporateEntry.status = status;
            corporateEntry.accounts = [];
            dispatch({
                type: 'GET_CORPORATE_RESPONSE',
                corporate: corporateEntry
            });
        });
    },
    listCorporates: () => (dispatch, getState) => {
        const request = new backendTypes.CorporatesList();
        BackendClientSingleton.getClient().get(request)
            .then(response => {
            let corporatesList = new Array();
            for (let corporate of response) {
                var corporateEntry = new CorporateUi();
                corporateEntry.title = corporate.Detail.Name;
                corporateEntry.icon = corporate.Icon;
                corporateEntry.id = corporate.Id;
                corporateEntry.subsids = corporate.Children;
                corporateEntry.parentId = corporate.ParentID;
                corporateEntry.fields = corporate.Fields;
                let status = new backendTypes.StatusEx();
                if (null == corporate.Status)
                    status.Status = "Approved_e";
                else
                    status.Status = corporate.Status.Status;
                corporateEntry.status = status;
                corporateEntry.accounts = [];
                corporatesList.push(corporateEntry);
            }
            dispatch({ type: 'LIST_CORPORATES_RESPONSE', corporates: corporatesList });
        });
    },
    selectCorporate: (id) => (dispatch, getState) => {
        dispatch({
            type: 'SELECT_CORPORATE',
            corporateId: id
        });
    },
    selectSubsid: (id) => (dispatch, getState) => {
        dispatch({
            type: 'SELECT_SUBSID',
            corporateId: id
        });
    },
    clearCorporates: () => (dispatch, getState) => {
        dispatch({
            type: 'CLEAR_CORPORATES'
        });
    }
};
const intialState = {
    corporateList: [],
    selectedCorporate: new CorporateUi(),
    selectedSubsid: new CorporateUi(),
};
export const reducer = (state, action) => {
    switch (action.type) {
        case 'LIST_CORPORATES_RESPONSE':
            {
                const newState = new CorporatesState();
                newState.corporateList = [...action.corporates];
                newState.selectedCorporate = Object.assign({}, state.selectedCorporate);
                newState.selectedSubsid = Object.assign({}, state.selectedSubsid);
                return newState;
            }
        case 'ADD_CORPORATE_RESPONSE':
            {
                const newState = new CorporatesState();
                newState.corporateList = state.corporateList.concat(action.corporate);
                if (action.corporate.parentId) {
                    const parentIndex = newState.corporateList.findIndex(corp => corp.id === action.corporate.parentId);
                    newState.corporateList[parentIndex].subsids.push(action.corporate.id);
                }
                newState.selectedCorporate = Object.assign({}, state.selectedCorporate);
                newState.selectedSubsid = Object.assign({}, state.selectedSubsid);
                return newState;
            }
        case 'SELECT_CORPORATE':
            {
                const newState = new CorporatesState();
                newState.corporateList = [...state.corporateList];
                newState.selectedCorporate = state.corporateList.filter(corporate => corporate.id === action.corporateId)[0];
                newState.selectedSubsid = new CorporateUi();
                return newState;
            }
        case 'SELECT_SUBSID':
            {
                const newState = new CorporatesState();
                newState.corporateList = [...state.corporateList];
                newState.selectedSubsid = state.corporateList.filter(corporate => corporate.id === action.corporateId)[0];
                newState.selectedCorporate = Object.assign({}, state.selectedCorporate);
                return newState;
            }
        case 'DELETE_CORPORATE_RESPONSE':
            {
                const newState = new CorporatesState();
                newState.corporateList = state.corporateList.filter(corporate => corporate.id !== action.corporateId);
                newState.selectedCorporate = Object.assign({}, state.selectedCorporate);
                newState.selectedSubsid = Object.assign({}, state.selectedSubsid);
                return newState;
            }
        case 'GET_CORPORATE_RESPONSE':
            {
                const newState = new CorporatesState();
                newState.corporateList = [action.corporate];
                newState.selectedCorporate = action.corporate;
                newState.selectedSubsid = new CorporateUi();
                return newState;
            }
        case 'UPDATE_STATUS_MESSAGE':
            {
                const newState = new CorporatesState();
                newState.corporateList = state.corporateList.map(corp => {
                    if (corp.id === action.corporate) {
                        const newStatus = new backendTypes.StatusEx();
                        newStatus.Status = action.corporatestatus;
                        corp.status = newStatus;
                        return corp;
                    }
                    return corp;
                });
                newState.selectedCorporate = Object.assign({}, state.selectedCorporate);
                newState.selectedSubsid = Object.assign({}, state.selectedSubsid);
                return newState;
            }
        case 'UPDATE_DEFINITIONS':
            {
                const newState = new CorporatesState();
                newState.corporateList = [...state.corporateList];
                const selected = Object.assign({}, state.selectedCorporate);
                selected.fields = action.definitions;
                newState.selectedCorporate = selected;
                newState.selectedSubsid = Object.assign({}, state.selectedSubsid);
                return newState;
            }
        case 'CLEAR_CORPORATES':
            {
                const newState = new CorporatesState();
                newState.corporateList = [];
                newState.selectedCorporate = new CorporateUi();
                newState.selectedSubsid = new CorporateUi();
                return newState;
            }
    }
    return state || intialState;
};
//# sourceMappingURL=corporateState.js.map