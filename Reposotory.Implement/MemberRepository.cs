using Domain.MemberData;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebCommon;

namespace Reposotory.Implement
{
    /// <summary>
    /// 會員資料相關
    /// </summary>
    public class MemberRepository:BaseRepository
    {
        private string _connectionString { set; get; }
        public MemberRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }
        public RegisterData GetMemberData(string  IDNO)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<RegisterData> lstMember = null;
            RegisterData obj = null;
            int nowCount = 0;
            string SQL = "SELECT [MEMIDNO],[MEMPWD],[MEMCNAME],[MEMTEL],ISNULL([MEMBIRTH],'') AS [MEMBIRTH] ";
            SQL += " ,[MEMCITY] AS MEMAREAID,[MEMADDR],[MEMEMAIL],[CARDNO],[UNIMNO] ";
            SQL += " ,[MEMSENDCD],[CARRIERID],[NPOBAN],[HasCheckMobile],[NeedChangePWD] ";
            SQL += " ,[HasBindSocial],[IrFlag],[PayMode],[HasVaildEMail],[Audit],[RentType] ";
            SQL += " ,Case When [ID_1]=1 And [ID_2] =1 Then B.ID_1 Else 0 End ID_pic ";
            SQL += " ,Case When [CarDriver_1]=1 And [CarDriver_2]=1 Then B.CarDriver_1 Else 0 End DD_pic ";
            SQL += " ,Case When [MotorDriver_1]=1 And [MotorDriver_2]=1 Then B.MotorDriver_1 Else 0 End MOTOR_pic ";
            SQL += " ,ISNULL([Self_1],0) As AA_pic ,ISNULL([Law_Agent],0) As F01_pic";
            SQL += " FROM TB_MemberData A WITH(NOLOCK) ";
            SQL += " Left Join [TB_Credentials] B WITH(NOLOCK) on B.IDNO=A.MEMIDNO ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            if (false==string.IsNullOrWhiteSpace(IDNO))
            {
                term += " MEMIDNO=@IDNO";
                para[nowCount] = new SqlParameter("@IDNO", SqlDbType.VarChar,20);
                para[nowCount].Value = IDNO;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if ("" != term)
            {
                SQL += " WHERE " + term;
            }
     
            lstMember = GetObjList<RegisterData>(ref flag, ref lstError, SQL, para, term);
            if(lstMember!=null)
            {
                if (lstMember.Count > 0)
                {
                    obj = new RegisterData();
                    obj = lstMember[0];
                }
            }
            return obj;
        }
    }
}