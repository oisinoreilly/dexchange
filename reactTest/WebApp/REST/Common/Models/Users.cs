using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace Models.DTO.V1
{
    /// <summary>
    /// note: Group is a collectoin 
    /// </summary>
    [ProtoContract(AsReferenceDefault = true)]
    public class Group
    {
        [ProtoMember(1)]
        public string Name { get; set; }

        [BsonId]
        [ProtoMember(2)]
        public string Id { get; set; }

        [ProtoMember(3)]
        public List<int> UserAuthIds { get; set; }

        [ProtoMember(4)]
        public List<string> Roles { get; set; }

        [ProtoMember(5)]
        public List<string> Subgroups { get; set; }
    }

    [ProtoContract(AsReferenceDefault = true)]
    public class Permission
    {
        [BsonId]
        [ProtoMember(1)]
        public string Id { get; set; }
        // TODO: Is resource type necessary>
        [ProtoMember(2)]
        public Resource ResourceType { get; set; }
        [ProtoMember(3)]
        public string ResourceId { get; set; }
        [ProtoMember(4)]
        public List<Access> Access { get; set; }
    }

    public enum Access
    {
        Read = 0,
        Write = 1,
        Delete = 2
    }

    [ProtoContract(AsReferenceDefault = true)]
    public class UserConfig
    {
        [ProtoMember(1)]
        [BsonId]
        public string Id { get; set; }

        [ProtoMember(2)]
        public int UserId { get; set; }

        [ProtoMember(3)]
        public Entity UserType { get; set; }

        [ProtoMember(4)]
        public Privilege UserPrivilege { get; set; }

        [ProtoMember(5)]
        public string EntityDisplayName { get; set; }

        [ProtoMember(6)]
        public string EntityIcon { get; set; }

        [ProtoMember(7)]
        public string EntityID { get; set; }

        [ProtoMember(8)]
        public string EntityName { get; set; }
    }

    public enum Privilege {User, Admin, SuperAdmin};
    public enum Resource { Bank, Corporate, Subsid, Account, AccountType, Document, Field };
    public enum Entity { Bank, Corporate };
}