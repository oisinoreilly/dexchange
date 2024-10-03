using System.Collections.Generic;
using ProtoBuf;
using ServiceStack;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Models.DTO.V1
{

    [Route("/api/v1/accounttypes", "GET")]
    [ProtoContract]
    ////[Authenticate]
    public class AccountTypeReadAll : IReturn<List<AccountType>>
    {
        [ProtoMember(1)]
        public string BankID { get; set; }
      
    }
    
    [Route("/api/v1/accounttypes/list", "GET")]
    [ProtoContract]
    //[Authenticate]
    public class AccountTypeList : IReturn<List<string>>
    {
        [ProtoMember(1)]
        public string BankID { get; set; }

    }

    [Route("/api/v1/accounttypes/listbyid", "GET")]
    [ProtoContract]
    //[Authenticate]
    public class AccountTypeListByID : IReturn<List<string>>
    {
        [ProtoMember(1)]
        public string BankID { get; set; }

    }

    [Route("/api/v1/accounttypes", "POST")]
    [ProtoContract]
    ////[Authenticate]
    public class AccountTypeCreate : IReturnVoid
    {
        [ProtoMember(1)]
        public AccountType Accounttype { get; set; }
  
    }

    [Route("/api/v1/accounttype", "GET")]
    [ProtoContract]
    ////[Authenticate]
    public class AccountTypeRead : IReturn<AccountType>
    {
        public string ID { get; set; }
    }

    [Route("/api/v1/accounttype/name", "GET")]
    [ProtoContract]
    //[Authenticate]
    public class AccountTypeReadByName : IReturn<AccountType>
    {
        public string Name { get; set; }
    }

    [Route("/api/v1/accounttype/documents", "GET")]
    [ProtoContract]
    //[Authenticate]
    public class AccountTypeDocumentContents : IReturn<DocumentContent>
    {
        public string AccountTypeID { get; set; }
        public string DocumentID { get; set; }
        public bool PrefillFields { get; set; }
    }

    [Route("/api/v1/accounttype", "PUT")]
    //[Authenticate]
    public class AccountTypeUpdate : IReturnVoid
    {
        [ProtoMember(1)]
        public string AccountID { get; set; }
        [ProtoMember(2)]
        public AccountType Accounttype { get; set; }
      
    }

    [Route("/api/v1/accounttype", "DELETE")]
    //[Authenticate]
    public class AccountTypeDelete : IReturnVoid
    {
        public string ID { get; set; }
    }
}
