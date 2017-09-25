using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommonTools.Models
{
    public class Article
    {
        /// <summary>
        /// 文章标题
        /// </summary>        
        public string Title { get; set; }
        /// <summary>
        /// 文章内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 文章作者 
        /// </summary>
        public string Authur { get; set; }
        /// <summary>
        /// 发布日期
        /// </summary>
        public DateTime InputDate { get; set; }
    }
}