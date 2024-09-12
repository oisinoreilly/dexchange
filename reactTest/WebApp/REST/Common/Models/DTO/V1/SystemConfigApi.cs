using ProtoBuf;
using ServiceStack;


namespace DataModels.DTO.V1
{

    [Route("/api/v1/systemConfig", "GET")]
    public class SystemConfigGet : IReturn<SystemConfig>
    {
        [ProtoMember(1)]
        public string RoleName { get; set; }
    }
}
