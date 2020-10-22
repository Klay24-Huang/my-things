using Domain.TB;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebCommon;

namespace Reposotory.Implement
{
    public class OtherRepository : BaseRepository
    {
        public OtherRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }
        public List<GetFeedBackKindData> GetFeedBackKind(int IsMotor)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<GetFeedBackKindData> lstFeedBackKind = null;

            int nowCount = 0;

            string SQL = @"
            SELECT [Star],[Descript],[FeedBackKindId]
            FROM
            (
                SELECT [IsMotor],[Star],[Descript],[FeedBackKindId] FROM [TB_FeedBackKind] WITH(NOLOCK) WHERE use_flag=1 AND Star = 1
                UNION 
                SELECT [IsMotor],[StarU] AS [Star],[Descript],[FeedBackKindId] FROM [TB_FeedBackKind] WITH(NOLOCK) WHERE use_flag=1 AND StarU= 2
                UNION 
                SELECT [IsMotor],[Star],[Descript],[FeedBackKindId] FROM [TB_FeedBackKind] WITH(NOLOCK) WHERE use_flag=1 AND Star = 3
                UNION 
                SELECT [IsMotor],[Star],[Descript],[FeedBackKindId] FROM [TB_FeedBackKind] WITH(NOLOCK) WHERE use_flag=1 AND Star = 4
                UNION 
                SELECT [IsMotor],[StarU] AS [Star],[Descript],[FeedBackKindId] FROM [TB_FeedBackKind] WITH(NOLOCK) WHERE use_flag=1 AND StarU= 5 
            ) T
            WHERE [IsMotor]=@IsMotor
            Order By [Star],[FeedBackKindId]
            ";

            SqlParameter[] para = new SqlParameter[10];
            string term = "";
            if (IsMotor >= 0 && IsMotor < 2)
            {
                term += (term == "") ? "" : " AND ";
                term += " (IsMotor = @IsMotor ) ";
                para[nowCount] = new SqlParameter("@IsMotor", SqlDbType.Int);
                para[nowCount].Value = IsMotor;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }

            lstFeedBackKind = GetObjList<GetFeedBackKindData>(ref flag, ref lstError, SQL, para, term);

            return lstFeedBackKind;
        }
    }
}