namespace Algos
{
    public static class MoveZerosToEnd
    {
        public static void MoveZeros(int[] arr)
        {
            if(arr is null || arr.Length <= 1)
            {
                return;
            }

            int nonZeroIndex = 0;
            for(int i=0; i<arr.Length; i++)
            {
                if(arr[i] != 0)
                {
                    arr[nonZeroIndex] = arr[i];
                    if(nonZeroIndex != i)
                    {
                        arr[i] = 0;
                    }
                    nonZeroIndex++;

                }
            }
        }

        public static void Run()
        {
            int[] arr = { 0, 1, 0, 3, 12 };
            MoveZeros(arr);
            Console.WriteLine($"expected: [1, 3, 12, 0, 0] so the test is {(arr.SequenceEqual(new int[] { 1, 3, 12, 0, 0 }) ? "passed" : "failed")} ");

            arr = new int[] { 0, 0, 0 };
            MoveZeros(arr);
            Console.WriteLine($"Test Passed expected: [0, 0, 0] so the test is {(arr.SequenceEqual(new int[] { 0, 0, 0 }) ? "passed" : "failed")} ");

            arr = new int[] { 1, 2, 3 };
            MoveZeros(arr);
            Console.WriteLine($"expected: [1, 2, 3] so the test is {(arr.SequenceEqual(new int[] { 1, 2, 3 }) ? "passed" : "failed")} ");

            arr = new int[] { };
            MoveZeros(arr);
            Console.WriteLine($"expected: [] so the test is {(arr.SequenceEqual(new int[] { }) ? "passed" : "failed")} ");
        }
    }
}
