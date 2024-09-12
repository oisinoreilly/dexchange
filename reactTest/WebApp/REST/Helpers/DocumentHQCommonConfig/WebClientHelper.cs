using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace ROISoft.Compendia.CommonConfig
{
    public class WebClientHelper
    {
        public static ISession CreateSessionFromUser()
        {
            string username = "";
            string password = "";
            string business = "";

            // Get user details from the registry.
            SessionFactory.GetUserDetails(out username, out password, out business);
            ISession sess = null;
            bool bShowAuthenticationDialog = false;
            if (!string.IsNullOrEmpty(username))
            {
                try
                {
                    sess = SessionFactory.CreateSessionForUser(username, password, business);
                }
                catch (SoapException ex)
                {
                    // If this is an invalid user error, then show up dialog.
                    if (ex.Message.Contains("Invalid user"))
                    {
                        bShowAuthenticationDialog = true;
                    }
                    else
                    {
                        MessageBox.Show("Unable to connect to server: " + ex.Code.Name, "Compendia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

            }
            else bShowAuthenticationDialog = true;

            if (bShowAuthenticationDialog)
            {
                CompendiaAuthenticationForm frm = new CompendiaAuthenticationForm();
                frm._username = username;
                frm._business = business;
                frm._password = password;
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    sess = frm._session;
                    SessionFactory.SetUserDetails(frm._username, frm._password, frm._business);
                }
            }

            return sess;
        }


        public string User
        {
            get;
            set;
        }
        public string Password
        {
            get;
            set;
        }
        public string Business
        {
            get;
            set;
        }
        public string Schema
        {
            get;
            set;
        }
        public string Server
        {
            get { return _server; }
            set { _server = value; }
        }
        private string _server = "http://app.onecompendia.com";
        private string _ashxName = "SmartphoneHandler.ashx";

        private string TestConnection = @"%@/%@?username=%@&password=%@&organisation=%@&customerkey=customerkey&clientVersion=3&operation=TestConnectionAppStoreEx";
        private string GetFolderListing = @"%@/%@?username=%@&organisation=%@&customerkey=%@&clientVersion=%@&operation=GetFolderListing&folderID=%@";
        private string CreateFolder = @"%@/%@?username=%@&organisation=%@&customerkey=%@&clientVersion=%@&operation=CreateFolder&folderID=%@&name=%@";
        private string DeleteFolder = @"%@/%@?username=%@&organisation=%@&customerkey=%@&clientVersion=%@&operation=DeleteFolder&folderID=%@";
        private string GetAssetContent = @"%@/%@?username=%@&organisation=%@&customerkey=%@&clientVersion=%@&operation=GetAssetContent&assetid=%@";
        private string CheckoutAsset = @"%@/%@?username=%@&organisation=%@&customerkey=%@&clientVersion=%@&operation=CheckoutAsset&assetid=%@";
        private string FinalizeAsset = @"%@/%@?username=%@&organisation=%@&customerkey=%@&clientVersion=%@&operation=FinalizeAsset&assetid=%@&auditchange=%@&reviewID=%@";
        private string GetAssetVersionContent = @"%@/%@?username=%@&organisation=%@&customerkey=%@&clientVersion=%@&operation=GetAssetContent&assetid=%@&version=%@";
        private string CreateAsset = @"%@/%@?username=%@&organisation=%@&customerkey=%@&clientVersion=%@&operation=CreateAsset&name=%@&folderID=%@&formatID=%@";
        private string DeleteAsset = @"%@/%@?username=%@&organisation=%@&customerkey=%@&clientVersion=%@&operation=DeleteAsset&assetID=%@";
        private string RenameAsset = @"%@/%@?username=%@&organisation=%@&customerkey=%@&clientVersion=%@&operation=RenameAsset&assetID=%@&name=%@";
        private string RenameFolderstr = @"%@/%@?username=%@&organisation=%@&customerkey=%@&clientVersion=%@&operation=RenameFolder&folderID=%@&name=%@";
        private string PostAssetContent = @"%@/%@?username=%@&organisation=%@&customerkey=%@&clientVersion=%@&operation=PostAssetContent&assetid=%@&version=%@&format=%@";
        private string PostAssetContentWithoutFinalize = @"%@/%@?username=%@&organisation=%@&customerkey=%@&clientVersion=%@&operation=PostAssetContent&assetid=%@&version=%@&format=%@";
        private string GetAssetVersionHistory = @"%@/%@?username=%@&organisation=%@&customerkey=%@&clientVersion=%@&operation=GetAssetVersionHistory&assetid=%@";
        private string GetAuditTrail = @"%@/%@?username=%@&organisation=%@&customerkey=%@&clientVersion=%@&operation=GetAuditTrail&assetid=%@";
        private string GetAssetPermissions = @"%@/%@?username=%@&organisation=%@&customerkey=%@&clientVersion=%@&operation=GetAssetPermissions&assetid=%@";
        private string GetFolderPermissions = @"%@/%@?username=%@&organisation=%@&customerkey=%@&clientVersion=%@&operation=GetFolderPermissions&folderid=%@";
        private string GetFolderListingInitial = @"%@/%@?username=%@&organisation=%@&customerkey=%@&clientVersion=%@&operation=GetFolderListing";
        private string SetAssetPermissions = @"%@/%@?username=%@&organisation=%@&customerkey=%@&clientVersion=%@&operation=SetAssetPermissions&assetid=%@&isInherited=%@&accessRights=%@";
        private string SetFolderPermissions = @"%@/%@?username=%@&organisation=%@&customerkey=%@&clientVersion=%@&operation=SetFolderPermissions&folderid=%@&isInherited=%@&accessRights=%@";
        private string RegisterCustomerAppStore = @"%@/%@?clientVersion=%@&customerkey=%@&operation=RegisterCustomerAppStoreEx&newUser=%@&userName=%@&password=%@&neworganisation=%@&organisation=%@&adminpassword=%@&activationcode=%@&country=%@&email=%@";

  
        public void ConnectToServer(string user, string password, string business)
        {
            //  private string TestConnection = @"%@/%@?username=%@&password=%@&organisation=%@&customerkey=customerkey&clientVersion=%@&operation=TestConnectionAppStoreEx";
      
            string connectURL = string.Format(TestConnection,_server, _ashxName,user, password,business);

            // Perform connection.
            string resp = PerformHTTPRequestWithResponse(connectURL);
            User = user;
            User = password;
            Business = business;
   
            // Process response, populate schema etc.
        }

        private string PerformHTTPRequestWithResponse(string url)
        {
            string ret = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                // Get the response stream  
                StreamReader reader = new StreamReader(response.GetResponseStream());

                // Read the whole contents and return as a string  
                ret = reader.ReadToEnd();
            }
            return ret;
        }


    }
}
