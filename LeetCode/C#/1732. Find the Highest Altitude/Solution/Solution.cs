namespace Solution
{
    public class Solution
    {
        public int LargestAltitude(int[] gain)
        {
            var ans = 0;
            var current = 0;

            foreach (var num in gain)
            {
                current += num;

                if (current > ans)
                {
                    ans = current;
                }
            }
            return ans;
        }
    }
}