using Domain.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    public class GetMemberScoreItemController : ApiController
    {
        private string connectStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        [HttpGet]
        public Dictionary<string, object> DoGetMemberScoreItem()
        {
            var objOutput = new Dictionary<string, object>();   //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success";  //預設成功
            string errCode = "000000";  //預設成功
            string funName = "GetMemberScoreItemController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            OAPI_GetMemberScoreItem outputApi = new OAPI_GetMemberScoreItem();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string Contentjson = "";

            return objOutput;
        }
    }
}