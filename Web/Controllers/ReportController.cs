﻿using Domain.TB.BackEnd;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Data;
using WebCommon;
using NLog;//增加NLOG機制

namespace Web.Controllers
{
    /// <summary>
    /// 報表
    /// </summary>
    public class ReportController : BaseSafeController //20210902唐改繼承BaseSafeController，寫nlog //Controller
    {
        //增加NLOG機制
        //protected static Logger logger = LogManager.GetCurrentClassLogger();
        //private string connetStr = ConfigurationManager.ConnectionStrings["IRentMirror"].ConnectionString;

        #region 整備人員報表查詢
        /// <summary>
        /// 整備人員報表查詢
        /// </summary>
        /// <returns></returns>
        public ActionResult MaintainLogReport()
        {
            return View();
        }
        [HttpPost]
        public ActionResult MaintainLogReport(string SDate, string EDate, string carid, string objStation, string userID, int? status)
        {
            BaseSafeController himsSafe = new BaseSafeController();
            himsSafe.nnlog(Session["User"], Session["Account"], System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                , "MaintainLogReport");

            List<BE_CleanData> lstData = new List<BE_CleanData>();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            DateTime SD, ED;
            if (!string.IsNullOrEmpty(SDate) && !string.IsNullOrEmpty(EDate))
            {

                SD = DateTime.Parse(SDate + " 00:00:00");
                ED = DateTime.Parse(EDate + " 23:59:59");
                if (SD.Subtract(ED).TotalMilliseconds > 0)
                {
                    ViewData["SDate"] = EDate;
                    ViewData["EDate"] = SDate;
                }
                else
                {
                    ViewData["SDate"] = SDate;
                    ViewData["EDate"] = EDate;
                }
            }
            else if (string.IsNullOrEmpty(SDate) && string.IsNullOrEmpty(EDate))
            {
                SDate = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd 00:00:00");
                EDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 23:59:59");
                ViewData["SDate"] = SDate.Split(' ')[0];
                ViewData["EDate"] = EDate.Split(' ')[0];
            }
            else
            {
                if (!string.IsNullOrEmpty(SDate))
                {
                    ViewData["SDate"] = SDate;
                }
                if (!string.IsNullOrEmpty(EDate))
                {
                    ViewData["EDate"] = EDate;
                }
            }
            if (!string.IsNullOrEmpty(carid))
            {
                ViewData["CarNo"] = carid;
            }
            if (!string.IsNullOrEmpty(objStation))
            {
                ViewData["objStation"] = objStation;
            }
            if (!string.IsNullOrEmpty(userID))
            {
                ViewData["userID"] = userID;
            }
            if (status.HasValue)
            {
                if (status < 3)
                {
                    ViewData["status"] = status;
                }
            }
            lstData = new CarClearRepository(connetStr).GetCleanData(SDate, EDate, carid, objStation, userID, (status.HasValue) ? status.Value : 3, ref lstError);
            var topFiveHundred = lstData.Take(500).ToList();
            return View(topFiveHundred);
        }

        public ActionResult MaintainLogReportDownload(string SDate, string EDate, string carid, string objStation, string userID, int? status)
        {
            BaseSafeController himsSafe = new BaseSafeController();
            himsSafe.nnlog(Session["User"], Session["Account"], System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                , "MaintainLogReportDownload");

            List<BE_CleanDataWithoutPIC> data = new List<BE_CleanDataWithoutPIC>();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            data = new CarClearRepository(connetStr).GetCleanDataWithOutPic(SDate, EDate, carid, objStation, userID, (status.HasValue) ? status.Value : 3, ref lstError);
            //增加NLOG機制
            logger.Trace(
                    "{ReportName:'整備人員報表查詢(MaintainLogReportDownload)'," +
                    "User" + ":'" + Session["User"] + "'," +
                    "IPAddr" + ":'" + System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] + "'," +
                    "Condition:{SDate" + ":'" + SDate + "'," +
                    "EDate" + ":'" + EDate + "'," +
                    "carid" + ":'" + carid + "'," +
                    "objStation" + ":'" + objStation + "'," +
                    "userID" + ":'" + userID + "'," +
                    "status" + ":'" + ((status.HasValue) ? status.Value : '無') + "'}," +
                    "RowCount" + ":" + data.Count.ToString() + "}"
                    );

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("搜尋結果");
            string[] headerField = { "帳號", "整備人員", "訂單編號", "車號", "據點", "狀態", "實際取車", "實際還車", "車外清潔", "車內清潔", "車輛救援", "車輛調度", "車輛調度(路邊租還)", "保養", "清潔時幾天未清", "出租次數", "備註" };
            int headerFieldLen = headerField.Length;

            IRow header = sheet.CreateRow(0);
            for (int j = 0; j < headerFieldLen; j++)
            {
                header.CreateCell(j).SetCellValue(headerField[j]);
                //sheet.AutoSizeColumn(j);
            }

            int len = data.Count;
            for (int k = 0; k < len; k++)
            {
                string OrderStatus = "已預約";
                if (data[k].OrderStatus == 1)
                {
                    OrderStatus = "已取車";
                }
                else if (data[k].OrderStatus == 2)
                {
                    OrderStatus = "已還車";
                }
                else if (data[k].OrderStatus == 3)
                {
                    OrderStatus = "已取消";
                }
                else if (data[k].OrderStatus == 4)
                {
                    OrderStatus = "逾時未取車(排程取消)";
                }
                else if (data[k].OrderStatus == 5)
                {
                    OrderStatus = "逾時未還車(排程取消)";
                }

                //double totalDay = ((data[k].lastCleanTime.ToString("yyyy-MM-dd HH:mm:ss") == "1900-01-01 00:00:00")) ? data[k].BookingStart.Subtract(data[k].lastCleanTime).TotalDays : -1;
                //string totalDayStr = (totalDay == -1) ? "從未清潔" : ((totalDay < 1) ? Math.Round(data[k].BookingStart.Subtract(data[k].lastCleanTime).TotalHours, MidpointRounding.AwayFromZero) + "小時" : Math.Round(totalDay).ToString());
                double totalDay = ((data[k].lastCleanTime.ToString("yyyy-MM-dd HH:mm:ss") == "1900-01-01 00:00:00") ? -1 : data[k].BookingStart.Subtract(data[k].lastCleanTime).TotalDays);
                string totalDayStr = (totalDay == -1) ? "從未清潔" : ((totalDay < 1) ? Math.Round(data[k].BookingStart.Subtract(data[k].lastCleanTime).TotalHours, MidpointRounding.AwayFromZero) + "小時" : Math.Round(totalDay).ToString());
                if (data[k].OrderStatus < 2)
                {
                    //totalDayStr = DateTime.Now.Date.Subtract(Convert.ToDateTime(data[k].lastCleanTime).Date).TotalDays.ToString();
                    totalDayStr = "";
                }
                IRow content = sheet.CreateRow(k + 1);
                content.CreateCell(0).SetCellValue(data[k].Account);
                content.CreateCell(1).SetCellValue(data[k].UserID);
                content.CreateCell(2).SetCellValue("H" + data[k].OrderNum.ToString().PadLeft(7, '0'));//合約
                content.CreateCell(3).SetCellValue(data[k].CarNo);//車號
                content.CreateCell(4).SetCellValue(data[k].lend_place);//據點
                content.CreateCell(5).SetCellValue(OrderStatus);//狀態
                if (data[k].OrderStatus < 1 || data[k].OrderStatus == 4)
                {
                    content.CreateCell(6).SetCellValue("未取車");//實際取車
                }
                else
                {
                    content.CreateCell(6).SetCellValue(data[k].BookingStart.ToString("yyyy-MM-dd HH:mm:ss").Replace("1900-01-01 00:00:00", "未取車"));  //實際取車
                }

                if (data[k].OrderStatus < 1 || data[k].OrderStatus == 4)
                {
                    content.CreateCell(7).SetCellValue("未取車");//實際還車
                }
                else if (data[k].OrderStatus == 1)
                {
                    content.CreateCell(7).SetCellValue("未還車");//實際還車
                }
                else if (data[k].OrderStatus == 5)
                {
                    if (data[k].BookingEnd.ToString("yyyy-MM-dd HH:mm:ss") != "1900-01-01 00:00:00" && data[k].BookingStart.ToString("yyyy-MM-dd HH:mm:ss") != "1900-01-01 00:00:00")
                    {
                        if (data[k].BookingEnd < data[k].BookingStart)
                        {
                            content.CreateCell(7).SetCellValue("逾時未還車【系統強還時間：" + data[k].BookingEnd.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss") + "】");//實際還車
                        }
                        else
                        {
                            content.CreateCell(7).SetCellValue("逾時未還車【系統強還時間：" + data[k].BookingEnd.ToString("yyyy-MM-dd HH:mm:ss") + "】");//實際還車
                        }
                    }


                }
                else if (data[k].OrderStatus == 2)
                {
                    if (data[k].BookingEnd < data[k].BookingStart)
                    {
                        data[k].BookingEnd = data[k].BookingEnd.AddHours(8);
                    }
                    content.CreateCell(7).SetCellValue(data[k].BookingEnd.ToString("yyyy-MM-dd HH:mm:ss").Replace("1900-01-01 00:00:00", "未還車"));
                }
                else
                {
                    content.CreateCell(7).SetCellValue(data[k].BookingEnd.ToString("yyyy-MM-dd HH:mm:ss").Replace("1900-01-01 00:00:00", "未還車"));//實際還車
                }
                content.CreateCell(8).SetCellValue((data[k].outsideClean == 1) ? "✔" : "✖");//車外清潔
                content.CreateCell(9).SetCellValue((data[k].insideClean == 1) ? "✔" : "✖");//車內清潔
                content.CreateCell(10).SetCellValue((data[k].rescue == 1) ? "✔" : "✖");//車輛救援
                content.CreateCell(11).SetCellValue((data[k].dispatch == 1) ? "✔" : "✖");//車輛調度
                content.CreateCell(12).SetCellValue((data[k].Anydispatch == 1) ? "✔" : "✖");//車輛調度(路邊租還)
                content.CreateCell(13).SetCellValue((data[k].Maintenance == 1) ? "✔" : "✖");
                content.CreateCell(14).SetCellValue(totalDayStr);
                content.CreateCell(15).SetCellValue(data[k].lastRentTimes);
                content.CreateCell(16).SetCellValue(data[k].remark);//備註
            }

            // 自動調整欄位大小，但這很耗資源
            //for (int l = 0; l < headerFieldLen; l++)
            //{
            //    sheet.AutoSizeColumn(l);
            //}

            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);

