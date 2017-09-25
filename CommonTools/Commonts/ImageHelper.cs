using System; 
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO; 
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text; 
using Encoder = System.Drawing.Imaging.Encoder;

namespace CommonTools.Commonts
{
    public class ImageHelper
    {
        public static string CropImage(byte[] content, int x, int y, int cropWidth, int cropHeight)
        {
            using (MemoryStream stream = new MemoryStream(content))
            {
                return CropImage(stream, x, y, cropWidth, cropHeight);
            }
        }

        /// <summary>
        /// 图片剪切返回流文件
        /// </summary>
        /// <param name="content"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="cropWidth"></param>
        /// <param name="cropHeight"></param>
        /// <returns></returns>
        public static string CropImage(Stream content, int x, int y, int cropWidth, int cropHeight)
        {
            using (Bitmap sourceBitmap = new Bitmap(content))
            {

                // 将选择好的图片缩放
                Bitmap bitSource = new Bitmap(sourceBitmap, sourceBitmap.Width, sourceBitmap.Height);

                Rectangle cropRect = new Rectangle(x, y, cropWidth, cropHeight);

                using (Bitmap newBitMap = new Bitmap(cropWidth, cropHeight))
                {
                    newBitMap.SetResolution(sourceBitmap.HorizontalResolution, sourceBitmap.VerticalResolution);
                    using (Graphics g = Graphics.FromImage(newBitMap))
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.DrawImage(bitSource, new Rectangle(0, 0, newBitMap.Width, newBitMap.Height), cropRect, GraphicsUnit.Pixel);

                        return GetBitmapBytes(newBitMap);
                    }
                }
            }
        }

        public static string GetBitmapBytes(Bitmap source)
        {
            ImageCodecInfo codec = ImageCodecInfo.GetImageEncoders()[4];
            EncoderParameters parameters = new EncoderParameters(1);
            parameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);

            using (MemoryStream tmpStream = new MemoryStream())
            {
                source.Save(tmpStream, codec, parameters);

                byte[] data = new byte[tmpStream.Length];
                tmpStream.Seek(0, SeekOrigin.Begin);
                tmpStream.Read(data, 0, (int)tmpStream.Length);

                string result = Convert.ToBase64String(data);
                return result;
            }
        }

         
        /// <summary>
        /// 图片转换Base64
        /// </summary>
        /// <param name="urlOrPath">图片网络地址或本地路径</param>
        /// <returns></returns>
        public static string ImageToBase64(string urlOrPath)
        {
            try
            {
                if (urlOrPath.Contains("http"))
                {
                    Uri url = new Uri(urlOrPath);

                    using (var webClient = new WebClient())
                    {
                        var imgData = webClient.DownloadData(url);

                        using (var ms = new MemoryStream(imgData))
                        {
                            byte[] data = new byte[ms.Length];
                            ms.Seek(0, SeekOrigin.Begin);
                            ms.Read(data, 0, Convert.ToInt32(ms.Length));

                            string netImage = Convert.ToBase64String(data);
                            return netImage;
                        }
                    }
                }
                else
                {
                    FileStream fileStream = new FileStream(urlOrPath, FileMode.Open);
                    Stream stream = fileStream as Stream;

                    byte[] data = new byte[stream.Length];
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.Read(data, 0, Convert.ToInt32(stream.Length));

                    string netImage = Convert.ToBase64String(data);
                    return netImage;
                }
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// 按比例缩放图片
        /// </summary>
        /// <param name="content"></param>
        /// <param name="resizeWidth"></param>
        /// <param name="resizeHeight"></param>
        /// <returns></returns>
        public static string ResizeImage(Stream content, int resizeWidth, int resizeHeight)
        {
            using (Bitmap sourceBitmap = new Bitmap(content))
            {
                int width = sourceBitmap.Width,
                    height = sourceBitmap.Height;
                if (height > resizeHeight || width > resizeWidth)
                {
                    if ((width * resizeHeight) > (height * resizeWidth))
                    {
                        resizeHeight = (resizeWidth * height) / width;
                    }
                    else
                    {
                        resizeWidth = (width * resizeHeight) / height;
                    }
                }
                else
                {
                    resizeWidth = width;
                    resizeHeight = height;
                }

                // 将选择好的图片缩放
                Bitmap bitSource = new Bitmap(sourceBitmap, resizeWidth, resizeHeight);
                bitSource.SetResolution(sourceBitmap.HorizontalResolution, sourceBitmap.VerticalResolution);
                using (MemoryStream stream = new MemoryStream())
                {
                    bitSource.Save(stream, ImageFormat.Jpeg);

                    byte[] data = new byte[stream.Length];
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.Read(data, 0, Convert.ToInt32(stream.Length));

                    string newImage = Convert.ToBase64String(data);
                    return newImage;
                }
            }

        }

        /// <summary> 
        /// 将一个object对象序列化，返回一个byte[]         
        /// </summary> 
        /// <param name="obj">能序列化的对象</param>         
        /// <returns></returns> 
        public static byte[] ObjectToBytes(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter(); formatter.Serialize(ms, obj); return ms.GetBuffer();
            }
        }

        /// <summary> 
        /// 将一个序列化后的byte[]数组还原         
        /// </summary>
        /// <param name="bytes"></param>         
        /// <returns></returns> 
        public static object BytesToObject(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                IFormatter formatter = new BinaryFormatter(); return formatter.Deserialize(ms);
            }
        }

        /// <summary>
        /// Base64Str 转byte[]
        /// </summary>
        /// <param name="strObj"></param>
        /// <param name="strEncoding">编码格式 utf-8</param>
        /// <returns></returns>
        public static byte[] Base64StrToBytes(string strObj, string strEncoding)
        {
            var myEncoding = Encoding.GetEncoding(!string.IsNullOrEmpty(strEncoding) ? strEncoding : "utf-8");
            return myEncoding.GetBytes(strObj); //Convert.FromBase64String(strObj);
        }

        /// <summary>
        /// Base64Str 转byte[]
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="strEncoding">编码格式 utf-8</param>
        /// <returns></returns>
        public static string BytesToBase64Str(byte[] bytes, string strEncoding)
        {
            var myEncoding = Encoding.GetEncoding(!string.IsNullOrEmpty(strEncoding) ? strEncoding : "utf-8");
            return myEncoding.GetString(bytes);
            //Convert.ToBase64String(bytes);
        }
    }
}