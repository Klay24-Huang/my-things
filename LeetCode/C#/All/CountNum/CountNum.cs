using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace All.CountNum
{
    public class CountNum
    {
        public int CountNum1(int number)
        {
            var count = 0;

            while (number > 0)
            {
                count += number % 2;
                number /= 2;
            }

            return count;
        }
    }
}
