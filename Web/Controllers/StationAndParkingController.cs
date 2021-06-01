﻿using Domain.SP.BE.Input;
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
        public ActionResult StationInfoSetting(string StationID,int? NotMach, int? NotMach2)
        {
            ViewData["StationID"] = StationID;
            ViewData["NotMuch"] = (NotMach.HasValue)?"1":"0";
            ViewData["NotMuch2"] = (NotMach2.HasValue) ? "1" : "0";
            List<BE_GetPartOfStationInfo> lstData = null;
            StationAndCarRepository repository = new StationAndCarRepository(connetStr);

            lstData = repository.GetPartOfStation(StationID);

            if (NotMach == 1 && NotMach2 == 1)
            {
                lstData = lstData.Where(i => i.AllowParkingNum != i.TotalCar).Where(i => i.TotalCar != (i.TotalCar - i.UnavailbleCar)).ToList();
            }
            else
            {
                if (NotMach == 1)
                {
                    lstData = lstData.Where(i => i.TotalCar != (i.TotalCar - i.UnavailbleCar)).ToList();
                }
                if(NotMach2 == 1)
                {
                    lstData = lstData.Where(i => i.AllowParkingNum != i.TotalCar).ToList();
                }
            }

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
        public ActionResult PolygonMaintain(string pStationID)
        {
            if (!string.IsNullOrWhiteSpace(pStationID))
            {
                List<BE_GetPolygonCombindData> lstData = null;
                StationAndCarRepository repository = new StationAndCarRepository(connetStr);
                lstData = repository.GetStationPolygonCombind(pStationID);
                ViewData["StationID"] = pStationID;
                return View(lstData);
            }
            else
            {
                RedirectToAction("Login");
            }
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
                        string[] field = { "停車場名稱", "地址", "經度", "緯度", "開放時間(起)", "開放時間(迄)", "ID" };
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
                                    UserID = UserId,
                                    ID = sheet.GetRow(i).GetCell(6).ToString().Replace(" ", ""),
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
        public ActionResult ExplodeChargeParkingSetting(string ExplodeParkingName)
        {
            List<BE_ChargeParkingData> lstData = null;
            ParkingRepository repository = new ParkingRepository(connetStr);

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("搜尋結果");

            string[] headerField = { "ID", "名稱", "營運商", "地址", "緯度", "經度", "開放時間(起)", "開放時間(迄)" };
            int headerFieldLen = headerField.Length;

            IRow header = sheet.CreateRow(0);
            for (int j = 0; j < headerFieldLen; j++)
            {
                header.CreateCell(j).SetCellValue(headerField[j]);
                sheet.AutoSizeColumn(j);
            }
            lstData = repository.GetChargeParking(ExplodeParkingName);
            int len = lstData.Count;
            for (int k = 0; k < len; k++)
            {
                IRow content = sheet.CreateRow(k + 1);
                content.CreateCell(0).SetCellValue(lstData[k].Id);
                content.CreateCell(1).SetCellValue(lstData[k].ParkingName);
                content.CreateCell(2).SetCellValue(lstData[k].Operator);
                content.CreateCell(3).SetCellValue(lstData[k].ParkingAddress);
                content.CreateCell(4).SetCellValue(Decimal.ToDouble(lstData[k].Longitude));
                content.CreateCell(5).SetCellValue(Decimal.ToDouble(lstData[k].Latitude));
                content.CreateCell(6).SetCellValue(lstData[k].StartTime);
                content.CreateCell(7).SetCellValue(lstData[k].CloseTime);

            }
            for (int l = 0; l < headerFieldLen; l++)
            {
                sheet.AutoSizeColumn(l);
            }
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            // workbook.Close();
            return base.File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "停車便利付清單" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
        }
        #region 共用元件類型
        #endregion
    }
}