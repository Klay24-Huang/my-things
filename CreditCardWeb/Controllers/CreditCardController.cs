using Newtonsoft.Json;
using NLog;
using Prometheus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CreditCardWeb.Controllers
{
    public class CreditCardController : Controller
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly Gauge BindCardSuccessCount = Metrics.CreateGauge("irent_taishin_bindcard_success", "iRent Taishin BindCard Success", new GaugeConfiguration
        {
            LabelNames = new[] { "ts" }
        });
        private static readonly Gauge BindCardFailCount = Metrics.CreateGauge("irent_taishin_bindcard_fail", "iRent Taishin BindCard Fail", new GaugeConfiguration
        {
            LabelNames = new[] { "ts", "BindRetCode" }
        });
        // GET: CreditCard
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 綁定卡片成功
        /// </summary>
        //[HttpGet]
        //public ActionResult BindSuccess()
        //{
        //    return View();
        //}
        //[HttpPost]
        public ActionResult BindSuccess(Dictionary<string, object> value)
        {
            logger.Trace("BindSuccess:" + JsonConvert.SerializeObject(value));
            BindCardSuccessCount.WithLabels(DateTime.Now.Ticks.ToString()).Set(1);
            //string LogPath = "~/Content/CreditCardBindLog";
            //string Log = collection.ToString();
            //DirectoryInfo di = new DirectoryInfo(Server.MapPath(LogPath));
            //if (!di.Exists)
            //{
            //    di.Create();
            //}
            //string data = Log;
            //string filename = string.Format("{0:yyyyMMdd}.txt", System.DateTime.Today);

            //System.IO.File.AppendAllText(LogPath + filename, data);
            return View();
        }
        /// <summary>
        /// 綁定卡片失敗
        /// </summary>
        public ActionResult BindFail(string id,string BindRetCode)
        {
            logger.Trace("BindFail: id=" + id+ ", BindRetCode=" + BindRetCode);
            BindCardFailCount.WithLabels(DateTime.Now.Ticks.ToString(), BindRetCode).Set(1);
            string errorMsg = "";
            switch (BindRetCode)
            {
                case "9001":
                    errorMsg = "系統資訊有誤，請確認後再重新操作，如果仍無法排除，請與APP客服聯繫";
                    break;
                case "9002":
                    errorMsg = "您輸入的資訊有誤，請確認後再重新操作，如果仍無法排除，請與發卡銀行聯繫";
                    break;
                case "9004":
                    errorMsg = "您使用的信用卡已過期，請確認後再重新操作";
                    break;
                case "9010":
                    errorMsg = "您已取消3D驗證，請確認後再重新操作";
                    break;
                case "9030":
                    errorMsg = "該卡片(帳號)已被綁定，請確認後再重新操作，或與APP客服聯繫";
                    break;
            }

            ViewData["errorMsg"] = errorMsg;
            return View();
        }
    }
}