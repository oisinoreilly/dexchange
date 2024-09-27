
using DataModels;
using DataModels.DTO.V1;
using Models.DTO.V1;
using ServiceStack.Auth;
using System;
using System.Collections.Generic;

namespace Core.Contracts
{
    public enum ObjectType_e
    {
        Document_e,
        Account_e,
        Bank_e,
        Corporate_e
    }

    public interface IDocumentHQRepository
    {
        // Bank methods.
        List<Bank> GetBanks(string filter);
        Bank GetBank(string ID);
        void CreateBank(Bank bank);
        void DeleteBank(string bankID);
        void UpdateBankNotifications(string bankID, Notification notification);
        List<Notification> GetNotificationsForBank(string bankID, int maximumCount);

        // Corporate methods.
        List<Corporate> GetCorporates(string filter);
        Corporate GetCorporate(string ID);
        void CreateCorporate(Corporate corporate);
        void DeleteCorporate(string corporateID, bool removeFromParent);
        void UpdateCorporateNotifications(string corporateID, Notification notification);
        List<Notification> GetNotificationsForCorporate(string corporateID, int maximumCount);

        List<FieldDefinition> GetCorporateFieldDefinitions(string corporateID);
        FieldDefinition GetCorporateFieldDefinitionByID(string fieldID);
        FieldDefinition GetCorporateFieldDefinitionByName(string fieldName);
        void CorporateFieldDefinitionAdd(string corporateID, FieldDefinition definition);
        void CorporateFieldDefinitionUpdate(string corporateID, FieldDefinition definition);
        void CorporateFieldDefinitionRemove(string corporateID, string fieldName);
        void UpdateCorporateFieldList(string corporateID, List<FieldDefinition> fields);

        // Account type methods.
        List<AccountType> GetAccountTypes(string bankID);
        List<string> GetAccountTypeLists(string bankID);
        List<string> GetAccountTypeIDs(string bankID);
        
        void CreateAccountType(AccountType accounttype);
        void UpdateAccountType(AccountType accounttype);
        AccountType GetAccountType(string ID);
        AccountType GetAccountTypeByName(string Name);
                
        void DeleteAccountType(string ID);
        DocumentContent GetAccountTypeDocumentContents(string AccountTypeID, string DocumentID, bool PreFillFields);

        // Account methods.
        List<Account> GetAccounts(string bankID, string corporateID);
        void CreateAccount(Account account, bool prefillDocuments);
        Account GetAccount(string accountID);
        List<Account> GetAllAccounts();
        void UpdateAccount(string accountID, Account account);
        void DeleteAccount(string accountID);
        Notification ChangeAccountStatus(string ID, StatusEx status);
        void FilledDocumentUpload(string accountID, string documentName, DocumentContent content);
       
        // Document methods.
        List<Document> GetDocuments(string accountID);
        Notification CreateDocument(Document document, string accountID, string DocumentContentBase64);
        Document GetDocument(string documentID);
        void UpdateDocument(string documentID, string name, StatusEx status);
        Notification UploadDocument(string documentID, string DocumentContentBase64);

       // void CreateDocumentVersion(string documentID, string versionID);
        void DeleteDocument(string documentID);
        void DeleteDocumentVersion(string documentID, string versionID);
        Notification ChangeDocumentStatus(string username, string ID, Status status);
        void GetBankAndCorporateForDocument(string docID, out string BankID, out string CorporateID);
        DocumentContent GetDocumentContents(string documentContentID);

        void DocumentFieldDefinitionSet(string corporateID, string documentID, FieldDefinition fielddefinition);
        void DocumentFieldDefinitionRemove(string corporateID, string documentID, string fielddefinitionID);

        string GetBankIDFromDocumentID(string id);
        string GetCorporateIDFromDocumentID(string id);
        string GetBankNameFromID(string id);
        string GetCorporateNameFromID(string id);

        // Link methods.
        void LinkDocumentToAccount(string documentID, string accountID);
        void LinkAccountToBank(string accountID, string bankID);
        void LinkAccountToCorporate(string accountID, string corporateID);

        // Chat methods.
        string CreateChat(string caller, string documentID);
        void AppendToChat(DocumentChatAppend request);
        void TerminateChat(string chatID, string documentID);

        //Role Methods
        List<RoleInfo> GetRolesList(RoleListGet request);
        RoleInfo GetRoleByName(string roleName);
        RoleInfo GetRoleByID(string roleID);
        List<RoleInfo> GetUserRoles(UserRoleInfoGet request);
        RoleInfo InsertRole(RoleInfoInsert request);
        void UpdateRole(RoleInfoUpdate request);
        void DeleteRole(RoleInfoDelete request);
        List<Group> GetGroupsForRole(GetGroupsForRole request);

        SystemConfig GetSystemConfig(SystemConfigGet request);

        //Users Methods
        void ValidateCreateUser(UserAuth UserToCreate, UserConfig config);
        List<UserAuth> GetUsersList();
        IUserAuth GetUserByName(string username);
        UserConfig GetUserConfig(int userID);
        void CreateUser(UserAuth userToCreate, string password, UserConfig config);
        void UpdateUser(UserAuth user, UserConfig detail);
        void DeleteUser(string userName);

        // Group methods.
        List<Group> GetGroupsList();
        Group GetGroupByName(string groupName);
        Group GetGroupByID(string groupID);
        void CreateGroup(Group group);
        void DeleteGroup(string groupId);
        void AddUserToGroup(string group, string user);
        void AddUsersToGroup(string groupId, List<int> userIds);
        void DeleteUserFromGroup(string groupId, int userId);
        List<UserAuth> GetUsersForGroup(string group);

        // Group/role methods.
        void AddRoleInfoToGroup(string role, string group);
        void AddRolesToGroup(List<string> roleIds, string groupId);
        void RemoveRoleInfoFromGroup(string roleId, string groupId);
        List<RoleInfo> GetRolesForGroup(string group);

        // AppPermission methods.
        void AddPermissionToRoleInfo(string role, Permission permission);
        void UpdatePermissionInRoleInfo(string role, Permission permission);
        void DeletePermissionFromRoleInfo(string role, string permission);
       
    }
}
