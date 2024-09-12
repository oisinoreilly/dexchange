/* Options:
Date: 2018-01-03 21:44:47
Version: 4.54
Tip: To override a DTO option, remove "//" prefix before updating
BaseUrl: http://localhost:5001

//GlobalNamespace: 
//MakePropertiesOptional: True
//AddServiceStackTypes: True
//AddResponseStatus: False
//AddImplicitVersion: 
//AddDescriptionAsComments: True
//IncludeTypes: 
//ExcludeTypes: 
//DefaultImports: 
*/


export interface IReturnVoid {
}

export interface IReturn<T> {
}

export class Group {
    Name: string;
    Id: string;
    UserAuthIds: number[];
    Roles: string[];
    Subgroups: string[];
}

export type Resource = "Bank" | "Corporate" | "Subsid" | "Account" | "AccountType" | "Document" | "Field";
export type Access = "Read" | "Write" | "Delete";
export type Entity = "Bank" | "Corporate";
export type Privilege = "User" | "Admin" | "SuperAdmin";


export class Permission {
    Id: string;
    ResourceType: Resource;
    ResourceId: string;
    Access: Access[];
}

export type Status = "Pending_e" | "Approved_e" | "Rejected_e" | "InformationRequired_e" | "InheritFromChild_e" | "NotSet_e";

export class StatusEx {
    Status: Status;
    Comment: string;
}

export type eDataType = "String_e" | "Bool_e" | "Number_e" | "StringArray_e";

export class FieldDefinition {
    Id: string;
    Name: string;
    DataType: eDataType;
    MinimumLength: number;
    DefaultValue: string;
}

export class DocumentVersion {
    Id: string;
    DocumentContentId: string;
    Creation: string;
}

export class ChatMessage {
    Id: number;
    Channel: string;
    FromUserId: string;
    Message: string;
    Private: boolean;
    Time: string;
}

export class Chat {
    Id: string;
    DocumentId: string;
    StartTime: string;
    EndTime: string;
    CreatorUserID: string;
    ChatMessages: ChatMessage[];
}

export class FieldDefinitionDocumentDetail {
    VerticalOffset: number;
    HorizontalOffset: number;
}

export class FieldDefinitionforDocument {
    FieldID: string;
    Value: string;
    DocumentDetail: FieldDefinitionDocumentDetail;
}

export class Document {
    Id: string;
    Name: string;
    FormatID: number;
    Status: StatusEx;
    Versions: DocumentVersion[];
    Accounts: string[];
    Chats: Chat[];
    Global: boolean;
    Fields: FieldDefinitionforDocument[];
}

export class AccountType {
    Id: string;
    Name: string;
    BankID: string;
    BaseDocument: Document[];
    Definitions: FieldDefinition[];
}

export class Bank {
    Name: string;
    Id: string;
    IconBase64: string;
    Accounts: string[];
    Notifications: string[];
    Status: StatusEx;
    FieldDefinitions: FieldDefinition[];
    AccountTypes: AccountType[];
}

export type NotificationType = "StatusCreateAccount_e" | "StatusChangeAccount_e" | "StatusCreateDocument_e" | "StatusChangeDocument_e" | "Message_e";

export class StatusCouple {
    ID: string;
    Status: Status;
}

export class StatusUpdate {
    DocumentUpdates: StatusCouple[];
    AccountUpdates: StatusCouple[];
    CorporateUpdates: StatusCouple[];
    BankUpdates: StatusCouple[];
}

export class Notification {
    Id: string;
    Type: NotificationType;
    StatusUpdate: StatusUpdate;
    DocumentID: string;
    AccountID: string;
    BankName: string;
    BankID: string;
    CorporateName: string;
    UserName: string;
    CreationDate: string;
    Message: string;
    Processed: boolean;
}

export class GenericFieldDefinitions {
    Fields: FieldDefinition[];
}

export class AccountDetail extends GenericFieldDefinitions {
    Type: string;
    Description: string;
    Overdraft: string;
    CompanyName: string;
    Address: string;
    CompanyContact: string;
    Email: string;
    Phone: string;
    Directors: string[];
    Signatories: string[];
}

export class Signatory {
    Name: string;
    Role: string;
}

