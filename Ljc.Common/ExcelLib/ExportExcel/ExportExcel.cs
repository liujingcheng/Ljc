using System.Linq;
using System.Text;
using CanYouLib.ExcelLib.Utility;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
//using Toxy;

namespace CanYouLib.ExcelLib
{
    /// <summary>
    /// Excel表格导出
    /// update by yyq 
    /// </summary>
    public class ExportExcel
    {
        #region 只读属性

        private IWorkbook _workbook;

        private int _maxSheetRowCount = 65534;
        /// <summary>
        /// 单个Sheet最大行数，本来是65535，但列名占了一行所以是65534
        /// </summary>
        public int MaxSheetRowCount
        {
            get { return _maxSheetRowCount; }
            private set { _maxSheetRowCount = value; }
        }

        private int _firstHeadRow = 1;
        /// <summary>
        /// 表头所在行 从1开始,默认第一行为表头 
        /// </summary>
        public int FirstHeadRow
        {
            get { return _firstHeadRow; }
            set { _firstHeadRow = value; }
        }

        #endregion

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ExportExcel()
        {

        }
        /// <summary>
        /// 如果模板导出使用此构造函数
        /// </summary>
        /// <param name="firstrowhead">表头所在行 从1开始,默认第一行为表头 </param>
        public ExportExcel(int firstrowhead)
        {
            FirstHeadRow = firstrowhead;
        }

        #region 单元格操作 public

        #region SetValue 单元格取值 public
        public string GetValue(int p_SheetIndex, string p_ColChar, int p_RowNumber)
        {
            return GetValue(p_SheetIndex, ExcelColumnTranslator.ToIndex(p_ColChar), p_RowNumber);
        }
        public string GetValue(int p_SheetIndex, int p_ColNumber, int p_RowNumber)
        {
            try
            {
                var sheet = _workbook.GetSheetAt(p_SheetIndex);
                return sheet.GetRow(p_RowNumber - 1).GetCell(p_ColNumber - 1).StringCellValue;
            }
            catch (Exception ex)
            {
                throw new Exception("单元格取值失败", ex);
            }
        }
        public string GetValue(string p_SheetName, string p_ColChar, int p_RowNumber)
        {
            return GetValue(p_SheetName, ExcelColumnTranslator.ToIndex(p_ColChar), p_RowNumber);
        }
        public string GetValue(string p_SheetName, int p_ColNumber, int p_RowNumber)
        {
            try
            {
                var sheet = _workbook.GetSheet(p_SheetName);
                return sheet.GetRow(p_RowNumber - 1).GetCell(p_ColNumber - 1).StringCellValue;
            }
            catch (Exception ex)
            {
                throw new Exception("单元格取值失败", ex);
            }
            
        }

        public int getRowCount(int p_SheetIndex)
        {
            try
            {
                var sheet = _workbook.GetSheetAt(p_SheetIndex);
                return sheet.LastRowNum + 1;
            }
            catch (Exception ex)
            {
                throw new Exception("取行数失败", ex);
            }
        }

        #endregion

        #region SetValue 单元格赋值 public
        /// <summary>
        /// 对单元格赋值 string类型
        /// </summary>
        /// <param name="p_RowIndex">行号，从1开始</param>
        /// <param name="p_ColumnIndex">列索引，从1开始</param>
        /// <param name="p_Value">期望值</param>
        /// <param name="TemplateFileName">Excel模板</param>
        /// <param name="p_SheetIndex">从哪个sheet开始，默认从第一个sheet</param>
        public void SetValue(int p_RowIndex, int p_ColumnIndex, string p_Value, string TemplateFileName, int p_SheetIndex = 0)
        {
            try
            {
                ISheet _sheet = OpenExcel(TemplateFileName, p_SheetIndex);
                _sheet.GetRow(p_RowIndex - 1).GetCell(p_ColumnIndex - 1).SetCellValue(p_Value);
                _sheet = null;
            }
            catch (Exception ex)
            {
                throw new Exception("单元格赋值失败", ex);
            }
        }
        /// <summary>
        /// 对单元格赋值 string类型
        /// </summary>
        /// <param name="p_RowIndex">行号，从1开始</param>
        /// <param name="p_ColChars">列名，从A开始</param>
        /// <param name="p_Value">期望值</param>
        /// <param name="TemplateFileName">Excel模板</param>
        /// <param name="p_SheetIndex">从哪个sheet开始，默认从第一个sheet</param>
        public void SetValue(int p_RowIndex, string p_ColChars, string p_Value, string TemplateFileName, int p_SheetIndex = 0)
        {
            SetValue(p_RowIndex, ExcelColumnTranslator.ToIndex(p_ColChars), p_Value, TemplateFileName, p_SheetIndex);
        }

        /// <summary>
        /// 对单元格赋值 double类型
        /// </summary>
        /// <param name="p_RowIndex">行号，从1开始</param>
        /// <param name="p_ColumnIndex">列号，从1开始</param>
        /// <param name="p_Value">期望值</param>
        /// <param name="TemplateFileName">Excel模板</param>
        /// <param name="p_SheetIndex">从哪个sheet开始，，默认从第一个sheet</param>
        public void SetValue(int p_RowIndex, int p_ColumnIndex, double p_Value, string TemplateFileName, int p_SheetIndex = 0)
        {
            try
            {
                ISheet _sheet = OpenExcel(TemplateFileName, p_SheetIndex);
                _sheet.GetRow(p_RowIndex - 1).GetCell(p_ColumnIndex - 1).SetCellValue(p_Value);
                _sheet = null;
            }
            catch (Exception ex)
            {
                throw new Exception("单元格赋值失败", ex);
            }
        }
        /// <summary>
        /// 对单元格赋值 double类型
        /// </summary>
        /// <param name="p_RowIndex">行号，从1开始</param>
        /// <param name="p_ColChars">列名，从A开始</param>
        /// <param name="p_Value">期望值</param>
        /// <param name="TemplateFileName">Excel模板</param>
        /// <param name="p_SheetIndex">从哪个sheet开始，默认从第一个sheet</param>
        public void SetValue(int p_RowIndex, string p_ColChars, double p_Value, string TemplateFileName, int p_SheetIndex = 0)
        {
            SetValue(p_RowIndex, ExcelColumnTranslator.ToIndex(p_ColChars), p_Value, TemplateFileName, p_SheetIndex);
        }
        /// <summary>
        /// 对单元格区域赋值
        /// </summary>
        /// <param name="p_startRowIndex">起始行号，从1开始</param>
        /// <param name="p_endRowIndex">结束行号，从1开始</param>
        /// <param name="p_startColIndex">起始列号，从1开始</param>
        /// <param name="p_endColIndex">结束列号，从1开始</param>
        /// <param name="TemplateFileName">Excel模板</param>
        /// <param name="p_SheetIndex">从哪个sheet开始，默认从第一个sheet</param>
        /// <param name="p_Values"></param>
        public void SetValue(int p_startRowIndex, int p_endRowIndex, int p_startColIndex, int p_endColIndex, System.Array p_Values, string TemplateFileName, int p_SheetIndex = 0)
        {
            p_endRowIndex += p_startRowIndex - 1;
            //遍历行与列
            int index1 = 0;
            int index2 = 0;
            for (int rowIndex = p_startRowIndex; rowIndex <= p_endRowIndex; rowIndex++)
            {
                index2 = 0;
                for (int colIndex = p_startColIndex; colIndex <= p_endColIndex; colIndex++)
                {
                    SetValue(rowIndex, colIndex, p_Values.GetValue(index1, index2).ToString(), TemplateFileName, p_SheetIndex);
                    index2++;
                }
                index1++;
            }
        }
        #endregion

        #region InsertPicture 在指定单元格插入图片 public
        /// <summary>
        /// 入图片到单元格区域，根据单元格区域自动缩放
        /// </summary>
        /// <param name="p_RowIndex">起始行号，从1开始</param>
        /// <param name="p_ColIndex">起始列号，从1开始</param>
        /// <param name="p_FileName">图片源文件完整路径</param>
        /// <param name="TemplateFileName">Excel模板</param>
        /// <param name="p_SheetIndex">从哪个sheet开始，默认从第一个sheet</param>
        public void InsertPicture(int p_RowIndex, int p_ColIndex, string p_FileName, string TemplateFileName, int p_SheetIndex = 0)
        {
            InsertPicture(p_RowIndex, p_RowIndex, p_ColIndex, p_ColIndex, p_FileName, 10, 10, TemplateFileName, p_SheetIndex);
        }

        /// <summary>
        /// 入图片到单元格区域，根据单元格区域自动缩放
        /// </summary>
        /// <param name="p_RowIndex">起始行号，从1开始</param>
        /// <param name="p_ColIndex">起始列号，从1开始</param>
        /// <param name="p_FileName">图片源文件完整路径</param>
        /// <param name="p_LeftRectify">图片相对单元格靠左的偏移值，最大值1023</param>
        /// <param name="p_TopRectify">图片相对单元格靠上的偏移值，最大值255</param>
        /// <param name="TemplateFileName">Excel模板</param>
        /// <param name="p_SheetIndex">从哪个sheet开始，默认从第一个sheet</param>
        public void InsertPicture(int p_RowIndex, int p_ColIndex, string p_FileName, int p_LeftRectify, int p_TopRectify, string TemplateFileName, int p_SheetIndex = 0)
        {
            InsertPicture(p_RowIndex, p_RowIndex, p_ColIndex, p_ColIndex, p_FileName, p_LeftRectify, p_TopRectify, TemplateFileName, p_SheetIndex);
        }

        /// <summary>
        /// 入图片到单元格区域，根据单元格区域自动缩放
        /// </summary>
        /// <param name="p_RowIndex">起始行号，从1开始</param>
        /// <param name="p_ColChars">起始列名，从A开始</param>
        /// <param name="p_FileName">图片源文件完整路径</param>
        /// <param name="TemplateFileName">Excel模板</param>
        /// <param name="p_SheetIndex">从哪个sheet开始，默认从第一个sheet</param>
        public void InsertPicture(int p_RowIndex, string p_ColChars, string p_FileName, string TemplateFileName, int p_SheetIndex = 0)
        {
            InsertPicture(p_RowIndex, p_RowIndex, ExcelColumnTranslator.ToIndex(p_ColChars), ExcelColumnTranslator.ToIndex(p_ColChars), p_FileName, 10, 10, TemplateFileName, p_SheetIndex);
        }

