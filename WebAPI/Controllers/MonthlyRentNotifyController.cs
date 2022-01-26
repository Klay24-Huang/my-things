using Domain.Common;
using Domain.SP.Input.MonthlyRent;
using Domain.SP.Output;
using Domain.SP.Output.MonthlyRent;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.ComboFunc;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    
    public class MonthlyRentNotifyController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        private CommonFunc baseVerify = new CommonFunc();

        [HttpPost]
        public Dictionary<string, object> DoJob(Dictionary<string, object> value)
        {
            logger.Trace("Init");
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            var objOutput = new Dictionary<string, object>();    //輸出
            //string Access_Token = "";
            //string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "MonthlyRentNotifyController";
            Int64 LogID = 0;
            Int16 ErrType = 0;

            //IAPI_CreditAuthJobV2 apiInput = new IAPI_CreditAuthJobV2();
            NullOutput apiOutput = new NullOutput();
            Token token = null;
            baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            List<MonthlyRentNotify> SendList = null;
            #endregion

           //寫入API Log
           string ClientIP = baseVerify.GetClientIp(Request);
           flag = baseVerify.InsAPLog("NA", ClientIP, funName, ref errCode, ref LogID);

            #region TB
            #region 取出待發送清單
            SendList = GetSendList(ref flag, ref lstError, ref errCode);
            #endregion
            if (flag)
            {
                logger.Trace("SendList Count:" + SendList.Count.ToString());

                foreach (var notify in SendList)
                {
                    SPInput_MonthlyRentNotifySendResult notifyResult =
                        new SPInput_MonthlyRentNotifySendResult
                        {
                            SeqNo = notify.SeqNo,
                            Status = -1,
                            PrgName = funName,
                            PrgUser = ""
                        };

                    try
                    {
                        if (notify.SendType == 2)
                        {
                            SendMail send = new SendMail();
                            var sendStatus = Task.Run(() => send.DoSendMail(notify.Title, notify.NotifyContent, notify.NotifyAddr)).Result;
                            notifyResult.Status = sendStatus ? 2 : -1;
                        }
                        else if(notify.SendType == 3)
                        {
                            CreditAuthJobComm creditAuthJobComm = new CreditAuthJobComm();

                            var isMobile = baseVerify.regexStr(notify.NotifyAddr, CommonFunc.CheckType.Mobile);
                            if(isMobile)
                            {
                                var sendStatus = creditAuthJobComm.SendSMS(notify.NotifyAddr, CreditAuthJobComm.MobileTemplateCode.CustomMsg, notify.NotifyContent);
                                notifyResult.Status = sendStatus ? 2 : -1;
                            }
                            else
                            {
                                notifyResult.Status = 97;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        notifyResult.Status = 98;
                        logger.Error($"authSeq:{notify.SeqNo}-- SendList Error:{ex.Message}");
                    }
                    finally
                    {
                        var updateFlag = UpdateSendStatus(notifyResult, ref lstError, ref errCode);
                    }
                }
            }
            #endregion

            #region 寫入錯誤Log
            if (flag == false && isWriteError == false)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }

        //取出發送清單
        private List<MonthlyRentNotify> GetSendList(ref bool flag, ref List<ErrorInfo> lstError, ref string errCode)
        {
            var SendList = new List<MonthlyRentNotify>();

            string SPName = "usp_MonthlyRentNotifySendList_Q01";
            SPOutput_Base spOutBase = new SPOutput_Base();
            SPInput_MonthlyRentNotifySend spInput = new SPInput_MonthlyRentNotifySend();
            SQLHelper<SPInput_MonthlyRentNotifySend, SPOutput_Base> sqlHelpQuery = new SQLHelper<SPInput_MonthlyRentNotifySend, SPOutput_Base>(connetStr);

            DataSet ds = new DataSet();
            flag = sqlHelpQuery.ExeuteSP(SPName, spInput, ref spOutBase, ref SendList, ref ds, ref lstError);
            baseVerify.checkSQLResult(ref flag, ref spOutBase, ref lstError, ref errCode);
            //判斷訂單狀態
            if (flag)
            {
                if (SendList.Count == 0)
                {
                    flag = false;
                    errCode = "ERR203";
                    logger.Trace("GetSendList Error:" + JsonConvert.SerializeObject(lstError));
                }
            }
            return SendList;

        }

        /// <summary>
        /// 更新發送結果
        /// </summary>
        /// <param name="input"></param>
        /// <param name="lstError"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        private bool UpdateSendStatus(SPInput_MonthlyRentNotifySendResult input, ref List<ErrorInfo> lstError, ref string errCode)
        {
            string SPName = "usp_MonthlyRentNotifySendResult_U01";
            SPOutput_Base spOut = new SPOutput_Base();
            SQLHelper<SPInput_MonthlyRentNotifySendResult, SPOutput_Base> sqlHelper = new SQLHelper<SPInput_MonthlyRentNotifySendResult, SPOutput_Base>(connetStr);
            var flag = sqlHelper.ExecuteSPNonQuery(SPName, input, ref spOut, ref lstError);

            if (flag == false)
            {
                logger.Trace("UpdateSendStatus Params:" + JsonConvert.SerializeObject(input));
                logger.Trace("UpdateSendStatus Error:" + JsonConvert.SerializeObject(lstError));
            }
            baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

            return flag;
        }
    }
}
