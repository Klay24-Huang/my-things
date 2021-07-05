using Domain.TB.BackEnd;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCommon;

namespace Reposotory.Implement
{
    public class FeedBackRepository:BaseRepository
    {
        private string _connectionString;
        public FeedBackRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }
        /// <summary>
        /// 用於後台管理→查詢意見回饋
        /// </summary>
        /// <param name="IDNO"></param>
        /// <param name="SDate"></param>
        /// <param name="EDate"></param>
        /// <param name="isHandle"></param>
        /// <returns></returns>
        public List<BE_FeedBackMain> GetFeedBackQuery(string IDNO, string SDate, string EDate, int isHandle)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_FeedBackMain> lstFeedBack = null;
            string SQL = "SELECT * FROM VW_BE_GetFeedBackMain ";
            SqlParameter[] para = new SqlParameter[4];
            string term = "";
            int nowCount = 0;
            if (false == string.IsNullOrEmpty(SDate))
            {
                term = " MKTime>=@SD ";
                para[nowCount] = new SqlParameter("@SD", SqlDbType.VarChar, 30);
                para[nowCount].Value = SDate;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (false == string.IsNullOrEmpty(EDate))
            {

                if ("" != term) { term += " AND "; }
                term += " MKTime<=@ED ";
                para[nowCount] = new SqlParameter("@ED", SqlDbType.VarChar, 30);
                para[nowCount].Value = EDate;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;

            }
            if (false == string.IsNullOrEmpty(IDNO))
            {

                if ("" != term) { term += " AND "; }
                term += " IDNO=@IDNO ";
                para[nowCount] = new SqlParameter("@IDNO", SqlDbType.VarChar, 30);
                para[nowCount].Value = IDNO;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;

            }
            if (isHandle < 2)
            {
                if ("" != term) { term += " AND "; }
                term += " isHandle=@isHandle ";
                para[nowCount] = new SqlParameter("@isHandle", SqlDbType.TinyInt, 1);
                para[nowCount].Value = isHandle;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }

            if ("" != term)
            {
                SQL += " WHERE " + term + " AND mode=2";
            }
            SQL += "  ORDER BY MKTime DESC";

            lstFeedBack = GetObjList<BE_FeedBackMain>(ref flag, ref lstError, SQL, para, term);
            return lstFeedBack;
        }
        //VW_GetFeedBackByOrderMain_201905
        public List<BE_FeedBackMainDetail> GetCarFeedBackQuery(string IDNO, string SDate, string EDate, int isHandle, string CarNo, string CarPlace)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_FeedBackMainDetail> lstFeedBack = null;
            string SQL = "SELECT * FROM VW_BE_GetFeedBackByOrderMain ";
            SqlParameter[] para = new SqlParameter[6];
            string term = "";
            int nowCount = 0;
            //if (false == string.IsNullOrEmpty(SDate))
            //{
            //    term = " MKTime>=@SD ";
            //    para[nowCount] = new SqlParameter("@SD", SqlDbType.VarChar, 30);
            //    para[nowCount].Value = SDate;
            //    para[nowCount].Direction = ParameterDirection.Input;
            //    nowCount++;
            //}
            //if (false == string.IsNullOrEmpty(EDate))
            //{

            //    if ("" != term) { term += " AND "; }
            //    term += " MKTime<=@ED ";
            //    para[nowCount] = new SqlParameter("@ED", SqlDbType.VarChar, 30);
            //    para[nowCount].Value = EDate;
            //    para[nowCount].Direction = ParameterDirection.Input;
            //    nowCount++;

            //}
            if (false == string.IsNullOrEmpty(SDate) || false == string.IsNullOrEmpty(EDate))
            {
                if (false == string.IsNullOrEmpty(SDate))
                {
                term = " CONVERT(CHAR(8),MKTime,112) >= replace(@SD,'-','') ";
                para[nowCount] = new SqlParameter("@SD", SqlDbType.VarChar, 30);
                para[nowCount].Value = SDate;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (false == string.IsNullOrEmpty(EDate))
            {

                if ("" != term) { term += " AND "; }
                term += " CONVERT(CHAR(8),MKTime,112) <= replace(@ED,'-','') ";
                para[nowCount] = new SqlParameter("@ED", SqlDbType.VarChar, 30);
                para[nowCount].Value = EDate;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;

            }
            if (false == string.IsNullOrEmpty(IDNO))
            {

                if ("" != term) { term += " AND "; }
                term += " IDNO=@IDNO ";
                para[nowCount] = new SqlParameter("@IDNO", SqlDbType.VarChar, 30);
                para[nowCount].Value = IDNO;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;

            }
            if (false == string.IsNullOrEmpty(CarNo))
            {

                if ("" != term) { term += " AND "; }
                term += " CarNo=@CarNo ";
                para[nowCount] = new SqlParameter("@CarNo", SqlDbType.VarChar, 30);
                para[nowCount].Value = CarNo;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;

            }
            if (false == string.IsNullOrEmpty(CarPlace))
            {

                if ("" != term) { term += " AND "; }
                term += " nowStationID=@CarPlace ";
                para[nowCount] = new SqlParameter("@CarPlace", SqlDbType.VarChar, 30);
                para[nowCount].Value = CarPlace;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;

            }
            if (isHandle < 2)
            {
                if ("" != term) { term += " AND "; }
                term += " isHandle=@isHandle ";
                para[nowCount] = new SqlParameter("@isHandle", SqlDbType.TinyInt, 1);
                para[nowCount].Value = isHandle;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }

            if ("" != term)
            {
                SQL += " WHERE " + term + " AND mode<2";
            }
            SQL += "  ORDER BY MKTime DESC";

            lstFeedBack = GetObjList<BE_FeedBackMainDetail>(ref flag, ref lstError, SQL, para, term);
            return lstFeedBack;
        }
    }
}
