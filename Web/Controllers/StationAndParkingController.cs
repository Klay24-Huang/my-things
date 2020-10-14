using Domain.TB.BackEnd;
using Microsoft.SqlServer.Server;
using Reposotory.Implement;
using Reposotory.Implement.BackEnd;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
            ViewData["Mode"] = null;
            ViewData["ParkingName"] = null;
            ViewData["errorLine"] = null;
            ViewData["IsShowMessage"] = null;
            return View();
        }
        [HttpPost]
        public ActionResult TransParkingSetting(string ddlObj,string ParkingName, HttpPostedFileBase fileImport)
        {
            string Mode = ddlObj;
            List<BE_ParkingData> lstPark = null;
            ViewData["Mode"] = Mode;
            ViewData["ParkingName"] = ParkingName;
            ViewData["errorLine"] = null;
            ViewData["IsShowMessage"] = null;
            if (Mode == "Edit")
            {
                ParkingRepository repository = new ParkingRepository(connetStr);
                lstPark = repository.GetTransParking(ParkingName);
            }
            else
            {
                if (fileImport != null)
                {

                }
            }


            // return this.JavaScript(js);
            //return JavaScriptResult(js);
            if (Mode == "Edit")
            {
                return View(lstPark);
            }
            else
            {
                return View();
            }
       
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