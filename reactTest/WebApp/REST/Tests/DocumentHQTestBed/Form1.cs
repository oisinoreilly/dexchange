using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using Models.DTO.V1;
using MongoDB.Bson;
using ServiceStack;
using Newtonsoft.Json;
using DocumentHQ.CommonConfig;
using ServiceStack.Auth;
using DataModels;
using System.IO;
using DataModels.DTO.V1;
using DocumentationHQ;

namespace DocHQTestBed
{
    public partial class Form1 : Form
    {
        private string m_url = CommonGlobals.serverUrl + ":5001";
        private JsonServiceClient m_svc;

        private ServerEventsClient m_SSESVC;
        private List<ServerEventMessage> m_msgs;

        private List<string> m_defaultAccountName = new List<string>() { "Current", "Deposit" };
        private List<string> m_defaultDocumentNames = new List<string>() { "CRS", "W9" };

        private List<string> m_defaultGroups = new List<string>() { "Group1" };
        private List<string> m_defaultUsers = new List<string>() { "User1" };
        private List<string> m_defaultAdmin = new List<string>() { "Admin1" };
        private List<string> m_defaultSuperAdmin = new List<string>() { "SuperAdmin" };

        private List<string> m_defaultRoles = new List<string>() { "Role1" };
        private string m_password = "password";

        private List<Bank> m_defaultBanks;
        private List<Corporate> m_defaultCorporate;

        public Form1()
        {
            InitializeComponent();

            m_defaultCorporate = Data.GetCorporates();
            m_defaultBanks = Data.GetBanks();
        }


        private void CreateBank_Click(object sender, EventArgs e)
        {
            CreateBank_Click(sender, e, m_defaultSuperAdmin[0]);
        }

        /// <summary>
        /// Create bank.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateBank_Click(object sender, EventArgs e, string user)
        {
            try
            {
                foreach (Bank bank in m_defaultBanks)
                {
                    var ssClient = GetClient();
                    BankCreate create = new BankCreate() { Bank = bank };
                    ssClient.Post(create);

                    try
                    {
                        UserConfig config = new UserConfig
                        {
                            EntityDisplayName = bank.Name,
                            EntityIcon = bank.IconBase64,
                            EntityID = bank.Id,
                            EntityName = bank.Name,
                            UserType = Entity.Bank,
                        };
                        string strippedBankName = bank.Name.ToLower().Replace(" ", string.Empty);

                        CreateUserImpl("john_" + strippedBankName, Privilege.User, m_defaultSuperAdmin[0], config);
                        CreateUserImpl("admin_" + strippedBankName, Privilege.Admin, m_defaultSuperAdmin[0], config);
                    }
                    catch (Exception ex)
                    {
                        AddMessage(string.Format("User Creation failed, unable to create user for bank : {0}. Exception: {1}",
                            bank.Name,
                            ex.Message));
                        return;
                    }

                    AddMessage("Created bank " + bank.Name + " with ID " + bank.Id);
                }
            }
            catch (WebServiceException ex)
            {
                AddMessage("Unable to create bank: " + ex.ErrorMessage);
            }
            catch (Exception ex)
            {
                AddMessage("Unable to create bank: " + ex.Message);
            }
        }


        /// <summary>
        /// Create new account.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateAccount_Click(object sender, EventArgs e, string user)
        {

            CreateAccountImpl(m_defaultAccountName[0], false, user);
        }


        /// <summary>
        /// List banks.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBanks_Click(object sender, EventArgs e)
        {
            try
            {
                var ssClient = GetClient();
                BankList list = new BankList() { Filter = "" };
                List<Bank> banks = ssClient.Get(list);

                AddMessage("Number of banks found: " + banks.Count);
            }
            catch (Exception ex)
            {
                AddMessage("Error: " + ex.Message);
            }
        }

        private void ListAccounts_Click(object sender, EventArgs e)
        {
            var ssClient = GetClient();


            BankList list = new BankList() { Filter = "" };
            List<Bank> banks = ssClient.Get(list);

            int nAccountTotal = 0;

            foreach (Bank bank in banks)
            {

                AccountList accountlist = new AccountList() { BankID = bank.Id };
                List<Account> accounts = ssClient.Get(accountlist);
                nAccountTotal += accounts.Count;
            }

            AddMessage("Number of accounts found: " + nAccountTotal);
        }



        private void DeleteBank_Click(object sender, EventArgs e)
        {
            string bankName = m_defaultBanks[0].Name;
            try
            {
                DeleteBankImpl(bankName);
            }

            catch (WebServiceException ex)
            {
                AddMessage("Unable to delete bank " + bankName + ": " + ex.ErrorMessage);
            }
            catch (Exception ex)
            {
                AddMessage("Unable to delete corporate " + bankName + ": " + ex.Message);
            }

        }

        private void DeleteBankImpl(string bankID)
        {
            var ssClient = GetClient();

            BankDelete delete = new BankDelete();
            delete.BankID = bankID;

            ssClient.Delete(delete);

            AddMessage("Deleted bank with ID " + bankID);

        }

        private void DeleteCorporateImpl(string ID)
        {

            try
            {
                var ssClient = GetClient();
                CorporateDelete delete = new CorporateDelete();
                delete.CorporateID = ID;
                ssClient.Delete(delete);
                AddMessage("Deleted corporate " + ID);
            }
            catch (Exception ex)
            {
                AddMessage("Unable to delete corporate with ID " + ID + ": " + ex.Message);
            }

        }

        private void DeleteAcountImpl(string ID)
        {
            var ssClient = GetClient();
            AccountDelete delete = new AccountDelete();
            delete.ID = ID;
            ssClient.Delete(delete);
            AddMessage("Deleted account " + ID);

        }
        private void DeleteAcountTypeImpl(string ID)
        {
            var ssClient = GetClient();
            AccountTypeDelete delete = new AccountTypeDelete();
            delete.ID = ID;
            ssClient.Delete(delete);
            AddMessage("Deleted account type " + ID);

        }


        private void DeleteDocumentImpl(string ID)
        {
            var ssClient = GetClient();
            DocumentDelete delete = new DocumentDelete();
            delete.ID = ID;
            ssClient.Delete(delete);
            AddMessage("Deleted document " + ID);

        }

        private void DeleteAccount_Click(object sender, EventArgs e)
        {
            /*  try
              {
                  var ssClient = new ServiceStack.JsonServiceClient(m_url);

                  if (null == m_bankID)
                  {
                      BankList list = new BankList() { Filter = "" };
                      List<Bank> banks = ssClient.Get(list);

                      // Get bank ID.
                      m_bankID = banks[0].Id;
                  }

                  AccountList accountlist = new AccountList() {BankID = m_bankID };
                  List<Account> accounts = ssClient.Get(accountlist);

                  if (accounts.Count == 0 )
                  {
                      AddMessage("No account found for bank");
                      return;
                  }

                  // Perform account deletion.
                  AccountDelete delete = new AccountDelete() { ID = accounts[0].Id };
                  ssClient.Delete(delete);

                  AddMessage("Account " + accounts[0].Id + " deleted");

              }
              catch (Exception ex)
              {
                  AddMessage("Error: " + ex.Message);
              }*/
        }

        private void DeleteDocument_Click(object sender, EventArgs e)
        {
            /* try
             {
                 var ssClient = new ServiceStack.JsonServiceClient(m_url);

                 if (null == m_bankID)
                 {
                     BankList list = new BankList() { Filter = "" };
                     List<Bank> banks = ssClient.Get(list);

                     // Get bank ID.
                     m_bankID = banks[0].Id;
                 }

                 AccountList accountlist = new AccountList() { BankID = m_bankID };
                 List<Account> accounts = ssClient.Get(accountlist);

                 if (accounts.Count == 0)
                 {
                     AddMessage("No account found for bank");
                     return;
                 }

                 Account account = accounts[0];
                 m_accountID = account.Id;
                 string doc = null;
                 if ((null == account.Documents) || account.Documents.Count == 0)
                 {
                     Document document = null;
                     // Create document now before deleting it.
                     try
                     {
                         document = new Document();
                         document.Id = ObjectId.GenerateNewId().ToString();
                         document.FormatID = 3;
                         document.Name = "Identification document " + DateTime.Now.Ticks.ToString();
                         document.Accounts = new List<string> { m_accountID };

                         DocumentVersion docVersion = new DocumentVersion();
                         docVersion.Id = ObjectId.GenerateNewId().ToString();
                         docVersion.Creation = DateTime.Now.ToString();
                         docVersion.Content = null;
                         document.Versions = new List<DocumentVersion> { docVersion };

                         // Create document.
                         DocumentCreate create = new DocumentCreate() { File = document, AccountID = m_accountID };
                         ssClient.Post(create);

                         AddMessage("Created new document");
                     }
                     catch (Exception ex)
                     {
                         AddMessage("Error: " + ex.Message);
                     }

                     doc = document.Id;
                     AddMessage("No documents found account " + account.Id);
                     return;
                 }
                 else
                 {
                     doc = account.Documents[0];
                 }

                 // Perform document deletion.
                 DocumentDelete delete = new DocumentDelete() { ID = doc };
                 ssClient.Delete(delete);

                 AddMessage("Document " + doc + " deleted");

             }
             catch (Exception ex)
             {
                 AddMessage("Error: " + ex.Message);
             }*/
        }


        private void DeleteDocumentVersion_Click(object sender, EventArgs e)
        {
            try
            {
                var ssClient = GetClient();
                Document document = GetDocument(ssClient);
                if (null == document)
                {
                    AddMessage("Document not found");
                    return;
                }
                List<DocumentVersion> versions = document.Versions;

                if (versions.Count == 0)
                {
                    AddMessage("No document versions found");
                    return;
                }

                string versionID = versions[0].Id;


                DocumentVersionDelete delete = new DocumentVersionDelete() { DocumentID = document.Id, VersionID = versionID };
                ssClient.Delete(delete);

                AddMessage("Document version deleted");
            }
            catch (Exception ex)
            {
                AddMessage("Error encountered: " + ex.Message);
            }
        }
        private void CreateDocument_Click(object sender, EventArgs e)
        {
            CreateDocumentImpl(m_defaultDocumentNames[0], "");
        }


        private void ListDocuments_Click(object sender, EventArgs e)
        {
            var ssClient = GetClient();

            Account account = GetAccount(ssClient);
            DocumentList doclist = new DocumentList() { AccountID = account.Id };
            List<Document> docs = ssClient.Get(doclist);
            AddMessage("Founded " + docs.Count + " documents");
        }


        #region HELPER_FUNCTIONS

        private ServiceStack.JsonServiceClient GetClient(bool force = false, string user = "", string password = "")
        {
            if (string.IsNullOrEmpty(user))
            {
                user = textBoxUser.Text;
                password = textBoxPassword.Text;
            }
            else
            {
                if (user == "admin")
                    password = "admin";
            }

            m_svc = new ServiceStack.JsonServiceClient(m_url);
            var response = m_svc.Post(new Authenticate
            {
                UserName = user,
                Password = password,
                RememberMe = true, //important tell client to retain permanent cookies

            });


            m_SSESVC = new ServerEventsClient(m_url, new string[] { "channel1", "channel2" });


            m_SSESVC.OnConnect = (msg) =>
            {
            };

            m_SSESVC.OnHeartbeat = () =>
            {

            };
            m_SSESVC.OnCommand = (msg) =>
            {

            };
            m_SSESVC.OnException = (msg) =>
            {

            };
            m_SSESVC.OnMessage = (msg) =>
            {
                try
                {
                    if (null == m_msgs)
                    {
                        m_msgs = new List<ServerEventMessage>();
                    }

                    m_msgs.Add(msg);

                    if (msg.Data.Contains("{"))
                    {
                        string notifyText = msg.Data.Substring(msg.Data.IndexOf("{"));

                        Notification notify = JsonConvert.DeserializeObject<Notification>(notifyText);
                    }
                }
                catch (Exception ex)
                {
                    // TODO: Reeport this.
                }
                // AddMessage("Status: " + notify.StatusUpdate.DocumentUpdates[0].Status + ", bankname: " + notify.BankName + ", corporate " + notify.CorporateName);
            };

            m_SSESVC.Start();


            /* Dictionary<string, ServerEventCallback> handlers = new Dictionary<string, ServerEventCallback>();
            handlers.Add("messages", (client, msg) =>
            {

            });
                m_SSESVC.RegisterHandlers(handlers);*/


            return m_svc;

        }


