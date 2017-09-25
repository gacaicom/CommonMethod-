using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CommonTools.Commonts
{
    public class StaticPageHelper
    {
        /// <summary>
        /// 生成静态页面
        /// </summary>
        /// <param name="strStaticPageAbsolutePath">生成静态页面的名称</param>
        /// <param name="strStaticPageRelativeDirectory">静态页面所在相对目录</param>
        /// <param name="strStaticPageBasicPath">静态页面所在基础路径</param>
        /// <param name="strTemplateAbsolutePath">静态页面模板文件路径</param>
        /// <param name="arrPlaceholder">占位符数组</param>
        /// <param name="arrReplaceContent">要替换的内容数组</param>
        /// <param name="strMessage">返回信息</param>
        /// <returns>生成成功返回true,失败false</returns>
        public static bool GenerateStaticPage(string strStaticPageAbsolutePath, string strStaticPageRelativeDirectory, string strStaticPageBasicPath, string strTemplateAbsolutePath, string[] arrPlaceholder, string[] arrReplaceContent, out string strMessage)
        {
            string strStaticPageBaseDirectory = string.Format("{0}\\{1}", strStaticPageBasicPath.TrimEnd('\\'), strStaticPageRelativeDirectory.TrimEnd('\\'));
            return GenerateStaticPage(strStaticPageAbsolutePath, strStaticPageBaseDirectory, strTemplateAbsolutePath, arrPlaceholder, arrReplaceContent, out strMessage);
        }

        /// <summary>
        /// 生成静态页面
        /// </summary>
        /// <param name="strStaticPageAbsolutePath">生成静态页面的名称</param> 
        /// <param name="strStaticPageBaseDirectory">静态页面所在基础目录</param>
        /// <param name="strTemplateAbsolutePath">静态页面模板文件路径</param>
        /// <param name="arrPlaceholder">占位符数组</param>
        /// <param name="arrReplaceContent">要替换的内容数组</param>
        /// <param name="strMessage">返回信息</param>
        /// <returns>生成成功返回true,失败false</returns>
        public static bool GenerateStaticPage(string strStaticPageAbsolutePath, string strStaticPageBaseDirectory, string strTemplateAbsolutePath, string[] arrPlaceholder, string[] arrReplaceContent, out string strMessage)
        {
            //获取静态页面完整路径
            string strStaticPageFullPath = string.Format("{0}\\{1}", strStaticPageBaseDirectory, strStaticPageAbsolutePath);
            return GenerateStaticPage(strStaticPageAbsolutePath, strTemplateAbsolutePath, arrPlaceholder, arrReplaceContent, out strMessage);
        }

        /// <summary>
        /// 根据静态的HTML页面模板生成静态页面
        /// </summary>
        /// <param name="strStaticPageAbsolutePath">存放静态页面所在绝对路径</param>
        /// <param name="strTemplateAbsolutePath">静态模板页面的绝对路径</param>
        /// <param name="arrPlaceholder">占位符数组</param>
        /// <param name="arrReplaceContent">要替换的内容数组</param>
        /// <param name="strMessage">返回信息</param>
        /// <returns>生成成功返回true,失败false</returns>
        public static bool GenerateStaticPage(string strStaticPageAbsolutePath, string strTemplateAbsolutePath, string[] arrPlaceholder, string[] arrReplaceContent, out string strMessage)
        {
            bool isSuccess = false;
            try
            {
                //生成存放静态页面目录                            
                if (!Directory.Exists(Path.GetDirectoryName(strStaticPageAbsolutePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(strStaticPageAbsolutePath));
                }
                //验证占位符
                if (arrPlaceholder.Length != arrReplaceContent.Length)
                {
                    strMessage = string.Format("生成静态页面失败,占位符数组个数和替换内容个数不相等！存放路径为：{0}", strStaticPageAbsolutePath);
                    return false;
                }
                //生成存放静态页面文件
                if (File.Exists(strStaticPageAbsolutePath))
                {
                    File.Delete(strStaticPageAbsolutePath);
                }
                //获取模板HTML
                StringBuilder strHtml = new StringBuilder();
                strHtml.Append(File.ReadAllText(strTemplateAbsolutePath, Encoding.UTF8));
                //替换模板占位符,获取要生成的静态页面HTML
                for (int i = 0; i < arrPlaceholder.Length; i++)
                {
                    strHtml.Replace(arrPlaceholder[i], arrReplaceContent[i]);
                }
                //生成静态页面
                File.WriteAllText(strStaticPageAbsolutePath, strHtml.ToString());
                strMessage = string.Format("生成静态页面成功！存放路径：{0}", strStaticPageAbsolutePath);
                isSuccess = true;
            }
            catch (IOException ex)
            {
                strMessage = ex.Message;
                isSuccess = false;
            }
            catch (Exception ex)
            {
                strMessage = ex.Message;
                isSuccess = false;
            }
            return isSuccess;
        }

        /// <summary>
        /// 根据View视图生成静态页面
        /// </summary>
        /// <param name="strStaticPageAbsolutePath">存放静态页面所在绝对路径</param>
        /// <param name="context">ControllerContext</param>
        /// <param name="strViewName">视图名称</param>
        /// <param name="strMasterName">模板视图名称</param>
        /// <param name="model">参数实体模型</param>
        /// <param name="strMessage">返回信息</param>
        /// <param name="isPartial">是否分布视图</param>
        /// <returns>生成成功返回true,失败false</returns>
        public static bool GenerateStaticPage(string strStaticPageAbsolutePath, ControllerContext context, string strViewName, string strMasterName, object model, out string strMessage, bool isPartial = false)
        {
            bool isSuccess = false;
            try
            {
                //创建存放静态页面目录                            
                if (!Directory.Exists(Path.GetDirectoryName(strStaticPageAbsolutePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(strStaticPageAbsolutePath));
                }
                //删除已有的静态页面
                if (File.Exists(strStaticPageAbsolutePath))
                {
                    File.Delete(strStaticPageAbsolutePath);
                }

                ViewEngineResult result = null;
                if (isPartial)
                {
                    result = ViewEngines.Engines.FindPartialView(context, strViewName);
                }
                else
                {
                    result = ViewEngines.Engines.FindView(context, strViewName, strMasterName);
                }

                if (model != null)
                {
                    context.Controller.ViewData.Model = model;
                }

                /*
                 * 设置临时数据字典作为静态化标识
                 * 可以在视图上使用TempData["IsStatic"]来控制某些元素显示。
                 */
                if (!context.Controller.TempData.ContainsKey("IsStatic"))
                {
                    context.Controller.TempData.Add("IsStatic", true);
                }

                if (result.View != null)
                {
                    using (var sw = new StringWriter())
                    {
                        string strResultHtml = string.Empty;
                        //填充数据模型到视图中，并获取完整的页面
                        ViewContext viewContext = new ViewContext(context, result.View, context.Controller.ViewData, context.Controller.TempData, sw);
                        result.View.Render(viewContext, sw);
                        strResultHtml = sw.ToString();
                        //通过IO操作将页面内容生成静态页面
                        File.WriteAllText(strStaticPageAbsolutePath, strResultHtml);
                        strMessage = string.Format("生成静态页面成功！存放路径：{0}", strStaticPageAbsolutePath);
                        isSuccess = true;
                    }
                }
                else
                {
                    isSuccess = false;
                    strMessage = "生成静态页面失败！未找到视图！";
                }

            }
            catch (IOException ex)
            {
                strMessage = ex.Message;
                isSuccess = false;
            }
            catch (Exception ex)
            {
                strMessage = ex.Message;
                isSuccess = false;
            }
            return isSuccess;
        }
    }
}