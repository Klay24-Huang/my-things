using Domain.SP.Input.OtherService.Common;
using Domain.WebAPI.Input.HiEasyRentAPI;
using Domain.WebAPI.output.HiEasyRentAPI;
using Newtonsoft.Json;
using OtherService.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
    public class HiEasyRentAPI
    {
        protected string userid;
        protected string apikey;
        protected string BaseURL;
        protected string NPR013RegURL;   //新增短租會員 20201126 ADD BY ADAM
        protected string NPR010SaveURL;  //新增短租會員 20201126 ADD BY ADAM
        protected string NPR060SaveURL;  //060
        protected string NPR125SaveURL;  //
        protected string NPR130SaveURL;  //
        protected string NPR136SaveURL;  //
        protected string NPR172QueryURL; //查詢黑名單
        protected string NPR260SendURL;  //簡訊發送
        protected string NPR270QueryURL; //點數查詢
        protected string NPR271QueryURL; //點數歷程查詢
        protected string NPR320QueryURL; //點數兌換
        protected string NPR330QueryURL; //欠費查詢
        protected string NPR340SaveURL;  //欠費沖銷
        protected string NPR350CheckURL; //合約查詢
        protected string NPR370CheckURL; //點數轉贈前檢查
        protected string NPR370SaveURL;  //進行轉贈
        protected string NPR380SaveURL;  //換電獎勵 20201201 ADD BY ADAM 
        protected string NPR390QueryURL; //主動取款查詢 20210714 ADD BY 唐瑋祁
        protected string EinvBizURL;     //手機條碼檢核
        protected string MonthlyRentURL; //月租訂閱
        protected string ETAG010QueryURL;//ETAG查詢(合約編號)
        protected string SetMonthlyRentDataURL; //月租訂閱合約儲存
        protected string SetMonthlyRentDataV2URL;//月租訂閱合約儲存(欠費用)
        protected string ETAG020QueryURL;//ETAG查詢(身份證)
        protected string ETAG031SaveURL; //ETAG沖銷
        protected string TransIRentMemCMKURL; // 會員條款更新至官網 20210824 ADD BY YEH
        protected string NPR136V2SaveURL;   //20220309 ADD BY ADAM REASON.營損匯入使用
        protected string connetStr;
        protected string NPR138SaveURL;  //春節定金收款發票
        protected string NPR420SaveURL;  //錢包介面轉檔

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
            NPR340SaveURL = (ConfigurationManager.AppSettings.Get("NPR340SaveURL") == null) ? "" : ConfigurationManager.AppSettings.Get("NPR340SaveURL").ToString();
            NPR350CheckURL = (ConfigurationManager.AppSettings.Get("NPR350CheckURL") == null) ? "" : ConfigurationManager.AppSettings.Get("NPR350CheckURL").ToString();

            EinvBizURL = (ConfigurationManager.AppSettings.Get("EinvBizURL") == null) ? "" : ConfigurationManager.AppSettings.Get("EinvBizURL").ToString();
            NPR320QueryURL = (ConfigurationManager.AppSettings.Get("NPR320QueryURL") == null) ? "" : ConfigurationManager.AppSettings.Get("NPR320QueryURL").ToString();
            MonthlyRentURL = (ConfigurationManager.AppSettings.Get("MonthlyRentURL") == null) ? "" : ConfigurationManager.AppSettings.Get("MonthlyRentURL").ToString();
            NPR172QueryURL = (ConfigurationManager.AppSettings.Get("NPR172QueryURL") == null) ? "" : ConfigurationManager.AppSettings.Get("NPR172QueryURL").ToString();
            ETAG010QueryURL = (ConfigurationManager.AppSettings.Get("ETAG010QueryURL") == null) ? "" : ConfigurationManager.AppSettings.Get("ETAG010QueryURL").ToString();
            ETAG020QueryURL = (ConfigurationManager.AppSettings.Get("ETAG020QueryURL") == null) ? "" : ConfigurationManager.AppSettings.Get("ETAG020QueryURL").ToString();
            ETAG031SaveURL = (ConfigurationManager.AppSettings.Get("ETAG031SaveURL") == null) ? "" : ConfigurationManager.AppSettings.Get("ETAG031SaveURL").ToString();
            //20201126 ADD BY ADAM REASON.新增短租會員
            NPR013RegURL = (ConfigurationManager.AppSettings.Get("NPR013RegURL") == null) ? "" : ConfigurationManager.AppSettings.Get("NPR013RegURL").ToString();
            //20201201 ADD BY ADAM REASON.換電獎勵
            NPR380SaveURL = (ConfigurationManager.AppSettings.Get("NPR380SaveURL") == null) ? "" : ConfigurationManager.AppSettings.Get("NPR380SaveURL").ToString();
            //20201126 ADD BY Jerry REASON.同步會員資料
            NPR010SaveURL = (ConfigurationManager.AppSettings.Get("NPR010SaveURL") == null) ? "" : ConfigurationManager.AppSettings.Get("NPR010SaveURL").ToString();
            //主動取款查詢 20210714 ADD BY 唐瑋祁
            NPR390QueryURL = (ConfigurationManager.AppSettings.Get("NPR390QueryURL") == null) ? "" : ConfigurationManager.AppSettings.Get("NPR390QueryURL").ToString();

            SetMonthlyRentDataURL = (ConfigurationManager.AppSettings.Get("SetMonthlyRentDataURL") == null) ? "" : ConfigurationManager.AppSettings.Get("SetMonthlyRentDataURL").ToString();
            SetMonthlyRentDataV2URL = (ConfigurationManager.AppSettings.Get("SetMonthlyRentDataV2URL") == null) ? "" : ConfigurationManager.AppSettings.Get("SetMonthlyRentDataV2URL").ToString();
            // 會員條款更新至官網 20210824 ADD BY YEH
            TransIRentMemCMKURL = (ConfigurationManager.AppSettings.Get("TransIRentMemCMKURL") == null) ? "" : ConfigurationManager.AppSettings.Get("TransIRentMemCMKURL").ToString();

            NPR138SaveURL = (ConfigurationManager.AppSettings.Get("NPR138SaveURL") == null) ? "" : ConfigurationManager.AppSettings.Get("NPR138SaveURL").ToString();
            //20220309 ADD BY ADAM REASON.營損匯入使用
            NPR136V2SaveURL = (ConfigurationManager.AppSettings.Get("NPR136V2SaveURL") == null) ? "" : ConfigurationManager.AppSettings.Get("NPR136V2SaveURL").ToString();
            NPR420SaveURL = (ConfigurationManager.AppSettings.Get("NPR420SaveURL") == null) ? "" : ConfigurationManager.AppSettings.Get("NPR420SaveURL").ToString();
        }

        #region 產生簽章
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
        #endregion

        #region 發送簡訊
        public bool NPR260Send(string TARGET, string Message, string RENO, ref WebAPIOutput_NPR260Send output)
        {
            bool flag = true;
            WebAPIInput_NPR260Send input = new WebAPIInput_NPR260Send()
            {
                sig = GenerateSig(),
                user_id = userid,
                TARGET = TARGET,
                MESSAGE = Message,
                RENO = RENO
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
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + NPR260SendURL);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Timeout = 15000;    //設定TIMEOUT時間

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
                    WebAPIURL = BaseURL + NPR260SendURL
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
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + NPR271QueryURL);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Timeout = 15000;    //設定TIMEOUT時間
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
                TransMins = Pointer,
                TargetId = ""
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
        public bool NPR370Save(string IDNO, string TargetId, int Pointer, string GiftType, ref WebAPIOutput_NPR370Save output)
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
        /// 欠費查詢
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
        /// 欠費查詢
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
            request.Timeout = 15000;    //設定TIMEOUT時間

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
        #region 136修改合約
        public bool NPR136Save(WebAPIInput_NPR136Save input, ref WebAPIOutput_NPR136Save output)
        {
            bool flag = false;

            input.user_id = userid;
            input.sig = GenerateSig();

            output = DoNPR136Save(input).Result;
            if (output.Result)
            {
                flag = true;
            }
            return flag;
        }
        /// <summary>
        /// 136修改合約
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<WebAPIOutput_NPR136Save> DoNPR136Save(WebAPIInput_NPR136Save input)
        {
            WebAPIOutput_NPR136Save output = null;
            Int16 IsSuccess = 0;
            string ORDNO = "";
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + NPR136SaveURL);
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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_NPR136Save>(responseStr);
                        if (output.Result)
                        {
                            IsSuccess = 1;
                            //  ORDNO = output.Data[0].ORDNO;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_NPR136Save()
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
                    WebAPIName = "NPR136Save",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + NPR136SaveURL
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);

            }


            return output;
        }
        #endregion
        #region 340沖銷
        public bool NPR340Save(WebAPIInput_NPR340Save input, ref WebAPIOutput_NPR340Save output)
        {
            bool flag = false;

            input.user_id = userid;
            input.sig = GenerateSig();

            output = DoNPR340Save(input).Result;
            if (output.Result)
            {
                flag = true;
            }
            return flag;
        }
        /// <summary>
        /// 340
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<WebAPIOutput_NPR340Save> DoNPR340Save(WebAPIInput_NPR340Save input)
        {
            WebAPIOutput_NPR340Save output = null;
            Int16 IsSuccess = 0;
            string ORDNO = "";
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + NPR340SaveURL);
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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_NPR340Save>(responseStr);
                        if (output.Result)
                        {
                            IsSuccess = 1;
                            //  ORDNO = output.Data[0].ORDNO;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_NPR340Save()
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
                    WebAPIName = "NPR340Save",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + NPR340SaveURL
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
                if (output.Data == null)
                {
                    flag = false;
                }
                else
                {
                    if (output.RtnCode != "0")
                    {
                        flag = false;
                    }
                }
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
        #region 172查詢黑名單
        public bool NPR172Query(string IDNO, ref WebAPIOutput_NPR172Query output)
        {
            bool flag = true;
            WebAPIInput_NPR172Query input = new WebAPIInput_NPR172Query()
            {
                sig = GenerateSig(),
                user_id = userid
            };
            output = DoNPR172Query(input).Result;
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
        public async Task<WebAPIOutput_NPR172Query> DoNPR172Query(WebAPIInput_NPR172Query input)
        {
            WebAPIOutput_NPR172Query output = null;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + NPR172QueryURL);
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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_NPR172Query>(responseStr);
                    }
                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_NPR172Query()
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
                    WebAPIName = "NPR172Query",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + NPR172QueryURL
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }

            return output;
        }
        #endregion
        #region 出還車相關
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
        public bool NPR060Save(WebAPIInput_NPR060Save input, ref WebAPIOutput_NPR060Save output)
        {
            bool flag = false;

            input.user_id = userid;
            input.sig = GenerateSig();

            output = DoNPR060Save(input).Result;
            if (output.Result)
            {
                flag = true;
            }
            return flag;
        }
        /// <summary>
        /// 預約
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<WebAPIOutput_NPR060Save> DoNPR060Save(WebAPIInput_NPR060Save input)
        {
            WebAPIOutput_NPR060Save output = null;
            Int16 IsSuccess = 0;
            string ORDNO = "";
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
                        if (output.Result)
                        {
                            IsSuccess = 1;
                            ORDNO = output.Data[0].ORDNO;
                        }
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
        /// <summary>
        /// 出車
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool NPR125Save(WebAPIInput_NPR125Save input, ref WebAPIOutput_NPR125Save output)
        {
            bool flag = false;

            input.user_id = userid;
            input.sig = GenerateSig();

            output = DoNPR125Save(input).Result;
            if (output.Result)
            {
                flag = true;
            }
            return flag;
        }
        /// <summary>
        /// 出車
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<WebAPIOutput_NPR125Save> DoNPR125Save(WebAPIInput_NPR125Save input)
        {
            WebAPIOutput_NPR125Save output = null;
            Int16 IsSuccess = 0;
            string ORDNO = "";
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + NPR125SaveURL);
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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_NPR125Save>(responseStr);
                        if (output.Result)
                        {
                            IsSuccess = 1;

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_NPR125Save()
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
                    WebAPIName = "NPR125Save",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + NPR125SaveURL
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);

            }


            return output;
        }
        #endregion
        #region 130
        public bool NPR130Save(WebAPIInput_NPR130Save input, ref WebAPIOutput_NPR130Save output)
        {
            bool flag = false;

            input.user_id = userid;
            input.sig = GenerateSig();

            output = DoNPR130Save(input).Result;
            if (output.Result)
            {
                flag = true;
            }
            return flag;
        }
        /// <summary>
        /// 出車
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<WebAPIOutput_NPR130Save> DoNPR130Save(WebAPIInput_NPR130Save input)
        {
            WebAPIOutput_NPR130Save output = null;
            Int16 IsSuccess = 0;
            string ORDNO = "";
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + NPR130SaveURL);
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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_NPR130Save>(responseStr);
                        if (output.Result)
                        {
                            IsSuccess = 1;

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_NPR130Save()
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
                    WebAPIName = "NPR130Save",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + NPR130SaveURL
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);

            }
            return output;
        }
        #endregion
        #region NPR350查詢合約狀態
        public bool NPR350Check(WebAPIInput_NPR350Check input, ref WebAPIOutput_NPR350Check output)
        {
            bool flag = false;

            input.user_id = userid;
            input.sig = GenerateSig();

            output = DoNPR350Check(input).Result;
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
        private async Task<WebAPIOutput_NPR350Check> DoNPR350Check(WebAPIInput_NPR350Check input)
        {
            WebAPIOutput_NPR350Check output = null;
            Int16 IsSuccess = 0;
            string ORDNO = "";
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + NPR350CheckURL);
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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_NPR350Check>(responseStr);
                        if (output.Result)
                        {
                            IsSuccess = 1;

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_NPR350Check()
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
                    WebAPIName = "NPR350Check",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + NPR350CheckURL
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);

            }


            return output;
        }
        #endregion
        #region 136
        #endregion
        #endregion
        #region 會員相關
        public bool NPR013Reg(string IDNO, string MEMCNAME, ref WebAPIOutput_NPR013Reg output)
        {
            bool flag = false;
            WebAPIInput_NPR013Reg input = new WebAPIInput_NPR013Reg()
            {
                sig = GenerateSig(),
                user_id = userid,
                MEMIDNO = IDNO,
                MEMCNAME = MEMCNAME,
                MEMPWD = "",
                MEMCEIL = ""
            };

            output = DoNPR013Reg(input).Result;
            if (output.Result)
            {
                flag = true;
            }
            return flag;
        }

        public bool NPR010Save(WebAPIInput_NPR010Save input, ref WebAPIOutput_NPR013Reg output)
        {
            bool flag = false;

            input.sig = GenerateSig();

            output = DoNPR010Save(input).Result;
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
        private async Task<WebAPIOutput_NPR013Reg> DoNPR013Reg(WebAPIInput_NPR013Reg input)
        {
            WebAPIOutput_NPR013Reg output = null;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + NPR013RegURL);
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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_NPR013Reg>(responseStr);
                    }
                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_NPR013Reg()
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
                    WebAPIName = "NPR013Reg",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + NPR013RegURL
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }

            return output;
        }

        /// <summary>
        /// 點數查詢
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<WebAPIOutput_NPR013Reg> DoNPR010Save(WebAPIInput_NPR010Save input)
        {
            WebAPIOutput_NPR013Reg output = null;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + NPR010SaveURL);
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:4149/api/NPR010/Save/"); //20210218唐測試
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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_NPR013Reg>(responseStr);
                    }
                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_NPR013Reg()
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
                    WebAPIName = "NPR010Save",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + NPR010SaveURL
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }

            return output;
        }
        #endregion
        #region 換電獎勵兌換 20201201 ADD BY ADAM
        public bool NPR380Save(string IDNO, string POINT, string IRENTORDNO, ref WebAPIOutput_NPR380Save output)
        {
            bool flag = false;

            WebAPIInput_NPR380Save input = new WebAPIInput_NPR380Save()
            {
                sig = GenerateSig(),
                user_id = userid,
                MEMIDNO = IDNO,
                POINT = POINT,
                CNTRNO = IRENTORDNO
            };

            output = DoNPR380Save(input).Result;
            if (output.Result)
            {
                flag = true;
            }
            return flag;
        }

        public async Task<WebAPIOutput_NPR380Save> DoNPR380Save(WebAPIInput_NPR380Save input)
        {
            WebAPIOutput_NPR380Save output = null;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + NPR380SaveURL);
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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_NPR380Save>(responseStr);
                    }
                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_NPR380Save()
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
                    WebAPIName = "NPR380Save",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + NPR380SaveURL
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }

            return output;
        }
        #endregion

        #region 主動取款查詢 20210714 ADD BY 唐瑋祁
        public bool NPR390Query(WebAPIInput_IrentPaymentDetail input, ref WebAPIOutput_IrentPaymentDetail output)
        {
            bool flag = false;

            input.sig = GenerateSig();
            input.user_id = userid;

            output = DoNPR390Query(input).Result;
            if (output.Result)
            {
                flag = true;
            }
            return flag;
        }

        public async Task<WebAPIOutput_IrentPaymentDetail> DoNPR390Query(WebAPIInput_IrentPaymentDetail input)
        {
            WebAPIOutput_IrentPaymentDetail output = null;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + NPR390QueryURL);
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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_IrentPaymentDetail>(responseStr);
                    }
                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_IrentPaymentDetail()
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
                    WebAPIName = "NPR390QueryURL",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + NPR390QueryURL
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }

            return output;
        }

        public bool NPR390Query2(WebAPIInput_IrentPaymentDetail input, ref WebAPIOutput_IrentPaymentDetailExplode output)
        {
            bool flag = false;

            input.sig = GenerateSig();
            input.user_id = userid;

            output = DoNPR390Query2(input).Result;
            if (output.Result)
            {
                flag = true;
            }
            return flag;
        }

        public async Task<WebAPIOutput_IrentPaymentDetailExplode> DoNPR390Query2(WebAPIInput_IrentPaymentDetail input)
        {
            WebAPIOutput_IrentPaymentDetailExplode output = null;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + NPR390QueryURL);
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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_IrentPaymentDetailExplode>(responseStr);
                    }
                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_IrentPaymentDetailExplode()
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
                    WebAPIName = "NPR390QueryURL",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + NPR390QueryURL
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }

            return output;
        }

        public bool NPR390Query3(WebAPIInput_IrentPaymentDetail input, ref WebAPIOutput_IrentPaymentHistory output)
        {
            bool flag = false;

            input.sig = GenerateSig();
            input.user_id = userid;

            output = DoNPR390Query3(input).Result;
            if (output.Result)
            {
                flag = true;
            }
            return flag;
        }

        public async Task<WebAPIOutput_IrentPaymentHistory> DoNPR390Query3(WebAPIInput_IrentPaymentDetail input)
        {
            WebAPIOutput_IrentPaymentHistory output = null;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + NPR390QueryURL);
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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_IrentPaymentHistory>(responseStr);
                    }
                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_IrentPaymentHistory()
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
                    WebAPIName = "NPR390QueryURL",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + NPR390QueryURL
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }

            return output;
        }


        public bool NPR390Query4(WebAPIInput_IrentPaymentDetail input, ref WebAPIOutput_IrentPaymentHistoryExplode output)
        {
            bool flag = false;

            input.sig = GenerateSig();
            input.user_id = userid;

            output = DoNPR390Query4(input).Result;
            if (output.Result)
            {
                flag = true;
            }
            return flag;
        }

        public async Task<WebAPIOutput_IrentPaymentHistoryExplode> DoNPR390Query4(WebAPIInput_IrentPaymentDetail input)
        {
            WebAPIOutput_IrentPaymentHistoryExplode output = null;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + NPR390QueryURL);
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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_IrentPaymentHistoryExplode>(responseStr);
                    }
                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_IrentPaymentHistoryExplode()
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
                    WebAPIName = "NPR390QueryURL",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + NPR390QueryURL
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }

            return output;
        }

        public bool NPR390Query5(WebAPIInput_IrentPaymentDetail input, ref WebAPIOutput_IrentPaymentHistorySendMail output)
        {
            bool flag = false;

            input.sig = GenerateSig();
            input.user_id = userid;

            output = DoNPR390Query5(input).Result;//
            if (output.Result)
            {
                flag = true;
            }
            return flag;
        }

        public async Task<WebAPIOutput_IrentPaymentHistorySendMail> DoNPR390Query5(WebAPIInput_IrentPaymentDetail input)
        {
            WebAPIOutput_IrentPaymentHistorySendMail output = null;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + NPR390QueryURL);
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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_IrentPaymentHistorySendMail>(responseStr);
                    }
                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_IrentPaymentHistorySendMail()
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
                    WebAPIName = "NPR390QueryURL",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + NPR390QueryURL
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }

            return output;
        }
        #endregion

        #region 月租訂閱 20210623 Add By Frank
        public bool MonthlyRentSave(WebAPIInput_MonthlyRentSave input, ref WebAPIOutput_MonthlyRentSave output)
        {
            bool flag = false;

            input.user_id = userid;
            input.sig = GenerateSig();

            output = DoMonthlyRentSave(input).Result;

            if (output.Result)
            {
                flag = true;
            }
            return flag;
        }
        public bool MonthlyRentSaveV2(WebAPIInput_MonthlyRebtSaveV2 input, ref WebAPIOutput_MonthlyRentSave output)
        {
            bool flag = false;

            input.user_id = userid;
            input.sig = GenerateSig();

            output = DoMonthlyRentSaveV2(input).Result;
            if (output.Result)
            {
                flag = true;
            }
            return flag;
        }

        private async Task<WebAPIOutput_MonthlyRentSave> DoMonthlyRentSaveV2(WebAPIInput_MonthlyRebtSaveV2 input)
        {
            WebAPIOutput_MonthlyRentSave output = null;
            Int16 IsSuccess = 0;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:4149/api/" + SetMonthlyRentDataURL);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + SetMonthlyRentDataV2URL);
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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_MonthlyRentSave>(responseStr);
                        if (output.Result)
                        {
                            IsSuccess = 1;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_MonthlyRentSave()
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
                    WebAPIName = "MonthlyRentSaveV2",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + MonthlyRentURL
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);

            }

            return output;
        }

        private async Task<WebAPIOutput_MonthlyRentSave> DoMonthlyRentSave(WebAPIInput_MonthlyRentSave input)
        {
            WebAPIOutput_MonthlyRentSave output = null;
            Int16 IsSuccess = 0;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:4149/api/" + SetMonthlyRentDataURL);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + SetMonthlyRentDataURL);
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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_MonthlyRentSave>(responseStr);
                        if (output.Result)
                        {
                            IsSuccess = 1;
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_MonthlyRentSave()
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
                    WebAPIName = "MonthlyRentSave",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + MonthlyRentURL

                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);

            }
            return output;
        }
        #endregion

        #region 會員條款更新至官網 20210824 ADD BY YEH
        /// <summary>
        /// 會員條款更新至官網
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool TransIRentMemCMK(WebAPIInput_TransIRentMemCMK input, ref WebAPIOutput_TransIRentMemCMK output)
        {
            bool flag = false;

            input.user_id = userid;
            input.sig = GenerateSig();

            output = DoTransIRentMemCMK(input).Result;
            if (output.Result)
            {
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// 會員條款更新至官網
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<WebAPIOutput_TransIRentMemCMK> DoTransIRentMemCMK(WebAPIInput_TransIRentMemCMK input)
        {
            WebAPIOutput_TransIRentMemCMK output = null;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + TransIRentMemCMKURL);
            request.Method = "POST";
            request.ContentType = "application/json";

            try
            {
                string postBody = JsonConvert.SerializeObject(input);   // 將匿名物件序列化為json字串
                byte[] byteArray = Encoding.UTF8.GetBytes(postBody);    // 要發送的字串轉為byte[]

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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_TransIRentMemCMK>(responseStr);
                    }
                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_TransIRentMemCMK()
                {
                    Message = "發生異常錯誤:" + ex.Message,
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
                    WebAPIName = "TransIRentMemCMK",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + TransIRentMemCMKURL
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }
            return output;
        }
        #endregion

        #region 春節定金 20211118 ADD BY ADAM
        public bool NPR138Save(WebAPIInput_NPR138Save input, ref WebAPIOutput_NPR138Save output)
        {
            bool flag = false;

            input.sig = GenerateSig();
            input.user_id = userid;

            output = DoNPR138Save(input).Result;
            if (output.Result)
            {
                flag = true;
            }
            return flag;
        }

        public async Task<WebAPIOutput_NPR138Save> DoNPR138Save(WebAPIInput_NPR138Save input)
        {
            WebAPIOutput_NPR138Save output = null;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + NPR138SaveURL);
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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_NPR138Save>(responseStr);
                    }
                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_NPR138Save()
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
                    WebAPIName = "NPR138Save",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + NPR138SaveURL
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }

            return output;
        }
        #endregion
        #region 錢包介面轉檔
        public bool NPR420Save(WebAPIInput_NPR420Save input, ref WebAPIOutput_NPR420Save output)
        {
            bool flag = false;

            input.sig = GenerateSig();
            input.user_id = userid;

            output = DoNPR420Save(input).Result;
            if (output.Result)
            {
                flag = true;
            }
            return flag;
        }

        public async Task<WebAPIOutput_NPR420Save> DoNPR420Save(WebAPIInput_NPR420Save input)
        {
            WebAPIOutput_NPR420Save output = null;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            string URL = BaseURL + NPR420SaveURL;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                request.Method = "POST";
                request.ContentType = "application/json";

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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_NPR420Save>(responseStr);
                    }
                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_NPR420Save()
                {
                    Result = false,
                    RtnCode = "-2",
                    Message = $"發生異常錯誤 : {ex.Message}",

                };
            }
            finally
            {
                SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                {
                    MKTime = MKTime,
                    UPDTime = RTime,
                    WebAPIInput = JsonConvert.SerializeObject(input),
                    WebAPIName = "NPR420Save",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = URL
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }

            return output;
        }
        #endregion


        #region 營損匯入 20220309 ADD BY ADAM
        public bool NPR136V2Save(WebAPIInput_NPR136V2Save input, ref WebAPIOutput_NPR136Save output)
        {
            bool flag = false;

            input.user_id = userid;
            input.sig = GenerateSig();

            output = DoNPR136V2Save(input).Result;
            if (output.Result)
            {
                flag = true;
            }
            return flag;
        }
        /// <summary>
        /// 136修改合約
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<WebAPIOutput_NPR136Save> DoNPR136V2Save(WebAPIInput_NPR136V2Save input)
        {
            WebAPIOutput_NPR136Save output = null;
            Int16 IsSuccess = 0;
            string ORDNO = "";
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + NPR136V2SaveURL);
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
                        output = JsonConvert.DeserializeObject<WebAPIOutput_NPR136Save>(responseStr);
                        if (output.Result)
                        {
                            IsSuccess = 1;
                            //  ORDNO = output.Data[0].ORDNO;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_NPR136Save()
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
                    WebAPIName = "NPR136V2Save",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = BaseURL + NPR136SaveURL
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);

            }


            return output;
        }
        #endregion

    }
}