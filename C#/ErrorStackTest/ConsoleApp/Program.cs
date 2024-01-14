namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                SomeError();
                Console.WriteLine("Hello, World!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Inside catch.");
                throw;
            }
        }


        private static void SomeError()
        {
            try
            {
                throw new Exception();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}