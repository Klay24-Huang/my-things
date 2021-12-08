using Domain.WebAPI.Input.Hotai.Member;
using Domain.WebAPI.output.Hotai.Member;
using OtherService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotaiPayWebView.Services
{
    public class BaseService
    {
        HotaiMemberAPI hotaiApi = new HotaiMemberAPI();

        public void RefreshToken(ref string accessToken, ref string refreshToken)
        {
            bool flag = false;
            WebAPIInput_RefreshToken tokenInput = new WebAPIInput_RefreshToken
            {
                access_token = accessToken,
                refresh_token = refreshToken
            };
            WebAPIOutput_Token tokenOutput = new WebAPIOutput_Token();
            string errCode = "";
            int a = 200;
            flag = hotaiApi.DoRefreshToken(tokenInput, ref tokenOutput, ref errCode,ref a);

            if (flag)
            {
                accessToken = tokenOutput.access_token;
                refreshToken = tokenOutput.refresh_token;
            }
        }
    }
}