export class CorporateDetail {
    Name: string;
    TradingName: string;
    DateOfIncorporation: string;
    PhoneNumber: string;
    EmailAddress: string;
    CorrespondanceAddress: string;
    StockExchangeDetail: string;
    Turnover: string;
    TypeOfEntity: string;
    NatureOfBusiness: string;
    Signatories: Signatory[];
    Fields: FieldDefinition[];
}

// @DataContract
export class ResponseError {
    // @DataMember(Order=1, EmitDefaultValue=false)
    ErrorCode: string;

    // @DataMember(Order=2, EmitDefaultValue=false)
    FieldName: string;

    // @DataMember(Order=3, EmitDefaultValue=false)
    Message: string;

    // @DataMember(Order=4, EmitDefaultValue=false)
    Meta: { [index: string]: string; };
}

// @DataContract
export class ResponseStatus {
    // @DataMember(Order=1)
    ErrorCode: string;

    // @DataMember(Order=2)
    Message: string;

    // @DataMember(Order=3)
    StackTrace: string;

    // @DataMember(Order=4)
    Errors: ResponseError[];

    // @DataMember(Order=5)
    Meta: { [index: string]: string; };
}

export class RequestLogEntry {
    Id: number;
    DateTime: string;
    HttpMethod: string;
    AbsoluteUri: string;
    PathInfo: string;
    RequestBody: string;
    RequestDto: Object;
    UserAuthId: string;
    SessionId: string;
    IpAddress: string;
    ForwardedFor: string;
    Referer: string;
    Headers: { [index: string]: string; };
    FormData: { [index: string]: string; };
    Items: { [index: string]: string; };
    Session: Object;
    ResponseDto: Object;
    ErrorResponse: Object;
    RequestDuration: string;
}

export class UserAuth {
    Id: number;
    UserName: string;
    Email: string;
    PrimaryEmail: string;
    PhoneNumber: string;
    FirstName: string;
    LastName: string;
    DisplayName: string;
    Company: string;
    BirthDate: string;
    BirthDateRaw: string;
    Address: string;
    Address2: string;
    City: string;
    State: string;
    Country: string;
    Culture: string;
    FullName: string;
    Gender: string;
    Language: string;
    MailAddress: string;
    Nickname: string;
    PostalCode: string;
    TimeZone: string;
    Salt: string;
    PasswordHash: string;
    DigestHa1Hash: string;
    Roles: string[];
    Permissions: string[];
    CreatedDate: string;
    ModifiedDate: string;
    InvalidLoginAttempts: number;
    LastLoginAttempt: string;
    LockedDate: string;
    RecoveryToken: string;
    RefId: number;
    RefIdStr: string;
    Meta: { [index: string]: string; };
}

export class Account {
    Id: string;
    ParentID: string;
    CorporateId: string;
    Name: string;
    Creation: string;
    Detail: AccountDetail;
    Status: StatusEx;
    Documents: string[];
    AccountType: string;
}

export class DocumentContent {
    Id: string;
    ContentBase64: string;
    FieldDefinitions: FieldDefinitionforDocument[];
}

export class Corporate {
    Id: string;
    Icon: string;
    Detail: CorporateDetail;
    Accounts: string[];
    Notifications: string[];
    Status: StatusEx;
    Children: string[];
    ParentID: string;
    DisplayID: string;
    Fields: FieldDefinition[];
}

export class GetChatHistoryResponse {
    Results: ChatMessage[];
    ResponseStatus: ResponseStatus;
}

export class RoleInfo {
    Name: string;
    Id: string;
    Permissions: Permission[];
}

export class SystemConfig {
    BankId: string;
    BankDisplayName: string;
    BankIcon: string;
    CorporateId: string;
    CorporateName: string;
    CorporateIcon: string;
}

export class UserConfig {
    Id: string;
    UserId: number;
    UserType: Entity;
    UserPrivilege: Privilege;
    EntityDisplayName: string;
    EntityIcon: string;
    EntityID: string;
    EntityName: string;
}


// @DataContract
export class RequestLogsResponse {
    // @DataMember(Order=1)
    Results: RequestLogEntry[];

    // @DataMember(Order=2)
    Usage: { [index: string]: string; };

    // @DataMember(Order=3)
    ResponseStatus: ResponseStatus;
}

// @DataContract
export class RegisterResponse {
    // @DataMember(Order=1)
    UserId: string;

