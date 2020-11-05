using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_AuditImage:SPInput_Base
    {
        /// <summary>
        /// 身份證
        /// </summary>
        public string IDNO { get; set; }
        public int ID_1 { get; set; }
        public string ID_1_Image { set; get; }
        public int ID_1_Audit { get; set; }
        public string ID_1_Reason { get; set; }

        public int ID_2 { get; set; }
        public int ID_2_Audit { get; set; }
        public string ID_2_Image { set; get; }
        public string ID_2_Reason { get; set; }

        public int Car_1 { get; set; }
        public string Car_1_Image { get; set; }
        public int Car_1_Audit { get; set; }
        public string Car_1_Reason { get; set; }

        public int Car_2 { get; set; }
        public string Car_2_Image { get; set; }
        public int Car_2_Audit { get; set; }
        public string Car_2_Reason { get; set; }

        public int Motor_1 { get; set; }
        public string Motor_1_Image { get; set; }
        public int Motor_1_Audit { get; set; }
        public string Motor_1_Reason { get; set; }

        public int Motor_2 { get; set; }
        public string Motor_2_Image { get; set; }
        public int Motor_2_Audit { get; set; }
        public string Motor_2_Reason { get; set; }

        public int Self_1 { get; set; }
        public string Self_1_Image { get; set; }
        public int Self_1_Audit { get; set; }
        public string Self_1_Reason { get; set; }

        public int F01 { get; set; }
        public string F01_Image { get; set; }
        public int F01_Audit { get; set; }
        public string F01_Reason { get; set; }

        public int Other_1 { get; set; }
        public string Other_1_Image { get; set; }
        public int Other_1_Audit { get; set; }
        public string Other_1_Reason { get; set; }

        public int Business_1 { get; set; }
        public string Business_1_Image { get; set; }
        public int Business_1_Audit { get; set; }
        public string Business_1_Reason { get; set; }

        public int Signture_1 { get; set; }
        public string Signture_1_Image { get; set; }
        public int Signture_1_Audit { get; set; }
        public string Signture_1_Reason { get; set; }
        public string UserID { set; get; }
    }
}
