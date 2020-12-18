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
        private string BaseURL = ConfigurationManager.AppSettings["TaishinBaseURL"].ToString();                     //台新base網址
        private string ECBaseURL= ConfigurationManager.AppSettings["TaishinECBaseURL"].ToString();
        private string GetCardPage = ConfigurationManager.AppSettings["GetCardPage"].ToString();                    //取得綁卡網址
        private string GetCreditCardStatus = ConfigurationManager.AppSettings["GetCreditCardStatus"].ToString();    //取得綁卡狀態
        private string DeleteCreditCardAuth = ConfigurationManager.AppSettings["DeleteCreditCardAuth"].ToString();  //刪除綁卡
        private string GetCreditCardList = ConfigurationManager.AppSettings["GetCreditCardList"].ToString();        //取得綁卡列表
        private string GetPaymentInfo = ConfigurationManager.AppSettings["GetPaymentInfo"].ToString();              //查詢訂單狀態
        private string ECRefund = ConfigurationManager.AppSettings["ECRefund"].ToString();                          //退貨
        private string Auth = ConfigurationManager.AppSettings["Auth"].ToString();              //直接授權   
        private string AzureAPIBaseURL = ConfigurationManager.AppSettings["AzureAPIBaseUrl"].ToString();

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
        public bool DoBind(WebAPIInput_Base wsInput,ref string errCode,ref WebAPIOutput_Base output)
        {
            bool flag = true;
            string ori = string.Format("request={0}&apikey={1}", Newtonsoft.Json.JsonConvert.SerializeObject(wsInput), apikey);
            string checksum = GenerateSign(ori);
            
            WebAPIInput_Bind Input = new WebAPIInput_Bind()
            {
                ApiVer = wsInput.ApiVer,
                ApposId = wsInput.ApposId,
                Random = wsInput.Random,
                RequestParams = wsInput.RequestParams,
                CheckSum = checksum,
                TimeStamp=wsInput.TimeStamp
            };
           
            
            output = DoBindSend(Input).Result;
            if (output.RtnCode=="1000")
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
        /// 執行取得綁卡網址
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<WebAPIOutput_Base> DoBindSend(WebAPIInput_Bind input)
        {
            string Site = BaseURL + GetCardPage;
            //Site = AzureAPIBaseURL + @"api/TestTaishiBU";
            WebAPIOutput_Base output = null;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Site);
            request.Method = "POST";
            request.ContentType = "application/json";
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                string postBody = JsonConvert.SerializeObject(input);//將匿名物件序列化為json字串
                byte[] byteArray = Encoding.UTF8.GetBytes(postBody);//要發送的字串轉為byte[]

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);
                }



                //發出Request
                string responseStr = "";
                using (WebResponse response = request.GetResponse())
                {

                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseStr = reader.ReadToEnd();
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

                        //20201125紀錄接收資料
                        logger.Trace(responseStr);
                    }

                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_Base()
                {
                    RtnCode="0",
                    RtnMessage=ex.Message
                };
            }
            finally
            {
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
                TransNo=wsInput.TransNo
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
        
        public bool DoGetCreditCardListCache(PartOfGetCreditCardList wsInput, ref string errCode, ref WebAPIOutput_GetCreditCardList output,bool refresh=false)
        {//hack: DoGetCreditCardListCache
            bool flag = false;
            string cacheNm = "BankCardCache";
            string bankNm = "TSIB";
            int cacheMins = 10;
            bool reCall = false;

            var cItems = new List<BankCardCache>();
            var m = new BankCardCache();

            if (_cache != null && _cache[cacheNm] != null)
               cItems = (List<BankCardCache>)_cache[cacheNm];           

            if (cItems != null && cItems.Count() > 0)
                m = cItems.Where(x => x.BankNm == bankNm && x.IDNO == wsInput.RequestParams.MemberId).FirstOrDefault();

            if (refresh == false)
            {
                if (_cache != null && _cache[cacheNm] != null)
                {
                    if(m != null)
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
            return DoGetCreditCardListCache(wsInput, ref errCode, ref output);
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
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                string postBody = JsonConvert.SerializeObject(input);//將匿名物件序列化為json字串
                byte[] byteArray = Encoding.UTF8.GetBytes(postBody);//要發送的字串轉為byte[]

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);
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
                    }

                }
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
            string ori = string.Format("request={0}&apikey={1}", Newtonsoft.Json.JsonConvert.SerializeObject(wsInput), apikey);
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
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                string postBody = JsonConvert.SerializeObject(input);//將匿名物件序列化為json字串
                byte[] byteArray = Encoding.UTF8.GetBytes(postBody);//要發送的字串轉為byte[]

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);
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
                    }

                }
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
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                string postBody = JsonConvert.SerializeObject(input);//將匿名物件序列化為json字串
                byte[] byteArray = Encoding.UTF8.GetBytes(postBody);//要發送的字串轉為byte[]

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);
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
                    }

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
                    WebAPIName = "GetPaymentInfo",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + GetPaymentInfo
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
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
            string tmp="";
            Int64 tmpOrder = 0;
            int creditType = 99;
    
                if (Input.RequestParams.MerchantTradeNo.IndexOf("F_") > -1)
                {
                int Index = Input.RequestParams.MerchantTradeNo.IndexOf("F_");
                    tmp = Input.RequestParams.MerchantTradeNo.Substring(0,Index);
                    tmpOrder = Convert.ToInt64(tmp);
                    creditType = 0;
                }
                else if (Input.RequestParams.MerchantTradeNo.IndexOf("P_") > -1)
                {
                int Index = Input.RequestParams.MerchantTradeNo.IndexOf("P_");
                tmp = Input.RequestParams.MerchantTradeNo.Substring(0,Index);
              //  tmpOrder = Convert.ToInt64(tmp);
                    creditType = 1;
                }
                else if (Input.RequestParams.MerchantTradeNo.IndexOf("E_") > -1)
                {
                int Index = Input.RequestParams.MerchantTradeNo.IndexOf("E_");
                tmp = Input.RequestParams.MerchantTradeNo.Substring(0,Index);
               // tmpOrder = Convert.ToInt64(tmp);
                    creditType = 2;
                }
                else if (Input.RequestParams.MerchantTradeNo.IndexOf("G_") > -1)
                {
                int Index = Input.RequestParams.MerchantTradeNo.IndexOf("G_");
                tmp = Input.RequestParams.MerchantTradeNo.Substring(0,Index );
              //  tmpOrder = Convert.ToInt64(tmp);
                    creditType = 3;
                }
                SPInput_InsTrade SPInput = new SPInput_InsTrade()
                {
                    amount = Convert.ToInt32(Input.RequestParams.TradeAmount) / 100,
                    OrderNo = tmpOrder,
                    CreditType = creditType,
                    LogID = 0,
                    MerchantTradeNo = Input.RequestParams.MerchantTradeNo,
                    CardToken=Input.RequestParams.CardToken,
                    MemberID=tmp
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
        public bool DoCreditCardAuthV2(PartOfCreditCardAuth wsInput,string IDNO, ref string errCode, ref WebAPIOutput_Auth output)
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
            string[] tmp;
            Int64 tmpOrder = 0;
            int creditType = 99;
            if (Input.RequestParams.MerchantTradeNo.IndexOf('F') > -1)
            {
                tmp = Input.RequestParams.MerchantTradeNo.Split('F');
                tmpOrder = Convert.ToInt64(tmp[0]);
                creditType = 0;
            }
            else if (Input.RequestParams.MerchantTradeNo.IndexOf('P') > -1)
            {
                tmp = Input.RequestParams.MerchantTradeNo.Split('P');
                tmpOrder = Convert.ToInt64(tmp[0]);
                creditType = 1;
            }
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
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                string postBody = JsonConvert.SerializeObject(input);//將匿名物件序列化為json字串
                byte[] byteArray = Encoding.UTF8.GetBytes(postBody);//要發送的字串轉為byte[]

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);
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
                    }

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
                        DateTime.TryParseExact(output.ResponseParams.ResultData.ServiceTradeDate+ output.ResponseParams.ResultData.ServiceTradeTime, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out process);
                        UpdInput.IsSuccess = 1;
                        UpdInput.MerchantMemberID = output.ResponseParams.ResultData.MemberId;
                        UpdInput.process_date = process;
                        UpdInput.AUTHAMT = Convert.ToInt32(output.ResponseParams.ResultData.PayAmount) / 100;
                        UpdInput.AuthIdResp = Convert.ToInt32(output.ResponseParams.ResultData.AuthIdResp);
                        UpdInput.CardNumber = output.ResponseParams.ResultData.CardNumber;
                        UpdInput.RetCode = output.RtnCode;
                        UpdInput.RetMsg = output.RtnMessage;
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
            }



            return output;
        }
        #endregion
        #region 退貨（刷退）
        public bool DoCreditRefund(PartOfECRefund wsInput, ref string errCode, ref WebAPIOutput_ECRefund output)
        {
            bool flag = true;
            string ori = string.Format("request={0}&apikey={1}", Newtonsoft.Json.JsonConvert.SerializeObject(wsInput), apikey);
            string checksum = GenerateSign(ori);

            WebAPIInput_EC_Refund Input = new WebAPIInput_EC_Refund()
            {
                ApiVer = wsInput.ApiVer,
                ApposId = wsInput.ApposId,
                Random = wsInput.Random,
                RequestParams = wsInput.RequestParams,
                CheckSum = checksum,
                TimeStamp = wsInput.TimeStamp
                 
            };


            output = DoCreditRefundSend(Input).Result;
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
        public async Task<WebAPIOutput_ECRefund> DoCreditRefundSend(WebAPIInput_EC_Refund input)
        {
            string Site = ECBaseURL + ECRefund;
            WebAPIOutput_ECRefund output = null;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Site);
            request.Method = "POST";
            request.ContentType = "application/json";
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                string postBody = JsonConvert.SerializeObject(input);//將匿名物件序列化為json字串
                byte[] byteArray = Encoding.UTF8.GetBytes(postBody);//要發送的字串轉為byte[]

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);
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
                    }

                }
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
            BCode = (Timestamp % 10000).ToString().PadLeft(4,'0'); //除10000取餘數，等同於抓最後四碼
            CCode = Cal(BCode);
            DCode = Cal(CCode);
            for (int i = 0; i <4; i++)
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
        public string GenerateCardData(CardData data,string IV)
        {
            AESEncrypt encrypt = new AESEncrypt();
            return encrypt.doEncrypt(apikey, IV, JsonConvert.SerializeObject(data));

        }
        private string Cal(string num)
        {
            string output = "";
            int len = num.Length;
            int sum = 0;
            for(int i = 0; i <len; i++)
            {
                int baseValue = Convert.ToInt32(num.Substring(i, 1));
                if (baseValue > 0)
                {
                    sum += Convert.ToInt32(((Math.Pow(baseValue, 2)) % 10) * Math.Pow(10, 3 - i));
                }
                
            }
            return sum.ToString().PadLeft(3,'0');
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
