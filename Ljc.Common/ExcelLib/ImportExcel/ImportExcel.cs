using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Data.SqlClient;

/***
 * 最新版本号
 * Create Version:0.0.1.1
 * Code by:吴国超
 * Date:2010-02-06
 * 更新记录：
 * Version:0.0.1.0
 * Code by:吴国超
 * Date:2010-02-12
 * 1、实现简单导出以及模板导出
 * 2、简单导出支持选择性导出
 * Version:0.0.1.1
 * Code by:吴国超
 * Date:2010-02-25
 * 1、增加数据格式化属性，支持日期、数字等格式化
 * 2、简单导出增加了冻结第一行
 * Date:2010-06-07
 * 1、修正了日期、数字等类型为Null时导出异常的问题
 * 2、增加导出返回临时文件路径方法
 * Date:2011-09-13
 * 1、数据导入，完善了多Sheet导入
 * 2、数据导入，增加了大数据导入数据库方法，并且支持多表批量导入数据库。
 * 3、数据导入，增加了Sheet导入时指定标题行号功能，使得兼容标题行不在第一行的情况。
 * 4、数据导出，增加了单元格内容格式化功能，支持常用的日期、数值的格式化。
 * 5、数据导出，增加了文件属性中的摘要的设置。
 * 6、数据导出，优化了标题行以及内容行的部分样式
 ***/

namespace CanYouLib.ExcelLib
{
    /// <summary>
    /// Excel导入类
    /// </summary>
    public class ImportExcel
    {
        protected IWorkbook _workbook;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ImportExcel()
        { }


