﻿using Domain;
using Domain.TB;
using Domain.TB.BackEnd;
using Domain.TB.Mochi;
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
    /// 停車場相關
    /// </summary>
    public class ParkingRepository:BaseRepository
    {
        private string _connectionString { set; get; }
        public ParkingRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }
        /// <summary>
        /// 取得所有有效的停車場
        /// </summary>
        /// <returns></returns>
        public List<ParkingData> GetAllParking()
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<ParkingData> lstParking = null;
            int nowCount = 0;
            string SQL = "SELECT  [ParkingType],[ParkingName],[ParkingAddress],[Longitude],[Latitude],[OpenTime] ,[CloseTime] FROM [VW_GetParking] WITH(NOLOCK) WHERE use_flag=1 AND (OpenTime<=DATEADD(HOUR,8,GETDATE()) AND CloseTime>=DATEADD(HOUR,8,GETDATE())) ;";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            lstParking = GetObjList<ParkingData>(ref flag, ref lstError, SQL, para, term);
            return lstParking;
        }
        /// <summary>
        /// 依停車場類型取得所有有效的停車場
        /// </summary>
        /// <param name="ParkType">
        /// 停車場類型
        /// <para>0:一般（調度）</para>
        /// <para>1:特約（車麻吉及其他）</para>
        /// </param>
        /// <returns></returns>
        public List<ParkingData> GetAllParkingByType(int ParkType)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<ParkingData> lstParking = null;
            int nowCount = 0;
            string SQL = string.Format("SELECT  [ParkingType],[ParkingName],[ParkingAddress],[Longitude],[Latitude],[OpenTime] ,[CloseTime] FROM [VW_GetParking] WITH(NOLOCK) WHERE use_flag=1 AND (OpenTime<=DATEADD(HOUR,8,GETDATE()) AND CloseTime>=DATEADD(HOUR,8,GETDATE())) AND ParkingType={0}", ParkType);
            SqlParameter[] para = new SqlParameter[2];
            string term = "";

            lstParking = GetObjList<ParkingData>(ref flag, ref lstError, SQL, para, term);
            return lstParking;
        }
        /// <summary>
        /// 取出方圓據點內，所有有效的停車場
        /// </summary>
        /// <param name="lat">緯度</param>
        /// <param name="lng">經度</param>
        /// <param name="radius">半徑（單位公里）</param>
        /// <returns></returns>
        public List<ParkingData> GetAllParking(double lat, double lng, double radius)
        {

            bool flag = false, hasRange = true;
            double[] latlngLimit = { 0.0, 0.0, 0.0, 0.0 };
            if (lng > 0 && lat > 0 && radius > 0)
            {
                latlngLimit = GetAround(lat, lng, radius);
            }
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<ParkingData> lstParking = null;
            int nowCount = 0;
            string SQL = "SELECT  [ParkingType],[ParkingName],[ParkingAddress],[Longitude],[Latitude],[OpenTime] ,[CloseTime] FROM [VW_GetParking] WITH(NOLOCK) WHERE use_flag=1 AND (OpenTime<=DATEADD(HOUR,8,GETDATE()) AND CloseTime>=DATEADD(HOUR,8,GETDATE())) ";

            SqlParameter[] para = new SqlParameter[2];

            for (int j = 0; j < 4; j++)
            {
                if (latlngLimit[j] == 0)
                {
                    hasRange = false;
                    break;
                }
            }
            string term = "";



            if (hasRange)
            {
                //最小緯度lat、最小經度lng、最大緯度lat、最大經度lng
                SQL += string.Format(" AND (Latitude>={0} AND Latitude<={1}) AND (Longitude>={2} AND Longitude<={3})", latlngLimit[0], latlngLimit[2], latlngLimit[1], latlngLimit[3]);
            }
      
            lstParking = GetObjList<ParkingData>(ref flag, ref lstError, SQL, para, term);
            return lstParking;
        }
        public List<ParkingData> GetAllParkingByType(int ParkType,double lat, double lng, double radius)
        {
            bool flag = false, hasRange = true;
            double[] latlngLimit = { 0.0, 0.0, 0.0, 0.0 };
            if (lng > 0 && lat > 0 && radius > 0)
            {
                latlngLimit = GetAround(lat, lng, radius);
            }
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<ParkingData> lstParking = null;
            int nowCount = 0;
            string SQL = "SELECT  [ParkingType],[ParkingName],[ParkingAddress],[Longitude],[Latitude],[OpenTime] ,[CloseTime] FROM [VW_GetParking] WITH(NOLOCK) WHERE use_flag=1 AND (OpenTime<=DATEADD(HOUR,8,GETDATE()) AND CloseTime>=DATEADD(HOUR,8,GETDATE()))  ";

            SqlParameter[] para = new SqlParameter[2];

            for (int j = 0; j < 4; j++)
            {
                if (latlngLimit[j] == 0)
                {
                    hasRange = false;
                    break;
                }
            }
            string term = "";



            if (hasRange)
            {
                //最小緯度lat、最小經度lng、最大緯度lat、最大經度lng
                SQL += string.Format(" AND (Latitude>={0} AND Latitude<={1}) AND (Longitude>={2} AND Longitude<={3}) AND ParkingType={4}", latlngLimit[0], latlngLimit[2], latlngLimit[1], latlngLimit[3],ParkType);
            }
            //SQL += "ORDER BY StationID ASC;";
            lstParking = GetObjList<ParkingData>(ref flag, ref lstError, SQL, para, term);
            return lstParking;
        }
        public List<SyncMachiParkId> GetMachiParkId()
        {
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<SyncMachiParkId> lstStation = null;
            bool flag = false;
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            string SQL = "SELECT Id,Name,0 as use_flag FROM [TB_MochiPark] WHERE use_flag=1 "; //已修改TB指向
           lstStation = GetObjList<SyncMachiParkId>(ref flag, ref lstError, SQL, para, term);

            return lstStation;
        }
        /// <summary>
        /// 取得調度停車場
        /// </summary>
        /// <param name="ParkingName"></param>
        /// <returns></returns>
        public List<BE_ParkingData> GetTransParking(string ParkingName)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_ParkingData> lstParking = null;
            int nowCount = 0;
            string SQL = "SELECT  [ParkingID],[ParkingName],[ParkingAddress],[ParkingLng] AS Longitude ,[ParkingLat] AS Latitude,[OpenTime],[CloseTime] FROM [dbo].[TB_ParkingData] ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            if (string.IsNullOrEmpty(ParkingName) == false && string.IsNullOrWhiteSpace(ParkingName) == false)
            {
              //  SQL += string.Format(" WITH(NOLOCK) WHERE ParkingName like '%{0}%' ", ParkingName);
                term = " ParkingName like @ParkingName";
                para[nowCount] = new SqlParameter("@ParkingName", SqlDbType.NVarChar, 100);
                para[nowCount].Value = "%"+ParkingName+"%";
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (term != "")
            {
                SQL += " WITH(NOLOCK) WHERE " + term;
            }
            SQL += " ORDER BY ParkingID ASC ";
            lstParking = GetObjList<BE_ParkingData>(ref flag, ref lstError, SQL, para, term);
            return lstParking;
        }
        /// <summary>
        /// 取得停車便利付停車場設定
        /// </summary>
        /// <param name="ParkingName"></param>
        /// <returns></returns>
        public List<BE_ChargeParkingData> GetChargeParking(string ParkingName)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_ChargeParkingData> lstParking = null;
            int nowCount = 0;
            string SQL = "SELECT  * FROM [dbo].[VW_BE_GetChargeParking] ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            if (string.IsNullOrEmpty(ParkingName) == false && string.IsNullOrWhiteSpace(ParkingName) == false)
            {
                //  SQL += string.Format(" WITH(NOLOCK) WHERE ParkingName like '%{0}%' ", ParkingName);
                term = " ParkingName like @ParkingName";
                para[nowCount] = new SqlParameter("@ParkingName", SqlDbType.NVarChar, 100);
                para[nowCount].Value = "%" + ParkingName + "%";
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (term != "")
            {
                SQL += " WITH(NOLOCK) WHERE " + term;
            }
            SQL += " ORDER BY ParkId ASC ";
            lstParking = GetObjList<BE_ChargeParkingData>(ref flag, ref lstError, SQL, para, term);
            return lstParking;
        }
    }
}
