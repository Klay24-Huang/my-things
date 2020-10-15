using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 對機車下指令
    /// </summary>
    public class IAPI_SendMotorCMD
    {
        /// <summary>
        /// 命令
        /// <para>0:設定租約狀態</para>
        ///  <para>1:解除租約狀態</para>
        /// <para>2:開啟電源</para>
        /// <para>3:關閉電源</para>
        /// <para>4:啟動喇叭尋車功能</para>
        /// <para>5:啟動閃燈尋車功能</para>
        /// <para>6:開啟坐墊</para>
        /// <para>7:開啟/關閉電池蓋</para>
        /// <para>8:中控解鎖</para>
        /// </summary>

        public int CmdType { set; get; }
        /// <summary>
        /// 車機編號
        /// </summary>
        public string CID { set; get; }
        /// <summary>
        /// 遠傳車機編號
        /// </summary>
        public string deviceToken { set; get; }
        public string BLE_Code { set; get; }
        /// <summary>
        /// 操作者
        /// </summary>
        public string UserId { set; get; }

    }
}