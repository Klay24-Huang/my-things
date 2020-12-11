﻿using Domain.TB.BackEnd;
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

namespace Web.Controllers
{
    public class ContactManageController : Controller
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        public ActionResult BookingQuery()
        {
            return View();
        }
        [HttpPost]
        public ActionResult BookingQuery(string OrderNo, string IDNO, string StationID, string CarNo, string StartDate, string EndDate, string Mode)
        {
            ViewData["errorLine"] = null;
            ViewData["IsShowMessage"] = null;
            ContactRepository repository = new ContactRepository(connetStr);
            ViewData["CarNo"] = CarNo;
            ViewData["StationID"] = StationID;
            ViewData["IDNO"] = IDNO;
            ViewData["Mode"] = Mode;
            ViewData["SDate"] = StartDate;
            ViewData["EDate"] = EndDate;
            ViewData["OrderNo"] = OrderNo;
            string errorLine = "";
            string errorMsg = "";
            bool flag = true;
            string errCode = "";
            Int64 tmpOrder = 0;

            if (StartDate != "" && EndDate == "")
            {

                StartDate = StartDate + " 00:00:00";
            }
            else if (StartDate == "" && EndDate != "")
            {
                EndDate = EndDate + " 23:59:59";
            }
            else if (StartDate != "" && EndDate != "")
            {
                StartDate = StartDate + " 00:00:00";
                EndDate = EndDate + " 23:59:59";
            }
            if (OrderNo != "")
            {
                if (OrderNo.IndexOf("H") < 0)
                {
                    flag = false;
                    errCode = "ERR900";
                    errorMsg = "訂單編號格式不符";
                }
                if (flag)
                {
                    flag = Int64.TryParse(OrderNo.Replace("H", ""), out tmpOrder);
                    if (flag)
                    {
                        if (tmpOrder <= 0)
                        {
                            flag = false;
                            errCode = "ERR900";
                            errorMsg = "訂單編號格式不符";
                        }

                    }
                }
            }
            List<BE_GetBookingQueryForWeb> lstData = null;
            if (flag)
            {
                ViewData["errorLine"] = "ok";
                lstData = repository.GetBookingQueryForWeb(tmpOrder, IDNO, StationID, CarNo, StartDate, EndDate);
            }
            else
            {
                ViewData["errorMsg"] = errorMsg;
                ViewData["errorLine"] = errCode.ToString();
            }

            return View(lstData);
        }
        [HttpPost]
        public ActionResult BookingQueryExplode(string ExplodeSDate, string ExplodeEDate, string ExplodeobjCar, string ExplodeuserID, string ExplodeOrderNum, string ExplodeobjStation)
        {
          
            List<BE_OrderDetailData> lstBook = new List<BE_OrderDetailData>();
            ContactRepository repository = new ContactRepository(connetStr);
            bool flag = true;
            ExplodeobjCar = (string.IsNullOrEmpty(ExplodeobjCar)) ? "" : ExplodeobjCar;
            ExplodeobjCar = ("-1" == ExplodeobjCar) ? "" : ExplodeobjCar;
            ExplodeSDate = (string.IsNullOrEmpty(ExplodeSDate) ? "" : ExplodeSDate);
            ExplodeEDate = (string.IsNullOrEmpty(ExplodeEDate) ? "" : ExplodeEDate);
            ExplodeOrderNum = string.IsNullOrEmpty(ExplodeOrderNum) ? "" : ExplodeOrderNum;
            string tmpOrder = ExplodeOrderNum.ToUpper();
            ExplodeuserID = (string.IsNullOrEmpty(ExplodeuserID) ? "" : ExplodeuserID);

            if (ExplodeSDate != "" && ExplodeEDate == "")
            {
                ExplodeSDate = ExplodeSDate + " 00:00:00";
            }
            else if (ExplodeSDate == "" && ExplodeEDate != "")
            {
                ExplodeEDate = ExplodeEDate + " 23:59:59";
            }
            else if (ExplodeSDate != "" && ExplodeEDate != "")
            {
                ExplodeSDate = ExplodeSDate + " 00:00:00";
                ExplodeEDate = ExplodeEDate + " 23:59:59";
            }
            ExplodeobjStation = (string.IsNullOrEmpty(ExplodeobjStation)) ? "" : ExplodeobjStation;
            string tmpStation = ExplodeobjStation;
            if (ExplodeobjStation != "" && ExplodeobjStation.ToLower() != "all")
            {
                int index = ExplodeobjStation.IndexOf('(');
                if (index > -1)
                {
                    index += 1;
                }
                if (index > -1)
                {
                    tmpStation = ExplodeobjStation.Substring(index);
                    tmpStation = tmpStation.Replace(")", "");
                }
            }

            if (ExplodeobjCar == "" && ExplodeOrderNum == "" && ExplodeuserID == "" && ExplodeSDate == "" && ExplodeEDate == "" && ExplodeobjStation == "")
            {
                flag = false;
            }
            if (flag)
            {
                if (tmpOrder != "")
                {

                    // OrderNum = OrderNum.ToUpper();
                    if (tmpOrder.Replace(" ", "").ToUpper().IndexOf('H') >= 0)
                    {
                        tmpOrder = Convert.ToInt64(tmpOrder.Replace("H", "")).ToString();
                    }
                    else
                    {
                        flag = false;
                    }

                }
                else
                {
                    tmpOrder = "0";
                }

            }
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("搜尋結果");
            string[] headerField = { "訂單編號", "會員帳號", "會員姓名",  "訂單類型", "取/還車站", "車型", "車牌號碼", "優惠方案", "實際取車時間", "實際還車時間"
                                    ,"取車左邊電池電量","取車右邊電池電量","取車核心電池電量","取車平均電量","還車左邊電池電量","還車右邊電池電量","還車核心電池電量","還車平均電量"
                                    ,"取車里程","還車里程","租金","罰金","油資","ETag費用","轉乘優惠","時數折抵(汽車)","時數折抵(機車)","結算金額"};
            int headerFieldLen = headerField.Length;

            IRow header = sheet.CreateRow(0);
            for (int j = 0; j < headerFieldLen; j++)
            {
                header.CreateCell(j).SetCellValue(headerField[j]);
                sheet.AutoSizeColumn(j);
            }
            if (flag)
            {

            
                lstBook = repository.GetOrderExplodeData(Convert.ToInt64(tmpOrder), ExplodeuserID, tmpStation, ExplodeobjCar, ExplodeSDate, ExplodeEDate,true);
                int BookCount = lstBook.Count();
                if (BookCount > 0)
                {
                   
                    int DataLen = lstBook.Count();
                    for (int i = 0; i < DataLen; i++)
                    {
                        string OrderStatus = "預約完成";
                        if (lstBook[i].CS > 0)
                        {
                            OrderStatus = "取消訂單";
                        }
                        else
                        {
                            if (lstBook[i].CMS >= 4 && lstBook[i].CMS < 15)
                            {
                                OrderStatus = "取車完成";
                            }
                            else if (lstBook[i].CMS == 15)
                            {
                                OrderStatus = "完成還車付款";
                            }

                        }
                        IRow content = sheet.CreateRow(i + 1);
                        content.CreateCell(0).SetCellValue("H" + lstBook[i].OrderNo.ToString().PadLeft(7, '0'));    //合約
                        content.CreateCell(1).SetCellValue(lstBook[i].IDNO);                                  //會員帳號
                        content.CreateCell(2).SetCellValue(lstBook[i].UserName);                                     //會員姓名
                   
                        content.CreateCell(3).SetCellValue(OrderStatus);                                                    //訂單類型
                        content.CreateCell(4).SetCellValue(lstBook[i].LStation + "/" + lstBook[i].RStation);                                     //取/還車站
                        content.CreateCell(5).SetCellValue(lstBook[i].CarTypeName);                                     //車型
                        content.CreateCell(6).SetCellValue(lstBook[i].CarNo);    //車牌號碼
                        content.CreateCell(7).SetCellValue(lstBook[i].PRONAME);                                   //優惠方案

                        content.CreateCell(8).SetCellValue((lstBook[i].FS.ToString("yyyy-MM-dd HH:mm:ss") == "1911-01-01 00:00:00") ? "未取車" : lstBook[i].FS.ToString("yyyy/MM/dd HH:mm"));   //實際取車時間
                        content.CreateCell(9).SetCellValue((lstBook[i].FE.ToString("yyyy-MM-dd HH:mm:ss") == "1911-01-01 00:00:00") ? "未還車" : lstBook[i].FE.ToString("yyyy/MM/dd HH:mm"));    //實際還車時間
                        content.CreateCell(10).SetCellValue((lstBook[i].P_LBA) < 0 ? "" : string.Format("{0}%", Convert.ToInt32(lstBook[i].P_LBA)));                                     //取車左邊電池電量
                        content.CreateCell(11).SetCellValue((lstBook[i].P_RBA) < 0 ? "" : string.Format("{0}%", Convert.ToInt32(lstBook[i].P_RBA)));                                     //取車右邊電池電量
                        content.CreateCell(12).SetCellValue((lstBook[i].P_MBA) < 0 ? "" : string.Format("{0}%", Convert.ToInt32(lstBook[i].P_MBA)));                                     //取車核心電池電量
                        content.CreateCell(13).SetCellValue((lstBook[i].P_TBA) < 0 ? "" : string.Format("{0}%", Convert.ToInt32(lstBook[i].P_TBA)));    //取車平均電量
                        content.CreateCell(14).SetCellValue((lstBook[i].R_LBA) < 0 ? "" : string.Format("{0}%", Convert.ToInt32(lstBook[i].R_LBA)));   //還車左邊電池電量
                        content.CreateCell(15).SetCellValue((lstBook[i].R_RBA) < 0 ? "" : string.Format("{0}%", Convert.ToInt32(lstBook[i].R_RBA)));   //還車右邊電池電量
                        content.CreateCell(16).SetCellValue((lstBook[i].R_MBA) < 0 ? "" : string.Format("{0}%", Convert.ToInt32(lstBook[i].R_MBA)));   //還車核心電池電量
                        content.CreateCell(17).SetCellValue((lstBook[i].R_TBA) < 0 ? "" : string.Format("{0}%", Convert.ToInt32(lstBook[i].R_TBA)));   //還車平均電量

                        content.CreateCell(18).SetCellValue((lstBook[i].StartMile < 0) ? "無資料" : lstBook[i].StartMile.ToString());                                     //取車里程
                        content.CreateCell(19).SetCellValue((lstBook[i].StopMile < 0) ? "無資料" : lstBook[i].StopMile.ToString());                                     //還車里程
                        content.CreateCell(20).SetCellValue((lstBook[i].PurePrice < 0) ? "" : lstBook[i].PurePrice.ToString());    //租金
                        content.CreateCell(21).SetCellValue((lstBook[i].FinePrice < 0) ? "" : lstBook[i].FinePrice.ToString());                                   //罰金
                        content.CreateCell(22).SetCellValue((lstBook[i].Mileage < 0) ? "" : lstBook[i].Mileage.ToString());                                     //油資
                        content.CreateCell(23).SetCellValue((lstBook[i].eTag < 0) ? "" : lstBook[i].eTag.ToString());    //ETag費用
                        content.CreateCell(24).SetCellValue((lstBook[i].TransDiscount > 0) ? "" : (-1 * lstBook[i].TransDiscount).ToString());    //轉乘優惠
                        content.CreateCell(25).SetCellValue(lstBook[i].CarPoint);                                     //時數折抵(分)
                        content.CreateCell(26).SetCellValue(lstBook[i].MotorPoint);                                     //時數折抵(分)
                        content.CreateCell(27).SetCellValue(lstBook[i].FinalPrice);                                     //會員姓名
                      
                    }
                }
            }



            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
           // workbook.Close();
            //   return View();
            return base.File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "預約匯出_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
        }
        /// <summary>
        /// 預約查詢
        /// </summary>
        /// <returns></returns>
        public ActionResult ContactQuery()
        {
            return View();
        }
        /// <summary>
        /// 合約查詢
        /// </summary>
        /// <param name="OrderNo"></param>
        /// <param name="IDNO"></param>
        /// <param name="StationID"></param>
        /// <param name="CarNo"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="Mode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ContactQuery(string OrderNo,string IDNO,string StationID,string CarNo,string StartDate,string EndDate,string Mode)
        {
            ViewData["errorLine"] = null;
            ViewData["IsShowMessage"] = null;
            ContactRepository repository = new ContactRepository(connetStr);
              ViewData["CarNo"] = CarNo;
              ViewData["StationID"] = StationID;
            ViewData["IDNO"] = IDNO;
            ViewData["Mode"] = Mode;
           ViewData["SDate"] = StartDate;
           ViewData["EDate"] = EndDate;
          ViewData["OrderNo"] = OrderNo;
            string errorLine = "";
            string errorMsg = "";
            bool flag = true;
            string errCode = "";
            Int64 tmpOrder = 0;

            if (StartDate != "" && EndDate == "")
            {
              
                StartDate = StartDate + " 00:00:00";
            }
            else if (StartDate == "" && EndDate != "")
            {
                EndDate = EndDate + ":59";
            }
            else if (StartDate != "" && EndDate != "")
            {
                StartDate = StartDate + " 00:00:00";
                EndDate = EndDate + ":59";
            }
            if (OrderNo != "")
            {
                if (OrderNo.IndexOf("H") < 0)
                {
                    flag = false;
                    errCode = "ERR900";
                    errorMsg = "訂單編號格式不符";
                }
                if (flag)
                {
                    flag = Int64.TryParse(OrderNo.Replace("H", ""), out tmpOrder);
                    if (flag)
                    {
                        if (tmpOrder <= 0)
                        {
                            flag = false;
                            errCode = "ERR900";
                            errorMsg = "訂單編號格式不符";
                        }

                    }
                }
            }
            List<BE_GetOrderQueryForWeb> lstData = null;
            if (flag)
            {
                ViewData["errorLine"] = "ok";
                lstData = repository.GetOrderQueryForWeb(tmpOrder, IDNO, StationID, CarNo, StartDate, EndDate);
            }
            else
            {
                ViewData["errorMsg"] = errorMsg;
                ViewData["errorLine"] = errCode.ToString();
            }
            
            return View(lstData);
        }
        /// <summary>
        /// 訂單記錄歷程查詢
        /// </summary>
        /// <returns></returns>
        public ActionResult ContactHistoryQuery(string OrderNo)
        {
            ViewData["OrderNo"] = OrderNo;
            if (string.IsNullOrWhiteSpace(OrderNo) == false)
            {
                ContactRepository repository = new ContactRepository(connetStr);
                List<BE_OrderHistoryData> lstData = new List<BE_OrderHistoryData>();
                lstData = repository.GetOrderHistory(Convert.ToInt64(OrderNo.Replace("H","")));
                return View(lstData);
            }
            else
            {
                return View();
            }
          
        }
        /// <summary>
        /// 合約匯出
        /// </summary>
        /// <param name="ExplodeSDate"></param>
        /// <param name="ExplodeEDate"></param>
        /// <param name="ExplodeobjCar"></param>
        /// <param name="ExplodeuserID"></param>
        /// <param name="ExplodeOrderNum"></param>
        /// <param name="ExplodeobjStation"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ContactQueryExplode(string ExplodeSDate, string ExplodeEDate, string ExplodeobjCar, string ExplodeuserID, string ExplodeOrderNum, string ExplodeobjStation)
        {

            List<BE_OrderDetailData> lstBook = new List<BE_OrderDetailData>();
            ContactRepository repository = new ContactRepository(connetStr);
            bool flag = true;
            ExplodeobjCar = (string.IsNullOrEmpty(ExplodeobjCar)) ? "" : ExplodeobjCar;
            ExplodeobjCar = ("-1" == ExplodeobjCar) ? "" : ExplodeobjCar;
            ExplodeSDate = (string.IsNullOrEmpty(ExplodeSDate) ? "" : ExplodeSDate);
            ExplodeEDate = (string.IsNullOrEmpty(ExplodeEDate) ? "" : ExplodeEDate);
            ExplodeOrderNum = string.IsNullOrEmpty(ExplodeOrderNum) ? "" : ExplodeOrderNum;
            string tmpOrder = ExplodeOrderNum.ToUpper();
            ExplodeuserID = (string.IsNullOrEmpty(ExplodeuserID) ? "" : ExplodeuserID);

            if (ExplodeSDate != "" && ExplodeEDate == "")
            {
                ExplodeSDate = ExplodeSDate + " 00:00:00";
            }
            else if (ExplodeSDate == "" && ExplodeEDate != "")
            {
                ExplodeEDate = ExplodeEDate + " 23:59:59";
            }
            else if (ExplodeSDate != "" && ExplodeEDate != "")
            {
                ExplodeSDate = ExplodeSDate + " 00:00:00";
                ExplodeEDate = ExplodeEDate + " 23:59:59";
            }
            ExplodeobjStation = (string.IsNullOrEmpty(ExplodeobjStation)) ? "" : ExplodeobjStation;
            string tmpStation = ExplodeobjStation;
            if (ExplodeobjStation != "" && ExplodeobjStation.ToLower() != "all")
            {
                int index = ExplodeobjStation.IndexOf('(');
                if (index > -1)
                {
                    index += 1;
                }
                if (index > -1)
                {
                    tmpStation = ExplodeobjStation.Substring(index);
                    tmpStation = tmpStation.Replace(")", "");
                }
            }

            if (ExplodeobjCar == "" && ExplodeOrderNum == "" && ExplodeuserID == "" && ExplodeSDate == "" && ExplodeEDate == "" && ExplodeobjStation == "")
            {
                flag = false;
            }
            if (flag)
            {
                if (tmpOrder != "")
                {

                    // OrderNum = OrderNum.ToUpper();
                    if (tmpOrder.Replace(" ", "").ToUpper().IndexOf('H') >= 0)
                    {
                        tmpOrder = Convert.ToInt64(tmpOrder.Replace("H", "")).ToString();
                    }
                    else
                    {
                        flag = false;
                    }

                }
                else
                {
                    tmpOrder = "0";
                }

            }
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("搜尋結果");
            string[] headerField = { "訂單編號", "會員帳號", "會員姓名",  "訂單類型", "取/還車站", "車型", "車牌號碼", "優惠方案", "實際取車時間", "實際還車時間"
                                    ,"取車左邊電池電量","取車右邊電池電量","取車核心電池電量","取車平均電量","還車左邊電池電量","還車右邊電池電量","還車核心電池電量","還車平均電量"
                                    ,"取車里程","還車里程","租金","罰金","油資","ETag費用","轉乘優惠","時數折抵(汽車)","時數折抵(機車)","結算金額"};
            int headerFieldLen = headerField.Length;

            IRow header = sheet.CreateRow(0);
            for (int j = 0; j < headerFieldLen; j++)
            {
                header.CreateCell(j).SetCellValue(headerField[j]);
                sheet.AutoSizeColumn(j);
            }
            if (flag)
            {


                lstBook = repository.GetOrderExplodeData(Convert.ToInt64(tmpOrder), ExplodeuserID, tmpStation, ExplodeobjCar, ExplodeSDate, ExplodeEDate, false);
                int BookCount = lstBook.Count();
                if (BookCount > 0)
                {

                    int DataLen = lstBook.Count();
                    for (int i = 0; i < DataLen; i++)
                    {
                        string OrderStatus = "預約完成";
                        if (lstBook[i].CS > 0)
                        {
                            OrderStatus = "取消訂單";
                        }
                        else
                        {
                            if (lstBook[i].CMS >= 4 && lstBook[i].CMS < 15)
                            {
                                OrderStatus = "取車完成";
                            }
                            else if (lstBook[i].CMS == 15)
                            {
                                OrderStatus = "完成還車付款";
                            }

                        }
                        IRow content = sheet.CreateRow(i + 1);
                        content.CreateCell(0).SetCellValue("H" + lstBook[i].OrderNo.ToString().PadLeft(7, '0'));    //合約
                        content.CreateCell(1).SetCellValue(lstBook[i].IDNO);                                  //會員帳號
                        content.CreateCell(2).SetCellValue(lstBook[i].UserName);                                     //會員姓名

                        content.CreateCell(3).SetCellValue(OrderStatus);                                                    //訂單類型
                        content.CreateCell(4).SetCellValue(lstBook[i].LStation + "/" + lstBook[i].RStation);                                     //取/還車站
                        content.CreateCell(5).SetCellValue(lstBook[i].CarTypeName);                                     //車型
                        content.CreateCell(6).SetCellValue(lstBook[i].CarNo);    //車牌號碼
                        content.CreateCell(7).SetCellValue(lstBook[i].PRONAME);                                   //優惠方案

                        content.CreateCell(8).SetCellValue((lstBook[i].FS.ToString("yyyy-MM-dd HH:mm:ss") == "1911-01-01 00:00:00") ? "未取車" : lstBook[i].FS.ToString("yyyy/MM/dd HH:mm"));   //實際取車時間
                        content.CreateCell(9).SetCellValue((lstBook[i].FE.ToString("yyyy-MM-dd HH:mm:ss") == "1911-01-01 00:00:00") ? "未還車" : lstBook[i].FE.ToString("yyyy/MM/dd HH:mm"));    //實際還車時間
                        content.CreateCell(10).SetCellValue((lstBook[i].P_LBA) < 0 ? "" : string.Format("{0}%", Convert.ToInt32(lstBook[i].P_LBA)));                                     //取車左邊電池電量
                        content.CreateCell(11).SetCellValue((lstBook[i].P_RBA) < 0 ? "" : string.Format("{0}%", Convert.ToInt32(lstBook[i].P_RBA)));                                     //取車右邊電池電量
                        content.CreateCell(12).SetCellValue((lstBook[i].P_MBA) < 0 ? "" : string.Format("{0}%", Convert.ToInt32(lstBook[i].P_MBA)));                                     //取車核心電池電量
                        content.CreateCell(13).SetCellValue((lstBook[i].P_TBA) < 0 ? "" : string.Format("{0}%", Convert.ToInt32(lstBook[i].P_TBA)));    //取車平均電量
                        content.CreateCell(14).SetCellValue((lstBook[i].R_LBA) < 0 ? "" : string.Format("{0}%", Convert.ToInt32(lstBook[i].R_LBA)));   //還車左邊電池電量
                        content.CreateCell(15).SetCellValue((lstBook[i].R_RBA) < 0 ? "" : string.Format("{0}%", Convert.ToInt32(lstBook[i].R_RBA)));   //還車右邊電池電量
                        content.CreateCell(16).SetCellValue((lstBook[i].R_MBA) < 0 ? "" : string.Format("{0}%", Convert.ToInt32(lstBook[i].R_MBA)));   //還車核心電池電量
                        content.CreateCell(17).SetCellValue((lstBook[i].R_TBA) < 0 ? "" : string.Format("{0}%", Convert.ToInt32(lstBook[i].R_TBA)));   //還車平均電量

                        content.CreateCell(18).SetCellValue((lstBook[i].StartMile < 0) ? "無資料" : lstBook[i].StartMile.ToString());                                     //取車里程
                        content.CreateCell(19).SetCellValue((lstBook[i].StopMile < 0) ? "無資料" : lstBook[i].StopMile.ToString());                                     //還車里程
                        content.CreateCell(20).SetCellValue((lstBook[i].PurePrice < 0) ? "" : lstBook[i].PurePrice.ToString());    //租金
                        content.CreateCell(21).SetCellValue((lstBook[i].FinePrice < 0) ? "" : lstBook[i].FinePrice.ToString());                                   //罰金
                        content.CreateCell(22).SetCellValue((lstBook[i].Mileage < 0) ? "" : lstBook[i].Mileage.ToString());                                     //油資
                        content.CreateCell(23).SetCellValue((lstBook[i].eTag < 0) ? "" : lstBook[i].eTag.ToString());    //ETag費用
                        content.CreateCell(24).SetCellValue((lstBook[i].TransDiscount > 0) ? "" : (-1 * lstBook[i].TransDiscount).ToString());    //轉乘優惠
                        content.CreateCell(25).SetCellValue(lstBook[i].CarPoint);                                     //時數折抵(分)
                        content.CreateCell(26).SetCellValue(lstBook[i].MotorPoint);                                     //時數折抵(分)
                        content.CreateCell(27).SetCellValue(lstBook[i].FinalPrice);                                     //會員姓名

                    }
                }
            }



            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            // workbook.Close();
            //   return View();
            return base.File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "預約匯出_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
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
        /// <summary>
        /// 訂單（合約）明細
        /// </summary>
        /// <param name="OrderNo"></param>
        /// <returns></returns>
        public ActionResult ContactDetail(string DetailOrderNo)
        {
            BE_OrderDataCombind obj = null;
            ContactRepository repository = new ContactRepository(connetStr);
            Int64 tmpOrder = 0;
            bool flag = true;
            if (string.IsNullOrEmpty(DetailOrderNo))
            {
                flag = false;

            }
            else
            {
                if (DetailOrderNo != "")
                {

                    tmpOrder = Convert.ToInt64(DetailOrderNo.Replace("H", ""));

                    //lstBook = _repository.GetBookingDetailNew(OrderNO);
                    //  lstNewBooking = _repository.GetBookingDetailHasImgNew(OrderNO);
                    obj = new BE_OrderDataCombind()
                    {
                        Data = repository.GetOrderDetail(tmpOrder),
                        PickCarImage = repository.GetOrdeCarImage(tmpOrder, 0,false),
                        ReturnCarImage = repository.GetOrdeCarImage(tmpOrder, 1,false),
                        ParkingCarImage = repository.GetOrderParkingImage(tmpOrder)
                    };

                }
                else
                {
                    flag = false;
                }
            }
            return View(obj);
    
        }
        /// <summary>
        /// 訂單（合約）明細（機車）
        /// </summary>
        /// <param name="OrderNo"></param>
        /// <returns></returns>

        public ActionResult ContactMotorDetail(string DetailOrderNo)
        {
            BE_OrderDataCombind obj = null;
            ContactRepository repository = new ContactRepository(connetStr);
            Int64 tmpOrder = 0;
            bool flag = true;
            if (string.IsNullOrEmpty(DetailOrderNo))
            {
                flag = false;

            }
            else
            {
                if (DetailOrderNo != "")
                {

                    tmpOrder = Convert.ToInt64(DetailOrderNo.Replace("H", ""));

                    //lstBook = _repository.GetBookingDetailNew(OrderNO);
                    //  lstNewBooking = _repository.GetBookingDetailHasImgNew(OrderNO);
                    obj = new BE_OrderDataCombind()
                    {
                        Data = repository.GetOrderDetail(tmpOrder),
                        PickCarImage = repository.GetOrdeCarImage(tmpOrder, 0,false),
                        ReturnCarImage = repository.GetOrdeCarImage(tmpOrder, 1,false),
                        ParkingCarImage = repository.GetOrderParkingImage(tmpOrder)
                    };

                }
                else
                {
                    flag = false;
                }
            }
            return View(obj);
        }
    }
}