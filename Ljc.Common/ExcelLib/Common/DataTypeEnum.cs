using System;
using System.Collections.Generic;
using System.Text;

namespace CanYouLib.ExcelLib
{
    /// <summary>
    /// 数据类型枚举
    /// 单元格数据类型，若要使模板中的单元格
    /// 格式起作用请对每个列都设置DataType。
    /// </summary>
    public enum DataType
    {
        /// <summary>
        /// 普通字符串
        /// </summary>
        Text,
        /// <summary>
        /// 数字
        /// </summary>
        Number,
        /// <summary>
        ///日期
        /// </summary>
        DateTime,
        /// <summary>
        ///布尔型
        /// </summary>
        Boolean
    }
}
