using Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebCommon;

namespace Reposotory.Implement
{
    /// <summary>
    /// base
    /// </summary>
    public class BaseRepository
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
        public void ExecNonResponse(ref bool flag,string SQL)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(this.ConnectionString))
                using (SqlCommand command = new SqlCommand(SQL, conn))
                {

                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 180;

                    if (conn.State != ConnectionState.Open) conn.Open();

                    int result = command.ExecuteNonQuery();
                }
            }
            catch(Exception ex)
            {
                flag = false;
            }
        
        }
        /// <summary>
        /// 取得經緯度最大範圍
        /// </summary>
        /// <param name="lat">目前緯度</param>
        /// <param name="lon">目前經度</param>
        /// <param name="raidus">半徑(單位公尺)</param>
        /// <returns>double array依序最小緯度lat、最小經度lng、最大緯度lat、最大經度lng</returns>
        public double[] GetAround(double lat, double lon, double raidus)
        {
            Double latitude = lat;
            Double longitude = lon;

            Double degree = (24901 * 1609) / 360.0;
            double raidusMile = raidus * 1000;

            Double dpmLat = 1 / degree;
            Double radiusLat = dpmLat * raidusMile;
            Double minLat = latitude - radiusLat;
            Double maxLat = latitude + radiusLat;

            Double mpdLng = degree * Math.Cos(latitude * (Math.PI / 180));
            Double dpmLng = 1 / mpdLng;
            Double radiusLng = dpmLng * raidusMile;
            Double minLng = longitude - radiusLng;
            Double maxLng = longitude + radiusLng;
            return new double[] { minLat, minLng, maxLat, maxLng };

        }
    }
}
