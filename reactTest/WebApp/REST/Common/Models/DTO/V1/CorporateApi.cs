using ProtoBuf;
using ServiceStack;
using System.Collections.Generic;
namespace Models.DTO.V1
{
    [Route("/api/v1/corporates", "GET")]
    [ProtoContract]
    public class CorporatesList : IReturn<List<Corporate>>
    {
        [ProtoMember(1)]
        public string Parent { get; set; }
    }

    [Route("/api/v1/corporate", "GET")]
    [ProtoContract]
    public class CorporateRead : IReturn<Corporate>
    {
        [ProtoMember(1)]
        public string CorporateID { get; set; }
    }

    [Route("/api/v1/corporates", "POST")]
    [ProtoContract]
    public class CorporateCreate : IReturnVoid
    {
        [ProtoMember(1)]
        public Corporate Corporate { get; set; }
    }

    [Route("/api/v1/corporates", "DELETE")]
    [ProtoContract]
    public class CorporateDelete : IReturnVoid
    {
        //Note: All children need to be deleted recursively as well.
        [ProtoMember(1)]
        public string CorporateID { get; set; }
    }

    [Route("/api/v1/corporatenotifications", "GET")]
    [ProtoContract]
    [Authenticate]
    public class CorporateNotifications : IReturn<List<Notification>>
    {
        [ProtoMember(1)]
        public string CorporateID { get; set; }
        
        [ProtoMember(2)]
        public int MaximumCount { get; set; }
    }

    [Route("/api/v1/corporatenotifications", "PUT")]
    [ProtoContract]
    [Authenticate]
    public class CorporateNotification : IReturnVoid
    {
        [ProtoMember(1)]
        public string CorporateID { get; set; }
        [ProtoMember(2)]
        public Notification Notification { get; set; }
    }

    [Route("/api/v1/corporate/fielddefinitions", "GET")]
    [Authenticate]
    public class CorporateFieldDefinitionList : IReturn<List<FieldDefinition>>
    {
        public string CorporateID { get; set; }
    }


    [Route("/api/v1/corporate/fielddefinition/id", "GET")]
    [Authenticate]
    public class CorporateFieldDefinitionGetByID : IReturn<FieldDefinition>
    {
        public string FieldID { get; set; }
    }


    [Route("/api/v1/corporate/fielddefinition/name", "GET")]
    [Authenticate]
    public class CorporateFieldDefinitionGetByName : IReturn<FieldDefinition>
    {
        public string FieldName { get; set; }
    }

    [Route("/api/v1/corporate/fielddefinition", "POST")]
    [Authenticate]
    public class CorporateFieldDefinitionAdd : IReturnVoid
    {
        public string CorporateID { get; set; }
        public FieldDefinition Definition { get; set; }
    }
    
    [Route("/api/v1/corporate/fielddefinition", "PUT")]
    [Authenticate]
    public class CorporateFieldDefinitionUpdate : IReturnVoid
    {
        public string CorporateID { get; set; }
        public string FieldDefinitionID { get; set; }
        public FieldDefinition Definition { get; set; }
    }


    [Route("/api/v1/corporate/fielddefinition/all", "PUT")]
    [ProtoContract]
    public class CorporateAllFieldDefinitionsUpdate : IReturnVoid
    {
        [ProtoMember(1)]
        public string CorporateID { get; set; }

        [ProtoMember(2)]
        public List<FieldDefinition> FieldDefinitions { get; set; }
    }

    [Route("/api/v1/corporate/fielddefinition", "DELETE")]
    [Authenticate]
    public class CorporateFieldDefinitionRemove : IReturnVoid
    {
        public string CorporateID { get; set; }
        public string FieldDefinitionName { get; set; }
    }

}
