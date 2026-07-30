
using System.Collections.Generic;

namespace Algo
{
public static class FirstNonRepeatingChar
{
    public static char? FindFirstNonRepeatingChar(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        Dictionary<char, int> charCount = new Dictionary<char, int>();

        // Count the occurrences of each character
        foreach (char c in input)
        {
            if (charCount.ContainsKey(c))
            {
                charCount[c]++;
            }
            else
            {
                charCount[c] = 1;
            }
        }

        // Find the first character with a count of 1
        foreach (char c in input)
        {
            if (charCount[c] == 1)
            {
                return c;
            }
        }

        return null; // No non-repeating character found    
    }

    public static void Run()
    {
        string input = "swiss";
        char? result = FindFirstNonRepeatingChar(input); // w is the ans
        Console.WriteLine($"Input: {input} First Non-Repeating Character: {result}");

            input = "aabbcc";
            result = FindFirstNonRepeatingChar(input); // null is the ans
            Console.WriteLine($"Input: {input} First Non-Repeating Character: {result}");

        input = "aabbc";
        result = FindFirstNonRepeatingChar(input); // c is the ans
        Console.WriteLine($"Input: {input} First Non-Repeating Character: {result}");

        input = " aabbcc";
        result = FindFirstNonRepeatingChar(input); // ' ' is the ans
        Console.WriteLine($"Input: {input} First Non-Repeating Character: {result}");
    }
}
}