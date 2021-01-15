using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_Audit:IAPI_BE_Base
    {
        public AuditImageData ImageData { get; set; }
        public string IDNO { get; set; }
        public List<string> Driver { get; set; }
        public string SPECSTATUS { get; set; }
        public string SPSD { get; set; }
        public string SPED { get; set; }
        public string Birth { get; set; }
        public string Mobile { get; set; }
        public string Area { get; set; }
        public string Addr { get; set; }
        public string UniCode { get; set; }
        public string InvoiceType { get; set; }
        public int AuditStatus { get; set; }
        public string NotAuditReason { get; set; }
        public string RejectReason { get; set; }
        public int SendMessage { get; set; }
        public int IsNew { set; get; }

        //20201125 UPD BY JERRY 增加欄位處理
        public string MEMHTEL { get; set; }
        public string MEMCOMTEL { get; set; }
        public string MEMCONTRACT { get; set; }
        public string MEMCONTEL { get; set; }
        public string MEMEMAIL { get; set; }
        public int HasVaildEMail { get; set; }
        public string MEMMSG { get; set; }
        //20201125 UPD BY 堂尾鰭 增加欄位處理
        public string MEMONEW { get; set; }

    }
    public class AuditImageData
    {
        public int ID_1 { get; set; }
        public int ID_1_new { get; set; }
        public int ID_1_Audit { get; set; }
        public string ID_1_Reason { get; set; }
        public string ID_1_Image { get; set; }
        public int ID_2 { get; set; }
        public int ID_2_new { get; set; }
        public int ID_2_Audit { get; set; }
        public string ID_2_Reason { get; set; }
        public string ID_2_Image { get; set; }
        public int Car_1 { get; set; }
        public int Car_1_new { get; set; }
        public int Car_1_Audit { get; set; }
        public string Car_1_Reason { get; set; }
        public string Car_1_Image { get; set; }
        public int Car_2 { get; set; }
        public int Car_2_new { get; set; }
        public int Car_2_Audit { get; set; }
        public string Car_2_Reason { get; set; }
        public string Car_2_Image { get; set; }
        public int Motor_1 { get; set; }
        public int Motor_1_new { get; set; }
        public int Motor_1_Audit { get; set; }
        public string Motor_1_Reason { get; set; }
        public string Motor_1_Image { get; set; }
        public int Motor_2 { get; set; }
        public int Motor_2_new { get; set; }
        public int Motor_2_Audit { get; set; }
        public string Motor_2_Reason { get; set; }
        public string Motor_2_Image { get; set; }
        public int Self_1 { get; set; }
        public int Self_1_new { get; set; }
        public int Self_1_Audit { get; set; }
        public string Self_1_Reason { get; set; }
        public string Self_1_Image { get; set; }
        public int F01 { get; set; }
        public int F01_new { get; set; }
        public int F01_Audit { get; set; }
        public string F01_Reason { get; set; }
        public string F01_Image { get; set; }
        public int Other_1 { get; set; }
        public int Other_1_new { get; set; }
        public int Other_1_Audit { get; set; }
        public string Other_1_Reason { get; set; }
        public string Other_1_Image { get; set; }
        public int Business_1 { get; set; }
        public int Business_1_new { get; set; }
        public int Business_1_Audit { get; set; }
        public string Business_1_Reason { get; set; }
        public string Business_1_Image { get; set; }
        public int Signture_1 { get; set; }
        public int Signture_1_new { get; set; }
        public int Signture_1_Audit { get; set; }
        public string Signture_1_Reason { get; set; }
        public string Signture_1_Image { get; set; }
    }
}