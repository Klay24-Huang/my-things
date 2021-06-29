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
        public ActionResult GetUserGroupList(int SEQNO,int OperatorID)
        {

            List<BE_UserGroup> lstUserGroup = null;
            if (OperatorID > 0)
            {
                lstUserGroup = new AccountManageRepository(connetStr).GetUserGroup("", "", OperatorID, "", "",0);
            }
            else
            {
                lstUserGroup = new AccountManageRepository(connetStr).GetUserGroup("", "", 0, "", "",0);
            }
           
            bool showAll = false;

            ViewData["UserGroup"] = SEQNO;
            return View(lstUserGroup);
        }
        //[ChildActionOnly]
        //public ActionResult GetUserGroupList(int SEQNO)
        //{
        //    List<BE_UserGroup> lstUserGroup = new AccountManageRepository(connetStr).GetUserGroup("","",0,"","");
        //    bool showAll = false;

        //    ViewData["UserGroup"] = SEQNO;
        //    return View(lstUserGroup);
        //}
        [ChildActionOnly]
        public ActionResult GetPowerList()
        {
            List<BE_MenuCombindConsistPower> lstData = new CommonRepository(connetStr).GetMenuListConsistPower();
            return View(lstData);
        }
        [ChildActionOnly]
        public ActionResult GetDisablePowerList()
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


        [ChildActionOnly]
        public ActionResult GetSCITEM(string SEQNO)
        {
            List<BE_SCITEM> lstOperators = new MemberScore(connetStr).GetSCITEM();
            //bool showAll = false;
            ViewData["_SCITEM"] = SEQNO;
            return View(lstOperators);
        }
        [ChildActionOnly]
        public ActionResult GetSCMITEM(string scitem, string scmitem)
        {
            List<BE_SCMITEM> lstUserGroup = null;
            lstUserGroup = new MemberScore(connetStr).GetSCMITEM(scitem);
            //bool showAll = false;
            ViewData["_UserGroup"] = scmitem;
            return View(lstUserGroup);
        }
        [ChildActionOnly]
        public ActionResult GetScore(string scmitem)
        {
            List<BE_MemScore> lstScore = new MemberScore(connetStr).GetScore(scmitem);
            return View(lstScore);
        }
    }
}