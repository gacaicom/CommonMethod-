/*
 * 表单验证
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

namespace CommonTools.Controllers
{
    public class FormValidationController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        ///     判断用户证件号是否重复
        /// </summary>
        /// <param name="cardId">证件号码</param>
        /// <returns>1 存在 0 不存在</returns>
        [HttpPost]
        public string CheckUserCardId(string cardId)
        {
           
            return  "0";
        }
    }
}