        /// <summary>
        /// 入图片到单元格区域，根据单元格区域自动缩放
        /// </summary>
        /// <param name="p_RowIndex">起始行号，从1开始</param>
        /// <param name="p_ColChars">起始列名，从A开始</param>
        /// <param name="p_FileName">图片源文件完整路径</param>
        /// <param name="p_LeftRectify">图片相对单元格靠左的偏移值，最大值1023</param>
        /// <param name="p_TopRectify">图片相对单元格靠上的偏移值，最大值255</param>
        /// <param name="TemplateFileName">Excel模板</param>
        /// <param name="p_SheetIndex">从哪个sheet开始，默认从第一个sheet</param>
        public void InsertPicture(int p_RowIndex, string p_ColChars, string p_FileName, int p_LeftRectify, int p_TopRectify, string TemplateFileName, int p_SheetIndex = 0)
        {
            InsertPicture(p_RowIndex, p_RowIndex, ExcelColumnTranslator.ToIndex(p_ColChars), ExcelColumnTranslator.ToIndex(p_ColChars), p_FileName, p_LeftRectify, p_TopRectify, TemplateFileName, p_SheetIndex);
        }


        /// <summary>
        /// 入图片到单元格区域，根据单元格区域自动缩放
        /// </summary>
        /// <param name="p_StartRowIndex">起始行号，从1开始</param>
        /// <param name="p_EndRowIndex">结束行号，从1开始</param>
        /// <param name="p_StartColChars">起始列名，从A开始</param>
        /// <param name="p_EndColChars">结束列名，从A开始</param>
        /// <param name="p_FileName">图片源文件完整路径</param>
        /// <param name="TemplateFileName">Excel模板</param>
        /// <param name="p_SheetIndex">从哪个sheet开始，默认从第一个sheet</param>
        public void InsertPicture(int p_StartRowIndex, int p_EndRowIndex, string p_StartColChars, string p_EndColChars, string p_FileName, string TemplateFileName, int p_SheetIndex = 0)
        {
            InsertPicture(p_StartRowIndex, p_EndRowIndex, ExcelColumnTranslator.ToIndex(p_StartColChars), ExcelColumnTranslator.ToIndex(p_EndColChars), p_FileName, 10, 10, TemplateFileName, p_SheetIndex);
        }


        /// <summary>
        /// 入图片到单元格区域，根据单元格区域自动缩放
        /// </summary>
        /// <param name="p_StartRowIndex">起始行号，从1开始</param>
        /// <param name="p_EndRowIndex">结束行号，从1开始</param>
        /// <param name="p_StartColChars">起始列名，从A开始</param>
        /// <param name="p_EndColChars">结束列名，从A开始</param>
        /// <param name="p_FileName">图片源文件完整路径</param>
        /// <param name="p_LeftRectify">图片相对单元格靠左的偏移值，最大值1023</param>
        /// <param name="p_TopRectify">图片相对单元格靠上的偏移值，最大值255</param>
        /// <param name="TemplateFileName">Excel模板</param>
        /// <param name="p_SheetIndex">从哪个sheet开始，默认从第一个sheet</param>
        public void InsertPicture(int p_StartRowIndex, int p_EndRowIndex, string p_StartColChars, string p_EndColChars, string p_FileName, int p_LeftRectify, int p_TopRectify, string TemplateFileName, int p_SheetIndex = 0)
        {
            InsertPicture(p_StartRowIndex, p_EndRowIndex, ExcelColumnTranslator.ToIndex(p_StartColChars), ExcelColumnTranslator.ToIndex(p_EndColChars), p_FileName, p_LeftRectify, p_TopRectify, TemplateFileName, p_SheetIndex);
        }

        /// <summary>
        /// 入图片到单元格区域，根据单元格区域自动缩放
        /// </summary>
        /// <param name="p_StartRowIndex">起始行号，从1开始</param>
        /// <param name="p_EndRowIndex">结束行号，从1开始</param>
        /// <param name="p_StartColumnIndex">起始列号，从1开始</param>
        /// <param name="p_EndColumnIndex">结束列号，从1开始</param>
        /// <param name="p_FileName">图片源文件完整路径</param>
        /// <param name="TemplateFileName">Excel模板</param>
        /// <param name="p_SheetIndex">从哪个sheet开始，默认从第一个sheet</param>
        public void InsertPicture(int p_StartRowIndex, int p_EndRowIndex, int p_StartColumnIndex, int p_EndColumnIndex, string p_FileName, string TemplateFileName, int p_SheetIndex = 0)
        {
            InsertPicture(p_StartRowIndex, p_EndRowIndex, p_StartColumnIndex, p_EndColumnIndex, p_FileName, 10, 10, TemplateFileName, p_SheetIndex);
        }

        /// <summary>
        /// 入图片到单元格区域，根据单元格区域自动缩放
        /// </summary>
        /// <param name="p_RowIndex">行号，从1开始</param>
        /// <param name="p_ColChars">列名，从A开始</param>
        /// <param name="p_Bitmap">图片源</param>
        /// <param name="TemplateFileName">Excel模板</param>
        /// <param name="p_SheetIndex">从哪个sheet开始，默认从第一个sheet</param>
        public void InsertPicture(int p_RowIndex, string p_ColChars, Bitmap p_Bitmap, string TemplateFileName, int p_SheetIndex = 0)
        {
            InsertPicture(p_RowIndex, p_RowIndex, ExcelColumnTranslator.ToIndex(p_ColChars), ExcelColumnTranslator.ToIndex(p_ColChars), p_Bitmap, 10, 10, TemplateFileName, p_SheetIndex);
        }

        /// <summary>
        /// 入图片到单元格区域，根据单元格区域自动缩放
        /// </summary>
        /// <param name="p_RowIndex">行号，从1开始</param>
        /// <param name="p_ColChars">列名，从A开始</param>
        /// <param name="p_Bitmap">图片源</param>
        /// <param name="p_LeftRectify">图片相对单元格靠左的偏移值，最大值1023</param>
        /// <param name="p_TopRectify">图片相对单元格靠上的偏移值，最大值255</param>
        /// <param name="TemplateFileName">Excel模板</param>
        /// <param name="p_SheetIndex">从哪个sheet开始，默认从第一个sheet</param>
        public void InsertPicture(int p_RowIndex, string p_ColChars, Bitmap p_Bitmap, int p_LeftRectify, int p_TopRectify, string TemplateFileName, int p_SheetIndex = 0)
        {
            InsertPicture(p_RowIndex, p_RowIndex, ExcelColumnTranslator.ToIndex(p_ColChars), ExcelColumnTranslator.ToIndex(p_ColChars), p_Bitmap, p_LeftRectify, p_TopRectify, TemplateFileName, p_SheetIndex);
        }

        /// <summary>
        /// 入图片到单元格区域，根据单元格区域自动缩放
        /// </summary>
        /// <param name="p_StartRowIndex">起始行号，从1开始</param>
        /// <param name="p_EndRowIndex">结束行号，从1开始</param>
        /// <param name="p_StartColChars">起始列名，从A开始</param>
        /// <param name="p_EndColChars">结束列名，从A开始</param>
        /// <param name="p_Bitmap">图片源</param>
        /// <param name="p_LeftRectify">图片相对单元格靠左的偏移值，最大值1023</param>
        /// <param name="p_TopRectify">图片相对单元格靠上的偏移值，最大值255</param>
        /// <param name="TemplateFileName">Excel模板</param>
        /// <param name="p_SheetIndex">从哪个sheet开始，默认从第一个sheet</param>
        public void InsertPicture(int p_StartRowIndex, int p_EndRowIndex, string p_StartColChars, string p_EndColChars, Bitmap p_Bitmap, int p_LeftRectify, int p_TopRectify, string TemplateFileName, int p_SheetIndex = 0)
        {
            InsertPicture(p_StartRowIndex, p_EndRowIndex, ExcelColumnTranslator.ToIndex(p_StartColChars), ExcelColumnTranslator.ToIndex(p_EndColChars), p_Bitmap, p_LeftRectify, p_TopRectify, TemplateFileName, p_SheetIndex);
        }

        /// <summary>
        /// 入图片到单元格区域，根据单元格区域自动缩放
        /// modify tjq 修改sheet获取方式，之前的方式会导致数据被清空
        /// </summary>
        /// <param name="p_StartRowIndex">起始行号，从1开始</param>
        /// <param name="p_EndRowIndex">结束行号，从1开始</param>
        /// <param name="p_StartColumnIndex">起始列号，从1开始</param>
        /// <param name="p_EndColumnIndex">结束列号，从1开始</param>
        /// <param name="p_Bitmap">图片源</param>
        /// <param name="p_LeftRectify">图片相对单元格靠左的偏移值，最大值1023</param>
        /// <param name="p_TopRectify">图片相对单元格靠上的偏移值，最大值255</param>
        /// <param name="TemplateFileName">Excel模板</param>
        /// <param name="p_SheetIndex">从哪个sheet开始，默认从第一个sheet</param>
        public void InsertPicture(int p_StartRowIndex, int p_EndRowIndex, int p_StartColumnIndex, int p_EndColumnIndex, Bitmap p_Bitmap, int p_LeftRectify, int p_TopRectify, string TemplateFileName, int p_SheetIndex = 0)
        {
            try
            {
                ISheet sheet = _workbook.GetSheetAt(p_SheetIndex);
                p_StartRowIndex = p_StartRowIndex - 1;
                p_EndRowIndex = p_EndRowIndex - 1;
                p_StartColumnIndex = p_StartColumnIndex - 1;
                p_EndColumnIndex = p_EndColumnIndex - 1;

                //缩放
                p_Bitmap = PictureZoom(p_StartRowIndex, p_EndRowIndex, p_StartColumnIndex, p_EndColumnIndex, p_Bitmap, sheet);

                //将图片对象添加到workbook中
                int pictureIdx = _workbook.AddPicture(FileUtility.ImageToByte(p_Bitmap), PictureType.JPEG);

                //创建一个顶层的图形绘画者。
                IDrawing patriarch = sheet.CreateDrawingPatriarch();
                var anchor = new HSSFClientAnchor
                {
                    Col1 = p_StartColumnIndex,
                    Row1 = p_StartRowIndex,
                    Col2 = p_EndColumnIndex + 1,
                    Row2 = p_EndRowIndex + 1
                };
                //显示图片
                var pict = (HSSFPicture)patriarch.CreatePicture(anchor, pictureIdx);
                //取消自动拉伸效果，此时anchor设置偏移量无效需要重新设置
                pict.Resize();
                pict.Anchor.Dx1 = p_LeftRectify;
                pict.Anchor.Dy1 = p_TopRectify;
            }
            catch (Exception ex)
            {
                throw new Exception("创建图片失败", ex);
            }
        }

