using Domain.SP.Input;
using Domain.SP.Output;
using Domain.TB;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using WebCommon;
using System.ServiceModel.Channels;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Output;
using Domain.Common;
using static WebAPI.Models.BaseFunc.CommonFunc;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace WebAPI.Models.BaseFunc
{
    /// <summary>
    /// 共用模組
    /// 
    /// </summary>
    public class CommonFunc
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
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
            latlng
        }
        /// <summary>
        /// 基本防呆
        /// </summary>
        /// <param name="value"></param>
        /// <param name="errCode"></param>
        /// <param name="funName"></param>
        /// <returns></returns>
        public bool baseCheck(Dictionary<string, object> value, ref string errCode, string funName)
        {
            bool flag = true;
            Int16 IsSystem = 0;
            string ExceptionMessage = "";
            if (null == value)
            {
                flag = false;
                errCode = "ERR901";
            }
            if (flag)
            {
                //判斷有沒有para這個參數
                try
                {
                    if (value["para"] == null)
                    {
                        flag = false;
                        errCode = "ERR902";
                    }
                }
                catch (Exception ex)
                {
                    flag = false;
                    errCode = "ERR902";
                    IsSystem = 1;
                    ExceptionMessage = ex.Message;
                }
            }
            if (false == flag)
            {
                flag = InsErrorLog(funName, errCode, 0, 0, IsSystem, 0, ExceptionMessage);
                flag = false;

            }
            return flag;
        }
        public bool baseCheck(Dictionary<string, object> value,ref string Contentjson, ref string errCode, string funName)
        {
            bool flag = true;
            Int16 IsSystem = 0;
            string ExceptionMessage = "";
            if (null == value)
            {
                flag = false;
                errCode = "ERR901";
            }
            if (flag)
            {
                //判斷有沒有para這個參數
                try
                {
                    if (value== null)
                    {
                        flag = false;
                        errCode = "ERR902";
                    }
                    else
                    {
                        Contentjson= JsonConvert.SerializeObject(value);
                    }
                }
                catch (Exception ex)
                {
                    flag = false;
                    errCode = "ERR902";
                    IsSystem = 1;
                    ExceptionMessage = ex.Message;
                }
            }
            if (false == flag)
            {
                flag = InsErrorLog(funName, errCode, 0, 0, IsSystem, 0, ExceptionMessage);
                flag = false;

            }
            return flag;
        }
        /// <summary>
        /// 基本判斷（含token）
        /// </summary>
        /// <param name="value"></param>
        /// <param name="Contentjson"></param>
        /// <param name="errCode"></param>
        /// <param name="funName"></param>
        /// <param name="Access_token_string"></param>
        /// <param name="AccessToken"></param>
        /// <returns></returns>
        public bool baseCheck(Dictionary<string, object> value, ref string Contentjson, ref string errCode, string funName,string Access_token_string,ref string AccessToken)
        {
            bool flag = true;
            Int16 IsSystem = 0;
            string ExceptionMessage = "";
            if (Access_token_string == "")
            {
                flag = false;
                errCode = "ERR001";
            }
            else
            {
                string[] Access_tokens = Access_token_string.Split(' ');
                if (Access_tokens.Count() < 2)
                {
                    flag = false;
                    errCode = "ERR001";
                }
                else
                {
                    if(Access_tokens[0].ToUpper()!= "BEARER")
                    {
                        flag = false;
                        errCode = "ERR001";
                    }
                    else if(string.IsNullOrEmpty(Access_tokens[1]))
                    {
                        flag = false;
                        errCode = "ERR001";
                    }
                    else
                    {
                        AccessToken = Access_tokens[1];
                    }
                }
            }
            if (null == value)
            {
                flag = false;
                errCode = "ERR901";
            }
            if (flag)
            {
                //判斷有沒有para這個參數
                try
                {
                    if (value == null)
                    {
                        flag = false;
                        errCode = "ERR902";
                    }
                    else
                    {
                        Contentjson = JsonConvert.SerializeObject(value);
                    }
                }
                catch (Exception ex)
                {
                    flag = false;
                    errCode = "ERR902";
                    IsSystem = 1;
                    ExceptionMessage = ex.Message;
                }
            }
            if (false == flag)
            {
                flag = InsErrorLog(funName, errCode, 0, 0, IsSystem, 0, ExceptionMessage);
                flag = false;

            }
            return flag;
        }
        /// <summary>
        /// 僅需驗證token
        /// </summary>
        /// <param name="errCode"></param>
        /// <param name="funName"></param>
        /// <param name="Access_token_string"></param>
        /// <param name="AccessToken"></param>
        /// <param name="isGuest"></param>
        /// <returns></returns>
        public bool baseCheck(ref string errCode, string funName, string Access_token_string, ref string AccessToken, ref bool isGuest)
        {
            bool flag = true;
            Int16 IsSystem = 0;
            string ExceptionMessage = "";
            if (Access_token_string == "")
            {
                isGuest = true;
            }
            else
            {
                isGuest = false;
                string[] Access_tokens = Access_token_string.Split(' ');
                if (Access_tokens.Count() < 2)
                {
                    flag = false;
                    errCode = "ERR001";
                }
                else
                {
                    if (Access_tokens[0].ToUpper() != "BEARER")
                    {
                        flag = false;
                        errCode = "ERR001";
                    }
                    else if (string.IsNullOrEmpty(Access_tokens[1]))
                    {
                        flag = false;
                        errCode = "ERR001";
                    }
                    else
                    {
                        AccessToken = Access_tokens[1];
                    }
                }
            }

            if (false == flag)
            {
                flag = InsErrorLog(funName, errCode, 0, 0, IsSystem, 0, ExceptionMessage);
                flag = false;

            }
            return flag;
        }
        /// <summary>
        /// 基本判斷（含token）, 若無Token當訪客
        /// </summary>
        /// <param name="value"></param>
        /// <param name="Contentjson"></param>
        /// <param name="errCode"></param>
        /// <param name="funName"></param>
        /// <param name="Access_token_string"></param>
        /// <param name="AccessToken"></param>
        /// <param name="isGuest">
        ///     <para>True:訪客，不需驗證token</para>
        ///     <para>False:會員，需驗證token</para>
        /// </param>
        /// <returns></returns>
        public bool baseCheck(Dictionary<string, object> value, ref string Contentjson, ref string errCode, string funName, string Access_token_string, ref string AccessToken,ref bool isGuest)
        {
            bool flag = true;
            Int16 IsSystem = 0;
            string ExceptionMessage = "";
            if (Access_token_string == "")
            {
                isGuest = true;
            }
            else
            {
                isGuest = false;
                string[] Access_tokens = Access_token_string.Split(' ');
                if (Access_tokens.Count() < 2)
                {
                    flag = false;
                    errCode = "ERR001";
                }
                else
                {
                    if (Access_tokens[0].ToUpper() != "BEARER")
                    {
                        flag = false;
                        errCode = "ERR001";
                    }
                    else if (string.IsNullOrEmpty(Access_tokens[1]))
                    {
                        isGuest = true;
                    }
                    else
                    {
                        AccessToken = Access_tokens[1];
                    }
                }
            }
            if (null == value)
            {
                flag = false;
                errCode = "ERR901";
            }
            if (flag)
            {
                //判斷有沒有para這個參數
                try
                {
                    if (value == null)
                    {
                        flag = false;
                        errCode = "ERR902";
                    }
                    else
                    {
                        Contentjson = JsonConvert.SerializeObject(value);
                    }
                }
                catch (Exception ex)
                {
                    flag = false;
                    errCode = "ERR902";
                    IsSystem = 1;
                    ExceptionMessage = ex.Message;
                }
            }
            if (false == flag)
            {
                flag = InsErrorLog(funName, errCode, 0, 0, IsSystem, 0, ExceptionMessage);
                flag = false;

            }
            return flag;
        }
        /// <summary>
        /// 基本判斷（含token）, 若無Token當訪客
        /// </summary>
        /// <param name="value"></param>
        /// <param name="Contentjson"></param>
        /// <param name="errCode"></param>
        /// <param name="funName"></param>
        /// <param name="Access_token_string"></param>
        /// <param name="AccessToken"></param>
        /// <param name="isGuest">
        ///     <para>True:訪客，不需驗證token</para>
        ///     <para>False:會員，需驗證token</para>
        /// </param>
        /// <param name="needInput">是否有輸入參數
        /// <para>true:是</para>
        /// <para>false:否</para>
        /// </param>
        /// <returns></returns>
        public bool baseCheck(Dictionary<string, object> value, ref string Contentjson, ref string errCode, string funName, string Access_token_string, ref string AccessToken, ref bool isGuest,bool needInput)
        {
            bool flag = true;
            Int16 IsSystem = 0;
            string ExceptionMessage = "";
            if (Access_token_string == "")
            {
                isGuest = true;
            }
            else
            {
                isGuest = false;
                string[] Access_tokens = Access_token_string.Split(' ');
                if (Access_tokens.Count() < 2)
                {
                    flag = false;
                    errCode = "ERR001";
                }
                else
                {
                    if (Access_tokens[0].ToUpper() != "BEARER")
                    {
                        flag = false;
                        errCode = "ERR001";
                    }
                    else if (string.IsNullOrEmpty(Access_tokens[1]) || string.IsNullOrWhiteSpace(Access_tokens[1]))
                    {
                        //flag = false;
                        //errCode = "ERR001";
                        isGuest = true;
                    }
                    else
                    {
                        AccessToken = Access_tokens[1];
                    }
                }
            }
            if (needInput)
            {
                if (null == value)
                {
                    flag = false;
                    errCode = "ERR901";
                }
                if (flag)
                {
                    //判斷有沒有para這個參數
                    try
                    {
                        if (value == null)
                        {
                            flag = false;
                            errCode = "ERR902";
                        }
                        else
                        {
                            Contentjson = JsonConvert.SerializeObject(value);
                        }
                    }
                    catch (Exception ex)
                    {
                        flag = false;
                        errCode = "ERR902";
                        IsSystem = 1;
                        ExceptionMessage = ex.Message;
                    }
                }
                if (false == flag)
                {
                    flag = InsErrorLog(funName, errCode, 0, 0, IsSystem, 0, ExceptionMessage);
                    flag = false;

                }
            }
            
            return flag;
        }
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

        /// <summary>
        /// 寫入API呼叫記錄
        /// </summary>
        /// <param name="apiInput"></param>
        /// <param name="ClientIP"></param>
        /// <param name="funName"></param>
        /// <param name="errCode"></param>
        /// <param name="LogID"></param>
        /// <returns></returns>
        public bool InsAPLog(string apiInput, string ClientIP, string funName, ref string errCode,ref Int64 LogID)
        {
            bool flag = true;
            try
            {
                SPInput_InsAPILog spInput = new SPInput_InsAPILog()
                {
                    APIName = funName,
                    APIInput = apiInput,
                    ClientIP = ClientIP
                };
                SPOuptput_InsAPILog spOut = new SPOuptput_InsAPILog();
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                string spName = new ObjType().GetSPName(ObjType.SPType.InsAPILog);
                SQLHelper<SPInput_InsAPILog, SPOuptput_InsAPILog> sqlHelp = new SQLHelper<SPInput_InsAPILog, SPOuptput_InsAPILog>(WebApiApplication.connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                flag = !(spOut.Error == 1 || spOut.ErrorCode != "0000");
                if (spOut.ErrorCode == "0000" && lstError.Count > 0)
                {
                    spOut.ErrorCode = lstError[0].ErrorCode;
                }
                if (flag)
                {
                    LogID = spOut.LogID;
                }

                string[] tmpData = (apiInput.Replace("{", "").Replace("}", "")).Split(',');
                int tmpLen = tmpData.Length;
                string[] filter = { "\"app\":", "\"appVersion\":" };
                int appIndex = -1;
                int appVersionIndex = -1;
                int app = -1;
                int appVersion = -1;
                for (int i = 0; i < tmpLen; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        if (tmpData[i].IndexOf(filter[j]) >= 0)
                        {
                            if (j == 0)
                            {
                                app = Convert.ToInt32(tmpData[i].Split(':')[1].Replace(" ", "").Replace("\r\n", "").Replace("\"", ""));
                                break;
                            }
                            else
                            {
                                appVersion = Convert.ToInt32(tmpData[i].Split(':')[1].Replace(" ", "").Replace("\r\n", "").Replace("\"", "").Replace(".", ""));
                                break;
                            }
                        }
                    }
                    if (app > -1 && appVersion > -1)
                    {
                        break;
                    }
                }
                if (app > -1 && appVersion > -1)
                {
                    //nowVersion = Convert.ToInt32(apiInput.appVersion.Replace(".", ""));
                    string android = ConfigurationManager.AppSettings.Get("android");
                    string ios = ConfigurationManager.AppSettings.Get("ios");
                    int trueVersion = 999;
                    if (app == 0)
                    {
                        trueVersion = Convert.ToInt32(android);

                    }
                    else
                    {
                        trueVersion = Convert.ToInt32(ios);
                    }
                    if (appVersion < trueVersion)
                    {
                        flag = false;
                        errCode = "ERR102";
                    }
                }



            }
            catch (Exception ex)
            {
                //寫入檔案
               // writeErrorFile(funName, errCode, 1, "", "", ex.HResult.ToString(), ex.Message);
            }
            return flag;


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkList"></param>
        /// <param name="errList"></param>
        /// <param name="errCode"></param>
        /// <param name="funName"></param>
        /// <returns></returns>
        public bool CheckISNull(string[] checkList, string[] errList, ref string errCode, string funName,Int64 LogID)
        {
            bool flag = (errCode == "000000") ? true : false;
            if (flag)
            {
                int len = checkList.Length;
                for (int i = 0; i < len; i++)
                {
                    if (string.IsNullOrEmpty(checkList[i]))
                    {
                        flag = false;
                        errCode = errList[i];
                        break;
                    }
                }
            }

            if (false == flag)
            {
                flag = InsErrorLog(funName, errCode,0,LogID, 0, 0, "");
                flag = false;
            }
            return flag;
        }
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
                    param = @"^([A-Z]{1}(1|2){1}(\d{8}))$";
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
            }
            return param;
        }
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
        /// <summary>
        /// 產生輸出結果
        /// </summary>
        /// <typeparam name="T">class</typeparam>
        /// <param name="objOutput">要輸出的值</param>
        /// <param name="flag">執行結果
        /// <para>true:成功</para>
        /// <para>false:失敗</para>
        /// </param>
        /// <param name="errCode">錯誤代碼</param>
        /// <param name="errMsg">錯誤訊息</param>
        /// <param name="apiOutput">Data裡的資料</param>
        /// <param name="token">Token</param>
        public void GenerateOutput<T>(ref Dictionary<string, object> objOutput, bool flag, string errCode, string errMsg, T apiOutput, Token token)
  where T : class
        {

            objOutput.Add("Result", (flag == true && errCode == "000000") ? "1" : "0");
            objOutput.Add("ErrorCode", errCode);
            int NeedReloadin = (errCode == "ERR101") ? 1 : 0;
            int NeedUpgrade = (errCode == "ERR102") ? 1 : 0;

            objOutput.Add("NeedRelogin", NeedReloadin);
            objOutput.Add("NeedUpgrade", NeedUpgrade);

            if (errCode != "000000" && errCode != "ERR")
            {
                errMsg = GetErrorMsg(errCode);
            }

            objOutput.Add("ErrorMessage", errMsg);
            if (token != null) { objOutput.Add("Token", token); }
            //  if (apiOutput != null) { objOutput.Add("Data", apiOutput); }
            objOutput.Add("Data", apiOutput);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objOutput"></param>
        /// <param name="flag"></param>
        /// <param name="errCode"></param>
        /// <param name="errMsg"></param>
        /// <param name="oAPI_Base"></param>
        /// <param name="outDataAPI"></param>
        public void GenerateOutput<T>(ref Dictionary<string, object> objOutput, bool flag, string errCode, string errMsg, OAPI_Base oAPI_Base, T outDataAPI, bool NeedReLogin, bool NeedUpgrade)
        where T : class
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// /// <typeparam name="TBase"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="objOutput"></param>
        /// <param name="flag"></param>
        /// <param name="errCode"></param>
        /// <param name="errMsg"></param>
        /// <param name="outBaseAPI"></param>
        /// <param name="outDataAPI"></param>
        public void GenerateOutput<TBase, T>(ref Dictionary<string, object> objOutput, bool flag, string errCode, string errMsg, TBase outBaseAPI, T outDataAPI)
        where TBase : class
        where T : class
        {

        }
        /// <summary>
        /// 取得Client IP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string GetClientIp(HttpRequestMessage request = null)
        {


            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            //else if (request.Properties.ContainsKey(System.ServiceModel.Channels.RemoteEndpointMessageProperty.Name))
            //{
            //    RemoteEndpointMessageProperty prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
            //    return prop.Address;
            //}
            else if (HttpContext.Current != null)
            {
                return HttpContext.Current.Request.UserHostAddress;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 基本防呆
        /// </summary>
        /// <param name="checkList">需檢核的參數</param>
        /// <param name="errList">檢核失敗所對應的參數列表</param>
        /// <param name="errCode">錯誤代碼</param>
        /// <param name="funName">呼叫的功能</param>
        /// <param name="ErrType">錯誤類型</param>
        /// <param name="LogID">LogID</param>
        /// <returns></returns>
        public bool CheckISNull(string[] checkList, string[] errList, ref string errCode, string funName, Int16 ErrType, Int64 LogID)
        {
            bool flag = (errCode == "000000") ? true : false;
            if (flag)
            {
                int len = checkList.Length;
                for (int i = 0; i < len; i++)
                {
                    if (string.IsNullOrEmpty(checkList[i]))
                    {
                        flag = false;
                        errCode = errList[i];
                        break;
                    }
                }
            }

            if (false == flag)
            {
                flag = InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 寫入錯誤到TB
        /// </summary>
        /// <param name="funName">功能名稱</param>
        /// <param name="errCode">錯誤代碼</param>
        /// <param name="ErrType">錯誤類型
        /// <para>0:一般錯誤</para>
        /// <para>1:金流回傳</para>
        /// <para>2:短租回傳</para>
        /// <para>3:車機回傳</para>
        /// <para>4:執行SQL發生錯誤</para>
        /// </param>
        /// <param name="LogID">造成此錯誤的API LOG</param>
        /// <param name="IsSystem">
        /// <para>0:功能錯誤</para>
        /// <para>1:系統錯誤</para>
        /// </param>
        /// <param name="SQLErrorCode">SQL Error Code</param>
        /// <param name="SQLErrorDesc">SQL Error Message</param>
        /// <returns></returns>
        public bool InsErrorLog(string funName, string errCode, Int16 ErrType, Int64 LogID, Int16 IsSystem, int SQLErrorCode, string SQLErrorDesc)
        {
            bool flag = false;
            try
            {
                SPInput_InsError spInput = new SPInput_InsError()
                {

                    FunName = funName,
                    ErrorCode = errCode,
                    IsSystem = IsSystem,
                    ErrType = ErrType,
                    LogID = LogID,
                    SQLErrorCode = SQLErrorCode,
                    SQLErrorDesc = SQLErrorDesc
                };
                SPOutput_Base spOut = new SPOutput_Base();
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                string spName = new ObjType().GetSPName(ObjType.SPType.InsError);
                SQLHelper<SPInput_InsError, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_InsError, SPOutput_Base>(WebApiApplication.connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                flag = !(spOut.Error == 1 || spOut.ErrorCode != "0000");
                if (spOut.ErrorCode == "0000" && lstError.Count > 0)
                {
                    spOut.ErrorCode = lstError[0].ErrorCode;
                }
                if (false == flag)
                {
                    errCode = spOut.ErrorCode;
                    //   writeErrorFile(funName, errCode, IsSystem, spOut.SQLExceptionCode, spOut.SQLExceptionMsg, "", ""); //20200626封印，先暫時不寫文字log
                }
            }
            catch (Exception ex)
            {
                //寫入檔案
                // writeErrorFile(funName, errCode, IsSystem,"", "", ex.HResult.ToString(), ex.Message); //20200626封印，先暫時不寫文字log
            }
            return flag;
        }
        /// <summary>
        /// 取出錯誤訊息
        /// </summary>
        /// <param name="errCode">錯誤代碼</param>
        /// <returns>錯誤訊息（正體中文）</returns>
        public string GetErrorMsg(string errCode)
        {
            string Message = "";
            CommonRepository commonRepository = new CommonRepository(connetStr);
            ErrorMessageList errObj = commonRepository.GetErrorMessage(errCode);
            if (errObj != null)
            {
                Message = errObj.ErrMsg;
            }
            return Message;
        }
        /// <summary>
        /// 取出錯誤訊息（多語系）
        /// </summary>
        /// <param name="errCode"></param>
        /// <param name="Language">語系
        /// <para>0:正體中文</para>
        /// <para>1:簡體中文</para>
        /// <para>2:英文</para>
        /// <para>3:日文</para>
        /// <para>4:韓文</para>
        /// </param>
        /// <returns>錯誤訊息</returns>
        public string GetErrorMsg(string errCode, int Language)
        {
            string Message = "";
            CommonRepository commonRepository = new CommonRepository(connetStr);
            ErrorMessageList errObj = commonRepository.GetErrorMessage(errCode, Language);
            if (errObj != null)
            {
                Message = errObj.ErrMsg;
            }
            return Message;
        }

        #region SQL相關
        /// <summary>
        /// 驗證SP回傳值
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="spOut"></param>
        /// <param name="lstError"></param>
        /// <param name="errCode"></param>
        public void checkSQLResult(ref bool flag, ref SPOutput_Base spOut, ref List<ErrorInfo> lstError, ref string errCode)
        {
            if (flag)
            {
                if (spOut.Error == 1)
                {
                    lstError.Add(new ErrorInfo() { ErrorCode = spOut.ErrorCode });
                    errCode = spOut.ErrorCode;
                    flag = false;
                }
                else
                {
                    if (spOut.ErrorCode != "0000")
                    {
                        lstError.Add(new ErrorInfo() { ErrorCode = spOut.ErrorCode });
                        errCode = spOut.ErrorCode;
                        flag = false;
                    }
                }
            }
            else
            {
                if (lstError.Count > 0)
                {
                    errCode = lstError[0].ErrorCode;
                }
            }
        }
        /// <summary>
        /// 驗證SP回傳值
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="Error"></param>
        /// <param name="ErrorCode"></param>
        /// <param name="lstError"></param>
        /// <param name="errCode"></param>
        public void checkSQLResult(ref bool flag, int Error, string ErrorCode, ref List<ErrorInfo> lstError, ref string errCode)
        {
            if (flag)
            {
                if (Error == 1)
                {
                    lstError.Add(new ErrorInfo() { ErrorCode = ErrorCode });
                    errCode = ErrorCode;
                    flag = false;
                }
                else
                {
                    if (ErrorCode != "0000")
                    {
                        lstError.Add(new ErrorInfo() { ErrorCode = ErrorCode });
                        errCode = ErrorCode;
                        flag = false;
                    }
                }
            }
            else
            {
                if (lstError.Count > 0)
                {
                    errCode = lstError[0].ErrorCode;
                }
            }
        }
        #endregion
    }
}