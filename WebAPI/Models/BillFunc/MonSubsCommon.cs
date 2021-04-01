using Domain.SP.Input.Bill;
using Domain.SP.Input.Rent;
using Domain.SP.Input.Subscription;
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
using Domain.SP.Output.Subscription;

namespace WebAPI.Models.BillFunc
{
    /// <summary>
    /// 月租訂閱制
    /// </summary>
    public class MonSubsCommon
    {
       
    }

    /// <summary>
    /// 月租訂閱制sp
    /// </summary>
    public class MonSubsSp
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 取得月租列表
        /// </summary>
        /// <param name="spInput"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public List<SPOutput_GetMonthList> sp_GetMonthList(SPInput_GetMonthList spInput, ref string errCode)
        {
            var re = new List<SPOutput_GetMonthList>();

            try
            {
                //string SPName = new ObjType().GetSPName(ObjType.SPType.GetMonthList);
                string SPName = "usp_GetMonthList_U1";//hack: fix spNm
                object[][] parms1 = {
                    new object[] {
                        spInput.IDNO,
                        spInput.LogID,
                        spInput.IsMoto,
                        spInput.MonType
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
                        re = objUti.ConvertToList<SPOutput_GetMonthList>(ds1.Tables[0]);
                    else if (ds1.Tables.Count == 1)
                    {
                        var re_db = objUti.GetFirstRow<SPOutput_Base>(ds1.Tables[0]);
                        if (re_db != null && re_db.Error != 0 && !string.IsNullOrWhiteSpace(re_db.ErrorMsg))
                            errCode = re_db.ErrorCode;
                    }
                }

                return re;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 取得月租Group
        /// </summary>
        /// <param name="spInput"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public List<SPOut_GetMonthGroup> sp_GetMonthGroup(SPInput_GetMonthGroup spInput, ref string errCode)
        {
            var re = new List<SPOut_GetMonthGroup>();

            try
            {
                //string SPName = new ObjType().GetSPName(ObjType.SPType.GetMonthGroup);
                string SPName = "usp_GetMonthGroup_Q01";//hack: fix spNm
                object[][] parms1 = {
                    new object[] {
                        spInput.IDNO,
                        spInput.LogID,
                        spInput.MonProjID,
                        spInput.SetNow
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
                        re = objUti.ConvertToList<SPOut_GetMonthGroup>(ds1.Tables[0]);
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

        public List<SPOut_GetBuyNowInfo> sp_GetBuyNowInfo(SPInput_GetBuyNowInfo spInput, ref string errCode)
        {
            var re = new List<SPOut_GetBuyNowInfo>();

            try
            {
                //string SPName = new ObjType().GetSPName(ObjType.SPType.GetBuyNowInfo);
                string SPName = "usp_GetBuyNowInfo_Q1";//hack: fix spNm
                object[][] parms1 = {
                    new object[] {
                        spInput.IDNO,
                        spInput.LogID
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
                        re = objUti.ConvertToList<SPOut_GetBuyNowInfo>(ds1.Tables[0]);
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

        public bool sp_CreateSubsMonth(SPInput_CreateSubsMonth spInput, ref string errCode)
        {
            bool flag = false;
            //string spName = new ObjType().GetSPName(ObjType.SPType.CreateSubsMonth);
            string spName = "usp_CreateSubsMonth_U1";//hack: fix spNm

            var lstError = new List<ErrorInfo>();
            var spOutBase = new SPOutput_Base();
            var spOut = new SPOut_CreateSubsMonth();
            SQLHelper<SPInput_CreateSubsMonth, SPOut_CreateSubsMonth> sqlHelp = new SQLHelper<SPInput_CreateSubsMonth, SPOut_CreateSubsMonth>(connetStr);
            bool spFlag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);

            if (spFlag && spOut != null)
            {
                if(spOut.ErrorCode != "0000")
                  errCode = spOut.ErrorCode;
                flag = spOut.xError == 0;
            }

            return flag;
        }
    }

}