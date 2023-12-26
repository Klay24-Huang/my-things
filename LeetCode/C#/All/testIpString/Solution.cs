using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace All.testIpString
{
    public class Solution
    {
        public void Test()
        {
            var ips = new List<string> {
                "211.72.238.168, 147.243.138.115:50526",
                "2401:e180:8883:9c50:6c30:4189:58c4:3066, 147.243.31.203:44609"
            };

            foreach (var ip in ips)
            {
                // Console.WriteLine(ip.Split(new char[] { ',' }).FirstOrDefault());
            }
        }
    }
}