        /// <summary>
        /// add tjq
        /// 插入图片到单元格区域，根据单元格区域自动缩放
        /// </summary>
        /// <param name="p_StartRowIndex"></param>
        /// <param name="p_EndRowIndex"></param>
        /// <param name="p_StartColumnIndex"></param>
        /// <param name="p_EndColumnIndex"></param>
        /// <param name="p_Bitmap"></param>
        /// <param name="p_LeftRectify"></param>
        /// <param name="p_TopRectify"></param>
        /// <param name="p_SheetName"></param>
        public void InsertPicture(int p_StartRowIndex, int p_EndRowIndex, int p_StartColumnIndex, int p_EndColumnIndex, Bitmap p_Bitmap, int p_LeftRectify, int p_TopRectify, string p_SheetName)
        {
            try
            {
                ISheet sheet = _workbook.GetSheet(p_SheetName);
                p_StartRowIndex = p_StartRowIndex - 1;
                p_EndRowIndex = p_EndRowIndex - 1;
                p_StartColumnIndex = p_StartColumnIndex - 1;
                p_EndColumnIndex = p_EndColumnIndex - 1;

                //缩放
                p_Bitmap = PictureZoom(p_StartRowIndex, p_EndRowIndex, p_StartColumnIndex, p_EndColumnIndex, p_Bitmap, sheet);

                //将图片对象添加到workbook中
                int pictureIdx = _workbook.AddPicture(FileUtility.ImageToByte(p_Bitmap), PictureType.JPEG);

                //创建一个顶层的图形绘画者。
                IDrawing patriarch = sheet.CreateDrawingPatriarch();
                var anchor = new HSSFClientAnchor
                {
                    Col1 = p_StartColumnIndex,
                    Row1 = p_StartRowIndex,
                    Col2 = p_EndColumnIndex + 1,
                    Row2 = p_EndRowIndex + 1
                };
                //显示图片
                var pict = (HSSFPicture)patriarch.CreatePicture(anchor, pictureIdx);
                //取消自动拉伸效果，此时anchor设置偏移量无效需要重新设置
                pict.Resize();
                pict.Anchor.Dx1 = p_LeftRectify;
                pict.Anchor.Dy1 = p_TopRectify;
            }
            catch (Exception ex)
            {
                throw new Exception("创建图片失败", ex);
            }
        }

        /// <summary>
        /// 入图片到单元格区域，根据单元格区域自动缩放
        /// </summary>
        /// <param name="p_StartRowIndex">起始行号，从1开始</param>
        /// <param name="p_EndRowIndex">结束行号，从1开始</param>
        /// <param name="p_StartColumnIndex">起始列号，从1开始</param>
        /// <param name="p_EndColumnIndex">结束列号，从1开始</param>
        /// <param name="p_LeftRectify">图片相对单元格靠左的偏移值，最大值1023</param>
        /// <param name="p_TopRectify">图片相对单元格靠上的偏移值，最大值255</param>
        /// <param name="p_FileName">图片源文件完整路径</param>
        /// <param name="TemplateFileName">Excel模板</param>
        /// <param name="p_SheetIndex">从哪个sheet开始，默认从第一个sheet</param>
        public void InsertPicture(int p_StartRowIndex, int p_EndRowIndex, int p_StartColumnIndex, int p_EndColumnIndex, string p_FileName, int p_LeftRectify, int p_TopRectify, string TemplateFileName, int p_SheetIndex = 0)
        {
            //将文件存到Bitmap
            var bitmap = new Bitmap(p_FileName);

            //插入
            InsertPicture(p_StartRowIndex, p_EndRowIndex, p_StartColumnIndex, p_EndColumnIndex, bitmap, p_LeftRectify, p_TopRectify, TemplateFileName, p_SheetIndex);
        }
        #endregion

        #region SetFormula  对单元格设置公式 public
        /// <summary>
        /// 对单元格设置公式
        /// </summary>
        /// <param name="p_RowIndex">行号，从1开始</param>
        /// <param name="p_ColChars">列名，从A开始</param>
        /// <param name="p_Formula">公式，如：A1*A2</param>
        /// <param name="TemplateFileName">Excel模板</param>
        /// <param name="p_SheetIndex">从哪个sheet开始，默认从第一个sheet</param>
        public void SetFormula(int p_RowIndex, string p_ColChars, string p_Formula, string TemplateFileName, int p_SheetIndex = 0)
        {
            SetFormula(p_RowIndex, ExcelColumnTranslator.ToIndex(p_ColChars), p_Formula, TemplateFileName, p_SheetIndex);
        }
        /// <summary>
        /// 对单元格设置公式
        /// </summary>
        /// <param name="p_RowIndex">行号，从1开始</param>
        /// <param name="p_ColumnIndex">列号，从1开始</param>
        /// <param name="p_Value">公式，如：A1*A2</param>
        /// <param name="TemplateFileName">Excel模板</param>
        /// <param name="p_SheetIndex">从哪个sheet开始，默认从第一个sheet</param>
        public void SetFormula(int p_RowIndex, int p_ColumnIndex, string p_Formula, string TemplateFileName, int p_SheetIndex = 0)
        {
            ISheet _sheet = OpenExcel(TemplateFileName, p_SheetIndex);
            _sheet.GetRow(p_RowIndex - 1).GetCell(p_ColumnIndex - 1).SetCellFormula(p_Formula);
            _sheet = null;
        }
        #endregion

        #region AddMergedRegion 合并单元格 public
        /// <summary>
        /// 添加合并，例如将C3:E5合并为一个单元格,那么调用例子为：
        /// AddMergedRegion(3,5,"C","E")
        /// </summary>
        /// <param name="p_StartRowIndex">开始行</param>
        /// <param name="p_EndRowIndex">结束行</param>
        /// <param name="p_StartColChars">开始列标识</param>
        /// <param name="p_EndColChars">结束列标识</param>
        /// <param name="TemplateFileName">Excel模板</param>
        /// <param name="p_SheetIndex">从哪个sheet开始，默认从第一个sheet</param>
        public void AddMergedRegion(int p_StartRowIndex, int p_EndRowIndex, string p_StartColChars, string p_EndColChars, string TemplateFileName, int p_SheetIndex = 0)
        {
            ISheet _sheet = OpenExcel(TemplateFileName, p_SheetIndex);
            _sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(p_StartRowIndex - 1, ExcelColumnTranslator.ToIndex(p_StartColChars) - 1, p_EndRowIndex - 1, ExcelColumnTranslator.ToIndex(p_EndColChars) - 1));
            _sheet = null;
        }
        #endregion

        #region CreateRows 在起始行上方插入新行,并赋值起始行的样式 public
        /// <summary>
        /// 在起始行上方插入新行,并赋值起始行的样式。
        /// </summary>
        /// <param name="p_StartRow">起始行号，从1开始</param>
        /// <param name="p_Count">所要创建的行数</param>
        /// <param name="TemplateFileName">Excel模板</param>
        /// <param name="p_SheetIndex">从哪个sheet开始，默认从第一个sheet</param>
        public void CreateRows(int p_StartRow, int p_Count, string TemplateFileName, int p_SheetIndex = 0)
        {
            p_Count = p_Count - 1;
            p_StartRow = p_StartRow;
            ISheet _sheet = OpenExcel(TemplateFileName, p_SheetIndex);
            IRow mySourceStyleRow = _sheet.GetRow(p_StartRow);//获取源格式行
            //_sheet.ShiftRows(p_StartRow + 1, this._sheet.LastRowNum, p_Count, true, false, true);
            ////调用插入行方法

            MyInsertRow(_sheet, p_StartRow, p_Count, mySourceStyleRow);
            _sheet = null;
        }

        /// <summary>
        /// 在起始行上方插入新行,并赋值起始行的样式。
        /// </summary>
        /// <param name="sheet">sheet</param>
        /// <param name="InsertRow">插入行</param>
        /// <param name="InsertCountRows">插入总行数</param>
        /// <param name="RowStyles">样式</param>
        private void MyInsertRow(ISheet sheet, int InsertRow, int InsertCountRows, IRow RowStyles)
        {
            #region 对批量移动后空出的空行插，创建相应的行，并以插入行的上一行为格式源(即：插入行-1的那一行)
            for (int i = InsertRow; i <= InsertRow + InsertCountRows - 1; i++)
            {
                IRow targetRow = null;
                ICell sourceCell = null;
                ICell targetCell = null;

                targetRow = sheet.CreateRow(i + 1);

                for (int m = RowStyles.FirstCellNum; m < RowStyles.LastCellNum; m++)
                {
                    sourceCell = RowStyles.GetCell(m);
                    if (sourceCell == null)
                        continue;
                    targetCell = targetRow.CreateCell(m);
                    targetCell.CellStyle = sourceCell.CellStyle;
                    targetCell.SetCellType(sourceCell.CellType);

                }
            }
            #endregion
        }

        /// <summary>
        /// 在起始行下方插入新行,并赋值起始行的样式
        /// add tjq 20160919
        /// </summary>
        /// <param name="p_StartRow">起始行号，与excel中一至</param>
        /// <param name="p_Count">行数</param>
        /// <param name="p_SheetName">sheet名</param>
        public void CreateRowsBySheetName(int p_StartRow, int p_Count, string p_SheetName)
        {
            var sheet = _workbook.GetSheet(p_SheetName);
            var mySourceStyleRow = sheet.GetRow(p_StartRow - 1);//获取源格式行

            //对批量移动后空出的空行插，创建相应的行，并以插入行的上一行为格式源(即：插入行-1的那一行)
            for (int i = p_StartRow; i < p_StartRow + p_Count; i++)
            {
                var targetRow = sheet.CreateRow(i);
                for (int m = mySourceStyleRow.FirstCellNum; m < mySourceStyleRow.LastCellNum; m++)
                {
                    var sourceCell = mySourceStyleRow.GetCell(m);
                    if (sourceCell == null)
                        continue;
                    var targetCell = targetRow.CreateCell(m);
                    targetCell.CellStyle = sourceCell.CellStyle;
                    targetCell.SetCellType(sourceCell.CellType);
                }
            }
        }
        #endregion

        #endregion

        #region Save 保存文件 public
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="p_FileName"></param>
        public void Save(string p_FileName)
        {
            using (var file = new FileStream(p_FileName, FileMode.Create))
            {
                _workbook.Write(file);
                file.Close();
            }

        }
        #endregion

        #region  导出方法 public

        #region ExportDataInFile 导出方法 生成临时文件 三种重载