        private Bank GetBank(ServiceStack.JsonServiceClient client)
        {

            BankList list = new BankList() { Filter = "" };
            List<Bank> banks = client.Get(list);
            Bank bank = null;
            if (banks.Count > 0)
            {

                bank = banks[0];
            }
            return bank;
        }


        private Account GetAccount(ServiceStack.JsonServiceClient client)
        {
            Account account = null;
            Bank bank = GetBank(client);

            AccountList accountlist = new AccountList() { BankID = bank.Id };
            List<Account> accounts = client.Get(accountlist);

            if (accounts.Count > 0)
                account = accounts[0];
            return account;
        }

        private Document GetDocument(ServiceStack.JsonServiceClient client)
        {
            Document document = null;

            Account account = GetAccount(client);
            DocumentList doclist = new DocumentList() { AccountID = account.Id };
            List<Document> docs = client.Get(doclist);

            if (docs.Count > 0)
            {
                document = docs[0];

            }
            else
            {
                document = new Document();
                document.Id = ObjectId.GenerateNewId().ToString();
                document.FormatID = 3;
                document.Name = "Identification document " + DateTime.Now.Ticks.ToString();
                document.Accounts = new List<string> { account.Id };

                DocumentVersion docVersion = new DocumentVersion();
                docVersion.Creation = DateTime.Now.ToString();
                docVersion.Id = ObjectId.GenerateNewId().ToString();
                docVersion.DocumentContentId = null;
                document.Versions = new List<DocumentVersion> { docVersion };

                // Create document.
                DocumentCreate create = new DocumentCreate() { Document = document };
                client.Post(create);


            }
            return document;
        }

        #endregion
        private void btnCreateChat_Click(object sender, EventArgs e)
        {
            CreateChatImpl("NewChat" + System.DateTime.Now.Ticks.ToString());
        }

        private void btnAppendChat_Click(object sender, EventArgs e)
        {
            AppendChatImpl();
        }

        private void btnTerminateChat_Click(object sender, EventArgs e)
        {
            TerminateChatImpl();
        }

        #region HELPER_FUNCTIONS

        private void CreateCorporateImpl(string name, string ID, string icon, string user)
        {

            Corporate corporate = new Corporate();
            corporate.Detail = new CorporateDetail();
            corporate.Detail.Name = name;
            corporate.Icon = icon;
            corporate.Id = ID;
            corporate.Accounts = new List<string>();

            var ssClient = GetClient(true, user, m_password);
            CorporateCreate create = new CorporateCreate() { Corporate = corporate };
            ssClient.Post(create);
            AddMessage("Created Corporate " + name + " with ID " + ID);

            // Now create children. 
            // CreateSubsids(corporate, ssClient, name);

        }

        private void CreateSubsids(Corporate corporate, JsonServiceClient ssClient, string name)
        {
            for (int i = 0; i < 5; i++)
            {
                // Create 5 children.
                Corporate corporateChild = new Corporate();
                corporateChild.Detail = new CorporateDetail();
                corporateChild.Detail.Name = name + "Subsidiary" + i;
                corporateChild.Id = ObjectId.GenerateNewId().ToString();
                corporateChild.Accounts = new List<string>();

                // Set parent.
                corporateChild.ParentID = corporate.Id;
                CorporateCreate createChild = new CorporateCreate() { Corporate = corporateChild };
                ssClient.Post(createChild);

                AddMessage("Created Subsidiary " + corporateChild.Detail.Name + " with ID " + corporateChild.Id);

                for (int j = 0; j < 5; j++)
                {
                    // Create 5 grandchildren.
                    Corporate corporateGrandChild = new Corporate();
                    corporateGrandChild.Detail = new CorporateDetail();
                    corporateGrandChild.Detail.Name = name + "SubSubsidiary" + i + j;
                    corporateGrandChild.Id = ObjectId.GenerateNewId().ToString();
                    corporateGrandChild.Accounts = new List<string>();

                    // Set parent.
                    corporateGrandChild.ParentID = corporateChild.Id;
                    CorporateCreate createGrandChild = new CorporateCreate() { Corporate = corporateGrandChild };
                    ssClient.Post(createGrandChild);

                    AddMessage("Created Subsidiary " + corporateGrandChild.Detail.Name + " with ID " + corporateGrandChild.Id);

                    for (int k = 0; k < 5; k++)
                    {
                        // Create 5 grandchildren.
                        Corporate corporateGreatGrandChild = new Corporate();
                        corporateGreatGrandChild.Detail = new CorporateDetail();
                        corporateGreatGrandChild.Detail.Name = name + "SubSubsidiary" + j + k;
                        corporateGreatGrandChild.Id = ObjectId.GenerateNewId().ToString();
                        corporateGreatGrandChild.Accounts = new List<string>();

                        // Set parent.
                        corporateGreatGrandChild.ParentID = corporateGrandChild.Id;
                        CorporateCreate createGreatGrandChild = new CorporateCreate() { Corporate = corporateGreatGrandChild };
                        ssClient.Post(createGreatGrandChild);

                        AddMessage("Created Subsidiary " + corporateGreatGrandChild.Detail.Name + " with ID " + corporateGreatGrandChild.Id);

                    }

                }
            }
        }

        private Account CreateAccountImpl(string name, bool singleAccount, string user)
        {
            // Get client using this user. For this to succeed, user should be an admin.
            var ssClient = GetClient(true, user, m_password);
            Account ret = null;

            // Set up Bank/corporate if they don't exist.
            BankList list = new BankList();
            IEnumerable<Bank> realBanks = ssClient.Get(list)
                .Where(bank => bank.Id != CommonGlobals.GlobalIdentifier); //don't add account for dummy bank (for global docs)

            CorporatesList corporateslist = new CorporatesList();
            IEnumerable<Corporate> realCorporates = ssClient.Get(corporateslist)
                .Where(corp => corp.Id != CommonGlobals.GlobalIdentifier); //don't add account for dummy corp (for global docs)


            foreach (Bank bank in realBanks)
            {

                foreach (Corporate corporate in realCorporates)
                {
                    // Create account based on Bank we've just listed.
                    try
                    {
                        Account account = new Account();
                        account.Id = ObjectId.GenerateNewId().ToString();
                        account.Name = name + "_" + bank.Name + "_" + corporate.Detail.Name;
                        account.ParentID = bank.Id;
                        account.CorporateId = corporate.Id;
                        account.Status = new StatusEx();
                        account.Status.Status = Status.Pending_e;
                        AccountCreate create = new AccountCreate() { Account = account, BankID = bank.Id };

                        ssClient.Post(create);

                        // While we're at it, add a document to the account.
                        foreach (string doc in m_defaultDocumentNames)
                        {
                            CreateDocumentImpl(doc, account.Id);
                        }

                        ret = account;

                        if (singleAccount)
                            break;

                    }
                    catch (Exception ex)
                    {
                        AddMessage("Unable to create account for bank " + bank.Id + ", corporate " + corporate.Id + ": " + ex.Message);
                    }

                }
            }
            return ret;

        }
        private List<string> GetAccountTypeList(string bankID)
        {
            // now get first bank.
            var ssClient = GetClient();
            AccountTypeListByID list = new AccountTypeListByID();

            list.BankID = bankID;
            return ssClient.Get(list);
        }

        private void DeleteAccountTypeImpl(string name)
        {
            // now get first bank.
            var ssClient = GetClient();
            AccountTypeDelete del = new AccountTypeDelete();

            AccountTypeReadByName request = new AccountTypeReadByName();
            request.Name = name;
            AccountType acctype = ssClient.Get(request);

            AccountTypeDelete delRequest = new AccountTypeDelete();
            delRequest.ID = acctype.Id;
            ssClient.Delete(delRequest);
        }

        private List<UserAuth> GetUserList(string user)
        {
            // now get first bank.
            var ssClient = GetClient(true, user, m_password);
            UsersListGet list = new UsersListGet();
            return ssClient.Get(list);
        }


        private UserConfig GetUserConfig(int userID)
        {
            // now get first bank.
            var ssClient = GetClient();

            UserConfigGet getconfig = new UserConfigGet() { UserID = userID };

            return ssClient.Get(getconfig);
        }


