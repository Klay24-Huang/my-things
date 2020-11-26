using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.TB;
using Domain.TB.Maintain;
using WebCommon;

namespace Reposotory.Implement
{
    /// <summary>
    /// 整備人員專用
    /// </summary>
    public class CarRepository : BaseRepository
    {
        private string _connectionString;
        public CarRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">模式
        /// <para>0:同站租還</para>
        /// <para>1:路邊租還</para>
        /// <param name="StationID">據點，string[]</param>
        /// <param name="lon">經度</param>
        /// <param name="lat">緯度</param>
        /// <param name="raidus">雷達圈半徑</param>
        /// <returns></returns>
        public List<CarCleanDataNew> GetCleanCarData(int type, string StationID, double lon, double lat, double raidus)
        {
            List<CarCleanDataNew> carInfo = null;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string term = "";
            bool flag = false, hasRange = true;
            double[] latlngLimit = { 0.0, 0.0, 0.0, 0.0 };
            if (lon > 0 && lat > 0 && raidus > 0)
            {
                latlngLimit = GetAround(lat, lon, raidus);
            }
            string SQL = "SELECT * FROM VW_MA_GetAllCarData";
            if (type == 0)
            {
                SQL += " WITH(NOLOCK) WHERE NowStationID NOT IN ('X0SR','X0R4','X0U4')";
            }
            else
            {
                SQL += " WITH(NOLOCK) WHERE NowStationID  IN ('X0SR','X0R4','X0U4')";
            }
            for (int j = 0; j < 4; j++)
            {
                if (latlngLimit[j] == 0)
                {
                    hasRange = false;
                    break;
                }
            }
            if (hasRange)
            {
                //最小緯度lat、最小經度lng、最大緯度lat、最大經度lng
                SQL += string.Format(" AND (Lat>={0} AND Lat<={1}) AND (Lng>={2} AND Lng<={3})", latlngLimit[0], latlngLimit[2], latlngLimit[1], latlngLimit[3]);
            }
            if (StationID != "")
            {
                string[] tmpStationID = StationID.Split(';');
                int len = tmpStationID.Length;
                string StationTerm = "";
                if (len > 0 && len < 2)
                {
                    StationTerm += string.Format("'{0}'", tmpStationID[0]);

                }
                else if (len > 1)
                {
                    StationTerm += string.Format("'{0}'", tmpStationID[0]);
                    for (int i = 1; i < len; i++)
                    {
                        StationTerm += string.Format(",'{0}'", tmpStationID[i]);
                    }
                }
                if (StationTerm != "")
                {
                    SQL += " AND ManageStationID IN (" + StationTerm + ")";
                }
            }

            SQL += "ORDER BY nowStationID ASC  ";

            SqlParameter[] para = new SqlParameter[2];
            carInfo = GetObjList<CarCleanDataNew>(ref flag, ref lstError, SQL, para, term);
            return carInfo;
        }
        /// <summary>
        /// 整備人員取得需保養的車輛
        /// </summary>
        /// <param name="StationID"></param>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        /// <param name="raidus"></param>
        /// <returns></returns>
        public List<CarCleanDataNew> GetCleanCarDataOfMaintenance(int type, string StationID, double lon, double lat, double raidus)
        {
            List<CarCleanDataNew> carInfo = null;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string term = "";
            bool flag = false, hasRange = true;
            double[] latlngLimit = { 0.0, 0.0, 0.0, 0.0 };
            if (lon > 0 && lat > 0 && raidus > 0)
            {
                latlngLimit = GetAround(lat, lon, raidus);
            }
            string SQL = "SELECT * FROM VW_MA_GetAllCarData WITH(NOLOCK) ";
            if (type == 0)
            {
                SQL += "  WHERE NowStationID NOT IN ('X0SR','X0R4','X0U4') ";
            }
            else
            {
                SQL += "  WHERE NowStationID  IN ('X0SR','X0R4','X0U4') ";
            }
            for (int j = 0; j < 4; j++)
            {
                if (latlngLimit[j] == 0)
                {
                    hasRange = false;
                    break;
                }
            }
            if (hasRange)
            {
                //最小緯度lat、最小經度lng、最大緯度lat、最大經度lng
                term += string.Format(" AND (Lat>={0} AND Lat<={1}) AND (Lng>={2} AND Lng<={3})", latlngLimit[0], latlngLimit[2], latlngLimit[1], latlngLimit[3]);
            }
            if (StationID != "")
            {
                string[] tmpStationID = StationID.Split(';');
                int len = tmpStationID.Length;
                string StationTerm = "";
                if (len > 0 && len < 2)
                {
                    StationTerm += string.Format("'{0}'", tmpStationID[0]);

                }
                else if (len > 1)
                {
                    StationTerm += string.Format("'{0}'", tmpStationID[0]);
                    for (int i = 1; i < len; i++)
                    {
                        StationTerm += string.Format(",'{0}'", tmpStationID[i]);
                    }
                }
                if (StationTerm != "")
                {

                    term += " AND ManageStationID IN (" + StationTerm + ")";
                }
            }

            term += " AND (Milage-LastMaintenanceMilage)>10000 ";

            SQL += term;
            term = "";
            SQL += " ORDER BY nowStationID ASC  ";

            SqlParameter[] para = new SqlParameter[2];
            carInfo = GetObjList<CarCleanDataNew>(ref flag, ref lstError, SQL, para, term);
            return carInfo;
        }
        /// <summary>
        /// 整備人員(汽車)供下載使用
        /// </summary>
        /// <param name="StationID"></param>
        /// <returns></returns>
        public List<CarCleanDataNew> GetCleanCarData(string StationID)
        {
            List<CarCleanDataNew> carInfo = null;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string term = "";
            bool flag = false;
            string SQL = "SELECT * FROM VW_MA_GetAllCarData WITH(NOLOCK) ";

            if (StationID != "")
            {
                string[] tmpStationID = StationID.Split(';');
                int len = tmpStationID.Length;
                string StationTerm = "";
                if (len > 0 && len < 2)
                {
                    StationTerm += string.Format("'{0}'", tmpStationID[0]);

                }
                else if (len > 1)
                {
                    StationTerm += string.Format("'{0}'", tmpStationID[0]);
                    for (int i = 1; i < len; i++)
                    {
                        StationTerm += string.Format(",'{0}'", tmpStationID[i]);
                    }
                }
                if (StationTerm != "")
                {
                    SQL += " WHERE ManageStationID IN (" + StationTerm + ") ";
                }
            }

            SQL += "ORDER BY nowStationID ASC  ";

            SqlParameter[] para = new SqlParameter[2];
            carInfo = GetObjList<CarCleanDataNew>(ref flag, ref lstError, SQL, para, term);
            return carInfo;
        }
        /// <summary>
        /// 整備網頁版(機車)
        /// </summary>

