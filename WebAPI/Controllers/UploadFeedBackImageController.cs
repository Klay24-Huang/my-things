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
        public Dictionary<string, object> DoUploadFeedBackImage(IAPI_UploadFeedBackImage apiInput)
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

            string Contentjson = "";
            bool isGuest = true;
            bool CheckFlag = true;
            string IDNO = "";
            string PIC1 = string.Empty; //照片1檔案名稱
            string PIC2 = string.Empty; //照片2檔案名稱
            string PIC3 = string.Empty; //照片3檔案名稱
            string PIC4 = string.Empty; //照片4檔案名稱
            #endregion
            #region 防呆
            Dictionary<string, object> value = new Dictionary<string, object>();
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest, false);

            if (flag)
            {
                //apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_UploadCarImage>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                //IAPI_UploadFeedBackImage tmpAPI = apiInput;
                //int len = tmpAPI.FeedBack.Count;
                //for(int i = 0; i < len; i++)
                //{
                //    tmpAPI.FeedBack[i].FeedBackFile = tmpAPI.FeedBack[i].FeedBackFile.Length.ToString();
                //    if(tmpAPI.FeedBack[i].SEQNO<1 || tmpAPI.FeedBack[i].SEQNO > 4)
                //    {
                //        CheckFlag = false;
                //        break;
                //    }
                //}
                List<FeedBackImage> restoreCarImg = apiInput.FeedBack;
                List<FeedBackImage> tmpCarImg = new List<FeedBackImage>();
                int len = apiInput.FeedBack.Count;
                for (int i = 0; i < len; i++)
                {
                    tmpCarImg.Add(
                    new FeedBackImage()
                    {
                        SEQNO = apiInput.FeedBack.ToArray()[i].SEQNO,
                        FeedBackFile = apiInput.FeedBack.ToArray()[i].FeedBackFile.Length.ToString()
                    });
                }
                //20201015 暫存不需要存完整圖檔
                apiInput.FeedBack = tmpCarImg;

                flag = baseVerify.InsAPLog(JsonConvert.SerializeObject(apiInput), ClientIP, funName, ref errCode, ref LogID);
                apiInput.FeedBack = restoreCarImg;
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

            //if (flag)
            //{
            //    FeedBackImage[] carImages = apiInput.FeedBack.ToArray();
            //    SPOutput_Base spOut = new SPOutput_Base();
            //    SQLHelper<SPInput_UploadFeedBackImage, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_UploadFeedBackImage, SPOutput_Base>(connetStr);
            //    List<FeedBackImageData> CarImgDataLists = new List<FeedBackImageData>();
            //    DataSet ds = new DataSet();
            //    //for (int i = 0; i < carImages.Length; i++)
            //    //{
            //    //    SPInput_UploadFeedBackImage spInput = new SPInput_UploadFeedBackImage()
            //    //    {
            //    //        IDNO = IDNO,
            //    //        LogID = LogID,
            //    //        Token = Access_Token,
            //    //        FeedBackFile = carImages[i].FeedBackFile,
            //    //        SEQNO = Convert.ToInt16(carImages[i].SEQNO),
            //    //        OrderNo = tmpOrder
            //    //    };
            //    //    string SPName = new ObjType().GetSPName(ObjType.SPType.UploadFeedBackImage);
            //    //    flag = sqlHelp.ExeuteSP(SPName, spInput, ref spOut, ref CarImgDataLists, ref ds, ref lstError);
            //    //    baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
            //    //    if (flag == false)
            //    //    {
            //    //        break;
            //    //    }
            //    //}

            //    string SPName = new ObjType().GetSPName(ObjType.SPType.UploadFeedBackImage);
            //    object[] objparms = new object[carImages.Length == 0 ? 1 : carImages.Length];
            //    if (carImages.Length > 0)
            //    {
            //        for (int i = 0; i < carImages.Length; i++)
            //        {
            //            objparms[i] = new
            //            {
            //                CarImageType = carImages[i].SEQNO,
            //                CarImage = carImages[i].FeedBackFile
            //            };
            //        }
            //    }
            //    else
            //    {
            //        objparms[0] = new
            //        {
            //            CarImageType = 0,
            //            CarImage = ""
            //        };
            //    }

            //    object[][] parms1 = {
            //            new object[] {
            //                IDNO,
            //                tmpOrder,
            //                apiInput.CarDesc,
            //                Access_Token,
            //                LogID
            //        },
            //            objparms
            //        };

            //    DataSet ds1 = null;
            //    string returnMessage = "";
            //    string messageLevel = "";
            //    string messageType = "";

            //    ds1 = WebApiClient.SPExeBatchMultiArr2(ServerInfo.GetServerInfo(), SPName, parms1, true, ref returnMessage, ref messageLevel, ref messageType);

            //    //logger.Trace(JsonConvert.SerializeObject(ds1));
            //    if (ds1.Tables.Count == 0)
            //    {
            //        flag = false;
            //        errCode = "ERR999";
            //        errMsg = returnMessage;
            //    }
            //    else
            //    {
            //        if (ds1.Tables.Count == 1)
            //        {
            //            baseVerify.checkSQLResult(ref flag, Convert.ToInt32(ds1.Tables[0].Rows[0]["Error"]), ds1.Tables[0].Rows[0]["ErrorCode"].ToString(), ref lstError, ref errCode);
            //        }
            //        else
            //        {
            //            if (ds1.Tables[1].Rows.Count > 0)
            //            {
            //                for (int i = 0; i < ds1.Tables[1].Rows.Count; i++)
            //                {
            //                    CarImgDataLists.Add(new FeedBackImageData
            //                    {
            //                        SEQNO = Convert.ToInt32(ds1.Tables[1].Rows[i]["SEQNO"]),
            //                        HasUpload = Convert.ToInt32(ds1.Tables[1].Rows[i]["HasUpload"])
            //                    });
            //                }
            //            }
            //        }
            //    }
            //    ds1.Dispose();

            //    if (flag)
            //    {
            //        outputApi.FeedBackImageObj = new List<FeedBackImageData>();
            //        for (int i = 1; i < 5; i++)
            //        {
            //            FeedBackImageData obj = new FeedBackImageData()
            //            {
            //                SEQNO = i,
            //                HasUpload = 0
            //            };
            //            int Index = CarImgDataLists.FindIndex(delegate (FeedBackImageData cardata)
            //            {
            //                return cardata.SEQNO == i;
            //            });
            //            if (Index > -1)
            //            {
            //                obj.HasUpload = 1;
            //            }
            //            outputApi.FeedBackImageObj.Add(obj);
            //        }
            //    }
            //}

            // 取車照片直接上傳Azure
            if (flag)
            {
                FeedBackImage[] carImages = apiInput.FeedBack.ToArray();
                foreach (var image in carImages)
                {
                    try
                    {
                        string FileName = string.Format("{0}_PIC{1}_{2}.png", apiInput.OrderNo, image.SEQNO, DateTime.Now.ToString("yyyyMMddHHmmss"));
                        flag = new AzureStorageHandle().UploadFileToAzureStorage(image.FeedBackFile, FileName, "feedbackpic");

                        switch (image.SEQNO)
                        {
                            case 1:
                                PIC1 = FileName;
                                break;
                            case 2:
                                PIC2 = FileName;
                                break;
                            case 3:
                                PIC3 = FileName;
                                break;
                            case 4:
                                PIC4 = FileName;
                                break;
                        }
                    }
                    catch
                    {
                        flag = false;
                        errCode = "ERR228";
                    }
                }
            }

            if (flag)
            {
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_InsFeedBack, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_InsFeedBack, SPOutput_Base>(connetStr);

                SPInput_InsFeedBack spInput = new SPInput_InsFeedBack()
                {
                    IDNO = IDNO,
                    OrderNo = tmpOrder,
                    Mode = 0,
                    FeedBackKind = "",
                    Descript = apiInput.CarDesc,
                    Star = 0,
                    Token = Access_Token,
                    LogID = LogID,
                    PIC1 = PIC1,
                    PIC2 = PIC2,
                    PIC3 = PIC3,
                    PIC4 = PIC4
                };
                string SPName = new ObjType().GetSPName(ObjType.SPType.InsFeedBack);
                flag = sqlHelp.ExecuteSPNonQuery(SPName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
            }

            if (flag)
            {
                outputApi.FeedBackImageObj = new List<FeedBackImageData>();

                FeedBackImageData obj1 = new FeedBackImageData() { SEQNO = 1, HasUpload = 0 };
                FeedBackImageData obj2 = new FeedBackImageData() { SEQNO = 2, HasUpload = 0 };
                FeedBackImageData obj3 = new FeedBackImageData() { SEQNO = 3, HasUpload = 0 };
                FeedBackImageData obj4 = new FeedBackImageData() { SEQNO = 4, HasUpload = 0 };

                if (!string.IsNullOrWhiteSpace(PIC1))
                    obj1.HasUpload = 1;
                if (!string.IsNullOrWhiteSpace(PIC2))
                    obj2.HasUpload = 1;
                if (!string.IsNullOrWhiteSpace(PIC3))
                    obj3.HasUpload = 1;
                if (!string.IsNullOrWhiteSpace(PIC4))
                    obj4.HasUpload = 1;

                outputApi.FeedBackImageObj.Add(obj1);
                outputApi.FeedBackImageObj.Add(obj2);
                outputApi.FeedBackImageObj.Add(obj3);
                outputApi.FeedBackImageObj.Add(obj4);
            }
            #endregion

            #region 寫入錯誤Log
            if (flag == false && isWriteError == false)
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