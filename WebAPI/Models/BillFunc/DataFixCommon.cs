using Domain.SP.Input.Bill;
using Domain.SP.Input.Rent;
using Domain.SP.Output;
using Domain.SP.Output.Bill;
using Domain.SP.Output.Rent;
using OtherService.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using WebAPI.Utils;
using WebCommon;
using System.Configuration;
using WebAPI.Models.BaseFunc;
using Domain.WebAPI.output.Taishin;
using OtherService;
using Domain.WebAPI.Input.Taishin.GenerateCheckSum;
using Domain.WebAPI.Input.Taishin;
using System.Threading;
using WebAPI.Models.Param.Output.PartOfParam;
using WebAPI.Models.Param.Output;
using Domain.TB;
using Reposotory.Implement;
using Domain.WebAPI.Input.Taishin.Wallet;
using Newtonsoft.Json;
using Domain.WebAPI.output.Taishin.Wallet;
using Domain.MemberData;
using Domain.SP.Input.Member;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.IO;
using Domain.SP.Input.OtherService.Common;
using OtherService.Common;

namespace WebAPI.Models.BillFunc
{
    public class DataFixCommon
    {
        #region ReDoGetPayDetail呼叫

        public async Task<Tuple<bool, OAPI_ReDoGetPayDetail>> CallReDoGetPayDetail(IAPI_ReDoGetPayDetail input)
        {
            bool flag = false;
            var output = new OAPI_ReDoGetPayDetail();
            var apiRe = await xCallReDoGetPayDetail(input);
            if (apiRe == null)
            {
                output.Result = false;
                output.Message = "Api呼叫失敗";
                output.ApiResult.Msg = "Api呼叫失敗";
            }
            else
            {
                output = apiRe;
                flag = output.Result;
            }

            return new Tuple<bool, OAPI_ReDoGetPayDetail>(flag,output);
        }

        public async Task<OAPI_ReDoGetPayDetail> xCallReDoGetPayDetail(IAPI_ReDoGetPayDetail input)
        {
            OAPI_ReDoGetPayDetail output = null;
            string responseStr = "";
            string callUrl = String.Format("http://{0}/api/ReDoGetPayDetail", "localhost:2061");//hack: fix: 修正CallReDoGetPayDetail的url
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(callUrl);
            request.Method = "POST";
            request.ContentType = "application/json";
            try
            {
                string postBody = JsonConvert.SerializeObject(input);//將匿名物件序列化為json字串
                byte[] byteArray = Encoding.UTF8.GetBytes(postBody);//要發送的字串轉為byte[]

                using (Stream reqStream = await request.GetRequestStreamAsync())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);
                }

