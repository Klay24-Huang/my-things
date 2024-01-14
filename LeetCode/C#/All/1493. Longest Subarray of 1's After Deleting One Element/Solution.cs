using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;

namespace All._1493._Longest_Subarray_of_1_s_After_Deleting_One_Element
{
    public class Solution
    {
        public int LongestSubarray(int[] nums)
        {
            var ans = 0;
            var hasZero = false;
            var prev = 0;
            var curr = 0;
            for (var i = 0; i < nums.Length; i++)
            {
                var num = nums[i];
                if (num == 0)
                {
                    hasZero = true;
                    if (i - 1 >= 0 && nums[i - 1] == 0)
                    {
                        // two zero continue; chain break;
                        prev = 0;
                    }

                    // meet first zero after chain

                    // calculate result
                    var r = curr + prev;
                    ans = r > ans ? r : ans;
                    //// Console.WriteLine($"curr {curr}, prev {prev}");
                    prev = curr;
                    curr = 0;
                }
                else
                {
                    // is one
                    curr++;
                }
            }

            if (curr > 0)
            {
                // calculate result
                var r = curr + prev;
                ans = r > ans ? r : ans;
            }

            ans = !hasZero ? --ans : ans;

            return ans;
        }
    }
}
