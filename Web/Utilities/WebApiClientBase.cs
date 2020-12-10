using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Web.Utilities
{
    /// <summary>
    /// WebApiClientBase 的摘要描述
    /// </summary>
    public class WebApiClientBase
    {
        static private readonly string BaseAddress = ConfigurationManager.AppSettings["SSAPIAddress"].Trim();
        private static string Token { set; get; }
        
        protected static JObject SpRetBase(Object result, string apiUrl)
        {
            Token = "VfaU+LJXyYZp7Nr3mFhCQtBfZ/rL2AQmOjkOW4W1uZVumEKn0wIHcD/RsdkmgB8di2Y9HFgUS/7HFxHm4m9eACLvfBCTdBEGoGqcd6RDUeZNSwlOrVeFarS9bEalGyz6";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(BaseAddress);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //20130814 ADD BY JERRY 增加Token機制
            string token = "";
            if (HttpContext.Current != null && HttpContext.Current.Request.Cookies["Token"] != null)
            {
                token = HttpContext.Current.Request.Cookies["Token"].Value.ToString();
            }
            else
            {
                if (Token != "")
                {
                    token = Token;
                }
                else
                {
                    if (HttpContext.Current != null && HttpContext.Current.Request["Token"] != null)
                    {
                        token = HttpContext.Current.Request["Token"].ToString();
                    }
                }
            }
            token = "VfaU+LJXyYZp7Nr3mFhCQtBfZ/rL2AQmOjkOW4W1uZVumEKn0wIHcD/RsdkmgB8di2Y9HFgUS/7HFxHm4m9eACLvfBCTdBEGoGqcd6RDUeZNSwlOrVeFarS9bEalGyz6";
            token = HttpUtility.UrlEncode(token, System.Text.Encoding.UTF8);
            // Send the request.
            //HttpResponseMessage resp = Client.PostAsync(apiUrl, content).Result;
            //20130814 ADD BY JERRY 增加Token機制
            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8,"application/json");
            HttpResponseMessage resp = client.PostAsync(apiUrl + "?Token=" + token, httpContent).Result;

            Stream respons = resp.Content.ReadAsStreamAsync().Result;
            StreamReader sr = new StreamReader(respons);
            string response = sr.ReadToEnd();

            JObject reader = JsonConvert.DeserializeObject<JObject>(response);

            return reader;
        }
        protected static void SetToken(JObject reader)
        {
            //20130814 ADD BY JERRY 增加Token機制
            string token = reader["Token"].ToString();
            HttpCookie cookie = new HttpCookie("Token");
            cookie.Value = token;
            cookie.Expires = DateTime.Now.AddDays(1);
            HttpContext.Current.Response.Cookies.Add(cookie);
            WebApiClientBase.Token = token;
        }

    }
}