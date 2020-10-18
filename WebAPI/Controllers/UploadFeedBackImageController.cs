using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Input.Rent;
using Domain.SP.Output;
using Domain.SP.Output.Common;
using Domain.SP.Output.Rent;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    public class UploadFeedBackImageController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 上傳取車回饋
        /// </summary>
        /// <param name="apiInput"></param>
        /// <returns></returns>
        [HttpPost]
        //public Dictionary<string, object> DoUploadCarImage(Dictionary<string, object> value)
        public Dictionary<string, object> DoUploadCarImage(IAPI_UploadFeedBackImage apiInput)
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
            string funName = "UploadFeedBackImageController";
            Int64 LogID = 0;
            Int16 ErrType = 0;

            OAPI_UploadFeedBackImage outputApi = new OAPI_UploadFeedBackImage();
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
            Dictionary<string, object> value = new Dictionary<string, object>();
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest,false);

            if (flag)
            {
                //apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_UploadCarImage>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                IAPI_UploadFeedBackImage tmpAPI = apiInput;
                int len = tmpAPI.FeedBack.Count;
                for(int i = 0; i < len; i++)
                {
                    tmpAPI.FeedBack[i].FeedBackFile = tmpAPI.FeedBack[i].FeedBackFile.Length.ToString();
                    if(tmpAPI.FeedBack[i].SEQNO<1 || tmpAPI.FeedBack[i].SEQNO > 4)
                    {
                        CheckFlag = false;
                        break;
                    }
                }

                flag = baseVerify.InsAPLog(tmpAPI.ToString(), ClientIP, funName, ref errCode, ref LogID);
                if (false == CheckFlag)
                {
                    flag = false;
                    errCode = "ERR900";
                }
                if (flag)
                {
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
            if (flag)
            {
                FeedBackImage[] carImages = apiInput.FeedBack.ToArray();
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_UploadFeedBackImage, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_UploadFeedBackImage, SPOutput_Base>(connetStr);
                List<FeedBackImageData> CarImgDataLists = new List<FeedBackImageData>();
                DataSet ds = new DataSet();
                for (int i = 0; i < carImages.Length; i++)
                {

                    SPInput_UploadFeedBackImage spInput = new SPInput_UploadFeedBackImage()
                    {
                        IDNO = IDNO,
                        LogID = LogID,
                        Token = Access_Token,
                        FeedBackFile = carImages[i].FeedBackFile,
                        SEQNO = Convert.ToInt16(carImages[i].SEQNO),
                        OrderNo = tmpOrder
                    };
                    string SPName = new ObjType().GetSPName(ObjType.SPType.UploadFeedBackImage);
                    flag = sqlHelp.ExeuteSP(SPName, spInput, ref spOut, ref CarImgDataLists, ref ds, ref lstError);
                    baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                    if (flag == false)
                    {
                        break;
                    }
                }
                if (flag)
                {
                    outputApi.FeedBackImageObj = new List<FeedBackImageData>();
                    for (int i = 1; i < 5; i++)
                    {
                        FeedBackImageData obj = new FeedBackImageData()
                        {
                            SEQNO = i,
                            HasUpload = 0
                        };
                        int Index = CarImgDataLists.FindIndex(delegate (FeedBackImageData cardata)
                        {
                            return cardata.SEQNO == i;
                        });
                        if (Index > -1)
                        {
                            obj.HasUpload = 1;
                        }
                        outputApi.FeedBackImageObj.Add(obj);
                    }
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
