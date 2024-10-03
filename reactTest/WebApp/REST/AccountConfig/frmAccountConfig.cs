using AccountConfig.Helpers;
using DocumentHQ.CommonConfig;
using Microsoft.Win32;
using Models.DTO.V1;
using MongoDB.Bson;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AccountConfig
{
    public partial class frmAccountConfig : Form
    {
        string url = CommonGlobals.serverUrl + ":5001";
        private RESTManager m_restManager = new RESTManager();

        private const int IMAGE_INDEX_SUCCESS = 0;
        private const int IMAGE_INDEX_WARNING = IMAGE_INDEX_SUCCESS + 1;
        private const int IMAGE_INDEX_ERROR = IMAGE_INDEX_WARNING + 1;
        private const int IMAGE_INDEX_BANK = IMAGE_INDEX_ERROR + 1;
        private const int IMAGE_INDEX_ACCOUNTTYPE = IMAGE_INDEX_BANK + 1;
        private const int IMAGE_INDEX_DOCUMENT = IMAGE_INDEX_ACCOUNTTYPE + 1;
        private const int IMAGE_INDEX_FIELDDEFINITION = IMAGE_INDEX_DOCUMENT + 1;

        public frmAccountConfig()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void frmAccountConfig_Load(object sender, EventArgs e)
        {
            RefreshView();

        }
        private void RefreshView()
        {
            // Clear Treeview.
            treeView1.Nodes.Clear();
            TreeNode treenode = new TreeNode();
            treeView1.Nodes.Add("Banks", "Banks", 0, 0);

            // Add dummy.
            AddDummyTreeNode(treeView1.Nodes[0]);

            JsonServiceClient ret = null;
            try
            {
                if (null == (ret = GetRESTHandle()))
                {
                    Application.Exit();
                    Application.DoEvents();
                    return;

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to connect to REST Server: " + ex.Message, "DocumentHQ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
                Application.DoEvents();
                return;
            }

            // Now add 


        }

        private JsonServiceClient GetRESTHandle()
        {
            JsonServiceClient ret = null;
            bool bShowConnectForm = (1 == (Convert.ToInt32(Registry.GetValue(CommonGlobals.RegistryKey, "ShowConnect", 1))));
            string user = Convert.ToString(Registry.GetValue(CommonGlobals.RegistryKey, "User", ""));
            string password = Convert.ToString(Registry.GetValue(CommonGlobals.RegistryKey, "Password", ""));

            if (null == m_restManager.RESTHandle)
            {
                if (bShowConnectForm)
                {
                    frmConnect frm = new frmConnect();
                    frm.RestManager = m_restManager;

                    if (DialogResult.OK == frm.ShowDialog(this))
                    {
                        ret = m_restManager.RESTHandle = frm.RestManager.RESTHandle;

                    }
                }
                else
                {
                    try
                    {
                        JsonServiceClient svc = new ServiceStack.JsonServiceClient(url);
                        var response = svc.Post(new Authenticate
                        {
                            UserName = user,
                            Password = password,
                            RememberMe = true, //important tell client to retain permanent cookies

                        });
                        ret = m_restManager.RESTHandle = svc;

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Unable to connect to REST Server: " + ex.Message, CommonGlobals.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        // Show dialog regardless of setting, as we don't have connectivity to REST Server.
                        frmConnect frm = new frmConnect();
                        frm.RestManager = m_restManager;

                        if (DialogResult.OK == frm.ShowDialog(this))
                        {
                            ret = m_restManager.RESTHandle = frm.RestManager.RESTHandle;
                        }
                    }


                }
            }
            return ret;
        }

        private void AddDummyTreeNode(TreeNode parent)
        {
            if (null != parent)
            {
                parent.Nodes.Add("Dummy", "Dummy", 0, 0);
            }
        }

        private void RemoveDummyTreeNode(TreeNode parent)
        {
            if (null != parent)
            {
                TreeNode[] node = parent.Nodes.Find("Dummy", false);

                // Remove dummy if it exists.
                if (node.GetLength(0) > 0)
                {
                    treeView1.Nodes.Remove(node[0]);
                }

            }
        }

        private void ExpandTreeView()
        {
            RefreshView();

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode treenode = e.Node;

            if (null == treenode)
                return;

            try
            {
                GetRESTHandle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to connect to REST Server: " + ex.Message, CommonGlobals.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                //  if (null == treenode.Tag)
                FillTree(e.Node);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to expand tree: " + ex.Message, CommonGlobals.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

        }

        /// <summary>
        /// After expanding tree view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_AfterExpand(object sender, TreeViewEventArgs e)
        {
            try
            {
                GetRESTHandle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to connect to REST Server: " + ex.Message, CommonGlobals.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FillTree(e.Node);
        }

        private void FillTree(TreeNode selNode)
        {
            // Remove selected Tree node.
            RemoveDummyTreeNode(selNode);

            switch (selNode.ImageIndex)
            {
                case IMAGE_INDEX_SUCCESS:
                    {
                        PopulateBanks();
                        break;
                    }
                case IMAGE_INDEX_BANK:
                    {
                        PopulateAccountTypes(selNode);
                        break;
                    }
                case IMAGE_INDEX_ACCOUNTTYPE:
                    {
                        PopulateDocs(selNode);
                        break;
                    }
                default:
                    break;
            }
        }

        private void PopulateBanks()
        {
            try
            {
                List<Bank> banks = null;
                List<string> names = new List<string>();

                TreeNode banksnode = treeView1.Nodes[0];

                if (banksnode.Nodes.Count == 0)
                {
                    BankList list = new BankList() { Filter = "" };
                    banks = m_restManager.RESTHandle.Get(list);

                    foreach (Bank bank in banks)
                    {
                        names.Add(bank.Name);
                        TreeNode node = banksnode.Nodes.Add(bank.Name, bank.Name, IMAGE_INDEX_BANK, IMAGE_INDEX_BANK);
                        node.Tag = bank;
                    }
                }
                else
                {
                    // already added, let's grab objects from the tree.
                    foreach (TreeNode node in banksnode.Nodes)
                    {
                        Bank bank = node.Tag as Bank;
                        names.Add(bank.Name);
                    }
                }

                // Populate list view with names.
                FillChildren(names, IMAGE_INDEX_ACCOUNTTYPE);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to populate Banks: " + ex.Message, "DocumentHQ", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
        }


        private void PopulateAccountTypes(TreeNode treenode)
        {
            try
            {
                Bank bank = treenode.Tag as Bank;
                if (null == bank)
                    return;

                string bankID = bank.Id;
                List<string> names = new List<string>();
                if (treenode.Nodes.Count == 0)
                {
                    AccountTypeReadAll list = new AccountTypeReadAll() { BankID = bankID };
                    var accountTypes = m_restManager.RESTHandle.Get(list);

                    foreach (var accounttype in accountTypes)
                    {
                        names.Add(accounttype.Name);
                        TreeNode node = treenode.Nodes.Add(accounttype.Name, accounttype.Name, IMAGE_INDEX_ACCOUNTTYPE, IMAGE_INDEX_ACCOUNTTYPE);
                        node.Tag = accounttype;
                    }
                }
                else
                {
                    // already added, let's grab objects from the tree.
                    foreach (TreeNode node in treenode.Nodes)
                    {
                        string accountName = node.Tag as string;
                        names.Add(accountName);
                    }
                }

                // Populate list view with names.
                FillChildren(names, IMAGE_INDEX_ACCOUNTTYPE);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to populate Banks: " + ex.Message, "DocumentHQ", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
        }

        private void PopulateDocs(TreeNode treenode)
        {
            try
            {
                AccountType accounttype = treenode.Tag as AccountType;
                if (null == accounttype)
                    return;

                string accounttypeid = accounttype.Id;
                List<string> docs = accounttype.BaseDocumentNames;
                List<string> ids = accounttype.BaseDocumentIDs;
               // var names = new List<string>();
                if (null != docs)
                {

                    if (treenode.Nodes.Count == 0)
                    {
                        for (int i=0;i<docs.Count;i++)
                        {
                      //      names.Add(docs[i]);
                            TreeNode node = treenode.Nodes.Add(docs[i], docs[i], IMAGE_INDEX_DOCUMENT, IMAGE_INDEX_DOCUMENT);
                            node.Tag = ids[i];
                        }
                    }
                   

                }

                // Populate list view with names.
                FillChildren(docs, IMAGE_INDEX_DOCUMENT);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to populate Banks: " + ex.Message, "DocumentHQ", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
        }

        /// <summary>
        /// Fill List View with children.
        /// </summary>
        /// <param name="names"></param>
        /// <param name="imageIndex"></param>
        private void FillChildren(List<string> names, int imageIndex)
        {
            listView1.Items.Clear();

            listView1.Columns.Clear();
            listView1.Columns.Add("Name", "Name");
            for (int i = 0; (null != names) && i < names.Count; i++)
            {
                ListViewItem newitem = new ListViewItem(names[i]);
                newitem.ImageIndex = imageIndex;


                listView1.Items.Add(newitem);
            }

            listView1.Columns[0].Width = -2;
        }

        /// <summary>
        /// Add account type.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemAdd_Click(object sender, EventArgs e)
        {
            TreeNode selNode = treeView1.SelectedNode;
            if (null == selNode)
            {
                MessageBox.Show("Tree node not selected", "DocumentHQ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (null == selNode.Tag)
                return;

            if (selNode.Tag is Bank)
            {

                Bank bank = selNode.Tag as Bank;

                // Add account type.
                frmAddNode frm = new frmAddNode();
                if (DialogResult.Cancel == frm.ShowDialog(this))
                {
                    return;
                }

                // Get account type name.
                string accountTypeName = frm.textBoxName.Text;

                AccountType newtype = new AccountType { Name = accountTypeName, Id = ObjectId.GenerateNewId().ToString(), BankID = bank.Id };
                AccountTypeCreate create = new AccountTypeCreate() { Accounttype = newtype };
                try
                {
                    // Add account type.
                    m_restManager.RESTHandle.Post(create);

                    TreeNode newnode = selNode.Nodes.Add(accountTypeName, accountTypeName, IMAGE_INDEX_ACCOUNTTYPE, IMAGE_INDEX_ACCOUNTTYPE);
                    newnode.Tag = newtype;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to create Account Type: " + ex.Message, "DocumentHQ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else if (selNode.Tag is AccountType)
            {
                // Add document definition.
                addDocumentToolStripMenuItem_Click(this, e);
            }
        }

        private void addAccountTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Get the selected bank
            var selectedBank = treeView1.SelectedNode.Tag as Bank;
            if (selectedBank == null)
            {
                MessageBox.Show("No bank selected.");
                return;
            }
            //Show the add account dialog
            var frm = new frmAddNode("Add Account Type");
            if (frm.ShowDialog() == DialogResult.OK)
            {
                // Get account type name.
                string accountTypeName = frm.textBoxName.Text;

                AccountType newtype = new AccountType { Name = accountTypeName, Id = ObjectId.GenerateNewId().ToString(), BankID = selectedBank.Id };
                AccountTypeCreate create = new AccountTypeCreate() { Accounttype = newtype };
                try
                {
                    // Add account type.
                    m_restManager.RESTHandle.Post(create);

                    TreeNode newnode = treeView1.SelectedNode.Nodes.Add(accountTypeName, accountTypeName, IMAGE_INDEX_ACCOUNTTYPE, IMAGE_INDEX_ACCOUNTTYPE);
                    newnode.Tag = newtype;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to create Account Type: " + ex.Message, "DocumentHQ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
        }

        private void addDocumentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Get the selected bank
            var selectedAccountType = treeView1.SelectedNode.Tag as AccountType;
            if (selectedAccountType == null)
            {
                MessageBox.Show("No account type selected.");
                return;
            }
            //Show the add account dialog
            var frm = new frmAddDocument();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Document newDocument = CreateDocument(frm.textBoxName.Text, frm.DocumentContentBase64, ObjectId.GenerateNewId().ToString());
                    UpdateAccountType(newDocument.Id, newDocument.Name, selectedAccountType.Name);

                    TreeNode newnode = treeView1.SelectedNode.Nodes.Add(newDocument.Name, newDocument.Name, IMAGE_INDEX_DOCUMENT, IMAGE_INDEX_DOCUMENT);
                    newnode.Tag = newDocument.Id;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to create Docment: " + ex.Message, "DocumentHQ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
        }


        private void deleteDocumentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Get the selected bank
            var selectedDocument = treeView1.SelectedNode.Tag as string;
            if (selectedDocument == null)
            {
                MessageBox.Show("No document selected.");
                return;
            }
            try
            {
                DeleteDocument(selectedDocument);
                 
                treeView1.SelectedNode.Parent.Nodes.Remove(treeView1.SelectedNode);
                 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to delete Docment: " + ex.Message, "DocumentHQ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
          
        }


        private Document CreateDocument(string name, string base64Content, string id)
        {
            //Create a doc to represent this and associate with the account type
            Document newDocument = new Document { Name = name, Id = id };
            DocumentCreate documentCreate = new DocumentCreate
            {
                Document = newDocument,
                DocumentContentBase64 = base64Content
            };

            m_restManager.RESTHandle.Post(documentCreate);
            return newDocument;
        }

        private void DeleteDocument(string id)
        {
            //Create a doc to represent this and associate with the account type
            DocumentDelete documentDelete = new DocumentDelete
            {
                 ID = id
            };

            m_restManager.RESTHandle.Delete(documentDelete);
        
        }

        private void UpdateAccountType(string docId, string docName, string selectedAccountTypeName)
        {
            AccountTypeReadByName request = new AccountTypeReadByName { Name = selectedAccountTypeName };
            AccountType accountType = m_restManager.RESTHandle.Get(request);

            //set documents references in account type
            if (accountType.BaseDocumentIDs.IsNullOrEmpty())
            {
                accountType.BaseDocumentIDs = new List<string> { };
            }
            if (accountType.BaseDocumentNames.IsNullOrEmpty())
            {
                accountType.BaseDocumentNames = new List<string> { };
            }
            accountType.BaseDocumentIDs.Add(docId);
            accountType.BaseDocumentNames.Add(docName);

            //perform update on accountype with new references to documents just added
            AccountTypeUpdate accountTypeUpdate = new AccountTypeUpdate { Accounttype = accountType };
            m_restManager.RESTHandle.Put(accountTypeUpdate);
        }
    }
}
