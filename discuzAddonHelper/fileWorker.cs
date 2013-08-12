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
        /// <param name="stringBuilder_SC_GBK">新文件 stringBuilder (SC_GBK)</param>
        /// <param name="stringBuilder_SC_UTF8">新文件 stringBuilder (SC_UTF8)</param>
        /// <param name="stringBuilder_TC_UTF8">新文件 stringBuilder (TC_UTF8)</param>
        /// <param name="stringBuilder_TC_BIG5">新文件 stringBuilder (TC_BIG5)</param>
        /// <param name="content">原文件内容</param>
        /// <param name="lastEnd">上次替换结束位置</param>
        /// <returns>替换结束位置</returns>
        public int Convert(
            ref StringBuilder stringBuilder_SC_GBK, ref StringBuilder stringBuilder_SC_UTF8,
            ref StringBuilder stringBuilder_TC_UTF8, ref StringBuilder stringBuilder_TC_BIG5,
            ref string content, int lastEnd)
        {
            string Behind = content.Substring(lastEnd, _Start - lastEnd);
            stringBuilder_SC_GBK.Append(Behind)
                .Append(_Insert);

            byte[] SC_Byte = gbk.GetBytes(Behind);
            stringBuilder_SC_UTF8.Append(utf8.GetString(Encoding.Convert(gbk, utf8, SC_Byte)))
                .Append(_Insert);

            byte[] TC_Byte = Encoding.Convert(gbk, big5, SC_Byte);
            stringBuilder_TC_BIG5.Append(big5.GetString(TC_Byte))
                .Append(_Insert);

            stringBuilder_TC_UTF8.Append(utf8.GetString(Encoding.Convert(big5, utf8, TC_Byte)))
                .Append(_Insert);
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
            FileInfo f = new FileInfo(FullName);
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

            StreamWriter sw = f.CreateText();
            sw.Write(stringBuilder.ToString());
            sw.Close();

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

            StringBuilder stringBuilder_SC_GBK = new StringBuilder();
            StringBuilder stringBuilder_SC_UTF8 = new StringBuilder();
            StringBuilder stringBuilder_TC_UTF8 = new StringBuilder();
            StringBuilder stringBuilder_TC_BIG5 = new StringBuilder();
            int lastEnd = 0;

            foreach (fileJob fJ in Jobs)
            {
                lastEnd = fJ.Convert(
                    ref stringBuilder_SC_GBK, ref stringBuilder_SC_UTF8, ref stringBuilder_TC_UTF8, ref stringBuilder_TC_BIG5, 
                    ref content, lastEnd);
            }

            if (lastEnd < content.Length) // 将最后的内容写回文件
            {
                string Last = content.Substring(lastEnd);
                stringBuilder_SC_GBK.Append(Last);
                stringBuilder_SC_UTF8.Append(Last);
                stringBuilder_TC_BIG5.Append(Last);
                stringBuilder_TC_UTF8.Append(Last);
            }

            f.Delete();
            /*
            StreamWriter sw = f.CreateText();
            sw.Write(stringBuilder.ToString());
            sw.Close();

            stringBuilder.Clear();
            Jobs.Clear();*/

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
