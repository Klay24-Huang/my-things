using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Input.JointRent;
using Domain.SP.Output;
using Domain.SP.Output.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    public class JointRentInviteeModifyController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> DoJointRentInviteeModify(Dictionary<string, object> value)
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
            string funName = "JointRentInviteeModifyController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_JointRentInviteeModify apiInput = null;
            NullOutput outputApi = new NullOutput();
            //OAPI_JointRentInviteeListQuery outputApi = new OAPI_JointRentInviteeListQuery();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            bool HasInput = true;
            Int64 orderNo = 0;
            //邀請清單
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest, HasInput);

            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_JointRentInviteeModify>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
            }

            //不開放訪客
            if (flag)
            {
                if (isGuest)
                {
                    flag = false;
                    errCode = "ERR150";
                }
            }
            //檢查訂單編號格式
            if(flag)
            {     
                var checkOrderNo = OrderNoFormatVerify(apiInput.OrderNo);
                flag = checkOrderNo.status;
                errCode = checkOrderNo.errorCode;
                orderNo = checkOrderNo.orderNo;
            }
            #endregion

            #region TB
            //Token判斷
            if (flag)
            {
                string CheckTokenName = new ObjType().GetSPName(ObjType.SPType.CheckTokenReturnID);
                SPInput_CheckTokenOnlyToken spCheckTokenInput = new SPInput_CheckTokenOnlyToken()
                {
                    LogID = LogID,
                    Token = Access_Token
                };
                SPOutput_CheckTokenReturnID spOut = new SPOutput_CheckTokenReturnID();
                SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID> sqlHelp = new SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(CheckTokenName, spCheckTokenInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    IDNO = spOut.IDNO;
                }
            }

            if (flag)
            {
                string SPName = new ObjType().GetSPName(ObjType.SPType.JointRentInviteeModify);

                SPInput_JointRentInviteeModify spInput = new SPInput_JointRentInviteeModify()
                {
                    InviteeId = apiInput.InviteeId,
                    OrderNo = orderNo,
                    ActionType = apiInput.ActionType,
                    LogID = LogID,
                    Token = Access_Token,
                    IDNO = IDNO,
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_JointRentInviteeModify, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_JointRentInviteeModify, SPOutput_Base>(connetStr);

                flag = sqlHelp.ExecuteSPNonQuery(SPName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
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


        private (bool status, string errorCode, Int64 orderNo) OrderNoFormatVerify(string OrderNo)
        {
            (bool status, string errorCode, Int64 orderNo) result = (true, "", 0);
            result.errorCode = "000000";
            if (string.IsNullOrWhiteSpace(OrderNo))
            {
                result.status = false;
                result.errorCode = "ERR900";
            }
            else
            {
                if (OrderNo.IndexOf("H") < 0)
                {
                    result.status = false;
                    result.errorCode = "ERR902";
                }
                if (result.status)
                {
                    if (Int64.TryParse(OrderNo.Replace("H", ""), out Int64 tmpOrder))
                    {
                        if (tmpOrder <= 0)
                        {
                            result.status = false;
                            result.errorCode = "ERR902";
                        }
                        else
                        {
                            result.orderNo = tmpOrder;
                        }
                    }
                }
            }
            return result;
        }
    }
}