using Domain.WebAPI.Input.Hotai.Payment;
using Domain.WebAPI.Input.Hotai.Payment.Param;
using Newtonsoft.Json;
using NLog;
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
    public class HotaiPaymentAPI
    {
		protected static Logger logger = LogManager.GetCurrentClassLogger();
		private string PaymentUrl = ConfigurationManager.AppSettings["HotaiPaymentURL"].ToString();                                
		private string EntryURL = ConfigurationManager.AppSettings["HotaiPaymentSingleEntry"].ToString();                                
		private byte[] Key = Convert.FromBase64String(ConfigurationManager.AppSettings["HotaiPaymentKey"].ToString());                               
		private byte[] IV = Convert.FromBase64String(ConfigurationManager.AppSettings["HotaiPaymentIV"].ToString());

		public void GetHotaiCardList(WebAPIInput_GetCreditCards input)
		{
	
			var a = HotaiPaymentApiPost<object, object>(null, "GET", "/creditcard/card", input.AccessToken);
		}

		//手動綁定信用卡
		public void AddCard(WebAPIInput_AddCard input)
		{
            var Body = new Body_AddCard
			{
                RedirectURL = input.RedirectURL
			};

			var a = HotaiPaymentApiPost<object, Body_AddCard>(Body, "POST", "/creditcard/ad", input.AccessToken);

		}

		public void FastAddCard(WebAPIInput_FastAddCard input)
		{
			var Body = new Body_CardFbinding
			{
				RedirectURL = input.RedirectURL,
				IdNo = input.IDNO,
				Birthday = input.Birthday
			};

			var a = HotaiPaymentApiPost<object, Body_CardFbinding>(Body, "POST", "/creditcard/fbinding", input.AccessToken);
		}

		private (bool Succ, string ErrCode, string Message, TResponse Data)
			HotaiPaymentApiPost<TResponse, TRequest>(TRequest Body, string Method, string API, string access_token)
		{
			(bool Succ, string ErrCode, string Message, TResponse Data) valueTuple =
				(false, "000000", "", default(TResponse));

			string BaseUrl = PaymentUrl;
			string api = EntryURL;
			var requestUrl = $"{BaseUrl}{api}";

			var header = SetRequestHeader(access_token);

			var body = Body == null ? "" : JsonConvert.SerializeObject(Body);

			var resinfo = SetRequestBody(body, API, Method);
			
			string content = JsonConvert.SerializeObject(resinfo);

			logger.Info($"Post Body:{content}");

			var result = ApiPost.DoApiPostJson(requestUrl, content, "POST", header);

			valueTuple.Succ = result.Succ;
			valueTuple.ErrCode = result.ErrCode;

			if (result.Succ)
			{
				var a = HotaiReponseDncrypt(result.ResponseData);

				valueTuple.Data = JsonConvert.DeserializeObject<TResponse>(a);
			}

			return valueTuple;
		}

		private HotaiPCRequestBody SetRequestBody(string Body, string Api, string Method )
		{

			return new HotaiPCRequestBody
			{
				Body = string.IsNullOrEmpty(Body) ? "" : HotaiBodyEncrypt(Body),
				Method = Method,
				API = Api
			};

		}

		private WebHeaderCollection SetRequestHeader(string access_token)
		{
			string AppId = ConfigurationManager.AppSettings["HotaiAppId"].ToString();

		    var header = new WebHeaderCollection();
			header.Add("appid", AppId);
			header.Add("token", access_token);

			return header;

		}

		private string HotaiBodyEncrypt(string Body)
        {
			return string.IsNullOrWhiteSpace(Body) ? "" : AESEncrypt.AES128Encrypt(Body,Key, IV,Encoding.UTF8);
        }

		private string HotaiReponseDncrypt(string encryptData)
		{

			return string.IsNullOrWhiteSpace(encryptData) ? "" : AESEncrypt.AES128Decrypt(encryptData, Key, IV, Encoding.UTF8);
		}



	}
}
