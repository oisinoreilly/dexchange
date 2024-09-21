using Core.Contracts;
using DataModels;
using Models.DTO.V1;
using ServiceStack.Auth;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using DocumentHQ.CommonConfig;
using System.Reflection;
using ServiceStack;
using DataModels.DTO.V1;

using System.IO;

using iTextSharp;
using System.Collections;

namespace Core.Repositories
{
    public class MongoDBRepository : IDocumentHQRepository
    {

        protected static IMongoClient _client;
        protected static IMongoDatabase _database;
        protected IAuthSession m_session = null;
        protected Service m_service = null;

        string _dbName = "";
        string _dbLocation = "";
        public MongoDBRepository(string dbName, string dbLocation, Service svc)
        {
            // Specify settings for database connection.
            // If database doesn't exist, create it.
            _dbName = dbName;
            _dbLocation = dbLocation;
            m_service = svc;

            _client = new MongoClient("mongodb://localhost:27019/?safe=true");
            _database = _client.GetDatabase("DocumentHQ");

        }
        #region BANK_METHODS
        // Bank entries.
        public List<Bank> GetBanks(string filter)
        {
            // Get banks.
            List<Bank> banks = GetColl<Bank>(CommonGlobals.BanksCollectionName);

            List<Bank> retList = new List<Bank>();
            for (int i = 0; i < banks.Count; i++)
            {
                // Perform read check for each account.
                try
                {
                    PerformSecurityCheck(Access.Read, banks[i].Id, Resource.Bank, null);
                    retList.Add(banks[i]);

                }
                catch (Exception) { }
            }

            Bank global = new Bank();
            global.Id = CommonGlobals.GlobalIdentifier;
            global.Status = new StatusEx() { Status = Status.Approved_e };
            global.Accounts = new List<string>();
            global.Name = "Groupwide";
            retList.Add(global);
            return retList;
        }

        public Bank GetBank(string ID)
        {
            return GetObjectByFieldID<Bank>(CommonGlobals.BanksCollectionName, CommonGlobals.IdFieldName, ID);
        }

        public void CreateBank(Bank bank)
        {
            Bank bank2 = null;
            try
            {

                bank2 = GetObjectByFieldID<Bank>(CommonGlobals.BanksCollectionName, CommonGlobals.IdFieldName, bank.Id);

            }
            catch (Exception) { }

            if (null != bank2)
                throw new Exception("Bank with ID " + bank.Id + " already exists");

            if (null == bank.Notifications)
            {
                bank.Notifications = new List<string>();
            }

            if (null == bank.Status)
            {
                bank.Status = new StatusEx { Status = Status.Pending_e };
            }

            if (null == bank.Accounts)
            {
                bank.Accounts = new List<string>();
            }

            IMongoCollection<Bank> banks = _database.GetCollection<Bank>(CommonGlobals.BanksCollectionName);
            banks.InsertOne(bank);
        }

        /// <summary>
        /// Delete bank.
        /// </summary>
        /// <param name="bankID"></param>
        public void DeleteBank(string bankID)
        {
            Bank bank2 = null;

            try
            {
                bank2 = GetObjectByFieldID<Bank>(CommonGlobals.BanksCollectionName, CommonGlobals.IdFieldName, bankID);
            }
            catch (Exception)
            {
                throw new Exception("Bank with ID " + bankID + " not found");
            }

            // Perform security check.
            PerformSecurityCheck(Access.Delete, bankID, Resource.Bank, bankID);

            // Get accounts.
            IMongoCollection<Bank> banks = _database.GetCollection<Bank>(CommonGlobals.BanksCollectionName);
            var bankfilter = Builders<Bank>.Filter.Eq(CommonGlobals.IdFieldName, bankID);

            var accountColl = _database.GetCollection<Account>(CommonGlobals.AccountsCollectionName);
            var filter = Builders<Account>.Filter.Eq(CommonGlobals.ParentID, bankID);

            // Find all accounts that use this bank.
            List<Account> accounts = accountColl.Find(filter).ToList();

            // Iterate through accounts that used this bank, and delete them.
            foreach (Account account in accounts)
            {
                // Delete account. This will also remove document references.
                DeleteAccount(account.Id);
            }

            // Perform bank deletion.
            DeleteResult res = banks.DeleteOne(bankfilter);
            if (res.DeletedCount == 0)
            {
                throw new Exception("Bank with ID " + bankID + " not deleted");
            }

        }

        /// <summary>
        /// Get notifications for bank.
        /// </summary>
        /// <param name="bankID"></param>
        /// <returns></returns>
        public List<Notification> GetNotificationsForBank(string bankID, int maximumCount)
        {
            // TODO: Once the client can identify the corporate ID when making this call we will return 
            // notificaitons for the corporate. For now, we return all notifications for any corporate.
            // Get collection.
            IMongoCollection<Notification> coll = _database.GetCollection<Notification>("notifications");

            // Only return "Approved" or "Rejected" notifications.
            List<Notification> notifications = coll.Find(i => (i.BankID != "")).ToList();
            List<Notification> notificationList = new List<Notification>();

            foreach (Notification notify in notifications)
            {
                if ((null != notify.StatusUpdate) && (notify.StatusUpdate.DocumentUpdates.Count > 0))
                {
                    StatusCouple status = notify.StatusUpdate.DocumentUpdates[0];
                    if (status.Status == Status.Pending_e)
                    {
                        notificationList.Add(notify);
                    }
                }
            }

            int nMinIndex = 0;
            int nMaxIndex = notifications.Count;

            if (maximumCount < nMaxIndex)
            {
                nMinIndex = nMaxIndex - maximumCount;
            }

            // Add notifications to list up to the maximum count.
            List<Notification> retList = new List<Notification>();
            for (int i = nMinIndex; i < nMaxIndex; i++)
            {
                retList.Add(notifications[i]);

            }
            return retList;

        }

        /// <summary>
        /// Send message to bank.
        /// </summary>
        /// <param name="bankID"></param>
        /// <param name="message"></param>
        public void UpdateBankNotifications(string bankID, Notification notification)
        {
            // Get notifications.
            IMongoCollection<Notification> notificationColl = _database.GetCollection<Notification>("notifications");

            // TODO: fix situations where this is not set.
            if (!string.IsNullOrEmpty(bankID))
            {
                try
                {
                    // Set processed flag for all notifications for this bank.
                    if (!string.IsNullOrEmpty(notification.DocumentID))
                    {
                        var notificationFilter = Builders<Notification>.Filter.Eq("DocumentID", notification.DocumentID);
                        List<Notification> accounts = notificationColl.Find(notificationFilter).ToList();
                        var updateNotificationOperation = Builders<Notification>.Update
                            .Set("Processed", true);
                        // Perform update.
                        notificationColl.UpdateManyAsync(notificationFilter, updateNotificationOperation);
                    }

                    // Insert notification.
                    notificationColl.InsertOne(notification);

                    // Now add the notification to the Bank.
                    var updateOperation = Builders<Bank>.Update.PushEach<string>("Notifications", new List<string> { notification.Id });
                    var filter = Builders<Bank>.Filter.Eq(CommonGlobals.IdFieldName, bankID);
                    IMongoCollection<Bank> bankColl = _database.GetCollection<Bank>(CommonGlobals.BanksCollectionName);
                    var result = bankColl.UpdateOneAsync(filter, updateOperation);
                }
                catch (Exception ex)
                {
                    // The bank ID may not be set.
                }
            }
        }
        #endregion

        #region CORPORATE_METHODS

        // Corporate entries.
        public List<Corporate> GetCorporates(string parent)
        {
            // Get corporates.
            //   List < Corporate > corporates = GetColl<Corporate>("corporates");

            List<Corporate> corporates = null;
            if (string.IsNullOrEmpty(parent))
            {
                corporates = GetColl<Corporate>(CommonGlobals.CorporatesCollectionName);
            }
            else
            {
                // If parent has been specified we need corporates who's parent matches parameter.
                var corporateFilter = Builders<Corporate>.Filter.Eq("ParentID", parent);
                IMongoCollection<Corporate> corporateColl = _database.GetCollection<Corporate>(CommonGlobals.CorporatesCollectionName);
                corporates = corporateColl.Find(corporateFilter).ToList();
            }

            List<Corporate> retList = new List<Corporate>();
            for (int i = 0; i < corporates.Count; i++)
            {
                // Perform read check for each account.
                try
                {
                    PerformSecurityCheck(Access.Read, corporates[i].Id, Resource.Corporate, null);
                    retList.Add(corporates[i]);

                }
                catch (Exception) { }
            }

          
            return retList;
        }

        // Get corporate.
        public Corporate GetCorporate(string ID)
        {
            return GetObjectByFieldID<Corporate>(CommonGlobals.CorporatesCollectionName, CommonGlobals.IdFieldName, ID);
        }

        // Create corporate.
        public void CreateCorporate(Corporate corporate)
        {
            IMongoCollection<Corporate> corporates = _database.GetCollection<Corporate>(CommonGlobals.CorporatesCollectionName);


            if (null == corporate.Status)
            {
                corporate.Status = new StatusEx { Status = Status.Pending_e };
            }

            if (null == corporate.Children)
            {
                corporate.Children = new List<string>();
            }

            if (null == corporate.Notifications)
            {
                corporate.Notifications = new List<string>();
            }

            if (null == corporate.Accounts)
            {
                corporate.Accounts = new List<string>();
            }

            corporates.InsertOne(corporate);

            // Now we need to add corporate to its parent's children.
            string parentID = corporate.ParentID;
            if (!string.IsNullOrEmpty(parentID))
            {
                // Get parent corporate by ID.
                Corporate parentCorporate = GetObjectByFieldID<Corporate>(CommonGlobals.CorporatesCollectionName, CommonGlobals.IdFieldName, parentID);

                // Now add the notification to the Corporate.
                var updateOperation = Builders<Corporate>.Update.PushEach<string>("Children", new List<string> { corporate.Id });
                var filter = Builders<Corporate>.Filter.Eq(CommonGlobals.IdFieldName, parentID);
                var result = corporates.UpdateOneAsync(filter, updateOperation);
            }
        }

        // Delete corporate.
        public void DeleteCorporate(string corporateID, bool bDeleteFromParentsChildren)
        {
            Corporate corporate = GetObjectByFieldID<Corporate>(CommonGlobals.CorporatesCollectionName, CommonGlobals.IdFieldName, corporateID);
            if (null == corporate)
                throw new Exception("Corporate with ID " + corporateID + " not found");

            // Security check for deleting corporate.
            PerformSecurityCheck(Access.Delete, corporate.Id, Resource.Corporate, corporate.Id);

            // Delete children first.
            List<string> children = corporate.Children;

            var corporatefilter = Builders<Corporate>.Filter.Eq(CommonGlobals.IdFieldName, corporateID);
            IMongoCollection<Corporate> corporates = _database.GetCollection<Corporate>(CommonGlobals.CorporatesCollectionName);
            DeleteResult res = corporates.DeleteOne(corporatefilter);
            if (res.DeletedCount == 0)
            {
                throw new Exception("Corporate with ID " + corporateID + " not deleted");
            }

            if (null != children)
            {
                for (int i = 0; i < children.Count; i++)
                    DeleteCorporate(children[i], false);
            }

            // Remove from parent's children if we are working down the hierarchy.
            string parentID = corporate.ParentID;

            if (bDeleteFromParentsChildren && !string.IsNullOrEmpty(parentID))
            {
                // Update children.
                var parentfilter = Builders<Corporate>.Filter.Eq(CommonGlobals.IdFieldName, parentID);
                var updateOperation = Builders<Corporate>.Update.Pull<string>("Children", corporateID);
                var result = corporates.UpdateOneAsync(parentfilter, updateOperation);
            }

            var accountColl = _database.GetCollection<Account>(CommonGlobals.AccountsCollectionName);
            var filter = Builders<Account>.Filter.Eq("CorporateID", corporateID);

            // Find all accounts for this corporate.
            List<Account> accounts = accountColl.Find(filter).ToList();

            // Iterate through accounts that used this bank, and delete them.
            foreach (Account account in accounts)
            {
                // Delete account. This will also remove document references.
                DeleteAccount(account.Id);
            }
            // Corporate has been deleted. Now remove from parent.
        }

        // Get notifications for corporate.
        public List<Notification> GetNotificationsForCorporate(string corporateID, int maximumCount)
        {
            // TODO: Once the client can identify the corporate ID when making this call we will return 
            // notificaitons for the corporate. For now, we return all notifications for any corporate.
            // Get collection.
            IMongoCollection<Notification> coll = _database.GetCollection<Notification>("notifications");

            // Only return "Approved" or "Rejected" notifications.
            List<Notification> notifications = coll.Find(i => (i.BankID != "")).ToList();
            List<Notification> ret = new List<Notification>();

            foreach (Notification notify in notifications)
            {
                if ((null != notify.StatusUpdate) && (notify.StatusUpdate.DocumentUpdates.Count > 0))
                {
                    StatusCouple status = notify.StatusUpdate.DocumentUpdates[0];
                    if ((status.Status == Status.Approved_e) || (status.Status == Status.Rejected_e))
                    {
                        ret.Add(notify);
                    }

                }
            }

            int nMinIndex = 0;
            int nMaxIndex = notifications.Count;

            if (maximumCount < nMaxIndex)
            {
                nMinIndex = nMaxIndex - maximumCount;
            }

            // Add notifications to list up to the maximum count.
            List<Notification> retList = new List<Notification>();
            for (int i = nMinIndex; i < nMaxIndex; i++)
            {
                retList.Add(notifications[i]);

            }
            return retList;
        }

        /// <summary>
        /// Get field definitions associated with corporate.
        /// </summary>
        /// <param name="corporateID"></param>
        /// <returns></returns>
        public List<FieldDefinition> GetCorporateFieldDefinitions(string corporateID)
        {
            Corporate corporate = GetObjectByFieldID<Corporate>(CommonGlobals.CorporatesCollectionName, CommonGlobals.IdFieldName, corporateID);
            if (null == corporate)
                throw new Exception("Corporate with ID " + corporateID + " not found");

            if (null == corporate.Fields)
                return new List<FieldDefinition>();
            return corporate.Fields;
        }

        public FieldDefinition GetCorporateFieldDefinitionByID(string fieldID)
        {
            return null;
        }


        public FieldDefinition GetCorporateFieldDefinitionByName(string fieldName)
        {
            return null;
        }


        public void CorporateFieldDefinitionAdd(string corporateID, FieldDefinition definition)
        {
            Corporate corporate = GetObjectByFieldID<Corporate>(CommonGlobals.CorporatesCollectionName, CommonGlobals.IdFieldName, corporateID);
            if (null == corporate)
                throw new Exception("Corporate with ID " + corporateID + " not found");

            List<FieldDefinition> fields = corporate.Fields;
            if (null == fields)
                fields = new List<FieldDefinition>();
            fields.Add(definition);


            UpdateCorporateFieldList(corporateID, fields);

        }

        public void UpdateCorporateFieldList(string corporateID, List<FieldDefinition> fields)
        {
            Corporate corporate = GetObjectByFieldID<Corporate>(CommonGlobals.CorporatesCollectionName, CommonGlobals.IdFieldName, corporateID);
            if (null == corporate)
                throw new Exception("Corporate with ID " + corporateID + " not found");

            // Update Fields list for this corporate.
            var collection = _database.GetCollection<Corporate>(CommonGlobals.CorporatesCollectionName);
            var filter = Builders<Corporate>.Filter.Eq(CommonGlobals.IdFieldName, corporate.Id);
            var updateOperation = Builders<Corporate>.Update
                .Set("Fields", fields);

            // Perform update.
            var result = collection.UpdateOneAsync(filter, updateOperation);
        }

