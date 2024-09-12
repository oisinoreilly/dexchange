using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AccountConfig.Helpers
{
    public class RESTManager
    {
        private string m_URL = "http://localhost:5001";
        public JsonServiceClient RESTHandle
        {
            get;set;
        }
        private void Connect(string user, string password)
        {
            if (null == RESTHandle)
            {
                try
                {
                    RESTHandle = new ServiceStack.JsonServiceClient(m_URL);
                    var response = RESTHandle.Post(new Authenticate
                    {
                        UserName = user,
                        Password = password,
                        RememberMe = true, //important tell client to retain permanent cookies

                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to connect to REST Server: " + ex.Message, "DocumentHQ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                  
                }
            }
        }

    }
}
