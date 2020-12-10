using System;
using System.Configuration;
using Web.Models.Params;

namespace Web.Utilities
{
    public class ServerInfo
    {
        private static readonly string SERVER_NAME = ConfigurationManager.AppSettings["SERVER_NAME"].Trim();
        private static readonly string DATABASE_NAME = ConfigurationManager.AppSettings["DATABASE_NAME"].Trim();
        private static readonly string LOGIN = ConfigurationManager.AppSettings["LOGIN"].Trim();
        private static readonly string PASSWORD = ConfigurationManager.AppSettings["PASSWORD"].Trim();
        private static readonly string DATABASE_TYPE = ConfigurationManager.AppSettings["DATABASE_TYPE"].Trim();
        public static string[] GetServerInfo(BaseParams param)
        {
            return new String[6] { 
                param != null ? (param.SERVER_NAME ?? SERVER_NAME) : SERVER_NAME, 
                param != null ? param.DATABASE_NAME ?? DATABASE_NAME : DATABASE_NAME, 
                param != null ? param.LOGIN ?? LOGIN : LOGIN, 
                param != null ? param.PASSWORD ?? PASSWORD : PASSWORD, 
                param != null ? param.DATABASE_TYPE ?? DATABASE_TYPE : DATABASE_TYPE  ,
                //20180205 ADD BY JERRY 增加SSAPI位址
                param != null ? (param.SSAPI_ADDRESS!=null?param.SSAPI_ADDRESS:""):""
            };

        }
    }
}