using System;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.IO;
using System.Drawing;
using CanYouLib.ExcelLib.Utility;

/***
 * Create Version:0.0.0.2
 * Code by:吴国超
 * Date:2010-04-08
 * 更新记录：
 * Code by:吴国超
 * Date:2011-03-11
 * 1、增加了插入行操作（基于前一行样式）
 * 2、增加了插入图片功能，并且支持自动缩放。
 * 3、支持了页眉页脚的设置
 * Date:2011-06-07
 * 1、支持模板多表头导出功能
 * 2、增加合并行功能
 * Date:2011-09-13
 * 1、增加了公式支持
 * 2、公开了_workbook与_sheet变量
 ***/
namespace CanYouLib.ExcelLib
{
    /// <summary>
    /// 基于Excel模板的报表生成类。
    /// 吴国超 2010-04-08
    /// 注：无需引用Office.Excel
    /// </summary>
    public class ExcelExpert : IDisposable
    {
        private IWorkbook _workbook;
        private ISheet _sheet;
        private string _templateFileName;
        /// <summary>
        /// 模板文件的完整文件名
        /// </summary>
        public string TemplateFileName
        {
            get { return _templateFileName; }
            set { _templateFileName = value; }
        }

        /// <summary>
        /// 构造函数,默认操作第一个Sheet表
        /// </summary>
        /// <param name="p_FileName">模板文件完整路径</param>
        public ExcelExpert(string p_FileName)
        {

            this.TemplateFileName = p_FileName;
            //打开Excel文件流并将内容放到HSSFWorkbook对象。
            using (var file = new FileStream(p_FileName, FileMode.Open, FileAccess.Read))
            {
                _workbook = WorkbookFactory.Create(file);
            }
            //设置当前操作的Sheet，默认为第一个，这样就可以操作多个。
            _sheet = _workbook.GetSheetAt(0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_FileName"></param>
        /// <param name="p_SheetIndex"></param>
        public ExcelExpert(string p_FileName, int p_SheetIndex)
        {

            this.TemplateFileName = p_FileName;
            //删除临时文件

            //打开Excel文件流并将内容放到HSSFWorkbook对象。
            using (FileStream file = new FileStream(p_FileName, FileMode.Open, FileAccess.Read))
            {
                _workbook = WorkbookFactory.Create(file);
            }
            //设置当前操作的Sheet，默认为第一个，这样就可以操作多个。
            _sheet = _workbook.GetSheetAt(p_SheetIndex);
        }

        #region SetValue 对单元格区域赋值 public
        /// <summary>
        /// 对单元格区域赋值
        /// </summary>
        /// <param name="p_startRowIndex">起始行号，从1开始</param>
        /// <param name="p_endRowIndex">结束行号，从1开始</param>
        /// <param name="p_startColChars">起始列名，从A开始</param>
        /// <param name="p_endColChars">结束列名，从A开始</param>
        /// <param name="p_Values"></param>
        public void SetValue(int p_startRowIndex, int p_endRowIndex, string p_startColChars, string p_endColChars, Array p_Values)
        {
            SetValue(p_startRowIndex, p_endRowIndex, ExcelColumnTranslator.ToIndex(p_startColChars), ExcelColumnTranslator.ToIndex(p_endColChars), p_Values);
        }
        /// <summary>
        /// 对单元格区域赋值
        /// </summary>
        /// <param name="p_startRowIndex">起始行号，从1开始</param>
        /// <param name="p_endRowIndex">结束行号，从1开始</param>
        /// <param name="p_startColIndex">起始列号，从1开始</param>
        /// <param name="p_endColIndex">结束列号，从1开始</param>
        /// <param name="p_Values"></param>
        public void SetValue(int p_startRowIndex, int p_endRowIndex, int p_startColIndex, int p_endColIndex, Array p_Values)
        {
            //遍历行与列
            int index1 = 0;
            int index2 = 0;
            for (int rowIndex = p_startRowIndex; rowIndex <= p_endRowIndex; rowIndex++)
            {
                index2 = 0;
                for (int colIndex = p_startColIndex; colIndex <= p_endColIndex; colIndex++)
                {
                    SetValue(rowIndex, colIndex, p_Values.GetValue(index1, index2).ToString());
                    index2++;
                }
                index1++;
            }
        }



        /// <summary>
        /// 对单元格赋值
        /// </summary>
        /// <param name="p_RowIndex">行号，从1开始</param>
        /// <param name="p_ColChars">列名，从A开始</param>
        /// <param name="p_Value">期望值</param>
        public void SetValue(int p_RowIndex, string p_ColChars, string p_Value)
        {
            SetValue(p_RowIndex, ExcelColumnTranslator.ToIndex(p_ColChars), p_Value);
        }

        /// <summary>
        /// 对单元格赋值
        /// </summary>
        /// <param name="p_RowIndex">行号，从1开始</param>
        /// <param name="p_ColChars">列名，从A开始</param>
        /// <param name="p_Value">期望值</param>
        public void SetValue(int p_RowIndex, string p_ColChars, double p_Value)
        {
            SetValue(p_RowIndex, ExcelColumnTranslator.ToIndex(p_ColChars), p_Value);
        }



        /// <summary>
        /// 对单元格赋值
        /// </summary>
        /// <param name="p_RowIndex">行号，从1开始</param>
        /// <param name="p_ColChars">列名，从A开始</param>
        /// <param name="p_Value">期望值</param>
        public void SetValue(int p_RowIndex, string p_ColChars, DateTime p_Value)
        {
            SetValue(p_RowIndex, ExcelColumnTranslator.ToIndex(p_ColChars), p_Value);
        }





        /// <summary>
        /// 对单元格赋值
        /// </summary>
        /// <param name="p_RowIndex">行号，从1开始</param>
        /// <param name="p_ColumnIndex">列号，从1开始</param>
        /// <param name="p_Value">期望值</param>
        public void SetValue(int p_RowIndex, int p_ColumnIndex, DateTime p_Value)
        {
            _sheet.GetRow(p_RowIndex - 1).GetCell(p_ColumnIndex - 1).SetCellValue(p_Value);
        }

        /// <summary>
        /// 对单元格赋值
        /// </summary>
        /// <param name="p_RowIndex">行号，从1开始</param>
        /// <param name="p_ColumnIndex">列号，从1开始</param>
        /// <param name="p_Value">期望值，从1开始</param>
        private void SetValue(int p_RowIndex, int p_ColumnIndex, string p_Value)
        {
            _sheet.GetRow(p_RowIndex - 1).GetCell(p_ColumnIndex - 1).SetCellValue(p_Value);
        }
        /// <summary>
        /// 对单元格赋值
        /// </summary>
        /// <param name="p_RowIndex">行号，从1开始</param>
        /// <param name="p_ColumnIndex">列号，从1开始</param>
        /// <param name="p_Value">期望值</param>
        public void SetValue(int p_RowIndex, int p_ColumnIndex, double p_Value)
        {
            _sheet.GetRow(p_RowIndex - 1).GetCell(p_ColumnIndex - 1).SetCellValue(p_Value);
        }
        #endregion

        #region SetColumnHidden 隐藏列 public
        /// <summary>
        /// 设置隐藏列
        /// </summary>
        /// <param name="p_StartcolIndex">隐藏的起始列序号</param>
        /// <param name="p_EndcolIndex">隐藏的终止列序号</param>
        public void SetColumnHidden(int p_StartcolIndex, int p_EndcolIndex)
        {
            for (int i = p_StartcolIndex; i <= p_EndcolIndex; i++)
            {
                _sheet.SetColumnHidden(i, true);
            }
        }


        /// <summary>
        /// 设置隐藏列
        /// </summary>
        /// <param name="p_StartcolChar">隐藏的起始列序号</param>
        /// <param name="p_EndcolChar">隐藏的终止列序号</param>
        public void SetColumnHidden(string p_StartcolChar, string p_EndcolChar)
        {
            SetColumnHidden(ExcelColumnTranslator.ToIndex(p_StartcolChar) - 1,
                ExcelColumnTranslator.ToIndex(p_EndcolChar) - 1);
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
        public void AddMergedRegion(int p_StartRowIndex, int p_EndRowIndex, string p_StartColChars, string p_EndColChars)
        {
            _sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(p_StartRowIndex - 1, ExcelColumnTranslator.ToIndex(p_StartColChars) - 1, p_EndRowIndex - 1, ExcelColumnTranslator.ToIndex(p_EndColChars) - 1));
        }
        #endregion

        #region InsertPicture 在指定单元格插入图片 public
        /// <summary>
        /// 入图片到单元格区域，根据单元格区域自动缩放
        /// </summary>
        /// <param name="p_RowIndex">起始行号，从1开始</param>
        /// <param name="p_ColIndex">起始列号，从1开始</param>
        /// <param name="p_FileName">图片源文件完整路径</param>
        public void InsertPicture(int p_RowIndex, int p_ColIndex, string p_FileName)
        {
            InsertPicture(p_RowIndex, p_RowIndex, p_ColIndex, p_ColIndex, p_FileName, 10, 10);
        }

        /// <summary>
        /// 入图片到单元格区域，根据单元格区域自动缩放
        /// </summary>
        /// <param name="p_RowIndex">起始行号，从1开始</param>
        /// <param name="p_ColIndex">起始列号，从1开始</param>
        /// <param name="p_FileName">图片源文件完整路径</param>
        /// <param name="p_LeftRectify">图片相对单元格靠左的偏移值，最大值1023</param>
        /// <param name="p_TopRectify">图片相对单元格靠上的偏移值，最大值255</param>
        public void InsertPicture(int p_RowIndex, int p_ColIndex, string p_FileName, int p_LeftRectify, int p_TopRectify)
        {
            InsertPicture(p_RowIndex, p_RowIndex, p_ColIndex, p_ColIndex, p_FileName, p_LeftRectify, p_TopRectify);
        }

        /// <summary>
        /// 入图片到单元格区域，根据单元格区域自动缩放
        /// </summary>
        /// <param name="p_RowIndex">起始行号，从1开始</param>
        /// <param name="p_ColChars">起始列名，从A开始</param>
        /// <param name="p_FileName">图片源文件完整路径</param>
        public void InsertPicture(int p_RowIndex, string p_ColChars, string p_FileName)
        {
            InsertPicture(p_RowIndex, p_RowIndex, ExcelColumnTranslator.ToIndex(p_ColChars), ExcelColumnTranslator.ToIndex(p_ColChars), p_FileName, 10, 10);
        }

        /// <summary>
        /// 入图片到单元格区域，根据单元格区域自动缩放
        /// </summary>
        /// <param name="p_RowIndex">起始行号，从1开始</param>
        /// <param name="p_ColChars">起始列名，从A开始</param>
        /// <param name="p_FileName">图片源文件完整路径</param>
        /// <param name="p_LeftRectify">图片相对单元格靠左的偏移值，最大值1023</param>
        /// <param name="p_TopRectify">图片相对单元格靠上的偏移值，最大值255</param>
        public void InsertPicture(int p_RowIndex, string p_ColChars, string p_FileName, int p_LeftRectify, int p_TopRectify)
        {
            InsertPicture(p_RowIndex, p_RowIndex, ExcelColumnTranslator.ToIndex(p_ColChars), ExcelColumnTranslator.ToIndex(p_ColChars), p_FileName, p_LeftRectify, p_TopRectify);
        }


        /// <summary>
        /// 入图片到单元格区域，根据单元格区域自动缩放
        /// </summary>
        /// <param name="p_StartRowIndex">起始行号，从1开始</param>
        /// <param name="p_EndRowIndex">结束行号，从1开始</param>
        /// <param name="p_StartColChars">起始列名，从A开始</param>
        /// <param name="p_EndColChars">结束列名，从A开始</param>
        /// <param name="p_FileName">图片源文件完整路径</param>
        public void InsertPicture(int p_StartRowIndex, int p_EndRowIndex, string p_StartColChars, string p_EndColChars, string p_FileName)
        {
            InsertPicture(p_StartRowIndex, p_EndRowIndex, ExcelColumnTranslator.ToIndex(p_StartColChars), ExcelColumnTranslator.ToIndex(p_EndColChars), p_FileName, 10, 10);
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
        public void InsertPicture(int p_StartRowIndex, int p_EndRowIndex, string p_StartColChars, string p_EndColChars, string p_FileName, int p_LeftRectify, int p_TopRectify)
        {
            InsertPicture(p_StartRowIndex, p_EndRowIndex, ExcelColumnTranslator.ToIndex(p_StartColChars), ExcelColumnTranslator.ToIndex(p_EndColChars), p_FileName, p_LeftRectify, p_TopRectify);
        }

        /// <summary>
        /// 入图片到单元格区域，根据单元格区域自动缩放
        /// </summary>
        /// <param name="p_StartRowIndex">起始行号，从1开始</param>
        /// <param name="p_EndRowIndex">结束行号，从1开始</param>
        /// <param name="p_StartColumnIndex">起始列号，从1开始</param>
        /// <param name="p_EndColumnIndex">结束列号，从1开始</param>
        /// <param name="p_FileName">图片源文件完整路径</param>
        public void InsertPicture(int p_StartRowIndex, int p_EndRowIndex, int p_StartColumnIndex, int p_EndColumnIndex, string p_FileName)
        {
            InsertPicture(p_StartRowIndex, p_EndRowIndex, p_StartColumnIndex, p_EndColumnIndex, p_FileName, 10, 10);
        }

        /// <summary>
        /// 入图片到单元格区域，根据单元格区域自动缩放
        /// </summary>
        /// <param name="p_RowIndex">行号，从1开始</param>
        /// <param name="p_ColChars">列名，从A开始</param>
        /// <param name="p_Bitmap">图片源</param>
        public void InsertPicture(int p_RowIndex, string p_ColChars, Bitmap p_Bitmap)
        {
            InsertPicture(p_RowIndex, p_RowIndex, ExcelColumnTranslator.ToIndex(p_ColChars), ExcelColumnTranslator.ToIndex(p_ColChars), p_Bitmap, 10, 10);
        }

        /// <summary>
        /// 入图片到单元格区域，根据单元格区域自动缩放
        /// </summary>
        /// <param name="p_RowIndex">行号，从1开始</param>
        /// <param name="p_ColChars">列名，从A开始</param>
        /// <param name="p_Bitmap">图片源</param>
        /// <param name="p_LeftRectify">图片相对单元格靠左的偏移值，最大值1023</param>
        /// <param name="p_TopRectify">图片相对单元格靠上的偏移值，最大值255</param>
        public void InsertPicture(int p_RowIndex, string p_ColChars, Bitmap p_Bitmap, int p_LeftRectify, int p_TopRectify)
        {
            InsertPicture(p_RowIndex, p_RowIndex, ExcelColumnTranslator.ToIndex(p_ColChars), ExcelColumnTranslator.ToIndex(p_ColChars), p_Bitmap, p_LeftRectify, p_TopRectify);
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
        public void InsertPicture(int p_StartRowIndex, int p_EndRowIndex, string p_StartColChars, string p_EndColChars, Bitmap p_Bitmap, int p_LeftRectify, int p_TopRectify)
        {
            InsertPicture(p_StartRowIndex, p_EndRowIndex, ExcelColumnTranslator.ToIndex(p_StartColChars), ExcelColumnTranslator.ToIndex(p_EndColChars), p_Bitmap, p_LeftRectify, p_TopRectify);
        }

        /// <summary>
        /// 入图片到单元格区域，根据单元格区域自动缩放
        /// </summary>
        /// <param name="p_StartRowIndex">起始行号，从1开始</param>
        /// <param name="p_EndRowIndex">结束行号，从1开始</param>
        /// <param name="p_StartColumnIndex">起始列号，从1开始</param>
        /// <param name="p_EndColumnIndex">结束列号，从1开始</param>
        /// <param name="p_Bitmap">图片源</param>
        /// <param name="p_LeftRectify">图片相对单元格靠左的偏移值，最大值1023</param>
        /// <param name="p_TopRectify">图片相对单元格靠上的偏移值，最大值255</param>
        public void InsertPicture(int p_StartRowIndex, int p_EndRowIndex, int p_StartColumnIndex, int p_EndColumnIndex, Bitmap p_Bitmap, int p_LeftRectify, int p_TopRectify)
        {
            p_StartRowIndex = p_StartRowIndex - 1;
            p_EndRowIndex = p_EndRowIndex - 1;
            p_StartColumnIndex = p_StartColumnIndex - 1;
            p_EndColumnIndex = p_EndColumnIndex - 1;

            //缩放
            p_Bitmap = PictureZoom(p_StartRowIndex, p_EndRowIndex, p_StartColumnIndex, p_EndColumnIndex, p_Bitmap);

            //将图片对象添加到workbook中
            int pictureIdx = _workbook.AddPicture(FileUtility.ImageToByte(p_Bitmap), PictureType.JPEG);

            //创建一个顶层的图形绘画者。
            IDrawing patriarch = _sheet.CreateDrawingPatriarch();
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
        public void InsertPicture(int p_StartRowIndex, int p_EndRowIndex, int p_StartColumnIndex, int p_EndColumnIndex, string p_FileName, int p_LeftRectify, int p_TopRectify)
        {
            //将文件存到Bitmap
            var bitmap = new Bitmap(p_FileName);

            //插入
            InsertPicture(p_StartRowIndex, p_EndRowIndex, p_StartColumnIndex, p_EndColumnIndex, bitmap, p_LeftRectify, p_TopRectify);
        }
        #endregion

        #region SetFormula  对单元格设置公式 public
        /// <summary>
        /// 对单元格设置公式
        /// </summary>
        /// <param name="p_RowIndex">行号，从1开始</param>
        /// <param name="p_ColChars">列名，从A开始</param>
        /// <param name="p_Formula">公式，如：A1*A2</param>
        public void SetFormula(int p_RowIndex, string p_ColChars, string p_Formula)
        {
            SetFormula(p_RowIndex, ExcelColumnTranslator.ToIndex(p_ColChars), p_Formula);
        }
        /// <summary>
        /// 对单元格设置公式
        /// </summary>
        /// <param name="p_RowIndex">行号，从1开始</param>
        /// <param name="p_ColumnIndex">列号，从1开始</param>
        /// <param name="p_Value">公式，如：A1*A2</param>
        public void SetFormula(int p_RowIndex, int p_ColumnIndex, string p_Formula)
        {
            _sheet.GetRow(p_RowIndex - 1).GetCell(p_ColumnIndex - 1).SetCellFormula(p_Formula);
        }
        #endregion

        #region CreateRows 在起始行上方插入新行,并赋值起始行的样式 public
        /// <summary>
        /// 在起始行上方插入新行,并赋值起始行的样式。
        /// </summary>
        /// <param name="p_StartRow">起始行号，从1开始</param>
        /// <param name="p_Count">所要创建的行数</param>
        public void CreateRows(int p_StartRow, int p_Count)
        {
            p_Count = p_Count - 1;
            p_StartRow = p_StartRow - 1;
            IRow mySourceStyleRow = this._sheet.GetRow(p_StartRow);//获取源格式行
            //_sheet.ShiftRows(p_StartRow + 1, this._sheet.LastRowNum, p_Count, true, false, true);
            ////调用插入行方法

            MyInsertRow(this._sheet, p_StartRow, p_Count, mySourceStyleRow);

        }
        #endregion

        #region ExcelToByte  转换格式，该方法会自动在转换后删除原文件 public
        /// <summary>
        /// 转换格式，该方法会自动在转换后删除原文件。
        /// </summary>
        /// <param name="p_FileName"></param>
        /// <returns></returns>
        public byte[] ExcelToByte(string p_FileName)
        {
            try
            {
                byte[] byteResult = ReadFile(p_FileName);

                File.Delete(p_FileName);


                return byteResult;
            }
            catch (Exception e)
            {

                if (File.Exists(p_FileName))
                    File.Delete(p_FileName);
                throw new Exception("将Excel文件转为Byte时出错，错误信息：" + e.Message);
            }
        }
        #endregion

        #region 保存文件
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

        #region 辅助方法 private
        /// <summary>
        /// 将Bitmap根据单元格区域大小进行缩放
        /// </summary>
        /// <param name="p_StartRowIndex"></param>
        /// <param name="p_EndRowIndex"></param>
        /// <param name="p_StartColumnIndex"></param>
        /// <param name="p_EndColumnIndex"></param>
        /// <param name="p_Bitmap"></param>
        /// <returns></returns>
        private Bitmap PictureZoom(int p_StartRowIndex, int p_EndRowIndex, int p_StartColumnIndex, int p_EndColumnIndex, Bitmap p_Bitmap)
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


        /// <summary>
        /// 读取文件流
        /// </summary>
        /// <param name="p_FileName"></param>
        /// <returns></returns>
        private byte[] ReadFile(string p_FileName)
        {
            FileStream pFileStream = null;

            var pReadByte = new byte[0];

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
                    pFileStream.Close();
                throw new Exception("读取导出后的文件出错，错误信息" + e.Message);
            }

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
        #endregion
        /// <summary>
        /// 释放资源
        /// </summary>
        void IDisposable.Dispose()
        {
            _sheet = null;
            _workbook = null;
        }

    }
}
