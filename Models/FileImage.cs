/*
 * hrj
 * 2017-08-23
 * 文件图片处理需要的类
 * 
 */
 

namespace CommonTools.Models
{
    /// <summary>
    ///  图片上传专用model
    /// </summary>
    public class JsonPhoto
    {
        /// <summary>
        ///   图片地址
        /// </summary>
        public string Result { get; set; }
        public string ImageUrl { get; set; }
        public int State { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    ///     图片剪切专用
    /// </summary>
    public class ImageCrop
    {
        /// <summary>
        ///     x起始坐标
        /// </summary>
        public decimal X { get; set; }

        /// <summary>
        ///     y起始坐标
        /// </summary>
        public decimal Y { get; set; }

        /// <summary>
        ///     高度
        /// </summary>
        public decimal Height { get; set; }

        /// <summary>
        ///     宽度
        /// </summary>
        public decimal Width { get; set; }

        /// <summary>
        ///     旋转度
        /// </summary>
        public decimal Rotate { get; set; }
    }

}