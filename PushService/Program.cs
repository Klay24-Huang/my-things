using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json;

namespace PushService
{
    class Program
    {
        private static string _accessToken = "";
        private static JwtSecurityTokenHandler _tokenHandler;

        static void Main(string[] args)
        {
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
