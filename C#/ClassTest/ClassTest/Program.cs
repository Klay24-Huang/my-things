using System.Text.Json;

namespace ClassTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 可以忽略欄位
            var b = new B { MyProperty = 1, Boo = 100 };
            A a = b;
            Console.WriteLine($"a is {JsonSerializer.Serialize(a)}");
            Console.WriteLine($"b is {JsonSerializer.Serialize(b)}");
        }
    }

    public class A {
        public int MyProperty { get; set; }
    }

    public class B : A
    {
        public int Boo { get; set; }
    }
}