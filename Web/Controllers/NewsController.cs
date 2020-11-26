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
    public class NewsController : Controller
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        // GET: News
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult NewsManage(int? NewsID)
        {
            int NewsId = (NewsID == null) ? 0 : Convert.ToInt32(NewsID);
            List<Domain.TB.News> lstNews = null;
            if (NewsId > 0)
            {
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                NewsRepository _repository = new NewsRepository(connetStr);
                lstNews = _repository.GetNewsList(NewsId, ref lstError);
            }
   

            return View(lstNews);
        }
        public ActionResult NewsList()
        {
            List<Domain.TB.News> lstNews = null;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            NewsRepository _repository = new NewsRepository(connetStr);
            lstNews = _repository.GetNewsList(ref lstError);
            return View(lstNews);
        }
    }
}