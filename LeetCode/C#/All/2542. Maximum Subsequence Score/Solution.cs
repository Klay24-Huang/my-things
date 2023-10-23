using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace All._2542._Maximum_Subsequence_Score
{
    public class Solution
    {
        public long MaxScore(int[] nums1, int[] nums2, int k)
        {
            Array.Sort(nums2, nums1);
            var sub1 = nums1.Take(k);
            var sub2 = nums2.Take(k);
            long ans = sub1.Sum() * sub2.Min();
            
            
            for (var i = k; i < nums1.Length; i++)
            {
                //Console.WriteLine(i);
                //Console.WriteLine(k < nums1.Length);
                sub1 = sub1.Skip(1);
                sub1 = sub1.Append(nums1[i]);
                sub2 = sub2.Skip(1);
                sub2 = sub2.Append(nums2[i]);
                long r = sub1.Sum() * sub2.Min();
                if (r > ans)
                    ans = r;
            }
            return ans;
        }
    }
}
