using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCommon
{
    public class DataConvertHelper
    {

        public static DateTime? ConvertToDateTime(string DateTimeString, string CultureCode = "")
        {
            string[] DateTimeFormats = {
                            "yyyy/M/d","yyyy/MM/dd",
                            "yyyy/M/d hh:mm","yyyy/MM/dd hh:mm",
                            "yyyy/M/d hh:mm:ss","yyyy/MM/dd hh:mm:ss",
                            "yyyy/M/d tt hh:mm","yyyy/MM/dd tt hh:mm",
                            "yyyy/M/d tt hh:mm:ss","yyyy/MM/dd tt hh:mm:ss",
                            "yyyy/M/d hh:mm tt","yyyy/MM/dd hh:mm tt",
                            "yyyy/M/d hh:mm:ss tt","yyyy/MM/dd hh:mm:ss tt",

                            "yyyy/M/d h:mm","yyyy/MM/dd h:mm",
                            "yyyy/M/d h:mm:ss","yyyy/MM/dd h:mm:ss",
                            "yyyy/M/d tt h:mm","yyyy/MM/dd tt h:mm",
                            "yyyy/M/d tt h:mm:ss","yyyy/MM/dd tt h:mm:ss",
                            "yyyy/M/d h:mm tt","yyyy/MM/dd h:mm tt",
                            "yyyy/M/d h:mm:ss tt","yyyy/MM/dd h:mm:ss tt",

                            "M/d/yyyy","MM/dd/yyyy",
                            "M/d/yyyy hh:mm","MM/dd/yyyy hh:mm",
                            "M/d/yyyy hh:mm:ss","MM/dd/yyyy hh:mm:ss",
                            "M/d/yyyy tt hh:mm","MM/dd/yyyy tt hh:mm",
                            "M/d/yyyy tt hh:mm:ss","MM/dd/yyyy tt hh:mm:ss",
                            "M/d/yyyy hh:mm tt","MM/dd/yyyy hh:mm tt",
                            "M/d/yyyy hh:mm:ss tt","MM/dd/yyyy hh:mm:ss tt",

                            "M/d/yyyy h:mm","MM/dd/yyyy h:mm",
                            "M/d/yyyy h:mm:ss","MM/dd/yyyy h:mm:ss",
                            "M/d/yyyy tt h:mm","MM/dd/yyyy tt h:mm",
                            "M/d/yyyy tt h:mm:ss","MM/dd/yyyy tt h:mm:ss",
                            "M/d/yyyy h:mm tt","MM/dd/yyyy h:mm tt",
                            "M/d/yyyy h:mm:ss tt","MM/dd/yyyy h:mm:ss tt"
                        };

            IFormatProvider ifp = string.IsNullOrEmpty(CultureCode) ? CultureInfo.InvariantCulture : new CultureInfo(CultureCode, true);

            DateTime dt;
            if (DateTime.TryParseExact(DateTimeString, DateTimeFormats, ifp, DateTimeStyles.AllowWhiteSpaces, out dt))
                return dt;
            else
            {
                if (CultureCode == "zh-TW")
                {
                    return null;
                }
                else
                {
                    return ConvertToDateTime(DateTimeString, "zh-TW");
                }
            }
        }
    }
}