    // @DataMember(Order=2)
    SessionId: string;

    // @DataMember(Order=3)
    UserName: string;

    // @DataMember(Order=4)
    ReferrerUrl: string;

    // @DataMember(Order=5)
    ResponseStatus: ResponseStatus;

    // @DataMember(Order=6)
    Meta: { [index: string]: string; };
}

// @DataContract
export class AuthenticateResponse {
    // @DataMember(Order=1)
    UserId: string;

    // @DataMember(Order=2)
    SessionId: string;

    // @DataMember(Order=3)
    UserName: string;

    // @DataMember(Order=4)
    DisplayName: string;

    // @DataMember(Order=5)
    ReferrerUrl: string;

    // @DataMember(Order=6)
    BearerToken: string;

    // @DataMember(Order=7)
    ResponseStatus: ResponseStatus;

    // @DataMember(Order=8)
    Meta: { [index: string]: string; };
}

// @DataContract
export class AssignRolesResponse {
    // @DataMember(Order=1)
    AllRoles: string[];

    // @DataMember(Order=2)
    AllPermissions: string[];

    // @DataMember(Order=3)
    ResponseStatus: ResponseStatus;
}

// @DataContract
export class UnAssignRolesResponse {
    // @DataMember(Order=1)
    AllRoles: string[];

    // @DataMember(Order=2)
    AllPermissions: string[];

    // @DataMember(Order=3)
    ResponseStatus: ResponseStatus;
}

// @DataContract
export class ConvertSessionToTokenResponse {
    // @DataMember(Order=1)
    Meta: { [index: string]: string; };

    // @DataMember(Order=2)
    ResponseStatus: ResponseStatus;
}

// @Route("/api/v1/users", "GET")
export class UsersListGet implements IReturn<Array<UserAuth>>
{
    createResponse() { return new Array<UserAuth>(); }
    getTypeName() { return "UsersListGet"; }
}

// @Route("/api/v1/user", "GET")
export class UserInfoGet implements IReturn<UserAuth>
{
    UserName: string;
    createResponse() { return new UserAuth(); }
    getTypeName() { return "UserInfoGet"; }
}

// @Route("/api/v1/user", "POST")
export class UserInfoPost implements IReturnVoid {
    UserToCreate: UserAuth;
    Password: string;
    Config: UserConfig;
    createResponse() { }
    getTypeName() { return "UserInfoPost"; }
}

// @Route("/api/v1/user", "PUT")
export class UserInfoPut implements IReturnVoid {
    UserToUpdate: UserAuth;
    createResponse() { }
    getTypeName() { return "UserInfoPut"; }
}

// @Route("/api/v1/user", "DELETE")
export class UserInfoDelete implements IReturnVoid {
    UserToDelete: string;
    createResponse() { }
    getTypeName() { return "UserInfoDelete"; }
}

// @Route("/api/v1/groups", "GET")
export class GroupsListGet implements IReturn<Array<Group>>
{
    Groups: Group[];
    createResponse() { return new Array<Group>(); }
    getTypeName() { return "GroupsListGet"; }
}

// @Route("/api/v1/groups", "POST")
export class GroupCreate implements IReturnVoid {
    Group: Group;
    createResponse() { }
    getTypeName() { return "GroupCreate"; }
}

// @Route("/api/v1/groups", "DELETE")
export class GroupDelete implements IReturnVoid {
    GroupId: string;
    createResponse() { }
    getTypeName() { return "GroupDelete"; }
}

// @Route("/api/v1/group/user", "PUT")
export class AddUserToGroup implements IReturnVoid {
    Group: string;
    User: string;
    createResponse() { }
    getTypeName() { return "AddUserToGroup"; }
}

// @Route("/api/v1/group/users", "PUT")
export class AddUsersToGroup implements IReturnVoid {
    GroupId: string;
    UserIds: number[];
    createResponse() { }
    getTypeName() { return "AddUsersToGroup"; }
}

// @Route("/api/v1/group/user", "DELETE")
export class DeleteUserFromGroup implements IReturnVoid {
    GroupId: string;
    UserId: number;
    createResponse() { }
    getTypeName() { return "DeleteUserFromGroup"; }
}

// @Route("/api/v1/group/role", "PUT")
export class AddRoleInfoToGroup implements IReturnVoid {
    Group: string;
    Role: string;
    createResponse() { }
    getTypeName() { return "AddRoleInfoToGroup"; }
}

