namespace Algos
{
    public static class NonRepeatingLongestSubString
    {
        public static int LengthOfLongestSubstring(string s)
        {
            if(string.IsNullOrEmpty(s))
            {
                return 0;
            }

            int maxLength = 0;
            Dictionary<char, int> LastSeen = new();
            int start = 0;

            for(int i = 0; i < s.Length; i++)
            {
                if(LastSeen.TryGetValue(s[i], out var index) && index >= start)
                {
                    start = index + 1;
                }
                
                LastSeen[s[i]] = i;
                maxLength = Math.Max(maxLength, i - start + 1);
            }
            return maxLength;

        }
    }
}