using Domain.TB;
using Domain.TB.BackEnd;
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
    /// 車輛狀況相關
    /// </summary>
    public class CarStatusCommon:BaseRepository
    {
        private string _connectionString;
        public CarStatusCommon(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }
        /// <summary>
        /// 取得汽車最新狀態
        /// </summary>
        /// <param name="CID"></param>
        /// <returns></returns>
        public CarInfo GetInfoByCar(string CID)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<CarInfo> lstCar = null;
            CarInfo obj = null;
            int nowCount = 0;
            string SQL = " SELECT [CID],[deviceType],[ACCStatus],[GPSStatus],[GPSTime],[OBDStatus],[GPRSStatus] ";
                   SQL+= "  ,[PowerOnStatus],[CentralLockStatus],[DoorStatus],[LockStatus],[IndoorLightStatus]";
                   SQL += "  ,[SecurityStatus],[Speed],[Volt],[Latitude],[Longitude],[Millage]";
                   SQL += "  ,[extDeviceStatus1],[extDeviceStatus2],[extDeviceData2],[extDeviceData3]";
                   SQL += "  ,[extDeviceData4],[extDeviceData7]";
                   SQL +="  FROM[dbo].[TB_CarStatus]  ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            if (false == string.IsNullOrWhiteSpace(CID))
            {
                term += " CID=@CID";
                para[nowCount] = new SqlParameter("@CID", SqlDbType.VarChar, 10);
                para[nowCount].Value = CID;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if ("" != term)
            {
                SQL += " WITH(NOLOCK) WHERE " + term;
            }

            lstCar = GetObjList<CarInfo>(ref flag, ref lstError, SQL, para, term);
            if (lstCar != null)
            {
                if (lstCar.Count > 0)
                {
                    obj = new CarInfo();
                    obj = lstCar[0];
                }
            }
            return obj;
        }
        /// <summary>
        /// 取得機車最新狀態
        /// </summary>
        /// <param name="CID"></param>
        /// <returns></returns>
        public MotorInfo GetInfoByMotor(string CID)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<MotorInfo> lstMotor = null;
            MotorInfo obj = null;
            int nowCount = 0;
            string SQL = " SELECT [CID],[deviceType],[ACCStatus],[GPSStatus] ";
            SQL += " ,[GPSTime],[GPRSStatus],[Speed],[Volt],[Latitude] ";
            SQL += " ,[Longitude],[Millage],[deviceCourse],[deviceRPM],[device2TBA] ";
            SQL += " ,[device3TBA],[deviceRSOC],[deviceRDistance],[deviceMBA],[deviceMBAA] ";
            SQL += " ,[deviceMBAT_Hi],[deviceMBAT_Lo],[deviceRBA],[deviceRBAA],[deviceRBAT_Hi] ";
            SQL += " ,[deviceRBAT_Lo],[deviceLBA],[deviceLBAA],[deviceLBAT_Hi],[deviceLBAT_Lo] ";
            SQL += " ,[deviceTMP],[deviceCur],[deviceTPS],[deviceVOL],[deviceErr] ";
            SQL += " ,[deviceALT],[deviceGx],[deviceGy],[deviceGz],[deviceBLE_Login] ";
            SQL += " ,[deviceBLE_BroadCast],[devicePwr_Mode],[deviceReversing],[devicePut_Down],[devicePwr_Relay] ";
            SQL += " ,[deviceStart_OK],[deviceHard_ACC],[deviceEMG_Break],[deviceSharp_Turn],[deviceBat_Cover] ";
            SQL +=" ,[deviceLowVoltage],[extDeviceStatus1],[extDeviceData5],[extDeviceData6]";
            SQL += "  FROM [dbo].[TB_CarStatus]  ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            if (false == string.IsNullOrWhiteSpace(CID))
            {
                term += " CID=@CID";
                para[nowCount] = new SqlParameter("@CID", SqlDbType.VarChar, 10);
                para[nowCount].Value = CID;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if ("" != term)
            {
                SQL += " WITH(NOLOCK) WHERE " + term;
            }

            lstMotor = GetObjList<MotorInfo>(ref flag, ref lstError, SQL, para, term);
            if (lstMotor != null)
            {
                if (lstMotor.Count > 0)
                {
                    obj = new MotorInfo();
                    obj = lstMotor[0];
                }
            }
            return obj;
        }
        /// <summary>
        ///後台使用
        /// </summary>
        /// <param name="CarNo"></param>
        /// <param name="StationID"></param>
        /// <param name="ShowType">顯示類型
        /// <para>2:全部</para>
        /// <para>1:僅顯示有回應</para>
        /// <para>0:僅顯示無回應</para>
        /// </param>
        /// <param name="terms">篩選條件
        /// <para>0:低電量機車(3TBA)</para>
        /// <para>1:低電量機車(2TBA)</para>
        /// <para>2:發動</para>
        /// <para>3:電池蓋開啟</para>
        /// <para>4:一小時無回應</para>
        /// <para>5:無回應</para>
        /// </param>
        /// <returns></returns>
        public List<BE_CarDashBoardData> GetDashBoard(string CarNo,string StationID,int ShowType,List<string> terms)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_CarDashBoardData> lstDashBoard = null;
            DateTime OneHour = DateTime.Now.AddHours(-1);
            DateTime noResonse = DateTime.Now.AddHours(-2);
            bool hasNoResponse = false,hasOneNoResponse=false;

            int nowCount = 0;
            string SQL = " SELECT * FROM VW_BE_GetCarDashBoardData ";
     
            SqlParameter[] para = new SqlParameter[10];
            string term = "";
            if (false == string.IsNullOrWhiteSpace(CarNo))
            {
                term += " CarNo=@CarNo";
                para[nowCount] = new SqlParameter("@CarNo", SqlDbType.VarChar, 10);
                para[nowCount].Value = CarNo;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (false == string.IsNullOrWhiteSpace(StationID))
            {
                term += (term == "") ? "" : " AND ";
                term += " StationID=@StationID";
                para[nowCount] = new SqlParameter("@StationID", SqlDbType.VarChar, 10);
                para[nowCount].Value = StationID;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (ShowType < 2)
            {
                if (ShowType == 0)
                {
                    term += (term == "") ? "" : " AND ";
                    term += " LastUpdate<@LastUpdate";
                    para[nowCount] = new SqlParameter("@LastUpdate", SqlDbType.DateTime);
                    para[nowCount].Value = noResonse.ToString("yyyy-MM-dd HH:mm:ss");
                    para[nowCount].Direction = ParameterDirection.Input;
                    nowCount++;
                    hasNoResponse = true;
                }
                else
                {
                    term += (term == "") ? "" : " AND ";
                    term += " LastUpdate<=@LastUpdate";
                    para[nowCount] = new SqlParameter("@LastUpdate", SqlDbType.DateTime);
                    para[nowCount].Value = OneHour.ToString("yyyy-MM-dd HH:mm:ss");
                    para[nowCount].Direction = ParameterDirection.Input;
                    nowCount++;
                    hasOneNoResponse = true;
                }
            }
            int TermsLen = terms.Count();
            if (TermsLen > 0)
            {
                bool hasAddisMotor = false;
                for(int i = 0; i < TermsLen; i++)
                {
                    switch (terms[i])
                    {
                        case "0":
                            if (hasAddisMotor == false)
                            {
                                term += (term == "") ? "" : " AND ";
                                term += " isMoto=1 ";
                                hasAddisMotor = true;
                            }
                            term += (term == "") ? "" : " AND ";
                            term += " TBA3<30 ";

                     
                            break;
                        case "1":
                            if (hasAddisMotor == false)
                            {
                                term += (term == "") ? "" : " AND ";
                                term += " isMoto=1 ";
                                hasAddisMotor = true;
                            }
                            term += (term == "") ? "" : " AND ";
                            term += " TBA2<30 ";
                            break;
                        case "2":
                            term += (term == "") ? "" : " AND ";
                            term += " ACCStatus=1 ";
                            break;
                        case "3":
                            if (hasAddisMotor == false)
                            {
                                term += (term == "") ? "" : " AND ";
                                term += " isMoto=1 ";
                                hasAddisMotor = true;
                            }
                            term += (term == "") ? "" : " AND ";
                            term += " BatCover=1 ";
                            break;
                        case "4":
                            if (hasOneNoResponse == false)
                            {
                                term += (term == "") ? "" : " AND ";
                                term += " LastUpdate<=@LastUpdate";
                                para[nowCount] = new SqlParameter("@LastUpdate", SqlDbType.DateTime);
                                para[nowCount].Value = OneHour.ToString("yyyy-MM-dd HH:mm:ss");
                                para[nowCount].Direction = ParameterDirection.Input;
                                nowCount++;
                                hasOneNoResponse = true;
                            }
                            break;
                        case "5":
                            if (hasNoResponse == false)
                            {
                                term += (term == "") ? "" : " AND ";
                                term += " LastUpdate<@LastUpdate";
                                para[nowCount] = new SqlParameter("@LastUpdate", SqlDbType.DateTime);
                                para[nowCount].Value = noResonse.ToString("yyyy-MM-dd HH:mm:ss");
                                para[nowCount].Direction = ParameterDirection.Input;
                                nowCount++;
                                hasNoResponse = true;
                            }
                            break;
                    }
                }
            }
            if ("" != term)
            {
                SQL += " WITH(NOLOCK) WHERE " + term;
            }

            lstDashBoard = GetObjList<BE_CarDashBoardData>(ref flag, ref lstError, SQL, para, term);
      
            return lstDashBoard;
        }

        public List<BE_CarSettingData> GetCarSettingData(string CarNo, string StationID, int ShowType)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_CarSettingData> lstCarSettingData = null;


            int nowCount = 0;
            string SQL = " SELECT StationID,StationName,CarNo FROM VW_BE_GetCarDashBoardData ";

            SqlParameter[] para = new SqlParameter[10];
            string term = "";
            if (false == string.IsNullOrWhiteSpace(CarNo))
            {
                term += " CarNo=@CarNo";
                para[nowCount] = new SqlParameter("@CarNo", SqlDbType.VarChar, 10);
                para[nowCount].Value = CarNo;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (false == string.IsNullOrWhiteSpace(StationID))
            {
                term += (term == "") ? "" : " AND ";
                term += " StationID=@StationID";
                para[nowCount] = new SqlParameter("@StationID", SqlDbType.VarChar, 10);
                para[nowCount].Value = StationID;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (ShowType < 3)
            {
                term += (term == "") ? "" : " AND ";
                term += " NowStatus=@NowStatus";
                para[nowCount] = new SqlParameter("@NowStatus", SqlDbType.Int);
                para[nowCount].Value = ShowType;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if ("" != term)
            {
                SQL += " WITH(NOLOCK) WHERE " + term;
            }

            lstCarSettingData = GetObjList<BE_CarSettingData>(ref flag, ref lstError, SQL, para, term);

            return lstCarSettingData;

        }
        /// <summary>
        /// 車輛資料管理
        /// </summary>
        /// <param name="CarNo">車號</param>
        /// <param name="StationID">據點</param>
        /// <param name="ShowType">
        /// 
        /// </param>
        /// <returns></returns>
        public List<BE_GetPartOfCarDataSettingData> GetCarDataSettingData(string CarNo, string StationID, int ShowType)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_GetPartOfCarDataSettingData> lstCarDataSettingData = null;


            int nowCount = 0;
            string SQL = " SELECT * FROM VW_BE_GetPartOfCarDataSetting ";

            SqlParameter[] para = new SqlParameter[10];
            string term = "";
            if (false == string.IsNullOrWhiteSpace(CarNo))
            {
                term += " CarNo like @CarNo";
                para[nowCount] = new SqlParameter("@CarNo", SqlDbType.VarChar, 10);
                para[nowCount].Value = string.Format("%{0}%",CarNo);
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (false == string.IsNullOrWhiteSpace(StationID))
            {
                term += (term == "") ? "" : " AND ";
                term += " (StationID like @StationID OR nowStationID like @StationID) ";
                para[nowCount] = new SqlParameter("@StationID", SqlDbType.VarChar, 10);
                para[nowCount].Value = string.Format("%{0}%", StationID);
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (ShowType < 3)
            {
                term += (term == "") ? "" : " AND ";
                term += " NowStatus=@NowStatus";
                para[nowCount] = new SqlParameter("@NowStatus", SqlDbType.Int);
                para[nowCount].Value = ShowType;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if ("" != term)
            {
                SQL += " WITH(NOLOCK) WHERE " + term;
            }

            lstCarDataSettingData = GetObjList<BE_GetPartOfCarDataSettingData>(ref flag, ref lstError, SQL, para, term);

            return lstCarDataSettingData;

        }
        /// <summary>
        /// 查詢車輛明細
        /// </summary>
        /// <param name="CarNo"></param>
        /// <returns></returns>
        public BE_GetCarDetail GetCarDataSettingDetail(string CarNo)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_GetCarDetail> lstCarDataSettingData = null;
            BE_GetCarDetail obj = null;


            int nowCount = 0;
            string SQL = " SELECT * FROM VW_GetCarDetail ";

            SqlParameter[] para = new SqlParameter[10];
            string term = "";
            if (false == string.IsNullOrWhiteSpace(CarNo))
            {
                term += " CarNo like @CarNo";
                para[nowCount] = new SqlParameter("@CarNo", SqlDbType.VarChar, 10);
                para[nowCount].Value = string.Format("%{0}%", CarNo);
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
          
        
            if ("" != term)
            {
                SQL += " WITH(NOLOCK) WHERE " + term;
            }

            lstCarDataSettingData = GetObjList<BE_GetCarDetail>(ref flag, ref lstError, SQL, para, term);
            if (lstCarDataSettingData != null)
            {
                if (lstCarDataSettingData.Count > 0)
                {
                    obj = new BE_GetCarDetail();
                    obj = lstCarDataSettingData[0];
                }
            }
            return obj;

        }

    }
}
