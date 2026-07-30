namespace Algos
{
    public static class MaximumSubarraySum
    {
        public static int FindMaximumSubarraySum(int[] nums)
        {
            if(nums == null || nums.Length == 0)
            {
                return 0;
            }

            int maxSumSoFar = nums[0];
            int currentSum = nums[0];

            for(int i = 1; i < nums.Length; i++)
            {
                currentSum = Math.Max(nums[i], currentSum + nums[i]);
                maxSumSoFar = Math.Max(maxSumSoFar, currentSum);
            }
            return maxSumSoFar;
        }
    }
}