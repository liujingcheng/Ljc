using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CanYouLib.ExcelLib.Utility
{
    /// <summary>
    /// 文件简单帮助类。
    /// Code by:吴国超
    /// </summary>
    public static class FileUtility
    {
        /// <summary>
        /// 从一个完整文件名中取得完整路径
        /// </summary>
        /// <param name="p_FileName"></param>
        /// <returns></returns>
        public static string GetPath(string p_FileName)
        {
            string filename = System.IO.Path.GetFileName(p_FileName);//取得文件名“Default.aspx”
            return p_FileName.Replace(filename, "");
        }

        /// <summary>
        /// 从一个完整文件名中取得文件名
        /// </summary>
        /// <param name="p_FileName"></param>
        /// <returns></returns>
        public static string GetFileName(string p_FileName)
        {
            return System.IO.Path.GetFileName(p_FileName);//取得文件名“Default.aspx”
        }

        /// <summary>
        /// 从一个完整文件名中取得扩展名
        /// </summary>
        /// <param name="p_FileName"></param>
        /// <returns></returns>
        public static string GetExtension(string p_FileName)
        {
            return System.IO.Path.GetExtension(p_FileName);//扩展名 “.aspx”
        }

        /// <summary>
        /// 实现Image转Byte
        /// </summary>
        /// <param name="p_Image"></param>
        /// <returns></returns>
        public static byte[] ImageToByte(System.Drawing.Image p_Image)
        {
            MemoryStream ms = new MemoryStream();
            p_Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        } 
    }
}
