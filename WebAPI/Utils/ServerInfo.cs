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

        //20220129 ADD BY ADAM REASON.把訂閱制部分查詢移至鏡像
        private static readonly string SERVER_NAME_MIRROR = ConfigurationManager.AppSettings["SERVER_NAME_MIRROR"].Trim();
        private static readonly string DATABASE_NAME_MIRROR = ConfigurationManager.AppSettings["DATABASE_NAME_MIRROR"].Trim();
        private static readonly string LOGIN_MIRROR = ConfigurationManager.AppSettings["LOGIN_MIRROR"].Trim();
        private static readonly string PASSWORD_MIRROR = ConfigurationManager.AppSettings["PASSWORD_MIRROR"].Trim();

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

        //20220129 ADD BY ADAM REASON.把訂閱制部分查詢移至鏡像
        public static string[] GetMirrorServerInfo()
        {
            return new String[5] {
                SERVER_NAME_MIRROR,
                DATABASE_NAME_MIRROR,
                LOGIN_MIRROR,
                PASSWORD_MIRROR,
                DATABASE_TYPE  ,
            };
        }
    }
}