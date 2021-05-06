using Domain.TB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebCommon;
using Domain.TB.BackEnd;

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
        public bool HandleHoilday(string[] newHoildays, string[] newShortHoildays, string[] DelStr)
        {
            bool flag = true;
            string SQL = "";
            int len = newHoildays.Length;
            int DelLen = DelStr.Length;
            if (len > 0)
            {
                for(int i = 0; i < len; i++)
                {
                    SQL += string.Format("INSERT INTO TB_Holiday([HolidayYearMonth],[HolidayDate],[use_flag])VALUES({0},{1},1);", newShortHoildays[i], newHoildays[i]);
                }
                ExecNonResponse(ref flag, SQL);
            }
            string DelSQLStr = string.Join(",", DelStr);
            SQL = "DELETE FROM TB_Holiday WHERE HolidayDate IN (" + DelSQLStr + ");";
            ExecNonResponse(ref flag, SQL);
            return flag;
        }
        /// <summary>
        /// 取得暫存出還車照
        /// </summary>
        /// <param name="OrderNo"></param>
        /// <param name="Mode"></param>
        /// <returns></returns>
        public List<CarPIC> GetCarPIC(Int64 OrderNo,Int16 Mode)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<CarPIC> lstCarPIC = null;

            int nowCount = 0;

            string SQL = @"SELECT [ImageType],[Image]  FROM TB_CarImageTemp ";

            SqlParameter[] para = new SqlParameter[10];
            string term = "";
            if (OrderNo > 0)
            {
                term += (term == "") ? "" : " AND ";
                term += " (OrderNo = @OrderNo ) ";
                para[nowCount] = new SqlParameter("@OrderNo", SqlDbType.BigInt);
                para[nowCount].Value = OrderNo;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (Mode > 0)
            {
                term += (term == "") ? "" : " AND ";
                term += " (Mode = @Mode ) ";
                para[nowCount] = new SqlParameter("@Mode", SqlDbType.TinyInt);
                para[nowCount].Value = Mode;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (term != "")
            {
                SQL += " WITH(NOLOCK) WHERE " + term;
            }
            lstCarPIC = GetObjList<CarPIC>(ref flag, ref lstError, SQL, para, term);

            return lstCarPIC;
        }
        public bool HandleTempCarPIC(Int64 OrderNo,Int16 Mode,Int16 ImgType, string FileName)
        {
            bool flag = true;
            string SQL = "";

            SQL = "UPDATE TB_CarImageTemp SET Image='" + FileName + "',HasUpload=1,UPDTime='"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+"' WHERE OrderNo =" + OrderNo+" AND Mode="+Mode+" AND ImageType="+ImgType+";";
            ExecNonResponse(ref flag, SQL);
            return flag;
        }
        /// <summary>
        /// 取得出車回饋照
        /// </summary>
        /// <param name="OrderNo"></param>
        /// <returns></returns>
        public List<FeedBackPIC> GetFeedBackPIC(Int64 OrderNo)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<FeedBackPIC> lstFeedBackPic = null;

            int nowCount = 0;

            string SQL = @"
            SELECT [tmpFeedBackPICID] AS FeedBackPICID,[SEQNO],[FeedBackFile]  FROM TB_tmpFeedBackPIC ";

            SqlParameter[] para = new SqlParameter[10];
            string term = "";
            if (OrderNo > 0 )
            {
                term += (term == "") ? "" : " AND ";
                term += " (OrderNo = @OrderNo ) ";
                para[nowCount] = new SqlParameter("@OrderNo", SqlDbType.BigInt);
                para[nowCount].Value = OrderNo;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (term != "")
            {
                SQL += " WITH(NOLOCK) WHERE " + term;
            }
            SQL += "  Order By SEQNO ASC";
            lstFeedBackPic = GetObjList<FeedBackPIC>(ref flag, ref lstError, SQL, para, term);

            return lstFeedBackPic;
        }

        public bool HandleTempFeedBackPIC(Int64 FeedBackPICID,string FileName)
        {
            bool flag = true;
            string SQL = "";
       
            SQL = "UPDATE TB_tmpFeedBackPIC SET FeedBackFile='"+FileName+ "',UPDTime='"+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE tmpFeedBackPICID =" + FeedBackPICID + ";";
            ExecNonResponse(ref flag, SQL);
            return flag;
        }


        /// <summary>
        /// 光陽維運app查詢
        /// </summary>
        public List<BE_GetKymcoList> GetKymcoLists(int AuditMode, string StartDate, string EndDate)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_GetKymcoList> lstAudits = null;
            SqlParameter[] para = new SqlParameter[10];
            string term = "";
            //string SQL = " SELECT TOP 300 * FROM VW_GetAuditList WITH(NOLOCK) ";
            string SQL = " EXEC usp_BE_GetKymcoList  '" + AuditMode.ToString() +
                "','" + (StartDate == "" ? "" : StartDate + " 00:00:00") +
                "','" + (EndDate == "" ? "" : EndDate + " 23:59:59") + "'";

            lstAudits = GetObjList<BE_GetKymcoList>(ref flag, ref lstError, SQL, para, term);
            return lstAudits;
        }


        /// <summary>
        /// 會員審核明細報表查詢-20210305唐加
        /// </summary>
        public List<BE_GetMemList> GetMemLists(int AuditMode, string StartDate, string EndDate, string ID)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_GetMemList> lstAudits = null;
            SqlParameter[] para = new SqlParameter[9];
            string term = "";
            //string SQL = " SELECT TOP 300 * FROM VW_GetAuditList WITH(NOLOCK) ";
            string SQL = " EXEC usp_BE_GetMemList  '" + AuditMode.ToString() +
                "','" + (StartDate == "" ? "" : StartDate + " 00:00:00") +
                "','" + (EndDate == "" ? "" : EndDate + " 23:59:59") +
                "','" + ID + "'";

            lstAudits = GetObjList<BE_GetMemList>(ref flag, ref lstError, SQL, para, term);
            return lstAudits;
        }
    }
}