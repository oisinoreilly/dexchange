import { JsonServiceClient } from 'servicestack-client';

export class BackendClientSingleton {
    private static instance: BackendClientSingleton;
    private static client: JsonServiceClient;

    private static bearerToken: string = "";
    private static backendServerUrl:string = "http://localhost:5001"; //UNCOMMENT FOR DEV
    //private static backendServerUrl:string = "http://ec2-63-32-159-120.eu-west-1.compute.amazonaws.com:5001"; //UNCOMMENT FOR DEPLOYMENT
    private constructor() {
    }

    static getClient() {
        if (!(window as any).client) {
            (window as any).client = new JsonServiceClient(this.backendServerUrl);
        }
        ((window as any).client as JsonServiceClient).bearerToken = sessionStorage.getItem("AuthToken");;
        return (window as any).client;
        //TODO: Get the singleton working or look up a better pattern.
        //if (!BackendClientSingleton.client) {
        //    BackendClientSingleton.client = new JsonServiceClient(this.backendServerUrl);
        //}
        //return BackendClientSingleton.client;
    }
     
    static setAuthToken(token: string){
        this.bearerToken = token;
    }

       static getAuthToken(){
        return this.bearerToken;
    }

    static getInstance(): BackendClientSingleton {
        if (!BackendClientSingleton.instance) {
            BackendClientSingleton.instance = new BackendClientSingleton();
        }
        return BackendClientSingleton.instance;
    }
}