// @Route("/api/v1/group/role", "PUT")
export class AddRolesToGroup implements IReturnVoid {
    GroupId: string;
    RoleIds: string[];
    createResponse() { }
    getTypeName() { return "AddRolesToGroup"; }
}

// @Route("/api/v1/group/role", "DELETE")
export class RemoveRoleInfoFromGroup implements IReturnVoid {
    GroupId: string;
    RoleId: string;
    createResponse() { }
    getTypeName() { return "RemoveRoleInfoFromGroup"; }
}

// @Route("/api/v1/group/users", "GET")
export class GetUsersForGroup implements IReturn<Array<UserAuth>>
{
    Group: string;
    createResponse() { return new Array<UserAuth>(); }
    getTypeName() { return "GetUsersForGroup"; }
}

// @Route("/api/v1/group/roles", "GET")
export class GetRolesForGroup implements IReturn<Array<RoleInfo>>
{
    Group: string;
    createResponse() { return new Array<RoleInfo>(); }
    getTypeName() { return "GetRolesForGroup"; }
}

// @Route("/api/v1/role/groups", "GET")
export class GetGroupsForRole implements IReturn<Array<Group>>
{
    RoleId: string;
    createResponse() { return new Array<Group>(); }
    getTypeName() { return "GetGroupsForRole"; }
}

// @Route("/api/v1/role/permission", "PUT")
export class AddPermissionToRoleInfo implements IReturnVoid {
    Role: string;
    Permission: Permission;
    createResponse() { }
    getTypeName() { return "AddPermissionToRoleInfo"; }
}

// @Route("/api/v1/role/permission", "POST")
export class UpdatePermissionInRoleInfo implements IReturnVoid {
    Role: string;
    Permission: Permission;
    createResponse() { }
    getTypeName() { return "UpdatePermissionInRoleInfo"; }
}

// @Route("/api/v1/role/permission", "DELETE")
export class DeletePermissionFromRoleInfo implements IReturnVoid {
    Role: string;
    PermissionID: string;
    createResponse() { }
    getTypeName() { return "DeletePermissionFromRoleInfo"; }
}

// @Route("/api/v1/banks", "GET")
export class BankList implements IReturn<Array<Bank>>
{
    Filter: string;
    createResponse() { return new Array<Bank>(); }
    getTypeName() { return "BankList"; }
}

// @Route("/api/v1/banks", "POST")
export class BankCreate implements IReturnVoid {
    Bank: Bank;
    createResponse() { }
    getTypeName() { return "BankCreate"; }
}

// @Route("/api/v1/banks", "DELETE")
export class BankDelete implements IReturnVoid {
    BankID: string;
    createResponse() { }
    getTypeName() { return "BankDelete"; }
}

// @Route("/api/v1/banknotifications", "GET")
export class BankNotifications implements IReturn<Array<Notification>>
{
    BankID: string;
    MaximumCount: number;
    createResponse() { return new Array<Notification>(); }
    getTypeName() { return "BankNotifications"; }
}

// @Route("/api/v1/corporates", "GET")
export class CorporatesList implements IReturn<Array<Corporate>>
{
    Parent: string;
    createResponse() { return new Array<Corporate>(); }
    getTypeName() { return "CorporatesList"; }
}

// @Route("/api/v1/corporate/fielddefinitions", "GET")
export class CorporateFieldDefinitionList implements IReturn<Array<FieldDefinition>>
{
    CorporateID: string;
    createResponse() { return new Array<FieldDefinition>(); }
    getTypeName() { return "CorporateFieldDefinitionList"; }
}

// @Route("/api/v1/corporate/fielddefinition/id", "GET")
export class CorporateFieldDefinitionGetByID implements IReturn<FieldDefinition>
{
    FieldID: string;
    createResponse() { return new FieldDefinition(); }
    getTypeName() { return "CorporateFieldDefinitionGetByID"; }
}

// @Route("/api/v1/corporate/fielddefinition/name", "GET")
export class CorporateFieldDefinitionGetByName implements IReturn<FieldDefinition>
{
    FieldName: string;
    createResponse() { return new FieldDefinition(); }
    getTypeName() { return "CorporateFieldDefinitionGetByName"; }
}

