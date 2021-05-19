using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Output.Common;
using Domain.SP.Output.Member;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Utils;
using WebCommon;

namespace WebAPI.Controllers
{
    public class GetMemberScoreController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        [HttpPost]
        public Dictionary<string, object> DoGetMemberScore(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"];    //Bearer 
            var objOutput = new Dictionary<string, object>();   //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success";  //預設成功
            string errCode = "000000";  //預設成功
            string funName = "GetMemberScoreController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_GetMemberScore apiInput = null;
            OAPI_GetMemberScore outputApi = new OAPI_GetMemberScore();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            int NowPage = 0;
            int pageSize = 10;
            #endregion

            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                apiInput = JsonConvert.DeserializeObject<IAPI_GetMemberScore>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                if (apiInput.NowPage.HasValue)
                {
                    if (apiInput.NowPage.Value < 0)
                    {
                        NowPage = 1;
                    }
                    else
                    {
                        NowPage = apiInput.NowPage.Value;
                    }
                }
                else
                {
                    NowPage = 1;
                }
            }
            //不開放訪客
            if (flag)
            {
                if (isGuest)
                {
                    flag = false;
                    errCode = "ERR101";
                }
            }
            #endregion

            #region TB
            //Token判斷
            if (flag && isGuest == false)
            {
                string CheckTokenName = new ObjType().GetSPName(ObjType.SPType.CheckTokenReturnID);
                SPInput_CheckTokenOnlyToken spCheckTokenInput = new SPInput_CheckTokenOnlyToken()
                {
                    LogID = LogID,
                    Token = Access_Token
                };
                SPOutput_CheckTokenReturnID spOut = new SPOutput_CheckTokenReturnID();
                SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID> sqlHelp = new SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(CheckTokenName, spCheckTokenInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    IDNO = spOut.IDNO;
                }
            }

            if (flag)
            {
                string spName = new ObjType().GetSPName(ObjType.SPType.GetMemberScore);

                object[][] parms1 = {
                    new object[] {
                        IDNO,
                        NowPage,
                        pageSize,
                        LogID
                }};

                DataSet ds1 = null;
                string returnMessage = "";
                string messageLevel = "";
                string messageType = "";

                ds1 = WebApiClient.SPExeBatchMultiArr2(ServerInfo.GetServerInfo(), spName, parms1, true, ref returnMessage, ref messageLevel, ref messageType);

                if (ds1.Tables.Count == 0)
                {
                    flag = false;
                    errCode = "ERR999";
                    errMsg = returnMessage;
                }
                else if (ds1.Tables.Count == 3)
                {
                    baseVerify.checkSQLResult(ref flag, Convert.ToInt32(ds1.Tables[2].Rows[0]["Error"]), ds1.Tables[2].Rows[0]["ErrorCode"].ToString(), ref lstError, ref errCode);

                    if (flag)
                    {
                        outputApi.Score = Convert.ToInt32(ds1.Tables[0].Rows[0]["SCORE"]);
                        outputApi.DetailList = new List<MemberScoreList>();

                        DataTable dt = ds1.Tables[1];

                        if (dt.Rows.Count > 0)
                        {
                            outputApi.TotalPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Convert.ToInt32(dt.Rows[0]["TotalCount"]) / pageSize))) + ((Convert.ToInt32(dt.Rows[0]["TotalCount"]) % pageSize > 0) ? 1 : 0);

                            foreach (DataRow dr in dt.Rows)
                            {
                                var TmpList = new MemberScoreList
                                {
                                    TotalCount = Convert.ToInt32(dr["TotalCount"]),
                                    RowNo = Convert.ToInt32(dr["RowNo"]),
                                    GetDate = Convert.ToDateTime(dr["GetDate"]),
                                    SEQ = Convert.ToInt32(dr["SEQ"]),
                                    SCORE = Convert.ToInt32(dr["SCORE"]),
                                    UIDESC = dr["UIDESC"].ToString()
                                };

                                outputApi.DetailList.Add(TmpList);
                            }
                        }
                    }
                }
            }
            #endregion

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
