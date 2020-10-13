using Domain.CarMachine;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebCommon;

namespace Reposotory.Implement
{
    /// <summary>
    /// 卡片相關
    /// </summary>
    public class CarCardCommonRepository : BaseRepository
    {
        private string _connectionString;
        public CarCardCommonRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }
        /// <summary>
        /// 取得顧客卡
        /// </summary>
        /// <param name="IDNO"></param>
        /// <param name="lstError"></param>
        /// <returns></returns>
        public List<CardList> GetCardListByCustom(string IDNO, ref List<ErrorInfo> lstError)
        {
            bool flag = false;
            List<CardList> lstCardList = null;
            string SQL = "SELECT  CardNO,'C' AS CardType FROM TB_MemberData ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            int nowCount = 0;
            if (IDNO != "")
            {
                term = " MEMIDNO=@IDNO ";
                para[nowCount] = new SqlParameter("@IDNO", SqlDbType.VarChar, 10);
                para[nowCount].Value = IDNO;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }


            if ("" != term)
            {
                SQL += " WITH(NOLOCK) WHERE (" + term + ")";
            }


            lstCardList = GetObjList<CardList>(ref flag, ref lstError, SQL, para, term);
            return lstCardList;
        }
    }
}
