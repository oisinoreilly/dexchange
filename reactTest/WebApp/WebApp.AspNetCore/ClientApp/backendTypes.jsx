/* Options:
Date: 2018-01-03 21:44:47
Version: 4.54
Tip: To override a DTO option, remove "//" prefix before updating
BaseUrl: http://ec2-63-32-159-120.eu-west-1.compute.amazonaws.com:5001

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
export class Group {
}
export class Permission {
}
export class StatusEx {
}
export class FieldDefinition {
}
export class DocumentVersion {
}
export class ChatMessage {
}
export class Chat {
}
export class FieldDefinitionDocumentDetail {
}
export class FieldDefinitionforDocument {
}
export class Document {
}
export class AccountType {
}
export class Bank {
}
export class StatusCouple {
}
export class StatusUpdate {
}
export class Notification {
}
export class GenericFieldDefinitions {
}
export class AccountDetail extends GenericFieldDefinitions {
}
export class Signatory {
}
export class CorporateDetail {
}
// @DataContract
export class ResponseError {
}
// @DataContract
export class ResponseStatus {
}
export class RequestLogEntry {
}
export class UserAuth {
}
export class Account {
}
export class DocumentContent {
}
export class Corporate {
}
export class GetChatHistoryResponse {
}
export class RoleInfo {
}
export class SystemConfig {
}
export class UserConfig {
}
// @DataContract
export class RequestLogsResponse {
}
// @DataContract
export class RegisterResponse {
}
// @DataContract
export class AuthenticateResponse {
}
// @DataContract
export class AssignRolesResponse {
}
// @DataContract
export class UnAssignRolesResponse {
}
// @DataContract
export class ConvertSessionToTokenResponse {
}
// @Route("/api/v1/users", "GET")
export class UsersListGet {
    createResponse() { return new Array(); }
    getTypeName() { return "UsersListGet"; }
}
// @Route("/api/v1/user", "GET")
export class UserInfoGet {
    createResponse() { return new UserAuth(); }
    getTypeName() { return "UserInfoGet"; }
}
// @Route("/api/v1/user", "POST")
export class UserInfoPost {
    createResponse() { }
    getTypeName() { return "UserInfoPost"; }
}
// @Route("/api/v1/user", "PUT")
export class UserInfoPut {
    createResponse() { }
    getTypeName() { return "UserInfoPut"; }
}
// @Route("/api/v1/user", "DELETE")
export class UserInfoDelete {
    createResponse() { }
    getTypeName() { return "UserInfoDelete"; }
}
// @Route("/api/v1/groups", "GET")
export class GroupsListGet {
    createResponse() { return new Array(); }
    getTypeName() { return "GroupsListGet"; }
}
// @Route("/api/v1/groups", "POST")
export class GroupCreate {
    createResponse() { }
    getTypeName() { return "GroupCreate"; }
}
// @Route("/api/v1/groups", "DELETE")
export class GroupDelete {
    createResponse() { }
    getTypeName() { return "GroupDelete"; }
}
// @Route("/api/v1/group/user", "PUT")
export class AddUserToGroup {
    createResponse() { }
    getTypeName() { return "AddUserToGroup"; }
}
// @Route("/api/v1/group/users", "PUT")
export class AddUsersToGroup {
    createResponse() { }
    getTypeName() { return "AddUsersToGroup"; }
}
// @Route("/api/v1/group/user", "DELETE")
export class DeleteUserFromGroup {
    createResponse() { }
    getTypeName() { return "DeleteUserFromGroup"; }
}
// @Route("/api/v1/group/role", "PUT")
export class AddRoleInfoToGroup {
    createResponse() { }
    getTypeName() { return "AddRoleInfoToGroup"; }
}
// @Route("/api/v1/group/role", "PUT")
export class AddRolesToGroup {
    createResponse() { }
    getTypeName() { return "AddRolesToGroup"; }
}
// @Route("/api/v1/group/role", "DELETE")
export class RemoveRoleInfoFromGroup {
    createResponse() { }
    getTypeName() { return "RemoveRoleInfoFromGroup"; }
}
// @Route("/api/v1/group/users", "GET")
export class GetUsersForGroup {
    createResponse() { return new Array(); }
    getTypeName() { return "GetUsersForGroup"; }
}
// @Route("/api/v1/group/roles", "GET")
export class GetRolesForGroup {
    createResponse() { return new Array(); }
    getTypeName() { return "GetRolesForGroup"; }
}
// @Route("/api/v1/role/groups", "GET")
export class GetGroupsForRole {
    createResponse() { return new Array(); }
    getTypeName() { return "GetGroupsForRole"; }
}
// @Route("/api/v1/role/permission", "PUT")
export class AddPermissionToRoleInfo {
    createResponse() { }
    getTypeName() { return "AddPermissionToRoleInfo"; }
}
// @Route("/api/v1/role/permission", "POST")
export class UpdatePermissionInRoleInfo {
    createResponse() { }
    getTypeName() { return "UpdatePermissionInRoleInfo"; }
}
// @Route("/api/v1/role/permission", "DELETE")
export class DeletePermissionFromRoleInfo {
    createResponse() { }
    getTypeName() { return "DeletePermissionFromRoleInfo"; }
}
// @Route("/api/v1/banks", "GET")
export class BankList {
    createResponse() { return new Array(); }
    getTypeName() { return "BankList"; }
}
// @Route("/api/v1/banks", "POST")
export class BankCreate {
    createResponse() { }
    getTypeName() { return "BankCreate"; }
}
// @Route("/api/v1/banks", "DELETE")
export class BankDelete {
    createResponse() { }
    getTypeName() { return "BankDelete"; }
}
// @Route("/api/v1/banknotifications", "GET")
export class BankNotifications {
    createResponse() { return new Array(); }
    getTypeName() { return "BankNotifications"; }
}
// @Route("/api/v1/corporates", "GET")
export class CorporatesList {
    createResponse() { return new Array(); }
    getTypeName() { return "CorporatesList"; }
}
// @Route("/api/v1/corporate/fielddefinitions", "GET")
export class CorporateFieldDefinitionList {
    createResponse() { return new Array(); }
    getTypeName() { return "CorporateFieldDefinitionList"; }
}
// @Route("/api/v1/corporate/fielddefinition/id", "GET")
export class CorporateFieldDefinitionGetByID {
    createResponse() { return new FieldDefinition(); }
    getTypeName() { return "CorporateFieldDefinitionGetByID"; }
}
// @Route("/api/v1/corporate/fielddefinition/name", "GET")
export class CorporateFieldDefinitionGetByName {
    createResponse() { return new FieldDefinition(); }
    getTypeName() { return "CorporateFieldDefinitionGetByName"; }
}
// @Route("/api/v1/corporate/fielddefinition", "POST")
export class CorporateFieldDefinitionAdd {
    createResponse() { }
    getTypeName() { return "CorporateFieldDefinitionAdd"; }
}
// @Route("/api/v1/corporate/fielddefinition", "PUT")
export class CorporateFieldDefinitionUpdate {
    createResponse() { }
    getTypeName() { return "CorporateFieldDefinitionUpdate"; }
}
// @Route("/api/v1/corporate/fielddefinition/all", "PUT")
export class CorporateAllFieldDefinitionsUpdate {
    createResponse() { }
    getTypeName() { return "CorporateAllFieldDefinitionsUpdate"; }
}
// @Route("/api/v1/corporate/fielddefinition", "DELETE")
export class CorporateFieldDefinitionRemove {
    createResponse() { }
    getTypeName() { return "CorporateFieldDefinitionRemove"; }
}
// @Route("/api/v1/accounts", "GET")
export class AccountList {
    createResponse() { return new Array(); }
    getTypeName() { return "AccountList"; }
}
// @Route("/api/v1/accounts/all", "GET")
export class AccountListAll {
    createResponse() { return new Array(); }
    getTypeName() { return "AccountListAll"; }
}
// @Route("/api/v1/accounts", "POST")
export class AccountCreate {
    createResponse() { }
    getTypeName() { return "AccountCreate"; }
}
// @Route("/api/v1/account", "GET")
export class AccountRead {
    createResponse() { return new Account(); }
    getTypeName() { return "AccountRead"; }
}
// @Route("/api/v1/account", "PUT")
export class AccountUpdate {
    createResponse() { }
    getTypeName() { return "AccountUpdate"; }
}
// @Route("/api/v1/accountstatus", "PUT")
export class ChangeAccountStatus {
    createResponse() { }
    getTypeName() { return "ChangeAccountStatus"; }
}
// @Route("/api/v1/account/documentupload", "PUT")
export class AccountFilledDocumentUpload {
    createResponse() { }
    getTypeName() { return "AccountFilledDocumentUpload"; }
}
// @Route("/api/v1/sign/envelope", "POST")
export class EnvelopeCreate {
    createResponse() { return ""; }
    getTypeName() { return "EnvelopeCreate"; }
}
// @Route("/api/v1/sign/document", "POST")
export class DocumentUploadSigned {
    createResponse() { return ""; }
    getTypeName() { return "DocumentUploadSigned"; }
}
// @Route("/api/v1/account", "DELETE")
export class AccountDelete {
    createResponse() { }
    getTypeName() { return "AccountDelete"; }
}
// @Route("/api/v1/documents", "GET")
export class DocumentList {
    createResponse() { return new Array(); }
    getTypeName() { return "DocumentList"; }
}
// @Route("/api/v1/documents", "POST")
export class DocumentCreate {
    createResponse() { }
    getTypeName() { return "DocumentCreate"; }
}
// @Route("/api/v1/documentUpload", "POST")
export class DocumentUpload {
    createResponse() { }
    getTypeName() { return "DocumentUpload"; }
}
// @Route("/api/v1/document", "GET")
export class DocumentRead {
    createResponse() { }
    getTypeName() { return "DocumentRead"; }
}
// @Route("/api/v1/documentcontent", "GET")
export class DocumentContentRead {
    createResponse() { return new DocumentContent(); }
    getTypeName() { return "DocumentContentRead"; }
}
// @Route("/api/v1/documents", "DELETE")
export class DocumentDelete {
    createResponse() { }
    getTypeName() { return "DocumentDelete"; }
}
// @Route("/api/v1/documentstatus", "PUT")
export class ChangeDocumentStatus {
    createResponse() { }
    getTypeName() { return "ChangeDocumentStatus"; }
}
// @Route("/api/v1/document/fielddefinition", "POST")
export class DocumentFieldDefinitionSet {
    createResponse() { }
    getTypeName() { return "DocumentFieldDefinitionSet"; }
}
// @Route("/api/v1/document/fielddefinition", "PUT")
export class DocumentFieldDefinitionRemove {
    createResponse() { }
    getTypeName() { return "DocumentFieldDefinitionRemove"; }
}
// @Route("/api/v1/documentversions", "DELETE")
export class DocumentVersionDelete {
    createResponse() { }
    getTypeName() { return "DocumentVersionDelete"; }
}
// @Route("/api/v1/documentlink", "PUT")
export class LinkDocumentToAccount {
    createResponse() { }
    getTypeName() { return "LinkDocumentToAccount"; }
}
// @Route("/api/v1/accountlink", "PUT")
export class LinkAccountToBank {
    createResponse() { }
    getTypeName() { return "LinkAccountToBank"; }
}
// @Route("/api/v1/corporate", "GET")
export class CorporateRead {
    createResponse() { return new Corporate(); }
    getTypeName() { return "CorporateRead"; }
}
// @Route("/api/v1/corporates", "POST")
export class CorporateCreate {
    createResponse() { }
    getTypeName() { return "CorporateCreate"; }
}
// @Route("/api/v1/corporates", "DELETE")
export class CorporateDelete {
    createResponse() { }
    getTypeName() { return "CorporateDelete"; }
}
// @Route("/api/v1/corporatenotifications", "GET")
export class CorporateNotifications {
    createResponse() { return new Array(); }
    getTypeName() { return "CorporateNotifications"; }
}
// @Route("/api/v1/corporatenotifications", "PUT")
export class CorporateNotification {
    createResponse() { }
    getTypeName() { return "CorporateNotification"; }
}
// @Route("/api/v1/accounttypes", "GET")
export class AccountTypeReadAll {
    createResponse() { return new Array(); }
    getTypeName() { return "AccountTypeReadAll"; }
}
// @Route("/api/v1/accounttypes/list", "GET")
export class AccountTypeList {
    createResponse() { return new Array(); }
    getTypeName() { return "AccountTypeList"; }
}
// @Route("/api/v1/accounttypes", "POST")
export class AccountTypeCreate {
    createResponse() { }
    getTypeName() { return "AccountTypeCreate"; }
}
// @Route("/api/v1/accounttype", "GET")
export class AccountTypeRead {
    createResponse() { return new AccountType(); }
    getTypeName() { return "AccountTypeRead"; }
}
// @Route("/api/v1/accounttype/documents", "GET")
export class AccountTypeDocumentContents {
    createResponse() { return new DocumentContent(); }
    getTypeName() { return "AccountTypeDocumentContents"; }
}
// @Route("/api/v1/accounttype", "DELETE")
export class AccountTypeDelete {
    createResponse() { }
    getTypeName() { return "AccountTypeDelete"; }
}
// @Route("/api/v1/documentchats", "POST")
export class DocumentChatCreate {
    createResponse() { }
    getTypeName() { return "DocumentChatCreate"; }
}
// @Route("/api/v1/documentchats", "PUT")
export class DocumentChatAppend {
    createResponse() { }
    getTypeName() { return "DocumentChatAppend"; }
}
// @Route("/api/v1/documentchats", "PUT")
export class DocumentChatCancelSubscription {
    createResponse() { }
    getTypeName() { return "DocumentChatCancelSubscription"; }
}
// @Route("/api/v1/chathistory", "GET")
// @Route("/chathistory")
export class GetChatHistory {
    createResponse() { return new GetChatHistoryResponse(); }
    getTypeName() { return "GetChatHistory"; }
}
// @Route("/api/v1/roles", "GET")
export class RoleListGet {
    createResponse() { return new Array(); }
    getTypeName() { return "RoleListGet"; }
}
// @Route("/api/v1/role", "GET")
export class RoleInfoGet {
    createResponse() { return new RoleInfo(); }
    getTypeName() { return "RoleInfoGet"; }
}
// @Route("/api/v1/userRoles", "GET")
export class UserRoleInfoGet {
    createResponse() { return new Array(); }
    getTypeName() { return "UserRoleInfoGet"; }
}
// @Route("/api/v1/systemConfige", "GET")
export class SystemConfigGet {
    createResponse() { return new SystemConfig(); }
    getTypeName() { return "SystemConfigGet"; }
}
// @Route("/api/v1/user/info", "GET")
export class UserConfigGet {
    createResponse() { return new UserConfig(); }
    getTypeName() { return "UserConfigGet"; }
}
// @Route("/api/v1/role", "POST")
export class RoleInfoInsert {
    createResponse() { }
    getTypeName() { return "RoleInfoInsert"; }
}
// @Route("/api/v1/role", "PUT")
export class RoleInfoUpdate {
    createResponse() { }
    getTypeName() { return "RoleInfoUpdate"; }
}
// @Route("/api/v1/role", "DELETE")
export class RoleInfoDelete {
    createResponse() { }
    getTypeName() { return "RoleInfoDelete"; }
}
// @Route("/requestlogs")
// @DataContract
export class RequestLogs {
    createResponse() { return new RequestLogsResponse(); }
    getTypeName() { return "RequestLogs"; }
}
// @Route("/register")
// @DataContract
export class Register {
    createResponse() { return new RegisterResponse(); }
    getTypeName() { return "Register"; }
}
// @Route("/auth")
// @Route("/auth/{provider}")
// @Route("/authenticate")
// @Route("/authenticate/{provider}")
// @DataContract
export class Authenticate {
    createResponse() { return new AuthenticateResponse(); }
    getTypeName() { return "Authenticate"; }
}
// @Route("/assignroles")
// @DataContract
export class AssignRoles {
    createResponse() { return new AssignRolesResponse(); }
    getTypeName() { return "AssignRoles"; }
}
// @Route("/unassignroles")
// @DataContract
export class UnAssignRoles {
    createResponse() { return new UnAssignRolesResponse(); }
    getTypeName() { return "UnAssignRoles"; }
}
// @Route("/session-to-token")
// @DataContract
export class ConvertSessionToToken {
    createResponse() { return new ConvertSessionToTokenResponse(); }
    getTypeName() { return "ConvertSessionToToken"; }
}
//# sourceMappingURL=backendTypes.jsx.map