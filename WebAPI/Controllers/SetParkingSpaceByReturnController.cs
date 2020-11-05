using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Input.Rent;
using Domain.SP.Output;
using Domain.SP.Output.Common;
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
using WebAPI.Models.Param.Output.PartOfParam;
using WebAPI.Utils;
using WebCommon;
namespace WebAPI.Controllers
{
    /// <summary>
    /// 設定上傳位置
    /// </summary>
    public class SetParkingSpaceByReturnController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 上傳停車格照片及設定文字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
      [HttpPost]
        //public Dictionary<string, object> DoSetParkingSpaceByReturn(Dictionary<string, object> value)
        public Dictionary<string, object> DoSetParkingSpaceByReturn(IAPI_SetParkingSpaceByReturn apiInput)
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
            string funName = "SetParkingSpaceByReturnController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            //IAPI_SetParkingSpaceByReturn apiInput = null;
            NullOutput outputApi = new NullOutput();
            Int64 tmpOrder = -1;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();


            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;

            string IDNO = "";


            #endregion
            #region 防呆
            Dictionary<string, object> value = new Dictionary<string, object>();
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                //apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_SetParkingSpaceByReturn>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                //string restoreCarImg = apiInput.ParkingSpaceImage;
                //string tmpCarImg = apiInput.ParkingSpaceImage.Length.ToString();

                flag = baseVerify.InsAPLog(apiInput.ToString(), ClientIP, funName, ref errCode, ref LogID);
                //apiInput.ParkingSpaceImage = restoreCarImg;
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
                if (string.IsNullOrEmpty(apiInput.ParkingSpace))
                {
                    flag = false;
                    errCode = "ERR900";
                }
            }
            //if (flag)
            //{
            //    if (string.IsNullOrEmpty(apiInput.ParkingSpaceImage))
            //    {
            //        flag = false;
            //        errCode = "ERR900";
            //    }
            //}
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
                for (int i = 0; i < apiInput.ParkingSpacePic.Count; i++)
                {
                    string FileName = string.Format("{0}_ParkingSpace_{1}_{2}.png", apiInput.OrderNo,apiInput.ParkingSpacePic[i].SEQNO.ToString() , DateTime.Now.ToString("yyyyMMddHHmmss"));
                    #region 加入azure
                    //if (apiInput.ParkingSpaceImage.Length > 0)
                    if (apiInput.ParkingSpacePic[i].ParkingSpaceFile.Length > 0)
                    {
                        try
                        {
                            //flag = new AzureStorageHandle().UploadFileToAzureStorage(apiInput.ParkingSpaceImage, FileName, "carpic");
                            flag = new AzureStorageHandle().UploadFileToAzureStorage(apiInput.ParkingSpacePic[i].ParkingSpaceFile, FileName, "carpic");
                            apiInput.ParkingSpacePic[i].ParkingSpaceFile = FileName;//base64轉檔名存入db
                        }
                        catch (Exception ex)
                        {
                            flag = true;
                        }
                    }
                    #endregion
                }
                //SPInput_SetingParkingSpaceByReturn spInput = new SPInput_SetingParkingSpaceByReturn()
                //{
                //    IDNO = IDNO,
                //    OrderNo = tmpOrder,
                //    ParkingSpace = apiInput.ParkingSpace,
                //    ParkingSpaceImage = apiInput.ParkingSpaceImage,
                //    LogID = LogID,
                //    Token = Access_Token
                //};
                string SPName = new ObjType().GetSPName(ObjType.SPType.SettingParkingSpce);
                //SPOutput_Base spOut = new SPOutput_Base();
                //SQLHelper<SPInput_SetingParkingSpaceByReturn, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_SetingParkingSpaceByReturn, SPOutput_Base>(connetStr);

                //flag = sqlHelp.ExecuteSPNonQuery(SPName, spInput, ref spOut, ref lstError);
                //baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                ParkingSpaceImage[] parkingImages = apiInput.ParkingSpacePic.ToArray();
                object[] objparms = new object[parkingImages.Length == 0 ? 1 : parkingImages.Length];
                if (parkingImages.Length > 0)
                {
                    for(int i=0;i<parkingImages.Length;i++)
                    {
                        objparms[i] = new
                        {
                            parkingSeqno = parkingImages[i].SEQNO,
                            parkingImage = parkingImages[i].ParkingSpaceFile
                        };
                    }
                }
                else
                {
                    objparms[0] = new
                    {
                        parkingSeqno = 0,
                        parkingImage = ""
                    };
                }

                object[][] parms1 = {
                        new object[] {
                            IDNO,
                            tmpOrder,
                            apiInput.ParkingSpace,
                            //(FileName!="")?FileName:apiInput.ParkingSpaceImage,
                            Access_Token,
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
