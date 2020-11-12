using Domain.TB.BackEnd;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebCommon;
namespace Reposotory.Implement
{
    public class AccountManageRepository : BaseRepository
    {
        private string _connectionString;

        public AccountManageRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }

        /// <summary>
        /// 取得功能群組列表
        /// </summary>
        /// <param name="FuncGroupID"></param>
        /// <param name="FuncGroupName"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public List<BE_GetFuncGroup> GetFuncGroup(string FuncGroupID, string FuncGroupName, string StartDate, string EndDate)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_GetFuncGroup> lstFunc = null;
  

            int nowCount = 0;
            string SQL = "SELECT * FROM VW_BE_GetFuncGroup ";


            SqlParameter[] para = new SqlParameter[10];
            string term = "";

            if (!string.IsNullOrEmpty(FuncGroupID))
            {
                term += (term == "") ? "" : " AND ";
                term += " FuncGroupID like @FuncGroupID ";
                para[nowCount] = new SqlParameter("@FuncGroupID", SqlDbType.VarChar,60);
                para[nowCount].Value ="%"+FuncGroupID+"%";
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (!string.IsNullOrEmpty(FuncGroupName))
            {
                term += (term == "") ? "" : " AND ";
                term += " FuncGroupName like @FuncGroupName";
                para[nowCount] = new SqlParameter("@FuncGroupName", SqlDbType.NVarChar, 60);
                para[nowCount].Value = string.Format("%{0}%",FuncGroupName);
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                term += (term == "") ? "" : " AND ";
                term += " ((StartDate BETWEEN @StartDate AND @EndDate) AND (EndDate BETWEEN @StartDate AND @EndDate)) ";
                para[nowCount] = new SqlParameter("@StartDate", SqlDbType.DateTime);
                para[nowCount].Value = StartDate;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
                para[nowCount] = new SqlParameter("@EndDate", SqlDbType.DateTime);
                para[nowCount].Value = EndDate;
                para[nowCount].Direction = ParameterDirection.Input;
            }else if(!string.IsNullOrEmpty(StartDate) && string.IsNullOrEmpty(EndDate))
            {
                term += (term == "") ? "" : " AND ";
                term += " ((@StartDate BETWEEN StartDate AND EndDate) ) ";
                para[nowCount] = new SqlParameter("@StartDate", SqlDbType.DateTime);
                para[nowCount].Value = StartDate;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            else if(string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                term += (term == "") ? "" : " AND ";
                term += " ((@EndDate BETWEEN StartDate AND EndDate) ) ";
                para[nowCount] = new SqlParameter("@EndDate", SqlDbType.DateTime);
                para[nowCount].Value = EndDate;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }


            if ("" != term)
            {
                SQL += " WHERE " + term + " ORDER BY SEQNO DESC;";

            }

            lstFunc = GetObjList<BE_GetFuncGroup>(ref flag, ref lstError, SQL, para, term);


            return lstFunc;
        }

        public List<BE_UserGroup> GetUserGroup(string UserGroupID, string UserGroupName,int OperatorID, string StartDate, string EndDate)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_UserGroup> lstUserGroup = null;


            int nowCount = 0;
            string SQL = "SELECT * FROM VW_BE_GetUserGroup ";


            SqlParameter[] para = new SqlParameter[10];
            string term = "";

            if (!string.IsNullOrEmpty(UserGroupID))
            {
                term += (term == "") ? "" : " AND ";
                term += " UserGroupID like @UserGroupID ";
                para[nowCount] = new SqlParameter("@UserGroupID", SqlDbType.VarChar, 60);
                para[nowCount].Value = "%" + UserGroupID + "%";
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (!string.IsNullOrEmpty(UserGroupName))
            {
                term += (term == "") ? "" : " AND ";
                term += " UserGroupName like @UserGroupName";
                para[nowCount] = new SqlParameter("@UserGroupName", SqlDbType.NVarChar, 60);
                para[nowCount].Value = string.Format("%{0}%", UserGroupName);
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (OperatorID>0)
            {
                term += (term == "") ? "" : " AND ";
                term += " OperatorID =@OperatorID";
                para[nowCount] = new SqlParameter("@OperatorID", SqlDbType.Int);
                para[nowCount].Value = OperatorID;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                term += (term == "") ? "" : " AND ";
                term += " ((StartDate BETWEEN @StartDate AND @EndDate) AND (EndDate BETWEEN @StartDate AND @EndDate)) ";
                para[nowCount] = new SqlParameter("@StartDate", SqlDbType.DateTime);
                para[nowCount].Value = StartDate;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
                para[nowCount] = new SqlParameter("@EndDate", SqlDbType.DateTime);
                para[nowCount].Value = EndDate;
                para[nowCount].Direction = ParameterDirection.Input;
            }
            else if (!string.IsNullOrEmpty(StartDate) && string.IsNullOrEmpty(EndDate))
            {
                term += (term == "") ? "" : " AND ";
                term += " ((@StartDate BETWEEN StartDate AND EndDate) ) ";
                para[nowCount] = new SqlParameter("@StartDate", SqlDbType.DateTime);
                para[nowCount].Value = StartDate;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            else if (string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                term += (term == "") ? "" : " AND ";
                term += " ((@EndDate BETWEEN StartDate AND EndDate) ) ";
                para[nowCount] = new SqlParameter("@EndDate", SqlDbType.DateTime);
                para[nowCount].Value = EndDate;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }


            if ("" != term)
            {
                SQL += " WHERE " + term + " ORDER BY USEQNO DESC;";

            }

            lstUserGroup = GetObjList<BE_UserGroup>(ref flag, ref lstError, SQL, para, term);


            return lstUserGroup;
        }
        public BE_GetFuncPower GetFuncPower(int FuncGroupID)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_GetFuncPower> lstFuncGroup = null;

            BE_GetFuncPower obj = null;

            int nowCount = 0;
            string SQL = "SELECT * FROM VW_BE_GetFuncPower ";


            SqlParameter[] para = new SqlParameter[10];
            string term = "";

  
            if (FuncGroupID > 0)
            {
                term += (term == "") ? "" : " AND ";
                term += " FuncGroupID =@FuncGroupID";
                para[nowCount] = new SqlParameter("@FuncGroupID", SqlDbType.Int);
                para[nowCount].Value = FuncGroupID;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
      


            if ("" != term)
            {
                SQL += " WHERE " + term + " ORDER BY FuncGroupID DESC;";

            }

            lstFuncGroup = GetObjList<BE_GetFuncPower>(ref flag, ref lstError, SQL, para, term);
            if (lstFuncGroup != null)
            {
                if (lstFuncGroup.Count > 0)
                {
                    obj = lstFuncGroup[0];
                }
            }

            return obj;
        }
    }
}
