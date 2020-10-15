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
        /// 取出據點的名稱及代碼
        /// </summary>
        /// <returns></returns>
        public List<iRentStationBaseInfo> GetAlliRentStationBaseData()
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<iRentStationBaseInfo> lstStation = null;
            int nowCount = 0;
            string SQL = "SELECT  [StationID],[Location] AS StationName   FROM [dbo].[TB_iRentStation] WITH(NOLOCK) WHERE use_flag=3 ORDER BY StationID ASC;";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            lstStation = GetObjList<iRentStationBaseInfo>(ref flag, ref lstError, SQL, para, term);
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
            string SQL = "SELECT [CarNo],[CarType],CONCAT([CarBrend],' ',[CarTypeName]) AS CarTypeName,REPLACE([PRONAME],'路邊汽車推廣專案','') AS CarOfArea, ";
            SQL += " [PRONAME] AS ProjectName,[PRICE]/10 AS Rental,2.5 AS Mileage,0 AS Insurance,0 As InsurancePrice,0 As ShowSpecial,'' As SpecialInfo, ";
            SQL += " [Latitude] ,[Longitude], OperatorICon[Operator], Score[OperatorScore], CarTypeImg[CarTypePic], Seat, [PROJID] as ProjID ";
            SQL += " FROM [VW_GetAllAnyRentData] WITH(NOLOCK) WHERE GPSTime>=DATEADD(MINUTE,-30,GETDATE())";
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
            string SQL = "SELECT [CarNo],[CarType],CONCAT([CarBrend],' ',[CarTypeName]) AS CarTypeName,REPLACE([PRONAME],'路邊汽車推廣專案','') AS CarOfArea, ";
            SQL += " [PRONAME] AS ProjectName,[PRICE]/10 AS Rental,2.5 AS Mileage,0 AS Insurance,0 As InsurancePrice,0 As ShowSpecial,'' As SpecialInfo, ";
            SQL += " [Latitude] ,[Longitude] ,OperatorICon[Operator] ,Score[OperatorScore] ,CarTypeImg[CarTypePic], Seat, [PROJID] as ProjID ";
            SQL += " FROM [VW_GetAllAnyRentData] WITH(NOLOCK) WHERE GPSTime>=DATEADD(MINUTE,-30,GETDATE()) ";

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
        #region 取出電池站
        public List<BatExchangeStationData> GetAllBatStation()
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BatExchangeStationData> lstBat = null;
            int nowCount = 0;
            string SQL = "SELECT [Name],Station,Addr,lon AS Longitude,lat AS  Latitude,TotalCnt,EmptyCnt,FullCnt,UPDTime AS UpdateTime   FROM [TB_BAT_Station] WITH(NOLOCK)  WHERE use_flag = 1 ";
    
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            lstBat = GetObjList<BatExchangeStationData>(ref flag, ref lstError, SQL, para, term);
            return lstBat;
        }
        public List<BatExchangeStationData> GetAllBatStation(double lat, double lng, double radius)
        {

            bool flag = false, hasRange = true;
            double[] latlngLimit = { 0.0, 0.0, 0.0, 0.0 };
            if (lng > 0 && lat > 0 && radius > 0)
            {
                latlngLimit = GetAround(lat, lng, radius);
            }
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BatExchangeStationData> lstBat = null;
            int nowCount = 0;
            string SQL = "SELECT [Name],Station,Addr,lon AS Longitude,lat AS  Latitude,TotalCnt,EmptyCnt,FullCnt,UPDTime AS UpdateTime   FROM [TB_BAT_Station] WITH(NOLOCK)  WHERE use_flag = 1 ";

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

            lstBat = GetObjList<BatExchangeStationData>(ref flag, ref lstError, SQL, para, term);
            return lstBat;
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
            string SQL = "SELECT [CarNo],[CarTypeName] as CarType,CONCAT([CarBrend],' ',[CarTypeName]) AS CarTypeName,REPLACE([PRONAME],'10載便利','') AS CarOfArea, ";
            SQL += " [PRONAME] AS ProjectName,[PRICE] AS Rental,2.5 AS Mileage,0 AS Insurance,0 As InsurancePrice,0 As ShowSpecial,'' As SpecialInfo ";
            SQL += " ,[Latitude] ,[Longitude],device2TBA AS 'Power',deviceRDistance AS RemainingMileage ";
            SQL += " ,[OperatorICon] As Operator,[Score] As OperatorScore, [PROJID] As ProjID, [BaseMinutes], [BaseMinutesPrice] As BasePrice, [PerMinutesPrice] ";  
            SQL += " FROM [VW_GetAllMotorAnyRentData] WITH(NOLOCK) WHERE GPSTime>=DATEADD(MINUTE,-30,GETDATE()) AND device2TBA>=30 ";
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
            string SQL = "SELECT [CarNo],[CarTypeName] AS CarType,CONCAT([CarBrend],' ',[CarTypeName]) AS CarTypeName,REPLACE([PRONAME],'10載便利','') AS CarOfArea, ";
            SQL += " [PRONAME] AS ProjectName,[PRICE] AS Rental,2.5 AS Mileage,0 AS Insurance,0 As InsurancePrice,0 As ShowSpecial,'' As SpecialInfo ";
            SQL += " ,[Latitude] ,[Longitude],device2TBA AS 'Power',deviceRDistance AS RemainingMileage ";
            SQL += " ,[OperatorICon] As Operator,[Score] As OperatorScore, [PROJID] As ProjID, [BaseMinutes], [BaseMinutesPrice] As BasePrice, [PerMinutesPrice] "; 
            SQL += " FROM [VW_GetAllMotorAnyRentData] WITH(NOLOCK) WHERE GPSTime>=DATEADD(MINUTE,-30,GETDATE()) AND device2TBA>=30 ";

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
            //string SQL = " SELECT [CarBrend],[CarTypeGroupCode] AS CarType,[CarTypeName],[CarTypeImg] As CarTypePic,OperatorICon AS Operator,Score As OperatorScore,Seat,-1 AS Price FROM [dbo].[VW_GetFullProjectCollectionOfCarTypeGroup]  WITH(NOLOCK) ";
            string SQL = @"
            SELECT v.CarBrend,
                   v.CarTypeGroupCode AS CarType,
                   v.CarTypeName,
                   v.CarTypeImg AS CarTypePic,
                   v.OperatorICon AS Operator,
                   v.Score AS OperatorScore,
                   v.Seat,
                   v.Price,
                   v.PRICE_H
            FROM [dbo].[VW_GetFullProjectCollectionOfCarTypeGroup] v WITH(NOLOCK)
            ";

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
                SQL += " WHERE " + term + "  AND CarTypeName<>''  ";
            }
            else
            {
                SQL += " WHERE  CarTypeName<>'' ";
            }
            SQL += " GROUP BY  [CarBrend],[CarTypeGroupCode],[CarTypeName],[CarTypeImg],OperatorICon,Score,Seat,Price,PRICE_H";
            lstStation = GetObjList<CarTypeData>(ref flag, ref lstError, SQL, para, term);
            return lstStation;
        }
        /// <summary>
        /// 用車號取出車型及目前所在據點
        /// </summary>
        /// <param name="CarNo"></param>
        /// <returns></returns>
        public CarData GetCarData(string CarNo)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<CarData> lstCarData = null;
            CarData obj = null;
            int nowCount = 0;
            string SQL = " SELECT CarType,nowStationID AS StationID FROM [dbo].[TB_Car]  WITH(NOLOCK) ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            if (string.IsNullOrEmpty(CarNo) == false && string.IsNullOrWhiteSpace(CarNo) == false)
            {
                term = " CarNo=@CarNo";
                para[nowCount] = new SqlParameter("@CarNo", SqlDbType.VarChar, 20);
                para[nowCount].Value = CarNo;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (term != "")
            {
                SQL += " WHERE " + term + "   ";
            }
        
            lstCarData = GetObjList<CarData>(ref flag, ref lstError, SQL, para, term);
            if (lstCarData != null)
            {
                if (lstCarData.Count > 0)
                {
                    obj = lstCarData[0];
                }
            }
            return obj;
        }
        /// <summary>
        /// 以預約時間、據點取出據點內還有的車型
        /// </summary>
        /// <param name="StationID"></param>
        /// <param name="SDate"></param>
        /// <param name="EDate"></param>
        /// <returns></returns>
        public List<ProjectAndCarTypeData> GetStationCarType(string StationID,DateTime SDate,DateTime EDate)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<ProjectAndCarTypeData> lstStation = null;
            int nowCount = 0;
            string SQL = "SELECT PROJID,PRONAME,Price,PRICE_H,[CarBrend],[CarTypeGroupCode] AS CarType,[CarTypeName],[CarTypeImg] As CarTypePic,OperatorICon AS Operator,Score As OperatorScore,Seat FROM  VW_GetFullProjectCollectionOfCarTypeGroup AS VW ";
            SQL += "INNER JOIN TB_Car AS Car ON Car.CarType=VW.CarType AND CarNo NOT IN ( ";
            SQL += "SELECT  CarNo FROM [TB_OrderMain]  ";
            SQL += "WHERE (booking_status<5 AND car_mgt_status<16 AND cancel_status=0) AND  CarNo in (SELECT [CarNo] ";
            SQL += "  FROM [dbo].[TB_Car] WHERE nowStationID =@StationID AND CarType IN ( ";
            SQL += "  SELECT CarType FROM VW_GetFullProjectCollectionOfCarTypeGroup WHERE StationID=@StationID ";
            SQL += "  ) AND available<2 ) ";
            SQL += "							  AND ( ";
            SQL += "								   (start_time between @SD AND @ED)  ";
            SQL += "								OR (stop_time between @SD AND @ED) ";
            SQL += "								OR (@SD BETWEEN start_time AND stop_time) ";
            SQL += "								OR (@ED BETWEEN start_time AND stop_time) ";
            SQL += "								OR (DATEADD(MINUTE,-30,@SD) between start_time AND stop_time) ";
            SQL += "								OR (DATEADD(MINUTE,30,@ED) between start_time AND stop_time) ";
            SQL += "							  ) ";
            SQL += "						 ) ";
            SQL += "WHERE  VW.StationID=@StationID AND SPCLOCK='Z' ";
            SQL += "GROUP BY PROJID,PRONAME,Price,PRICE_H,[CarBrend],[CarTypeGroupCode] ,[CarTypeName],[CarTypeImg] ,OperatorICon ,Score ,Seat ";
            SQL += "ORDER BY Price,PRICE_H ASC ";
            SqlParameter[] para = new SqlParameter[4];
            string term = " ";
            if (string.IsNullOrEmpty(StationID) == false && string.IsNullOrWhiteSpace(StationID) == false)
            {
               
                para[nowCount] = new SqlParameter("@StationID", SqlDbType.VarChar, 20);
                para[nowCount].Value = StationID;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
                para[nowCount] = new SqlParameter("@SD", SqlDbType.DateTime);
                para[nowCount].Value = SDate;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
                para[nowCount] = new SqlParameter("@ED", SqlDbType.DateTime);
                para[nowCount].Value = EDate;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
           
            
            lstStation = GetObjList<ProjectAndCarTypeData>(ref flag, ref lstError, SQL, para, term);
            return lstStation;
        }
        /// <summary>
        /// 取得多據點專案
        /// </summary>
        /// <param name="StationID">據點代碼（1~多個）</param>
        /// <param name="SDate">起日</param>
        /// <param name="EDate">迄日</param>
        /// <param name="CarType">車型群組代碼</param>
        /// <returns></returns>
        public List<StationAndProjectAndCarTypeData> GetStationCarTypeOfMutiStation(string StationID, DateTime SDate, DateTime EDate,string CarType)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<StationAndProjectAndCarTypeData> lstStation = null;
            int nowCount = 0;
            string SQL = "SELECT PROJID,PRONAME,Price,PRICE_H,[CarBrend],[CarTypeGroupCode] AS CarType,[CarTypeName],[CarTypeImg] As CarTypePic,OperatorICon AS Operator,Score As OperatorScore,Seat,iRentStation.StationID,iRentStation.ADDR,iRentStation.Location AS StationName,iRentStation.Longitude,iRentStation.Latitude,iRentStation.Content,PayMode,Car.CarOfArea ";
            SQL += " FROM VW_GetFullProjectCollectionOfCarTypeGroup AS VW ";
            SQL += "INNER JOIN TB_Car AS Car ON Car.CarType=VW.CarType AND CarNo NOT IN ( ";
            SQL += "SELECT  CarNo FROM [TB_OrderMain]  ";
            SQL += "WHERE (booking_status<5 AND car_mgt_status<16 AND cancel_status=0) AND  CarNo in (SELECT [CarNo] ";
            SQL += "  FROM [dbo].[TB_Car] WHERE nowStationID IN (" + StationID + ") AND CarType IN ( ";
            SQL += "  SELECT CarType FROM VW_GetFullProjectCollectionOfCarTypeGroup WHERE StationID IN (" + StationID + ") ";
            SQL += "  ) AND available<2 )  AND ( ";
            SQL += "   (start_time between @SD AND @ED)  ";
            SQL += "	OR (stop_time between @SD AND @ED) ";
            SQL += "	OR (@SD BETWEEN start_time AND stop_time) ";
            SQL += "	OR (@ED BETWEEN start_time AND stop_time) ";
            SQL += "	OR (DATEADD(MINUTE,-30,@SD) between start_time AND stop_time) ";
            SQL += "	OR (DATEADD(MINUTE,30,@ED) between start_time AND stop_time) ";
            SQL += "  ) ";
            SQL += " ) ";
            SQL += " LEFT JOIN TB_iRentStation AS iRentStation ON iRentStation.StationID=VW.StationID ";
            SQL += "WHERE  VW.StationID IN (" + StationID + ") AND SPCLOCK='Z' ";

            SqlParameter[] para = new SqlParameter[5];
            string term = " ";

            para[nowCount] = new SqlParameter("@SD", SqlDbType.DateTime);
            para[nowCount].Value = SDate;
            para[nowCount].Direction = ParameterDirection.Input;
            nowCount++;
            para[nowCount] = new SqlParameter("@ED", SqlDbType.DateTime);
            para[nowCount].Value = EDate;
            para[nowCount].Direction = ParameterDirection.Input;
            nowCount++;

            if (CarType != "")
            {
                SQL += " AND VW.CarTypeGroupCode=@CarType ";
                para[nowCount] = new SqlParameter("@CarType", SqlDbType.VarChar, 20);
                para[nowCount].Value = CarType.ToUpper();
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            SQL += "GROUP BY PROJID,PRONAME,Price,PRICE_H,[CarBrend],[CarTypeGroupCode] ,[CarTypeName],[CarTypeImg] ,OperatorICon ,Score ,Seat,iRentStation.StationID,iRentStation.ADDR,iRentStation.Location ,iRentStation.Longitude ,iRentStation.Latitude,iRentStation.Content,PayMode,CarOfArea ";
            SQL += "ORDER BY Price,PRICE_H ASC ";

            lstStation = GetObjList<StationAndProjectAndCarTypeData>(ref flag, ref lstError, SQL, para, term);
            return lstStation;
        }
        /// <summary>
        /// 路邊租還取得專案
        /// </summary>
        /// <param name="CarNo"></param>
        /// <param name="SDate"></param>
        /// <param name="EDate"></param>
        /// <returns></returns>
        public List<ProjectAndCarTypeData> GetProjectOfAnyRent(string CarNo, DateTime SDate, DateTime EDate)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<ProjectAndCarTypeData> lstStation = null;
            int nowCount = 0;
            #region 改前SQL保留 
            //string SQL = "SELECT PROJID,PRONAME,Price,PRICE_H,[CarBrend],[CarTypeGroupCode] AS CarType,[CarTypeName],[CarTypeImg] As CarTypePic,OperatorICon AS Operator,Score As OperatorScore,Seat, PayMode ";
            //SQL += " FROM VW_GetFullProjectCollectionOfCarTypeGroup AS VW ";
            //SQL += " INNER JOIN  TB_Car AS Car ON Car.CarType=VW.CarType AND VW.StationID=Car.nowStationID  ";
            //SQL += " WHERE CarNo=@CarNo AND SPCLOCK='Z' AND use_flag=1  "; //AND ((PRSTDT BETWEEN @SD AND @ED) AND (PRENDT BETWEEN @SD AND @ED))
            //SQL += "ORDER BY PROJID ASC ";
            #endregion
            string SQL =
                @"SELECT VW.PROJID,
                       VW.PRONAME,
                       VW.Price,
                       VW.PRICE_H,
                       VW.CarBrend,
                       VW.CarTypeGroupCode AS CarType,
                       VW.CarTypeName,
                       VW.CarTypeImg AS CarTypePic,
                       VW.OperatorICon AS Operator,
                       VW.Score AS OperatorScore,
                       VW.Seat,
                       VW.PayMode,
                       irs.Content,
                       Car.CarOfArea
                FROM VW_GetFullProjectCollectionOfCarTypeGroup AS VW
                INNER JOIN TB_Car AS Car ON Car.CarType=VW.CarType
                INNER JOIN TB_iRentStation irs ON irs.StationID = VW.StationID
                AND VW.StationID=Car.nowStationID
                WHERE Car.CarNo = @CarNo
                  AND SPCLOCK='Z'
                  AND VW.use_flag=1
                ORDER BY PROJID ASC";
            SqlParameter[] para = new SqlParameter[4];
            string term = " ";
            if (string.IsNullOrEmpty(CarNo) == false && string.IsNullOrWhiteSpace(CarNo) == false)
            {

                para[nowCount] = new SqlParameter("@CarNo", SqlDbType.VarChar, 20);
                para[nowCount].Value = CarNo;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
                //para[nowCount] = new SqlParameter("@SD", SqlDbType.DateTime);
                //para[nowCount].Value = SDate;
                //para[nowCount].Direction = ParameterDirection.Input;
                //nowCount++;
                //para[nowCount] = new SqlParameter("@ED", SqlDbType.DateTime);
                //para[nowCount].Value = EDate;
                //para[nowCount].Direction = ParameterDirection.Input;
                //nowCount++;
            }

            lstStation = GetObjList<ProjectAndCarTypeData>(ref flag, ref lstError, SQL, para, term);
            return lstStation;
        }
        /// <summary>
        /// 機車路邊租還取得專案
        /// </summary>
        /// <param name="CarNo"></param>
        /// <param name="SDate"></param>
        /// <param name="EDate"></param>
        /// <returns></returns>
        public List<ProjectAndCarTypeDataForMotor> GetProjectOfMotorRent(string CarNo, DateTime SDate, DateTime EDate)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<ProjectAndCarTypeDataForMotor> lstStation = null;
            int nowCount = 0;
            #region 原SQL保留
            //string SQL = "SELECT DISTINCT VW.PROJID,PRONAME,[CarBrend],[CarTypeGroupCode] AS CarType,[CarTypeName],[CarTypeImg] As CarTypePic,OperatorICon AS Operator,Score As OperatorScore,Seat ,PriceByMinutes.[BaseMinutes],PriceByMinutes.[BaseMinutesPrice] AS BasePrice ,PriceByMinutes.[Price] AS PerMinutesPrice ,PriceByMinutes.[MaxPrice] ";
            //SQL += " FROM VW_GetFullProjectCollectionOfCarTypeGroup AS VW ";
            //SQL += " INNER JOIN  TB_Car AS Car ON Car.CarType=VW.CarType AND VW.StationID=Car.nowStationID  ";
            //SQL += " INNER JOIN  TB_PriceByMinutes AS PriceByMinutes ON PriceByMinutes.ProjID=VW.ProjID AND PriceByMinutes.use_flag=1 ";
            //SQL += " WHERE Car.CarNo=@CarNo AND SPCLOCK='Z' AND VW.use_flag=1  "; //AND ((PRSTDT BETWEEN @SD AND @ED) AND (PRENDT BETWEEN @SD AND @ED))
            //SQL += "ORDER BY PROJID ASC ";
            #endregion
            string SQL = @"
            SELECT DISTINCT 
                   VW.PROJID,
		           VW.PRONAME,
		           VW.CarBrend,
		           VW.CarTypeGroupCode AS CarType,
		           VW.CarTypeName,
		           VW.CarTypeImg AS CarTypePic,
		           VW.OperatorICon AS Operator,
		           VW.Score AS OperatorScore,
		           VW.Seat,
		           PriceByMinutes.[BaseMinutes],
		           PriceByMinutes.[BaseMinutesPrice] AS BasePrice,
		           PriceByMinutes.[Price] AS PerMinutesPrice,
		           PriceByMinutes.[MaxPrice],
		           Car.CarOfArea,
                   irs.Content,
				   cs.device3TBA AS Power,
				   ISNULL(cs.deviceRDistance,'') AS RemainingMileage
            FROM VW_GetFullProjectCollectionOfCarTypeGroup AS VW
            INNER JOIN TB_Car AS Car ON Car.CarType=VW.CarType
            AND VW.StationID=Car.nowStationID
            INNER JOIN TB_CarStatus AS cs ON cs.CarNo = Car.CarNo
            INNER JOIN TB_iRentStation irs ON irs.StationID = VW.StationID
            INNER JOIN TB_PriceByMinutes AS PriceByMinutes ON PriceByMinutes.ProjID=VW.ProjID
            AND PriceByMinutes.use_flag=1
            WHERE Car.CarNo=@CarNo
              AND SPCLOCK='Z'
              AND VW.use_flag=1
            ORDER BY PROJID ASC";

            SqlParameter[] para = new SqlParameter[4];
            string term = " ";
            if (string.IsNullOrEmpty(CarNo) == false && string.IsNullOrWhiteSpace(CarNo) == false)
            {

                para[nowCount] = new SqlParameter("@CarNo", SqlDbType.VarChar, 20);
                para[nowCount].Value = CarNo;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
                //para[nowCount] = new SqlParameter("@SD", SqlDbType.DateTime);
                //para[nowCount].Value = SDate;
                //para[nowCount].Direction = ParameterDirection.Input;
                //nowCount++;
                //para[nowCount] = new SqlParameter("@ED", SqlDbType.DateTime);
                //para[nowCount].Value = EDate;
                //para[nowCount].Direction = ParameterDirection.Input;
                //nowCount++;
            }

            lstStation = GetObjList<ProjectAndCarTypeDataForMotor>(ref flag, ref lstError, SQL, para, term);
            return lstStation;
        }
        /// <summary>
        /// 取得電子柵欄
        /// </summary>
        /// <param name="StationID">據點代碼</param>
        /// <returns></returns>
        public List<GetPolygonRawData> GetPolygonRaws(string StationID)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<GetPolygonRawData> lstPolygon = null;
            int nowCount = 0;
            string SQL = "SELECT PolygonMode,Longitude,Latitude";
            SQL += " FROM TB_Polygon  ";

            SQL += " WHERE StationID=@StationID  AND use_flag=1  "; //AND ((PRSTDT BETWEEN @SD AND @ED) AND (PRENDT BETWEEN @SD AND @ED))

            SQL += "ORDER BY PolygonMode ASC ";
            SqlParameter[] para = new SqlParameter[4];
            string term = " ";
            if (string.IsNullOrEmpty(StationID) == false && string.IsNullOrWhiteSpace(StationID) == false)
            {

                para[nowCount] = new SqlParameter("@StationID", SqlDbType.VarChar, 20);
                para[nowCount].Value = StationID;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;

            }
            lstPolygon = GetObjList<GetPolygonRawData>(ref flag, ref lstError, SQL, para, term);
            return lstPolygon;
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
