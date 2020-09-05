using Domain;
using Domain.TB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebCommon;

namespace Reposotory.Implement
{
    /// <summary>
    /// 停車場相關
    /// </summary>
    public class ParkingRepository:BaseRepository
    {
        private string _connectionString { set; get; }
        public ParkingRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }
        /// <summary>
        /// 取得所有有效的停車場
        /// </summary>
        /// <returns></returns>
        public List<ParkingData> GetAllParking()
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<ParkingData> lstParking = null;
            int nowCount = 0;
            string SQL = "SELECT  [ParkingType],[ParkingName],[ParkingAddress],[Longitude],[Latitude],[OpenTime] ,[CloseTime] FROM [VW_GetParking] WITH(NOLOCK) WHERE use_flag=1 AND (OpenTime<=DATEADD(HOUR,8,GETDATE()) AND CloseTime>=DATEADD(HOUR,8,GETDATE())) ;";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            lstParking = GetObjList<ParkingData>(ref flag, ref lstError, SQL, para, term);
            return lstParking;
        }
        /// <summary>
        /// 依停車場類型取得所有有效的停車場
        /// </summary>
        /// <param name="ParkType">
        /// 停車場類型
        /// <para>0:一般（調度）</para>
        /// <para>1:特約（車麻吉及其他）</para>
        /// </param>
        /// <returns></returns>
        public List<ParkingData> GetAllParkingByType(int ParkType)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<ParkingData> lstParking = null;
            int nowCount = 0;
            string SQL = string.Format("SELECT  [ParkingType],[ParkingName],[ParkingAddress],[Longitude],[Latitude],[OpenTime] ,[CloseTime] FROM [VW_GetParking] WITH(NOLOCK) WHERE use_flag=1 AND (OpenTime<=DATEADD(HOUR,8,GETDATE()) AND CloseTime>=DATEADD(HOUR,8,GETDATE())) AND ParkingType={0}", ParkType);
            SqlParameter[] para = new SqlParameter[2];
            string term = "";

            lstParking = GetObjList<ParkingData>(ref flag, ref lstError, SQL, para, term);
            return lstParking;
        }
        /// <summary>
        /// 取出方圓據點內，所有有效的停車場
        /// </summary>
        /// <param name="lat">緯度</param>
        /// <param name="lng">經度</param>
        /// <param name="radius">半徑（單位公里）</param>
        /// <returns></returns>
        public List<ParkingData> GetAllParking(double lat, double lng, double radius)
        {

            bool flag = false, hasRange = true;
            double[] latlngLimit = { 0.0, 0.0, 0.0, 0.0 };
            if (lng > 0 && lat > 0 && radius > 0)
            {
                latlngLimit = GetAround(lat, lng, radius);
            }
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<ParkingData> lstParking = null;
            int nowCount = 0;
            string SQL = "SELECT  [ParkingType],[ParkingName],[ParkingAddress],[Longitude],[Latitude],[OpenTime] ,[CloseTime] FROM [VW_GetParking] WITH(NOLOCK) WHERE use_flag=1 AND (OpenTime<=DATEADD(HOUR,8,GETDATE()) AND CloseTime>=DATEADD(HOUR,8,GETDATE())) ";

            SqlParameter[] para = new SqlParameter[2];

            for (int j = 0; j < 4; j++)
            {
                if (latlngLimit[j] == 0)
                {
                    hasRange = false;
                    break;
                }
            }
            string term = "";



            if (hasRange)
            {
                //最小緯度lat、最小經度lng、最大緯度lat、最大經度lng
                SQL += string.Format(" AND (Latitude>={0} AND Latitude<={1}) AND (Longitude>={2} AND Longitude<={3})", latlngLimit[0], latlngLimit[2], latlngLimit[1], latlngLimit[3]);
            }
      
            lstParking = GetObjList<ParkingData>(ref flag, ref lstError, SQL, para, term);
            return lstParking;
        }
        public List<ParkingData> GetAllParkingByType(int ParkType,double lat, double lng, double radius)
        {
            bool flag = false, hasRange = true;
            double[] latlngLimit = { 0.0, 0.0, 0.0, 0.0 };
            if (lng > 0 && lat > 0 && radius > 0)
            {
                latlngLimit = GetAround(lat, lng, radius);
            }
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<ParkingData> lstParking = null;
            int nowCount = 0;
            string SQL = "SELECT  [ParkingType],[ParkingName],[ParkingAddress],[Longitude],[Latitude],[OpenTime] ,[CloseTime] FROM [VW_GetParking] WITH(NOLOCK) WHERE use_flag=1 AND (OpenTime<=DATEADD(HOUR,8,GETDATE()) AND CloseTime>=DATEADD(HOUR,8,GETDATE()))  ";

            SqlParameter[] para = new SqlParameter[2];

            for (int j = 0; j < 4; j++)
            {
                if (latlngLimit[j] == 0)
                {
                    hasRange = false;
                    break;
                }
            }
            string term = "";



            if (hasRange)
            {
                //最小緯度lat、最小經度lng、最大緯度lat、最大經度lng
                SQL += string.Format(" AND (Latitude>={0} AND Latitude<={1}) AND (Longitude>={2} AND Longitude<={3}) AND ParkingType={4}", latlngLimit[0], latlngLimit[2], latlngLimit[1], latlngLimit[3],ParkType);
            }
            //SQL += "ORDER BY StationID ASC;";
            lstParking = GetObjList<ParkingData>(ref flag, ref lstError, SQL, para, term);
            return lstParking;
        }
    }
}
