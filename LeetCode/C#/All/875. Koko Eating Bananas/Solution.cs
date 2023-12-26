using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace All._875._Koko_Eating_Bananas
{
    public class Solution
    {
        public int MinEatingSpeed(int[] piles, int h)
        {
            Array.Sort(piles);
            var n = piles.Length;

            if(n == 1)
            {
                decimal pile = piles[0];
                return (int)Math.Ceiling(pile / h);
            }

            var l = 0;
            var r = piles[n - 1];
            int action(int hour, int speedToEat)
            {
                for (var i = n - 1; i >= 0; i--)
                {
                    decimal pile = piles[i];
                    hour -= (int)Math.Ceiling(pile / speedToEat);
                    // Console.WriteLine($"current eat hour: {(int)Math.Ceiling(pile / speedToEat)}");
                    // Console.WriteLine($"hour is {hour}");

                    if (hour < 0)
                    {
                        break;
                    }
                }
                return hour;
            }

            while (l <= r)
            {
                int speedToEat = (l + r) / 2;
                // Console.WriteLine($"eat speed {speedToEat}");
                // Console.WriteLine($"l: {l}, r: {r}");
                var hour = h;
                hour = action(hour, speedToEat);
                // Console.WriteLine($"end, hour: {hour}");

                if (hour == 0)
                {
                    if (action(h, speedToEat - 1) < 0)
                    {
                        return speedToEat;
                    }
                    else
                    {
                        r = speedToEat - 1;
                    }
                }
                else if (hour < 0)
                {
                    l = speedToEat + 1;
                }
                else
                {
                    // hour > 0
                    r = speedToEat - 1;
                }
            }
            return -1;
        }
    }
}
