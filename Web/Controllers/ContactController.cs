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
    public class ContactController : Controller
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        public ActionResult returnCarNew(string OrderNum)
        {
            BE_OrderDataCombind obj = null;
            ContactRepository repository = new ContactRepository(connetStr);
            Int64 tmpOrder = 0;
            bool flag = true;
            if (string.IsNullOrEmpty(OrderNum))
            {
                flag = false;

            }
            else
            {
                if (OrderNum != "")
                {

                    tmpOrder = Convert.ToInt64(OrderNum.Replace("H", ""));

                    //lstBook = _repository.GetBookingDetailNew(OrderNO);
                    //  lstNewBooking = _repository.GetBookingDetailHasImgNew(OrderNO);
                    obj = new BE_OrderDataCombind()
                    {
                        Data = repository.GetOrderDetail(tmpOrder),
                        PickCarImage = repository.GetOrdeCarImage(tmpOrder, 0, false),
                        ReturnCarImage = repository.GetOrdeCarImage(tmpOrder, 1, false)
                    };

                    List<BE_AuditImage> lstAudits = new MemberRepository(connetStr).GetAuditImage(obj.Data.IDNO);
                    for(int i=0;i< lstAudits.Count; i++)
                    {
                        if (lstAudits[i].CrentialsType == 11)
                        {
                            obj.CredentialImage = lstAudits[i];
                        }
                        if (lstAudits[i].AlreadyType == 11)
                        {
                            obj.CredentialImage = lstAudits[i];
                        }
                    }
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