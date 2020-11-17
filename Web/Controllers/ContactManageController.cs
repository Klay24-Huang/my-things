using Domain.TB.BackEnd;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
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
                        ReturnCarImage = repository.GetOrdeCarImage(tmpOrder, 1,false)
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
                        ReturnCarImage = repository.GetOrdeCarImage(tmpOrder, 1,false)
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