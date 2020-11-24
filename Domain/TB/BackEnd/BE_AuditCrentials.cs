using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    /// <summary>
    /// 
    /// </summary>
    public class BE_AuditCrentials
    {
        public string ID_1 { set; get; } = "";
        public string ID_2 { set; get; } = "";
        public string Car_1 { set; get; } = "";
        public string Car_2 { set; get; } = "";
        public string Motor_1 { set; get; } = "";
        public string Motor_2 { set; get; } = "";
        public string Self_1 { set; get; } = "";
        public string Signture_1 { set; get; } = "";
        public string F01 { set; get; } = "";
        public string Other_1 { set; get; } = "";
        public string Business_1 { set; get; } = "";
        /// <summary>
        /// 是否是待審核
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// <para>2:舊系統未同步照片</para>
        /// </summary>
        public int ID_1_IsNew { set; get; } = 2;
        public int ID_2_IsNew { set; get; } = 2;
        public int Car_1_IsNew { set; get; } = 2;
        public int Car_2_IsNew { set; get; } = 2;
        public int Motor_1_IsNew { set; get; } = 2;
        public int Motor_2_IsNew { set; get; } = 2;
        public int Self_1_IsNew { set; get; } = 2;
        public int Signture_1_IsNew { set; get; } = 2;
        public int F01_IsNew { set; get; } = 2;
        public int Other_1_IsNew { set; get; } = 2;
        public int Business_1_IsNew { set; get; } = 2;

        public string ID_1_UPDTime { set; get; } = "";
        public string ID_2_UPDTime { set; get; } = "";
        public string Car_1_UPDTime { set; get; } = "";
        public string Car_2_UPDTime { set; get; } = "";
        public string Motor_1_UPDTime { set; get; } = "";
        public string Motor_2_UPDTime { set; get; } = "";
        public string Self_1_UPDTime { set; get; } = "";
        public string Signture_1_UPDTime { set; get; } = "";
        public string F01_UPDTime { set; get; } = "";
        public string Other_1_UPDTime { set; get; } = "";
        public string Business_1_UPDTime { set; get; } = "";

    }
}