        /// <summary>
        /// 模板导出
        /// </summary>
        /// <param name="p_DataSource">数据源 List或datatable</param>
        /// <param name="p_Columns">要导出列集合</param>
        /// <param name="p_SheetName">sheet名称</param>
        /// <param name="p_FileName">临时文件</param>
        /// <param name="p_TemplateFileName">模板文件</param>
        /// <param name="p_FirstRow">内容开始行，从1开始。默认从第二行开始</param>
        public void ExportDataInFile(Object p_DataSource, List<ExcelColumInfo> p_Columns, string p_SheetName, string p_FileName, string p_TemplateFileName, int p_FirstRow = 2)
        {
            ExportDataInFile(p_DataSource, p_Columns, p_SheetName, p_FileName, p_TemplateFileName, null, p_FirstRow);
        }

        /// <summary>
        /// 无模板导出
        /// </summary>
        ///  <param name="p_DataSource">数据源 List或datatable</param>
        /// <param name="p_Columns">要导出列集合</param>
        /// <param name="p_SheetName">sheet名称</param>
        /// <param name="p_FileName">临时文件</param>
        /// <param name="p_FirstRow">内容开始行，从1开始。默认从第二行开始</param>
        public void ExportDataInFile(Object p_DataSource, List<ExcelColumInfo> p_Columns, string p_SheetName, string p_FileName, int p_FirstRow = 2)
        {
            ExportDataInFile(p_DataSource, p_Columns, p_SheetName, p_FileName, "", null, p_FirstRow);
        }

        /// <summary>
        /// 无模板导出 带文档信息
        /// </summary>
        /// <param name="p_DataSource">数据源 List或datatable</param>
        /// <param name="p_Columns">要导出列集合</param>
        /// <param name="p_SheetName">sheet名称</param>
        /// <param name="p_FileName">临时文件</param>
        /// <param name="p_Summary">文档信息</param>
        /// <param name="p_FirstRow">内容开始行，从1开始。默认从第二行开始</param>
        public void ExportDataInFile(Object p_DataSource, List<ExcelColumInfo> p_Columns, string p_SheetName, string p_FileName, DocumentSummary p_Summary, int p_FirstRow = 2)
        {
            ExportDataInFile(p_DataSource, p_Columns, p_SheetName, p_FileName, "", p_Summary, p_FirstRow);
        }

        #endregion

        #region  ExportData 导出方法 返回byte[] 8种重载
        #region ExportData 非模板导出 4中重载
        /// <summary>
        /// 非模板导出数据源全部列
        /// </summary>
        /// <param name="p_DataSource">数据源</param>
        /// <param name="p_SheetName">sheet名称</param>
        /// <param name="p_Path">临时文件路径</param>
        /// <param name="p_Summary">文档信息</param>
        /// <param name="p_FirstRow">内容开始行，从1开始。默认从第二行开始</param>
        /// <returns></returns>
        public byte[] ExportData(Object p_DataSource, string p_SheetName, string p_Path, DocumentSummary p_Summary, int p_FirstRow = 2)
        {
            return ExportData(p_DataSource, GetExcelColumInfos(p_DataSource), p_SheetName, p_Path, _maxSheetRowCount, "", p_Summary, p_FirstRow);
        }

        /// <summary>
        ///  非模板导出数据源全部列 多个sheet导出
        /// </summary>
        /// <param name="p_DataSource">数据源</param>
        /// <param name="p_SheetName">sheet名称</param>
        /// <param name="p_Path">临时文件路径</param>
        /// <param name="p_SheetRowCount">单个Sheet最大行数，范围从1-65534</param>
        /// <param name="p_Summary">文档信息</param>
        /// <param name="p_FirstRow">内容开始行，从1开始。默认从第二行开始</param>
        /// <returns></returns>
        public byte[] ExportData(Object p_DataSource, string p_SheetName, string p_Path, int p_SheetRowCount, DocumentSummary p_Summary, int p_FirstRow = 2)
        {
            return ExportData(p_DataSource, GetExcelColumInfos(p_DataSource), p_SheetName, p_Path, p_SheetRowCount, "", p_Summary, p_FirstRow);
        }

        /// <summary>
        ///  非模板选择性导出 多个sheet导出
        /// </summary>
        /// <param name="p_DataSource"></param>
        /// <param name="p_Columns"></param>
        /// <param name="p_SheetName"></param>
        /// <param name="p_Path"></param>
        /// <param name="p_Summary"></param>
        /// <param name="p_FirstRow"></param>
        /// <returns></returns>
        public byte[] ExportData(Object p_DataSource, List<ExcelColumInfo> p_Columns, string p_SheetName, string p_Path, DocumentSummary p_Summary, int p_FirstRow = 2)
        {
            return ExportData(p_DataSource, p_Columns, p_SheetName, p_Path, _maxSheetRowCount, "", p_Summary, p_FirstRow);
        }

        /// <summary>
        /// 非模板选择性导出 多个sheet导出
        /// </summary>
        /// <param name="p_DataSource"></param>
        /// <param name="p_Columns"></param>
        /// <param name="p_SheetName"></param>
        /// <param name="p_Path"></param>
        /// <param name="p_SheetRowCount"></param>
        /// <param name="p_Summary"></param>
        /// <param name="p_FirstRow"></param>
        /// <returns></returns>
        public byte[] ExportData(Object p_DataSource, List<ExcelColumInfo> p_Columns, string p_SheetName, string p_Path, int p_SheetRowCount, DocumentSummary p_Summary, int p_FirstRow = 2)
        {
            return ExportData(p_DataSource, p_Columns, p_SheetName, p_Path, p_SheetRowCount, "", p_Summary, p_FirstRow);

        }
        #endregion

        #region  ExportData 模板导出 4中重载
        /// <summary>
        ///  模板导出数据源全部列
        /// </summary>
        /// <param name="p_DataSource"></param>
        /// <param name="p_SheetName"></param>
        /// <param name="p_Path"></param>
        /// <param name="p_TemplateFileName"></param>
        /// <param name="p_FirstRow"></param>
        /// <returns></returns>
        public byte[] ExportData(Object p_DataSource, string p_SheetName, string p_Path, string p_TemplateFileName, int p_FirstRow = 2)
        {
            return ExportData(p_DataSource, GetExcelColumInfos(p_DataSource), p_SheetName, p_Path, _maxSheetRowCount, p_TemplateFileName, null, p_FirstRow);
        }


        /// <summary>
        ///  模板导出数据源全部列 多个sheet导出
        /// </summary>
        /// <param name="p_DataSource"></param>
        /// <param name="p_SheetName"></param>
        /// <param name="p_Path"></param>
        /// <param name="p_SheetRowCount"></param>
        /// <param name="p_TemplateFileName"></param>
        /// <param name="p_Summary"></param>
        /// <param name="p_FirstRow"></param>
        /// <returns></returns>
        public byte[] ExportData(Object p_DataSource, string p_SheetName, string p_Path, int p_SheetRowCount, string p_TemplateFileName, DocumentSummary p_Summary, int p_FirstRow = 2)
        {
            return ExportData(p_DataSource, GetExcelColumInfos(p_DataSource), p_SheetName, p_Path, p_SheetRowCount, p_TemplateFileName, null, p_FirstRow);
        }

        /// <summary>
        /// 模板选择性导出 多个sheet导出
        /// </summary>
        /// <param name="p_DataSource"></param>
        /// <param name="p_Columns"></param>
        /// <param name="p_SheetName"></param>
        /// <param name="p_Path"></param>
        /// <param name="p_TemplateFileName"></param>
        /// <param name="p_FirstRow"></param>
        /// <returns></returns>
        public byte[] ExportData(Object p_DataSource, List<ExcelColumInfo> p_Columns, string p_SheetName, string p_Path, string p_TemplateFileName, int p_FirstRow = 2)
        {
            return ExportData(p_DataSource, p_Columns, p_SheetName, p_Path, _maxSheetRowCount, p_TemplateFileName, null, p_FirstRow);
        }


        /// <summary>
        /// 模板选择性导出 单sheet
        /// </summary>
        /// <param name="p_DataSource"></param>
        /// <param name="p_Columns"></param>
        /// <param name="p_SheetName"></param>
        /// <param name="p_Path"></param>
        /// <param name="p_TemplateFileName"></param>
        /// <param name="p_Summary"></param>
        /// <param name="p_FirstRow"></param>
        /// <returns></returns>
        public byte[] ExportData(Object p_DataSource, List<ExcelColumInfo> p_Columns, string p_SheetName, string p_Path, string p_TemplateFileName, DocumentSummary p_Summary, int p_FirstRow = 2)
        {
            return ExportData(p_DataSource, p_Columns, p_SheetName, p_Path, _maxSheetRowCount, p_TemplateFileName, p_Summary, p_FirstRow);
        }
        #endregion
        #endregion

        #endregion

        #region  导出方法准备 private
        /// <summary>
        /// 方法作用: 判断数据源类型
        /// </summary>
        /// <param name="p_DataSource">数据源</param>
        /// <param name="p_Columns">要导出列集合</param>
        /// <param name="p_SheetName">sheet名称。模板导出时sheet名称应与模板sheet值一致</param>
        /// <param name="p_FileName">临时文件</param>
        /// <param name="p_TemplateFileName">模板文件</param>
        /// <param name="p_Summary">文档信息，只对非模板导出起作用，模板导出时可传null</param>
        /// <param name="p_FirstRow">内容开始行,从1开始。默认从第二行开始写内容</param>
        private void ExportDataInFile(Object p_DataSource, List<ExcelColumInfo> p_Columns, string p_SheetName, string p_FileName, string p_TemplateFileName, DocumentSummary p_Summary, int p_FirstRow = 2)
        {
            if (p_DataSource.GetType().Name.ToLower().Contains("datatable"))
            {
                ExportDataInFile((DataTable)p_DataSource, p_Columns, p_SheetName, p_FileName, p_TemplateFileName, p_Summary, p_FirstRow);
            }
            else if (p_DataSource is IList)
            {
                ExportDataInFile((IList)p_DataSource, p_Columns, p_SheetName, p_FileName, p_TemplateFileName,
                    p_Summary, p_FirstRow);
            }
            else
            {
                throw new Exception("导出Excel时出错，数据源的类型是无效的类型，目前只支持DataTable和集合导出。");
            }
        }

