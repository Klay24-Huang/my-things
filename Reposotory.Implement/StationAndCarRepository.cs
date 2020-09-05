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
    /// 車輛及據點專用
    /// </summary>
   public class StationAndCarRepository
    {
        private string _connectionString;
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }
        public StationAndCarRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }
        private List<T> GetObjList<T>(ref bool flag, ref List<ErrorInfo> lstError, string SQL, SqlParameter[] para, string term)
            where T : class
        {
            List<T> lstObj;
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            using (SqlCommand command = new SqlCommand(SQL, conn))
            {
                if ("" != term)
                {
                    for (int i = 0; i < para.Length; i++)
                    {
                        if (null != para[i])
                        {
                            command.Parameters.Add(para[i]);
                        }

                    }
                }
                command.CommandType = CommandType.Text;
                command.CommandTimeout = 180;
                lstObj = new List<T>();
                if (conn.State != ConnectionState.Open) conn.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //  MachineInfo item = new MachineInfo();
                        T obj = (T)Activator.CreateInstance(typeof(T));

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            PropertyInfo property = obj.GetType().GetProperty(reader.GetName(i));

                            if (property != null && !reader.GetValue(i).Equals(DBNull.Value))
                            {
                                flag = new ReflectionHelper().SetPropertyValue(property.Name, reader.GetValue(i).ToString(), ref obj, ref lstError);
                            }
                        }

                        lstObj.Add(obj);
                    }
                }
            }

            return lstObj;
        }
        /// <summary>
        /// 取得所有據點
        /// </summary>
        /// <returns></returns>
        public List<iRentStationData> GetAlliRentStation()
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<iRentStationData> lstStation = null;
            int nowCount = 0;
            string SQL = "SELECT  [StationID],[Location] AS StationName,[Tel],[ADDR],[Latitude],[Longitude],[Content]  FROM [dbo].[TB_iRentStation] WITH(NOLOCK) WHERE use_flag=3 ORDER BY StationID ASC;";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
                    lstStation = GetObjList<iRentStationData>(ref flag, ref lstError, SQL, para, term);
            return lstStation;
        }
        /// <summary>
        /// 取出方圓據點
        /// </summary>
        /// <param name="lat">緯度</param>
        /// <param name="lng">經度</param>
        /// <param name="radius">半徑（單位公里）</param>
        /// <returns></returns>
        public List<iRentStationData> GetAlliRentStation(double lat,double lng,double radius)
        {

            bool flag = false, hasRange = true;
            double[] latlngLimit = { 0.0, 0.0, 0.0, 0.0 };
            if (lng > 0 && lat > 0 && radius > 0)
            {
                latlngLimit = GetAround(lat, lng, radius);
            }
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<iRentStationData> lstStation = null;
            int nowCount = 0;
            string SQL = "SELECT  [StationID],[Location] AS StationName,[Tel],[ADDR],[Latitude],[Longitude],[Content]  FROM [dbo].[TB_iRentStation] WITH(NOLOCK) WHERE use_flag=3 ";
          
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
            SQL += "ORDER BY StationID ASC;";
            lstStation = GetObjList<iRentStationData>(ref flag, ref lstError, SQL, para, term);
            return lstStation;
        }
        #region GetAnyRent
        /// <summary>
        /// 取得所有車輛
        /// </summary>
        /// <returns></returns>
        public List<AnyRentObj> GetAllAnyRent()
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<AnyRentObj> lstCar = null;
            int nowCount = 0;
            string SQL = "SELECT [CarNo],[CarType],CONCAT([CarBrend],' ',[CarTypeName]) AS CarTypeName,REPLACE([PRONAME],'路邊汽車推廣專案','') AS CarOfArea,[PRONAME] AS ProjectName,[PRICE]/10 AS Rental,2.5 AS Mileage,0 AS Insurance,0 As InsurancePrice,0 As ShowSpecial,'' As SpecialInfo,[Latitude] ,[Longitude]";
                   SQL +=" FROM  [VW_GetAllAnyRentData] WITH(NOLOCK) WHERE GPSTime>=DATEADD(MINUTE,-30,GETDATE())";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            lstCar = GetObjList<AnyRentObj>(ref flag, ref lstError, SQL, para, term);
            return lstCar;
        }
        /// <summary>
        /// 取出方圓車輛
        /// </summary>
        /// <param name="lat">緯度</param>
        /// <param name="lng">經度</param>
        /// <param name="radius">半徑（單位公里）</param>
        /// <returns></returns>
        public List<AnyRentObj> GetAllAnyRent(double lat, double lng, double radius)
        {

            bool flag = false, hasRange = true;
            double[] latlngLimit = { 0.0, 0.0, 0.0, 0.0 };
            if (lng > 0 && lat > 0 && radius > 0)
            {
                latlngLimit = GetAround(lat, lng, radius);
            }
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<AnyRentObj> lstCar = null;
            int nowCount = 0;
            string SQL = "SELECT [CarNo],[CarType],CONCAT([CarBrend],' ',[CarTypeName]) AS CarTypeName,REPLACE([PRONAME],'路邊汽車推廣專案','') AS CarOfArea,[PRONAME] AS ProjectName,[PRICE]/10 AS Rental,2.5 AS Mileage,0 AS Insurance,0 As InsurancePrice,0 As ShowSpecial,'' As SpecialInfo,[Latitude] ,[Longitude] ";
            SQL += " FROM  [VW_GetAllAnyRentData] WITH(NOLOCK) WHERE GPSTime>=DATEADD(MINUTE,-30,GETDATE()) ";

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
          
            lstCar = GetObjList<AnyRentObj>(ref flag, ref lstError, SQL, para, term);
            return lstCar;
        }
        #endregion
        #region GetMotorRent
        /// <summary>
        /// 取得所有車輛
        /// </summary>
        /// <returns></returns>
        public List<MotorRentObj> GetAllMotorRent()
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<MotorRentObj> lstCar = null;
            int nowCount = 0;
            string SQL = "SELECT [CarNo],[CarType],CONCAT([CarBrend],' ',[CarTypeName]) AS CarTypeName,REPLACE([PRONAME],'10載便利','') AS CarOfArea,[PRONAME] AS ProjectName,[PRICE] AS Rental,2.5 AS Mileage,0 AS Insurance,0 As InsurancePrice,0 As ShowSpecial,'' As SpecialInfo,[Latitude] ,[Longitude]";
            SQL += " ,device2TBA AS 'Power',deviceRDistance AS RemainingMileage ";
            SQL += " FROM  [VW_GetAllMotorAnyRentData] WITH(NOLOCK) WHERE GPSTime>=DATEADD(MINUTE,-30,GETDATE())  AND device2TBA>=30 ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            lstCar = GetObjList<MotorRentObj>(ref flag, ref lstError, SQL, para, term);
            return lstCar;
        }
        /// <summary>
        /// 取出方圓車輛
        /// </summary>
        /// <param name="lat">緯度</param>
        /// <param name="lng">經度</param>
        /// <param name="radius">半徑（單位公里）</param>
        /// <returns></returns>
        public List<MotorRentObj> GetAllMotorRent(double lat, double lng, double radius)
        {

            bool flag = false, hasRange = true;
            double[] latlngLimit = { 0.0, 0.0, 0.0, 0.0 };
            if (lng > 0 && lat > 0 && radius > 0)
            {
                latlngLimit = GetAround(lat, lng, radius);
            }
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<MotorRentObj> lstCar = null;
            int nowCount = 0;
            string SQL = "SELECT [CarNo],[CarType],CONCAT([CarBrend],' ',[CarTypeName]) AS CarTypeName,REPLACE([PRONAME],'10載便利','') AS CarOfArea,[PRONAME] AS ProjectName,[PRICE] AS Rental,2.5 AS Mileage,0 AS Insurance,0 As InsurancePrice,0 As ShowSpecial,'' As SpecialInfo,[Latitude] ,[Longitude]";
            SQL += " ,device2TBA AS 'Power',deviceRDistance AS RemainingMileage ";
            SQL += " FROM  [VW_GetAllMotorAnyRentData] WITH(NOLOCK) WHERE GPSTime>=DATEADD(MINUTE,-30,GETDATE()) AND device2TBA>=30 ";

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

            lstCar = GetObjList<MotorRentObj>(ref flag, ref lstError, SQL, para, term);
            return lstCar;
        }
        #endregion
        /// <summary>
        /// 取得常用站點
        /// </summary>
        /// <param name="IDNO">帳號</param>
        /// <returns></returns>
        public List<iRentStationData> GetFavoriteStation(string IDNO)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<iRentStationData> lstStation = null;
            int nowCount = 0;
            string SQL = "SELECT [StationID],[StationName],[ADDR],[Tel],[Latitude],[Longitude],[Content] FROM [dbo].[VW_GetFavoriteStation] WITH(NOLOCK) ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            if(string.IsNullOrEmpty(IDNO)==false && string.IsNullOrWhiteSpace(IDNO) == false)
            {
                term = " IDNO=@IDNO";
                para[nowCount] = new SqlParameter("@IDNO", SqlDbType.VarChar, 20);
                para[nowCount].Value = IDNO;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (term != "")
            {
                SQL += " WHERE " + term;
            }
            SQL += " ORDER BY MKTime DESC ";
            lstStation = GetObjList<iRentStationData>(ref flag, ref lstError, SQL, para, term);
            return lstStation;
        }
        /// <summary>
        /// 取得同站租還據點車型
        /// </summary>
        /// <param name="StationID">據點代碼</param>
        /// <returns></returns>
        public List<CarTypeData> GetStationCarType(string StationID)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<CarTypeData> lstStation = null;
            int nowCount = 0;
            string SQL = "SELECT [CarBrend],[CarType],[CarTypeName] FROM [dbo].[VW_FullProjectCollection] WITH(NOLOCK) ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            if (string.IsNullOrEmpty(StationID) == false && string.IsNullOrWhiteSpace(StationID) == false)
            {
                term = " StationID=@StationID";
                para[nowCount] = new SqlParameter("@StationID", SqlDbType.VarChar, 20);
                para[nowCount].Value = StationID;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (term != "")
            {
                SQL += " WHERE " + term + "  AND CarTypeName<>''";
            }
            else
            {
                SQL += " WHERE  CarTypeName<>'' ";
            }
            SQL += " GROUP BY  [CarBrend],[CarType],[CarTypeName]";
            lstStation = GetObjList<CarTypeData>(ref flag, ref lstError, SQL, para, term);
            return lstStation;
        }
        public List<CarTypeData> GetStationCarTypeWithOutCarType(string StationID)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<CarTypeData> lstStation = null;
            int nowCount = 0;
            string SQL = "SELECT [CarBrend],[CarTypeName] FROM [dbo].[VW_FullProjectCollection] WITH(NOLOCK) ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            if (string.IsNullOrEmpty(StationID) == false && string.IsNullOrWhiteSpace(StationID) == false)
            {
                term = " StationID=@StationID";
                para[nowCount] = new SqlParameter("@StationID", SqlDbType.VarChar, 20);
                para[nowCount].Value = StationID;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (term != "")
            {
                SQL += " WHERE " + term + "  AND CarTypeName<>''";
            }
            else
            {
                SQL += " WHERE  CarTypeName<>'' ";
            }
            SQL += " GROUP BY  [CarBrend],[CarTypeName]";
            lstStation = GetObjList<CarTypeData>(ref flag, ref lstError, SQL, para, term);
            return lstStation;
        }
        /// <summary>
        /// 取得經緯度最大範圍
        /// </summary>
        /// <param name="lat">目前緯度</param>
        /// <param name="lon">目前經度</param>
        /// <param name="raidus">半徑(單位公尺)</param>
        /// <returns>double array依序最小緯度lat、最小經度lng、最大緯度lat、最大經度lng</returns>
        public double[] GetAround(double lat, double lon, double raidus)
        {
            Double latitude = lat;
            Double longitude = lon;

            Double degree = (24901 * 1609) / 360.0;
            //Double degree = 1 / 360.0;
           

            Double dpmLat = 1 / degree;
            double raidusMile = (raidus * 750);
            Double radiusLat = dpmLat * raidusMile;
            Double minLat = latitude - radiusLat;
            Double maxLat = latitude + radiusLat;

            Double mpdLng = degree * Math.Cos(latitude * (Math.PI / 180));
            Double dpmLng = 1 / mpdLng;
            Double radiusLng = dpmLng * raidusMile;
            Double minLng = longitude - radiusLng;
            Double maxLng = longitude + radiusLng;
            return new double[] { minLat, minLng, maxLat, maxLng };

        }
        //public double[] CalLagLatToMin(double lat,double lon,double raidus)
        //{
        //    //x度 y分 z秒 = x + y / 60 + z / 3600 度
        //    #region 經緯度轉度分秒
        //      int du = Convert.ToInt32(lat);
        //      int min = Convert.ToInt32((lat - du) * 60);
        //       double second = (((lat - du) * 60) - min) / 60;
        //    #endregion
        //    const double meterPerSecond = 40000000 / 3600 / 60 / 60;
        //    double unit = (raidus * 1000) / meterPerSecond; //半徑xx公里要多少秒
        //    int unitMin = 0;
        //    double unitSecond = 0.0;
        //    #region 度分秒轉回經緯度
        //    if (unit >= 1)
        //    {
        //        unitMin = Convert.ToInt32(unit);
        //        unitSecond = unit - unitMin;
        //    }
        //    else
        //    {
        //        unitSecond = unit;
        //    }
        //    #endregion
        //}
    }
}
