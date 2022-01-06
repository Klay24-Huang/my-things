using Domain.SP.Input.OtherService.Common;
using Domain.SP.Input.OtherService.Taishin;
using Domain.WebAPI.Input.Taishin;
using Domain.WebAPI.Input.Taishin.GenerateCheckSum;
using Domain.WebAPI.output.Taishin;
using Newtonsoft.Json;
using NLog;
using OtherService.Common;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebCommon;

namespace OtherService
{
    /// <summary>
    /// 台新綁卡WebAPI
    /// </summary>
    public class TaishinCreditCardBindAPI
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();

        private string apikey = ConfigurationManager.AppSettings["TaishinAPIKey"].ToString();
        //20211213 ADD BY ADAM REASON.強制把換卡改到舊的商代，此目的在解決台新預授權用的商代在綁卡設定上有問題做的處置
        private string oldapikey = ConfigurationManager.AppSettings["oldTaishinAPIKey"].ToString();
        private string BaseURL = ConfigurationManager.AppSettings["TaishinBaseURL"].ToString();                     //台新base網址
        private string ECBaseURL = ConfigurationManager.AppSettings["TaishinECBaseURL"].ToString();
        private string GetCardPage = ConfigurationManager.AppSettings["GetCardPage"].ToString();                    //取得綁卡網址
        private string GetCreditCardStatus = ConfigurationManager.AppSettings["GetCreditCardStatus"].ToString();    //取得綁卡狀態
        private string DeleteCreditCardAuth = ConfigurationManager.AppSettings["DeleteCreditCardAuth"].ToString();  //刪除綁卡
        private string GetCreditCardList = ConfigurationManager.AppSettings["GetCreditCardList"].ToString();        //取得綁卡列表
        private string GetPaymentInfo = ConfigurationManager.AppSettings["GetPaymentInfo"].ToString();              //查詢訂單狀態
        private string GetECOrderInfo = ConfigurationManager.AppSettings["GetECOrderInfo"].ToString();              //查詢訂單狀態
        private string ECRefund = ConfigurationManager.AppSettings["ECRefund"].ToString();                          //退貨
        private string Auth = ConfigurationManager.AppSettings["Auth"].ToString();              //直接授權   
        private string AzureAPIBaseURL = ConfigurationManager.AppSettings["AzureAPIBaseUrl"].ToString();
        private string CreditCardTest = ConfigurationManager.AppSettings["CreditCardTest"].ToString();
        private string RelayBaseURL = ConfigurationManager.AppSettings["RelayBaseURL"].ToString();                  //中繼網址
        private string RelayPostApi = ConfigurationManager.AppSettings["RelayPostApi"].ToString();                  //中繼API
        private string RelayStatus = ConfigurationManager.AppSettings["RelayStatus"].ToString();                    //是否要啟用中繼
        private string relayEnKey = ConfigurationManager.AppSettings["RelayEnKey"].ToString();                      //中繼加密key
        private string relayEnSalt = ConfigurationManager.AppSettings["RelayEnSalt"].ToString();                    //中繼加密Salt

        private static MemoryCache _cache = MemoryCache.Default;

        //Azure Api URL
        #region 取得綁卡網址
        /// <summary>
        /// 取得綁卡網址
        /// </summary>
        /// <param name="wsInput"></param>
        /// <param name="errCode"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoBind(WebAPIInput_Base wsInput, ref string errCode, ref WebAPIOutput_Base output)
        {
            bool flag = true;
            //string ori = string.Format("request={0}&apikey={1}", Newtonsoft.Json.JsonConvert.SerializeObject(wsInput), apikey);
            string ori = string.Format("request={0}&apikey={1}", Newtonsoft.Json.JsonConvert.SerializeObject(wsInput), oldapikey);
            string checksum = GenerateSign(ori);

            WebAPIInput_Bind Input = new WebAPIInput_Bind()
            {
                ApiVer = wsInput.ApiVer,
                ApposId = wsInput.ApposId,
                Random = wsInput.Random,
                RequestParams = wsInput.RequestParams,
                CheckSum = checksum,
                TimeStamp = wsInput.TimeStamp
            };


            output = DoBindSend(Input).Result;
            if (output.RtnCode == "1000")
            {
                //if (output.Data == null)
                //{
                //    flag = false;
                //}
            }
            else
            {
                flag = false;
            }
            return flag;
        }

