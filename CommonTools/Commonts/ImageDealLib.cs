/*
 * 2017-8-23
 * hrj
 * 处理图片的公共方法
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging; 
using System.IO;
using System.Text;
using System.Web;


namespace CommonTools.Commonts
{
    /// <summary>
    /// 处理图片的公共方法
    /// </summary>
    public class ImageDealLib
    {
        #region 公共函数

        /// <summary>
        ///     根据文件路径判断文件是否存在
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <param name="model">返回模式,m:返回map地址不检查文件是否存在,c:检测文件是否存在,并返回map地址</param>
        /// <param name="mappath">map路径</param>
        /// <returns></returns>
        public static bool FileExistMapPath(string filepath, FileCheckModel model, out string mappath)
        {
            var checkresult = false;
            switch (model)
            {
                case FileCheckModel.M:
                    mappath = HttpContext.Current.Server.MapPath(filepath);
                    checkresult = true;
                    break;
                case FileCheckModel.C:
                    if (File.Exists(HttpContext.Current.Server.MapPath(filepath)))
                    {
                        mappath = HttpContext.Current.Server.MapPath(filepath);
                        checkresult = true;
                    }
                    else
                    {
                        mappath = null;
                    }
                    break;
                default:
                    mappath = "";
                    break;
            }
            return checkresult;
        }

        /// <summary>
        ///     图片保存类型
        ///     JPEG:.jpg格式;
        ///     GIF:.gif格式;
        ///     PNG:.png格式;
        /// </summary>
        public enum ImageType
        {
            Jpeg,
            Gif,
            Png
        }

        /// <summary>
        ///     水印模式
        ///     Center:中间;
        ///     CenterUp:中上;
        ///     CenterDown:中下;
        ///     LeftUp:左上;
        ///     LeftDown:左下;
        ///     RightUp:右上;
        ///     RightDown:右下;
        ///     Random:随机;
        /// </summary>
        public enum WaterType
        {
            Center,
            CenterUp,
            CenterDown,
            LeftUp,
            LeftDown,
            RightUp,
            RightDown,
            Random
        }

        /// <summary>
        ///     缩略模式
        ///     X--按宽度缩放,高着宽比例;
        ///     Y--按高度缩放,宽着宽比例;
        ///     XY--按给定mwidth,mheight(此模式mwidth,mheight为必须值)进行缩略;
        /// </summary>
        public enum ResizeType
        {
            X,
            Y,
            Xy
        }

        /// <summary>
        ///     文件检测模式
        ///     M:不检测文件是否存在,返回ServerMapPath;
        ///     C:检测文件是否存在,返回ServerMapPath;
        /// </summary>
        public enum FileCheckModel
        {
            M,
            C
        }

        /// <summary>
        /// 返回图片类型
        /// str64==图片流
        /// strUrl=图片地址
        /// </summary>
        public enum FileByType
        {
            Str64,
            StrUrl
        }

        /// <summary>
        ///     原图文件是否保存
        ///     Delete:保存
        ///     Save:不保存,删除
        /// </summary>
        public enum FileCache
        {
            Save,
            Delete
        }

        #endregion

        #region 图片剪切

        public static string Lastcroppic = ""; //上一张已剪切生成的文件名
        public static string Diffpicpath = ""; //上一张要被剪切的原图地址

        /// <summary>
        ///     图片剪切
        /// </summary>
        /// <param name="picpath">源图片文件地址</param>
        /// <param name="spath">剪切临时文件地址</param>
        /// <param name="x1">x起始坐标</param>
        /// <param name="y1">y起始坐标</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="filename">剪切图片名称</param>
        /// <param name="filecache">源文件处理方式</param>
        /// <param name="warning">处理警告信息</param>
        /// <returns>剪切图片地址</returns>
        public static string Imgcrop(string picpath, string spath, int x1, int y1, int width, int height,string filename,
            FileCache filecache, out string warning)
        {
            //反馈信息
            var checkmessage = new StringBuilder();

            //从指定源图片,创建image对象
            var sourceimgCommonMappath = "";

            //检测源文件
            var checkfile = false;
            checkfile = FileExistMapPath(picpath, FileCheckModel.C, out sourceimgCommonMappath);

            if (checkfile)
            {
                //从源文件创建imgage
                var sourceimgCommon = Image.FromFile(sourceimgCommonMappath);

                //从指定width、height创建bitmap对象
                var currimgCommon = new Bitmap(width, height);

                //从_currimg_common创建画笔
                var gCommon = Graphics.FromImage(currimgCommon);

                //设置画笔
                gCommon.CompositingMode = CompositingMode.SourceOver;
                gCommon.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gCommon.PixelOffsetMode = PixelOffsetMode.Half;

                //绘制图片
                gCommon.DrawImage(sourceimgCommon, new Rectangle(0, 0, width, height),
                    new Rectangle(x1, y1, width, height), GraphicsUnit.Pixel);

                //保存图片
                var spathCommonMappath = "";

                //判断是否是对同一张图片进行剪切
                //判断是否，已更新剪切图片,防止覆盖上一张已完成剪切的图片
                if (!string.IsNullOrEmpty(filename))
                {
                    filename = filename + ".jpg";
                }
                else
                {
                    filename = Guid.NewGuid() + ".jpg";
                }
                spath = string.IsNullOrEmpty(Lastcroppic)
                    ? spath + filename
                    : (Diffpicpath == picpath ? Lastcroppic : spath + filename);

                Lastcroppic = spath;
                Diffpicpath = picpath;

                FileExistMapPath(spath, FileCheckModel.M, out spathCommonMappath);

                currimgCommon.Save(spathCommonMappath, ImageFormat.Jpeg);

                //释放
                sourceimgCommon.Dispose();
                currimgCommon.Dispose();
                gCommon.Dispose();

                //处理原文件
                var filecachecode = filecache.GetHashCode();

                //文件缓存方式:Delete,删除原文件
                if (filecachecode == 1)
                {
                    File.Delete(sourceimgCommonMappath);
                }

                //返回相对虚拟路径
                warning = "";
                return spath;
            }

            checkmessage.Append("error:未能找到剪切原图片;");

            warning = checkmessage.ToString().TrimEnd(';');

            return "";
        }


        #endregion

        #region 缩略图片处理

        /// <summary>
        ///     根据指定：缩略宽、高,缩略图片并保存
        ///     返回图片虚拟路径,和一个警告信息,可根据此信息获取图片合成信息
        /// </summary>
        /// <param name="picpath">原图路径</param>
        /// <param name="model">缩略模式[X,Y,XY](默认XY模式)</param>
        /// <param name="spath">文件保存路径(默认跟路径)</param>
        /// <param name="imgtype">图片保存类型</param>
        /// <param name="mwidth">缩略宽度(默认原图高度)</param>
        /// <param name="mheight">缩略高度(默认原图高度)</param>
        /// <param name="filecache">原文件处理方式</param>
        /// <param name="newfilename">新图片名称</param>
        /// <param name="warning">处理警告信息</param>
        /// <returns>错误,返回错误信息;成功,返回图片路径</returns>
        public static string Resizepic(string picpath, ResizeType model, string spath, ImageType imgtype, double? mwidth,
            double? mheight, FileCache filecache, string newfilename, out string warning)
        {
            //反馈信息
            var checkmessage = new StringBuilder();

            //文件保存路径
            spath = string.IsNullOrEmpty(spath) ? "/" : spath;

            //缩略宽度
            var swidth = mwidth.HasValue ? double.Parse(mwidth.ToString()) : 0;

            //缩略高度
            var sheight = mheight.HasValue ? double.Parse(mheight.ToString()) : 0;

            //从指定源图片,创建image对象
            var sourceimgCommonMappath = "";

            //检测源文件
            var checkfile = false;
            checkfile = FileExistMapPath(picpath, FileCheckModel.C, out sourceimgCommonMappath);

            Image sourceimgCommon = null;
            Bitmap currimgCommon = null;
            Graphics gCommon = null;

            if (checkfile)
            {
                //从源文件创建imgage
                sourceimgCommon = Image.FromFile(sourceimgCommonMappath);

                #region 缩略模式

                //缩略模式
                switch (model)
                {
                    case ResizeType.X:

                        #region X模式

                        //根据给定尺寸,获取绘制比例
                        var widthScale = swidth / sourceimgCommon.Width;
                        //高着比例
                        sheight = sourceimgCommon.Height * widthScale;

                        #endregion 
                        break;
                    case ResizeType.Y:

                        #region Y模式

                        //根据给定尺寸,获取绘制比例
                        var heightScale = sheight / sourceimgCommon.Height;
                        //宽着比例
                        swidth = sourceimgCommon.Width * heightScale;

                        #endregion 
                        break;
                    case ResizeType.Xy:

                        #region XY模式

                        //当选择XY模式时,mwidth,mheight为必须值
                        if (swidth < 0 || sheight < 0)
                        {
                            checkmessage.Append("error:XY模式,mwidth,mheight为必须值;");
                        }
                        if (swidth.Equals(0) || sheight.Equals(0))
                        {
                            sheight = sourceimgCommon.Height;
                            swidth = sourceimgCommon.Width;
                        }

                        #endregion
                        break;

                    default:

                        #region 默认XY模式

                        //当默认XY模式时,mwidth,mheight为必须值
                        if (swidth < 0 || sheight < 0)
                        {
                            checkmessage.Append("error:你当前未选择缩略模式,系统默认XY模式,mwidth,mheight为必须值;");
                        }
                        break;

                    #endregion
                }

                #endregion
            }
            else
            {
                checkmessage.Append("error:未能找到缩略原图片," + picpath + ";");
            }

            if (string.IsNullOrEmpty(checkmessage.ToString()))
            {
                //创建bitmap对象
                currimgCommon = new Bitmap((int)swidth, (int)sheight);

                gCommon = Graphics.FromImage(currimgCommon);

                //设置画笔
                gCommon.CompositingMode = CompositingMode.SourceOver;
                gCommon.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gCommon.PixelOffsetMode = PixelOffsetMode.Half;

                //绘制图片
                if (sourceimgCommon != null)
                {
                    gCommon.DrawImage(sourceimgCommon, new Rectangle(0, 0, (int)swidth, (int)sheight),
                        new Rectangle(0, 0, sourceimgCommon.Width, sourceimgCommon.Height), GraphicsUnit.Pixel);

                    //保存图片
                    var spathCommonMappath = "";

                    //获取图片类型的hashcode值,生成图片后缀名
                    var extro = imgtype.GetHashCode();

                    var extend = extro == 0 ? ".jpg" : (extro == 1 ? ".gif" : (extro == 2 ? ".png" : ".jpg"));

                    //全局文件名
                    spath = spath + (string.IsNullOrEmpty(newfilename) ? Guid.NewGuid().ToString("N") : newfilename) + extend;

                    FileExistMapPath(spath, FileCheckModel.M, out spathCommonMappath);

                    switch (imgtype)
                    {
                        case ImageType.Jpeg:
                            currimgCommon.Save(spathCommonMappath, ImageFormat.Jpeg);
                            break;
                        case ImageType.Gif:
                            currimgCommon.Save(spathCommonMappath, ImageFormat.Gif);
                            break;
                        case ImageType.Png:
                            currimgCommon.Save(spathCommonMappath, ImageFormat.Png);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("imgtype", imgtype, null);
                    }

                    //释放
                    sourceimgCommon.Dispose();
                }
                currimgCommon.Dispose();
                gCommon.Dispose();

                //处理原文件
                var filecachecode = filecache.GetHashCode();

                //文件缓存方式:Delete,删除原文件
                if (filecachecode == 1)
                {
                    File.Delete(sourceimgCommonMappath);
                }

                //返回相对虚拟路径
                warning = "";
                return spath;
            }

            //释放
            if (sourceimgCommon != null)
            {
                sourceimgCommon.Dispose();
            }

            warning = checkmessage.ToString().TrimEnd(';');

            return "";
        }

        #endregion

        #region 文件上传

        /// <summary>
        /// 使用流方式,上传并保存文件
        /// </summary>
        /// <param name="s">文件流</param>
        /// <param name="savepath">保存文件路径</param>
        /// <param name="filename">设置图片名称</param>
        /// <param name="warning">处理警告信息,若为空,则处理成功</param>
        /// <returns>返回文件保存完整路径</returns>
        public static string Uploadbystream(Stream s, string savepath, string filename, out string warning)
        {
            if (s.Length > 0)
            {
                var ms = new MemoryStream();

                var be = 0;
                while ((be = s.ReadByte()) != -1)
                {
                    ms.WriteByte((byte)be);
                }

                var newpath = "";
                if (!string.IsNullOrEmpty(filename))
                {
                    newpath = savepath + filename + ".jpg";
                }
                else
                {
                    newpath = savepath + String.Format("{0}", DateTime.Now.Ticks) + ".jpg";
                }


                var lastpath = "";

                if (!FileExistMapPath(newpath, FileCheckModel.C, out lastpath))
                {
                    lastpath = HttpContext.Current.Server.MapPath(newpath);
                    //创建目录

                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath(savepath));
                }

                var fs = new FileStream(lastpath, FileMode.Create);

                ms.WriteTo(fs);

                //释放
                ms.Dispose();
                fs.Dispose();

                warning = "";
                return newpath;
            }

            warning = "error";
            return "";
        }




        /// <summary>
        /// 使用流方式,上传并保存文件
        /// </summary>
        /// <param name="s">文件流</param>
        /// <param name="savepath">保存文件路径</param>
        /// <param name="fileName">保存图片名称</param>
        /// <returns>返回文件保存完整路径</returns>
        public static string UploadbystreamToName(Stream s, string savepath, string fileName)
        {
            if (s.Length > 0)
            {
                try
                {
                    var ms = new MemoryStream();

                    var be = 0;
                    while ((be = s.ReadByte()) != -1)
                    {
                        ms.WriteByte((byte)be);
                    }

                    var newpath = savepath + fileName + ".jpg";

                    var lastpath = "";

                    if (!FileExistMapPath(newpath, FileCheckModel.C, out lastpath))
                    {
                        lastpath = HttpContext.Current.Server.MapPath(newpath);
                        //创建目录

                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath(savepath));
                    }

                    var fs = new FileStream(lastpath, FileMode.Create);

                    ms.WriteTo(fs);

                    //释放
                    ms.Dispose();
                    fs.Dispose();
                    return newpath;
                }
                catch
                {
                    // ignored
                }
            }
            return "";
        }

        #endregion

        #region 删除图片

        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="imgurl">~/update/img</param>
        /// <returns></returns>
        public static bool DeleteImgByUrl(string imgurl)
        {
            if (!string.IsNullOrEmpty(imgurl))
            {
                try
                {
                    var imgpath = imgurl.Substring(0, imgurl.LastIndexOf('/') + 1);
                    var imgname = imgurl.Substring(imgurl.LastIndexOf('/') + 1, imgurl.Length - imgurl.LastIndexOf('/') - 1);
                    var lastpath = HttpContext.Current.Server.MapPath(imgpath);
                    var sourceimgCommonMappath = lastpath + imgname;
                    File.Delete(sourceimgCommonMappath);
                    return true;
                }
                catch
                {

                    return false;
                }

            }

            return false;
        }
        //有需要删除图片
        //
        //File.Delete(sourceimgCommonMappath);


        //if (!FileExistMapPath(newpath, FileCheckModel.C, out lastpath))
        //{
        //    lastpath = HttpContext.Current.Server.MapPath(newpath);
        //    //创建目录

        //    Directory.CreateDirectory(HttpContext.Current.Server.MapPath(savepath));
        //}
        //File.Delete(sourceimgCommonMappath);

        #endregion

        #region 图片水印处理

        /// <summary>
        ///     水印图片
        ///     【如果图片需要缩略,请使用skeletonize.Resizepic()方法对图片进行缩略】
        ///     返回图片虚拟路径,和一个警告信息,可根据此信息获取图片合成信息
        /// </summary>
        /// <param name="picpath">需要水印的图片路径</param>
        /// <param name="waterspath">水印图片路径</param>
        /// <param name="watermodel">水印模式</param>
        /// <param name="spath">文件保存路径</param>
        /// <param name="imgtype">保存文件类型</param>
        /// <param name="filecache">原文件处理方式</param>
        /// <param name="warning">处理警告信息</param>
        /// <returns>错误,返回错误信息;成功,返回图片路径</returns>
        public static string Makewatermark(string picpath, string waterspath, WaterType watermodel, string spath, ImageType imgtype, FileCache filecache, out string warning)
        {
            #region

            //反馈信息
            var checkmessage = new StringBuilder();

            //检测源文件
            var sourceimgCommonMappath = "";
            var checkfile = false;

            //检测水印源文件
            var sourceimgWaterMappath = "";
            var checkfilewater = false;

            checkfile = FileExistMapPath(picpath, FileCheckModel.C, out sourceimgCommonMappath);
            checkfilewater = FileExistMapPath(waterspath, FileCheckModel.C, out sourceimgWaterMappath);

            Image sourceimgCommon = null;
            Image sourceimgWater = null;

            if (checkfile)
            {
                //从指定源文件,创建image对象
                sourceimgCommon = Image.FromFile(sourceimgCommonMappath);
            }
            else
            {
                checkmessage.Append("error:找不到需要的水印图片!" + picpath + ";");
            }
            if (checkfilewater)
            {
                //从指定源文件,创建image对象
                sourceimgWater = Image.FromFile(sourceimgWaterMappath);
            }
            else
            {
                checkmessage.Append("error:找不到需要水印图片!" + waterspath + ";");
            }

            #endregion

            #region

            if (string.IsNullOrEmpty(checkmessage.ToString()))
            {
                //源图宽、高
                if (sourceimgCommon != null && sourceimgWater != null)
                {
                    var sourceimgCommonWidth = sourceimgCommon.Width;
                    var sourceimgCommonHeight = sourceimgCommon.Height;

                    //水印图片宽、高
                    var sourceimgWaterWidth = sourceimgWater.Width;
                    var sourceimgWaterHeight = sourceimgWater.Height;

                    #region 水印坐标

                    //水印坐标
                    var sourceimgWaterPointX = 0;
                    var sourceimgWaterPointY = 0;

                    switch (watermodel)
                    {
                        case WaterType.Center:
                            sourceimgWaterPointX = (sourceimgCommonWidth - sourceimgWaterWidth) / 2;
                            sourceimgWaterPointY = (sourceimgCommonHeight - sourceimgWaterHeight) / 2;
                            ;
                            break;
                        case WaterType.CenterDown:
                            sourceimgWaterPointX = (sourceimgCommonWidth - sourceimgWaterWidth) / 2;
                            sourceimgWaterPointY = sourceimgCommonHeight - sourceimgWaterHeight;
                            ;
                            break;
                        case WaterType.CenterUp:
                            sourceimgWaterPointX = (sourceimgCommonWidth - sourceimgWaterWidth) / 2;
                            sourceimgWaterPointY = 0;
                            ;
                            break;
                        case WaterType.LeftDown:
                            sourceimgWaterPointX = 0;
                            sourceimgWaterPointY = sourceimgCommonHeight - sourceimgWaterHeight;
                            ;
                            break;
                        case WaterType.LeftUp:
                            ;
                            break;
                        case WaterType.Random:
                            var r = new Random();
                            var xRandom = r.Next(0, sourceimgCommonWidth);
                            var yRandom = r.Next(0, sourceimgCommonHeight);

                            sourceimgWaterPointX = xRandom > sourceimgCommonWidth - sourceimgWaterWidth ? sourceimgCommonWidth - sourceimgWaterWidth : xRandom;

                            sourceimgWaterPointY = yRandom > sourceimgCommonHeight - sourceimgWaterHeight ? sourceimgCommonHeight - sourceimgWaterHeight : yRandom;

                            ;
                            break;
                        case WaterType.RightDown:
                            sourceimgWaterPointX = sourceimgCommonWidth - sourceimgWaterWidth;
                            sourceimgWaterPointY = sourceimgCommonHeight - sourceimgWaterHeight;
                            ;
                            break;
                        case WaterType.RightUp:
                            sourceimgWaterPointX = sourceimgCommonWidth - sourceimgWaterWidth;
                            sourceimgWaterPointY = 0;
                            ;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("watermodel", watermodel, null);
                    }

                    #endregion

                    //从源图创建画板
                    var gCommon = Graphics.FromImage(sourceimgCommon);

                    //设置画笔
                    gCommon.CompositingMode = CompositingMode.SourceOver;
                    gCommon.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    gCommon.PixelOffsetMode = PixelOffsetMode.Half;

                    //绘制水印图片
                    gCommon.DrawImage(sourceimgWater, new Rectangle(sourceimgWaterPointX, sourceimgWaterPointY, sourceimgWaterWidth, sourceimgWaterHeight), new Rectangle(0, 0, sourceimgWaterWidth, sourceimgWaterHeight), GraphicsUnit.Pixel);

                    //保存图片
                    var spathCommonMappath = "";
                    //全局文件名

                    //获取图片类型的hashcode值,生成图片后缀名
                    var extro = imgtype.GetHashCode();
                    var extend = extro == 0 ? ".jpg" : (extro == 1 ? ".gif" : (extro == 2 ? ".png" : ".jpg"));

                    spath = spath + Guid.NewGuid() + extend;

                    FileExistMapPath(spath, FileCheckModel.M, out spathCommonMappath);

                    switch (imgtype)
                    {
                        case ImageType.Jpeg:
                            sourceimgCommon.Save(spathCommonMappath, ImageFormat.Jpeg);
                            break;
                        case ImageType.Gif:
                            sourceimgCommon.Save(spathCommonMappath, ImageFormat.Gif);
                            break;
                        case ImageType.Png:
                            sourceimgCommon.Save(spathCommonMappath, ImageFormat.Png);
                            break;
                    }


                    //释放
                    sourceimgCommon.Dispose();
                    sourceimgWater.Dispose();
                    gCommon.Dispose();
                }

                //处理原文件
                var filecachecode = filecache.GetHashCode();
                //删除原文件
                if (filecachecode == 1)
                {
                    File.Delete(sourceimgCommonMappath);
                }

                warning = "";
                return spath;
            }

            #endregion

            //释放
            if (sourceimgCommon != null)
            {
                sourceimgCommon.Dispose();
            }
            if (sourceimgWater != null)
            {
                sourceimgWater.Dispose();
            }

            warning = checkmessage.ToString().TrimEnd(';');
            return "";
        }

        #endregion

        #region 物理删除图片
        /// <summary>
        /// 删除磁盘中文件
        /// </summary> 
        /// <param name="imageUrl">文件物理路径</param>
        public static bool DeleteDiskFile(string imageUrl)
        {
            bool fbool;
            try
            {

                var fileUrl = HttpContext.Current.Server.MapPath(imageUrl);

                if (File.Exists(fileUrl))
                {
                    File.Delete(fileUrl);

                    // System.IO.Directory.Delete(Server.MapPath("file"), true);//删除文件夹以及文件夹中的子目录，文件   
                }
                fbool = true;
            }
            catch
            {
                fbool = false;
            }
            return fbool;
        }
        #endregion
    }
}
