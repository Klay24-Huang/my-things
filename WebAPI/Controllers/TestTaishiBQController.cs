using Domain.WebAPI.output.Taishin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Http;
using NLog;

namespace WebAPI.Controllers
{
    public class TestTaishiBQController : ApiController
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private string BaseURL = ConfigurationManager.AppSettings["TaishinBaseURL"].ToString();                     //台新base網址
        private string GetCreditCardList = ConfigurationManager.AppSettings["GetCreditCardList"].ToString();        //取得綁卡列表
        /// <summary>
        /// 這隻只是在防火牆還沒開通前用的
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public WebAPIOutput_GetCreditCardList doTestTaishiBQ(Dictionary<string, object> value)
        {
            string Site = BaseURL + GetCreditCardList;
            WebAPIOutput_GetCreditCardList output = null;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Site);
            request.Method = "POST";
            request.ContentType = "application/json";
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                string postBody = JsonConvert.SerializeObject(value);//將匿名物件序列化為json字串
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
                        logger.Debug(responseStr);
                        //RTime = DateTime.Now;
                        output = JsonConvert.DeserializeObject<WebAPIOutput_GetCreditCardList>(responseStr);
                    }

                }

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return output;
            //return JsonConvert.SerializeObject(output);
        }
    }
}