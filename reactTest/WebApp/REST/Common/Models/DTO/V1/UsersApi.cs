using DataModels;
using ProtoBuf;
using ServiceStack;
using ServiceStack.Auth;
using System.Collections.Generic;
namespace Models.DTO.V1
{
    #region USER_CLASSES
    [Route("/api/v1/users", "GET")]
    [ProtoContract]
    public class UsersListGet : IReturn<List<UserAuth>>
    {
    }

    [Route("/api/v1/user", "GET")]
    [ProtoContract]
    public class UserInfoGet : IReturn<UserAuth>
    {
        [ProtoMember(1)]
        public string UserName { get; set; }
    }

    [Route("/api/v1/user", "POST")]
    [ProtoContract]
    public class UserInfoPost : IReturnVoid
    {
        public UserAuth UserToCreate { get; set; }
        public string Password { get; set; }
        public UserConfig Config { get; set; }       
    }

    [Route("/api/v1/user", "PUT")]
    [ProtoContract]
    public class UserInfoPut : IReturnVoid
    {
        [ProtoMember(1)]
        public UserAuth UserToUpdate { get; set; }
        [ProtoMember(2)]
        public UserConfig Config { get; set; }
    }

    [Route("/api/v1/user", "DELETE")]
    [ProtoContract]
    public class UserInfoDelete : IReturnVoid
    {
        [ProtoMember(1)]
        public string UserToDelete { get; set; }
    }

    [Route("/api/v1/user/info", "GET")]
    [ProtoContract]
    public class UserConfigGet : IReturn<UserConfig>
    {
        [ProtoMember(1)]
        public int UserID { get; set; }
    }

    #endregion

    #region GROUP_CLASSES


    [Route("/api/v1/groups/id", "GET")]
    [ProtoContract]
    public class GroupGetByID : IReturn<Group>
    {
        [ProtoMember(1)]
        public string GroupID { get; set; }
    }


    [Route("/api/v1/groups/name", "GET")]
    [ProtoContract]
    public class GroupGetByName : IReturn<Group>
    {
        [ProtoMember(1)]
        public string GroupName { get; set; }
    }

    [Route("/api/v1/groups/list", "GET")]
    [ProtoContract]
    public class GroupsListGet : IReturn<List<Group>>
    {
        [ProtoMember(1)]
        public List<Group> Groups { get; set; }
    }


    [Route("/api/v1/groups", "POST")]
    [ProtoContract]
    public class GroupCreate : IReturnVoid
    {
        [ProtoMember(1)]
        public Group Group { get; set; }
    }


    [Route("/api/v1/groups", "DELETE")]
    [ProtoContract]
    public class GroupDelete : IReturnVoid
    {
        [ProtoMember(1)]
        public string GroupId { get; set; }
    }


    [Route("/api/v1/group/user", "PUT")]
    [ProtoContract]
    public class AddUserToGroup : IReturnVoid
    {
        [ProtoMember(1)]
        public string Group { get; set; }
        [ProtoMember(2)]
        public string User { get; set; }
    }

    [Route("/api/v1/group/users", "PUT")]
    [ProtoContract]
    public class AddUsersToGroup : IReturnVoid
    {
        [ProtoMember(1)]
        public string GroupId { get; set; }
        [ProtoMember(2)]
        public List<int> UserIds { get; set; }
    }

    [Route("/api/v1/group/user", "DELETE")]
    [ProtoContract]
    public class DeleteUserFromGroup : IReturnVoid
    {
        [ProtoMember(1)]
        public string GroupId { get; set; }
        [ProtoMember(2)]
        public int UserId { get; set; }
    }

    [Route("/api/v1/group/subgroup", "PUT")]
    [ProtoContract]
    public class AddSubgroupToGroup : IReturnVoid
    {
        [ProtoMember(1)]
        public string Group { get; set; }
        [ProtoMember(2)]
        public Group Subgroup { get; set; }
    }

    [Route("/api/v1/group/permission", "DELETE")]
    [ProtoContract]
    public class RemoveSubgroupFromGroup : IReturnVoid
    {
        [ProtoMember(1)]
        public string Group { get; set; }
        [ProtoMember(2)]
        public string PermissionID { get; set; }
        [ProtoMember(3)]
        public Permission Permission { get; set; }
    }

    [Route("/api/v1/group/users", "GET")]
    [ProtoContract]
    public class GetUsersForGroup : IReturn<List<UserAuth>>
    {
        [ProtoMember(1)]
        public string Group { get; set; }
    }

    [Route("/api/v1/group/roles", "GET")]
    [ProtoContract]
    public class GetRolesForGroup : IReturn<List<RoleInfo>>
    {
        [ProtoMember(1)]
        public string Group { get; set; }
    }
    #endregion

}