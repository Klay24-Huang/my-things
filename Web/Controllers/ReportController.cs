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
            return View();
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
                content.CreateCell(8).SetCellValue((lstSubScription[k].MotoTotalHours * 60).ToString("f1"));   //機車

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
                content.CreateCell(5).SetCellValue((lstSubScription[k].UseMotoTotalHours * 60).ToString("f1"));   //機車
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
        /// <summary>
        /// 代收停車費明細
        /// </summary>
        /// <returns></returns>
        public ActionResult ChargeParkingDetailQuery()
        {
            return View();
        }
    }
}