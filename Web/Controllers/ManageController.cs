using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    /// <summary>
    /// 後台管理
    /// </summary>
    public class ManageController : Controller
    {
      /// <summary>
      /// 強制刷卡取款
      /// </summary>
      /// <returns></returns>
        public ActionResult AuthAndPay()
        {
            return View();
        }
        /// <summary>
        /// 欠費查詢
        /// </summary>
        /// <returns></returns>
        public ActionResult ArrearQuery()
        {
            return View();
        }
        /// <summary>
        /// 平假日維護
        /// </summary>
        /// <returns></returns>
        public ActionResult HoildayMaintain()
        {
            return View();
        }
        ///// <summary>
        ///// 卡號解除
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult UnBindCard()
        //{
        //    return View();
        //}
        /// <summary>
        /// 信用卡解綁
        /// </summary>
        /// <returns></returns>
        public ActionResult UnBindCreditCard()
        {
            return View();
        }
        /// <summary>
        /// 短租補傳
        /// </summary>
        /// <returns></returns>
        public ActionResult ResendToHieasyrent()
        {
            return View();
        }
    }
}