using Domain.Common;
using Domain.SP.MA.Input;
using Domain.SP.Output;
using Domain.TB.Maintain;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Maintain.Input;
using WebAPI.Models.Param.Maintain.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    public class MA_MaintainUserGetLatLngController : ApiController
    {
        /// <summary>
        /// 整備人員取得經緯度
        /// </summary>
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> DoMA_MaintainUserGetLatLng(Dictionary<string, object> value)
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
            string funName = "MA_MaintainUserGetLatLngController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_MA_GetManageStationSetting apiInput = null;
            OAPI_MA_MaintainUserGetLatLng outputApi = null;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            CarRepository _repository = new CarRepository(connetStr);
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            string ManageStation = "";
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;

            string IDNO = "";
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_MA_GetManageStationSetting>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

            }
            #endregion
            #region 取得使用者資料
            MemberCleanSetting setting = _repository.GetMemberCleanSetting(apiInput.Account);
            if (setting != null)
            {

                outputApi = new OAPI_MA_MaintainUserGetLatLng()
                        {
                            Account = setting.Account,
                            Lat = setting.Lat,
                            Lng = setting.Lng
                        };
            
            }
           
            #endregion

            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion
        }
    }
}
