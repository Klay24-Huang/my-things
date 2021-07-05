using Domain.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Output;
using WebCommon;
using Domain.SP.BE.Output;
using Domain.SP.BE.Input;
using System.Data;
using Domain.TB.BackEnd;

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

            string SPName = new ObjType().GetSPName(ObjType.SPType.GetMemberScoreItem);
            SPOutput_BE_GetMemberScoreItem spOut = new SPOutput_BE_GetMemberScoreItem();
            SPInput_BE_GetOrderInfoBeforeModify spInput = new SPInput_BE_GetOrderInfoBeforeModify();
            SQLHelper<SPInput_BE_GetOrderInfoBeforeModify, SPOutput_BE_GetMemberScoreItem> sqlHelp = new SQLHelper<SPInput_BE_GetOrderInfoBeforeModify, SPOutput_BE_GetMemberScoreItem>(connectStr);
            List<BE_GetMemberScoreItem> OrderDataLists = new List<BE_GetMemberScoreItem>();
            DataSet ds = new DataSet();
            flag = sqlHelp.ExeuteSP(SPName, spInput, ref spOut, ref OrderDataLists, ref ds, ref lstError);
            new CommonFunc().checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
            if (flag)
            {
                outputApi.datalst = OrderDataLists.ToList();
            }

            #region 寫入錯誤Log
            if (flag == false && isWriteError == false)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion
        }
    }
}