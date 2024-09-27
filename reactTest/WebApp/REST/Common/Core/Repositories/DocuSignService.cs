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
using DocuSign.eSign.Client.Auth;
using ServiceStack.Text;
using System.Security.Cryptography;
using static DocuSign.eSign.Client.Auth.OAuth;
using System.ComponentModel;
using iTextSharp.text.pdf.parser;

namespace Core.Repositories
{
    public class DocuSignService : IDocuSignService
    {
        //Note: You only need a DocuSign account to SEND documents,
        // signing is always free and signers do not need an account.
        string _docuSignUserName;
        string _docuSignPassword;
        string _apiKey;
        string _receipientId = "1";
        string _receipientName = "";
        string _receipientEmail = "";

        private static string _apiBasePath = "https://demo.docusign.net/restapi";
        private static string _clientId = "aaa0311e-9da5-41d1-bdcf-2c9dd4441082"; // Your Integrator Key (Client ID)
        private static string _authServer = "account-d.docusign.com"; // For demo environment
        private static string _privateKeyFilename = "c:\\DeXchange\\data\\private.pem"; // Path to your RSA private key
        private static string _accountId = null; // Will be set after login

     
        /// <summary>
        /// Uses Json Web Token (JWT) Authentication Method to obtain the necessary information needed to make API calls.
        /// </summary>
        /// <returns>Auth token needed for API calls</returns>
        public OAuthToken AuthenticateWithJwt(string api, string clientId, string impersonatedUserId, string authServer, byte[] privateKeyBytes)
        {
            var docuSignClient = new DocuSignClient();
            var scopes = new List<string>
                {
                    "signature",
                    "impersonation",
                };
          
            return docuSignClient.RequestJWTUserToken(
                clientId,
                impersonatedUserId,
                authServer,
                privateKeyBytes,
                1,
                scopes);
        }

        // Example method to send an envelope after authentication
        public static void SendEnvelope()
        {
            if (_accountId == null)
            {
                Console.WriteLine("Error: No account ID found.");
                return;
            }

            // Use the authenticated ApiClient and account ID to send documents, envelopes, etc.
            // Example sending an envelope code would go here
        }

        public DocuSignService(string userName, string password, string apiKey)
        {
            _docuSignUserName = userName;
            _docuSignUserName = "f41a999f-b255-4257-a7df-468675442b5a";
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
            //string accountId = Login(_docuSignUserName, _docuSignPassword);
            OAuthToken accessToken;
            try
            {
                accessToken = AuthenticateWithJwt("ESignature", _clientId, _docuSignUserName, _authServer, File.ReadAllBytes(_privateKeyFilename));
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to authenticate with JWT: " + ex.Message);
            }
            

            ApiClient apiClient = new ApiClient("https://demo.docusign.net/restapi");
            EnvelopesApi envelopesApi = new EnvelopesApi(apiClient);
            EnvelopeDocumentsResult docsList = envelopesApi.ListDocuments(_accountId, envelopeId);

            //first document in the array in the actual document, other document contains signing information
            MemoryStream docStream = (MemoryStream)envelopesApi.GetDocument(_accountId, envelopeId, docsList.EnvelopeDocuments[0].DocumentId);
            byte[] content = docStream.ToArray();
            return string.Concat("data:application/pdf;base64,", Convert.ToBase64String(content));
        }

        private string CreateReceipientView(string base64Content, string documentId, string docName)
        {
            ApiClient apiClient = new ApiClient("https://demo.docusign.net/restapi");
            // instantiate api client with appropriate environment (for production change to www.docusign.net/restapi)
            //  ConfigureApiClient("https://demo.docusign.net/restapi"); //TODO: change for production

            // call the Login() API which sets the user's baseUrl and returns their accountId
            // string accountId = Login(_docuSignUserName, _docuSignPassword);

            //string accountId = Login(_docuSignUserName, _docuSignPassword);
            OAuthToken accessToken;
            try
            {
                accessToken = AuthenticateWithJwt("ESignature", _clientId, _docuSignUserName, _authServer, File.ReadAllBytes(_privateKeyFilename));
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to authenticate with JWT: " + ex.Message);
            }




            EnvelopeDefinition envDef = CreateEvelopeDefinition(base64Content, docName);

            //The envelope holds the document that has been sent to DocuSign for 
            EnvelopesApi envelopesApi = new EnvelopesApi(apiClient);
            EnvelopeSummary envelopeSummary = envelopesApi.CreateEnvelope(_accountId, envDef);

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

            ViewUrl recipientView = envelopesApi.CreateRecipientView(_accountId, envelopeSummary.EnvelopeId, viewOptions);

            // log the JSON response
            string response = string.Format("ViewUrl:\n{0}", JsonConvert.SerializeObject(recipientView));
            EventLogger.LogMessage(response, System.Diagnostics.EventLogEntryType.Information);

            return recipientView.Url;
        }

        private EnvelopeDefinition CreateEvelopeDefinition(string base64Contents,string docName)
        {
            EnvelopeDefinition envDef = new EnvelopeDefinition();
            envDef.EmailSubject = "DeXchange - Please sign this doc";

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
           // Configuration.Default.ApiClient = apiClient;
        }


       /* private string Login(string usr, string pwd)
        {
            // we set the api client in global config when we configured the client 
           // ApiClient apiClient = Configuration.Default.ApiClient;

            ApiClient apiClient = new ApiClient("https://demo.docusign.net/restapi");

            string authHeader = "{\"Username\":\"" + usr + "\", \"Password\":\"" + pwd + "\", \"IntegratorKey\":\"" + _apiKey + "\"}";
            Configuration.Default.AddDefaultHeader("X-DocuSign-Authentication", authHeader);

            // we will retrieve this from the login() results
            string accountId = null;

            // the authentication api uses the apiClient (and X-DocuSign-Authentication header) that are set in Configuration object
            AuthenticationApi authApi = new AuthenticationApi(apiClient);
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
        }*/
    }


    public enum ExamplesApiType
    {
        /// <summary>
        /// Rooms API
        /// </summary>
        [Description("reg")]
        Rooms = 0,

        /// <summary>
        /// ESignature API
        /// </summary>
        [Description("eg")]
        ESignature = 1,

        /// <summary>
        /// Click API
        /// </summary>
        [Description("ceg")]
        Click = 2,

        /// <summary>
        /// Monitor API
        /// </summary>
        [Description("meg")]
        Monitor = 3,

        /// <summary>
        /// Admin API
        /// </summary>
        [Description("aeg")]
        Admin = 4,

        /// <summary>
        /// Connect API
        /// </summary>
        [Description("con")]
        Connect = 5,

        /// <summary>
        /// Web Forms API
        /// </summary>
        [Description("web")]
        WebForms = 6,
    }

    public static class ExamplesApiTypeExtensions
    {
        public static string ToKeywordString(this ExamplesApiType val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
}




