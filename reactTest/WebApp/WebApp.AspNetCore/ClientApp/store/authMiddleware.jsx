import { push } from 'react-router-redux';
const authMiddleware = (store) => next => (action) => {
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
    }
    catch (e) {
        //Helpers.reportError(e);
        return (next);
    }
    return next(action);
};
export default authMiddleware;
//# sourceMappingURL=authMiddleware.jsx.map