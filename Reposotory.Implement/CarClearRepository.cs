using Domain.TB.BackEnd;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebCommon;
namespace Reposotory.Implement
{
   public  class CarClearRepository:BaseRepository
    {
        private string _connectionString;
        public CarClearRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SDate"></param>
        /// <param name="EDate"></param>
        /// <param name="carid"></param>
        /// <param name="objStation"></param>
        /// <param name="userID"></param>
        /// <param name="status"></param>
        /// <param name="lstError"></param>
        /// <returns></returns>
        public List<BE_CleanData> GetCleanData(string SDate, string EDate, string carid, string objStation, string userID, int status, ref List<ErrorInfo> lstError)
        {
            bool flag = false;
            List<BE_CleanData> lstCardList = null;
            string SQL = "SELECT  * FROM VW_BE_CleanDataQuery ";
            SqlParameter[] para = new SqlParameter[8];
            string term = "";
            int nowCount = 0;
            DateTime SD, ED;
            if (!string.IsNullOrEmpty(SDate) && !string.IsNullOrEmpty(EDate))
            {
                SD = DateTime.Parse(SDate + " 00:00:00");
                ED = DateTime.Parse(EDate + " 23:59:59");
                if (SD.Subtract(ED).TotalMilliseconds > 0)
                {
                    term += string.Format("  (( BookingStart >='{0}' AND BookingEnd <='{1}') OR ( start_time>='{0}' AND stop_time<='{1}' ))", EDate + " 00:00:00", SDate + " 23:59:59");                
                }
                else
                {
                    term += string.Format("  (( BookingStart >='{0}' AND BookingEnd <='{1}') OR ( start_time>='{0}' AND stop_time<='{1}' ))", SDate + " 00:00:00", EDate + " 23:59:59");
                }
            }
            else if (string.IsNullOrEmpty(SDate) && string.IsNullOrEmpty(EDate))
            {
                SDate = DateTime.Now.AddHours(8).AddDays(-2).ToString("yyyy-MM-dd 00:00:00");
                EDate = DateTime.Now.AddHours(8).AddDays(-1).ToString("yyyy-MM-dd 23:59:59");
                term += string.Format("  (( BookingStart >='{0}' AND BookingEnd <='{1}') OR ( start_time>='{0}' AND stop_time<='{1}' ))", SDate, EDate);
            }
            else
            {
                if (!string.IsNullOrEmpty(SDate))
                {
                    term += string.Format("  (('{0}' between BookingStart AND BookingEnd) OR ('{0}' between start_time AND stop_time ))", SDate + " 00:00:00");
                    nowCount++;
                }
                if (!string.IsNullOrEmpty(EDate))
                {
                    term += string.Format("  (('{0}' between BookingStart AND BookingEnd) OR ('{0}' between start_time AND stop_time ))", EDate + " 00:00:00");  
                }
            }
            if (!string.IsNullOrEmpty(carid))
            {
                if (term != "")
                {
                    term += " AND ";
                }
                term += string.Format("  CarNo='{0}'", carid);
            }
            if (!string.IsNullOrEmpty(objStation))
            {  
                if (term != "")
                {
                    term += " AND ";
                }
                term += string.Format("  lend_place='{0}'", objStation);
            }
            if (!string.IsNullOrEmpty(userID))
            {
                if (term != "")
                {
                    term += " AND ";
                }
                term += string.Format("  UserID='{0}'", userID);
            }      
                if (status < 3)
                {
                    if (term != "")
                    {
                        term += " AND ";
                    }
                    term += string.Format("  OrderStatus={0}", status);
                }
            if ("" != term)
            {
                SQL += " WITH(NOLOCK) WHERE (" + term + ")";
            }

            lstCardList = GetObjList<BE_CleanData>(ref flag, ref lstError, SQL, para, term);
            return lstCardList;
        }
        public List<BE_CleanDataWithoutPIC> GetCleanDataWithOutPic(string SDate, string EDate, string carid, string objStation, string userID, int status, ref List<ErrorInfo> lstError)
        {
            bool flag = false;
            List<BE_CleanDataWithoutPIC> lstCardList = null;
            string SQL = "SELECT  distinct ISNULL(VW.Account,UserID) AS Account,OrderNum,ISNULL(ISNULL(IIF(LEN(UserID)=5 AND SUBSTRING(UserID,1,1) IN ('1','2','3','4','5','6','7','8','9'),Maintain.UserName,VW.UserID),Manager.UserName),VW.UserID) AS UserID,";
            SQL+= " outsideClean,insideClean,rescue,dispatch,Anydispatch,Maintenance,OrderStatus,remark,BookingStart,BookingEnd,CarNo,lastCleanTime,lastRentTimes,lend_place FROM VW_BE_CleanDataQueryWithOutPIC AS VW  WITH(NOLOCK)";
            SQL += " LEFT JOIN TB_Maintain_User AS Maintain ON Maintain.Account=VW.Account ";
            SQL += " LEFT JOIN TB_Manager AS Manager ON Manager.Account = VW.Account";
            SqlParameter[] para = new SqlParameter[8];
            string term = "";
            int nowCount = 0;
            DateTime SD, ED;
            if (!string.IsNullOrEmpty(SDate) && !string.IsNullOrEmpty(EDate))
            {

                SD = DateTime.Parse(SDate + " 00:00:00");
                ED = DateTime.Parse(EDate + " 23:59:59");
                if (SD.Subtract(ED).TotalMilliseconds > 0)
                {
                    term += string.Format("  (( BookingStart >='{0}' AND BookingEnd <='{1}') OR ( start_time>='{0}' AND stop_time<='{1}' ))", EDate + " 00:00:00", SDate + " 23:59:59");

                }
                else
                {
                    term += string.Format("  (( BookingStart >='{0}' AND BookingEnd <='{1}') OR ( start_time>='{0}' AND stop_time<='{1}' ))", SDate + " 00:00:00", EDate + " 23:59:59");

                }
            }
            else if (string.IsNullOrEmpty(SDate) && string.IsNullOrEmpty(EDate))
            {
                SDate = DateTime.Now.AddHours(8).AddDays(-2).ToString("yyyy-MM-dd 00:00:00");
                EDate = DateTime.Now.AddHours(8).AddDays(-1).ToString("yyyy-MM-dd 23:59:59");
                term += string.Format("  (( BookingStart >='{0}' AND BookingEnd <='{1}') OR ( start_time>='{0}' AND stop_time<='{1}' ))", SDate, EDate);

            }
            else
            {
                if (!string.IsNullOrEmpty(SDate))
                {
                    term += string.Format("  (('{0}' between BookingStart AND BookingEnd) OR ('{0}' between start_time AND stop_time ))", SDate + " 00:00:00");
                    nowCount++;
                }
                if (!string.IsNullOrEmpty(EDate))
                {
                    term += string.Format("  (('{0}' between BookingStart AND BookingEnd) OR ('{0}' between start_time AND stop_time ))", EDate + " 00:00:00");

                }
            }
            if (!string.IsNullOrEmpty(carid))
            {
                if (term != "")
                {
                    term += " AND ";
                }
                term += string.Format("  CarNo='{0}'", carid);

            }
            if (!string.IsNullOrEmpty(objStation))
            {


                if (term != "")
                {
                    term += " AND ";
                }
                term += string.Format("  lend_place='{0}'", objStation);

            }
            if (!string.IsNullOrEmpty(userID))
            {
                if (term != "")
                {
                    term += " AND ";
                }
                term += string.Format("  UserID='{0}'", userID);

            }

            if (status < 3)
            {
                if (term != "")
                {
                    term += " AND ";
                }
                term += string.Format("  OrderStatus={0}", status);

            }






            if ("" != term)
            {
                SQL += " WHERE (" + term + ")  GROUP BY VW.Account,Maintain.UserName,Manager.UserName,OrderNum,UserID,outsideClean,insideClean,rescue,dispatch,Anydispatch,Maintenance,OrderStatus,remark,BookingStart,BookingEnd,CarNo,lastCleanTime,lastRentTimes,lend_place";
            }
            else
            {
                SQL += " GROUP BY VW.Account,Maintain.UserName,Manager.UserName,OrderNum,UserID,outsideClean,insideClean,rescue,dispatch,Anydispatch,Maintenance,OrderStatus,remark,BookingStart,BookingEnd,CarNo,lastCleanTime,lastRentTimes,lend_place";
            }


            lstCardList = GetObjList<BE_CleanDataWithoutPIC>(ref flag, ref lstError, SQL, para, term);
            return lstCardList;
        }
    }
}
