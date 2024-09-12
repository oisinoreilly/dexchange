import { Action, Reducer, ActionCreator } from 'redux';
import { AppThunkAction } from './';

export class Toast {
    id: number;
    messsage: string;
}


export class ToastsState {
    toastList: Toast[]
    nextId: number;
}

interface AddToast {
    type: 'ADD_TOAST',
    message: string;
    id: number;
}

interface RemoveToast {
    type: 'REMOVE_TOAST',
    id: number;
}

export type KnownToastAction = AddToast | RemoveToast;

export const actionsCreators = {
    addToast: (message: string): AppThunkAction<KnownToastAction> => (dispatch, getState) => {
        const id = ++getState().toasts.nextId
        dispatch({
            type: 'ADD_TOAST',
            message,
            id
        });
        setTimeout(() => {
            dispatch({
                type: 'REMOVE_TOAST',
                id
            });
        }, 2000);
    },
    removeToast: (id: number): AppThunkAction<KnownToastAction> => (dispatch, getState) => {
        dispatch({
            type: 'REMOVE_TOAST',
            id
        })
    }
}

const unloadedState: ToastsState = {
    toastList: [],
    nextId: 1
}

export const reducer: Reducer<ToastsState> = (state: ToastsState, action: KnownToastAction) => {
    switch (action.type) {
        case 'ADD_TOAST':
            {
                const newState = new ToastsState();
                const newToast = new Toast();
                newToast.messsage = action.message;
                newToast.id = action.id
                newState.toastList = state.toastList.concat([newToast]);
                newState.nextId = ++state.nextId;
                return newState;
            }
        case 'REMOVE_TOAST':
            {
                const newState = new ToastsState();
                newState.toastList = state.toastList.filter(toast => toast.id !== action.id);
                newState.nextId = state.nextId;
                return newState;
            }
    }
    return state || unloadedState;
}