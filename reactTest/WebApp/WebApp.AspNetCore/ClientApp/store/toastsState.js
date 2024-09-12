export class Toast {
}
export class ToastsState {
}
export const actionsCreators = {
    addToast: (message) => (dispatch, getState) => {
        const id = ++getState().toasts.nextId;
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
    removeToast: (id) => (dispatch, getState) => {
        dispatch({
            type: 'REMOVE_TOAST',
            id
        });
    }
};
const unloadedState = {
    toastList: [],
    nextId: 1
};
export const reducer = (state, action) => {
    switch (action.type) {
        case 'ADD_TOAST':
            {
                const newState = new ToastsState();
                const newToast = new Toast();
                newToast.messsage = action.message;
                newToast.id = action.id;
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
};
//# sourceMappingURL=toastsState.js.map