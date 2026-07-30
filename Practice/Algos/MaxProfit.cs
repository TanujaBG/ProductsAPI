namespace Algos
{
    public static class FindMaxProfit
    {
        public static int MaxProfit(int[] prices)
        {
            if(prices == null || prices.Length < 2)
            {
                return 0;
            }

            int res = 0;
            int minPrice = prices[0];

            for(int i = 1; i < prices.Length; i++)
            {
                if(prices[i] < minPrice)
                {
                    minPrice = prices[i];
                }
                else
                {
                    res = Math.Max(res, prices[i] - minPrice);
                }
            }
            return res;
        }
    }
}