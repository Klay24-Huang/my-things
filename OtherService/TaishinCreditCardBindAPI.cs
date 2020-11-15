using Domain.SP.Input.OtherService.Common;
using Domain.WebAPI.Input.Taishin;
using Domain.WebAPI.Input.Taishin.GenerateCheckSum;
using Domain.WebAPI.output.Taishin;
using Newtonsoft.Json;
using OtherService.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebCommon;

namespace OtherService
{
    /// <summary>
    /// 台新綁卡WebAPI
    /// </summary>
    public class TaishinCreditCardBindAPI
    {
        private string apikey = ConfigurationManager.AppSettings["TaishinAPIKey"].ToString();
        private string BaseURL = ConfigurationManager.AppSettings["TaishinBaseURL"].ToString();                     //台新base網址
        private string ECBaseURL= ConfigurationManager.AppSettings["TaishinECBaseURL"].ToString();
        private string GetCardPage = ConfigurationManager.AppSettings["GetCardPage"].ToString();                    //取得綁卡網址
        private string GetCreditCardStatus = ConfigurationManager.AppSettings["GetCreditCardStatus"].ToString();    //取得綁卡狀態
        private string DeleteCreditCardAuth = ConfigurationManager.AppSettings["DeleteCreditCardAuth"].ToString();  //刪除綁卡
        private string GetCreditCardList = ConfigurationManager.AppSettings["GetCreditCardList"].ToString();        //取得綁卡列表
        private string Auth = ConfigurationManager.AppSettings["Auth"].ToString();              //直接授權   
        private string AzureAPIBaseURL = ConfigurationManager.AppSettings["AzureAPIBaseUrl"].ToString();               //Azure Api URL
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
                    }

                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_Base()
                {
                    RtnCode="0",
                    RtnMessage=ex.Message.Substring(0,200)
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
        public bool DoGetCreditCardList(PartOfGetCreditCardList wsInput, ref string errCode, ref WebAPIOutput_GetCreditCardList output)
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
                    }

                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_GetCreditCardList()
                {
                    RtnCode = "0",
                    RtnMessage = ex.Message.Substring(0, 200)
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
                    }

                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_DeleteCreditCardAuth()
                {
                    RtnCode = "0",
                    RtnMessage = ex.Message.Substring(0, 200)
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
        #region 授權
        public bool DoCreditCardAuth(PartOfCreditCardAuth wsInput, ref string errCode, ref WebAPIOutput_Auth output)
        {
            bool flag = true;
            string ori = string.Format("request={0}&apikey={1}", Newtonsoft.Json.JsonConvert.SerializeObject(wsInput), apikey);
            string checksum = GenerateSign(ori);

            WebAPIInput_Auth Input = new WebAPIInput_Auth()
            {
                ApiVer = wsInput.ApiVer,
                ApposId = wsInput.ApposId,
                Random = wsInput.Random,
                RequestParams = wsInput.RequestParams,
                CheckSum = checksum,
                TimeStamp = wsInput.TimeStamp
                
            };


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
                    }

                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WebAPIOutput_Auth()
                {
                    RtnCode = "0",
                    RtnMessage = ex.Message.Substring(0, 200)
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
}