        public static void SetHeaderValue(WebHeaderCollection header, string name, string value)
        {
            var property = typeof(WebHeaderCollection).GetProperty("InnerCollection",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (property != null)
            {
                var collection = property.GetValue(header, null) as NameValueCollection;
                collection[name] = value;
            }
        }
        /// <summary>
        /// 執行取得綁卡網址
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<WebAPIOutput_Base> DoBindSend(WebAPIInput_Bind input)
        {
            string Site = BaseURL + GetCardPage;
            Site = AzureAPIBaseURL + @"api/TestTaishiBU";
            //20201125紀錄接收資料
            logger.Trace("DoBindSend:" + Site);
            WebAPIOutput_Base output = null;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Site);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.KeepAlive = false;
            //SetHeaderValue(request.Headers, "Connection", "close");
            request.Timeout = 30000;

            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                string postBody = JsonConvert.SerializeObject(input);//將匿名物件序列化為json字串
                byte[] byteArray = Encoding.UTF8.GetBytes(postBody);//要發送的字串轉為byte[]

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);
                    reqStream.Dispose();
                }



                //發出Request
                string responseStr = "";
                using (WebResponse response = request.GetResponse())
                {

                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseStr = reader.ReadToEnd();
                        //20201125紀錄接收資料
                        logger.Trace(responseStr);
                        reader.Close();
                        reader.Dispose();

                        RTime = DateTime.Now;
                        output = JsonConvert.DeserializeObject<WebAPIOutput_Base>(responseStr);

                        //錯誤檢核
                        if (output.RtnCode != "1000")
                        {
                            output = new WebAPIOutput_Base()
                            {
                                RtnCode = "ERR600",
                                RtnMessage = output.RtnMessage
                            };
                        }

                    }
                    //增加關閉連線的呼叫
                    response.Close();
                    response.Dispose();
                }
                //增加關閉連線的呼叫
                //try
                //{
                //    HttpWebRequest requestClose = (HttpWebRequest)WebRequest.Create(Site);
                //    requestClose.KeepAlive = false;
                //    requestClose.Method = "POST";
                //    HttpWebResponse myHttpWebResponse = (HttpWebResponse)requestClose.GetResponse();
                //    myHttpWebResponse.Close();
                //}
                //catch (Exception exc)
                //{
                //    logger.Error(exc.Message);
                //}
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_Base()
                {
                    RtnCode = "0",
                    RtnMessage = ex.Message
                };
                logger.Error(ex.Message);
            }
            finally
            {

                //request.Abort();
                SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                {
                    MKTime = MKTime,
                    UPDTime = RTime,
                    WebAPIInput = JsonConvert.SerializeObject(input),
                    WebAPIName = "GetCardPage",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + GetCardPage
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);

                //增加關閉Request的處理
                request.Abort();
            }


            return output;
        }
        #endregion
        #region 取回綁定信用卡|銀行帳號列表
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wsInput"></param>
        /// <param name="errCode"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoGetCreditCardList_ori(PartOfGetCreditCardList wsInput, ref string errCode, ref WebAPIOutput_GetCreditCardList output)
        {
            bool flag = true;
            string ori = string.Format("request={0}&apikey={1}", Newtonsoft.Json.JsonConvert.SerializeObject(wsInput), apikey);
            string checksum = GenerateSign(ori);

            WebAPIInput_GetCreditCardList Input = new WebAPIInput_GetCreditCardList()
            {
                ApiVer = wsInput.ApiVer,
                ApposId = wsInput.ApposId,
                Random = wsInput.Random,
                RequestParams = wsInput.RequestParams,
                CheckSum = checksum,
                TimeStamp = wsInput.TimeStamp,
                TransNo = wsInput.TransNo
            };

            output = DoGetCreditCardListSend(Input).Result;
            if (output.RtnCode == "1000")
            {
                //if (output.Data == null)
                //{
                //    flag = false;
                //}
            }
            else
            {
                flag = false;
            }
            return flag;
        }

        public bool DoGetCreditCardListCache(PartOfGetCreditCardList wsInput, ref string errCode, ref WebAPIOutput_GetCreditCardList output, bool refresh = false)
        {
            bool flag = false;
            string cacheNm = "BankCardCache";
            string bankNm = "TSIB";
            int cacheMins = 10;
            bool reCall = false;

            var cItems = new List<BankCardCache>();
            var m = new BankCardCache();

            string lastTime = DateTime.Now.AddMinutes(cacheMins * (-1)).ToString("yyyyMMddHHmmss");

            if (_cache != null && _cache[cacheNm] != null)
            {
                cItems = (List<BankCardCache>)_cache[cacheNm];
                if (cItems != null && cItems.Count() > 0)
                    cItems = cItems.Where(x => string.Compare(x.CacheTime, lastTime) >= 0).ToList();
            }

            if (cItems != null && cItems.Count() > 0)
                m = cItems.Where(x => x.BankNm == bankNm && x.IDNO == wsInput.RequestParams.MemberId).FirstOrDefault();

            if (refresh == false)
            {
                if (_cache != null && _cache[cacheNm] != null)
                {
                    if (m != null)
                    {
                        reCall = false;
                        if (string.IsNullOrWhiteSpace(m.apiJson))
                            output = JsonConvert.DeserializeObject<WebAPIOutput_GetCreditCardList>(m.apiJson);
                        errCode = m.errCode;
                        return true;
                    }
                    else
                        reCall = true;
                }
                else
                    reCall = true;
            }
            else
                reCall = true;

            if (reCall)
            {
                bool xflag = DoGetCreditCardList_ori(wsInput, ref errCode, ref output);
                if (xflag)
                {
                    _cache.Remove(cacheNm);
                    BankCardCache newItem = new BankCardCache()
                    {
                        IDNO = wsInput.RequestParams.MemberId,
                        BankNm = bankNm,
                        CacheTime = DateTime.Now.ToString("yyyyMMddHHmmss"),
                        apiJson = JsonConvert.SerializeObject(output),
                        errCode = errCode
                    };
                    cItems = cItems.Where(x => x.BankNm == bankNm && x.IDNO != m.IDNO).ToList();
                    cItems.Add(newItem);
                    CacheItemPolicy cacheItemPolicy = new CacheItemPolicy() { AbsoluteExpiration = DateTime.Now.AddMinutes(cacheMins) };
                    _cache.Add(cacheNm, cItems, cacheItemPolicy);
                }
                return xflag;
            }

            return flag;
        }

        public bool DoGetCreditCardList(PartOfGetCreditCardList wsInput, ref string errCode, ref WebAPIOutput_GetCreditCardList output)
        {
            //return DoGetCreditCardListCache(wsInput, ref errCode, ref output);
            return DoGetCreditCardList_ori(wsInput, ref errCode, ref output);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<WebAPIOutput_GetCreditCardList> DoGetCreditCardListSend(WebAPIInput_GetCreditCardList input)
        {
            string Site = BaseURL + GetCreditCardList;
            //Site = AzureAPIBaseURL + @"api/TestTaishiBQ";
            WebAPIOutput_GetCreditCardList output = null;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Site);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.KeepAlive = false;
            //SetHeaderValue(request.Headers, "Connection", "close");
            request.Timeout = 30000;
            //request.Headers["Connection"] = "keep-alive";
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                string postBody = JsonConvert.SerializeObject(input);//將匿名物件序列化為json字串
                byte[] byteArray = Encoding.UTF8.GetBytes(postBody);//要發送的字串轉為byte[]

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);
                    reqStream.Dispose();
                }



                //發出Request
                string responseStr = "";
                using (WebResponse response = request.GetResponse())
                {

                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseStr = reader.ReadToEnd();
                        RTime = DateTime.Now;
                        output = JsonConvert.DeserializeObject<WebAPIOutput_GetCreditCardList>(responseStr);

                        //錯誤檢核
                        if (output.RtnCode != "1000")
                        {
                            output = new WebAPIOutput_GetCreditCardList()
                            {
                                RtnCode = "ERR601",
                                RtnMessage = output.RtnMessage
                            };
                        }

                        //20201125紀錄接收資料
                        logger.Trace(responseStr);
                        reader.Close();
                        reader.Dispose();
                    }

                    //增加關閉連線的呼叫
                    response.Close();
                    response.Dispose();
                }
                //增加關閉連線的呼叫
                //HttpWebRequest requestClose = (HttpWebRequest)WebRequest.Create(Site);
                //requestClose.Connection = "Close";
                //HttpWebResponse myHttpWebResponse = (HttpWebResponse)requestClose.GetResponse();
                //myHttpWebResponse.Close();
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_GetCreditCardList()
                {
                    RtnCode = "0",
                    RtnMessage = ex.Message
                };
            }
            finally
            {
                //request.Abort();
                SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                {
                    MKTime = MKTime,
                    UPDTime = RTime,
                    WebAPIInput = JsonConvert.SerializeObject(input),
                    WebAPIName = "GetCreditCardList",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + GetCreditCardList
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
                //增加關閉Request的處理
                request.Abort();
            }


            return output;
        }
        #endregion
        #region 取消綁定
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wsInput"></param>
        /// <param name="errCode"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoDeleteCreditCardAuth(PartOfDeleteCreditCardAuth wsInput, ref string errCode, ref WebAPIOutput_DeleteCreditCardAuth output)
        {
            bool flag = true;
            string ori = string.Format("request={0}&apikey={1}", Newtonsoft.Json.JsonConvert.SerializeObject(wsInput), oldapikey);//解綁鎖定在舊商代
            string checksum = GenerateSign(ori);

            WebAPIInput_DeleteCreditCardAuth Input = new WebAPIInput_DeleteCreditCardAuth()
            {
                ApiVer = wsInput.ApiVer,
                ApposId = wsInput.ApposId,
                Random = wsInput.Random,
                RequestParams = wsInput.RequestParams,
                CheckSum = checksum,
                TimeStamp = wsInput.TimeStamp,
                TransNo = wsInput.TransNo
            };


            output = DoDeleteCreditCardAuthSend(Input).Result;
            if (output.RtnCode == "1000")
            {
                //if (output.Data == null)
                //{
                //    flag = false;
                //}
            }
            else
            {
                flag = false;
            }
            return flag;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<WebAPIOutput_DeleteCreditCardAuth> DoDeleteCreditCardAuthSend(WebAPIInput_DeleteCreditCardAuth input)
        {
            string Site = BaseURL + DeleteCreditCardAuth;
            WebAPIOutput_DeleteCreditCardAuth output = null;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Site);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.KeepAlive = false;
            //SetHeaderValue(request.Headers, "Connection", "close");
            request.Timeout = 78000;
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                string postBody = JsonConvert.SerializeObject(input);//將匿名物件序列化為json字串
                byte[] byteArray = Encoding.UTF8.GetBytes(postBody);//要發送的字串轉為byte[]

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);
                    reqStream.Dispose();
                }



                //發出Request
                string responseStr = "";
                using (WebResponse response = request.GetResponse())
                {

                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseStr = reader.ReadToEnd();
                        RTime = DateTime.Now;
                        output = JsonConvert.DeserializeObject<WebAPIOutput_DeleteCreditCardAuth>(responseStr);

                        //20201125紀錄接收資料
                        logger.Trace(responseStr);
                        reader.Close();
                        reader.Dispose();
                    }

                    //增加關閉連線的呼叫
                    response.Close();
                    response.Dispose();
                }
                //增加關閉連線的呼叫
                //HttpWebRequest requestClose = (HttpWebRequest)WebRequest.Create(Site);
                //requestClose.Connection = "Close";
                //HttpWebResponse myHttpWebResponse = (HttpWebResponse)requestClose.GetResponse();
                //myHttpWebResponse.Close();
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_DeleteCreditCardAuth()
                {
                    RtnCode = "0",
                    RtnMessage = ex.Message
                };
            }
            finally
            {
                SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                {
                    MKTime = MKTime,
                    UPDTime = RTime,
                    WebAPIInput = JsonConvert.SerializeObject(input),
                    WebAPIName = "DeleteCreditCardAuth",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + GetCreditCardList
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
                //增加關閉Request的處理
                request.Abort();
            }


            return output;
        }
        #endregion
        #region 查詢訂單狀態
        /// <summary>
        /// 查詢訂單狀態，ApiVer要用1.0.1
        /// </summary>
        /// <param name="wsInput"></param>
        /// <param name="errCode"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoCreditCardAuthQuery(PartOfGetPaymentInfo wsInput, ref string errCode, ref WebAPIOutput_GetPaymentInfo output)
        {
            bool flag = true;
            string ori = string.Format("request={0}&apikey={1}", Newtonsoft.Json.JsonConvert.SerializeObject(wsInput), apikey);
            string checksum = GenerateSign(ori);

            WebAPIInput_GetPaymentInfo Input = new WebAPIInput_GetPaymentInfo()
            {
                ApiVer = wsInput.ApiVer,
                ApposId = wsInput.ApposId,
                Random = wsInput.Random,
                RequestParams = wsInput.RequestParams,
                CheckSum = checksum,
                TimeStamp = wsInput.TimeStamp,
                TransNo = wsInput.TransNo
            };


            output = DoCreditCardAuthQuerySend(Input).Result;
            if (output.RtnCode == "1000")
            {
                //if (output.Data == null)
                //{
                //    flag = false;
                //}
            }
            else
            {
                flag = false;
            }
            return flag;
        }
        public async Task<WebAPIOutput_GetPaymentInfo> DoCreditCardAuthQuerySend(WebAPIInput_GetPaymentInfo input)
        {
            string Site = BaseURL + GetPaymentInfo;
            WebAPIOutput_GetPaymentInfo output = null;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Site);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.KeepAlive = false;
            //SetHeaderValue(request.Headers, "Connection", "close");
            request.Timeout = 78000;
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                string postBody = JsonConvert.SerializeObject(input);//將匿名物件序列化為json字串
                byte[] byteArray = Encoding.UTF8.GetBytes(postBody);//要發送的字串轉為byte[]

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);
                    reqStream.Dispose();
                }



                //發出Request
                string responseStr = "";
                using (WebResponse response = request.GetResponse())
                {

                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseStr = reader.ReadToEnd();
                        RTime = DateTime.Now;
                        output = JsonConvert.DeserializeObject<WebAPIOutput_GetPaymentInfo>(responseStr);
                        reader.Close();
                        reader.Dispose();
                    }

                    //增加關閉連線的呼叫
                    response.Close();
                    response.Dispose();
                }
                //增加關閉連線的呼叫
                //HttpWebRequest requestClose = (HttpWebRequest)WebRequest.Create(Site);
                //requestClose.Connection = "Close";
                //HttpWebResponse myHttpWebResponse = (HttpWebResponse)requestClose.GetResponse();
                //myHttpWebResponse.Close();
                //request.Abort();
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_GetPaymentInfo()
                {
                    RtnCode = "0",
                    RtnMessage = ex.Message
                };
            }
            finally
            {
                SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                {
                    MKTime = MKTime,
                    UPDTime = RTime,
                    WebAPIInput = JsonConvert.SerializeObject(input),
                    WebAPIName = "GetPaymentInfo",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + GetPaymentInfo
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
                //增加關閉Request的處理
                request.Abort();
            }


            return output;
        }


        /// <summary>
        /// 查詢訂單狀態，ApiVer要用1.0.1
        /// </summary>
        /// <param name="wsInput"></param>
        /// <param name="errCode"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoECOrderQuery(PartOfGetECOrderInfo wsInput, ref string errCode, ref WebAPIOutput_GetPaymentInfo output)
        {
            bool flag = true;
            string ori = string.Format("request={0}&apikey={1}", Newtonsoft.Json.JsonConvert.SerializeObject(wsInput), apikey);
            string checksum = GenerateSign(ori);

            WebAPIInput_GetECOrderInfo Input = new WebAPIInput_GetECOrderInfo()
            {
                ApiVer = wsInput.ApiVer,
                ApposId = wsInput.ApposId,
                Random = wsInput.Random,
                RequestParams = wsInput.RequestParams,
                CheckSum = checksum,
                TimeStamp = wsInput.TimeStamp,
                TransNo = wsInput.TransNo
            };


            output = DoECOrderQuerySend(Input).Result;
            if (output.RtnCode == "1000")
            {
                if (output.ResponseParams.ResultCode != "1000")
                {
                    flag = false;
                }
            }
            else
            {
                flag = false;
            }
            return flag;
        }

        public async Task<WebAPIOutput_GetPaymentInfo> DoECOrderQuerySend(WebAPIInput_GetECOrderInfo input)
        {
            string Site = ECBaseURL.Replace("ECPay/", "ws/" + GetECOrderInfo);
            WebAPIOutput_GetPaymentInfo output = null;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Site);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.KeepAlive = false;
            //SetHeaderValue(request.Headers, "Connection", "close");
            request.Timeout = 78000;
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                string postBody = JsonConvert.SerializeObject(input);//將匿名物件序列化為json字串
                byte[] byteArray = Encoding.UTF8.GetBytes(postBody);//要發送的字串轉為byte[]

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);
                    reqStream.Dispose();
                }

                //發出Request
                string responseStr = "";
                using (WebResponse response = request.GetResponse())
                {

                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseStr = reader.ReadToEnd();
                        RTime = DateTime.Now;
                        output = JsonConvert.DeserializeObject<WebAPIOutput_GetPaymentInfo>(responseStr);
                        reader.Close();
                        reader.Dispose();
                    }

                    //增加關閉連線的呼叫
                    response.Close();
                    response.Dispose();
                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_GetPaymentInfo()
                {
                    RtnCode = "0",
                    RtnMessage = ex.Message
                };
            }
            finally
            {
                SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                {
                    MKTime = MKTime,
                    UPDTime = RTime,
                    WebAPIInput = JsonConvert.SerializeObject(input),
                    WebAPIName = "GetECOrderInfo",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + GetECOrderInfo
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
                //增加關閉Request的處理
                request.Abort();
            }


            return output;
        }
        #endregion
        #region 授權
        public bool DoCreditCardAuth(PartOfCreditCardAuth wsInput, ref string errCode, ref WebAPIOutput_Auth output)
        {
            bool flag = true;
            string ori = string.Format("request={0}&apikey={1}", Newtonsoft.Json.JsonConvert.SerializeObject(wsInput), apikey);
            string checksum = GenerateSign(ori);
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            WebAPIInput_Auth Input = new WebAPIInput_Auth()
            {
                ApiVer = wsInput.ApiVer,
                ApposId = wsInput.ApposId,
                Random = wsInput.Random,
                RequestParams = wsInput.RequestParams,
                CheckSum = checksum,
                TimeStamp = wsInput.TimeStamp

            };

            var orderInfo = GetOrderInfoFromMerchantTradeNo(Input.RequestParams.MerchantTradeNo);
            int creditType = orderInfo.creditType;
            Int64 tmpOrder = orderInfo.OrderNo;
            string tmp = orderInfo.OrderString;

            //string tmp = "";
            //Int64 tmpOrder = 0;
            //int creditType = 99;

            //if (Input.RequestParams.MerchantTradeNo.IndexOf("F_") > -1)
            //{
            //    int Index = Input.RequestParams.MerchantTradeNo.IndexOf("F_");
            //    tmp = Input.RequestParams.MerchantTradeNo.Substring(0, Index);
            //    tmpOrder = Convert.ToInt64(tmp);
            //    creditType = 0;
            //}
            //else if (Input.RequestParams.MerchantTradeNo.IndexOf("P_") > -1)
            //{
            //    int Index = Input.RequestParams.MerchantTradeNo.IndexOf("P_");
            //    tmp = Input.RequestParams.MerchantTradeNo.Substring(0, Index);
            //    //  tmpOrder = Convert.ToInt64(tmp);
            //    creditType = 1;
            //}
            //else if (Input.RequestParams.MerchantTradeNo.IndexOf("E_") > -1)
            //{
            //    int Index = Input.RequestParams.MerchantTradeNo.IndexOf("E_");
            //    tmp = Input.RequestParams.MerchantTradeNo.Substring(0, Index);
            //    // tmpOrder = Convert.ToInt64(tmp);
            //    creditType = 2;
            //}
            //else if (Input.RequestParams.MerchantTradeNo.IndexOf("G_") > -1)
            //{
            //    int Index = Input.RequestParams.MerchantTradeNo.IndexOf("G_");
            //    tmp = Input.RequestParams.MerchantTradeNo.Substring(0, Index);
            //    //  tmpOrder = Convert.ToInt64(tmp);
            //    creditType = 3;
            //}
            SPInput_InsTrade SPInput = new SPInput_InsTrade()
            {
                amount = Convert.ToInt32(Input.RequestParams.TradeAmount) / 100,
                OrderNo = tmpOrder,
                CreditType = creditType,
                LogID = 0,
                MerchantTradeNo = Input.RequestParams.MerchantTradeNo,
                CardToken = Input.RequestParams.CardToken,
                MemberID = tmp
            };
            new WebAPILogCommon().InsCreditAuthData(SPInput, ref flag, ref errCode, ref lstError);
            if (flag)
            {
                output = DoCreditCardAuthSend(Input).Result;
                if (output.RtnCode == "1000")
                {
                    //if (output.Data == null)
                    //{
                    //    flag = false;
                    //}
                }
                else
                {
                    flag = false;
                }
            }

            return flag;
        }
        /// <summary>
        /// 2020/11/24 ADD BY ADAM REASON.InsTrade增加IDNO存檔
        /// </summary>
        /// <param name="wsInput"></param>
        /// <param name="IDNO"></param>
        /// <param name="errCode"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoCreditCardAuthV2(PartOfCreditCardAuth wsInput, string IDNO, ref string errCode, ref WebAPIOutput_Auth output)
        {
            bool flag = true;
            string ori = string.Format("request={0}&apikey={1}", Newtonsoft.Json.JsonConvert.SerializeObject(wsInput), apikey);
            string checksum = GenerateSign(ori);
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            WebAPIInput_Auth Input = new WebAPIInput_Auth()
            {
                ApiVer = wsInput.ApiVer,
                ApposId = wsInput.ApposId,
                Random = wsInput.Random,
                RequestParams = wsInput.RequestParams,
                CheckSum = checksum,
                TimeStamp = wsInput.TimeStamp

            };

            var orderInfo = GetOrderInfoFromMerchantTradeNo(Input.RequestParams.MerchantTradeNo);
            int creditType = orderInfo.creditType;
            Int64 tmpOrder = orderInfo.OrderNo;
            //string tmp = orderInfo.OrderString;

            ////string[] tmp;
            //string tmp = "";
            //Int64 tmpOrder = 0;
            //int creditType = 99;
            //if (Input.RequestParams.MerchantTradeNo.IndexOf("F_") > -1)
            //{
            //    int Index = Input.RequestParams.MerchantTradeNo.IndexOf("F_");
            //    tmp = Input.RequestParams.MerchantTradeNo.Substring(0, Index);
            //    tmpOrder = Convert.ToInt64(tmp);
            //    creditType = 0;
            //}
            //else if (Input.RequestParams.MerchantTradeNo.IndexOf("P_") > -1)
            //{
            //    int Index = Input.RequestParams.MerchantTradeNo.IndexOf("P_");
            //    tmp = Input.RequestParams.MerchantTradeNo.Substring(0, Index);
            //    //  tmpOrder = Convert.ToInt64(tmp);
            //    creditType = 1;
            //}
            //else if (Input.RequestParams.MerchantTradeNo.IndexOf("E_") > -1)
            //{
            //    int Index = Input.RequestParams.MerchantTradeNo.IndexOf("E_");
            //    tmp = Input.RequestParams.MerchantTradeNo.Substring(0, Index);
            //    // tmpOrder = Convert.ToInt64(tmp);
            //    creditType = 2;
            //}
            //else if (Input.RequestParams.MerchantTradeNo.IndexOf("G_") > -1)
            //{
            //    int Index = Input.RequestParams.MerchantTradeNo.IndexOf("G_");
            //    tmp = Input.RequestParams.MerchantTradeNo.Substring(0, Index);
            //    //  tmpOrder = Convert.ToInt64(tmp);
            //    creditType = 3;
            //}
            //else if (Input.RequestParams.MerchantTradeNo.IndexOf("M_") > -1)
            //{
            //    int Index = Input.RequestParams.MerchantTradeNo.IndexOf("M_");
            //    tmp = Input.RequestParams.MerchantTradeNo.Substring(0, Index);
            //    //  tmpOrder = Convert.ToInt64(tmp);
            //    creditType = 4;
            //}
            //else if (Input.RequestParams.MerchantTradeNo.IndexOf("MA_") > -1)
            //{
            //    int Index = Input.RequestParams.MerchantTradeNo.IndexOf("MA_");
            //    tmp = Input.RequestParams.MerchantTradeNo.Substring(0, Index);
            //    //  tmpOrder = Convert.ToInt64(tmp);
            //    creditType = 5;
            //}
            SPInput_InsTrade SPInput = new SPInput_InsTrade()
            {
                amount = Convert.ToInt32(Input.RequestParams.TradeAmount) / 100,        //台新奇妙的規則  金額都要除100才是正確的金額
                OrderNo = tmpOrder,
                CreditType = creditType,
                LogID = 0,
                MerchantTradeNo = Input.RequestParams.MerchantTradeNo,
                MemberID = IDNO,
                CardToken = Input.RequestParams.CardToken
            };

            new WebAPILogCommon().InsCreditAuthData(SPInput, ref flag, ref errCode, ref lstError);


            if (flag)
            {
                output = DoCreditCardAuthSend(Input).Result;
                if (output.RtnCode == "1000")
                {
                    //if (output.Data == null)
                    //{
                    //    flag = false;
                    //}
                }
                else
                {
                    flag = false;
                }
            }

            return flag;
        }

        /// <summary>
        /// 2021/10/26 加入自動關帳邏輯
        /// </summary>
        /// <param name="wsInput"></param>
        /// <param name="IDNO"></param>
        /// <param name="AutoClosed"></param>
        /// <param name="errCode"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoCreditCardAuthV3(PartOfCreditCardAuth wsInput, string IDNO, int AutoClosed, string funName, string InsUser, ref string errCode, ref WebAPIOutput_Auth output, int AuthType = 0)
        {
            bool flag = true;
            string ori = string.Format("request={0}&apikey={1}", Newtonsoft.Json.JsonConvert.SerializeObject(wsInput), apikey);
            string checksum = GenerateSign(ori);
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            WebAPIInput_Auth Input = new WebAPIInput_Auth()
            {
                ApiVer = wsInput.ApiVer,
                ApposId = wsInput.ApposId,
                Random = wsInput.Random,
                RequestParams = wsInput.RequestParams,
                CheckSum = checksum,
                TimeStamp = wsInput.TimeStamp

            };
            var orderInfo = GetOrderInfoFromMerchantTradeNo(Input.RequestParams.MerchantTradeNo);
            int creditType = orderInfo.creditType;
            Int64 tmpOrder = orderInfo.OrderNo;

            SPInput_InsTradeForClose SPInput = new SPInput_InsTradeForClose()
            {
                amount = Convert.ToInt32(Input.RequestParams.TradeAmount) / 100,        //台新奇妙的規則  金額都要除100才是正確的金額
                OrderNo = tmpOrder,
                CreditType = creditType,
                LogID = 0,
                MerchantTradeNo = Input.RequestParams.MerchantTradeNo,
                MemberID = IDNO,
                CardToken = Input.RequestParams.CardToken,
                AutoClose = AutoClosed,
                AuthType = AuthType,
                MerchantID = wsInput.ApposId,
            };

            new WebAPILogCommon().InsCreditAuthDataforClose(SPInput, ref flag, ref errCode, ref lstError);

            if (flag)
            {
                output = DoCreditCardAuthSendForClose(Input, AutoClosed, AuthType, funName, InsUser).Result;
                if (output.RtnCode == "1000")
                {
                    //if (output.Data == null)
                    //{
                    //    flag = false;
                    //}
                }
                else
                {
                    flag = false;
                }
            }

            return flag;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<WebAPIOutput_Auth> DoCreditCardAuthSend(WebAPIInput_Auth input)
        {

            string Site = ECBaseURL + Auth;
            WebAPIOutput_Auth output = null;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Site);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.KeepAlive = false;
            //SetHeaderValue(request.Headers, "Connection", "close");
            request.Timeout = 78000;
            //設定刷卡逾時設定15秒
            //if (Site.ToUpper().Contains("AUTH"))
            //{
            //    request.Timeout = 15000;
            //}
            try
            {

                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                string postBody = JsonConvert.SerializeObject(input);//將匿名物件序列化為json字串
                byte[] byteArray = Encoding.UTF8.GetBytes(postBody);//要發送的字串轉為byte[]

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);
                    reqStream.Dispose();
                }



                //發出Request
                string responseStr = "";
                using (WebResponse response = request.GetResponse())
                {

                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseStr = reader.ReadToEnd();
                        RTime = DateTime.Now;
                        output = JsonConvert.DeserializeObject<WebAPIOutput_Auth>(responseStr);

                        //20201125紀錄接收資料
                        logger.Trace(responseStr);
                        reader.Close();
                        reader.Dispose();
                    }

                    //增加關閉連線的呼叫
                    response.Close();
                    response.Dispose();
                }
                //增加關閉連線的呼叫
                //HttpWebRequest requestClose = (HttpWebRequest)WebRequest.Create(Site);
                //requestClose.Connection = "Close";
                //HttpWebResponse myHttpWebResponse = (HttpWebResponse)requestClose.GetResponse();
                //myHttpWebResponse.Close();
                //request.Abort();
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_Auth()
                {
                    RtnCode = "0",
                    RtnMessage = ex.Message
                };
            }
            finally
            {
                SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                {
                    MKTime = MKTime,
                    UPDTime = RTime,
                    WebAPIInput = JsonConvert.SerializeObject(input),
                    WebAPIName = "Auth",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = ECBaseURL + Auth
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);

                #region 更新刷卡結果
                string tmp;
                Int64 tmpOrder = 0;
                int creditType = 99;
                if (input.RequestParams.MerchantTradeNo.IndexOf("F_") > -1)
                {
                    int Index = input.RequestParams.MerchantTradeNo.IndexOf("F_");
                    tmp = input.RequestParams.MerchantTradeNo.Substring(0, Index);
                    tmpOrder = Convert.ToInt64(tmp);
                    creditType = 0;
                }
                else if (input.RequestParams.MerchantTradeNo.IndexOf("P_") > -1)
                {
                    int Index = input.RequestParams.MerchantTradeNo.IndexOf("P_");
                    tmp = input.RequestParams.MerchantTradeNo.Substring(0, Index);
                    //  tmpOrder = Convert.ToInt64(tmp);
                    creditType = 1;
                }
                else if (input.RequestParams.MerchantTradeNo.IndexOf("E_") > -1)
                {
                    int Index = input.RequestParams.MerchantTradeNo.IndexOf("E_");
                    tmp = input.RequestParams.MerchantTradeNo.Substring(0, Index);
                    // tmpOrder = Convert.ToInt64(tmp);
                    creditType = 2;
                }
                else if (input.RequestParams.MerchantTradeNo.IndexOf("G_") > -1)
                {
                    int Index = input.RequestParams.MerchantTradeNo.IndexOf("G_");
                    tmp = input.RequestParams.MerchantTradeNo.Substring(0, Index);
                    //  tmpOrder = Convert.ToInt64(tmp);
                    creditType = 3;
                }
                else if (input.RequestParams.MerchantTradeNo.IndexOf("M_") > -1)
                {
                    int Index = input.RequestParams.MerchantTradeNo.IndexOf("M_");
                    tmp = input.RequestParams.MerchantTradeNo.Substring(0, Index);
                    //  tmpOrder = Convert.ToInt64(tmp);
                    creditType = 4;
                }
                else if (input.RequestParams.MerchantTradeNo.IndexOf("MA_") > -1)
                {
                    int Index = input.RequestParams.MerchantTradeNo.IndexOf("MA_");
                    tmp = input.RequestParams.MerchantTradeNo.Substring(0, Index);
                    //  tmpOrder = Convert.ToInt64(tmp);
                    creditType = 5;
                }
                //20211116 ADD BY ADAM REASON.春節定金
                else if (input.RequestParams.MerchantTradeNo.IndexOf("D_") > -1)
                {
                    int Index = input.RequestParams.MerchantTradeNo.IndexOf("D_");
                    tmp = input.RequestParams.MerchantTradeNo.Substring(0, Index);
                    //  tmpOrder = Convert.ToInt64(tmp);
                    creditType = 6;
                }
                SPInput_UpdTrade UpdInput = new SPInput_UpdTrade()
                {
                    LogID = 0,
                    OrderNo = tmpOrder,
                    MerchantTradeNo = input.RequestParams.MerchantTradeNo
                };
                if (output.RtnCode == "0")
                {
                    UpdInput.IsSuccess = -2;
                }
                else
                {
                    if (output.RtnCode == "1000")
                    {
                        DateTime process;
                        DateTime.TryParseExact(output.ResponseParams.ResultData.ServiceTradeDate + output.ResponseParams.ResultData.ServiceTradeTime, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out process);
                        if (output.ResponseParams.ResultCode == "1000")
                        {
                            UpdInput.IsSuccess = 1;

                        }
                        else
                        {
                            UpdInput.IsSuccess = -1;

                        }
                        UpdInput.MerchantMemberID = output.ResponseParams.ResultData.MemberId == null ? "" : output.ResponseParams.ResultData.MemberId;
                        UpdInput.process_date = process;
                        UpdInput.AUTHAMT = Convert.ToInt32(output.ResponseParams.ResultData.PayAmount) / 100;

                        //UpdInput.AuthIdResp = Convert.ToInt32(output.ResponseParams.ResultData.AuthIdResp);
                        try
                        {
                            UpdInput.AuthIdResp = Convert.ToInt32(output.ResponseParams.ResultData.AuthIdResp == "" ? "0" : output.ResponseParams.ResultData.AuthIdResp);
                        }
                        catch (Exception ex)
                        {
                            UpdInput.AuthIdResp = 0;
                            logger.Trace("更新刷卡結果Param:" + JsonConvert.SerializeObject(output) + ",ExceptionMessage:" + ex.Message);
                        }
                        UpdInput.CardNumber = output.ResponseParams.ResultData.CardNumber;
                        UpdInput.RetCode = output.ResponseParams.ResultCode;
                        //UpdInput.RetMsg = output.RtnMessage;
                        UpdInput.RetMsg = output.ResponseParams.ResultMessage;      //20210106 ADD BY ADAM REASON.
                        UpdInput.TaishinTradeNo = output.ResponseParams.ResultData.ServiceTradeNo;
                    }
                    else
                    {
                        UpdInput.IsSuccess = -2;
                        UpdInput.RetCode = output.RtnCode;
                        UpdInput.RetMsg = output.RtnMessage;
                    }
                    new WebAPILogCommon().UpdCreditAuthData(UpdInput, ref flag, ref errCode, ref lstError);
                }
                #endregion
                //增加關閉Request的處理
                request.Abort();
            }



            return output;
        }


        public async Task<WebAPIOutput_Auth> DoCreditCardAuthSendForClose(WebAPIInput_Auth input, int AutoClose, int AuthType, string FunName, string InsUser)
        {
            WebAPIOutput_Auth output = null;
            string Site = RelayStatus == "0" ? $"{ECBaseURL}{Auth}" : $"{RelayBaseURL}{RelayPostApi}";
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Site);
            request.Method = "POST";
            request.ContentType = "application/json";
            //request.KeepAlive = true;
            request.KeepAlive = false;
            SetHeaderValue(request.Headers, "Connection", "close");
            request.Timeout = 60000;
            //設定刷卡逾時設定15秒
            //if (Site.ToUpper().Contains("AUTH"))
            //{
            //    request.Timeout = 15000;
            //}
            try
            {
                if (CreditCardTest == "0")
                {
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    string body = JsonConvert.SerializeObject(input);//將匿名物件序列化為json字串
                    string postBody = "";
                    #region 中繼API啟用判斷
                    if (RelayStatus == "0") // 0:不啟用
                    {
                        postBody = body;
                    }
                    else
                    {
                        WebAPIInput_RelayPost relayPostinput = new WebAPIInput_RelayPost()
                        {
                            BaseUrl = "TaishinECBaseURL",
                            ApiUrl = "Auth",
                            RequestData = new AESEncrypt().doEncrypt(relayEnKey, relayEnSalt, body)
                        };
                        
                        postBody = JsonConvert.SerializeObject(relayPostinput);
                    }
                    #endregion

                    byte[] byteArray = Encoding.UTF8.GetBytes(postBody);//要發送的字串轉為byte[]

                    using (Stream reqStream = request.GetRequestStream())
                    {
                        reqStream.Write(byteArray, 0, byteArray.Length);
                        reqStream.Dispose();
                    }
                    //發出Request
                    string responseStr = "";
                    using (WebResponse response = request.GetResponse())
                    {

                        using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                        {
                            responseStr = reader.ReadToEnd();
                            RTime = DateTime.Now;

                            if (RelayStatus == "0") // 0:不啟用
                            {
                                output = JsonConvert.DeserializeObject<WebAPIOutput_Auth>(responseStr);
                            }
                            else
                            {
                                var result = JsonConvert.DeserializeObject<WebAPIOutput_RelayPost>(responseStr);                               
                                if (result.IsSuccess)
                                {
                                    responseStr = "";
                                    responseStr = new AESEncrypt().doDecrypt(relayEnKey, relayEnSalt, result.ResponseData);
                                    output = JsonConvert.DeserializeObject<WebAPIOutput_Auth>(responseStr);
                                }
                                else
                                {
                                    output = new WebAPIOutput_Auth()
                                    {
                                        RtnCode = "0",
                                        RtnMessage = result.RtnMessage
                                    };

                                }
                            }


                            //20201125紀錄接收資料
                            logger.Trace(responseStr);
                            reader.Close();
                            reader.Dispose();
                        }

                        //增加關閉連線的呼叫
                        response.Close();
                        response.Dispose();
                    }
                }
                else
                {
                    output = ForTest(input.RequestParams.TradeAmount);
                }
            }


            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_Auth()
                {
                    RtnCode = "0",
                    RtnMessage = ex.Message

                };
            }
            finally
            {
                SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                {
                    MKTime = MKTime,
                    UPDTime = RTime,
                    WebAPIInput = JsonConvert.SerializeObject(input),
                    WebAPIName = "Auth",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = ECBaseURL + Auth
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);

                #region 更新刷卡結果
                var orderInfo = GetOrderInfoFromMerchantTradeNo(input.RequestParams.MerchantTradeNo);
                int creditType = orderInfo.creditType;
                Int64 tmpOrder = orderInfo.OrderNo;

                SPInput_UpdTradeForClose UpdInput = new SPInput_UpdTradeForClose()
                {
                    LogID = 0,
                    OrderNo = tmpOrder,
                    MerchantTradeNo = input.RequestParams.MerchantTradeNo,
                    ChkClose = (AutoClose == 1) ? 1 : 0,
                    CardType = 1,
                    ProName = FunName,
                    UserID = (InsUser == FunName) ? "" : ((InsUser.Length > 20) ? InsUser.Substring(0, 20) : InsUser),
                    AuthType = AuthType

                };
                if (output.RtnCode == "0")
                {
                    UpdInput.IsSuccess = -2;
                }
                else
                {
                    if (output.RtnCode == "1000")
                    {
                        DateTime process;
                        DateTime.TryParseExact(output.ResponseParams.ResultData.ServiceTradeDate + output.ResponseParams.ResultData.ServiceTradeTime, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out process);
                        if (output.ResponseParams.ResultCode == "1000")
                        {
                            UpdInput.IsSuccess = 1;
                        }
                        else
                        {
                            UpdInput.IsSuccess = -1;
                        }
                        UpdInput.MerchantMemberID = output.ResponseParams.ResultData.MemberId == null ? "" : output.ResponseParams.ResultData.MemberId;
                        UpdInput.process_date = process;
                        UpdInput.AUTHAMT = Convert.ToInt32(output.ResponseParams.ResultData.PayAmount) / 100;

                        try
                        {
                            UpdInput.AuthIdResp = Convert.ToInt32(output.ResponseParams.ResultData.AuthIdResp == "" ? "0" : output.ResponseParams.ResultData.AuthIdResp);
                        }
                        catch (Exception ex)
                        {
                            UpdInput.AuthIdResp = 0;
                            logger.Trace("更新刷卡結果Param:" + JsonConvert.SerializeObject(output) + ",ExceptionMessage:" + ex.Message);
                        }
                        UpdInput.CardNumber = output.ResponseParams.ResultData.CardNumber;
                        UpdInput.RetCode = output.ResponseParams.ResultCode;
                        //UpdInput.RetMsg = output.RtnMessage;
                        UpdInput.RetMsg = output.ResponseParams.ResultMessage;      //20210106 ADD BY ADAM REASON.
                        UpdInput.TaishinTradeNo = output.ResponseParams.ResultData.ServiceTradeNo;
                    }
                    else
                    {
                        UpdInput.IsSuccess = -2;
                        UpdInput.RetCode = output.RtnCode;
                        UpdInput.RetMsg = output.RtnMessage;
                    }
                    new WebAPILogCommon().UpdCreditAuthDataForClose(UpdInput, ref flag, ref errCode, ref lstError);
                }
                #endregion
                //增加關閉Request的處理
                request.Abort();
            }
            return output;
        }

        #endregion
        #region 退貨（刷退）
        public bool DoCreditRefund(PartOfECRefund wsInput, Int64 tmpOrder, string IDNO, ref string errCode, ref string errMsg, ref WebAPIOutput_ECRefund output)
        {
            bool flag = true;
            string ori = string.Format("request={0}&apikey={1}", Newtonsoft.Json.JsonConvert.SerializeObject(wsInput), apikey);
            string checksum = GenerateSign(ori);
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            WebAPIInput_EC_Refund Input = new WebAPIInput_EC_Refund()
            {
                ApiVer = wsInput.ApiVer,
                ApposId = wsInput.ApposId,
                Random = wsInput.Random,
                RequestParams = wsInput.RequestParams,
                CheckSum = checksum,
                TimeStamp = wsInput.TimeStamp

            };
            SPInput_InsTrade SPInput = new SPInput_InsTrade()
            {
                amount = Convert.ToInt32(Input.RequestParams.TradeAmount) / 100,        //台新奇妙的規則  金額都要除100才是正確的金額
                OrderNo = tmpOrder,
                CreditType = 66,
                LogID = 0,
                MerchantTradeNo = Input.RequestParams.MerchantTradeNo,
                MemberID = IDNO,
                CardToken = Input.RequestParams.CardToken,

            };

            Int64 TradeRefundID = new WebAPILogCommon().InsCreditRefundDataNew(SPInput, ref flag, ref errCode, ref lstError);

            output = DoCreditRefundSend(Input, TradeRefundID).Result;
            if (output.RtnCode == "1000")
            {
                if (output.ResponseParams.ResultCode == "1000")
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                    errCode = output.ResponseParams.ResultCode;
                    errMsg = output.ResponseParams.ResultMessage;

                }
                //if (output.Data == null)
                //{
                //    flag = false;
                //}
            }
            else
            {
                flag = false;
            }
            return flag;
        }
        public async Task<WebAPIOutput_ECRefund> DoCreditRefundSend(WebAPIInput_EC_Refund input, Int64 TradeRefundID)
        {
            string Site = ECBaseURL + ECRefund;
            bool CreditRefundFlag = true;
            WebAPIOutput_ECRefund output = null;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Site);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.KeepAlive = false;
            //SetHeaderValue(request.Headers, "Connection", "close");
            request.Timeout = 78000;
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                string postBody = JsonConvert.SerializeObject(input);//將匿名物件序列化為json字串
                byte[] byteArray = Encoding.UTF8.GetBytes(postBody);//要發送的字串轉為byte[]

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);
                    reqStream.Dispose();
                }



                //發出Request
                string responseStr = "";
                using (WebResponse response = request.GetResponse())
                {

                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseStr = reader.ReadToEnd();
                        RTime = DateTime.Now;
                        output = JsonConvert.DeserializeObject<WebAPIOutput_ECRefund>(responseStr);
                        reader.Close();
                        reader.Dispose();
                    }

                    //增加關閉連線的呼叫
                    response.Close();
                    response.Dispose();
                }
                //增加關閉連線的呼叫
                //HttpWebRequest requestClose = (HttpWebRequest)WebRequest.Create(Site);
                //requestClose.Connection = "Close";
                //HttpWebResponse myHttpWebResponse = (HttpWebResponse)requestClose.GetResponse();
                //myHttpWebResponse.Close();
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_ECRefund()
                {
                    RtnCode = "0",
                    RtnMessage = ex.Message
                };
            }
            finally
            {
                SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                {
                    MKTime = MKTime,
                    UPDTime = RTime,
                    WebAPIInput = JsonConvert.SerializeObject(input),
                    WebAPIName = "Refund",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = ECBaseURL + ECRefund
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
                Int64 tmpOrder = 0;
                int Index = input.RequestParams.MerchantTradeNo.IndexOf("R_");
                string tmp = input.RequestParams.MerchantTradeNo.Substring(0, Index);
                tmpOrder = Convert.ToInt64(tmp);
                int creditType = 66;
                SPInput_UpdTrade UpdInput = new SPInput_UpdTrade()
                {
                    LogID = 0,
                    OrderNo = tmpOrder,
                    MerchantTradeNo = input.RequestParams.MerchantTradeNo,
                    TradeRefundID = TradeRefundID
                };

                //更新刷退結果
                if (output.RtnCode == "1000")
                {
                    if (output.ResponseParams.ResultCode == "1000")
                    {
                        DateTime process;
                        DateTime.TryParseExact(output.ResponseParams.ResultData.ServiceTradeDate + output.ResponseParams.ResultData.ServiceTradeTime, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out process);
                        UpdInput.IsSuccess = 1;
                        UpdInput.MerchantMemberID = output.ResponseParams.ResultData.MemberId;
                        UpdInput.process_date = process;
                        UpdInput.AUTHAMT = Convert.ToInt32(output.ResponseParams.ResultData.TradeAmount) / 100;
                        UpdInput.CardNumber = output.ResponseParams.ResultData.CardNumber;
                        UpdInput.RetCode = output.RtnCode;
                        UpdInput.RetMsg = output.RtnMessage;
                        UpdInput.TaishinTradeNo = output.ResponseParams.ResultData.ServiceTradeNo;
                        UpdInput.AuthIdResp = Convert.ToInt32(output.ResponseParams.ResultData.AuthIdResp == "" ? "0" : output.ResponseParams.ResultData.AuthIdResp);
                        try
                        {

                            UpdInput.AuthIdResp = Convert.ToInt32(output.ResponseParams.ResultData.AuthIdResp == "" ? "0" : output.ResponseParams.ResultData.AuthIdResp);
                        }
                        catch (Exception ex)
                        {
                            UpdInput.AuthIdResp = 0;
                            logger.Trace("更新刷退結果Param:" + JsonConvert.SerializeObject(output) + ",ExceptionMessage:" + ex.Message);
                        }
                        UpdInput.CardNumber = output.ResponseParams.ResultData.CardNumber;
                        UpdInput.RetCode = output.RtnCode;
                        UpdInput.RetMsg = output.RtnMessage;
                        UpdInput.TaishinTradeNo = output.ResponseParams.ResultData.ServiceTradeNo;
                    }
                    else
                    {
                        CreditRefundFlag = false;


                        DateTime process = DateTime.Now;

                        UpdInput.IsSuccess = -2;
                        UpdInput.MerchantMemberID = "";
                        UpdInput.process_date = process;
                        UpdInput.AUTHAMT = 0;
                        UpdInput.CardNumber = "";
                        UpdInput.RetCode = output.ResponseParams.ResultCode;
                        UpdInput.RetMsg = output.ResponseParams.ResultMessage;
                        UpdInput.TaishinTradeNo = "";
                        UpdInput.AuthIdResp = 0;
                    }

                    //UpdInput.AuthIdResp = Convert.ToInt32(output.ResponseParams.ResultData.AuthIdResp);


                }
                else
                {
                    UpdInput.IsSuccess = -2;
                    UpdInput.RetCode = output.ResponseParams.ResultCode;
                    UpdInput.RetMsg = output.ResponseParams.ResultMessage;
                }
                new WebAPILogCommon().UpdCreditRefundData(UpdInput, ref flag, ref errCode, ref lstError);
                //增加關閉Request的處理
                request.Abort();

            }


            return output;
        }
        #endregion
        /// <summary>
        /// 產生CardData IV
        /// </summary>
        /// <param name="Timestamp"></param>
        /// <returns></returns>
        public string GenerateCardDataIV(long Timestamp)
        {
            string ACode, BCode, CCode, DCode, ECode = "";
            BCode = (Timestamp % 10000).ToString().PadLeft(4, '0'); //除10000取餘數，等同於抓最後四碼
            CCode = Cal(BCode);
            DCode = Cal(CCode);
            for (int i = 0; i < 4; i++)
            {
                int value = (Convert.ToInt32(BCode.Substring(i, 1)) + Convert.ToInt32(CCode.Substring(i, 1)) + Convert.ToInt32(DCode.Substring(i, 1))) % 10;
                ECode += value.ToString();
            }
            return BCode + CCode + DCode + ECode;

        }
        /// <summary>
        /// 產生CardData加密
        /// </summary>
        /// <param name="MemberId">會員編號</param>
        /// <param name="CellPhone">手機號碼</param>
        /// <param name="CardNumber">信用卡號</param>
        /// <param name="ExpDate">信用卡有效日期YYMM</param>
        /// <param name="Cvv2">信用卡末三碼</param>
        /// <param name="CardName">卡片名稱</param>
        /// <param name="Timestamp">用來產生電文的Timestamp（10碼）</param>
        /// <returns></returns>
        public string GenerateCardData(CardData data, string IV)
        {
            AESEncrypt encrypt = new AESEncrypt();
            return encrypt.doEncrypt(apikey, IV, JsonConvert.SerializeObject(data));

        }
        private string Cal(string num)
        {
            string output = "";
            int len = num.Length;
            int sum = 0;
            for (int i = 0; i < len; i++)
            {
                int baseValue = Convert.ToInt32(num.Substring(i, 1));
                if (baseValue > 0)
                {
                    sum += Convert.ToInt32(((Math.Pow(baseValue, 2)) % 10) * Math.Pow(10, 3 - i));
                }

            }
            return sum.ToString().PadLeft(3, '0');
        }
        /// <summary>
        /// 產生簽章
        /// </summary>
        /// <param name="ori"></param>
        /// <returns></returns>
        public string GenerateSign(string ori)
        {
            System.Security.Cryptography.SHA256 sha256 = new System.Security.Cryptography.SHA256CryptoServiceProvider();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(ori);
            byte[] hash = sha256.ComputeHash(bytes);
            string sign = Convert.ToBase64String(hash).ToUpper();
            return sign;
        }

        private (int creditType, string OrderString, Int64 OrderNo) GetOrderInfoFromMerchantTradeNo(string ori)
        {
            (int creditType, string OrderString, Int64 OrderNo) orderInfo = (99, "", 0);

            var payTypeInfos = GetCreditCardPayInfoColl();

            //Dictionary<string, int> typeDic = new Dictionary<string, int>()
            //{
            //    {"F_",0},{"P_",1},{"E_",2},{"G_",3},{"M_",4},{"MA_",5},{"W_",6}
            //};

            Dictionary<string, int> typeDic = payTypeInfos.ToDictionary(p => p.PayTypeCode, p => p.PayType);

            foreach (KeyValuePair<string, int> type in typeDic)
            {
                int Index = ori.IndexOf(type.Key);
                if (Index > -1)
                {
                    orderInfo.creditType = type.Value;
                    orderInfo.OrderString = ori.Substring(0, Index);
                    if (orderInfo.creditType == 0 || orderInfo.creditType == 6) //租金or訂金
                    {
                        orderInfo.OrderNo = Convert.ToInt64(orderInfo.OrderString);
                    }
                    break;
                }
            }

            return orderInfo;
        }


        public List<CreditCardPayInfo> GetCreditCardPayInfoColl()
        {
            List<CreditCardPayInfo> CreditCardPayInfoColl = new List<CreditCardPayInfo>()
            {
                new CreditCardPayInfo{ PayType = 0,PayTypeStr = "租金",PayTypeCode="F_"},
                new CreditCardPayInfo{ PayType = 1,PayTypeStr = "罰金",PayTypeCode="P_"},//沒在用
                new CreditCardPayInfo{ PayType = 2,PayTypeStr = "eTag",PayTypeCode="E_"},//沒在用
                new CreditCardPayInfo{ PayType = 3,PayTypeStr = "補繳",PayTypeCode="G_"},
                new CreditCardPayInfo{ PayType = 4,PayTypeStr = "訂閱",PayTypeCode="M_"},
                new CreditCardPayInfo{ PayType = 5,PayTypeStr = "訂閱",PayTypeCode="MA_"},
                new CreditCardPayInfo{ PayType = 6,PayTypeStr = "春節訂金",PayTypeCode="D_"},
                new CreditCardPayInfo{ PayType = 7,PayTypeStr = "錢包",PayTypeCode="W_"},
            };

            return CreditCardPayInfoColl;
        }

        private WebAPIOutput_Auth ForTest(string TradeAmount)
        {
            return ForTestTrue(TradeAmount);
            //Random rnd = new Random();
            //int result = rnd.Next(1, 1);

            //if (result == 0)
            //{
            //    return ForTestTrue(TradeAmount);
            //}
            //else
            //{
            //    return ForTestFalse(TradeAmount);
            //}
        }

        private WebAPIOutput_Auth ForTestTrue(string TradeAmount)
        {
            WebAPIOutput_Auth output =
            new WebAPIOutput_Auth()
            {
                RtnCode = "1000",
                RtnMessage = "",

                ResponseParams = new AuthResponseParams
                {
                    ResultCode = "1000",
                    ResultMessage = "交易成功",
                    ResultData = new Domain.WebAPI.output.Taishin.ResultData.AuthResultData
                    {
                        CardNumber = "****************",
                        ServiceTradeDate = DateTime.Now.ToString("yyyyMMdd"),
                        ServiceTradeTime = DateTime.Now.ToString("HHmmss"),
                        ServiceTradeNo = Guid.NewGuid().ToString().Replace("-", ""),
                        PayAmount = TradeAmount,
                        AuthIdResp = "0"

                    }
                },

            };

            return output;

        }

        private WebAPIOutput_Auth ForTestFalse(string TradeAmount)
        {
            WebAPIOutput_Auth output =
            new WebAPIOutput_Auth()
            {
                RtnCode = "100",
                RtnMessage = "",

                ResponseParams = new AuthResponseParams
                {
                    ResultCode = "100",
                    ResultMessage = "交易失敗",
                    ResultData = new Domain.WebAPI.output.Taishin.ResultData.AuthResultData
                    {
                        CardNumber = "****************",
                        ServiceTradeDate = DateTime.Now.ToString("yyyyMMdd"),
                        ServiceTradeTime = DateTime.Now.ToString("HHmmss"),
                        ServiceTradeNo = Guid.NewGuid().ToString().Replace("-", ""),
                        PayAmount = "0",
                        AuthIdResp = "3"

                    }
                },

            };

            return output;

        }
    }


    public class BankCardCache
    {
        public string BankNm { get; set; }
        public string CacheTime { get; set; }//yyyyMMddHHmmss
        public string IDNO { get; set; }
        public string apiJson { get; set; }//回傳字串
        public string errCode { get; set; }
    }
}
