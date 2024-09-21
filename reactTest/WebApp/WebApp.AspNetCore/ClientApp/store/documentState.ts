import { Action, Reducer, ActionCreator } from 'redux';
import * as Immutable from "seamless-immutable";
import { AppThunkAction } from './';
import * as backendTypes from '../backendTypes';
import { BackendClientSingleton } from '../BackendClientSingleton';
import { KnownCorporateActions } from './corporateState';
import { KnownBankAction } from './bankState';

export class DocumentState {
    documentList: DocumentUi[];
    selectedDocument: DocumentUi;
    documentsLoading: {};
    downloadedDocuments: {};
}

//TODO: remove this and replace with backendTypes.Document
//Also replace Chat type
export class DocumentUi {
    id: string;
    accountId: string;
    icon: string;
    title: string;
    owner: string;
    type: string;
    daysOverdue: number;
    status: backendTypes.StatusEx;
    documentBase64: string;
    documentContentId: string;
    chatHistory: ChatUI[];
    Versions: backendTypes.DocumentVersion[];
}

export class ChatUI {
    id: string;
    messages: ChatMessage[];
}

export class ChatMessage {
    caller: string;
    time: string;
    message: string;
}

interface ReceiveDocumentsAction {
    type: 'RECEIVE_DOCUMENTS_LIST',
    accountId: string,
    documents: DocumentUi[]
}

interface AddDocumentAction {
    type: 'ADD_DOCUMENT';
    accountId: string;
    document: DocumentUi;
}

interface SelectDocumentAction {
    type: 'SELECT_DOCUMENT';
    documentIndex: string;
}

interface DeleteDocumentAction {
    type: 'DELETE_DOCUMENT';
    documentId: string;
}

interface UploadDocumentAction {
    type: 'UPLOAD_DOCUMENT';
    documentIndex: string;
    documentBase64: string;
}

interface RetrieveAndUploadSignedDoc {
    type: 'UPLOAD_SIGNED_DOC_RESPONSE';
    documentId: string;
    envelopeId: string;
    content: string;
}

interface UpdateDocumentStatusAction {
    type: 'UPDATE_DOCUMENT_STATUS';
    documentIndex: string;
    documentStatus: backendTypes.StatusEx;
}

export interface AddChatMessageAction {
    type: 'ADD_CHAT_MESSAGE';
    documentIndex: string;
    fromUser: string;
    chatMessage: string;
}

interface ReceiveContentAction {
    type: 'RECEIVE_CONTENT_RESPONSE';
    documentId: string;
    content: string;
    contentId: string;
}

interface SignDocument {
    type: 'SIGN_DOCUMENT';
    documentId: string;
}

interface SetDocumentLoading {
    type: 'SET_LOADING';
    documentId: string;
}

interface SetDocumentNotLoading {
    type: 'SET_NOT_LOADING';
    documentId: string;
}


interface ClearDocuments {
    type: 'CLEAR_DOCUMENTS',
}

export interface UpdateStatusAction {
    type: 'UPDATE_STATUS_MESSAGE';
    account: string,
    document: string,
    corporate: string,
    bank: string,
    documentstatus: backendTypes.Status,
    accountstatus: backendTypes.Status,
    corporatestatus: backendTypes.Status,
    bankstatus: backendTypes.Status,
}

export type KnownDocumentAction = ReceiveDocumentsAction |
    AddDocumentAction |
    DeleteDocumentAction |
    UploadDocumentAction |
    SelectDocumentAction |
    UpdateDocumentStatusAction |
    AddChatMessageAction |
    UpdateStatusAction |
    ReceiveContentAction |
    RetrieveAndUploadSignedDoc |
    SignDocument |
    SetDocumentLoading |
    SetDocumentNotLoading |
    ClearDocuments;