// @Route("/api/v1/corporate/fielddefinition", "POST")
export class CorporateFieldDefinitionAdd implements IReturnVoid {
    CorporateID: string;
    Definition: FieldDefinition;
    createResponse() { }
    getTypeName() { return "CorporateFieldDefinitionAdd"; }
}

// @Route("/api/v1/corporate/fielddefinition", "PUT")
export class CorporateFieldDefinitionUpdate implements IReturnVoid {
    CorporateID: string;
    FieldDefinitionID: string;
    Definition: FieldDefinition;
    createResponse() { }
    getTypeName() { return "CorporateFieldDefinitionUpdate"; }
}

// @Route("/api/v1/corporate/fielddefinition/all", "PUT")
export class CorporateAllFieldDefinitionsUpdate implements IReturnVoid {
    CorporateID: string;
    FieldDefinitions: FieldDefinition[];
    createResponse() { }
    getTypeName() { return "CorporateAllFieldDefinitionsUpdate"; }
}

// @Route("/api/v1/corporate/fielddefinition", "DELETE")
export class CorporateFieldDefinitionRemove implements IReturnVoid {
    CorporateID: string;
    FieldDefinitionID: string;
    createResponse() { }
    getTypeName() { return "CorporateFieldDefinitionRemove"; }
}

// @Route("/api/v1/accounts", "GET")
export class AccountList implements IReturn<Array<Account>>
{
    BankID: string;
    CorporateID: string;
    createResponse() { return new Array<Account>(); }
    getTypeName() { return "AccountList"; }
}

// @Route("/api/v1/accounts/all", "GET")
export class AccountListAll implements IReturn<Array<Account>>
{
    createResponse() { return new Array<Account>(); }
    getTypeName() { return "AccountListAll"; }
}

// @Route("/api/v1/accounts", "POST")
export class AccountCreate implements IReturnVoid {
    Account: Account;
    BankID: string;
    Username: string;
    PrefillDocuments: boolean;
    createResponse() { }
    getTypeName() { return "AccountCreate"; }
}

// @Route("/api/v1/account", "GET")
export class AccountRead implements IReturn<Account>
{
    ID: string;
    createResponse() { return new Account(); }
    getTypeName() { return "AccountRead"; }
}

// @Route("/api/v1/account", "PUT")
export class AccountUpdate implements IReturnVoid {
    AccountID: string;
    Account: Account;
    Username: string;
    createResponse() { }
    getTypeName() { return "AccountUpdate"; }
}

// @Route("/api/v1/accountstatus", "PUT")
export class ChangeAccountStatus implements IReturnVoid {
    AccountID: string;
    Status: StatusEx;
    Username: string;
    createResponse() { }
    getTypeName() { return "ChangeAccountStatus"; }
}

// @Route("/api/v1/account/documentupload", "PUT")
export class AccountFilledDocumentUpload implements IReturnVoid {
    AccountID: string;
    DocumentID: string;
    DocumentContent: DocumentContent;
    createResponse() { }
    getTypeName() { return "AccountFilledDocumentUpload"; }
}

// @Route("/api/v1/sign/envelope", "POST")
export class EnvelopeCreate implements IReturn<string>
{
    DocumentId: string;
    createResponse() { return ""; }
    getTypeName() { return "EnvelopeCreate"; }
}

// @Route("/api/v1/sign/document", "POST")
export class DocumentUploadSigned implements IReturn<string>
{
    DocumentId: string;
    EnvelopeId: string;
    createResponse() { return ""; }
    getTypeName() { return "DocumentUploadSigned"; }
}

// @Route("/api/v1/account", "DELETE")
export class AccountDelete implements IReturnVoid {
    ID: string;
    createResponse() { }
    getTypeName() { return "AccountDelete"; }
}

// @Route("/api/v1/documents", "GET")
export class DocumentList implements IReturn<Array<Document>>
{
    AccountID: string;
    createResponse() { return new Array<Document>(); }
    getTypeName() { return "DocumentList"; }
}

// @Route("/api/v1/documents", "POST")
export class DocumentCreate implements IReturnVoid {
    Document: Document;
    Username: string;
    DocumentContentBase64: string;
    createResponse() { }
    getTypeName() { return "DocumentCreate"; }
}

