using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtherService.Enum
{
   public class ObjType
    {
        public enum SPType
        {
            /// <summary>
            /// 寫入呼叫webapi的log資料
            /// </summary>
             InsWebAPILog
        }
        /// <summary>
        /// 取出SPName
        /// </summary>
        /// <param name="type">對應ObjType.SPType</param>
        /// <returns></returns>
        public string GetSPName(ObjType.SPType type)
        {
            string SPName = "";
            switch (type)
            {
                case SPType.InsWebAPILog:  //寫入API呼叫LOG進資料庫
                    SPName = "usp_InsWebAPILog";
                    break;
            }
            return SPName;
        }

    }
}
