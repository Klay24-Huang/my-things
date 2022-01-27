using Domain.Common;
using Domain.MemberData;
using Domain.SP.Input.Wallet;
using Domain.SP.Output;
using Domain.SP.Output.Wallet;
using Domain.WebAPI.Input.Taishin;
using Domain.WebAPI.Input.Taishin.Wallet;
using Domain.WebAPI.Input.Taishin.Wallet.Param;
using Domain.WebAPI.output.Taishin;
using Newtonsoft.Json;
using NLog;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.CusFun.Input;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Service;
using WebAPI.Utils;
using WebCommon;

namespace WebAPI.Controllers
{

    public class WalletStoreShopController : ApiController
    {
        private string TaishinCID = ConfigurationManager.AppSettings["TaishinCID"].ToString();                 //用戶代碼Base64
        private string GetBarCode = ConfigurationManager.AppSettings["GetBarCode"].ToString();                 //超商繳費條碼查詢
        private string CreateCvsPayInfo = ConfigurationManager.AppSettings["CreateCvsPayInfo"].ToString();     //超商繳費資訊上傳-新增
        private string isDebug = ConfigurationManager.AppSettings["isDebug"].ToString();    // 測試Flag

        /// <summary>
        /// 錢包儲值-超商條碼
        /// </summary>
        [HttpPost]
        public Dictionary<string, object> DoWalletStoreShop(Dictionary<string, object> value)
        {
            #region 初始宣告
            var trace = new TraceCom();
            var carRepo = new CarRentRepo();
            var wsp = new WalletSp();
            var walletService = new WalletService();

            HttpContext httpContext = HttpContext.Current;
            var objOutput = new Dictionary<string, object>();    //輸出
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "WalletStoreShopController";
            Int64 LogID = 0;
            Int16 ErrType = 0;

            TaishinWallet WalletAPI = new TaishinWallet();
            IAPI_WalletStoreShop apiInput = null;
            OAPI_WalletStoreShop apiOutput = null;
            var spOutput = new SPOutput_GetWallet();
            WebAPIOutput_GetBarCode outBarCode = null;

            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            string cvsPayToken = "";
            string cvsCode = "";
            string paymentId = "";
            string hmacVal = "";
            string base64String = "";
            DateTime dueDate = new DateTime();

            #endregion
            trace.traceAdd("apiIn", value);

            try
            {
                #region 防呆
                flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
                if (flag)
                {
                    //寫入API Log
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_WalletStoreShop>(Contentjson);
                    string ClientIP = baseVerify.GetClientIp(Request);
                    flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                    //必填檢查
                    if (apiInput.StoreMoney <= 0 || apiInput.CvsType < 0 && apiInput.CvsType > 1)
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                    else if (apiInput.StoreMoney > 0 && apiInput.StoreMoney < 300)
                    {
                        flag = false;
                        errCode = "ERR284";
                    }
                    else if (apiInput.StoreMoney > 20000)
                    {
                        flag = false;
                        errCode = "ERR285";
                    }

                    //不開放訪客
                    if (isGuest)
                    {
                        flag = false;
                        errCode = "ERR101";
                    }
                    trace.FlowList.Add("防呆");
                }
                #endregion
                #region TB
                #region Token判斷
                if (flag && isGuest == false)
                {
                    flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
                }
                #endregion
                #region 儲值金額限制檢核
                if (flag)
                {
                    var walletInfo = walletService.CheckStoreAmtLimit(apiInput.StoreMoney, IDNO, LogID, Access_Token, ref errCode);
                    flag = walletInfo.flag;
                    spOutput = walletInfo.Info;
                }
                #endregion

                if (flag)
                {
                    #region 取得台新APIToken
                    WebAPIOutput_GetTaishinCvsPayToken outToken = new WebAPIOutput_GetTaishinCvsPayToken();
                    flag = WalletAPI.DoGetTaishinCvsPayToken(ref outToken);
                    if (flag && outToken != null)
                    {
                        cvsPayToken = outToken.access_token;
                    }
                    trace.traceAdd("DoGetTaishinCvsPayToken", new { outToken });
                    trace.FlowList.Add("取得台新APIToken");
                    #endregion
                    #region 產生超商銷帳編號
                    dueDate = DateTime.Now.AddHours(3);
                    if (flag)
                    {
                        string cvsIdentifier = GetCvsCode(apiInput.CvsType).Item3; //超商業者識別碼
                        SPInput_GetCvsPaymentId inputPayment = new SPInput_GetCvsPaymentId()
                        {
                            IDNO = IDNO,
                            Token = Access_Token,
                            LogID = LogID,
                            CvsIdentifier = cvsIdentifier
                        };

                        SPOutput_GetCvsPaymentId outputPayment = wsp.sp_GetCvsPaymentId(inputPayment, ref errCode);
                        if (!string.IsNullOrWhiteSpace(outputPayment.PaymentId))
                        {
                            paymentId = outputPayment.PaymentId;
                        }
                        else
                        {
                            flag = false;
                        }

                        trace.traceAdd("sp_GetCvsPaymentId", new { outputPayment });
                        trace.FlowList.Add("產生銷帳編號");
                    }
                    #endregion
                    #region 超商繳費資訊上傳-新增
                    if (flag)
                    {
                        cvsCode = GetCvsCode(apiInput.CvsType).Item2; //超商代收代號
                        IHUBReqHeader header = SetReqHeader(CreateCvsPayInfo);

                        List<CVSPayInfoDetailReq> payInfoList = new List<CVSPayInfoDetailReq>();
                        CVSPayInfoDetailReq detailReq = new CVSPayInfoDetailReq
                        {
                            paidDue = dueDate.ToString("yyyyMMdd"),
                            paymentId = paymentId,
                            payAmount = apiInput.StoreMoney,
                            payPeriod = 1,
                            overPaid = "N",
                            custId = IDNO,
                            custMobile = spOutput.PhoneNo,
                            custEmail = spOutput.Email,
                            memo = ""
                        };
                        payInfoList.Add(detailReq);


                        WebAPI_CreateCvsPayInfo inputCvsPay = new WebAPI_CreateCvsPayInfo()
                        {
                            header = header,
                            body = new CvsPayInfoReq()
                            {
                                txType = "i",
                                txDate = DateTime.Now.ToString("yyyyMMdd"),
                                txBatchNo = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                                recordCount = 1,
                                totalAmount = apiInput.StoreMoney,
                                cvsCode = new CvsCode { cvsCode = cvsCode, cvsType = apiInput.CvsType },
                                detail = payInfoList
                            }
                        };

                        hmacVal = WalletAPI.GetHmacVal(inputCvsPay, inputCvsPay.header.cTxSn);

                        WebAPIOutput_CreateCvsPayInfo outCvsPay = new WebAPIOutput_CreateCvsPayInfo();
                        flag = WalletAPI.DoStoreShopCreateCvsPayInfo(inputCvsPay, cvsPayToken, hmacVal, ref errCode, ref outCvsPay);
                    }
                    #endregion
                    #region Barcode查詢
                    if (flag)
                    {
                        IHUBReqHeader header = SetReqHeader(GetBarCode);
                        WebAPI_GetBarcode inputBarcode = new WebAPI_GetBarcode()
                        {
                            header = header,
                            body = new BarcodeReq()
                            {
                                cvsCode = cvsCode,
                                dueDate = dueDate.ToString("yyyyMMdd"),
                                cvsType = apiInput.CvsType,
                                payAmount = apiInput.StoreMoney,
                                paymentId = paymentId,
                                memo = "",
                                payPeriod = 1
                            }
                        };

                        hmacVal = WalletAPI.GetHmacVal(inputBarcode, inputBarcode.header.cTxSn);

                        outBarCode = new WebAPIOutput_GetBarCode();
                        flag = WalletAPI.DoStoreShopGetBarcode(inputBarcode, cvsPayToken, hmacVal, ref errCode, ref outBarCode);

                        trace.traceAdd("DoStoreShopGetBarcode", new { inputBarcode, cvsPayToken, hmacVal, errCode, outBarCode });
                        trace.FlowList.Add("台新回傳Barcode");
                    }
                    #endregion
                    #region 台新回傳Base64需轉向輸出給APP
                    if (flag && outBarCode.body.barcode64 != null)
                    {
                        byte[] binaryData = Convert.FromBase64String(outBarCode.body.barcode64);
                        using (var memoryStream = new MemoryStream(binaryData))
                        {
                            var rotateImage = System.Drawing.Image.FromStream(memoryStream);
                            rotateImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            base64String = ImageToBase64(rotateImage, System.Drawing.Imaging.ImageFormat.Png);
                        }
                    }
                    #endregion
                    #region 輸出
                    if (flag && !string.IsNullOrWhiteSpace(base64String))
                    {
                        apiOutput = new OAPI_WalletStoreShop()
                        {
                            StoreMoney = apiInput.StoreMoney,
                            Barcode64 = base64String.Replace(" ",""),       //20211125 ADD BY ADAM REASON.要濾空白app才有辦法顯示
                            ShopBarCode1 = outBarCode.body.code1,
                            ShopBarCode2 = outBarCode.body.code2,
                            ShopBarCode3 = outBarCode.body.code3,
                            PayDeadline = dueDate.ToString("yyyy/MM/dd HH:mm")
                        };
                    }
                    else
                    {
                        if (isDebug == "0")
                        {
                            errCode = "ERR940";
                            errMsg = "超商條碼產生失敗，請洽系統管理員";
                        }
                        else
                        {
                            flag = true;
                            if (apiInput.CvsType == 0)
                            {   // 7-11
                                apiOutput = new OAPI_WalletStoreShop()
                                {
                                    StoreMoney = apiInput.StoreMoney,
                                    Barcode64 = "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",
                                    ShopBarCode1 = "",
                                    ShopBarCode2 = "",
                                    ShopBarCode3 = "",
                                    PayDeadline = dueDate.ToString("yyyy/MM/dd HH:mm")
                                };
                            }
                            else
                            {   // 全家
                                apiOutput = new OAPI_WalletStoreShop()
                                {
                                    StoreMoney = apiInput.StoreMoney,
                                    Barcode64 = "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",
                                    ShopBarCode1 = "101125K9A",
                                    ShopBarCode2 = "IRF0000000000034",
                                    ShopBarCode3 = "235927000000300",
                                    PayDeadline = dueDate.ToString("yyyy/MM/dd HH:mm")
                                };
                            }
                        }
                    }
                    #endregion
                }
                #endregion
            }
            catch (Exception ex)
            {
                flag = false;
                errCode = "ERR918";
                trace.BaseMsg = ex.Message;
            }

            trace.traceAdd("TraceFinal", new { errCode, errMsg });
            carRepo.AddTraceLog(222, funName, trace, flag);

            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }


