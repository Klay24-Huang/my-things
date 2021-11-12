using Domain.Common;
using Domain.SP.Input.Wallet;
using Domain.SP.Output.Wallet;
using Domain.WebAPI.Input.Taishin.Wallet;
using Domain.WebAPI.output.Taishin.Wallet;
using Newtonsoft.Json;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebAPI.Service;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 錢包轉贈
    /// </summary>
    /// 2021-09-28 UPD BY YANKEY 調整日期輸出格式
    /// 2021-11/08 UPD BY YANKEY 調整輸出格式-不輸出data
    public class WalletTransferStoredValueController : ApiController
    {
        private string connetStr    = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string APIToken     = ConfigurationManager.AppSettings["TaishinWalletAPIToken"].ToString();
        private string APIKey       = ConfigurationManager.AppSettings["TaishinWalletAPIKey"].ToString();
        private string MerchantId   = ConfigurationManager.AppSettings["TaishiWalletMerchantId"].ToString();
        private string BaseURL      = ConfigurationManager.AppSettings["TaishinWalletBaseURL"].ToString();

        [HttpPost]
        public Dictionary<string, object> DoWalletPayTransaction(Dictionary<string, object> value)
        {
            #region 初始宣告
            var wsp = new WalletSp();
            var wMap = new WalletMap(); 
            var cr_com = new CarRentCommon();
            var trace = new TraceCom();
            var carRepo = new CarRentRepo();

            HttpContext httpContext = HttpContext.Current;
            var objOutput = new Dictionary<string, object>();    //輸出
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "WalletTransferStoredValueController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_WalletTransferStoredValue apiInput = null;
            var apiOutput = new OAPI_WalletTransferStoredValue();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";

            string IDNO_To = "";//受贈人身分證號
            string PhoneNo_To = "";//受贈人手機號碼        

            #endregion

            trace.traceAdd("apiIn", value);

            try
            {
                #region 防呆

                //flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest, false);
                flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

                if (flag)
                {
                    //寫入API Log
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_WalletTransferStoredValue>(Contentjson);
                    string ClientIP = baseVerify.GetClientIp(Request);
                    flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                    if (apiInput.Amount <= 0)
                    {
                        flag = false;
                        errCode = "ERR900";
                        errMsg = "轉贈金額不可為0";
                    }
                    if (flag)
                    {

                        if (apiInput == null || string.IsNullOrWhiteSpace(apiInput.IDNO))
                        {
                            flag = false;
                            errMsg = "參數遺漏";
                            errCode = "ERR257";//參數遺漏
                        }
                        else
                        {
                            if (Int32.TryParse(apiInput.IDNO, out int intPhoneNo))
                                PhoneNo_To = intPhoneNo.ToString();
                            else
                            {
                                flag = baseVerify.checkIDNO(apiInput.IDNO);
                                if (flag)
                                    IDNO_To = apiInput.IDNO;
                                else
                                {
                                    errMsg = "身分證格式錯誤";
                                    errCode = "ERR103";
                                }
                            }
                        }
                    }
                }
                //不開放訪客
                if (isGuest)
                {
                    flag = false;
                    errCode = "ERR101";
                }
                #endregion

                #region TB
                //Token判斷
                if (flag && isGuest == false)
                {
                    flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
                }
                #endregion

                #region 轉贈前確認

                var CkFrom  = new SPOut_WalletTransferCheck(); //贈與人
                var CkTo    = new SPOut_WalletTransferCheck(); //受贈人
                if (flag)
                {
                    string sp1ErrCode = "", sp2ErrCode = "";

                    #region 贈與人檢核

                    var sp1In = new SPInput_WalletTransferCheck()
                    {
                        IDNO = IDNO,
                        LogID = LogID,
                    };
                    var sp1_list = wsp.sp_WalletTransferCheck(sp1In, ref sp1ErrCode);
                    if (sp1_list != null && sp1_list.Count() > 0)
                        CkFrom = sp1_list.FirstOrDefault();
                    else
                    {
                        flag = false;
                        errMsg = "贈與人ID查無會員資料";
                        errCode = "ERR915";//查無對應之會員
                        CkFrom = null;
                    }

                    if (flag)
                    {
                        if (sp1ErrCode != "0000")
                        {
                            flag = false;
                            errMsg = "spError";
                            errCode = sp1ErrCode;
                        }
                    }

                    trace.traceAdd("sp1_lnfo", new { sp1In, sp1_list, sp1ErrCode });
                    trace.FlowList.Add("贈與人檢核");

                    #endregion

                    #region 受贈人檢核

                    if (flag)
                    {
                        var sp2In = new SPInput_WalletTransferCheck()
                        {
                            IDNO = IDNO_To,//受贈人
                            PhoneNo = PhoneNo_To,//受贈人
                            LogID = LogID,
                        };
                        var sp2_list = wsp.sp_WalletTransferCheck(sp2In, ref sp2ErrCode);
                        if (sp2_list != null && sp2_list.Count() > 0)
                        {
                            CkTo = sp2_list.FirstOrDefault();
                            IDNO_To = CkTo.IDNO;
                        }
                        else
                        {
                            flag = false;
                            errMsg = "受贈人不存在";
                            errCode = "ERR915";//資料不存在
                            CkTo = null;
                        }

                        if (flag)
                        {
                            if (sp2ErrCode != "0000")
                            {
                                flag = false;
                                errMsg = "spError";
                                errCode = sp2ErrCode;
                            }
                        }

                        trace.traceAdd("sp2_lnfo", new { sp2In, sp2_list, sp2ErrCode });
                        trace.FlowList.Add("受贈人檢核");
                    }

                    #endregion

                    #region 商業邏輯檢查
                    if (flag)
                    {
                        if (apiInput.Amount > CkFrom.WalletAmount)
                        {
                            flag = false;
                            errMsg = "錢包餘額不足";
                            errCode = "ERR281";
                        }

                        if (flag)
                        {
                            if ((apiInput.Amount + CkTo.WalletAmount) > 50000)
                            {
                                flag = false;
                                errMsg = "轉贈後對方餘額超過上限";
                                errCode = "ERR282";
                            }
                        }

                        if (flag)
                        {
                            if ((apiInput.Amount + CkTo.MonTransIn) > 300000)
                            {
                                flag = false;
                                errMsg = "轉贈後對方金流超過當月上限";
                                errCode = "ERR280";
                            }
                        }                        
                        trace.FlowList.Add("商業邏輯檢查");
                    }
                    #endregion
                }

                #endregion

                #region 取個人資料
                if (flag)
                {
                    string SPName = new ObjType().GetSPName(ObjType.SPType.GetWalletInfo);
                    SPInput_GetWalletInfo SPInput = new SPInput_GetWalletInfo()
                    {
                        IDNO = IDNO,
                        LogID = LogID,
                        Token = Access_Token
                    };
                    SPOutput_GetWalletInfo SPOutput = new SPOutput_GetWalletInfo();//取得會員錢包帳號
                    SQLHelper<SPInput_GetWalletInfo, SPOutput_GetWalletInfo> sqlHelp = new SQLHelper<SPInput_GetWalletInfo, SPOutput_GetWalletInfo>(connetStr);
                    flag = sqlHelp.ExecuteSPNonQuery(SPName, SPInput, ref SPOutput, ref lstError);
                    baseVerify.checkSQLResult(ref flag, SPOutput.Error, SPOutput.ErrorCode, ref lstError, ref errCode);

                    trace.FlowList.Add("贈與人錢包");
                    trace.traceAdd("GetWalletInfo",new {SPInput, SPOutput });

                    SPName = new ObjType().GetSPName(ObjType.SPType.GetWalletInfoByTrans);
                    SPInput_GetWalletInfo SPTransInput = new SPInput_GetWalletInfo()
                    {
                        IDNO = IDNO_To,
                        LogID = LogID,
                        Token = Access_Token
                    };
                    SPOutput_GetWalletInfo SPTransOutput = new SPOutput_GetWalletInfo();
                    sqlHelp = new SQLHelper<SPInput_GetWalletInfo, SPOutput_GetWalletInfo>(connetStr);
                    flag = sqlHelp.ExecuteSPNonQuery(SPName, SPTransInput, ref SPTransOutput, ref lstError);
                    baseVerify.checkSQLResult(ref flag, SPTransOutput.Error, SPTransOutput.ErrorCode, ref lstError, ref errCode);
                    if (SPTransOutput.Name == "" || SPTransOutput.PhoneNo == "" || SPTransOutput.Email == "")
                    {
                        flag = false;
                        errCode = "ERR201";
                    }

                    trace.FlowList.Add("受贈人錢包");
                    trace.traceAdd("GetWalletInfoByTrans", new { SPTransInput, SPTransOutput });

                    #region 台新錢包轉贈

                    WebAPIOutput_TransferStoreValueCreateAccount output = null;
                    if (flag)
                    {
                        DateTime NowTime = DateTime.Now;
                        string guid = Guid.NewGuid().ToString().Replace("-", "");
                        int nowCount = 1;
                        WebAPI_TransferStoredValueCreateAccount wallet = new WebAPI_TransferStoredValueCreateAccount()
                        {
                            AccountId = SPOutput.WalletAccountID,
                            ApiVersion = "0.1.01",
                            GUID = guid,
                            MerchantId = MerchantId,
                            POSId = "",
                            SourceFrom = "2",
                            StoreId = "",
                            StoreName = "",
                            StoreTransId = string.Format("{0}S{1}", IDNO.Substring(0, 9), NowTime.ToString("MMddHHmmss")),
                            Amount = apiInput.Amount,
                            BarCode = "",
                            StoreTransDate = NowTime.ToString("yyyyMMddHHmmss"),
                            Email = SPOutput.Email,
                            Name = SPOutput.Name,
                            ID = IDNO,
                            PhoneNo = SPOutput.PhoneNo,
                            TransMemo = string.Format("由{0}轉贈", SPOutput.Name),
                            AccountData = new List<Domain.WebAPI.Input.Taishin.Wallet.Param.AccountData>()
                        };
                        Domain.WebAPI.Input.Taishin.Wallet.Param.AccountData obj = new Domain.WebAPI.Input.Taishin.Wallet.Param.AccountData()
                        {
                            TransferAccountId = string.Format("{0}Wallet{1}", IDNO_To, nowCount.ToString().PadLeft(4, '0')),
                            TransferEmail = SPTransOutput.Email,
                            TransferID = IDNO_To,
                            TransferName = SPTransOutput.Name,
                            TransferPhoneNo = SPTransOutput.PhoneNo
                        };
                        wallet.AccountData.Add(obj);
                        var body = JsonConvert.SerializeObject(wallet);
                        TaishinWallet WalletAPI = new TaishinWallet();
                        string utcTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
                        string SignCode = WalletAPI.GenerateSignCode(wallet.MerchantId, utcTimeStamp, body, APIKey);
                        //WebAPIOutput_TransferStoreValueCreateAccount output = null;
                        flag = WalletAPI.DoTransferStoreValueCreateAccount(wallet, MerchantId, utcTimeStamp, SignCode, ref errCode, ref output);
                        if (flag == false)
                        {
                            errCode = "ERR";
                            errMsg = output.Message;
                        }

                        trace.traceAdd("TaishinOut", output);
                        trace.FlowList.Add("台新錢包");
                    }
                    #endregion

                    #region 寫入錢包紀錄

                    if (flag)
                    {
                        if (output != null && output.Result != null
                            && !string.IsNullOrWhiteSpace(output.Result.TransId)
                            && !string.IsNullOrWhiteSpace(output.Result.StoreTransId)
                            && output.Result.ActualAmount > 0)
                        {
                            trace.FlowList.Add("寫入錢包紀錄");

                            var ret = output.Result;

                            string spErrCode = "";
                            var spIn = new SPInput_SetWalletTrade()
                            {
                                IDNO = IDNO,
                                LogID = LogID,
                                StoreTransId = ret.StoreTransId,
                                TransId = ret.TransId,
                                IDNO_To = IDNO_To,
                                TradeAMT = ret.ActualAmount
                            };
                            flag = wsp.sp_SetWalletTrade(spIn, ref spErrCode);
                            if (!flag)
                                errCode = spErrCode;

                            trace.traceAdd("SetWalletTrade", new {spIn, spErrCode });
                        }
                    }

                    #endregion
                }
                #endregion
                //2021-09-28 UPD BY YANKEY 調整日期輸出格式
                //apiOutput.SystemTime = DateTime.Now.ToString("G", CultureInfo.CreateSpecificCulture("zh-CN"));    //Display:"2021/9/28 15:36:52"
                //apiOutput.SystemTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");                                                  //Display:"2021-09-28T15:41:03"
                //apiOutput.TranResult = flag ? 1 : 0;
            }
            catch (Exception ex)
            {
                flag = false;
                //apiOutput.TranResult = 0;
                trace.BaseMsg = ex.Message;
            }

            trace.traceAdd("TraceFinal", new { errCode, errMsg });
            carRepo.AddTraceLog(79, funName, trace, flag);

            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }
    }
}
