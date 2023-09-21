using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace All._831._Masking_Personal_Information
{
    public class Solution
    {
        public string MaskPII(string s)
        {
            var indexOfAt = s.IndexOf("@");
            // is email
            if (indexOfAt >= 0)
            {
                return $"{s[0]}*****{s[(indexOfAt - 1)..]}".ToLower();
            }

            // is phone
            var removeChar = new string[] { " ", "(", ")", "-", "+" };
            foreach (var c in removeChar)
            {
                s = s.Replace(c, string.Empty);
            }
            var common = "***-***-";
            var countyCodeCount = s.Length - 10;
            var prefix = countyCodeCount > 0 ? $"+{new string('*', countyCodeCount)}-" : "";
            return $"{prefix}{common}{s[(s.Length-4)..]}";
        }
    }
}