        public void CorporateFieldDefinitionUpdate(string corporateID, FieldDefinition definition)
        {
            Corporate corporate = GetObjectByFieldID<Corporate>(CommonGlobals.CorporatesCollectionName, CommonGlobals.IdFieldName, corporateID);
            if (null == corporate)
                throw new Exception("Corporate with ID " + corporateID + " not found");

            List<FieldDefinition> fields = corporate.Fields;
            bool bFound = false;
            foreach (FieldDefinition field in fields)
            {
                if (0 == field.Name.CompareTo(definition.Name))
                {
                    bFound = true;
                    field.DefaultValue = definition.DefaultValue;
                    break;
                }
            }

            if (!bFound)
                throw new Exception("Unable to find field definition with this name");

            UpdateCorporateFieldList(corporateID, fields);
        }

        public void CorporateFieldDefinitionRemove(string corporateID, string fielddefinitionName)
        {
            Corporate corporate = GetObjectByFieldID<Corporate>(CommonGlobals.CorporatesCollectionName, CommonGlobals.IdFieldName, corporateID);
            if (null == corporate)
                throw new Exception("Corporate with ID " + corporateID + " not found");

            List<FieldDefinition> fields = corporate.Fields;
            List<FieldDefinition> newFieldList = new List<FieldDefinition>();
            bool bFound = false;
            foreach (FieldDefinition field in fields)
            {
                if (0 != field.Name.CompareTo(fielddefinitionName))
                {
                    newFieldList.Add(field);
                }
                else
                {
                    bFound = true;
                }
            }

            if (!bFound)
                throw new Exception("Unable to find field definition with this name");

            UpdateCorporateFieldList(corporateID, newFieldList);
        }

        /// <summary>
        /// Send message to corporate.
        /// </summary>
        /// <param name="corporateID"></param>
        /// <param name="message"></param>
        public void UpdateCorporateNotifications(string corporateID, Notification notification)
        {
            // Get bank by ID.
            Corporate corporate = GetObjectByFieldID<Corporate>(CommonGlobals.CorporatesCollectionName, CommonGlobals.IdFieldName, corporateID);

            IMongoCollection<Notification> notificationColl = _database.GetCollection<Notification>("notifications");

            // TODO: fix situations where this is not set.
            if (!string.IsNullOrEmpty(corporateID))
            {
                try
                {
                    // Get name for corporate. This is used to get the notifications by corporate name.
                    string corporateName = corporate.Detail.Name;

                    // Set processed flag for all notifications for this bank.
                    if (!string.IsNullOrEmpty(notification.DocumentID))
                    {
                        var notificationFilter = Builders<Notification>.Filter.Eq("DocumentID", notification.DocumentID);
                        List<Notification> accounts = notificationColl.Find(notificationFilter).ToList();
                        var updateNotificationOperation = Builders<Notification>.Update
                            .Set("Processed", true);
                        // Perform update.
                        notificationColl.UpdateManyAsync(notificationFilter, updateNotificationOperation);
                    }

                    // Insert notification.
                    notificationColl.InsertOne(notification);

                    // Now add the notification to the Corporate.
                    var updateOperation = Builders<Corporate>.Update.PushEach<string>("Notifications", new List<string> { notification.Id });
                    var filter = Builders<Corporate>.Filter.Eq(CommonGlobals.IdFieldName, corporateID);
                    IMongoCollection<Corporate> corporateColl = _database.GetCollection<Corporate>(CommonGlobals.CorporatesCollectionName);
                    var result = corporateColl.UpdateOneAsync(filter, updateOperation);
                }
                catch (Exception ex)
                {

                }
            }
        }

        #endregion

        #region ACCOUNTS

        public List<Account> GetAccounts(string bankID, string corporateId)
        {
            // Get accounts.
            IMongoCollection<Account> accounts = _database.GetCollection<Account>(CommonGlobals.AccountsCollectionName);
            List<Account> accountList = null;
            // Check for global first.
            if (((null != bankID) && (0 == bankID.CompareTo(CommonGlobals.GlobalIdentifier))
                || ((null != corporateId) && (0 == corporateId.CompareTo(CommonGlobals.GlobalIdentifier)))))
            {
                // Global identifier, add "dummy" global accounts view.
                Account account = new Account();
                account.Id = CommonGlobals.GlobalIdentifier;
                account.ParentID = CommonGlobals.GlobalIdentifier;
                account.CorporateId = CommonGlobals.GlobalIdentifier;
                account.Detail = new AccountDetail();
                account.Documents = new List<string>();
                account.Creation = DateTime.Now.ToString();
                account.Name = "Groupwide";
                account.Documents = new List<string>();
                account.Status = new StatusEx() { Status = Status.Approved_e };
                // Return list with one entry.
                accountList = new List<Account>
                {
                    account
                };
                return accountList;
            }
            else
            {
                // We are looking for accounts by Bank ID and CorporateId.
                // Define filter.
                var bankfilter = Builders<Account>.Filter.Eq(CommonGlobals.ParentID, bankID);
                var corporateFilter = Builders<Account>.Filter.Eq("CorporateId", corporateId);

                var filterdef = Builders<Account>.Filter.And(bankfilter, corporateFilter);

                // Get accounts that match this filter.
                accountList = accounts.Find(filterdef).ToList();

                // Get parent, based on whether bank id or corporate ID were specified.
                string parentID = (bankID != null) ? bankID : corporateId;

                List<Account> retList = new List<Account>();
                for (int i = 0; i < accountList.Count; i++)
                {
                    // Perform read check for each account.
                    try
                    {
                        //Perform Security check.
                        try
                        {
                            PerformSecurityCheck(Access.Read, accountList[i].Id, Resource.Account, parentID);
                            retList.Add(accountList[i]);
                        }
                        catch (Exception)
                        {
                            //continue;
                        }
                    }
                    catch (Exception) { }
                }
                return retList;

            }
        }

        public List<Account> GetAllAccounts()
        {
            IMongoCollection<Account> accounts = _database.GetCollection<Account>("accounts");
            return accounts.AsQueryable().ToList();
        }

        // Account entries.
        public List<Account> GetAccountsForCorporate(string corporateID)
        {
            // Get accounts.
            IMongoCollection<Account> accounts = _database.GetCollection<Account>(CommonGlobals.AccountsCollectionName);

            // Define filter.
            var filterdef = Builders<Account>.Filter.Eq("CorporateId", corporateID);

            // Get accounts that match this filter.
            return accounts.Find(filterdef).ToList();
        }

        /// <summary>
        /// Create account.
        /// </summary>
        /// <param name="account"></param>
        public void CreateAccount(Account account, bool prefillDocuments)
        {
            // List has been created.  Get Account type first.
            AccountType accountType = null;
            if (!string.IsNullOrEmpty(account.AccountType))
            {
                accountType = GetObjectByFieldID<AccountType>(CommonGlobals.AccountTypesCollectionName, CommonGlobals.IdFieldName, account.AccountType);
            }

            if (null == account.ParentID)
            {
                account.ParentID = accountType.BankID;
            }

            if (null == account.ParentID)
                throw new Exception("Bank ID must be specified");

            // Now find bank.
            IMongoCollection<Bank> bankColl = _database.GetCollection<Bank>(CommonGlobals.BanksCollectionName);
            var filterdef = Builders<Bank>.Filter.Eq(CommonGlobals.IdFieldName, account.ParentID);
            Bank bank = null;

            try
            {
                bank = bankColl.Find(filterdef).First();
            }
            catch (Exception ex)
            {
                throw new Exception("Bank with ID " + account.ParentID + " not found.");
            }

            if (null == bank)
                throw new Exception("Bank with ID " + account.ParentID + " not found.");

            // Check write access to bank (required for creating an account).
            PerformSecurityCheck(Access.Write, bank.Id, Resource.Bank, bank.Id);

            if (null == account.Status)
            {
                account.Status = new StatusEx { Status = Status.Pending_e };
            }

            // If corporate ID has been specified, check if it exists.
            IMongoCollection<Corporate> corporateColl = null;
            if (!string.IsNullOrEmpty(account.Id))
            {
                // Now find corporate.
                if (!string.IsNullOrEmpty(account.CorporateId))
                {
                    corporateColl = _database.GetCollection<Corporate>(CommonGlobals.CorporatesCollectionName);
                    var corporatefilterdef = Builders<Corporate>.Filter.Eq(CommonGlobals.IdFieldName, account.CorporateId);
                    Corporate corporate = corporateColl.Find(corporatefilterdef).First();

                    if (null == corporate)
                        throw new Exception("Corporate with ID " + account.CorporateId + " not found.");
                }
            }

            // Get accounts.
            IMongoCollection<Account> accountColl = _database.GetCollection<Account>(CommonGlobals.AccountsCollectionName);

            // Create documents if it doesn't exist.
            if (account.Documents == null || account.Documents.Count == 0)
            {
                // Create documents list. We need to resolve these documents now.
                account.Documents = new List<string>();

                // List has been created.  Get Account type first.
                if (!string.IsNullOrEmpty(account.AccountType))
                {
                    if (null != accountType)
                    {
                        // Get document contents.
                        IMongoCollection<DocumentContent> docContentColl = _database.GetCollection<DocumentContent>(CommonGlobals.DocumentContentsCollectionName);

                        // Get documents.
                        IMongoCollection<Document> docColl = _database.GetCollection<Document>(CommonGlobals.DocumentsCollectionName);


                        for (int i = 0; i < accountType.BaseDocumentIDs.Count; i++)
                        {
                            DocumentContent content = GetAccountTypeDocumentContents(account.AccountType, accountType.BaseDocumentIDs[i], prefillDocuments);

                            Document doc = new Document();
                            doc.Versions = new List<DocumentVersion>();
                            doc.Name = accountType.BaseDocumentNames[i];
                            doc.Id = ObjectId.GenerateNewId().ToString();
                            doc.Accounts = new List<string>();
                            doc.Accounts.Add(account.Id);
                            doc.Status = new StatusEx()
                            {
                                Status = Status.Pending_e,
                            };

                            // Create document version.
                            DocumentVersion version = new DocumentVersion();
                            version.Id = ObjectId.GenerateNewId().ToString();
                            version.DocumentContentId = content.Id;
                            doc.Versions.Add(version);

                            // Add to document collection.
                            docContentColl.InsertOne(content);

                            // Add document.
                            docColl.InsertOne(doc);

                            // Add document to account documents list.
                            account.Documents.Add(doc.Id);
                        }
                    }
                }
            }

            // Insert account.
            accountColl.InsertOne(account);

            //Now update bank list of accounts to include this one.
            List<string> accounts = bank.Accounts;

            if (accounts.Contains(account.Id))
                throw new Exception("Account " + account.Id + " not found for bank  with ID " + account.ParentID);

            // Append the account ID to the accounts list for the bank.
            var updateOperation = Builders<Bank>.Update.PushEach<string>("Accounts", new List<string> { account.Id });
            var filter = Builders<Bank>.Filter.Eq(CommonGlobals.IdFieldName, account.ParentID);
            var result = bankColl.UpdateOneAsync(filter, updateOperation);

            //Thow exception if we have a problem.
            if (null != result.Exception)
            {
                if (null != result.Exception.InnerException)
                    throw new Exception("Unable to Create account: " + result.Exception.InnerException.ToString());
                else
                    throw new Exception("Unable to Create account");
            }


            // If corporate ID has been specified, add to list of accounts for corporate.
            if (!string.IsNullOrEmpty(account.CorporateId))
            {
                // Append the account ID to the accounts list for the bank.
                var corporateupdateOperation = Builders<Corporate>.Update.PushEach<string>("Accounts", new List<string> { account.Id });
                var corporatefilter = Builders<Corporate>.Filter.Eq(CommonGlobals.IdFieldName, account.ParentID);
                var corporateresult = corporateColl.UpdateOneAsync(corporatefilter, corporateupdateOperation);

                //Thow exception if we have a problem.
                if (null != result.Exception)
                {
                    if (null != result.Exception.InnerException)
                        throw new Exception("Unable to Create account: " + result.Exception.InnerException.ToString());
                    else
                        throw new Exception("Unable to Create account");
                }
            }
        }

        /// <summary>
        /// Get account by ID.
        /// </summary>
        /// <param name="read"></param>
        /// <returns></returns>
        public Account GetAccount(string ID)
        {
            // Get accounts.
            IMongoCollection<Account> accounts = _database.GetCollection<Account>(CommonGlobals.AccountsCollectionName);

            // Define filter.
            var filterdef = Builders<Account>.Filter.Eq(CommonGlobals.IdFieldName, ID);

            // Return account.
            return accounts.Find(filterdef).FirstOrDefault();
        }

        /// <summary>
        /// Update account.
        /// </summary>
        /// <param name="update"></param>
        public void UpdateAccount(string accountID, Account account)
        {
            var collection = _database.GetCollection<Account>(CommonGlobals.AccountsCollectionName);
            var filter = Builders<Account>.Filter.Eq(CommonGlobals.IdFieldName, accountID);
            var updateOperation = Builders<Account>.Update
                .Set("Detail", account.Detail)
                  .Set("Status", account.Status);

            // Perform update.
            var result = collection.UpdateOneAsync(filter, updateOperation);

            //Thow exception if we have a problem.
            if (null != result.Exception)
            {
                if (null != result.Exception.InnerException)
                    throw new Exception("Unable to rename account: " + result.Exception.InnerException.ToString());
                else
                    throw new Exception("Unable to rename account");
            }

        }

        /// <summary>
        /// Delete account.
        /// </summary>
        /// <param name=CommonGlobals.IdFieldName></param>
        public void DeleteAccount(string accountID)
        {
            var accountColl = _database.GetCollection<Account>(CommonGlobals.AccountsCollectionName);
            var filter = Builders<Account>.Filter.Eq(CommonGlobals.IdFieldName, accountID);

            // Get account reference.
            Account account = accountColl.Find(filter).FirstOrDefault();
            if (null == account)
                throw new Exception("Account with ID " + accountID + " not found.");

            // Perform deletion security check for account.
            PerformSecurityCheck(Access.Delete, accountID, Resource.Account, account.ParentID);

            // Now find bank.
            IMongoCollection<Bank> bankColl = _database.GetCollection<Bank>(CommonGlobals.BanksCollectionName);
            var filterdef = Builders<Bank>.Filter.Eq(CommonGlobals.IdFieldName, account.ParentID);
            Bank bank = bankColl.Find(filterdef).First();

            // Remove account from bank.
            if (null != bank)
            {
                // remove account from Accounts list.
                var updateOperation = Builders<Bank>.Update.Pull<string>("Accounts", account.Id);
                var result = bankColl.UpdateOneAsync(filterdef, updateOperation);

                //Thow exception if we have a problem.
                if (null != result.Exception)
                {
                    if (null != result.Exception.InnerException)
                        throw new Exception("Unable to Update bank account reference: " + result.Exception.InnerException.ToString());
                    else
                        throw new Exception("Unable to Update bank account reference");
                }
            }

            // Perform account deletion.
            accountColl.DeleteOne(filter);
        }

        #endregion

