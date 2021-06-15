using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reposotory.Implement.Enum
{
    public class ObjType
    {
        public enum SPType
        {
            /// <summary>
            /// 寫入月租使用記錄
            /// </summary>
            InsMonthHistory,
            /// <summary>
            /// 還原月租使用記錄
            /// </summary>
            ClearMonthTmpHistory
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
                case SPType.InsMonthHistory:
                    SPName = "usp_InsMonthlyHistory_0301";
                    break;
                case SPType.ClearMonthTmpHistory:
                    SPName = "usp_ClearMonthlyTmpHistory";
                    break;
            }
            return SPName;
        }
    }
}
