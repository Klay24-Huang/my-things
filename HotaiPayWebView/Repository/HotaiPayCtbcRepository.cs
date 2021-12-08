using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HotaiPayWebView.Models;
using System.Data.SqlClient;
using System.Data;
using WebCommon;

namespace HotaiPayWebView.Repository
{
    public class HotaiPayCtbcRepository : Repository
    {
        private string _connectionString { set; get; }
        public HotaiPayCtbcRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }
        public List<CreditCardChoose> GetCreditCarLists(string IDNO)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<CreditCardChoose> lstMember = null;
            SqlParameter[] para = new SqlParameter[1];
            int nowCount = 0;
            string term = "";
            string SQL = " select CardType,CardNo from TB_MemberHotaiCard ";

            term += " IDNO=@IDNO";
            para[nowCount] = new SqlParameter("@IDNO", SqlDbType.VarChar, 20);
            para[nowCount].Value = IDNO;
            para[nowCount].Direction = ParameterDirection.Input;
            nowCount++;

            if ("" != term)
            {
                SQL += " WHERE " + term;
            }

            lstMember = GetObjList<CreditCardChoose>(ref flag, ref lstError, SQL, para, term);
            return lstMember;
        }
    }
}