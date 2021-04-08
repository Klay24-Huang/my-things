using Domain.SP.Input.PolygonList;
using Domain.SP.Output;
using Domain.SP.Output.PolygonList;
using OtherService.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using WebAPI.Utils;

namespace WebAPI.Models.BillFunc
{
    public class StationCommon
    {
        /// <summary>
        /// 取出方圓同站據點
        /// </summary>
        /// <param name="spInput"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public List<SPOut_GetAlliRentStation> sp_GetAlliRentStation(SpInput_GetAlliRentStation spInput, ref string errCode)
        {
            var re = new List<SPOut_GetAlliRentStation>();

            try
            {
                //string SPName = new ObjType().GetSPName(ObjType.SPType.GetAlliRentStation);
                string SPName = "usp_GetAlliRentStation_Q1";//hack: fix spNm
                object[][] parms1 = {
                    new object[] {
                        spInput.LogID,
                        spInput.lat,
                        spInput.lng,
                        spInput.radius,
                        spInput.CarTypes,
                        spInput.Seats,
                        spInput.SD,
                        spInput.ED,
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
                        re = objUti.ConvertToList<SPOut_GetAlliRentStation>(ds1.Tables[0]);
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
}