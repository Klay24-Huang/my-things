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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using WebCommon;

namespace OtherService
{
    public class HotaiPaymentAPI
    {
		protected static Logger logger = LogManager.GetCurrentClassLogger();
		private static ConfigManager configManager = new ConfigManager("hotaipayment");
		private string PaymentUrl = configManager.GetKey("HotaiPaymentURL");//ConfigurationManager.AppSettings["HotaiPaymentURL"].ToString();                                
		private string EntryURL = configManager.GetKey("HotaiPaymentSingleEntry");//ConfigurationManager.AppSettings["HotaiPaymentSingleEntry"].ToString();                                
		private string _bindCardURL = configManager.GetKey("CTBCBindCard"); //ConfigurationManager.AppSettings["CTBCBindCard"].ToString();
		private string _fastBind = configManager.GetKey("CTBCFastBind");//ConfigurationManager.AppSettings["CTBCFastBind"].ToString();
		private byte[] Key = Convert.FromBase64String(configManager.GetKey("HotaiPaymentKey"));//Convert.FromBase64String(ConfigurationManager.AppSettings["HotaiPaymentKey"].ToString());                               
		private byte[] IV = Convert.FromBase64String(configManager.GetKey("HotaiPaymentIV"));//Convert.FromBase64String(ConfigurationManager.AppSettings["HotaiPaymentIV"].ToString());

		public bool GetHotaiCardList(WebAPIInput_GetCreditCards input, ref WebAPIOutput_GetCreditCards output)
		{
			var apiResult = HotaiPaymentApiPost<WebAPIOutput_PaymentGeneric<List<HotaiCardInfoOriginal>>, object>(null, "GET", "/creditcard/card", input.AccessToken);
			logger.Info($"GetHotaiCardList apiResult {JsonConvert.SerializeObject(apiResult)}");

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
			//自訂物件 務必先初始化
			output.PostData = new HotaiResReqJsonPwd();
			var Body = new Body_AddCard
			{
				RedirectURL = input.RedirectURL
			};

			logger.Info($"AddCard requset body {JsonConvert.SerializeObject(Body)}");
			var apiResult = HotaiPaymentApiPost<WebAPIOutput_PaymentGeneric<HotaiResAddCard>, Body_AddCard>(Body, "POST", "/creditcard/add", input.AccessToken);

			logger.Info($"AddCard apiResult {JsonConvert.SerializeObject(apiResult)}");
			if (apiResult.Succ)
			{
				output.Succ = apiResult.Succ;
				output.GotoUrl = _bindCardURL;
				output.ResponseData = apiResult.Data?.Data;
				output.PostData.reqjsonpwd = apiResult.Data?.Data?.reqjsonpwd;
			}

			logger.Info($"AddCard output {JsonConvert.SerializeObject(output)}");
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
			logger.Info($"FastAddCard apiResult {JsonConvert.SerializeObject(apiResult)}");
			
			if (apiResult.Succ)
			{
				output.Succ = apiResult.Succ;
				output.GotoUrl = _fastBind;
				output.PostData = apiResult.Data?.Data;
			}
			logger.Info($"FastAddCard output {JsonConvert.SerializeObject(output)}");
			return apiResult.Succ;

		}

