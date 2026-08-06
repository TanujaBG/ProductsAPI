namespace Practice.Geico
{
    public class ShrinkString
    {
        /// <summary>
        /// Reduces the input so that only characters occurring an odd number of times remain,
        /// each kept exactly once and arranged as the lexicographically smallest valid order.
        /// </summary>
        /// <param name="input">The string to reduce.</param>
        /// <returns>The reduced string.</returns>
        public string ReduceString(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            // Last index tells us whether a character still appears later in the input.
            Dictionary<char, int> lastIndex = BuildLastIndexMap(input);

            // Only characters with an odd occurrence count survive the reduction.
            HashSet<char> charactersToKeep = GetOddOccurrenceCharacters(input);

            return BuildSmallestSubsequence(input, lastIndex, charactersToKeep);
        }

        /// <summary>
        /// Maps each character to the last index at which it appears in the input.
        /// </summary>
        /// <param name="input">The string being scanned.</param>
        /// <returns>A dictionary from character to its last occurrence index.</returns>
        private static Dictionary<char, int> BuildLastIndexMap(string input)
        {
            var lastIndex = new Dictionary<char, int>();

            for (int index = 0; index < input.Length; index++)
            {
                lastIndex[input[index]] = index;
            }

            return lastIndex;
        }

        /// <summary>
        /// Finds the characters that occur an odd number of times in the input.
        /// </summary>
        /// <param name="input">The string being scanned.</param>
        /// <returns>The set of characters with an odd occurrence count.</returns>
        private static HashSet<char> GetOddOccurrenceCharacters(string input)
        {
            var counts = new Dictionary<char, int>();

            foreach (char character in input)
            {
                counts.TryGetValue(character, out int count);
                counts[character] = count + 1;
            }

            var oddCharacters = new HashSet<char>();

            foreach (var pair in counts)
            {
                if (pair.Value % 2 == 1)
                {
                    oddCharacters.Add(pair.Key);
                }
            }

            return oddCharacters;
        }

        /// <summary>
        /// Builds the lexicographically smallest subsequence that contains each character
        /// to keep exactly once, using a greedy stack that pops larger characters which
        /// still occur later in the input.
        /// </summary>
        /// <param name="input">The string being reduced.</param>
        /// <param name="lastIndex">Last occurrence index for every character.</param>
        /// <param name="charactersToKeep">Characters that must appear in the result.</param>
        /// <returns>The reduced, lexicographically smallest string.</returns>
        private static string BuildSmallestSubsequence(
            string input,
            Dictionary<char, int> lastIndex,
            HashSet<char> charactersToKeep)
        {
            var result = new List<char>();
            var inResult = new HashSet<char>();

            for (int index = 0; index < input.Length; index++)
            {
                char current = input[index];

                // Skip characters that occur an even number of times.
                if (!charactersToKeep.Contains(current))
                {
                    continue;
                }

                // Keep only a single occurrence of each character.
                if (inResult.Contains(current))
                {
                    continue;
                }

                // Drop a larger trailing character when it still appears later,
                // because placing the smaller character first is lexicographically better.
                while (result.Count > 0
                    && current < result[result.Count - 1]
                    && lastIndex[result[result.Count - 1]] > index)
                {
                    char removed = result[result.Count - 1];
                    result.RemoveAt(result.Count - 1);
                    inResult.Remove(removed);
                }

                result.Add(current);
                inResult.Add(current);
            }

            return new string(result.ToArray());
        }    

    }

    public class ShrinkStringTests
    {
        public void RunTests()
        {
            var testcases = new (string input, string expected)[]
            {
                ("CBA", "CBA"),
                ("AAABBBCCC", "ABC"),
                ("", ""),
                ("AFKFKMOGFKB", "AFKMOGB"),
                ("AABAC", "ABC"),
                ("ABAC", "BC"),
                ("AABBCC", ""),
                ("ZYXZYX", ""),
                ("BABB", "AB"),
                ("KMKK", "KM"),
                ("CBAC", "BA"),
            };
            var shrinker = new ShrinkString();
            foreach (var (input, expected) in testcases)
            {
                var result = shrinker.ReduceString(input);
                // compare the result with the expected output
                bool isEqual = result == expected;
                //if(!isEqual)
                {
                    Console.WriteLine($"{(isEqual ? "Passed" : "Failed")} - Input: {input}, Expected: {expected}, Result: {result}");
                }
            }
        }
    }
}