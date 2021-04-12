using Domain.Common;
using Domain.MemberData;
using Domain.SP.Input.Common;
using Domain.SP.Input.Member;
using Domain.SP.Input.Wallet;
using Domain.SP.Output;
using Domain.TB;
using Domain.WebAPI.Input.Taishin;
using Domain.WebAPI.Input.Taishin.GenerateCheckSum;
using Domain.WebAPI.output.Taishin;
using Domain.WebAPI.output.Taishin.ResultData;
using Newtonsoft.Json;
using NLog;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading;
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
    /// <summary>
    /// 非同步接收台新回傳結果
    /// </summary>
    public class RecieveTsacController : ApiController
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string TaishinAPPOS = ConfigurationManager.AppSettings["TaishinAPPOS"].ToString();
        private string ApiVer = ConfigurationManager.AppSettings["ApiVer"].ToString();
        private string ApiVerOther = ConfigurationManager.AppSettings["ApiVerOther"].ToString();

        public Dictionary<string, object> RecieveTsac(Dictionary<string, object> value)
        {
            #region 初始宣告

            logger.Trace("Init:" + JsonConvert.SerializeObject(value));
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "recieveTsacController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_RecieveTsac apiInput = null;
            OAPI_Base apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            string Access_Token_string = "", Access_Token = "";
            #endregion
            #region 防呆
            try
            {

                flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

                //apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BindResult>(Contentjson);
                apiInput = JsonConvert.DeserializeObject<IAPI_RecieveTsac>(Contentjson);

                if (flag)
                {
                    //寫入API Log
                    string ClientIP = baseVerify.GetClientIp(Request);
                    flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);


                }
                
                if (flag)
                {
                    object[][] parms1 = {
                        new object[] {
                            apiInput.row.TransactionNo==null?"":apiInput.row.TransactionNo,
                            apiInput.row.TDATE==null?"":apiInput.row.TDATE,
                            apiInput.row.SIGN==null?"+":apiInput.row.SIGN,
                            apiInput.row.AMT==null?"0":apiInput.row.AMT,
                            apiInput.row.TRNACTNO==null?"":apiInput.row.TRNACTNO,
                            apiInput.row.TXNCODE==null?"":apiInput.row.TXNCODE,
                            apiInput.row.SDATE==null?"":apiInput.row.SDATE,
                            apiInput.row.TIME==null?"":apiInput.row.TIME,
                            apiInput.row.OUTBANK==null?"":apiInput.row.OUTBANK,
                            apiInput.row.OUTACTNO==null?"":apiInput.row.OUTACTNO
                        }};

                    DataSet ds1 = null;
                    string returnMessage = "";
                    string messageLevel = "0";
                    string messageType = "0";
                    string SPName = new ObjType().GetSPName(ObjType.SPType.SaveRecieveTSAC);

                    //ds1 = WebApiClient.SPExeBatchMultiArr2(ServerInfo.GetServerInfo(), SPName, parms1, true, ref returnMessage, ref messageLevel, ref messageType);
                    ds1 = WebApiClient.SPRetB(ServerInfo.GetServerInfo(), SPName, parms1[0], ref returnMessage, ref messageLevel, ref messageType);

                    //logger.Trace(JsonConvert.SerializeObject(ds1));
                    if (ds1.Tables.Count == 0)
                    {
                        flag = false;
                        errCode = "ERR999";
                        errMsg = returnMessage;
                        logger.Trace("SaveBindCard:" + ",Error:" + returnMessage);
                    }
                    else
                    {
                        errCode = ds1.Tables[0].Rows[0]["ErrorCode"].ToString();
                        errMsg = ds1.Tables[0].Rows[0]["ErrorMsg"].ToString();
                    }
                    ds1.Dispose();
                    
                    logger.Trace("Call:" + JsonConvert.SerializeObject(apiInput) + ",Error:" + errCode);
                }
                #endregion
                #region 寫入錯誤Log
                if (false == flag && false == isWriteError)
                {
                    baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
                }
                #endregion
            }
            catch (Exception ex)
            {
                logger.Trace("OUTTER_ERROR:" + ",Error:" + ex.Message);
                errMsg = ex.Message;
                flag = false;
            }
            #region 輸出
            Dictionary<string, object> output = new Dictionary<string, object>();
            output.Add("Result", flag == true ? (errCode != "200" ? 0 : 1) : 0);
            output.Add("Message", errMsg);
            output.Add("StatusCode", flag == true ? (errCode != "200" ? errCode : "200") : "543");

            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            logger.Trace("Output:" + JsonConvert.SerializeObject(output));
            return output;
            #endregion
        }

    }
}