// @Route("/api/v1/documentUpload", "POST")
export class DocumentUpload implements IReturnVoid {
    DocumentContentBase64: string;
    DocumentID: string;
    Username: string;
    createResponse() { }
    getTypeName() { return "DocumentUpload"; }
}

// @Route("/api/v1/document", "GET")
export class DocumentRead implements IReturnVoid {
    ID: string;
    createResponse() { }
    getTypeName() { return "DocumentRead"; }
}

// @Route("/api/v1/documentcontent", "GET")
export class DocumentContentRead implements IReturn<DocumentContent>
{
    ID: string;
    createResponse() { return new DocumentContent(); }
    getTypeName() { return "DocumentContentRead"; }
}

// @Route("/api/v1/documents", "DELETE")
export class DocumentDelete implements IReturnVoid {
    ID: string;
    createResponse() { }
    getTypeName() { return "DocumentDelete"; }
}

// @Route("/api/v1/documentstatus", "PUT")
export class ChangeDocumentStatus implements IReturnVoid {
    ID: string;
    Status: StatusEx;
    Username: string;
    createResponse() { }
    getTypeName() { return "ChangeDocumentStatus"; }
}

// @Route("/api/v1/document/fielddefinition", "POST")
export class DocumentFieldDefinitionSet implements IReturnVoid {
    CorporateID: string;
    DocumentID: string;
    FieldDefinition: FieldDefinitionforDocument;
    createResponse() { }
    getTypeName() { return "DocumentFieldDefinitionSet"; }
}

// @Route("/api/v1/document/fielddefinition", "PUT")
export class DocumentFieldDefinitionRemove implements IReturnVoid {
    CorporateID: string;
    DocumentID: string;
    FieldDefinitionID: string;
    createResponse() { }
    getTypeName() { return "DocumentFieldDefinitionRemove"; }
}

// @Route("/api/v1/documentversions", "DELETE")
export class DocumentVersionDelete implements IReturnVoid {
    DocumentID: string;
    VersionID: string;
    createResponse() { }
    getTypeName() { return "DocumentVersionDelete"; }
}

// @Route("/api/v1/documentlink", "PUT")
export class LinkDocumentToAccount implements IReturnVoid {
    DocumentID: string;
    AccountID: string;
    createResponse() { }
    getTypeName() { return "LinkDocumentToAccount"; }
}

// @Route("/api/v1/accountlink", "PUT")
export class LinkAccountToBank implements IReturnVoid {
    AccountID: string;
    BankID: string;
    createResponse() { }
    getTypeName() { return "LinkAccountToBank"; }
}

// @Route("/api/v1/corporate", "GET")
export class CorporateRead implements IReturn<Corporate>
{
    CorporateID: string;
    createResponse() { return new Corporate(); }
    getTypeName() { return "CorporateRead"; }
}

// @Route("/api/v1/corporates", "POST")
export class CorporateCreate implements IReturnVoid {
    Corporate: Corporate;
    createResponse() { }
    getTypeName() { return "CorporateCreate"; }
}

// @Route("/api/v1/corporates", "DELETE")
export class CorporateDelete implements IReturnVoid {
    CorporateID: string;
    createResponse() { }
    getTypeName() { return "CorporateDelete"; }
}

// @Route("/api/v1/corporatenotifications", "GET")
export class CorporateNotifications implements IReturn<Array<Notification>>
{
    CorporateID: string;
    MaximumCount: number;
    createResponse() { return new Array<Notification>(); }
    getTypeName() { return "CorporateNotifications"; }
}

// @Route("/api/v1/corporatenotifications", "PUT")
export class CorporateNotification implements IReturnVoid {
    CorporateID: string;
    Notification: Notification;
    createResponse() { }
    getTypeName() { return "CorporateNotification"; }
}

// @Route("/api/v1/accounttypes", "GET")
export class AccountTypeReadAll implements IReturn<Array<AccountType>>
{
    BankID: string;
    createResponse() { return new Array<AccountType>(); }
    getTypeName() { return "AccountTypeReadAll"; }
}

// @Route("/api/v1/accounttypes/list", "GET")
export class AccountTypeList implements IReturn<Array<string>>
{
    BankID: string;
    createResponse() { return new Array<string>(); }
    getTypeName() { return "AccountTypeList"; }
}

