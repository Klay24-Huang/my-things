using Domain.SP.BE.Input;
using Domain.SP.Output;
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
using Web.Models.Enum;
using WebAPI.Models.BaseFunc;
using WebCommon;

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
        [HttpPost]
        public ActionResult StationInfoSetting(string StationID,int? NotMach)
        {
            ViewData["StationID"] = StationID;
            ViewData["NotMuch"] = (NotMach.HasValue)?"1":"0";
            List<BE_GetPartOfStationInfo> lstData = null;
            StationAndCarRepository repository = new StationAndCarRepository(connetStr);
            lstData = repository.GetPartOfStation(StationID, NotMach.HasValue);
            return View(lstData);
        }
        /// <summary>
        /// 據點資訊新增
        /// </summary>
        /// <returns></returns>
        public ActionResult StationInfoAdd()
        {
            return View();
        }
        /// <summary>
        /// 據點資訊修改
        /// </summary>
        /// <returns></returns>
        public ActionResult StationInfoMaintain()
        {
            return View();
        }
        [HttpPost]
        public ActionResult StationInfoMaintain(string MaintainStationID)
        {
            BE_StationDetailCombind Data = null;
            if (string.IsNullOrWhiteSpace(MaintainStationID))
            {
                RedirectToAction("StationInfoAdd");
            }
            else
            {
                StationAndCarRepository repository = new StationAndCarRepository(connetStr);
                Data = new BE_StationDetailCombind()
                {
                    detail = repository.GetStationData(MaintainStationID),
                    StationImage = repository.GetStationInfo(MaintainStationID),
                    StationPolygon = repository.GetStationPolygon(MaintainStationID)
                };
            }
            return View(Data);
        }
        /// <summary>
        /// 電子柵欄
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PolygonMaintain(string pStationID)
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
        [Obsolete]
        public ActionResult TransParkingSetting(string ddlObj,string ParkingName, HttpPostedFileBase fileImport)
        {
            string Mode = ddlObj;
            string errorLine = "";
            string errorMsg = "";
            bool flag = true;
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
                List<SPInput_BE_InsTransParking> lstData = new List<SPInput_BE_InsTransParking>();
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                string errCode = "";
                CommonFunc baseVerify = new CommonFunc();
                if (fileImport != null)
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
                        string[] field = { "停車場名稱", "地址", "經度", "緯度", "開放時間(起)", "開放時間(迄)" };
                        int fieldLen = field.Length;
                        //第一關，判斷位置是否相等
                        for (int i = 0; i < fieldLen; i++)
                        {
                            ICell headCell = sheet.GetRow(0).GetCell(i);
                            if (headCell.ToString().Replace(" ", "").ToUpper() != field[i])
                            {
                                errorLine = "標題列不相符";
                                flag = false;
                                break;
                            }
                        }
                        //通過第一關 
                        if (flag)
                        {
                            string UserId = ((Session["Account"] == null) ? "" : Session["Account"].ToString());
                            string SPName = new ObjType().GetSPName(ObjType.SPType.InsTransParking);
                            for (int i = 1; i <= sheetLen; i++)
                            {
                                decimal Longitude = 0, Latitude = 0;
                                DateTime SD = DateTime.Now, ED = DateTime.Now;
                                SPInput_BE_InsTransParking data = new SPInput_BE_InsTransParking()
                                {
                                    ParkingName = sheet.GetRow(i).GetCell(0).ToString().Replace(" ", ""),
                                    ParkingAddress = sheet.GetRow(i).GetCell(1).ToString().Replace(" ", ""),
                                    UserID = UserId
                                };
                                flag = Decimal.TryParse(sheet.GetRow(i).GetCell(2).ToString(), out Longitude);
                                if (flag == false)
                                {
                                    errorMsg = string.Format("第{0}行的經度{1}不是正確的值", i.ToString(), sheet.GetRow(i).GetCell(2).ToString());
                                    break;
                                }
                                else
                                {
                                    data.Longitude = Longitude;
                                }
                                if (flag)
                                {
                                    flag = Decimal.TryParse(sheet.GetRow(i).GetCell(3).ToString(), out Latitude);
                                    if (flag == false)
                                    {
                                        errorMsg = string.Format("第{0}行的緯度{1}不是正確的值", i.ToString(), sheet.GetRow(i).GetCell(3).ToString());
                                        break;
                                    }
                                    else
                                    {
                                        data.Latitude = Latitude;
                                    }
                                }
                                if (flag)
                                {
                                    flag = DateTime.TryParse(sheet.GetRow(i).GetCell(4).ToString(), out SD);
                                    if (flag == false)
                                    {
                                        errorMsg = string.Format("第{0}行的開放時間(起)：{1}不是正確的日期格式", i.ToString(), sheet.GetRow(i).GetCell(4).ToString());
                                        break;
                                    }
                                    else
                                    {
                                        data.OpenTime = SD;
                                    }
                                }
                                if (flag)
                                {
                                    flag = DateTime.TryParse(sheet.GetRow(i).GetCell(5).ToString(), out ED);
                                    if (flag == false)
                                    {
                                        errorMsg = string.Format("第{0}行的開放時間(迄)：{1}不是正確的日期格式", i.ToString(), sheet.GetRow(i).GetCell(5).ToString());
                                        break;
                                    }
                                    else
                                    {
                                        data.CloseTime = ED;
                                    }
                                }

                                if (flag)
                                {
                                    SPOutput_Base SPOutput = new SPOutput_Base();
                                    flag = new SQLHelper<SPInput_BE_InsTransParking, SPOutput_Base>(connetStr).ExecuteSPNonQuery(SPName, data, ref SPOutput, ref lstError);
                                    baseVerify.checkSQLResult(ref flag, SPOutput.Error, SPOutput.ErrorCode, ref lstError, ref errCode);
                                    if (flag == false)
                                    {
                                        errorLine = i.ToString();
                                        errorMsg = string.Format("寫入第{0}筆資料時，發生錯誤：{1}", i.ToString(), baseVerify.GetErrorMsg(errCode));
                                    }
                                }

                            }

                        }

                    

                    }
                    else
                    {
                        flag = false;
                        errorMsg = "請上傳要匯入的資料";
                    }
                }
                else
                {
                    flag = false;
                    errorMsg = "請上傳要匯入的資料";
                }
                if (flag)
                {
                    ViewData["errorLine"] = "ok";
                }
                else
                {
                    ViewData["errorMsg"] = errorMsg;
                    ViewData["errorLine"] = errorLine.ToString();
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
        [HttpPost]
        public ActionResult ChargeParkingSetting(string ParkingName)
        {
            ViewData["ParkingName"] = ParkingName;
            List<BE_ChargeParkingData> lstData = null;
            ParkingRepository repository = new ParkingRepository(connetStr);
            lstData = repository.GetChargeParking(ParkingName);
            return View(lstData);
        }

        #region 共用元件類型
        #endregion
    }
}