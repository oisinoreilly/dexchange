using Models.DTO.V1;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class RoleInfo
    {
        [ProtoMember(1)]
        public string Name { get; set; }
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]

        [ProtoMember(2)]
        public string Id { get; set; }

        [ProtoMember(3)]
        public List<Permission> Permissions { get; set; }
    }
}
