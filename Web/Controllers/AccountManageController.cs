using Domain.SP.BE.Input;
using Domain.SP.Output;
using Domain.TB.BackEnd;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using Web.Models.Enum;
using WebAPI.Models.BaseFunc;
using WebCommon;

namespace Web.Controllers
{
    public class AccountManageController : Controller
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 加盟業者維護
        /// </summary>
        /// <returns></returns>
        public ActionResult FranchiseesMaintain()
        {
            return View();
        }
        [HttpPost]
        public ActionResult FranchiseesMaintain(string ddlObj,string Operator,string OperatorName,string StartDate,string EndDate, HttpPostedFileBase fileImport)
        {
           
            string errorLine = "";
            string errorMsg = "";
            string Mode = ddlObj;
            string FileName = "";
            bool flag = true;
            string errCode = "";
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string UserId = ((Session["Account"] == null) ? "" : Session["Account"].ToString());
            ViewData["Mode"] = ddlObj;
            ViewData["Operator"] = Operator;
            ViewData["OperatorName"] = OperatorName;
            ViewData["StartDate"] = StartDate;
            ViewData["EndDate"] = EndDate;
            ViewData["errorLine"] = null;
            ViewData["IsShowMessage"] = null;
            if (Mode == "Add")
            {
                CommonFunc baseVerify = new CommonFunc();
                flag = baseVerify.checkUniNum(Operator);
                if (flag==false) {
                    errorMsg = "統一編號不正確";
                }
                if (flag)
                {

                }
                if (flag)
                {
                    if (fileImport != null)
                    {
                        flag = new AzureStorageHandle().UploadFileToAzureStorage(fileImport, "operatoricon");
                        FileName = fileImport.FileName;
                    }
                    else
                    {
                        flag = false;
                        errorMsg = "請上傳檔案";
                    }
                }
                if (flag)
                {
                    string spName = new ObjType().GetSPName(ObjType.SPType.InsOperator);
                    SPInput_BE_InsOperator spInput = new SPInput_BE_InsOperator()
                    {
       
                        UserID = UserId,
                        EndDate = Convert.ToDateTime(EndDate+" 23:59:59"),
                        StartDate = Convert.ToDateTime(StartDate+ " 00:00:00"),
                        OperatorName = OperatorName,
                        OperatorAccount = Operator,
                        OperatorICon = FileName

                    };
                    SPOutput_Base spOut = new SPOutput_Base();
                    SQLHelper<SPInput_BE_InsOperator, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_BE_InsOperator, SPOutput_Base>(connetStr);
                    flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                    baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                    if (false == flag)
                    {
                        errorMsg = baseVerify.GetErrorMsg(errCode);
                    }
                }
                if (flag)
                {
                    ViewData["errorLine"] = "ok";
                }
                else
                {
                    ViewData["errorMsg"] = errorMsg;
                    ViewData["errorLine"] = errorLine.ToString();
                }
                return View();
            }
            else
            {
                List<BE_Operator> lstOperators = new OperatorRepository(connetStr).GetOperators(Operator, OperatorName, StartDate, EndDate);
                return View(lstOperators);
            }
         
     
           
        }
        /// <summary>
        /// 功能群組維護
        /// </summary>
        /// <returns></returns>
        public ActionResult FuncGroupMaintain()
        {
            return View();
        }
        [HttpPost]
        public ActionResult FuncGroupMaintain(string ddlObj,string FuncGroupID,string FuncGroupName,string StartDate,string EndDate)
        {
            string errorLine = "";
            string errorMsg = "";
            string Mode = ddlObj;
            bool flag = true;
            string errCode = "";
            AccountManageRepository repository = new AccountManageRepository(connetStr);
            List<BE_GetFuncGroup> lstData = null;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string UserId = ((Session["Account"] == null) ? "" : Session["Account"].ToString());
            ViewData["Mode"] = ddlObj;
            ViewData["FuncGroupID"] = FuncGroupID;
            ViewData["FuncGroupName"] = FuncGroupName;
            ViewData["StartDate"] = StartDate;
            ViewData["EndDate"] = EndDate;
            ViewData["errorLine"] = null;
            ViewData["IsShowMessage"] = null;
            if (Mode == "Add")
            {
                CommonFunc baseVerify = new CommonFunc();
                string spName = new ObjType().GetSPName(ObjType.SPType.InsFuncGroup);
                SPInput_BE_InsFuncGroup spInput = new SPInput_BE_InsFuncGroup()
                {

                    UserID = UserId,
                    EndDate = Convert.ToDateTime(EndDate + " 23:59:59"),
                    StartDate = Convert.ToDateTime(StartDate + " 00:00:00"),
                     FuncGroupID=FuncGroupID,
                      FuncGroupName=FuncGroupName

                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_BE_InsFuncGroup, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_BE_InsFuncGroup, SPOutput_Base>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (false == flag)
                {
                    errorMsg = baseVerify.GetErrorMsg(errCode);
                }
            
                if (flag)
                {
                    ViewData["errorLine"] = "ok";
                    lstData = repository.GetFuncGroup("","","","");
                }
                else
                {
                    ViewData["errorMsg"] = errorMsg;
                    ViewData["errorLine"] = errorLine.ToString();
                }
               
            }
            else
            {
                if (!string.IsNullOrEmpty(StartDate))
                {
                    StartDate = StartDate + " 00:00:00";
                }
                if (!string.IsNullOrEmpty(EndDate))
                {
                    EndDate = EndDate + " 23:59:59";
                }
                lstData = repository.GetFuncGroup(FuncGroupID, FuncGroupName, StartDate, EndDate);
            }
            return View(lstData);

        }
        /// <summary>
        /// 功能維護
        /// </summary>
        /// <returns></returns>
        public ActionResult FuncMaintain()
        {
            return View();
        }
        /// <summary>
        /// 使用者群組維護
        /// </summary>
        /// <returns></returns>
        public ActionResult UserGroupMaintain()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UserGroupMaintain(string ddlObj, string ddlOperator,string ddlFuncGroup, string UserGroupID, string UserGroupName, string StartDate, string EndDate)
        {
            string errorLine = "";
            string errorMsg = "";
            string Mode = ddlObj;
            int OperatorID = (ddlOperator == "") ? 0 : Convert.ToInt32(ddlOperator);
            int FuncGroupID = (ddlFuncGroup == "") ? 0 : Convert.ToInt32(ddlFuncGroup);
            bool flag = true;
            string errCode = "";
            AccountManageRepository repository = new AccountManageRepository(connetStr);
            List<BE_UserGroup> lstData = null;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string UserId = ((Session["Account"] == null) ? "" : Session["Account"].ToString());
            ViewData["Mode"] = ddlObj;
            ViewData["UserGroupID"] = UserGroupID;
            ViewData["UserGroupName"] = UserGroupName;
            ViewData["StartDate"] = StartDate;
            ViewData["EndDate"] = EndDate;
            ViewData["OperatorID"] = OperatorID;
            ViewData["errorLine"] = null;
            ViewData["IsShowMessage"] = null;
            ViewData["FuncGroupID"] = FuncGroupID;
            if (Mode == "Add")
            {
                CommonFunc baseVerify = new CommonFunc();
                string spName = new ObjType().GetSPName(ObjType.SPType.InsUserGroup);
                SPInput_BE_InsUserGroup spInput = new SPInput_BE_InsUserGroup()
                {

                    UserID = UserId,
                    EndDate = Convert.ToDateTime(EndDate + " 23:59:59"),
                    StartDate = Convert.ToDateTime(StartDate + " 00:00:00"),
                    LogID=0,
                     OperatorID=OperatorID,
                      UserGroupID=UserGroupID,
                       UserGroupName=UserGroupName,
                        FuncGroupID= FuncGroupID

                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_BE_InsUserGroup, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_BE_InsUserGroup, SPOutput_Base>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (false == flag)
                {
                    errorMsg = baseVerify.GetErrorMsg(errCode);
                }

                if (flag)
                {
                    ViewData["errorLine"] = "ok";
                    lstData = repository.GetUserGroup("", "",0, "", "",0);
                }
                else
                {
                    ViewData["errorMsg"] = errorMsg;
                    ViewData["errorLine"] = errorLine.ToString();
                }

            }
            else
            {
                if (!string.IsNullOrEmpty(StartDate))
                {
                    StartDate = StartDate + " 00:00:00";
                }
                if (!string.IsNullOrEmpty(EndDate))
                {
                    EndDate = EndDate + " 23:59:59";
                }
                lstData = repository.GetUserGroup(UserGroupID, UserGroupName,OperatorID, StartDate, EndDate,FuncGroupID);
            }
            return View(lstData);
       
        }
        /// <summary>
        /// 使用者維護
        /// </summary>
        /// <returns></returns>
        public ActionResult UserMaintain()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UserMaintain(FormCollection collection)
        {
            ViewData["OperatorID"] = (collection["ddlOperator"] == null) ? 0 : Convert.ToInt32(collection["ddlOperator"].ToString());
            string UserId = ((Session["Account"] == null) ? "" : Session["Account"].ToString());
            ViewData["Mode"] = collection["ddlObj"];
            ViewData["UserGroup"] = (collection["ddlUserGroup"] == null) ? 0 : Convert.ToInt32(collection["ddlUserGroup"].ToString());
            ViewData["UserAccount"] = collection["UserAccount"];
            ViewData["StartDate"] = collection["StartDate"]; ;
            ViewData["EndDate"] = collection["EndDate"]; ;
            ViewData["errorLine"] = null;
            ViewData["IsShowMessage"] = null;
            return View();
        }


    }
}