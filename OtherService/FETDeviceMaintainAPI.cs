using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Domain.WebAPI.Input.FET;
using Domain.WebAPI.output.FET;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace OtherService
{
    public class FETDeviceMaintainAPI
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();

        string BasePath = ConfigurationManager.AppSettings.Get("CatDeviceMaintainBaseURL");
        string GCPBasePath = ConfigurationManager.AppSettings.Get("GCPDeviceMaintainBaseURL");
        string user = ConfigurationManager.AppSettings.Get("CatAccount");
        string pwd = ConfigurationManager.AppSettings.Get("CatPassword");
        string token = "";
        string refreshToken = "";

        public FETDeviceMaintainAPI()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
        }
        public enum CATCarType
        {
            Car,
            Motor
        }
        public enum GCPCarType
        {
            Vehicle,
            Motorcycle
        }
        public WebAPIOutput_ResultDTO<WebAPIOutput_Login> DoLogin()
        {
            WebAPIOutput_ResultDTO<WebAPIOutput_Login> result = new WebAPIOutput_ResultDTO<WebAPIOutput_Login>();

            try
            {
                string url = "api/auth/login";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BasePath + url);
                request.Method = "POST";
                request.ContentType = "application/json";

                var loginData = new
                {
                    username = user,
                    password = pwd
                };

                //要發送的字串轉為byte[] 
                byte[] byteArray = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(loginData));
                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);
                }

                //API回傳的字串
                string responseStr = "";
                //發出Request
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseStr = sr.ReadToEnd();
                        var loginResult = JsonConvert.DeserializeObject<WebAPIOutput_Login>(responseStr);
                        token = loginResult.token;
                        refreshToken = loginResult.refreshToken;
                        if (token != "")
                        {
                            result.Result = true;
                        }
                        else
                        {
                            throw new Exception("token is empty");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                result.Message = ex.Message;
            }

            return result;
        }

        public WebAPIOutput_ResultDTO<WebAPIOutput_AddDeviceToken> AddDeviceToken(string carNo, CATCarType carType)
        {
            WebAPIOutput_ResultDTO<WebAPIOutput_AddDeviceToken> result = new WebAPIOutput_ResultDTO<WebAPIOutput_AddDeviceToken>();

            try
            {
                if (!string.IsNullOrEmpty(carNo))
                {
                    carNo = carNo.Trim();
                    var guid = Guid.NewGuid().ToString().ToUpper().Substring(0, 20 - carNo.Length - 1);
                    string accessToken = string.Format("{0}-{1}", carNo, guid);

                    string url = string.Format("api/device?accessToken={0}", accessToken);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BasePath + url);
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.Headers.Add("X-Authorization", "Bearer " + token);

                    var addDeviceTokenData = new
                    {
                        name = carNo,
                        type = carType.ToString()
                    };

                    //要發送的字串轉為byte[] 
                    byte[] byteArray = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(addDeviceTokenData));
                    using (Stream reqStream = request.GetRequestStream())
                    {
                        reqStream.Write(byteArray, 0, byteArray.Length);
                    }

                    //API回傳的字串
                    string responseStr = "";
                    //發出Request
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                        {
                            responseStr = sr.ReadToEnd();
                            JObject jObjData = JsonConvert.DeserializeObject<JObject>(responseStr);
                            if (jObjData["id"] != null && jObjData["id"]["id"] != null)
                            {
                                result.Result = true;
                                result.Data = new WebAPIOutput_AddDeviceToken
                                {
                                    deviceId = jObjData["id"]["id"].ToString(),
                                    deviceToken = accessToken
                                };
                            }
                            else
                            {
                                throw new Exception("無法取得資料。source=" + responseStr);
                            }
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                using (StreamReader sr = new StreamReader(ex.Response.GetResponseStream(), Encoding.UTF8))
                {
                    var responseStr = sr.ReadToEnd();
                    JObject jObjErrData = JsonConvert.DeserializeObject<JObject>(responseStr);
                    logger.Error(responseStr);
                    if (jObjErrData != null && jObjErrData["message"] != null)
                    {
                        result.Message = jObjErrData["message"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                result.Message = ex.Message;
            }

            return result;
        }

        public WebAPIOutput_ResultDTO<WebAPIOutput_QueryDeviceToken> QueryDeviceToken(string deviceId)
        {
            WebAPIOutput_ResultDTO<WebAPIOutput_QueryDeviceToken> result = new WebAPIOutput_ResultDTO<WebAPIOutput_QueryDeviceToken>();

            try
            {
                string url = string.Format("api/device/{0}/credentials", deviceId);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BasePath + url);
                request.Method = "GET";
                request.Headers.Add("X-Authorization", "Bearer " + token);

                //API回傳的字串
                string responseStr = "";
                //發出Request
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseStr = sr.ReadToEnd();
                        result.Result = true;
                        result.Data = JsonConvert.DeserializeObject<WebAPIOutput_QueryDeviceToken>(responseStr);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                result.Message = ex.Message;
            }

            return result;
        }

        public WebAPIOutput_ResultDTO<WebAPIOutput_GetDeviceInfo> QueryDeviceInfo(string deviceId)
        {
            WebAPIOutput_ResultDTO<WebAPIOutput_GetDeviceInfo> result = new WebAPIOutput_ResultDTO<WebAPIOutput_GetDeviceInfo>();

            try
            {
                string url = string.Format("api/plugins/telemetry/DEVICE/{0}/values/timeseries", deviceId);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BasePath + url);
                request.Method = "GET";
                request.Headers.Add("X-Authorization", "Bearer " + token);

                //API回傳的字串
                string responseStr = "";
                //發出Request
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseStr = sr.ReadToEnd();
                        JObject jObjData = JsonConvert.DeserializeObject<JObject>(responseStr);
                        if (jObjData["deviceCID"] != null && jObjData["deviceName"] != null)
                        {
                            result.Result = true;
                            result.Data = new WebAPIOutput_GetDeviceInfo
                            {
                                deviceCID = jObjData["deviceCID"]["value"].ToString(),
                                deviceName = jObjData["deviceName"]["value"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                result.Message = ex.Message;
            }

            return result;
        }

        public WebAPIOutput_ResultDTO<WebAPIOutput_GetDeviceId> QueryDeviceId(string carNo)
        {
            WebAPIOutput_ResultDTO<WebAPIOutput_GetDeviceId> result = new WebAPIOutput_ResultDTO<WebAPIOutput_GetDeviceId>();

            try
            {
                string url = string.Format("/api/tenant/devices?customerId=&limit=10&type&textSearch={0}", carNo);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BasePath + url);
                request.Method = "GET";
                request.Headers.Add("X-Authorization", "Bearer " + token);

                //API回傳的字串
                string responseStr = "";
                //發出Request
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseStr = sr.ReadToEnd();
                        JObject jObjData = JsonConvert.DeserializeObject<JObject>(responseStr);
                        if (jObjData["data"] != null)
                        {
                            JArray jArrData = (JArray)jObjData["data"];
                            if (jArrData.Count > 0)
                            {
                                JObject jObjId = (JObject)jArrData[0];
                                if (jObjId["id"] != null)
                                {
                                    if (jObjId["id"]["id"] != null)
                                    {
                                        result.Result = true;
                                        result.Data = new WebAPIOutput_GetDeviceId
                                        {
                                            deviceId = jObjId["id"]["id"].ToString()
                                        };
                                    }
                                }
                            }
                        }
                        if (!result.Result)
                        {
                            throw new Exception("無法取得資料。source=" + responseStr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                result.Message = ex.Message;
            }

            return result;
        }

        public WebAPIOutput_ResultDTO<List<WebAPIOutput_GCPUpMapping>> GCPUpMapping(List<WebAPIInput_GCPUpMapping> listData)
        {
            WebAPIOutput_ResultDTO<List<WebAPIOutput_GCPUpMapping>> result = new WebAPIOutput_ResultDTO<List<WebAPIOutput_GCPUpMapping>>();

            try
            {
                string url = "DevMapping/api/IDUMap/UpMapping";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GCPBasePath + url);
                request.Method = "POST";
                request.ContentType = "application/json";

                //要發送的字串轉為byte[] 
                byte[] byteArray = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(listData));
                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);
                }

                //API回傳的字串
                string responseStr = "";
                //發出Request
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseStr = sr.ReadToEnd();
                        Regex regex = new Regex("^\"(.+)\"$");
                        Match match = regex.Match(responseStr);
                        if (match.Success && match.Groups.Count > 1)
                        {
                            var mStr = match.Groups[1].Value.Replace("\\", "");
                            result.Data = JsonConvert.DeserializeObject<List<WebAPIOutput_GCPUpMapping>>(mStr);
                        }
                        else
                        {
                            result.Data = JsonConvert.DeserializeObject<List<WebAPIOutput_GCPUpMapping>>(responseStr);
                        }
                        result.Result = true;
                        if (!result.Result)
                        {
                            throw new Exception("無法取得資料。source=" + responseStr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                result.Message = ex.Message;
            }

            return result;
        }
    }
}
