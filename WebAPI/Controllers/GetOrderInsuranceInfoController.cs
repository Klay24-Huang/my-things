using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Input.JointRent;
using Domain.SP.Output;
using Domain.SP.Output.Common;
using Domain.SP.Output.JointRent;
using OtherService.Enum;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    public class GetOrderInsuranceInfoController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> DoGetOrderInsuranceInfo(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            //string[] headers=httpContext.Request.Headers.AllKeys;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "GetOrderInsuranceInfoController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_GetOrderInsuranceInfo apiInput = new IAPI_GetOrderInsuranceInfo();
            OAPI_GetOrderInsuranceInfo outputApi = new OAPI_GetOrderInsuranceInfo();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            bool HasInput = true;
            #endregion

            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest, HasInput);

            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_GetOrderInsuranceInfo>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
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
            //檢查訂單編號格式
            var checkOrderNo = OrderNoFormatVerify(apiInput.OrderNo);
            flag = checkOrderNo.status;
            errCode = checkOrderNo.errorCode;
            #endregion

            #region TB
            //Token判斷
            if (flag)
            {
                string SPName = new ObjType().GetSPName(ObjType.SPType.GetOrderInsuranceInfo);
                SPInput_GetOrderInsuranceInfo spInput = new SPInput_GetOrderInsuranceInfo()
                {
                    OrderNo = checkOrderNo.orderNo.ToString()
                };
                SPOutput_GetOrderInsuranceInfo spOut = new SPOutput_GetOrderInsuranceInfo();
                SQLHelper<SPInput_GetOrderInsuranceInfo, SPOutput_GetOrderInsuranceInfo> sqlHelp = new SQLHelper<SPInput_GetOrderInsuranceInfo, SPOutput_GetOrderInsuranceInfo>(connetStr);
                //flag = sqlHelp.ExecuteSPNonQuery(SPName, spInput, ref spOut, ref lstError);
                try
                {
                    using (SqlConnection conn = new SqlConnection(connetStr))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(SPName, conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@OrderNo", SqlDbType.VarChar).Value = checkOrderNo.orderNo.ToString();
                            var reader = cmd.ExecuteReader();

                            if (reader.HasRows)
                            {
                                errCode = "000000";
                                while (reader.Read())
                                {
                                    outputApi.Insurance = (int)reader[0];
                                    outputApi.MainInsurancePerHour = (int)reader[1];
                                    outputApi.JointInsurancePerHour = (int)reader[2];
                                    outputApi.JointAlertMessage = reader[3].ToString();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    errCode = "ERR";
                    errMsg = ex.Message.ToString();
                    outputApi = new OAPI_GetOrderInsuranceInfo();
                    baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
                }
            }
            #endregion
            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion
        }

        private (bool status, string errorCode, Int64 orderNo) OrderNoFormatVerify(string OrderNo)
        {
            (bool status, string errorCode, Int64 orderNo) result = (true, "", 0);

            if (string.IsNullOrWhiteSpace(OrderNo))
            {
                result.status = false;
                result.errorCode = "ERR900";
            }
            else
            {
                if (OrderNo.IndexOf("H") < 0)
                {
                    result.status = false;
                    result.errorCode = "ERR900";
                }
                if (result.status)
                {
                    if (Int64.TryParse(OrderNo.Replace("H", ""), out Int64 tmpOrder))
                    {
                        if (tmpOrder <= 0)
                        {
                            result.status = false;
                            result.errorCode = "ERR900";
                        }
                        else
                        {
                            result.orderNo = tmpOrder;
                        }
                    }
                }
            }
            return result;
        }
    }
}

