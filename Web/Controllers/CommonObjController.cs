using Reposotory.Implement.BackEnd;
using System.Configuration;
using System.Web.Mvc;
using Domain.TB;

namespace Web.Controllers
{
    /// <summary>
    /// 共用元件
    /// </summary>
    public class CommonObjController : Controller
    {
        private StationRepository _repository;
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        /// <summary>
        /// 取出據點代碼及據點名稱
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult GetStationHiddenList()
        {
            _repository = new StationRepository(connetStr);
            bool showAll = false;
            var station = this._repository.GetPartOfStation(showAll);
            return View(station);
        }

    }
}