// @Route("/api/v1/accounttypes", "POST")
export class AccountTypeCreate implements IReturnVoid {
    Accounttype: AccountType;
    createResponse() { }
    getTypeName() { return "AccountTypeCreate"; }
}

// @Route("/api/v1/accounttype", "GET")
export class AccountTypeRead implements IReturn<AccountType>
{
    ID: string;
    createResponse() { return new AccountType(); }
    getTypeName() { return "AccountTypeRead"; }
}

// @Route("/api/v1/accounttype/documents", "GET")
export class AccountTypeDocumentContents implements IReturn<DocumentContent>
{
    AccountTypeID: string;
    DocumentID: string;
    createResponse() { return new DocumentContent(); }
    getTypeName() { return "AccountTypeDocumentContents"; }
}

// @Route("/api/v1/accounttype", "DELETE")
export class AccountTypeDelete implements IReturnVoid {
    ID: string;
    createResponse() { }
    getTypeName() { return "AccountTypeDelete"; }
}

// @Route("/api/v1/documentchats", "POST")
export class DocumentChatCreate implements IReturnVoid {
    Caller: string;
    DocumentID: string;
    createResponse() { }
    getTypeName() { return "DocumentChatCreate"; }
}

// @Route("/api/v1/documentchats", "PUT")
export class DocumentChatAppend implements IReturnVoid {
    Channel: string;
    From: string;
    ToUserId: string;
    DocumentID: string;
    Message: string;
    Selector: string;
    createResponse() { }
    getTypeName() { return "DocumentChatAppend"; }
}

// @Route("/api/v1/documentchats", "PUT")
export class DocumentChatCancelSubscription implements IReturnVoid {
    SubscriptionID: string;
    ChannelID: string;
    DocumentID: string;
    createResponse() { }
    getTypeName() { return "DocumentChatCancelSubscription"; }
}

// @Route("/api/v1/chathistory", "GET")
// @Route("/chathistory")
export class GetChatHistory implements IReturn<GetChatHistoryResponse>
{
    Channels: string[];
    AfterId: number;
    Take: number;
    createResponse() { return new GetChatHistoryResponse(); }
    getTypeName() { return "GetChatHistory"; }
}

// @Route("/api/v1/roles", "GET")
export class RoleListGet implements IReturn<Array<RoleInfo>>
{
    createResponse() { return new Array<RoleInfo>(); }
    getTypeName() { return "RoleListGet"; }
}

// @Route("/api/v1/role", "GET")
export class RoleInfoGet implements IReturn<RoleInfo>
{
    RoleName: string;
    createResponse() { return new RoleInfo(); }
    getTypeName() { return "RoleInfoGet"; }
}

// @Route("/api/v1/userRoles", "GET")
export class UserRoleInfoGet implements IReturn<Array<RoleInfo>>
{
    Username: string;
    createResponse() { return new Array<RoleInfo>(); }
    getTypeName() { return "UserRoleInfoGet"; }
}

// @Route("/api/v1/systemConfige", "GET")
export class SystemConfigGet implements IReturn<SystemConfig>
{
    RoleName: string;
    createResponse() { return new SystemConfig(); }
    getTypeName() { return "SystemConfigGet"; }
}

// @Route("/api/v1/user/info", "GET")
export class UserConfigGet implements IReturn<UserConfig>
{
    UserID: number;
    createResponse() { return new UserConfig(); }
    getTypeName() { return "UserConfigGet"; }
}

// @Route("/api/v1/role", "POST")
export class RoleInfoInsert implements IReturnVoid {
    Role: RoleInfo;
    createResponse() { }
    getTypeName() { return "RoleInfoInsert"; }
}

// @Route("/api/v1/role", "PUT")
export class RoleInfoUpdate implements IReturnVoid {
    Role: RoleInfo;
    createResponse() { }
    getTypeName() { return "RoleInfoUpdate"; }
}

// @Route("/api/v1/role", "DELETE")
export class RoleInfoDelete implements IReturnVoid {
    RoleId: string;
    createResponse() { }
    getTypeName() { return "RoleInfoDelete"; }
}

// @Route("/requestlogs")
// @DataContract
export class RequestLogs implements IReturn<RequestLogsResponse>
{
    // @DataMember(Order=1)
    BeforeSecs: number;

    // @DataMember(Order=2)
    AfterSecs: number;

