using Domain.TB;
using Domain.TB.BackEnd;
using System.Collections.Generic;
using System.Data.SqlClient;
using WebCommon;

namespace Reposotory.Implement.BackEnd
{
    /// <summary>
    /// 後台使用據點相關
    /// </summary>
    public class StationRepository:BaseRepository
    {
        private string _ConnectionString;
        public StationRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }
        /// <summary>
        /// 取iRent據點
        /// </summary>
        /// <param name="showAll"></param>
        /// <returns></returns>
        public IEnumerable<iRentStationBaseInfo> GetPartOfStation(bool showAll)
        {
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<iRentStationBaseInfo> lstStation = null;
            bool flag = false;
            string SQL = "SELECT StationID,ISNULL(TRANSLATE(Location,'()','[]'),'') AS StationName FROM TB_iRentStation ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            lstStation = GetObjList<iRentStationBaseInfo>(ref flag, ref lstError, SQL, para, term);
            return lstStation;
        }
        /// <summary>
        /// 取得車輛列表
        /// </summary>
        /// <param name="showAll"></param>
        /// <returns></returns>
        public IEnumerable<iRentCarBase> GetPartOfCar(bool showAll)
        {
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<iRentCarBase> lstStation = null;
            bool flag = false;
            string SQL = "SELECT CarNo FROM TB_Car ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            lstStation = GetObjList<iRentCarBase>(ref flag, ref lstError, SQL, para, term);
            return lstStation;
        }
    }
}