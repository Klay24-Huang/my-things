using System;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Web.Utilities
{
    /// <summary>
    /// WebApiClient 的摘要描述
    /// </summary>
    public class WebApiClient : WebApiClientBase
    {
        static readonly string LOGIN_API = "API/LOGIN";
        static readonly string SPRETB_API = "API/SPRetB";
        static readonly string EXESP_API = "API/ExeSP";
        static readonly string EXEBATCHSP_API = "API/ExeBatchSP";
        static readonly string EXESPB2_API = "API/ExeSPB2";
        static readonly string EXESPT2_API = "API/ExeSPT2";
        static readonly string EXESPT3_API = "API/ExeSPT3";
        static readonly string EXESPSS_API = "API/ExeSPSS";
        static readonly string EXEMULTIARR_API = "API/ExeMultiArr";

        //20130814 ADD BY JERRY
        public static string LOGIN(string ACCOUNT, string PASSWD, ref string MESSAGE)
        {
            try
            {
                var result = new
                {
                    USRID = ACCOUNT,
                    PSWD = PASSWD,
                    TokenTimeout = "0"
                };

                JObject reader = SpRetBase(result, LOGIN_API);

                //20130814 ADD BY JERRY 增加Token機制
                SetToken(reader);

                MESSAGE = reader["MESSAGE"].ToString();
                return reader["Token"].ToString(); ;
            }
            catch (Exception ex)
            {
                MESSAGE = "WebApiClient error:" + ex.Message;
                return "";
            }
        }
        public static DataSet SPRetB(string[] SVRNM, string SPNM, object[] PARMS, ref string MESSAGE)
        {
            try
            {
                //var result = new
                //{
                //    SPNM = SPNM,
                //    SVRNM = from a in SVRNM select a,
                //    PARMS = from b in PARMS select b
                //};
                var result = new
                {
                    SPNM = SPNM,
                    SVRNM = SVRNM,
                    PARMS = PARMS
                };
                JObject reader = SpRetBase(result, SPRETB_API);

                //20130814 ADD BY JERRY 增加Token機制
                SetToken(reader);

                DataSet ds = JsonConvert.DeserializeObject<DataSet>(reader["DATA"].ToString());
                MESSAGE = reader["MESSAGE"].ToString();
                return ds;
            }
            catch (Exception ex)
            {
                MESSAGE = "WebApiClient error:" + ex.Message;
                return new DataSet();
            }
        }

        public static bool ExeSP(string[] SVRNM, string SPNM, object[] PARMS, ref string MESSAGE)
        {
            try
            {

                var result = new
                {
                    SPNM = SPNM,
                    SVRNM = SVRNM,
                    PARMS = PARMS
                };


                JObject reader = SpRetBase(result, EXESP_API);

                //20130814 ADD BY JERRY 增加Token機制
                SetToken(reader);

                MESSAGE = reader["MESSAGE"].ToString();
                return Convert.ToBoolean(reader["RESULT"].ToString());
            }
            catch (Exception ex)
            {
                MESSAGE = "WebApiClient error:" + ex.Message;
                return false;
            }
        }
        public static bool ExeBatchSP(string[] SVRNM, string SPNM, object[,] PARMS, ref string MESSAGE)
        {
            try
            {
                //用Linq直接組
                var result = new
                {
                    SPNM = SPNM,
                    SVRNM = SVRNM,
                    PARMS = PARMS
                };

                JObject reader = SpRetBase(result, EXEBATCHSP_API);

                //20130814 ADD BY JERRY 增加Token機制
                SetToken(reader);

                MESSAGE = reader["MESSAGE"].ToString();
                return Convert.ToBoolean(reader["RESULT"].ToString());
            }
            catch (Exception ex)
            {
                MESSAGE = "WebApiClient error:" + ex.Message;
                return false;
            }
        }
        public static bool ExeSPB2(string[] SVRNM, object[,] PARMS, object[,] PARMS1, string SPNM, string SPNM1, ref string MESSAGE)
        {
            try
            {

                //用Linq直接組
                var result = new
                {
                    SPNM = SPNM,
                    SPNM1 = SPNM1,
                    SVRNM = SVRNM,
                    PARMS = PARMS,
                    PARMS1 = PARMS1
                };

                JObject reader = SpRetBase(result, EXESPB2_API);

                //20130814 ADD BY JERRY 增加Token機制
                SetToken(reader);

                MESSAGE = reader["MESSAGE"].ToString();
                return Convert.ToBoolean(reader["RESULT"].ToString());
            }
            catch (Exception ex)
            {
                MESSAGE = "WebApiClient error:" + ex.Message;
                return false;
            }
        }
        public static bool ExeSPT2(string[] SVRNM, object[] PARMS, object[,] PARMS1, string SPNM, string SPNM1, ref string MESSAGE)
        {
            try
            {

                //用Linq直接組
                var result = new
                {
                    SPNM = SPNM,
                    SPNM1 = SPNM1,
                    SVRNM = SVRNM,
                    PARMS = PARMS,
                    PARMS1 = PARMS1
                };


                // Send the request.
                //HttpResponseMessage resp = Client.PostAsync("API/ExeSPT2", Content).Result;


                JObject reader = SpRetBase(result, EXESPT2_API);

                //20130814 ADD BY JERRY 增加Token機制
                SetToken(reader);

                MESSAGE = reader["MESSAGE"].ToString();
                return Convert.ToBoolean(reader["RESULT"].ToString());
            }
            catch (Exception ex)
            {
                MESSAGE = "WebApiClient error:" + ex.Message;
                return false;
            }
        }
        public static bool ExeSPT3(string[] SVRNM, object[] PARMS, object[,] PARMS1, string SPNM, string SPNM1, ref string MESSAGE)
        {
            try
            {

                //用Linq直接組
                var result = new
                {
                    SPNM = SPNM,
                    SPNM1 = SPNM1,
                    SVRNM = SVRNM,
                    PARMS = PARMS,
                    PARMS1 = PARMS1
                };

                JObject reader = SpRetBase(result, EXESPT3_API);

                //20130814 ADD BY JERRY 增加Token機制
                SetToken(reader);

                MESSAGE = reader["MESSAGE"].ToString();
                return Convert.ToBoolean(reader["RESULT"].ToString());
            }
            catch (Exception ex)
            {
                MESSAGE = "WebApiClient error:" + ex.Message;
                return false;
            }
        }
        public static bool ExeSPSS(string[] SVRNM, object[,] PARMS, object[,] PARMS1, object[,] PARMS2, object[,] PARMS3, string SPNM, string SPNM1, string SPNM2, string SPNM3, ref string MESSAGE)
        {
            try
            {

                //用Linq直接組
                var result = new
                {
                    SPNM = SPNM,
                    SPNM1 = SPNM1,
                    SPNM2 = SPNM2,
                    SPNM3 = SPNM3,
                    SVRNM = SVRNM,
                    PARMS = PARMS,
                    PARMS1 = PARMS1,
                    PARMS2 = PARMS2,
                    PARMS3 = PARMS3
                };


                // Send the request.
                //HttpResponseMessage resp = Client.PostAsync("API/ExeSPSS", Content).Result;


                JObject reader = SpRetBase(result, EXESPSS_API);

                //20130814 ADD BY JERRY 增加Token機制
                SetToken(reader);

                MESSAGE = reader["MESSAGE"].ToString();
                return Convert.ToBoolean(reader["RESULT"].ToString());
            }
            catch (Exception ex)
            {
                MESSAGE = "WebApiClient error:" + ex.Message;
                return false;
            }
        }
        public static bool ExeMultiArr(string[][] SVRNM, string[] SPNM, object[][,] PARMS, object[][,] PARMARRS, ref string MESSAGE)
        {
            try
            {

                //用Linq直接組
                var result = new
                {
                    SPNM = SPNM,
                    PARMARR = PARMS,
                    SVRNM = SVRNM,
                    SP_PARMARR = PARMARRS
                };

                JObject reader = SpRetBase(result, EXEMULTIARR_API);

                //20130814 ADD BY JERRY 增加Token機制
                SetToken(reader);

                MESSAGE = reader["MESSAGE"].ToString();
                return Convert.ToBoolean(reader["RESULT"].ToString());
            }
            catch (Exception ex)
            {
                MESSAGE = "WebApiClient error:" + ex.Message;
                return false;
            }
        }


    }
}