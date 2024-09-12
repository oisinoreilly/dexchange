using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Models.DTO.V1
{
    public enum Status
    {
        Pending_e,
        Approved_e,
        Rejected_e,
        InformationRequired_e,
        InheritFromChild_e,
        NotSet_e
    }

    [ProtoContract]
    [Authenticate]
    public class StatusEx
    {
        [ProtoMember(1)]
        public Status Status { get; set; }

        [ProtoMember(2)]
        public string Comment { get; set; }
    }


    [ProtoContract]
    [Authenticate]
    public class AccountDetail : GenericFieldDefinitions
    {
        [ProtoMember(1)]
        public string Type { get; set; }

        [ProtoMember(2)]
        public string Description { get; set; }

        [ProtoMember(3)]
        public string Overdraft { get; set; }

        [ProtoMember(4)]
        public string CompanyName { get; set; }

        [ProtoMember(5)]
        public string Address { get; set; }

        [ProtoMember(6)]
        public string CompanyContact { get; set; }

        [ProtoMember(7)]
        public string Email { get; set; }
        
        [ProtoMember(8)]
        public string Phone { get; set; }

        [ProtoMember(9)]
        public List<string> Directors { get; set; }
        
        [ProtoMember(10)]
        public List<string> Signatories { get; set; }
    }


    [ProtoContract(AsReferenceDefault = true)]
    public class Account
    {
        [ProtoMember(1)]
        [BsonId]
        public string Id { get; set; }
        [ProtoMember(2)]
        public string ParentID { get; set; }
        [ProtoMember(3)]
        [Obsolete]
        public string CorporateId { get; set; }
        [ProtoMember(4)]
        public string Name { get; set; }
        [ProtoMember(5)]
        public string Creation { get; set; }
        [ProtoMember(6)]
        public AccountDetail Detail { get; set; }
        [ProtoMember(7)]
        public StatusEx Status { get; set; }
        [ProtoMember(8)]
        public List<string> Documents { get; set; }
        [ProtoMember(9)]
        public string AccountType { get; set; }

    }
}
