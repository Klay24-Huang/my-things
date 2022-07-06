using Domain.SP.Input.Bill;
using Domain.SP.Input.Booking;
using Domain.SP.Input.Notification;
using Domain.SP.Input.Rent;
using Domain.SP.Output;
using Domain.SP.Output.Bill;
using Domain.SP.Output.OrderList;
using Domain.TB;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Utils;
using WebCommon;
namespace WebAPI.Service
{
    /// <summary>
    /// 共用Service
    /// </summary>
    public class CommonService
    {
        CommonFunc baseVerify = new CommonFunc();
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private float Mildef = (ConfigurationManager.AppSettings["Mildef"] == null) ? 3 : Convert.ToSingle(ConfigurationManager.AppSettings["Mildef"].ToString());

        #region 取得安心服務每小時價格
        /// <summary>
        /// 取得安心服務每小時價格
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="IDNO">會員編號</param>
        /// <param name="CarTypeCode">車型代碼</param>
        /// <param name="LogID"></param>
        /// <param name="errCode">錯誤代碼</param>
        /// <returns></returns>
        public (bool checkFlag, int InsurancePerHours) GetInsurancePrice(string IDNO, string CarTypeCode, long LogID, ref string errCode)
        {
            (bool checkFlag, int InsurancePerHours) result = (false, 0);
            int insurancePerHours = 0;
            string spName = "usp_GetInsurancePrice";
            //20201103 ADD BY ADAM REASON.取得安心服務每小時價格
            SPInput_GetInsurancePrice spGetInsurancePrice = new SPInput_GetInsurancePrice()
            {
                IDNO = IDNO,
                CarType = CarTypeCode,
                LogID = LogID
            };
            List<SPOutput_GetInsurancePrice> re = new List<SPOutput_GetInsurancePrice>();
            SPOutput_Base spOutput_Base = new SPOutput_Base();
            SQLHelper<SPInput_GetInsurancePrice, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_GetInsurancePrice, SPOutput_Base>(connetStr);
            DataSet ds = new DataSet();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            bool flag = sqlHelp.ExeuteSP(spName, spGetInsurancePrice, ref spOutput_Base, ref re, ref ds, ref lstError);
            baseVerify.checkSQLResult(ref flag, spOutput_Base.Error, spOutput_Base.ErrorCode, ref lstError, ref errCode);
            if (flag && re.Count > 0)
            {
                insurancePerHours = int.Parse(re[0].InsurancePerHours.ToString());
            }
            result.checkFlag = flag;
            result.InsurancePerHours = insurancePerHours;
            return result;
        }
        #endregion

        #region 訂單資訊(For計算預授權金額用)
        /// <summary>
        /// 訂單資訊(For計算預授權金額用)
        /// </summary>
        /// <param name="orderNumber">訂單編號</param>
        /// <returns></returns>
        public SPOutput_OrderForPreAuth GetOrderForPreAuth(long orderNumber)
        {
            var re = new SPOutput_OrderForPreAuth();
            var lstError = new List<ErrorInfo>();
            string SPName = "usp_GetOrderInfoForPreAuth_Q01";
            SPOutput_Base spOutBase = new SPOutput_Base();
            SPInput_GetOrderInfo spInput = new SPInput_GetOrderInfo() { OrderNumber = orderNumber };
            SQLHelper<SPInput_GetOrderInfo, SPOutput_Base> sqlHelpQuery = new SQLHelper<SPInput_GetOrderInfo, SPOutput_Base>(connetStr);
            DataSet ds = new DataSet();
            sqlHelpQuery.ExeuteSP(SPName, spInput, ref spOutBase, ref ds, ref lstError);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                re = objUti.GetFirstRow<SPOutput_OrderForPreAuth>(ds.Tables[0]);
            return re;
        }
        #endregion

