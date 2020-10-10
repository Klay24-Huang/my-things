﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class ContactManageController : Controller
    {
        /// <summary>
        /// 預約（合約）查詢
        /// </summary>
        /// <returns></returns>
        public ActionResult ContactQuery()
        {
            return View();
        }
        /// <summary>
        /// 訂單記錄歷程查詢
        /// </summary>
        /// <returns></returns>
        public ActionResult ContactHistoryQuery()
        {
            return View();
        }
        /// <summary>
        /// 機車合約修改
        /// </summary>
        /// <returns></returns>
        public ActionResult ContactMaintainOfMotor()
        {
            return View();
        }
        /// <summary>
        /// 汽車合約修改
        /// </summary>
        /// <returns></returns>
        public ActionResult ContactMaintainOfCar()
        {
            return View();
        }
        /// <summary>
        /// 時數折抵
        /// </summary>
        /// <returns></returns>
        public ActionResult ContactMaintainByDiscount()
        {
            return View();
        }
        /// <summary>
        /// 強制延長用車
        /// </summary>
        /// <returns></returns>
        public ActionResult ContactExtend()
        {
            return View();
        }
        /// <summary>
        /// 人工新增預約
        /// </summary>
        /// <returns></returns>
        public ActionResult ContactBooking()
        {
            return View();
        }
        /// <summary>
        /// 強制取還車（含作廢合約）
        /// </summary>
        /// <returns></returns>
        public ActionResult ContactSetting()
        {
            return View();
        }
    }
}