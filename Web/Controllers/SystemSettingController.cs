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
    public class SystemSettingController : Controller
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        public ActionResult FeedBackKindSetting()
        {
            return View();
        }
        [HttpPost]
        public ActionResult FeedBackKindSetting(string descript,int star,int ShowType)
        {
            SystemSettingRepository repository = new SystemSettingRepository(connetStr);
            ViewData["descript"] = descript;
            ViewData["star"] = star;
            ViewData["ShowType"] = ShowType;
            List<BE_FeedBackKindData> lstData = new List<BE_FeedBackKindData>();
            lstData = repository.GetFeedBackKind( descript,  star,  ShowType);
            return View(lstData);
        }
    }
}