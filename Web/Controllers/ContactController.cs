using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class ContactController : Controller
    {
        public ActionResult returnCarNew(string OrderNum)
        {
            //CreateContractNew obj = new CreateContractNew();
            //ContractOnlyRepository _repository = new ContractOnlyRepository(MvcApplication.connetStr);
            //if (!string.IsNullOrEmpty(OrderNum))
            //{
            //    List<CreateContractNew> lstObj = _repository.GetContractData(OrderNum.Replace("H", ""));
            //    if (lstObj != null)
            //    {
            //        if (lstObj.Count > 0)
            //        {
            //            obj = lstObj[0];
            //        }
            //    }
            //}
            return View();
        }
    }
}