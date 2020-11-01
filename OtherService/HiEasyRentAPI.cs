﻿using Domain.SP.Input.OtherService.Common;
using Domain.WebAPI.Input.HiEasyRentAPI;
using Domain.WebAPI.output.HiEasyRentAPI;
using Newtonsoft.Json;
using OtherService.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebCommon;

namespace OtherService
{
    /// <summary>
    /// 和雲WebAPI
    /// </summary>
   public  class HiEasyRentAPI
    {
        protected string userid;
        protected string apikey;
        protected string BaseURL;
        protected string NPR060SaveURL; //060
        protected string NPR125SaveURL; //
        protected string NPR130SaveURL; //
        protected string NPR136SaveURL; //
        protected string NPR260SendURL; //簡訊發送
        protected string NPR270QueryURL; //點數查詢
        protected string NPR271QueryURL; //點數歷程查詢
        protected string NPR370CheckURL; //點數轉贈前檢查
        protected string NPR370SaveURL;  //進行轉贈
        protected string NPR330QueryURL; //欠費查詢
        protected string ETAG010QueryURL; //ETAG查詢
        protected string EinvBizURL;     //手機條碼檢核
        protected string NPR320QueryURL; //點數兌換
        protected string NPR350CheckURL;
        protected string MonthlyRentURL; //月租訂閱
        protected string connetStr;
        bool disposed = false;
        /// <summary>
        /// 建構式
        /// </summary>
        public HiEasyRentAPI()
        {
            apikey = ConfigurationManager.AppSettings.Get("HLCkey");
            connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
            userid = ConfigurationManager.AppSettings.Get("userid");
            BaseURL = (ConfigurationManager.AppSettings.Get("BaseURL") == null) ? "" : ConfigurationManager.AppSettings.Get("BaseURL").ToString();
            NPR060SaveURL = (ConfigurationManager.AppSettings.Get("NPR060SaveURL") == null) ? "" : ConfigurationManager.AppSettings.Get("NPR060SaveURL").ToString();
            NPR125SaveURL = (ConfigurationManager.AppSettings.Get("NPR125SaveURL") == null) ? "" : ConfigurationManager.AppSettings.Get("NPR125SaveURL").ToString();
            NPR130SaveURL = (ConfigurationManager.AppSettings.Get("NPR130SaveURL") == null) ? "" : ConfigurationManager.AppSettings.Get("NPR130SaveURL").ToString();
            NPR136SaveURL = (ConfigurationManager.AppSettings.Get("NPR136SaveURL") == null) ? "" : ConfigurationManager.AppSettings.Get("NPR136SaveURL").ToString();
            NPR260SendURL = (ConfigurationManager.AppSettings.Get("NPR260SendURL") == null) ? "" : ConfigurationManager.AppSettings.Get("NPR260SendURL").ToString();
            NPR270QueryURL = (ConfigurationManager.AppSettings.Get("NPR270QueryURL") == null) ? "" : ConfigurationManager.AppSettings.Get("NPR270QueryURL").ToString();
            NPR271QueryURL = (ConfigurationManager.AppSettings.Get("NPR271QueryURL") == null) ? "" : ConfigurationManager.AppSettings.Get("NPR271QueryURL").ToString();
            NPR370CheckURL = (ConfigurationManager.AppSettings.Get("NPR370CheckURL") == null) ? "" : ConfigurationManager.AppSettings.Get("NPR370CheckURL").ToString();
            NPR370SaveURL = (ConfigurationManager.AppSettings.Get("NPR370SaveURL") == null) ? "" : ConfigurationManager.AppSettings.Get("NPR370SaveURL").ToString();
            NPR330QueryURL = (ConfigurationManager.AppSettings.Get("NPR330QueryURL") == null) ? "" : ConfigurationManager.AppSettings.Get("NPR330QueryURL").ToString();
            ETAG010QueryURL = (ConfigurationManager.AppSettings.Get("ETAG010QueryURL") == null) ? "" : ConfigurationManager.AppSettings.Get("ETAG010QueryURL").ToString();
            EinvBizURL = (ConfigurationManager.AppSettings.Get("EinvBizURL") == null) ? "" : ConfigurationManager.AppSettings.Get("EinvBizURL").ToString();
            NPR320QueryURL = (ConfigurationManager.AppSettings.Get("NPR320QueryURL") == null) ? "" : ConfigurationManager.AppSettings.Get("NPR320QueryURL").ToString();
            MonthlyRentURL = (ConfigurationManager.AppSettings.Get("MonthlyRentURL") == null) ? "" : ConfigurationManager.AppSettings.Get("MonthlyRentURL").ToString();
        }
        /// <summary>
        /// 產生簽章
        /// </summary>
        /// <returns></returns>
        public string GenerateSig()
        {
            string EncryptStr = "";
            string sourceStr = apikey + userid + System.DateTime.Now.ToString("yyyyMMdd");
            ASCIIEncoding enc = new ASCIIEncoding();
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] shaHash = sha.ComputeHash(enc.GetBytes(sourceStr));
            EncryptStr = System.BitConverter.ToString(shaHash).Replace("-", string.Empty);
            return EncryptStr;
        }
        #region 發送簡訊
        public bool NPR260Send(string TARGET, string Message,string RENO, ref WebAPIOutput_NPR260Send output)
        {
            bool flag = true;
            WebAPIInput_NPR260Send input = new WebAPIInput_NPR260Send()
            {
                sig = GenerateSig(),
                user_id = userid,
                TARGET=TARGET,
                 MESSAGE=Message,
                 RENO=RENO
            };
            output = DoNPR260Send(input).Result;
            if (output.Result)
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
        /// 發送簡訊執行端
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<WebAPIOutput_NPR260Send> DoNPR260Send(WebAPIInput_NPR260Send input)
        {
            WebAPIOutput_NPR260Send output = null;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL+NPR260SendURL);
            request.Method = "POST";
            request.ContentType = "application/json";
            try
            {
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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_NPR260Send>(responseStr);
                    }

                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_NPR260Send()
                {

                    Message = "發生異常錯誤",
                    Result = false
                };
            }
            finally
            {
                SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                {
                    MKTime = MKTime,
                    UPDTime = RTime,
                    WebAPIInput = JsonConvert.SerializeObject(input),
                    WebAPIName = "NPR260Send",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL+NPR260SendURL
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }


            return output;
        }
        #endregion
        #region 點數查詢
        /// <summary>
        /// 點數查詢
        /// </summary>
        /// <param name="IDNO"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool NPR270Query(string IDNO, ref WebAPIOutput_NPR270Query output)
        {
            bool flag = false;
            WebAPIInput_NPR270Query input = new WebAPIInput_NPR270Query()
            {
                sig = GenerateSig(),
                user_id = userid,
                ID = IDNO
            };


            output = DoNPR270Query(input).Result;
            if (output.Result)
            {
                flag = true;
            }
            return flag;
        }
        /// <summary>
        /// 點數查詢
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<WebAPIOutput_NPR270Query> DoNPR270Query(WebAPIInput_NPR270Query input)
        {
            WebAPIOutput_NPR270Query output = null;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL+NPR270QueryURL);
            request.Method = "POST";
            request.ContentType = "application/json";
            try
            {
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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_NPR270Query>(responseStr);
                    }

                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_NPR270Query()
                {

                    Message = "發生異常錯誤",
                    Result = false
                };
            }
            finally
            {
                SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                {
                    MKTime = MKTime,
                    UPDTime = RTime,
                    WebAPIInput = JsonConvert.SerializeObject(input),
                    WebAPIName = "NPR270Query",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + NPR270QueryURL
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }


            return output;
        }
        #endregion
        #region 點數歷程查詢
        /// <summary>
        /// 點數歷程查詢
        /// </summary>
        /// <param name="IDNO"></param>
        /// <param name="SEQNO"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool NPR271Query(string IDNO, int SEQNO, ref WebAPIOutput_NPR271Query output)
        {
            bool flag = false;
            WebAPIInput_NPR271Query input = new WebAPIInput_NPR271Query()
            {
                sig = GenerateSig(),
                user_id = userid,
                ID = IDNO,
                SEQNO = SEQNO
            };

            output = DoNPR271Query(input).Result;
            if (output.Result)
            {
                flag = true;
            }
            return flag;
        }
        /// <summary>
        /// 點數歷程查詢
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<WebAPIOutput_NPR271Query> DoNPR271Query(WebAPIInput_NPR271Query input)
        {
            WebAPIOutput_NPR271Query output = null;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + NPR270QueryURL);
            request.Method = "POST";
            request.ContentType = "application/json";
            try
            {
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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_NPR271Query>(responseStr);
                    }

                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_NPR271Query()
                {

                    Message = "發生異常錯誤",
                    Result = false
                };
            }
            finally
            {
                SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                {
                    MKTime = MKTime,
                    UPDTime = RTime,
                    WebAPIInput = JsonConvert.SerializeObject(input),
                    WebAPIName = "NPR271Query",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + NPR271QueryURL
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }


            return output;
        }
        #endregion
        #region 點數轉贈前檢查
        /// <summary>
        /// 呼叫NPR370CHECK，確認此帳號可以轉贈點數
        /// </summary>
        /// <param name="IDNO">轉贈者身份證</param>
        /// <param name="Mobile">被轉贈者手機</param>
        /// <param name="Pointer">點數</param>
        /// <returns></returns>
        public bool NPR370Check(string IDNO, string Mobile, int Pointer, ref WebAPIOutput_NPR370Check output)
        {
            bool flag = true;
            WebAPIInput_NPR370Check input = new WebAPIInput_NPR370Check()
            {
                sig = GenerateSig(),
                user_id = userid,
                SourceId = IDNO,
                Mobile = Mobile,
                TransMins = Pointer
            };
            output = DoNPR370Check(input).Result;
            if (output.Result)
            {
                //if (output.Data==null)
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
        /// 呼叫webapi執行NPR370CHECK
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<WebAPIOutput_NPR370Check> DoNPR370Check(WebAPIInput_NPR370Check input)
        {
            WebAPIOutput_NPR370Check output = null;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + NPR370CheckURL);
            request.Method = "POST";
            request.ContentType = "application/json";
            try
            {
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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_NPR370Check>(responseStr);
                    }

                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_NPR370Check()
                {

                    Message = "發生異常錯誤",
                    Result = false
                };
            }
            finally
            {
                SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                {
                    MKTime = MKTime,
                    UPDTime = RTime,
                    WebAPIInput = JsonConvert.SerializeObject(input),
                    WebAPIName = "NPR370Check",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + NPR370CheckURL
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }


            return output;
        }
        #endregion
        #region 點數轉贈
        public bool NPR370Save(string IDNO, string TargetId, int Pointer, string GiftType,  ref WebAPIOutput_NPR370Save output)
        {
            bool flag = true;
            WebAPIInput_NPR370Save input = new WebAPIInput_NPR370Save()
            {
                sig = GenerateSig(),
                user_id = userid,
                SourceId = IDNO,
                TargetId = TargetId,
                TransMins = Pointer,
                GiftType = GiftType
            };
            output = DoNPR370Save(input).Result;
            if (output.Result)
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
        private async Task<WebAPIOutput_NPR370Save> DoNPR370Save(WebAPIInput_NPR370Save input)
        {
            WebAPIOutput_NPR370Save output = null;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + NPR370SaveURL);
            request.Method = "POST";
            request.ContentType = "application/json";
            try
            {
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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_NPR370Save>(responseStr);
                    }

                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_NPR370Save()
                {

                    Message = "發生異常錯誤",
                    Result = false
                };
            }
            finally
            {
                SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                {
                    MKTime = MKTime,
                    UPDTime = RTime,
                    WebAPIInput = JsonConvert.SerializeObject(input),
                    WebAPIName = "NPR370Save",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + NPR370SaveURL
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }


            return output;
        }
        #endregion
        #region 手機條碼檢核
        /// <summary>
        /// 確認手機條碼
        /// </summary>
        /// <param name="carrierid"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool CheckEinvBiz(string carrierid, ref string errCode)
        {
            bool flag = false;
            WebAPIInput_EinvBiz input = new WebAPIInput_EinvBiz()
            {
                CARRIERID = carrierid,
                sig = GenerateSig(),
                user_id = userid
            };
            WebAPIOutput_EinvBiz output = DoCheckEinvBiz(input).Result;
            if (output.isExist == "N")
            {
                flag = false;
                errCode = "ERR194";
                //手機條碼或自然人憑證載具有誤，請至會員中心修改
                //系統例行維護中
            }
            else if (output.isExist == "Y")
            {
                flag = true;
            }
            return flag;
        }
        /// <summary>
        /// 確認手機條碼
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<WebAPIOutput_EinvBiz> DoCheckEinvBiz(WebAPIInput_EinvBiz input)
        {
            WebAPIOutput_EinvBiz output = null;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + EinvBizURL);
            request.Method = "POST";
            request.ContentType = "application/json";
            try
            {
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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_EinvBiz>(responseStr);
                    }

                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_EinvBiz()
                {
                    isExist = "N",
                    Message = "發生異常錯誤",
                    Result = false
                };
            }
            finally
            {
                SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                {
                    MKTime = MKTime,
                    UPDTime = RTime,
                    WebAPIInput = JsonConvert.SerializeObject(input),
                    WebAPIName = "EinvBiz",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = EinvBizURL
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }


            return output;
        }
        #endregion
        #region 欠費查詢
        /// <summary>
        /// 點數查詢
        /// </summary>
        /// <param name="IDNO"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool NPR330Query(string IDNO, ref WebAPIOutput_ArrearQuery output)
        {
            bool flag = false;
            WebAPIInput_ArrearsQuery input = new WebAPIInput_ArrearsQuery()
            {
                sig = GenerateSig(),
                user_id = userid,
                MEMIDNO = IDNO
            };


            output = DoNPR330Query(input).Result;
            if (output.Result)
            {
                flag = true;
            }
            return flag;
        }
        /// <summary>
        /// 點數查詢
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<WebAPIOutput_ArrearQuery> DoNPR330Query(WebAPIInput_ArrearsQuery input)
        {
            WebAPIOutput_ArrearQuery output = null;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + NPR330QueryURL);
            request.Method = "POST";
            request.ContentType = "application/json";
            try
            {
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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_ArrearQuery>(responseStr);
                    }

                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_ArrearQuery()
                {

                    Message = "發生異常錯誤",
                    Result = false
                };
            }
            finally
            {
                SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                {
                    MKTime = MKTime,
                    UPDTime = RTime,
                    WebAPIInput = JsonConvert.SerializeObject(input),
                    WebAPIName = "NPR330Query",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + NPR330QueryURL
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }


            return output;
        }
        #endregion
        #region 點數兌換
        /// <summary>
        /// NPR320
        /// </summary>
        /// <param name="IDNO"></param>
        /// <param name="COUPONNO"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool NPR320Query(string IDNO, string COUPONNO, ref WebAPIOutput_NPR320Query output)
        {
            bool flag = true;
            WebAPIInput_NPR320Query input = new WebAPIInput_NPR320Query()
            {
                sig = GenerateSig(),
                user_id = userid,
                MEMIDNO = IDNO,
                COUPONNO = COUPONNO
            };
            output = DoNPR320Query(input).Result;
            if (output.Result)
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
        /// 兌換點數
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<WebAPIOutput_NPR320Query> DoNPR320Query(WebAPIInput_NPR320Query input)
        {
            WebAPIOutput_NPR320Query output = null;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + NPR320QueryURL);
            request.Method = "POST";
            request.ContentType = "application/json";
            try
            {
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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_NPR320Query>(responseStr);
                    }

                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_NPR320Query()
                {

                    Message = "發生異常錯誤",
                    Result = false
                };
            }
            finally
            {
                SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                {
                    MKTime = MKTime,
                    UPDTime = RTime,
                    WebAPIInput = JsonConvert.SerializeObject(input),
                    WebAPIName = "NPR320Query",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + NPR320QueryURL
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }


            return output;
        }
        #endregion
        #region ETAG010查詢費用(合約)
        public bool ETAG010Send(string IRENTORDNO, string RNTDATETIME, ref WebAPIOutput_ETAG010 output)
        {
            bool flag = true;
            WebAPIInput_ETAG010 input = new WebAPIInput_ETAG010()
            {
                sig = GenerateSig(),
                user_id = userid,
                 IRENTORDNO = IRENTORDNO,
                RNTDATETIME = RNTDATETIME
            };
            output = DoETAG010Send(input).Result;
            if (output.Result)
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
        /// 發送簡訊執行端
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<WebAPIOutput_ETAG010> DoETAG010Send(WebAPIInput_ETAG010 input)
        {
            WebAPIOutput_ETAG010 output = null;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + ETAG010QueryURL);
            request.Method = "POST";
            request.ContentType = "application/json";
            try
            {
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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_ETAG010>(responseStr);
                    }

                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_ETAG010()
                {

                    Message = "發生異常錯誤",
                    Result = false
                };
            }
            finally
            {
                SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                {
                    MKTime = MKTime,
                    UPDTime = RTime,
                    WebAPIInput = JsonConvert.SerializeObject(input),
                    WebAPIName = "ETAG010Query",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + ETAG010QueryURL
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }


            return output;
        }
        #endregion
        #region 060
        public bool NPR06Save(WebAPIInput_NPR060Save input, ref WebAPIOutput_NPR060Save output)
        {
            bool flag = false;



            output = DoNPR060Save(input).Result;
            if (output.Result)
            {
                flag = true;
            }
            return flag;
        }
        /// <summary>
        /// 點數查詢
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<WebAPIOutput_NPR060Save> DoNPR060Save(WebAPIInput_NPR060Save input)
        {
            WebAPIOutput_NPR060Save output = null;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + NPR060SaveURL);
            request.Method = "POST";
            request.ContentType = "application/json";
            try
            {
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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_NPR060Save>(responseStr);
                    }

                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_NPR060Save()
                {

                    Message = "發生異常錯誤",
                    Result = false
                };
            }
            finally
            {
                SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                {
                    MKTime = MKTime,
                    UPDTime = RTime,
                    WebAPIInput = JsonConvert.SerializeObject(input),
                    WebAPIName = "NPR060Save",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + NPR060SaveURL
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }


            return output;
        }
        #endregion
        #region 125
        #endregion
        #region 130
        #endregion
        #region 136
        #endregion
    }
}
