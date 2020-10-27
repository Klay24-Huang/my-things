using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Input.Rent;
using Domain.SP.Output;
using Domain.SP.Output.Common;
using Domain.TB;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 取還車回饋
    /// </summary>
    public class FeedBackController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 取還車回饋
        /// </summary>
        /// <param name="apiInput"></param>
        /// <returns></returns>
        [HttpPost]
        //public Dictionary<string, object> DoUploadCarImage(Dictionary<string, object> value)
        public Dictionary<string, object> DoFeedBack(Dictionary<string, object> value)
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
            string funName = "FeedBackController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_FeedBack apiInput = null;
            NullOutput outputApi = new NullOutput();
            Int64 tmpOrder = -1;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            string FeedBackKindStr = "";

            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_FeedBack>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                if (string.IsNullOrWhiteSpace(apiInput.OrderNo))
                {
                    flag = false;
                    errCode = "ERR900";
                }
                else
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
            if (flag)
            {
                if (string.IsNullOrWhiteSpace(apiInput.Descript))
                {
                    flag = false;
                    errCode = "ERR900";
                }
            }
            if (flag)
            {
                if(apiInput.Mode<0 || apiInput.Mode > 1)
                {
                    flag = false;
                    errCode = "ERR900";
                }
                else
                {
                    if (apiInput.Mode == 1)
                    {
                        if (apiInput.FeedBackKind != null)
                        {
                            int FeedBackKindLen = apiInput.FeedBackKind.Count();
                            if (FeedBackKindLen > 0)
                            {
                                FeedBackKindStr = apiInput.FeedBackKind[0].ToString();
                                
                                for (int i = 0; i < FeedBackKindLen; i++)
                                {
                                    FeedBackKindStr += string.Format(",{0}", apiInput.FeedBackKind[i]);
                                }
                            }
                        }
                    }
                    else
                    {
                        apiInput.Star = 0;
                    }
                }
            }
            if (flag)
            {
                if (apiInput.Star < -1 || apiInput.Star > 5)
                {
                    flag = false;
                    errCode = "ERR900";
                }
            }
            //不開放訪客
            if (flag)
            {
                if (isGuest)
                {
                    flag = false;
                    errCode = "ERR101";
                }
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
            #region 上傳圖片到azure
            if (flag)
            {
                if (apiInput.Mode == 0)
                {
                    OtherRepository otherRepository = new OtherRepository(connetStr);
                    List<FeedBackPIC> lstFeedBackPIC = otherRepository.GetFeedBackPIC(tmpOrder);
                    int PICLen = lstFeedBackPIC.Count;
                    for(int i = 0; i < PICLen; i++)
                    {
                        try
                        {
                            string FileName = string.Format("{0}_PIC{1}_{2}.png", apiInput.OrderNo, lstFeedBackPIC[i].SEQNO, DateTime.Now.ToString("yyyyMMddHHmmss"));
                            flag = new AzureStorageHandle().UploadFileToAzureStorage(lstFeedBackPIC[i].FeedBackFile, FileName, "feedbackpic");
                            if (flag)
                            {
                                bool DelFlag=otherRepository.HandleTempFeedBackPIC(lstFeedBackPIC[i].FeedBackPICID,FileName); //更新為azure的檔名
                            }
                        }catch(Exception ex)
                        {
                            flag = true; //先bypass，之後補傳再刪
                        }
                    }
                }
            }
            #endregion
            if (flag)
            {
               
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_InsFeedBack, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_InsFeedBack, SPOutput_Base>(connetStr);


                    SPInput_InsFeedBack spInput = new SPInput_InsFeedBack()
                    {
                        IDNO = IDNO,
                        LogID = LogID,
                        Token = Access_Token,
                         Descript=apiInput.Descript,
                          FeedBackKind= FeedBackKindStr,
                           Mode=apiInput.Mode,
                            Star=apiInput.Star,
                        OrderNo = tmpOrder
                    };
                    string SPName = new ObjType().GetSPName(ObjType.SPType.InsFeedBack);
                    flag = sqlHelp.ExecuteSPNonQuery(SPName, spInput, ref spOut,ref lstError);
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
    }
}
