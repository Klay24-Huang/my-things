using Reposotory.Implement.BackEnd;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    /// <summary>
    /// 據點管理
    /// </summary>
    public class StationAndParkingController : Controller
    {
        private StationRepository _repository;
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 據點資訊設定
        /// </summary>
        /// <returns></returns>

        public ActionResult StationInfoSetting()
        {
            return View();
        }
        /// <summary>
        /// 調度停車場資訊設定
        /// </summary>
        /// <returns></returns>

        public ActionResult TransParkingSetting()
        {
            return View();
        }
        /// <summary>
        /// 停車便利付停車場設定
        /// </summary>
        /// <returns></returns>
 
        public ActionResult ChargeParkingSetting()
        {
            return View();
        }

        #region 共用元件類型
        #endregion
    }
}