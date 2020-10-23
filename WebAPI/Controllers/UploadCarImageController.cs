using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Input.Rent;
using Domain.SP.Output;
using Domain.SP.Output.Common;
using Newtonsoft.Json;
using NLog;
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
    /// <summary>
    /// 上傳出還車照
    /// </summary>
    public class UploadCarImageController : ApiController
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();

        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 上傳出還車照
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        //public Dictionary<string, object> DoUploadCarImage(Dictionary<string, object> value)
        public Dictionary<string, object> DoUploadCarImage(IAPI_UploadCarImage2 apiInput)
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
            string funName = "UploadCarImageController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            //IAPI_UploadCarImage apiInput = null;
            //20201015 UPD BY JERRY 假資料供檢核使用
            Dictionary<string, object> value = new Dictionary<string, object>() {
                {"Key1", "AAAA"}
            };
            OAPI_UploadCarImage outputApi = new OAPI_UploadCarImage();
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

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                //apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_UploadCarImage>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                //string restoreCarImg = apiInput.CarImage;
                //string tmpCarImg = apiInput.CarImage.Length.ToString();
                List<CarImages> restoreCarImg = apiInput.CarImages;
                List<CarImages> tmpCarImg = new List<CarImages> {
                    new CarImages() {
                    CarType=0,
                    CarImage=apiInput.CarImages.ToArray().Length.ToString()
                } };
                //20201015 暫存不需要存完整圖檔
                apiInput.CarImages = tmpCarImg;

                flag = baseVerify.InsAPLog(apiInput.ToString(), ClientIP, funName, ref errCode, ref LogID);
                apiInput.CarImages = restoreCarImg;
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
                if (apiInput.Mode < 0 || apiInput.Mode > 1)
                {
                    flag = false;
                    errCode = "ERR900";
                }
            }
            //20201015 多筆改在迴圈內檢核
            //if (flag)
            //{
            //    if (apiInput.CarType < 1 || apiInput.CarType > 8)
            //    {
            //        flag = false;
            //        errCode = "ERR900";
            //    }
            //}
            //if (flag)
            //{
            //    if (string.IsNullOrEmpty(apiInput.CarImage))
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
                CarImages[] carImages = apiInput.CarImages.ToArray();
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_UploadCarImage, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_UploadCarImage, SPOutput_Base>(connetStr);
                List<CarImageData> CarImgDataLists = new List<CarImageData>();
                DataSet ds = new DataSet();
                //20201018 修改 by eric，以效能的方面來看，carImages.Length會變成每一次迴圈都會去取lenth，建議改為int len=carImages.Length
                int CarImagesLen = carImages.Length;

                string SPName = new ObjType().GetSPName(ObjType.SPType.UploadCarImage);
                //string SPName = "usp_InsTmpCarImageBatch";
                if (SPName == "usp_InsTmpCarImageBatch")
                {
                    object[] objparms = new object[CarImagesLen == 0 ? 1 : CarImagesLen];
                    if (CarImagesLen > 0)
                    {
                        for (int i = 0; i < CarImagesLen; i++)
                        {
                            objparms[i] = new
                            {
                                CarImageType = carImages[i].CarType,
                                CarImage = carImages[i].CarImage
                            };
                        }
                    }
                    else
                    {
                        objparms[0] = new
                        {
                            CarImageType = 0,
                            CarImage = ""
                        };
                    }

                    object[][] parms1 = {
                        new object[] {
                            IDNO,
                            tmpOrder,
                            Access_Token,
                            Convert.ToInt16(apiInput.Mode),
                            LogID
                    },
                        objparms
                    };

                    DataSet ds1 = null;
                    string returnMessage = "";
                    string messageLevel = "";
                    string messageType = "";

                    ds1 = WebApiClient.SPExeBatchMultiArr2(ServerInfo.GetServerInfo(), SPName, parms1, true, ref returnMessage, ref messageLevel, ref messageType);

                    //logger.Trace(JsonConvert.SerializeObject(ds1));
                    if (ds1.Tables.Count == 0)
                    {
                        flag = false;
                        errCode = "ERR999";
                    }
                    else
                    {
                        if (ds1.Tables.Count == 1)
                        {
                            baseVerify.checkSQLResult(ref flag, Convert.ToInt32(ds1.Tables[0].Rows[0]["Error"]), ds1.Tables[0].Rows[0]["ErrorCode"].ToString(), ref lstError, ref errCode);
                        }
                        else
                        {
                            if (ds1.Tables[1].Rows.Count > 0)
                            {
                                for (int i = 0; i < ds1.Tables[1].Rows.Count; i++)
                                {
                                    CarImgDataLists.Add(new CarImageData
                                    {
                                        CarImageType = Convert.ToInt32(ds1.Tables[1].Rows[i]["CarImageType"]),
                                        HasUpload = Convert.ToInt32(ds1.Tables[1].Rows[i]["HasUpload"])
                                    });
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < CarImagesLen; i++)
                    {

                        if (carImages[i].CarType < 1 || carImages[i].CarType > 8)
                        {
                            flag = false;
                            errCode = "ERR900";
                            break;
                        }
                        if (string.IsNullOrEmpty(carImages[i].CarImage))
                        {
                            flag = false;
                            errCode = "ERR900";
                            break;
                        }
                        SPInput_UploadCarImage spInput = new SPInput_UploadCarImage()
                        {
                            IDNO = IDNO,
                            LogID = LogID,
                            Token = Access_Token,
                            //CarImage=apiInput.CarImage,
                            //CarImageType=Convert.ToInt16(apiInput.CarType),
                            CarImage = carImages[i].CarImage,
                            CarImageType = Convert.ToInt16(carImages[i].CarType),
                            Mode = Convert.ToInt16(apiInput.Mode),
                            OrderNo = tmpOrder
                        };
                        flag = sqlHelp.ExeuteSP(SPName, spInput, ref spOut, ref CarImgDataLists, ref ds, ref lstError);
                        baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                        if (flag == false)
                        {
                            break;
                        }
                    }
                }
                if (flag)
                {
                    outputApi.CarImageObj = new List<CarImageData>();
                    for (int i = 1; i < 9; i++)
                    {
                        CarImageData obj = new CarImageData()
                        {
                            CarImageType = i,
                            HasUpload = 0
                        };
                        int Index = CarImgDataLists.FindIndex(delegate (CarImageData cardata)
                        {
                            return cardata.CarImageType == i;
                        });
                        if (Index > -1)
                        {
                            obj.HasUpload = 1;
                        }
                        outputApi.CarImageObj.Add(obj);
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
