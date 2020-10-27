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
    /// <summary>
    /// 專案設定相關
    /// </summary>
    public class ProjectRepository : BaseRepository
    {
        private string _connectionString;
        public ProjectRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }
        /// <summary>
        /// 取得所有有效的停車場
        /// </summary>
        /// <returns></returns>
        public ProjectInfo GetProjectInfo(string ProjID)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<ProjectInfo> lstProjInfo = null;
            ProjectInfo obj = null;
            int nowCount = 0;
            string SQL = "SELECT TOP 1 [PROJID],[PROJTYPE],[PayMode],[IsMonthRent]  FROM [dbo].[TB_Project]  ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            if (false == string.IsNullOrWhiteSpace(ProjID))
            {
                term += " ProjID=@ProjID";
                para[nowCount] = new SqlParameter("@ProjID", SqlDbType.VarChar, 20);
                para[nowCount].Value = ProjID;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if ("" != term)
            {
                SQL += " WITH(NOLOCK) WHERE " + term;
            }
            lstProjInfo = GetObjList<ProjectInfo>(ref flag, ref lstError, SQL, para, term);
            if (lstProjInfo != null)
            {
                if (lstProjInfo.Count > 0)
                {
                    obj = lstProjInfo[0];
                }
            }
            return obj;
        }
        /// <summary>
        /// 取得平日及假日價
        /// </summary>
        /// <param name="ProjID">專案代碼</param>
        /// <param name="CarType">車型</param>
        /// <param name="projType">
        /// <para>0:同站</para>
        /// <para>3:路邊</para>
        /// </param>
        /// <returns></returns>
        public ProjectPriceBase GetProjectPriceBase(string ProjID,string CarType,int projType)
        {
           
                   bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<ProjectPriceBase> lstProjInfo = null;
            ProjectPriceBase obj = null;
            int nowCount = 0;
            string SQL = "SELECT TOP 1  ProjID,PRICE,PRICE_H  FROM VW_GetFullProjectCollectionOfCarTypeGroup  AS VW ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            if (false == string.IsNullOrWhiteSpace(ProjID))
            {
                term += " ProjID=@ProjID";
                para[nowCount] = new SqlParameter("@ProjID", SqlDbType.VarChar, 20);
                para[nowCount].Value = ProjID;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (term != "")
            {
                term += " AND ";
            }
            if (false == string.IsNullOrWhiteSpace(CarType))
            {
                if (projType == 0)
                {
                    //term += " CarTypeGroupCode=@CarType";
                    term += " CarTypeName=@CarType";    //20201027 ADD BY ADAM REASON.目前輸出是由CarTypeName
                    para[nowCount] = new SqlParameter("@CarType", SqlDbType.VarChar, 20);
                    para[nowCount].Value = CarType.ToUpper();
                    para[nowCount].Direction = ParameterDirection.Input;
                    nowCount++;
                }
                else
                {
                    SQL += " INNER JOIN TB_Car AS Car WITH(NOLOCK) ON Car.CarType=VW.CarType AND Car.nowStationID=VW.StationID ";
                    term += " CarNo=@CarNo";
                    para[nowCount] = new SqlParameter("@CarNo", SqlDbType.VarChar, 20);
                    para[nowCount].Value = CarType;
                    para[nowCount].Direction = ParameterDirection.Input;
                    nowCount++;
                }
              
            }
            if ("" != term)
            {
                SQL += "  WHERE " + term;
            }
            SQL += " ORDER BY PRICE DESC ";
            lstProjInfo = GetObjList<ProjectPriceBase>(ref flag, ref lstError, SQL, para, term);
            if (lstProjInfo != null)
            {
                if (lstProjInfo.Count > 0)
                {
                    obj = lstProjInfo[0];
                }
            }
            return obj;
        }
        /// <summary>
        /// 取出以分計費
        /// </summary>
        /// <param name="ProjID"></param>
        /// <param name="CarNo"></param>
        /// <returns></returns>
        public ProjectPriceOfMinuteBase GetProjectPriceBaseByMinute(string ProjID, string CarNo)
        {

            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<ProjectPriceOfMinuteBase> lstProjInfo = null;
            ProjectPriceOfMinuteBase obj = null;
            int nowCount = 0;
            string SQL = "SELECT TOP 1 [ProjID],[BaseMinutes],[BaseMinutesPrice],[BaseMinutesPriceH] ,[Price],[PriceH],[MaxPrice],[MaxPriceH]  FROM [dbo].[TB_PriceByMinutes] AS PriceByMinute ";
            SQL += "  INNER JOIN [dbo].[TB_Car] AS Car  WITH(NOLOCK) ON Car.CarType=PriceByMinute.CarType  ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            if (false == string.IsNullOrWhiteSpace(ProjID))
            {
                term += " ProjID=@ProjID";
                para[nowCount] = new SqlParameter("@ProjID", SqlDbType.VarChar, 20);
                para[nowCount].Value = ProjID;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (term != "")
            {
                term += " AND ";
            }
            if (false == string.IsNullOrWhiteSpace(CarNo))
            {
              
                    term += " Car.CarNo=@CarNo";
                    para[nowCount] = new SqlParameter("@CarNo", SqlDbType.VarChar, 20);
                    para[nowCount].Value = CarNo;
                    para[nowCount].Direction = ParameterDirection.Input;
                    nowCount++;
                

            }
            if ("" != term)
            {
                SQL += " WHERE " + term;
            }
            SQL += " ORDER BY PRICE DESC ";
            lstProjInfo = GetObjList<ProjectPriceOfMinuteBase>(ref flag, ref lstError, SQL, para, term);
            if (lstProjInfo != null)
            {
                if (lstProjInfo.Count > 0)
                {
                    obj = lstProjInfo[0];
                }
            }
            return obj;
        }
    }
}