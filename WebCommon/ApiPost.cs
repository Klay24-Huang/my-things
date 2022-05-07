using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace WebCommon
{
    public class ApiPost
    {
        /// <summary>
        /// Post for a API Result
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="apiSite"></param>
        /// <param name="apiPath"></param>
        /// <param name="input"></param>
        /// <param name="AccountToken"></param>
        /// <param name="FunName"></param>
        /// <returns></returns>
        public static (bool Succ, string errCode, string Message, TResponse Data) DoApiPost<TResponse, TRequest>
            (TRequest input, string url, string AccountToken)
        {
            (bool Succ, string errCode, string Message, TResponse Data) valueTuple =
                (false, "000000", "", default(TResponse));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = "application/json";
            request.KeepAlive = false;
            if (!string.IsNullOrWhiteSpace(AccountToken))
                request.Headers.Add("Authorization", "Bearer " + AccountToken);
            request.Timeout = 90000;

            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                string jsonText = JsonConvert.SerializeObject(input);//將匿名物件序列化為json字串
                byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonText);//要發送的字串轉為byte[]
                request.ContentLength = jsonBytes.Length;

                using (var requestStream = request.GetRequestStream())
                {
                    requestStream.Write(jsonBytes, 0, jsonBytes.Length);
                    requestStream.Flush();
                }
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var result = reader.ReadToEnd();

                        if (result.Length <= 0) throw new Exception($"errCode:ERR918");
                        valueTuple.Succ = true;
                        valueTuple.Data = JsonConvert.DeserializeObject<TResponse>(result);
                    }
                }
            }
            catch (WebException webex)
            {
                valueTuple.Succ = false;
                valueTuple.errCode = "ERR918";
                valueTuple.Message = webex.Message;
            }
            catch (Exception ex)
            {
                valueTuple.Succ = false;
                if (ex.Message.Contains("errCode:"))
                {
                    string errCode = ex.Message.Split(':')[1]; //Api呼叫失敗
                    valueTuple.errCode = errCode;
                }
                else
                {
                    valueTuple.errCode = "ERR913";//發生錯誤,請洽系統管理員
                    valueTuple.Message = ex.Message;
                }

                valueTuple.Data = default(TResponse);
            }
            finally
            {

                //增加關閉Request的處理
                request.Abort();
            }

            return valueTuple;
        }

        /// <summary>
        /// Post Json for a API Result
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <param name="Method"></param>
        /// <param name="Header"></param>
        /// <returns></returns>
        public static PostJsonResultInfo DoApiPostJson(string url, string content, string Method, WebHeaderCollection Header)
        {
            var resultInfo = new PostJsonResultInfo();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            if(Header != null)
                request.Headers = Header;
            request.Method = Method;
            request.ContentType = "application/json";
            request.KeepAlive = false;
            request.Timeout = 30000;

            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                if (!string.IsNullOrWhiteSpace(content))
                {
                    byte[] jsonBytes = Encoding.UTF8.GetBytes(content);//要發送的字串轉為byte[]
                    request.ContentLength = jsonBytes.Length;

                    using (var requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(jsonBytes, 0, jsonBytes.Length);
                    }
                }
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var result = reader?.ReadToEnd() ?? "";

                        if (result == null) throw new Exception($"errCode:ERR918");

                        resultInfo.Succ = true;
                        resultInfo.ResponseData = result;
                        resultInfo.ProtocolStatusCode = (int)response.StatusCode;
                    }
                }
            }
            catch (WebException webex)
            {
                resultInfo.Succ = false;
                resultInfo.ErrCode = "ERR917";
                resultInfo.Message = webex.Message;

                var response = webex.Response as HttpWebResponse;

                if (response != null)
                {
                    resultInfo.ProtocolStatusCode = (int)response.StatusCode;
                    using (StreamReader reader = new StreamReader(webex.Response.GetResponseStream()))
                    {
                        string responseContent = reader.ReadToEnd();
                        resultInfo.ResponseData = responseContent;
                    }
                }

            }
            catch (Exception ex)
            {
                resultInfo.Succ = false;
                if (ex.Message.Contains("errCode:"))
                {
                    string errCode = ex.Message.Split(':')[1]; //Api呼叫失敗
                    resultInfo.ErrCode = errCode;
                }
                else
                {
                    resultInfo.ErrCode = "ERR913";//發生錯誤,請洽系統管理員
                    resultInfo.Message = ex.Message;
                }
            }
            finally
            {

                //增加關閉Request的處理
                request.Abort();
            }

            return resultInfo;

        }


        /// <summary>
        /// Post Json for a API Result
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <param name="Method"></param>
        /// <param name="Header"></param>
        /// <returns></returns>
        public static PostJsonResultInfo DoApiPostForm<T>(string url, T content, string Method, WebHeaderCollection Header)
        {
            var resultInfo = new PostJsonResultInfo();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            if (Header != null)
                request.Headers = Header;
            request.Method = Method;
            request.ContentType = "application/x-www-form-urlencoded";
            request.KeepAlive = false;
            request.Timeout = 30000;

            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                string postData = ObjToFormData<T>(content);
                byte[] postDataByte = Encoding.UTF8.GetBytes(postData);//要發送的字串轉為byte[]

                request.ContentLength = postDataByte.Length;

                using (var requestStream = request.GetRequestStream())
                {
                    requestStream.Write(postDataByte, 0, postDataByte.Length);
                }
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var result = reader.ReadToEnd();

                        if (result.Length <= 0) throw new Exception($"errCode:ERR918");

                        resultInfo.Succ = true;
                        resultInfo.ResponseData = result;
                        resultInfo.ProtocolStatusCode = (int)response.StatusCode;
                    }
                }
            }
            catch (WebException webex)
            {
                resultInfo.Succ = false;
                resultInfo.ErrCode = "ERR917";
                resultInfo.Message = webex.Message;

                var response = webex.Response as HttpWebResponse;

                if (response != null)
                {
                    resultInfo.ProtocolStatusCode = (int)response.StatusCode;
                    using (StreamReader reader = new StreamReader(webex.Response.GetResponseStream()))
                    {
                        string responseContent = reader.ReadToEnd();
                        resultInfo.ResponseData = responseContent;
                    }
                }

            }
            catch (Exception ex)
            {
                resultInfo.Succ = false;
                if (ex.Message.Contains("errCode:"))
                {
                    string errCode = ex.Message.Split(':')[1]; //Api呼叫失敗
                    resultInfo.ErrCode = errCode;
                }
                else
                {
                    resultInfo.ErrCode = "ERR913";//發生錯誤,請洽系統管理員
                    resultInfo.Message = ex.Message;
                }
            }
            finally
            {

                //增加關閉Request的處理
                request.Abort();
            }

            return resultInfo;

        }



        private static string ObjToFormData<T>(T input)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var t in input.GetType().GetProperties())
            {
                string value = t.GetValue(input)?.ToString() ?? "";
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (sb.Length > 0)
                        sb.Append("&");
                    sb.Append(t.Name).Append("=").Append(value);
                }
            }
            return sb.ToString(); ;
        }
    }
}
