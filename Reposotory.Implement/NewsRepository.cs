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
        public List<BE_GetBannerInfo> GetBannerInfo(string name,string order, List<string> terms)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_GetBannerInfo> lstBanner = null;
            string SQL = "SELECT SEQNO,STARTDATE,ENDDATE,QUEUE,URL,MarqueeText,PIC_NAME," +
                "'Status' = CASE WHEN convert(char(8),getdate(),112) > convert(char(8),ENDDATE,112) THEN '失效' " +
                "WHEN convert(char(8),getdate(),112) between convert(char(8),STARTDATE,112) and convert(char(8),ENDDATE,112) THEN '有效' " +
                "WHEN convert(char(8),getdate(),112) < convert(char(8), STARTDATE, 112) THEN '待生效' END " +
                ",CAST(convert(char(25),STARTDATE,121) AS VARCHAR(25)) AS STARTDATE2,CAST(convert(char(25),ENDDATE,121) AS VARCHAR(25)) AS ENDDATE2 " +
                "FROM TB_Banner WITH(NOLOCK)";
            SqlParameter[] para = new SqlParameter[1];
            string term = "";

            if (name != "")
            {
                term = " MarqueeText like '%" + name + "%' AND ";
            }
            if (order!="0")
            {
                term += "QUEUE =" + order + " AND ";
            }

            //switch (status)
            //{
            //    case "0":
            //        term += "convert(char(8),getdate(),112) > convert(char(8),ENDDATE,112)";
            //        break;
            //    case "1":
            //        term += "convert(char(8),getdate(),112) between convert(char(8),STARTDATE,112) and convert(char(8),ENDDATE,112)";
            //        break;
            //    case "2":
            //        term += "convert(char(8),getdate(),112) < convert(char(8),STARTDATE,112)";
            //        break;
            //    case "3":
            //        term += "1=1";
            //        break;
            //}
            int TermsLen = terms.Count();
            if (TermsLen > 0)
            {
                if (TermsLen==1)
                {
                    for (int i = 0; i < TermsLen; i++)
                    {
                        switch (terms[i])
                        {
                            case "0"://失效
                                term += "convert(char(8),getdate(),112) > convert(char(8),ENDDATE,112)";
                                break;
                            case "1"://有效
                                term += "convert(char(8),getdate(),112) between convert(char(8),STARTDATE,112) and convert(char(8),ENDDATE,112)";
                                break;
                            case "2"://待生效
                                term += "convert(char(8),getdate(),112) < convert(char(8),STARTDATE,112)";
                                break;
                        }
                    }
                }
                else if (TermsLen==2)
                {
                    List<string> checklist = new List<string>();
                    List<string> checklist2 = new List<string>();
                    List<string> checklist3 = new List<string>();
                    checklist.Add("0");
                    checklist.Add("1");
                    checklist2.Add("1");
                    checklist2.Add("2");
                    checklist3.Add("0");
                    checklist3.Add("2");
                    if (terms.SequenceEqual(checklist))//失效、有效
                    {
                        term += "(convert(char(8),getdate(),112) > convert(char(8),ENDDATE,112) or convert(char(8),getdate(),112) between convert(char(8),STARTDATE,112) and convert(char(8),ENDDATE,112))";
                    }
                    else if (terms.SequenceEqual(checklist2))//有效、待生效
                    {
                        term += "(convert(char(8),getdate(),112) between convert(char(8),STARTDATE,112) and convert(char(8),ENDDATE,112) or convert(char(8),getdate(),112) < convert(char(8),STARTDATE,112))";
                    }
                    else if (terms.SequenceEqual(checklist3))//失效、待生效
                    {
                        term += "(convert(char(8),getdate(),112) > convert(char(8),ENDDATE,112) or convert(char(8),getdate(),112) < convert(char(8),STARTDATE,112))";
                    }
                }
            }
            else
            {
                term += " 1=1 ";
            }

            if (term != "")
            {
                SQL += " WHERE " + term;
            }
            SQL += " ORDER BY A_SYSDT DESC ";
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
