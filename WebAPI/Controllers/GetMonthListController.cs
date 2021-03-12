using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Output.Common;
using Domain.TB;
using Domain.WebAPI.output.HiEasyRentAPI;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;
using System.Data;
using WebAPI.Utils;
using Domain.SP.Output;
using System.CodeDom;
using Domain.SP.Input.Arrears;
using WebAPI.Models.BillFunc;
using Domain.SP.Input.Rent;

namespace WebAPI.Controllers
{
    public class GetMonthListController : ApiController
    {
        [HttpPost()]
        public Dictionary<string, object> DoGetMonthList([FromBody] Dictionary<string, object> value)
        {
            #region 初始宣告
            var ms_com = new MonSubsCommon();
            var cr_com = new CarRentCommon();
            var trace = new TraceCom();
            HttpContext httpContext = HttpContext.Current;
            //string[] headers=httpContext.Request.Headers.AllKeys;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "GetMonthListController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_GetMonthList apiInput = null;
            OAPI_GetMonthList outputApi = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";

            #endregion

            try
            {
                #region 防呆

                flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
                if (flag)
                {
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_GetMonthList>(Contentjson);
                    //寫入API Log
                    string ClientIP = baseVerify.GetClientIp(Request);
                    flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                    //if (string.IsNullOrWhiteSpace(apiInput.IDNO))
                    //{
                    //    flag = false;
                    //    errCode = "ERR900";
                    //}

                    //if (flag)
                    //{
                    //    //2.判斷格式
                    //    flag = baseVerify.checkIDNO(apiInput.IDNO);
                    //    if (false == flag)
                    //    {
                    //        errCode = "ERR103";
                    //    }
                    //}
                }

                #endregion

                #region TB

                var spIn = new SPInput_GetMonthList()
                {

                };
                //取出月租列表
                var sp_mList = ms_com.sp_GetMonthList(spIn, ref errMsg);
                if(sp_mList != null && sp_mList.Count() > 0)
                {
                    var cards = (from a in sp_mList
                                 select new MonCardParam
                                 {

                                 }).ToList();
                    outputApi.MonCards = cards;
                }

                #endregion
            }
            catch (Exception ex)
            {

            }

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion        
        }
    }
}