            return base.File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "整備人員查詢結果_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
        }
        #endregion

        #region 車況回饋查詢
        /// <summary>
        /// 車況回饋查詢
        /// </summary>
        /// <returns></returns>
        public ActionResult CarFeedBackQuery()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CarFeedBackQuery(string SDate, string EDate, string userID, string carid, string objStation, int? isHandle, int? NowPage)
        {
            BaseSafeController himsSafe = new BaseSafeController();
            himsSafe.nnlog(Session["User"], Session["Account"], System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                , "CarFeedBackQuery");

            List<BE_FeedBackMainDetail> lstFeedBack = new List<BE_FeedBackMainDetail>();
            FeedBackRepository _repository = new FeedBackRepository(connetStr);
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            int tmpIsHandle = 2;
            string tSDate = "", tEDate = "", tUserID = "", tCarID = "", tStation = "";
            if (isHandle.HasValue)
            {
                tmpIsHandle = isHandle.Value;
                ViewData["isHandle"] = tmpIsHandle;
            }
            if (!string.IsNullOrEmpty(SDate))
            {
                tSDate = SDate;
                ViewData["SDate"] = tSDate;
            }
            if (!string.IsNullOrEmpty(EDate))
            {
                tEDate = EDate;
                ViewData["EDate"] = tEDate;
            }
            if (!string.IsNullOrEmpty(userID))
            {
                tUserID = userID;
                ViewData["userID"] = tUserID;
            }
            if (!string.IsNullOrEmpty(carid))
            {

                tCarID = carid.ToUpper();
                ViewData["carid"] = tCarID;

            }
            if (!string.IsNullOrEmpty(objStation))
            {
                int index = objStation.IndexOf('(');
                if (index > -1)
                {
                    index += 1;
                }
                if (index > -1)
                {
                    tStation = objStation.Substring(index);
                    tStation = tStation.Replace(")", "");
                }

                tStation = objStation.ToUpper();

                ViewData["objStation"] = objStation;


            }
            if (tmpIsHandle < 2 || tUserID != "" || tSDate != "" || tEDate != "")
            {
                if (tCarID == "ALL")
                {
                    tCarID = "";
                }
                if (tStation == "ALL")
                {
                    tStation = "";
                }
                lstFeedBack = _repository.GetCarFeedBackQuery(userID, tSDate, tEDate, tmpIsHandle, tCarID, tStation);
            }
            return View(lstFeedBack);
        }
        public ActionResult FeedBackDownload(string SDate, string EDate, string userID, string carid, string objStation, int? isHandle)
        {
            BaseSafeController himsSafe = new BaseSafeController();
            himsSafe.nnlog(Session["User"], Session["Account"], System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                , "FeedBackDownload");

            List<BE_FeedBackMainDetail> lstFeedBack = new List<BE_FeedBackMainDetail>();
            FeedBackRepository _repository = new FeedBackRepository(connetStr);
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            int tmpIsHandle = 2;
            string tSDate = "", tEDate = "", tUserID = "", tCarID = "", tStation = "";
            if (isHandle.HasValue)
            {
                tmpIsHandle = isHandle.Value;
                ViewData["isHandle"] = tmpIsHandle;
            }
            if (!string.IsNullOrEmpty(SDate))
            {
                tSDate = SDate;
                ViewData["SDate"] = tSDate;
            }
            if (!string.IsNullOrEmpty(EDate))
            {
                tEDate = EDate;
                ViewData["EDate"] = tEDate;
            }
            if (!string.IsNullOrEmpty(userID))
            {
                tUserID = userID;
                ViewData["userID"] = tUserID;
            }
            if (!string.IsNullOrEmpty(carid))
            {

                tCarID = carid.ToUpper();
                ViewData["carid"] = tCarID;

            }
            if (!string.IsNullOrEmpty(objStation))
            {
                int index = objStation.IndexOf('(');
                if (index > -1)
                {
                    index += 1;
                }
                if (index > -1)
                {
                    tStation = objStation.Substring(index);
                    tStation = tStation.Replace(")", "");
                }

                tStation = objStation.ToUpper();

                ViewData["objStation"] = objStation;


            }
            if (tmpIsHandle < 2 || tUserID != "" || tSDate != "" || tEDate != "")
            {
                if (tCarID == "ALL")
                {
                    tCarID = "";
                }
                if (tStation == "ALL")
                {
                    tStation = "";
                }
                lstFeedBack = _repository.GetCarFeedBackQuery(userID, tSDate, tEDate, tmpIsHandle, tCarID, tStation);
            }
            //增加NLOG機制
            logger.Trace(
                    "{ReportName:'車況回饋查詢(FeedBackDownload)'," +
                    "User" + ":'" + Session["User"] + "'," +
                    "IPAddr" + ":'" + System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] + "'," +
                    "Condition:{SDate" + ":'" + SDate + "'," +
                    "EDate" + ":'" + EDate + "'," +
                    "carid" + ":'" + carid + "'," +
                    "objStation" + ":'" + objStation + "'," +
                    "userID" + ":'" + userID + "'," +
                    "isHandle" + ":'" + ((isHandle.HasValue) ? isHandle.Value : '無') + "'}," +
                    "RowCount" + ":" + lstFeedBack.Count.ToString() + "}"
                    );

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("搜尋結果");
            string[] headerField = { "回饋日期", "回饋狀態", "合約NO.", "車號", "ID", "姓名", "手機", "內容", "狀態", "處理結果", "處理者" };
            int headerFieldLen = headerField.Length;

            IRow header = sheet.CreateRow(0);
            for (int j = 0; j < headerFieldLen; j++)
            {
                header.CreateCell(j).SetCellValue(headerField[j]);
                //sheet.AutoSizeColumn(j);
            }

            int len = lstFeedBack.Count;
            for (int k = 0; k < len; k++)
            {
                IRow content = sheet.CreateRow(k + 1);
                content.CreateCell(0).SetCellValue(lstFeedBack[k].MKTime.ToString("yyyy-MM-dd HH:mm:ss"));  //回饋日期
                content.CreateCell(1).SetCellValue((lstFeedBack[k].mode == 1) ? "還車" : "取車");         //處理狀態
                content.CreateCell(2).SetCellValue("H" + lstFeedBack[k].OrderNo.ToString().PadLeft(7, '0'));   //合約
                content.CreateCell(3).SetCellValue(lstFeedBack[k].CarNo);   //車號
                content.CreateCell(4).SetCellValue(lstFeedBack[k].IDNO);   //ID
                content.CreateCell(5).SetCellValue(lstFeedBack[k].MEMCNAME);   //姓名
                content.CreateCell(6).SetCellValue(lstFeedBack[k].MEMTEL);   //手機
                content.CreateCell(7).SetCellValue(lstFeedBack[k].descript);   //內容
                content.CreateCell(8).SetCellValue((lstFeedBack[k].isHandle == 0) ? "未處理" : "已處理");   //處理狀態
                content.CreateCell(9).SetCellValue(lstFeedBack[k].handleDescript);   //處理結果
                content.CreateCell(10).SetCellValue(lstFeedBack[k].opt);   //處理者
            }