        private List<Tuple<int, string, string>> CvsCodeList = new List<Tuple<int, string, string>>()
        {
            //超商類型(0:7-11 1:全家),超商代收代號,業者識別碼
            new Tuple<int, string,string>(0,"E6H","IRS"),
            new Tuple<int, string,string>(1,"K9A","IRF"),
        };

        private Tuple<int, string, string> GetCvsCode(int CvsType)
        {
            return CvsCodeList.Where(x => x.Item1 == CvsType).FirstOrDefault();
        }

        /// <summary>
        /// 交易請求共通表頭物件
        /// </summary>
        /// <param name="txId"></param>
        /// <returns></returns>
        private IHUBReqHeader SetReqHeader(string txId)
        {
            DateTime NowTime = DateTime.Now;
            string guid = Guid.NewGuid().ToString().Replace("-", "");

            return new IHUBReqHeader()
            {
                cTxSn = guid,
                txDate = string.Format("{0:yyyyMMddTHHmmssK}", NowTime).Replace(":", ""),
                txId = txId,
                cId = TaishinCID
            };
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            var codec = codecs.Where(x => x.FormatID == format.Guid).FirstOrDefault();
            return codec;
        }

        /// <summary>
        /// 圖片轉Base64String
        /// </summary>
        /// <param name="image"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        private string ImageToBase64(Image image, ImageFormat format)
        {
            string base64String = "";
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(Encoder.Quality, 100);
            myEncoderParameters.Param[0] = myEncoderParameter;
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, GetEncoder(format), myEncoderParameters);
                byte[] imageBytes = ms.ToArray();
                base64String = Convert.ToBase64String(imageBytes);
            }
            return base64String;
        }
    }
}