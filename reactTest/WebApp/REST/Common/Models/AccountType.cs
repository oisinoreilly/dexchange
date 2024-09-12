using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;
using System.Collections.Generic;

namespace Models.DTO.V1
{
    /// <summary>
    /// Account Type class. 
    /// This class is modified on the corporate database with default values for fields.
    /// </summary>
    public class AccountType
    {
        [ProtoMember(1)]
        public string Id { get; set; }
        [ProtoMember(2)]
        public string Name { get; set; }
        [ProtoMember(3)]
        public string BankID { get; set; }
        [ProtoMember(4)]
        public List<string> BaseDocumentIDs { get; set; }
        [ProtoMember(5)]
        public List<string> BaseDocumentNames { get; set; }
        // Definitions at account type level.
        [ProtoMember(6)]
        public List<FieldDefinition> Definitions { get; set; }
    }

}
