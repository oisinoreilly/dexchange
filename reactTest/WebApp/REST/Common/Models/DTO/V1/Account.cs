using System.Collections.Generic;
using ProtoBuf;
using ServiceStack;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Models.DTO.V1
{

    [Route("/api/v1/accounts", "GET")]
    [ProtoContract]
    [Authenticate]
    public class AccountList : IReturn<List<Account>>
    {
        [ProtoMember(1)]
        public string BankID { get; set; }
        [ProtoMember(2)]
        public string CorporateID { get; set; }
    }

    [Route("/api/v1/accounts/all", "GET")]
    [ProtoContract]
    [Authenticate]
    public class AccountListAll : IReturn<List<Account>>
    {
    }


    [Route("/api/v1/accounts", "POST")]
    [ProtoContract]
    [Authenticate]
    public class AccountCreate : IReturnVoid
    {
        [ProtoMember(1)]
        public Account Account { get; set; }
        [ProtoMember(2)]
        public string BankID { get; set; }
        [ProtoMember(3)]
        public string Username { get; set; }
        [ProtoMember(4)]
        public bool PrefillDocuments { get; set; }
    }

    [Route("/api/v1/account", "GET")]
    [ProtoContract]
    [Authenticate]
    public class AccountRead : IReturn<Account>
    {
        public string ID { get; set; }
    }

    [Route("/api/v1/account", "PUT")]
    [Authenticate]
    public class AccountUpdate : IReturnVoid
    {
        [ProtoMember(1)]
        public string AccountID { get; set; }
        [ProtoMember(2)]
        public Account Account { get; set; }
        [ProtoMember(3)]
        public string Username { get; set; }
    }

    [Route("/api/v1/accountstatus", "PUT")]
    [Authenticate]
    public class ChangeAccountStatus : IReturnVoid
    {
        [ProtoMember(1)]
        public string AccountID { get; set; }
        [ProtoMember(2)]
        public StatusEx Status { get; set; }
        [ProtoMember(3)]
        public string Username { get; set; }
    }

    [Route("/api/v1/account", "DELETE")]
    [Authenticate]
    public class AccountDelete : IReturnVoid
    {
        public string ID { get; set; }
    }


    [Route("/api/v1/account/documentupload", "PUT")]
    [ProtoContract]
    [Authenticate]
    public class FilledDocumentUpload : IReturnVoid
    {
        public string AccountID { get; set; }
        public string DocumentName { get; set; }
        public DocumentContent DocumentContent { get; set; }

    }


}