        private List<Document> CreateDocsForAccountTypes(List<string> fileNames)
        {
            var ssClient = GetClient();
            List<Document> docs = new List<Document>();

            foreach (string fileName in fileNames)
            {
                string docPath = Application.StartupPath + "\\documents\\" + fileName;

                Document doc = new Document();
                doc.Name = fileName;
                doc.Id = ObjectId.GenerateNewId().ToString();
                doc.Global = false;
                docs.Add(doc);

                // Read file into byte array.
                FileStream fs = new FileStream(docPath,
                                               FileMode.Open,
                                               FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                long numBytes = new FileInfo(docPath).Length;

                // Create document.
                DocumentCreate doccreate = new DocumentCreate() { Document = doc, Username = "", DocumentContentBase64 = Convert.ToBase64String(br.ReadBytes((int)numBytes)) };
                ssClient.Post(doccreate);


            }
            // We now get the document back based on ID, and we add this to the accounttype.
            return docs;
        }

        private void CreateFieldsIncorp(string corpId)
        {
            CorporateAllFieldDefinitionsUpdate corpFields = new CorporateAllFieldDefinitionsUpdate
            {
                CorporateID = corpId,
                FieldDefinitions = new List<FieldDefinition>
                {
                    //crs fields
                    new FieldDefinition {  Name = "entityName", DefaultValue = "Pfizer",  Id = ObjectId.GenerateNewId().ToString() },
                    new FieldDefinition {  Name = "countryOfIncorporation", DefaultValue = "Ireland",  Id = ObjectId.GenerateNewId().ToString() },
                    new FieldDefinition {  Name = "taxCountry1", DefaultValue = "Ireland",  Id = ObjectId.GenerateNewId().ToString() },
                    new FieldDefinition {  Name = "taxCountry2", DefaultValue = "United Kingdom",  Id = ObjectId.GenerateNewId().ToString() },
                    new FieldDefinition {  Name = "taxCountry3", DefaultValue = "France",  Id = ObjectId.GenerateNewId().ToString() },
                    new FieldDefinition {  Name = "taxId1", DefaultValue = "4587897546a",  Id = ObjectId.GenerateNewId().ToString() },
                    new FieldDefinition {  Name = "taxId2", DefaultValue = "5468475454b",  Id = ObjectId.GenerateNewId().ToString() },
                    new FieldDefinition {  Name = "taxId3", DefaultValue = "15464877f",  Id = ObjectId.GenerateNewId().ToString() },
                    new FieldDefinition() { Name = "Family Name", DefaultValue = "Smith", Id = ObjectId.GenerateNewId().ToString() },
                    new FieldDefinition() { Name = "Country", DefaultValue = "Ireland", Id = ObjectId.GenerateNewId().ToString() },

                    //w9 fields
                     new FieldDefinition {  Name = "name", DefaultValue = "Pfizer",  Id = ObjectId.GenerateNewId().ToString() },
                    new FieldDefinition {  Name = "businessName", DefaultValue = "Pfizer Corp",  Id = ObjectId.GenerateNewId().ToString() },
                    new FieldDefinition {  Name = "isCCorporation", DefaultValue = "true", DataType = eDataType.Bool_e,  Id = ObjectId.GenerateNewId().ToString() },
                    new FieldDefinition {  Name = "exemptPayeeCode", DefaultValue = "IE67",  Id = ObjectId.GenerateNewId().ToString() },
                    new FieldDefinition {  Name = "exemptionCodeFatca", DefaultValue = "5464564",  Id = ObjectId.GenerateNewId().ToString() },
                    new FieldDefinition {  Name = "address", DefaultValue = " Grange Castle Business Park Baldonnel Rd, Kilmahuddrick, Co. Dublin",  Id = ObjectId.GenerateNewId().ToString() },
                    new FieldDefinition {  Name = "cityStateZip", DefaultValue = "Co. Dublin",  Id = ObjectId.GenerateNewId().ToString() },
                    new FieldDefinition {  Name = "accountNumbers", DefaultValue = "425674548, 4587426",  Id = ObjectId.GenerateNewId().ToString() },
                    new FieldDefinition() { Name = "employerIdNo", DefaultValue = "784568", Id = ObjectId.GenerateNewId().ToString() }
                }
            };
            var ssClient = GetClient();
            ssClient.Put(corpFields);
        }

        private AccountType CreateAccTypeInDB(List<Document> docs, string name, string bankId)
        {
            AccountType acctype = new AccountType();
            acctype.Id = ObjectId.GenerateNewId().ToString();
            acctype.Name = name;
            acctype.BankID = bankId;
            // add document ids to account type.
            acctype.BaseDocumentIDs = new List<string>();
            acctype.BaseDocumentNames = new List<string>();
            foreach (Document doc in docs)
            {
                acctype.BaseDocumentIDs.Add(doc.Id);
                acctype.BaseDocumentNames.Add(doc.Name);
            }
            AccountTypeCreate create = new AccountTypeCreate() { Accounttype = acctype };
            var client = GetClient();
            client.Post(create);
            return acctype;
        }

        /// <summary>
        /// Create Account Type.
        /// </summary>
        /// <param name="accountTypeName"></param>
        /// <returns></returns>
        private AccountType CreateAccountTypeImpl(string accountTypeName, List<string> fileNames, string bankId)
        {
            var ssClient = GetClient();
            // We now define field definitions for corporate so they can be resolved when retrieivng the document.
            // Get corporate from Sys. Config.
            SystemConfigGet getsysConfig = new SystemConfigGet() { RoleName = "" };
            SystemConfig config = ssClient.Get(getsysConfig);
            string corporateID = config.CorporateId;

            //1. create fields in corp
            CreateFieldsIncorp(corporateID);

            //2. create documents for account type
            List<Document> docs = CreateDocsForAccountTypes(fileNames);

            //3. Create the account type
            AccountType acctype = CreateAccTypeInDB(docs, accountTypeName, bankId);

            // Now create an account of that Type.
            Account account = new Account();
            account.Id = ObjectId.GenerateNewId().ToString();
            account.Name = accountTypeName + " account";
            account.AccountType = acctype.Id;
            account.ParentID = bankId;

            // Create account.
            AccountCreate accountcreate = new AccountCreate() { Account = account };
            ssClient.Post(accountcreate);

            return acctype;
        }



        private void CreateDocumentImpl(string name, string accountID)
        {
            var ssClient = GetClient();
            BankList list = new BankList();
            List<Bank> banks = ssClient.Get(list);

            CorporatesList corporateslist = new CorporatesList();
            List<Corporate> corporates = ssClient.Get(corporateslist);
            int nDocumentCount = 0;

            if (string.IsNullOrEmpty(accountID))
            {
                foreach (Bank bank in banks)
                {
                    foreach (Corporate corporate in corporates)
                    {
                        AccountList accountList = new AccountList();
                        accountList.BankID = bank.Id;
                        accountList.CorporateID = corporate.Id;

                        List<Account> accounts = ssClient.Get(accountList);

                        foreach (Account account in accounts)
                        {
                            // Create account based on Bank we've just listed.
                            try
                            {
                                Document document = new Document();
                                document.Id = ObjectId.GenerateNewId().ToString();
                                document.FormatID = 3;
                                document.Name = name;
                                document.Accounts = new List<string> { account.Id };

                                DocumentVersion docVersion = new DocumentVersion();
                                docVersion.Creation = DateTime.Now.ToString();
                                docVersion.DocumentContentId = null;
                                document.Versions = new List<DocumentVersion> { docVersion };

                                // Create document.
                                DocumentCreate create = new DocumentCreate() { Document = document };
                                ssClient.Post(create);
                                nDocumentCount++;

                                AddMessage("Created document " + name + " in account " + account.Name);


                            }
                            catch (Exception ex)
                            {
                                AddMessage("Error: " + ex.Message);
                                return;
                            }
                        }

                    }

                }

                AddMessage("Created " + nDocumentCount + " documents");
            }
            else
            {
                try
                {
                    Document document = new Document();
                    document.Id = ObjectId.GenerateNewId().ToString();
                    document.FormatID = 3;
                    document.Name = name;
                    document.Accounts = new List<string> { accountID };

                    DocumentVersion docVersion = new DocumentVersion();
                    docVersion.Creation = DateTime.Now.ToString();
                    docVersion.DocumentContentId = ObjectId.GenerateNewId().ToString();
                    document.Versions = new List<DocumentVersion> { docVersion };

                    // Create document.
                    DocumentCreate create = new DocumentCreate() { Document = document, DocumentContentBase64 = "" };
                    ssClient.Post(create);


                    nDocumentCount++;

                }
                catch (Exception ex)
                {
                    AddMessage("Error: " + ex.Message);
                    return;
                }
            }

        }

        private void CreateChatImpl(string name)
        {
            try
            {
                var ssClient = GetClient();

                Account account = GetAccount(ssClient);

                DocumentList doclist = new DocumentList() { AccountID = account.Id };
                List<Document> docs = ssClient.Get(doclist);

                if (docs.Count == 0)
                {
                    CreateDocumentImpl("TestDocument", account.Id);
                    docs = ssClient.Get(doclist);
                }

                Document doc = docs[0];

                // Create chat.
                DocumentChatCreate create = new DocumentChatCreate()
                {
                    Caller = "CurrentUser" + DateTime.Now.Ticks.ToString(),
                    DocumentID = docs[0].Id
                };

                ssClient.Post(create);
            }
            catch (Exception ex)
            {
                AddMessage("Error: " + ex.Message);
            }
        }

        private Chat GetChat()
        {

            var ssClient = GetClient();

            Account account = GetAccount(ssClient);

            DocumentList doclist = new DocumentList() { AccountID = account.Id };
            List<Document> docs = ssClient.Get(doclist);

            if (docs.Count == 0)
            {
                CreateDocumentImpl("TestDocument", account.Id);
                docs = ssClient.Get(doclist);
            }

            Document doc = docs[0];

            List<Chat> chats = doc.Chats;
            if (0 == chats.Count)
            {
                return null;
            }
            return chats[0];
        }

        private void AppendChatImpl()
        {
            try
            {
                Chat chat = GetChat();
                if (null == chat)
                {
                    CreateChatImpl("Chat" + System.DateTime.Now.Ticks.ToString());
                    chat = GetChat();
                }

                // Create chat.
                DocumentChatAppend append = new DocumentChatAppend();
                append.From = chat.CreatorUserID;
                append.Channel = chat.Id;
                append.DocumentID = chat.DocumentId;
                append.Message = "Testing text";

                var ssClient = GetClient();
                ssClient.Put(append);

            }
            catch (Exception ex)
            {
                AddMessage("Unable to append chat: " + ex.Message);
            }
        }

        private void TerminateChatImpl()
        {
            try
            {
                Chat chat = GetChat();
                if (null == chat)
                {
                    AddMessage("Chat not found");
                    return;
                }

                // Create chat.
                /*   DocumentChatCancelSubscription terminate = new DocumentChatCancelSubscription();
                   terminate.ChatID = chat.Id;
                   terminate.DocumentID = chat.DocumentId;
                   var ssClient = GetClient();
                   ssClient.Put(terminate);*/
            }
            catch (Exception ex)
            {
                AddMessage("Unable to terminate chat: " + ex.Message);
            }
        }
        #endregion

        private void btnAuthenticate_Click(object sender, EventArgs e)
        {
            try
            {
                var ssClient = GetClient();

            }
            catch (Exception ex)
            {

                AddMessage("Unable to authentidate to REST Server: " + ex.Message);

            }

        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string user = textBoxUser.Text;
            string password = textBoxPassword.Text;

            RegisterUserImpl("admin", "admin");
        }

        private void RegisterUserImpl(string user, string password)
        {
            try
            {
                JsonServiceClient svc = new ServiceStack.JsonServiceClient(m_url);
                svc.Post(new Register() { UserName = user, Password = password, DisplayName = textBoxUser.Text });
            }
            catch (Exception ex)
            {
                AddMessage("Unable to register user: " + ex.Message);
            }
        }

        private void CreateCorporate_Click(object sender, EventArgs e, string user)
        {
            foreach (Corporate corporate in m_defaultCorporate)
            {
                string corpName = corporate.Detail.Name;
                // Delete first before creating.
                try
                {
                    DeleteCorporateImpl(corpName);
                }
                catch (WebServiceException ex)
                {
                    AddMessage("Unable to delete corporate " + corpName + ": " + ex.ErrorMessage);
                }
                catch (Exception ex)
                {
                    AddMessage("Unable to delete corporate " + corpName + ": " + ex.Message);
                }

                try
                {
                    var ssClient = GetClient();
                    CorporateCreate create = new CorporateCreate() { Corporate = corporate };
                    ssClient.Post(create);
                    AddMessage("Created Corporate " + corpName + " with ID " + corporate.Id);

                    UserConfig config = new UserConfig
                    {
                        EntityDisplayName = corporate.Detail.Name,
                        EntityIcon = corporate.Icon,
                        EntityID = corporate.Id,
                        EntityName = corporate.Detail.Name,
                        UserType = Entity.Corporate
                    };
                    string strippedCorpName = corporate.Detail.Name.ToLower().Replace(" ", string.Empty);

                    CreateUserImpl("john_" + strippedCorpName, Privilege.User, m_defaultSuperAdmin[0], config);
                    CreateUserImpl("admin_" + strippedCorpName, Privilege.Admin, m_defaultSuperAdmin[0], config);

                    // Now create children. 
                    // CreateSubsids(corporate, ssClient, name);
                }
                catch (WebServiceException ex)
                {
                    AddMessage("Unable to create corporate " + corporate.Id + ": " + ex.ErrorMessage);
                    continue;
                }
                catch (Exception ex)
                {
                    AddMessage("Unable to create corporate " + corporate.Id + ": " + ex.Message);
                    continue;
                }
            }
        }

        private void DeleteCorporate_Click(object sender, EventArgs e)
        {
            foreach (Corporate corporate in m_defaultCorporate)
            {
                try
                {
                    DeleteCorporateImpl(corporate.Detail.Name);
                }
                catch (Exception ex)
                {
                    AddMessage("Unable to delete corporate " + corporate.Detail.Name + ": " + ex.Message);
                    continue;
                }


            }
        }

        private void ListCorporates_Click(object sender, EventArgs e)
        {
            try
            {
                ReportCorporates("");

                foreach (Corporate corporate in m_defaultCorporate)
                {
                    ReportCorporates(corporate.Detail.Name);
                }
            }
            catch (Exception ex)
            {
                AddMessage("Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Report all corporates recursively.
        /// </summary>
        /// <param name="parentID"></param>
        private void ReportCorporates(string parentID)
        {
            // The global corporate doesn't have children.
            if (0 == string.Compare(CommonGlobals.GlobalIdentifier, parentID))
                return;

            var ssClient = GetClient();
            CorporatesList list = new CorporatesList() { Parent = parentID };
            List<Corporate> corporates = ssClient.Get(list);

            if (string.IsNullOrEmpty(parentID))
            {
                AddMessage("Total of  " + corporates.Count + " corporates found ");
            }

            for (int i = 0; i < corporates.Count; i++)
            {
                AddMessage("Parent " + parentID + " Corporate name: " + corporates[i].Detail.Name + ", ID: " + corporates[i].Id);

                ReportCorporates(corporates[i].Id);
            }
        }

        private void btnSetup_Click(object sender, EventArgs e)
        {
            try
            {
                TearDownImpl();
            }
            catch (Exception ex)
            {
                AddMessage("Unable to tear down environment: " + ex.Message);
                return;
            }

            try
            {
                // Start with admin/admin.
                RegisterUserImpl("admin", "admin");
            }
            catch (Exception) { }

            CreateUserImpl(m_defaultSuperAdmin[0], Privilege.SuperAdmin, "admin");


            // Create bank, corporate, account, document, document version.
            // Need to be super admin to create bank.
            CreateBank_Click(null, new EventArgs(), m_defaultSuperAdmin[0]);

            CreateCorporate_Click(null, new EventArgs(), m_defaultSuperAdmin[0]);

            CreateAccount_Click(null, new EventArgs(), m_defaultSuperAdmin[0]);

            CreatGlobalDocs(null, new EventArgs());

            CreateAccountTypes();

            try
            {
                CreateGroupImpl(m_defaultGroups[0], m_defaultSuperAdmin[0]);
            }
            catch (Exception ex)
            {
                AddMessage("Setup failed, unable to create group: " + ex.Message);
                return;
            }

            try
            {
                AddUserToGroupImpl(m_defaultGroups[0], m_defaultUsers[0], m_defaultSuperAdmin[0]);
            }
            catch (Exception ex)
            {
                AddMessage("Setup failed, unable to add user " + m_defaultUsers[0] + " to group: " + m_defaultGroups[0] + ": " + ex.Message);
                return;
            }

            try
            {
                // Create role.
                CreateRoleImpl(m_defaultRoles[0]);
            }
            catch (Exception ex)
            {
                AddMessage("Created role " + m_defaultRoles[0]);
            }

            // For first corporate, create the same account types for each bank.
            CreateAccountTypes();
        }

        private void CreatGlobalDocs(object p, EventArgs eventArgs)
        {
            CreateDocumentImpl("Benifical Owner Doc", CommonGlobals.GlobalIdentifier);
        }

        private void btnTeardown_Click(object sender, EventArgs e)
        {
            try
            {
                TearDownImpl();
            }
            catch (Exception ex)
            {
                AddMessage("Unable to tear down environment: " + ex.Message);
            }
        }


        private void TearDownImpl()
        {
            try
            {
                // Start with admin/admin.
                RegisterUserImpl("admin", "admin");
            }
            catch (Exception) { }

            CreateUserImpl(m_defaultSuperAdmin[0], Privilege.SuperAdmin, "admin");

            var ssClient = GetClient(true, m_defaultSuperAdmin[0], m_password);
            BankList list = new BankList();
            List<Bank> banks = ssClient.Get(list);

            foreach (Bank bank in banks)
            {
                if (bank.Id != CommonGlobals.GlobalIdentifier)
                {
                    List<string> accounttypelist = GetAccountTypeList(bank.Id);

                    // iterate through account types.
                    foreach (string str in accounttypelist)
                    {
                        DeleteAcountTypeImpl(str);
                    }
                    // Delete bank.
                    DeleteBankImpl(bank.Id);
                }

            }

            CorporatesList corporatelist = new CorporatesList();
            List<Corporate> corporates = ssClient.Get(corporatelist);

            foreach (Corporate corporate in corporates)
            {
                if (corporate.Id != CommonGlobals.GlobalIdentifier)
                    DeleteCorporateImpl(corporate.Id);
            }

            AccountList acountlist = new AccountList();
            List<Account> accounts = ssClient.Get(acountlist);

            foreach (Account account in accounts)
            {
                if (account.Id != CommonGlobals.GlobalIdentifier)
                {
                    DeleteAcountImpl(account.Id);

                    DocumentList doclist = new DocumentList();
                    doclist.AccountID = account.Id;
                    List<Document> documents = ssClient.Get(doclist);

                    foreach (Document doc in documents)
                    {
                        if (doc.Id != CommonGlobals.GlobalIdentifier)
                            DeleteDocumentImpl(doc.Id);
                    }
                }
            }


            List<Group> groups = GetGroups(m_defaultSuperAdmin[0]);
            foreach (Group group in groups)
            {
                DeleteGroupImpl(group.Id, m_defaultSuperAdmin[0]);
            }

            List<UserAuth> users = GetUserList(m_defaultSuperAdmin[0]);
            foreach (UserAuth auth in users)
            {
                if (auth.UserName != "admin")
                    DeleteUserImpl(auth.UserName, m_defaultSuperAdmin[0]);
            }

        }

        private void btnCreateDocVersion_Click(object sender, EventArgs e)
        {
            var ssClient = GetClient();
            Account account = null;
            try
            {

                account = GetAccount(ssClient);

            }
            catch (Exception ex)
            {
                AddMessage("Unable to get account:  " + ex.Message);
                return;
            }
            DocumentList doclist = null;
            List<Document> docs = null;

            try
            {
                doclist = new DocumentList() { AccountID = account.Id };
                docs = ssClient.Get(doclist);
            }
            catch (Exception ex)
            {
                AddMessage("Unable to get documents:  " + ex.Message);
                return;
            }
            try
            {
                foreach (Document doc in docs)
                {
                    DocumentUpload upload = new DocumentUpload();
                    upload.DocumentID = doc.Id;

                    // Upload document get notification.
                    ssClient.Post(upload);
                }
            }
            catch (Exception ex)
            {
                AddMessage("Unable to create new document version: " + ex.Message);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var ssClient = GetClient();
            Account account = null;
            try
            {

                account = GetAccount(ssClient);

            }
            catch (Exception ex)
            {
                AddMessage("Unable to get account:  " + ex.Message);
                return;
            }
            DocumentList doclist = null;
            List<Document> docs = null;

            try
            {
                doclist = new DocumentList() { AccountID = account.Id };
                docs = ssClient.Get(doclist);
            }
            catch (Exception ex)
            {
                AddMessage("Unable to get documents:  " + ex.Message);
                return;
            }
            try
            {
                foreach (Document doc in docs)
                {
                    ChangeDocumentStatus changeStatus = new ChangeDocumentStatus();
                    changeStatus.ID = doc.Id;
                    changeStatus.Status = new StatusEx() { Status = Status.Approved_e };

                    // Upload document get notification.
                    ssClient.Put(changeStatus);
                }
            }
            catch (Exception ex)
            {
                AddMessage("Unable to create new document version: " + ex.Message);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnListNotificationsCorporate_Click(object sender, EventArgs e)
        {
            var ssClient = GetClient();
            try
            {
                CorporateNotifications notifications = new CorporateNotifications();
                notifications.CorporateID = "";
                notifications.MaximumCount = 5;
                List<Notification> listnotifications = ssClient.Get(notifications);

            }
            catch (Exception ex)
            {
                AddMessage("Unable to get account:  " + ex.Message);
                return;
            }
        }

        private void btnBankNotifications_Click(object sender, EventArgs e)
        {
            var ssClient = GetClient();
            try
            {
                BankNotifications notifications = new BankNotifications();
                notifications.BankID = "";
                notifications.MaximumCount = 5;
                List<Notification> listnotifications = ssClient.Get(notifications);

            }
            catch (Exception ex)
            {
                AddMessage("Unable to get account:  " + ex.Message);
                return;
            }
        }

        private void AddMessage(string msg)
        {
            ListViewItem item = new ListViewItem(msg);
            listViewMessages.Items.Add(item);

        }

        private void btnCreateGroup_Click(object sender, EventArgs e)
        {
            try
            {
                CreateGroupImpl(m_defaultGroups[0], m_defaultSuperAdmin[0]);
            }
            catch (Exception ex)
            {
                AddMessage("Created group " + m_defaultGroups[0]);
            }

        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            try
            {
                AddUserToGroupImpl(m_defaultGroups[0], m_defaultUsers[0], m_defaultSuperAdmin[0]);
            }
            catch (Exception ex)
            {
                AddMessage("Added user " + m_defaultUsers[0] + " to group " + m_defaultGroups[0]);
            }
        }

        private void btnRemoveUser_Click(object sender, EventArgs e)
        {
            try
            {
                RemoveUserFromGroupImpl(m_defaultGroups[0], m_defaultUsers[0], m_defaultSuperAdmin[0]);
            }
            catch (Exception ex)
            {
                AddMessage("Remove user " + m_defaultUsers[0] + " from group " + m_defaultGroups[0]);
            }
        }

        private void btnAddRoleInfo_Click(object sender, EventArgs e)
        {
            try
            {
                CreateRoleImpl(m_defaultRoles[0]);
            }
            catch (Exception ex)
            {
                AddMessage("Created role " + m_defaultRoles[0]);
            }
        }

        private void btnDeleteRole_Click(object sender, EventArgs e)
        {
            // Delete role.
            try
            {
                DeleteRoleImpl(m_defaultRoles[0]);
            }
            catch (Exception ex)
            {
                AddMessage("Deleted role " + m_defaultRoles[0]);
            }
        }

        /// <summary>
        /// Get groups for role.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetGroupsForRole_Click(object sender, EventArgs e)
        {
            // Create role.
            RoleInfo role = CreateRoleImpl("Role" + DateTime.Now.Ticks);

            Group newGroup = CreateGroupImpl("Group" + DateTime.Now.Ticks, m_defaultSuperAdmin[0]);

            var ssClient = GetClient();

            // Now add this role to a group.
            AddRoleInfoToGroup addRole = new AddRoleInfoToGroup() { Group = newGroup.Id, Role = role.Id };

            try
            {
                ssClient.Put(addRole);
            }
            catch (Exception ex1)
            {
                string msg = "Error: unable to add role to group" + ex1.Message;
                AddMessage(msg);
                throw new Exception(msg);
            }

            GetGroupsForRole getgroups = new GetGroupsForRole();
            getgroups.RoleId = role.Id;

            try
            {
                ssClient.Get(getgroups);
            }
            catch (Exception ex1)
            {
                string msg = "Error: unable to get groups for role: " + ex1.Message;
                AddMessage(msg);
                throw new Exception(msg);
            }

        }

        private void btnTestPermissions_Click(object sender, EventArgs e)
        {
            try
            {
                TestPermissionsImpl();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to test permissions: " + ex.Message);
            }
        }

        private RoleInfo TestPermissionsImpl()
        {
            // Set permission for new account.
            RoleInfo roleinfo = null;

            Permission permission = new Permission();
            permission.Access = new List<Access>() { Access.Delete };

            var ssClient = GetClient();

            // Update permission.
            UpdatePermissionInRoleInfo updatepermission = new UpdatePermissionInRoleInfo();
            updatepermission.Role = roleinfo.Id;
            updatepermission.Permission = permission;

            try
            {
                ssClient.Post(updatepermission);
            }
            catch (Exception ex)
            {
                string msg = "Error: unable to update permission to role: " + ex.Message;
                AddMessage(msg);
                throw new Exception(msg);
            }

            // Now delete the permission.
            DeletePermissionFromRoleInfo deletepermission = new DeletePermissionFromRoleInfo();
            deletepermission.Role = roleinfo.Id;
            deletepermission.PermissionID = permission.Id;

            try
            {
                ssClient.Delete(deletepermission);
            }
            catch (Exception ex)
            {
                string msg = "Error: unable to delete permission to role: " + ex.Message;
                AddMessage(msg);
                throw new Exception(msg);
            }

            return roleinfo;
        }

        #region GROUPS

        /// <summary>
        /// Create group.
        /// </summary>
        /// <param name="name"></param>
        private Group CreateGroupImpl(string name, string user)
        {
            try
            {
                DeleteGroupImpl(name, user);
            }
            catch (Exception) { }

            Group obj = new Group();
            obj.Id = ObjectId.GenerateNewId().ToString();
            var ssClient = GetClient();

            // Create group with no name.
            try
            {
                GroupCreate create = new GroupCreate() { Group = obj };
                ssClient.Post(create);
                AddMessage("Error: group with no name was successfully created");
                throw new Exception("Error: group with no name was successfully created");

            }
            catch (Exception ex)
            {
                // Set name and create again.
                obj = new Group()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Name = name,
                };
                GroupCreate create2 = new GroupCreate() { Group = obj };
                ssClient.Post(create2);
                AddMessage("Group " + name + " successfully created");

                // Now try and create another group of the same name.
                try
                {
                    ssClient.Post(create2);
                    string msg = "Error: duplicate group was successfully created";
                    AddMessage(msg);
                    throw new Exception(msg);
                }
                catch (Exception)
                {
                    // Exception successfully generated.
                }

            }

            return obj;
        }



        /// <summary>
        /// Create group.
        /// </summary>
        /// <param name="name"></param>
        private void DeleteGroupImpl(string name, string user)
        {

            var ssClient = GetClient(true, user, m_password);

            // Create group with no name.
            try
            {
                GroupDelete del = new GroupDelete() { GroupId = name };
                ssClient.Delete(del);
                AddMessage("Group with ID " + name + " successfully deleted");
            }
            catch (Exception ex)
            {
                AddMessage("Group not deleted: " + ex.Message);
            }
        }

        /// <summary>
        /// Delete user.
        /// </summary>
        /// <param name="name"></param>
        private void DeleteUserImpl(string name, string user)
        {

            var ssClient = GetClient(true, user, m_password);

            int nUserID1 = 0;
            UserAuth auth1 = null;
            try
            {
                UserInfoGet getUser1 = new UserInfoGet() { UserName = name };
                auth1 = ssClient.Get(getUser1);
                nUserID1 = auth1.Id;
            }
            catch (Exception ex)
            {
                // Didn't find user.
                AddMessage("User " + name + " not found, not deleted");
                return;
            }

            // Delete user.
            try
            {
                UserInfoDelete del = new UserInfoDelete() { UserID = nUserID1 };
                ssClient.Delete(del);
                AddMessage("User with ID " + nUserID1 + " successfully deleted");

            }
            catch (Exception ex)
            {
                AddMessage("User not deleted: " + ex.Message);
            }

        }


        /// <summary>
        /// Add user to group.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="user"></param>
        private void AddUserToGroupImpl(string group, string user, string authuser)
        {
            var ssClient = GetClient(true, authuser, m_password);

            // Make sure user exists.
            UserInfoGet get = new UserInfoGet() { UserName = user };

            try
            {
                UserAuth auth = ssClient.Get(get);
            }
            catch (Exception)
            {
                // Create user (to ensure success).
                CreateUserImpl(user, Privilege.SuperAdmin, user);
            }

            // Make sure user exists.
            GroupsListGet getGroups = new GroupsListGet() { };

            try
            {
                List<Group> groups = ssClient.Get(getGroups);

                bool bFound = false;
                foreach (Group groupObj in groups)
                {
                    if (0 == groupObj.Name.CompareTo(group))
                    {
                        bFound = true;
                    }
                }

                if (!bFound)
                {
                    // Create group (to ensure success).
                    CreateGroupImpl(group, authuser);
                }
            }
            catch (Exception)
            {

            }

            // Get group from name.
            GroupGetByName getgroup = new GroupGetByName();
            getgroup.GroupName = group;

            Group gp = ssClient.Get(getgroup);
            string groupID = gp.Id;

            // Add blank user.
            try
            {
                AddUserToGroup add = new AddUserToGroup() { Group = groupID, User = "" };
                ssClient.Put(add);
                string msg = "Error: user with no name was successfully added to group";
                AddMessage(msg);
                throw new Exception(msg);

            }
            catch (Exception ex)
            {
                AddUserToGroup add = new AddUserToGroup() { Group = groupID, User = user };
                ssClient.Put(add);

                try
                {
                    ssClient.Put(add);

                    string msg = "Error: duplicate user was added to a group";
                    AddMessage(msg);
                }
                catch (Exception)
                {
                    // Successful test, this should throw an error as a duplicate user.
                }
            }

        }


        /// <summary>
        /// Remove user from group.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="user"></param>
        private void RemoveUserFromGroupImpl(string group, string user, string authuser)
        {
            var ssClient = GetClient(true, authuser, m_password);


            // Make sure user exists.
            UserInfoGet get = new UserInfoGet() { UserName = user };

            try
            {
                UserAuth auth = ssClient.Get(get);
            }
            catch (Exception)
            {
                // Create user (to ensure success).
                CreateUserImpl(user, Privilege.SuperAdmin, user);
            }

            // Make sure user exists.
            GroupsListGet getGroups = new GroupsListGet() { };
            bool bFound = false;
            try
            {
                List<Group> groups = ssClient.Get(getGroups);


                foreach (Group groupObj in groups)
                {
                    if (0 == groupObj.Name.CompareTo(group))
                    {
                        bFound = true;
                    }
                }

                if (!bFound)
                {
                    // Create group (to ensure success).
                    CreateGroupImpl(group, authuser);
                }
            }
            catch (Exception)
            {

            }


            // Make sure user is in group.
            GetUsersForGroup getusers = new GetUsersForGroup();
            getusers.Group = group;

            List<UserAuth> users = ssClient.Get(getusers);
            bFound = false;
            foreach (UserAuth nextuser in users)
            {
                if (nextuser.UserName == user)
                    bFound = true;
            }

            // Add user to group if it's not found.
            if (!bFound)
            {
                AddUserToGroupImpl(group, user, authuser);
            }

            // Create group with no name.
            try
            {
                DeleteUserFromGroup del = new DeleteUserFromGroup() { GroupId = group };
                ssClient.Delete(del);
                string msg = "Error: user with no name was successfully removed from group";
                AddMessage(msg);
                throw new Exception(msg);

            }
            catch (Exception ex)
            {
                DeleteUserFromGroup del = new DeleteUserFromGroup() { GroupId = group };
                ssClient.Delete(del);

                try
                {
                    ssClient.Delete(del);
                    string msg = "Error: non-existent user was removed from a group";
                    AddMessage(msg);
                }
                catch (Exception)
                {
                    // Successful test, this should throw an error as a non-existent user.
                }
            }

        }

        private List<Group> GetGroups(string user)
        {
            GroupsListGet list = new GroupsListGet();
            try
            {
                var ssClient = GetClient(true, user, m_password);
                return ssClient.Get(list);

            }
            catch (Exception)
            {
                // Successful test, this should throw an error as a non-existent user.
            }
            return null;

        }


        /// <summary>
        /// Create new role. Also add role to group and remove it again.
        /// </summary>
        /// <param name="name"></param>
        private RoleInfo CreateRoleImpl(string name)
        {

            RoleInfo obj = new RoleInfo();
            obj.Id = ObjectId.GenerateNewId().ToString();
            var ssClient = GetClient();

            // Create role with no name.
            try
            {
                RoleInfoInsert create = new RoleInfoInsert() { Role = obj };
                ssClient.Post(create);
                AddMessage("Error: Role with no name was successfully created");
                throw new Exception("Error: Role with no name was successfully created");

            }
            catch (Exception ex)
            {
                // Set name and create again.
                obj = new RoleInfo()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Name = name,
                };
                RoleInfoInsert create2 = new RoleInfoInsert() { Role = obj };
                ssClient.Post(create2);
                AddMessage("Role " + name + " successfully created");

                // Now try and create another role of the same name.
                try
                {
                    ssClient.Post(create2);
                    string msg = "Error: duplicate role was successfully created";
                    AddMessage(msg);
                    throw new Exception(msg);
                }
                catch (Exception)
                {
                    // Exception successfully generated.
                }

                // Add permission. We're going to give this role write access to all corporates.
                foreach (Corporate corporate in m_defaultCorporate)
                {
                    Permission permission = new Permission();
                    permission.Id = ObjectId.GenerateNewId().ToString();
                    permission.ResourceType = Resource.Corporate;
                    permission.ResourceId = corporate.Id;
                    permission.Access = new List<Access>() { Access.Write };

                    AddPermissionToRoleInfo addpermission = new AddPermissionToRoleInfo();
                    addpermission.Role = obj.Id;
                    addpermission.Permission = permission;

                    try
                    {
                        ssClient.Put(addpermission);
                    }
                    catch (Exception ex2)
                    {
                        string msg = "Error: unable to add permission to role: " + ex2.Message;
                        AddMessage(msg);
                        throw new Exception(msg);
                    }
                }
                Group newGroup = CreateGroupImpl("Group" + DateTime.Now.Ticks, m_defaultSuperAdmin[0]);

                // Add default user to this new group.  This means that this user will have write access 
                // to all corporates.
                AddUserToGroupImpl(newGroup.Name, m_defaultUsers[0], m_defaultSuperAdmin[0]);

                // Now add this role to a group.
                AddRoleInfoToGroup addRole = new AddRoleInfoToGroup() { Group = newGroup.Id, Role = obj.Id };

                try
                {
                    ssClient.Put(addRole);
                }
                catch (Exception ex1)
                {
                    string msg = "Error: unable to add role to group" + ex1.Message;
                    AddMessage(msg);
                    throw new Exception(msg);
                }
            }
            return obj;
        }

        /// <summary>
        /// Create group.
        /// </summary>
        /// <param name="name"></param>
        private void DeleteRoleImpl(string name)
        {
            var ssClient = GetClient();
            RoleInfo roleinfo = CreateRoleImpl(name);
            RoleInfoDelete deleteRolinfo = null;

            // Delete Role with no name.
            try
            {
                deleteRolinfo = new RoleInfoDelete();
                ssClient.Delete(deleteRolinfo);

                AddMessage("Error: Role with no name was successfully deleted");
                throw new Exception("Error: Role with no name was successfully deleted");
            }
            catch (Exception ex)
            {
                deleteRolinfo.RoleId = roleinfo.Id;
                ssClient.Delete(deleteRolinfo);
                AddMessage("Role " + name + " successfully deleted");

                // Now try and delete another role of the same name.
                try
                {
                    ssClient.Delete(deleteRolinfo);
                    string msg = "Error:  non-existent  role was successfully deleted";
                    AddMessage(msg);
                    throw new Exception(msg);
                }
                catch (Exception)
                {
                    // Exception successfully generated.
                }

            }





        }


        #endregion

        private void btnDeleteGroup_Click(object sender, EventArgs e)
        {
            try
            {
                DeleteGroupImpl(m_defaultGroups[0], m_defaultSuperAdmin[0]);
            }
            catch (Exception ex)
            {
                AddMessage("Created group " + m_defaultGroups[0]);
            }
        }

        private void btnCreateUser_Click(object sender, EventArgs e)
        {
            try
            {
                CreateUserImpl(m_defaultUsers[0], Privilege.User, "admin");
            }
            catch (Exception ex)
            {
                AddMessage("Created group " + m_defaultGroups[0]);
            }
        }

        /// <summary>
        /// Create group.
        /// </summary>
        /// <param name="userNameToCreate"></param>
        private void CreateUserImpl(string userNameToCreate, Privilege priv, string userForClient)
        {
            UserConfig config = new UserConfig();
            // Set corporate detail for User. This is a corporate admin user.
            Corporate corp = m_defaultCorporate[0];

            config.EntityID = corp.Id;
            config.EntityName = corp.Detail.Name;
            config.EntityDisplayName = corp.Detail.Name;
            config.UserType = Entity.Corporate;

            CreateUserImpl(userNameToCreate, priv, userForClient, config);
        }

        /// <summary>
        /// Create group.
        /// </summary>
        /// <param name="userNameToCreate"></param>
        private void CreateUserImpl(string userNameToCreate, Privilege priv, string userForClient, UserConfig config)
        {
            try
            {
                DeleteUserImpl(userNameToCreate, userForClient);
            }
            catch (Exception) { }

            UserAuth obj = new UserAuth();

            var ssClient = GetClient(true, userForClient, m_password);


            config.UserPrivilege = priv;

            try
            {
                UserInfoPost create = new UserInfoPost() { UserToCreate = obj, Config = config, Password = m_password };
                ssClient.Post(create);
                AddMessage("Error: user with no name was successfully created");
                throw new Exception("Error: group with no name was successfully created");

            }
            catch (Exception ex)
            {
                // Set name and create again.
                obj = new UserAuth()
                {
                    UserName = userNameToCreate,
                    FirstName = "John",
                    LastName = "Holmes",
                    DisplayName = userNameToCreate,
                    Email = string.Format("{0}@{1}.com", userNameToCreate, config.EntityDisplayName)
                };
                UserInfoPost create2 = new UserInfoPost() { UserToCreate = obj, Config = config, Password = m_password };
                ssClient.Post(create2);
                AddMessage("User " + userNameToCreate + " successfully created");

                // Now try and create another group of the same name.
                try
                {
                    ssClient.Post(create2);
                    string msg = "Error: duplicate user was successfully created";
                    AddMessage(msg);
                    throw new Exception(msg);
                }
                catch (Exception)
                {
                    // Exception successfully generated.
                }

            }

        }

        #region TEST_SECURITY
        private void btnTestSecurityListAccounts_Click(object sender, EventArgs e)
        {
            try
            {
                TestSecurityListAccounts(true);
            }
            catch (Exception ex)
            {
                AddMessage("Unable to test security for ListAccounts: " + ex.Message);
            }
        }

        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            try
            {
                TestSecurityCreateAccount();
            }
            catch (Exception ex)
            {
                AddMessage("Unable to test security for Creating account: " + ex.Message);
            }
        }

        private void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            try
            {
                TestSecurityDeleteAccount();
            }
            catch (Exception ex)
            {
                AddMessage("Unable to test security for Deleting account: " + ex.Message);
            }
        }

        private void btnTestListDocuments_Click(object sender, EventArgs e)
        {
            try
            {
                TestSecurityListDocuments();
            }
            catch (Exception ex)
            {
                AddMessage("Unable to test security for ListDocuments: " + ex.Message);
            }
        }


        private void btnCreateDocument_Click(object sender, EventArgs e)
        {
            try
            {
                TestSecurityCreateDocument();
            }
            catch (Exception ex)
            {
                AddMessage("Unable to test security for ListDocuments: " + ex.Message);
            }
        }

        private void btnDeleteDocument_Click(object sender, EventArgs e)
        {
            try
            {
                TestSecurityDeleteDocument();
            }
            catch (Exception ex)
            {
                AddMessage("Unable to test security for ListDocuments: " + ex.Message);
            }
        }

        private void btnListDeleteBanks_Click(object sender, EventArgs e)
        {
            try
            {
                TestSecurityListDeleteBanks();
            }
            catch (Exception ex)
            {
                AddMessage("Unable to test security for ListBanks: " + ex.Message);
            }

        }

        private void btnListDeleteCorporates_Click(object sender, EventArgs e)
        {
            try
            {
                TestSecurityListDeleteCorporates();
            }
            catch (Exception ex)
            {
                AddMessage("Unable to test security for ListCorporates: " + ex.Message);
            }
        }

        #endregion


        private void TestSecurityListAccounts(bool bTearDown = true)
        {
            // Set up the following in this basic function:
            // User 1, User 2, group, role.
            string user1name, user2name, groupID, roleID;
            SetupSecurityEnvironment(out user1name, out user2name, out groupID, out roleID);
            string bankName = "Bank1" + DateTime.Now.Ticks;
            string accountName = "Account1" + DateTime.Now.Ticks;
            JsonServiceClient ssClient = null;
            try
            {
                ssClient = GetClient();
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);
                throw new Exception("Unable to get Client: " + ex.Message);
            }
            // Step 5
            // Create Bank
            string bankID = ObjectId.GenerateNewId().ToString();
            try
            {

                Bank bank = new Bank();
                bank.Name = bankName;
                bank.Id = bankID;
                bank.Accounts = new List<string>();

                BankCreate create = new BankCreate() { Bank = bank };
                ssClient.Post(create);
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Unable to create Bank: " + ex.Message);
            }


            // Create permission.
            Permission permission3 = new Permission();
            permission3.Id = ObjectId.GenerateNewId().ToString();
            permission3.ResourceType = Resource.Bank;
            permission3.ResourceId = bankID;
            permission3.Access = new List<Access>() { Access.Write, Access.Read };

            AddPermissionToRoleInfo addpermission2 = new AddPermissionToRoleInfo();
            addpermission2.Role = roleID;
            addpermission2.Permission = permission3;

            try
            {
                ssClient.Put(addpermission2);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to add permission to Role: " + ex.Message);
            }


            // Create Cororate
            string corporateID = ObjectId.GenerateNewId().ToString();
            try
            {

                Corporate corporate = new Corporate();
                corporate.Detail = new CorporateDetail();
                corporate.Detail.Name = "Corporate1";
                corporate.Id = corporateID;
                corporate.Accounts = new List<string>();

                CorporateCreate create = new CorporateCreate() { Corporate = corporate };
                ssClient.Post(create);
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Unable to create Bank: " + ex.Message);
            }

            // Create permission.
            Permission permission4 = new Permission();
            permission4.Id = ObjectId.GenerateNewId().ToString();
            permission4.ResourceType = Resource.Corporate;
            permission4.ResourceId = corporateID;
            permission4.Access = new List<Access>() { Access.Write, Access.Read };

            AddPermissionToRoleInfo addpermission3 = new AddPermissionToRoleInfo();
            addpermission3.Role = roleID;
            addpermission3.Permission = permission3;

            try
            {
                ssClient.Put(addpermission3);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to add permission to Role: " + ex.Message);
            }



            ssClient = GetClient(true, user2name, m_password);

            // Step 5A
            // Create account.
            string accountID = ObjectId.GenerateNewId().ToString();
            try
            {
                Account account = new Account();
                account.Id = accountID;
                account.Name = accountName;
                account.ParentID = bankID;
                account.CorporateId = "";
                account.Status = new StatusEx();
                account.Status.Status = Status.Pending_e;
                AccountCreate create = new AccountCreate() { Account = account, BankID = bankID };
                ssClient.Post(create);
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Unable to create Account: " + ex.Message);
            }

            // Step 6
            // Set permissions for role - (only second user is member of associated group).

            // Create permission.
            Permission permission = new Permission();
            permission.Id = ObjectId.GenerateNewId().ToString();
            permission.ResourceType = Resource.Account;
            permission.ResourceId = accountID;
            permission.Access = new List<Access>() { Access.Write, Access.Read };

            AddPermissionToRoleInfo addpermission = new AddPermissionToRoleInfo();
            addpermission.Role = roleID;
            addpermission.Permission = permission;

            try
            {
                ssClient.Put(addpermission);
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Unable to add permission to Role: " + ex.Message);
            }


            // Step 8
            // Log on as first user - List Accounts for bank.
            // Ensure that you do not see the new account.
            ssClient = GetClient(true, user1name, m_password);
            AccountList accountlist = new AccountList() { BankID = bankID };
            try
            {
                List<Account> accounts = ssClient.Get(accountlist);
                if (accounts.Count > 0)
                {
                    throw new Exception("AccountList returned account when access should not have been granted.");
                }
                AddMessage("Success: no account retrieved as user1");
            }
            catch (Exception ex1)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Error: unable to list accounts: " + ex1.Message);
            }

            // Step 9
            // Log on as second user - List account sfor bank.
            // Ensure that you do have access to the new account.
            ssClient = GetClient(true, user2name, m_password);
            accountlist = new AccountList() { BankID = bankID };
            try
            {
                List<Account> accounts = ssClient.Get(accountlist);
                if (accounts.Count == 0)
                {
                    DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                    throw new Exception("AccountList returned no account when access should have been granted.");
                }
                AddMessage("Success: account retrieved as user2");
            }
            catch (Exception ex1)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Error: unable to list accounts: " + ex1.Message);
            }

            if (bTearDown)
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

        }

