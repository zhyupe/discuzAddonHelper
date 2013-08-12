using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace discuzAddonHelper
{
    public class treeBuilder
    {
        Form1.AddNodeDele _addNode;
        DirectoryInfo _dir;
        TreeNodeCollection _tree;

        char[] content;

        public treeBuilder(DirectoryInfo dir, TreeNodeCollection tree, Form1.AddNodeDele addNode)
        {
            this._dir = dir;
            this._tree = tree;
            this._addNode = addNode;
        }

        public void Work() {
            _Work(_dir, _tree);
            _tree[0].Remove(); // remove loading
        }

        private void _Work(DirectoryInfo dir, TreeNodeCollection tree)
        {
            DirectoryInfo[] dirs = dir.GetDirectories();
            foreach (DirectoryInfo f in dirs)
            {
                _Work(f, _addNode(tree, f.Name, "[D]" + f.Name, "D|" + f.FullName));
            }

            FileInfo[] files = dir.GetFiles();
            string type = "";
            foreach (FileInfo f in files)
            {
                switch (f.Extension.ToLower())
                {
                    case ".php":
                        type = "P";
                        break;
                    case ".htm":
                        type = "H";
                        break;
                    case ".xml":
                        type = "X";
                        break;
                    default:
                        continue;
                }

                FileStream fs = f.OpenRead();
                StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("GBK"));
                content = sr.ReadToEnd().ToCharArray();
                sr.Close();
                fs.Close();

                _Work_F(_addNode(tree, f.Name, "[F:" + type + "]" + f.Name, type + "|" + f.FullName), type);
            }
        }

        private void _Work_F(TreeNodeCollection tree, string type)
        {
            switch (type)
            {
                case "P":
                    _Work_P(tree);
                    break;
                case "H":
                    _Work_H(tree);
                    break;
                case "X":
                    _Work_X(tree);
                    break;
            }
        }

        /// <summary>
        /// PHP 文件处理逻辑
        /// </summary>
        /// <param name="tree">文件对应的 TreeNodeCollection</param>
        private void _Work_P(TreeNodeCollection tree)
        {
            int i = 0;
            int l = content.Length;

            int find = -1;
            int lang = -1;

            int status = -1;
            string _status = "<?";

            int temp = _status.Length;
            int start = 0;

            StringBuilder _val = new StringBuilder();
            string _lang;

            char quota = '-';

            while (i < l)
            {
                if (content[i] == '\r')
                    content[i] = '\n';

                if (lang == 1 && find > -1)
                {
                    if (content[i] == '\\')
                    {
                        _val.Append(content[i++]);
                        _val.Append(content[i++]);
                        continue;
                    }

                    if (content[i++] == quota)
                    {
                        _lang = _val.ToString();
                        _val.Clear();

                        _addNode(tree, null, "[L]" + _lang, string.Concat("L|", find, '|', i - find, '|', quota, '|', _lang));
                        find = -1;
                        lang = -1;
                        quota = '-';
                        continue;
                    }
                    _val.Append(content[i - 1]);
                }
                else
                {
                    #region 清理注释及 Html (status != 0)
                    if (status != 0)
                    {
                        if (status == -1)
                        {
                            _val.Append(content[i++]);

                            if (i >= temp && _val.ToString(i - temp, _status.Length) == _status)
                            {
                                status = 0; _status = ""; temp = 0;
                                _val.Clear();
                            }
                        }
                        else if (status == 1 && (content[i] == '\n') ||
                          status == 2 && content[i] == '*' && content[i + 1] == '/')
                        {
                            status = 0;

                            _lang = _val.ToString();
                            _val.Clear();

                            _addNode(tree, null, "[D]" + _lang, string.Concat("D|", find, '|', i + 1 - find, '|', _lang));
                        }
                        else
                        {
                            _val.Append(content[i++]);
                        }
                        continue;
                    }
                    #endregion
                    #region 识别中文
                    if (content[i] > 0x4e00 && content[i] < 0x9fff)
                    {
                        if (find == -1)
                        {
                            start = 0;
                            find = i;
                            lang = 0;
                        }
                        _val.Append(content[i++]);

                        continue;
                    }
                    else if (lang == 0 && find > -1)
                    {
                        _lang = _val.ToString().TrimStart();
                        _val.Clear();

                        // 借用 temp 暂存变量
                        temp = content[i] == quota ? 1 : 0;
                        // 如果中文之后即为字符串，则去掉最后一个引号（增加一位识别）

                        _addNode(tree, null, "[C:" + quota + "]" + _lang, string.Concat("C|", find, '|', i + temp - find, '|', quota, '|', temp, '|', _lang));
                        find = -1;
                        lang = -1;

                        // temp 归零
                        temp = 0;
                    }
                    #endregion

                    #region 跳过字符串
                    if (quota != '-')
                    {
                        if (content[i++] == quota)
                            quota = '-';
                        continue;
                    }
                    #endregion

                    switch (content[i++])
                    {
                        #region 转义字符 \
                        case '\\':
                            i += 1; // 跳过下一个字符
                            break;
                        #endregion 
                        #region 注释 // /* #
                        case '/':
                            if (content[i] == '/')
                            {
                                status = 1;
                            }
                            else if (content[i] == '*')
                            {
                                status = 2;
                            }
                            find = i;
                            break;
                        case '#':
                            status = 1;
                            find = i;
                            break;
                        #endregion
                        #region 字符串 ' "
                        case '\'':
                            if (quota == '-')
                            {
                                quota = '\'';
                            }
                            else if (quota == '\'')
                            {
                                quota = '-';
                            }

                            if (start == 3)
                                lang = 1;

                            break;
                        case '"':
                            if (quota == '-')
                            {
                                quota = '"';
                            }
                            else if (quota == '"')
                            {
                                quota = '-';
                            }

                            if (start == 3)
                                lang = 1;

                            break;
                        #endregion
                        #region 定界符 <? <<<
                        case '?':
                            if (content[i] == '>')
                            {
                                status = -1;
                                _status = "<?";
                                i++;
                                temp = i + 2;
                            }
                            break;
                        case '<':
                            if (string.Concat(content[i], content[i + 1]) == "<<")
                            {
                                i += 2; // 跳过 <<
                                _val.Append('\n');
                                while (content[i] != '\n')
                                {
                                    _val.Append(content[i++]);
                                }
                                _val.Append(';');

                                status = -1;
                                _status = _val.ToString();
                                i++;
                                 temp = i + _val.Length;
                                _val.Clear();
                            }
                            break;
                        #endregion;
                        #region 类方法 -> ::
                        case '-':
                            if (content[i] == '>')
                            {
                                start = 1;
                            }
                            break;
                        case ':':
                            if (content[i] == ':')
                            {
                                start = 1;
                            }
                            break;
                        case '$':
                            start = 1;
                            break;
                        #endregion
                        #region 重新判定符 , ; = . )
                        case ',':
                        case ';':
                        case '=':
                        case '.':
                        case ')':
                            start = 0;
                            break;
                        #endregion
                        #region showmessage 函数判定
                        case 'S':
                        case 's':
                            if ((start == 0 || start == 3) && string.Concat(
                                content[i], content[i + 1], content[i + 2], content[i + 3],
                                content[i + 4], content[i + 5], content[i + 6], content[i + 7],
                                content[i + 8], content[i + 9]).ToLower() == "howmessage")
                            {
                                find = i - 1;
                                i += 10;
                                start = 2;
                            }
                            else
                            {
                                start = 1;
                            }
                            break;
                        #endregion
                        #region cpmsg 函数判定
                        case 'C':
                        case 'c':
                            if ((start == 0 || start == 3) && string.Concat(
                                content[i], content[i + 1], content[i + 2], content[i + 3]).ToLower() == "pmsg")
                            {
                                find = i - 1;
                                i += 4;
                                start = 2;
                            }
                            else
                            {
                                start = 1;
                            }
                            break;
                        #endregion
                        #region lang 函数判定
                        case 'L':
                        case 'l':
                            if ((start == 0 || start == 3) && string.Concat(
                                content[i], content[i + 1], content[i + 2]).ToLower() == "ang")
                            {
                                find = i - 1;
                                i += 3;
                                start = 2;
                            }
                            else
                            {
                                start = 1;
                            }
                            break;
                        #endregion
                        #region 函数相关 (
                        case '(':
                            switch (start)
                            {
                                case 1: // 跳过此函数
                                    start = 0;
                                    break;
                                case 2: // 匹配成功
                                    start = 3;
                                    break;
                                default: // 没有检测到函数
                                    start = 0;
                                    break;
                            }
                            break;
                        #endregion
                        #region 无意义字符
                        case ' ':
                        case '\t':
                        case '\n':
                            break;
                        #endregion
                        default:
                            start = 1;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Html 文件处理逻辑
        /// </summary>
        /// <param name="tree">文件对应的 TreeNodeCollection</param>
        private void _Work_H(TreeNodeCollection tree)
        {
            int i = 0;
            int l = content.Length;

            int find = -1;
            bool lang = false;

            StringBuilder _val = new StringBuilder();
            string _lang;

            while (i < l)
            {
                if (lang && find > -1)
                {
                    switch (content[i++])
                    {
                        case '}':
                            _lang = _val.ToString().TrimStart();
                            _val.Clear();

                            _addNode(tree, null, "[L]" + _lang, string.Concat("L|", find, '|', i - find, '|', _lang));
                            find = -1;
                            continue;
                        case '\n':
                            find = -1;
                            continue;
                    }

                    _val.Append(content[i - 1]);
                }
                else
                {
                    if (content[i] > 0x4e00 && content[i] < 0x9fff)
                    {
                        if (find == -1)
                        {
                            find = i;
                            lang = false;
                        }
                        _val.Append(content[i++]);

                        continue;
                    }
                    else if (find > -1)
                    {
                        _lang = _val.ToString().TrimStart();
                        _val.Clear();

                        _addNode(tree, null, "[C]" + _lang, string.Concat("C|", find, '|', i + 1 - find, '|', _lang));
                        find = -1;
                    }

                    if (content[i++] == '{' && string.Concat(content[i], content[i + 1], content[i + 2], content[i + 3]) == "lang")
                    {
                        find = i;
                        i += 5;
                        lang = true;
                    }
                }
            }
        }

        /// <summary>
        /// Xml 文件处理逻辑
        /// </summary>
        /// <param name="tree">文件对应的 TreeNodeCollection</param>
        private void _Work_X(TreeNodeCollection tree)
        {
            int i = 0;
            int l = content.Length;

            int find = -1;

            StringBuilder _val = new StringBuilder();
            string _lang;

            while (i < l)
            {
                if (content[i] > 0x4e00 && content[i] < 0x9fff)
                {
                    if (find == -1)
                    {
                        find = i;
                    }
                    _val.Append(content[i++]);

                    continue;
                }
                else if (find > -1)
                {
                    _lang = _val.ToString().TrimStart();
                    _val.Clear();

                    _addNode(tree, null, "[C]" + _lang, string.Concat("C|", find, '|', i + 1 - find, '|', _lang));
                    find = -1;
                }
                i++;
            }
        }
    }
}
