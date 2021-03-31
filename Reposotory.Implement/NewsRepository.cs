using Domain.TB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCommon;
using Domain.TB.BackEnd;

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

        //20210315唐加
        public List<BE_GetBannerInfo> GetBannerInfo(string name)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_GetBannerInfo> lstBanner = null;
            string SQL = "SELECT * FROM TB_Banner WITH(NOLOCK) ";
            SqlParameter[] para = new SqlParameter[1];
            string term = "";

            term = " MarqueeText like '%" + name + "%'";

            if (term != "")
            {
                SQL += " WHERE " + term;
            }
            SQL += " ORDER BY SEQNO DESC ";
            lstBanner = GetObjList<BE_GetBannerInfo>(ref flag, ref lstError, SQL, para, term);
            return lstBanner;
        }
        public List<BE_GetBannerInfo> GetBannerInfo2(string seqno)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_GetBannerInfo> lstBanner = null;
            string SQL = "SELECT * FROM TB_Banner WITH(NOLOCK) ";
            SqlParameter[] para = new SqlParameter[1];
            string term = "";

            term = " SEQNO='" + seqno + "'";

            if (term != "")
            {
                SQL += " WHERE " + term;
            }
            SQL += " ORDER BY SEQNO DESC ";
            lstBanner = GetObjList<BE_GetBannerInfo>(ref flag, ref lstError, SQL, para, term);
            return lstBanner;
        }
    }
}
