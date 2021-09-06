using Domain.TB.BackEnd;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebView.Models.ViewModel;
using WebView.Repository;

namespace WebView.Controllers
{
    public class TogetherPassengerController : Controller
    {
        private string conn = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        
        public ActionResult InvitingStatus(string OrderNo, string IDNO, string StationID, string CarNo, string StartDate, string EndDate, string Mode)
        {
            VM_TogetherPassenger vm = new VM_TogetherPassenger();
            TogethePassengerRepository repository = new TogethePassengerRepository(conn);
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

            var bookingData = repository.GetBookingStatus(tmpOrder, IDNO, StationID, CarNo, StartDate, EndDate);
            if(bookingData.Count > 0)
            {
                vm.CarNo = bookingData[0].CarNo;
                vm.IDNO = bookingData[0].IDNO;
                vm.OrderNum = bookingData[0].OrderNum;
                vm.SD = bookingData[0].SD;
                vm.ED = bookingData[0].ED;
                vm.StationName = bookingData[0].StationName;
            }
            
            return View(vm);
        }
    }
}