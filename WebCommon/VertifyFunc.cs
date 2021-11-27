using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using WebCommon;

namespace WebCommon
{
    /// <summary>
    /// 驗證模組 
    /// </summary>
    public class VertifyFunc
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        #region 要驗證的類型
        /// <summary>
        /// 要驗證的類型
        /// </summary>
        public enum CheckType
        {
            /// <summary>
            /// 身份證
            /// </summary>
            IDNO,
            /// <summary>
            /// 外國居留證
            /// </summary>
            FIDNO,  //外國居留證
            /// <summary>
            /// 手機
            /// </summary>
            Mobile,
            PWD,
            InvoiceType,
            SyncAddress,
            Price,
            City,
            ZipCode,
            MoroCMD,
            latlng,
            /// <summary>
            /// 統編
            /// </summary>
            UniCode,
            HotaiPWD
        }
        #endregion

        #region 判斷日期
        /// <summary>
        /// 判斷日期
        /// </summary>
        /// <param name="ISDate">起始日期</param>
        /// <param name="IEDate">結束日期</param>
        /// <param name="errCode">錯誤代碼</param>
        /// <returns>boolean</returns>
        public bool CheckDate(string ISDate, string IEDate, ref string errCode, ref DateTime SDate, ref DateTime EDate)
        {
            bool flag = true; ;


            //判斷日期
            if (flag)
            {
                if (string.IsNullOrWhiteSpace(ISDate) == false && string.IsNullOrWhiteSpace(IEDate) == false)
                {
                    flag = DateTime.TryParse(ISDate, out SDate);
                    if (flag)
                    {
                        flag = DateTime.TryParse(IEDate, out EDate);
                        if (flag)
                        {
                            if (SDate >= EDate)
                            {
                                flag = false;
                                errCode = "ERR153";
                            }
                            else
                            {
                                if (DateTime.Now > SDate)
                                {
                                    flag = false;
                                    errCode = "ERR154";
                                }

                            }

                        }
                        else
                        {
                            errCode = "ERR152";
                        }
                    }
                    else
                    {
                        errCode = "ERR151";
                    }
                }
            }
            return flag;
        }

        /// <summary>
        /// 顯示天
        /// </summary>
        /// <param name="DateTime1">結束日</param>
        /// <param name="DateTime2">起始日</param>
        /// <returns></returns>
        public string DateDiff(DateTime DateTime1, DateTime DateTime2)
        {
            string dateDiff = null;
            TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
            TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            dateDiff = ts.Days.ToString() + "天" + ts.Hours.ToString() + "小時" + ts.Minutes.ToString() + "分鐘";// + ts.Seconds.ToString() + "秒";
            return dateDiff;
        }
        #endregion

        #region 正規化比對
        /// <summary>
        /// 正規化比對
        /// </summary>
        /// <param name="sourceStr">原始文字</param>
        /// <param name="regex">列舉型別RegexKind</param>
        /// <returns>回傳Boolean
        ///			<para>true:成功</para>
        ///			<para>false:失敗</para>		
        /// </returns>
        public bool regexStr(string sourceStr, CheckType regex)
        {
            string regexParam = getRegStr(regex);
            Regex r = new Regex(regexParam, RegexOptions.None);
            Match m = r.Match(sourceStr);
            return m.Success;
        }
        #endregion 

        #region 身份證驗證公式
        /// <summary>
        /// 身份證驗證公式
        /// </summary>
        /// <param name="IDNo"></param>
        /// <returns>
        /// bool
        /// <para>true:合法</para>
        /// <para>false:不合法</para>
        /// </returns>
        public bool checkIDNO(string IDNo)
        {
            bool flag = false;

            if (IDNo.Length == 10)
            {
                IDNo = IDNo.ToUpper();
                string regexParam = getRegStr(CheckType.IDNO);
                Regex r = new Regex(regexParam, RegexOptions.IgnoreCase);
                Match m = r.Match(IDNo);
                if (m.Success)
                {
                    if (IDNo[0] >= 'A' && IDNo[0] <= 'Z')
                    {
                        var a = new[] { 10, 11, 12, 13, 14, 15, 16, 17, 34, 18, 19, 20, 21, 22, 35, 23, 24, 25, 26, 27, 28, 29, 32, 30, 31, 33 };
                        var b = new int[11];
                        b[1] = a[(IDNo[0]) - 65] % 10;
                        var c = b[0] = a[(IDNo[0]) - 65] / 10;
                        for (var i = 1; i <= 9; i++)
                        {
                            b[i + 1] = IDNo[i] - 48;
                            c += b[i] * (10 - i);
                        }
                        if (((c % 10) + b[10]) % 10 == 0)
                        {
                            flag = true;
                        }
                    }
                }
                else
                {
                    regexParam = getRegStr(CheckType.FIDNO);
                    r = new Regex(regexParam, RegexOptions.IgnoreCase);
                    m = r.Match(IDNo);
                    if (m.Success)
                    {
                        //直接將加乘後的結果寫上去例如A=>10=>(第一位數*1)%10+(第二位數*9)%10=1，B=1+9=10......
                        int[] pidResidentFirstInt = { 1, 10, 9, 8
                                                    , 7, 6, 5, 4
                                                    , 9, 3, 2, 2
                                                    , 11, 10, 8, 9
                                                    , 8, 7, 6, 5
                                                    , 4, 3, 11, 3
                                                    , 12, 10 };
                        //將第二碼由英文轉數字取尾數*8，例A=>0=>0*8=>0,B=>11=>1*8=8;C=>12=(2*8)%10=6;D=>13=(3*8)%10=4
                        int[] second = { 0, 8, 6, 4 };
                        int sum = pidResidentFirstInt[(IDNo[0]) - 65];
                        string muti = "7654321";
                        sum += second[IDNo[1] - 65];
                        for (int i = 2; i < 9; i++)
                        {
                            sum += ((((IDNo[i] - 48) * (muti[i - 2] - 48))) % 10);
                        }
                        int vc = sum % 10;
                        if ((vc == 0 && vc == (IDNo[9] - 48)) || ((10 - vc) == (IDNo[9] - 48)))
                        {
                            flag = true;
                        }

                    }
                }

            }
            return flag;
        }
        #endregion

