var __rest = (this && this.__rest) || function (s, e) {
    var t = {};
    for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p) && e.indexOf(p) < 0)
        t[p] = s[p];
    if (s != null && typeof Object.getOwnPropertySymbols === "function")
        for (var i = 0, p = Object.getOwnPropertySymbols(s); i < p.length; i++) if (e.indexOf(p[i]) < 0)
            t[p[i]] = s[p[i]];
    return t;
};
import { BackendClientSingleton } from '../BackendClientSingleton';
import * as backendTypes from '../backendTypes';
import * as Immutable from "seamless-immutable";
import { push } from 'react-router-redux';
export class AuthState {
}
export const actionCreators = {
    requestLogin: (username, password) => (dispatch, getState) => {
        //Login to the server
        var client = BackendClientSingleton.getClient();
        //TODO: Use the JT tokens fully and just pass them in on the cookie with an empty auth attempt
        //If our credentials are saved with the server we should be able to auth.
        let request = new backendTypes.Authenticate();
        request.provider = "credentials";
        request.UserName = username;
        request.Password = password;
        request.RememberMe = true;
        const userInfoPromise = client.post(request).then((userInfoResponse) => {
            //Save the auth token.
            BackendClientSingleton.setAuthToken(userInfoResponse.BearerToken);
            sessionStorage.setItem("AuthToken", userInfoResponse.BearerToken);
            //Also update the client we are using with the new token
            client.BearerToken = userInfoResponse.BearerToken;
            //Get the system configuration.
            return userInfoResponse;
        });
        const userConfigPromise = userInfoPromise.then(function (userInfo) {
            const userConfigRequest = new backendTypes.UserConfigGet();
            userConfigRequest.UserID = parseInt(userInfo.UserId);
            return client.get(userConfigRequest);
        });
        Promise.all([userInfoPromise, userConfigPromise]).then(function ([userInfo, userConfig]) {
            const { UserPrivilege, UserType } = userConfig;
            const isAdmin = UserPrivilege === "SuperAdmin" || UserPrivilege === "Admin";
            if (isAdmin) {
                //Update the state to reflect the logged in user
                dispatch({
                    type: 'RESPONSE_LOGIN_ACTION',
                    username: username,
                    role: "admin",
                    loggedIn: true,
                    displayName: userInfo.DisplayName,
                    userConfig
                });
                dispatch(push('/admin'));
            }
            else {
                if (UserType === "Bank") {
                    dispatch({
                        type: 'RESPONSE_LOGIN_ACTION',
                        username: username,
                        role: "bank",
                        loggedIn: true,
                        displayName: userInfo.DisplayName,
                        userConfig
                    });
                    dispatch(push('/corporates'));
                }
                else if (UserType === "Corporate") {
                    dispatch({
                        type: 'RESPONSE_LOGIN_ACTION',
                        username: username,
                        role: "corporate",
                        loggedIn: true,
                        displayName: userInfo.DisplayName,
                        userConfig
                    });
                    dispatch(push('/banks'));
                }
            }
        });
    },
    requestLogoff: () => (dispatch, getState) => {
        //Log off from server.
        var client = BackendClientSingleton.getClient();
        let request = new backendTypes.Authenticate();
        request.provider = "logout";
        client.post(request).then(response => {
            //Clear our auth token
            sessionStorage.setItem("AuthToken", "");
            //Dispatch response to reducer to update state
            dispatch({ type: 'RESPONSE_LOGOFF_ACTION' });
            //Route to the login page
            dispatch(push('/login'));
        });
    },
    requestRegisterUser: (username, password) => (dispatch, getState) => {
        //Login to the server
        var client = BackendClientSingleton.getClient();
        //TODO: Use the JT tokens fully and just pass them in on the cookie with an empty auth attempt
        //If our credentials are saved with the server we should be able to auth.
        let request = new backendTypes.Register();
        request.UserName = username;
        request.Password = password;
        //TODO: Take this from the user
        request.Email = username + "@documentationHQ.com";
        client.post(request).then(response => {
            dispatch({ type: 'RESPONSE_REGISTER_ACTION', loggedIn: false });
        })
            .catch(error => {
            //TODO: Handle error here. Need to show the validation exceptions
            console.error('error in registering: ' + error.responseStatus.message);
            //dispatch({ type: 'RESPONSE_LOGIN_ACTION', loggedIn: false });
        });
        //TODO: Save client
    },
    redirectUrl: (url, loggedIn) => (dispatch, getState) => {
        //Log off from server.
        //TODO
        //Dispatch response to reducer to update state
        dispatch({ type: 'REDIRECT_URL_ACTION', url: url, loggedIn: loggedIn });
    }
};
const unloadedState = {
    loggedIn: false,
    role: "",
    username: "",
    displayName: "",
    redirectUrl: "",
    bearerToken: "",
    userConfig: new backendTypes.UserConfig()
};
export const reducer = (state, action) => {
    switch (action.type) {
        case "RESPONSE_LOGIN_ACTION":
            {
                //take all properties except type from the action and copy into state
                let { type } = action, actionProperties = __rest(action, ["type"]);
                let newState = Object.assign({}, actionProperties, { bearerToken: BackendClientSingleton.getAuthToken(), redirectUrl: "" });
                sessionStorage.setItem("AuthDetails", JSON.stringify(newState));
                return Immutable.from(state).merge(newState);
            }
        case "RESPONSE_LOGOFF_ACTION":
            {
                BackendClientSingleton.setAuthToken("");
                sessionStorage.setItem("AuthDetails", "");
                return Immutable.from(state).merge(Object.assign({}, unloadedState));
            }
        case "RESPONSE_REGISTER_ACTION":
            {
                BackendClientSingleton.setAuthToken("");
                return Immutable.from(state).merge(Object.assign({}, unloadedState));
            }
        case "REDIRECT_URL_ACTION":
            {
                const newState = new AuthState();
                let actionRedirectCast = action;
                newState.loggedIn = actionRedirectCast.loggedIn;
                newState.redirectUrl = actionRedirectCast.url;
                newState.bearerToken = BackendClientSingleton.getAuthToken();
                return Immutable.from(state).merge(newState);
            }
    }
    return state || unloadedState;
};
//# sourceMappingURL=authStore.js.map