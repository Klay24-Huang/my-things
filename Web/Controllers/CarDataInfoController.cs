﻿using Domain.SP.BE.Input;
using Domain.SP.Output;
using Domain.TB.BackEnd;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models.Enum;
using Web.Models.Params.Search.Input;
using WebAPI.Models.BaseFunc;
using WebCommon;

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
            ViewData["CarNo"] = CarNo;
            ViewData["StationID"] = StationID;
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
            //GetCarBindData
            return View();
        }
        [HttpPost]
        public ActionResult CarBind(string CarNo, string CID,int BindStatus,string Mode, HttpPostedFileBase fileImport)
        {
            //GetCarBindData
            CarStatusCommon carStatusCommon = new CarStatusCommon(connetStr);
            ViewData["BindStatus"] = BindStatus;
            ViewData["CarNo"] = CarNo;
            ViewData["CID"] = CID;
            ViewData["Mode"] = Mode;
            ViewData["errorLine"] = null;
            ViewData["IsShowMessage"] = null;
            if (Mode == "Query")
            {
                List<BE_GetCarBindData> lstData = new List<BE_GetCarBindData>();
                lstData = carStatusCommon.GetCarBindData(CarNo, CID, BindStatus);
                return View(lstData);
            }
            else
            {
                return View();
            }
           
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
            ViewData["CarNo"] = CarNo;
            ViewData["StationID"] = StationID;
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
        [Obsolete]
        [HttpPost]
        public ActionResult ImportMotorData(HttpPostedFileBase fileImport)
        {
            string errorLine = "";
            string errorMsg = "";
            bool flag = true;
            ViewData["errorLine"] = null;
            ViewData["IsShowMessage"] = null;
            List<SPInput_BE_InsTransParking> lstData = new List<SPInput_BE_InsTransParking>();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string errCode = "";
            CommonFunc baseVerify = new CommonFunc();
            if (fileImport != null)
            {
                if (fileImport.ContentLength > 0)
                {
                    string fileName = string.Concat(new string[]{
                    "ImportMotoData_",
                    ((Session["Account"]==null)?"":Session["Account"].ToString()),
                    "_",
                    DateTime.Now.ToString("yyyyMMddHHmmss"),
                    ".xlsx"
                    });
                    DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/Content/upload/ImportMotoData"));
                    if (!di.Exists)
                    {
                        di.Create();
                    }
                    string path = Path.Combine(Server.MapPath("~/Content/upload/ImportMotoData"), fileName);
                    fileImport.SaveAs(path);
                    IWorkbook workBook = new XSSFWorkbook(path);
                    ISheet sheet = workBook.GetSheetAt(0);
                    int sheetLen = sheet.LastRowNum;
                    string[] field = {"CarNo","TSEQNO","CarType","Seat","FactoryYear","CarColor","EngineNO","BodyNO","CCNum"};
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
                        string SPName = new ObjType().GetSPName(ObjType.SPType.ImportMotoData);
                        for (int i = 1; i <= sheetLen; i++)
                        {


                            SPInput_BE_ImportMotoData data = new SPInput_BE_ImportMotoData()
                            {

                                CarNo = sheet.GetRow(i).GetCell(0).ToString().Replace(" ", ""),
                                TSEQNO = Convert.ToInt32(sheet.GetRow(i).GetCell(1).ToString().Replace(" ", "")),
                                CarType = sheet.GetRow(i).GetCell(2).ToString().Replace(" ", "").PadLeft(6,'0'),
                                Seat = Convert.ToInt16(sheet.GetRow(i).GetCell(3).ToString().Replace(" ", "")),
                                FactoryYear = sheet.GetRow(i).GetCell(4).ToString().Replace(" ", ""),
                                CarColor = sheet.GetRow(i).GetCell(5).ToString().Replace(" ", ""),
                                EngineNO = sheet.GetRow(i).GetCell(6).ToString().Replace(" ", ""),
                                BodyNO = sheet.GetRow(i).GetCell(7).ToString().Replace(" ", ""),
                                CCNum = Convert.ToInt32(sheet.GetRow(i).GetCell(8).ToString().Replace(" ", "")),
                                IsMotor=1,
                                UserID = UserId,
                                LogID = 0
                            };


                            if (flag)
                            {
                                SPOutput_Base SPOutput = new SPOutput_Base();
                                flag = new SQLHelper<SPInput_BE_ImportMotoData, SPOutput_Base>(connetStr).ExecuteSPNonQuery(SPName, data, ref SPOutput, ref lstError);
                                baseVerify.checkSQLResult(ref flag, SPOutput.Error, SPOutput.ErrorCode, ref lstError, ref errCode);
                                if (flag == false)
                                {
                                    errorLine = i.ToString();
                                    errorMsg = string.Format("寫入第{0}筆資料時，發生錯誤：{1}", i.ToString(), baseVerify.GetErrorMsg(errCode));
                                }
                            }

                        }

                    }
                    else
                    {
                        ViewData["errorMsg"] = "未上傳檔案";
                    }
                }
                else
                {
                    flag = false;
                    errorMsg = "未上傳檔案";

                }
            }
            else
            {
                flag = false;
                errorMsg = "未上傳檔案";
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


            return View();
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
        [Obsolete]
        [HttpPost]
        public ActionResult ImportCarData(HttpPostedFileBase fileImport)
        {
            string errorLine = "";
            string errorMsg = "";
            bool flag = true;
            ViewData["errorLine"] = null;
            ViewData["IsShowMessage"] = null;
            List<SPInput_BE_InsTransParking> lstData = new List<SPInput_BE_InsTransParking>();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string errCode = "";
            CommonFunc baseVerify = new CommonFunc();
            if (fileImport != null)
            {
                if (fileImport.ContentLength > 0)
                {
                    string fileName = string.Concat(new string[]{
                    "ImportCarData_",
                    ((Session["Account"]==null)?"":Session["Account"].ToString()),
                    "_",
                    DateTime.Now.ToString("yyyyMMddHHmmss"),
                    ".xlsx"
                    });
                    DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/Content/upload/ImportCarData"));
                    if (!di.Exists)
                    {
                        di.Create();
                    }
                    string path = Path.Combine(Server.MapPath("~/Content/upload/ImportCarData"), fileName);
                    fileImport.SaveAs(path);
                    IWorkbook workBook = new XSSFWorkbook(path);
                    ISheet sheet = workBook.GetSheetAt(0);
                    int sheetLen = sheet.LastRowNum;
                    string[] field = { "CarNo", "TSEQNO", "CarType", "Seat", "FactoryYear", "CarColor", "EngineNO", "BodyNO", "CCNum" };
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
                        string SPName = new ObjType().GetSPName(ObjType.SPType.ImportCarData);
                        for (int i = 1; i <= sheetLen; i++)
                        {


                            SPInput_BE_ImportCarData data = new SPInput_BE_ImportCarData()
                            {

                                CarNo = sheet.GetRow(i).GetCell(0).ToString().Replace(" ", ""),
                                TSEQNO = Convert.ToInt32(sheet.GetRow(i).GetCell(1).ToString().Replace(" ", "")),
                                CarType = sheet.GetRow(i).GetCell(2).ToString().Replace(" ", "").PadLeft(6, '0'),
                                Seat = Convert.ToInt16(sheet.GetRow(i).GetCell(3).ToString().Replace(" ", "")),
                                FactoryYear = sheet.GetRow(i).GetCell(4).ToString().Replace(" ", ""),
                                CarColor = sheet.GetRow(i).GetCell(5).ToString().Replace(" ", ""),
                                EngineNO = sheet.GetRow(i).GetCell(6).ToString().Replace(" ", ""),
                                BodyNO = sheet.GetRow(i).GetCell(7).ToString().Replace(" ", ""),
                                CCNum = Convert.ToInt32(sheet.GetRow(i).GetCell(8).ToString().Replace(" ", "")),
                                IsMotor = 1,
                                UserID = UserId,
                                LogID = 0
                            };


                            if (flag)
                            {
                                SPOutput_Base SPOutput = new SPOutput_Base();
                                flag = new SQLHelper<SPInput_BE_ImportCarData, SPOutput_Base>(connetStr).ExecuteSPNonQuery(SPName, data, ref SPOutput, ref lstError);
                                baseVerify.checkSQLResult(ref flag, SPOutput.Error, SPOutput.ErrorCode, ref lstError, ref errCode);
                                if (flag == false)
                                {
                                    errorLine = i.ToString();
                                    errorMsg = string.Format("寫入第{0}筆資料時，發生錯誤：{1}", i.ToString(), baseVerify.GetErrorMsg(errCode));
                                }
                            }

                        }

                    }
                    else
                    {
                        ViewData["errorMsg"] = "未上傳檔案";
                    }
                }
                else
                {
                    flag = false;
                    errorMsg = "未上傳檔案";

                }
            }
            else
            {
                flag = false;
                errorMsg = "未上傳檔案";
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


            return View();
   
        }
        public ActionResult ImportCarMachineData()
        {
            return View();
        }
        /// <summary>
        /// 匯入車機資料（含SIM）
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        [HttpPost]
        public ActionResult ImportCarMachineData(HttpPostedFileBase fileImport)
        {
            string errorLine = "";
            string errorMsg = "";
            bool flag = true;
            ViewData["errorLine"] = null;
            ViewData["IsShowMessage"] = null;
            List<SPInput_BE_InsTransParking> lstData = new List<SPInput_BE_InsTransParking>();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string errCode = "";
            CommonFunc baseVerify = new CommonFunc();
            if (fileImport != null)
            {
                if (fileImport.ContentLength > 0)
                {
                    string fileName = string.Concat(new string[]{
                    "ImportCarMachineData_",
                    ((Session["Account"]==null)?"":Session["Account"].ToString()),
                    "_",
                    DateTime.Now.ToString("yyyyMMddHHmmss"),
                    ".xlsx"
                    });
                    DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/Content/upload/ImportCarMachineData"));
                    if (!di.Exists)
                    {
                        di.Create();
                    }
                    string path = Path.Combine(Server.MapPath("~/Content/upload/ImportCarMachineData"), fileName);
                    fileImport.SaveAs(path);
                    IWorkbook workBook = new XSSFWorkbook(path);
                    ISheet sheet = workBook.GetSheetAt(0);
                    int sheetLen = sheet.LastRowNum;
                    string[] field = { "車機編號", "門號(遠傳)", "卡號(遠傳)","deviceToken" };
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
                        string SPName = new ObjType().GetSPName(ObjType.SPType.ImportCarMachineData);
                        for (int i = 1; i <= sheetLen; i++)
                        {


                            SPInput_BE_ImportCarMachineData data = new SPInput_BE_ImportCarMachineData()
                            {

                                CID = sheet.GetRow(i).GetCell(0).ToString().Replace(" ", ""),
                                MobileNum = sheet.GetRow(i).GetCell(1).ToString().Replace(" ", ""),
                                SIMCardNo = sheet.GetRow(i).GetCell(2).ToString().Replace(" ", ""),
                                deviceToken= sheet.GetRow(i).GetCell(3).ToString().Replace(" ", ""),
                                UserID = UserId,
                                LogID = 0
                            };


                            if (flag)
                            {
                                SPOutput_Base SPOutput = new SPOutput_Base();
                                flag = new SQLHelper<SPInput_BE_ImportCarMachineData, SPOutput_Base>(connetStr).ExecuteSPNonQuery(SPName, data, ref SPOutput, ref lstError);
                                baseVerify.checkSQLResult(ref flag, SPOutput.Error, SPOutput.ErrorCode, ref lstError, ref errCode);
                                if (flag == false)
                                {
                                    errorLine = i.ToString();
                                    errorMsg = string.Format("寫入第{0}筆資料時，發生錯誤：{1}", i.ToString(), baseVerify.GetErrorMsg(errCode));
                                }
                            }

                        }

                    }
                    else
                    {
                        ViewData["errorMsg"] = "未上傳檔案";
                    }
                }else{
                    flag = false;
                    errorMsg = "未上傳檔案";

                }
            }
            else
            {
                flag = false;
                errorMsg = "未上傳檔案";
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
            
          
            return View();
        }
    }
}