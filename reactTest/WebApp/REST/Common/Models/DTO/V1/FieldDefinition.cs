using ProtoBuf;
using ServiceStack;
namespace Models.DTO.V1
{
    [Route("/api/v1/fielddefinition", "POST")]
    [Authenticate]
    public class FieldDefinitionAdd : IReturnVoid
    {
        public string CorporateID { get; set; }
        public FieldDefinition Definition { get; set; }
    }


    [Route("/api/v1/fielddefinition", "PUT")]
    [Authenticate]
    public class FieldDefinitionUpdate : IReturnVoid
    {
        public string CorporateID { get; set; }
        public string FieldDefinitionID { get; set; }
        public FieldDefinition Definition { get; set; }
    }


    [Route("/api/v1/fielddefinition", "DELETE")]
    [Authenticate]
    public class FieldDefinitionRemove : IReturnVoid
    {
        public string CorporateID { get; set; }
        public string FieldDefinitionID { get; set; }
    }

    /// <summary>
    /// Class for basic field definition (derived from CSV file).
    /// </summary>
    public class FieldDefinition
    {
        [ProtoMember(1)]
        public string Id { get; set; }
        [ProtoMember(2)]
        public string Name { get; set; }
        [ProtoMember(3)]
        public eDataType DataType { get; set; }
        [ProtoMember(4)]
        public int MinimumLength { get; set; }
        // Default value is used if field value is presented to user for new document
        // using a field that has been specified in a previous document.
        [ProtoMember(5)]
        public string DefaultValue { get; set; }
        // TODO: Boolean to "use default if set" ..?
    }


    public enum eDataType
    {
        String_e,
        Bool_e,
        Number_e,
        StringArray_e
    }

    // Define scope of field.
   /* public enum eFieldScope
    {
        Corporate_e,
        AccountType_e,
        Document_e       
    }*/
}
