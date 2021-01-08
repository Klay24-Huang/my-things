using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace WebAPI.Utils
{
    public static class objUti
    {
        public static List<T> ConvertToList<T>(DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                var columnNames = dt.Columns.Cast<DataColumn>()
                        .Select(c => c.ColumnName)
                        .ToList();
                var properties = typeof(T).GetProperties();
                return dt.AsEnumerable().Select(row =>
                {
                    var objT = Activator.CreateInstance<T>();
                    foreach (var pro in properties)
                    {
                        if (columnNames.Contains(pro.Name))
                        {
                            PropertyInfo pI = objT.GetType().GetProperty(pro.Name);
                            var oType =  pI.PropertyType; 
                            if (oType == typeof(string))
                                pro.SetValue(objT, row[pro.Name] == DBNull.Value ? "" : Convert.ChangeType(row[pro.Name], pI.PropertyType));
                            else
                            {
                                var defv = Activator.CreateInstance(oType);
                                pro.SetValue(objT, row[pro.Name] == DBNull.Value ? defv : Convert.ChangeType(row[pro.Name], pI.PropertyType));
                            } 
                        }
                    }
                    return objT;
                }).ToList();
            }
            else
                return default(List<T>);
        }

        public static T GetFirstRow<T>(DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                var source = ConvertToList<T>(dt);
                if (source != null && source.Count() > 0)
                    return source.FirstOrDefault();
                else
                    return default(T);
            }
            else
                return default(T);
        }

        public static List<string> StrEmuList<T>()
        {
            return Enum.GetNames(typeof(T)).ToList();
        }

        public static T Clone<T>(T sour)
        {
            if (sour != null)
                return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(sour));
            else
                return default(T);
        }
    
        public static T2 TTMap<T1,T2>(T1 sour)
        {
            var re = JsonConvert.DeserializeObject<T2>(JsonConvert.SerializeObject(sour));
            return re;
        } 
    }
}