export const actionCreators = {
    addChatMessage: (documentIndex: string, fromUser: string, chatMessage: string): AppThunkAction<KnownDocumentAction> => (dispatch, getState) => {
        // Add message to the document.
        const request = new backendTypes.DocumentChatAppend();
        request.DocumentID = documentIndex;
        request.Message = chatMessage;
        request.From = fromUser;

        // OOR TODO: Use current user, rather than user passed in. This user is the user set when the channel was opened.
        //request.From = currentUser;
        BackendClientSingleton.getClient().put(request).then(response => {
            //We have notified the server. No need to update the UI, as the update will come back from the server
            //and then we will draw the message.
        });
    },
    addDocument: (accountId: string, document: DocumentUi): AppThunkAction<KnownDocumentAction> => (dispatch, getState) => {

       /* const request = new backendTypes.DocumentCreate();
        request.Username = "user";
        request.Document = new backendTypes.Document();
        request.Document.Id = document.id;
        request.Document.Name = document.title;
        request.Document.Status = document.status
        request.Document.Accounts = [accountId];
        request.DocumentContentBase64 = document.documentBase64;

        BackendClientSingleton.getClient().post(request).then(response => {
            dispatch({
                type: 'ADD_DOCUMENT',
                accountId,
                document
            });
        });*/


         // Create the request payload object
        const request = {
            Username: "user",
            Document: {
                Id: document.id,
                Name: document.title,
                Status: document.status,
                Accounts: [accountId],
            },
            DocumentContentBase64: document.documentBase64
        };

        // Prepare the request options for the fetch call
        const requestOptions = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + localStorage.getItem('AuthToken'), // Assuming you're using token-based auth
            },
            body: JSON.stringify(request) // Convert the request payload to a JSON string
        };

        // Replace this URL with your actual API endpoint
        const apiUrl = 'http://localhost:5001/api/v1/documents'; // Modify as needed for your backend route

        // Make the POST request using fetch
        fetch(apiUrl, requestOptions)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Failed to add document');
                }
                return; // response.json(); // Parse the response as JSON
            })
            .then(
                // Dispatch the action to update the state
                dispatch({
                    type: 'ADD_DOCUMENT',
                    accountId,
                    document
                );
            })
            .catch(error => {
                console.error('Error adding document:', error);
            });


    },
    deleteDocument: (id: string): AppThunkAction<KnownDocumentAction> => (dispatch, getState) => {
        const request = new backendTypes.DocumentDelete();
        request.ID = id;
        BackendClientSingleton.getClient().delete(request).then(response => {
            dispatch({ type: 'DELETE_DOCUMENT', documentId: id });
        });
    },
    listDocuments: (accountId: string): AppThunkAction<KnownDocumentAction> => (dispatch, getState) => {
        const request = new backendTypes.DocumentList();
        request.AccountID = accountId;

        BackendClientSingleton.getClient().get(request)
            .then(response => {
                let docList = new Array<DocumentUi>();
                for (let doc of response as backendTypes.Document[]) {

                    const hasVersions = doc.Versions && doc.Versions.length > 0
                    const contentId = hasVersions
                        ? doc.Versions[doc.Versions.length - 1].DocumentContentId
                        : null;

                    const docEntry = new DocumentUi();
                    docEntry.title = doc.Name;
                    docEntry.daysOverdue = 0;
                    docEntry.accountId = accountId;
                    docEntry.status = doc.Status;
                    docEntry.id = doc.Id;
                    docEntry.documentContentId = contentId;
                    docEntry.Versions = doc.Versions;
                    docEntry.documentBase64 = "";

                    if (doc.Chats != null) {
                        docEntry.chatHistory = doc.Chats.map(chat => {
                            const newChatUi = new ChatUI();
                            newChatUi.messages = chat.ChatMessages.map(chatMessage => {
                                const message = new ChatMessage();
                                message.caller = chatMessage.FromUserId;
                                message.time = chatMessage.Time;
                                message.message = chatMessage.Message;
                                return message;
                            });
                            return newChatUi;
                        })
                    }
                    docList.push(docEntry);
                }
                dispatch({ type: 'RECEIVE_DOCUMENTS_LIST', accountId, documents: docList });
            });
    },
    selectDocument: (documentIndex: string): AppThunkAction<KnownDocumentAction> => (dispatch, getState) => {
        const request = new backendTypes.DocumentChatCreate();
        request.DocumentID = documentIndex;
        //TODO: needs value
        request.Caller = "";

        BackendClientSingleton.getClient().post(request).then(response => {
            dispatch({
                type: 'SELECT_DOCUMENT',
                documentIndex
            });
        });
    },
    uploadDocument: (documentIndex: string, documentBase64: string): AppThunkAction<KnownDocumentAction> => (dispatch, getState) => {
        const request = new backendTypes.DocumentUpload();
        request.DocumentID = documentIndex;
        request.DocumentContentBase64 = documentBase64;
        BackendClientSingleton.getClient().post(request).then(response => {
            dispatch({
                type: 'UPLOAD_DOCUMENT',
                documentIndex,
                documentBase64
            });
        });
    },
    uploadSignedDocument: (documentId: string, envelopeId: string): AppThunkAction<KnownDocumentAction> => (dispatch, getState) => {
        const request = new backendTypes.DocumentUploadSigned;
        request.DocumentId = documentId
        request.EnvelopeId = envelopeId;
        BackendClientSingleton.getClient().post(request).then(response => {
            dispatch({
                type: 'UPLOAD_SIGNED_DOC_RESPONSE',
                documentId,
                envelopeId,
                content: response
            });
        }).catch(error => {
            //TODO: Handle error here
            console.log(error);
        });

    },
    signDocument: (documentId: string): AppThunkAction<KnownDocumentAction> => (dispatch, getState) => {
        const request = new backendTypes.EnvelopeCreate;
        request.DocumentId = documentId;
        //start the spinner
        dispatch({
            type: 'SET_LOADING',
            documentId
        });
        BackendClientSingleton.getClient().post(request).then(response => {
            dispatch({
                type: 'SIGN_DOCUMENT',
                documentId
            });
            window.location.href = response;
        }).catch(error => {
            //TODO: Handle error here
            console.log(error);
        });

    },
    updateDocumentStatus: (documentIndex: string, documentStatus: backendTypes.StatusEx): AppThunkAction<KnownDocumentAction> => (dispatch, getState) => {
        const request = new backendTypes.ChangeDocumentStatus();
        request.Status = documentStatus;
        request.ID = documentIndex;

        BackendClientSingleton.getClient().put(request).then(response => {
            dispatch({
                type: 'UPDATE_DOCUMENT_STATUS',
                documentIndex,
                documentStatus
            });
        });
    },
    requestContent: (documentId: string, contentId: string, ): AppThunkAction<KnownDocumentAction> => (dispatch, getState) => {
        const request = new backendTypes.DocumentContentRead();
        request.ID = contentId;

        BackendClientSingleton.getClient().get(request).then(response => {
            //get content
            const content = response.ContentBase64;

            dispatch({
                type: 'RECEIVE_CONTENT_RESPONSE',
                documentId,
                content,
                contentId
            });
        });
    },
    setDocumentToLoading: (documentId: string): AppThunkAction<KnownDocumentAction> => (dispatch, getState) => {
        dispatch({
            type: 'SET_LOADING',
            documentId
        });
    },
    clearDocuments: (): AppThunkAction<KnownDocumentAction> => (dispatch, getState) => {
        dispatch({
            type: 'CLEAR_DOCUMENTS'
        });
    },
}

