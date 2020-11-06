using Domain.WebAPI.output.Taishin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Http;

namespace WebAPI.Controllers
{
    public class TestTaishiBUController : ApiController
    {
        private string BaseURL = ConfigurationManager.AppSettings["TaishinBaseURL"].ToString();                     //台新base網址
        private string GetCardPage = ConfigurationManager.AppSettings["GetCardPage"].ToString();                    //取得綁卡網址

        [HttpPost]
        public WebAPIOutput_Base doTestTaishiBU(Dictionary<string, object> value)
        {
            string Site = BaseURL + GetCardPage;
            WebAPIOutput_Base output = null;

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
                        //RTime = DateTime.Now;
                        output = JsonConvert.DeserializeObject<WebAPIOutput_Base>(responseStr);
                    }

                }

            }
            catch (Exception ex)
            {

            }
            return output;
        }
    }
}