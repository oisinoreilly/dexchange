using System.Collections.Generic;
using ProtoBuf;
using ServiceStack;

namespace Models.DTO.V1
{
    [Route("/api/v1/sign/envelope", "POST")]
    [ProtoContract]
    [Authenticate]
    public class EnvelopeCreate : IReturn<string>
    {
        [ProtoMember(1)]
        public string DocumentId { get; set; }
    }

    [Route("/api/v1/sign/document", "POST")]
    [ProtoContract]
    [Authenticate]
    public class DocumentUploadSigned : IReturn<string>
    {
        [ProtoMember(1)]
        public string DocumentId { get; set; }

        [ProtoMember(2)]
        public string EnvelopeId { get; set; }
    }
}
