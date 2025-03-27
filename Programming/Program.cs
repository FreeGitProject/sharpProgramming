using Programming;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            int[] nums = { 1, 2, 3, 4, 5, 6, 7 };
            int k = 3;

            RotateArray.RotateArrayMethod(nums,k);
        }
    }
}