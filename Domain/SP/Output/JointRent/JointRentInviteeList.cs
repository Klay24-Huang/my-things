using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.JointRent
{
    public class JointRentInviteeList
    {
        /// <summary>
        /// 主承租人邀請時輸入的ID
        /// </summary>
        public string APPUSEID { get; set; }
        /// <summary>
        /// 身分證
        /// </summary>
        public string MEMIDNO { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string MEMCNAME { get; set; }
        /// <summary>
        /// 邀請狀態 Y = 已接受，N = 已拒絕，F = 已取消，S = 邀請中
        /// </summary>
        public string ChkType { get; set; }
        //APPUSEID,MEMIDNO,MEMCNAME,ChkType  
    }
}
