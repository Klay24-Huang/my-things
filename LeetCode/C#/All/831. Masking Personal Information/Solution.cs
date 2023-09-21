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
            var emailReg = "^[a-zA-Z0-9._%+-]{2,}@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$";
            var isEmail = Regex.IsMatch(s, emailReg);
            if (isEmail)
            {
                var subs = s.Split("@");
                subs[0] = subs[0].ToLower();
                subs[0] = $"{subs[0].First()}*****{subs[0].Last()}";
                subs[1] = subs[1].ToLower();
                return string.Join("@", subs);
            }

            var count = 0;
            var lastfourNumb = new char[4];
            for (var i = s.Length -1; i >= 0; i--)
            {
                var c = s[i];
                var isNum = char.IsNumber(c);

                if (count < 4 && isNum)
                {
                    lastfourNumb[3-count] = c;
                }

                if(isNum)
                {
                    count++;
                }
            }

            var baseStr = "***-***-";
            var prefix = "";
            var countryCodeCount= count - 10;

            if(countryCodeCount != 0)
            {
                prefix = "+" + new string('*', countryCodeCount)+"-";
            }

            Console.WriteLine(JsonSerializer.Serialize(lastfourNumb));
            return $"{prefix}{baseStr}{string.Join("", lastfourNumb)}";

            //var phoneReg = "";
            //var isPhone = Regex.IsMatch(s, phoneReg);
            //if (isPhone)
            //{

            //}

            //return "";
        }

        //public string MaskEmail(string s)
        //{
        //    var subs = s.Split("@");
        //    subs[0] = $"{s[0]}*****";
        //    return string.Join("@", subs);
        //}

        //public string MaskPhone(string s)
        //{

        //}
    }
}
