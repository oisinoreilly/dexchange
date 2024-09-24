import { JsonServiceClient } from 'servicestack-client';
export class BackendClientSingleton {
    //private static backendServerUrl:string = "http://ec2-54-202-32-215.us-west-2.compute.amazonaws.com:5001"; //UNCOMMENT FOR DEPLOYMENT
    constructor() {
    }
    static getClient() {
        if (!window.client) {
            window.client = new JsonServiceClient(this.backendServerUrl);
        }
        window.client.bearerToken = sessionStorage.getItem("AuthToken");
        ;
        return window.client;
        //TODO: Get the singleton working or look up a better pattern.
        //if (!BackendClientSingleton.client) {
        //    BackendClientSingleton.client = new JsonServiceClient(this.backendServerUrl);
        //}
        //return BackendClientSingleton.client;
    }
    static setAuthToken(token) {
        this.bearerToken = token;
    }
    static getAuthToken() {
        return this.bearerToken;
    }
    static getInstance() {
        if (!BackendClientSingleton.instance) {
            BackendClientSingleton.instance = new BackendClientSingleton();
        }
        return BackendClientSingleton.instance;
    }
}
BackendClientSingleton.bearerToken = "";
BackendClientSingleton.backendServerUrl = "http://63.32.159.120:5001"; //UNCOMMENT FOR DEV
//# sourceMappingURL=BackendClientSingleton.js.map