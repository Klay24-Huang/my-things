using System;
using System.Globalization;

namespace Web.Utilities
{
    public static class ConvertHelper
    {
        public static object FromNullable(object data)
        {
            return (object)data ?? (object)DBNull.Value;

        }

        public static DateTime? ToNullableDateTime(object data)
        {
            DateTime? d = null;
            if (data == DBNull.Value)
                d = null;
            else if (data.GetType() == typeof(DateTime))
                d = (DateTime?)data;
            else if (data.GetType() == typeof(string))
            {
                string[] formaters = {   
                                    "yyyy/M/d tt hh:mm:ss",   
                                    "yyyy/MM/dd tt hh:mm:ss",   
                                    "yyyy/MM/dd HH:mm:ss",   
                                    "yyyy/M/d HH:mm:ss",   
                                    "yyyy/M/d",   
                                    "yyyy/MM/dd"   
                                    };

                d = DateTime.ParseExact((string)data, formaters, CultureInfo.CurrentUICulture, DateTimeStyles.AllowWhiteSpaces);
            }
            return d;
        }

        public static String ToNullableString(object data)
        {
            return (data == DBNull.Value ? (String)null : (String)data);
        }
        public static int? ToNullableInt(object data)
        {
            return (data == DBNull.Value ? (int?)null : Convert.ToInt32( data));
        }
    }
}
