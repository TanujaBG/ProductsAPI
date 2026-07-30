namespace Algos
{
    public static class FindTwoSums
    {
        public static int[] FindTwoSum(int[] numbers, int target)
        {
            Dictionary<int, int> numToIndex = new();

            for(int i =0; i< numbers.Length; i++)
            {
                int reminder = target - numbers[i];
                if(numToIndex.TryGetValue(reminder, out int index))
                {
                    return new int[] { index, i};
                }
                else
                {
                    numToIndex[numbers[i]] = i;
                }
            }

            return [];
        }

        public static void Run()
        {
            int[] arr = { 0, 1, 0, 3, 12 };
            var res = FindTwoSum(arr, 15);
            Console.WriteLine($"Test is {(res.SequenceEqual(new int[] { 1, 3 }) ? "passed" : "failed")} because expected: [1, 3] so the ");
            
            arr = new int[] { 1, 2, 3 };
            res = FindTwoSum(arr, 5);
            Console.WriteLine($"test is {(res.SequenceEqual(new int[] { 2, 3 }) ? "passed" : "failed")} because expected: [2, 3] so the ");

            arr = new int[] { 0, 0, 0 };
            res = FindTwoSum(arr, 0);
            Console.WriteLine($"Test is {(res.SequenceEqual(new int[] { 0, 0 }) ? "passed" : "failed")} because expected: [0, 0] so the ");
        }
    }
}