        /// <returns></returns>
        public List<CarCleanData> GetCleanMotoData()
        {
            List<CarCleanData> carInfo = null;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string term = "";
            bool flag = false;
            string SQL = "SELECT * FROM VW_MA_GetAllMotoData";
            SqlParameter[] para = new SqlParameter[2];
            carInfo = GetObjList<CarCleanData>(ref flag, ref lstError, SQL, para, term);
            return carInfo;
        }
        /// <summary>
        /// 整備網頁版(機車)供下載使用
        /// </summary>
        /// <param name="StationID"></param>
        /// <returns></returns>
        public List<CarCleanDataNew> GetCleanMotoData(string StationID)
        {
            List<CarCleanDataNew> carInfo = null;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            bool flag = false;
            string term = "";
            string SQL = "SELECT * FROM VW_MA_GetAllMotoData ";
            if (StationID != "")
            {
                string[] tmpStationID = StationID.Split(';');
                int len = tmpStationID.Length;
                string StationTerm = "";
                if (len > 0 && len < 2)
                {
                    StationTerm += string.Format("'{0}'", tmpStationID[0]);

                }
                else if (len > 1)
                {
                    StationTerm += string.Format("'{0}'", tmpStationID[0]);
                    for (int i = 1; i < len; i++)
                    {
                        StationTerm += string.Format(",'{0}'", tmpStationID[i]);
                    }
                }
                if (StationTerm != "")
                {
                    if (term != "")
                    {
                        term += " AND ManageStationID IN (" + StationTerm + ")";
                    }
                    else
                    {
                        term = " ManageStationID IN (" + StationTerm + ")";
                    }

                }
            }

            if (term != "")
            {
                SQL += " WITH(NOLOCK) WHERE " + term;
            }
            SQL += "ORDER BY nowStationID ASC  ";
            SqlParameter[] para = new SqlParameter[2];
            carInfo = GetObjList<CarCleanDataNew>(ref flag, ref lstError, SQL, para, term);
            return carInfo;
        }
        /// <summary>
        /// 整備網頁版(機車)
        /// </summary>
        /// <param name="StationID">據點，string[]</param>
        /// <param name="lon">經度</param>
        /// <param name="lat">緯度</param>
        /// <param name="raidus">雷達圈半徑</param>
        /// <returns></returns>
        public List<CarCleanDataNew> GetCleanMotoData(string StationID, double lon, double lat, double raidus)
        {
            List<CarCleanDataNew> carInfo = null;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string term = "";
            bool flag = false, hasRange = true;
            double[] latlngLimit = { 0.0, 0.0, 0.0, 0.0 };
            if (lon > 0 && lat > 0 && raidus > 0)
            {
                latlngLimit = GetAround(lat, lon, raidus);
            }
            string SQL = "SELECT * FROM VW_MA_GetAllMotoData ";
            for (int j = 0; j < 4; j++)
            {
                if (latlngLimit[j] == 0)
                {
                    hasRange = false;
                    break;
                }
            }
            if (hasRange)
            {
                //最小緯度lat、最小經度lng、最大緯度lat、最大經度lng
                term += string.Format("  (Lat>={0} AND Lat<={1}) AND (Lng>={2} AND Lng<={3})", latlngLimit[0], latlngLimit[2], latlngLimit[1], latlngLimit[3]);
            }
            if (StationID != "")
            {
                string[] tmpStationID = StationID.Split(';');
                int len = tmpStationID.Length;
                string StationTerm = "";
                if (len > 0 && len < 2)
                {
                    StationTerm += string.Format("'{0}'", tmpStationID[0]);

                }
                else if (len > 1)
                {
                    StationTerm += string.Format("'{0}'", tmpStationID[0]);
                    for (int i = 1; i < len; i++)
                    {
                        StationTerm += string.Format(",'{0}'", tmpStationID[i]);
                    }
                }
                if (StationTerm != "")
                {
                    if (term != "")
                    {
                        term += " AND ManageStationID IN (" + StationTerm + ")";
                    }
                    else
                    {
                        term = " ManageStationID IN (" + StationTerm + ")";
                    }

                }
            }

            if (term != "")
            {
                SQL += " WITH(NOLOCK) WHERE " + term;
            }
            SQL += "ORDER BY nowStationID ASC  ";
            SqlParameter[] para = new SqlParameter[2];
            carInfo = GetObjList<CarCleanDataNew>(ref flag, ref lstError, SQL, para, term);
            return carInfo;
        }
        /// <summary>
        /// 整備人員，取得保養車輛
        /// </summary>
        /// <param name="StationID"></param>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        /// <param name="raidus"></param>
        /// <returns></returns>
        public List<CarCleanDataNew> GetCleanMotoDataOfMaintenance(string StationID, double lon, double lat, double raidus)
        {
            List<CarCleanDataNew> carInfo = null;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string term = "";
            bool flag = false, hasRange = true;
            double[] latlngLimit = { 0.0, 0.0, 0.0, 0.0 };
            if (lon > 0 && lat > 0 && raidus > 0)
            {
                latlngLimit = GetAround(lat, lon, raidus);
            }
            string SQL = "SELECT * FROM VW_MA_GetAllMotoData ";
            for (int j = 0; j < 4; j++)
            {
                if (latlngLimit[j] == 0)
                {
                    hasRange = false;
                    break;
                }
            }
            if (hasRange)
            {
                //最小緯度lat、最小經度lng、最大緯度lat、最大經度lng
                term += string.Format("  (Lat>={0} AND Lat<={1}) AND (Lng>={2} AND Lng<={3})", latlngLimit[0], latlngLimit[2], latlngLimit[1], latlngLimit[3]);
            }
            if (StationID != "")
            {
                string[] tmpStationID = StationID.Split(';');
                int len = tmpStationID.Length;
                string StationTerm = "";
                if (len > 0 && len < 2)
                {
                    StationTerm += string.Format("'{0}'", tmpStationID[0]);

                }
                else if (len > 1)
                {
                    StationTerm += string.Format("'{0}'", tmpStationID[0]);
                    for (int i = 1; i < len; i++)
                    {
                        StationTerm += string.Format(",'{0}'", tmpStationID[i]);
                    }
                }
                if (StationTerm != "")
                {
                    if (term != "")
                    {
                        term += " AND ManageStationID IN (" + StationTerm + ")";
                    }
                    else
                    {
                        term = " ManageStationID IN (" + StationTerm + ")";
                    }

                }
            }
            if (term != "")
            {
                term += " AND ";
            }
            term += " (LastMaintenanceMilage<1000 OR ((Milage-LastMaintenanceMilage)>3000)) ";

            if (term != "")
            {
                SQL += " WITH(NOLOCK) WHERE " + term;
            }
            SQL += "ORDER BY nowStationID ASC  ";
            SqlParameter[] para = new SqlParameter[2];
            carInfo = GetObjList<CarCleanDataNew>(ref flag, ref lstError, SQL, para, term);
            return carInfo;
        }
        /// <summary>
        /// 取得整備人員資訊
        /// </summary>
        /// <param name="Account">取得整備人員資訊</param>
        /// <returns></returns>
        public MemberCleanSetting GetMemberCleanSetting(string Account)
        {
            List<MemberCleanSetting> memberInfo = null;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            bool flag = false;
            string SQL = "SELECT ISNULL([Account],'') AS Account,ISNULL([StationGroup],'') AS StationGroup,ISNULL([Lat],0.0) AS Lat,ISNULL([Lng],0.0) AS Lng  FROM [TB_MemberCleanSetting] ";
            int nowCount = 0;
            if (false == string.IsNullOrEmpty(Account))
            {
                term = " Account=@Account ";
                para[nowCount] = new SqlParameter("@Account", SqlDbType.VarChar, 50);
                para[nowCount].Value = Account;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (term != "")
            {
                SQL += " WITH(NOLOCK) WHERE " + term;
            }
            SQL += " ORDER BY Account ASC  ";


            memberInfo = GetObjList<MemberCleanSetting>(ref flag, ref lstError, SQL, para, term);
            if (memberInfo != null)
            {
                if (memberInfo.Count > 0)
                {
                    return memberInfo[0];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
            //return carInfo;
        }
        /// <summary>
        /// 取得所有管轄據點
        /// </summary>
        /// <returns></returns>
        public List<iRentManagerStation> GetiRentManageStation()
        {
            bool flag = false;
            List<iRentManagerStation> ManageStationInfo = null;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            string SQL = "SELECT * FROM TB_ManagerStation  ORDER BY StationID ASC  ";
            ManageStationInfo = GetObjList<iRentManagerStation>(ref flag, ref lstError, SQL, para, term);
            return ManageStationInfo;
        }
    }
}
