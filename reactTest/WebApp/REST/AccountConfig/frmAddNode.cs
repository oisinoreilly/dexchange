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

namespace AccountConfig
{

    public partial class frmAddNode : Form
    {
        string url = CommonGlobals.serverUrl + ":5001";
        public RESTManager RestManager
        {
            get;set;
        }

        public frmAddNode()
        {
            InitializeComponent();
        }

        public frmAddNode(string nodeTypeText)
        {
            InitializeComponent();
            this.Text = nodeTypeText;
            if (this.groupBoxNodeType != null)
                this.groupBoxNodeType.Text = nodeTypeText;
        }

        /// <summary>
        /// On clicking OK. Name must be specified.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxName.Text.Trim()))
            {
                MessageBox.Show("Name must be specified", "Connect", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBoxName.Focus();
                return;
            }

            DialogResult = DialogResult.OK;
            this.Close();

        }
    }
}
