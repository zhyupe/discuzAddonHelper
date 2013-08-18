/* * 
 * Copyright 2010-2013 DianFen Network
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Xml;

namespace discuzAddonHelper
{
    public partial class Form1 : Form
    {
        private string iden = "";
        [Flags]
        private enum _StatusEnum
        {
            /// <summary>
            /// 正在进行处理
            /// </summary>
            Working = 1,
            /// <summary>
            /// 已经进行过文件检查
            /// </summary>
            Checked = 2,
        }
        private _StatusEnum _Status = 0;
        
        /// <summary>
        /// 锁定
        /// </summary>
        private _StatusEnum Status
        {
            get
            {
                return _Status;
            }
            set
            {
                _Status = value;
                _b_指定应用目录.Enabled = (value & _StatusEnum.Working) == 0;
                _b_检查文件.Enabled = _b_四种编码生成.Enabled = _b_提取语言文件.Enabled = _b_应用语言文件.Enabled = _b_载入语言文件.Enabled = _b_导出语言文件.Enabled
                    = value == _StatusEnum.Checked;
            }
        }


        private delegate void LogDelegate(string log);
        private static LogDelegate _LogDelegate;

        public static void Log(string log)
        {
            _LogDelegate(log);
        }

        private void _Log(string log)
        {
            if (this.InvokeRequired == false)
            {
                _t_Log.AppendText(string.Concat("[", DateTime.Now, "] ", log, "\r\n"));
            }
            else
            {
                this.Invoke(_LogDelegate, log);
            }
        }

        public Form1()
        {
            InitializeComponent();

            langDictionary.AddLanguage = new AddLanguageDele(AddLanguage);
            _LogDelegate = new LogDelegate(_Log);

            Status = 0;
        }

        #region TreeView 选中辅助
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
        #endregion

        #region 指定应用目录
        private void _b_指定应用目录_Click(object sender, EventArgs e)
        {
            if ((Status & _StatusEnum.Working) != 0)
            {
                MessageBox.Show("有其它任务正在执行");
                return;
            }

            Status = Status | _StatusEnum.Working;
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _l_应用目录.Text = openFileDialog1.InitialDirectory = saveFileDialog1.InitialDirectory = folderBrowserDialog1.SelectedPath;

                _t_树形图.Nodes.Clear();
                _t_语言包.Items.Clear();
                langDictionary.Clear();

                Form1.Log("提示: 已将应用目录指定为: " + _l_应用目录.Text);
                Form1.Log("提示: Form1.指定应用目录 正在检查文件");
                _b_检查文件_Work(new WorkFinishDele(_b_指定应用目录_Finish));
            }
            else
            {
                Status = Status & (~_StatusEnum.Working);
            }
        }
        private void _b_指定应用目录_Finish()
        {
            if (this.InvokeRequired == false)
            {
                _t_树形图.Nodes[0].Remove();
                Form1.Log("进程: Form1.指定应用目录 执行完毕");
                Form1.Log("提示: 如果应用已经进行过语言包提取，请先导入原语言包！");
                Status = (Status & (~_StatusEnum.Working)) | _StatusEnum.Checked;
            }
            else
            {
                WorkFinishDele workFinish = new WorkFinishDele(_b_指定应用目录_Finish);
                this.Invoke(workFinish);
            }
        }
        #endregion
        #region 检查文件
        private void _b_检查文件_Click(object sender, EventArgs e)
        {

            if ((Status & _StatusEnum.Working) != 0)
            {
                MessageBox.Show("有其它任务正在执行");
                return;
            }
            else if (_l_应用目录.Text == "")
            {
                MessageBox.Show("请指定应用目录");
                return;
            }
            else
            {
                Status = Status | _StatusEnum.Working;
                Form1.Log("进程: Form1.检查文件 开始执行");
                _b_检查文件_Work(new WorkFinishDele(_b_检查文件_Finish));
            }
        }

        private void _b_检查文件_Work(WorkFinishDele workFinish)
        {
            _t_树形图.Nodes.Clear();
            _t_树形图.Nodes.Add("加载中, 请稍候 ...");

            DirectoryInfo di = new DirectoryInfo(_l_应用目录.Text);
            iden = di.Name;
            saveFileDialog1.Filter = "Discuz! 开发语言包|*.lang.php";

            AddNode(_t_树形图.Nodes, di.Name, "[F]" + di.Name, "F|" + di.FullName);
            (new Thread(new treeBuilder(di, _t_树形图.Nodes[1].Nodes, new AddNodeDele(AddNode), workFinish).Work)).Start();
        }

        private void _b_检查文件_Finish()
        {
            if (this.InvokeRequired == false)
            {
                _t_树形图.Nodes[0].Remove();
                Form1.Log("进程: Form1.检查文件 执行完毕");
                Form1.Log("提示: 如果应用已经进行过语言包提取，请先导入原语言包！");
                Status = (Status & (~_StatusEnum.Working)) | _StatusEnum.Checked;
            }
            else
            {
                WorkFinishDele workFinish = new WorkFinishDele(_b_检查文件_Finish);
                this.Invoke(workFinish);
            }
        }
        #endregion
        #region 四种编码生成
        private void _b_四种编码生成_Click(object sender, EventArgs e)
        {
            Dictionary<string, ArrayList> CheckedItems;
            if (treeChecked(out CheckedItems))
            {
                Form1.Log("进程: Form1.四种编码生成 开始执行");
                (new Thread(_b_四种编码生成_Work)).Start(CheckedItems);
            }
        }
        private void _b_四种编码生成_Work(object treeToDo)
        {
            Dictionary<string, ArrayList> _treeToDo = (Dictionary<string, ArrayList>)treeToDo;

            fileWorker fW;
            string[] Configs;

            foreach (KeyValuePair<string, ArrayList> obj in _treeToDo)
            {
                fW = new fileWorker(obj.Key.Substring(2));
                foreach (string Config in obj.Value)
                {
                    if (Config[0] != 'C') // 只处理 C 标记
                        continue;

                    Configs = Config.Split('|');

                    if (obj.Key[0] == 'P')
                    {
                        if (Configs[3] == "F")
                        {
                            fW.AddJob(Convert.ToInt32(Configs[1]), Convert.ToInt32(Configs[2]),
                                "'" + Configs[6].Replace("'", "\\'") + "'");
                        }
                        else
                        {
                            // 因为是原样写回文件，所以只将包含的引号还原
                            // 同时在预处理时对引号进行了反转义，写回时重新转义
                            fW.AddJob(Convert.ToInt32(Configs[1]), Convert.ToInt32(Configs[2]),
                                (Configs[4] == "1" ? Configs[3] : "") +
                                Configs[6].Replace(Configs[3], "\\" + Configs[3]) + 
                                (Configs[5] == "1" ? Configs[3] : ""));
                        }
                    }
                    else
                    {
                        fW.AddJob(Convert.ToInt32(Configs[1]), Convert.ToInt32(Configs[2]), Configs[6]);
                    }
                }
                fW.Convert();
            }
            Form1.Log("提示: Form1.四种编码生成 正在重新检查文件");
            this.Invoke(new Action(() => { _b_检查文件_Work(new WorkFinishDele(_b_四种编码生成_Finish)); }));
        }
        private void _b_四种编码生成_Finish()
        {
            if (this.InvokeRequired == false)
            {
                _t_树形图.Nodes[0].Remove();
                Form1.Log("进程: Form1.四种编码生成 执行完毕");
                Status = Status & (~_StatusEnum.Working);
            }
            else
            {
                WorkFinishDele workFinish = new WorkFinishDele(_b_四种编码生成_Finish);
                this.Invoke(workFinish);
            }
        }
        #endregion
        #region 提取语言文件
        private void _b_提取语言文件_Click(object sender, EventArgs e)
        {
            Dictionary<string, ArrayList> CheckedItems;
            if (treeChecked(out CheckedItems))
            {
                Form1.Log("进程: Form1.提取语言文件 开始执行");
                (new Thread(_b_提取语言文件_Work)).Start(CheckedItems);
            }
        }
        private void _b_提取语言文件_Work(object treeToDo)
        {
            Dictionary<string, ArrayList> _treeToDo = (Dictionary<string, ArrayList>)treeToDo;

            string[] Configs;
            foreach (KeyValuePair<string, ArrayList> obj in _treeToDo)
            {
                if (obj.Key[0] == 'X')  // 不处理 XML
                    continue;

                foreach (string Config in obj.Value)
                {
                    if (Config[0] != 'C') // 只处理 C 标记
                        continue;

                    Configs = Config.Split('|');
                    langDictionary.AddNoKey(Configs[6], obj.Key[0] == 'P' ? "P" : "H");
                }
            }

            this.Invoke(new Action(() => { _t_语言包.Items.Clear(); }));
            langDictionary.Update();

            fileWorker fW;
            foreach (KeyValuePair<string, ArrayList> obj in _treeToDo)
            {
                if (obj.Key[0] == 'X')  // 不处理 XML
                    continue;

                fW = new fileWorker(obj.Key.Substring(2));
                foreach (string Config in obj.Value)
                {
                    Configs = Config.Split('|');
                    switch (Configs[0])
                    {
                        case "D": // 这个还好吧
                            fW.AddJob(Convert.ToInt32(Configs[1]), Convert.ToInt32(Configs[2]));
                            break;
                        case "L": // 一般般，后面写过了
                            if (obj.Key[0] == 'P')
                            {
                                fW.AddJob(Convert.ToInt32(Configs[1]), Convert.ToInt32(Configs[2]), 
                                    "lang('plugin/" + iden + "', '" + langDictionary.GetEnglish(Configs[3], "P") + "')");
                            }
                            else
                            {
                                fW.AddJob(Convert.ToInt32(Configs[1]), Convert.ToInt32(Configs[2]),
                                    "{lang " + iden + ":" + langDictionary.GetEnglish(Configs[3], "H") + "}");
                            }
                            break;
                        case "C": // 这块处理真心麻烦 ……
                            if (obj.Key[0] == 'P')
                            {
                                //C|起始位置|长度|前（后）缀符号或 F（函数调用）|是否去掉前缀|是否去掉后缀|匹配内容
                                if (Configs[3] == "F")
                                {
                                    fW.AddJob(Convert.ToInt32(Configs[1]), Convert.ToInt32(Configs[2]),
                                        "'" + iden + ":" + langDictionary.AddNoKey(Configs[6], "P") + "'");
                                }
                                else
                                {
                                    // 包含的引号说明前（后）面没有需要连接的内容
                                    // 所以仅需对不包含引号的增加连接符和引号
                                    // 同时在预处理时对引号进行了反转义，写回时重新转义
                                    fW.AddJob(Convert.ToInt32(Configs[1]), Convert.ToInt32(Configs[2]),
                                        (Configs[4] == "1" ? "" : Configs[3] + " . ") +
                                        "lang('plugin/" + iden + "', '" + langDictionary.AddNoKey(Configs[6], "P") + "')" +
                                        (Configs[5] == "1" ? "" : " . " + Configs[3]));
                                }
                            }
                            else
                            {
                                fW.AddJob(Convert.ToInt32(Configs[1]), Convert.ToInt32(Configs[2]),
                                    "{lang " + iden + ":" + langDictionary.AddNoKey(Configs[6], "H") + "}");
                            }
                            break;
                    }
                }
                fW.Do();
            }
            Form1.Log("提示: Form1.提取语言文件 正在重新检查文件");
            this.Invoke(new Action(() => { _b_检查文件_Work(new WorkFinishDele(_b_提取语言文件_Finish)); }));
        }
        private void _b_提取语言文件_Finish()
        {
            if (this.InvokeRequired == false)
            {
                _t_树形图.Nodes[0].Remove();
                Form1.Log("进程: Form1.提取语言文件 执行完毕");
                Status = Status & (~_StatusEnum.Working);
            }
            else
            {
                WorkFinishDele workFinish = new WorkFinishDele(_b_提取语言文件_Finish);
                this.Invoke(workFinish);
            }
        }
        #endregion
        #region 应用语言文件
        private void _b_应用语言文件_Click(object sender, EventArgs e)
        {
            Dictionary<string, ArrayList> CheckedItems;
            if (_t_语言包.Items.Count == 0)
            {
                MessageBox.Show("请先导入或提取语言包");
            }
            else if (treeChecked(out CheckedItems))
            {
                Form1.Log("进程: Form1.应用语言文件 开始执行");
                (new Thread(_b_应用语言文件_Work)).Start(CheckedItems);
            }
        }
        private void _b_应用语言文件_Work(object treeToDo)
        {
            Dictionary<string, ArrayList> _treeToDo = (Dictionary<string, ArrayList>)treeToDo;

            fileWorker fW;
            string[] Configs;

            foreach (KeyValuePair<string, ArrayList> obj in _treeToDo)
            {
                if (obj.Key[0] == 'X') // 不处理 XML
                    continue;

                fW = new fileWorker(obj.Key.Substring(2));
                foreach (string Config in obj.Value)
                {
                    if (Config[0] != 'L') // 只处理 L 标记
                        continue;

                    Configs = Config.Split('|');

                    // 这个标记的处理出乎意料的简单呢
                    if (obj.Key[0] == 'P')
                    {
                        fW.AddJob(Convert.ToInt32(Configs[1]), Convert.ToInt32(Configs[2]),
                            "'" + langDictionary.GetChinese(Configs[3], "P").Replace("'", "\\'") + "'");
                    }
                    else
                    {
                        fW.AddJob(Convert.ToInt32(Configs[1]), Convert.ToInt32(Configs[2]),
                            langDictionary.GetChinese(Configs[3], "H"));
                    }
                }
                fW.Do();
            }
            Form1.Log("提示: Form1.应用语言文件 正在重新检查文件");
            this.Invoke(new Action(() => { _b_检查文件_Work(new WorkFinishDele(_b_应用语言文件_Finish)); }));
        }
        private void _b_应用语言文件_Finish()
        {
            if (this.InvokeRequired == false)
            {
                _t_树形图.Nodes[0].Remove();
                Form1.Log("进程: Form1.应用语言文件 执行完毕");
                Status = Status & (~_StatusEnum.Working);
            }
            else
            {
                WorkFinishDele workFinish = new WorkFinishDele(_b_应用语言文件_Finish);
                this.Invoke(workFinish);
            }
        }
        #endregion
        #region 载入语言文件
        private void _b_载入语言文件_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Status = Status | _StatusEnum.Working;
                Form1.Log("进程: Form1.载入语言文件 开始执行");

                _t_语言包.Items.Clear();
                langDictionary.Clear();

                XmlDocument doc = new XmlDocument();
                doc.Load(openFileDialog1.FileName);

                Encoding gbk = Encoding.GetEncoding("GBK");
                Encoding iso = Encoding.GetEncoding("ISO-8859-1");
                XmlNodeList listNodes = null;

                listNodes = doc.SelectNodes("/root/item[@id='Data']/item[@id='language']/item[@id='scriptlang']/item");
                foreach (XmlNode node in listNodes)
                {
                    langDictionary.Add(node.Attributes["id"].Value, gbk.GetString(iso.GetBytes(node.InnerText)), "P");
                }

                listNodes = doc.SelectNodes("/root/item[@id='Data']/item[@id='language']/item[@id='templatelang']/item");
                foreach (XmlNode node in listNodes)
                {
                    langDictionary.Add(node.Attributes["id"].Value, gbk.GetString(iso.GetBytes(node.InnerText)), "H");
                }

                Form1.Log("进程: Form1.载入语言文件 执行完毕");
                Status = Status & (~_StatusEnum.Working);
            }
        }
        #endregion
        #region 导出语言文件
        private void _b_导出语言文件_Click(object sender, EventArgs e)
        {
            if ((Status & _StatusEnum.Working) != 0)
            {
                MessageBox.Show("有其它任务正在执行");
            }
            else if ((Status & _StatusEnum.Checked) == 0 || _t_树形图.Nodes.Count == 0)
            {
                MessageBox.Show("文件检查未完成或检查结果为空");
            }
            else if (_t_语言包.Items.Count == 0)
            {
                MessageBox.Show("请先导入或提取语言包");
            }
            else if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Status = Status | _StatusEnum.Working;
                Form1.Log("进程: Form1.导出语言文件 开始执行");

                Dictionary<string, string> _P, _H;
                langDictionary.Export(out _P, out _H);

                FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("GBK"));
                sw.Write("<?php\n\n$scriptlang['" + iden + "'] = array(\n");
                foreach (KeyValuePair<string, string> obj in _P)
                {
                    sw.Write("    '" + obj.Key + "' => '" + obj.Value.Replace("'", "\\'") + "',\n");
                }
                sw.Write(");\n\n$templatelang['" + iden + "'] = array(\n");
                foreach (KeyValuePair<string, string> obj in _H)
                {
                    sw.Write("    '" + obj.Key + "' => '" + obj.Value.Replace("'", "\\'") + "',\n");
                }
                sw.Write(");\n\n$installlang['" + iden + "'] = array(\n    // Install Lang Here\n);");
                sw.Close();
                fs.Close();

                Form1.Log("进程: Form1.导出语言文件 执行完毕");
                Status = Status & (~_StatusEnum.Working);
            }
        }
        #endregion

        #region Work 辅助函数
        /// <summary>
        /// 读取选择项目
        /// </summary>
        /// <param name="CheckedItems">Dictionary 文件类型|文件位置, 选中项 Tag</param>
        /// <returns>是否继续执行</returns>
        private bool treeChecked(out Dictionary<string, ArrayList> CheckedItems)
        {
            if ((Status & _StatusEnum.Working) != 0)
            {
                MessageBox.Show("有其它任务正在执行");
            } else if ((Status & _StatusEnum.Checked) == 0 || _t_树形图.Nodes.Count == 0)
            {
                MessageBox.Show("文件检查未完成或检查结果为空");
            }
            else
            {
                Status = Status | _StatusEnum.Working;
                CheckedItems = treeChecked(_t_树形图.Nodes);
                if (CheckedItems.Count == 0)
                {
                    Status = Status & (~_StatusEnum.Working);
                    MessageBox.Show("您没有选中任何项目");
                }
                else
                {
                    return true;
                }
            }

            CheckedItems = null;
            return false;
        }

        /// <summary>
        /// 读取选择项目
        /// </summary>
        /// <param name="Tree">要读取的 TreeNodeCollection</param>
        /// <returns>Dictionary 文件类型|文件位置, 选中项 Tag</returns>
        private Dictionary<string, ArrayList> treeChecked(TreeNodeCollection Tree)
        {
            Dictionary<string, ArrayList> CheckedItems = new Dictionary<string, ArrayList>();
            Dictionary<string, ArrayList> CheckedItems2;

            string Tag = "";
            foreach (TreeNode treeNode in Tree)
            {
                if (treeNode.Nodes.Count == 0)
                    continue;

                Tag = (string)treeNode.Tag;

                switch (Tag[0])
                {
                    case 'F':
                        CheckedItems2 = treeChecked(treeNode.Nodes);
                        if (CheckedItems2.Count != 0)
                        {
                            foreach (KeyValuePair<string, ArrayList> obj in CheckedItems2)
                            {
                                CheckedItems[obj.Key] = obj.Value;
                            }
                        }
                        break;
                    case 'P':
                    case 'H':
                    case 'X':
                        CheckedItems[Tag] = new ArrayList();

                        foreach (TreeNode subTreeNode in treeNode.Nodes)
                        {
                            if (subTreeNode.Checked)
                                CheckedItems[Tag].Add(subTreeNode.Tag);
                        }

                        if (CheckedItems[Tag].Count == 0)
                            CheckedItems.Remove(Tag);

                        break;
                }
            }

            return CheckedItems;
        }

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
        void AddLanguage(string English, string Chinese)
        {
            if (this.InvokeRequired == false)
            {
                ListViewItem item = new ListViewItem(English);
                item.SubItems.Add(Chinese);

                _t_语言包.Items.Add(item);
            }
            else
            {
                AddLanguageDele addLanguage = new AddLanguageDele(AddLanguage);
                this.Invoke(addLanguage, English, Chinese);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            (new License()).ShowDialog();
            Log("程序载入完成");
        }
        #endregion
    }
}