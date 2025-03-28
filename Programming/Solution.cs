namespace Programming
{
    public static class Solution
    {
        public static void Rotate(int[] nums, int k)
        {

            int len = nums.Length;
            k %= len;
            rotatearray(ref nums, 0, len - 1);
            rotatearray(ref nums, 0, k - 1);
            rotatearray(ref nums, k, len - 1);
            for (int i = 0; i < len; i++)
                Console.Write(" " + nums[i]);
        }
        public static void PrintArray()
        {
            int[] arr = new int[5];
            arr[0] = 1;
            arr[1] = 2;
            arr[2] = 3;
            arr[3] = 4;
            arr[4] = 5;
           
           // arr =  { 1,2,3,4,4};
            for (int i = 0; i < arr.Length; i++)
            {
                Console.WriteLine(arr[i]);
            }
        }

        static void rotatearray(ref int[] num, int i, int j)
        {
            int temp;
            while (i < j)
            {
                temp = num[i];
                num[i] = num[j];
                num[j] = temp;
                i++;
                j--;
            }

        }
        public static void TwoSum()
        {
            int[] nums = { 2, 7, 11, 15 };
            int target = 9, i = 0, j = 0;
            int len = nums.Length;
            for (i = 0; i < len; i++)
            {
                for (j = i + 1; j < len; j++)
                {
                    if (nums[i] + nums[j] == target)
                    {
                        Console.WriteLine(" i is  " + i + " and j is " + j);
                        //return new int[] { i, j };
                    }
                }
            }
            // return null;
        }

    }

    

}