        /// <summary>
        /// Excel转化为DtaSet
        /// </summary>
        /// <param name="p_FileName">Excel</param>
        /// <param name="p_AllowNullRow">是否允许空行</param>
        /// <param name="p_FirstRow">内容开始行从第0行开始</param>
        /// <param name="TitleRows">表头所在行，从第0行开始</param>
        /// <returns>DataSet</returns>
        public DataSet ImportDataSet(string p_FileName, bool p_AllowNullRow, int p_FirstRow, int TitleRows)
        {
            int index = 0;
            try
            {
                GetWorkBook(p_FileName);
                var ds = new DataSet();
                for (int i = 0; i < _workbook.NumberOfSheets; i++)
                {
                    index = i;
                    DataTable dt = ConvertToDataTable(i, TitleRows, p_AllowNullRow, p_FirstRow);
                    ds.Tables.Add(dt);
                }
                _workbook = null;
                return ds;
            }
            catch (Exception ex)
            {
                if (_workbook != null)
                {
                    _workbook = null;
                }
                index++;
                throw new Exception("导入数据出错，第" + index.ToString() + "个Sheet出错", ex);
            }
        }
        /// <summary>
        /// 将Excel数据插入到数据库
        /// </summary>
        /// <param name="p_FileName">Excel文件</param>
        /// <param name="p_AllowNullRow">是否允许空行</param>
        /// <param name="p_FirstRow">内容开始行，从第0行开始</param>
        /// <param name="TitleRows">表头开始行</param>
        /// <param name="DataBaseConfig">数据库连接字符串</param>
        /// <param name="DicMapping">数据源与数据库表字段映射，键为数据库列，值为数据源列</param>
        /// <param name="TableName">数据库表名</param>
        /// <param name="msg">返回信息</param>
        /// <returns></returns>
        public bool BatchInsertExcelToDataBase(string p_FileName, bool p_AllowNullRow, int p_FirstRow, int TitleRows, string DataBaseConfig, Dictionary<string, string> DicMapping, string TableName, out string msg)
        {
            try
            {
                DataSet DsExcel = ImportDataSet(p_FileName, p_AllowNullRow, p_FirstRow, TitleRows);
                using (SqlBulkCopy copy = new SqlBulkCopy(DataBaseConfig))
                {
                    copy.BulkCopyTimeout = 1000;
                    copy.DestinationTableName = TableName;
                    foreach (KeyValuePair<string, string> item in DicMapping)
                    {
                        copy.ColumnMappings.Add(item.Value, item.Key);
                    }
                    copy.WriteToServer(DsExcel.Tables[0]);
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
            msg = "操作成功";
            return true;
        }


        /// <summary>
        /// 打开Excel文件并将内容放到HSSFWorkbook
        /// </summary>
        /// <param name="p_FileName">完整文件名</param>
        private void GetWorkBook(string p_FileName)
        {
            try
            {
                using (FileStream file = new FileStream(p_FileName, FileMode.Open, FileAccess.Read))
                { _workbook = new HSSFWorkbook(file); }
            }
            catch (Exception e)
            {
                throw new Exception("打开Excel文件时出错:"+e.Message, e);
            }
        }
        /// <summary>
        /// 把Excel转化为DataTable
        /// </summary>
        /// <param name="p_SheetIndex">sheet开始</param>
        /// <param name="p_TitleRowIndex">表头开始行</param>
        /// <param name="p_AllowNullRow">是否允许为空</param>
        /// <param name="p_FirstRow">内容开始行，从0开始</param>
        /// <returns></returns>
        private DataTable ConvertToDataTable(int p_SheetIndex, int p_TitleRowIndex, bool p_AllowNullRow, int p_FirstRow)
        {
            try
            {

                ISheet sheet = _workbook.GetSheetAt(p_SheetIndex);
                System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
                DataTable dt = GetDataTableStruct(p_SheetIndex, p_TitleRowIndex).Copy();
                int count = 0;
                int NullColumnCount = 0;
                while (rows.MoveNext())
                {
                    count++;
                    if (count > p_FirstRow)
                    {
                        HSSFRow row = (HSSFRow)rows.Current;
                        DataRow dr = dt.NewRow();
                        NullColumnCount = 0;
                        for (int i = 0; i < row.LastCellNum; i++)
                        {
                            ICell cell = row.GetCell(i);
                            if (cell == null)
                            {
                                dr[i] = null;
                                NullColumnCount++;
                            }
                            else
                            {
                                if (cell.ToString().Length == 0)
                                {
                                    NullColumnCount++;
                                }
                                switch (cell.CellType)
                                {
                                    case CellType.Boolean:
                                        dr[i] = cell.BooleanCellValue.ToString();
                                        break;
                                    case CellType.Numeric:
                                        double d = cell.NumericCellValue;
                                        if (HSSFDateUtil.IsCellDateFormatted(cell))
                                        {
                                            if (HSSFDateUtil.IsValidExcelDate(d))
                                            {
                                                dr[i] = HSSFDateUtil.GetJavaDate(d);
                                            }
                                            else
                                            {
                                                dr[i] = cell.NumericCellValue;
                                            }
                                        }
                                        else
                                        {
                                            dr[i] = cell.NumericCellValue;
                                        }
                                        break;
                                    case CellType.String:

                                        dr[i] = cell.StringCellValue;
                                        break;
                                    case CellType.Error:
                                        dr[i] = cell.ErrorCellValue;
                                        break;
                                    case CellType.Formula:
                                        var e = new HSSFFormulaEvaluator(_workbook);
                                        cell = e.EvaluateInCell(cell);
                                        dr[i] = cell.NumericCellValue;
                                        break;
                                    default:
                                        dr[i] = cell.ToString();
                                        break;
                                }
                            }
                        }

                        if (p_AllowNullRow)//允许记录空行
                        {
                            dt.Rows.Add(dr);
                        }
                        else
                        {
                            if (NullColumnCount < row.LastCellNum)
                                dt.Rows.Add(dr);
                        }
                    }
                }
                return dt;
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString(), e);
            }
        }
        /// <summary>
        /// 返回的DataBale的所有列的类型都是string
        /// </summary>
        /// <param name="p_SheetIndex">Sheet下标,从0开始</param>
        /// <param name="p_TitleRowIndex">标题行的起始行号,从0开始</param>
        /// <returns></returns>
        private DataTable GetDataTableStruct(int p_SheetIndex, int p_TitleRowIndex)
        {
            try
            {
                var dt = new DataTable { TableName = _workbook.GetSheetName(p_SheetIndex) };
                IRow row = _workbook.GetSheetAt(p_SheetIndex).GetRow(p_TitleRowIndex);
                var dcRowNum = new DataColumn();
                for (int i = 0; i < row.LastCellNum; i++)//从第一有数据的列读取到最后有数据的列
                {
                    var dc = new DataColumn();
                    dc.AllowDBNull = true;
                    dc.ColumnName = row.GetCell(i).StringCellValue;
                    dc.DataType = Type.GetType("System.String");
                    dc.MaxLength = 4000;
                    dt.Columns.Add(dc);
                }
                return dt;
            }
            catch (Exception e)
            {
                throw new Exception("构造DataTable结构时出错:", e);
            }
        }

    }
}
