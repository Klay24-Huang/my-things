using Domain.SP.Input.OtherService.Common;
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
        protected string NPR260SendURL; //折抵時數轉贈確認(判斷身份證及時數可轉贈)
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
            NPR260SendURL = (ConfigurationManager.AppSettings.Get("NPR260SendURL") == null) ? "" : ConfigurationManager.AppSettings.Get("NPR260SendURL").ToString();
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
        /// 兌換點數
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
    }
}
