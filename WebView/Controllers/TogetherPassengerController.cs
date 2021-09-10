using Domain.TB.BackEnd;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebCommon;
using WebView.Models.Param.TogetherPassenger;
using WebView.Models.ViewModel;
using WebView.Repository;
using NLog;

namespace WebView.Controllers
{
    public class TogetherPassengerController : Controller
    {
        private string conn = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public ActionResult InvitingStatus()
        {
            TogethePassengerRepository repository = new TogethePassengerRepository(conn);
            string OrderNo = "";
            string inviteeId = "";
            string inviterId = "";
            string MEMCNAME = "";
            string MEMTEL = "";
            string Station = "";
            string CarNo = "";
            string StartDate = "";
            string EndDate = "";

            var encryptString = Request.Url.Query.Substring(1);
            encryptString = HttpUtility.UrlDecode(encryptString);
            //var encryptString = "KtwBd/MrrFcEG2SMxUTqQ0t1FcXedyF/dtqaNMyHAztEtOIrdP6MIbG/zR6l6ymy";
            logger.Info($"傳入Url參數:{encryptString}");
            //base64解碼
            string KEY = ConfigurationManager.AppSettings["AES128KEY"].Trim();
            string IV = ConfigurationManager.AppSettings["AES128IV"].Trim();
            string ReqParam = AESEncrypt.DecryptAES128(encryptString, KEY, IV);
            
            //拆解字串
            if (ReqParam != "")
            {
                string[] parms = ReqParam.Split(new char[] { '&' });
                for (int i = 0; i < parms.Length; i++)
                {
                    string[] txts = parms[i].Split(new char[] { '=' });
                    if (txts[0] == "OrderNo")
                    {
                        OrderNo = txts[1];
                    }
                    else if (txts[0] == "InviteeId")
                    {
                        inviteeId = txts[1];
                    }
                }
            }

            logger.Info($"傳入參數:{OrderNo} & {inviteeId}");

            //測試用
            //OrderNo = "12213028";
            //inviteeId = "G221795414";

            TempData["inviteeId"] = inviteeId;

            ViewData["chkType"] = repository.CheckInvitingStatus(OrderNo, inviteeId);
            logger.Info("受邀人狀態確認成功");

            var orderInfo = repository.GetOrderInfo(OrderNo)[0];
            logger.Info("訂單資料獲取成功");
            inviterId = orderInfo.IDNO;
            MEMCNAME = orderInfo.MEMCNAME;
            MEMTEL = orderInfo.MEMTEL;

            
            var bookingData = repository.GetBookingStatus(OrderNo, inviterId, Station, CarNo, StartDate, EndDate);
            logger.Info("預約資料獲取成功");


            if (bookingData.Count > 0)
            {
                CarNo = bookingData[0].CarNo;
                StartDate = bookingData[0].SD.ToString();
                EndDate = bookingData[0].ED.ToString();
                Station = bookingData[0].StationName;
            }

            TempData["OrderNo"] = OrderNo;
            TempData["MEMCNAME"] = MEMCNAME;
            TempData["MEMTEL"] = MEMTEL;
            TempData["StartDate"] = StartDate;
            TempData["EndDate"] = EndDate;
            TempData["CarNo"] = CarNo;
            TempData["Station"] = Station;


            //合約用加密字串
            var ContractReqParam = $"OrderNum={OrderNo}&ID={inviterId}";
            var EncryptString = AESEncrypt.EncryptAES128(ContractReqParam, KEY, IV);
            EncryptString = HttpUtility.UrlEncode(EncryptString);
            ViewData["EncryptString"] = EncryptString;

            return View();
        }

        [HttpPost]
        public ActionResult InvitingStatus(string OrderNo, string Accept)
        {
            //初始設定
            TogethePassengerRepository repository = new TogethePassengerRepository(conn);
            string inviteeId = TempData["inviteeId"].ToString();
            string inviterId = "";
            string MEMCNAME = "";
            string MEMTEL = "";
            string Station = "";
            string CarNo = "";
            string StartDate = "";
            string EndDate = "";

            //結果儲存至Table
            var FeedbackType = "N";
            if(Accept == "true")
            {
                FeedbackType = "Y";
            }

            string KEY = ConfigurationManager.AppSettings["AES128KEY"].Trim();
            string IV = ConfigurationManager.AppSettings["AES128IV"].Trim();
            var ReqParam = $"OrderNo=H{OrderNo}&InviteeId={inviteeId}&FeedbackType={FeedbackType}";
            var EncryptString = AESEncrypt.EncryptAES128(ReqParam, KEY, IV);
            EncryptString = HttpUtility.UrlEncode(EncryptString);

            var result = repository.SaveInviteeResponse(EncryptString);
            logger.Info($"圈存結果 : {result.ErrorMessage}");

            if (result.Result == "1")
            {

                //獲取頁面顯示資訊
                ViewData["chkType"] = repository.CheckInvitingStatus(OrderNo, inviteeId);

                var orderInfo = repository.GetOrderInfo(OrderNo)[0];
                inviterId = orderInfo.IDNO;
                MEMCNAME = orderInfo.MEMCNAME;
                MEMTEL = orderInfo.MEMTEL;

                var bookingData = repository.GetBookingStatus(OrderNo, inviterId, Station, CarNo, StartDate, EndDate);
                if (bookingData.Count > 0)
                {
                    CarNo = bookingData[0].CarNo;
                    StartDate = bookingData[0].SD.ToString();
                    EndDate = bookingData[0].ED.ToString();
                    Station = bookingData[0].StationName;
                }

                TempData["OrderNo"] = OrderNo;
                TempData["MEMCNAME"] = MEMCNAME;
                TempData["MEMTEL"] = MEMTEL;
                TempData["StartDate"] = StartDate;
                TempData["EndDate"] = EndDate;
                TempData["CarNo"] = CarNo;
                TempData["Station"] = Station;

                return View();
            }
            logger.Info("請求失敗");

            return RedirectToAction("ErrorPage", "Home", new { ErrorMessage = result.ErrorMessage});
        }
    }
}