        /// <summary>
        /// Upload document for account.
        /// </summary>
        public void FilledDocumentUpload(string accountID, string documentName, DocumentContent content)
        {
            Account account = GetObjectByFieldID<Account>(CommonGlobals.AccountsCollectionName, CommonGlobals.IdFieldName, accountID);

            // TODO: We may be able to tell which document we are updating based on meta data in the document.
            List<string> docs = account.Documents;

            // Iterate through documents, get document associated with this name.
            Document doc = null;
            bool bExistingDocument = false;
            foreach (string docId in docs)
            {
                Document document = GetObjectByFieldID<Document>(CommonGlobals.DocumentsCollectionName, CommonGlobals.IdFieldName, docId);
                if (0 == document.Name.CompareTo(documentName))
                {
                    bExistingDocument = true;
                    doc = document;
                    break;
                }

            }

            string docID = "";
            if (null == doc)
            {
                // Document doesn't exist, we need to create a new one and add it to the Account list.
                doc = new Document()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Global = false,
                    Accounts = new List<string>(),
                    Chats = new List<Chat>()

                };
            }

            docID = doc.Id;

            // Create a new version.
            List<DocumentVersion> versions = doc.Versions;
            DocumentVersion version = new DocumentVersion();
            version.DocumentContentId = content.Id;
            version.Creation = DateTime.Now.ToString();

            // Get contents collection.
            IMongoCollection<DocumentContent> contentsColl = _database.GetCollection<DocumentContent>(CommonGlobals.DocumentContentsCollectionName);

            // Insert document content.
            contentsColl.InsertOne(content);

            // Now update versions for document.
            var updateOperation = Builders<Document>.Update.PushEach("Versions", new List<DocumentVersion> { version });
            var filter = Builders<Document>.Filter.Eq(CommonGlobals.IdFieldName, docID);
            IMongoCollection<Document> coll = _database.GetCollection<Document>(CommonGlobals.DocumentsCollectionName);
            var result = coll.UpdateOneAsync(filter, updateOperation);

            // If document was newly created and did not belong in account, we need to add it to the account list.
            if (!bExistingDocument)
            {
                // Now update documents list for this account.
                var updateAccountOperation = Builders<Account>.Update.PushEach("Documents", new List<string> { docID });
                var accountFilter = Builders<Account>.Filter.Eq(CommonGlobals.IdFieldName, accountID);
                IMongoCollection<Account> accountColl = _database.GetCollection<Account>(CommonGlobals.AccountsCollectionName);
                var accountResult = accountColl.UpdateOneAsync(accountFilter, updateAccountOperation);

            }

        }



        /// <summary>
        /// Create account application.
        /// </summary>
        /// <param name=CommonGlobals.IdFieldName></param>
        public Notification ChangeAccountStatus(string ID, StatusEx status)
        {
            Account account = GetObjectByFieldID<Account>(CommonGlobals.AccountsCollectionName, CommonGlobals.IdFieldName, ID);

            var collection = _database.GetCollection<Account>(CommonGlobals.AccountsCollectionName);
            var filter = Builders<Account>.Filter.Eq(CommonGlobals.IdFieldName, ID);
            var updateOperation = Builders<Account>.Update
                .Set("Status", status);

            // Perform update.
            var result = collection.UpdateOneAsync(filter, updateOperation);

            //Thow exception if we have a problem.
            if (null != result.Exception)
            {
                if (null != result.Exception.InnerException)
                    throw new Exception("Unable to update account: " + result.Exception.InnerException.ToString());
                else
                    throw new Exception("Unable to update account");
            }

            StatusUpdate statusupdate = new StatusUpdate();
            statusupdate.DocumentUpdates = new List<StatusCouple>();
            statusupdate.AccountUpdates = new List<StatusCouple>() { new StatusCouple() { ID = account.Id, Status = status.Status } };

            // Delete notifications for this  account.
            if (status.Status != Status.Pending_e)
            {
                statusupdate.CorporateUpdates = new List<StatusCouple>() { new StatusCouple() { ID = account.CorporateId, Status = status.Status } };

                Notification notify = new Notification { UserName = "User", Id = ObjectId.GenerateNewId().ToString(), Type = NotificationType.StatusChangeAccount_e, StatusUpdate = statusupdate };
                notify = CompleteNotify(notify);

                // Delete notifications for this account, once it's approved, rejected.
                UpdateCorporateNotifications(account.CorporateId, notify);
            }
            else
            {
                statusupdate.BankUpdates = new List<StatusCouple>() { new StatusCouple() { ID = account.ParentID, Status = status.Status } };

                Notification notify = new Notification { UserName = "User", Id = ObjectId.GenerateNewId().ToString(), Type = NotificationType.StatusChangeAccount_e, StatusUpdate = statusupdate };
                notify = CompleteNotify(notify);

                UpdateBankNotifications(account.ParentID, null);
            }

            return null;
        }

        #region DOCUMENT_METHODS

        // Document entries.
        public List<Document> GetDocuments(string accountID)
        {
            if (string.IsNullOrEmpty(accountID))
                throw new Exception("Unable to get documents, account ID not specified");

            var accountColl = _database.GetCollection<Account>(CommonGlobals.AccountsCollectionName);

            IMongoCollection<Document> documents = _database.GetCollection<Document>(CommonGlobals.DocumentsCollectionName);

            List<Document> globalDocuments = new List<Document>();

            // Get documents whose bankID and corporate ID are the global identifier.
            var docIDFilter = Builders<Document>.Filter.Eq("Global", true);

            // Get account reference.
            globalDocuments = documents.Find(docIDFilter).ToList();


            // If we're looking for global documents, look for global parm set to true.
            if (0 == accountID.CompareTo(CommonGlobals.GlobalIdentifier))
            {
                return globalDocuments;
            }

            var filter = Builders<Account>.Filter.Eq(CommonGlobals.IdFieldName, accountID);

            // Get account reference.
            Account account = accountColl.Find(filter).FirstOrDefault();

            string corporateID = null;
            try
            {
                corporateID = GetCorporateIDFromAccountID(account.Id);
            }
            catch (Exception) { }

            List<string> docIDs = account.Documents;
            List<Document> ret = new List<Document>();
            // Iterate through document IDs.
            foreach (string docID in docIDs)
            {
                // Define filter.
                var filterDoc = Builders<Document>.Filter.Eq(CommonGlobals.IdFieldName, docID);

                // Get document based on filter.
                Document doc = documents.Find(filterDoc).FirstOrDefault();

                // Perform read check for each document.
                try
                {
                    // If we don't have read access to the document, an exception will be thrown and 
                    // we continue to the next Document.
                    PerformSecurityCheck(Access.Read, doc.Id, Resource.Document, corporateID);
                    ret.Add(doc);
                }
                catch (Exception) { }
            }
            // Add global documents 
            ret.AddRange(globalDocuments);

            return ret;
        }

        private DocumentVersion createFirstVersion(string content)
        {
            DocumentVersion firstVersion = new DocumentVersion()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                DocumentContentId = ObjectId.GenerateNewId().ToString(),
                Creation = DateTime.Now.ToUniversalTime().ToString()
            };
            DocumentContent docData = new DocumentContent()
            {
                Id = firstVersion.DocumentContentId,
                ContentBase64 = content
            };

