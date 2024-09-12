import * as BackendDtos from '../backendTypes';
import * as Store from './index';
import * as AuthStore from './authStore';
import * as Redux from 'redux';
import { BackendClientSingleton } from '../BackendClientSingleton';
import { push } from 'react-router-redux';

const authMiddleware: any = (store: Redux.Store<Store.ApplicationState>) => next => (action: any) => {
    try {
        const authState = store.getState().auth;
        //We need to fetch the auth token between reloads of the page.
        var authToken = sessionStorage.getItem("AuthToken");
        switch (action.type) {
            case "@@router/LOCATION_CHANGE":
                // Reroute to the login page if we have no JWT token saved
                if (!action.payload.pathname.includes("/login")) {
                    if ((authToken == null) || (authToken === "")) {
                        store.dispatch(push('/login'));
                        return next;
                    }
                }
                break;
            default:
                // do nothing 
                break;
        }
    } catch (e) {
        //Helpers.reportError(e);
        return (next);
    }
    return next(action);
}

export default authMiddleware;