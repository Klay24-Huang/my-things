using Domain.WebAPI.Input.Hotai.Payment;
using Domain.WebAPI.Input.Hotai.Payment.Param;
using Domain.WebAPI.output.Hotai.Payment;
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
		private static ConfigManager configManager = new ConfigManager("HotaiPaySetting.json");
		private string PaymentUrl = configManager.GetKey("HotaiPaymentURL");//ConfigurationManager.AppSettings["HotaiPaymentURL"].ToString();                                
		private string EntryURL = configManager.GetKey("HotaiPaymentSingleEntry");//ConfigurationManager.AppSettings["HotaiPaymentSingleEntry"].ToString();                                
		private string _bindCardURL = configManager.GetKey("CTBCBindCard"); //ConfigurationManager.AppSettings["CTBCBindCard"].ToString();
		private string _fastBind = configManager.GetKey("CTBCFastBind");//ConfigurationManager.AppSettings["CTBCFastBind"].ToString();
		private byte[] Key = Convert.FromBase64String(configManager.GetKey("HotaiPaymentKey"));//Convert.FromBase64String(ConfigurationManager.AppSettings["HotaiPaymentKey"].ToString());                               
		private byte[] IV = Convert.FromBase64String(configManager.GetKey("HotaiPaymentIV"));//Convert.FromBase64String(ConfigurationManager.AppSettings["HotaiPaymentIV"].ToString());

		public bool GetHotaiCardList(WebAPIInput_GetCreditCards input, ref WebAPIOutput_GetCreditCards output)
		{
			logger.Info("GetHotaiCardList init");
			var apiResult = HotaiPaymentApiPost<WebAPIOutput_PaymentGeneric<List<HotaiCardInfoOriginal>>, object>(null, "GET", "/creditcard/card", input.AccessToken);

			List<HotaiCardInfoOriginal> hotaiCards = apiResult.Succ ?
				apiResult.Data.Data?.Where(t => t.IsAvailable && !t.IsOverwrite).ToList() :
				new List<HotaiCardInfoOriginal>();

			output.HotaiCards = hotaiCards;
			output.CardCount = hotaiCards.Count;

			logger.Info($"hotaiCards:{JsonConvert.SerializeObject(hotaiCards)}");
			return output.CardCount > 0 ? true : false;
		}

		/// <summary>
		/// 手動綁定信用卡
		/// </summary>
		/// <param name="input"></param>
		/// <param name="output"></param>
		/// <returns></returns>
		public bool AddCard(WebAPIInput_AddCard input, ref WebAPIOutput_AddHotaiCards output)
		{
			logger.Info("AddCard init");
			var Body = new Body_AddCard
			{
				RedirectURL = input.RedirectURL
			};

			logger.Info($"AddCard requset body {JsonConvert.SerializeObject(Body)}");
			var apiResult = HotaiPaymentApiPost<WebAPIOutput_PaymentGeneric<HotaiResAddCard>, Body_AddCard>(Body, "POST", "/creditcard/add", input.AccessToken);


			logger.Info($"AddCard body {JsonConvert.SerializeObject(apiResult)}");
			if (apiResult.Succ)
			{
				output.Succ = apiResult.Succ;
				output.GotoUrl = _bindCardURL;
				output.PostData = apiResult.Data?.Data;
			}

			return apiResult.Succ;
		}
		/// <summary>
		/// 快速步驟
		/// </summary>
		/// <param name="input"></param>
		/// <param name="output"></param>
		/// <returns></returns>
		public bool FastAddCard(WebAPIInput_FastAddCard input, ref WebAPIOutput_FastAddHotaiCard output)
		{
			logger.Info("FastAddCard init");
			var Body = new Body_CardFbinding
			{
				RedirectURL = input.RedirectURL,
				IdNo = input.IDNO,
				Birthday = input.Birthday
			};
			logger.Info($"FastAddCard requset body {JsonConvert.SerializeObject(Body)}");

			var apiResult = HotaiPaymentApiPost<WebAPIOutput_PaymentGeneric<HotaiResFastBind>, Body_CardFbinding>(Body, "POST", "/creditcard/fbinding", input.AccessToken);
			if (apiResult.Succ)
			{
				output.Succ = apiResult.Succ;
				output.GotoUrl = _fastBind;
				output.PostData = apiResult.Data?.Data;
			}
			return apiResult.Succ;

		}

		/// <summary>
		/// 信用卡授權請求
		/// </summary>
		/// <returns></returns>
		public bool CreaditCardPay(WebAPIInput_CreditCardPay input)
		{
			var Body = new Body_CreditCardPay
			{
				TokenID = input.TokenID,
				MerID = input.MerID,
				TerMinnalID = input.TerMinnalID,
				Lidm = input.Lidm,
				PurchAmt = input.PurchAmt,
				TxType = input.TxType,
				AutoCap = input.AutoCap,
				RedirectUrl = input.RedirectUrl,
				OrderDesc = input.OrderDesc,
				Pid = input.Pid,
				Birthday = input.Birthday,
				Customize = input.Customize,
				MerchantName = input.MerchantName,
				NumberOfPay = input.NumberOfPay,
				PromoCode = input.PromoCode,
				ProdCode = input.ProdCode
			};

			var apiResult = HotaiPaymentApiPost<WebAPIOutput_PaymentGeneric<HotaiResCreditCardPay>, Body_CreditCardPay>(Body, "POST", "/creditcard/pay/json", input.AccessToken);
			var reqjsonpwd = "";

			if (apiResult.Succ)
			{
				reqjsonpwd = apiResult.Data?.Data?.reqjsonpwd ?? "";
			}

			//var CTBCResult = new PostJsonResultInfo();
			//if (!string.IsNullOrWhiteSpace(reqjsonpwd))
			//	CTBCResult = ApiPost.DoApiPostForm(_bindCardURL, reqjsonpwd, "POSTk", null);
			

			return apiResult.Succ;
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

			logger.Info($@"Request Info--requestUrl:{requestUrl}
									   --method:{Method}
									   --header:{header}
							           --origana body
									   --{body}");

			var resinfo = SetRequestBody(body, API, Method);

			string content = JsonConvert.SerializeObject(resinfo);

			logger.Info($"Request Info -- Encrypt:{content}");

			var result = ApiPost.DoApiPostJson(requestUrl, content, "Post", header);

			valueTuple.Succ = result.Succ;
			valueTuple.ErrCode = result.ErrCode;

			logger.Info($"response Info -- {JsonConvert.SerializeObject(result)}");
			if (result.Succ)
			{
				var a = HotaiReponseDncrypt(result.ResponseData);

				valueTuple.Data = JsonConvert.DeserializeObject<TResponse>(a);
			}
			else
			{
				if (result.ProtocolStatusCode == 401)
					valueTuple.Data = JsonConvert.DeserializeObject<TResponse>(result.ResponseData);
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