    // @DataMember(Order=3)
    IpAddress: string;

    // @DataMember(Order=4)
    ForwardedFor: string;

    // @DataMember(Order=5)
    UserAuthId: string;

    // @DataMember(Order=6)
    SessionId: string;

    // @DataMember(Order=7)
    Referer: string;

    // @DataMember(Order=8)
    PathInfo: string;

    // @DataMember(Order=9)
    Ids: number[];

    // @DataMember(Order=10)
    BeforeId: number;

    // @DataMember(Order=11)
    AfterId: number;

    // @DataMember(Order=12)
    HasResponse: boolean;

    // @DataMember(Order=13)
    WithErrors: boolean;

    // @DataMember(Order=14)
    Skip: number;

    // @DataMember(Order=15)
    Take: number;

    // @DataMember(Order=16)
    EnableSessionTracking: boolean;

    // @DataMember(Order=17)
    EnableResponseTracking: boolean;

    // @DataMember(Order=18)
    EnableErrorTracking: boolean;

    // @DataMember(Order=19)
    DurationLongerThan: string;

    // @DataMember(Order=20)
    DurationLessThan: string;
    createResponse() { return new RequestLogsResponse(); }
    getTypeName() { return "RequestLogs"; }
}

// @Route("/register")
// @DataContract
export class Register implements IReturn<RegisterResponse>
{
    // @DataMember(Order=1)
    UserName: string;

    // @DataMember(Order=2)
    FirstName: string;

    // @DataMember(Order=3)
    LastName: string;

    // @DataMember(Order=4)
    DisplayName: string;

    // @DataMember(Order=5)
    Email: string;

    // @DataMember(Order=6)
    Password: string;

    // @DataMember(Order=7)
    AutoLogin: boolean;

    // @DataMember(Order=8)
    Continue: string;
    createResponse() { return new RegisterResponse(); }
    getTypeName() { return "Register"; }
}

// @Route("/auth")
// @Route("/auth/{provider}")
// @Route("/authenticate")
// @Route("/authenticate/{provider}")
// @DataContract
export class Authenticate implements IReturn<AuthenticateResponse>
{
    // @DataMember(Order=1)
    provider: string;

    // @DataMember(Order=2)
    State: string;

    // @DataMember(Order=3)
    oauth_token: string;

    // @DataMember(Order=4)
    oauth_verifier: string;

    // @DataMember(Order=5)
    UserName: string;

    // @DataMember(Order=6)
    Password: string;

    // @DataMember(Order=7)
    RememberMe: boolean;

    // @DataMember(Order=8)
    Continue: string;

    // @DataMember(Order=9)
    nonce: string;

    // @DataMember(Order=10)
    uri: string;

    // @DataMember(Order=11)
    response: string;

    // @DataMember(Order=12)
    qop: string;

    // @DataMember(Order=13)
    nc: string;

    // @DataMember(Order=14)
    cnonce: string;

    // @DataMember(Order=15)
    UseTokenCookie: boolean;

    // @DataMember(Order=16)
    Meta: { [index: string]: string; };
    createResponse() { return new AuthenticateResponse(); }
    getTypeName() { return "Authenticate"; }
}

// @Route("/assignroles")
// @DataContract
export class AssignRoles implements IReturn<AssignRolesResponse>
{
    // @DataMember(Order=1)
    UserName: string;

    // @DataMember(Order=2)
    Permissions: string[];

    // @DataMember(Order=3)
    Roles: string[];
    createResponse() { return new AssignRolesResponse(); }
    getTypeName() { return "AssignRoles"; }
}

// @Route("/unassignroles")
// @DataContract
export class UnAssignRoles implements IReturn<UnAssignRolesResponse>
{
    // @DataMember(Order=1)
    UserName: string;

    // @DataMember(Order=2)
    Permissions: string[];

    // @DataMember(Order=3)
    Roles: string[];
    createResponse() { return new UnAssignRolesResponse(); }
    getTypeName() { return "UnAssignRoles"; }
}

// @Route("/session-to-token")
// @DataContract
export class ConvertSessionToToken implements IReturn<ConvertSessionToTokenResponse>
{
    // @DataMember(Order=1)
    PreserveSession: boolean;
    createResponse() { return new ConvertSessionToTokenResponse(); }
    getTypeName() { return "ConvertSessionToToken"; }
}