        #region 試算預授權金額
        /// <summary>
        /// 試算預授權金額
        /// </summary>
        /// <param name="input">試算資料</param>
        /// <param name="dayMaxHour">單日時數上限</param>
        /// <returns></returns>
        public void EstimatePreAuthAmt(EstimateData input, out EstimateDetail outData, int dayMaxHour = 10)
        {
            BillCommon billCommon = new BillCommon();
            List<Holiday> lstHoliday = new CommonRepository(connetStr).GetHolidays(input.SD.ToString("yyyyMMdd"), input.ED.ToString("yyyyMMdd"));
            //計算安心服務金額
            var InsurancePurePrice = (input.Insurance == 1) ? Convert.ToInt32(billCommon.CalSpread(input.SD, input.ED, input.InsurancePerHours * dayMaxHour, input.InsurancePerHours * dayMaxHour, lstHoliday)) : 0;
            //計算預估租金
            var Rent =  billCommon.CarRentCompute(input.SD, input.ED, input.WeekdayPrice, input.HoildayPrice, dayMaxHour, lstHoliday);
            //計算里程費
            float MilUnit = billCommon.GetMilageBase(input.ProjID, input.CarTypeGroupCode, input.CarNo, input.SD, input.ED, 0);
            int MilagePrice = billCommon.CarMilageCompute(input.SD, input.ED, MilUnit, Mildef, 20, lstHoliday);

            if (input.TaxID != null && input.TaxID.Length == 8) {//是否為企業月結
                InsurancePurePrice =  input.EnterpriseInsurance ?  0 : InsurancePurePrice; //判斷企業是否將安心服務列為月結
                Rent = 0;
                MilagePrice = 0;
            }

            outData = new EstimateDetail();
            outData.InsurancePurePrice = InsurancePurePrice;
            outData.Rent = Rent;
            outData.MilUnit = MilUnit;
            outData.MilagePrice = MilagePrice;
            outData.estimateAmt = Rent + InsurancePurePrice; //(租金+安心服務)
        }
        #endregion

        #region 新增預授權
        /// 新增預授權
        /// </summary>
        /// <param name="spInput"></param>
        /// <param name="errCode">錯誤代碼</param>
        /// <returns></returns>F
        public bool sp_InsOrderAuthAmount(SPInput_InsOrderAuthAmount spInput, ref string errCode)
        {
            bool flag = false;
            var lstError = new List<ErrorInfo>();
            SPOutput_Base spOutput = new SPOutput_Base();
            string SPName = "usp_OrderAuthAmount_I01";
            flag = new SQLHelper<SPInput_InsOrderAuthAmount, SPOutput_Base>(connetStr).ExecuteSPNonQuery(SPName, spInput, ref spOutput, ref lstError);
            if (flag)
            {
                if (spOutput.Error == 1 || spOutput.ErrorCode != "0000")
                {
                    flag = false;
                    errCode = spOutput.ErrorCode;
                }
            }
            else
            {
                if (lstError.Count > 0)
                {
                    errCode = lstError[0].ErrorCode;
                }
            }

            return flag;
        }
        #endregion

        #region 新增個人推播訊息
        /// <summary>
        /// 新增個人推播訊息
        /// </summary>
        /// <param name="spInput"></param>
        /// <param name="errCode">錯誤代碼</param>
        /// <returns></returns>
        public bool sp_InsPersonNotification(SPInput_InsPersonNotification spInput, ref string errCode)
        {
            bool flag = false;
            var lstError = new List<ErrorInfo>();
            SPOutput_Base spOutput = new SPOutput_Base();
            string SPName = "usp_InsPersonNotification_I01";
            flag = new SQLHelper<SPInput_InsPersonNotification, SPOutput_Base>(connetStr).ExecuteSPNonQuery(SPName, spInput, ref spOutput, ref lstError);
            if (flag)
            {
                if (spOutput.Error == 1 || spOutput.ErrorCode != "0000")
                {
                    flag = false;
                    errCode = spOutput.ErrorCode;
                }
            }
            else
            {
                if (lstError.Count > 0)
                {
                    errCode = lstError[0].ErrorCode;
                }
            }

            return flag;
        }
        #endregion

