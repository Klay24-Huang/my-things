using Newtonsoft.Json;
using NLog;
using RelayAPI.Models.Input;
using RelayAPI.Models.Output;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Http;
using WebCommon;

namespace RelayAPI.Controllers
{
    
    public class RelayPost_WalletWithdrawController : ApiController
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        private string relayEnKey = ConfigurationManager.AppSettings["RelayEnKey"].ToString();                      //中繼加密key
        private string relayEnSalt = ConfigurationManager.AppSettings["RelayEnSalt"].ToString();                    //中繼加密Salt
        private string APIToken = ConfigurationManager.AppSettings["TaishinWalletAPIToken"].ToString();
        private string MerchantId = ConfigurationManager.AppSettings["TaishiWalletMerchantId"].ToString();


        [HttpPost]
        public OAPI_RelayPost DoRelayPost_WalletWithdrawController(Dictionary<string, object> value)
        {
            string Contentjson = JsonConvert.SerializeObject(value);
            IAPI_RelayPost_WalletWithdraw apiInput = null;
            OAPI_RelayPost output = new OAPI_RelayPost();
            output.IsSuccess = false;
            output.RtnMessage = "";
            apiInput = JsonConvert.DeserializeObject<IAPI_RelayPost_WalletWithdraw>(Contentjson);
            logger.Trace("DoRelayPostInput: " + Contentjson);

            if (apiInput != null)
            {
                string url = apiInput.BaseUrl;
                string api = apiInput.ApiUrl;
                string baseUrl = ConfigurationManager.AppSettings[url].ToString();
                string apiUrl = ConfigurationManager.AppSettings[api].ToString();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"{baseUrl}{apiUrl}");
                request.Headers.Add("Authorization", string.Format("Bearer {0}", APIToken));
                request.Headers.Add("ClientId", MerchantId);
                request.Headers.Add("UtcTimeStamp", apiInput.UtcTimeStamp);
                request.Headers.Add("SignCode", apiInput.SignCode);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.KeepAlive = false;
                request.Timeout = 30000;
                

                try
                {
                    string postBody = new AESEncrypt().doDecrypt(relayEnKey, relayEnSalt, apiInput.RequestData);
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
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
                            output.IsSuccess = true;
                            output.ResponseData = new AESEncrypt().doEncrypt(relayEnKey, relayEnSalt, responseStr);
                            logger.Trace("DoRelayPostResponse: " + responseStr);
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
                    output.IsSuccess = false;
                    output.RtnMessage = ex.Message;
                    logger.Error("DoRelayPostException: " + ex.Message);
                }

                //增加關閉Request的處理
                request.Abort();
            }
            else
            {
                output.IsSuccess = false;
                output.RtnMessage = "傳入API格式有誤";
            }

            return output;
        }
    }
}