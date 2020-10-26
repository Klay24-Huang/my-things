﻿using System;
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
                            pro.SetValue(objT, row[pro.Name] == DBNull.Value ? null : Convert.ChangeType(row[pro.Name], pI.PropertyType));
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
    }
}