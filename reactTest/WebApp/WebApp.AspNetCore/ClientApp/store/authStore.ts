import { Action, Reducer, ActionCreator } from 'redux';
import { AppThunkAction } from './';
import { BackendClientSingleton } from '../BackendClientSingleton';
import { JsonServiceClient } from 'servicestack-client';
import * as backendTypes from '../backendTypes';
import * as Immutable from "seamless-immutable";
import { push, RouterAction } from 'react-router-redux';

export class AuthState {
    loggedIn: boolean;
    username: string;
    role: string;
    displayName: string;
    redirectUrl: string;
    bearerToken: string;
    userConfig: backendTypes.UserConfig;
}

//interface RequestLogInAction {
//    type: "REQUEST_LOGIN_ACTION";
//    username: string;
//    password: string;
//}

interface ResponseLogInAction {
    type: "RESPONSE_LOGIN_ACTION";
    loggedIn: boolean;
    displayName: string;
    username: string;
    role: string;
    userConfig: backendTypes.UserConfig;
}

interface ResponseRegisterAction {
    type: "RESPONSE_REGISTER_ACTION";
    loggedIn: boolean;
}

interface RedirectUrlAction {
    type: "REDIRECT_URL_ACTION";
    loggedIn: boolean;
    url: string;
}

interface ResponseLogOffAction {
    type: "RESPONSE_LOGOFF_ACTION";
}

interface LocationChangeAction {
    type: "@@router/LOCATION_CHANGE";
}

export type KnownAuthAction = ResponseLogInAction | ResponseLogOffAction | RedirectUrlAction | LocationChangeAction | ResponseRegisterAction | RouterAction;

export const actionCreators = {
 requestLogin: (username: string, password: string): AppThunkAction<KnownAuthAction> => (dispatch, getState) => {
        // Create the request payload
        const requestPayload = {
            provider: "credentials",
            UserName: username,
            Password: password,
            RememberMe: true
        };

        // Get the client instance
        const client = BackendClientSingleton.getClient();
       const userInfoPromise = fetch('http://localhost:5001/auth/credentials', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            credentials: 'include',  // Add this to include cookies or HTTP authentication credentials
            body: JSON.stringify(requestPayload)
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to authenticate');
            }
            return response.json();
        })
        .then((userInfoResponse) => {
            if (!userInfoResponse) {
                throw new Error('No user information returned from the server');
            }

            // Save the auth token
            BackendClientSingleton.setAuthToken(userInfoResponse.BearerToken);
            sessionStorage.setItem("AuthToken", userInfoResponse.BearerToken);

            // Update client token
            client.BearerToken = userInfoResponse.BearerToken;

            return userInfoResponse;
        })
        .catch(error => {
            console.error('Error during login:', error);
            return null;
        });


        // After userInfoPromise is resolved, make another request for user configuration
        const userConfigPromise = userInfoPromise.then(userInfo => {
            if (!userInfo) {
                throw new Error('UserInfo is null');
            }

            const userConfigRequest = {
                UserID: parseInt(userInfo.UserId, 10)
            };

            // Use fetch to request user config instead of ServiceStack client
            return fetch(`http://localhost:5001/user/config?UserID=${userConfigRequest.UserID}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${client.BearerToken}` // Include Bearer token in headers
                }
            })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Failed to fetch user config');
                }
                return response.json();
            })
            .then(userConfig => {
                return userConfig; // Return the user config data
            })
            .catch(error => {
                console.error('Error fetching user config:', error);
                return null;  // Ensure `null` is returned on error
            });
        });

        // Handle both promises
        Promise.all([userInfoPromise, userConfigPromise]).then(([userInfo, userConfig]) => {
            if (!userInfo || !userConfig) {
                console.error("Failed to retrieve user information or user configuration");
                return; // Early return if any promise failed
            }

            const { UserPrivilege, UserType } = userConfig;
            const isAdmin = UserPrivilege === "SuperAdmin" || UserPrivilege === "Admin";

            if (isAdmin) {
                dispatch({
                    type: 'RESPONSE_LOGIN_ACTION',
                    username,
                    role: "admin",
                    loggedIn: true,
                    displayName: userInfo.DisplayName,
                    userConfig
                });
                dispatch(push('/admin'));
            } else {
                if (UserType === "Bank") {
                    dispatch({
                        type: 'RESPONSE_LOGIN_ACTION',
                        username,
                        role: "bank",
                        loggedIn: true,
                        displayName: userInfo.DisplayName,
                        userConfig
                    });
                    dispatch(push('/corporates'));
                } else if (UserType === "Corporate") {
                    dispatch({
                        type: 'RESPONSE_LOGIN_ACTION',
                        username,
                        role: "corporate",
                        loggedIn: true,
                        displayName: userInfo.DisplayName,
                        userConfig
                    });
                    dispatch(push('/banks'));
                }
            }
        }).catch(error => {
            console.error("Failed to handle login or user configuration", error);
        });
    },
    requestLogoff: (): AppThunkAction<KnownAuthAction | RouterAction> => (dispatch, getState) => {
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
    requestRegisterUser: (username: string, password: string): AppThunkAction<KnownAuthAction> => (dispatch, getState) => {
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
    redirectUrl: (url: string, loggedIn: boolean): AppThunkAction<KnownAuthAction> => (dispatch, getState) => {
        //Log off from server.
        //TODO
        //Dispatch response to reducer to update state
        dispatch({ type: 'REDIRECT_URL_ACTION', url: url, loggedIn: loggedIn });
    }
};


const unloadedState: AuthState = {
    loggedIn: false,
    role: "",
    username: "",
    displayName: "",
    redirectUrl: "",
    bearerToken: "",
    userConfig: new backendTypes.UserConfig()
};

export const reducer: Reducer<AuthState> = (state: AuthState, action: KnownAuthAction) => {
    switch (action.type) {
        case "RESPONSE_LOGIN_ACTION":
            {
                //take all properties except type from the action and copy into state
                let { type, ...actionProperties } = action;
                let newState: AuthState = {
                    ...actionProperties,
                    bearerToken: BackendClientSingleton.getAuthToken(),
                    redirectUrl: ""
                }
                sessionStorage.setItem("AuthDetails", JSON.stringify(newState));
                return Immutable.from(state).merge(newState);
            }
        case "RESPONSE_LOGOFF_ACTION":
            {
                BackendClientSingleton.setAuthToken("");
                sessionStorage.setItem("AuthDetails", "");
                return Immutable.from(state).merge({ ...unloadedState });
            }
        case "RESPONSE_REGISTER_ACTION":
            {
                BackendClientSingleton.setAuthToken("");
                return Immutable.from(state).merge({ ...unloadedState });
            }
        case "REDIRECT_URL_ACTION":
            {
                const newState = new AuthState();
                let actionRedirectCast = action as RedirectUrlAction;
                newState.loggedIn = actionRedirectCast.loggedIn;
                newState.redirectUrl = actionRedirectCast.url;
                newState.bearerToken = BackendClientSingleton.getAuthToken();
                return Immutable.from(state).merge(newState);
            }
    }
    return state || unloadedState;
};