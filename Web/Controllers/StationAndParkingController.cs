using Domain.TB.BackEnd;
using Microsoft.SqlServer.Server;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
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
            string errorLine = "";
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
                if (fileImport.ContentLength > 0)
                {
                    string fileName = string.Concat(new string[]{
                    "TransParkingImport_",
                    ((Session["Account"]==null)?"":Session["Account"].ToString()),
                    "_",
                    DateTime.Now.ToString("yyyyMMddHHmmss"),
                    ".xlsx"
                    });
                    DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/Content/upload/TransParkingImport"));
                    if (!di.Exists)
                    {
                        di.Create();
                    }
                    string path = Path.Combine(Server.MapPath("~/Content/upload/TransParkingImport"), fileName);
                    fileImport.SaveAs(path);
                    IWorkbook workBook = new XSSFWorkbook(path);
                    ISheet sheet = workBook.GetSheetAt(0);
                    int sheetLen = sheet.LastRowNum;
                    string[] field = { "停車場名稱","地址","經度","緯度","開放時間(起)","開放時間(迄)"};
                    int fieldLen = field.Length;
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