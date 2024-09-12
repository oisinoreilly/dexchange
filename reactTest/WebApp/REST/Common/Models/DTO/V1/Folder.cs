using System.Collections.Generic;
using System.Net;
using ProtoBuf;
using ServiceStack;

namespace DataModels.DTO.V1
{
    [Route("/api/v1/folder", "GET", Summary = @"Retrieve list of subfolders for a path")]
    [ProtoContract]
    public class FolderList : IReturn<string[]>
    {
        [ApiMember(Description = "Root folder", ParameterType = "path", DataType = "string", IsRequired = false)]
        [ProtoMember(1)]
        public string Path { get; set; }
    }

    #region CRUD operations
    [Route("/api/v1/folder", "POST", Summary = @"Create a folder")]
    [ProtoContract]
    public class FolderCreate : IReturnVoid
    {
        [ApiMember(Description = "Folder name", ParameterType = "path", DataType = "string", IsRequired = true)]
        [ProtoMember(1)]
        public string Name { get; set; }

        [ApiMember(Description = "Folder path", ParameterType = "path", DataType = "string", IsRequired = true)]
        [ProtoMember(1)]
        public string Path { get; set; }
    }

    [Route("/api/v1/folder", "PUT", Summary = @"")]
    [ProtoContract]
    public class FolderRename : IReturnVoid
    {
        [ProtoMember(1)]
        public string Path { get; set; }

        [ProtoMember(2)]
        public string NewName { get; set; }
    }

    [Route("/api/v1/folder", "DELETE", Summary = @"")]
    [ProtoContract]
    public class FolderDelete: IReturnVoid
    {
        [ProtoMember(1)]
        public string Path  { get; set; }
        [ProtoMember(2)]
        public string Name { get; set; }
    }
    #endregion

}