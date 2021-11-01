using Domain.SP.Input;
using Domain.SP.Input.Bill;
using Domain.SP.Input.Notification;
using Domain.SP.Input.Rent;
using Domain.SP.Output;
using Domain.SP.Output.Bill;
using Domain.TB;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
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

        /// <summary>
        /// 試算預授權金額
        /// </summary>
        /// <param name="input">訂單資訊</param>
        /// <param name="insurance">是否加購安心服務</param>
        /// <param name="dayMaxHour">單日時數上限</param>
        /// <returns></returns>

        public int EstimatePreAuthAmt(SPOutput_OrderForPreAuth input, int insurance, int dayMaxHour = 10)
        {
            BillCommon billCommon = new BillCommon();
            List<Holiday> lstHoliday = new CommonRepository(connetStr).GetHolidays(input.SD.ToString("yyyyMMdd"), input.ED.ToString("yyyyMMdd"));
            //計算安心服務金額
            var InsurancePurePrice = (insurance == 1) ? Convert.ToInt32(billCommon.CalSpread(input.SD, input.ED, input.InsurancePerHours * dayMaxHour, input.InsurancePerHours * dayMaxHour, lstHoliday)) : 0;
            //計算預估租金
            var Rent = billCommon.CarRentCompute(input.SD, input.ED, input.PRICE, input.PRICE_H, dayMaxHour, lstHoliday);
            //計算里程費
            float MilUnit = billCommon.GetMilageBase(input.ProjID, input.CarTypeGroupCode, input.SD, input.ED, 0);
            int MilagePrice = Convert.ToInt32(billCommon.CarMilageCompute(input.SD, input.ED, MilUnit, Mildef, 20, lstHoliday));
            //預授權金額
            return Rent + InsurancePurePrice + MilagePrice; //(租金+安心服務+里程費)
        }

        /// <summary>
        /// 檢查信用卡是否綁卡
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="IDNO">會員編號</param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public (bool hasFind, string cardToken) CheckBindCard(ref bool flag, string IDNO, ref string errCode)
        {
            (bool hasFind, string cardToken) result = (false, "");
            //20201219 ADD BY JERRY 更新綁卡查詢邏輯，改由資料庫查詢
            DataSet ds = Common.getBindingList(IDNO, ref flag, ref errCode, ref errCode);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result.hasFind = true;
                result.cardToken = ds.Tables[0].Rows[0]["CardToken"].ToString();
            }
            else
            {
                flag = false;
                errCode = "ERR730";
            }
            ds.Dispose();
            return result;
        }


        /// <summary>
        /// 新增預授權
        /// </summary>
        /// <param name="spInput"></param>
        /// <param name="errCode"></param>
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

        /// <summary>
        /// 新增個人推播訊息
        /// </summary>
        /// <param name="spInput"></param>
        /// <param name="errCode"></param>
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
    }
}