using Domain.SP.Input.Mochi;
using Domain.SP.Input.OtherService.Machi;
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
using Newtonsoft.Json;

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

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();  //20210422 ADD BY ADAM 

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
        public bool GetParkingBill(Int64 LogID, string CarNo, DateTime StartDate, DateTime EndDate, ref int ParkingBill, ref WebAPIOutput_QueryBillByCar wsOut)
        {
            bool flag = true;
            string Token = "";
            flag = GetToken(LogID, ref Token);
            ParkingBill = 0;
            if (flag)
            {
                flag = WebAPI.DoQueryBillByCar(Token, CarNo, StartDate.ToString("yyyyMMdd"), EndDate.ToString("yyyyMMdd"), ref wsOut);
                if (wsOut != null)
                {
                    if (wsOut.data.Count() > 0)
                    {
                        var api_re = wsOut.data.ToList();

                        //完全包含停車時間
                        var xre = api_re.Where(x => StartDate <= x.details.parking_checked_in_at && EndDate >= x.details.parking_checked_out_at).ToList();                      
                        if (xre != null && xre.Count() > 0)
                        {
                            ParkingBill += xre.Select(x => Convert.ToInt32(Convert.ToDouble(x.amount))).Sum();
                            return flag;
                        }
                        else
                        {
                            ////進場時間>還車時間>出場時間
                            //var ck = api_re.Where(x =>
                            //  x.details.parking_checked_in_at <= x.details.parking_checked_out_at &&
                            //  x.details.parking_checked_in_at >= EndDate &&
                            //  EndDate >= x.details.parking_checked_out_at).ToList();

                            ////停車場可還車?白名單過濾

                            //double parkMins = 0;
                            //if (ck != null && ck.Count() > 0)
                            //    parkMins = api_re.Select(x => x.details.parking_checked_out_at.Subtract(x.details.parking_checked_in_at).TotalMinutes).Sum();

                            //if(parkMins > 72)
                            //{     
                            //    //依比例掛帳
                            //    ParkingBill += ck.Select(x => Convert.ToInt32(x.amount)).Sum();
                            //}
                            //else
                            //{
                            //    //事後全部掛帳
                            //}
                            //return flag;
                        }
                    }
                }
            }

            return flag;
        }
        /// <summary>
        /// 寫入車麻吉停車費
        /// </summary>
        /// <param name="LogID"></param>
        /// <param name="OrderNo">訂單編號（去掉H, 並轉Int64)</param>
        /// <param name="CarNo">車號</param>
        /// <param name="StartDate">此訂單實際取車時間</param>
        /// <param name="EndDate">此訂單按下還車時間</param>
        /// <param name="ParkingBill">回傳車麻吉停車費</param>
        /// <param name="wsOut"></param>
        /// <returns></returns>
        public bool GetParkingBill(Int64 LogID,Int64 OrderNo, string CarNo, DateTime StartDate, DateTime EndDate, ref int ParkingBill, ref WebAPIOutput_QueryBillByCar wsOut)
        {
            //車麻吉重大提示，日期要隔一天才有效果，且只能查當天的資料，去掉車號只能查這台車當年的資料
            bool flag = true;
            string Token = "";
            flag = GetToken(LogID, ref Token);
            //DateTime SD = Convert.ToDateTime(StartDate);
            //DateTime ED = Convert.ToDateTime(EndDate);
            ParkingBill = 0;
            if (flag)
            {
                //logger.Info(JsonConvert.SerializeObject(new { Token, CarNo, StartDate, EndDate }));
                flag = WebAPI.DoQueryBillByCar(Token, CarNo, StartDate.ToString("yyyyMMdd"), EndDate.ToString("yyyyMMdd"), ref wsOut);
                if (wsOut != null)
                {
                    //logger.Info(JsonConvert.SerializeObject(wsOut));
                    
                    if (wsOut.data.Count() > 0)
                    {
                        int len = wsOut.data.Count();
                        for (int i = 0; i < len; i++)
                        {
                            
                            if (wsOut.data[i].details.parking_checked_in_at > StartDate && wsOut.data[i].details.parking_checked_out_at <= EndDate && wsOut.data[i].plate_number == CarNo.Replace(" ", ""))
                            {
                                try
                                {
                                    SPInput_InsParkingFeeData spInsPark = new SPInput_InsParkingFeeData()
                                    {
                                        Amount = Convert.ToInt32(Convert.ToDouble(wsOut.data[i].amount)),
                                        Check_in = Convert.ToDateTime(wsOut.data[i].details.parking_checked_in_at),
                                        Check_out = Convert.ToDateTime(wsOut.data[i].details.parking_checked_out_at),
                                        machi_id = wsOut.data[i].id,
                                        machi_parking_id = wsOut.data[i].store_id,
                                        OrderNo = OrderNo,
                                        CarNo = CarNo,  //20210421 ADD BY ADAM REASON.補上車號
                                        LogID = LogID
                                    };

                                    SPOutput_Base spInsOut = new SPOutput_Base();
                                    SQLHelper<SPInput_InsParkingFeeData, SPOutput_Base> sqlInsHelp = new SQLHelper<SPInput_InsParkingFeeData, SPOutput_Base>(connetStr);
                                    string SPName = new ObjType().GetSPName(ObjType.SPType.InsMachiParkData);
                                    flag = sqlInsHelp.ExecuteSPNonQuery(SPName, spInsPark, ref spInsOut, ref lstError);
                                }
                                catch (Exception ex)
                                {
                                    logger.Error("spInsPark=" + ex.Message);
                                    throw;
                                }
                                //ParkingBill += Convert.ToInt32(wsOut.data[i].amount);
                                try
                                {
                                    ParkingBill += Convert.ToInt32(Convert.ToDouble(wsOut.data[i].amount));
                                }
                                catch (Exception ex)
                                {
                                    logger.Error("ParkingBill+=" + ex.Message);
                                    throw;
                                }
                            }                        
                        }
                    }
                }
            }

            return flag;
        }
    }
}