		/// <summary>
		/// 信用卡授權請求加密
		/// </summary>
		/// <returns></returns>
		public bool CreaditCardPayEncpypt(WebAPIInput_CreditCardPayEncpypt input, ref WebAPIOutput_CreditCardPayEncrypt output)
		{
			var Body = new Body_CreditCardPay
			{
				TokenID = int.Parse(input.CardToken),
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
			
			var jsonSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
			logger.Info($"CreaditCardPayEncpypt requset body {JsonConvert.SerializeObject(Body, Formatting.Indented, jsonSetting)}");

			var apiResult = HotaiPaymentApiPost<WebAPIOutput_PaymentGeneric<HotaiResReqJsonPwd>, Body_CreditCardPay>(Body, "POST", "/creditcard/pay/json", input.AccessToken);

			var reqjsonpwd = "";
			logger.Info($"CreaditCardPayEncpypt apiResult {JsonConvert.SerializeObject(apiResult)}");
			if (apiResult.Succ)
			{
				reqjsonpwd = apiResult.Data?.Data?.reqjsonpwd ?? "";
			}
			if(string.IsNullOrWhiteSpace(reqjsonpwd))
            {
				apiResult.Succ = false;

			}
			if (apiResult.Succ)
			{
				output.PostData = apiResult.Data?.Data;
			}
			logger.Info($"CreaditCardPayEncpypt output {JsonConvert.SerializeObject(output)}");
			return apiResult.Succ;
		}

		/// <summary>
		/// 信用卡授權請求
		/// </summary>
		/// <returns></returns>
		public bool CreaditCardPay(WebAPIInput_CreditCardPay input, ref WebAPIOutput_CreditCardPay output)
		{
			var apiResult = new PostJsonResultInfo();

			logger.Info($"CreaditCardPay requset {JsonConvert.SerializeObject(input.PostData)}");

			apiResult = ApiPost.DoApiPostForm<HotaiResReqJsonPwd>(_bindCardURL, input.PostData, "POST", null);

			logger.Info($"CreaditCardPay apiResult {JsonConvert.SerializeObject(apiResult)}");

			output.ProtocolStatusCode = apiResult.ProtocolStatusCode;
			if (output.ProtocolStatusCode == 200)
			{
				output.PageText = apiResult.ResponseData;
				output.PageTitle = GetCTBCPageTitle(apiResult.ResponseData);
			}
			else
			{
				apiResult.Succ = false;
				output.ErrorCode = apiResult.ErrCode;
				output.ErrorMessage = apiResult.Message;
			}

			logger.Info($"CreaditCardPay output {JsonConvert.SerializeObject(output)}");
			return apiResult.Succ;
		}


		public bool DecryptCTBCHtml(WebAPIInput_DecryptCTBCHtml input, ref WebAPIOutput_DecryptCTBCHtml output)
        {

			var flag = true;
			var Body = new Body_CreditCardDecrypt
			{
				Data = Uri.EscapeUriString(input.PageText)
				//WebUtility.UrlEncode()  空白會轉成+
			};

			logger.Info($"Body_DecryptCTBCHtml requset body {JsonConvert.SerializeObject(Body)}");
			
			var apiResult = HotaiPaymentApiPost<WebAPIOutput_PaymentGeneric<String>, Body_CreditCardDecrypt>(Body, "POST", "/creditcard/decrypt/html", input.AccessToken);
			
			logger.Info($"Body_DecryptCTBCHtml apiResult {JsonConvert.SerializeObject(apiResult)}");


			if (!apiResult.Succ)
            {
				flag = false;
				output.ErrorCode = apiResult.ErrCode;
				output.ErrorMessage = apiResult.Message;
			}
			
			if(flag)
            {
				var QueryStringToObject = new QueryStringHelper().QueryStringToObject<WebAPIOutput_DecryptCTBCHtml>(apiResult.Data.Data);
				output = QueryStringToObject;
				output.FullString = apiResult.Data.Data;

				if(output.Errcode == "00" && output.StatusCode == "I0000" && output.Status == 0)
                {
					flag = true;
				}
				else
                {
					flag = false;
                }
			}
			return flag;
		}

		/// <summary>
		/// Hotai金流 共用 POST Method
		/// </summary>
		/// <typeparam name="TResponse"></typeparam>
		/// <typeparam name="TRequest"></typeparam>
		/// <param name="Body"></param>
		/// <param name="Method"></param>
		/// <param name="API"></param>
		/// <param name="access_token"></param>
		/// <returns></returns>
		private (bool Succ, string ErrCode, string Message, TResponse Data)
			HotaiPaymentApiPost<TResponse, TRequest>(TRequest Body, string Method, string API, string access_token)
		{
			(bool Succ, string ErrCode, string Message, TResponse Data) valueTuple =
				(false, "000000", "", default(TResponse));

			string BaseUrl = PaymentUrl;
			string api = EntryURL;
			var requestUrl = $"{BaseUrl}{api}";

			var header = SetRequestHeader(access_token);


			var jsonSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
			var body = Body == null ? "" : JsonConvert.SerializeObject(Body, Formatting.Indented,jsonSetting);

			logger.Info($@"Request Info
								--requestUrl:{requestUrl}
								--API:{API}
								--method:{Method}
								--header:{header}
								--origana body:
								{body}");

			var resinfo = SetRequestBody(body, API, Method);

			string content = JsonConvert.SerializeObject(resinfo);

			logger.Info($"Request Info -- Encrypt:{content}");

			var result = ApiPost.DoApiPostJson(requestUrl, content, "Post", header);

			valueTuple.Succ = result.Succ;
			valueTuple.ErrCode = result.ErrCode;

			logger.Info($"response Info -- {JsonConvert.SerializeObject(result)}");
			if (result.Succ)
			{
				var reponseDncrypt = HotaiReponseDncrypt(result.ResponseData);

				valueTuple.Data = JsonConvert.DeserializeObject<TResponse>(reponseDncrypt);
			}
			else
			{
				if(result.ProtocolStatusCode == 400 || result.ProtocolStatusCode == 500)
                {
					var reponseDncrypt = HotaiReponseDncrypt(result.ResponseData);

					valueTuple.Data = JsonConvert.DeserializeObject<TResponse>(reponseDncrypt);
				}
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
			string AppId = configManager.GetKey("HotaiAppId");

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

		private string GetCTBCPageTitle(string input)
        {

			string MatchPattern = @"<title>(?'title'.*)</title>";
			
			var match = Regex.Match(input, MatchPattern);
			
			string title = "";
			if (match.Success)
			{
				title = match.Groups["title"].Value;
			}

			return title;

		}

	
	}
}
