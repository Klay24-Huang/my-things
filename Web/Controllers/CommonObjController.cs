using Reposotory.Implement.BackEnd;
using System.Configuration;
using System.Web.Mvc;
using Domain.TB;
using System.Collections.Generic;
using Domain.Common.BackEnd;
using Microsoft.Ajax.Utilities;
using Reposotory.Implement;
using Domain.TB.BackEnd;

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
        [ChildActionOnly]
        public ActionResult GetCarHiddenList()
        {
            _repository = new StationRepository(connetStr);
            bool showAll = false;
            var car = this._repository.GetPartOfCar(showAll);
            return View(car);
        }
        /// <summary>
        /// 產出共用的處理項目下拉式
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult GetHandleList()
        {
            string[] itemValue = { "","新增","修改"};
            string[] itemText = { "","Add","Edit"};
            List<OperatorItem> list = new List<OperatorItem>();
            int Len = itemValue.Length;
            for(int i = 0; i < Len; i++)
            {
                OperatorItem item = new OperatorItem()
                {
                    OptText = itemText[i],
                    OptValue = itemValue[i]
                };
                list.Add(item);
            }
           
            return View(list);
        }
        [ChildActionOnly]
        public ActionResult GetCarMachineUnBindList(int SEQNO)
        {
            CarStatusCommon CarRepository = new CarStatusCommon(connetStr);
            bool showAll = false;
            ViewData["Operator"] = SEQNO;
            var MachineNoList = CarRepository.GetCarMachineUnBind();
            
            return View(MachineNoList);
        }
        [ChildActionOnly]
        public ActionResult GetOperatorList(int SEQNO)
        {
            List<BE_Operator> lstOperators = new OperatorRepository(connetStr).GetOperatorsALL();
            bool showAll = false;
        
            ViewData["Operator"] = SEQNO;
            return View(lstOperators);
        }
        [ChildActionOnly]
        public ActionResult GetPowerList()
        {
            List<BE_MenuCombindConsistPower> lstData = new CommonRepository(connetStr).GetMenuListConsistPower();
            return View(lstData);
        }
        [ChildActionOnly]
        public ActionResult GetFuncGroup(int SEQNO)
        {
            ViewData["FuncGroup"] = SEQNO;
            List<BE_GetFuncGroup> lstData = new AccountManageRepository(connetStr).GetFuncGroup("", "", "", "");
            return View(lstData);
        }

    }
}