                //發出Request
                
                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseStr = await reader.ReadToEndAsync();
                        RTime = DateTime.Now;
                        output = JsonConvert.DeserializeObject<OAPI_ReDoGetPayDetail>(responseStr);
                    }
                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new OAPI_ReDoGetPayDetail()
                {
                    Message = responseStr,
                    Result = false
                };
            }
            finally
            {
                SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                {
                    MKTime = MKTime,
                    UPDTime = RTime,
                    WebAPIInput = JsonConvert.SerializeObject(input),
                    WebAPIName = "ETAG010Query",
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = callUrl
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }

            return output;
        }

        #endregion
    }

    public class DataFixSp 
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        public bool sp_CalFinalPrice_ForDataFix(SPInput_CalFinalPrice_ForDataFix spInput, ref string errCode)
        {
            bool flag = false;
            string spName = "usp_CalFinalPrice_ForDataFix";

            var lstError = new List<ErrorInfo>();
            var spOutBase = new SPOutput_Base();
            var spOut = new SPOut_CalFinalPrice_ForDataFix();
            SQLHelper<SPInput_CalFinalPrice_ForDataFix, SPOut_CalFinalPrice_ForDataFix> sqlHelp = new SQLHelper<SPInput_CalFinalPrice_ForDataFix, SPOut_CalFinalPrice_ForDataFix>(connetStr);
            bool spFlag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);

            if (spFlag && spOut != null)
            {
                if (spOut.ErrorCode != "0000")
                    errCode = spOut.ErrorCode;
                flag = spOut.xError == 0;
            }

            return flag;
        }

        /// <summary>
        /// 由錯誤的專案代號取得正確的專案資訊
        /// </summary>
        /// <param name="spInput"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public SPOut_GetDataFix sp_GetDataFix_single(SPInput_GetDataFix spInput, ref string errCode)
        {
            var sp_re = sp_GetDataFix(spInput, ref errCode);
            if (sp_re != null && sp_re.Count() > 0)
                return sp_re.FirstOrDefault();
            else
                return null;
        }

        /// <summary>
        /// 由錯誤的專案代號取得正確的專案資訊
        /// </summary>
        /// <param name="spInput"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public List<SPOut_GetDataFix> sp_GetDataFix(SPInput_GetDataFix spInput, ref string errCode)
        {
            var re = new List<SPOut_GetDataFix>();

            try
            {
                string SPName = "usp_GetDataFix_Q1";
                object[][] parms1 = {
                    new object[] {
                    },
                };

                DataSet ds1 = null;
                string returnMessage = "";
                string messageLevel = "";
                string messageType = "";

                ds1 = WebApiClient.SPExeBatchMultiArr2(ServerInfo.GetServerInfo(), SPName, parms1, true, ref returnMessage, ref messageLevel, ref messageType);

                if (string.IsNullOrWhiteSpace(returnMessage) && ds1 != null && ds1.Tables.Count >= 0)
                {
                    if (ds1.Tables.Count >= 2)
                        re = objUti.ConvertToList<SPOut_GetDataFix>(ds1.Tables[0]);
                    else if (ds1.Tables.Count == 1)
                    {
                        var re_db = objUti.GetFirstRow<SPOutput_Base>(ds1.Tables[0]);
                        if (re_db != null && re_db.Error != 0 && !string.IsNullOrWhiteSpace(re_db.ErrorMsg))
                            errCode = re_db.ErrorMsg;
                    }
                }

                return re;
            }
            catch (Exception ex)
            {
                errCode = ex.ToString();
                throw ex;
            }
        }

    }

    #region SpVM

    public class SPInput_CalFinalPrice_ForDataFix
    {
        public string IDNO { get; set; }
        public Int64 OrderNo { get; set; }
        public int final_price { get; set; }
        public int pure_price { get; set; }
        public int mileage_price { get; set; }
        public int Insurance_price { get; set; }
        public int fine_price { get; set; }
        public int gift_point { get; set; }
        public int gift_motor_point { get; set; }
        public double monthly_workday { get; set; }
        public double monthly_holiday { get; set; }
        public int Etag { get; set; }
        public int parkingFee { get; set; }
        public int TransDiscount { get; set; }
    }

    public class SPOut_CalFinalPrice_ForDataFix : SPOutput_Base
    {
        public int xError { get; set; }
    }

    public class SPInput_GetDataFix 
    {

    }

    public class SPOut_GetDataFix 
    {
        public string IDNO { get; set; }
        public Int64 OrderNo { get; set; }
        public int Discount { get; set; }
        public int MotorDiscount { get; set; }

        /// <summary>
        /// 正確的專案ID
        /// </summary>
        public string PROJID { get; set; }
        /// <summary>
        /// 正確的專案類型
        /// </summary>
        public int PROJTYPE { get; set; }
        /// <summary>
        /// 正確的平日優惠價
        /// </summary>
        public double PRICE { get; set; }
        /// <summary>
        /// 正確的假日優惠價
        /// </summary>
        public double PRICE_H { get; set; }
        /// <summary>
        /// 修正後每分鐘多少-機車
        /// </summary>
        public float MinuteOfPrice { get; set;}
    }

    #endregion

    #region ApiVM

    public class IAPI_ReDoGetPayDetail
    {
        public string IDNO { get; set; }
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { set; get; }
        /// <summary>
        /// 汽車使用的點數
        /// </summary>
        public int Discount { set; get; }

        /// <summary>
        /// 機車使用的點數
        /// </summary>
        public int MotorDiscount { set; get; }

        #region 更正錯誤資料

        /// <summary>
        /// 修正後的專案ID
        /// </summary>
        public string PROJID { get; set; }
        /// <summary>
        /// 修正後的專案代碼
        /// </summary>
        public int PROJTYPE { get; set; }
        /// <summary>
        /// 修正後的汽車專案平日優惠價
        /// </summary>
        public int PRICE { get; set; }
        /// <summary>
        /// 修正後的汽車假日專案優惠價
        /// </summary>
        public int PRICE_H { get; set; }
        /// <summary>
        /// 修正後每分鐘多少-機車
        /// </summary>
        public float MinuteOfPrice { get; set; }

        #endregion
    }

    public class OAPI_ReDoGetPayDetail: OAPI_GetPayDetail
    {
        public OAPI_ApiCallResult ApiResult { get; set; }
        public string Message { get; set; }
        public bool Result { get; set; }
    }

    public class OAPI_ApiCallResult 
    {       
        public string ExMsg { get; set; }
        public string Msg { get; set; }
        public string ErrMsg { get; set; }
        public string ErrCode { get; set; }
    }

    public class IAPI_MultiCallApi 
    {
    
    }

    public class OAPI_MultiCallApi 
    {
       public List<ApiErrMsg> ApiErrList { get; set; }
    }

    public class ApiErrMsg
    { 
       public string OrderNo { get; set; }
       public string InJson { get; set; }
       public string OutJson { get; set; }
    }

    #endregion
}