        private void TestSecurityCreateAccount()
        {
            // Step 1:
            // Create 2 Users
            string user1name, user2name, groupID, roleID;

            // Set up the following in this basic function:
            // User 1, User 2, group, role.
            SetupSecurityEnvironment(out user1name, out user2name, out groupID, out roleID);
            string bankName = "Bank1" + DateTime.Now.Ticks;
            string accountName = "Account1" + DateTime.Now.Ticks;

            JsonServiceClient ssClient = null;
            try
            {
                ssClient = GetClient();
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Unable to get Client: " + ex.Message);
            }
            // Step 5
            // Create Bank
            string bankID = ObjectId.GenerateNewId().ToString();
            try
            {

                Bank bank = new Bank();
                bank.Name = bankName;
                bank.Id = bankID;
                bank.Accounts = new List<string>();

                BankCreate create = new BankCreate() { Bank = bank };
                ssClient.Post(create);
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Unable to create Bank: " + ex.Message);
            }


            // Create permission.
            Permission permission = new Permission();
            permission.Id = ObjectId.GenerateNewId().ToString();
            permission.ResourceType = Resource.Bank;
            permission.ResourceId = bankID;
            permission.Access = new List<Access>() { Access.Write, Access.Read };

            AddPermissionToRoleInfo addpermission = new AddPermissionToRoleInfo();
            addpermission.Role = roleID;
            addpermission.Permission = permission;

            try
            {

                ssClient.Put(addpermission);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to add permission to Role: " + ex.Message);
            }


            // Log on as first user - Create account.
            // Ensure that you can not create new account, as you do not have write access to the parent bank.
            ssClient = GetClient(true, user1name, m_password);
            try
            {
                Account account = new Account();
                account.Id = ObjectId.GenerateNewId().ToString();
                account.Name = accountName;
                account.ParentID = bankID;
                account.CorporateId = "";
                account.Status = new StatusEx();
                account.Status.Status = Status.Pending_e;
                AccountCreate create = new AccountCreate() { Account = account, BankID = bankID };
                ssClient.Post(create);
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Error: Account was created when access should not have been granted");
            }
            catch (Exception ex)
            {
                // Exception thrown as expected.
                AddMessage("AccountCreate test succeeded, exception thrown with no access");
            }

            // Log on as second user - List account sfor bank.
            // Ensure that you do have access to the new account.
            ssClient = GetClient(true, user2name, m_password);
            try
            {
                Account account = new Account();
                account.Id = ObjectId.GenerateNewId().ToString();
                account.Name = accountName;
                account.ParentID = bankID;
                account.CorporateId = "";
                account.Status = new StatusEx();
                account.Status.Status = Status.Pending_e;
                AccountCreate create = new AccountCreate() { Account = account, BankID = bankID };
                ssClient.Post(create);

                AddMessage("AccountCreate test succeeded, account created with correct access");
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Error: Account was not created when access should have been granted");
                // Exception thrown as expected.
            }
            DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

        }

