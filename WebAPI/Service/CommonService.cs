using Domain.SP.Input.Bill;
using Domain.SP.Input.Rent;
using Domain.SP.Output;
using Domain.SP.Output.Bill;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using WebAPI.Models.BaseFunc;
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
        /// 寫入信用卡授權排程清單
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool InsertOrderAuth(SPInput_OrderAuth Input, ref string errCode)
        {
            var lstError = new List<ErrorInfo>();
            string SPName = "usp_InsOrderAuth_I01";
            SPOutput_Base spOut = new SPOutput_Base();
            SQLHelper<SPInput_OrderAuth, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_OrderAuth, SPOutput_Base>(connetStr);

            var flag = sqlHelp.ExecuteSPNonQuery(SPName, Input, ref spOut, ref lstError);
            baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

            return flag;
        }


        /// <summary>
        /// 寫入預約信用卡授權排程清單
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool InsertOrderAuthReservation(SPInput_OrderAuthReservation Input, ref string errCode)
        {
            var lstError = new List<ErrorInfo>();
            string SPName = "usp_InsOrderAuth_I01";
            SPOutput_Base spOut = new SPOutput_Base();
            SQLHelper<SPInput_OrderAuthReservation, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_OrderAuthReservation, SPOutput_Base>(connetStr);

            var flag = sqlHelp.ExecuteSPNonQuery(SPName, Input, ref spOut, ref lstError);
            baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

            return flag;
        }
    }
}