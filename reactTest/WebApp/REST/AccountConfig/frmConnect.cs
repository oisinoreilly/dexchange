using AccountConfig.Helpers;
using DocumentHQ.CommonConfig;
using Microsoft.Win32;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DocumentHQ.CommonConfig;

namespace AccountConfig
{

    public partial class frmConnect : Form
    {
        string url = CommonGlobals.serverUrl + ":5001";
        public RESTManager RestManager
        {
            get;set;
        }

        public frmConnect()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxUser.Text.Trim()))
            {
                MessageBox.Show("User must be specified", "Connect", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBoxUser.Focus();
                return;
            }

            if (string.IsNullOrEmpty(textBoxPassword.Text.Trim()))
            {
                MessageBox.Show("Password must be specified", "Connect", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBoxPassword.Focus();
                return;
            }

            try
            {
                RestManager.RESTHandle = new ServiceStack.JsonServiceClient(url);
                var response = RestManager.RESTHandle.Post(new Authenticate
                {
                    UserName = textBoxUser.Text.Trim(),
                    Password = textBoxPassword.Text.Trim(),
                    RememberMe = true, //important tell client to retain permanent cookies
                });


                Registry.SetValue(CommonGlobals.RegistryKey, "ShowConnect", chkShowAgain.Checked ? 1:0);
                Registry.SetValue(CommonGlobals.RegistryKey, "User", textBoxUser.Text.Trim());
                Registry.SetValue(CommonGlobals.RegistryKey, "Password", textBoxPassword.Text.Trim());
                DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to connect to server: " + ex.Message);
                return;
            }
        }
        
    }
}
