using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer
{
    public static class StringHelper
    {
        /// <summary>
        /// 得到字符串的长度，一个汉字算2个字符，一个英文算1个字符 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="del"></param>
        /// <returns></returns>
        public static int GetStringEnLength(this object o ,int def = 0)
        {
            string all_english = " `1234567890-=\\qwertyuiop[]asdfghjkl;'zxcvbnm,./~!@#$%^&*()_+|QWERTYUIOP{}ASDFGHJKL:\"ZXCVBNM<>?";
            string str = o.ToString();
            for (int i = 0; i < str.ToString().Length; i++)
            {
                if (all_english.IndexOf(str[i].ToString()) >= 0)
                {
                    def = def + 1;
                }
                else
                {
                    def = def + 2;
                }
            }
            return def;
        }
        /// <summary>
        /// 得到字符串的长度，一个汉字算1个字符，一个英文算0.5个字符 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="del"></param>
        /// <returns></returns>
        public static double GetStringChLength(this object o, double def = 0)
        {
            string all_english = " `1234567890-=\\qwertyuiop[]asdfghjkl;'zxcvbnm,./~!@#$%^&*()_+|QWERTYUIOP{}ASDFGHJKL:\"ZXCVBNM<>?";
            string str = o.ToString();
            for (int i = 0; i < str.ToString().Length; i++)
            {
                if (all_english.IndexOf(str[i].ToString()) >= 0)
                {
                    def = def + 0.5;
                }
                else
                {
                    def = def + 1;
                }
            }
            return def;
        }
    }
}