        private void TestSecurityDeleteAccount()
        {
            // Step 1:
            // Create 2 Users
            string user1name, user2name, groupID, roleID;

            // Set up the following in this basic function:
            // User 1, User 2, group, role.
            SetupSecurityEnvironment(out user1name, out user2name, out groupID, out roleID);
            string bankName = "Bank1" + DateTime.Now.Ticks;
            string accountName = "Account1" + DateTime.Now.Ticks;

            JsonServiceClient ssClient = null;
            try
            {
                ssClient = GetClient();
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Unable to get Client: " + ex.Message);
            }
            // Step 5
            // Create Bank
            string bankID = ObjectId.GenerateNewId().ToString();
            try
            {

                Bank bank = new Bank();
                bank.Name = bankName;
                bank.Id = bankID;
                bank.Accounts = new List<string>();

                BankCreate create = new BankCreate() { Bank = bank };
                ssClient.Post(create);
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Unable to create Bank: " + ex.Message);
            }


            // Create permission.
            Permission permission = new Permission();
            permission.Id = ObjectId.GenerateNewId().ToString();
            permission.ResourceType = Resource.Bank;
            permission.ResourceId = bankID;
            permission.Access = new List<Access>() { Access.Write, Access.Read, Access.Delete };

            AddPermissionToRoleInfo addpermission = new AddPermissionToRoleInfo();
            addpermission.Role = roleID;
            addpermission.Permission = permission;

            try
            {
                ssClient.Put(addpermission);
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Unable to add permission to Role: " + ex.Message);
            }


            // Log on as first user - Create account.
            // Ensure that you can not create new account, as you do not have write access to the parent bank.
            ssClient = GetClient(true, user2name, m_password);
            string accountID = ObjectId.GenerateNewId().ToString(); ;
            try
            {
                Account account = new Account();
                account.Id = accountID;
                account.Name = accountName;
                account.ParentID = bankID;
                account.CorporateId = "";
                account.Status = new StatusEx();
                account.Status.Status = Status.Pending_e;
                AccountCreate create = new AccountCreate() { Account = account, BankID = bankID };
                ssClient.Post(create);

                AddMessage("AccountCreate test succeeded, account created with correct access");
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Error: Account was not created when access should have been granted");
                // Exception thrown as expected.
            }

            // Now make sure first user doesn't have delete access to the account.
            ssClient = GetClient(true, user1name, m_password);


            try
            {

                AccountDelete delete = new AccountDelete() { ID = accountID };
                ssClient.Delete(delete);
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Error: Account was deleted when user should not have had access to delete the account.");
            }
            catch (Exception ex)
            {

                AddMessage("AccountDelete test succeeded, access was not granted");

            }


            // Now make sure second user does have delete access to the account.
            ssClient = GetClient(true, user2name, m_password);
            try
            {
                // Now mak
                AccountDelete delete = new AccountDelete() { ID = accountID };
                ssClient.Delete(delete);

                AddMessage("AccountDelete test succeeded, access was granted");
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Error: Access was not granted to user to delete user. User should have had access by virtue of access to parent bank");
            }
            DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);
        }