        /// <summary>
        /// 返回byte[]字节数组
        /// </summary>
        /// <param name="p_DataSource">数据源</param>
        /// <param name="p_Columns">要导出列集合</param>
        /// <param name="p_SheetName"></param>
        /// <param name="p_Path">临时文件目录</param>
        /// <param name="p_SheetRowCount">单个Sheet最大行数，范围从1-65534，值越小产生Sheet数越多性能越低，建议设置为适合大小。非业务需求一般都设置为65534。</param>
        /// <param name="p_TemplateFileName">模板文件</param>
        /// <param name="p_Summary">文档信息</param>
        /// <returns>buye[]</returns>
        private byte[] ExportData(Object p_DataSource, List<ExcelColumInfo> p_Columns, string p_SheetName, string p_Path, int p_SheetRowCount, string p_TemplateFileName, DocumentSummary p_Summary, int p_FirstRow = 2)
        {
            _maxSheetRowCount = p_SheetRowCount;
            p_Path = string.Format("{0}\\{1}.xls", p_Path, Guid.NewGuid().ToString("N"));
            ExportDataInFile(p_DataSource, p_Columns, p_SheetName, p_Path, p_TemplateFileName, p_Summary, p_FirstRow);

            return ExcelToByte(p_Path);
        }
        #endregion

        #region 导出方法private

        /// <summary>
        /// 导出数据到Excel文件，在使用完导出的文件之后请主动删除。List集合数据源
        /// </summary>
        /// <param name="p_DataSource">数据源</param>
        /// <param name="p_Columns">需要导出的数据列信息</param>
        /// <param name="p_SheetName">期望的Sheet名</param>
        /// <param name="p_FileName">存放临时文件名</param>
        /// <param name="p_TemplateFileName">模板文件名</param>
        /// <param name="p_Summary">文件属性中的摘要信息，可为Null，模板导出模式时无效</param>
        /// <returns></returns>
        protected void ExportDataInFile(IList p_DataSource, List<ExcelColumInfo> p_Columns, string p_SheetName, string p_FileName, string p_TemplateFileName, DocumentSummary p_Summary, int p_FirstRow = 2)
        {
            if (p_DataSource.Count == 0)
            {
                throw new Exception("导出Excel时出错，数据源行数不能为0。");
            }

            if (_maxSheetRowCount <= 0 || this._maxSheetRowCount >= 65535)
            {
                throw new Exception("导出Excel时出错，单个Sheet行数范围必须在1-65534内。");
            }

            if (File.Exists(p_FileName))
            {
                throw new Exception("导出Excel时出错，临时文件已存在。");
            }

            if (p_TemplateFileName != "" && !File.Exists(p_TemplateFileName))
            {
                throw new Exception("导出Excel时出错，模板文件不存在。");
            }

            try
            {
                //根据行数计算工作表的个数
                int intSheetCount = GetSheetCount(p_DataSource.Count, _maxSheetRowCount);

                //创建Excel文件
                if (p_TemplateFileName == "")
                {
                    //创建Excel同时创建空Sheet（无行无列）
                    CreateExcel(p_SheetName, intSheetCount);
                }
                else
                {
                    //根据模板文件创建Sheet
                    CreateExcel(p_SheetName, intSheetCount, p_TemplateFileName);
                }

                //使用循环导出数据 非模板导出
                if (p_TemplateFileName == "")
                {
                    WriteExcel(p_DataSource, p_Columns, p_SheetName, false, p_FirstRow);

                    //设置表头
                    SetSheetHeader(intSheetCount, p_Columns.Count);

                    //设置文档信息
                    SetInformation(p_Summary);
                }
                else//模板导出
                {
                    WriteExcel(p_DataSource, p_Columns, p_SheetName, true, p_FirstRow);
                }

                //保存并关闭Excel
                SaveExcel(p_FileName);


            }
            catch (Exception e)
            {
                _workbook = null;
                //最后删除文件
                if (File.Exists(p_FileName))
                    File.Delete(p_FileName);
                throw new Exception("方法ExportDataInFile出错", e);
            }
        }



        /// <summary>
        /// 导出数据到Excel文件，在使用完导出的文件之后请主动删除。List集合数据源
        /// </summary>
        /// <param name="p_DataSource">数据源</param>
        /// <param name="p_Columns">需要导出的数据列信息</param>
        /// <param name="p_SheetName">期望的Sheet名</param>
        /// <param name="p_FileName">存放临时文件名</param>
        /// <param name="p_TemplateFileName">模板文件名</param>
        /// <param name="p_Summary">文件属性中的摘要信息，可为Null，模板导出模式时无效</param>
        /// <returns></returns>
        public void ExportDataInFile(IList p_DataSource, string headName, string p_SheetName, string p_FileName, string p_TemplateFileName)
        {
            if (p_DataSource.Count == 0)
            {
                throw new Exception("导出Excel时出错，数据源行数不能为0。");
            }

            if (_maxSheetRowCount <= 0 || this._maxSheetRowCount >= 65535)
            {
                throw new Exception("导出Excel时出错，单个Sheet行数范围必须在1-65534内。");
            }

            if (File.Exists(p_FileName))
            {
                throw new Exception("导出Excel时出错，临时文件已存在。");
            }

            if (p_TemplateFileName != "" && !File.Exists(p_TemplateFileName))
            {
                throw new Exception("导出Excel时出错，模板文件不存在。");
            }

            try
            {
                //根据行数计算工作表的个数
                int intSheetCount = GetSheetCount(p_DataSource.Count, _maxSheetRowCount);

                //创建Excel文件
                if (p_TemplateFileName == "")
                {
                    //创建Excel同时创建空Sheet（无行无列）
                    CreateExcel(p_SheetName, intSheetCount);
                }
                else
                {
                    //根据模板文件创建Sheet
                    CreateExcel(p_SheetName, intSheetCount, p_TemplateFileName);
                }


                //  InsertExcelValueByHeadName(p_TemplateFileName, p_SheetName, p_DataSource, modelProproperty, headName, p_FirstRow);


                //保存并关闭Excel
                SaveExcel(p_FileName);


            }
            catch (Exception e)
            {
                _workbook = null;
                //最后删除文件
                if (File.Exists(p_FileName))
                    File.Delete(p_FileName);
                throw new Exception("方法ExportDataInFile出错", e);
            }
        }
        /// <summary>
        /// 导出数据到Excel文件，在使用完导出的文件之后请主动删除。DataTable数据源
        /// </summary>
        /// <param name="p_DataSource">数据源</param>
        /// <param name="p_Columns">需要导出的数据列信息</param>
        /// <param name="p_SheetName">期望的Sheet名</param>
        /// <param name="p_FileName">存放临时文件名</param>
        /// <param name="p_TemplateFileName">模板文件名</param>
        /// <param name="p_Summary">文件属性中的摘要信息，可为Null，模板导出模式时无效</param>
        /// <returns></returns>
        protected void ExportDataInFile(DataTable p_DataSource, List<ExcelColumInfo> p_Columns, string p_SheetName, string p_FileName, string p_TemplateFileName, DocumentSummary p_Summary, int p_FirstRow = 2)
        {
            if (p_DataSource.Rows.Count == 0)
            {
                throw new Exception("导出Excel时出错，数据源行数不能为0。");
            }

            if (_maxSheetRowCount <= 0 || _maxSheetRowCount >= 65535)
            {
                throw new Exception("导出Excel时出错，单个Sheet行数范围必须在1-65534内。");
            }

            if (File.Exists(p_FileName))
            {
                throw new Exception("导出Excel时出错，临时文件已存在。");
            }

            if (p_TemplateFileName != "" && !File.Exists(p_TemplateFileName))
            {
                throw new Exception("导出Excel时出错，模板文件不存在。");
            }

            try
            {
                int intSheetCount = GetSheetCount(p_DataSource.Rows.Count, _maxSheetRowCount);

                //1、创建
                if (p_TemplateFileName == "")
                {
                    CreateExcel(p_SheetName, intSheetCount); //创建Excel同时创建空Sheet（无行无列）
                }
                else
                {
                    CreateExcel(p_SheetName, intSheetCount, p_TemplateFileName);//根据模板文件创建Sheet
                }

                //2、使用循环导出数据
                if (p_TemplateFileName == "")
                {
                    WriteExcel(p_DataSource, p_Columns, p_SheetName, false, p_FirstRow);

                    //设置表头
                    SetSheetHeader(intSheetCount, p_Columns.Count);

                    //设置文档信息
                    SetInformation(null);
                }
                else
                {
                    WriteExcel(p_DataSource, p_Columns, p_SheetName, true, p_FirstRow);
                }

                //3、保存并关闭Excel
                SaveExcel(p_FileName);

            }
            catch (Exception e)
            {
                _workbook = null;

                //最后删除文件
                if (File.Exists(p_FileName))
                {
                    File.Delete(p_FileName);
                }

                throw new Exception("方法ExportDataInFile出错", e);
            }
        }
        #endregion

        #region 辅助方法private

        #region CreateExcel 创建Excel文件 两种重载方法 1、创建空Excel文件 2、根据模板创建Excel
        /// <summary>
        /// 创建一个空的Excel文件，没有列没有行。
        /// </summary>
        /// <param name="p_SheetName">表名</param>
        /// <param name="p_SheetCount">Sheet个数</param>
        public void CreateExcel(string p_SheetName, int p_SheetCount)
        {
            //Todo:以下将来需要改为使用接口来创建 
            _workbook = new HSSFWorkbook();
            try
            {
                if (p_SheetCount == 0)
                {
                    throw new Exception("创建Excel文件时出错，异常信息：Sheet的数量不能为0。");
                }

                //动态创建Sheet，为数据超过限制分Sheet准备
                for (int i = 0; i < p_SheetCount; i++)
                {
                    if (i == 0)
                    {
                        _workbook.CreateSheet(p_SheetName);
                    }
                    else
                    {
                        int intIndex = i + 1;
                        _workbook.CreateSheet(p_SheetName + Convert.ToString(intIndex));
                    }
                }
            }
            catch (Exception e)
            {
                _workbook = null;
                throw new Exception("创建Excel文件时出错", e);
            }
        }

        /// <summary>
        /// 创建基于模板的Excel
        /// </summary>
        /// <param name="p_SheetName">期望的SheetName</param>
        /// <param name="p_SheetCount">个数</param>
        /// <param name="p_FileName">模板文件名</param>
        public void CreateExcel(string p_SheetName, int p_SheetCount, string p_FileName)
        {
            try
            {
                using (var file = new FileStream(p_FileName, FileMode.Open, FileAccess.Read))
                {
                    _workbook = WorkbookFactory.Create(file);
                }

                //string sheetName = _workbook.GetSheetName(0);
                string sheetName = p_SheetName;
                for (int i = 1; i < p_SheetCount; i++)
                {
                    int index = i + 1;
                    _workbook.SetSheetName(i, sheetName + Convert.ToString(index));
                }
            }
            catch (Exception e)
            {
                _workbook = null;
                throw new Exception("创建基于模板的Excel文件时出错", e);
            }
        }

