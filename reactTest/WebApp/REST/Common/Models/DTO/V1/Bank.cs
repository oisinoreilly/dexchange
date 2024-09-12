using System.Collections.Generic;
using ProtoBuf;
using ServiceStack;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Models.DTO.V1
{
    #region CRUD Operations

    [Route("/api/v1/banks", "GET")]
    [ProtoContract]
    [Authenticate]
    public class BankList : IReturn<List<Bank>>
    {
        [ProtoMember(1)]
        public string Filter { get; set; } 
    }

    [Route("/api/v1/banks", "POST")]
    [ProtoContract]
    [Authenticate]
    public class BankCreate : IReturnVoid
    {
        [ProtoMember(1)]
        public Bank Bank { get; set; }
    }

    [Route("/api/v1/banks", "DELETE")]
    [ProtoContract]
    [Authenticate]
    public class BankDelete : IReturnVoid
    {
        [ProtoMember(1)]
        public string BankID { get; set; }

    }

    [Route("/api/v1/banknotifications", "GET")]
    [ProtoContract]
    [Authenticate]
    public class BankNotifications : IReturn<List<Notification>>
    {
        [ProtoMember(1)]
        public string BankID { get; set; }

        [ProtoMember(2)]
        public int MaximumCount { get; set; }
    }


    [ProtoContract(AsReferenceDefault = true)]
    public class Bank
    {
        [ProtoMember(1)]
        public string Name { get; set; }
        [ProtoMember(2)]
        [BsonId]
        public string Id { get; set; }
        [ProtoMember(3)]
        public string IconBase64 { get; set; }
        [ProtoMember(4)]
        public List<string> Accounts { get; set; }
        [ProtoMember(5)]
        public List<string> Notifications { get; set; }
        [ProtoMember(6)]
        public StatusEx Status { get; set; }
        [ProtoMember(7)]
        public List<FieldDefinition> FieldDefinitions { get; set; }
        [ProtoMember(8)]
        public List<AccountType> AccountTypes { get; set; }
    }

    #endregion
}
