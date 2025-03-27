namespace Programming
{
    public static  class RotateArray
    {
        public static void RotateArrayMethod(int[] nums,int k)
        {

            for (int i = 0; i < 7; i++)
                Console.Write(" " + nums[i]);

            int len = nums.Length;
            int temp = 0;
            while (k != 0)
            {
                int length = nums.Length - 1;

                temp = nums[length];
                for (int i = 0; i < len; i++)
                {
                    if (length == 0)
                        break;
                    nums[length] = nums[length - 1];
                    length--;

                }
                nums[0] = temp;
                Console.WriteLine();
                for (int i = 0; i < len; i++)
                    Console.Write(" " + nums[i]);

                k--;
            }
        }
    }
}
