using Domain.TB;
using Domain.WebAPI.output.HiEasyRentAPI;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using WebAPI.Models.BillFunc;

namespace WebAPI.Models.ComboFunc
{
    public class PointerComm
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        public bool GetPointer(string IDNO,string FS,string BED,string FE,string FT,int PROJTYPE,ref int TotalLastPoint, ref int TotalLastPointCar, ref int TotalLastPointMotor,ref int CanUseTotalCarPoint,ref int CanUseTotalMotorPoint)
        {
            #region TB

            bool flag = true;
            //開始送短租查詢

                WebAPIOutput_NPR270Query wsOutput = new WebAPIOutput_NPR270Query();
                HiEasyRentAPI wsAPI = new HiEasyRentAPI();
                flag = wsAPI.NPR270Query(IDNO, ref wsOutput);


                if (flag)
                {
                    int giftLen = wsOutput.Data.Length;

                    if (giftLen > 0)
                    {

                        for (int i = 0; i < giftLen; i++)
                        {
                            DateTime tmpDate;
                            int tmpPoint = 0;
                            bool DateFlag = DateTime.TryParse(wsOutput.Data[i].EDATE, out tmpDate);
                            bool PointFlag = int.TryParse(wsOutput.Data[i].GIFTPOINT, out tmpPoint);
                            if (DateFlag && (tmpDate >= DateTime.Now) && PointFlag)
                            {
                                //  totalPoint += tmpPoint;

                                BonusData objPoint = new BonusData()
                                {
                                    SEQNO = wsOutput.Data[i].SEQNO,

                                    PointType = (wsOutput.Data[i].GIFTTYPE == "01") ? 0 : 1,
                                    EDATE = (wsOutput.Data[i].EDATE == "") ? "" : (wsOutput.Data[i].EDATE.Split(' ')[0]).Replace("/", "-"),
                                    GIFTNAME = wsOutput.Data[i].GIFTNAME,
                                    GIFTPOINT = string.IsNullOrEmpty(wsOutput.Data[i].GIFTPOINT) ? "0" : wsOutput.Data[i].GIFTPOINT,
                                    LASTPOINT = string.IsNullOrEmpty(wsOutput.Data[i].LASTPOINT) ? "0" : wsOutput.Data[i].LASTPOINT,
                                    AllowSend = string.IsNullOrEmpty(wsOutput.Data[i].RCVFLG) ? 0 : ((wsOutput.Data[i].RCVFLG == "Y") ? 1 : 0)

                                };
                                if (objPoint.PointType == 0)
                                {
                                  
                                  
                                    TotalLastPointCar += int.Parse(objPoint.LASTPOINT);
                                  
                                }
                                else if (objPoint.PointType == 1)
                                {
                                   
                                    TotalLastPointMotor += int.Parse(objPoint.LASTPOINT);
                                  
                                }


                                //點數加總
                       
                                TotalLastPoint += int.Parse(objPoint.LASTPOINT);


                            }

                        }

                    DateTime SD = Convert.ToDateTime(FS);
                    DateTime ED = Convert.ToDateTime(FE);
                    List<Holiday> lstHoliday = new CommonRepository(connetStr).GetHolidays(SD.ToString("yyyyMMdd"), ED.ToString("yyyyMMdd"));
                    int days = 0, hours = 0, minutes = 0;
               
                    if (FT != "")
                    {
                        ED = Convert.ToDateTime(BED);
                    }
                    new BillCommon().CalDayHourMin(SD, ED, ref days, ref hours, ref minutes);
                    int needPointer = (days * 60 * 10) + (hours * 10) + minutes;
                    if (PROJTYPE == 4)
                    {

                        CanUseTotalCarPoint = Math.Min(TotalLastPoint, needPointer);
                       CanUseTotalMotorPoint = CanUseTotalCarPoint;
                    }
                    else
                    {
                        CanUseTotalMotorPoint = 0;
                        needPointer -= (needPointer % 30);
                        CanUseTotalCarPoint = Math.Min(TotalLastPointCar, needPointer);

                    }

                }
                }
                else
                {
                    //errCode = "ERR";
                    //errMsg = wsOutput.Message;
                }


            return flag;
            #endregion
        }
        public bool GetPointer(string IDNO, string FS, string BED, string FE, string FT, int PROJTYPE,int BaseMinutes, ref int TotalLastPoint, ref int TotalLastPointCar, ref int TotalLastPointMotor, ref int CanUseTotalCarPoint, ref int CanUseTotalMotorPoint)
        {
            #region TB

            bool flag = true;
            //開始送短租查詢

            WebAPIOutput_NPR270Query wsOutput = new WebAPIOutput_NPR270Query();
            HiEasyRentAPI wsAPI = new HiEasyRentAPI();
            flag = wsAPI.NPR270Query(IDNO, ref wsOutput);


            if (flag)
            {
                int giftLen = wsOutput.Data.Length;

                if (giftLen > 0)
                {

                    for (int i = 0; i < giftLen; i++)
                    {
                        DateTime tmpDate;
                        int tmpPoint = 0;
                        bool DateFlag = DateTime.TryParse(wsOutput.Data[i].EDATE, out tmpDate);
                        bool PointFlag = int.TryParse(wsOutput.Data[i].GIFTPOINT, out tmpPoint);
                        if (DateFlag && (tmpDate >= DateTime.Now) && PointFlag)
                        {
                            //  totalPoint += tmpPoint;

                            BonusData objPoint = new BonusData()
                            {
                                SEQNO = wsOutput.Data[i].SEQNO,

                                PointType = (wsOutput.Data[i].GIFTTYPE == "01") ? 0 : 1,
                                EDATE = (wsOutput.Data[i].EDATE == "") ? "" : (wsOutput.Data[i].EDATE.Split(' ')[0]).Replace("/", "-"),
                                GIFTNAME = wsOutput.Data[i].GIFTNAME,
                                GIFTPOINT = string.IsNullOrEmpty(wsOutput.Data[i].GIFTPOINT) ? "0" : wsOutput.Data[i].GIFTPOINT,
                                LASTPOINT = string.IsNullOrEmpty(wsOutput.Data[i].LASTPOINT) ? "0" : wsOutput.Data[i].LASTPOINT,
                                AllowSend = string.IsNullOrEmpty(wsOutput.Data[i].RCVFLG) ? 0 : ((wsOutput.Data[i].RCVFLG == "Y") ? 1 : 0)

                            };
                            if (objPoint.PointType == 0)
                            {


                                TotalLastPointCar += int.Parse(objPoint.LASTPOINT);

                            }
                            else if (objPoint.PointType == 1)
                            {

                                TotalLastPointMotor += int.Parse(objPoint.LASTPOINT);

                            }


                            //點數加總

                            TotalLastPoint += int.Parse(objPoint.LASTPOINT);


                        }

                    }

                    DateTime SD = Convert.ToDateTime(FS);
                    DateTime ED = Convert.ToDateTime(FE);
                    List<Holiday> lstHoliday = new CommonRepository(connetStr).GetHolidays(SD.ToString("yyyyMMdd"), ED.ToString("yyyyMMdd"));
                    int days = 0, hours = 0, minutes = 0;

                    if (FT != "")
                    {
                        ED = Convert.ToDateTime(BED);
                    }
                    new BillCommon().CalDayHourMin(SD, ED, ref days, ref hours, ref minutes);
                    int needPointer = (days * 60 * 10) + (hours * 60) + minutes;
                    if(minutes==0 && PROJTYPE < 4)
                    {
                        needPointer = 60;
                    }
                    if (PROJTYPE == 4)
                    {
                        if(needPointer< BaseMinutes)
                        {
                            needPointer = BaseMinutes;
                        }
                        CanUseTotalCarPoint = Math.Min(TotalLastPoint, needPointer);
                        CanUseTotalMotorPoint = CanUseTotalCarPoint;
                    }
                    else
                    {
                        CanUseTotalMotorPoint = 0;
                        needPointer -= (needPointer % 30);
                        CanUseTotalCarPoint = Math.Min(TotalLastPointCar, needPointer);

                    }

                }
            }
            else
            {
                //errCode = "ERR";
                //errMsg = wsOutput.Message;
            }


            return flag;
            #endregion
        }
    }
}