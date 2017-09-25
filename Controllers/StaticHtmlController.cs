/*
 * 静态页面生成
 * mvc
 * 胡锐坚
 * 2017-08-23
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonTools.Commonts;
using CommonTools.Models;

namespace CommonTools.Controllers
{
    public class StaticHtmlController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 使用HTML模板静态化
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UseHtmlTemplateStatic()
        {
            string strMessage = string.Empty;
            //静态页面模板路径
            string strTemplateFullPath = string.Format("{0}StaticTemplate/{1}", AppDomain.CurrentDomain.BaseDirectory, "ArticleTemplate.html");
            //保存静态页面的绝对路径
            string strStaticPageAbsolutePath = GetStaticPageAbsolutePath();
            //获取模板占位符数组
            string[] arrPlaceholder = new string[4];
            arrPlaceholder[0] = "@template_placeholder_title";
            arrPlaceholder[1] = "@template_placeholder_author";
            arrPlaceholder[2] = "@template_placeholder_date";
            arrPlaceholder[3] = "@template_placeholder_content";

            //获取填充到模板中的占位符(自定义标识)所对应的数据数组
            Article entity = GetArticleModel();
            string[] arrReplaceContent = new string[4];
            arrReplaceContent[0] = entity.Title;
            arrReplaceContent[1] = entity.Authur;
            arrReplaceContent[2] = entity.InputDate.ToString("yyyy/MM/dd");
            arrReplaceContent[3] = entity.Content;

            StaticPageHelper.GenerateStaticPage(strStaticPageAbsolutePath, strTemplateFullPath, arrPlaceholder, arrReplaceContent, out strMessage);
            return Content("使用HTML模板生成静态页面-----" + strMessage);
        }

        /// <summary>
        /// 使用视图引擎静态化
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UseViewEngineStatic()
        {
            string strMessage = string.Empty;
            Article entity = GetArticleModel(true);

            //保存静态页面的绝对路径
            string strStaticPageAbsolutePath = GetStaticPageAbsolutePath();
            //生成静态页面,其中的Article是视图名称
            StaticPageHelper.GenerateStaticPage(strStaticPageAbsolutePath, ControllerContext, "Article", null, entity, out strMessage);

            return Content("使用视图引擎实现页面静态化-----" + strMessage);
        }
        /// <summary>
        /// 文章内容展示页
        /// </summary>
        /// <returns></returns>
        public ActionResult Article()
        {
            Article entity = GetArticleModel();
            return View(entity);
        }

        #region 自定义方法
        /// <summary>
        /// 获取文章数据模型，一般是要从数据库查询
        /// </summary>
        /// <param name="isViewEngine">是否使用视图引擎</param>
        /// <returns></returns>
        private Article GetArticleModel(bool isViewEngine = false)
        {
            Article entity = new Article();
            entity.Title = "ASP.NET MVC静态化DEMO";
            if (isViewEngine)
            {
                entity.Title += "----------------使用视图引擎";
            }
            else
            {
                entity.Title += "----------------使用HTML模板";
            }
            entity.Authur = "zkxc";
            entity.InputDate = DateTime.Now;
            entity.Content = "<p>分别演示了使用HTML模板和视图引擎这两种方式生成静态页面。</p>";
            return entity;
        }

        /// <summary>
        /// 获取保存静态页面绝对路径
        /// </summary>        
        /// <returns></returns>
        private string GetStaticPageAbsolutePath()
        {
            //静态页面名称
            string strStaticPageName = string.Format("{0}.html", DateTime.Now.Ticks.ToString());
            //静态页面相对路径
            string strStaticPageRelativePath = string.Format("Upload\\article\\{0}\\{1}", DateTime.Now.ToString("yyyy/MM").Replace('/', '\\'), strStaticPageName);
            //静态页面完整路径                                    
            string strStaticPageAbsolutePath = AppDomain.CurrentDomain.BaseDirectory + strStaticPageRelativePath;
            return strStaticPageAbsolutePath;
        }

        #endregion
    }
}