using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace All._72._Edit_Distance
{
    public class Solution
    {
        public int MinDistance(string word1, string word2)
        {
            int w1 = word1.Length, w2 = word2.Length;
            int[] prev = new int[w2 + 1];
            for (int i = 0; i <= w2; i++)
                prev[i] = i;

            for (int i = 1; i <= w1; i++)
            {
                int[] curr = new int[w2 + 1];
                curr[0] = i;
                for (int j = 1; j <= w2; j++)
                {
                    if (word1[i - 1] == word2[j - 1])
                        curr[j] = prev[j - 1];
                    else
                        curr[j] = Math.Min(prev[j - 1], Math.Min(prev[j], curr[j - 1])) + 1;
                }
                prev = curr;
            }
            return prev[w2];
        }
    }
}
