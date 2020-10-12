using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.Enum
{
    public class ObjType
    {
        /// <summary>
        /// 列舉sp
        /// <para>InsError:寫入錯誤資訊進資料庫</para>
        /// <para>InsAPILog:寫入API呼叫LOG進資料庫</para>
        /// </summary>
        public enum SPType
        {
            /// <summary>
            /// 登入
            /// </summary>
            Login,
        }
        public string GetSPName(ObjType.SPType type)
        {
            string SPName = "";
            switch (type)
            {
                case SPType.Login:  //寫入API呼叫LOG進資料庫
                    SPName = "usp_Login_BE";
                    break;
            }
            return SPName;
        }

     }
}