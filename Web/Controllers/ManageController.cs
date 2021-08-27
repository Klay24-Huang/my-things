using Domain.TB.BackEnd;
using Domain.WebAPI.output.HiEasyRentAPI;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Output.PartOfParam;

namespace Web.Controllers
{
    /// <summary>
    /// 後台管理
    /// </summary>
    public class ManageController : Controller
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 強制刷卡取款
        /// </summary>
        /// <returns></returns>
        public ActionResult AuthAndPay()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AuthAndPay(string IDNO)
        {
            ViewData["IDNO"] = IDNO;
            ViewData["errorLine"] = null;
            ViewData["IsShowMessage"] = null;
            string errorLine = "";
            string errorMsg = "";
            bool flag = true;
            string errCode = "";
            List<WebAPIOutput_NPR330QueryData> lstData = null;
            CommonFunc baseVerify = new CommonFunc();

            if (IDNO != "")
            {
                flag = baseVerify.checkIDNO(IDNO);
                if (flag == false)
                {
                    errCode = "ERR103";
                    errorMsg = baseVerify.GetErrorMsg(errCode);
                }
            }
            if (flag)
            {
                HiEasyRentAPI api = new HiEasyRentAPI();
                WebAPIOutput_ArrearQuery WebAPIOutput = null;
                flag = api.NPR330Query(IDNO, ref WebAPIOutput);
                if (flag)
                {
                    if (WebAPIOutput.Result)
                    {
                        if (WebAPIOutput.RtnCode == "0")
                        {
                            lstData = new List<WebAPIOutput_NPR330QueryData>();
                            lstData = WebAPIOutput.Data.ToList();
                        }
                        else
                        {
                            errCode = "ERR";
                            errorMsg = WebAPIOutput.Message;
                            flag = false;
                        }

                    }
                    else
                    {
                        errCode = "ERR";
                        errorMsg = WebAPIOutput.Message;
                        flag = false;
                    }
                }
                else
                {
                    flag = false;
                    errCode = "ERR301"; 
                }
            }
            if (flag)
            {
                ViewData["errorLine"] = "ok";

            }
            else
            {
                ViewData["errorMsg"] = errorMsg;
                ViewData["errorLine"] = errCode.ToString();
            }
            return View(lstData);
        }
        /// <summary>
        /// 欠費查詢
        /// </summary>
        /// <returns></returns>
        public ActionResult ArrearQuery()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ArrearQuery(string IDNO)
        {
            ViewData["IDNO"] = IDNO;
            ViewData["errorLine"] = null;
            ViewData["IsShowMessage"] = null;
            string errorLine = "";
            string errorMsg = "";
            bool flag = true;
            string errCode = "";
            List<WebAPIOutput_NPR330QueryData> lstData = null;
            CommonFunc baseVerify = new CommonFunc();

            if (IDNO != "")
            {
                flag = baseVerify.checkIDNO(IDNO);
                if (flag==false)
                {
                    errCode = "ERR103";
                    errorMsg = baseVerify.GetErrorMsg(errCode);
                }
            }
            if (flag)
            {
                HiEasyRentAPI api = new HiEasyRentAPI();
                WebAPIOutput_ArrearQuery WebAPIOutput = null;
                flag = api.NPR330Query(IDNO, ref WebAPIOutput);
                if (flag)
                {
                    if (WebAPIOutput.Result)
                    {
                        if (WebAPIOutput.RtnCode == "0")
                        {
                            lstData = new List<WebAPIOutput_NPR330QueryData>();
                            lstData = WebAPIOutput.Data.ToList();
                        }
                        else
                        {
                            errCode = "ERR";
                            errorMsg = WebAPIOutput.Message;
                            flag = false;
                        }
                    }
                    else
                    {
                        errCode = "ERR";
                        errorMsg = WebAPIOutput.Message;
                        flag = false;
                    }
                }
                else
                {
                    flag = false;
                    errCode = "ERR301"; //贈送查詢失敗，請稍候再試
                }
            }
            if (flag)
            {
                ViewData["errorLine"] = "ok";
            }
            else
            {
                ViewData["errorMsg"] = errorMsg;
                ViewData["errorLine"] = errCode.ToString();
            }
            return View(lstData);
        }
        /// <summary>
        /// 平假日維護
        /// </summary>
        /// <returns></returns>
        public ActionResult HoildayMaintain()
        {
            return View();
        }
        [HttpPost]
        public ActionResult HoildayMaintain(int HoildayYear,int Season)
        {
            ViewData["HoildayYear"] = HoildayYear;
            ViewData["Season"] = Season;
            return View();
        }
        public ActionResult InsCleanOrder()
        {
            return View();
        }
        ///// <summary>
        ///// 卡號解除
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult UnBindCard()
        //{
        //    return View();
        //}
        /// <summary>
        /// 信用卡解綁
        /// </summary>
        /// <returns></returns>
        public ActionResult UnBindCreditCard()
        {
            return View();
        }
        /// <summary>
        /// 短租補傳
        /// </summary>
        /// <returns></returns>
        public ActionResult ResendToHieasyrent()
        {
            return View();
        }

        /// <summary>
        /// 查詢APILog
        /// </summary>
        /// <returns></returns>
        public ActionResult APILogQuery()
        {
            return View();
        }
        [HttpPost]
        public ActionResult APILogQuery(string IPAddress, string StartDate, string EndDate)
        {
            ViewData["IPAddress"] = IPAddress;
            ViewData["StartDate"] = StartDate;
            ViewData["EndDate"] = EndDate;
            List<BE_APILog> lstData = null;
            APILogRepository repository = new APILogRepository(connetStr);
            lstData = repository.GetAPILog(IPAddress, StartDate+" 00:00:00.000", EndDate + " 23:59:59.999");
            return View(lstData);
        }

        /// <summary>
        /// 查詢APILog
        /// </summary>
        /// <returns></returns>
        public ActionResult RealtimeSale()
        {
            List<BE_RealtimeSale> lstData = null;
            APILogRepository repository = new APILogRepository(connetStr);
            lstData = repository.GetRealtimeSale("");
            return View(lstData);
        }
        [HttpPost]
        public ActionResult RealtimeSale( string StartDate)
        {
            ViewData["StartDate"] = StartDate;
            List<BE_RealtimeSale> lstData = null;
            APILogRepository repository = new APILogRepository(connetStr);
            lstData = repository.GetRealtimeSale(StartDate.Replace("-",""));
            return View(lstData);
        }
    }
}