using Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using ServiceStack;
using ServiceStack.Auth;
using DocumentHQ.CommonConfig;
using Models.DTO.V1;
using DocuSign.eSign.Model;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;

namespace Core.Repositories
{
    public class DocuSignService : IDocuSignService
    {
        //Note: You only need a DocuSign account to SEND documents,
        // signing is always free and signers do not need an account.
        string _docuSignUserName;
        string _docuSignPassword;
        string _apiKey;
        string _clientId = "1234";
        string _receipientId = "1";
        string _receipientName = "";
        string _receipientEmail = "";


        public DocuSignService(string userName, string password, string apiKey)
        {
            _docuSignUserName = userName;
            _docuSignPassword = password;
            _apiKey = apiKey;
        }

        public string CreateEnvelope(string base64Content, string documentId, string documentName, UserAuth user)
        {
            _receipientName = string.Format("{0} {1}", user.FirstName, user.LastName);
            _receipientEmail = user.Email;

            return CreateReceipientView(base64Content, documentId, documentName);
        }

        public string GetSignedDocumentContent(string documentId, string envelopeId)
        {
            string accountId = Login(_docuSignUserName, _docuSignPassword);

            EnvelopesApi envelopesApi = new EnvelopesApi();
            EnvelopeDocumentsResult docsList = envelopesApi.ListDocuments(accountId, envelopeId);

            //first document in the array in the actual document, other document contains signing information
            MemoryStream docStream = (MemoryStream)envelopesApi.GetDocument(accountId, envelopeId, docsList.EnvelopeDocuments[0].DocumentId);
            byte[] content = docStream.ToArray();
            return string.Concat("data:application/pdf;base64,", Convert.ToBase64String(content));
        }

        private string CreateReceipientView(string base64Content, string documentId, string docName)
        {
            // instantiate api client with appropriate environment (for production change to www.docusign.net/restapi)
            ConfigureApiClient("https://demo.docusign.net/restapi"); //TODO: change for production

            // call the Login() API which sets the user's baseUrl and returns their accountId
            string accountId = Login(_docuSignUserName, _docuSignPassword);

            EnvelopeDefinition envDef = CreateEvelopeDefinition(base64Content, accountId, docName);

            //The envelope holds the document that has been sent to DocuSign for 
            EnvelopesApi envelopesApi = new EnvelopesApi();
            EnvelopeSummary envelopeSummary = envelopesApi.CreateEnvelope(accountId, envDef);

            string url = CommonGlobals.serverUrl + ":5000/signed";

            RecipientViewRequest viewOptions = new RecipientViewRequest()
            {

                ReturnUrl = string.Format("{0}/{1}/{2}",
                url, documentId, envelopeSummary.EnvelopeId),//DocHQ Client

                ClientUserId = _clientId,  // must match clientUserId from earlier step
                AuthenticationMethod = "email",
                UserName = envDef.Recipients.Signers[0].Name,
                Email = envDef.Recipients.Signers[0].Email,
                RecipientId = _receipientId
            };

            ViewUrl recipientView = envelopesApi.CreateRecipientView(accountId, envelopeSummary.EnvelopeId, viewOptions);

            // log the JSON response
            string response = string.Format("ViewUrl:\n{0}", JsonConvert.SerializeObject(recipientView));
            EventLogger.LogMessage(response, System.Diagnostics.EventLogEntryType.Information);

            return recipientView.Url;
        }

        private EnvelopeDefinition CreateEvelopeDefinition(string base64Contents, string accountId, string docName)
        {
            EnvelopeDefinition envDef = new EnvelopeDefinition();
            envDef.EmailSubject = "DocumentationHQ - Please sign this doc";

            // Add a document to the envelope
            DocuSign.eSign.Model.Document doc = new DocuSign.eSign.Model.Document();
            doc.DocumentBase64 = base64Contents.Replace("data:application/pdf;base64,", string.Empty);
            doc.Name = docName;
            doc.DocumentId = "1";
            doc.FileExtension = "pdf";

            envDef.Documents = new List<DocuSign.eSign.Model.Document>();
            envDef.Documents.Add(doc);

            // Add a recipient to sign the documeent
            Signer signer = new Signer();
            signer.Email = _receipientEmail;
            signer.Name = _receipientName;
            signer.RoutingOrder = "1";
            signer.RecipientId = _receipientId;
            signer.ClientUserId = _clientId; // must set |clientUserId| to embed the recipient!

            envDef.Recipients = new Recipients();
            envDef.Recipients.Signers = new List<Signer>();
            envDef.Recipients.Signers.Add(signer);

            // set envelope status to "sent" to immediately send the signature request
            envDef.Status = "sent";
            return envDef;

        }

        private void ConfigureApiClient(string basePath)
        {
            // instantiate a new api client
            ApiClient apiClient = new ApiClient(basePath);

            // set client in global config so we don't need to pass it to each API object.
            Configuration.Default.ApiClient = apiClient;
        }


        private string Login(string usr, string pwd)
        {
            // we set the api client in global config when we configured the client 
            ApiClient apiClient = Configuration.Default.ApiClient;
            string authHeader = "{\"Username\":\"" + usr + "\", \"Password\":\"" + pwd + "\", \"IntegratorKey\":\"" + _apiKey + "\"}";
            Configuration.Default.AddDefaultHeader("X-DocuSign-Authentication", authHeader);

            // we will retrieve this from the login() results
            string accountId = null;

            // the authentication api uses the apiClient (and X-DocuSign-Authentication header) that are set in Configuration object
            AuthenticationApi authApi = new AuthenticationApi();
            LoginInformation loginInfo = authApi.Login();

            // find the default account for this user
            foreach (LoginAccount loginAcct in loginInfo.LoginAccounts)
            {
                if (loginAcct.IsDefault == "true")
                {
                    accountId = loginAcct.AccountId;
                    break;
                }
            }
            if (accountId == null)
            { // if no default found set to first account
                accountId = loginInfo.LoginAccounts[0].AccountId;
            }
            return accountId;
        }
    }
}

