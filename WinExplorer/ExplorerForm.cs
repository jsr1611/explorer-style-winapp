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

namespace WinExplorer
{
    public partial class ExplorerForm : Form
    {
        public ExplorerForm()
        {
            InitializeComponent();
        }

        private void ExplorerForm_Load(object sender, EventArgs e)
        {
            ViewToolStripComboBox.SelectedIndex = 0;
            listView1.Columns.Add("Name", 250);
            listView1.Columns.Add("Date modified", 150);
            listView1.Columns.Add("Size", 75, HorizontalAlignment.Right);


            var docs = new TreeNode("My Documents");
            docs.Tag = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            treeView1.Nodes.Add(docs);

            GetFolders(docs);
            docs.Expand();
        }
        private void GetFolders(TreeNode node)
        {
            var dir = new DirectoryInfo(node.Tag.ToString());

            try
            {
                foreach (var childDir in dir.GetDirectories())
                {
                    if (childDir.Attributes.HasFlag(FileAttributes.Hidden) == true)
                    {
                        continue;
                    }
                    var childNode = new TreeNode(childDir.Name);
                    childNode.Tag = childDir.FullName;
                    node.Nodes.Add(childNode);

                    GetFolders(childNode);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }


        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var dir = new DirectoryInfo(e.Node.Tag.ToString());

            listView1.Items.Clear();
            SmallImageList.Images.RemoveByKey(".exe");
            LargeImageList.Images.RemoveByKey(".exe");

            foreach (var thisFile in dir.GetFiles())
            {
                var item = new ListViewItem(thisFile.Name);
                var lastwrite = thisFile.LastWriteTime;
                item.SubItems.Add(lastwrite.ToShortDateString() + " " + lastwrite.ToShortTimeString());
                item.SubItems.Add(Math.Ceiling((decimal)thisFile.Length / 1024) + " KB");
                if (!LargeImageList.Images.ContainsKey(thisFile.Extension))
                {
                    var thisIcon = Icon.ExtractAssociatedIcon(thisFile.FullName);
                    SmallImageList.Images.Add(thisFile.Extension, thisIcon);
                    LargeImageList.Images.Add(thisFile.Extension, thisIcon);
                }
                item.ImageKey = thisFile.Extension;
                listView1.Items.Add(item);
            }
        }

        private void ViewToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(ViewToolStripComboBox.Text)
            {
                case "LargeIcons" :
                    listView1.View = View.LargeIcon;
                    break;
                case "Small Icons" :
                    listView1.View = View.SmallIcon;
                    break;
                case "Details":
                    listView1.View = View.Details;
                    break;
            }
        }
    }
}