        /// <summary>
        /// Test security for getting list of documents.
        /// </summary>
        private void TestSecurityListDocuments()
        {
            // Create 2 Users
            string user1name, user2name, groupID, roleID;

            // Set up the following in this basic function:
            // User 1, User 2, group, role.
            SetupSecurityEnvironment(out user1name, out user2name, out groupID, out roleID);
            string bankName = "Bank1" + DateTime.Now.Ticks;
            string accountName = "Account1" + DateTime.Now.Ticks;
            string documentName1 = "document" + DateTime.Now.Ticks;
            string documentName2 = "document" + DateTime.Now.Ticks;

            JsonServiceClient ssClient = null;
            try
            {
                ssClient = GetClient();
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Unable to get Client: " + ex.Message);
            }


            // Create Bank
            ssClient = GetClient(true, user2name, m_password);
            string bankID = ObjectId.GenerateNewId().ToString();
            try
            {

                Bank bank = new Bank();
                bank.Name = bankName;
                bank.Id = bankID;
                bank.Accounts = new List<string>();

                BankCreate create = new BankCreate() { Bank = bank };
                ssClient.Post(create);
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Unable to create Bank: " + ex.Message);
            }



            // Create permission.
            Permission permission3 = new Permission();
            permission3.Id = ObjectId.GenerateNewId().ToString();
            permission3.ResourceType = Resource.Bank;
            permission3.ResourceId = bankID;
            permission3.Access = new List<Access>() { Access.Write };

            AddPermissionToRoleInfo addpermission3 = new AddPermissionToRoleInfo();
            addpermission3.Role = roleID;
            addpermission3.Permission = permission3;

            try
            {
                ssClient.Put(addpermission3);
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Unable to add permission to Role: " + ex.Message);
            }


            // Step 5A
            // Create account.
            string accountID = ObjectId.GenerateNewId().ToString();
            try
            {
                Account account = new Account();
                account.Id = accountID;
                account.Name = accountName;
                account.ParentID = bankID;
                account.CorporateId = "";
                account.Status = new StatusEx();
                account.Status.Status = Status.Pending_e;
                AccountCreate create = new AccountCreate() { Account = account, BankID = bankID };
                ssClient.Post(create);
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Unable to create Account: " + ex.Message);
            }

            // Create 2 documents.
            // User1 has access to both, User2 has access to 1.
            string documentID1 = ObjectId.GenerateNewId().ToString();
            try
            {
                Document doc = new Document();
                doc.Id = documentID1;
                doc.Accounts = new List<string> { accountID };
                doc.Name = documentName1;
                doc.Status = new StatusEx() { Status = Status.Pending_e };

                // Create document.
                DocumentCreate create = new DocumentCreate() { Document = doc };
                ssClient.Post(create);
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Unable to create Document: " + ex.Message);
            }

            string documentID2 = ObjectId.GenerateNewId().ToString();
            try
            {
                Document doc = new Document();
                doc.Id = documentID2;
                doc.Accounts = new List<string> { accountID };
                doc.Name = documentName2;
                doc.Status = new StatusEx() { Status = Status.Pending_e };

                // Create document.
                DocumentCreate create = new DocumentCreate() { Document = doc };
                ssClient.Post(create);
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Unable to create Account: " + ex.Message);
            }


            // Step 6
            // Set permissions for role - (only second user is member of associated group).

            // Create permission.
            Permission permission = new Permission();
            permission.Id = ObjectId.GenerateNewId().ToString();
            permission.ResourceType = Resource.Document;
            permission.ResourceId = documentID1;
            permission.Access = new List<Access>() { Access.Read };

            AddPermissionToRoleInfo addpermission = new AddPermissionToRoleInfo();
            addpermission.Role = roleID;
            addpermission.Permission = permission;

            try
            {
                ssClient.Put(addpermission);
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Unable to add permission to Role: " + ex.Message);
            }

            // Create permission.
            Permission permission2 = new Permission();
            permission2.Id = ObjectId.GenerateNewId().ToString();
            permission2.ResourceType = Resource.Document;
            permission2.ResourceId = documentID2;
            permission2.Access = new List<Access>() { Access.Read };

            AddPermissionToRoleInfo addpermission2 = new AddPermissionToRoleInfo();
            addpermission2.Role = roleID;
            addpermission2.Permission = permission2;

            try
            {
                ssClient.Put(addpermission2);
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Unable to add permission to Role: " + ex.Message);
            }


            // Step 8
            // Log on as first user - List Accounts for bank.
            // Ensure that you do not see the new account.
            ssClient = GetClient(true, user1name, m_password);
            DocumentList documentlist = new DocumentList() { AccountID = accountID };
            try
            {
                List<Document> documents = ssClient.Get(documentlist);
                if (documents.Count > 0)
                {
                    throw new Exception("DocumentList returned account when access should not have been granted.");
                }
                AddMessage("Success: document was not returned as user1");
            }
            catch (Exception ex1)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Error: unable to list documents: " + ex1.Message);
            }

