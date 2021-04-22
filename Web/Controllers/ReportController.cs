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
using WebCommon;

namespace Web.Controllers
{
    /// <summary>
    /// 報表
    /// </summary>
    public class ReportController : Controller
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
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
            return View(lstData);
        }
        public ActionResult MaintainLogReportDownload(string SDate, string EDate, string carid, string objStation, string userID, int? status)
        {
            List<BE_CleanDataWithoutPIC> data = new List<BE_CleanDataWithoutPIC>();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            data = new CarClearRepository(connetStr).GetCleanDataWithOutPic(SDate, EDate, carid, objStation, userID, (status.HasValue) ? status.Value : 3, ref lstError);
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("搜尋結果");
            string[] headerField = { "帳號", "整備人員", "訂單編號", "車號", "據點", "狀態", "實際取車", "實際還車", "車外清潔", "車內清潔", "車輛救援", "車輛調度", "車輛調度(路邊租還)", "保養", "清潔時幾天未清", "出租次數", "備註" };
            int headerFieldLen = headerField.Length;

            IRow header = sheet.CreateRow(0);
            for (int j = 0; j < headerFieldLen; j++)
            {
                header.CreateCell(j).SetCellValue(headerField[j]);
                sheet.AutoSizeColumn(j);
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

                double totalDay = ((data[k].lastCleanTime.ToString("yyyy-MM-dd HH:mm:ss") == "1900-01-01 00:00:00")) ? data[k].BookingStart.Subtract(data[k].lastCleanTime).TotalDays : -1;
                string totalDayStr = (totalDay == -1) ? "從未清潔" : ((totalDay < 1) ? Math.Round(data[k].BookingStart.Subtract(data[k].lastCleanTime).TotalHours, MidpointRounding.AwayFromZero) + "小時" : Math.Round(totalDay).ToString());
                if (data[k].OrderStatus < 2)
                {
                    totalDayStr = DateTime.Now.Date.Subtract(Convert.ToDateTime(data[k].lastCleanTime).Date).TotalDays.ToString();
                }
                IRow content = sheet.CreateRow(k + 1);
                content.CreateCell(0).SetCellValue(data[k].Account);
                content.CreateCell(1).SetCellValue(data[k].UserID);
                content.CreateCell(2).SetCellValue("H" + data[k].OrderNum.ToString().PadLeft(7, '0'));           //合約
                content.CreateCell(3).SetCellValue(data[k].CarNo);                                               //車號
                content.CreateCell(4).SetCellValue(data[k].lend_place);                                          //據點
                content.CreateCell(5).SetCellValue(OrderStatus);                                                 //狀態
                if(data[k].OrderStatus<1 || data[k].OrderStatus == 4)
                {
                    content.CreateCell(6).SetCellValue( "未取車");  //實際取車
                }
                else
                {
                    content.CreateCell(6).SetCellValue(data[k].BookingStart.ToString("yyyy-MM-dd HH:mm:ss").Replace("1900-01-01 00:00:00", "未取車"));  //實際取車
                }
                if (data[k].OrderStatus < 1 || data[k].OrderStatus == 4)
                {
                    content.CreateCell(7).SetCellValue("未取車");     //實際還車
                }else if(data[k].OrderStatus==1)
                {
                    content.CreateCell(7).SetCellValue("未還車");     //實際還車
                }
                else if (data[k].OrderStatus == 5)
                {
                    content.CreateCell(7).SetCellValue("逾時未還車【系統強還時間："+ data[k].BookingEnd.ToString("yyyy-MM-dd HH:mm:ss") + "】");     //實際還車
                }
                else
                {
                    content.CreateCell(7).SetCellValue(data[k].BookingEnd.ToString("yyyy-MM-dd HH:mm:ss").Replace("1900-01-01 00:00:00", "未還車"));    //實際還車
                }
                content.CreateCell(8).SetCellValue((data[k].outsideClean == 1) ? "✔" : "✖");                                                 //車外清潔
                content.CreateCell(9).SetCellValue((data[k].insideClean == 1) ? "✔" : "✖");                                                 //車內清潔
                content.CreateCell(10).SetCellValue((data[k].rescue == 1) ? "✔" : "✖");                                                 //車輛救援
                content.CreateCell(11).SetCellValue((data[k].dispatch == 1) ? "✔" : "✖");                                                 //車輛調度
                content.CreateCell(12).SetCellValue((data[k].Anydispatch == 1) ? "✔" : "✖");                                                 //車輛調度(路邊租還)
                content.CreateCell(13).SetCellValue((data[k].Maintenance == 1) ? "✔" : "✖");
                content.CreateCell(14).SetCellValue(totalDayStr);
                content.CreateCell(15).SetCellValue(data[k].lastRentTimes);
                content.CreateCell(16).SetCellValue(data[k].remark);                                                 //備註

            }
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);

            return base.File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "整備人員查詢結果_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
        }
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
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("搜尋結果");
            string[] headerField = { "回饋日期", "回饋狀態", "合約NO.", "車號", "ID", "姓名", "手機", "內容", "狀態", "處理結果", "處理者" };
            int headerFieldLen = headerField.Length;

            IRow header = sheet.CreateRow(0);
            for (int j = 0; j < headerFieldLen; j++)
            {
                header.CreateCell(j).SetCellValue(headerField[j]);
                sheet.AutoSizeColumn(j);
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
                sheet.AutoSizeColumn(0);
                sheet.AutoSizeColumn(1);
                sheet.AutoSizeColumn(2);
                sheet.AutoSizeColumn(3);
                sheet.AutoSizeColumn(4);
                sheet.AutoSizeColumn(5);
                sheet.AutoSizeColumn(6);
                sheet.AutoSizeColumn(7);
                sheet.AutoSizeColumn(8);
                sheet.AutoSizeColumn(9);
                sheet.AutoSizeColumn(10);
            }
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
           // workbook.Close();
            return base.File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "車況回饋_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
        }
        /// <summary>
        /// 綠界交易記錄查詢
        /// </summary>
        /// <returns></returns>
        public ActionResult TradeQuery()
        {
            return View();
        }
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
            List<BE_MonthlyQuery> lstSubScription = new List<BE_MonthlyQuery>();
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
            if (tmpIsHandle < 2 || tUserID != "" || tSDate != "" || tEDate != "")
            {
                lstSubScription = _repository.BE_QueryMonthlyMain(userID, tSDate, tEDate, tmpIsHandle);
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
            List<BE_MonthlyQuery> lstSubScription = new List<BE_MonthlyQuery>();
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
            if (tmpIsHandle < 2 || tUserID != "" || tSDate != "" || tEDate != "")
            {
                lstSubScription = _repository.BE_QueryMonthlyMain(userID, tSDate, tEDate, tmpIsHandle);
            }
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("搜尋結果");
            string[] headerField = { "訂閱方案編號", "方案代碼", "方案名稱", "方案生效時間", "方案結束時間", "IDNO", "汽車－平日", "汽車－假日", "機車" };
            int headerFieldLen = headerField.Length;

            IRow header = sheet.CreateRow(0);
            for (int j = 0; j < headerFieldLen; j++)
            {
                header.CreateCell(j).SetCellValue(headerField[j]);
                sheet.AutoSizeColumn(j);
            }

            int len = lstSubScription.Count;
            for (int k = 0; k < len; k++)
            {
                IRow content = sheet.CreateRow(k + 1);
                content.CreateCell(0).SetCellValue(lstSubScription[k].SEQNO);   //ID
                content.CreateCell(1).SetCellValue(lstSubScription[k].ProjID);   //ID
                content.CreateCell(2).SetCellValue(lstSubScription[k].ProjNM);   //ID
                content.CreateCell(3).SetCellValue(lstSubScription[k].StartDate.ToString("yyyy-MM-dd HH:mm"));  //合約起
                content.CreateCell(4).SetCellValue(lstSubScription[k].EndDate.ToString("yyyy-MM-dd HH:mm"));    //合約迄                                                                                 //  content.CreateCell(1).SetCellValue((lstFeedBack[k].isHandle == 1) ? "還車" : "取車");         //處理狀態
                                                                                                                // content.CreateCell(2).SetCellValue("H" + lstFeedBack[k].order_number.ToString().PadLeft(7, '0'));   //合約
                                                                                                                // content.CreateCell(3).SetCellValue(lstFeedBack[k].CarNo);   //車號
                content.CreateCell(5).SetCellValue(lstSubScription[k].IDNO);   //ID
                content.CreateCell(6).SetCellValue(lstSubScription[k].WorkDayHours);   //汽車－平日
                content.CreateCell(7).SetCellValue(lstSubScription[k].HolidayHours);   //汽車－假日
                content.CreateCell(8).SetCellValue((lstSubScription[k].MotoTotalHours ).ToString("f1"));   //機車

                sheet.AutoSizeColumn(0);
                sheet.AutoSizeColumn(1);
                sheet.AutoSizeColumn(2);
                sheet.AutoSizeColumn(3);
                sheet.AutoSizeColumn(4);
                sheet.AutoSizeColumn(5);
                sheet.AutoSizeColumn(6);
                sheet.AutoSizeColumn(7);
                sheet.AutoSizeColumn(8);

                /* sheet.AutoSizeColumn(8);
                 sheet.AutoSizeColumn(9);
                 sheet.AutoSizeColumn(10);*/
            }
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            //workbook.Close();
            return base.File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "月租訂閱總表_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
        }
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
            List<BE_MonthlyReportData> lstSubScription = new List<BE_MonthlyReportData>();
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
            if (tOrderNum != "" || tUserID != "" || tSDate != "" || tEDate != "")
            {
                lstSubScription = _repository.GetMonthlyReportQuery(tOrderNum, tUserID, tSDate, tEDate);
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
            List<BE_MonthlyReportData> lstSubScription = new List<BE_MonthlyReportData>();
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
            if (tOrderNum != "" || tUserID != "" || tSDate != "" || tEDate != "")
            {
                lstSubScription = _repository.GetMonthlyReportQuery(tOrderNum, tUserID, tSDate, tEDate);
            }
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("搜尋結果");
            string[] headerField = { "訂單編號", "IDNO", "出車據點", "使用汽車－平日(時)", "使用汽車－假日(時)", "使用機車(分)", "使用時間", "扣抵訂閱方案編號", "扣抵方案代碼", "扣抵方案名稱" };
            int headerFieldLen = headerField.Length;

            IRow header = sheet.CreateRow(0);
            for (int j = 0; j < headerFieldLen; j++)
            {
                header.CreateCell(j).SetCellValue(headerField[j]);
                sheet.AutoSizeColumn(j);
            }

            int len = lstSubScription.Count;
            for (int k = 0; k < len; k++)
            {
                IRow content = sheet.CreateRow(k + 1);
                content.CreateCell(0).SetCellValue("H" + lstSubScription[k].OrderNo.ToString().PadLeft(7, '0'));  //訂單編號
                                                                                                                   //合約迄                                                                                 //  content.CreateCell(1).SetCellValue((lstFeedBack[k].isHandle == 1) ? "還車" : "取車");         //處理狀態
                                                                                                                   // content.CreateCell(2).SetCellValue("H" + lstFeedBack[k].order_number.ToString().PadLeft(7, '0'));   //合約
                                                                                                                   // content.CreateCell(3).SetCellValue(lstFeedBack[k].CarNo);   //車號
                content.CreateCell(1).SetCellValue(lstSubScription[k].IDNO);   //ID
                content.CreateCell(2).SetCellValue(lstSubScription[k].lend_place);   //ID
                content.CreateCell(3).SetCellValue(lstSubScription[k].UseWorkDayHours);   //汽車－平日
                content.CreateCell(4).SetCellValue(lstSubScription[k].UseHolidayHours);   //汽車－假日
                content.CreateCell(5).SetCellValue((lstSubScription[k].UseMotoTotalHours).ToString("f1"));   //機車
                content.CreateCell(6).SetCellValue(lstSubScription[k].MKTime.ToString("yyyy-MM-dd HH:mm"));
                content.CreateCell(7).SetCellValue(lstSubScription[k].SEQNO);   //ID
                content.CreateCell(8).SetCellValue(lstSubScription[k].ProjID);   //ID
                content.CreateCell(9).SetCellValue(lstSubScription[k].ProjNM);   //汽車－平日
                sheet.AutoSizeColumn(0);
                sheet.AutoSizeColumn(1);
                sheet.AutoSizeColumn(2);
                sheet.AutoSizeColumn(3);
                sheet.AutoSizeColumn(4);
                sheet.AutoSizeColumn(5);
                sheet.AutoSizeColumn(6);
                sheet.AutoSizeColumn(7);
                sheet.AutoSizeColumn(8);
                sheet.AutoSizeColumn(9);

                /* sheet.AutoSizeColumn(8);
                 sheet.AutoSizeColumn(9);
                 sheet.AutoSizeColumn(10);*/
            }
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            return base.File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "月租訂閱明細_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
        }
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
            List<BE_QueryOrderMachiParkData> lstDetail = null;
            if (!string.IsNullOrEmpty(OrderNo))
            {
                ViewData["OrderNo"] = OrderNo;
                lstDetail = new ParkingRepository(connetStr).GetOrderMachiParkDetail(OrderNo.Replace("H", ""));
            }
            return View(lstDetail);
        }
        /// <summary>
        /// 代收停車費明細
        /// </summary>
        /// <returns></returns>
        public ActionResult ChargeParkingDetailQuery()
        {
            return View();
        }
        public ActionResult ExplodeParkingReport(DateTime? SDate, DateTime? EDate, string CarNo)
        {
            List<BE_RawDataOfMachi> lstRawDataOfMachi = new List<BE_RawDataOfMachi>();
            ParkingRepository _repository = new ParkingRepository(connetStr);
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            string tSDate = "", tEDate = "", tCarNo = "";


            if (SDate.HasValue)
            {
                tSDate = SDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }
            if (EDate.HasValue)
            {
                tEDate = EDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }
            if (!string.IsNullOrEmpty(CarNo))
            {
                tCarNo = CarNo;

            }

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("搜尋結果");
            string[] headerField = { "訂單編號(車麻吉)", "合約編號", "車牌號碼", "停車地點", "入場時間", "出場時間", "停車時數", "iRent取車時間", "iRent還車時間", "停車費用", "優惠折扣", "付款時間", "發票號碼(車麻吉)", "疑似違規" };
            int headerFieldLen = headerField.Length;

            IRow header = sheet.CreateRow(0);
            for (int j = 0; j < headerFieldLen; j++)
            {
                header.CreateCell(j).SetCellValue(headerField[j]);
                sheet.AutoSizeColumn(j);
            }
            lstRawDataOfMachi = _repository.GetMachiReport(tSDate, tEDate, CarNo);
            int len = lstRawDataOfMachi.Count;
            for (int k = 0; k < len; k++)
            {
                IRow content = sheet.CreateRow(k + 1);
                content.CreateCell(0).SetCellValue(lstRawDataOfMachi[k].machi_id);   //訂單編號(車麻吉)
                content.CreateCell(1).SetCellValue(((lstRawDataOfMachi[k].OrderNo == 0) ? "未掛帳" : "H" + lstRawDataOfMachi[k].OrderNo.ToString().PadLeft(7, '0'))); //合約編號
                content.CreateCell(2).SetCellValue(lstRawDataOfMachi[k].CarNo);   //車牌號碼
                content.CreateCell(3).SetCellValue(lstRawDataOfMachi[k].Name);   //停車地點
                content.CreateCell(4).SetCellValue(lstRawDataOfMachi[k].Check_in.ToString("yyyy-MM-dd HH:mm:ss"));   //入場時間
                content.CreateCell(5).SetCellValue(lstRawDataOfMachi[k].Check_out.ToString("yyyy-MM-dd HH:mm:ss"));   //出場時間
                TimeSpan diffSecond = lstRawDataOfMachi[k].Check_out.Subtract(lstRawDataOfMachi[k].Check_in).Duration();
                content.CreateCell(6).SetCellValue(string.Format("{0}天{1}小時{2}分{3}秒", diffSecond.Days, diffSecond.Hours, diffSecond.Minutes, diffSecond.Seconds)); //停車時間
                content.CreateCell(7).SetCellValue(((lstRawDataOfMachi[k].SD.ToString("yyyy-MM-dd HH:mm:ss") == "1911-01-01 00:00:00") ? "未掛帳" : lstRawDataOfMachi[k].SD.ToString("yyyy-MM-dd HH:mm:ss"))); //iRent取車時間
                content.CreateCell(8).SetCellValue(((lstRawDataOfMachi[k].ED.ToString("yyyy-MM-dd HH:mm:ss") == "1911-01-01 00:00:00") ? "未掛帳" : lstRawDataOfMachi[k].ED.ToString("yyyy-MM-dd HH:mm:ss"))); //iRent還車時間
                content.CreateCell(9).SetCellValue(lstRawDataOfMachi[k].Amount);   //停車費用
                content.CreateCell(10).SetCellValue(lstRawDataOfMachi[k].refund_amount);   //優惠折扣
                content.CreateCell(11).SetCellValue(lstRawDataOfMachi[k].paid_at.ToString("yyyy-MM-dd HH:mm:ss"));   //付款時間
                content.CreateCell(12).SetCellValue("無");   //發票號碼(車麻吉)
                content.CreateCell(13).SetCellValue((lstRawDataOfMachi[k].Conviction == 1) ? "疑似" : "否");   //發票號碼(車麻吉)


                /* sheet.AutoSizeColumn(0);
               sheet.AutoSizeColumn(1);
               sheet.AutoSizeColumn(2);
               sheet.AutoSizeColumn(3);
               sheet.AutoSizeColumn(4);
               sheet.AutoSizeColumn(5);
               sheet.AutoSizeColumn(6);

              sheet.AutoSizeColumn(8);
                sheet.AutoSizeColumn(9);
                sheet.AutoSizeColumn(10);*/
            }
            for (int l = 0; l < headerFieldLen; l++)
            {
                sheet.AutoSizeColumn(l);
            }
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
           // workbook.Close();
            return base.File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "代收停車費明細_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
        }
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
            ViewData["AuditMode"] = AuditMode;
            ViewData["StartDate"] = StartDate;
            ViewData["EndDate"] = EndDate;
            List<BE_GetKymcoList> lstData = new OtherRepository(connetStr).GetKymcoLists(AuditMode, StartDate, EndDate);
            return View(lstData);
        }
        public ActionResult ExplodeKymcoQuery(string ExplodeSDate, string ExplodeEDate, int ExplodeAuditMode)
        {
            List<BE_GetKymcoList> lstRawDataOfMachi = new List<BE_GetKymcoList>();
            OtherRepository _repository = new OtherRepository(connetStr);

            string tSDate = "", tEDate = "";
            int tAuditMode = 0;

            tSDate = ExplodeSDate;
            tEDate = ExplodeEDate;
            tAuditMode = ExplodeAuditMode;

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("搜尋結果");

            if (ExplodeAuditMode==1)
            {
                string[] headerField = { "員工代號", "員工姓名", "區域", "種類", "車號", "維修方", "經銷商", "地址", "原因次分類", "車輛是否下線", "修改時間" };
                int headerFieldLen = headerField.Length;

                IRow header = sheet.CreateRow(0);
                for (int j = 0; j < headerFieldLen; j++)
                {
                    header.CreateCell(j).SetCellValue(headerField[j]);
                    sheet.AutoSizeColumn(j);
                }
                lstRawDataOfMachi = _repository.GetKymcoLists(tAuditMode, tSDate, tEDate);
                int len = lstRawDataOfMachi.Count;
                for (int k = 0; k < len; k++)
                {
                    IRow content = sheet.CreateRow(k + 1);
                    content.CreateCell(0).SetCellValue(lstRawDataOfMachi[k].UserID);
                    content.CreateCell(1).SetCellValue(lstRawDataOfMachi[k].UserName);
                    content.CreateCell(2).SetCellValue(lstRawDataOfMachi[k].Area);
                    content.CreateCell(3).SetCellValue(lstRawDataOfMachi[k].TypeK);
                    content.CreateCell(4).SetCellValue(lstRawDataOfMachi[k].CarNo);
                    content.CreateCell(7).SetCellValue(lstRawDataOfMachi[k].MaintainType);
                    content.CreateCell(5).SetCellValue(lstRawDataOfMachi[k].DealerCodeValue);
                    content.CreateCell(6).SetCellValue(lstRawDataOfMachi[k].MemoAddr);
                    content.CreateCell(8).SetCellValue(lstRawDataOfMachi[k].Reason);
                    content.CreateCell(9).SetCellValue(lstRawDataOfMachi[k].Offline);
                    content.CreateCell(10).SetCellValue(lstRawDataOfMachi[k].UpdTime);

                }
                for (int l = 0; l < headerFieldLen; l++)
                {
                    sheet.AutoSizeColumn(l);
                }
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
                    sheet.AutoSizeColumn(j);
                }
                lstRawDataOfMachi = _repository.GetKymcoLists(tAuditMode, tSDate, tEDate);
                int len = lstRawDataOfMachi.Count;
                for (int k = 0; k < len; k++)
                {
                    IRow content = sheet.CreateRow(k + 1);
                    content.CreateCell(0).SetCellValue(lstRawDataOfMachi[k].UserID);
                    content.CreateCell(1).SetCellValue(lstRawDataOfMachi[k].UserName);
                    content.CreateCell(2).SetCellValue(lstRawDataOfMachi[k].Area);
                    content.CreateCell(3).SetCellValue(lstRawDataOfMachi[k].TypeK);
                    content.CreateCell(4).SetCellValue(lstRawDataOfMachi[k].CarNo);
                    content.CreateCell(5).SetCellValue(lstRawDataOfMachi[k].DealerCodeValue);
                    content.CreateCell(6).SetCellValue(lstRawDataOfMachi[k].MemoAddr);
                    content.CreateCell(7).SetCellValue(lstRawDataOfMachi[k].UpdTime);

                }
                for (int l = 0; l < headerFieldLen; l++)
                {
                    sheet.AutoSizeColumn(l);
                }
                MemoryStream ms = new MemoryStream();
                workbook.Write(ms);
                // workbook.Close();
                return base.File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "光陽維護資料" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
            }

            
        }
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
            font_cell.FontName = "微軟正黑體";
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
                sheet.AutoSizeColumn(j);
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
            for (int l = 0; l < headerFieldLen; l++)
            {
                sheet.AutoSizeColumn(l);
            }
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            ms.Close();
            // workbook.Close();
            return base.File(ms.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ExplodeSendDate.Replace("-", "") + "_" + ExplodeCarNo + ".xlsx");
        }
        /// <summary>
        /// 會員審核明細報表 - 20210305唐加
        /// </summary>
        /// <returns></returns>
        public ActionResult MemberDetailQuery()
        {
            return View();
        }
        [HttpPost]
        public ActionResult MemberDetailQuery(string StartDate, string EndDate, string[] IDNOSuff, int AuditMode)
        {
            //ViewData["IDNOSuff"] = (Id == null) ? "" : string.Join(",", Id);
            List<BE_GetMemList> lstRawDataOfMachi = new List<BE_GetMemList>();//SP回傳的資料欄位
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


            string[] headerField = { "身分證字號", "審核人員", "員工編號", "審核人員群組", "審核日期", "審核結果", "不通過原因", "處理項目" };
            int headerFieldLen = headerField.Length;

            IRow header = sheet.CreateRow(0);
            for (int j = 0; j < headerFieldLen; j++)
            {
                header.CreateCell(j).SetCellValue(headerField[j]);
                sheet.AutoSizeColumn(j);
            }
            lstRawDataOfMachi = _repository.GetMemLists(tAuditMode, tSDate, tEDate, IDNoSuffCombind);
            int len = lstRawDataOfMachi.Count;
            for (int k = 0; k < len; k++)
            {
                IRow content = sheet.CreateRow(k + 1);
                content.CreateCell(0).SetCellValue(lstRawDataOfMachi[k].ID);
                content.CreateCell(1).SetCellValue(lstRawDataOfMachi[k].NAME);
                content.CreateCell(2).SetCellValue(lstRawDataOfMachi[k].HIID);
                content.CreateCell(3).SetCellValue(lstRawDataOfMachi[k].Group);
                content.CreateCell(4).SetCellValue(lstRawDataOfMachi[k].DATE);
                content.CreateCell(7).SetCellValue(lstRawDataOfMachi[k].ITEM);
                content.CreateCell(5).SetCellValue(lstRawDataOfMachi[k].TYPE);
                content.CreateCell(6).SetCellValue(lstRawDataOfMachi[k].REASON);

            }
            for (int l = 0; l < headerFieldLen; l++)
            {
                sheet.AutoSizeColumn(l);
            }
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            // workbook.Close();
            return base.File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "會員審核明細" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
        }

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

            List<BE_GetEasyWalletList> lstData = new MemberRepository(connetStr).GetEasyWalletList(IDNO);

            return View(lstData);

        }
    }
}