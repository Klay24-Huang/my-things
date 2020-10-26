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
    /// 會員管理
    /// </summary>
    public class MemberManageController : Controller
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 會員審核
        /// </summary>
        /// <returns></returns>
        public ActionResult Audit()
        {
            return View();
        }
        public ActionResult AuditDetail(string IDNO)
        {
            return View();
        }
        public ActionResult AuditHistory(string IDNO)
        {
            return View();
        }
        public ActionResult CredentialsView(string IDNO)
        {
            return View();
        }
        /// <summary>
        /// 手機重複清單
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewSameMobile()
        {
            MemberRepository repository = new MemberRepository(connetStr);
            List<BE_SameMobileData> lstData = repository.GetSameMobile();
            return View(lstData);
        }
    }
}