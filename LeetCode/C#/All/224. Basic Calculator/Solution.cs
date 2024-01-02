using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace All._224._Basic_Calculator
{
    public class Solution
    {
        public int Calculate(string s)
        {
            var left = 0;
            var isPlus = false;
            var isMinus = false;

            foreach (char c in s)
            {
                if (c == '+')
                {

                }
                else if (c == '-')
                {

                }
                else if (c == '(')
                {

                }
                else if (c == ')')
                {

                }
                else if (c == ' ')
                {
                    continue;
                }
                else
                {
                    // number
                    var num = Convert.ToInt32(c);
                    left = left * 10 + num;
                }
            }
        }

        //private int CalculateSubstring(string s)
        //{
        //    var left = 0;
        //    var isPlus = false;
        //    var isMinus = false;
        //    foreach (char c in s)
        //    {
        //        if (c == '+')
        //        {
        //            isPlus = true;
        //        }

        //        if (c == '-')
        //        {
        //            isMinus = false;
        //        }
        //    }
        //}
    }
}
