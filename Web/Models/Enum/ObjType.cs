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
            /// <summary>
            /// 匯入調度停車場
            /// </summary>
            InsTransParking,
            /// <summary>
            /// 匯入車機車輛綁定資料
            /// </summary>
            ImportCarBindData,
            /// <summary>
            /// 匯入車機資料
            /// </summary>
            ImportCarMachineData,
            /// <summary>
            /// 匯入汽車資料
            /// </summary>
            ImportCarData,
            /// <summary>
            /// 匯入機車資料
            /// </summary>
            ImportMotoData,
            /// <summary>
            /// 匯入萬用卡
            /// </summary>
            ImportMasterCardData,
        }
        public string GetSPName(ObjType.SPType type)
        {
            string SPName = "";
            switch (type)
            {
                case SPType.Login: 
                    SPName = "usp_Login_BE";
                    break;
                case SPType.InsTransParking:  
                    SPName = "usp_BE_InsTransParking";
                    break;
                case SPType.ImportCarBindData:
                    SPName = "usp_BE_ImportCarBindData";
                    break;
                case SPType.ImportCarMachineData:
                    SPName = "usp_BE_ImportCarMachineData";
                    break;
                case SPType.ImportCarData:
                    SPName = "usp_BE_ImportCarData";
                    break;
                case SPType.ImportMotoData:
                    SPName = "usp_BE_ImportMotoData";
                    break;
                case SPType.ImportMasterCardData:
                    SPName = "usp_BE_ImportMasterCardData";
                    break;
            }
            return SPName;
        }

     }
}