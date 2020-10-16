using Domain.TB.BackEnd;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models.Params.Search.Input;

namespace Web.Controllers
{
    /// <summary>
    /// 車輛管理
    /// </summary>
    public class CarDataInfoController : Controller
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 車輛中控台
        /// </summary>
        /// <returns></returns>
        public ActionResult CarDashBoard()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CarDashBoard(FormCollection collection)
        {
            Input_CarDashBoard QueryData = null;
            CarStatusCommon carStatusCommon = new CarStatusCommon(connetStr);
            List<BE_CarDashBoardData> lstData = new List<BE_CarDashBoardData>();
            if (collection["queryData"] != null)
            {
                ViewData["QueryData"] = collection["queryData"];
                QueryData = Newtonsoft.Json.JsonConvert.DeserializeObject<Input_CarDashBoard>(collection["queryData"].ToString());
                if (QueryData != null)
                {
                    lstData = carStatusCommon.GetDashBoard(QueryData.CarNo, QueryData.StationID, QueryData.ShowType, QueryData.Terms);
                }

            }
            return View(lstData);
        }
        /// <summary>
        /// 保有車輛設定
        /// </summary>
        /// <returns></returns>
        public ActionResult CarSetting()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CarSetting(string CarNo,string StationID,int ShowType=3)
        {
            //BE_CarSettingData
            CarStatusCommon carStatusCommon = new CarStatusCommon(connetStr);
            ViewData["ShowType"] = ShowType;
            List<BE_CarSettingData> lstData = new List<BE_CarSettingData>();
            lstData = carStatusCommon.GetCarSettingData(CarNo, StationID, ShowType);
            return View(lstData);
        }
        /// <summary>
        /// 車輛車機綁定
        /// </summary>
        /// <returns></returns>
        public ActionResult CarBind()
        {
            return View();
        }
        /// <summary>
        /// 車輛資料管理
        /// </summary>
        /// <returns></returns>
        public ActionResult CarDataSetting()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CarDataSetting(string CarNo, string StationID, int ShowType = 3)
        {
            //BE_CarSettingData
            CarStatusCommon carStatusCommon = new CarStatusCommon(connetStr);
            ViewData["ShowType"] = ShowType;
            List<BE_GetPartOfCarDataSettingData> lstData = new List<BE_GetPartOfCarDataSettingData>();
            lstData = carStatusCommon.GetCarDataSettingData(CarNo, StationID, ShowType);
            return View(lstData);
        }
        [HttpPost]
        public ActionResult ViewCarDetail(string ShowCarNo)
        {
            CarStatusCommon carStatusCommon = new CarStatusCommon(connetStr);

            BE_GetCarDetail obj  = null;
            obj = carStatusCommon.GetCarDataSettingDetail(ShowCarNo);
            return View(obj);
           
        }
        /// <summary>
        /// 匯入機車車輛檔
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportMotorData()
        {
            return View();
        }
        /// <summary>
        /// 匯入汽車車輛檔
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportCarData()
        {
            return View();
        }
        /// <summary>
        /// 匯入車機資料（含SIM）
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportCarMachineData()
        {
            return View();
        }
    }
}