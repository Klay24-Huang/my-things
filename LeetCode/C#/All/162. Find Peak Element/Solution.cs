using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace All._162._Find_Peak_Element
{
    public class Solution
    {
        public int FindPeakElement(int[] nums)
        {
            var l = 0;
            var r = nums.Length;
            while (l <= r)
            {
                var i = (l + r) / 2;
                // Console.WriteLine("index" + i);
                if ((i - 1 < 0 || nums[i] > nums[i - 1]) && (i + 1 == nums.Length || nums[i] > nums[i + 1]))
                {
                    return i;
                }

                if ((i + 1 < nums.Length) && (nums[i + 1] > nums[i]))
                {
                    l = ++i;
                }
                else
                {
                    r = --i;
                }
                // Console.WriteLine($"i {l}, j {r}");
            }
            return -1;
        }
    }
}
