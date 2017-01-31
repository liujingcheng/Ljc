using System;
using System.Data;
using System.Drawing;
using System.IO;

namespace CanYouLib.ExcelLib.Utility
{
    /// <summary>
    /// 图片缩放。Code by:吴国超
    /// </summary>
    public  class PictureZoom
    {
        /// <summary>
        /// 缩小图片
        /// </summary>
        /// <param name="strOldPic">源图文件名(包括路径)</param>
        /// <param name="strNewPic">缩小后保存为文件名(包括路径)</param>
        /// <param name="intWidth">缩小至宽度</param>
        /// <param name="intHeight">缩小至高度</param>
        public void Zoom(string strOldPic, string strNewPic, int intWidth, int intHeight)
        {
            if (intWidth > intHeight)//如果宽大于高，就指定高，自动计算宽
                ZoomAutoWith(strOldPic, strNewPic, intHeight);
            else
                ZoomAutoHight(strOldPic, strNewPic, intWidth);
        }

        /// <summary>
        /// 按比例缩小图片，自动计算高度
        /// </summary>
        /// <param name="strOldPic">源图文件名(包括路径)</param>
        /// <param name="strNewPic">缩小后保存为文件名(包括路径)</param>
        /// <param name="intWidth">缩小至宽度</param>
        public void ZoomAutoHight(string strOldPic, string strNewPic, int intWidth)
        {

            System.Drawing.Bitmap objPic, objNewPic;
            try
            {
                objPic = new System.Drawing.Bitmap(strOldPic);
                //int intHeight = (intWidth * objPic.Height) / objPic.Width;//自动计算高度，按原比例计算
                //objNewPic = new System.Drawing.Bitmap(objPic, intWidth, intHeight);
                objNewPic = BitmapZoomAutoHight(objPic, intWidth);
                objNewPic.Save(strNewPic,System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (Exception exp) { throw exp; }
            finally
            {
                objPic = null;
                objNewPic = null;
            }
        }
        /// <summary>
        /// 按比例缩小图片，自动计算宽度
        /// </summary>
        /// <param name="strOldPic">源图文件名(包括路径)</param>
        /// <param name="strNewPic">缩小后保存为文件名(包括路径)</param>
        /// <param name="intHeight">缩小至高度</param>
        public void ZoomAutoWith(string strOldPic, string strNewPic, int intHeight)
        {

            System.Drawing.Bitmap objPic, objNewPic;
            try
            {
                objPic = new System.Drawing.Bitmap(strOldPic);
                //int intWidth = (intHeight * objPic.Width) / objPic.Height;//自动计算宽度，按原比例计算
                //objNewPic = new System.Drawing.Bitmap(objPic, intWidth, intHeight);
                objNewPic = BitmapZoomAutoWith(objPic, intHeight);
                objNewPic.Save(strNewPic, System.Drawing.Imaging.ImageFormat.Jpeg);

            }
            catch (Exception exp) { throw exp; }
            finally
            {
                objPic = null;
                objNewPic = null;
            }
        }

        /// <summary>
        /// 对Bitmap根据宽度自动缩放
        /// </summary>
        /// <param name="p_Bitmap">图片源</param>
        /// <param name="intWidth">宽度</param>
        /// <returns></returns>
        private System.Drawing.Bitmap BitmapZoomAutoHight(Bitmap p_Bitmap, int intWidth)
        {
            int intHeight = (intWidth * p_Bitmap.Height) / p_Bitmap.Width;//自动计算高度，按原比例计算
            return new System.Drawing.Bitmap(p_Bitmap, intWidth, intHeight);
        }
        /// <summary>
        /// 对Bitmap根据宽度自动缩放
        /// </summary>
        /// <param name="p_Bitmap">图片源</param>
        /// <param name="intHeight">高度</param>
        /// <returns></returns>
        private System.Drawing.Bitmap BitmapZoomAutoWith(Bitmap p_Bitmap, int intHeight)
        {
            int intWidth = (intHeight * p_Bitmap.Width) / p_Bitmap.Height;//自动计算宽度，按原比例计算
            return new System.Drawing.Bitmap(p_Bitmap, intWidth, intHeight);
        }

        /// <summary>
        /// 缩放Bitmap
        /// </summary>
        /// <param name="p_MaxHeight">超过该高度进行缩放</param>
        /// <param name="p_MaxWidth">超过该宽度进行缩放</param>
        /// <param name="p_Bitmap">图片源</param>
        /// <returns></returns>
        public Bitmap ZoomBitmap(int p_MaxHeight, int p_MaxWidth, Bitmap p_Bitmap)
        {
            //转化成bmp后的图片的原始大小与单元格的大小比较，看看是否需要进行缩放
            if (p_Bitmap.Size.Height > p_MaxHeight || p_Bitmap.Size.Width > p_MaxWidth)//需要缩放
            {
                //根据单元格区域宽高来决定自动缩放宽或高

                if (p_MaxHeight > p_MaxWidth)//也就是说宽可以指定，但高就是自动计算
                    return BitmapZoomAutoHight(p_Bitmap, p_MaxWidth);
                else
                    return BitmapZoomAutoWith(p_Bitmap, p_MaxHeight);
            }
            else
                return new Bitmap(p_Bitmap, p_Bitmap.Size.Width, p_Bitmap.Size.Height);
        }
        /// <summary>
        /// 缩放Bitmap
        /// </summary>
        /// <param name="p_MaxHeight">超过该高度进行缩放</param>
        /// <param name="p_MaxWidth">超过该宽度进行缩放</param>
        /// <param name="p_Bitmap">图片源</param>
        /// <param name="p_FileNameToSave">压缩后的文件保存路径</param>
        /// <returns></returns>
        public void ZoomBitmap(int p_MaxHeight, int p_MaxWidth, Bitmap p_Bitmap, string p_FileNameToSave)
        {
            ZoomBitmap(p_MaxHeight, p_MaxWidth, p_Bitmap).Save(p_FileNameToSave, System.Drawing.Imaging.ImageFormat.Jpeg);
        }


    }
}
