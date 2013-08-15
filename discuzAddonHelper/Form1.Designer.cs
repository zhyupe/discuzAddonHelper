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
namespace discuzAddonHelper
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this._b_指定应用目录 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this._l_应用目录 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this._t_树形图 = new System.Windows.Forms.TreeView();
            this._b_检查文件 = new System.Windows.Forms.Button();
            this._b_四种编码生成 = new System.Windows.Forms.Button();
            this._b_提取语言文件 = new System.Windows.Forms.Button();
            this._b_应用语言文件 = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this._b_导出语言文件 = new System.Windows.Forms.Button();
            this._b_载入语言文件 = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this._t_语言包 = new System.Windows.Forms.ListView();
            this.english = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chinese = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._t_Log = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // _b_指定应用目录
            // 
            this._b_指定应用目录.Dock = System.Windows.Forms.DockStyle.Right;
            this._b_指定应用目录.Location = new System.Drawing.Point(495, 0);
            this._b_指定应用目录.Name = "_b_指定应用目录";
            this._b_指定应用目录.Size = new System.Drawing.Size(75, 23);
            this._b_指定应用目录.TabIndex = 0;
            this._b_指定应用目录.Text = "指定 ...";
            this._b_指定应用目录.UseVisualStyleBackColor = true;
            this._b_指定应用目录.Click += new System.EventHandler(this._b_指定应用目录_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this._l_应用目录);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this._b_指定应用目录);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.panel1.Size = new System.Drawing.Size(570, 35);
            this.panel1.TabIndex = 1;
            // 
            // _l_应用目录
            // 
            this._l_应用目录.Dock = System.Windows.Forms.DockStyle.Fill;
            this._l_应用目录.Location = new System.Drawing.Point(64, 0);
            this._l_应用目录.Name = "_l_应用目录";
            this._l_应用目录.Size = new System.Drawing.Size(431, 23);
            this._l_应用目录.TabIndex = 2;
            this._l_应用目录.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 23);
            this.label1.TabIndex = 1;
            this.label1.Text = "应用目录";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _t_树形图
            // 
            this._t_树形图.CheckBoxes = true;
            this._t_树形图.Dock = System.Windows.Forms.DockStyle.Fill;
            this._t_树形图.Location = new System.Drawing.Point(0, 0);
            this._t_树形图.Name = "_t_树形图";
            this._t_树形图.Size = new System.Drawing.Size(377, 160);
            this._t_树形图.TabIndex = 2;
            this._t_树形图.TabStop = false;
            this._t_树形图.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.StaffSelect);
            // 
            // _b_检查文件
            // 
            this._b_检查文件.Location = new System.Drawing.Point(3, 15);
            this._b_检查文件.Name = "_b_检查文件";
            this._b_检查文件.Size = new System.Drawing.Size(75, 23);
            this._b_检查文件.TabIndex = 3;
            this._b_检查文件.Text = "检查文件";
            this._b_检查文件.UseVisualStyleBackColor = true;
            this._b_检查文件.Click += new System.EventHandler(this._b_检查文件_Click);
            // 
            // _b_四种编码生成
            // 
            this._b_四种编码生成.Location = new System.Drawing.Point(84, 15);
            this._b_四种编码生成.Name = "_b_四种编码生成";
            this._b_四种编码生成.Size = new System.Drawing.Size(90, 23);
            this._b_四种编码生成.TabIndex = 4;
            this._b_四种编码生成.Text = "四种编码生成";
            this._b_四种编码生成.UseVisualStyleBackColor = true;
            this._b_四种编码生成.Click += new System.EventHandler(this._b_四种编码生成_Click);
            // 
            // _b_提取语言文件
            // 
            this._b_提取语言文件.Location = new System.Drawing.Point(192, 0);
            this._b_提取语言文件.Name = "_b_提取语言文件";
            this._b_提取语言文件.Size = new System.Drawing.Size(90, 23);
            this._b_提取语言文件.TabIndex = 5;
            this._b_提取语言文件.Text = "提取语言文件";
            this._b_提取语言文件.UseVisualStyleBackColor = true;
            this._b_提取语言文件.Click += new System.EventHandler(this._b_提取语言文件_Click);
            // 
            // _b_应用语言文件
            // 
            this._b_应用语言文件.Location = new System.Drawing.Point(96, 0);
            this._b_应用语言文件.Name = "_b_应用语言文件";
            this._b_应用语言文件.Size = new System.Drawing.Size(90, 23);
            this._b_应用语言文件.TabIndex = 6;
            this._b_应用语言文件.Text = "应用语言文件";
            this._b_应用语言文件.UseVisualStyleBackColor = true;
            this._b_应用语言文件.Click += new System.EventHandler(this._b_应用语言文件_Click);
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "请选择应用目录。应用目录须为应用的唯一标识符";
            this.folderBrowserDialog1.ShowNewFolderButton = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this._b_检查文件);
            this.panel2.Controls.Add(this._b_四种编码生成);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(12, 284);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.panel2.Size = new System.Drawing.Size(570, 41);
            this.panel2.TabIndex = 7;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this._b_导出语言文件);
            this.panel3.Controls.Add(this._b_载入语言文件);
            this.panel3.Controls.Add(this._b_提取语言文件);
            this.panel3.Controls.Add(this._b_应用语言文件);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(189, 15);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(378, 23);
            this.panel3.TabIndex = 7;
            // 
            // _b_导出语言文件
            // 
            this._b_导出语言文件.Location = new System.Drawing.Point(288, 0);
            this._b_导出语言文件.Name = "_b_导出语言文件";
            this._b_导出语言文件.Size = new System.Drawing.Size(90, 23);
            this._b_导出语言文件.TabIndex = 7;
            this._b_导出语言文件.Text = "导出语言文件";
            this._b_导出语言文件.UseVisualStyleBackColor = true;
            this._b_导出语言文件.Click += new System.EventHandler(this._b_导出语言文件_Click);
            // 
            // _b_载入语言文件
            // 
            this._b_载入语言文件.Location = new System.Drawing.Point(0, 0);
            this._b_载入语言文件.Name = "_b_载入语言文件";
            this._b_载入语言文件.Size = new System.Drawing.Size(90, 23);
            this._b_载入语言文件.TabIndex = 6;
            this._b_载入语言文件.Text = "载入语言文件";
            this._b_载入语言文件.UseVisualStyleBackColor = true;
            this._b_载入语言文件.Click += new System.EventHandler(this._b_载入语言文件_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(12, 47);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this._t_Log);
            this.splitContainer1.Size = new System.Drawing.Size(570, 237);
            this.splitContainer1.SplitterDistance = 160;
            this.splitContainer1.TabIndex = 8;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this._t_语言包);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this._t_树形图);
            this.splitContainer2.Size = new System.Drawing.Size(570, 160);
            this.splitContainer2.SplitterDistance = 189;
            this.splitContainer2.TabIndex = 10;
            // 
            // _t_语言包
            // 
            this._t_语言包.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.english,
            this.chinese});
            this._t_语言包.Dock = System.Windows.Forms.DockStyle.Fill;
            this._t_语言包.Location = new System.Drawing.Point(0, 0);
            this._t_语言包.Name = "_t_语言包";
            this._t_语言包.Size = new System.Drawing.Size(189, 160);
            this._t_语言包.TabIndex = 0;
            this._t_语言包.UseCompatibleStateImageBehavior = false;
            this._t_语言包.View = System.Windows.Forms.View.Details;
            // 
            // english
            // 
            this.english.Text = "索引";
            // 
            // chinese
            // 
            this.chinese.Text = "内容";
            // 
            // _t_Log
            // 
            this._t_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this._t_Log.Location = new System.Drawing.Point(0, 0);
            this._t_Log.Multiline = true;
            this._t_Log.Name = "_t_Log";
            this._t_Log.Size = new System.Drawing.Size(570, 73);
            this._t_Log.TabIndex = 9;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Discuz! 应用配置文件|discuz_plugin_*.xml";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 337);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(510, 360);
            this.Name = "Form1";
            this.Padding = new System.Windows.Forms.Padding(12);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Discuz 应用编码转换助手";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _b_指定应用目录;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label _l_应用目录;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TreeView _t_树形图;
        private System.Windows.Forms.Button _b_检查文件;
        private System.Windows.Forms.Button _b_四种编码生成;
        private System.Windows.Forms.Button _b_提取语言文件;
        private System.Windows.Forms.Button _b_应用语言文件;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button _b_载入语言文件;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView _t_语言包;
        private System.Windows.Forms.ColumnHeader english;
        private System.Windows.Forms.ColumnHeader chinese;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TextBox _t_Log;
        private System.Windows.Forms.Button _b_导出语言文件;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}