            // Step 9
            // Log on as second user - List account sfor bank.
            // Ensure that you do have access to the new account.
            ssClient = GetClient(true, user2name, m_password);
            try
            {
                List<Document> documents = ssClient.Get(documentlist);
                if (documents.Count == 0)
                {
                    DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                    throw new Exception("DocumentList returned no document when access should have been granted.");
                }
                AddMessage("Success: document was returned as user2");
            }
            catch (Exception ex1)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Error: unable to list accounts: " + ex1.Message);
            }
        }

        private void TestSecurityCreateDocument()
        {
            // Create 2 Users
            string user1name, user2name, groupID, roleID;

            // Set up the following in this basic function:
            // User 1, User 2, group, role.
            SetupSecurityEnvironment(out user1name, out user2name, out groupID, out roleID);
            string documentName1 = "document" + DateTime.Now.Ticks;
            string documentName2 = "document" + DateTime.Now.Ticks;
            string bankName = "Bank" + DateTime.Now.Ticks;
            string accountName = "Account" + DateTime.Now.Ticks;

            JsonServiceClient ssClient = null;
            try
            {
                ssClient = GetClient();
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Unable to get Client: " + ex.Message);
            }

            // Create Bank
            string bankID = ObjectId.GenerateNewId().ToString();
            try
            {
                Bank bank = new Bank();
                bank.Name = bankName;
                bank.Id = bankID;
                bank.Accounts = new List<string>();
                BankCreate create = new BankCreate() { Bank = bank };
                ssClient.Post(create);
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Unable to create Bank: " + ex.Message);
            }

            // Create permission.
            Permission permission3 = new Permission();
            permission3.Id = ObjectId.GenerateNewId().ToString();
            permission3.ResourceType = Resource.Bank;
            permission3.ResourceId = bankID;
            permission3.Access = new List<Access>() { Access.Write };
            AddPermissionToRoleInfo addpermission3 = new AddPermissionToRoleInfo();
            addpermission3.Role = roleID;
            addpermission3.Permission = permission3;

            try
            {
                ssClient.Put(addpermission3);
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);
                throw new Exception("Unable to add permission to Role: " + ex.Message);
            }

            // Connect as user 2 to create account.
            ssClient = GetClient(true, user2name, m_password);

            // Step 5A
            // Create account.
            string accountID = ObjectId.GenerateNewId().ToString();
            try
            {
                Account account = new Account();
                account.Id = accountID;
                account.Name = accountName;
                account.ParentID = bankID;
                account.CorporateId = "";
                account.Status = new StatusEx();
                account.Status.Status = Status.Pending_e;
                AccountCreate create = new AccountCreate() { Account = account, BankID = bankID };
                ssClient.Post(create);
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Unable to create Account: " + ex.Message);
            }

            // Set permissions for role - (only second user is member of associated group).
            // So only second user has write acess to the account. This means that
            // only second user can create a document in the Account.
            // Create permission.
            Permission permission = new Permission();
            permission.Id = ObjectId.GenerateNewId().ToString();
            permission.ResourceType = Resource.Document;
            permission.ResourceId = accountID;
            permission.Access = new List<Access>() { Access.Read };

            AddPermissionToRoleInfo addpermission = new AddPermissionToRoleInfo();
            addpermission.Role = roleID;
            addpermission.Permission = permission;

            try
            {
                ssClient.Put(addpermission);
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Unable to add permission to Role: " + ex.Message);
            }

            // Log on as user1, we should not have access to create a new document.
            ssClient = GetClient(true, user1name, m_password);
            Document doc = new Document();
            string documentID = ObjectId.GenerateNewId().ToString();
            doc.Id = documentID;
            doc.Accounts = new List<string> { accountID };
            doc.Name = documentName2;
            doc.Status = new StatusEx() { Status = Status.Pending_e };

            try
            {
                // Create document.
                DocumentCreate create = new DocumentCreate() { Document = doc };
                ssClient.Post(create);
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Access was incorrectly granted to create document");
            }
            catch (Exception ex)
            {
                AddMessage("Success: access was denied creating document as user1");
            }

            // Log on as user1, we should not have access to create a new document.
            ssClient = GetClient(true, user2name, m_password);


            try
            {
                // Create document.
                DocumentCreate create = new DocumentCreate() { Document = doc };
                ssClient.Post(create);
                AddMessage("Success: access was denied creating document as user2");
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Access was denied creating document as user2");

            }
            DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

        }

        private void TestSecurityDeleteDocument()
        {
            // Create 2 Users
            string user1name, user2name, groupID, roleID;

            // Set up the following in this basic function:
            // User 1, User 2, group, role.
            SetupSecurityEnvironment(out user1name, out user2name, out groupID, out roleID);
            string documentName1 = "document" + DateTime.Now.Ticks;
            string documentName2 = "document" + DateTime.Now.Ticks;
            string bankName = "Bank" + DateTime.Now;
            string accountName = "Account" + DateTime.Now;

            JsonServiceClient ssClient = null;
            try
            {
                ssClient = GetClient();
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Unable to get Client: " + ex.Message);
            }

            // Create Bank
            string bankID = ObjectId.GenerateNewId().ToString();
            try
            {
                Bank bank = new Bank();
                bank.Name = bankName;
                bank.Id = bankID;
                bank.Accounts = new List<string>();

                BankCreate create = new BankCreate() { Bank = bank };
                ssClient.Post(create);
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Unable to create Bank: " + ex.Message);
            }


            // Create permission.
            Permission permission3 = new Permission();
            permission3.Id = ObjectId.GenerateNewId().ToString();
            permission3.ResourceType = Resource.Bank;
            permission3.ResourceId = bankID;
            permission3.Access = new List<Access>() { Access.Write };

            AddPermissionToRoleInfo addpermission3 = new AddPermissionToRoleInfo();
            addpermission3.Role = roleID;
            addpermission3.Permission = permission3;

            try
            {
                ssClient.Put(addpermission3);
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);
                throw new Exception("Unable to add permission to Role: " + ex.Message);
            }

            // Connect as user2.
            ssClient = GetClient(true, user2name, m_password);

            // Step 5A
            // Create account.
            string accountID = ObjectId.GenerateNewId().ToString();
            try
            {
                Account account = new Account();
                account.Id = accountID;
                account.Name = accountName;
                account.ParentID = bankID;
                account.CorporateId = "";
                account.Status = new StatusEx();
                account.Status.Status = Status.Pending_e;
                AccountCreate create = new AccountCreate() { Account = account, BankID = bankID };
                ssClient.Post(create);
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Unable to create Account: " + ex.Message);
            }

            // Set permissions for role - (only second user is member of associated group).
            // So only second user has write acess to the account. This means that
            // only second user can create a document in the Account.
            // Create permission.
            Permission permission = new Permission();
            permission.Id = ObjectId.GenerateNewId().ToString();
            permission.ResourceType = Resource.Document;
            permission.ResourceId = accountID;
            permission.Access = new List<Access>() { Access.Read };

            AddPermissionToRoleInfo addpermission = new AddPermissionToRoleInfo();
            addpermission.Role = roleID;
            addpermission.Permission = permission;

            try
            {
                ssClient.Put(addpermission);
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Unable to add permission to Role: " + ex.Message);
            }

            // Log on as user2, we should have access to create document.
            ssClient = GetClient(true, user2name, m_password);
            string documentID = ObjectId.GenerateNewId().ToString();
            try
            {
                // Create document.
                Document doc = new Document();

                doc.Id = documentID;
                doc.Accounts = new List<string> { accountID };
                doc.Name = documentName2;
                doc.Status = new StatusEx() { Status = Status.Pending_e };

                DocumentCreate create = new DocumentCreate() { Document = doc };
                ssClient.Post(create);
                AddMessage("Success: access was denied creating document as user2");
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Access was denied creating document as user2");
            }

            // Log on as user1, we should not have access to delete document.
            ssClient = GetClient(true, user1name, "password");

            try
            {

                DocumentDelete delete = new DocumentDelete() { ID = documentID };
                ssClient.Delete(delete);
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Access was incorrectly granted deleting document as user1");
            }
            catch (Exception ex)
            {
                AddMessage("Success: access was denied deleting document as user1");

            }

            // Log on as user2, we should have access to delete document.
            ssClient = GetClient(true, user2name, "password");
            try
            {

                DocumentDelete delete = new DocumentDelete() { ID = documentID };
                ssClient.Delete(delete);
                AddMessage("Success: access was granted deleting document as user2");
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Access was denied deleting document as user2");
            }
            DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

        }

        private void SetupSecurityEnvironment(out string user1name, out string user2name, out string groupID,
            out string roleID)
        {
            user1name = "User1" + DateTime.Now.Second;
            user2name = "User2" + DateTime.Now.Second;
            string groupName = "Group" + DateTime.Now.Ticks;
            string roleName = "Role" + DateTime.Now.Ticks;
            roleID = ObjectId.GenerateNewId().ToString();
            groupID = ObjectId.GenerateNewId().ToString();

            JsonServiceClient ssClient = null;
            try
            {
                ssClient = GetClient();
            }
            catch (Exception)
            {
                UserAuth userAdmin = new UserAuth() { UserName = textBoxUser.Text, Id = DateTime.Now.Second };
                try
                {
                    ssClient = new ServiceStack.JsonServiceClient(m_url);
                    UserInfoPost createUser = new UserInfoPost() { UserToCreate = userAdmin, Password = textBoxPassword.Text };
                    ssClient.Post(createUser);
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to create new user " + user1name);
                }
            }


            UserAuth user1 = new UserAuth() { UserName = user1name, Id = DateTime.Now.Millisecond };

            try
            {
                UserInfoPost createUser = new UserInfoPost() { UserToCreate = user1, Password = "password" };
                ssClient.Post(createUser);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to create new user " + user1name);
            }


            UserAuth user2 = new UserAuth() { UserName = user2name, Id = DateTime.Now.Millisecond };
            try
            {
                UserInfoPost createUser2 = new UserInfoPost() { UserToCreate = user2, Password = "password" };
                ssClient.Post(createUser2);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to create new user " + user2name);
            }


            // Step 2
            // Create Group
            Group group = new Group();
            group.Id = groupID;
            group.Name = groupName;

            // Create group with no name.
            try
            {
                GroupCreate createGroup = new GroupCreate() { Group = group };
                ssClient.Post(createGroup);

            }
            catch (Exception ex)
            {
                throw new Exception("Error: group " + groupName + " not created: " + ex.Message);
            }

            // Step 3
            // Add user to group

            List<int> userids = new List<int>() { user2.Id };
            try
            {
                AddUsersToGroup add = new AddUsersToGroup() { GroupId = group.Id, UserIds = userids };
                ssClient.Put(add);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to add users to group " + groupName + ": " + ex.Message);
            }

            // Step 4 
            // Create role

            RoleInfo role = new RoleInfo();
            role.Id = roleID;
            role.Name = roleName;

            // Create role with no name.
            try
            {
                RoleInfoInsert createRole = new RoleInfoInsert() { Role = role };
                ssClient.Post(createRole);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to create Role: " + ex.Message);
            }

            // Step 7
            // Add role to group.
            // Now add this role to a group.
            AddRoleInfoToGroup addRole = new AddRoleInfoToGroup() { Group = group.Id, Role = roleID };

            try
            {
                ssClient.Put(addRole);
            }
            catch (Exception ex1)
            {
                throw new Exception("Error: unable to add role to group: " + ex1.Message);
            }
        }


        private void DeleteSecurityEnvironment(string user1name, string user2name, string groupID, string roleID)
        {
            JsonServiceClient ssClient = null;
            ssClient = GetClient();

            // Delete users.
            try
            {
                int nUserID1 = 0;
                UserAuth auth1 = null;
                try
                {
                    UserInfoGet getUser1 = new UserInfoGet() { UserName = user1name };
                    auth1 = ssClient.Get(getUser1);
                    nUserID1 = auth1.Id;
                }
                catch (Exception ex)
                {
                    // Didn't find user.

                }


                if (0 != nUserID1)
                {
                    UserInfoDelete deleteUser1 = new UserInfoDelete() { UserID = nUserID1 };
                    ssClient.Delete(deleteUser1);
                }

                int nUserID2 = 0;
                UserAuth auth2 = null;
                try
                {
                    UserInfoGet getUser2 = new UserInfoGet() { UserName = user2name };
                    auth2 = ssClient.Get(getUser2);
                    nUserID1 = auth2.Id;
                }
                catch (Exception ex)
                {
                    // Didn't find user.

                }

                // If user ID has been set, delete user.
                if (0 != nUserID2)
                {
                    UserInfoDelete deleteUser2 = new UserInfoDelete() { UserID = nUserID2 };
                    ssClient.Delete(deleteUser2);
                }

                GroupDelete groupdelete = new GroupDelete() { GroupId = groupID };
                ssClient.Delete(groupdelete);

                RoleInfoDelete roleDelete = new RoleInfoDelete() { RoleId = roleID };
                ssClient.Delete(roleDelete);


            }
            catch (Exception ex)
            {
                throw new Exception("Unable to delete Environment:  " + ex.Message);
            }
        }




        /// <summary>
        /// Test Security for listing and deleting bank.
        /// </summary>
        private void TestSecurityListDeleteBanks()
        {
            // Set up the following in this basic function:
            // User 1, User 2, group, role.
            string user1name, user2name, groupID, roleID;
            SetupSecurityEnvironment(out user1name, out user2name, out groupID, out roleID);
            string bankName = "Bank1";
            JsonServiceClient ssClient = null;
            try
            {
                ssClient = GetClient();
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Unable to get Client: " + ex.Message);
            }

            // Create Bank
            string bankID = ObjectId.GenerateNewId().ToString();
            try
            {

                Bank bank = new Bank();
                bank.Name = bankName;
                bank.Id = bankID;
                bank.Accounts = new List<string>();

                BankCreate create = new BankCreate() { Bank = bank };
                ssClient.Post(create);
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Unable to create Bank: " + ex.Message);
            }

            // Create permission.
            Permission permission = new Permission();
            permission.Id = ObjectId.GenerateNewId().ToString();
            permission.ResourceType = Resource.Bank;
            permission.ResourceId = bankID;
            permission.Access = new List<Access>() { Access.Write, Access.Read };

            AddPermissionToRoleInfo addpermission = new AddPermissionToRoleInfo();
            addpermission.Role = roleID;
            addpermission.Permission = permission;

            try
            {
                ssClient.Put(addpermission);
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Unable to add permission to Role: " + ex.Message);
            }


            // Log on as first user - List Banks.
            // Ensure that you do not see the new bank when listing banks.
            ssClient = GetClient(true, user1name, "password");
            BankList banklist = new BankList() { };
            try
            {
                List<Bank> banks = ssClient.Get(banklist);
                bool bFound = false;
                foreach (Bank bank in banks)
                {
                    if (bank.Id == bankID)
                    {
                        bFound = true;
                    }
                }

                if (bFound)
                {
                    throw new Exception("BankList returned bank when access should not have been granted.");
                }
                AddMessage("Success: access to bank not granted as user1");
            }
            catch (Exception ex1)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Error: unable to list banks: " + ex1.Message);
            }

            // Delete bank as user1.
            BankDelete bankDelete = new BankDelete() { BankID = bankID };
            try
            {
                ssClient.Delete(bankDelete);
                throw new Exception("Error: Access not granted to delete bank as user");
            }
            catch (Exception)
            {
                AddMessage("Success: bank not deleted as user1");
            }


            // Log on as second user - delete bank as user2 (should have access).
            // Ensure that you do have access to the new account.
            ssClient = GetClient(true, user2name, "password");
            try
            {
                ssClient.Delete(bankDelete);
                AddMessage("Success: bank deleted as user2");
            }
            catch (Exception)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Error: Access not granted to delete bank as user");

            }
            DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

        }




        private void TestSecurityListDeleteCorporates()
        {
            // Set up the following in this basic function:
            // User 1, User 2, group, role.
            string user1name, user2name, groupID, roleID;
            SetupSecurityEnvironment(out user1name, out user2name, out groupID, out roleID);

            JsonServiceClient ssClient = null;
            try
            {
                ssClient = GetClient();
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Unable to get Client: " + ex.Message);
            }

            // Create Corporate
            string corpID = ObjectId.GenerateNewId().ToString();
            try
            {

                Corporate corp = new Corporate();
                corp.Detail = new CorporateDetail() { Name = "Corporate" };
                corp.Id = corpID;
                corp.Accounts = new List<string>();

                CorporateCreate create = new CorporateCreate() { Corporate = corp };
                ssClient.Post(create);
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Unable to create Corporate: " + ex.Message);
            }

            // Create permission.
            Permission permission = new Permission();
            permission.Id = ObjectId.GenerateNewId().ToString();
            permission.ResourceType = Resource.Bank;
            permission.ResourceId = corpID;
            permission.Access = new List<Access>() { Access.Write, Access.Read };

            AddPermissionToRoleInfo addpermission = new AddPermissionToRoleInfo();
            addpermission.Role = roleID;
            addpermission.Permission = permission;

            try
            {
                ssClient.Put(addpermission);
            }
            catch (Exception ex)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Unable to add permission to Role: " + ex.Message);
            }


            // Log on as first user - List Corporates.
            // Ensure that you do not see the new corporate when listing corporates.
            ssClient = GetClient(true, user1name, "password");
            CorporatesList corporatelist = new CorporatesList() { };
            try
            {
                List<Corporate> corporates = ssClient.Get(corporatelist);
                if (corporates.Count > 1)
                {
                    // Note the "1" is due to Global Corporate.
                    throw new Exception("CorporatesList returned corporate when access should not have been granted.");
                }
                AddMessage("Success: access to corporate not granted as user1");
            }
            catch (Exception ex1)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Error: unable to list corporates: " + ex1.Message);
            }

            // Delete corporate as user1.
            CorporateDelete corporateDelete = new CorporateDelete() { CorporateID = corpID };
            try
            {
                ssClient.Delete(corporateDelete);
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Error: Access not granted to delete corporate as user");
            }
            catch (Exception)
            {
                AddMessage("Success: corporate not deleted as user1");
            }


            // Log on as second user - delete corporate as user2 (should have access).
            // Ensure that you do have access to the new account.
            ssClient = GetClient(true, user2name, "password");
            try
            {
                ssClient.Delete(corporateDelete);
                AddMessage("Success: corporate deleted as user2");
            }
            catch (Exception)
            {
                DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

                throw new Exception("Error: Access not granted to delete corporate as user2");

            }
            DeleteSecurityEnvironment(user1name, user2name, groupID, roleID);

        }


        private void btnDeleteAccountType_Click(object sender, EventArgs e)
        {
            try
            {
                DeleteAccountTypeImpl("AccountType");
            }
            catch (Exception ex)
            {
                AddMessage("Unable to delete Account Type: " + ex.Message);
            }
        }


        private void btnCreateAccountType_Click_1(object sender, EventArgs e)
        {
            CreateAccountTypes();
        }

        private void CreateAccountTypes()
        {
            try
            {
                foreach (Bank bank in m_defaultBanks)
                {
                    if (Data.DocumentsForAccountTypes.ContainsKey(bank.Name))
                    {
                        var filenames = Data.DocumentsForAccountTypes[bank.Name];
                        CreateAccountTypeImpl(bank.Name + " Current", filenames, bank.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                AddMessage("Unable to create Account Type: " + ex.Message);
            }
        }

        private void btnUpdateCorpDefinitions_Click(object sender, EventArgs e)
        {
            JsonServiceClient ssClient = null;
            try
            {
                ssClient = GetClient();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to get Client: " + ex.Message);
            }


            string corporateId = m_defaultCorporate[1].Id;

            List<FieldDefinition> defs = new List<FieldDefinition>()
            {
                new FieldDefinition(){
                    DataType = eDataType.String_e,
                    DefaultValue = "Ushers, Dublin 8",
                    Name = "LegalAddress",
                    Id = "dummyId123"
                },
                new FieldDefinition(){
                    DataType = eDataType.String_e,
                    DefaultValue = "Corp Name",
                    Name = "LegalName",
                    Id = "dummyId123"
                }
            };

            CorporateAllFieldDefinitionsUpdate request = new CorporateAllFieldDefinitionsUpdate()
            {
                CorporateID = corporateId,
                FieldDefinitions = defs
            };

            try
            {
                ssClient.Put(request);
                throw new Exception("Error: Could not update definitions for corporate: " + corporateId);
            }
            catch (Exception)
            {
                AddMessage("Success: Corporate defintions updated for corporate id :" + corporateId);
            }

        }

    }
}
