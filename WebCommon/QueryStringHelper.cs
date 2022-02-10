using System;
using System.Reflection;
using System.Web;

namespace WebCommon
{
    public class QueryStringHelper
    {
        public T QueryStringToObject<T>(string input) where T : new()
        {
            var dict = HttpUtility.ParseQueryString(input);

            T t = GetObject<T>();

            foreach (var key in dict.AllKeys)
            {
                PropertyInfo property = t.GetType().GetProperty(key);
                if (property == null)
                    continue;

                property.SetValue(t, ReturnValue(dict[key], property.PropertyType));

            }
            return t;

        }
        private static T GetObject<T>()
        {
            T obj = default(T);
            obj = Activator.CreateInstance<T>();
            return obj;
        }
        public object ReturnValue(object data, Type TagatType)
        {
            if (data == System.DBNull.Value)
                return null;

            string typename = TagatType.FullName.ToLower();

            if (typename.Contains("string"))
            {
                return data.ToString();
            }
            else if (typename.Contains("int"))
            {
                return int.Parse(data.ToString());
            }
            else if (typename.Contains("datetime"))
            {
                DateTime dateTimeValue = new DateTime();
                DateTime.TryParse(data.ToString(), out dateTimeValue);
                return dateTimeValue;
            }
            else if (typename.Contains("decimal") && data != null)
            {
                return decimal.Parse(data.ToString());
            }
            else if (typename.Contains("bool"))
            {
                if (Int32.TryParse(data.ToString(), out int intdata))
                {
                    return Convert.ToBoolean(intdata);
                }
                else
                {
                    return Convert.ToBoolean(data.ToString());
                }
            }
            else
                return null;
        }
    }
}