const intialState: DocumentState = {
    documentList: [],
    selectedDocument: new DocumentUi(),
    documentsLoading: {},
    downloadedDocuments: {}
}

export const reducer: Reducer<DocumentState> = (state: DocumentState, action: KnownDocumentAction | KnownCorporateActions | KnownBankAction) => {
    switch (action.type) {
        case "RECEIVE_DOCUMENTS_LIST":
            {
                const newState = new DocumentState();
                newState.documentList = [...action.documents];
                return Immutable.from(state).merge(newState);
            }
        case "ADD_DOCUMENT":
            {
                const newState = Immutable.from(state).asMutable({ deep: true });
                newState.documentList = state.documentList.concat([action.document]);
                return Immutable.from(state).merge(newState);
            }
        case 'SELECT_DOCUMENT':
            {
                const newState = Immutable.from(state).asMutable({ deep: true });
                newState.selectedDocument = state.documentList.filter(doc => action.documentIndex === doc.id)[0];
                return Immutable.from(state).merge(newState);
            }
        case 'DELETE_DOCUMENT':
            {
                const newState = Immutable.from(state).asMutable({ deep: true });
                newState.documentList = state.documentList.filter((doc) => doc.id !== action.documentId);
                return Immutable.from(state).merge(newState);
            }
        case 'UPLOAD_DOCUMENT':
            {
                const newState = Immutable.from(state).asMutable({ deep: true });
                newState.documentList = newState.documentList.map((doc) => {
                    if (doc.id === action.documentIndex) {
                        doc.documentBase64 = action.documentBase64;
                        return doc;
                    }
                    return doc;
                });
                newState.selectedDocument = newState.documentList.filter(doc => state.selectedDocument.id === doc.id)[0];
                return Immutable.from(state).merge(newState);
            }
        case 'UPDATE_DOCUMENT_STATUS':
            {
                const newState = Immutable.from(state).asMutable({ deep: true });
                newState.documentList = state.documentList.map(doc => {
                    if (doc.id === action.documentIndex) {
                        const copy = { ...doc };
                        copy.status = action.documentStatus;
                        return copy
                    }
                    return doc;
                });
                newState.selectedDocument = newState.documentList.filter(doc => state.selectedDocument.id === doc.id)[0];
                return Immutable.from(state).merge(newState);
            }
        case 'ADD_CHAT_MESSAGE':
            {
                const newState = Immutable.from(state).asMutable({ deep: true });
                newState.documentList = newState.documentList.map(document => {
                    if (document.id !== action.documentIndex)
                        return document;

                    var chatMessages = new Array<ChatMessage>();
                    var message = new ChatMessage();
                    message.caller = action.fromUser;
                    message.message = action.chatMessage
                    var date = new Date();
                    const options = { month: 'short', day: 'numeric', hour: "2-digit", minute: "2-digit", hour12: false };
                    message.time = date.toLocaleDateString('en-US', options);

                    if (document.chatHistory) {
                        if (document.chatHistory.length > 0) {
                            var chat = document.chatHistory[document.chatHistory.length - 1];

                            if (chat.messages) {
                                for (var j = 0; j < chat.messages.length; j++) {
                                    chatMessages.push(chat.messages[j]);
                                }
                            }
                            chatMessages.push(message);
                            document.chatHistory[document.chatHistory.length - 1].messages = chatMessages;
                        }
                        else {
                            var chat = new ChatUI();
                            chat.messages = new Array<ChatMessage>();
                            chatMessages.push(message);
                            document.chatHistory.push(chat);
                        }

                    }
                    else {
                        var chatUI = new Array<ChatUI>();
                        chatMessages.push(message);

                        var chat = new ChatUI();
                        chat.messages = chatMessages;
                        chatUI.push(chat);
                        document.chatHistory = chatUI;
                    }
                    return document;
                });
                newState.selectedDocument = newState.documentList.filter(doc => state.selectedDocument.id === doc.id)[0];
                return Immutable.from(state).merge(newState);
            }
        case 'UPDATE_STATUS_MESSAGE':
            {
                const newState = Immutable.from(state).asMutable({ deep: true });
                newState.documentList = newState.documentList.map(doc => {
                    if (doc.id === action.document) {
                        const newStatus = new backendTypes.StatusEx();
                        newStatus.Status = action.documentstatus;
                        doc.status = newStatus;
                        return doc;
                    }
                    return doc;
                });
                newState.selectedDocument = newState.documentList.filter(doc => state.selectedDocument.id === doc.id)[0];
                return Immutable.from(state).merge(newState);
            }
        case 'SELECT_CORPORATE':
            {
                return intialState;
            }
        case 'SELECT_SUBSID':
            {
                return intialState;
            }
        case 'SELECT_BANK':
            {
                return intialState;
            }
        case 'RECEIVE_CONTENT_RESPONSE':
            {
                const newState = Immutable.from(state).asMutable({ deep: true });
                newState.documentList = state.documentList.map((doc) => {
                    if (doc.id === action.documentId) {
                        const copy = { ...doc };
                        copy.documentBase64 = action.content;
                        return copy;
                    }
                    return doc;
                });
                newState.downloadedDocuments[action.contentId] = action.content;
                newState.selectedDocument = newState.documentList.filter(doc => state.selectedDocument.id === doc.id)[0];
                return Immutable.from(state).merge(newState);
            }
        case 'UPLOAD_SIGNED_DOC_RESPONSE':
            {
                const newState = Immutable.from(state).asMutable({ deep: true });
                //Update the downloaded documents list with the content that was just received
                newState.downloadedDocuments[action.documentId] = action.content;
                //Update the document in the state with it's new content
                newState.documentList = state.documentList.map((doc) => {
                    if (doc.id === action.documentId) {
                        const copy = { ...doc };
                        copy.documentBase64 = action.content;
                        //Create a version entry
                        var docVersionInfo = new backendTypes.DocumentVersion();
                        docVersionInfo.DocumentContentId = action.documentId;
                        //TODO: Check how these date strings are being handled, as there's a localisation issue here I think.
                        var date = new Date();
                        docVersionInfo.Creation = date.toLocaleString();
                        var mutableVerions = Immutable.from(copy.Versions).asMutable({ deep: true });
                        mutableVerions.push(docVersionInfo);
                        copy.Versions = mutableVerions;
                        return copy;
                    }
                    return doc;
                });
                newState.selectedDocument = newState.documentList.filter(doc => state.selectedDocument.id === doc.id)[0];
                newState.documentsLoading = {
                    ...state.documentsLoading,
                    [action.documentId]: false
                };
                return Immutable.from(state).merge(newState);
            }
        case 'SIGN_DOCUMENT':
            {
                const newState = Immutable.from(state).asMutable({ deep: true });
                newState.documentList = [...state.documentList];
                newState.selectedDocument = { ...state.selectedDocument };
                newState.documentsLoading = { ...state.documentsLoading };
                return newState;
            }
        case 'SET_LOADING':
            {
                const newState = Immutable.from(state).asMutable({ deep: true });
                newState.documentsLoading = {
                    ...state.documentsLoading,
                    [action.documentId]: true
                };
                return Immutable.from(state).merge(newState);
            }
        case 'CLEAR_DOCUMENTS':
            {
                return intialState;
            }
    }

    return state || intialState;
}