        /// <summary>
        /// 创建基于模板的Excel
        /// </summary>
        /// <param name="p_FileName">模板文件名</param>
        public void CreateExcel(string p_FileName)
        {
            try
            {
                using (var file = new FileStream(p_FileName, FileMode.Open, FileAccess.Read))
                {
                    _workbook = WorkbookFactory.Create(file);
                }
            }
            catch (Exception e)
            {
                _workbook = null;
                throw new Exception("创建基于模板的Excel文件时出错", e);
            }
        }

        /// <summary>
        /// 创建基于模板的Excel，根据提供的sheet名映射，复制相应的sheet
        /// add tjq 20160918
        /// </summary>
        /// <param name="p_SheetNames">sheet键值对《新sheet名,原模板sheet名》</param>
        /// <param name="p_FileName">模板文件名</param>
        public void CreateExcel(Dictionary<string,string> p_SheetNames, string p_FileName)
        {
            try
            {
                if (p_SheetNames.Count <= 0)
                {
                    throw new Exception("创建Excel文件时出错，异常信息：Sheet的数量不能为0。");
                }
                
                using (var file = new FileStream(p_FileName, FileMode.Open, FileAccess.Read))
                {
                    _workbook = WorkbookFactory.Create(file);
                    int tplCount = _workbook.NumberOfSheets;//数模板sheet
                    //复制模板sheet
                    foreach (var sheetNameKV in p_SheetNames)
                    {
                        if (_workbook.GetSheetIndex(sheetNameKV.Value) < 0)
                        {
                            throw new Exception("创建Excel文件时出错，异常信息：Sheet(" + sheetNameKV.Value + ")不存在。");
                        }
                        var newSheet = _workbook.CloneSheet(_workbook.GetSheetIndex(sheetNameKV.Value));
                        _workbook.SetSheetName(_workbook.GetSheetIndex(newSheet), sheetNameKV.Key);
                    }
                    //移除模板sheet
                    for (int i = 0; i < tplCount; i++)
                    {
                        _workbook.RemoveSheetAt(0);
                    }
                }
            }
            catch (Exception e)
            {
                _workbook = null;
                throw new Exception("创建基于模板的Excel文件时出错", e);
            }
        }

        #endregion

        #region GetSheetCount 根据行数计算工作表的个数。
        /// <summary>
        /// 根据行数计算工作表的个数。
        /// </summary>
        /// <param name="p_Count">数据总行数。</param>
        /// <param name="p_SheetRowCount">每个工作表的总行数。</param>
        /// <returns></returns>
        private int GetSheetCount(int p_Count, int p_SheetRowCount)
        {
            if (p_Count == 0)
            {
                return 1;
            }

            int bookCount = p_Count / p_SheetRowCount;
            if (bookCount <= 0)
            {
                return 1;
            }

            int Mo = p_Count % p_SheetRowCount;
            if (Mo > 0)
            {
                return bookCount + 1;
            }

            return bookCount;
        }
        #endregion

        #region WriteExcel 写入Excel内容 两种重载方法 数据源为list集合和Datable
        /// <summary>
        /// 导出数据 
        /// </summary>
        /// <param name="p_List">数据集合</param>
        /// <param name="p_Columns">要导出数据列集合</param>
        /// <param name="p_SheetName">sheet名字</param>
        /// <param name="p_Template">是否使用模板 true:使用模板 false：不使用模板</param>
        /// <param name="p_FirstRow">内容开始行从1开始，默认值从第二行开始导出</param>
        private void WriteExcel(IList p_List, List<ExcelColumInfo> p_Columns, string p_SheetName, bool p_Template, int p_FirstRow = 2)
        {

            try
            {
                #region  用循环导出数据。
                int rowIndexWhichInsert = 0;
                int k = 1;//记录是第几个Sheet
                string tempSheetName = p_SheetName;
                //将数据插入到Tables中
                if (p_Template)
                {
                    rowIndexWhichInsert = p_FirstRow;//用于记录更新到了Sheet的哪一行
                }
                //内容单元格样式
                ICellStyle valueCellStyle = _workbook.CreateCellStyle();
                valueCellStyle.BorderBottom = BorderStyle.Thin;//设置四个边框
                valueCellStyle.BorderLeft = BorderStyle.Thin;
                valueCellStyle.BorderRight = BorderStyle.Thin;
                valueCellStyle.BorderTop = BorderStyle.Thin;
                valueCellStyle.WrapText = true;//自动换行

                //内容单元格样式
                PropertyInfo[] propertys = p_List[0].GetType().GetProperties();

                //这种循环的写法比以前的大大提高了效率
                for (int i = 0; i < p_List.Count; i++)
                {
                    rowIndexWhichInsert++;
                    //第一个或下一个Sheet，这里是这个循环的关键
                    if (i < _maxSheetRowCount * k)
                    {
                        if (k > 1)
                        {
                            tempSheetName = p_SheetName + k;
                        }

                        //导入数据之前先创建表头行和列 非模板导出 第一个sheet
                        if (rowIndexWhichInsert == 1 & !p_Template)
                        {
                            CreateSheetHeard(tempSheetName, p_Columns, p_FirstRow);
                        }

                        if (!p_Template || rowIndexWhichInsert > 1) //下一个sheet
                        {
                            //为导出数据创建内容行和列 
                            CreateSheetRowsAndColumns(tempSheetName, rowIndexWhichInsert, p_Columns, p_Template, valueCellStyle);
                        }

                        ISheet sheet = _workbook.GetSheet(tempSheetName);
                        IRow row = sheet.GetRow(rowIndexWhichInsert);
                        //对单元格赋值;
                        for (int j = 0; j < p_Columns.Count; j++)
                        {
                            //这里要根据p_Columns中有的数据列来赋值这样就支持选择性导出，
                            //DataType要对应模板中单元格个数这样就使得模板起作用。

                            var cellValueobject = propertys[p_Columns[j].PropertyIndex].GetValue(p_List[i], null);
                            if (cellValueobject != null)
                            {
                                var cellValue = cellValueobject.ToString();
                                if (p_Template)
                                {
                                    #region 模板导出单元格赋值

                                    if (p_Columns[j].DataType == DataType.Text)
                                    {
                                        row.GetCell(j).SetCellValue(cellValue);
                                    }
                                    else if (p_Columns[j].DataType == DataType.DateTime)
                                    {
                                        row.GetCell(j).SetCellValue(DateTime.Parse(cellValue));
                                    }
                                    else if (p_Columns[j].DataType == DataType.Number)
                                    {
                                        row.GetCell(j).SetCellValue(double.Parse(cellValue));
                                    }
                                    else
                                    {
                                        row.GetCell(j).SetCellValue(cellValue);
                                    }

                                    #endregion
                                }
                                else
                                {
                                    #region 非模板导出 单元格赋值

                                    if (p_Columns[j].Format != null)
                                    //这里的if else写得有些繁琐但运行效率高,这里的double类型可能在一些运算上精度不够，以后需要改进
                                    {
                                        if (p_Columns[j].DataType == DataType.Text)
                                        {
                                            row.GetCell(j).SetCellValue(cellValue);
                                        }
                                        else if (p_Columns[j].DataType == DataType.DateTime)
                                        {
                                            row.GetCell(j)
                                                .SetCellValue(string.Format(p_Columns[j].Format,
                                                    DateTime.Parse(cellValue)));
                                        }
                                        else if (p_Columns[j].DataType == DataType.Number)
                                        {
                                            row.GetCell(j)
                                                .SetCellValue(string.Format(p_Columns[j].Format,
                                                    double.Parse(cellValue)));
                                        }
                                        else
                                        {
                                            row.GetCell(j)
                                                .SetCellValue(string.Format(p_Columns[j].Format, cellValue));
                                        }
                                    }
                                    else
                                    {
                                        if (p_Columns[j].DataType == DataType.Text)
                                        {
                                            row.GetCell(j).SetCellValue(cellValue);
                                        }
                                        else if (p_Columns[j].DataType == DataType.DateTime)
                                        {
                                            row.GetCell(j).SetCellValue(DateTime.Parse(cellValue));
                                        }
                                        else if (p_Columns[j].DataType == DataType.Number)
                                        {
                                            row.GetCell(j).SetCellValue(double.Parse(cellValue));
                                        }
                                        else
                                        {
                                            row.GetCell(j).SetCellValue(cellValue);
                                        }
                                    }

                                    #endregion
                                }
                            }

                        }
                    }
                    else//表明已经进入下一个Sheet
                    {
                        k++;//进入下一个Sheet
                        rowIndexWhichInsert = 0;
                    }
                }
                #endregion
            }
            catch (Exception e)
            {
                _workbook = null;
                throw new Exception("循环导出数据时出错", e);
            }
        }

