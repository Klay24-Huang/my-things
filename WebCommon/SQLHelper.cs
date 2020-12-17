using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace WebCommon
{
    public class SQLHelper<InputPara, OutputPara>
         where InputPara : class
         where OutputPara : class
    {
        private string _connectionString;
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }
        private const Int32 _LimitDbParameter = 100;
        private const string QuerySpParameter = "SELECT DISTINCT PARAMETER_NAME,DATA_TYPE,ISNULL(CHARACTER_MAXIMUM_LENGTH,0) as 'LENGTH',PARAMETER_MODE,ORDINAL_POSITION "
                                                + "FROM INFORMATION_SCHEMA.PARAMETERS WITH(NOLOCK) "
                                                + "WHERE SPECIFIC_NAME=@SPName "
                                                + "ORDER BY ORDINAL_POSITION ASC ";
        private SqlConnection sqlConnection;
        private SqlCommand sqlCommand;
        public SQLHelper(string _connectString)
        {
            ConnectionString = _connectString;
            sqlConnection = new SqlConnection(ConnectionString);
        }
        /// <summary>
        /// 產生sqlParameter
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="dbtype"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public SqlParameter CreateParameter(string field, object value, DbType dbtype, ParameterDirection direction)
        {
            SqlParameter p = new SqlParameter();
            p.ParameterName = field;
            p.Value = value;
            p.DbType = dbtype;
            p.Direction = direction;
            return p;
        }
        public bool ConnectionDB()
        {
            bool flag = false;

            if (this.sqlConnection.State == ConnectionState.Open)
            {
                this.sqlConnection.Close();
            }
            this.sqlConnection.Open();
            return flag;
        }
        public void closeDB()
        {
            if (this.sqlConnection.State == ConnectionState.Open)
            {
                this.sqlConnection.Close();
            }
        }
        private SqlParameter[] CreateParameter(InputPara objInput, OutputPara objOutput, ref string SQL)
        {
            SqlParameter[] tmpParameter = new SqlParameter[_LimitDbParameter];
            int i = 0;
            foreach (var prop in objInput.GetType().GetProperties())
            {
                SqlParameter p = CreateParameter("@" + prop.Name, GetValue(objInput, prop.Name, true), GetDbType(objInput, prop.Name), ParameterDirection.Input);
                tmpParameter[i] = p;
                SQL += "@" + prop.Name + ",";
                i++;
            }
            SQL = SQL.Substring(0, SQL.Length - 1);
            foreach (var prop in objOutput.GetType().GetProperties())
            {
                SqlParameter p;
                if (SQL.IndexOf(prop.Name) == -1)
                {
                    if (prop.Name == ParameterDirection.ReturnValue.ToString("G") || prop.Name == "Error")
                    {
                        p = CreateParameter("@" + prop.Name, 0, GetDbType(objOutput, prop.Name), ParameterDirection.ReturnValue);
                        SQL = "DECLARE " + "@" + prop.Name + " int; " + " EXEC " + "@" + prop.Name + "=" + SQL;
                    }
                    else
                    {
                        p = CreateParameter("@" + prop.Name, "", GetDbType(objOutput, prop.Name), ParameterDirection.Output);
                        SQL += ",@" + prop.Name + " OUTPUT";
                    }

                tmpParameter[i] = p;
                i++;
                }
            }

            Array.Resize(ref tmpParameter, i);
            return tmpParameter;
        }
        private SqlParameter[] GenerateSQL(ref StringBuilder SQL, string SPName, InputPara objInput, OutputPara objOutput)
        {
            this.ConnectionDB();
            sqlCommand = new SqlCommand(QuerySpParameter, sqlConnection);
            SqlParameter sqlPara = new SqlParameter("@SPName", SqlDbType.VarChar, 100);
            sqlPara.Value = SPName;
            sqlCommand.Parameters.Add(sqlPara);
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            SQL.Append("EXEC " + SPName + " ");
            SqlParameter[] tmpParameter = new SqlParameter[_LimitDbParameter];
            System.Reflection.PropertyInfo[] inputProp = objInput.GetType().GetProperties();
            System.Reflection.PropertyInfo[] outputProp = objOutput.GetType().GetProperties();
            int inputLen = inputProp.Length;
            int outputLen = outputProp.Length;
            int nowCount = 0;
            string suff = "";
            while (sqlDataReader.Read())
            {
                SqlParameter p;
                string paraName = sqlDataReader["PARAMETER_NAME"].ToString().Remove(0, 1);
                SqlDbType dtype = GetDbType(sqlDataReader["DATA_TYPE"].ToString());
                int length = int.Parse(sqlDataReader["LENGTH"].ToString());
                ParameterDirection PARAMETER_MODE = getParaDir(sqlDataReader["PARAMETER_MODE"].ToString());
                for (int i = 0; i < inputLen; i++)
                {
                    if (inputProp[i].Name == paraName)
                    {
                        p = new SqlParameter("@" + paraName, dtype, length);
                        p.Value = GetValue(objInput, inputProp[i].Name, true);
                        p.Direction = PARAMETER_MODE;
                        tmpParameter[nowCount] = p;
                        nowCount++;
                        break;
                    }
                }
                if (ParameterDirection.InputOutput == PARAMETER_MODE)
                {
                    suff = " OUTPUT ";
                    for (int i = 0; i < outputLen; i++)
                    {
                        if (outputProp[i].Name == paraName)
                        {
                            p = new SqlParameter("@" + paraName, dtype, length);
                            p.Value = "0";
                            p.Direction = PARAMETER_MODE;
                            tmpParameter[nowCount] = p;
                            nowCount++;
                            break;
                        }
                    }

                }
                else
                {
                    suff = "";
                }
                SQL.Append("@" + paraName + " " + suff + ",");
            }
            if (SQL.Length > 0)
            {
                SQL = SQL.Remove(SQL.Length - 1, 1);
            }
            sqlDataReader.Dispose();
            closeDB();
            Array.Resize(ref tmpParameter, nowCount);
            return tmpParameter;
        }
        private SqlParameter[] GenerateSQL(ref StringBuilder SQL, string SPName, InputPara objInput, OutputPara objOutput, string returnPara, SqlDbType returnParaType, int returnParaTypeLen)
        {
            this.ConnectionDB();
            sqlCommand = new SqlCommand(QuerySpParameter, sqlConnection);
            SqlParameter sqlPara = new SqlParameter("@SPName", SqlDbType.VarChar, 100);
            sqlPara.Value = SPName;
            sqlCommand.Parameters.Add(sqlPara);

            SQL.Append("DECLARE @" + returnPara + " INT;");
            SQL.Append("EXEC @" + returnPara + "=" + SPName + " ");
            SqlParameter[] tmpParameter = new SqlParameter[_LimitDbParameter];
            System.Reflection.PropertyInfo[] inputProp = objInput.GetType().GetProperties();
            System.Reflection.PropertyInfo[] outputProp = objOutput.GetType().GetProperties();
            int inputLen = inputProp.Length;
            int outputLen = outputProp.Length;
            int nowCount = 0;
            string suff = "";
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            while (sqlDataReader.Read())
            {
                if (SQL.ToString().IndexOf(sqlDataReader["PARAMETER_NAME"].ToString() + " ") == -1)
                {

                    SqlParameter p;
                    string paraName = sqlDataReader["PARAMETER_NAME"].ToString().Remove(0, 1);
                    SqlDbType dtype = GetDbType(sqlDataReader["DATA_TYPE"].ToString());
                    int length = int.Parse(sqlDataReader["LENGTH"].ToString());
                    ParameterDirection PARAMETER_MODE = getParaDir(sqlDataReader["PARAMETER_MODE"].ToString());
                    for (int i = 0; i < inputLen; i++)
                    {
                        if (inputProp[i].Name == paraName)
                        {
                            p = new SqlParameter("@" + paraName, dtype, length);
                            p.Value = GetValue(objInput, inputProp[i].Name, true);
                            p.Direction = PARAMETER_MODE;
                            tmpParameter[nowCount] = p;
                            nowCount++;
                            break;
                        }
                    }
                    if (ParameterDirection.InputOutput == PARAMETER_MODE)
                    {
                        for (int i = 0; i < outputLen; i++)
                        {
                            if (outputProp[i].Name == paraName)
                            {
                                p = new SqlParameter("@" + paraName, dtype, length);
                                p.Value = GetValue(objOutput, outputProp[i].Name, true);
                                p.Direction = PARAMETER_MODE;
                                tmpParameter[nowCount] = p;
                                nowCount++;
                                suff = " OUTPUT ";
                                break;
                            }
                        }

                    }
                    else
                    {
                        suff = "";
                    }

                    SQL.Append("@" + paraName + " " + suff + ",");
                }
            }

            if (SQL.Length > 0)
            {
                SQL = SQL.Remove(SQL.Length - 1, 1);
            }
            SqlParameter returnP;
            if (returnParaTypeLen == 0)
            {
                returnP = new SqlParameter("@" + returnPara, returnParaType);
                returnP.Value = 0;
            }
            else
            {
                returnP = new SqlParameter("@" + returnPara, returnParaType, returnParaTypeLen);
                returnP.Value = " ";
            }


            returnP.Direction = ParameterDirection.ReturnValue;
            tmpParameter[nowCount] = returnP;
            nowCount++;
            sqlDataReader.Dispose();
            closeDB();
            Array.Resize(ref tmpParameter, nowCount);
            return tmpParameter;
        }
        private ParameterDirection getParaDir(string paraDirection)
        {
            ParameterDirection returnDir = ParameterDirection.Input;
            switch (paraDirection)
            {

                case "INOUT":
                    returnDir = ParameterDirection.InputOutput;
                    break;
                case "OUT":
                    returnDir = ParameterDirection.Output;
                    break;
            }
            return returnDir;
        }
        /// <summary>
        /// 取物件內屬性資料
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        private object GetValue(object instance, string pName, bool IsDBNull = false)
        {
            object returnValue;

            if (IsDBNull)
            {
                returnValue = DBNull.Value;
            }
            else
            {
                returnValue = "";
            }

            Object property = instance.GetType().GetProperty(pName).GetValue(instance, null); ;


            if (property != null)
            {
                switch (property.ToString())
                {
                    case "DateTime":
                        if (((DateTime)property).ToString("yyyyMMdd") == "00010101")
                        {
                            returnValue = DBNull.Value;
                        }
                        else
                        {
                            returnValue = ((DateTime)property).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        break;
                    case "float":
                        returnValue = ((float)property);
                        break;
                    case "double":
                        returnValue = ((double)property);
                        break;
                    case "int":
                        returnValue = ((int)property);
                        break;
                }
                if (property is DateTime)
                {
                    if (((DateTime)property).ToString("yyyyMMdd") == "00010101")
                    {
                        returnValue = DBNull.Value;
                    }
                    else
                    {
                        returnValue = ((DateTime)property).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }
                else
                {
                    if (property is SecureString && property != null)
                    {
                        returnValue = SecureStringToString(property as SecureString);
                    }
                    else
                    {
                        returnValue = property.ToString();
                    }
                }
            }
            return returnValue;
        }
        private void GetSQLReturnValue(SqlParameterCollection sqlParam, ref OutputPara objOutput)
        {
            System.Reflection.PropertyInfo[] outputProp = objOutput.GetType().GetProperties();
            int outputLen = outputProp.Length;
            for (int i = 0; i < outputLen; i++)
            {
                SetValue(objOutput, outputProp[i].Name, sqlParam["@" + outputProp[i].Name].Value);
            }
        }
        /// <summary>
        /// 依照SQL回傳的dataType產生對應的DBType
        /// </summary>
        /// <param name="typeName">SQL回傳的DataType</param>
        /// <returns></returns>
        private SqlDbType GetDbType(string typeName)
        {
            SqlDbType ReturnDBType = SqlDbType.VarChar;
            switch (typeName)
            {
                case "bigint":
                    ReturnDBType = SqlDbType.BigInt;
                    break;
                case "binary":
                    ReturnDBType = SqlDbType.Binary;
                    break;
                case "bit":
                    ReturnDBType = SqlDbType.Bit;
                    break;
                case "char":
                    ReturnDBType = SqlDbType.Char;
                    break;
                case "date":
                    ReturnDBType = SqlDbType.Date;
                    break;
                case "datetime":
                    ReturnDBType = SqlDbType.DateTime;
                    break;
                case "datetime2":
                    ReturnDBType = SqlDbType.DateTime2;
                    break;
                case "datetimeoffset":
                    ReturnDBType = SqlDbType.DateTimeOffset;
                    break;
                case "decimal":
                    ReturnDBType = SqlDbType.Decimal;
                    break;
                case "float":
                    ReturnDBType = SqlDbType.Float;
                    break;
                case "int":
                    ReturnDBType = SqlDbType.Int;
                    break;
                case "money":
                    ReturnDBType = SqlDbType.Money;
                    break;
                case "nchar":
                    ReturnDBType = SqlDbType.NChar;
                    break;
                case "ntext":
                    ReturnDBType = SqlDbType.NText;
                    break;
                case "numeric":
                    ReturnDBType = SqlDbType.Decimal;
                    break;
                case "nvarchar":
                    ReturnDBType = SqlDbType.NVarChar;
                    break;
                case "real":
                    ReturnDBType = SqlDbType.Real;
                    break;
                case "text":
                    ReturnDBType = SqlDbType.Text;
                    break;
                case "time":
                    ReturnDBType = SqlDbType.Time;
                    break;
                case "tinyint":
                    ReturnDBType = SqlDbType.TinyInt;
                    break;
                case "uniqueidentifier":
                    ReturnDBType = SqlDbType.UniqueIdentifier;
                    break;
                case "varbinary":
                    ReturnDBType = SqlDbType.VarBinary;
                    break;
                case "varchar":
                    ReturnDBType = SqlDbType.VarChar;
                    break;
                case "xml":
                    ReturnDBType = SqlDbType.Xml;
                    break;
            }
            return ReturnDBType;
        }
        private DbType GetDbType(object instance, string pName)
        {
            DbType ReturnDBType = DbType.AnsiString;
            string TypeName = "";
            if (instance is ExpandoObject)
            {
                TypeName = DbType.String.ToString("G");
            }
            else
            {
                TypeName = instance.GetType().GetProperty(pName).PropertyType.Name;
            }

            //System.TypeCode
            switch (TypeName)
            {
                case "bool":
                    //// 摘要:
                    ////     表示 true 或 false 的布林值的簡單型別。
                    ReturnDBType = DbType.Boolean;
                    break;

                case "Boolean":
                    //// 摘要:
                    ////     表示 true 或 false 的布林值的簡單型別。
                    ReturnDBType = DbType.Boolean;
                    break;

                case "Date":
                    //// 摘要:
                    ////     代表日期值的型別。
                    ReturnDBType = DbType.Date;
                    break;

                case "DateTime":
                    //// 摘要:
                    ////     表示日期和時間值的型別。
                    ReturnDBType = DbType.DateTime;
                    break;

                case "Decimal":
                    //// 摘要:
                    ////     簡單型別，表示具有 28-29 個有效位數、從 1.0 x 10 -28 到大約 7.9 x 10 -28 的數值範圍。
                    ReturnDBType = DbType.Decimal;
                    break;

                case "Double":
                    //// 摘要:
                    ////     浮點型別，表示具有 15-16 位數精確度、從 5.0 x 10 -324 到大約 1.7 x 10 308 的數值範圍。
                    ReturnDBType = DbType.Double;
                    break;
                case "float":
                    //// 摘要:
                    ////     浮點型別
                    ReturnDBType = DbType.Single;
                    break;
                case "int":
                    //// 摘要:
                    ////     表示帶正負號的 16 位元整數的整數型別，其值介於 -2147483647 和 2147483647 之間。
                    ReturnDBType = DbType.Int32;
                    break;

                case "Int16":
                    //// 摘要:
                    ////     表示帶正負號的 16 位元整數的整數型別，其值介於 -32768 和 32767 之間。
                    ReturnDBType = DbType.Int16;
                    break;

                case "Int32":
                    //// 摘要:
                    ////     表示帶正負號的 32 位元整數的整數型別，其值介於 -2147483648 和 2147483647 之間。
                    ReturnDBType = DbType.Int32;
                    break;

                case "Int64":
                    //// 摘要:
                    ////     表示帶正負號的 64 位元整數的整數型別，其值介於 -9223372036854775808 和 9223372036854775807 之間。
                    ReturnDBType = DbType.Int64;
                    break;

                case "Single":
                    //// 摘要:
                    ////     浮點型別，表示具有 7 位數精確度、從 1.5 x 10 -45 到大約 3.4 x 10 38 的數值範圍。
                    ReturnDBType = DbType.Single;
                    break;

                case "String":
                    //// 摘要:
                    ////     表示 Unicode 字元字串的型別。
                    ReturnDBType = DbType.String;
                    break;

                case "Char":
                    //// 摘要:
                    ////     表示 Unicode 字元字串的型別。
                    ReturnDBType = DbType.String;
                    break;

                case "Byte":
                    //// 摘要:
                    ////     表示 Unicode 字元字串的型別。
                    ReturnDBType = DbType.Byte;
                    break;

                case "Guid":
                    //// 摘要:
                    ////     全域唯一識別項 (或 GUID)。
                    ReturnDBType = DbType.Guid;
                    break;

                case "Object":
                    //// 摘要:
                    ////     表示未明確由其他 DbType 值表示的任何參考或實值型別之一般型別。
                    ReturnDBType = DbType.Object;
                    break;

                case "SecureString":
                    ReturnDBType = DbType.String;
                    break;

                default:
                    ReturnDBType = DbType.String;
                    break;
            }

            return ReturnDBType;
        }
        /// <summary>
        /// 設定物件內屬性資料
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="pName"></param>
        /// <param name="pValue"></param>
        private void SetValue(OutputPara instance, string pName, object pValue)
        {
            string type = instance.GetType().GetProperty(pName).PropertyType.ToString().Replace("System.", "");
            switch (type)
            {
                case "String":
                    pValue = pValue.ToString();
                    break;
                case "Int32":
                    pValue = Convert.ToInt32(pValue);
                    break;
                case "Int64":
                    pValue = Convert.ToInt64(pValue);
                    break;
                case "Int16":
                    pValue = Convert.ToInt16(pValue);
                    break;
                case "Double":
                    pValue = Convert.ToDouble(pValue);
                    break;
                case "Single":
                    pValue = Convert.ToSingle(pValue);
                    break;
            }
            instance.GetType().GetProperty(pName).SetValue(instance, pValue, null);

        }
        /// <summary>
        /// 將SecureString轉為String
        /// <para>http://stackoverflow.com/questions/818704/how-to-convert-securestring-to-system-string</para>
        /// <para>Marshal https://msdn.microsoft.com/en-us/library/system.runtime.interopservices.marshal.aspx </para>
        /// </summary>
        /// <param name="value">安全字串</param>
        /// <returns></returns>
        private String SecureStringToString(SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }
        public bool ExeuteSP<dsOut>(string SQL, InputPara objInput, ref OutputPara objOutput, ref List<dsOut> lstOutRow, ref DataSet ds, ref List<ErrorInfo> lstErrInfo)
            where dsOut : class
        {
            bool flag = false;
            try
            {
                StringBuilder tmpSQL = new StringBuilder();
                SqlParameter[] sqlPara = GenerateSQL(ref tmpSQL, SQL, objInput, objOutput, "Error", SqlDbType.Int, 0); //CreateParameter(objInputPara, objOutputPara,ref SQL);
                sqlCommand = new SqlCommand();
                for (int i = 0; i < sqlPara.Length; i++)
                {
                    sqlCommand.Parameters.Add(sqlPara[i]);
                }
                ConnectionDB();
                sqlCommand.CommandText = tmpSQL.ToString();
                sqlCommand.Connection = this.sqlConnection;
                DataTable table = new DataTable();
                table.Load(sqlCommand.ExecuteReader());
                ds.Tables.Add(table);
                flag = getPropertyNames<dsOut>(ds, ref lstOutRow);
                GetSQLReturnValue(sqlCommand.Parameters, ref objOutput);

                flag = true;
            }
            catch (Exception ex)
            {
                lstErrInfo.Add(new ErrorInfo() { ErrorCode = ex.HResult.ToString(), ErrorMsg = ex.Message });
            }
            finally
            {
                closeDB();
            }
            return flag;
        }
        public async Task<Dictionary<string, object>> ExeuteSPAsync<dsOut>(string SQL, InputPara objInput, OutputPara objOutput, List<dsOut> lstOutRow, DataSet ds, List<ErrorInfo> lstErrInfo, bool isaysnc)
            where dsOut : class
        {
            Dictionary<string, object> output = new Dictionary<string, object>();
            bool flag = false;
            try
            {
                StringBuilder tmpSQL = new StringBuilder();
                SqlParameter[] sqlPara = GenerateSQL(ref tmpSQL, SQL, objInput, objOutput, "Error", SqlDbType.Int, 0); //CreateParameter(objInputPara, objOutputPara,ref SQL);
                sqlCommand = new SqlCommand();
                for (int i = 0; i < sqlPara.Length; i++)
                {
                    sqlCommand.Parameters.Add(sqlPara[i]);
                }
                ConnectionDB();
                sqlCommand.CommandText = tmpSQL.ToString();
                sqlCommand.Connection = this.sqlConnection;
                DataTable table = new DataTable();
                SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync();
                table.Load(sqlDataReader);
                ds.Tables.Add(table);
                flag = getPropertyNames<dsOut>(ds, ref lstOutRow);
                GetSQLReturnValue(sqlCommand.Parameters, ref objOutput);
                output.Add("ExecuteReturn", objOutput);
                output.Add("Data", lstOutRow);
                flag = true;
            }
            catch (Exception ex)
            {
                lstErrInfo.Add(new ErrorInfo() { ErrorCode = ex.HResult.ToString(), ErrorMsg = ex.Message });

                output.Add("ErrorInfo", lstErrInfo);
            }
            finally
            {
                output.Add("Result", flag);
                closeDB();
            }
            return output;
        }
        public bool ExeuteSP(string SQL, InputPara objInput, ref OutputPara objOutput, ref DataSet ds, ref List<ErrorInfo> lstErrInfo)
        {
            bool flag = false;
            try
            {
                StringBuilder tmpSQL = new StringBuilder();
                SqlParameter[] sqlPara = GenerateSQL(ref tmpSQL, SQL, objInput, objOutput, "Error", SqlDbType.Int, 0); //CreateParameter(objInputPara, objOutputPara,ref SQL);
                sqlCommand = new SqlCommand();
                for (int i = 0; i < sqlPara.Length; i++)
                {
                    sqlCommand.Parameters.Add(sqlPara[i]);
                }
                ConnectionDB();
                sqlCommand.CommandText = tmpSQL.ToString();
                sqlCommand.Connection = this.sqlConnection;
                DataTable table = new DataTable();
                table.Load(sqlCommand.ExecuteReader());
                ds.Tables.Add(table);

                GetSQLReturnValue(sqlCommand.Parameters, ref objOutput);

                flag = true;
            }
            catch (Exception ex)
            {
                lstErrInfo.Add(new ErrorInfo() { ErrorCode = ex.HResult.ToString(), ErrorMsg = ex.InnerException.Message });
            }
            finally
            {
                closeDB();
            }
            return flag;
        }
        public bool getValue(DataSet ds, ref OutputPara objOutput, ref List<ErrorInfo> lstError)
        {
            bool flag = true;
            int Count = 0;

            Count = ds.Tables.Count;
            if (0 == Count)
            {
                flag = false;
            }

            if (flag)
            {
                //使用泛形去assign objOutput
                flag = getPropertyName(ds, ref objOutput);
            }
            return flag;
        }
        public bool getPropertyName(DataSet ds, ref OutputPara objOutput)
        {
            System.Reflection.PropertyInfo[] wMembers = typeof(OutputPara).GetProperties();
            int wMembersLen = wMembers.Length;
            int resultLen = ds.Tables[0].Rows.Count;
            bool flag = true;
            if (resultLen > 0)
            {
                for (int i = 0; i < wMembersLen; i++)
                {
                    // PropertyName.Add(wMembers[i].Name, wMembers[i].PropertyType.Name);
                    Object pValue = ds.Tables[0].Rows[0][wMembers[i].Name.ToString()];
                    switch (wMembers[i].PropertyType.ToString())
                    {
                        case "String":
                            pValue = pValue.ToString();
                            break;
                        case "Int32":
                            pValue = Convert.ToInt32(pValue);
                            break;
                        case "Int64":
                            pValue = Convert.ToInt64(pValue);
                            break;
                        case "Int16":
                            pValue = Convert.ToInt16(pValue);
                            break;
                        case "Double":
                            pValue = Convert.ToDouble(pValue);
                            break;
                        case "Single":
                            pValue = Convert.ToSingle(pValue);
                            break;
                    }

                    objOutput.GetType().GetProperty(wMembers[i].Name).SetValue(objOutput, pValue, null);
                }
            }

            return flag;
        }
        public bool getPropertyName<dsOut>(DataSet ds, ref List<dsOut> lstOutput)
            where dsOut : class
        {
            System.Reflection.PropertyInfo[] wMembers = typeof(dsOut).GetProperties();
            int wMembersLen = wMembers.Length;
            int rowCount = ds.Tables[0].Rows.Count;
            bool flag = true;
            for (int RowIndx = 0; RowIndx < rowCount; RowIndx++)
            {
                dsOut obj = (dsOut)Activator.CreateInstance(typeof(dsOut));
                for (int i = 0; i < wMembersLen; i++)
                {
                    // PropertyName.Add(wMembers[i].Name, wMembers[i].PropertyType.Name);
                    Object pValue = ds.Tables[0].Rows[RowIndx][wMembers[i].Name.ToString()];
                    switch (wMembers[i].PropertyType.ToString().Replace("System.", ""))
                    {
                        case "String":
                            if (pValue.GetType().Equals(typeof(DateTime)))
                            {
                                pValue = Convert.ToString(pValue, new CultureInfo("en-US"));
                            }
                            else
                            {
                                pValue = pValue.ToString();
                            }

                            break;
                        case "Int32":
                            pValue = Convert.ToInt32(pValue);
                            break;
                        case "Int64":
                            pValue = Convert.ToInt64(pValue);
                            break;
                        case "Int16":
                            pValue = Convert.ToInt16(pValue);
                            break;
                        case "Double":
                            pValue = Convert.ToDouble(pValue);
                            break;
                        case "Single":
                            pValue = Convert.ToSingle(pValue);
                            break;
                        case "DateTime":
                            pValue = Convert.ToDateTime(pValue);

                            break;
                    }

                    obj.GetType().GetProperty(wMembers[i].Name).SetValue(obj, pValue, null);
                }
                lstOutput.Add(obj);

            }

            return flag;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:必須檢視 SQL 查詢中是否有安全性弱點")]
        //public bool ExecuteSPNonQuery(SQLType.SPKind SQL, InputPara objInputPara, ref OutputPara objOutputPara, ref List<ErrorInfo> lstErrorInfo)
        //{
        //    bool flag = false;
        //    try
        //    {
        //        string SQLCmd = new SQLType().getSPName(SQL);
        //        StringBuilder tmpSQL = new StringBuilder();
        //        SqlParameter[] sqlPara = GenerateSQL(ref tmpSQL, SQLCmd, objInputPara, objOutputPara, "Error", SqlDbType.Int, 0); //CreateParameter(objInputPara, objOutputPara,ref SQL);

        //        sqlCommand = new SqlCommand();
        //        for (int i = 0; i < sqlPara.Length; i++)
        //        {
        //            sqlCommand.Parameters.Add(sqlPara[i]);
        //        }
        //        ConnectionDB();
        //        sqlCommand.CommandText = tmpSQL.ToString();
        //        sqlCommand.Connection = this.sqlConnection;
        //        sqlCommand.ExecuteNonQuery();
        //        GetSQLReturnValue(sqlCommand.Parameters, ref objOutputPara);
        //        flag = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        lstErrorInfo.Add(new ErrorInfo() { ErrorCode = "ER000", ErrorMsg = "發生異常錯誤_2");
        //        // closeDB();

        //    }
        //    finally
        //    {
        //        closeDB();
        //    }
        //    return flag;
        //}
        public bool getPropertyNames<T>(DataSet ds, ref List<T> lstOutput)
         where T : class
        {
            System.Reflection.PropertyInfo[] wMembers = typeof(T).GetProperties();
            int wMembersLen = wMembers.Length;
            int rowCount = ds.Tables[0].Rows.Count;
            bool flag = true;
            for (int RowIndx = 0; RowIndx < rowCount; RowIndx++)
            {
                T obj = (T)Activator.CreateInstance(typeof(T));
                for (int i = 0; i < wMembersLen; i++)
                {
                    // PropertyName.Add(wMembers[i].Name, wMembers[i].PropertyType.Name);
                    Object pValue = ds.Tables[0].Rows[RowIndx][wMembers[i].Name.ToString()];
                    switch (wMembers[i].PropertyType.ToString().Replace("System.", ""))
                    {
                        case "String":
                            pValue = pValue.ToString();
                            break;
                        case "Int32":
                            pValue = Convert.ToInt32(pValue);
                            break;
                        case "Int64":
                            pValue = Convert.ToInt64(pValue);
                            break;
                        case "Int16":
                            pValue = Convert.ToInt16(pValue);
                            break;
                        case "Double":
                            pValue = Convert.ToDouble(pValue);
                            break;
                        case "Single":
                            pValue = Convert.ToSingle(pValue);
                            break;
                        case "DateTime":
                            pValue = Convert.ToDateTime(pValue);
                            break;
                    }

                    obj.GetType().GetProperty(wMembers[i].Name).SetValue(obj, pValue, null);
                }
                lstOutput.Add(obj);

            }

            return flag;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:必須檢視 SQL 查詢中是否有安全性弱點")]
        public bool ExecuteSPNonQuery(String SQL, InputPara objInputPara, ref OutputPara objOutputPara, ref List<ErrorInfo> lstErrorInfo)
        {
            bool flag = false;
            try
            {

                StringBuilder tmpSQL = new StringBuilder();
                SqlParameter[] sqlPara = GenerateSQL(ref tmpSQL, SQL, objInputPara, objOutputPara, "Error", SqlDbType.Int, 0); //CreateParameter(objInputPara, objOutputPara,ref SQL);

                sqlCommand = new SqlCommand();
                sqlCommand.Parameters.Clear();
                string sqlParaString = "";
                for (int i = 0; i < sqlPara.Length; i++)
                {
                    if(sqlParaString.IndexOf(sqlPara[i].ParameterName + ",") == -1)
                    {
                        sqlCommand.Parameters.Add(sqlPara[i]);
                        sqlParaString += sqlPara[i].ParameterName + ",";
                    }
                }
                ConnectionDB();
                sqlCommand.CommandText = tmpSQL.ToString();
                sqlCommand.Connection = this.sqlConnection;
                //先避掉ErrorLog造成的錯誤，會有參數重複的問題看來文斌應該已經知道有這件事
                if(SQL!= "usp_InsErrorLog")
                {
                    sqlCommand.ExecuteNonQuery(); //這行會掛
                    GetSQLReturnValue(sqlCommand.Parameters, ref objOutputPara);
                }

                flag = true;
            }
            catch (Exception ex)
            {
                lstErrorInfo.Add(new ErrorInfo() { ErrorCode = "ER00A", ErrorMsg = ex.Message });

            }
            finally
            {
                closeDB();
            }

            return flag;
        }

    }
}
