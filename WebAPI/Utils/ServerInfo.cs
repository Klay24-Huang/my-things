using System;
using System.Configuration;

namespace WebAPI.Utils
{
    public class ServerInfo
    {
        private static readonly string SERVER_NAME = ConfigurationManager.AppSettings["SERVER_NAME"].Trim();
        private static readonly string DATABASE_NAME = ConfigurationManager.AppSettings["DATABASE_NAME"].Trim();
        private static readonly string LOGIN = ConfigurationManager.AppSettings["LOGIN"].Trim();
        private static readonly string PASSWORD = ConfigurationManager.AppSettings["PASSWORD"].Trim();
        private static readonly string DATABASE_TYPE = ConfigurationManager.AppSettings["DATABASE_TYPE"].Trim();
        public static string[] GetServerInfo()
        {
            return new String[5] { 
                SERVER_NAME, 
                DATABASE_NAME, 
                LOGIN, 
                PASSWORD, 
                DATABASE_TYPE  ,
            };

        }
    }
}