        #region 取消訂單
        /// <summary>
        /// 取消訂單
        /// </summary>
        /// <param name="spInput"></param>
        /// <param name="errCode">錯誤代碼</param>
        /// <returns></returns>
        public bool sp_BookingCancel(SPInput_BookingCancel spInput, ref string errCode)
        {
            bool flag = false;
            var lstError = new List<ErrorInfo>();
            SPOutput_Base spOutput = new SPOutput_Base();
            string SPName = "usp_BookingCancel_U01";
            flag = new SQLHelper<SPInput_BookingCancel, SPOutput_Base>(connetStr).ExecuteSPNonQuery(SPName, spInput, ref spOutput, ref lstError);
            if (flag)
            {
                if (spOutput.Error == 1 || spOutput.ErrorCode != "0000")
                {
                    flag = false;
                    errCode = spOutput.ErrorCode;
                }
            }
            else
            {
                if (lstError.Count > 0)
                {
                    errCode = lstError[0].ErrorCode;
                }
            }

            return flag;
        }
        #endregion

        #region 取得訂單完整資訊
        /// <summary>
        /// 取得訂單完整資訊
        /// </summary>
        /// <param name="spInput"></param>
        /// <param name="flag"></param>
        /// <param name="errCode">錯誤代碼</param>
        /// <returns></returns>
        public List<OrderQueryFullData> GetOrderStatusByOrderNo(SPInput_GetOrderStatusByOrderNo spInput, ref bool flag, ref string errCode)
        {
            List<OrderQueryFullData> result = new List<OrderQueryFullData>();
            flag = false;
            var lstError = new List<ErrorInfo>();
            string spName = "usp_GetOrderStatusByOrderNo";        //里程費設定先跳開，後續在合併版本
            SPOutput_Base spOutput = new SPOutput_Base();
            SQLHelper<SPInput_GetOrderStatusByOrderNo, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_GetOrderStatusByOrderNo, SPOutput_Base>(connetStr);
            DataSet ds = new DataSet();
            flag = sqlHelp.ExeuteSP(spName, spInput, ref spOutput, ref result, ref ds, ref lstError);
            baseVerify.checkSQLResult(ref flag, ref spOutput, ref lstError, ref errCode);

            return result;
        }
        #endregion

        #region 寫入信用卡授權排程清單
        /// <summary>
        /// 寫入信用卡授權排程清單
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool InsertOrderAuth(SPInput_OrderAuth Input, ref string errCode, ref List<ErrorInfo> lstError)
        {
            string SPName = "usp_InsOrderAuth_I01";
            SPOutput_Base spOut = new SPOutput_Base();
            SQLHelper<SPInput_OrderAuth, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_OrderAuth, SPOutput_Base>(connetStr);

            var flag = sqlHelp.ExecuteSPNonQuery(SPName, Input, ref spOut, ref lstError);
            baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

            return flag;
        }
        #endregion

        #region 寫入預約信用卡授權排程清單
        /// <summary>
        /// 寫入預約信用卡授權排程清單
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool InsertOrderAuthReservation(SPInput_OrderAuthReservation Input, ref string errCode, ref List<ErrorInfo> lstError)
        {
            string SPName = "usp_InsOrderAuthReservation_I01";
            SPOutput_Base spOut = new SPOutput_Base();
            SQLHelper<SPInput_OrderAuthReservation, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_OrderAuthReservation, SPOutput_Base>(connetStr);

            var flag = sqlHelp.ExecuteSPNonQuery(SPName, Input, ref spOut, ref lstError);
            baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

            return flag;
        }
        #endregion

