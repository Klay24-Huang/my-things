using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace All._1004._Max_Consecutive_Ones_III
{
    public class Solution
    {
        public int LongestOnes(int[] nums, int k)
        {
            if (nums.All(x => x == 1))
            {
                return nums.Length;
            }

            if (nums.All(x => x == 0))
            {
                return k;
            }

            var l = 0;
            var r = 0;
            var replacing = 0;
            var ans = 0;
            while (r < nums.Length)
            {
                if (nums[r] == 1)
                {
                    r++;
                }
                else
                {
                    // zero
                    if (replacing < k)
                    {
                        replacing++;
                        r++;
                    }
                    else
                    {
                        if (nums[l] == 0)
                        {
                            // replace by first zero
                            l++;
                            r++;
                        }
                        else
                        {
                            // is one find next zero
                            while (nums[l] != 0)
                            {
                                l++;
                            }
                        }
                    }
                }
                ans = Math.Max(ans, r - l);
            }
            return ans;
        }
    }
}
