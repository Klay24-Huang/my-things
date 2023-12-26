using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace All._2352._Equal_Row_and_Column_Pairs
{
    public class Solution
    {
        public int EqualPairs(int[][] grid)
        {
            var ans = 0;
            var len = grid.Length;
            var dic = new Dictionary<string, int>();
            var separator = ",";
            for (var i = 0; i < len; i++)
            {
                var str = string.Join(separator, grid[i]);
                if (dic.ContainsKey(str))
                {
                    dic[str]++;
                    continue;
                }
                dic[str] = 1;
            }

            for ( var i = 0; i < len; i++ )
            {
                var arr = new int[len];
                for (var j = 0; j < len; j++)
                {
                    arr[j] = grid[j][i];
                }

                var str = string.Join(separator, arr);
                if (dic.ContainsKey(str))
                {
                    //// Console.WriteLine(string.Join("",arr));
                    ans += dic[str];
                }
            }
            return ans;
        }
    }
}