        /// <summary>
        /// 导出数据
        /// </summary>
        /// <param name="p_DataSource">数据集合</param>
        /// <param name="p_Columns">要导出数据列集合</param>
        /// <param name="p_SheetName">sheet名字</param>
        /// <param name="p_Template">是否使用模板 true:使用模板 false：不使用模板</param>
        /// <param name="p_FirstRow">内容开始行从1开始，默认值从第二行开始导出</param>
        private void WriteExcel(DataTable p_DataSource, List<ExcelColumInfo> p_Columns, string p_SheetName, bool p_Template, int p_FirstRow = 2)
        {
            try
            {
                #region  用循环导出数据

                int k = 1;//记录是第几个Sheet
                string tempSheetName = p_SheetName;

                //将数据插入到Tables中
                int rowIndexWhichInsert = 0;//用于记录更新到了Sheet的哪一行
                if (p_Template && p_FirstRow > 1)
                {
                    rowIndexWhichInsert = p_FirstRow - 2;
                }

                //内容单元格样式
                ICellStyle valueCellStyle = _workbook.CreateCellStyle();
                valueCellStyle.BorderBottom = BorderStyle.Thin;//设置四个边框
                valueCellStyle.BorderLeft = BorderStyle.Thin;
                valueCellStyle.BorderRight = BorderStyle.Thin;
                valueCellStyle.BorderTop = BorderStyle.Thin;
                valueCellStyle.WrapText = true;//自动换行

                //内容单元格样式
                for (int i = 0; i < p_DataSource.Rows.Count; i++)//这种循环的写法比以前的大大提高了效率
                {
                    rowIndexWhichInsert++;
                    if (i < _maxSheetRowCount * k)//第一个或下一个Sheet，这里是这个循环的关键
                    {
                        if (k > 1)
                        {
                            tempSheetName = p_SheetName + k;
                        }

                        //导入数据之前先创建行和列
                        if (rowIndexWhichInsert == 1 & !p_Template)
                        {
                            CreateSheetHeard(tempSheetName, p_Columns, p_FirstRow);
                        }

                        //为导出数据创建行和列                        
                        if (!p_Template || rowIndexWhichInsert > 0)
                        {
                            CreateSheetRowsAndColumns(tempSheetName, rowIndexWhichInsert, p_Columns, p_Template, valueCellStyle);
                        }

                        IRow row = _workbook.GetSheet(tempSheetName).GetRow(rowIndexWhichInsert);

                        //对单元格赋值;
                        for (int j = 0; j < p_Columns.Count; j++)
                        {//这里要根据p_Columns中有的数据列来赋值这样就支持选择性导出，DataType要对应模板中单元格个数这样就使得模板起作用。
                            try
                            {
                                var cellValue = p_DataSource.Rows[i][p_Columns[j].TableColumnName].ToString();

                                if (p_Template)
                                {
                                    if (p_Columns[j].DataType == DataType.Text)
                                    {
                                        row.GetCell(j).SetCellValue(cellValue);
                                    }
                                    else if (p_Columns[j].DataType == DataType.DateTime)
                                    {
                                        row.GetCell(j).SetCellValue(DateTime.Parse(cellValue));
                                    }
                                    else if (p_Columns[j].DataType == DataType.Number)
                                    {
                                        row.GetCell(j).SetCellValue(double.Parse(cellValue));
                                    }
                                    else
                                    {
                                        row.GetCell(j).SetCellValue(cellValue);
                                    }
                                }
                                else
                                {
                                    if (p_Columns[j].Format != null)//这里的if else写得有些繁琐但运行效率高,这里的double类型可能在一些运算上精度不够，以后需要改进
                                    {
                                        if (p_Columns[j].DataType == DataType.Text)
                                        {
                                            row.GetCell(j).SetCellValue(string.Format(p_Columns[j].Format, cellValue));
                                        }
                                        else if (p_Columns[j].DataType == DataType.DateTime)
                                        {
                                            row.GetCell(j).SetCellValue(string.Format(p_Columns[j].Format, DateTime.Parse(cellValue)));
                                        }
                                        else if (p_Columns[j].DataType == DataType.Number)
                                        {
                                            row.GetCell(j).SetCellValue(string.Format(p_Columns[j].Format, double.Parse(cellValue)));
                                        }
                                        else
                                        {
                                            row.GetCell(j).SetCellValue(string.Format(p_Columns[j].Format, cellValue));
                                        }
                                    }
                                    else
                                    {
                                        if (p_Columns[j].DataType == DataType.Text)
                                        {
                                            row.GetCell(j).SetCellValue(cellValue);
                                        }
                                        else if (p_Columns[j].DataType == DataType.DateTime)
                                        {
                                            row.GetCell(j).SetCellValue(DateTime.Parse(cellValue));
                                        }
                                        else if (p_Columns[j].DataType == DataType.Number)
                                        {
                                            row.GetCell(j).SetCellValue(double.Parse(cellValue));
                                        }
                                        else
                                        {
                                            row.GetCell(j).SetCellValue(cellValue);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message);
                            }
                        }

                    }
                    else//表明已经进入下一个Sheet
                    {
                        k++;//进入下一个Sheet
                        rowIndexWhichInsert = 0;
                    }
                }
                #endregion
            }
            catch (Exception e)
            {
                _workbook = null;
                throw new Exception("循环导出数据时出错", e);
            }
        }

        /// <summary>
        /// 导出数据 
        /// </summary>
        /// <param name="p_List">数据集合</param>
        /// <param name="p_Columns">要导出数据列集合</param>
        /// <param name="p_SheetName">sheet名字</param>
        /// <param name="p_Template">是否使用模板 true:使用模板 false：不使用模板</param>
        /// <param name="p_FirstRow">内容开始行从1开始，默认值从第二行开始导出</param>
        private void WriteExcel(IList p_List, List<ExcelColumInfo> p_Columns, KeyValuePair<int, string>? cell, string p_SheetName, bool p_Template, int p_FirstRow = 2)
        {

            try
            {
                #region  用循环导出数据。
                int rowIndexWhichInsert = 0;
                int k = 1;//记录是第几个Sheet
                string tempSheetName = p_SheetName;
                //将数据插入到Tables中
                if (p_Template)
                {
                    rowIndexWhichInsert = p_FirstRow;//用于记录更新到了Sheet的哪一行
                }
                //内容单元格样式
                ICellStyle valueCellStyle = _workbook.CreateCellStyle();
                valueCellStyle.BorderBottom = BorderStyle.Thin;//设置四个边框
                valueCellStyle.BorderLeft = BorderStyle.Thin;
                valueCellStyle.BorderRight = BorderStyle.Thin;
                valueCellStyle.BorderTop = BorderStyle.Thin;
                valueCellStyle.WrapText = true;//自动换行

                //内容单元格样式
                PropertyInfo[] propertys = p_List[0].GetType().GetProperties();

                //这种循环的写法比以前的大大提高了效率
                for (int i = 0; i < p_List.Count; i++)
                {
                    rowIndexWhichInsert++;
                    //第一个或下一个Sheet，这里是这个循环的关键
                    if (i < _maxSheetRowCount * k)
                    {
                        if (k > 1)
                        {
                            tempSheetName = p_SheetName + k;
                        }

                        //导入数据之前先创建表头行和列 非模板导出 第一个sheet
                        if (rowIndexWhichInsert == 1 & !p_Template)
                        {
                            CreateSheetHeard(tempSheetName, p_Columns, p_FirstRow);
                        }

                        if (!p_Template || rowIndexWhichInsert > 1) //下一个sheet
                        {
                            //为导出数据创建内容行和列 
                            CreateSheetRowsAndColumns(tempSheetName, rowIndexWhichInsert, p_Columns, p_Template, valueCellStyle);
                        }

                        ISheet sheet = _workbook.GetSheet(tempSheetName);
                        IRow row = sheet.GetRow(rowIndexWhichInsert);
                        //对单元格赋值;
                        for (int j = 0; j < p_Columns.Count; j++)
                        {
                            //这里要根据p_Columns中有的数据列来赋值这样就支持选择性导出，
                            //DataType要对应模板中单元格个数这样就使得模板起作用。

                            var cellValueobject = propertys[p_Columns[j].PropertyIndex].GetValue(p_List[i], null);
                            if (cellValueobject != null)
                            {
                                var cellValue = cellValueobject.ToString();
                                if (p_Template)
                                {
                                    #region 模板导出单元格赋值

                                    if (p_Columns[j].DataType == DataType.Text)
                                    {
                                        row.GetCell(j).SetCellValue(cellValue);
                                    }
                                    else if (p_Columns[j].DataType == DataType.DateTime)
                                    {
                                        row.GetCell(j).SetCellValue(DateTime.Parse(cellValue));
                                    }
                                    else if (p_Columns[j].DataType == DataType.Number)
                                    {
                                        row.GetCell(j).SetCellValue(double.Parse(cellValue));
                                    }
                                    else
                                    {
                                        row.GetCell(j).SetCellValue(cellValue);
                                    }

                                    #endregion
                                }

                            }

                        }
                    }
                    else//表明已经进入下一个Sheet
                    {
                        k++;//进入下一个Sheet
                        rowIndexWhichInsert = 0;
                    }
                }
                #endregion
            }
            catch (Exception e)
            {
                _workbook = null;
                throw new Exception("循环导出数据时出错", e);
            }
        }

        #endregion

        #region SetSheetHeader 设置表头样式 针对非模板导出
        /// <summary>
        /// 设置表头
        /// </summary>
        /// <param name="p_SheetCount">Sheet数</param>
        /// <param name="p_ColumnCount">列数</param>
        private void SetSheetHeader(int p_SheetCount, int p_ColumnCount)
        {
            try
            {
                //设置字体
                IFont font = _workbook.CreateFont();
                font.Boldweight = (short)FontBoldWeight.Bold;
                font.FontHeightInPoints = 12;

                //设置单元格样式
                ICellStyle style = _workbook.CreateCellStyle();
                style.BorderBottom = BorderStyle.Thin;//设置四个边框
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Thin;

                //设置粗体
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                for (int k = 0; k < p_SheetCount; k++)
                {
                    IRow row = this._workbook.GetSheetAt(k).GetRow(0);//sheet1.GetRow(i + 1);//下标从0开始
                    row.Height = 21 * 20;//这个高度在实际中值为21
                    for (int i = 0; i < p_ColumnCount; i++)
                    {
                        ICell cell = row.GetCell(i);
                        cell.CellStyle = style;
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);

            }
        }
        #endregion

        #region CreateSheetHeard 创建表头行和列 非模板导出
        /// <summary>
        /// CreateSheetHeard 创建行和列 
        /// </summary>
        /// <param name="p_SheetName">sheet名称</param>
        /// <param name="p_ExcelColumnInfos">导出字段的集合</param>
        /// <param name="p_FirstRow">内容开始行从1开始</param>
        private void CreateSheetHeard(string p_SheetName, List<ExcelColumInfo> p_ExcelColumnInfos, int p_FirstRow)
        {
            try
            {
                for (int i = 0; i < p_FirstRow - 1; i++)
                {
                    IRow row = _workbook.GetSheet(p_SheetName).CreateRow(i);

                    for (int j = 0; j < p_ExcelColumnInfos.Count; j++)
                    {
                        ICell cell = row.CreateCell(j);//下标从0开始
                        cell.SetCellValue(p_ExcelColumnInfos[j].HeadName);
                    }
                }

                //调整列宽
                for (int k = 0; k < p_ExcelColumnInfos.Count; k++)
                {
                    _workbook.GetSheet(p_SheetName).SetColumnWidth(k, p_ExcelColumnInfos[k].Width * 265);
                }
            }
            catch (Exception e)
            {
                throw new Exception("创建Sheet的表头时出错", e);
            }
        }
        #endregion

        #region CreateSheetRowsAndColumns 创建内容行和列
        /// <summary>
        /// 创建行和列
        /// </summary>
        /// <param name="p_SheetName">Sheet名</param>
        /// <param name="p_RowNum"></param>
        /// <param name="p_ExcelColumnInfos">数据列集合</param>
        /// <param name="p_Template">是否使用模板true:使用模板 false不使用模板</param>
        /// <param name="p_ValueCellStyle">内容单元格样式</param>
        private void CreateSheetRowsAndColumns(string p_SheetName, int p_RowNum, List<ExcelColumInfo> p_ExcelColumnInfos, bool p_Template, ICellStyle p_ValueCellStyle)
        {
            try
            {
                IRow row = _workbook.GetSheet(p_SheetName).CreateRow(p_RowNum);
                List<ICell> cells = _workbook.GetSheet(p_SheetName).GetRow(FirstHeadRow - 1).Cells;
                if (cells == null || cells.Count < p_ExcelColumnInfos.Count)
                {
                    throw new IndexOutOfRangeException("模板列数小于导出需要的列数");
                }
                for (int i = 0; i < p_ExcelColumnInfos.Count; i++)
                {
                    ICell cell = row.CreateCell(i);//下标从0开始
                    if (p_RowNum > 0 & p_Template)
                    {
                        // _FirstRow - 1;
                        cell.CellStyle = cells[i].CellStyle;
                    }
                    else
                    {
                        cell.CellStyle = p_ValueCellStyle;
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("创建Sheet的行和列时出错", e);
            }
        }
        #endregion

        #region SetInformation 设置文档信息 针对非模板导出，模板导出无效
        /// <summary>
        /// 设置文档信息 针对非模板导出，模板导出无效
        /// </summary>
        /// <param name="p_Summary"></param>
        private void SetInformation(DocumentSummary p_Summary)
        {
            if (p_Summary != null)
            {
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                dsi.Company = p_Summary.Company;

                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                si.Subject = p_Summary.Subject;
                si.ApplicationName = p_Summary.ApplicationName;
                si.Author = p_Summary.Author;
                si.Comments = p_Summary.Comments;//注释
                si.Keywords = p_Summary.Keywords;
                si.CreateDateTime = DateTime.Now;
                si.Title = p_Summary.Title;
            }
        }
        #endregion

        #region SaveExcel 保存excel
        public void SaveExcel(string p_FileName)
        {
            try
            {
                //将文件转为byte[]好让Web调用端输出到浏览器。
                var file = new FileStream(p_FileName, FileMode.Create);
                _workbook.Write(file);
                file.Close();
            }
            catch (Exception e)
            {
                throw new Exception("将Workbook对象保存到文件时出错。", e);
            }
            finally
            {
                _workbook = null;
            }
        }
        #endregion

        #region 格式转换
        /// <summary>
        /// 转换格式
        /// </summary>
        /// <param name="p_FileName"></param>
        /// <returns></returns>
        private byte[] ExcelToByte(string p_FileName)
        {
            try
            {
                byte[] byteResult = ReadFile(p_FileName);

                //最后删除文件
                File.Delete(p_FileName);

                //GC.Collect();
                return byteResult;
            }
            catch (Exception e)
            {
                _workbook = null;
                //最后删除文件
                if (File.Exists(p_FileName))
                {
                    File.Delete(p_FileName);
                }

                throw new Exception("将Excel文件转为Byte时出错", e);
            }
        }
        #endregion

        #region 读取文件流
        /// <summary>
        /// 读取文件流
        /// </summary>
        /// <param name="p_FileName"></param>
        /// <returns></returns>
        private byte[] ReadFile(string p_FileName)
        {
            FileStream pFileStream = null;

            byte[] pReadByte = new byte[0];

            try
            {
                pFileStream = new FileStream(p_FileName, FileMode.Open, FileAccess.Read);

                var r = new BinaryReader(pFileStream);

                r.BaseStream.Seek(0, SeekOrigin.Begin);    //将文件指针设置到文件开

                pReadByte = r.ReadBytes((int)r.BaseStream.Length);
                pFileStream.Close();
                pFileStream = null;

                return pReadByte;
            }
            catch (Exception e)
            {
                if (pFileStream != null)
                {
                    pFileStream.Close();
                }

                throw new Exception("读取导出后的文件出错", e);
            }

        }
        #endregion

        #region 根据DataTable数据源返回列集合
        /// <summary>
        /// 根据数据源返回列集合
        /// </summary>
        /// <param name="p_Collumns">数据源</param>
        /// <returns></returns>
        private List<ExcelColumInfo> GetExcelColumInfos(Object p_Collumns)//DataColumnCollection p_Collumns
        {
            var ExcelColumInfos = new List<ExcelColumInfo>();
            try
            {
                if (p_Collumns.GetType().Name.ToLower().Contains("datatable"))
                {
                    foreach (DataColumn dc in (DataColumnCollection)p_Collumns)
                    {
                        var info = new ExcelColumInfo();
                        info.HeadName = dc.ColumnName;
                        info.TableColumnName = dc.ColumnName;
                        info.DataType = DataType.Text;
                        ExcelColumInfos.Add(info);
                    }
                }
                else//if (p_Collumns.GetType().Name.ToLower().Contains("ilist"))
                {
                    PropertyInfo[] propertys = ((IList)p_Collumns)[0].GetType().GetProperties();
                    int i = 0;
                    foreach (PropertyInfo pi in propertys)
                    {
                        var info = new ExcelColumInfo
                        {
                            HeadName = pi.Name,
                            TableColumnName = pi.Name,
                            PropertyIndex = i,
                            DataType = DataType.Text
                        };
                        ExcelColumInfos.Add(info);
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("导出Excel时出错，数据源的类型是无效的类型，目前只支持DataTable和集合导出。", ex);
            }
            return ExcelColumInfos;
        }
        #endregion

        /// <summary>
        /// 打开Excel文件流并将内容放到HSSFWorkbook对象。
        /// </summary>
        /// <param name="p_FileName">模板文件</param>
        /// <param name="p_SheetIndex">sheet默认从当前sheet开始</param>
        /// <returns>ISheet</returns>
        private ISheet OpenExcel(string p_FileName, int p_SheetIndex = 0)
        {
            try
            {
                ISheet sheet = null;
                using (var file = new FileStream(p_FileName, FileMode.Open, FileAccess.Read))
                {
                    _workbook = WorkbookFactory.Create(file);
                }
                sheet = _workbook.GetSheetAt(p_SheetIndex);
                return sheet;
            }
            catch (Exception ex)
            {
                throw new Exception("打开excel失败。", ex);
            }
        }
        /// <summary>
        /// 将Bitmap根据单元格区域大小进行缩放
        /// </summary>
        /// <param name="p_StartRowIndex"></param>
        /// <param name="p_EndRowIndex"></param>
        /// <param name="p_StartColumnIndex"></param>
        /// <param name="p_EndColumnIndex"></param>
        /// <param name="p_Bitmap"></param>
        /// <returns></returns>
        private Bitmap PictureZoom(int p_StartRowIndex, int p_EndRowIndex, int p_StartColumnIndex, int p_EndColumnIndex, Bitmap p_Bitmap, ISheet _sheet)
        {
            //首先取得Excel单元区域大小
            //行高/15=像素
            int height = 0;
            for (int j = p_StartRowIndex; j <= p_EndRowIndex; j++)
                height += (int)(_sheet.GetRow(j).Height / 15);
            int with = 0;
            //列宽/32=像素
            for (int i = p_StartColumnIndex; i <= p_EndColumnIndex; i++)
                with += (int)(_sheet.GetColumnWidth(i) / 32);

            //下面进行缩放
            var pz = new PictureZoom();

            return pz.ZoomBitmap(height, with, p_Bitmap);
        }
        
        #endregion

        /// <summary>
        /// 向指定单元格插入值
        /// </summary>
        /// <param name="p_TemplateFileName"></param>
        /// <param name="p_FileName"></param>
        /// <param name="p_SheetName"></param>
        /// <param name="cellValue"></param>
        /// <param name="colColumnName"></param>
        /// <param name="rowCount"></param>
        public void InsertExcelValueByHeadName(string p_TemplateFileName, string p_FileName, string p_SheetName, string cellValue, string colColumnName, int rowCount)
        {


            if (_maxSheetRowCount <= 0 || this._maxSheetRowCount >= 65535)
            {
                throw new Exception("导出Excel时出错，单个Sheet行数范围必须在1-65534内。");
            }

            if (File.Exists(p_FileName))
            {
                throw new Exception("导出Excel时出错，临时文件已存在。");
            }

            if (p_TemplateFileName != "" && !File.Exists(p_TemplateFileName))
            {
                throw new Exception("导出Excel时出错，模板文件不存在。");
            }

            try
            {
                //列转化(A->1)
                int index = MoreCharToInt(colColumnName) - 1;
                ISheet sheet = _workbook.GetSheet(p_SheetName);
                //
                ICell cell = sheet.GetRow(rowCount).GetCell(index);
                cell.SetCellValue(cellValue);
                // SetValueAndFormat(_workbook, cell, cellValue, 1);
            }
            catch (Exception e)
            {

                throw new Exception("方法ExportDataInFile出错", e);
            }
        }
        /// <summary>
        /// 向指定单元格插入值
        /// add tjq 20160919
        /// </summary>
        /// <param name="p_FileName">文件名</param>
        /// <param name="p_SheetName">sheet名</param>
        /// <param name="cellValue">插入值</param>
        /// <param name="colColumnName">excel列序号</param>
        /// <param name="excRowNumber">excel行序号</param>
        public void InsertExcelValueByHeadName(string p_FileName, string p_SheetName, string cellValue, string colColumnName, int excRowNumber)
        {

            var rowCount = excRowNumber - 1;
            if (_maxSheetRowCount <= 0 || this._maxSheetRowCount >= 65535)
            {
                throw new Exception("导出Excel时出错，单个Sheet行数范围必须在1-65534内。");
            }

            if (File.Exists(p_FileName))
            {
                throw new Exception("导出Excel时出错，临时文件已存在。");
            }

            try
            {
                //列转化(A->1)
                int index = MoreCharToInt(colColumnName) - 1;
                ISheet sheet = _workbook.GetSheet(p_SheetName);
                ICell cell = sheet.GetRow(rowCount).GetCell(index);
                cell.SetCellValue(cellValue);
            }
            catch (Exception e)
            {

                throw new Exception("方法ExportDataInFile出错", e);
            }
        }
        /// <summary>
        /// 把传来的字母转换成相应的数字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private int MoreCharToInt(string value)
        {
            int rtn = 0;
            int powIndex = 0;

            for (int i = value.Length - 1; i >= 0; i--)
            {
                int tmpInt = value[i];
                tmpInt -= 64;

                rtn += (int)Math.Pow(26, powIndex) * tmpInt;
                powIndex++;
            }

            return rtn;
        }
    }
}
