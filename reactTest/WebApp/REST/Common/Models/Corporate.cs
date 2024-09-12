using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;
using ServiceStack;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Models.DTO.V1
{
    public enum NotificationType
    {
        StatusCreateAccount_e,
        StatusChangeAccount_e,
        StatusCreateDocument_e,
        StatusChangeDocument_e,
        Message_e
    }

    [ProtoContract]
    public class StatusUpdate
    {
        public List<StatusCouple> DocumentUpdates { get; set; }
        public List<StatusCouple> AccountUpdates { get; set; }
        public List<StatusCouple> CorporateUpdates { get; set; }
        public List<StatusCouple> BankUpdates { get; set; }
    }

    [ProtoContract]
    public class StatusCouple
    {
        [ProtoMember(1)]
        public string ID { get; set; }

        [ProtoMember(2)]
        public Status Status { get; set; }
    }


    [ProtoContract]
    public class Notification
    {
        [ProtoMember(1)]
        [BsonId]
        public string Id { get; set; }

        [ProtoMember(2)]
        public NotificationType Type { get; set; }

        [ProtoMember(3)]
        public StatusUpdate StatusUpdate { get; set; }

        [ProtoMember(4)]
        public string DocumentID { get; set; }

        [ProtoMember(5)]
        public string AccountID { get; set; }

        [ProtoMember(6)]
        public string BankName { get; set; }
        
        [ProtoMember(7)]
        public string BankID { get; set; }

        [ProtoMember(8)]
        public string CorporateName{ get; set; }

        [ProtoMember(9)]
        public string UserName { get; set; }

        [ProtoMember(10)]
        public string CreationDate { get; set; }

        [ProtoMember(11)]
        public string Message { get; set; }
        
        [ProtoMember(12)]
        public bool Processed { get; set; }
    }

    [ProtoContract]
    [Authenticate]
    public class Corporate
    {
        [ProtoMember(1)]
        [BsonId]
        public string Id { get; set; }
        [ProtoMember(2)]
        public string Icon { get; set; }
        [ProtoMember(3)]
        public CorporateDetail Detail { get; set; }
        [ProtoMember(4)]
        public List<string> Accounts { get; set; }
        [ProtoMember(5)]
        public List<string> Notifications { get; set; }
        [ProtoMember(6)]
        public StatusEx Status { get; set; }
        [ProtoMember(7)]
        public List<string> Children { get; set; }
        [ProtoMember(8)]
        public string ParentID { get; set; }
        [ProtoMember(9)]
        public string DisplayID { get; set; }
        [ProtoMember(10)]
        public List<FieldDefinition> Fields { get; set; }
    }

    [ProtoContract]
    [Authenticate]
    public class CorporateDetail
    {
        [ProtoMember(1)]
        public string Name{ get; set; }
        [ProtoMember(2)]
        public string TradingName { get; set; }
        [ProtoMember(3)]
        public string DateOfIncorporation { get; set; }
        [ProtoMember(4)]
        public string PhoneNumber { get; set; }
        [ProtoMember(6)]
        public string EmailAddress{ get; set; }
        [ProtoMember(7)]
        public string CorrespondanceAddress { get; set; }
        [ProtoMember(8)]
        public string StockExchangeDetail { get; set; }
        [ProtoMember(9)]
        public string Turnover { get; set; }
        [ProtoMember(10)]
        public string TypeOfEntity { get; set; }
        [ProtoMember(10)]
        public string NatureOfBusiness { get; set; }
        [ProtoMember(11)]
        public List<Signatory> Signatories { get; set; }
        [ProtoMember(12)]
        public List<FieldDefinition> Fields { get; set; }
    }

  
    [ProtoContract]
    [Authenticate]
    public class Signatory
    {
        [ProtoMember(1)]
        public string Name { get; set; }
        [ProtoMember(2)]
        public string Role { get; set; }
    }

    [ProtoContract]
    [Authenticate]
    public class GenericFieldDefinitions
    {
        [ProtoMember(1)]
        public List<FieldDefinition> Fields { get; set; }
    }
}
