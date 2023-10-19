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
            if (string.IsNullOrEmpty(word1))
                return word2.Length;

            if (string.IsNullOrEmpty(word2))
                return word1.Length;

            int[][] dp = new int[word1.Length][];

            for (var i = 0; i < word1.Length; i++)
            {
                dp[i] = new int[word2.Length];
                var prev = i - 1 < 0 ? 0 : dp[i - 1][0];
                dp[i][0] = prev + (word1[i] != word2[0] ? 1 : 0);
            }

            for (var j = 0; j < word2.Length; j++)
            {
                var prev = j - 1 < 0 ? 0 : dp[0][j - 1];
                dp[0][j] = prev + (word1[0] != word2[j] ? 1 : 0);
            }

            for (var i = 1; i < dp.Length; i++)
            {
                for (var j = 1; j < dp[i].Length; j++)
                {
                    var preMax = new int[] { dp[i - 1][j - 1], dp[i - 1][j], dp[i][j - 1] }.Min();
                    if (word1[i] != word2[j])
                    {
                        dp[i][j] = 1 + preMax;
                    }
                    else
                    {
                        dp[i][j] = preMax;
                    }
                }
            }
            Console.WriteLine(JsonSerializer.Serialize(dp));
            return dp[word1.Length - 1][word2.Length - 1];
        }
    }
}
