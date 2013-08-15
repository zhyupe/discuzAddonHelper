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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace discuzAddonHelper
{
    /// <summary>
    /// 语言包显示委托
    /// </summary>
    /// <param name="English">索引</param>
    /// <param name="Chinese">中文值</param>
    public delegate void AddLanguageDele(string English, string Chinese);

    /// <summary>
    /// 语言包管理器
    /// </summary>
    static class langDictionary
    {
        /// <summary>
        /// 语言包显示委托
        /// </summary>
        public static AddLanguageDele AddLanguage;

        /// <summary>
        /// 正向词典
        /// </summary>
        static Dictionary<string, string> _EngToChi = new Dictionary<string, string>();
        /// <summary>
        /// 反向词典
        /// </summary>
        static Dictionary<string, string> _ChiToEng = new Dictionary<string, string>();
        /// <summary>
        /// 索引词典（生成新索引后替换索引用）
        /// </summary>
        static Dictionary<string, string> _OldToNew = new Dictionary<string, string>();

        /// <summary>
        /// 已调用过的索引（导出用）
        /// </summary>
        static Dictionary<string, string> _Used = new Dictionary<string, string>();

        /// <summary>
        /// 向词典中添加未索引中文值
        /// </summary>
        /// <param name="Chinese">中文值</param>
        /// <param name="Type">语言包类型（P: Script, H: Template）</param>
        /// <returns>索引</returns>
        public static string AddNoKey(string Chinese, string Type)
        {
            if (_ChiToEng.ContainsKey(Chinese))
            {
                return _ChiToEng[Chinese];
            }
            else
            {
                //生成临时索引值
                string English = Guid.NewGuid().ToString();
                Add(English, Chinese, Type);
                return English;
            }
        }

        /// <summary>
        /// 向词典中添加语言
        /// </summary>
        /// <param name="English">索引</param>
        /// <param name="Chinese">中文值</param>
        /// <param name="Type">语言包类型（P: Script, H: Template）</param>
        public static void Add(string English, string Chinese, string Type)
        {
            if (English.IndexOf(':') != -1)
            {
                English = English.Split(':')[1];
            }

            _EngToChi[Type + English] = Chinese;

            if (_ChiToEng.ContainsKey(Chinese)) // 将该索引定位到已存在的索引
            {
                _OldToNew[Type + English] = _ChiToEng[Chinese];
            }
            else
            {
                _ChiToEng[Chinese] = Type + English;
            }

            AddLanguage(Type + English, Chinese);
        }

        /// <summary>
        /// 从词典中清空所有内容
        /// </summary>
        public static void Clear()
        {
            _ChiToEng.Clear();
            _EngToChi.Clear();
            _OldToNew.Clear();
            _Used.Clear();
        }

        /// <summary>
        /// 使用旧索引获取新索引
        /// </summary>
        /// <param name="English">索引</param>
        /// <param name="Type">语言包类型（P: Script, H: Template）</param>
        /// <returns></returns>
        public static string GetEnglish(string English, string Type)
        {
            if (_OldToNew.ContainsKey(Type + English))
            {
                _Used[Type + English] = _OldToNew[Type + English];
                return _OldToNew[Type + English];
            }
            else if (_EngToChi.ContainsKey(Type + English))
            {
                _Used[Type + English] = Type + English;
                return Type + English;
            }
            else
            {
                Form1.Log("警告: langDictionary::GetEnglish 尝试获取不存在的索引 " + Type + English);
                return null;
            }
        }

        /// <summary>
        /// 获取词典内容
        /// </summary>
        /// <param name="English">索引</param>
        /// <param name="Type">语言包类型（P: Script, H: Template）</param>
        /// <returns></returns>
        public static string GetChinese(string English, string Type)
        {
            if (English.IndexOf(':') != -1)
            {
                English = English.Split(':')[1];
            }

            if (_EngToChi.ContainsKey(Type + English))
            {
                _Used[Type + English] = Type + English;
                return _EngToChi[Type + English];
            }
            else if (_OldToNew.ContainsKey(Type + English))
            {
                Form1.Log("提示: langDictionary::GetChinese 请求被重定向到另一索引 " + Type + English);

                _Used[Type + English] = _OldToNew[Type + English];
                return _EngToChi[_OldToNew[Type + English]];
            }
            else if (_EngToChi.ContainsKey("_" + English)) // 通用语言包判断
            {
                Form1.Log("提示: langDictionary::GetChinese 请求被重定向到通用语言包 " + Type + English);

                _Used[Type + English] = "_" + English;
                return _EngToChi["_" + English];
            }
            else
            {
                Form1.Log("警告: langDictionary::GetChinese 尝试获取不存在的内容 " + Type + English);
                return English;
            }
        }

        /// <summary>
        /// 更新词典并生成新索引
        /// </summary>
        public static void Update()
        {
            Dictionary<string, string> OldToNew = new Dictionary<string, string>(_OldToNew);

            _OldToNew.Clear();
            _EngToChi.Clear();
            _Used.Clear();

            int Length = (int)Math.Ceiling(Math.Log(_ChiToEng.Count, 62));
            string English = "";
            int _Key = 0;

            foreach (KeyValuePair<string, string> obj in _ChiToEng)
            {
                English = "_" + ConvertTo62(_Key++, Length);

                _OldToNew[obj.Value] = English; // 将当前索引重定向到新索引
                _EngToChi[English] = obj.Key; // 新建正向索引
                _ChiToEng[obj.Key] = English; // 更新反向索引

                AddLanguage(English, obj.Key);
            }

            // 更新重定向旧数据
            foreach (KeyValuePair<string, string> obj in OldToNew)
            {
                _OldToNew[obj.Key] = _OldToNew[obj.Value];
                AddLanguage("[R]" + obj.Key, _EngToChi[obj.Value]);
            }
        }

        /// <summary>
        /// 导出语言包
        /// </summary>
        /// <param name="_P">Script 语言包</param>
        /// <param name="_H">Template 语言包</param>
        public static void Export(out Dictionary<string, string> _P, out Dictionary<string, string> _H)
        {
            _P = _H = new Dictionary<string,string>();
            foreach (KeyValuePair<string, string> obj in _Used)
            {
                switch (obj.Key[0])
                {
                    case 'P':
                        _P[obj.Key.Substring(1)] = _EngToChi[obj.Value];
                        break;
                    case 'H':
                        _H[obj.Key.Substring(1)] = _EngToChi[obj.Value];
                        break;
                }
            }
        }

        private static char[] charSet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        /// <summary>
        /// 将指定数字转换为指定长度的62进制
        /// </summary>
        /// <param name="value">要转换的数字</param>
        /// <param name="length">需要的长度</param>
        /// <returns>62进制表示格式</returns>
        private static string ConvertTo62(int value, int length)
        {
            string sixtyNum = string.Empty;
            if (value < 62)
            {
                sixtyNum = charSet[value].ToString().PadLeft(length, '0');
            }
            else
            {
                long result = value;
                while (result > 0)
                {
                    long val = result % 62;
                    sixtyNum = charSet[val] + sixtyNum;
                    result = result / 62;
                }
                sixtyNum = sixtyNum.PadLeft(length, '0');
            }
            return sixtyNum;
        }
    }
}
