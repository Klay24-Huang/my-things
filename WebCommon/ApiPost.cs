using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

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
            if (string.IsNullOrWhiteSpace(AccountToken))
                request.Headers.Add("Authorization", "Bearer " + AccountToken);
            request.Timeout = 30000;

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
    }
}
