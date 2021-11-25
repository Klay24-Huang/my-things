using Domain.WebAPI.Input.Hotai.Member;
using Domain.WebAPI.output.Hotai.Member;
using OtherService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotaiPayWebView.Repository
{
    public class LoginRepository
    {
        
        public bool DoLogin(string phone,string pwd,ref WebAPIOutput_Signin apioutput,ref string errCode)
        {
            bool flag = false;
            WebAPIInput_Signin apiInput = new WebAPIInput_Signin
            {
                account = phone,
                password = pwd
            };

            apioutput = new WebAPIOutput_Signin();

            HotaiMemberAPI hotaiAPI = new HotaiMemberAPI();
            flag = hotaiAPI.DoSignin(apiInput,ref apioutput,ref errCode);

            return flag;
        }
    }
}