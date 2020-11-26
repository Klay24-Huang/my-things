using Domain.TB;
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
    public class NewsRepository:BaseRepository
    {
        private string _connectionString;
        public NewsRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }
        public List<News> GetNewsList(ref List<ErrorInfo> lstError)
        {
            bool flag = false;
            List<News> lstNews = null;
            string SQL = "SELECT * FROM TB_News ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            int nowCount = 0;


            if ("" != term)
            {
                SQL += " WHERE (" + term + ")";
            }
            SQL += " ORDER BY SD DESC";


            lstNews = GetObjList<News>(ref flag, ref lstError, SQL, para, term);
            return lstNews;
        }
        public List<News> GetNewsList(int NewsID, ref List<ErrorInfo> lstError)
        {
            bool flag = false;
            List<News> lstNews = null;
            string SQL = "SELECT * FROM TB_News ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            int nowCount = 0;
            if (NewsID > 0)
            {
                term += " NewsID=@NewsID ";
                para[nowCount] = new SqlParameter("@NewsID", SqlDbType.Int);
                para[nowCount].Value = NewsID;
                para[nowCount].Direction = ParameterDirection.Input;
            }

            if ("" != term)
            {
                SQL += " WHERE (" + term + ")";
            }
            SQL += " ORDER BY SD DESC";


            lstNews = GetObjList<News>(ref flag, ref lstError, SQL, para, term);
            return lstNews;
        }
    }
}