            IMongoCollection<DocumentContent> docContentsCollection = _database.GetCollection<DocumentContent>("DocumentContents");
            docContentsCollection.InsertOne(docData);
            return firstVersion;
        }

        /// <summary>
        /// Create document.
        /// </summary>
        /// <param name="document"></param>
        public Notification CreateDocument(Document document, string accountID, string documentContent)
        {
            bool bTemplateDocument = false;
            bool bGlobal = false;
            IMongoCollection<Account> accountColl = _database.GetCollection<Account>(CommonGlobals.AccountsCollectionName);
            var filterdef = Builders<Account>.Filter.Eq(CommonGlobals.IdFieldName, accountID);

            List<Account> accounts = accountColl.Find(filterdef).ToList();
            if (accounts.Count == 0)
            {
                if (!string.IsNullOrEmpty(accountID) && accountID.CompareTo(CommonGlobals.GlobalIdentifier) == 0)
                {
                    // global account.
                    bGlobal = true;
                }
                else bTemplateDocument = true;
                // Permit this, account may not be set.
                // else throw new Exception("Account " + accountID + " not found.");
            }

            if (document.Status == null)
            {
                document.Status = new StatusEx { Status = Status.Pending_e };
            }
            document.Global = bGlobal;

            Account account = null;
            if (!bGlobal && !string.IsNullOrEmpty(accountID) && !bTemplateDocument)
            {
                account = accounts[0];

                if (null == account)
                    throw new Exception("Account with ID " + accountID + " not found.");

                string corporateID = null;
                try
                {
                    corporateID = GetCorporateIDFromAccountID(account.Id);
                }
                catch (Exception) { }

                // Perform update security check for parent account.
                PerformSecurityCheck(Access.Write, accounts[0].Id, Resource.Account, corporateID);
            }

            // Get documents.
            IMongoCollection<Document> documentColl = _database.GetCollection<Document>(CommonGlobals.DocumentsCollectionName);

            // Set ID for document and document version.
            if (string.IsNullOrEmpty(document.Id))
            {
                document.Id = ObjectId.GenerateNewId().ToString();
            }

            document.Versions = new List<DocumentVersion>();
            Boolean contentExists = !string.IsNullOrEmpty(documentContent);
            if (contentExists)
            {
                document.Versions.Add(createFirstVersion(documentContent));
            }

            string documentId = document.Id;
            string corporateId = "";
            string bankId = "";

            if (!bTemplateDocument)
            {
                // Append the account ID to the accounts list for the document.
                var updateOperation = Builders<Account>.Update.PushEach<string>("Documents", new List<string> { document.Id });
                var result = accountColl.UpdateOneAsync(filterdef, updateOperation);

                //Thow exception if we have a problem.
                if (null != result.Exception)
                {
                    if (null != result.Exception.InnerException)
                        throw new Exception("Unable to Create document: " + result.Exception.InnerException.ToString());
                    else
                        throw new Exception("Unable to Create document");
                }

                if (!string.IsNullOrEmpty(accountID))
                {
                    corporateId = bGlobal ? CommonGlobals.GlobalIdentifier : account.CorporateId;
                    bankId = bGlobal ? CommonGlobals.GlobalIdentifier : account.ParentID;
                }
                // Insert document.
                documentColl.InsertOne(document);

            }
            else
            {
                // This is a template document. We should extract fields from document and store in Mongo.
               string base64Marker = "data:application/pdf;base64,";
                    byte[] bytes = null;


                try
                {
                    bytes = System.Convert.FromBase64String(documentContent);
                }
                catch (Exception ex)
                {
                    // Remove marker and try again.
                    if (documentContent.StartsWith(base64Marker))
                    {
                        documentContent = documentContent.Substring(base64Marker.Length);
                    }
                    bytes = System.Convert.FromBase64String(documentContent);
                }

                using (MemoryStream stream = new MemoryStream(bytes))
                {
                    var pdfReader = new iTextSharp.text.pdf.PdfReader(stream);
                    List<FieldDefinition> fieldDefs = new List<FieldDefinition>();
                    foreach (string key in pdfReader.AcroFields.Fields.Keys)
                    {
                        iTextSharp.text.pdf.AcroFields.Item item = pdfReader.AcroFields.Fields[key];
                        FieldDefinition def = new FieldDefinition();
                        def.Id = ObjectId.GenerateNewId().ToString();
                        def.Name = key;
                        def.DefaultValue = "";

                        fieldDefs.Add(def);

                        Console.WriteLine("Adding field " + key + " to document " + document.Name);
                    }
                    document.Fields = fieldDefs;

                    // Insert document.
                    documentColl.InsertOne(document);
                }
            }
            // Send notification to Bank.
            StatusUpdate statusupdate = new StatusUpdate();
            statusupdate.DocumentUpdates = new List<StatusCouple>() { new StatusCouple() { ID = documentId, Status = document.Status.Status } };
            statusupdate.AccountUpdates = new List<StatusCouple>() { new StatusCouple() { ID = accountID, Status = Status.Pending_e } };
            statusupdate.CorporateUpdates = new List<StatusCouple>() { new StatusCouple() { ID = corporateId, Status = Status.Pending_e } };
            statusupdate.BankUpdates = new List<StatusCouple>() { new StatusCouple() { ID = bankId, Status = Status.Pending_e } };

            Notification notify = new Notification() { DocumentID = documentId, Type = NotificationType.StatusCreateDocument_e, StatusUpdate = statusupdate };
            return CompleteNotify(notify);
        }

        /// <summary>
        /// Create document.
        /// </summary>
        /// <param name="document"></param>
        public Notification UploadDocument(string documentId, string documentContentBase64)
        {
            DocumentVersion version = new DocumentVersion();
            version.Id = ObjectId.GenerateNewId().ToString();
            version.Creation = DateTime.Now.ToUniversalTime().ToString();
            version.DocumentContentId = ObjectId.GenerateNewId().ToString();
            //Also create a doc with this content and id
            var docData = new DocumentContent { Id = version.DocumentContentId, ContentBase64 = documentContentBase64 };

            var updateOperation = Builders<Document>.Update.PushEach("Versions", new List<DocumentVersion> { version });
            var filter = Builders<Document>.Filter.Eq(CommonGlobals.IdFieldName, documentId);
            IMongoCollection<Document> coll = _database.GetCollection<Document>(CommonGlobals.DocumentsCollectionName);
            var result = coll.UpdateOneAsync(filter, updateOperation);

            //Upload the document content
            IMongoCollection<DocumentContent> docContentsColl = _database.GetCollection<DocumentContent>("DocumentContents");
            docContentsColl.InsertOne(docData);

            //Thow exception if we have a problem.
            if (null != result.Exception)
            {
                if (null != result.Exception.InnerException)
                    throw new Exception("Unable to upload document: " + result.Exception.InnerException.ToString());
                else
                    throw new Exception("Unable to upload document");
            }

            // Document has been uploaded, firstly set status of document, account, corporate.
            // Set status of document first.
            ResolveStatus(Status.NotSet_e, Status.Pending_e, ObjectType_e.Document_e, documentId);

            // Now set status of account.
            Document document = GetObjectByFieldID<Document>(CommonGlobals.DocumentsCollectionName, CommonGlobals.IdFieldName, documentId);
            if ((null == document.Accounts) || document.Accounts.Count == 0)
                throw new Exception("Accounts not set for document");
            string accountID = document.Accounts[0];
            ResolveStatus(Status.Pending_e, Status.NotSet_e, ObjectType_e.Account_e, accountID);

            // Now set status of corporate.
            Account account = GetObjectByFieldID<Account>(CommonGlobals.AccountsCollectionName, CommonGlobals.IdFieldName, accountID);

            string corporateId = account.CorporateId;
            string bankId = account.ParentID;
            ResolveStatus(Status.Pending_e, Status.NotSet_e, ObjectType_e.Corporate_e, corporateId);

            string bankID = account.ParentID;


            ResolveStatus(Status.Pending_e, Status.NotSet_e, ObjectType_e.Bank_e, bankID);

            // Send notification to Bank.
            StatusUpdate statusupdate = new StatusUpdate();
            statusupdate.DocumentUpdates = new List<StatusCouple>() { new StatusCouple() { ID = documentId, Status = Status.Pending_e } };
            statusupdate.AccountUpdates = new List<StatusCouple>() { new StatusCouple() { ID = accountID, Status = Status.Pending_e } };
            statusupdate.CorporateUpdates = new List<StatusCouple>() { new StatusCouple() { ID = corporateId, Status = Status.Pending_e } };
            statusupdate.BankUpdates = new List<StatusCouple>() { new StatusCouple() { ID = bankId, Status = Status.Pending_e } };

            Notification notify = new Notification() { DocumentID = documentId, Type = NotificationType.StatusChangeDocument_e, StatusUpdate = statusupdate };
            notify = CompleteNotify(notify);

            UpdateBankNotifications(bankID, notify);
            return notify;
        }

        /// <summary>
        /// Get document based on ID.
        /// </summary>
        /// <param name="documentID"></param>
        /// <returns></returns>
        public Document GetDocument(string documentID)
        {
            return GetObjectByFieldID<Document>(CommonGlobals.DocumentsCollectionName, CommonGlobals.IdFieldName, documentID);
        }

        /// <summary>
        /// Get document based on ID.
        /// </summary>
        /// <param name="documentID"></param>
        /// <returns></returns>
        public DocumentContent GetDocumentContents(string documentContentID)
        {
            return GetObjectByFieldID<DocumentContent>(CommonGlobals.DocumentContentsCollectionName, CommonGlobals.IdFieldName, documentContentID);
        }

        public void DocumentFieldDefinitionSet(string corporateID, string documentID, FieldDefinition fielddefinition)
        {
        }

        public void DocumentFieldDefinitionRemove(string corporateID, string documentID, string fielddefinitionID)
        {
        }

        /// <summary>
        /// Update document name and status.
        /// </summary>
        /// <param name="documentID"></param>
        /// <param name="name"></param>
        /// <param name="status"></param>
        public void UpdateDocument(string documentID, string name, StatusEx status)
        {
            var collection = _database.GetCollection<Document>(CommonGlobals.DocumentsCollectionName);
            var filter = Builders<Document>.Filter.Eq(CommonGlobals.IdFieldName, documentID);
            var updateOperation = Builders<Document>.Update
                .Set("Name", name)
                .Set("Status", status);

            // Perform update.
            var result = collection.UpdateOneAsync(filter, updateOperation);

            //Thow exception if we have a problem.
            if (null != result.Exception)
            {
                if (null != result.Exception.InnerException)
                    throw new Exception("Unable to Update document: " + result.Exception.InnerException.ToString());
                else
                    throw new Exception("Unable to Update document");
            }
        }

        /// <summary>
        /// Delete document.
        /// </summary>
        /// <param name="documentID"></param>
        public void DeleteDocument(string documentID)
        {
            var documentColl = _database.GetCollection<Document>(CommonGlobals.DocumentsCollectionName);
            var filter = Builders<Document>.Filter.Eq(CommonGlobals.IdFieldName, documentID);

            // Get document reference.
            Document document = documentColl.Find(filter).FirstOrDefault();
            if (null == document)
                throw new Exception("Document with ID " + documentID + " not found.");

            List<string> accountIDs = document.Accounts;
            string corporateID = null;
            try
            {
                corporateID = GetCorporateIDFromAccountID(accountIDs[0]);
            }
            catch (Exception) { }

            // Perform update security check for parent account.
            PerformSecurityCheck(Access.Delete, document.Id, Resource.Document, corporateID);

            // Iterate through accounts and remove this document from them.
            IMongoCollection<Account> accountColl = _database.GetCollection<Account>(CommonGlobals.AccountsCollectionName);
            foreach (string accountID in accountIDs)
            {
                var filterdef = Builders<Account>.Filter.Eq(CommonGlobals.IdFieldName, accountID);
                Account account = accountColl.Find(filterdef).First();

                // remove document from Documents list for each account.
                var updateOperation = Builders<Account>.Update.Pull<string>("Documents", documentID);
                var result = accountColl.UpdateOneAsync(filterdef, updateOperation);


                //Thow exception if we have a problem.
                if (null != result.Exception)
                {
                    if (null != result.Exception.InnerException)
                        throw new Exception("Unable to Update account: " + result.Exception.InnerException.ToString());
                    else
                        throw new Exception("Unable to Update account");
                }
            }

            // Perform account deletion.
            documentColl.DeleteOne(filter);
        }

        /// <summary>
        /// Create account application.
        /// </summary>
        /// <param name=CommonGlobals.IdFieldName></param>
        public Notification ChangeDocumentStatus(string username, string ID, Status documentStatus)
        {
            Document document = GetObjectByFieldID<Document>(CommonGlobals.DocumentsCollectionName, CommonGlobals.IdFieldName, ID);

            var collection = _database.GetCollection<Document>(CommonGlobals.DocumentsCollectionName);
            var filter = Builders<Document>.Filter.Eq(CommonGlobals.IdFieldName, ID);

            StatusEx statusex = new StatusEx { Status = documentStatus };

            var updateOperation = Builders<Document>.Update
                .Set("Status", statusex);

            // Perform update.
            var result = collection.UpdateOneAsync(filter, updateOperation);

            //Thow exception if we have a problem.
            if (null != result.Exception)
            {
                if (null != result.Exception.InnerException)
                    throw new Exception("Unable to update document: " + result.Exception.InnerException.ToString());
                else
                    throw new Exception("Unable to update document");
            }

            // We need to set status of document, account, corporate.
            // Set status of document first.
            ResolveStatus(Status.NotSet_e, documentStatus, ObjectType_e.Document_e, ID);
            if ((null == document.Accounts) || document.Accounts.Count == 0)
                throw new Exception("Accounts not set for document");
            string accountID = document.Accounts[0];
            ResolveStatus(documentStatus, Status.NotSet_e, ObjectType_e.Account_e, accountID);

            // Now set status of corporate.
            // Now set status of corporate.
            Account account = GetObjectByFieldID<Account>(CommonGlobals.AccountsCollectionName, CommonGlobals.IdFieldName, accountID);
            Status accountStatus = account.Status.Status;
            string corporateId = account.CorporateId;
            string bankID = account.ParentID;

            ResolveStatus(accountStatus, Status.NotSet_e, ObjectType_e.Corporate_e, corporateId);
            ResolveStatus(accountStatus, Status.NotSet_e, ObjectType_e.Bank_e, bankID);

            Corporate corporate = GetObjectByFieldID<Corporate>(CommonGlobals.CorporatesCollectionName, CommonGlobals.IdFieldName, corporateId);
            Status corporateStatus = corporate.Status.Status;

            StatusUpdate statusupdate = new StatusUpdate();
            statusupdate.DocumentUpdates = new List<StatusCouple>() { new StatusCouple() { ID = ID, Status = documentStatus } };
            statusupdate.AccountUpdates = new List<StatusCouple>() { new StatusCouple() { ID = accountID, Status = accountStatus } };

            string user = !string.IsNullOrEmpty(username) ? username : "User";

            // Send notification depending on type of status change.
            Notification notify = null;

            statusupdate.CorporateUpdates = new List<StatusCouple>() { new StatusCouple() { ID = corporateId, Status = corporateStatus } };
            notify = new Notification { UserName = user, DocumentID = ID, Id = ObjectId.GenerateNewId().ToString(), Type = NotificationType.StatusChangeDocument_e, StatusUpdate = statusupdate };
            notify = CompleteNotify(notify);

            UpdateCorporateNotifications(corporateId, notify);

            statusupdate.BankUpdates = new List<StatusCouple>() { new StatusCouple() { ID = bankID, Status = corporateStatus } };
            notify = new Notification { UserName = user, DocumentID = ID, Id = ObjectId.GenerateNewId().ToString(), Type = NotificationType.StatusChangeDocument_e, StatusUpdate = statusupdate };
            notify = CompleteNotify(notify);
            UpdateBankNotifications(bankID, notify);

            return notify;
        }


        public void GetBankAndCorporateForDocument(string docID, out string BankID, out string CorporateID)
        {
            Document document = GetObjectByFieldID<Document>(CommonGlobals.DocumentsCollectionName, CommonGlobals.IdFieldName, docID);

            if (null == document)
                throw new Exception("Document with ID " + docID + " not found.");

            if (0 == document.Accounts.Count)
                throw new Exception("No accounts associated with document with ID " + docID);

            // Get first account associated with the document.
            Account account = GetObjectByFieldID<Account>(CommonGlobals.AccountsCollectionName, CommonGlobals.IdFieldName, document.Accounts[0]);

            BankID = account.ParentID;
            CorporateID = account.CorporateId;
        }

        /// <summary>
        /// Delete document version.
        /// </summary>
        /// <param name="documentID"></param>
        /// <param name="versionID"></param>
        public void DeleteDocumentVersion(string documentID, string versionID)
        {
            if (null == documentID)
                throw new Exception("Document ID not specified");

            if (null == versionID)
                throw new Exception("Version not specified");


            var documentColl = _database.GetCollection<Document>(CommonGlobals.DocumentsCollectionName);
            var filter = Builders<Document>.Filter.Eq(CommonGlobals.IdFieldName, documentID);

            // Get document reference.
            Document document = documentColl.Find(filter).FirstOrDefault();
            if (null == document)
                throw new Exception("Document with ID " + documentID + " not found.");

            // Iterate through accounts and remove this document from them.
            List<DocumentVersion> versions = document.Versions;

            // Find document version.
            DocumentVersion thisVersion = null;
            foreach (DocumentVersion version in versions)
            {
                if (0 == versionID.CompareTo(version.Id))
                {
                    thisVersion = version;
                }
            }

            // remove document version from Document.
            var deleteOperation = Builders<Document>.Update.Pull<DocumentVersion>("Versions", thisVersion);
            var result = documentColl.UpdateOneAsync(filter, deleteOperation);

            //Thow exception if we have a problem.
            if (null != result.Exception)
            {
                if (null != result.Exception.InnerException)
                    throw new Exception("Unable to delete Document Version: " + result.Exception.InnerException.ToString());
                else
                    throw new Exception("Unable to delete Document Version");
            }
        }


        #endregion

        // Link entries.
        public void LinkDocumentToAccount(string documentID, string accountID)
        {
            // Get accounts.
            IMongoCollection<Account> accounts = _database.GetCollection<Account>(CommonGlobals.AccountsCollectionName);

            // Define filter.
            var filterdef = Builders<Account>.Filter.Eq(CommonGlobals.IdFieldName, accountID);

            // Return account.
            Account thisAccount = accounts.Find(filterdef).FirstOrDefault();

            List<string> linkedDocuments = thisAccount.Documents;

            if (!linkedDocuments.Contains(documentID))
            {
                linkedDocuments.Add(documentID);
                thisAccount.Documents = linkedDocuments;
                var updateOperation = Builders<Account>.Update
                    .Set("Documents", linkedDocuments);

            }
        }
        public void LinkAccountToBank(string accountID, string bankID)
        {
            // Get accounts.
            IMongoCollection<Bank> banks = _database.GetCollection<Bank>(CommonGlobals.BanksCollectionName);

            // Define filter.
            var filterdef = Builders<Bank>.Filter.Eq(CommonGlobals.IdFieldName, bankID);

            // Return account.
            Bank thisBank = banks.Find(filterdef).FirstOrDefault();

            List<string> linkedAccounts = thisBank.Accounts;

            if (!linkedAccounts.Contains(accountID))
            {
                linkedAccounts.Add(accountID);
                thisBank.Accounts = linkedAccounts;
                var updateOperation = Builders<Bank>.Update
                    .Set("Accounts", linkedAccounts);
            }
        }

        public void LinkAccountToCorporate(string accountID, string corporateID)
        {
            // Get accounts.
            IMongoCollection<Corporate> corporates = _database.GetCollection<Corporate>(CommonGlobals.CorporatesCollectionName);

            // Define filter.
            var filterdef = Builders<Corporate>.Filter.Eq(CommonGlobals.IdFieldName, corporateID);

            // Return corporate.
            Corporate thisCorporate = corporates.Find(filterdef).FirstOrDefault();

            List<string> linkedAccounts = thisCorporate.Accounts;

            if (!linkedAccounts.Contains(accountID))
            {
                linkedAccounts.Add(accountID);
                thisCorporate.Accounts = linkedAccounts;
                var updateOperation = Builders<Bank>.Update
                    .Set("Accounts", linkedAccounts);
            }
        }

        public List<AccountType> GetAccountTypes(string bankID)
        {
            IMongoCollection<AccountType> coll = _database.GetCollection<AccountType>(CommonGlobals.AccountTypesCollectionName);

            // Only return "Approved" or "Rejected" notifications.
            return coll.Find(i => (i.BankID == bankID)).ToList();
        }

        public List<string> GetAccountTypeLists(string bankID)
        {
            IMongoCollection<AccountType> coll = _database.GetCollection<AccountType>(CommonGlobals.AccountTypesCollectionName);

            // Build string list of account type names.
            List<string> accountTypeNames = new List<string>();
            var matchingAccountTypes = coll.AsQueryable().ToList();
            foreach (var acctype in matchingAccountTypes)
            {
                accountTypeNames.Add(acctype.Name);
            }

            return accountTypeNames;
        }


        public List<string> GetAccountTypeIDs(string bankID)
        {
            IMongoCollection<AccountType> coll = _database.GetCollection<AccountType>(CommonGlobals.AccountTypesCollectionName);

            // Build string list of account type names.
            List<string> accountTypeIds = new List<string>();
            var matchingAccountTypes = coll.AsQueryable().ToList();
            foreach (var acctype in matchingAccountTypes)
            {
                accountTypeIds.Add(acctype.Id);
            }

            return accountTypeIds;
        }

        /// <summary>
        /// Create account type.
        /// </summary>
        /// <param name="accounttype"></param>
        public void CreateAccountType(AccountType accounttype)
        {
            // Make sure name is specified.
            if (string.IsNullOrEmpty(accounttype.Name))
            {
                throw new Exception("Account type can not be created, name not specified");
            }

            if (string.IsNullOrEmpty(accounttype.BankID))
            {
                throw new Exception("Bank ID must be specified for Account Type");
            }

            try
            {
                AccountType type = GetAccountTypeByName(accounttype.Id);
                throw new Exception("Account type can not be created, " + accounttype.Name + " already exists");
            }
            catch (Exception) { }

            if (string.IsNullOrEmpty(accounttype.Id))
                accounttype.Id = ObjectId.GenerateNewId().ToString();

            IMongoCollection<AccountType> accounttypes = _database.GetCollection<AccountType>(CommonGlobals.AccountTypesCollectionName);
            accounttypes.InsertOne(accounttype);

        }

        /// <summary>
        /// Get account type by ID.
        /// </summary>
        /// <param name=CommonGlobals.IdFieldName></param>
        /// <returns></returns>
        public AccountType GetAccountType(string Id)
        {

            AccountType obj = null;
            try
            {
                obj = GetObjectByFieldID<AccountType>(CommonGlobals.AccountTypesCollectionName, CommonGlobals.IdFieldName, Id);
            }
            catch (Exception ex)
            {
                throw new Exception("Account type with id " + Id + " not found.");
            }
            return obj;

        }

        /// <summary>
        /// Delete acount type.
        /// </summary>
        /// <param name=CommonGlobals.IdFieldName></param>
        public void DeleteAccountType(string ID)
        {
            // Make sure id is specified.
            if (string.IsNullOrEmpty(ID))
            {
                throw new Exception("Account Type can not be deleted, ID not specified");
            }

            try
            {
                AccountType accounttype = GetAccountType(ID);
            }
            catch (Exception)
            {
                throw new Exception("Account Type can not be deleted, " + ID + " does not exist");
            }


            // Delete RoleInfo.
            IMongoCollection<AccountType> coll = _database.GetCollection<AccountType>(CommonGlobals.AccountTypesCollectionName);
            var filter = Builders<AccountType>.Filter.Eq(CommonGlobals.IdFieldName, ID);
            var result = coll.DeleteMany(filter);
        }

        /// <summary>
        /// Update acount type.
        /// </summary>
        /// <param name=CommonGlobals.IdFieldName></param>
        public void UpdateAccountType(AccountType accounttype)
        {
            // Make sure id is specified.
            if (string.IsNullOrEmpty(accounttype.Id))
            {
                throw new Exception("Account Type can not be deleted, ID not specified");
            }

            IMongoCollection<AccountType> accountTypeCollection = _database.GetCollection<AccountType>(CommonGlobals.AccountTypesCollectionName);
            var filter = Builders<AccountType>.Filter.Eq(CommonGlobals.IdFieldName, accounttype.Id);

            //perform update
            var result = accountTypeCollection.ReplaceOne(filter, accounttype);
        }

        /// Get document contents from account type.
        /// </summary>
        /// <param name=CommonGlobals.IdFieldName></param>
        public DocumentContent GetAccountTypeDocumentContents(string AccountTypeID, string DocumentID, bool PreFillFields)
        {
            // Get account type firstly.
            AccountType accType = GetObjectByFieldID<AccountType>(CommonGlobals.AccountTypesCollectionName, CommonGlobals.IdFieldName, AccountTypeID);

            // We've got an account type. Now get document.
            List<string> accTypeDocuments = accType.BaseDocumentIDs;
            string docID = null;
            foreach (string nextDoc in accTypeDocuments)
            {
                if (0 == nextDoc.CompareTo(DocumentID))
                {
                    // Found document by ID.
                    docID = nextDoc;
                    break;
                }
            }
            if (null == docID)
            {
                throw new Exception("Document with ID " + DocumentID + " could not be found.");
            }

            Document doc = GetDocument(docID);

            List<DocumentVersion> versions = doc.Versions;
            if (versions.Count == 0)
                throw new Exception("No versions set for document with ID " + DocumentID);

            DocumentVersion version = versions[versions.Count - 1];

            // We need to get the Corporate handle 
            DocumentContent content = GetObjectByFieldID<DocumentContent>(CommonGlobals.DocumentContentsCollectionName, CommonGlobals.IdFieldName, version.DocumentContentId);

            // Check boolean to indicate whether we prefill the fields from Corporate or not.
            if (PreFillFields)
            {
                SystemConfig config = GetSystemConfig(new SystemConfigGet() { });
                Dictionary<string, string> fieldNameToValue = new Dictionary<string, string>();
                if (null != config.CorporateId)
                {
                    bool bDocumentModified = false;
                    Corporate corp = GetObjectByFieldID<Corporate>(CommonGlobals.CorporatesCollectionName, CommonGlobals.IdFieldName, config.CorporateId);
                    if ((null != corp) && (null != corp.Fields) && (0 < corp.Fields.Count))
                    {
                        string base64Content = content.ContentBase64;
                        byte[] bytes = System.Convert.FromBase64String(base64Content);
                        using (MemoryStream stream = new MemoryStream(bytes))
                        {
                            var pdfReader = new iTextSharp.text.pdf.PdfReader(stream);

                            using (MemoryStream msStamper = new MemoryStream())
                            {
                                using (iTextSharp.text.pdf.PdfStamper stamper = new iTextSharp.text.pdf.PdfStamper(pdfReader, msStamper))
                                {

                                    HashSet<string> matchedFields = new HashSet<string>();
                                    foreach (string key in pdfReader.AcroFields.Fields.Keys)
                                    {
                                        Console.WriteLine("Found field " + key + " in document " + doc.Name);
                                        matchedFields.Add(key);
                                    }

                                    iTextSharp.text.pdf.AcroFields pdfFields = pdfReader.AcroFields;

                                    // Work through corporate fields, to see if there is a value to override.
                                    foreach (FieldDefinition corpField in corp.Fields)
                                    {
                                        // Do we have a match, i.e. a value to override?
                                        if (matchedFields.Contains(corpField.Name))
                                        {
                                            stamper.AcroFields.SetField(corpField.Name, corpField.DefaultValue);
                                            bDocumentModified = true;

                                            Console.WriteLine("Replaced field " + corpField.Name + " in document " + doc.Name);

                                        }
                                    }

                                    if (bDocumentModified)
                                    {
                                        byte[] outArray = null;

                                        // flatten form fields and close document
                                        stamper.FormFlattening = true;
                                        stamper.Close();
                                        outArray = msStamper.ToArray();

                                        // Set content.
                                        content.ContentBase64 = Convert.ToBase64String(outArray);

                                    }
                                }
                            }
                        }
                    }
                }
            }
            content.ContentBase64 = string.Concat("data:application/pdf;base64,", content.ContentBase64);
            // Set document content ID.
            content.Id = ObjectId.GenerateNewId().ToString();

            return content;
        }

        /// <summary>
        /// Get account type by name.
        /// </summary>
        /// <param name="accounttypeName"></param>
        /// <returns></returns>
        public AccountType GetAccountTypeByName(string accounttypeName)
        {
            IMongoCollection<AccountType> accountTypes = _database.GetCollection<AccountType>(CommonGlobals.AccountTypesCollectionName);
            var matchingAccountTypes = accountTypes.AsQueryable().ToList();
            foreach (var acctype in matchingAccountTypes)
            {
                if (acctype.Name == accounttypeName)
                    return acctype;
            }
            //If we get here, the role doesn't exist
            throw new ArgumentNullException("No Account Type found with name: " + accounttypeName);
        }

        public string CreateChat(string creator, string documentID)
        {
            Document document = GetDocument(documentID);

            if (null == document)
                throw new Exception("Document with ID " + documentID + " not found.");

            // We've got the version, now we need to add to the chat history.
            if (null == document.Chats)
            {
                document.Chats = new List<Chat>();
            }

            // Create chat and add to the history.
            Chat chat = new Chat();
            chat.Id = ObjectId.GenerateNewId().ToString();
            chat.CreatorUserID = creator;
            chat.DocumentId = documentID;
            chat.StartTime = DateTime.Now.ToUniversalTime().ToString();
            chat.ChatMessages = new List<ChatMessage>();

            document.Chats.Add(chat);

            // Get document versions.
            IMongoCollection<Document> docs = _database.GetCollection<Document>(CommonGlobals.DocumentsCollectionName);

            var filter = Builders<Document>.Filter.Eq(CommonGlobals.IdFieldName, documentID);
            var updateOperation = Builders<Document>.Update
                .Set("Chats", document.Chats);

            // Perform update.
            var result = docs.UpdateOneAsync(filter, updateOperation);

            //Thow exception if we have a problem.
            if (null != result.Exception)
            {
                if (null != result.Exception.InnerException)
                    throw new Exception("Unable to create chat for document: " + result.Exception.InnerException.ToString());
                else
                    throw new Exception("Unable to create chat for document");
            }

            return chat.Id;
        }

        public void AppendToChat(DocumentChatAppend append)
        {
            Document document = GetDocument(append.DocumentID);

            if (null == document)
                throw new Exception("Document with ID " + append.DocumentID + " not found.");

            Chat chat = null;
            //Initialise the doc chats object if required.
            if (document.Chats == null)
            {
                //Create the chat channel and refetch the document
                CreateChat(append.From, append.DocumentID);
                document = GetDocument(append.DocumentID);
            }
            if (string.IsNullOrEmpty(append.Channel))
            {
                chat = document.Chats[document.Chats.Count - 1];
            }
            else
            {
                foreach (Chat next in document.Chats)
                {
                    if (0 == next.Id.CompareTo(append.Channel))
                    {
                        chat = next;
                        break;
                    }
                }
            }

            List<ChatMessage> entries = chat.ChatMessages;
            ChatMessage entry = new ChatMessage();

            entry.FromUserId = append.From;
            entry.Time = DateTime.Now.ToUniversalTime().ToString();
            entry.Message = append.Message;
            entries.Add(entry);

            // Get document versions.
            IMongoCollection<Document> docs = _database.GetCollection<Document>(CommonGlobals.DocumentsCollectionName);

            var filter = Builders<Document>.Filter.Eq(CommonGlobals.IdFieldName, append.DocumentID);
            var updateOperation = Builders<Document>.Update
                .Set("Chats", document.Chats);

            // Perform update.
            var result = docs.UpdateOneAsync(filter, updateOperation);

            //Thow exception if we have a problem.
            if (null != result.Exception)
            {
                if (null != result.Exception.InnerException)
                    throw new Exception("Unable to append to chat for document: " + result.Exception.InnerException.ToString());
                else
                    throw new Exception("Unable to append to chat for document");
            }



        }

        public void TerminateChat(string chatID, string documentID)
        {
            if (null == documentID)
            {
                return;
            }

            Document document = GetDocument(documentID);

            if (null == document)
                throw new Exception("Document with ID " + documentID + " not found.");

            Chat chat = null;

            if (string.IsNullOrEmpty(chatID))
            {
                chat = document.Chats[document.Chats.Count - 1];
            }
            else
            {
                foreach (Chat next in document.Chats)
                {
                    if (0 == next.Id.CompareTo(chatID))
                    {
                        chat = next;
                        break;
                    }
                }
            }

            chat.EndTime = DateTime.Now.ToUniversalTime().ToString();

            // Get document versions.
            IMongoCollection<Document> docs = _database.GetCollection<Document>(CommonGlobals.DocumentsCollectionName);

            var filter = Builders<Document>.Filter.Eq(CommonGlobals.IdFieldName, documentID);
            var updateOperation = Builders<Document>.Update
                .Set("Chats", document.Chats);

            // Perform update.
            var result = docs.UpdateOneAsync(filter, updateOperation);

            //Thow exception if we have a problem.
            if (null != result.Exception)
            {
                if (null != result.Exception.InnerException)
                    throw new Exception("Unable to append to chat for document: " + result.Exception.InnerException.ToString());
                else
                    throw new Exception("Unable to append to chat for document");
            }
        }


        #region Roles
        public List<RoleInfo> GetRolesList(RoleListGet request)
        {
            var returnValue = new List<string>();
            IMongoCollection<RoleInfo> roles = _database.GetCollection<RoleInfo>(CommonGlobals.RolesCollectionName);
            var matchingRoles = roles.AsQueryable().ToList();
            return matchingRoles;
        }

        public RoleInfo GetRoleByID(string roleID)
        {
            IMongoCollection<RoleInfo> roles = _database.GetCollection<RoleInfo>(CommonGlobals.RolesCollectionName);
            var matchingRoles = roles.AsQueryable().ToList();
            foreach (var role in matchingRoles)
            {
                if (role.Id == roleID)
                    return role;
            }
            //If we get here, the role doesn't exist
            throw new ArgumentNullException("No role found with ID: " + roleID);
        }

        public RoleInfo GetRoleByName(string roleName)
        {
            IMongoCollection<RoleInfo> roles = _database.GetCollection<RoleInfo>(CommonGlobals.RolesCollectionName);
            var matchingRoles = roles.AsQueryable().ToList();
            foreach (var role in matchingRoles)
            {
                if (role.Name == roleName)
                    return role;
            }
            //If we get here, the role doesn't exist
            throw new ArgumentNullException("No role found with name: " + roleName);
        }


        public List<RoleInfo> GetUserRoles(UserRoleInfoGet request)
        {
            var returnValue = new List<RoleInfo>();
            //Get the user
            var userCollection = _database.GetCollection<UserAuth>(CommonGlobals.UserAuthCollectionName);
            var users = userCollection.AsQueryable().ToList();
            foreach (var user in users)
            {
                if (user.UserName == request.Username)
                {
                    //Get the list of roles
                    foreach (var roleID in user.Roles)
                    {
                        //Get the role info for each role
                        returnValue.Add(GetRoleByID(roleID));
                    }
                    break;
                }
            }
            //Return the data
            return returnValue;
        }

        public SystemConfig GetSystemConfig(SystemConfigGet request)
        {
            var config = new SystemConfig();
            //Get the first bank and corporate in the database. 
            //We'll use this as the bank and corporate to use by default for the user
            var banksCollection = _database.GetCollection<Bank>(CommonGlobals.BanksCollectionName);
            var firstBank = banksCollection.AsQueryable().FirstOrDefault();
            if (firstBank != null)
            {
                config.BankId = firstBank.Id;
                config.BankDisplayName = firstBank.Name;
                config.BankIcon = firstBank.IconBase64;
            }
            var corpCollection = _database.GetCollection<Corporate>(CommonGlobals.CorporatesCollectionName);
            var firstCorp = corpCollection.AsQueryable().FirstOrDefault();
            if (firstCorp != null)
            {
                config.CorporateId = firstCorp.Id;
                config.CorporateName = firstCorp.Detail.Name;
                config.CorporateIcon = firstCorp.Icon;
            }
            return config;
        }

        public RoleInfo InsertRole(RoleInfoInsert request)
        {
            // Make sure name is specified.
            if (string.IsNullOrEmpty(request.Role.Name))
            {
                throw new Exception("Role can not be created, name not specified");
            }

            try
            {
                RoleInfo role = GetRoleByName(request.Role.Name);
                throw new Exception("Role can not be created, " + request.Role.Name + " already exists");
            }
            catch (Exception) { }

            if (string.IsNullOrEmpty(request.Role.Id))
                request.Role.Id = ObjectId.GenerateNewId().ToString();

            if (null == request.Role.Permissions)
                request.Role.Permissions = new List<Permission>();

            IMongoCollection<RoleInfo> roles = _database.GetCollection<RoleInfo>(CommonGlobals.RolesCollectionName);
            roles.InsertOne(request.Role);
            return request.Role;
        }

        public void UpdateRole(RoleInfoUpdate request)
        {
            try
            {
                RoleInfo role = GetRoleByName(request.Role.Name);

            }
            catch (Exception)
            {
                throw new Exception("Role can not be updated, " + request.Role.Name + " does not exist.");
            }

            IMongoCollection<RoleInfo> roles = _database.GetCollection<RoleInfo>(CommonGlobals.RolesCollectionName);
            var filter = Builders<RoleInfo>.Filter.Eq("Name", request.Role.Name);
            var update = Builders<RoleInfo>.Update.Set("Role", request.Role);
            var result = roles.UpdateOne(filter, update);
        }

        public void DeleteRole(RoleInfoDelete request)
        {
            string roleID = request.RoleId;

            // Make sure name is specified.
            if (string.IsNullOrEmpty(roleID))
            {
                throw new Exception("Role can not be deleted, ID not specified");
            }

            try
            {
                RoleInfo roleinfo = GetRoleByID(roleID);
            }
            catch (Exception)
            {
                throw new Exception("Role can not be deleted, " + roleID + " does not exist");
            }

            // Get Groups for Role. Remove role from groups.
            List<Group> groups = GetGroupsForRole(new GetGroupsForRole() { RoleId = roleID });

            // Remove Role Info from groups.
            foreach (Group group in groups)
            {
                RemoveRoleInfoFromGroup(roleID, group.Id);
            }

            // Delete RoleInfo.
            IMongoCollection<RoleInfo> coll = _database.GetCollection<RoleInfo>(CommonGlobals.RolesCollectionName);
            var filter = Builders<RoleInfo>.Filter.Eq(CommonGlobals.IdFieldName, roleID);
            var result = coll.DeleteMany(filter);
        }


        /// <summary>
        /// Get groups which contain this Role.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public List<Group> GetGroupsForRole(GetGroupsForRole request)
        {
            IMongoCollection<Group> groups = _database.GetCollection<Group>(CommonGlobals.GroupsCollectionName);
            List<Group> matchingGroups = groups.AsQueryable().ToList();
            List<Group> retGroups = new List<Group>();

            // Iterate through all groups, only return if this role is contained in the group.
            foreach (Group group in matchingGroups)
            {
                List<string> roles = group.Roles;
                if (roles.Contains(request.RoleId))
                    retGroups.Add(group);
            }
            return retGroups;

        }


        /// <summary>
        /// Get groups which contain this Role.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public List<Group> GetGroupsForUser(int userID)
        {
            IMongoCollection<Group> groups = _database.GetCollection<Group>(CommonGlobals.GroupsCollectionName);
            List<Group> matchingGroups = groups.AsQueryable().ToList();
            List<Group> retGroups = new List<Group>();

            foreach (Group group in matchingGroups)
            {
                List<int> users = group.UserAuthIds;
                if (users.Contains(userID))
                {
                    retGroups.Add(group);
                }
            }

            return retGroups;
        }


        #endregion

        #region Users

        /// <summary>
        /// Validate whether user can be created.
        /// </summary>
        /// <param name="UserToCreate"></param>
        /// <param name="config"></param>
        public void ValidateCreateUser(UserAuth UserToCreate, UserConfig config)
        {
            // Firstly check if we are creating a Super user. Must be a super user.
            IUserAuth currentUser = GetUserByName("");

            // Check for master admin/admin.
            // Return if this is "admin", allow creation.
            if (0 == currentUser.UserName.CompareTo("admin"))
                return;

            UserConfig currentconfig = GetUserConfig(currentUser.Id);
            switch (config.UserPrivilege)
            {
                case Privilege.SuperAdmin:
                    {
                        if (!((currentUser.UserName == "admin") || (currentconfig.UserPrivilege == Privilege.SuperAdmin)))
                        {
                            throw new AuthenticationException("Current user is not a Super Admin");
                        }
                        break;
                    }
                case Privilege.Admin:
                case Privilege.User:
                    {
                        if (!(currentconfig.UserPrivilege == Privilege.SuperAdmin))
                        {
                            // We're not a super admin, are we an admin.
                            if (!(currentconfig.UserPrivilege == Privilege.Admin)
                                || ((currentconfig.UserPrivilege == Privilege.Admin) && (config.EntityID != currentconfig.EntityID)))
                                throw new AuthenticationException("Current user does not have access to create an Admin user.");

                        }
                        break;
                    }
                default:
                    break;
            }


        }


        public List<UserAuth> GetUsersList()
        {
            IMongoCollection<UserAuth> users = _database.GetCollection<UserAuth>(CommonGlobals.UserAuthCollectionName);
            var matchingUsers = users.AsQueryable().ToList();
            return matchingUsers;
        }
        public IUserAuth GetUserByName(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                if (null == m_session)
                    m_session = m_service.GetSession();
                username = m_session.UserName;
            }

            IAuthRepository repos = m_service.AuthRepository;
            return repos.GetUserAuthByUserName(username);
            /*IMongoCollection<UserAuth> users = _database.GetCollection<UserAuth>(CommonGlobals.UserAuthCollectionName);
            var matchingUsers = users.AsQueryable();//.ToList();
            foreach (var user in matchingUsers)
            {
                if (user.UserName == username)
                    return user;
            }
            //If we get here, the role doesn't exist
            throw new ArgumentNullException("No user found with name: " + username);*/
        }

        /* public UserAuth GetUserByID(int userID)
         {
             IMongoCollection<UserAuth> users = _database.GetCollection<UserAuth>(CommonGlobals.UserAuthCollectionName);
             var matchingUsers = users.AsQueryable().ToList();
             foreach (var user in matchingUsers)
             {
                 if (user.Id == userID)
                     return user;
             }
             //If we get here, the role doesn't exist
             throw new ArgumentNullException("No user found with ID: " + userID);
         }*/

        /// <summary>
        /// Get user config by id.
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public UserConfig GetUserConfig(int userID)
        {
            IMongoCollection<UserConfig> users = _database.GetCollection<UserConfig>(CommonGlobals.UserConfigCollectionName);
            var matchingUsers = users.AsQueryable().ToList();
            foreach (var user in matchingUsers)
            {
                if (user.UserId == userID)
                    return user;
            }
            //If we get here, the role doesn't exist
            throw new ArgumentNullException("No user found with ID: " + userID);
        }

        /// <summary>
        /// Create user config.
        /// </summary>
        /// <param name="config"></param>
        public void CreateUser(UserAuth userToCreate, string password, UserConfig config)
        {
            ValidateCreateUser(userToCreate, config);

            // Create user.
            var repos = m_service.AuthRepository;
            repos.CreateUserAuth(userToCreate, password);

            // Get newly created user.
            IUserAuth createdUser = GetUserByName(userToCreate.UserName);
            config.UserId = createdUser.Id;

            IMongoCollection<UserConfig> userConfig = _database.GetCollection<UserConfig>(CommonGlobals.UserConfigCollectionName);
            if (string.IsNullOrEmpty(config.Id))
            {
                // Set Id on Config object.
                config.Id = ObjectId.GenerateNewId().ToString();
            }

            // Check to see if this is a duplicate user. Reject request if it is.
            // Make sure user is not already in collection.
            var matching = userConfig.AsQueryable().ToList();
            bool bFound = false;
            foreach (var nextUser in matching)
            {
                if (nextUser.UserId == config.UserId)
                {
                    bFound = true;
                    break;
                }
            }

            // Don't allow this if there is already a user with this ID.
            if (bFound)
            {
                throw new Exception("User with ID " + config.UserId + " already exists in database");
            }

            //Create an id for the new user
            userConfig.InsertOne(config);
        }

        /// <summary>
        /// Update user (update privilege only).
        /// </summary>
        /// <param name="user"></param>
        /// <param name="config"></param>
        public void UpdateUser(UserAuth user, UserConfig config)
        {
            // Replace UserAuth object.
            IMongoCollection<UserAuth> userAuthCollection = _database.GetCollection<UserAuth>(CommonGlobals.UserAuthCollectionName);
            var filter = Builders<UserAuth>.Filter.Eq("UserName", user.UserName);
            var result = userAuthCollection.ReplaceOne(filter, user);

            if (config != null)
            {
                // Get user config objects.
                IMongoCollection<UserConfig> userConfigCollection = _database.GetCollection<UserConfig>(CommonGlobals.UserConfigCollectionName);

                // Make sure name is specified.
                var matching = userConfigCollection.AsQueryable().ToList();
                bool bFound = false;

                foreach (var nextUser in matching)
                {
                    if (nextUser.UserId == config.UserId)
                    {
                        bFound = true;
                        break;
                    }
                }

                if (!bFound)
                {
                    throw new Exception("Unable to find userconfig with ID " + config.UserId);
                }

                var collection = _database.GetCollection<UserConfig>(CommonGlobals.UserConfigCollectionName);
                var configfilter = Builders<UserConfig>.Filter.Eq(CommonGlobals.UserIdFieldName, config.UserId);
                var updateOperation = Builders<UserConfig>.Update
                    .Set("UserPrivilege", config.UserPrivilege);

                // Perform update.
                var configresult = collection.UpdateOneAsync(configfilter, updateOperation);

                //Thow exception if we have a problem.
                if (null != configresult.Exception)
                {
                    if (null != configresult.Exception.InnerException)
                        throw new Exception("Unable to update user: " + configresult.Exception.InnerException.ToString());
                    else
                        throw new Exception("Unable to update user");
                }
            }
        }

        /// <summary>
        /// Delete user by ID.
        /// </summary>
        /// <param name="userID"></param>
        public void DeleteUser(int userID)
        {
            IMongoCollection<UserConfig> coll = _database.GetCollection<UserConfig>(CommonGlobals.UserConfigCollectionName);

            // Make sure name is specified.
            var matching = coll.AsQueryable().ToList();
            bool bFound = false;

            foreach (var nextUser in matching)
            {
                if (nextUser.UserId == userID)
                {
                    bFound = true;
                    break;
                }
            }

            if (bFound)
            {
                var filter = Builders<UserConfig>.Filter.Eq(CommonGlobals.IdFieldName, userID);
                var result = coll.DeleteMany(filter);
            }

            // Delete user from userauth table.
            IMongoCollection<UserAuth> userAuthcoll = _database.GetCollection<UserAuth>(CommonGlobals.UserAuthCollectionName);
            var userauthfilter = Builders<UserAuth>.Filter.Eq(CommonGlobals.IdFieldName, userID);
            var userauthresult = userAuthcoll.DeleteMany(userauthfilter);
        }


        #endregion

        #region GROUP

        public List<Group> GetGroupsList()
        {
            IMongoCollection<Group> groups = _database.GetCollection<Group>(CommonGlobals.GroupsCollectionName);
            var matchingGroups = groups.AsQueryable().ToList();
            return matchingGroups;
        }

        public Group GetGroupByName(string groupName)
        {
            IMongoCollection<Group> groups = _database.GetCollection<Group>(CommonGlobals.GroupsCollectionName);
            var matching = groups.AsQueryable().ToList();
            foreach (var nextGroup in matching)
            {
                if (nextGroup.Name == groupName)
                    return nextGroup;
            }
            //If we get here, the role doesn't exist
            throw new ArgumentNullException("No group found with name: " + groupName);
        }

        public Group GetGroupByID(string groupId)
        {
            IMongoCollection<Group> groups = _database.GetCollection<Group>(CommonGlobals.GroupsCollectionName);
            var matching = groups.AsQueryable().ToList();
            foreach (var nextGroup in matching)
            {
                if (nextGroup.Id == groupId)
                    return nextGroup;
            }
            //If we get here, the role doesn't exist
            throw new ArgumentNullException("No group found with ID: " + groupId);
        }

        public void CreateGroup(Group group)
        {
            // Make sure name is specified.
            if (string.IsNullOrEmpty(group.Name))
            {
                throw new Exception("Group can not be created, name not specified");
            }

            try
            {
                Group gp = GetGroupByName(group.Name);
                throw new Exception("Group can not be created, " + group.Name + " already exists");
            }
            catch (Exception) { }

            if (null == group.Roles)
                group.Roles = new List<string>();
            if (null == group.UserAuthIds)
                group.UserAuthIds = new List<int>();
            if (null == group.Subgroups)
                group.Subgroups = new List<string>();

            if (string.IsNullOrEmpty(group.Id))
                group.Id = ObjectId.GenerateNewId().ToString();
            IMongoCollection<Group> groups = _database.GetCollection<Group>(CommonGlobals.GroupsCollectionName);
            groups.InsertOne(group);
        }

        public void DeleteGroup(string groupID)
        {
            // Make sure name is specified.
            if (string.IsNullOrEmpty(groupID))
            {
                throw new Exception("Group can not be deleted, name not specified");
            }

            try
            {
                Group gp = GetGroupByID(groupID);
            }
            catch (Exception)
            {
                throw new Exception("Group can not be deleted, group with ID " + groupID + " does not exist");
            }

            IMongoCollection<Group> coll = _database.GetCollection<Group>(CommonGlobals.GroupsCollectionName);
            var filter = Builders<Group>.Filter.Eq(CommonGlobals.IdFieldName, groupID);
            var result = coll.DeleteMany(filter);
        }


        /// <summary>
        /// Add single user to group.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="user"></param>
        public void AddUserToGroup(string groupId, string username)
        {
            // We need to do some validation here first.
            if (string.IsNullOrEmpty(groupId))
                throw new Exception("Group must be specified when adding user to group");

            if (string.IsNullOrEmpty(username))
                throw new Exception("User name must be specified when adding user to group");

            Group gp = null;
            try
            {
                gp = GetGroupByID(groupId);
            }
            catch (Exception)
            {
                throw new Exception("Group " + groupId + " not found, user can not be added.");
            }

            IUserAuth user = null;
            try
            {
                user = GetUserByName(username);
            }
            catch (Exception)
            {
                throw new Exception("User with name " + username + " not found, user can not be added to group.");
            }

            // Get ID for user.
            int nUserID = user.Id;

            List<int> userIDs = gp.UserAuthIds;
            if (null != userIDs)
            {
                foreach (int nextUser in userIDs)
                {
                    if (nUserID == nextUser)
                    {
                        throw new Exception("User with name " + username + " is already present in the group.");
                    }
                }
            }

            // Add user to group - just pass a list of userID's of length 1.
            AddChildrenToList<Group, int>(CommonGlobals.GroupsCollectionName, gp.Id, "UserAuthIds", new List<int>() { nUserID });
        }

        /// <summary>
        /// Add multiple users to group,
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userIds"></param>
        public void AddUsersToGroup(string groupId, List<int> userIds)
        {
            // We need to do some validation here first.
            // Make sure group exists.
            Group gp = null;
            try
            {
                gp = GetGroupByID(groupId);
            }
            catch (Exception)
            {
                throw new Exception("Group " + groupId + " not found, users can not be added");
            }

            // Now make sure none of these User IDs are already in the group.
            List<int> groupuserIDs = gp.UserAuthIds;
            if (null != groupuserIDs)
            {
                foreach (int nextGroupUser in groupuserIDs)
                {
                    foreach (int nextInputUser in userIds)
                    {
                        if (0 == nextInputUser)
                        {
                            throw new Exception("Invalid ID of 0 Specified for user.");
                        }
                        if (nextInputUser == nextGroupUser)
                        {
                            throw new Exception("User with ID " + nextInputUser + " is already in group " + gp.Name);
                        }
                    }
                }
            }


            AddChildrenToList<Group, int>(CommonGlobals.GroupsCollectionName, groupId, "UserAuthIds", userIds);
        }

        /// <summary>
        /// Delete single user from group.
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        public void DeleteUserFromGroup(string groupId, int userId)
        {
            // We need to do some validation here first.
            // Make sure group exists.
            Group gp = null;
            try
            {
                gp = GetGroupByID(groupId);
            }
            catch (Exception)
            {
                throw new Exception("Group " + groupId + " not found, user can not be deleted.");
            }

            // Now make sure the user does actually exist in the group,
            bool bFound = false;
            List<int> userIDs = gp.UserAuthIds;
            if (null != userIDs)
            {
                foreach (int nextUser in userIDs)
                {
                    if (userId == nextUser)
                    {
                        bFound = true;
                    }
                }
            }

            if (!bFound)
            {
                throw new Exception("User with ID " + userId + " not found in group, user can not be deleted.");
            }

            DeleteChildFromList<Group, int>(CommonGlobals.GroupsCollectionName, groupId, "UserAuthIds", userId);
        }

        /// <summary>
        /// Add Role to group.
        /// </summary>
        /// <param name="role"></param>
        /// <param name="group"></param>
        public void AddRoleInfoToGroup(string roleId, string groupId)
        {
            // Make sure Group exists.
            Group gp = null;
            try
            {
                gp = GetGroupByID(groupId);
            }
            catch (Exception)
            {
                throw new Exception("Group " + groupId + " not found, Role can not be added to group.");
            }

            // Make sure Role exists.
            RoleInfo role = null;
            try
            {
                role = GetRoleByID(roleId);
            }
            catch (Exception)
            {
                throw new Exception("Role " + roleId + " not found, Role can not be added to group.");
            }

            AddChildToStringList<Group>(CommonGlobals.GroupsCollectionName, groupId, CommonGlobals.RolesChildName, roleId);
        }

        public void AddRolesToGroup(List<string> roleIds, string groupId)
        {
            // Make sure Group exists.
            Group gp = null;
            try
            {
                gp = GetGroupByID(groupId);
            }
            catch (Exception)
            {
                throw new Exception("Group " + groupId + " not found, Role can not be added to group.");
            }

            // Make sure each of these roleIDs represent a valid role.
            foreach (string roleid in roleIds)
            {
                try
                {
                    RoleInfo role = GetRoleByID(roleid);
                }
                catch (Exception)
                {
                    throw new Exception(roleid + " is not a valid Role ID, Roles will not be added to group.");
                }
            }

            // Ensure one of these roles is not already in the group.
            List<string> grouproles = gp.Roles;
            foreach (string grouprole in grouproles)
            {
                foreach (string inputrole in roleIds)
                {
                    if (grouprole == inputrole)
                    {
                        // We have a clash here, this role is already in the group,
                        throw new Exception("Role with ID " + grouprole + " is already in group with ID " + groupId + ", Roles will not be added");
                    }
                }
            }

            AddChildrenToList<Group, string>(CommonGlobals.GroupsCollectionName, groupId, CommonGlobals.RolesChildName, roleIds);
        }


        public void RemoveRoleInfoFromGroup(string role, string group)
        {
            DeleteChildFromStringList<Group>(CommonGlobals.GroupsCollectionName, group, CommonGlobals.RolesChildName, role);
        }

        public List<UserAuth> GetUsersForGroup(string group)
        {
            Group groupObj = GetObjectByFieldID<Group>(CommonGlobals.GroupsCollectionName, CommonGlobals.IdFieldName, group);

            if (null == group)
            {
                throw new Exception("Unable to find Group with id " + group);
            }

            List<UserAuth> users = new List<UserAuth>();
            foreach (int userId in groupObj.UserAuthIds)
            {
                UserAuth user = GetObjectByFieldID<UserAuth>(CommonGlobals.UserAuthCollectionName, CommonGlobals.IdFieldName, userId);
                if (null != user)
                {
                    users.Add(user);
                }
            }

            return users;
        }


        public List<RoleInfo> GetRolesForGroup(string groupId)
        {
            Group gp = null;
            try
            {
                gp = GetGroupByID(groupId);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to find Group with ID " + groupId);
            }

            List<RoleInfo> roles = new List<RoleInfo>();
            foreach (string role in gp.Roles)
            {
                RoleInfo roleObj = null;
                try
                {
                    roleObj = GetRoleByID(role);
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to add Roles to Group: " + ex.Message);
                }
                if (null != roleObj)
                {
                    roles.Add(roleObj);
                }
            }

            return roles;
        }


        #endregion

        #region PERMISSIONS
        /// <summary>
        /// Add AppPermission to RoleInfo.
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="permission"></param>
        public void AddPermissionToRoleInfo(string roleID, Permission permission)
        {
            RoleInfo roleinfo = null;
            try
            {
                roleinfo = GetRoleByID(roleID);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to add permission to Role: " + ex.Message);
            }

            AddChildToList<RoleInfo, Permission>(CommonGlobals.RolesCollectionName, roleID, "Permissions", permission);
        }


        public void UpdatePermissionInRoleInfo(string roleID, Permission permission)
        {
            RoleInfo parentObj = null;
            try
            {
                parentObj = GetRoleByID(roleID);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to update permission in Role: " + ex.Message);
            }

            List<Permission> permissions = parentObj.Permissions;

            for (int i = 0; i < permissions.Count; i++)
            {
                if (0 == permissions[i].Id.CompareTo(permission.Id))
                {
                    permissions[i] = permission;
                    break;
                }
            }

            // Get document versions.
            IMongoCollection<RoleInfo> roles = _database.GetCollection<RoleInfo>(CommonGlobals.RolesCollectionName);

            var filter = Builders<RoleInfo>.Filter.Eq(CommonGlobals.IdFieldName, roleID);
            var updateOperation = Builders<RoleInfo>.Update
                .Set("Permissions", permissions);

            // Perform update.
            var result = roles.UpdateOneAsync(filter, updateOperation);

        }

        public void DeletePermissionFromRoleInfo(string roleID, string permission)
        {
            RoleInfo parentObj = null;
            try
            {
                parentObj = GetRoleByID(roleID);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to delete permission in Role: " + ex.Message);
            }

            List<Permission> permissions = parentObj.Permissions;
            Permission permToDelete = null;
            for (int i = 0; i < permissions.Count; i++)
            {
                if (0 == permissions[i].Id.CompareTo(permission))
                {
                    permToDelete = permissions[i];
                    break;
                }
            }

            DeleteChildFromList<RoleInfo, Permission>(CommonGlobals.RolesCollectionName, roleID, "Permissions", permToDelete);
        }

        public string GetBankIDFromDocumentID(string id)
        {
            Document obj = null;
            try
            {
                obj = GetObjectByFieldID<Document>(CommonGlobals.DocumentsCollectionName, CommonGlobals.IdFieldName, id);
            }
            catch (Exception)
            {
                throw new Exception("Document with id " + id + " not found.");
            }

            if (null != obj)
            {
                Account account = GetAccount(obj.Accounts[0]);
                return account.ParentID;
            }
            else throw new Exception("Document with id " + id + " not found.");

        }

        public string GetCorporateIDFromDocumentID(string id)
        {

            Document obj = null;
            try
            {
                obj = GetObjectByFieldID<Document>(CommonGlobals.DocumentsCollectionName, CommonGlobals.IdFieldName, id);
            }
            catch (Exception)
            {
                throw new Exception("Document with id " + id + " not found.");
            }

            return GetCorporateIDFromAccountID(obj.Accounts[0]);

        }

        public string GetCorporateIDFromAccountID(string id)
        {
            if (null == id)
                throw new Exception("Corporate with id " + id + " not found.");

            Account account = GetAccount(id);
            if (null != account)
                return account.ParentID;
            else return null;
        }

        public string GetBankNameFromID(string id)
        {
            Bank obj = null;
            try
            {
                obj = GetObjectByFieldID<Bank>(CommonGlobals.BanksCollectionName, CommonGlobals.IdFieldName, id);
            }
            catch (Exception)
            {
                throw new Exception("Bank with id " + id + " not found.");
            }

            if (null != obj)
                return obj.Name;
            else throw new Exception("Bank with id " + id + " not found.");
        }


        public string GetCorporateNameFromID(string id)
        {
            Corporate corporate = GetObjectByFieldID<Corporate>(CommonGlobals.CorporatesCollectionName, CommonGlobals.IdFieldName, id);
            if (null != corporate)
                return corporate.Detail.Name;
            else throw new Exception("Corporate with id " + id + " not found.");
        }

        #endregion

        #region HELPER_FUNCTIONS
        /// <summary>
        /// General field update.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collName"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        /// <param name=CommonGlobals.IdFieldName></param>
        private void UpdateField<T>(string collName, string fieldName, object fieldValue, string ID)
        {
            IMongoCollection<T> coll = _database.GetCollection<T>(collName);

            var updateOperation = Builders<T>.Update
                    .Set(fieldName, fieldValue);

            // Update list of accounts for bank (with account reference removed.
            var filter = Builders<T>.Filter.Eq(CommonGlobals.IdFieldName, ID);
            var result = coll.UpdateOneAsync(filter, updateOperation);
        }


        private List<T> GetColl<T>(string type)
        {
            IMongoCollection<T> entries = _database.GetCollection<T>(type);
            var filt = Builders<T>.Filter.Empty;
            List<T> results = new List<T>();
            foreach (var nextEntry in entries.Find(filt).ToListAsync().Result)
            {
                results.Add(nextEntry);
            }

            return results;
        }

        private T GetObjectByFieldID<T>(string collName, string IDField, string IDValue)
        {
            // Get collection.
            IMongoCollection<T> coll = _database.GetCollection<T>(collName);
            var filter = Builders<T>.Filter.Eq(IDField, IDValue);

            // Find object with this ID (should only be one).
            List<T> list = coll.Find(filter).ToList();

            if (0 == list.Count)
                throw new Exception("Object of type " + typeof(T).ToString() + " with id " + IDValue + " not found.");

            return list[0];
        }

        private T GetObjectByFieldID<T>(string collName, string IDField, int IDValue)
        {
            // Get collection.
            IMongoCollection<T> coll = _database.GetCollection<T>(collName);
            var filter = Builders<T>.Filter.Eq(IDField, IDValue);

            // Find object with this ID (should only be one).
            List<T> list = coll.Find(filter).ToList();

            if (0 == list.Count)
                throw new Exception("Object of type " + typeof(T).ToString() + " with id " + IDValue + " not found.");

            return list[0];
        }


        /// <summary>
        /// Delete child from List<string> children
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collName"></param>
        /// <param name="parent"></param>
        /// <param name="childList"></param>
        /// <param name="IDValue"></param>
        private void DeleteChildFromStringList<T>(string collName, string parent, string childList, string IDValue)
        {
            if (string.IsNullOrEmpty(parent))
            {
                throw new Exception("Name in " + collName + " must be specified");
            }

            if (string.IsNullOrEmpty(IDValue))
            {
                throw new Exception("Name in " + childList + " must be specified");
            }

            if (!DoesValueExistInChildList<T>(collName, parent, childList, IDValue))
                throw new Exception("Value " + IDValue + " not found in " + childList);

            IMongoCollection<T> coll = _database.GetCollection<T>(collName);

            // Update children.
            var parentfilter = Builders<T>.Filter.Eq(CommonGlobals.IdFieldName, parent);
            var updateOperation = Builders<T>.Update.Pull<string>(childList, IDValue);
            var result = coll.UpdateOneAsync(parentfilter, updateOperation);
        }


        private void AddChildToStringList<T>(string collName, string parent, string childList, string IDValue)
        {
            if (string.IsNullOrEmpty(parent))
            {
                throw new Exception("Name in " + collName + " must be specified");
            }

            if (string.IsNullOrEmpty(IDValue))
            {
                throw new Exception("Name in " + childList + " must be specified");
            }

            if (DoesValueExistInChildList<T>(collName, parent, childList, IDValue))
                throw new Exception("Value " + IDValue + " already exists");

            // Update children.
            IMongoCollection<T> coll = _database.GetCollection<T>(collName);

            var parentfilter = Builders<T>.Filter.Eq(CommonGlobals.IdFieldName, parent);
            var updateOperation = Builders<T>.Update.Push<string>(childList, IDValue);
            var result = coll.UpdateOneAsync(parentfilter, updateOperation);
        }


        private bool DoesValueExistInChildList<T>(string collName, string parent, string childList, string IDValue)
        {
            // Get object by name.
            T obj;
            try
            {
                obj = GetObjectByFieldID<T>(collName, CommonGlobals.IdFieldName, parent);
            }
            catch (Exception)
            {
                throw new Exception("Object with name " + parent + " not found");
            }

            // Now get the child List from the object.
            Type type = typeof(T);
            MemberInfo info = type.GetField(childList) as MemberInfo ??
                     type.GetProperty(childList) as MemberInfo;

            List<string> children = GetValue(info, obj) as List<string>;
            bool bFound = false;
            foreach (string str in children)
            {
                if (0 == str.CompareTo(IDValue))
                {
                    bFound = true;
                    break;
                }
            }

            return bFound;
        }



        public object GetValue(MemberInfo member, object property)
        {
            if (member.MemberType == MemberTypes.Property)
                return ((PropertyInfo)member).GetValue(property, null);
            else if (member.MemberType == MemberTypes.Field)
                return ((FieldInfo)member).GetValue(property);
            else
                throw new Exception("Property must be of type FieldInfo or PropertyInfo");
        }

        /// <summary>
        /// Delete child from List<string> children
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collName"></param>
        /// <param name="parent"></param>
        /// <param name="childList"></param>
        /// <param name="IDValue"></param>
        private void DeleteChildFromList<T, U>(string collName, string parent, string childList, U childValue)
        {
            T parentObj = GetObjectByFieldID<T>(collName, CommonGlobals.IdFieldName, parent);
            IMongoCollection<T> coll = _database.GetCollection<T>(collName);

            // Update children.
            var parentfilter = Builders<T>.Filter.Eq(CommonGlobals.IdFieldName, parent);
            var updateOperation = Builders<T>.Update.Pull<U>(childList, childValue);
            var result = coll.UpdateOneAsync(parentfilter, updateOperation);
        }

        private void AddChildToList<T, U>(string collName, string parent, string childList, U childValue)
        {
            T parentObj = GetObjectByFieldID<T>(collName, CommonGlobals.IdFieldName, parent);
            IMongoCollection<T> coll = _database.GetCollection<T>(collName);

            // Update children.
            var parentfilter = Builders<T>.Filter.Eq(CommonGlobals.IdFieldName, parent);
            var updateOperation = Builders<T>.Update.Push<U>(childList, childValue);
            var result = coll.UpdateOneAsync(parentfilter, updateOperation);
        }

        private void AddChildrenToList<T, U>(string collName, string parent, string childList, IEnumerable<U> childValue)
        {
            T parentObj = GetObjectByFieldID<T>(collName, CommonGlobals.IdFieldName, parent);
            IMongoCollection<T> coll = _database.GetCollection<T>(collName);

            // Update children.
            var parentfilter = Builders<T>.Filter.Eq(CommonGlobals.IdFieldName, parent);
            var updateOperation = Builders<T>.Update.PushEach(childList, childValue);
            var result = coll.UpdateOneAsync(parentfilter, updateOperation);
        }

        private void UpdateChildInList<T, U>(string collName, string parent, string childList, U childValue)
        {
            T parentObj = GetObjectByFieldID<T>(collName, CommonGlobals.IdFieldName, parent);
            IMongoCollection<T> coll = _database.GetCollection<T>(collName);

            // Update children.
            var parentfilter = Builders<T>.Filter.Eq(CommonGlobals.IdFieldName, parent);
            var updateOperation = Builders<T>.Update.Push<U>(childList, childValue);
            var result = coll.UpdateOneAsync(parentfilter, updateOperation);
        }



        public void DeleteNotifications(string documentID, string accountID)
        {
            if (!string.IsNullOrEmpty(documentID))
            {
                IMongoCollection<Notification> coll = _database.GetCollection<Notification>("notifications");
                var filter = Builders<Notification>.Filter.Eq("DocumentID", documentID);
                var result = coll.DeleteMany(filter);
            }
            if (!string.IsNullOrEmpty(accountID))
            {
                IMongoCollection<Notification> coll = _database.GetCollection<Notification>("notifications");
                var filter = Builders<Notification>.Filter.Eq("AccountID", accountID);
                var result = coll.DeleteMany(filter);
            }
        }

        /// <summary>
        /// Resolve object status. Note that the status of a child affects the status of a parent.
        /// </summary>
        /// <param name="childstatus"></param>
        /// <param name="status"></param>
        /// <param name="objecttype"></param>
        /// <param name=CommonGlobals.IdFieldName></param>
        private void ResolveStatus(Status childstatus, Status status, ObjectType_e objecttype, string id)
        {
            StatusEx statusex = null;
            Corporate corporate = null;
            Bank bank = null;
            Account account = null;
            Document document = null;
            switch (objecttype)
            {
                case ObjectType_e.Corporate_e:
                    {
                        corporate = GetObjectByFieldID<Corporate>(CommonGlobals.CorporatesCollectionName, CommonGlobals.IdFieldName, id);
                        statusex = corporate.Status;
                        break;
                    }
                case ObjectType_e.Bank_e:
                    {
                        bank = GetObjectByFieldID<Bank>(CommonGlobals.BanksCollectionName, CommonGlobals.IdFieldName, id);
                        statusex = bank.Status;
                        break;
                    }
                case ObjectType_e.Account_e:
                    {
                        account = GetObjectByFieldID<Account>(CommonGlobals.AccountsCollectionName, CommonGlobals.IdFieldName, id);
                        statusex = account.Status;
                        break;
                    }
                case ObjectType_e.Document_e:
                    {
                        document = GetObjectByFieldID<Document>(CommonGlobals.DocumentsCollectionName, CommonGlobals.IdFieldName, id);
                        statusex = document.Status;
                        break;
                    }
                default:
                    break;
            }

            // TODO: Status should always be set on an object, shouldn't be null.
            if (null == statusex)
            {
                statusex = new StatusEx { Status = Status.Approved_e };
            }

            bool bUpdateStatus = true;

            switch (childstatus)
            {
                case Status.NotSet_e:
                    {
                        // We are setting status explicitly.
                        statusex.Status = status;
                        break;
                    }
                case Status.Pending_e:
                    {
                        // We're getting status from child.
                        if (statusex.Status == Status.Approved_e)
                        {
                            statusex.Status = childstatus;
                        }
                        break;

                    }
                case Status.Rejected_e:
                    {
                        // We're getting status from child.
                        if ((statusex.Status == Status.Approved_e) || (statusex.Status == Status.Pending_e))
                        {
                            statusex.Status = childstatus;
                        }
                        break;

                    }
                case Status.Approved_e:
                    {
                        // The child has been approved. In this case we need to check the status of all children. Only set status to approved if 
                        // all the children is approved.
                        bool bAllApproved = true;


                        switch (objecttype)
                        {
                            case ObjectType_e.Corporate_e:
                                {
                                    List<string> accounts = corporate.Accounts;
                                    foreach (string accountid in accounts)
                                    {
                                        Account childaccount = GetObjectByFieldID<Account>(CommonGlobals.AccountsCollectionName, CommonGlobals.IdFieldName, accountid);
                                        if ((childaccount.Status.Status == Status.Pending_e) || (childaccount.Status.Status == Status.Rejected_e))
                                            bAllApproved = false;
                                    }
                                    break;
                                }
                            case ObjectType_e.Account_e:
                                {
                                    List<string> documents = account.Documents;
                                    foreach (string documentid in documents)
                                    {
                                        Document childdocument = GetObjectByFieldID<Document>(CommonGlobals.DocumentsCollectionName, CommonGlobals.IdFieldName, documentid);
                                        if ((childdocument.Status.Status == Status.Pending_e) || (childdocument.Status.Status == Status.Rejected_e))
                                            bAllApproved = false;
                                    }
                                    break;
                                }

                            // The child status cannot be approved for a document, so there are no other clauses here.
                            default:
                                break;
                        }

                        if (!bAllApproved)
                        {
                            statusex.Status = Status.Pending_e;
                        }
                        else
                        {
                            statusex.Status = Status.Approved_e;
                        }
                        break;
                    }
                default:
                    break;
            }

            if (bUpdateStatus)
            {
                switch (objecttype)
                {
                    case ObjectType_e.Corporate_e:
                        {
                            UpdateField<Corporate>(CommonGlobals.CorporatesCollectionName, "Status", statusex, id); break;
                        }
                    case ObjectType_e.Account_e:
                        {
                            UpdateField<Account>(CommonGlobals.AccountsCollectionName, "Status", statusex, id);
                            break;
                        }
                    case ObjectType_e.Document_e:
                        {
                            UpdateField<Document>(CommonGlobals.DocumentsCollectionName, "Status", statusex, id);
                            break;
                        }
                    case ObjectType_e.Bank_e:
                        {
                            UpdateField<Bank>(CommonGlobals.BanksCollectionName, "Status", statusex, id);
                            break;
                        }
                    default:
                        break;
                }
            }
        }

        private Notification CompleteNotify(Notification notify)
        {
            // Store notification in database.
            if (string.IsNullOrEmpty(notify.Id))
            {
                notify.Id = ObjectId.GenerateNewId().ToString();
            }

            if (string.IsNullOrEmpty(notify.CreationDate))
            {
                notify.CreationDate = DateTime.Now.ToString();
            }


            StatusUpdate update = notify.StatusUpdate;

            string documentID = update.DocumentUpdates[0].ID;
            Document document = GetDocument(documentID);
            string accountID = update.AccountUpdates[0].ID;
            Account account = null;
            string corporateID = update.CorporateUpdates[0].ID;

            if (!string.IsNullOrEmpty(accountID) && (0 == accountID.CompareTo(CommonGlobals.GlobalIdentifier)))
            {
                notify.BankID = CommonGlobals.GlobalIdentifier;
                notify.CorporateName = "Groupwide";
                notify.BankName = "Groupwide";
            }
            else
            {
                string bankID = notify.BankName;
                if (string.IsNullOrEmpty(bankID))
                {
                    // If the bank ID is blank, get it from the document ID.
                    try
                    {
                        if (!string.IsNullOrEmpty(accountID))
                        {
                            account = GetAccount(accountID);
                            notify.BankID = account.ParentID;
                        }
                        notify.BankName = GetBankNameFromID(notify.BankID);
                    }
                    catch (Exception ex)
                    {
                        notify.BankID = notify.BankName = "(Unknown)";
                    }

                }
                else
                {
                    if (string.IsNullOrEmpty(notify.BankName))
                    {
                        // Fill in the bank name if it's not set.
                        notify.BankName = GetBankNameFromID(bankID);
                    }
                }

                if (!string.IsNullOrEmpty(corporateID))
                {
                    notify.CorporateName = GetCorporateNameFromID(corporateID);
                }
                else
                {
                    notify.CorporateName = "(Unknown)";
                }
            }

            string userName = notify.UserName;
            if (string.IsNullOrEmpty(notify.UserName))
                notify.UserName = notify.CorporateName + " user";

            // Set processed flag to false initially.
            notify.Processed = false;
            if (string.IsNullOrEmpty(notify.Message))
            {
                switch (notify.Type)
                {
                    case NotificationType.StatusChangeDocument_e:
                        {
                            notify.Message = "Document " + document.Name + " in " + notify.CorporateName + " has been updated";
                            if ((null != update.DocumentUpdates) && update.DocumentUpdates.Count > 0)
                            {
                                switch (update.DocumentUpdates[0].Status)
                                {
                                    case Status.Approved_e:
                                        {
                                            notify.Message = "Document " + document.Name + " in " + notify.CorporateName + " was approved";
                                            break;
                                        }
                                    case Status.Rejected_e:
                                        {
                                            notify.Message = "Document " + document.Name + " in " + notify.CorporateName + " was rejected";
                                            break;
                                        }
                                    case Status.Pending_e:
                                        {
                                            notify.Message = "A new version of " + document.Name + " in " + notify.CorporateName + " was uploaded";
                                            break;
                                        }
                                    default:
                                        break;
                                }
                            }
                            break;
                        }
                    case NotificationType.StatusCreateDocument_e:
                        {
                            notify.Message = "New document " + document.Name + " in " + notify.CorporateName + " was uploaded";
                            break;
                        }
                    case NotificationType.StatusChangeAccount_e:
                        {
                            notify.Message = "Account " + account.Name + " in " + notify.CorporateName + " was modified";


                            if ((null != update.AccountUpdates) && update.AccountUpdates.Count > 0)
                            {
                                switch (update.AccountUpdates[0].Status)
                                {
                                    case Status.Approved_e:
                                        {
                                            notify.Message = "Account " + account.Name + " in " + notify.CorporateName + " was approved";
                                            break;
                                        }
                                    case Status.Rejected_e:
                                        {
                                            notify.Message = "Account " + account.Name + " in " + notify.CorporateName + " was rejected";
                                            break;
                                        }
                                    case Status.Pending_e:
                                        {
                                            notify.Message = "A new version of " + account.Name + " in " + notify.CorporateName + " was uploaded";
                                            break;
                                        }
                                    default:
                                        break;
                                }
                            }

                            break;
                        }
                    default:
                        break;
                }
            }

            return notify;
        }

        #endregion

        #region SECURITY_HELPERS

        // TODO: Match resource parent against User ID. If it doesn't match, you must be super admin
        // or admin/admin to allow permission.

        private void PerformSecurityCheck(Access requiredAccess, string resourceID, Resource resType, string resourceEndParent)
        {
            // Get user based on current sessions username.
            IUserAuth user = null;
            try
            {
                user = GetUserByName("");
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to retrieve user for current session: " + ex.Message);
            }

            if (0 == user.UserName.CompareTo("admin"))
            {
                // Don't continue if we are logged on as admin. Also this user doesn't have a Config object.
                return;
            }

            // We've got the user, get the User Config. If this user is an admin, then return.
            UserConfig config = GetUserConfig(user.Id);

            // If user is a Super Admin then return (allow access).
            if (config.UserPrivilege == Privilege.SuperAdmin)
                return;

            // if user is an admin, and the user's parent matches the resource's parent, then return (allow access).
            // Special case - if resourceEndParent is null, then we allow access if the user is an admin.
            // Example of this requirement is the "GetBanks" call, where the user is belonging to a particular corporate, but
            // the call filter does not specify a corporate.
            bool bAdmin = ((config.UserPrivilege == Privilege.Admin) && ((null == resourceEndParent) || (0 == resourceEndParent.CompareTo(config.EntityID))));
            if (bAdmin)
                return;

            // If we don't find an explicit match of resource ID, we must check against the immediate parent.
            // If parent has an permissions entry, but not the permission you require, authentication has failed.
            // you must go all the way to top of hierarch to determine access, if nothing set at top, then permission denied.

            // Use object ID and type, the type will telll us how far we have to go.
            // Let's get all permissions associated with this user and its groups.
            // Once we have this, we will check permissions against 
            // 1) Current resource ID
            // 2) Parent and subsequent parents.

            List<Permission> permissionsForUser = new List<Permission>();
            List<string> roles = user.Roles;
            if (null != roles)
            {
                foreach (string roleID in roles)
                {

                    RoleInfo roleinfo = GetRoleByID(roleID);
                    if (null != roleinfo)
                    {
                        List<Permission> permissions = roleinfo.Permissions;
                        if (null != permissions)
                        {
                            permissionsForUser.AddRange(permissions);
                        }
                    }
                }
            }

            List<Group> groups = GetGroupsForUser(user.Id);
            foreach (Group group in groups)
            {
                foreach (string roleID in group.Roles)
                {
                    RoleInfo roleinfo = GetRoleByID(roleID);
                    if (null != roleinfo)
                    {
                        List<Permission> permissions = roleinfo.Permissions;
                        if (null != permissions)
                        {
                            permissionsForUser.AddRange(permissions);
                        }
                    }
                }
            }

            bool bFinished = false;
            string nextResourceID = resourceID;
            Resource nextResourceType = resType;
            bool accessGranted = false;
            while (!bFinished && permissionsForUser.Count > 0)
            {
                bool bMatch = CheckResourceAgainstPermissions(permissionsForUser, nextResourceID, nextResourceType, requiredAccess, out accessGranted);

                if (bMatch)
                {
                    if (!accessGranted)
                    {
                        throw new Exception("Access denied for resource " + nextResourceID);
                    }
                    bFinished = true;

                }
                else
                {
                    // We don't have a match, now get parent object and type.
                    // Downgrade required access.
                    if (requiredAccess == Access.Delete)
                        requiredAccess = Access.Write;
                    string parentResource = "";
                    Resource parentResourceType;
                    GetParentResource(nextResourceID, nextResourceType, config.UserType, out parentResource, out parentResourceType);
                    nextResourceID = parentResource;
                    nextResourceType = parentResourceType;
                    if (null == parentResource)
                    {
                        bFinished = true;
                        if (!accessGranted)
                        {
                            throw new Exception("Access denied for resource " + resourceID);
                        }
                    }
                }
            }
            if (permissionsForUser.Count == 0)
            {
                throw new Exception("Access denied for resource " + nextResourceID);
            }
        }

        /// <summary>
        /// Given an object and its type get parent and parents type.
        /// </summary>
        /// <param name="resID"></param>
        /// <param name="resType"></param>
        /// <param name="parentResID"></param>
        /// <param name="parentResType"></param>
        private void GetParentResource(string resID, Resource resType, Entity userType, out string parentResID, out Resource parentResType)
        {
            if ((resType == Resource.Bank) || (resType == Resource.Corporate))
            {
                parentResID = null;
                parentResType = resType;
                return;
            }
            else
            {
                parentResID = null;
                parentResType = Resource.Bank;
                switch (resType)
                {
                    case Resource.Account:
                        {
                            Account account = GetObjectByFieldID<Account>(CommonGlobals.AccountsCollectionName, CommonGlobals.IdFieldName, resID);
                            if (null == account)
                                throw new Exception("Unable to find account with id " + resID);

                            //depending on the user context the parent will be a bank or corporate
                            //if the user is a bank user their permissions will be to a list corporates
                            if (userType == Entity.Bank)
                            {
                                parentResType = Resource.Corporate;
                                parentResID = account.CorporateId;
                            }
                            else if (userType == Entity.Corporate)
                            {
                                parentResType = Resource.Bank;
                                parentResID = account.ParentID;
                            }
                            break;
                        }
                    case Resource.Document:
                        {
                            // Get Document Object.
                            parentResType = Resource.Account;

                            Document document = GetObjectByFieldID<Document>(CommonGlobals.DocumentsCollectionName, CommonGlobals.IdFieldName, resID);
                            if (null == document)
                                throw new Exception("Unable to find document with id " + resID);

                            if ((null != document.Accounts) && (document.Accounts.Count > 0))
                            {
                                parentResID = document.Accounts[0];
                            }
                            break;
                        }
                    case Resource.AccountType:
                        {
                            // Get Document Object.
                            parentResType = Resource.Bank;
                            AccountType accountype = GetObjectByFieldID<AccountType>(CommonGlobals.AccountTypesCollectionName, CommonGlobals.IdFieldName, resID);
                            if (null == accountype)
                                throw new Exception("Unable to find account type with id " + resID);
                            parentResID = accountype.BankID;

                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }

        }



        private bool CheckResourceAgainstPermissions(List<Permission> permissionsForUser, string resourceID, Resource resourceType, Access requiredAccess, out bool accessGranted)
        {
            bool bAccessFound = false;
            bool bAccessGranted = false;
            foreach (Permission perm in permissionsForUser)
            {
                if ((perm.ResourceType == resourceType)
                    && (perm.ResourceId == resourceID))
                {
                    bAccessFound = true;
                    foreach (Access permaccess in perm.Access)
                    {
                        if (requiredAccess == permaccess)
                        {
                            bAccessGranted = true;
                            break;
                        }
                    }
                }
            }

            accessGranted = bAccessGranted;

            return bAccessFound;
        }
        #endregion

    }

}
