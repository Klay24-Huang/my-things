using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace All.TestForeach
{
    public class TestForeach
    {
        public static void TestForeachFunc() { 
            var foo = new List<int> { 1, 2 ,3 };
            var bar = -1000;
            foo.ForEach(x => {
                x = 0;
                bar = x;
            });

            foo.ForEach(x =>
            {
                // Console.WriteLine(x);
            });
            // Console.WriteLine($"bar is {bar}");
            // Console.WriteLine("end");
        }
    }
}