        #region 取得預授權金額
        /// <summary>
        /// 取得預授權金額
        /// </summary>
        /// <param name="IDNO">帳號</param>
        /// <param name="Token">Token</param>
        /// <param name="OrderNo">訂單編號</param>
        /// <param name="NeedToken">是否需要Token</param>
        /// <param name="LogID"></param>
        /// <param name="flag"></param>
        /// <param name="errCode">錯誤代碼</param>
        /// <returns></returns>
        public PreAmountData GetPreAmount(string IDNO, string Token, Int64 OrderNo, string NeedToken, Int64 LogID, ref bool flag, ref string errCode)
        {
            PreAmountData Result = new PreAmountData()
            {
                DiffAmount = 0,
                TradeCloseList = new List<TradeCloseList>()
            }; // 回傳結果
            var lstError = new List<ErrorInfo>();

            string SPName = "usp_GetPreAmount";

            object[][] parms1 = {
                new object[] {
                    IDNO,
                    Token,
                    OrderNo,
                    NeedToken,
                    LogID
            }};

            DataSet ds1 = null;
            string returnMessage = "";
            string messageLevel = "";
            string messageType = "";

            ds1 = WebApiClient.SPExeBatchMultiArr2(ServerInfo.GetServerInfo(), SPName, parms1, true, ref returnMessage, ref messageLevel, ref messageType);

            if (ds1.Tables.Count != 3)
            {
                flag = false;
                errCode = "ERR999";
            }
            else
            {
                baseVerify.checkSQLResult(ref flag, Convert.ToInt32(ds1.Tables[2].Rows[0]["Error"]), ds1.Tables[2].Rows[0]["ErrorCode"].ToString(), ref lstError, ref errCode);

                if (flag)
                {
                    if (ds1.Tables[0].Rows.Count > 0)
                        Result.DiffAmount = Convert.ToInt32(ds1.Tables[0].Rows[0]["DiffAmount"]);

                    DataTable dt = ds1.Tables[1];

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            var TmpList = new TradeCloseList
                            {
                                CloseID = Convert.ToInt32(dr["CloseID"]),
                                CardType = Convert.ToInt32(dr["CardType"]),
                                AuthType = Convert.ToInt32(dr["AuthType"]),
                                CloseAmout = Convert.ToInt32(dr["CloseAmout"]),
                                ChkClose = Convert.ToInt32(dr["ChkClose"])
                            };

                            Result.TradeCloseList.Add(TmpList);
                        }
                    }
                }
            }

            return Result;
        }
        #endregion

        #region 訂單預授權判斷
        public List<TradeCloseList> DoPreAmount(PreAmountData PreAmount, int FinalPrice)
        {
            List<TradeCloseList> Result = new List<TradeCloseList>();   // 回傳結果
            Result = PreAmount.TradeCloseList;
            int RemainingAmount = FinalPrice;   // 剩餘金額 = 總價(final_price)

            if (PreAmount.DiffAmount > 0) // 補授權
            {
                // 補授權，將目前已收的款項壓上要關帳
                foreach (var item in Result)
                {
                    item.ChkClose = 1;
                }
            }
            else if (PreAmount.DiffAmount == 0)  // 不補不退
            {
                // 不補不退，將目前已收的款項壓上要關帳
                foreach (var item in Result)
                {
                    item.ChkClose = 1;
                }
            }
            else if (PreAmount.DiffAmount < 0)   // 調整授權金
            {
                // 逐筆更新關帳金額，還有餘額則要退款
                foreach (var item in Result)
                {
                    if (RemainingAmount > 0)
                    {
                        RemainingAmount = RemainingAmount - item.CloseAmout;    // 剩餘金額 = 剩餘金額 - 關帳金額

                        if (RemainingAmount < 0)    // 剩餘金額<0，代表要退款
                        {
                            item.RefundAmount = Math.Abs(RemainingAmount);
                        }

                        item.CloseAmout = item.CloseAmout - item.RefundAmount;  // 關帳金額 = 關帳金額 - 退款金額

                        item.ChkClose = 1;  // 整筆對上或調整金額，壓上1:要關
                    }
                    else
                    {
                        item.RefundAmount = item.CloseAmout;    // 整筆退的金額壓至退款金額

                        item.ChkClose = 2;  // 整筆退壓上2:退貨
                    }
                }
            }

            return Result;
        }
        #endregion

        #region 寫入訂單備註
        /// <summary>
        /// 寫入訂單備註
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool InsertOrderExtInfo(SPInput_OrderExtInfo Input, ref string errCode, ref List<ErrorInfo> lstError)
        {
            string SPName = "usp_InsOrderExtinfo_I01";
            SPOutput_Base spOut = new SPOutput_Base();
            SQLHelper<SPInput_OrderExtInfo, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_OrderExtInfo, SPOutput_Base>(connetStr);

            var flag = sqlHelp.ExecuteSPNonQuery(SPName, Input, ref spOut, ref lstError);
            baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

            return flag;
        }
        #endregion
    }
}