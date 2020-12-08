using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain.SP.Output;
using Domain.SP.Sync.Input;
using Domain.Sync.Input;
using Domain.Sync.Output;
using Domain.TB.Sync;
using IdentityModel.Client;
using Newtonsoft.Json;
using Reposotory.Implement.Sync;
using WebCommon;

namespace PushService
{
    class Program
    {
        private static string _accessToken = "";
        private static JwtSecurityTokenHandler _tokenHandler;
        private static int NowCount = 0;
        private static int MaxCount = 10;
        private static string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        static System.Threading.Thread PushMessageThread;
        static System.Threading.ThreadStart ts = new System.Threading.ThreadStart(new Program().DoPushMessage);
        public static int restartCount = 0;
        static void Main()
        {
            PushMessageThread = new System.Threading.Thread(ts);
            PushMessageThread.Start();

        }
        private void checkThread()
        {
            Thread waitThread = new Thread(checkThread);
            if (false == PushMessageThread.IsAlive)
            {
                PushMessageThread = new Thread(ts);
                PushMessageThread.Start();
                //  Console.WriteLine("重啟thread");
                if (waitThread.IsAlive)
                {
                    waitThread.Abort();
                }
            }
            else
            {
                // Thread.Sleep(1000);

                waitThread.Start();
            }
        }
        private async void DoPushMessage()
        {
            if (NowCount >= MaxCount)
            {
                Console.WriteLine("休息一分鐘開始");
                Thread.Sleep(60000);
                Console.WriteLine("休息一分鐘結束");
                NowCount = 0;
            }
            try
            {
                SendMsgRepository _repository = new SendMsgRepository(connetStr);
                List<Sync_SendMessage> lstData = _repository.GetMessage();
                if (lstData != null)
                {
                    int len = lstData.Count();
                    for(int i = 0; i < len; i++)
                    {
                        if (lstData[i].UserToken != "0")
                        {
                            await PushMessage(lstData[i].UserToken, lstData[i].Message, lstData[i].url, lstData[i].NotificationID, lstData[i].NType);
                        }
                        
                    }
                }
              
            }
            catch(Exception ex)
            {

            }
            finally
            {
                Console.WriteLine("休息五秒鐘開始");
                 Thread.Sleep(5000);
                if (NowCount >= MaxCount)
                {
                    NowCount = 0;
                }
                else
                {
                    NowCount++;
                }
                Console.WriteLine("休息五秒鐘結束，再次啟動");
                checkThread();
            }
           
        }
        private static async Task PushMessage(string registerIdsT,string messageContent,string weburlContent,Int64 NewsID,int Type)
        {
     

            var startDate = DateTime.Now;
            //推播web api url

            var pushServerUrl = "https://sspushserverprod.azurewebsites.net";
            //var pushServerUrl = "http://localhost:5010";
            var pushMessageApi = "api/notification/PushMessage";
            var registerIds = (Type!=0 && registerIdsT!="0")?registerIdsT.Split(',').Select(long.Parse).ToArray():null; // 推播的接收者id


            //string messageContent = "Test aaa";
            // string weburlContent = "";

            // for test 1600 registers from 243 to 1842
            //for (int i = 243; i <= 1342; i++)
            //{
            //    registerIds.Add(i);
            //}

             var httpClient = new HttpClient();
            var message = new SyncInput_MessageParam()
            {
                AppId = 12, // appid 目前測試主機上是7
                RegisterIds = registerIds,
                MessageId = 0, // 永遠設0
                Title = messageContent,
                Content = messageContent,
                MessageTypeCode = "03",
                CategoryCode = "01",
                ImageUrl = "",
                WebUrl = weburlContent,
                ShareTo = 0,
                ExternalTitle = ",",
                ExternalUrl = ",",
                Page2Title = ",",
                Page3Title = ",",
                Page3Content = ",",
                Info01 = "",
                Info02 = "",
                Info03 = "",
                Info04 = "",
                Info05 = "",
                Info06 = "",
                Info07 = "",
                Info08 = "",
                Info09 = "",
                Info10 = "",
                EndTime = ToUnixTime(DateTime.Now.AddDays(1)),
                UserId = "",
                 ListType=(Type==0)?"01":""
                 
            };
            var httpContent = new JsonContent(message);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json")
            {
                CharSet = "utf-8"
            };
            httpClient.BaseAddress = new Uri(pushServerUrl);
            httpClient.SetBearerToken(await RequestAccessToken());
            // httpClient.DefaultRequestHeaders.Accept.Clear();

            var response = await httpClient.PostAsync(pushMessageApi, httpContent);
            string resultString;
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<SyncOutput_ResultParam>(result);
                resultString = $"message: {data.Message}, result: {data.Result}";

                string spName = "usp_UpdNotification";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                SPInput_SYNC_UpdNotification spInput = new SPInput_SYNC_UpdNotification()
                {
                    isSend = 1,
                    PushTime = DateTime.Now.AddHours(8),
                    NotificationID = NewsID
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_SYNC_UpdNotification, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_SYNC_UpdNotification, SPOutput_Base>(connetStr);
               bool flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);

            }
            else
            {
                resultString = $"Http request error, status code: {response.StatusCode}";
            }

            Console.WriteLine("response:" + resultString);

            var endDate = DateTime.Now;
            Console.WriteLine("total time:" + (endDate - startDate).Seconds + "secs");
        }

        private static long ToUnixTime(DateTime dt)
        {
            return ((DateTimeOffset)dt).ToUnixTimeSeconds();

        }

        private static async Task<string> RequestAccessToken()
        {
            if (!JwtIsExpired(_accessToken)) return _accessToken;

            var client = new HttpClient();
            var ids4Address = "https://ssidentityserverprod.azurewebsites.net";
            var ids4ClientId = "sspushserverservice";
            var ids4ClientSecret = "8b3e2a36-5c82-4b00-93c9-b3af46065036";
            var ids4Scope = "sspush";

            var disco = await client.GetDiscoveryDocumentAsync(ids4Address);
            if (disco.IsError)
                // _logger.LogError(disco.Error);
                throw new Exception(disco.Error);

            // request access token
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = ids4ClientId,
                ClientSecret = ids4ClientSecret,
                Scope = ids4Scope
            });
            if (tokenResponse.IsError)
                // _logger.LogError(tokenResponse.Error);
                throw new Exception(tokenResponse.Error);

            var accessToken = tokenResponse.AccessToken;
            _accessToken = accessToken;
            return accessToken;
        }

        private class JsonContent : StringContent
        {
            public JsonContent(object obj) :
                base(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
            {
            }
        }

        private static bool JwtIsExpired(string jwt)
        {
            if (String.IsNullOrEmpty(jwt)) return true;
            var jwtSecrityToken = _tokenHandler.ReadJwtToken(jwt);
            return jwtSecrityToken.ValidTo <DateTime.Now.AddSeconds(30);
        }
    }
}
