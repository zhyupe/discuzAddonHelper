using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace discuzAddonHelper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void StaffSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.ByMouse && e.Action != TreeViewAction.ByKeyboard)
                return;

            SetChildChecked(e.Node);  // 判断是否是根节点             
            if (e.Node.Parent != null)
            {
                SetParentChecked(e.Node);
            }
        }
        /// <summary>    
        /// 根据子节点状态设置父节点的状态   
        /// </summary>          
        /// <param name="childNode"></param>     
        private void SetParentChecked(TreeNode childNode)
        {
            TreeNode parentNode = childNode.Parent;
            if (!parentNode.Checked && childNode.Checked)
            {
                int ichecks = 0;
                for (int i = 0; i < parentNode.GetNodeCount(false); i++)
                {
                    TreeNode node = parentNode.Nodes[i];
                    if (node.Checked) { ichecks++; }
                }
                if (ichecks == parentNode.GetNodeCount(false))
                {
                    parentNode.Checked = true;
                    if (parentNode.Parent != null)
                    {
                        SetParentChecked(parentNode);
                    }
                }
            }
            else if (parentNode.Checked && !childNode.Checked) { parentNode.Checked = false; }
        }
        /// <summary>     
        /// 根据父节点状态设置子节点的状态   
        /// </summary>          
        /// <param name="parentNode"></param>      
        private void SetChildChecked(TreeNode parentNode)
        {
            for (int i = 0; i < parentNode.GetNodeCount(false); i++)
            {
                TreeNode node = parentNode.Nodes[i];
                node.Checked = parentNode.Checked;
                if (node.GetNodeCount(false) > 0)
                {
                    SetChildChecked(node);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            _l_插件目录.Text = folderBrowserDialog1.SelectedPath;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (_l_插件目录.Text == "")
            {
                MessageBox.Show("请指定应用目录");
                return;
            }

            _t_树形图.Nodes.Clear();
            _t_树形图.Nodes.Add("加载中, 请稍候 ...");

            (new Thread((new treeBuilder(new DirectoryInfo(_l_插件目录.Text), _t_树形图.Nodes, new AddNodeDele(AddNode)).Work))).Start();
        }

        public delegate TreeNodeCollection AddNodeDele(TreeNodeCollection tree, string key, string value, string tag);
        TreeNodeCollection AddNode(TreeNodeCollection tree, string key, string value, string tag)
        {
            if (this.InvokeRequired == false)
            {
                if (key == null)
                {
                    tree.Add(value);
                }
                else
                {
                    tree.Add(key, value);
                }
                
                tree[tree.Count - 1].Tag = tag;

                return tree[tree.Count - 1].Nodes;
            }
            else
            {
                AddNodeDele addNode = new AddNodeDele(AddNode);
                return (TreeNodeCollection)this.Invoke(addNode, tree, key, value, tag);
            }
        }
    }
}