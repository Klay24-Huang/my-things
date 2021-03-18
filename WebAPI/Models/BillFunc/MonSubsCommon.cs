using Domain.SP.Input.Rent;
using Domain.SP.Output;
using Domain.SP.Output.Rent;
using OtherService.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using WebAPI.Utils;

namespace WebAPI.Models.BillFunc
{
    /// <summary>
    /// 月租訂閱制
    /// </summary>
    public class MonSubsCommon
    {
        /// <summary>
        /// 取得月租列表
        /// </summary>
        /// <param name="spInput"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public List<SPOutput_GetMonthList> sp_GetMonthList(SPInput_GetMonthList spInput, ref string errMsg)
        {
            List<SPOutput_GetMonthList> re = new List<SPOutput_GetMonthList>();

            try
            {
                //string SPName = new ObjType().GetSPName(ObjType.SPType.GetMonthList);
                string SPName = "usp_GetMonthList_U1";//hack: fix spNm
                object[][] parms1 = {
                    new object[] {
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
                            errMsg = re_db.ErrorMsg;
                    }
                }
                else
                    errMsg = returnMessage;

                return re;
            }
            catch (Exception ex)
            {
                errMsg = ex.ToString();
                throw ex;
            }
        }

    }
}