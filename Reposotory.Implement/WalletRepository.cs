using Domain.TB.BackEnd;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCommon;
using System.Data;

namespace Reposotory.Implement
{
    /// <summary>
    /// 和雲錢包相關
    /// </summary>
    public class WalletRepository : BaseRepository
    {
        private string _connectionString { set; get; }
        public WalletRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }

        public List<BE_WalletDetailQuery> GetWalletHistory(string IDNO, string SD, string ED)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_WalletDetailQuery> lst = null;
            SqlParameter[] param = new SqlParameter[3];
            string term = "";
            string SQL = "SELECT TradeType,TradeKey,TradeDate,TradeAMT, ProjNM, MonProPeriod FROM TB_WalletTradeMain Wallet";
            SQL += " LEFT JOIN TB_MonthlyRentUse Rent ON Wallet.TradeKey = CONVERT(varchar,Rent.MonthlyRentId)";
            int nowCount = 0;

            if(string.IsNullOrWhiteSpace(IDNO) == false)
            {
                param[nowCount] = new SqlParameter("@IDNO", SqlDbType.VarChar, 20);
                param[nowCount].Value = IDNO;
                param[nowCount].Direction = ParameterDirection.Input;
                term += " Wallet.IDNO=@IDNO";
                nowCount++;
            }

            if(string.IsNullOrWhiteSpace(SD) == false)
            {
                if (string.IsNullOrEmpty(ED) == false && ED != "")
                {
                    if (term != "") { term += " AND "; }
                    term += " TradeDate between @SD AND @ED";
                    param[nowCount] = new SqlParameter("@SD", SqlDbType.VarChar, 20);
                    param[nowCount].Value = SD;
                    param[nowCount].Direction = ParameterDirection.Input;
                    nowCount++;
                    param[nowCount] = new SqlParameter("@ED", SqlDbType.VarChar, 20);
                    param[nowCount].Value = ED;
                    param[nowCount].Direction = ParameterDirection.Input;
                    nowCount++;
                }
                else
                {
                    if (term != "") { term += " AND "; }
                    term += " TradeDate >= @SD";
                    param[nowCount] = new SqlParameter("@SD", SqlDbType.VarChar, 20);
                    param[nowCount].Value = SD;
                    param[nowCount].Direction = ParameterDirection.Input;
                    nowCount++;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(ED) == false && ED != "")
                {
                    if (term != "") { term += " AND "; }
                    term += " TradeDate <= @ED";
                    param[nowCount] = new SqlParameter("@ED", SqlDbType.VarChar, 20);
                    param[nowCount].Value = ED;
                    param[nowCount].Direction = ParameterDirection.Input;
                    nowCount++;
                }
            }

            if ("" != term)
            {
                SQL += " WHERE " + term;

            }

            lst = GetObjList<BE_WalletDetailQuery>(ref flag, ref lstError, SQL, param, term).OrderBy(i => i.TradeDate).ToList();

            return lst;
        }
    }
}
