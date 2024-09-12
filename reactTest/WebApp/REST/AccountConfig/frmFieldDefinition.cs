using DocumentHQ.CommonConfig;
using Models.DTO.V1;
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
    public partial class frmFieldDefinition : Form
    {
        public FieldDefinition FieldDef
        {
            get
            {
                if (null == m_fieldDefinition)
                {
                    m_fieldDefinition = new FieldDefinition();
                }
                m_fieldDefinition.Name = textBoxName.Text.Trim();
                m_fieldDefinition.DefaultValue = textBoxValue.Text.Trim();
                return m_fieldDefinition;
            }
            set
            {
                m_fieldDefinition = value;
                textBoxName.Text = m_fieldDefinition.Name;
                textBoxValue.Text = m_fieldDefinition.DefaultValue;
            }
        }
        private FieldDefinition m_fieldDefinition = null;
        public frmFieldDefinition()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxName.Text.Trim()))
            {
                MessageBox.Show("Name must be specified", CommonGlobals.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void frmFieldDefinition_Load(object sender, EventArgs e)
        {
            // Set name field to be disabled if we are modifying an existing field.
            if (null != m_fieldDefinition)
            {
                textBoxName.Enabled = false;
            }
        }
    }
}
