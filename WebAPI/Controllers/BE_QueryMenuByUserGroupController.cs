using Domain.Common;
using Domain.TB.BackEnd;
using Newtonsoft.Json;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.BackEnd.Input;
using WebAPI.Models.Param.BackEnd.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 【後台】用使用者群組查詢對應的功能權限
    /// </summary>
    public class BE_QueryMenuByUserGroupController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 【後台】用使用者群組查詢對應的功能權限
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public Dictionary<string, object> DoBE_QueryFunc(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            //string[] headers=httpContext.Request.Headers.AllKeys;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "BE_QueryFuncByUserGroupController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BE_QueryFuncByUserGroup apiInput = null;
            OAPI_BE_GetFuncPower apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            DateTime StartDate = DateTime.Now, EndDate = DateTime.Now;
            string IDNO = ""; bool isGuest = true;
            Int16 APPKind = 2;
            string Contentjson = "";
            List<BE_MenuCombindConsistPower> menuList = null;
            MenuList MenuList = new MenuList();

            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BE_QueryFuncByUserGroup>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.UserID, apiInput.UserGroupID };
                string[] errList = { "ERR900", "ERR900" };
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);

            }
            #endregion

            #region TB

            if (flag)
            {
                BE_GetFuncPower obj = new AccountManageRepository(connetStr).GetFuncPowerByUserGroupID(Convert.ToInt32(apiInput.UserGroupID));
                if (obj != null)
                {
                    apiOutput = new OAPI_BE_GetFuncPower()
                    {
                        Power = JsonConvert.DeserializeObject<List<Power>>(obj.FuncGroupPower)
                    };

                }

                menuList = new CommonRepository(connetStr).GetMenuListConsistPower();

                MenuList.beMenuList = JsonConvert.SerializeObject(menuList);
                MenuList.PowerList = apiOutput.Power;
                //if (menuList != null)
                //{
                //    for(int i=0;i< menuList.Count; i++)
                //    {
                //        for (int x = 0; x < menuList[i].lstSubMenu.Count; x++)
                //        {
                //            List<Power> powers = apiOutput.Power.FindAll(delegate (Power power)
                //            {
                //                return power.SubMenuCode == menuList[i].lstSubMenu[x].SubMenuCode;
                //            });
                //            if (powers.Count > 0)
                //            {
                //                for (int y = 0; y < menuList[i].lstSubMenu[x].lstPowerFunc[0].lstPowerFunc.Count; y++)
                //                {

                //                    List<PowerList> powerList = powers[0].PowerList.FindAll(delegate (PowerList power)
                //                    {
                //                        return power.Code == menuList[i].lstSubMenu[x].lstPowerFunc[0].lstPowerFunc[y].Code;
                //                    });
                //                    if (powerList.Count > 0)
                //                    {
                //                        menuList[i].lstSubMenu[x].lstPowerFunc[0].lstPowerFunc[y].hasPower = powerList[0].hasPower;
                //                    }
                //                    else
                //                    {
                //                        menuList[i].lstSubMenu[x].lstPowerFunc[0].lstPowerFunc[y].hasPower = 0;
                //                    }
                //                }
                //            }
                //            else
                //            {
                //                for (int y = 0; y < menuList[i].lstSubMenu[x].lstPowerFunc[0].lstPowerFunc.Count; y++)
                //                {
                //                    menuList[i].lstSubMenu[x].lstPowerFunc[0].lstPowerFunc[y].hasPower = 0;
                //                }
                //            }

                //        }
                //    }

                //}
            }
            #endregion

            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, MenuList, token);
            return objOutput;
            #endregion
        }
    }

}
