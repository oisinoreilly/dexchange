using Core.Contracts;
using Models.DTO.V1;
using System;
using System.Collections.Generic;

namespace Core.UnitsOfWork
{
    /// <summary>
    /// Note, I don't use an interface for my unit of work, as I don't want to swap it out for testing.
    /// Instead, these are the operations that I test, which use simple, interface driven objects that can be mocked.
    ///  If I want to test the objects passed in, this is done in a seperate harness, which again uses interfaces to replace
    /// the parts which have external requirements.
    /// Note2: Definition of Unit of work:
    /// Unit of Work design pattern does two important things: first it maintains in-memory updates and 
    /// second it sends these in-memory updates as one transaction to the database.
    /// In the case of this module, I'm using it as a staging area to work with different repositories. 
    /// An example of this is in node.
    /// </summary>

    public class DocumentHQUoW : IDocumentHQUoW
    {
        private readonly IDocumentHQRepository _infrastructureRepos;

        //For mocking, allow implementation to be swapped out.
        public DocumentHQUoW(IDocumentHQRepository infrastructure/*, IDatabase repos, INode node, IItem item*/)
        {
            _infrastructureRepos = infrastructure;
        }


        // Document operations.
        public List<Document> GetDocumentsAtPath(string path)
        {

            return _infrastructureRepos.GetDocumentsAtPath(path);
        }
        public void CreateDocument(string path, Document file)
        {
            _infrastructureRepos.CreateDocument(path, file);
        }

        public Document ReadDocument(string path, string name)
        {
            return _infrastructureRepos.ReadDocument(path, name);
        }
        public void UpdateDocument(string path, Document update)
        {
            _infrastructureRepos.UpdateDocument(path, update);
        }
        public void DeleteDocument(string path, string name)
        {
            _infrastructureRepos.DeleteDocument(path, name);
        }

        // Folder operations.
        public List<string> GetFoldersAtPath(string path)
        {
            return _infrastructureRepos.GetFoldersAtPath(path);
        }
        public void CreateFolder(string path, string name)
        {
            _infrastructureRepos.CreateFolder(path, name);
        }
        public void RenameFolder(string path, string newname)
        {
            _infrastructureRepos.RenameFolder(path, newname);
        }
        public void DeleteFolder(string path, string folderName)
        {
            _infrastructureRepos.DeleteFolder(path, folderName);
        }
    }
}

