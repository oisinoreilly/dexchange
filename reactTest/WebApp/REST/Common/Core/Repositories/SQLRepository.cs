using Core.Contracts;
using DExchange.CommonConfig;
using DExchange.OMS;
using DExchange.OMS.Support_Classes;
using Models.DTO.V1;
using System;
using System.Collections.Generic;

namespace Core.Repositories
{
    public class SQLRepository : IDocumentHQRepository
    {
        string _dbName = "";
        string _dbLocation = "";
        public SQLRepository(string dbName, string dbLocation)
        {
            // Specify settings for database connection.
            // If database doesn't exist, create it.
            _dbName = dbName;
            _dbLocation = dbLocation;
        }

        private ISession GetSession(string schemaName = "")
        {
            SessionConfig config = new SessionConfig();
            config.Server = "127.0.0.1";
            config.SupportsAuditTrail = true;
            config.ProviderTypeName = "DExchange.OMS.SqlServerDatabaseProvider";
           
            string server = RegistryHelper.GetStringValue("Server", "localhost", false);
            int port = RegistryHelper.GetIntValue("Port", 1433, false);
          //  string database = "DExchange";
             config.User = RegistryHelper.GetStringValue("Username", "dexchangeadmin", false);
            config.User = RegistryHelper.GetStringValue("Username", "dexchangeadmin", false);
            config.Password = RegistryHelper.GetStringValue("Password", "password", false);

            string connString = string.Format(
                "Server={0},{1};Database={2};User ID='{3}';Password='{4}'",
                server, port, _dbName, config.User, config.Password);

            if (string.IsNullOrEmpty(schemaName))
            {
                schemaName = RegistryHelper.GetStringValue("Schema", "schema636240850942940046", false);
            }

            config.Company = schemaName;

            config.ConnectionInfo = connString;

            Session sess = new Session(config);

            sess.EnsureDBExists(_dbName, _dbLocation);
            return  new Session(config);         
        }


        // Document operations.
        public List<Document> GetDocumentsAtPath(string path)
        {
            ISession sess = GetSession();

            return sess.GetFilesAtPath(path);
        }
    
        public void CreateDocument(string path, Document file)
        {
            GetSession().CreateFileAtPath(path, file.Name, file.Content, file.FormatID);
        }

        public Document ReadDocument(string path, string name)
        {
            return GetSession().GetFileAtPath(path, name);
        }
        public void UpdateDocument(string path, Document update)
        {
            // TODO
        }
        public void DeleteDocument(string path, string name)
        {
            GetSession().DeleteFileAtPath(path, name);
        }

        // Folder operations.
        public List<string> GetFoldersAtPath(string path)
        {
            return GetSession().GetFoldersAtPath(path);
        }
        public void CreateFolder(string path, string name)
        {
            GetSession().CreateFolder(path, name);
        }
        public void RenameFolder(string path, string newname)
        {
          // TODO
        }
        public void DeleteFolder(string path, string folderName)
        {
            GetSession().DeleteFolder(path, folderName);
        }
    }
}
