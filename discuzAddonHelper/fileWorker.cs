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
using System.Linq;
using System.Text;
using System.IO;

namespace discuzAddonHelper
{
    /// <summary>
    /// 处理任务类
    /// </summary>
    class fileJob : IComparable
    {
        /// <summary>
        /// 新建处理任务类
        /// </summary>
        /// <param name="Start">替换开始位置</param>
        /// <param name="Length">替换字符数</param>
        /// <param name="Insert">在原位置插入字符</param>
        public fileJob(int Start, int Length, string Insert)
        {
            this._Start = Start;
            this._Length = Length;
            this._Insert = Insert;

            this._End = Start + Length;
        }
        #region 参数声明
        int _Start;
        /// <summary>
        /// 替换开始位置
        /// </summary>
        public int Start
        {
            get { return _Start; }
        }

        int _Length;
        /// <summary>
        /// 替换字符数
        /// </summary>
        public int Length
        {
            get { return _Length; }
        }

        int _End;
        /// <summary>
        /// 替换结束位置
        /// </summary>
        public int End
        {
            get { return _End; }
        }

        string _Insert;

        Encoding big5 = Encoding.GetEncoding("BIG5");
        Encoding gbk = Encoding.GetEncoding("GBK");
        Encoding utf8 = Encoding.UTF8;
        #endregion

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="stringBuilder">新文件 stringBuilder</param>
        /// <param name="content">原文件内容</param>
        /// <param name="lastEnd">上次替换结束位置</param>
        /// <returns>替换结束位置</returns>
        public int Do(ref StringBuilder stringBuilder, ref string content, int lastEnd)
        {
            stringBuilder.Append(content.Substring(lastEnd, _Start - lastEnd))
                .Append(_Insert);
            return _End;
        } 

        /// <summary>
        /// 执行任务并转码
        /// </summary>
        /// <param name="stringBuilder">新文件 stringBuilder</param>
        /// <param name="content">原文件内容</param>
        /// <param name="lastEnd">上次替换结束位置</param>
        /// <returns>替换结束位置</returns>
        public int Convert(
            ref Dictionary<string, StringBuilder> stringBuilder,
            ref string content, int lastEnd)
        {
            string Behind = content.Substring(lastEnd, _Start - lastEnd);
            stringBuilder["SC_GBK"].Append(Behind)
                .Append(_Insert);

            stringBuilder["SC_UTF8"].Append(Behind)
                .Append(_Insert);

            Behind = chineseConverter.Do(Behind);
            string TC_Insert = chineseConverter.Do(_Insert);
            stringBuilder["TC_BIG5"].Append(Behind)
                .Append(TC_Insert);

            stringBuilder["TC_UTF8"].Append(Behind)
                .Append(TC_Insert);
            return _End;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            fileJob _fileJob = obj as fileJob;
            if (_fileJob == null)
                throw new ArgumentException("指定的类型不匹配 (_fileJob)");

            if (_End <= _fileJob.Start)
            {
                return -1;
            }
            else if (_Start >= _fileJob.End)
            {
                return 1;
            }
            else
            {
                throw new Exception("处理位置出现重叠");
            }
        }
    }

    /// <summary>
    /// 文件处理类
    /// </summary>
    class fileWorker
    {

        FileInfo f;
        string content = null;
        ArrayList Jobs = new ArrayList();

        /// <summary>
        /// 新建文件处理类
        /// </summary>
        /// <param name="FullName">文件位置</param>
        public fileWorker(string FullName)
        {
            f = new FileInfo(FullName);
            if (f.Exists)
            {
                FileStream fs = f.OpenRead();
                StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("GBK"));
                content = sr.ReadToEnd();
                sr.Close();
                fs.Close();
            }
        }

        /// <summary>
        /// 增加处理任务
        /// </summary>
        /// <param name="Start">替换开始位置</param>
        /// <param name="Length">替换字符数</param>
        public void AddJob(int Start, int Length)
        {
            AddJob(Start, Length, "");
        }

        /// <summary>
        /// 增加处理任务
        /// </summary>
        /// <param name="Start">替换开始位置</param>
        /// <param name="Length">替换字符数</param>
        /// <param name="Insert">在原位置插入字符</param>
        public void AddJob(int Start, int Length, string Insert)
        {
            if (Start + Length > content.Length)
            {
                throw new Exception("处理位置超出文件范围");
            }
            Jobs.Add(new fileJob(Start, Length, Insert));
        }

        /// <summary>
        /// 执行文件处理任务
        /// </summary>
        public void Do()
        {
            Jobs.Sort();

            StringBuilder stringBuilder = new StringBuilder();
            int lastEnd = 0;

            foreach (fileJob fJ in Jobs)
            {
                lastEnd = fJ.Do(ref stringBuilder, ref content, lastEnd);
            }

            if (lastEnd < content.Length) // 将最后的内容写回文件
            {
                stringBuilder.Append(content.Substring(lastEnd));
            }

            f.Delete();

            FileStream fs = f.Create();
            StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("GBK"));

            sw.Write(stringBuilder.ToString());
            sw.Close();
            fs.Close();

            stringBuilder.Clear();
            Jobs.Clear();

            content = null;
        }

        /// <summary>
        /// 执行文件处理任务并转码
        /// </summary>
        public void Convert()
        {
            Jobs.Sort();
            string[] character = { "SC_GBK", "SC_UTF8", "TC_UTF8", "TC_BIG5" };
            Dictionary<string, StringBuilder> stringBuilder = new Dictionary<string, StringBuilder>();
            foreach (string c1 in character)
            {
                stringBuilder[c1] = new StringBuilder();
            }
            int lastEnd = 0;

            foreach (fileJob fJ in Jobs)
            {
                lastEnd = fJ.Convert(
                    ref stringBuilder,
                    ref content, lastEnd);
            }

            if (lastEnd < content.Length) // 将最后的内容写回文件
            {
                string Last = content.Substring(lastEnd);
                foreach (string c2 in character)
                {
                    stringBuilder[c2].Append(Last);
                }
            }

            f.Delete();

            FileStream fs;
            StreamWriter sw;
            string fileName = f.FullName.Substring(0, f.FullName.LastIndexOf('.'));
            Encoding enc;
            foreach (string c3 in character)
            {
                switch (c3)
                {
                    case "SC_GBK":
                        enc = Encoding.GetEncoding("GBK");
                        break;
                    case "TC_BIG5":
                        enc = Encoding.GetEncoding("BIG5");
                        break;
                    default:
                        enc = Encoding.UTF8;
                        break;
                }

                fs = new FileStream(fileName + "_" + c3 + f.Extension, FileMode.Create);
                sw = new StreamWriter(fs, enc);
                sw.Write(stringBuilder[c3]);
                sw.Close();
                fs.Close();
            }

            stringBuilder.Clear();
            Jobs.Clear();

            content = null;
        }

        /// <summary>
        /// 获取文件载入状态
        /// </summary>
        public bool Loaded
        {
            get
            {
                return content == null;
            }
        }
    }
}
