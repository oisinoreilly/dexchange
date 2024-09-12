using DataModels;
using ProtoBuf;
using ServiceStack;
using System.Collections.Generic;
namespace Models.DTO.V1
{
    //TODO: Make this a generic system
    public enum RolesEnum
    {
        All,
        Bank,
        Corporate
    }

    [Route("/api/v1/roles", "GET")]
    public class RoleListGet : IReturn<List<RoleInfo>>
    {
    }

    [Route("/api/v1/role", "GET")]
    public class RoleInfoGet : IReturn<RoleInfo>
    {
        public string RoleName { get; set; }
    }

    [Route("/api/v1/userRoles", "GET")]
    public class UserRoleInfoGet : IReturn<List<RoleInfo>>
    {
        public string Username { get; set; }
    }

    [Route("/api/v1/role", "PUT")]
    public class RoleInfoUpdate : IReturnVoid
    {
        public RoleInfo Role { get; set; }
    }

    [Route("/api/v1/role", "POST")]
    public class RoleInfoInsert : IReturnVoid
    {
        public RoleInfo Role { get; set; }
    }

    [Route("/api/v1/role", "DELETE")]
    public class RoleInfoDelete : IReturnVoid
    {
        public string RoleId { get; set; }
    }

    [Route("/api/v1/role/groups", "GET")]
    [ProtoContract]
    public class GetGroupsForRole: IReturn<List<Group>>
    {
        [ProtoMember(1)]
        public string RoleId { get; set; }
    }


    [Route("/api/v1/group/role", "PUT")]
    public class AddRoleInfoToGroup : IReturnVoid
    {
        public string Group { get; set; }
        public string Role { get; set; }
    }

    [Route("/api/v1/group/role", "PUT")]
    public class AddRolesToGroup : IReturnVoid
    {
        public string GroupId { get; set; }
        public List<string> RoleIds { get; set; }
    }


    [Route("/api/v1/group/role", "DELETE")]
    public class RemoveRoleInfoFromGroup : IReturnVoid
    {
        public string GroupId { get; set; }
        public string RoleId { get; set; }
    }

    [Route("/api/v1/role/permission", "PUT")]
    public class AddPermissionToRoleInfo : IReturnVoid
    {
        public string Role { get; set; }
        public Permission Permission { get; set; }
    }

    [Route("/api/v1/role/permission", "POST")]
    public class UpdatePermissionInRoleInfo : IReturnVoid
    {
        public string Role { get; set; }
        public Permission Permission { get; set; }
    }

    [Route("/api/v1/role/permission", "DELETE")]
    public class DeletePermissionFromRoleInfo : IReturnVoid
    {
        public string Role { get; set; }
        public string PermissionID { get; set; }
    }
}