using System;
using System.Collections.Generic;
using System.Text;

namespace CanYouLib.ExcelLib.Utility
{
    public class DocumentSummary
    {
        /// <summary>
        /// 公司名称
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// 作者名称
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string Keywords { get; set; }
        /// <summary>
        /// 主题
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 开始写数据的行数
        /// </summary>
        public int FirstRow { get; set; }
    }
}
