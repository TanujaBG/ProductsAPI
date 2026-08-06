namespace Practice.Geico
{
    public class MinCoin
    {
        public int MinTokens(int[] values, int target)
        {
            if(target <= 0)
            return 0;

            if(values == null || values.Length == 0)
                return -1;

            int n = values.Length;

            // initialize dp with infinity
            int[] dp = new int[target + 1];
            const int unreachable = int.MaxValue;
            Array.Fill(dp, unreachable);
            dp[0] = 0;

            foreach(int value in values) // for each values, re calculate dp
            {
                for(int amount = value; amount <= target; amount++)
                {
                    if(dp[amount - value] != unreachable)
                    {
                        dp[amount] = Math.Min(dp[amount], dp[amount - value] + 1);
                    }
                }
            }

            return dp[target] == unreachable ? -1 : dp[target];
        }
    }

    public class MinCoinTest
    {
        public void RunTests()
        {
            var testCases = new (int[] values, int target, int expected)[]
            {
                // valid
                (new int[] {1, 2, 5}, 11, 3),
                // not possible
                (new int[] {2}, 3, -1),
                // target 0 
                (new int[] {1}, 0, 0),
                // single coin equal to target
                (new int[] {1}, 1, 1),
                // empty coin array
                (new int[] {}, 5, -1),
                // multiple coins but not possible
                (new int[] {2, 4}, 7, -1),
                // multiple options 
                (new int[] {1, 3, 4}, 6, 2),
                // large target with multiple coins
                (new int[] {1, 2, 5}, 100, 20),
                // All token values exceed the target
                (new int[] {5, 10}, 3, -1),
                // Values are not sorted
                (new int[] {4, 1, 3}, 6, 2),
            };

            foreach (var (values, target, expected) in testCases)
            {
                int result = new MinCoin().MinTokens(values, target);
                Console.WriteLine(result == expected ? "Pass" : "Fail");
            }
        }
    }
}