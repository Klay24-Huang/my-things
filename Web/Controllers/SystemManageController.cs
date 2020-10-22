using Domain.TB.BackEnd;
using Domain.WebAPI.Input.CENS;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
   
    /// <summary>
    /// 系統管理
    /// </summary>
    public class SystemManageController : Controller
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        public ActionResult MessageQuery()
        {
            return View();
        }
        /// <summary>
        /// 訊息記錄查詢
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MessageQuery(string OrderNo,string CarNo,string SendDate,string ShowType)
        {
            ViewData["CarNo"] = CarNo;
            ViewData["OrderNo"] = OrderNo;
            ViewData["SendDate"] = SendDate;
            ViewData["ShowType"] = ShowType;
            ViewData["errorLine"] = null;
            ViewData["IsShowMessage"] = null;
            List<BE_CarEventLog> lstCarEventLog = null; //車輛事件狀態
          
            List<BE_CardSettingData> lstCardSettingData = null; //卡號設定
            DateTime SDate = DateTime.Now, EDate = DateTime.Now,TmpSendDate=DateTime.Now;
            string errorLine = "";
            string errorMsg = "";
            bool flag = true;
            string errCode = "";
            Int64 tmpOrder = 0;
            int QueryMode = 0;//以訂單
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
                if (flag)
                {
                    ContactRepository contact = new ContactRepository(connetStr);
                    BE_GetOrderPartByMessageLogQuery obj = contact.GetOrderPartForMessageLogQuery(tmpOrder);
                    if (obj == null)
                    {
                        flag = false;
                        errorMsg = "找不到此訂單編號";
                    }
                    else
                    {
                        if(obj.final_start_time>new DateTime(1911, 1, 1, 0, 0, 0))
                        {
                            SDate = obj.final_start_time;
                        }
                        else
                        {
                            SDate = obj.start_time;
                        }
                        if (obj.final_stop_time > new DateTime(1911, 1, 1, 0, 0, 0))
                        {
                            EDate = obj.final_stop_time;
                        }
                        else
                        {
                            EDate = obj.stop_time;
                        }
                        CarNo = obj.CarNo;
                        ViewData["CarNo"] = CarNo;
                    }
                }

            }
            else
            {
                QueryMode = 1;
                flag = DateTime.TryParse(SendDate, out TmpSendDate);
                if (false == flag)
                {
                    errorMsg = "發送日期格式不符";
                }
                else
                {
                    SDate = TmpSendDate;
                    EDate = SDate.AddDays(1).AddSeconds(-1);
                }
            }


            if (flag)
            {
                ViewData["errorLine"] = "ok";
                ViewData["SDate"] = SDate.ToString("yyyy-MM-dd HH:mm:ss");
                ViewData["EDate"]= EDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                ViewData["errorMsg"] = errorMsg;
                ViewData["errorLine"] = errCode.ToString();
            }

            return View();
        }
        /// <summary>
        /// 車輛管理時間查詢
        /// </summary>
        /// <returns></returns>

        public ActionResult CarTimelineQuery()
        {
            return View();
        }
        [ChildActionOnly]
        public ActionResult GetCarEvent(string CarNo,string SDate,string EDate)
        {
            List<BE_CarEventLog> lstCarEventLogs = null; //讀卡
            CarCardCommonRepository repository = new CarCardCommonRepository(connetStr);
            lstCarEventLogs = repository.GetCarEventLogs(CarNo, SDate, EDate);
            return View(lstCarEventLogs);
        }
        [ChildActionOnly]
        public ActionResult GetReadCard(string CarNo, string SDate, string EDate)
        {
            List<BE_ReadCardLog> lstReadCarLog = null; //讀卡
            CarCardCommonRepository repository = new CarCardCommonRepository(connetStr);
            lstReadCarLog = repository.GetReadCardLogs(CarNo, SDate, EDate);
            return View(lstReadCarLog);
        }
        [ChildActionOnly]
        public ActionResult GetCardSettingData(string CarNo, string SDate, string EDate)
        {
            List<BE_CardSettingData> lstCarSettingLogs = null; //讀卡
            CarCardCommonRepository repository = new CarCardCommonRepository(connetStr);
            lstCarSettingLogs = repository.GetCardSettingLogs(CarNo, SDate, EDate);
            return View(lstCarSettingLogs);
            //return View();
        }
    }
}