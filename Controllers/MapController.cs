/*
 * 百度地图
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
    public class MapController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        #region 测试地图
     

        /// <summary>
        /// 根据IP定位
        /// </summary>
        /// <returns></returns>
        public ActionResult MapIp()
        {
            return View();
        }

        /// <summary>
        /// 设置线面可编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult MapAreaEdit()
        {
            return View();
        }

        /// <summary>
        /// 可视区域内的搜素
        /// </summary>
        /// <returns></returns>
        public ActionResult MapSearch()
        {
            return View();
        }

        /// <summary>
        /// 添加工具条比例尺
        /// </summary>
        /// <returns></returns>
        public ActionResult MapTool1()
        {
            return View();
        }

        /// <summary>
        /// 添加行政区域
        /// </summary>
        /// <returns></returns>
        public ActionResult MapGovArea()
        {
            return View();
        }

        /// <summary>
        /// 热力图
        /// </summary>
        /// <returns></returns>
        public ActionResult MapHot()
        {
            return View();
        }


        /// <summary>
        /// 坐标信息
        /// </summary>
        /// <returns></returns>
        public ActionResult MapPointInfo()
        {
            return View();
        }
        public ActionResult MapPointsInfo()
        {
            return View();
        }

        /// <summary>
        /// 聚焦图
        /// </summary>
        /// <returns></returns>
        public ActionResult MapPoints()
        {
            return View();
        }

        /// <summary>
        /// 点击获取坐标
        /// </summary>
        /// <returns></returns>
        public ActionResult MapGetPoint()
        {
            return View();
        }

        /// <summary>
        /// 鼠标绘图工具
        /// </summary>
        /// <returns></returns>
        public ActionResult MapConvert()
        {
            return View();
        }

        /// <summary>
        /// 地图加载完毕做的事情
        /// </summary>
        /// <returns></returns>
        public ActionResult MapLoad()
        {
            return View();
        }

        public ActionResult MapSearchData()
        {
            return View();
        }

        public ActionResult MapPointName()
        {
            return View();
        }

        public ActionResult MapAddress()
        {
            return View();
        }

        /// <summary>
        /// 地址反解析加商圈
        /// </summary>
        /// <returns></returns>
        public ActionResult MapAddressInfo()
        {
            return View();
        }

        public ActionResult MapAreaPoints()
        {
            return View();
        }
        #endregion
    }
}