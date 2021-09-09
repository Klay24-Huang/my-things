using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Input.JointRent;
using Domain.SP.Output;
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
using WebAPI.Models.Param.Output.PartOfParam;
using WebAPI.Utils;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 案件共同承租人回應邀請
    /// </summary>
    public class JointRentIviteeFeedBackController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> DoJointRentIviteeFeedBack(Dictionary<string, object> value)
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
            string funName = "JointRentIviteeFeedBackController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_JointRentIviteeFeedBack apiInput = null;
            NullOutput outputApi = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string Contentjson = "";
            bool isGuest = true;
            Int64 tmpOrder = -1;
            string OrderNo = "";
            string InviteeId = "";
            string FeedbackType = "";
            #endregion

            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_JointRentIviteeFeedBack>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
            }

            if (flag)
            {
                if (apiInput != null)
                {
                    if (!string.IsNullOrEmpty(apiInput.AESEncryptString))
                    {
                        string KEY = ConfigurationManager.AppSettings["AES128KEY"].Trim();
                        string IV = ConfigurationManager.AppSettings["AES128IV"].Trim();
                        string DecodeString = HttpUtility.UrlDecode(apiInput.AESEncryptString);
                        string ReqParam = string.IsNullOrWhiteSpace(DecodeString) ? "" : AESEncrypt.DecryptAES128(DecodeString, KEY, IV) ;
                        if (ReqParam != "")
                        {
                            string[] parms = ReqParam.Split(new char[] { '&' });
                            for (int i = 0; i < parms.Length; i++)
                            {
                                string[] txts = parms[i].Split(new char[] { '=' });
                                if (txts[0] == "OrderNo")
                                {
                                    OrderNo = txts[1];
                                }
                                else if (txts[0] == "InviteeId")
                                {
                                    InviteeId = txts[1];
                                }
                                else if (txts[0] == "FeedbackType")
                                {
                                    FeedbackType = txts[1];
                                }
                            }
                        }
                    }


                    if (string.IsNullOrWhiteSpace(OrderNo) || string.IsNullOrWhiteSpace(InviteeId))
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                    else if (!string.IsNullOrWhiteSpace(OrderNo))
                    {
                        if (OrderNo.IndexOf("H") < 0)
                        {
                            flag = false;
                            errCode = "ERR900";
                        }
                        if (flag)
                        {
                            flag = Int64.TryParse(OrderNo.Replace("H", ""), out tmpOrder);
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

                    if (!new[] { "Y", "N" }.Any(s => FeedbackType.Contains(s)))
                    {
                        flag = false;
                        errCode = "ERR902";
                    }
                }
            }

            #endregion

            #region TB
            if (flag)
            {
                string spName = new ObjType().GetSPName(ObjType.SPType.JointRentIviteeFeedBack);
                SPInput_JointRentIviteeFeedBack spInput = new SPInput_JointRentIviteeFeedBack()
                {
                    LogID = LogID,
                    Token = Access_Token,
                    OrderNo = tmpOrder,
                    InviteeId = InviteeId,
                    FeedbackType = FeedbackType
                };
                SPOutput_Base spOut = new SPOutput_Base();
                flag = new SQLHelper<SPInput_JointRentIviteeFeedBack, SPOutput_Base>(connetStr).ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, ref spOut, ref lstError, ref errCode);
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