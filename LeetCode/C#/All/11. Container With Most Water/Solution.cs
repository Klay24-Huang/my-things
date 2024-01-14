using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace All._11._Container_With_Most_Water
{
    public class Solution
    {
        public int MaxArea(int[] height)
        {
            var l = 0;
            var r = height.Length - 1;
            var ans = 0;
            while (l < r)
            {
                var curr = (r - l) * Math.Min(height[r], height[l]);
                // Console.WriteLine(curr);
                ans = Math.Max(ans, curr);
                
                if (height[r] > height[l])
                {
                    l++;
                }else
                {
                    r--;
                }
            }
            return ans;
        }
    }
}
