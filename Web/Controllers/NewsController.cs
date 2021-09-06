using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCommon;
using Domain.TB.BackEnd;
using Web.Models.Params.Search.Input;

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

        //20210315唐加banner設定
        public ActionResult BannerSet()
        {
            return View();
        }
        [HttpPost]
        public ActionResult BannerSet(string Banner, string Order, FormCollection collection)
        {
            ViewData["Banner"] = Banner;
            ViewData["Order"] = Order;
            ViewData["terms"] = collection["terms"];

            Input_Banner QueryData = null;
            NewsRepository repository = new NewsRepository(connetStr);
            List<BE_GetBannerInfo> lstData = new List<BE_GetBannerInfo>();
            if (collection["queryData"] != null)
            {
                
                QueryData = Newtonsoft.Json.JsonConvert.DeserializeObject<Input_Banner>(collection["queryData"].ToString());
                if (QueryData != null)
                {
                    lstData = repository.GetBannerInfo(Banner, Order, QueryData.Terms);
                }
            }
            return View(lstData);
        }
        public ActionResult BannerInfoAdd()
        {
            return View();
        }
        public ActionResult BannerInfoMaintain()
        {
            return View();
        }
        [HttpPost]
        public ActionResult BannerInfoMaintain(string MaintainBanner)//卡我這麼久，幹，結果是參數命名要和html/js一樣啦，白癡耶，靠北，吼~~~~~~~~~~~~~~~~~~~~
        {
            BE_BannerDetailCombind Data = null;
            NewsRepository repository = new NewsRepository(connetStr);
            Data = new BE_BannerDetailCombind()
            {
                detail = repository.GetBannerInfo2(MaintainBanner)
            };
            return View(Data);
        }
    }
}