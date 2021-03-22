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
    public class ContactController : Controller
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        public ActionResult returnCarNew(string OrderNum,string p)
        {
            BE_OrderDataCombind obj = null;
            ContactRepository repository = new ContactRepository(connetStr);
            Int64 tmpOrder = 0;
            bool flag = true;

            //20210315 ADD BY ADAM REASON.合約參數改為AES加密
            string IDNO = "";
            if (p != "")
            {
                //base64解碼
                string KEY = ConfigurationManager.AppSettings["AES128KEY"].Trim();
                string IV = ConfigurationManager.AppSettings["AES128IV"].Trim();
                string ReqParam = AESEncrypt.DecryptAES128(p, KEY, IV);

                if (ReqParam != "")
                {
                    string[] parms = ReqParam.Split(new char[] { '&' });
                    for (int i=0;i<parms.Length;i++)
                    {
                        string[] txts = parms[i].Split(new char[] { '=' });
                        if (txts[0] == "OrderNum")
                        {
                            OrderNum = txts[1];
                        }
                        else if (txts[0] == "ID")
                        {
                            IDNO = txts[1];
                        }
                    }
                   
                }
            }

            if (string.IsNullOrEmpty(OrderNum))
            {
                flag = false;

            }
            else
            {
                //if (OrderNum != "")
                if (OrderNum != "" && IDNO != "")
                {

                    tmpOrder = Convert.ToInt64(OrderNum.Replace("H", ""));

                    //lstBook = _repository.GetBookingDetailNew(OrderNO);
                    //  lstNewBooking = _repository.GetBookingDetailHasImgNew(OrderNO);
                    obj = new BE_OrderDataCombind()
                    {
                        //Data = repository.GetOrderDetail(tmpOrder),
                        //20210315 ADD BY ADAM REASON.合約參數改為AES加密
                        Data = repository.GetOrderDetail(tmpOrder,IDNO),
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