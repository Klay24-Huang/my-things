using Domain.WebAPI.Input.Taishin;
using Domain.WebAPI.output.Taishin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebCommon;

namespace OtherService
{
    /// <summary>
    /// 台新綁卡WebAPI
    /// </summary>
    public class TaishinCreditCardBindAPI
    {
        private string apikey = ConfigurationManager.AppSettings["TaishinAPIKey"].ToString();
        /// <summary>
        /// 取得綁卡網址
        /// </summary>
        /// <param name="wsInput"></param>
        /// <param name="errCode"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoBind(WebAPIInput_Base wsInput,ref string errCode,ref WebAPIOutput_Base output)
        {
            bool flag = true;
            string ori = string.Format("request={0}&apikey={1}", Newtonsoft.Json.JsonConvert.SerializeObject(wsInput), apikey);
            string checksum = GenerateSign(ori);
            
            WebAPIInput_Bind Input = new WebAPIInput_Bind()
            {
                ApiVer = wsInput.ApiVer,
                ApposId = wsInput.ApposId,
                Random = wsInput.Random,
                RequestParams = wsInput.RequestParams,
                CheckSum = checksum,
                TimeStamp=wsInput.TimeStamp
        };
            string IV = GenerateCardDataIV(Convert.ToInt64(wsInput.TimeStamp));
            CardData data = new CardData()
            {
                CardName = "玉山銀行",
                CardNumber = "5242556063006744",
                CellPhone = "0989695270",
                Cvv2 = "773",
                ExpDate = "2707",
                MemberId = "C121119150"
            };
            string CardDataStr = GenerateCardData(data, IV);
            output = DoBindSend(Input).Result;
            if (output.RtnCode=="1000")
            {
                //if (output.Data == null)
                //{
                //    flag = false;
                //}
            }
            else
            {
                flag = false;
            }
            return flag;
        }
        /// <summary>
        /// 執行取得綁卡網址
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<WebAPIOutput_Base> DoBindSend(WebAPIInput_Bind input)
        {
            WebAPIOutput_Base output = null;
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://apposweb-t.taishinbank.com.tw/O2OgwApi/api/ws/GetCardPage");
            request.Method = "POST";
            request.ContentType = "application/json";
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                string postBody = JsonConvert.SerializeObject(input);//將匿名物件序列化為json字串
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
                        RTime = DateTime.Now;
                        output = JsonConvert.DeserializeObject<WebAPIOutput_Base>(responseStr);
                    }

                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                //output = new WebAPIOutput_NPR260Send()
                //{

                //    Message = "發生異常錯誤",
                //    Result = false
                //};
            }
            finally
            {
                //SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                //{
                //    MKTime = MKTime,
                //    UPDTime = RTime,
                //    WebAPIInput = JsonConvert.SerializeObject(input),
                //    WebAPIName = "NPR260Send",
                //    WebAPIOutput = JsonConvert.SerializeObject(output),
                //    WebAPIURL = BaseURL + NPR260SendURL
                //};
                //bool flag = true;
                //string errCode = "";
                //List<ErrorInfo> lstError = new List<ErrorInfo>();
                //new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }


            return output;
        }
        /// <summary>
        /// 產生CardData IV
        /// </summary>
        /// <param name="Timestamp"></param>
        /// <returns></returns>
        public string GenerateCardDataIV(long Timestamp)
        {
            string ACode, BCode, CCode, DCode, ECode = "";
            BCode = (Timestamp % 10000).ToString().PadLeft(4,'0'); //除10000取餘數，等同於抓最後四碼
            CCode = Cal(BCode);
            DCode = Cal(CCode);
            for (int i = 0; i <4; i++)
            {
                int value = (Convert.ToInt32(BCode.Substring(i, 1)) + Convert.ToInt32(CCode.Substring(i, 1)) + Convert.ToInt32(DCode.Substring(i, 1))) % 10;
                ECode += value.ToString();
            }
            return BCode + CCode + DCode + ECode;

        }
        /// <summary>
        /// 產生CardData加密
        /// </summary>
        /// <param name="MemberId">會員編號</param>
        /// <param name="CellPhone">手機號碼</param>
        /// <param name="CardNumber">信用卡號</param>
        /// <param name="ExpDate">信用卡有效日期YYMM</param>
        /// <param name="Cvv2">信用卡末三碼</param>
        /// <param name="CardName">卡片名稱</param>
        /// <param name="Timestamp">用來產生電文的Timestamp（10碼）</param>
        /// <returns></returns>
        public string GenerateCardData(CardData data,string IV)
        {
            AESEncrypt encrypt = new AESEncrypt();
            return encrypt.doEncrypt(apikey, IV, JsonConvert.SerializeObject(data));

        }
        private string Cal(string num)
        {
            string output = "";
            int len = num.Length;
            int sum = 0;
            for(int i = 0; i <len; i++)
            {
                int baseValue = Convert.ToInt32(num.Substring(i, 1));
                if (baseValue > 0)
                {
                    sum += Convert.ToInt32(((Math.Pow(baseValue, 2)) % 10) * Math.Pow(10, 3 - i));
                }
                
            }
            return sum.ToString().PadLeft(3,'0');
        }
        /// <summary>
        /// 產生簽章
        /// </summary>
        /// <param name="ori"></param>
        /// <returns></returns>
        public string GenerateSign(string ori)
        {
            System.Security.Cryptography.SHA256 sha256 = new System.Security.Cryptography.SHA256CryptoServiceProvider();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(ori);
            byte[] hash = sha256.ComputeHash(bytes);
            string sign = Convert.ToBase64String(hash).ToUpper();
            return sign;
        }
    }
}
