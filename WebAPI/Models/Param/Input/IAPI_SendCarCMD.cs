using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 後台對車機下命令
    /// </summary>
    public class IAPI_SendCarCMD
    {
        /// <summary>
        /// 命令(0~14遠傳cat專用，
        /// <para>0:尋車</para>
        /// <para>1:查詢萬用卡號</para>
        /// <para>2:設定萬用卡號</para>
        /// <para>3:中控解鎖，防盜關閉</para>
        /// <para>4:中控上鎖，防解啟動</para>
        /// <para>5:汽車設為有租約狀態</para>
        /// <para>6:設定無租約狀態</para>
        /// <para>7:中控解鎖</para>
        /// <para>8:中控上鎖</para>
        /// <para>9:防盜關閉</para>
        /// <para>10:防盜啟動</para>
        /// <para>11:查詢顧客卡號</para>
        /// <para>12:設定顧客卡號</para>
        /// <para>13:清除全部顧客卡號</para>
        /// <para>14:清除全部萬用卡號</para>
        ///  <para>15:興聯-尋車</para>
        ///  <para>16:興聯-汽車設為有租約狀態</para>
        ///  <para>17:興聯-設定無租約狀態</para>
        ///  <para>18:興聯-全車解鎖</para>
        ///  <para>19:興聯-全車上鎖</para>
        ///  <para>20:興聯-中控解鎖</para>
        ///  <para>21:興聯-中控上鎖</para>
        ///  <para>22:興聯-防盜解鎖</para>
        ///  <para>23:興聯-防盜上鎖</para>
        ///  <para>24:興聯-設定萬用卡號</para>
        ///  <para>25:興聯-解除萬用卡號</para>
        ///  <para>26:興聯-設定客戶卡號</para>
        ///  <para>27:興聯-解除客戶卡號</para>
        ///  <para>28:興聯-開啟NFC電源</para>
        ///  <para>29:興聯-關閉NFC電源</para>
        ///  <para>30:興聯-重啟車機</para>
        /// </summary>
        public int CmdType { set; get; }
        /// <summary>
        /// 車機編號
        /// </summary>
        public string CID { set; get; }
        /// <summary>
        /// 是否為興聯車機
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int IsCens { set; get; }
        /// <summary>
        /// 顧客卡號
        /// </summary>
        public string[] ClientCardNo { set; get; }
        /// <summary>
        /// 萬用卡號
        /// </summary>
        public string[] UnivCard { set; get; }
        /// <summary>
        /// 操作者
        /// </summary>
        public string UserId { set; get; }
    }
}