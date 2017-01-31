using System.Collections.Generic;
using System.Text;

namespace CanYouLib.ExcelLib
{
    /// <summary>
    /// Excel列信息，使用该类可以实现选择性导出,
    /// 以及简单数据格式设置。
    /// </summary>
    public class ExcelColumInfo
    {
        private string _name;
        private string _columnName;
        private int _propertyindex;
        private int _width = 20;
        private DataType _columType = DataType.Text;
        private string format = null;
        /// <summary>
        /// Excel中的列名，就是表头名称。
        /// 在模板导出模式下不会修改模板中的表头名称仅用于匹配。
        /// </summary>
        public string HeadName
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        /// <summary>
        /// 对应DataTable的列名，DataTable导出的时候使用，
        /// 集合导出时无效
        /// </summary>
        public string TableColumnName
        {
            get
            {
                return _columnName;
            }
            set
            {
                _columnName = value;
            }
        }

        /// <summary>
        /// 对应的属性在对应的类的所有属性的下标（从0开始），
        /// 必须对应对应，集合导出的时候使用，DataTable导出时无效
        /// </summary>
        public int PropertyIndex
        {
            get
            {
                return _propertyindex;
            }
            set
            {
                _propertyindex = value;
            }
        }

        /// <summary>
        /// 列宽，在模板导出模式下无效
        /// </summary>
        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }
        }
        /// <summary>
        /// 数据类型
        /// </summary>
        public DataType DataType
        {
            get
            {
                return _columType;
            }
            set
            {
                _columType = value;
            }
        }
        /// <summary>
        /// 格式化信息，日期格式化例子：Format="{0:d}" 其结果为2005-11-5；Format="{0:G}" 
        /// 其结果为2005-11-5 14:23:23
        /// 数字格式化例子：Format="{0:C3}" 其结果为￥1,234.125
        /// 具体格式化请参见对应类型的ToString方法的格式化表达式。
        /// 在模板导出模式下无效
        /// </summary>
        public string Format
        {
            get { return format; }
            set { format = value; }
        }
    }
}
