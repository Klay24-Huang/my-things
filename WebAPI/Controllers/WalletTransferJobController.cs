using Domain.Common;
using Domain.SP.Input.Wallet;
using Domain.SP.Output;
using Domain.SP.Output.Wallet;
using Domain.WebAPI.Input.HiEasyRentAPI;
using Domain.WebAPI.Input.Taishin.Wallet;
using Domain.WebAPI.output.HiEasyRentAPI;
using Domain.WebAPI.output.Taishin.Wallet;
using Newtonsoft.Json;
using NLog;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
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
    /// 錢包介面轉檔排程
    /// </summary>
    public class WalletTransferJobController : ApiController
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        private CommonFunc BaseVerify { get; set; }

        [HttpPost]
        public Dictionary<string, object> DoWalletTransferJob()
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            var objOutput = new Dictionary<string, object>();    //輸出
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "WalletTransferJobController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            Token token = null;
            NullOutput apiOutput = new NullOutput();
            BaseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<WalletTransferData> wallets = null;
            WebAPIOutput_NPR420Save output = null;
            var trace = new TraceCom();
            var carRepo = new CarRentRepo();
            string sendMsg = "";
            #endregion

            #region 寫入API Log
            string ClientIP = BaseVerify.GetClientIp(Request);
            bool flag = BaseVerify.InsAPLog("NA", ClientIP, funName, ref errCode, ref LogID);
            #endregion
            #region TB
            if (flag)
            {
                //取得錢包介面轉檔清單
                wallets = GetWalletTransferList(funName, ref flag, ref lstError, ref errCode);
                trace.traceAdd("GetWalletList", new { flag, errCode, wallets.Count });
            }

            if (flag)
            {
                HiEasyRentAPI hiEasyRentAPI = new HiEasyRentAPI();
                WebAPIInput_NPR420Save WebAPIInput = new WebAPIInput_NPR420Save()
                {
                    NPR420SavePayments = wallets
                };

                flag = hiEasyRentAPI.NPR420Save(WebAPIInput, ref output);
                trace.traceAdd("NPR420Save", new { flag, output });

                if (!flag)
                {
                    sendMsg = output?.Message;
                }

                SPInput_WalletTransfer spInput = new SPInput_WalletTransfer()
                {
                    PRGName=funName,
                    F_TRFCOD = output?.Result ?? false ? "Y": "N"
                };

                flag = UpdateWalletTransfer(spInput, ref lstError, ref errCode);
                trace.traceAdd("UpdateWallet", new { flag, errCode });

                if (!flag)
                {
                    sendMsg += $" 更新資料庫失敗,錯誤代碼 : {errCode}";
                }


                if (!string.IsNullOrWhiteSpace(sendMsg)) //送irentService失敗 or 更新資料庫失敗
                {
                    List<CodeMailRecv> codeMails = new CommonRepository(connetStr).GetCodeMailRecv(funName);

                    if (codeMails !=null)
                    {
                        SendMail send = new SendMail();
                        string Receiver = string.Join(";", codeMails.Select(x => x.Mail.ToString()).ToArray());
                        string Title = $"iRent錢包帳務拋轉錯誤_轉檔日期{DateTime.Now.ToString("yyyyMMdd")}";
                        string Body = sendMsg;

                        try
                        {
                            send.DoSendMail(Title, Body, Receiver);
                        }
                        catch (Exception ex)
                        {
                            trace.BaseMsg = ex.Message;
                        }
                    }                  
                }
            }

            carRepo.AddTraceLog(283, funName, trace, flag);

            #endregion
            #region 寫入錯誤Log
            if (flag == false && isWriteError == false)
            {
                BaseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            BaseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }


        /// 取得錢包拋轉介面清單
        private List<WalletTransferData> GetWalletTransferList(string funName, ref bool flag, ref List<ErrorInfo> lstError, ref string errCode)
        {
            var wallets = new List<WalletTransferData>();

            SPInput_GetWalletReturn spInput = new SPInput_GetWalletReturn()
            {
                PRGName = funName
            };

            string SPName = "usp_WalletTransfer_Q01";
            SPOutput_Base spOutBase = new SPOutput_Base();
            SQLHelper<SPInput_GetWalletReturn, SPOutput_Base> sqlHelpQuery = new SQLHelper<SPInput_GetWalletReturn, SPOutput_Base>(connetStr);
            DataSet ds = new DataSet();
            flag = sqlHelpQuery.ExeuteSP(SPName, spInput, ref spOutBase, ref wallets, ref ds, ref lstError);
            BaseVerify.checkSQLResult(ref flag, ref spOutBase, ref lstError, ref errCode);
            
            if (flag)
            {
                if (wallets.Count == 0)
                {
                    flag = false;
                    errCode = "ERR203"; //錢包找不到符合資料轉檔
                }
            }
            return wallets;
        }

        /// 更新錢包拋轉結果
        private bool UpdateWalletTransfer(SPInput_WalletTransfer input, ref List<ErrorInfo> lstError, ref string errCode)
        {
            string SPName = "usp_WalletTransfer_U01";
            SPOutput_Base spOut = new SPOutput_Base();
            SQLHelper<SPInput_WalletTransfer, SPOutput_Base> SQLPayHelp = new SQLHelper<SPInput_WalletTransfer, SPOutput_Base>(connetStr);
            var flag = SQLPayHelp.ExecuteSPNonQuery(SPName, input, ref spOut, ref lstError);
            BaseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
            return flag;
        }

    }
}