using Domain.CarMachine;
using Domain.TB.BackEnd;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebCommon;

namespace Reposotory.Implement
{
    /// <summary>
    /// 卡片相關
    /// </summary>
    public class CarCardCommonRepository : BaseRepository
    {
        private string _connectionString;
        public CarCardCommonRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }
        /// <summary>
        /// 取得顧客卡
        /// </summary>
        /// <param name="IDNO"></param>
        /// <param name="lstError"></param>
        /// <returns></returns>
        public List<CardList> GetCardListByCustom(string IDNO, ref List<ErrorInfo> lstError)
        {
            bool flag = false;
            List<CardList> lstCardList = null;
            string SQL = "SELECT  CardNO,'C' AS CardType FROM TB_MemberData ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            int nowCount = 0;
            if (IDNO != "")
            {
                term = " MEMIDNO=@IDNO ";
                para[nowCount] = new SqlParameter("@IDNO", SqlDbType.VarChar, 10);
                para[nowCount].Value = IDNO;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }


            if ("" != term)
            {
                SQL += " WITH(NOLOCK) WHERE (" + term + ")";
            }


            lstCardList = GetObjList<CardList>(ref flag, ref lstError, SQL, para, term);
            return lstCardList;
        }
        public List<BE_MasterCarDataOfPart> GetAllCardListByMaster()
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_MasterCarDataOfPart> lstCardList = null;
            string SQL = "SELECT  [ManagerId],[CardNo],[CarNo] FROM  TB_MasterCard ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            int nowCount = 0;
            SQL += " ORDER BY ManagerId ASC,CardNo ASC";


            lstCardList = GetObjList<BE_MasterCarDataOfPart>(ref flag, ref lstError, SQL, para, term);
            return lstCardList;
        }
        public List<BE_MasterCarDataOfPart> GetAllCardListByMaster(string CardNo,string ManagerId)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_MasterCarDataOfPart> lstCardList = null;
            string SQL = "SELECT  [ManagerId],[CardNo],[CarNo] FROM  TB_MasterCard ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            int nowCount = 0;
            if (CardNo != "")
            {
                term = " CardNo=@CardNo ";
                para[nowCount] = new SqlParameter("@CardNo", SqlDbType.VarChar, 20);
                para[nowCount].Value = CardNo;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (ManagerId != "")
            {
                if (term != "")
                {
                    term += " AND ";
                }
                term = " ManagerId=@ManagerId ";
                para[nowCount] = new SqlParameter("@ManagerId", SqlDbType.VarChar, 20);
                para[nowCount].Value = ManagerId;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }


            if ("" != term)
            {
                SQL += " WITH(NOLOCK) WHERE (" + term + ")";
            }
            SQL += " ORDER BY ManagerId ASC,CardNo ASC";


            lstCardList = GetObjList<BE_MasterCarDataOfPart>(ref flag, ref lstError, SQL, para, term);
            return lstCardList;
        }
        /// <summary>
        /// 後台讀卡記錄
        /// </summary>
        /// <param name="CarNo"></param>
        /// <param name="SDate"></param>
        /// <param name="EDate"></param>
        /// <returns></returns>
        public List<BE_ReadCardLog> GetReadCardLogs(string CarNo,string SDate,string EDate)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_ReadCardLog> lstCardList = null;
            string SQL = "SELECT  [ReadCardID],[CID],[CarNo],[CardNo],[Status] ,[ReadTime] FROM  TB_ReadCard ";
            SqlParameter[] para = new SqlParameter[10];
            string term = "";
            int nowCount = 0;
            if (CarNo != "")
            {
                term = " CarNo=@CarNo ";
                para[nowCount] = new SqlParameter("@CarNo", SqlDbType.VarChar, 20);
                para[nowCount].Value = CarNo;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (SDate != "")
            {
                if (term != "")
                {
                    term += " AND ";
                }
                term += " ReadTime>=@SDate ";
                para[nowCount] = new SqlParameter("@SDate", SqlDbType.DateTime);
                para[nowCount].Value = SDate;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }

            if (EDate != "")
            {
                if (term != "")
                {
                    term += " AND ";
                }
                term += " ReadTime<=@EDate ";
                para[nowCount] = new SqlParameter("@EDate", SqlDbType.DateTime);
                para[nowCount].Value = EDate;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }


            if ("" != term)
            {
                SQL += " WITH(NOLOCK) WHERE (" + term + ")";
            }
            SQL += " ORDER BY ReadCardID ASC";


            lstCardList = GetObjList<BE_ReadCardLog>(ref flag, ref lstError, SQL, para, term);
            return lstCardList;
        }
        public List<BE_CarEventLog> GetCarEventLogs(string CarNo, string SDate, string EDate)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_CarEventLog> lstCarEventLogs = null;
            string SQL = "SELECT  * FROM  VW_BE_GetCarRawData ";
            SqlParameter[] para = new SqlParameter[10];
            string term = "";
            int nowCount = 0;
            if (CarNo != "")
            {
                term = " CarNo=@CarNo ";
                para[nowCount] = new SqlParameter("@CarNo", SqlDbType.VarChar, 20);
                para[nowCount].Value = CarNo;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (SDate != "")
            {
                if (term != "")
                {
                    term += " AND ";
                }
                term += " MKTime>=@SDate ";
                para[nowCount] = new SqlParameter("@SDate", SqlDbType.DateTime);
                para[nowCount].Value = SDate;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }

            if (EDate != "")
            {
                if (term != "")
                {
                    term += " AND ";
                }
                term += " MKTime<=@EDate ";
                para[nowCount] = new SqlParameter("@EDate", SqlDbType.DateTime);
                para[nowCount].Value = EDate;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }


            if ("" != term)
            {
                SQL += " WITH(NOLOCK) WHERE (" + term + ")";
            }
            SQL += " ORDER BY MKTime ASC";


            lstCarEventLogs = GetObjList<BE_CarEventLog>(ref flag, ref lstError, SQL, para, term);
            return lstCarEventLogs;
        }
        public List<BE_CardSettingData> GetCardSettingLogs(string CarNo, string SDate, string EDate)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_CardSettingData> lstCardSettingLogs = null;
            string SQL = "SELECT  * FROM  TB_CardSettingLog ";
            SqlParameter[] para = new SqlParameter[10];
            string term = "";
            int nowCount = 0;
            if (CarNo != "")
            {
                term = " CarNo=@CarNo ";
                para[nowCount] = new SqlParameter("@CarNo", SqlDbType.VarChar, 20);
                para[nowCount].Value = CarNo;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (SDate != "")
            {
                if (term != "")
                {
                    term += " AND ";
                }
                term += " MKTime>=@SDate ";
                para[nowCount] = new SqlParameter("@SDate", SqlDbType.DateTime);
                para[nowCount].Value = SDate;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }

            if (EDate != "")
            {
                if (term != "")
                {
                    term += " AND ";
                }
                term += " MKTime<=@EDate ";
                para[nowCount] = new SqlParameter("@EDate", SqlDbType.DateTime);
                para[nowCount].Value = EDate;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }


            if ("" != term)
            {
                SQL += " WITH(NOLOCK) WHERE (" + term + ")";
            }
            SQL += " ORDER BY MKTime ASC";


            lstCardSettingLogs = GetObjList<BE_CardSettingData>(ref flag, ref lstError, SQL, para, term);
            return lstCardSettingLogs;
        }
    }
}
