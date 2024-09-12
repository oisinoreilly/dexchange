using System.Collections.Generic;
using ProtoBuf;
using ServiceStack;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Models.DTO.V1
{
    #region CRUD Operations

    [Route("/api/v1/documents", "GET")]
    [ProtoContract]
    [Authenticate]
    public class DocumentList : IReturn<List<Document>>
    {
        [ProtoMember(1)]
        public string AccountID { get; set; }
    }

    [Route("/api/v1/documents", "POST")]
    [ProtoContract]
    [Authenticate]
    public class DocumentCreate : IReturnVoid
    {
        [ProtoMember(1)]
        public Document Document { get; set; }
        [ProtoMember(2)]
        public string Username { get; set; }
        [ProtoMember(3)]
        public string DocumentContentBase64 { get; set; }
    }

    [Route("/api/v1/documents", "PUT")]
    [ProtoContract]
    [Authenticate]
    public class DocumentUpdate : IReturnVoid
    {
        [ProtoMember(1)]
        public string ID { get; set; }
        [ProtoMember(2)]
        public string Name { get; set; }
        [ProtoMember(3)]
        public StatusEx Status { get; set; }
    }

    [Route("/api/v1/documentUpload", "POST")]
    [ProtoContract]
    [Authenticate]
    public class DocumentUpload : IReturnVoid
    {
        [ProtoMember(1)]
        public string DocumentContentBase64 { get; set; }

        [ProtoMember(2)]
        public string DocumentID { get; set; }

        [ProtoMember(3)]
        public string Username { get; set; }
    }

    [Route("/api/v1/documentversions", "DELETE")]
    [Authenticate]
    public class DocumentVersionDelete : IReturnVoid
    {
        [ProtoMember(1)]
        public string DocumentID { get; set; }

        [ProtoMember(2)]
        public string VersionID { get; set; }
    }

    [Route("/api/v1/documentversioncreate", "POST")]
    [Authenticate]
    public class DocumentVersionCreate : IReturnVoid
    {
        [ProtoMember(1)]
        public string DocumentID { get; set; }

        [ProtoMember(2)]
        public string VersionID { get; set; }
    }

    [Route("/api/v1/document", "GET")]
    [ProtoContract]
    [Authenticate]
    public class DocumentRead : IReturnVoid
    {
        [ProtoMember(1)]
        public string ID { get; set; }
    }




    [Route("/api/v1/documentcontent", "GET")]
    [ProtoContract]
    [Authenticate]
    public class DocumentContentRead : IReturn<DocumentContent>
    {
        [ProtoMember(1)]
        public string ID { get; set; }
    }

    [Route("/api/v1/documents", "DELETE")]
    [Authenticate]
    public class DocumentDelete : IReturnVoid
    {
        [ProtoMember(1)]
        public string ID { get; set; }     
    }

    [Route("/api/v1/documentstatus", "PUT")]
    [Authenticate]
    public class DocumentUpdateStatus : IReturnVoid
    {
        [ProtoMember(1)]
        public string DocumentID { get; set; }
        [ProtoMember(2)]
        public StatusEx Status { get; set; }
        [ProtoMember(3)]
        public string Username { get; set; }
    }


    [Route("/api/v1/documentlink", "PUT")]
    [Authenticate]
    public class LinkDocumentToAccount : IReturnVoid
    {
        [ProtoMember(1)]
        public string DocumentID { get; set; }
        [ProtoMember(2)]
        public string AccountID { get; set; }
    }

    [Route("/api/v1/document/fielddefinition", "POST")]
    [Authenticate]
    public class DocumentFieldDefinitionSet : IReturnVoid
    {
        public string CorporateID { get; set; }
        public string DocumentID { get; set; }
        public FieldDefinition FieldDefinition { get; set; }
     }

    [Route("/api/v1/document/fielddefinition", "PUT")]
    [Authenticate]
    public class DocumentFieldDefinitionRemove : IReturnVoid
    {
        public string CorporateID { get; set; }
        public string DocumentID { get; set; }
        public string FieldDefinitionID { get; set; }      
    }
    
    [Route("/api/v1/accountlink", "PUT")]
    [Authenticate]
    public class LinkAccountToBank : IReturnVoid
    {
        [ProtoMember(1)]
        public string AccountID { get; set; }
        [ProtoMember(2)]
        public string BankID { get; set; }
    }

    [Route("/api/v1/accountlink", "PUT")]
    [Authenticate]
    public class LinkAccountToCorporate : IReturnVoid
    {
        [ProtoMember(1)]
        public string AccountID { get; set; }
        [ProtoMember(2)]
        public string CorporateID { get; set; }
    }

    [Route("/api/v1/documentchats", "POST")]
    [ProtoContract]
    [Authenticate]
    public class DocumentChatCreate : IReturnVoid
    {
        [ProtoMember(1)]
        public string Caller { get; set; }

        [ProtoMember(2)]
        public string DocumentID { get; set; }
    }

    [Route("/api/v1/documentchats", "PUT")]
    [ProtoContract]
    [Authenticate]
    public class DocumentChatAppend : IReturnVoid
    {
        [ProtoMember(1)]
        public string Channel { get; set; }

        [ProtoMember(2)]
        public string From { get; set; }

        [ProtoMember(3)]
        public string ToUserId { get; set; }

        [ProtoMember(4)]
        public string DocumentID { get; set; }

        [ProtoMember(5)]
        public string Message { get; set; }

        [ProtoMember(6)]
        public string Selector { get; set; }
    }

    [Route("/api/v1/documentchats", "PUT")]
    [ProtoContract]
    [Authenticate]
    public class DocumentChatCancelSubscription : IReturnVoid
    {
        [ProtoMember(1)]
        public string SubscriptionID { get; set; }

        [ProtoMember(2)]
        public string ChannelID { get; set; }


        [ProtoMember(3)]
        public string DocumentID { get; set; }
    }

    [Route("/api/v1/chathistory", "GET")]
    [Route("/chathistory")]
    [Authenticate]
    public class GetChatHistory : IReturn<GetChatHistoryResponse>
    {
        [ProtoMember(1)]
        public List<string> Channels { get; set; }
        [ProtoMember(2)]
        public long? AfterId { get; set; }
        [ProtoMember(3)]
        public int? Take { get; set; }
    }

    [Route("/api/v1/documentstatus", "PUT")]
    [Authenticate]
    public class ChangeDocumentStatus : IReturnVoid
    {
        [ProtoMember(1)]
        public string ID { get; set; }
        [ProtoMember(2)]
        public StatusEx Status { get; set; }
         [ProtoMember(3)]
        public string Username { get; set; }
    }


    #endregion

    #region Base classes
    [ProtoContract(AsReferenceDefault = true)]
    public class Document
    {
        [ProtoMember(1)]
        [BsonId]
        public string Id { get; set; }
        [ProtoMember(2)]
        public string Name { get; set; }
        [ProtoMember(3)]
        public int FormatID { get; set; }
        [ProtoMember(4)]
        public StatusEx Status { get; set; }
        [ProtoMember(5)]
        public List<DocumentVersion> Versions { get; set; }
        [ProtoMember(6)]
        public List<string> Accounts { get; set; }
        [ProtoMember(7)]
        public List<Chat> Chats { get; set; }
        [ProtoMember(8)]
        public bool Global { get; set; }
        [ProtoMember(9)]
        public List<FieldDefinition> Fields { get; set; }  
    }

    [ProtoContract(AsReferenceDefault = true)]
    public class DocumentVersion
    {
        [ProtoMember(1)]
        [BsonId]
        public string Id { get; set; }
        [ProtoMember(2)]
        public string DocumentContentId { get; set; }
        [ProtoMember(3)]
        public string Creation { get; set; }
   }

    /// <summary>
    /// TODO: Change for GridFS, as this is limited to 16mb in size
    /// </summary>
    [ProtoContract(AsReferenceDefault = true)]
    public class DocumentContent
    {
        [ProtoMember(1)]
        [BsonId]
        public string Id { get; set; }
        [ProtoMember(2)]
        public string ContentBase64 { get; set; }
        [ProtoMember(3)]
        public List<FieldDefinition> FieldDefinitions { get; set; }
    }

    [ProtoContract(AsReferenceDefault = true)]
    public class Chat
    {
        [ProtoMember(1)]
        [BsonId]
        public string Id { get; set; }
        [ProtoMember(2)]
        public string DocumentId { get; set; }
        [ProtoMember(3)]
        public string StartTime { get; set; }
        [ProtoMember(4)]
        public string EndTime { get; set; }
        [ProtoMember(5)]
        public string CreatorUserID { get; set; }
        [ProtoMember(6)]
        public List<ChatMessage> ChatMessages{ get; set; }
    }

    [ProtoContract(AsReferenceDefault = true)]
    public class ChatMessage
    {
        [ProtoMember(1)]
        public long Id { get; set; }

        [ProtoMember(2)]
        public string Channel { get; set; }

        [ProtoMember(3)]
        public string FromUserId { get; set; }

        [ProtoMember(4)]
        public string FromName { get; set; }


        [ProtoMember(5)]
        public string Message { get; set; }
        
        [ProtoMember(6)]
        public bool Private { get; set; }

        [ProtoMember(7)]
        public string Time { get; set; }

    }



    [ProtoContract(AsReferenceDefault = true)]
    public class GetChatHistoryResponse
    {
        [ProtoMember(1)]
        public List<ChatMessage> Results { get; set; }
        [ProtoMember(2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

  
    


    #endregion
}
