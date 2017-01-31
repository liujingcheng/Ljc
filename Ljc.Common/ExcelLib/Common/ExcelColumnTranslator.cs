using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CanYouLib.ExcelLib.Utility
{
    /// <summary>
    /// Excel列名转换类。by 吴国超
    /// </summary>
    public static class ExcelColumnTranslator
    {
        /// <summary>
        /// 列名转列号，A对应1
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <returns></returns>
        public static int ToIndex(string columnName)
        {
            if (!Regex.IsMatch(columnName.ToUpper(), @"[A-Z]+"))
                throw new Exception("错误的excel列名:" + columnName);
            int index = 0;
            char[] chars = columnName.ToUpper().ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                index += ((int)chars[i] - (int)'A' + 1) * (int)Math.Pow(26, chars.Length - i - 1);
            }
            return index;
        }

        /// <summary>
        /// 列号转列名，1对应A
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string ToName(int index)
        {
            if (index < 0)
                throw new Exception("invalid ColIndex");
            List<string> chars = new List<string>();
            do
            {
                if (chars.Count > 0) index--;
                chars.Insert(0, ((char)(index % 26 + (int)'A')).ToString());
                index = (int)((index - index % 26) / 26);
            } while (index > 0);

            return String.Join(string.Empty, chars.ToArray());
        }
    }
}
