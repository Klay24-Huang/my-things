using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SSAPI.Service.Services.Interface;
using SSAPI.Service.Services;


/// <summary>
/// WebApiClientBase 的摘要描述
/// </summary>
namespace WebAPI.Utils
{
    public class WebApiClientBase
    {

        protected static ISSApiSp GetSSApiSp()
        {
            return SSApiSpFactory.GetSSApiSp();
        }
        protected static ISSApiSp GetSSApiSp(string apiName)
        {
            SSApiSpFactory.SetApiName(apiName);
            return SSApiSpFactory.GetSSApiSp();
        }
    }
}