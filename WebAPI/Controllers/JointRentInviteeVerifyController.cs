using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Input.JointRent;
using Domain.SP.Output.Common;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Utils;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 共同承租人邀請檢核
    /// </summary>
    public class JointRentInviteeVerifyController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> DoJointRentInviteeVerify(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "JointRentInviteeVerifyController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_JointRentInviteeVerify apiInput = null;
            OAPI_JointRentInviteeVerify outputApi = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string Contentjson = "";
            bool isGuest = true;
            Int64 tmpOrder = -1;
            string IDNO = "";
            #endregion

            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_JointRentInviteeVerify>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
            }

            if (flag)
            {
                if (apiInput != null)
                {
                    if (string.IsNullOrWhiteSpace(apiInput.OrderNo) || string.IsNullOrWhiteSpace(apiInput.QureyId))
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                    else if (!string.IsNullOrWhiteSpace(apiInput.OrderNo))
                    {
                        if (apiInput.OrderNo.IndexOf("H") < 0)
                        {
                            flag = false;
                            errCode = "ERR900";
                        }
                        if (flag)
                        {
                            flag = Int64.TryParse(apiInput.OrderNo.Replace("H", ""), out tmpOrder);
                            if (flag)
                            {
                                if (tmpOrder <= 0)
                                {
                                    flag = false;
                                    errCode = "ERR900";
                                }

                            }
                        }
                    }
                }
            }

            //不開放訪客
            if (isGuest)
            {
                flag = false;
                errCode = "ERR101";
            }
            #endregion

            #region TB
            //Token判斷
            if (flag && isGuest == false)
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
                string spName = new ObjType().GetSPName(ObjType.SPType.JointRentInviteeVerify);
                SPInput_JointRentInviteeVerify spInput = new SPInput_JointRentInviteeVerify()
                {
                    LogID = LogID,
                    IDNO = IDNO,
                    Token = Access_Token,
                    OrderNo = tmpOrder,
                    QureyId = apiInput.QureyId
                };
                SPOutput_JointRentInviteeVerify spOut = new SPOutput_JointRentInviteeVerify();
                flag = new SQLHelper<SPInput_JointRentInviteeVerify, SPOutput_JointRentInviteeVerify>(connetStr).ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    outputApi = new OAPI_JointRentInviteeVerify()
                    {
                        OrderNo = apiInput.OrderNo,
                        QureyId = apiInput.QureyId,
                        InviteeId = spOut.InviteeId
                    };

                }
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
