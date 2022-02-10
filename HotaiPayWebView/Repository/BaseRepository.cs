using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using HotaiPayWebView.Models;
using WebCommon;

namespace HotaiPayWebView.Repository
{
    public class Repository
    {
        private string _connectionString;
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }
        public void SetConnect(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }
        public List<T> GetObjList<T>(ref bool flag, ref List<ErrorInfo> lstError, string SQL, SqlParameter[] para, string term)
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






        public class ReflectionHelper
        {
            public bool SetPropertyValue<T>(string propertyName, string val, ref T obj, ref List<ErrorInfo> lstError)
                 where T : class
            {
                bool flag = true;
                if (null == obj)
                {
                    return false;
                }
                try
                {
                    Type type = obj.GetType();
                    PropertyInfo info = type.GetProperty(propertyName);

                    if (null == info) return false;

                    Type ptype = info.PropertyType;

                    if (ptype.Equals(typeof(string)))
                    {
                        info.SetValue(obj, val, new object[0]);
                    }
                    else if (ptype.Equals(typeof(Boolean)))
                    {
                        bool value = false;
                        value = val.ToLower().StartsWith("true");
                        info.SetValue(obj, value, new object[0]);
                    }
                    else if (ptype.Equals(typeof(int)))
                    {
                        int value = String.IsNullOrEmpty(val) ? 0 : int.Parse(val);
                        info.SetValue(obj, value, new object[0]);
                    }
                    else if (ptype.Equals(typeof(double)))
                    {
                        double value = 0.0d;
                        if (!String.IsNullOrEmpty(val))
                        {
                            value = Convert.ToDouble(val);
                        }
                        info.SetValue(obj, value, new object[0]);
                    }
                    else if (ptype.Equals(typeof(DateTime)))
                    {
                        DateTime value = String.IsNullOrEmpty(val)
                           ? DateTime.MinValue
                           : DateTime.Parse(val);
                        info.SetValue(obj, value, new object[0]);
                    }
                    else if (ptype.Equals(typeof(DateTime?)))
                    {
                        DateTime? value = String.IsNullOrEmpty(val)
                           ? DateTime.MinValue
                           : DateTime.Parse(val);
                        info.SetValue(obj, value, new object[0]);
                    }
                    else if (ptype.Equals(typeof(Decimal)))
                    {
                        Decimal value = 0.0M;
                        if (!String.IsNullOrEmpty(val))
                        {
                            value = Convert.ToDecimal(val);
                        }
                        info.SetValue(obj, value, new object[0]);
                    }
                    else if (ptype.Equals(typeof(Int64)))
                    {
                        Int64 value = 0;
                        if (!String.IsNullOrEmpty(val))
                        {
                            value = Convert.ToInt64(val);
                        }
                        info.SetValue(obj, value, new object[0]);
                    }
                    else if (ptype.Equals(typeof(Int16)))
                    {
                        Int16 value = 0;
                        if (!String.IsNullOrEmpty(val))
                        {
                            value = Convert.ToInt16(val);
                        }
                        info.SetValue(obj, value, new object[0]);
                    }
                    else if (ptype.Equals(typeof(Single)))
                    {
                        float value = 0;
                        if (!String.IsNullOrEmpty(val))
                        {
                            value = Convert.ToSingle(val);
                        }
                        info.SetValue(obj, value, new object[0]);
                    }

                }
                catch (Exception ex)
                {
                    lstError.Add(new ErrorInfo() { ErrorCode = ex.HResult.ToString(), ErrorMsg = ex.Message });
                    flag = false;
                }


                return flag;
            }

        }
    }
}