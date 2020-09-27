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

    }
}
