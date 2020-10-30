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
    /// <summary>
    /// 地圖監控
    /// </summary>
    public class MonitorController : Controller
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 地圖監控
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(int? Mode, string OrderNum, DateTime? start, DateTime? end, string objCar)
        {
            Int16 sMode = 2;
            List<BE_EvTimeLine> lstEv = null;
            if (Mode != null)
            {
                sMode = Convert.ToInt16(Mode);
                ViewData["Mode"] = sMode;
            }
            if (sMode < 2)
            {
                Int64 tmpOrder = (string.IsNullOrEmpty(OrderNum)) ? 0 : Convert.ToInt64(OrderNum.Replace("H", ""));
                string SD = (start == null) ? "" : Convert.ToDateTime(start).ToString("yyyy-MM-dd HH:mm:ss");
                string ED = (end == null) ? "" : Convert.ToDateTime(end).ToString("yyyy-MM-dd HH:mm:ss");
                string CarNo = (string.IsNullOrEmpty(objCar)) ? "" : objCar;
                EventHandleRepository _respository = new EventHandleRepository(connetStr);

                switch (sMode)
                {
                    case 0:
                        if (tmpOrder > 0)
                        {
                            ViewData["OrderNum"] = tmpOrder;

                            lstEv = _respository.GetMapDataByTimeLine(tmpOrder);
                        }
                        break;
                    case 1:
                        ViewData["CarNo"] = CarNo;
                        ViewData["SD"] = SD;
                        ViewData["ED"] = ED;
                        lstEv = _respository.GetMapDataByTimeLine(objCar, SD, ED);
                        break;
                }
            }


            return View(lstEv);
        }
    }
}