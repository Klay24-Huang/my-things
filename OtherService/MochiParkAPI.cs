using Domain.WebAPI.output.Mochi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace OtherService
{
    public class MochiParkAPI
    {
        private static bool isTest = (string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("Mochi_is_Test")) ==false)? ((ConfigurationManager.AppSettings.Get("Mochi_is_Test").ToString() == "1") ? true : false) : false;
        string BasePath_T = ConfigurationManager.AppSettings.Get("T_MochiParkBasePath");
        string BasePath_P = ConfigurationManager.AppSettings.Get("MochiParkBasePath");
        string T_user = ConfigurationManager.AppSettings.Get("Mochi_TUser");
        string T_PWD = ConfigurationManager.AppSettings.Get("Mochi_TPWD");
        string P_user = ConfigurationManager.AppSettings.Get("Mochi_User");
        string P_PWD = ConfigurationManager.AppSettings.Get("Mochi_PWD");
        string BasePath = "";
        string user = "";
        string PWD = "";
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        public MochiParkAPI()
        {
            if (isTest)
            {
                BasePath = BasePath_T;
                user = T_user;
                PWD = T_PWD;
            }
            else
            {
                BasePath = BasePath_P;
                user = P_user;
                PWD = P_PWD;
            }
        }
        public bool DoLogin(ref WebAPIOutput_MochiLogin wsout)
        {
            bool flag = true;
            string url = "v1/login";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BasePath + url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
            // request.Headers.Add("Authorization", "Bearer "+token);
            //必須透過ParseQueryString()來建立NameValueCollection物件，之後.ToString()才能轉換成queryString
            NameValueCollection postParams = HttpUtility.ParseQueryString(string.Empty);
            postParams.Add("username", user);
            postParams.Add("password", PWD);

            //Console.WriteLine(postParams.ToString());// 將取得"version=1.0&action=preserveCodeCheck&pCode=pCode&TxID=guid&appId=appId", key和value會自動UrlEncode
            //要發送的字串轉為byte[] 
            byte[] byteArray = Encoding.UTF8.GetBytes(postParams.ToString());
            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(byteArray, 0, byteArray.Length);
            }//end using

            //API回傳的字串
            string responseStr = "";
            //發出Request
            HttpWebResponse myHttpWebResponse = (HttpWebResponse)request.GetResponse();
            //if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
            //{
            using (WebResponse response = request.GetResponse())
            {

                using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    responseStr = sr.ReadToEnd();
                    wsout = Newtonsoft.Json.JsonConvert.DeserializeObject<WebAPIOutput_MochiLogin>(responseStr);
                }//end using  
            }
            //}
            //else
            //{
            //    flag = false;
            //}


            return flag;
        }
        public bool DoSyncPark(string token, ref WebAPIOutput_ParkData wsOut)
        {
            bool flag = true;
            string url = "v1/pois";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BasePath + url);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
            request.Headers.Add("Authorization", "Bearer " + token);
            //必須透過ParseQueryString()來建立NameValueCollection物件，之後.ToString()才能轉換成queryString
            NameValueCollection postParams = HttpUtility.ParseQueryString(string.Empty);
            //postParams.Add("username", user);
            //postParams.Add("password", PWD);

            using (var httpResponse = (HttpWebResponse)request.GetResponse())
            {
                //API回傳的字串
                string responseStr = "";
                //發出Request
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)request.GetResponse();
                if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (WebResponse response = request.GetResponse())
                    {

                        using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                        {
                            responseStr = sr.ReadToEnd();
                            wsOut = Newtonsoft.Json.JsonConvert.DeserializeObject<WebAPIOutput_ParkData>(responseStr);
                        }//end using  
                    }
                }
                else
                {
                    flag = false;
                }
            }




            return flag;
        }
        public bool DoQueryBillByCar(string token, string CarNo, ref WebAPIOutput_QueryBillByCar wsOut)
        {
            bool flag = true;
            string url = string.Format("v1/orders?plate_number={0}&transaction_type=parking", CarNo);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BasePath + url);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
            request.Headers.Add("Authorization", "Bearer " + token);
            //必須透過ParseQueryString()來建立NameValueCollection物件，之後.ToString()才能轉換成queryString
            NameValueCollection postParams = HttpUtility.ParseQueryString(string.Empty);
            //postParams.Add("username", user);
            //postParams.Add("password", PWD);

            using (var httpResponse = (HttpWebResponse)request.GetResponse())
            {
                //API回傳的字串
                string responseStr = "";
                //發出Request
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)request.GetResponse();
                if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (WebResponse response = request.GetResponse())
                    {

                        using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                        {
                            responseStr = sr.ReadToEnd();
                            wsOut = Newtonsoft.Json.JsonConvert.DeserializeObject<WebAPIOutput_QueryBillByCar>(responseStr);
                        }//end using  
                    }
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
        /// <param name="token"></param>
        /// <param name="CarNo"></param>
        /// <param name="stat_at"></param>
        /// <param name="end_at"></param>
        /// <param name="wsOut"></param>
        /// <returns></returns>
        public bool DoQueryBillByCar(string token, string CarNo, string stat_at, string end_at, ref WebAPIOutput_QueryBillByCar wsOut)
        {
            bool flag = true;
            string url = string.Format("v1/orders?plate_number={0}&transaction_type=parking&start_at={1}&end_at={2}", CarNo, stat_at, end_at);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BasePath + url);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
            request.Headers.Add("Authorization", "Bearer " + token);
            //必須透過ParseQueryString()來建立NameValueCollection物件，之後.ToString()才能轉換成queryString
            NameValueCollection postParams = HttpUtility.ParseQueryString(string.Empty);
            //postParams.Add("username", user);
            //postParams.Add("password", PWD);

            using (var httpResponse = (HttpWebResponse)request.GetResponse())
            {
                //API回傳的字串
                string responseStr = "";
                //發出Request
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)request.GetResponse();
                if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (WebResponse response = request.GetResponse())
                    {

                        using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                        {
                            responseStr = sr.ReadToEnd();
                            wsOut = Newtonsoft.Json.JsonConvert.DeserializeObject<WebAPIOutput_QueryBillByCar>(responseStr);
                        }//end using  
                    }
                }
                else
                {
                    flag = false;
                }
            }




            return flag;
        }
        /// <summary>
        /// 取得全部
        /// </summary>
        /// <param name="token"></param>
        /// <param name="CarNo"></param>
        /// <param name="current_page"></param>
        /// <param name="wsOut"></param>
        /// <returns></returns>
        public bool DoQueryBillByCar(string token, string CarNo, int current_page, ref WebAPIOutput_QueryBillByCar wsOut)
        {
            bool flag = true;
            string url = string.Format("v1/orders?plate_number={0}&page={1}&transaction_type=parking", CarNo, current_page);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BasePath + url);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
            request.Headers.Add("Authorization", "Bearer " + token);
            //必須透過ParseQueryString()來建立NameValueCollection物件，之後.ToString()才能轉換成queryString
            NameValueCollection postParams = HttpUtility.ParseQueryString(string.Empty);
            //postParams.Add("username", user);
            //postParams.Add("password", PWD);

            using (var httpResponse = (HttpWebResponse)request.GetResponse())
            {
                //API回傳的字串
                string responseStr = "";
                //發出Request
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)request.GetResponse();
                if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (WebResponse response = request.GetResponse())
                    {

                        using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                        {
                            responseStr = sr.ReadToEnd();
                            wsOut = Newtonsoft.Json.JsonConvert.DeserializeObject<WebAPIOutput_QueryBillByCar>(responseStr);
                        }//end using  
                    }
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
        /// <param name="token">金鑰</param>
        /// <param name="CarNo">車號</param>
        /// <param name="current_page">頁數</param>
        /// <param name="stat_at">訂單起始時間</param>
        /// <param name="end_at">訂單結束時間</param>
        /// <param name="wsOut">車麻吉輸出</param>
        /// <returns></returns>
        public bool DoQueryBillByCar(string token, string CarNo, int current_page, string stat_at, string end_at, ref WebAPIOutput_QueryBillByCar wsOut)
        {
            bool flag = true;
            string url = string.Format("v1/orders?plate_number={0}&page={1}&transaction_type=parking&start_at={2}&end_at={3}", CarNo, current_page, stat_at, end_at);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BasePath + url);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
            request.Headers.Add("Authorization", "Bearer " + token);
            //必須透過ParseQueryString()來建立NameValueCollection物件，之後.ToString()才能轉換成queryString
            NameValueCollection postParams = HttpUtility.ParseQueryString(string.Empty);
            //postParams.Add("username", user);
            //postParams.Add("password", PWD);

            using (var httpResponse = (HttpWebResponse)request.GetResponse())
            {
                //API回傳的字串
                string responseStr = "";
                //發出Request
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)request.GetResponse();
                if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (WebResponse response = request.GetResponse())
                    {

                        using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                        {
                            responseStr = sr.ReadToEnd();
                            wsOut = Newtonsoft.Json.JsonConvert.DeserializeObject<WebAPIOutput_QueryBillByCar>(responseStr);
                        }//end using  
                    }
                }
                else
                {
                    flag = false;
                }
            }




            return flag;
        }

    }
}