            //for (int l = 0; l < headerFieldLen; l++)
            //{
            //    sheet.AutoSizeColumn(l);
            //}

            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            // workbook.Close();
            return base.File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "車況回饋_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
        }
        #endregion

        #region 綠界交易記錄查詢
        /// <summary>
        /// 綠界交易記錄查詢
        /// </summary>
        /// <returns></returns>
        public ActionResult TradeQuery()
        {
            BaseSafeController himsSafe = new BaseSafeController();
            himsSafe.nnlog(Session["User"], Session["Account"], System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                , "TradeQuery");

            return View();
        }
        #endregion

        #region 月租總表
        /// <summary>
        /// 月租總表
        /// </summary>
        /// <returns></returns>
        public ActionResult MonthlyMainQuery()
        {
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SDate">查詢日期起</param>
        /// <param name="EDate">查詢日期迄</param>
        /// <param name="userID">查詢帳號</param>
        /// <param name="isHandle">模式
        /// <para>0:無點數</para>
        /// <para>1:有點數</para>
        /// <para>2:不限</para>
        /// </param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MonthlyMainQuery(string SDate, string EDate, string userID, int? isHandle)
        {
            BaseSafeController himsSafe = new BaseSafeController();
            himsSafe.nnlog(Session["User"], Session["Account"], System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                , "MonthlyMainQuery");

            List<BE_MonthlyMain> lstSubScription = new List<BE_MonthlyMain>();
            SubScriptionRepository _repository = new SubScriptionRepository(connetStr);
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            int tmpIsHandle = 2;
            string tSDate = "", tEDate = "", tUserID = "";

            if (isHandle.HasValue)
            {
                tmpIsHandle = isHandle.Value;
                ViewData["isHandle"] = tmpIsHandle;
            }
            if (!string.IsNullOrEmpty(SDate))
            {
                tSDate = SDate;
                ViewData["SDate"] = tSDate;
            }
            if (!string.IsNullOrEmpty(EDate))
            {
                tEDate = EDate;
                ViewData["EDate"] = tEDate;
            }
            if (!string.IsNullOrEmpty(userID))
            {
                tUserID = userID;
                ViewData["userID"] = tUserID;
            }

            bool isInDateRange = false;

            if (DateTime.TryParse(tSDate, out DateTime DS) && DateTime.TryParse(tEDate, out DateTime DE))
            {
                if (DE >= DS && DE <= DS.AddMonths(1))
                {
                    isInDateRange = true;
                    ViewData["outerOfDateRangeMsg"] = "";
                }
                else
                {
                    ViewData["outerOfDateRangeMsg"] = "查詢起迄日超過範圍";
                }
            }

            if (isInDateRange || tmpIsHandle < 2 || tUserID.Length > 0)
            {
                lstSubScription = _repository.BE_GetMonthlyMain(userID, tSDate, tEDate, tmpIsHandle);
            }



            return View(lstSubScription);
        }
        /// <summary>
        /// 月租總表下載
        /// </summary>
        /// <param name="SDate"></param>
        /// <param name="EDate"></param>
        /// <param name="userID"></param>
        /// <param name="isHandle"></param>
        /// <returns></returns>    
        public ActionResult MonthlyMainQueryDownLoad(string SDate, string EDate, string userID, int? isHandle)
        {
            BaseSafeController himsSafe = new BaseSafeController();
            himsSafe.nnlog(Session["User"], Session["Account"], System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                , "MonthlyMainQueryDownLoad");

            List<BE_MonthlyMain> lstSubScription = new List<BE_MonthlyMain>();
            SubScriptionRepository _repository = new SubScriptionRepository(connetStr);
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            int tmpIsHandle = 2;
            string tSDate = "", tEDate = "", tUserID = "";
            if (isHandle.HasValue)
            {
                tmpIsHandle = isHandle.Value;
                ViewData["isHandle"] = tmpIsHandle;
            }
            if (!string.IsNullOrEmpty(SDate))
            {
                tSDate = SDate;
                ViewData["SDate"] = tSDate;
            }
            if (!string.IsNullOrEmpty(EDate))
            {
                tEDate = EDate;
                ViewData["EDate"] = tEDate;
            }
            if (!string.IsNullOrEmpty(userID))
            {
                tUserID = userID;
                ViewData["userID"] = tUserID;
            }
            bool isInDateRange = false;

            if (DateTime.TryParse(tSDate, out DateTime DS) && DateTime.TryParse(tEDate, out DateTime DE))
            {
                if (DE >= DS && DE <= DS.AddMonths(1))
                {
                    isInDateRange = true;
                }
            }

            if (isInDateRange || tmpIsHandle < 2 || tUserID.Length > 0)
            {
                lstSubScription = _repository.BE_GetMonthlyMain(userID, tSDate, tEDate, tmpIsHandle);
            }
            //增加NLOG機制
            logger.Trace(
                    "{ReportName:'月租總表下載(MonthlyMainQueryDownLoad)'," +
                    "User" + ":'" + Session["User"] + "'," +
                    "IPAddr" + ":'" + System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] + "'," +
                    "Condition:{SDate" + ":'" + SDate + "'," +
                    "EDate" + ":'" + EDate + "'," +
                    "userID" + ":'" + userID + "'," +
                    "isHandle" + ":'" + ((isHandle.HasValue) ? isHandle.Value : '無') + "'}," +
                    "RowCount" + ":" + lstSubScription.Count.ToString() + "}"
                    );

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("搜尋結果");
            string[] headerField = { "訂閱方案編號", "方案代碼", "方案名稱", "方案生效時間", "方案結束時間",
                "IDNO", "汽車－平日", "汽車－假日", "機車","是否開啟自動續約" ,"方案是否綁約","綁約期數" };
            int headerFieldLen = headerField.Length;

            IRow header = sheet.CreateRow(0);
            for (int j = 0; j < headerFieldLen; j++)
            {
                header.CreateCell(j).SetCellValue(headerField[j]);
                //sheet.AutoSizeColumn(j);
            }

            int len = lstSubScription.Count;
            for (int k = 0; k < len; k++)
            {
                IRow content = sheet.CreateRow(k + 1);
                content.CreateCell(0).SetCellValue(lstSubScription[k].SEQNO);   //ID
                content.CreateCell(1).SetCellValue(lstSubScription[k].ProjID);   //ID
                content.CreateCell(2).SetCellValue(lstSubScription[k].ProjNM);   //ID
                content.CreateCell(3).SetCellValue(lstSubScription[k].StartDate.ToString("yyyy-MM-dd HH:mm"));  //合約起
                content.CreateCell(4).SetCellValue(lstSubScription[k].EndDate.ToString("yyyy-MM-dd HH:mm"));    //合約迄
                content.CreateCell(5).SetCellValue(lstSubScription[k].IDNO);   //ID
                content.CreateCell(6).SetCellValue(lstSubScription[k].WorkDayHours);   //汽車－平日
                content.CreateCell(7).SetCellValue(lstSubScription[k].HolidayHours);   //汽車－假日
                content.CreateCell(8).SetCellValue((lstSubScription[k].MotoTotalHours).ToString("f1"));   //機車
                content.CreateCell(9).SetCellValue(lstSubScription[k].AutomaticRenewal);
                content.CreateCell(10).SetCellValue(lstSubScription[k].IsTiedUp);
                content.CreateCell(11).SetCellValue(lstSubScription[k].MonProPeriod);
            }

            //for (int l = 0; l < headerFieldLen; l++)
            //{
            //    sheet.AutoSizeColumn(l);
            //}

            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            //workbook.Close();
            return base.File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "月租訂閱總表_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
        }
        #endregion

        #region 月租報表
        /// <summary>
        /// 月租報表
        /// </summary>
        /// <returns></returns>   
        public ActionResult MonthlyDetailQuery()
        {
            return View();
        }
        [HttpPost]
        public ActionResult MonthlyDetailQuery(string SDate, string EDate, string userID, string OrderNum)
        {
            BaseSafeController himsSafe = new BaseSafeController();
            himsSafe.nnlog(Session["User"], Session["Account"], System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                , "MonthlyDetailQuery");

            List<BE_MonthlyDetail> lstSubScription = new List<BE_MonthlyDetail>();
            SubScriptionRepository _repository = new SubScriptionRepository(connetStr);
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            string tSDate = "", tEDate = "", tUserID = "", tOrderNum = "";


            if (!string.IsNullOrEmpty(SDate))
            {
                tSDate = SDate;
                ViewData["SDate"] = tSDate;
            }
            if (!string.IsNullOrEmpty(EDate))
            {
                tEDate = EDate;
                ViewData["EDate"] = tEDate;
            }
            if (!string.IsNullOrEmpty(userID))
            {
                tUserID = userID;
                ViewData["userID"] = tUserID;
            }
            if (!string.IsNullOrEmpty(OrderNum))
            {
                tOrderNum = OrderNum;
                ViewData["OrderNum"] = tOrderNum;
                tOrderNum = tOrderNum.Replace("H", "");
            }
            bool isInDateRange = false;

            if (DateTime.TryParse(tSDate, out DateTime DS) && DateTime.TryParse(tEDate, out DateTime DE))
            {
                if (DE >= DS && DE <= DS.AddMonths(1))
                {
                    isInDateRange = true;
                    ViewData["outerOfDateRangeMsg"] = "";
                }
                else
                {
                    ViewData["outerOfDateRangeMsg"] = "查詢時數使用起迄日超過範圍";
                }
            }
            if (isInDateRange || tOrderNum.Length > 0 || tUserID.Length > 0)
            {
                lstSubScription = _repository.GetMonthlyDetail(tOrderNum, tUserID, tSDate, tEDate);

            }
            return View(lstSubScription);
        }
        /// <summary>
        /// 月租報表下載
        /// </summary>
        /// <param name="SDate"></param>
        /// <param name="EDate"></param>
        /// <param name="userID"></param>
        /// <param name="OrderNum"></param>
        /// <returns></returns>
        public ActionResult MonthlyDetailQueryDownload(string SDate, string EDate, string userID, string OrderNum)
        {
            BaseSafeController himsSafe = new BaseSafeController();
            himsSafe.nnlog(Session["User"], Session["Account"], System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                , "MonthlyDetailQueryDownload");

            List<BE_MonthlyDetail> lstSubScription = new List<BE_MonthlyDetail>();
            SubScriptionRepository _repository = new SubScriptionRepository(connetStr);
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            string tSDate = "", tEDate = "", tUserID = "", tOrderNum = "";

            if (!string.IsNullOrEmpty(SDate))
            {
                tSDate = SDate;
                ViewData["SDate"] = tSDate;
            }
            if (!string.IsNullOrEmpty(EDate))
            {
                tEDate = EDate;
                ViewData["EDate"] = tEDate;
            }
            if (!string.IsNullOrEmpty(userID))
            {
                tUserID = userID;
                ViewData["userID"] = tUserID;
            }
            if (!string.IsNullOrEmpty(OrderNum))
            {
                tOrderNum = OrderNum;
                ViewData["OrderNum"] = tOrderNum;
                tOrderNum = tOrderNum.Replace("H", "");
            }
            bool isInDateRange = false;

            if (DateTime.TryParse(tSDate, out DateTime DS) && DateTime.TryParse(tEDate, out DateTime DE))
            {
                if (DE >= DS && DE <= DS.AddMonths(1))
                {
                    isInDateRange = true;
                }
            }
            //if (tOrderNum != "" || tUserID != "" || tSDate != "" || tEDate != "")
            if (isInDateRange || tOrderNum != "" || tUserID != "")
            {
                lstSubScription = _repository.GetMonthlyDetail(tOrderNum, tUserID, tSDate, tEDate);
            }

            //增加NLOG機制
            logger.Trace(
                    "{ReportName:'月租報表下載(MonthlyDetailQueryDownload)'," +
                    "User" + ":'" + Session["User"] + "'," +
                    "IPAddr" + ":'" + System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] + "'," +
                    "Condition:{SDate" + ":'" + SDate + "'," +
                    "EDate" + ":'" + EDate + "'," +
                    "userID" + ":'" + userID + "'," +
                    "OrderNum" + ":'" + OrderNum + "'}," +
                    "RowCount" + ":" + lstSubScription.Count.ToString() + "}"
                    );

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("搜尋結果");
            string[] headerField = { "訂單編號", "IDNO", "出車據點", "使用汽車－平日(時)", "使用汽車－假日(時)"
                , "使用機車(分)", "使用時間", "扣抵訂閱方案編號", "扣抵方案代碼"
                ,"汽車平日優惠費率","汽車假日優惠費率","機車優惠費率","扣抵方案名稱" };

            int headerFieldLen = headerField.Length;

            IRow header = sheet.CreateRow(0);
            for (int j = 0; j < headerFieldLen; j++)
            {
                header.CreateCell(j).SetCellValue(headerField[j]);
                //sheet.AutoSizeColumn(j);
            }

            int len = lstSubScription.Count;
            for (int k = 0; k < len; k++)
            {
                IRow content = sheet.CreateRow(k + 1);
                content.CreateCell(0).SetCellValue("H" + lstSubScription[k].OrderNo.ToString().PadLeft(7, '0'));  //訂單編號
                content.CreateCell(1).SetCellValue(lstSubScription[k].IDNO);   //ID
                content.CreateCell(2).SetCellValue(lstSubScription[k].lend_place);   //ID
                content.CreateCell(3).SetCellValue(lstSubScription[k].UseWorkDayHours);   //汽車－平日
                content.CreateCell(4).SetCellValue(lstSubScription[k].UseHolidayHours);   //汽車－假日
                content.CreateCell(5).SetCellValue((lstSubScription[k].UseMotoTotalHours).ToString("f1"));   //機車
                content.CreateCell(6).SetCellValue(lstSubScription[k].MKTime.ToString("yyyy-MM-dd HH:mm"));
                content.CreateCell(7).SetCellValue(lstSubScription[k].SEQNO);   //ID
                content.CreateCell(8).SetCellValue(lstSubScription[k].ProjID);   //扣抵方案代碼
                content.CreateCell(9).SetCellValue(lstSubScription[k].WorkDayRateForCarHours);//汽車平日優惠費率
                content.CreateCell(10).SetCellValue(lstSubScription[k].HolidayRateForCarHours);   //汽車假日優惠費率
                content.CreateCell(11).SetCellValue(lstSubScription[k].RateForMotorHours);   //機車優惠費率
                content.CreateCell(12).SetCellValue(lstSubScription[k].ProjNM);   //扣抵方案名稱
            }

            //for (int l = 0; l < headerFieldLen; l++)
            //{
            //    sheet.AutoSizeColumn(l);
            //}

            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            return base.File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "月租訂閱明細_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
        }
        #endregion

        #region 進出停車場明細
        /// <summary>
        /// 進出停車場明細
        /// </summary>
        /// <returns></returns>
        public ActionResult ParkingCheckInQuery()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ParkingCheckInQuery(string OrderNo)
        {
            BaseSafeController himsSafe = new BaseSafeController();
            himsSafe.nnlog(Session["User"], Session["Account"], System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                , "ParkingCheckInQuery");

            List<BE_QueryOrderMachiParkData> lstDetail = null;
            if (!string.IsNullOrEmpty(OrderNo))
            {
                ViewData["OrderNo"] = OrderNo;
                lstDetail = new ParkingRepository(connetStr).GetOrderMachiParkDetail(OrderNo.Replace("H", ""));
            }
            return View(lstDetail);
        }
        #endregion

        #region 代收停車費明細
        /// <summary>
        /// 代收停車費明細
        /// </summary>
        /// <returns></returns>
        public ActionResult ChargeParkingDetailQuery()
        {
            BaseSafeController himsSafe = new BaseSafeController();
            himsSafe.nnlog(Session["User"], Session["Account"], System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                , "ChargeParkingDetailQuery");

            return View();
        }

        /// <summary>
        /// 代收停車費明細 匯出
        /// </summary>
        /// <param name="SDate"></param>
        /// <param name="EDate"></param>
        /// <param name="CarNo"></param>
        /// <returns></returns>
        public ActionResult ExplodeParkingReport(DateTime? SDate, DateTime? EDate, string CarNo)
        {
            BaseSafeController himsSafe = new BaseSafeController();
            himsSafe.nnlog(Session["User"], Session["Account"], System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                , "ExplodeParkingReport");

            List<BE_RawDataOfMachi> lstRawDataOfMachi = new List<BE_RawDataOfMachi>();
            ParkingRepository _repository = new ParkingRepository(connetStr);
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            string tSDate = "", tEDate = "";

            if (SDate.HasValue)
            {
                tSDate = SDate.Value.ToString("yyyy-MM-dd");
                tSDate = tSDate + " 00:00:00";
            }
            if (EDate.HasValue)
            {
                tEDate = EDate.Value.ToString("yyyy-MM-dd");
                tEDate = tEDate + " 23:59:59";
            }

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("搜尋結果");
            string[] headerField = { "訂單編號(車麻吉)", "合約編號", "車牌號碼", "@停車場業者", "停車地點", "@調度停車場", "入場時間", "出場時間", "停車時數", "iRent取車時間", "iRent還車時間", "停車費用", "@場內還車" };
            int headerFieldLen = headerField.Length;

            IRow header = sheet.CreateRow(0);
            for (int j = 0; j < headerFieldLen; j++)
            {
                header.CreateCell(j).SetCellValue(headerField[j]);
                //sheet.AutoSizeColumn(j);
            }

            lstRawDataOfMachi = _repository.GetMachiReport(tSDate, tEDate, CarNo);

            //增加NLOG機制
            logger.Trace(
                    "{ReportName:'代收停車費明細(ExplodeParkingReport)'," +
                    "User" + ":'" + Session["User"] + "'," +
                    "IPAddr" + ":'" + System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] + "'," +
                    "Condition:{SDate" + ":'" + SDate + "'," +
                    "EDate" + ":'" + EDate + "'," +
                    "CarNo" + ":'" + CarNo + "'," +
                    "RowCount" + ":" + lstRawDataOfMachi.Count.ToString() + "}"
                    );

            int len = lstRawDataOfMachi.Count;
            for (int k = 0; k < len; k++)
            {
                IRow content = sheet.CreateRow(k + 1);
                content.CreateCell(0).SetCellValue(lstRawDataOfMachi[k].machi_id);  //訂單編號(車麻吉)
                content.CreateCell(1).SetCellValue(((lstRawDataOfMachi[k].OrderNo == 0) ? "未掛帳" : "H" + lstRawDataOfMachi[k].OrderNo.ToString().PadLeft(7, '0'))); //合約編號
                content.CreateCell(2).SetCellValue(lstRawDataOfMachi[k].CarNo);     //車牌號碼
                content.CreateCell(3).SetCellValue(lstRawDataOfMachi[k].OP);        //@停車場業者,20210510唐加
                content.CreateCell(4).SetCellValue(lstRawDataOfMachi[k].Name);      //停車場名稱
                content.CreateCell(5).SetCellValue(lstRawDataOfMachi[k].PP);        //@調度停車場,20210510唐加
                content.CreateCell(6).SetCellValue(lstRawDataOfMachi[k].Check_in.ToString("yyyy-MM-dd HH:mm:ss"));   //入場時間
                content.CreateCell(7).SetCellValue(lstRawDataOfMachi[k].Check_out.ToString("yyyy-MM-dd HH:mm:ss"));   //出場時間
                TimeSpan diffSecond = lstRawDataOfMachi[k].Check_out.Subtract(lstRawDataOfMachi[k].Check_in).Duration();
                content.CreateCell(8).SetCellValue(string.Format("{0}天{1}小時{2}分{3}秒", diffSecond.Days, diffSecond.Hours, diffSecond.Minutes, diffSecond.Seconds)); //停車時間
                content.CreateCell(9).SetCellValue(((lstRawDataOfMachi[k].SD.ToString("yyyy-MM-dd HH:mm:ss") == "1911-01-01 00:00:00") ? "未掛帳" : lstRawDataOfMachi[k].SD.ToString("yyyy-MM-dd HH:mm:ss"))); //iRent取車時間
                content.CreateCell(10).SetCellValue(((lstRawDataOfMachi[k].ED.ToString("yyyy-MM-dd HH:mm:ss") == "1911-01-01 00:00:00") ? "未掛帳" : lstRawDataOfMachi[k].ED.ToString("yyyy-MM-dd HH:mm:ss"))); //iRent還車時間
                content.CreateCell(11).SetCellValue(lstRawDataOfMachi[k].Amount);   //停車費用
                content.CreateCell(12).SetCellValue(lstRawDataOfMachi[k].returnFlg);   //@場內還車,20210510唐加
            }

            //for (int l = 0; l < headerFieldLen; l++)
            //{
            //    sheet.AutoSizeColumn(l);
            //}

            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            // workbook.Close();
            return base.File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "代收停車費明細_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
        }
        #endregion

        #region 光陽維運APP報表
        /// <summary>
        /// 光陽維運APP報表 - 20210119唐加
        /// </summary>
        /// <returns></returns>
        public ActionResult KymcoQuery()
        {
            return View();
        }
        [HttpPost]
        public ActionResult KymcoQuery(int AuditMode, string StartDate, string EndDate)
        {
            BaseSafeController himsSafe = new BaseSafeController();
            himsSafe.nnlog(Session["User"], Session["Account"], System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                , "KymcoQuery");

            ViewData["AuditMode"] = AuditMode;
            ViewData["StartDate"] = StartDate;
            ViewData["EndDate"] = EndDate;
            List<BE_GetKymcoList> lstData = new OtherRepository(connetStr).GetKymcoLists(AuditMode, StartDate, EndDate);
            return View(lstData);
        }
        public ActionResult ExplodeKymcoQuery(string ExplodeSDate, string ExplodeEDate, int ExplodeAuditMode)
        {
            BaseSafeController himsSafe = new BaseSafeController();
            himsSafe.nnlog(Session["User"], Session["Account"], System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                , "ExplodeKymcoQuery");

            List<BE_GetKymcoList> lstRawDataOfKymco = new List<BE_GetKymcoList>();
            OtherRepository _repository = new OtherRepository(connetStr);

            string tSDate = "", tEDate = "";
            int tAuditMode = 0;

            tSDate = ExplodeSDate;
            tEDate = ExplodeEDate;
            tAuditMode = ExplodeAuditMode;

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("搜尋結果");

            if (ExplodeAuditMode == 1)
            {
                string[] headerField = { "員工代號", "員工姓名", "區域", "種類", "車號", "維修方", "經銷商", "地址", "原因次分類", "車輛是否下線", "修改時間" };
                int headerFieldLen = headerField.Length;

                IRow header = sheet.CreateRow(0);
                for (int j = 0; j < headerFieldLen; j++)
                {
                    header.CreateCell(j).SetCellValue(headerField[j]);
                    //sheet.AutoSizeColumn(j);
                }
                lstRawDataOfKymco = _repository.GetKymcoLists(tAuditMode, tSDate, tEDate);

                //增加NLOG機制
                logger.Trace(
                        "{ReportName:'光陽維運APP報表(ExplodeKymcoQuery)'," +
                        "User" + ":'" + Session["User"] + "'," +
                        "IPAddr" + ":'" + System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] + "'," +
                        "Condition:{ExplodeSDate" + ":'" + ExplodeSDate + "'," +
                        "ExplodeEDate" + ":'" + ExplodeEDate + "'," +
                        "ExplodeAuditMode" + ":'" + ExplodeAuditMode + "'," +
                        "RowCount" + ":" + lstRawDataOfKymco.Count.ToString() + "}"
                        );

                int len = lstRawDataOfKymco.Count;
                for (int k = 0; k < len; k++)
                {
                    IRow content = sheet.CreateRow(k + 1);
                    content.CreateCell(0).SetCellValue(lstRawDataOfKymco[k].UserID);
                    content.CreateCell(1).SetCellValue(lstRawDataOfKymco[k].UserName);
                    content.CreateCell(2).SetCellValue(lstRawDataOfKymco[k].Area);
                    content.CreateCell(3).SetCellValue(lstRawDataOfKymco[k].TypeK);
                    content.CreateCell(4).SetCellValue(lstRawDataOfKymco[k].CarNo);
                    content.CreateCell(7).SetCellValue(lstRawDataOfKymco[k].MaintainType);
                    content.CreateCell(5).SetCellValue(lstRawDataOfKymco[k].DealerCodeValue);
                    content.CreateCell(6).SetCellValue(lstRawDataOfKymco[k].MemoAddr);
                    content.CreateCell(8).SetCellValue(lstRawDataOfKymco[k].Reason);
                    content.CreateCell(9).SetCellValue(lstRawDataOfKymco[k].Offline);
                    content.CreateCell(10).SetCellValue(lstRawDataOfKymco[k].UpdTime);

                }
                //for (int l = 0; l < headerFieldLen; l++)
                //{
                //    sheet.AutoSizeColumn(l);
                //}
                MemoryStream ms = new MemoryStream();
                workbook.Write(ms);
                // workbook.Close();
                return base.File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "光陽維護資料" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
            }
            else
            {
                string[] headerField = { "員工代號", "員工姓名", "區域", "種類", "車號", "整備項目", "備註", "修改時間" };
                int headerFieldLen = headerField.Length;

                IRow header = sheet.CreateRow(0);
                for (int j = 0; j < headerFieldLen; j++)
                {
                    header.CreateCell(j).SetCellValue(headerField[j]);
                    //sheet.AutoSizeColumn(j);
                }
                lstRawDataOfKymco = _repository.GetKymcoLists(tAuditMode, tSDate, tEDate);

                //增加NLOG機制
                logger.Trace(
                        "{ReportName:'光陽維運APP報表(ExplodeKymcoQuery)'," +
                        "User" + ":'" + Session["User"] + "'," +
                        "IPAddr" + ":'" + System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] + "'," +
                        "Condition:{ExplodeSDate" + ":'" + ExplodeSDate + "'," +
                        "ExplodeEDate" + ":'" + ExplodeEDate + "'," +
                        "ExplodeAuditMode" + ":'" + ExplodeAuditMode + "'," +
                        "RowCount" + ":" + lstRawDataOfKymco.Count.ToString() + "}"
                        );

                int len = lstRawDataOfKymco.Count;
                for (int k = 0; k < len; k++)
                {
                    IRow content = sheet.CreateRow(k + 1);
                    content.CreateCell(0).SetCellValue(lstRawDataOfKymco[k].UserID);
                    content.CreateCell(1).SetCellValue(lstRawDataOfKymco[k].UserName);
                    content.CreateCell(2).SetCellValue(lstRawDataOfKymco[k].Area);
                    content.CreateCell(3).SetCellValue(lstRawDataOfKymco[k].TypeK);
                    content.CreateCell(4).SetCellValue(lstRawDataOfKymco[k].CarNo);
                    content.CreateCell(5).SetCellValue(lstRawDataOfKymco[k].DealerCodeValue);
                    content.CreateCell(6).SetCellValue(lstRawDataOfKymco[k].MemoAddr);
                    content.CreateCell(7).SetCellValue(lstRawDataOfKymco[k].UpdTime);

                }
                //for (int l = 0; l < headerFieldLen; l++)
                //{
                //    sheet.AutoSizeColumn(l);
                //}
                MemoryStream ms = new MemoryStream();
                workbook.Write(ms);
                // workbook.Close();
                return base.File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "光陽維護資料" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
            }
        }
        #endregion

        #region 機車電池狀態查詢
        /// <summary>
        /// 機車電池狀態查詢
        /// </summary>
        /// <returns></returns>
        public ActionResult MotorBatteryStatusQuery()
        {
            return View();
        }
        [HttpPost]
        public ActionResult MotorBatteryStatusQuery(string CarNo, string SendDate)
        {
            BaseSafeController himsSafe = new BaseSafeController();
            himsSafe.nnlog(Session["User"], Session["Account"], System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                , "MotorBatteryStatusQuery");

            if (string.IsNullOrEmpty(SendDate))
            {
                SendDate = System.DateTime.Now.ToString("yyyy-MM-dd");
            }
            string StartDate = SendDate + " 00:00";
            string EndDate = SendDate + " 23:59";

            ViewData["CarNo"] = CarNo;
            ViewData["StartDate"] = StartDate;
            ViewData["EndDate"] = EndDate;
            ViewData["SendDate"] = SendDate;
            List<BE_MotorBatteryStatus> lstData = new CarCardCommonRepository(connetStr).GetMotorBatteryStatus(CarNo, StartDate, EndDate);
            return View(lstData);
        }
        public ActionResult ExplodeMotorBatteryStatusQuery(string ExplodeCarNo, string ExplodeSendDate)
        {
            BaseSafeController himsSafe = new BaseSafeController();
            himsSafe.nnlog(Session["User"], Session["Account"], System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                , "ExplodeMotorBatteryStatusQuery");

            if (string.IsNullOrEmpty(ExplodeSendDate))
            {
                ExplodeSendDate = System.DateTime.Now.ToString("yyyy-MM-dd");
            }
            string StartDate = ExplodeSendDate + " 00:00";
            string EndDate = ExplodeSendDate + " 23:59";

            List<BE_MotorBatteryStatus> lstRawData = new List<BE_MotorBatteryStatus>();
            CarCardCommonRepository _repository = new CarCardCommonRepository(connetStr);

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("搜尋結果");
            sheet.CreateFreezePane(0, 1);
            XSSFCellStyle cs_cell = (XSSFCellStyle)workbook.CreateCellStyle();
            XSSFCellStyle cs_cell_date = (XSSFCellStyle)workbook.CreateCellStyle();
            XSSFCellStyle cs_cell_mba = (XSSFCellStyle)workbook.CreateCellStyle();
            XSSFCellStyle cs_cell_rba = (XSSFCellStyle)workbook.CreateCellStyle();
            XSSFCellStyle cs_cell_lba = (XSSFCellStyle)workbook.CreateCellStyle();
            XSSFFont font_cell = (XSSFFont)workbook.CreateFont();
            //font_cell.FontName = "微軟正黑體"; //20210705唐MARK，AZURE會有部分機率報錯
            font_cell.FontHeightInPoints = 12;
            cs_cell.SetFont(font_cell);
            cs_cell_date.SetFont(font_cell);
            cs_cell_date.DataFormat = workbook.CreateDataFormat().GetFormat("yyyy-MM-dd HH:mm:ss");
            cs_cell_mba.SetFont(font_cell);
            cs_cell_mba.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 211, 130)));
            cs_cell_mba.FillPattern = FillPattern.SolidForeground;
            cs_cell_rba.SetFont(font_cell);
            cs_cell_rba.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(76, 224, 230)));
            cs_cell_rba.FillPattern = FillPattern.SolidForeground;
            cs_cell_lba.SetFont(font_cell);
            cs_cell_lba.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(242, 224, 255)));
            cs_cell_lba.FillPattern = FillPattern.SolidForeground;

            string[] headerField = {
                "CarNo",
                "CID",
                "Time",
                "Volt",
                "2TBA",
                "3TBA",
                "RSOC",
                "MBA",
                "",  //MBAA
                "",  //MBAT_Hi
                "",  //MBAT_Lo
                "RBA",
                "",  //RBAA
                "",  //RBAT_Hi
                "",  //RBAT_Lo
                "LBA",
                "",  //LBAA
                "",  //LBAT_Hi
                ""   //LBAT_Lo
            };
            int headerFieldLen = headerField.Length;

            IRow header = sheet.CreateRow(0);
            for (int j = 0; j < headerFieldLen; j++)
            {
                var cell = header.CreateCell(j);
                cell.CellStyle = cs_cell;
                cell.SetCellValue(headerField[j]);
                //sheet.AutoSizeColumn(j);
            }

            lstRawData = _repository.GetMotorBatteryStatus(ExplodeCarNo, StartDate, EndDate);
            int len = lstRawData.Count;
            for (int k = 0; k < len; k++)
            {
                IRow content = sheet.CreateRow(k + 1);
                content.CreateCell(0).SetCellValue(lstRawData[k].CarNo);
                content.CreateCell(1).SetCellValue(lstRawData[k].CID);
                content.CreateCell(2).SetCellValue(lstRawData[k].MKTime);
                content.CreateCell(3).SetCellValue(lstRawData[k].Volt);
                content.CreateCell(4).SetCellValue(lstRawData[k].device2TBA);
                content.CreateCell(5).SetCellValue(lstRawData[k].device3TBA);
                content.CreateCell(6).SetCellValue(lstRawData[k].deviceRSOC);
                content.CreateCell(7).SetCellValue(lstRawData[k].deviceMBA);
                content.CreateCell(8).SetCellValue(lstRawData[k].deviceMBAA);
                content.CreateCell(9).SetCellValue(lstRawData[k].deviceMBAT_Hi);
                content.CreateCell(10).SetCellValue(lstRawData[k].deviceMBAT_Lo);
                content.CreateCell(11).SetCellValue(lstRawData[k].deviceRBA);
                content.CreateCell(12).SetCellValue(lstRawData[k].deviceRBAA);
                content.CreateCell(13).SetCellValue(lstRawData[k].deviceRBAT_Hi);
                content.CreateCell(14).SetCellValue(lstRawData[k].deviceRBAT_Lo);
                content.CreateCell(15).SetCellValue(lstRawData[k].deviceLBA);
                content.CreateCell(16).SetCellValue(lstRawData[k].deviceLBAA);
                content.CreateCell(17).SetCellValue(lstRawData[k].deviceLBAT_Hi);
                content.CreateCell(18).SetCellValue(lstRawData[k].deviceLBAT_Lo);

                content.GetCell(0).CellStyle = cs_cell;
                content.GetCell(1).CellStyle = cs_cell;
                content.GetCell(2).CellStyle = cs_cell_date;
                content.GetCell(3).CellStyle = cs_cell;
                content.GetCell(4).CellStyle = cs_cell;
                content.GetCell(5).CellStyle = cs_cell;
                content.GetCell(6).CellStyle = cs_cell;
                content.GetCell(7).CellStyle = cs_cell_mba;
                content.GetCell(8).CellStyle = cs_cell_mba;
                content.GetCell(9).CellStyle = cs_cell_mba;
                content.GetCell(10).CellStyle = cs_cell_mba;
                content.GetCell(11).CellStyle = cs_cell_rba;
                content.GetCell(12).CellStyle = cs_cell_rba;
                content.GetCell(13).CellStyle = cs_cell_rba;
                content.GetCell(14).CellStyle = cs_cell_rba;
                content.GetCell(15).CellStyle = cs_cell_lba;
                content.GetCell(16).CellStyle = cs_cell_lba;
                content.GetCell(17).CellStyle = cs_cell_lba;
                content.GetCell(18).CellStyle = cs_cell_lba;
            }
            //for (int l = 0; l < headerFieldLen; l++)
            //{
            //    sheet.AutoSizeColumn(l);
            //}
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            ms.Close();
            // workbook.Close();
            return base.File(ms.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ExplodeSendDate.Replace("-", "") + "_" + ExplodeCarNo + ".xlsx");
        }
        #endregion

        #region 會員審核明細報表
        /// <summary>
        /// 會員審核明細報表 - 20210305唐加
        /// </summary>
        /// <returns></returns>
        public ActionResult MemberDetailQuery()
        {
            BaseSafeController himsSafe = new BaseSafeController();
            himsSafe.nnlog(Session["User"], Session["Account"], System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                , "MemberDetailQuery");

            return View();
        }
        //[HttpPost]
        public ActionResult ExplodeMemberDetailQuery(string StartDate, string EndDate, string[] IDNOSuff, int AuditMode)
        {
            BaseSafeController himsSafe = new BaseSafeController();
            himsSafe.nnlog(Session["User"], Session["Account"], System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                , "ExplodeMemberDetailQuery");

            //ViewData["IDNOSuff"] = (Id == null) ? "" : string.Join(",", Id);
            List<BE_GetMemList> lstRawDataOfMember = new List<BE_GetMemList>();//SP回傳的資料欄位
            OtherRepository _repository = new OtherRepository(connetStr);

            string tSDate = StartDate;
            string tEDate = EndDate;
            int tAuditMode = AuditMode;
            string IDNoSuffCombind = "";
            if (IDNOSuff != null)
            {
                if (IDNOSuff.Length > 0)
                {
                    IDNoSuffCombind += string.Format("{0},", IDNOSuff[0]);
                    int IDLEN = IDNOSuff.Length;
                    for (int i = 1; i < IDLEN; i++)
                    {
                        //IDNoSuffCombind += string.Format(",'{0}'", IDNOSuff[i]);
                        IDNoSuffCombind += string.Format("{0},", IDNOSuff[i]);
                    }
                }
            }

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("搜尋結果");


            string[] headerField = { "身分證字號", "會員編號", "審核人員", "員工編號", "審核人員群組", "進入待審時間", "審核日期", "審核結果", "不通過原因", "處理項目" };
            int headerFieldLen = headerField.Length;

            IRow header = sheet.CreateRow(0);
            for (int j = 0; j < headerFieldLen; j++)
            {
                header.CreateCell(j).SetCellValue(headerField[j]);
                //sheet.AutoSizeColumn(j);
            }
            lstRawDataOfMember = _repository.GetMemLists(tAuditMode, tSDate, tEDate, IDNoSuffCombind);

            //增加NLOG機制
            logger.Trace(
                    "{ReportName:'會員審核明細報表(ExplodeMemberDetailQuery)'," +
                    "User" + ":'" + Session["User"] + "'," +
                    "IPAddr" + ":'" + System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] + "'," +
                    "Condition:{StartDate" + ":'" + StartDate + "'," +
                    "EndDate" + ":'" + EndDate + "'," +
                    "AuditMode" + ":'" + AuditMode + "'," +
                    "RowCount" + ":" + lstRawDataOfMember.Count.ToString() + "}"
                    );

            int len = lstRawDataOfMember.Count;
            for (int k = 0; k < len; k++)
            {
                IRow content = sheet.CreateRow(k + 1);
                content.CreateCell(0).SetCellValue(lstRawDataOfMember[k].ID);
                content.CreateCell(1).SetCellValue(lstRawDataOfMember[k].MEMRFNBR);
                content.CreateCell(2).SetCellValue(lstRawDataOfMember[k].NAME);
                content.CreateCell(3).SetCellValue(lstRawDataOfMember[k].HIID);
                content.CreateCell(4).SetCellValue(lstRawDataOfMember[k].Group);
                content.CreateCell(5).SetCellValue(lstRawDataOfMember[k].DATE_NEW);
                content.CreateCell(6).SetCellValue(lstRawDataOfMember[k].DATE);
                content.CreateCell(7).SetCellValue(lstRawDataOfMember[k].ITEM);
                content.CreateCell(8).SetCellValue(lstRawDataOfMember[k].TYPE);
                content.CreateCell(9).SetCellValue(lstRawDataOfMember[k].REASON);

            }
            //for (int l = 0; l < headerFieldLen; l++)
            //{
            //    sheet.AutoSizeColumn(l);
            //}
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            // workbook.Close();
            return base.File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "會員審核明細" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
        }
        #endregion

        #region 悠遊付退款
        /// <summary>
        /// 悠遊付退款
        /// </summary>
        /// <returns></returns>  
        public ActionResult ReFund()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ReFund(string IDNO)
        {
            BaseSafeController himsSafe = new BaseSafeController();
            himsSafe.nnlog(Session["User"], Session["Account"], System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                , "ReFund");

            ViewData["IDNO"] = IDNO;
            List<BE_GetEasyWalletList> lstData = new MemberRepository(connetStr).GetEasyWalletList(IDNO);
            return View(lstData);

        }
        public ActionResult ExplodeReFund(string ExplodeSDate, string ExplodeEDate)
        {
            BaseSafeController himsSafe = new BaseSafeController();
            himsSafe.nnlog(Session["User"], Session["Account"], System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                , "ExplodeReFund");

            ViewData["StartDate"] = ExplodeSDate;
            ViewData["EndDate"] = ExplodeEDate;

            List<BE_Refund> lstData = null;
            MemberRepository repository = new MemberRepository(connetStr);

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("搜尋結果");

            string[] headerField = { "訂單編號", "購買者id", "購買卡號", "購買日", "到期日", "購買方案", "收款總額", "手續費", "實收金額", "退款日期" };
            int headerFieldLen = headerField.Length;

            IRow header = sheet.CreateRow(0);
            for (int j = 0; j < headerFieldLen; j++)
            {
                header.CreateCell(j).SetCellValue(headerField[j]);
                //sheet.AutoSizeColumn(j);
            }
            lstData = repository.GetEasyWalletOrder(ExplodeSDate, ExplodeEDate);

            //增加NLOG機制
            logger.Trace(
                    "{ReportName:'悠遊付退款(ExplodeReFund)'," +
                    "User" + ":'" + Session["User"] + "'," +
                    "IPAddr" + ":'" + System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] + "'," +
                    "Condition:{ExplodeSDate" + ":'" + ExplodeSDate + "'," +
                    "ExplodeEDate" + ":'" + ExplodeEDate + "'," +
                    "RowCount" + ":" + lstData.Count.ToString() + "}"
                    );

            int len = lstData.Count;
            for (int k = 0; k < len; k++)
            {
                IRow content = sheet.CreateRow(k + 1);
                content.CreateCell(0).SetCellValue(lstData[k].orderNo);
                content.CreateCell(1).SetCellValue(lstData[k].IDNO);
                content.CreateCell(2).SetCellValue(lstData[k].easyCardNo);
                content.CreateCell(3).SetCellValue(lstData[k].orderTime);
                content.CreateCell(4).SetCellValue(lstData[k].endTime);
                content.CreateCell(5).SetCellValue(lstData[k].ITEM);
                content.CreateCell(6).SetCellValue(lstData[k].PRICE);
                content.CreateCell(7).SetCellValue(lstData[k].tax);
                content.CreateCell(8).SetCellValue(lstData[k].amount);
                content.CreateCell(9).SetCellValue(lstData[k].refunddate);

            }
            //for (int l = 0; l < headerFieldLen; l++)
            //{
            //    sheet.AutoSizeColumn(l);
            //}
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            // workbook.Close();
            return base.File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "悠遊付訂單" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
        }
        #endregion

        #region 營運狀態記錄報表
        /// <summary>
        /// 營運狀態記錄報表
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportCarSettingData()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ExportCarSettingData(string isExport, string StationID, string Time_Start, string Time_End)
        {
            BaseSafeController himsSafe = new BaseSafeController();
            himsSafe.nnlog(Session["User"], Session["Account"], System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                , "ExportCarSettingData");

            CarStatusCommon carStatusCommon = new CarStatusCommon(connetStr);
            List<BE_CarSettingRecord> lstData = new List<BE_CarSettingRecord>();
            lstData = carStatusCommon.GetCarSettingRecord(StationID, Time_Start, Time_End);

            //增加NLOG機制
            logger.Trace(
                    "{ReportName:'營運狀態記錄報表(ExportCarSettingData)'," +
                    "User" + ":'" + Session["User"] + "'," +
                    "IPAddr" + ":'" + System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] + "'," +
                    "Condition:{isExport" + ":'" + isExport + "'," +
                    "StationID" + ":'" + StationID + "'," +
                    "Time_Start" + ":'" + Time_Start + "'," +
                    "Time_End" + ":'" + Time_End + "'," +
                    "RowCount" + ":" + lstData.Count.ToString() + "}"
                    );

            if (isExport == "true")
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage ep = new ExcelPackage();
                ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("Sheet");

                int col = 1;
                int row = 2;
                MemoryStream fileStream = new MemoryStream();

                if (StationID == "X0SR" || StationID == "X0R4" || StationID == "X0U4" || StationID == "X1V4")
                {
                    sheet.Cells[1, col++].Value = "時間";
                    sheet.Cells[1, col++].Value = "據點";
                    sheet.Cells[1, col++].Value = "總數量";
                    sheet.Cells[1, col++].Value = "出租中";
                    sheet.Cells[1, col++].Value = "可出租";
                    sheet.Cells[1, col++].Value = "待上線";
                    sheet.Cells[1, col++].Value = "低電量";
                    sheet.Cells[1, col++].Value = "一小時無回應";
                    sheet.Cells[1, col++].Value = "無回應";


                    foreach (var i in lstData)
                    {
                        col = 1;
                        sheet.Cells[row, col++].Value = i.Time.ToString("yyyy-MM-dd HH:mm:ss");
                        sheet.Cells[row, col++].Value = i.Station;
                        sheet.Cells[row, col++].Value = i.Total;
                        sheet.Cells[row, col++].Value = i.Renting;
                        sheet.Cells[row, col++].Value = i.OnBoard;
                        sheet.Cells[row, col++].Value = i.OffBoard;
                        sheet.Cells[row, col++].Value = i.Volt;
                        sheet.Cells[row, col++].Value = i.Nonresponse_OneHour;
                        sheet.Cells[row, col++].Value = i.Nonresponse;
                        row++;
                    }

                    ep.SaveAs(fileStream);
                    ep.Dispose();
                    fileStream.Position = 0;
                    return File(fileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{Time_Start}_to_{Time_End}_營運狀態記錄.xlsx");
                }

                sheet.Cells[1, col++].Value = "時間";
                sheet.Cells[1, col++].Value = "據點";
                sheet.Cells[1, col++].Value = "總數量";
                sheet.Cells[1, col++].Value = "出租中";
                sheet.Cells[1, col++].Value = "可出租";
                sheet.Cells[1, col++].Value = "待上線";
                sheet.Cells[1, col++].Value = "低電量(3TBA)";
                sheet.Cells[1, col++].Value = "低電量(2TBA)";
                sheet.Cells[1, col++].Value = "一小時無回應";
                sheet.Cells[1, col++].Value = "無回應";


                foreach (var i in lstData)
                {
                    col = 1;
                    sheet.Cells[row, col++].Value = i.Time.ToString("yyyy-MM-dd HH:mm:ss");
                    sheet.Cells[row, col++].Value = i.Station;
                    sheet.Cells[row, col++].Value = i.Total;
                    sheet.Cells[row, col++].Value = i.Renting;
                    sheet.Cells[row, col++].Value = i.OnBoard;
                    sheet.Cells[row, col++].Value = i.OffBoard;
                    sheet.Cells[row, col++].Value = i.LowBattery_3TBA;
                    sheet.Cells[row, col++].Value = i.LowBattery_2TBA;
                    sheet.Cells[row, col++].Value = i.Nonresponse_OneHour;
                    sheet.Cells[row, col++].Value = i.Nonresponse;
                    row++;
                }

                ep.SaveAs(fileStream);
                ep.Dispose();
                fileStream.Position = 0;
                return File(fileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{Time_Start}_to_{Time_End}_營運狀態記錄.xlsx");
            }
            else
            {
                return View(lstData);
            }
        }
        #endregion

        #region 車輛隨租定位
        /// <summary>
        /// 車輛隨租定位
        /// </summary>
        /// <returns></returns>
        public ActionResult CarLocationQuery()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CarLocationQuery(string IsCar, string Time_Start, string Time_End, string Account)
        {
            BaseSafeController himsSafe = new BaseSafeController();
            himsSafe.nnlog(Session["User"], Session["Account"], System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                , "CarLocationQuery");

            CarStatusCommon carStatusCommon = new CarStatusCommon(connetStr);
            List<BE_CarLocationData> lstData = new List<BE_CarLocationData>();
            lstData = carStatusCommon.GetCarLocationData(Time_Start, Time_End, IsCar);
            string carType = (IsCar == "true") ? "汽車" : "機車";

            //增加NLOG機制
            logger.Trace(
                    "{ReportName:'車輛隨租定位(CarLocationQuery)'," +
                    "User" + ":'" + Session["User"] + "'," +
                    "IPAddr" + ":'" + System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] + "'," +
                    "Condition:{IsCar" + ":'" + ((IsCar == "true") ? "汽車" : "機車") + "'," +
                    "Time_Start" + ":'" + Time_Start + "'," +
                    "Time_End" + ":'" + Time_End + "'," +
                    "Account" + ":'" + Account + "'," +
                    "RowCount" + ":" + lstData.Count.ToString() + "}"
                    );

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("Sheet");

            int col = 1;
            int row = 2;
            MemoryStream fileStream = new MemoryStream();

            sheet.Cells[1, col++].Value = "order_number";
            sheet.Cells[1, col++].Value = "IDNO";
            sheet.Cells[1, col++].Value = "CarNo";
            sheet.Cells[1, col++].Value = "PRONAME";
            sheet.Cells[1, col++].Value = "ProjID";
            sheet.Cells[1, col++].Value = "lend_place";
            sheet.Cells[1, col++].Value = "final_start_time";
            sheet.Cells[1, col++].Value = "final_stop_time";
            sheet.Cells[1, col++].Value = "CID";
            sheet.Cells[1, col++].Value = "start_Lat";
            sheet.Cells[1, col++].Value = "start_Lng";
            sheet.Cells[1, col++].Value = "stop_Lat";
            sheet.Cells[1, col++].Value = "stop_Lng";

            foreach (var i in lstData)
            {
                col = 1;
                sheet.Cells[row, col++].Value = i.order_number;
                sheet.Cells[row, col++].Value = i.IDNO;
                sheet.Cells[row, col++].Value = i.CarNo;
                sheet.Cells[row, col++].Value = i.PRONAME;
                sheet.Cells[row, col++].Value = i.ProjID;
                sheet.Cells[row, col++].Value = i.lend_place;
                sheet.Cells[row, col++].Value = i.final_start_time.ToString("yyyy-MM-dd HH:mm:ss");
                sheet.Cells[row, col++].Value = i.final_stop_time.ToString("yyyy-MM-dd HH:mm:ss");
                sheet.Cells[row, col++].Value = i.CID;
                sheet.Cells[row, col++].Value = i.start_Lat;
                sheet.Cells[row, col++].Value = i.start_Lng;
                sheet.Cells[row, col++].Value = i.stop_Lat;
                sheet.Cells[row, col++].Value = i.stop_Lng;

                row++;
            }

            ep.SaveAs(fileStream);
            ep.Dispose();
            fileStream.Position = 0;
            return File(fileStream, "application/xlsx", $"{Time_Start}_to_{Time_End}_車輛隨租定位({carType}).xlsx");
        }
        #endregion

        #region 主動取款明細查詢
        /// <summary>
        /// 主動取款明細查詢 - 20210714唐加
        /// </summary>
        public ActionResult IrentPaymentDetail()
        {
            ViewData["StartDate"] = "";
            ViewData["EndDate"] = "";
            ViewData["StartDate2"] = "";
            ViewData["EndDate2"] = "";
            ViewData["MEMACCOUNT"] = "";
            return View();
        }
        #endregion

        #region 主動取款歷程查詢
        /// <summary>
        /// 主動取款歷程查詢 - 20210714唐加
        /// </summary>
        public ActionResult IrentPaymentHistory()
        {
            ViewData["StartDate"] = "";
            ViewData["EndDate"] = "";
            ViewData["StartDate2"] = "";
            ViewData["EndDate2"] = "";
            ViewData["StartDate3"] = "";
            ViewData["EndDate3"] = "";
            ViewData["MEMACCOUNT"] = "";
            return View();
        }
        #endregion

        #region 新北監管平台月報檔案上傳
        /// <summary>
        /// 新北監管平台月報檔案上傳 - 20210820 Frank加
        /// </summary>
        public ActionResult CarMapFileUpload()
        {
            return View();
        }

        [HttpPost]
        [Obsolete]
        public ActionResult CarMapFileUpload(HttpPostedFileBase fileImport,string month, string carType, string Account, string export)
        {
            BaseSafeController himsSafe = new BaseSafeController();
            himsSafe.nnlog(Session["User"], Session["Account"], System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                , "CarMapFileUpload");

            //匯出檔案
            if (export == "true")
            {
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["IRent"].ConnectionString);
                SqlTransaction tran;
                conn.Open();
                tran = conn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.Transaction = tran;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_GetIRentCarMapValue";
                cmd.Parameters.Add("@Key", SqlDbType.NVarChar, 20).Value = string.Format("{0}_{1}", month, carType);
                SqlParameter msg = cmd.Parameters.Add("@MSG", SqlDbType.VarChar, 200);
                SqlParameter fileName = cmd.Parameters.Add("@Value", SqlDbType.NVarChar, 50);
                msg.Direction = ParameterDirection.Output;
                fileName.Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                conn.Close();
                conn.Dispose();

                if (fileName.Value.ToString() == "")
                {
                    ViewData["result"] = "檔案尚未上傳";
                    return View();
                }

                var blob = new AzureStorageHandle().DownloadFile("monthlyreport", fileName.Value.ToString());
                Stream blobStream = blob.OpenRead();
                return File(blobStream, blob.Properties.ContentType, blob.Name);
            }

            //上傳檔案
            if(fileImport != null)
            {
                if(fileImport.ContentLength > 0)
                {

                    using (var reader = new StreamReader(fileImport.InputStream, Encoding.UTF8))
                    {
                        string file = reader.ReadToEnd();

                        var subFileName = fileImport.FileName.Substring(fileImport.FileName.IndexOf("."));
                        var fileName = string.Format("{0}_{1}_{2}_{3}", month, carType, fileImport.FileName.Substring(0, fileImport.FileName.IndexOf(".")), DateTime.Now.ToString("yyyyMMddHHmmss") + subFileName);

                        DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/Content/upload/CarMapFileUpload"));
                        if (!di.Exists)
                        {
                            di.Create();
                        }
                        string path = Path.Combine(Server.MapPath("~/Content/upload/CarMapFileUpload"), fileName);
                        fileImport.SaveAs(path);

                        var flag = new AzureStorageHandle().UploadFileToAzureStorage(fileImport, fileName, "monthlyreport", path);

                        //儲存key值進DB
                        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["IRent"].ConnectionString);
                        SqlTransaction tran;
                        conn.Open();
                        tran = conn.BeginTransaction();
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.Transaction = tran;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "usp_HandleIRentCarMapKey";
                        cmd.Parameters.Add("@Key", SqlDbType.NVarChar, 20).Value = string.Format("{0}_{1}", month, carType);
                        cmd.Parameters.Add("@Value", SqlDbType.NVarChar, 50).Value = fileName;
                        cmd.Parameters.Add("@User", SqlDbType.VarChar, 10).Value = Account;
                        SqlParameter msg = cmd.Parameters.Add("@MSG", SqlDbType.VarChar, 200);
                        msg.Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();
                        tran.Commit();
                        conn.Close();
                        conn.Dispose();


                        if (flag)
                        {
                            ViewData["result"] = "執行成功";
                        }
                        else
                        {
                            ViewData["result"] = "上傳雲端過程失敗";
                        }
                        reader.Close();
                    }
                }
            }
            else
            {
                ViewData["result"] = "未上傳任何檔案";
            }
            
            
            return View();
        }
        #endregion
    }
}