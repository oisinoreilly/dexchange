using Models.DTO.V1;
using System;
using System.Collections.Generic;

namespace Core.Contracts
{
    public interface IDocumentHQUoW
    {
        // Document operations.
        List<Document> GetDocumentsAtPath(string path);
        void CreateDocument(string path, Document file);
        Document ReadDocument(string path, string name);
        void UpdateDocument(string path, Document update);
        void DeleteDocument(string path, string name);

        // Folder operations.
        List<string> GetFoldersAtPath(string path);
        void CreateFolder(string path, string name);
        void RenameFolder(string path, string newname);
        void DeleteFolder(string path, string folderName);
    }
}
