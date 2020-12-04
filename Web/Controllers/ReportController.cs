using Domain.TB.BackEnd;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        /// 月租報表
        /// </summary>
        /// <returns></returns>
        public ActionResult MonthlyDetailQuery()
        {
            return View();
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