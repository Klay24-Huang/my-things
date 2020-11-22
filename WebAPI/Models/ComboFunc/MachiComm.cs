using Domain.SP.Input.Mochi;
using Domain.SP.Output;
using Domain.SP.Output.Mochi;
using Domain.WebAPI.output.Mochi;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using WebAPI.Models.Enum;
using WebCommon;

namespace WebAPI.Models.ComboFunc
{
    /// <summary>
    /// 車麻吉
    /// </summary>
    public class MachiComm
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        List<ErrorInfo> lstError = new List<ErrorInfo>();
        MochiParkAPI WebAPI = new MochiParkAPI();
        
        public bool GetToken(Int64 LogID,ref string MochiToken)
        {
            bool flag = true;
           
            DateTime NowDate = DateTime.Now;
            SPInput_GetMachiToken spIGetToken = new SPInput_GetMachiToken()
            {
                NowTime = DateTime.Now.ToString("yyyyMMddHHmmss"),
                LogID = LogID
            };
            SPOutput_GetMachiToken spOGetToken = new SPOutput_GetMachiToken();
            SQLHelper<SPInput_GetMachiToken, SPOutput_GetMachiToken> sqlGetHelp = new SQLHelper<SPInput_GetMachiToken, SPOutput_GetMachiToken>(connetStr);
            string spName = new ObjType().GetSPName(ObjType.SPType.GetMochiToken);

            flag = sqlGetHelp.ExecuteSPNonQuery(spName, spIGetToken, ref spOGetToken, ref lstError);
            if (spOGetToken.Token == "")
            {

                WebAPIOutput_MochiLogin wsOutLogin = new WebAPIOutput_MochiLogin();
                flag = WebAPI.DoLogin(ref wsOutLogin);
                if (flag && wsOutLogin.data.access_token != "")
                {
                    long second = wsOutLogin.data.expires_in;
                    DateTime TokenEnd = NowDate.AddSeconds(second);

                    SPInput_MaintainMachiToken spMaintain = new SPInput_MaintainMachiToken()
                    {
                        Token = wsOutLogin.data.access_token,
                        StartDate = NowDate,
                        EndDate = TokenEnd,
                        LogID = LogID
                    };
                    SPOutput_Base spMainOut = new SPOutput_Base();
                    SQLHelper<SPInput_MaintainMachiToken, SPOutput_Base> sqlMainHelp = new SQLHelper<SPInput_MaintainMachiToken, SPOutput_Base>(connetStr);
                    spName = new ObjType().GetSPName(ObjType.SPType.MaintainMachiToken);
                    flag = sqlMainHelp.ExecuteSPNonQuery(spName, spMaintain, ref spMainOut, ref lstError);
                    if (flag)
                    {
                        MochiToken = wsOutLogin.data.access_token;
                    }
                }
            }
            else
            {

                MochiToken = spOGetToken.Token;
            }
            return flag;
        }
        /// <summary>
        /// 取得此車號於訂單起迄內車麻吉明細
        /// </summary>
        /// <param name="LogID"></param>
        /// <param name="CarNo"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="wsOut"></param>
        /// <returns></returns>
        public bool GetParkingBill(Int64 LogID, string CarNo,string StartDate,string EndDate,ref int ParkingBill, ref WebAPIOutput_QueryBillByCar wsOut)
        {
            bool flag = true;
            string Token = "";
            flag = GetToken(LogID, ref Token);
            ParkingBill = 0;
            if (flag)
            {
                flag = WebAPI.DoQueryBillByCar(Token, CarNo, StartDate, EndDate, ref wsOut);
                if (wsOut != null)
                {
                    if (wsOut.data.Count() > 0)
                    {
                        int len = wsOut.data.Count();
                        for(int i = 0; i < len; i++)
                        {
                            ParkingBill += Convert.ToInt32(wsOut.data[i].amount);
                        }
                    }
                }
            }
         
            return flag;
        }
    }
}