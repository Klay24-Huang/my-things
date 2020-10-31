using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Input.Rent;
using Domain.SP.Output;
using Domain.SP.Output.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Utils;
using WebCommon;

namespace WebAPI.Controllers
{
    public class MonthlySubscriptionController : ApiController
    {
        
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        [HttpPost]
        public Dictionary<string, object> DoMonthSubscription(IAPI_MonthlySubscription apiInput)
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
            string funName = "MonthlyScriptionController";
            Int64 LogID = 0;
            Int16 ErrType = 0;

            OAPI_Base outputApi = new OAPI_Base();
            Int64 tmpOrder = -1;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            bool CheckFlag = true;
            string IDNO = "";
            #endregion

            #region 防呆
            if (flag)
            {
                //apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject <IAPI_MonthlySubscription>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
            }
            #endregion

            #region TB
            if (flag)
            {

                SPOutput_Base spOut = new SPOutput_Base();

                MonthlySubscriptionData[] MonthlyRentData = apiInput.MonthlySubscriptionObj.ToArray();
                string SPName = new ObjType().GetSPName(ObjType.SPType.MonthlySubscription);
                object[] objparms = new object[MonthlyRentData.Length == 0 ? 1 : MonthlyRentData.Length];
                if (MonthlyRentData.Length > 0)
                {
                    for (int i = 0; i < MonthlyRentData.Length; i++)
                    {
                        objparms[i] = new
                        {
                            IDNO = MonthlyRentData[i].IDNO,
                            Workday = MonthlyRentData[i].Workday,
                            Holiday = MonthlyRentData[i].Holiday
                        };
                    }
                }
                else
                {
                    objparms[0] = new
                    {
                        IDNO = "",
                        Workday = 0,
                        Holiday = 0,
                        MotoTotal = 0,
                        StartDate = "",
                        EndDate = "",
                        seqno = 0,
                        ProjID = "",
                        ProjNM = ""
                    };
                }

                object[][] parms1 = {
                        new object[] {
                            LogID
                    },
                        objparms
                };

                DataSet ds1 = null;
                string returnMessage = "";
                string messageLevel = "";
                string messageType = "";

                ds1 = WebApiClient.SPExeBatchMultiArr2(ServerInfo.GetServerInfo(), SPName, parms1, true, ref returnMessage, ref messageLevel, ref messageType);

                if (ds1.Tables.Count == 0)
                {
                    flag = false;
                    errCode = "ERR999";
                    errMsg = returnMessage;
                }
                else
                {
                    if (ds1.Tables.Count == 1)
                    {
                        baseVerify.checkSQLResult(ref flag, Convert.ToInt32(ds1.Tables[0].Rows[0]["Error"]), ds1.Tables[0].Rows[0]["ErrorCode"].ToString(), ref lstError, ref errCode);
                    }
                }
                ds1.Dispose();

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