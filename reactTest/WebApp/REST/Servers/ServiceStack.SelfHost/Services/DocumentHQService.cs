using System;
using System.Collections.Generic;
using System.Net;
using Core.Contracts;
using Core.Repositories;
using Microsoft.Win32;
using Models.DTO.V1;
using DocumentHQ.CommonConfig;
using System.Linq;
using System.Diagnostics;
using DataModels;
using MongoDB.Bson;
using ServiceStack.Auth;
using DataModels.DTO.V1;
using MongoDB.Driver;

namespace ServiceStack.SelfHost.Services
{
    public class DocumentHQService : Service
    {
        //private IDalUoW _uow; //TODO: Fix this to work, as impl is not matching interface.
        private readonly IDocumentHQRepository _repos;
        private readonly IDocuSignService _docusign;
        private readonly IAuthSession _session;

        public IServerEvents ServerEvents { get; set; }
        public IChatHistory ChatHistory { get; set; }


        public DocumentHQService()
        {
            try
            {
                _repos = GetRepository();
                _docusign = GetDocuSignService();
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Unable to get database repository: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
        }
       
        private IDocumentHQRepository GetRepository()
        {
            // Create new SQL repository.
            string defaultDBName = Convert.ToString(Registry.GetValue(CommonGlobals.RegistryKey, "DExchangeDB", "DExchangeDB"));
            string defaultDBPath = Convert.ToString(Registry.GetValue(CommonGlobals.RegistryKey, "DExchangeDBPath", @"C:\DExchange"));

            return new MongoDBRepository(defaultDBName, defaultDBPath, this);
        }

        private IDocuSignService GetDocuSignService()
        {
            //TODO: store values in config
            string docusignUserName = "tony@thedexchange.com";
            string docuSignPassword = "dochq99";
            string apiKey = "87fc7a4b-d26f-4247-a689-bfe258b56880";
            return new DocuSignService(docusignUserName, docuSignPassword, apiKey);
        }


        public UserAuth Get(UserConfig request)
        {
            // Resolve the UserAuthRepository from the container
            var userAuthRepo = ResolveService<IUserAuthRepository>();

            // Get the UserAuth object by userID (assuming it's an integer)
            var userAuth = userAuthRepo.GetUserAuth(request.Id.ToString());

            if (userAuth == null)
            {
                throw HttpError.NotFound("User not found");
            }

            return new UserAuth
            {
                Id = userAuth.Id,
                DisplayName = userAuth.DisplayName,
                Email = userAuth.Email,
                UserName = userAuth.UserName
            };

        }

        /// <summary>
        /// GetBanks.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<Bank> Get(BankList list)
        {
            try
            {
                return new List<Bank>(_repos.GetBanks(list.Filter));
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in GetBanks: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        /// <summary>
        /// Create bank.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public void Post(BankCreate bank)
        {
            try
            {
                _repos.CreateBank(bank.Bank);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in CreateBank: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }


        /// <summary>
        /// Delete bank.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public void Delete(BankDelete bank)
        {
            try
            {
                _repos.DeleteBank(bank.BankID);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in DeleteBank: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        /// <summary>
        /// Get notifications for bank.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<Notification> Get(BankNotifications bank)
        {
            try
            {
                return _repos.GetNotificationsForBank(bank.BankID, bank.MaximumCount);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in GetNotificationsForBank: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        /// <summary>
        /// Send notification for bank.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
       /* public void Put(BankNotification bank)
        {
            try
            {
                _repos.SendNotificationToBank(bank.BankID, bank.Notification);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in SendNotificationToBank: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }*/

        /// <summary>
        /// GetBanks.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<Corporate> Get(CorporatesList list)
        {
            try
            {
                return new List<Corporate>(_repos.GetCorporates(list.Parent));
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in GetCorporates: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        /// <summary>
        /// CorporateFieldDefinitionList.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<FieldDefinition> Get(CorporateFieldDefinitionList request)
        {
            try
            {
                return new List<FieldDefinition>(_repos.GetCorporateFieldDefinitions(request.CorporateID));
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in GetCorporates: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        /// <summary>
        /// CorporateFieldDefinitionGetByID.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public FieldDefinition Get(CorporateFieldDefinitionGetByID request)
        {
            try
            {
                return _repos.GetCorporateFieldDefinitionByID(request.FieldID);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in CorporateFieldDefinitionGetByID: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        /// <summary>
        /// CorporateFieldDefinitionGetByName.
        /// </summary>
        /// <returns></returns>
        public FieldDefinition Get(CorporateFieldDefinitionGetByName request)
        {
            try
            {
                return _repos.GetCorporateFieldDefinitionByName(request.FieldName);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in CorporateFieldDefinitionGetByName: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        /// <summary>
        /// CorporateFieldDefinitionAdd.
        /// </summary>
        /// <returns></returns>
        public void Post(CorporateFieldDefinitionAdd request)
        {
            try
            {
                _repos.CorporateFieldDefinitionAdd(request.CorporateID, request.Definition);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in CorporateFieldDefinitionAdd: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }


        /// <summary>
        /// CorporateFieldDefinitionUpdate.
        /// </summary>
        /// <returns></returns>
        public void Put(CorporateFieldDefinitionUpdate request)
        {
            try
            {
                _repos.CorporateFieldDefinitionUpdate(request.CorporateID, request.Definition);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in CorporateFieldDefinitionUpdate: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        /// <summary>
        /// CorporateAllFieldDefinitionsUpdate.
        /// </summary>
        /// <returns></returns>
        public void Put(CorporateAllFieldDefinitionsUpdate request)
        {
            try
            {
                _repos.UpdateCorporateFieldList(request.CorporateID, request.FieldDefinitions);
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Error in Updating Fields definitinos for Corporate ID : {0} Error: {1}", request.CorporateID, ex.Message);
                EventLogger.LogMessage(errorMsg, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }


        /// <summary>
        /// CorporateFieldDefinitionRemove.
        /// </summary>
        /// <returns></returns>
        public void Delete(CorporateFieldDefinitionRemove request)
        {
            try
            {
                _repos.CorporateFieldDefinitionRemove(request.CorporateID, request.FieldDefinitionName);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in CorporateFieldDefinitionRemove: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        /// <summary>
        /// Get accounts.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<Account> Get(AccountList list)
        {
            try
            {
                return _repos.GetAccounts(list.BankID, list.CorporateID);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in GetAccounts: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        /// <summary>
        /// Get all accounts.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<Account> Get(AccountListAll list)
        {
            try
            {
                return _repos.GetAllAccounts();
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in Get(AccountListAll): " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        /// <summary>
        /// Create account.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public void Post(AccountCreate request)
        {
            try
            {
                _repos.CreateAccount(request.Account, request.PrefillDocuments);

                // If notification is not set, we may have enough information to build it.
                //       Notification notify = _repos.GenerateNotify(request.Username, null, request.Account.Id, Status.Pending_e, NotificationType.StatusCreateAccount_e);

                // Send notification message back to client.
                //     SendNotifyMessage(notify);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in CreateAccount: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        /// <summary>
        /// Get account.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Account Get(AccountRead read)
        {
            try
            {
                return _repos.GetAccount(read.ID);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in GetAccount: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }


        /// <summary>
        /// Update account.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public void Put(AccountUpdate request)
        {
            try
            {
                // Update account.
                _repos.UpdateAccount(request.AccountID, request.Account);

                // If notification is not set, we may have enough information to build it.
                //    Notification notify = _repos.GenerateNotify(request.Username, null, request.Account.Id, Status.Pending_e, NotificationType.StatusChangeAccount_e);

                // Send notification message back to client.
                //  SendNotifyMessage(notify);

            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in UpdateAccount: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        /// <returns></returns>
        public void Post(ChangeAccountStatus request)
        {
            try
            {
                // Update account.
                Notification notify = _repos.ChangeAccountStatus(request.AccountID, request.Status);


                // Send notification message back to client.
                SendNotifyMessage(notify);

            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in AccountUpdateStatus: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        /// <returns></returns>
        public void Put(FilledDocumentUpload request)
        {
            try
            {
                // Update account.
                _repos.FilledDocumentUpload(request.AccountID, request.DocumentName, request.DocumentContent);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in AccountUpdateStatus: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        #region DOCUSIGN_METHODS

        /// <summary>
        /// Create DocuSign Envelope
        /// </summary>
        /// <returns>string</returns>
        public string Post(EnvelopeCreate post)
        {
            try
            {
                IAuthSession session = GetSession();
                   
                Document document = _repos.GetDocument(post.DocumentId);
                UserAuth user =  _repos.GetUserByName("") as UserAuth;

                if (user.FirstName.IsNullOrEmpty() || user.LastName.IsNullOrEmpty())
                    throw new Exception("First and last name are required");

                if (document.Versions.Count == 0)
                    throw new Exception(string.Format("Document with id of {0} has no versions", post.DocumentId));

                string contentIdOfMostRecentVersion = document.Versions.Last().DocumentContentId;
                DocumentContent documentContent = _repos.GetDocumentContents(contentIdOfMostRecentVersion);

                return _docusign.CreateEnvelope(documentContent.ContentBase64, post.DocumentId, document.Name, user);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Could not sign document with id of: " + post.DocumentId + ". Error: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        /// <summary>
        /// Upload a signed document.
        /// </summary>
        /// <returns>string</returns>
        public string Post(DocumentUploadSigned post)
        {
            try
            {
                string content = _docusign.GetSignedDocumentContent(post.DocumentId, post.EnvelopeId);

                // Upload document and save notification to database.
                Notification notify = _repos.UploadDocument(post.DocumentId, content);

                // Send notification message back to client.
                SendNotifyMessage(notify);

                return content; //return signed document to update the UI
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Could upload signed document: " + post.DocumentId + ". Error: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

#endregion

#region DOCUMENT_METHODS
        /// <summary>
        /// Delete account.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public void Delete(AccountDelete delete)
        {
            try
            {
                _repos.DeleteAccount(delete.ID);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in DeleteAccount: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        /// <summary>
        /// Get documents.
        /// </summary>
        /// <param name="list"></param>
        public List<Document> Get(DocumentList list)
        {
            try
            {
                return _repos.GetDocuments(list.AccountID);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in GetDocuments: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }


        /// <summary>
        /// Create document.
        /// </summary>
        /// <param name="list"></param>
        public void Post(DocumentCreate request)
        {
            try
            {
                // Relax requirement for document to have an account.

                request.Document.Status = new StatusEx()
                {
                    Status = !string.IsNullOrEmpty(request.DocumentContentBase64)
                    ? Status.Pending_e
                    : Status.Rejected_e
                };

                string accountName = "";
                if ((null != request.Document.Accounts) && request.Document.Accounts.Count > 0)
                    accountName = request.Document.Accounts[0];
                Notification notify = _repos.CreateDocument(request.Document, accountName, request.DocumentContentBase64);

                // Send notification message back to client.
                SendNotifyMessage(notify);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in CreateDocument: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }


        /// <summary>
        /// Create document.
        /// </summary>
        /// <param name="list"></param>
        public void Post(DocumentUpload request)
        {
            try
            {
                // Upload document and save notification to database.
                Notification notify = _repos.UploadDocument(request.DocumentID, request.DocumentContentBase64);

                // Send notification message back to client.
                SendNotifyMessage(notify);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in DocumentUpload: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        /// <summary>
        /// Get document.
        /// </summary>
        /// <param name="list"></param>
        public Document Get(DocumentRead read)
        {
            try
            {
                return _repos.GetDocument(read.ID);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in GetDocument: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        public DocumentContent Get(DocumentContentRead request)
        {
            try
            {
                return _repos.GetDocumentContents(request.ID);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in GetDocument: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }





        /// <summary>
        /// Delete document.
        /// </summary>
        /// <param name="list"></param>
        public void Delete(DocumentDelete delete)
        {
            try
            {
                _repos.DeleteDocument(delete.ID);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in UpdateAccount: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        /// <summary>
        /// Change document status.
        /// </summary>
        /// <param name="list"></param>
        public void Put(ChangeDocumentStatus changeStatus)
        {
            try
            {
                Notification notify = _repos.ChangeDocumentStatus(changeStatus.Username, changeStatus.ID, changeStatus.Status.Status);

                // Send notification message back to client.
                SendNotifyMessage(notify);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in ChangeDocumentStatus: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        public void Post(DocumentFieldDefinitionSet request)
        {
            try
            {
                _repos.DocumentFieldDefinitionSet(request.CorporateID, request.DocumentID, request.FieldDefinition);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in DocumentFieldDefinitionSet: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Put(DocumentFieldDefinitionRemove request)
        {
            try
            {
                _repos.DocumentFieldDefinitionRemove(request.CorporateID, request.DocumentID, request.FieldDefinitionID);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in DocumentFieldDefinitionRemove: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        /// <summary>
        /// Delete document.
        /// </summary>
        /// <param name="list"></param>
        public void Delete(DocumentVersionDelete delete)
        {
            try
            {
                _repos.DeleteDocumentVersion(delete.DocumentID, delete.VersionID);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in UpdateAccount: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }
        #endregion
        #region LINK_METHODS


        /// <summary>
        /// Delete document.
        /// </summary>
        /// <param name="list"></param>
        public void Put(LinkDocumentToAccount link)
        {
            try
            {
                _repos.LinkDocumentToAccount(link.DocumentID, link.AccountID);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in LinkDocumentToAccount: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        /// <summary>
        /// Delete document.
        /// </summary>
        /// <param name="list"></param>
        public void Put(LinkAccountToBank link)
        {
            try
            {
                _repos.LinkAccountToBank(link.AccountID, link.BankID);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in LinkAccountToBank: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }


        #endregion
        #region CORPORATE_METHODS

        public Corporate Get(CorporateRead request)
        {
            try
            {
                return _repos.GetCorporate(request.CorporateID);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in CorporateRead: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }


        public void Post(CorporateCreate request)
        {
            try
            {
                _repos.CreateCorporate(request.Corporate);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in CorporateCreate: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        public void Delete(CorporateDelete request)
        {
            try
            {
                _repos.DeleteCorporate(request.CorporateID, true);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in CorporateDelete: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        /// <summary>
        /// Get notifications for corporate.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<Notification> Get(CorporateNotifications corporate)
        {
            try
            {
                return _repos.GetNotificationsForCorporate(corporate.CorporateID, corporate.MaximumCount);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in GetNotificationsForCorporate: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        /// <summary>
        /// Send notification for corporate.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public void Put(CorporateNotification corporate)
        {
            try
            {
                _repos.UpdateCorporateNotifications(corporate.CorporateID, corporate.Notification);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in SendNotificationToCorporate: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        #endregion

        #region ACCOUNTTYPE_METHODS

        /// <summary>
        /// Get list of account types by name.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<string> Get(AccountTypeListByID list)
        {
            try
            {
                return _repos.GetAccountTypeIDs(list.BankID);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in SendNotificationToCorporate: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }


        /// <summary>
        /// Create account type.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public void Post(AccountTypeCreate request)
        {
            try
            {
                _repos.CreateAccountType(request.Accounttype);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in AccountTypeCreate: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }


        /// <summary>
        /// Get account type.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public AccountType Get(AccountTypeRead request)
        {
            try
            {
                return _repos.GetAccountType(request.ID);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in AccountTypeCreate: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        /// Get list of account types.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<AccountType> Get(AccountTypeReadAll list)
        {
            try
            {
                return _repos.GetAccountTypes(list.BankID);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in AccountTypeReadAll: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        /// <summary>
        /// Get list of account types.


        /// <summary>
        /// Get account type.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public AccountType Get(AccountTypeReadByName request)
        {
            try
            {
                return _repos.GetAccountTypeByName(request.Name);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in AccountTypeCreate: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        /// <summary>
        /// Get document contents given document ID.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public DocumentContent Get(AccountTypeDocumentContents request)
        {
            try
            {
                return _repos.GetAccountTypeDocumentContents(request.AccountTypeID, request.DocumentID, request.PrefillFields);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in AccountTypeCreate: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        /// <summary>
        /// Delete account type.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public void Delete(AccountTypeDelete request)
        {

            //Check against list
            try
            {
                _repos.DeleteAccountType(request.ID);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in AccountTypeDelete: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        /// <summary>
        /// Update an account type.
        /// </summary>
        /// <param name="accounttype"></param>
        /// <returns></returns>
        public void Put(AccountTypeUpdate request)
        {

            try
            {
                _repos.UpdateAccountType(request.Accounttype);
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Error in AccountTypeUpdate: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        #endregion

        #region CHAT_METHODS

        public void Post(DocumentChatCreate request)
        {
            try
            {
                if (0 == request.Caller.CompareTo(request.DocumentID))
                {
                    Debug.WriteLine("Chat not created, user and document ID match: " + request.DocumentID);
                    return;
                }

                if (string.IsNullOrEmpty(request.DocumentID))
                {
                    Debug.WriteLine("Chat not created, document ID not set for user " + request.Caller);
                    return;
                }


                bool bSubscribed = Subscribed(request.Caller, request.DocumentID);

                if (!bSubscribed)
                {
                    // Call  SubscribeToChannels to create a new channel if it doesn'nt already exist
                    ServerEvents.SubscribeToChannels(request.Caller, new string[] { request.DocumentID });

                    // This is called by the bank.ts on select document.
                    _repos.CreateChat(request.Caller, request.DocumentID);

                    EventLogger.LogMessage("Created chat for " + request.Caller + ", document ID " + request.DocumentID, System.Diagnostics.EventLogEntryType.Information);

                }
                else
                {
                    EventLogger.LogMessage("Chat already exists for " + request.Caller + ", document ID " + request.DocumentID, System.Diagnostics.EventLogEntryType.Information);

                }
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Unable to create chat: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        private bool Subscribed(string sub, string channel)
        {

            List<Dictionary<string, string>> details = ServerEvents.GetAllSubscriptionsDetails();

            bool bFoundExistingSub = false;
            for (int i = 0; i < details.Count; i++)
            {
                Dictionary<string, string> subDetails = details[i];

                if (0 == subDetails["userId"].CompareTo(sub))
                {
                    bFoundExistingSub = true;
                    break;
                }
            }
            return bFoundExistingSub;

        }

        public void Put(DocumentChatAppend request)
        {
            try
            {
                bool bSubscribed = Subscribed(request.From, request.DocumentID);
                if (!bSubscribed)
                {
                    // If it's null, then we subscribe to a channel now.
                    // Call  SubscribeToChannels to create a new channel if it doesn'nt already exist
                    ServerEvents.SubscribeToChannels(request.From, new string[] { request.DocumentID });

                    // This is called by the bank.ts on select document.
                    _repos.CreateChat(request.From, request.DocumentID);

                    EventLogger.LogMessage("Created new chat for " + request.From + ", document ID " + request.DocumentID, System.Diagnostics.EventLogEntryType.Information);
                }

                // Treat document ID as channel.
                var channel = request.DocumentID;

                // Create a DTO ChatMessage to hold all required info about this message
                var msg = new ChatMessage
                {
                    Id = ChatHistory.GetNextMessageId(channel),
                    Channel = request.DocumentID,
                    FromUserId = request.From,
                    Message = request.Message,
                };

                // Notify everyone in the channel for public messages
                //ServerEvents.NotifyChannel(channel, request.Selector, request.Message);
                ServerEvents.NotifyAll(request);

                if (!msg.Private)
                    ChatHistory.Log(channel, msg);


                _repos.AppendToChat(request);


                EventLogger.LogMessage("Appended text for chat for " + request.From + ", document ID " + request.DocumentID, System.Diagnostics.EventLogEntryType.Information);

            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Unable to append to chat: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        public void Put(DocumentChatCancelSubscription request)
        {
            try
            {
                if (null == request.DocumentID)
                {

                    EventLogger.LogMessage("Unable to terminate chat, document ID not set", System.Diagnostics.EventLogEntryType.Information);
                    return;
                }
                // Should subscription id be included here?
                ServerEvents.UnsubscribeFromChannels(request.SubscriptionID, new string[] { request.ChannelID });

                List<Dictionary<string, string>> subInfo = ServerEvents.GetAllSubscriptionsDetails();
                bool bFound = false;
                foreach (Dictionary<string, string> entry in subInfo)
                {
                    if (entry.ContainsKey(request.SubscriptionID))
                    {
                        bFound = true;
                        break;
                    }
                }

                // Check if there are no more subscriptions to that channel, so terminate chat.
                if (!bFound)
                {
                    // Can we check if chat is terminated, i.e. are all subscriptions gone?
                    _repos.TerminateChat(request.SubscriptionID, request.DocumentID);
                }
            }
            catch (Exception ex)
            {
                EventLogger.LogMessage("Unable to terminate chat: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        /// <summary>
        /// Get chat history based on multiple channels.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public object Get(GetChatHistory request)
        {
            var msgs = request.Channels.Map(x =>
                ChatHistory.GetRecentChatHistory(x, request.AfterId, request.Take))
                .SelectMany(x => x)
                .OrderBy(x => x.Id)
                .ToList();

            return new GetChatHistoryResponse
            {
                Results = msgs
            };
        }

        /// <summary>
        /// Send notification message (and store in database).
        /// </summary>
        /// <param name="notify"></param>
        private void SendNotifyMessage(Notification notify)
        {
            if (null == notify)
            {
                EventLogger.LogMessage("null notification encountered, notification will not be sent for operation.", System.Diagnostics.EventLogEntryType.Information);
                return;
            }
            // Notification notify = notification;
            bool bSubscribed = Subscribed(notify.UserName, "");

            string channel = "";
            if ((notify.Type == NotificationType.StatusChangeDocument_e)
                || (notify.Type == NotificationType.StatusCreateDocument_e))
                channel = notify.StatusUpdate.DocumentUpdates[0].ID;

            /* bool bSendToBank = true;
             string corporateID = "";
             if (notify.StatusUpdate.DocumentUpdates[0].Status == Status.Pending_e)
                 bSendToBank = true;
             else
             {
                 bSendToBank = false;
                 corporateID = notify.StatusUpdate.CorporateUpdates[0].ID;
             }*/

            if (!bSubscribed)
            {
                ServerEvents.SubscribeToChannels(notify.UserName, new string[] { channel });
                EventLogger.LogMessage("Created new notification subscription for " + notify.UserName, System.Diagnostics.EventLogEntryType.Information);
            }

            /*    if (bSendToBank)
                    _repos.UpdateBankNotifications(notify.BankID, notify);
                else
                    _repos.UpdateCorporateNotifications(corporateID, notify);
                    */
            // Notify everyone of this update.
            ServerEvents.NotifyAll(notify);
        }


#endregion


#region ROLE_METHODS

        public List<RoleInfo> Get(RoleListGet request)
        {
            return _repos.GetRolesList(request);
        }

        public RoleInfo Get(RoleInfoGet request)
        {
            return _repos.GetRoleByName(request.RoleName);
        }

        public List<RoleInfo> Get(UserRoleInfoGet request)
        {
            return _repos.GetUserRoles(request);
        }

        public SystemConfig Get(SystemConfigGet request)
        {
            return _repos.GetSystemConfig(request);
        }

        public RoleInfo Post(RoleInfoInsert request)
        {
            return _repos.InsertRole(request);
        }

        public void Put(RoleInfoUpdate request)
        {
            _repos.UpdateRole(request);
        }

        public void Delete(RoleInfoDelete request)
        {
            _repos.DeleteRole(request);
        }

#endregion


#region USER_METHODS

        public List<UserAuth> Get(UsersListGet request)
        {
            return _repos.GetUsersList();
        }

        public UserAuth Get(UserInfoGet request)
        {
            var userRepo = this.AuthRepository;
            return userRepo.GetUserAuthByUserName(request.UserName) as UserAuth;
        }

        public void Post(UserInfoPost request)
        {
            if (null == request.Config)
                throw new Exception("User Config must be set");

            // Create user detail (i.e. details of user parent, privilege etc.
            _repos.CreateUser(request.UserToCreate, request.Password, request.Config);
        }

        public void Put(UserInfoPut request)
        {
            _repos.UpdateUser(request.UserToUpdate, request.Config);
            // var userRepo = this.AuthRepository;
            // userRepo.UpdateUserAuth(request.UserToCreate, request.Password);
        }

        public void Delete(UserInfoDelete request)
        {
            _repos.DeleteUser(request.UserToDelete);
        }

        public UserConfig Get(UserConfigGet request)
        {
            var userConfig = _repos.GetUserConfig(request.UserID);
            if ((userConfig.EntityName.ToUpper().CompareTo("ADMIN") == 0) || (userConfig.EntityName.ToUpper().CompareTo("GROUPWIDE") == 0))
            {
                userConfig.UserPrivilege = Privilege.SuperAdmin;
            }
               

            return userConfig;
        }

        #endregion

        #region GROUP_METHODS

         public Group Get(GroupGetByID request)
        {
            return _repos.GetGroupByID(request.GroupID);
        }

        public Group Get(GroupGetByName request)
        {
            return _repos.GetGroupByName(request.GroupName);
        }

        public List<Group> Get(GroupsListGet request)
        {
            return _repos.GetGroupsList();
        }


        public void Post(GroupCreate request)
        {
            _repos.CreateGroup(request.Group);
        }

        public void Delete(GroupDelete request)
        {
            _repos.DeleteGroup(request.GroupId);
        }

        public void Put(AddUserToGroup request)
        {
            _repos.AddUserToGroup(request.Group, request.User);
        }

        public void Put(AddUsersToGroup request)
        {
            _repos.AddUsersToGroup(request.GroupId, request.UserIds);
        }

        public void Delete(DeleteUserFromGroup request)
        {
            _repos.DeleteUserFromGroup(request.GroupId, request.UserId);
        }

#endregion

#region ROLE_METHODS

        public void Put(AddRoleInfoToGroup request)
        {
            _repos.AddRoleInfoToGroup(request.Role, request.Group);
        }

        public void Put(AddRolesToGroup request)
        {
            _repos.AddRolesToGroup(request.RoleIds, request.GroupId);
        }

        public void Delete(RemoveRoleInfoFromGroup request)
        {
            _repos.RemoveRoleInfoFromGroup(request.RoleId, request.GroupId);
        }

        public List<UserAuth> Get(GetUsersForGroup request)
        {
            return _repos.GetUsersForGroup(request.Group);
        }


        public List<RoleInfo> Get(GetRolesForGroup request)
        {
            return _repos.GetRolesForGroup(request.Group);
        }

        public List<Group> Get(GetGroupsForRole request)
        {
            return _repos.GetGroupsForRole(request);
        }


#endregion


#region PERMISSION_METHODS

        public void Put(AddPermissionToRoleInfo request)
        {
            _repos.AddPermissionToRoleInfo(request.Role, request.Permission);
        }

        public void Post(UpdatePermissionInRoleInfo request)
        {
            _repos.UpdatePermissionInRoleInfo(request.Role, request.Permission);
        }

        public void Delete(DeletePermissionFromRoleInfo request)
        {
            _repos.DeletePermissionFromRoleInfo(request.Role, request.PermissionID);
        }


        #endregion

    }
}