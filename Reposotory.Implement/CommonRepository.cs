using Domain;
using Domain.TB;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using WebCommon;

namespace Reposotory.Implement
{
    public class CommonRepository : ICommonRepository
    {
        private string _connectionString;
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }
        public CommonRepository(string ConnStr)
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
        /// 取出錯誤列表
        /// </summary>
        /// <param name="ErrCode">錯誤代碼</param>
        /// <returns></returns>
        public List<ErrorMessageList> GetErrorList(string ErrCode)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<ErrorMessageList> lstErr = null;
            string SQL = "SELECT ErrCode,ErrMsg  FROM TB_ErrorMessageList ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            int nowCount = 0;
            if (false == string.IsNullOrEmpty(ErrCode))
            {
                term = " ErrCode=@ErrCode";
                para[nowCount] = new SqlParameter("@ErrCode", SqlDbType.VarChar, 30);
                para[nowCount].Value = ErrCode;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }

            if ("" != term)
            {
                SQL += " WITH(NOLOCK)  WHERE " + term;
            }


            lstErr = GetObjList<ErrorMessageList>(ref flag, ref lstError, SQL, para, term);
            return lstErr;
        }

        /// <summary>
        /// 取得單一筆錯誤訊息
        /// </summary>
        /// <param name="ErrCode">錯誤代碼</param>
        /// <returns>ErrorMessageList</returns>
        public ErrorMessageList GetErrorMessage(string ErrCode)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<ErrorMessageList> lstErr = null;
            string SQL = "SELECT ErrCode,ErrMsg  FROM TB_ErrorMessageList ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            int nowCount = 0;
            if (false == string.IsNullOrEmpty(ErrCode))
            {
                term = " ErrCode=@ErrCode";
                para[nowCount] = new SqlParameter("@ErrCode", SqlDbType.VarChar, 30);
                para[nowCount].Value = ErrCode;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }

            if ("" != term)
            {
                SQL += " WITH(NOLOCK)  WHERE " + term;
            }


            lstErr = GetObjList<ErrorMessageList>(ref flag, ref lstError, SQL, para, term);
            ErrorMessageList obj = null;
            if (lstErr != null)
            {
                if (lstErr.Count > 0)
                {
                    obj = new ErrorMessageList()
                    {
                        ErrCode = lstErr[0].ErrCode,
                        ErrMsg = lstErr[0].ErrMsg
                    };
                }
            }
            return obj;
        }
        public ErrorMessageList GetErrorMessage(string ErrCode, int Language)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<ErrorMessageList> lstErr = null;
            string SQL = "SELECT ErrCode,ErrMsg  FROM TB_ErrorMessageMutiLanguage ";
            SqlParameter[] para = new SqlParameter[3];
            string term = "";
            int nowCount = 0;
            if (false == string.IsNullOrEmpty(ErrCode))
            {
                term = " ErrCode=@ErrCode";
                para[nowCount] = new SqlParameter("@ErrCode", SqlDbType.VarChar, 30);
                para[nowCount].Value = ErrCode;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }

            if (term != "")
            {
                term += " AND ";
            }
            term += " LanguageId=@LanguageId";
            para[nowCount] = new SqlParameter("@LanguageId", SqlDbType.Int);
            para[nowCount].Value = Language;
            para[nowCount].Direction = ParameterDirection.Input;
            nowCount++;
            if ("" != term)
            {
                SQL += " WITH(NOLOCK) WHERE " + term;
            }


            lstErr = GetObjList<ErrorMessageList>(ref flag, ref lstError, SQL, para, term);
            ErrorMessageList obj = null;
            if (lstErr != null)
            {
                if (lstErr.Count > 0)
                {
                    obj = new ErrorMessageList()
                    {
                        ErrCode = lstErr[0].ErrCode,
                        ErrMsg = lstErr[0].ErrMsg
                    };
                }
            }
            return obj;
        }
        public List<CityData> GetAllCity()
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<CityData> lstZip = null;
            int nowCount = 0;
            string SQL = "SELECT * FROM TB_City ORDER BY CityID ASC";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
         
            lstZip = GetObjList<CityData>(ref flag, ref lstError, SQL, para, term);
            return lstZip;
        }
        /// <summary>
        ///  取得行政區列表
        /// </summary>
        /// <param name="CityID">縣市代碼</param>
        /// <returns></returns>
        public List<ZipCodeData> GetAllZip(int CityID)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<ZipCodeData> lstZip = null;
            int nowCount = 0;
            string SQL = "SELECT * FROM VW_GetZipCode ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            if (CityID > 0)
            {
                term += " CityID=@CityID";
                para[nowCount] = new SqlParameter("@CityID", SqlDbType.Int);
                para[nowCount].Value = CityID;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if ("" != term)
            {
                SQL += " WITH(NOLOCK) WHERE " + term;
            }
            SQL += " ORDER BY CityID,AreaID ASC";
            lstZip = GetObjList<ZipCodeData>(ref flag, ref lstError, SQL, para, term);
            return lstZip;
        }
        public List<iRentManagerStation> GetAllManageStation()
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<iRentManagerStation> lstManagerStation = null;
            int nowCount = 0;
            string SQL = "SELECT [StationID],[StationName] FROM [TB_ManagerStation] ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
         
            if ("" != term)
            {
                SQL += " WITH(NOLOCK) WHERE " + term;
            }
            SQL += " ORDER BY StationID ASC";
            lstManagerStation = GetObjList<iRentManagerStation>(ref flag, ref lstError, SQL, para, term);
            return lstManagerStation;
        }
        public UPDList GetUpdList()
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<UPDList> lstUPDList = null;
            UPDList ObjUPDList = null;
            int nowCount = 0;
            string SQL = "SELECT TOP 1 [AreaList],[LoveCode],[NormalRent],[Polygon],[Parking]  FROM [dbo].[TB_UPDDataWatchTable] ORDER BY Id DESC";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";

            lstUPDList = GetObjList<UPDList>(ref flag, ref lstError, SQL, para, term);
            if (lstUPDList != null)
            {
                if (lstUPDList.Count > 0)
                {
                    ObjUPDList = lstUPDList[0];
                }
            }
            return ObjUPDList;
        }
        /// <summary>
        /// 取得愛心捐贈碼
        /// </summary>
        /// <returns></returns>
        public List<LoveCodeListData> GetLoveCode()
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<LoveCodeListData> lstLoveCode = null;
            int nowCount = 0;
            string SQL = "SELECT [LoveName],[LoveCode],[LoveShortName],[UNICode] FROM TB_LoveCode ORDER BY Id ASC";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";

            lstLoveCode = GetObjList<LoveCodeListData>(ref flag, ref lstError, SQL, para, term);
            return lstLoveCode;
        }
        /// <summary>
        /// 取得愛心捐贈碼,指定筆數
        /// </summary>
        /// <param name="TakeCount">指定筆數</param>
        /// <returns></returns>
        public List<LoveCodeListData> GetLoveCode(int TakeCount)
        {
            bool flag = false;

            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<LoveCodeListData> lstLoveCode = null;
            string SQL = "SELECT TOP {0} [LoveName],[LoveCode],[LoveShortName],[UNICode] FROM TB_LoveCode ORDER BY Id ASC";

            string repWorld = "";
            if (TakeCount > 0) 
                repWorld = TakeCount.ToString();
            SQL = String.Format(SQL, repWorld);

            SqlParameter[] para = new SqlParameter[2];
            string term = "";

            lstLoveCode = GetObjList<LoveCodeListData>(ref flag, ref lstError, SQL, para, term);
            return lstLoveCode;
        }
        /// <summary>
        /// 取出日期區間內的所有假日
        /// </summary>
        /// <param name="SD">起日</param>
        /// <param name="ED">迄日</param>
        /// <returns></returns>
        public List<Holiday> GetHolidays(string SD, string ED)
        {
            bool flag = false;
            List<Holiday> lstHoliday = null;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string SQL = "SELECT HolidayDate FROM TB_Holiday  ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            flag = (false == string.IsNullOrEmpty(SD));
            if (flag)
            {
                flag = (false == string.IsNullOrEmpty(ED));
            }

            if (flag)
            {
                para[0] = new SqlParameter("@SD", SqlDbType.VarChar, 8);
                para[0].Value = SD;
                para[0].Direction = ParameterDirection.Input;
                para[1] = new SqlParameter("@ED", SqlDbType.VarChar, 8);
                para[1].Value = ED;
                para[1].Direction = ParameterDirection.Input;
                term = " HolidayDate>=@SD AND HolidayDate<=@ED";
                if ("" != term)
                {
                    SQL += " WHERE use_flag=1 AND " + term;
                }
                else
                {
                    SQL += " WHERE user_flag=1 ";
                }
                SQL += "  ORDER BY HolidayDate ASC";

                lstHoliday = GetObjList<Holiday>(ref flag, ref lstError, SQL, para, term);
            }

            return lstHoliday;

        }

    }
}

