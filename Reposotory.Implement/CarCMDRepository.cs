using Domain.CarMachine;
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
    public class CarCMDRepository:BaseRepository
    {
        private string _connectionString;
        public CarCMDRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }
        public CarCMDResponse GetCMDData(string requestId, string method)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<CarCMDResponse> lstCarCMDResponse = null;
            CarCMDResponse obj = null;
            int nowCount = 0;
            string SQL = "SELECT   [requestId],[method],[CmdReply] FROM [dbo].[TB_ReceiveFETCatCMD] ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            if (false == string.IsNullOrWhiteSpace(requestId))
            {
                term += " requestId=@requestId";
                para[nowCount] = new SqlParameter("@requestId", SqlDbType.VarChar, 100);
                para[nowCount].Value = requestId;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (false == string.IsNullOrWhiteSpace(method))
            {
                if (term != "") { term += " AND "; }
                term += " method=@method";
                para[nowCount] = new SqlParameter("@method", SqlDbType.VarChar, 100);
                para[nowCount].Value = method;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if ("" != term)
            {
                SQL += " WITH(NOLOCK) WHERE " + term;
            }
            lstCarCMDResponse = GetObjList<CarCMDResponse>(ref flag, ref lstError, SQL, para, term);
            if (lstCarCMDResponse != null)
            {
                if (lstCarCMDResponse.Count > 0)
                {
                    obj = lstCarCMDResponse[0];
                }
            }
            return obj;
        }
        public BLEInfo GetBLEInfo(string CID)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BLEInfo> lstBLEInfo = null;
            BLEInfo obj = null;
            int nowCount = 0;
            string SQL = "SELECT  extDeviceData5 AS BLE_Device,extDeviceData6 AS BLE_PWD FROM [dbo].[TB_CarStatus] ";
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
            lstBLEInfo = GetObjList<BLEInfo>(ref flag, ref lstError, SQL, para, term);
            if (lstBLEInfo != null)
            {
                if (lstBLEInfo.Count > 0)
                {
                    obj = lstBLEInfo[0];
                }
            }
            return obj;
        }
        /// <summary>
        /// 判斷是否有讀到卡
        /// </summary>
        /// <param name="CID">車機編號</param>
        /// <param name="NowDate">讀卡判斷時間</param>
        /// <returns></returns>
        public bool CheckHasReadCard(string CID,string NowDate,ref string CardNo)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            int nowCount = 0;
            string SQL = "SELECT CID,CardNo FROM TB_ReadCard WITH(NOLOCK) WHERE CID=@CID AND GPSTime BETWEEN @SD AND DATEADD(HOUR,8,GETDATE()) ORDER BY GPSTime DESC";
            SqlParameter[] para = new SqlParameter[2];
            string term = " ";
            if (false == string.IsNullOrWhiteSpace(CID))
            {
               
                para[nowCount] = new SqlParameter("@CID", SqlDbType.VarChar, 10);
                para[nowCount].Value = CID;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (false == string.IsNullOrWhiteSpace(NowDate))
            {
                
                para[nowCount] = new SqlParameter("@SD", SqlDbType.VarChar, 200);
                para[nowCount].Value = NowDate;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            List<ReadCardData> lstReadCard = GetObjList<ReadCardData>(ref flag, ref lstError, SQL, para, term);
            if (lstReadCard != null)
            {
                if (lstReadCard.Count > 0)
                {
                    if (lstReadCard[0].CardNo != "")
                    {
                        CardNo = lstReadCard[0].CardNo;
                        flag = true;
                    }
                }
            }
            return flag;
        }
        public CarCmdData GetCarCMDData(string CarNo,ref bool flag)
        {
            //bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<CarCmdData> lstCarCMDResponse = null;
            CarCmdData obj = null;
            int nowCount = 0;
            string SQL = "SELECT [CarNo],[TSEQNO],[CID],[deviceToken],[IsCens],[IsMotor] FROM [dbo].[TB_CarInfo] ";
            SqlParameter[] para = new SqlParameter[1];
            string term = "";
            if (false == string.IsNullOrWhiteSpace(CarNo))
            {
                term += " CarNo=@CarNo";
                para[nowCount] = new SqlParameter("@CarNo", SqlDbType.VarChar, 100);
                para[nowCount].Value = CarNo.Trim();
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if ("" != term)
            {
                SQL += " WITH(NOLOCK) WHERE " + term;
            }
            lstCarCMDResponse = GetObjList<CarCmdData>(ref flag, ref lstError, SQL, para, term);
            if (lstCarCMDResponse != null)
            {
                if (lstCarCMDResponse.Count > 0)
                {
                    obj = lstCarCMDResponse[0];
                }
            }
            return obj;
        }

        public CarCmdData GetCarCMDDataByCID(string CID, ref bool flag)
        {
            //bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<CarCmdData> lstCarCMDResponse = null;
            CarCmdData obj = null;
            int nowCount = 0;
            string SQL = "SELECT [CarNo],[TSEQNO],[CID],[deviceToken],[IsCens],[IsMotor],[CensFWVer] FROM [dbo].[TB_CarInfo] ";
            SqlParameter[] para = new SqlParameter[1];
            string term = "";
            if (false == string.IsNullOrWhiteSpace(CID))
            {
                term += " CID=@CID";
                para[nowCount] = new SqlParameter("@CID", SqlDbType.VarChar, 20);
                para[nowCount].Value = CID.Trim();
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if ("" != term)
            {
                SQL += " WITH(NOLOCK) WHERE " + term;
            }
            lstCarCMDResponse = GetObjList<CarCmdData>(ref flag, ref lstError, SQL, para, term);
            if (lstCarCMDResponse != null)
            {
                if (lstCarCMDResponse.Count > 0)
                {
                    obj = lstCarCMDResponse[0];
                }
            }
            return obj;
        }
    }
}
