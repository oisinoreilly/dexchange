using Models.DTO.V1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AccountConfig
{
    public partial class frmAddDocument : Form
    {

        public string DocumentContentBase64 { get; set; }

        public frmAddDocument()
        {
            InitializeComponent();
        }

        private void btnUploadDocument_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Stream myStream = null;
                    if ((myStream = fileDialog.OpenFile()) != null)
                    {
                        string fileName = fileDialog.FileName;
                        Byte[] bytes = File.ReadAllBytes(fileName);
                        DocumentContentBase64 = Convert.ToBase64String(bytes);                     
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

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
