using System;
using System.IO;
using System.Xml.Linq;

namespace Ljc.Common
{
    /// <summary>
    /// 解析Xml文件的类
    /// </summary>
    public class XmlParse
    {
        /// <summary>
        /// 文件绝对路径
        /// </summary>
        public string XmlFilePath { get; set; }

        /// <summary>
        /// 文件内容
        /// </summary>
        public XDocument Document { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="xmlFilePath">XML文件绝对路径</param>
        public XmlParse(string xmlFilePath)
        {
            XmlFilePath = xmlFilePath;
            try
            {
                if (File.Exists(XmlFilePath))
                {
                    Document = XDocument.Load(XmlFilePath);
                }
            }
            catch (Exception e)
            {
                throw new Exception("XDocument.Load(XmlFile)加载文件失败", e);
            }
        }
    }
}