        #region 統一編號驗證公式
        /// <summary>
        /// 統一編號驗證公式
        /// </summary>
        /// <param name="UniNum"></param>
        /// <returns>
        /// bool
        /// <para>true:合法</para>
        /// <para>false:不合法</para>
        /// </returns>
        public bool checkUniNum(string UniNum)
        {
            bool flag = false;
            if (UniNum.Length == 8)
            {
                if (UniNum == "00000000")
                {
                    return flag;
                }

                var cx = new[] { 1, 2, 1, 2, 1, 2, 4, 1 };
                var sum = 0;

                var cnum = UniNum.ToCharArray();
                for (int i = 0; i < cnum.Length; i++)
                {
                    int temp;
                    if (!int.TryParse(cnum[i].ToString(), out temp))
                    {
                        return flag;
                    }

                    sum += cc(temp * cx[i]);


                }
                if (sum % 10 == 0)
                {
                    flag = true;
                }
                else if (cnum[6].ToString() == "7" && (sum + 1) % 10 == 0)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }


            }
            return flag;
        }

        public int cc(int n)
        {
            if (n > 9)
            {
                var s = n.ToString() + "";
                int n1 = Convert.ToInt32(s.Substring(0, 1)) * 1;
                int n2 = Convert.ToInt32(s.Substring(1, 1)) * 1;
                n = n1 + n2;
            }
            return n;
        }
        #endregion

        #region 取出正規化表示法
        /// <summary>
        /// 取出正規化表示法
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string getRegStr(CheckType type)
        {
            string param = "";
            switch (type)
            {
                case CheckType.Mobile:
                    param = @"^([+]8869|09)(\d{8})$";
                    break;
                case CheckType.IDNO:
                    param = @"^([A-Z]{1}(1|2|8|9){1}(\d{8}))$";
                    break;
                case CheckType.FIDNO:
                    param = @"^[A-Z]{1}[A-D]{1}[0-9]{8}$";
                    break;
                case CheckType.PWD:
                    param = @"(?!^[A-Za-z~!\?@#\$%&\^\+\-\*/]*$)(?!^[0-9%s]~!\?@#\$%&\^\+\-\*/)^[A-Za-z0-9~!\?@#\$%&\^\+\-\*/]{6,16}";
                    break;
                case CheckType.InvoiceType:
                    param = @"^([1-7]){1}$";
                    break;
                case CheckType.MoroCMD:
                    param = @"^([1-7]){1}$";
                    break;
                case CheckType.SyncAddress:
                    param = @"^(0|1)$";
                    break;
                case CheckType.Price:
                    param = @"^[1-9]{1}(\d{2,})$";
                    break;
                case CheckType.City:
                    param = @"^[0-9]{1,2}$";
                    break;
                case CheckType.ZipCode:
                    param = @"^[1-9]{1}(\d{2,5})$";
                    break;
                case CheckType.latlng:
                    param = @"^[1-9]{1}[0-9]{1,2}[.]{1}[0-9]{4,}$";
                    break;
                case CheckType.HotaiPWD:
                    param = @"^(?=.*\d)(?=.*[a-zA-Z]).{6,12}$";
                    break;
            }
            return param;
        }
        #endregion

        #region 隨機亂碼
        /// <summary>
        /// 隨機亂碼
        /// </summary>
        /// <param name="mode">模式
        /// <para>0:大寫英文</para>
        /// <para>1:數字</para>
        /// </param>
        /// <returns></returns>
        public string getRndCode(int mode)
        {

            string returnStr = "";
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            if (mode == 0)
            {
                returnStr = Convert.ToChar(rand.Next(65, 90)).ToString();
            }
            else
            {
                returnStr = Convert.ToChar(rand.Next(48, 57)).ToString();
            }


            return returnStr;
        }
        #endregion

        #region 產生簡訊驗證碼
        /// <summary>
        /// 產生簡訊驗證碼
        /// </summary>
        /// <param name="min">最小數</param>
        /// <param name="max">最大數</param>
        /// <returns></returns>
        public string getRand(int min, int max)
        {
            Random rnd = new Random();
            int maxLen = max.ToString().Length;
            return rnd.Next(min, max).ToString().PadLeft(maxLen, '0');
        }
        #endregion

        #region 判斷如果null就回傳空字串，否則就回傳值
        /// <summary>
        /// 判斷如果null就回傳空字串，否則就回傳值
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public string BaseCheckString(string Source)
        {
            return (string.IsNullOrWhiteSpace(Source) ? "" : Source);
        }
        #endregion

    }
}