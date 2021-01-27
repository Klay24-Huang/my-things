using System;
using System.Collections.Generic;
using System.Reflection;
using WebCommon;

namespace Domain
{
    /// <summary>
    /// 利用Reflection的方式取出Property執行取值/設定值
    /// </summary>
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