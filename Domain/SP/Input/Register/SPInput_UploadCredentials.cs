using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Register
{
    public class SPInput_UploadCredentials
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 裝置ID
        /// </summary>
        public string DeviceID { set; get; }
        /// <summary>
        /// 模式
        /// <para>0:新增</para>
        /// <para>1:修改</para>
        /// </summary>
        public Int16 Mode { set; get; }
        /// <summary>
        /// 證件照類型
        /// <para>1:身份證正面</para>
        /// <para>2:身份證反面</para>
        /// <para>3:汽車駕照正面</para>
        /// <para>4:汽車駕照反面</para>
        /// <para>5:機車駕證正面</para>
        /// <para>6:機車駕證反面</para>
        /// <para>7:自拍照</para>
        /// <para>8:法定代理人</para>
        /// <para>9:其他（如台大專案）</para>
        /// <para>10:企業用戶</para>
        /// </summary>
        public Int16 CrentialsType{set;get;}
        /// <summary>
        /// 圖檔（使用base64)
        /// </summary>
        public string CrentialsFile{set;get;}
        /// <summary>
        /// 
        /// </summary>
        public Int64 LogID { set; get; }
    }
 }
