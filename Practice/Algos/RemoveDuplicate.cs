using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Algos
{
    public static class RemoveDuplicateCharacters
    {
        public static string RemoveDuplicates(string input)
        {
            if(string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            List<char> res = new List<char>();
            HashSet<char> set = new HashSet<char>();

            foreach(char c in input)
            {
                if(!set.Contains(c))
                {
                    res.Add(c);
                    set.Add(c);
                }
            }

            return new string(res.ToArray());
        } 

        public static void Run()
        {
            string input = "Programming";
            string output = RemoveDuplicates(input);
            Console.WriteLine($"Input: {input} Output: {output}");

            output = RemoveDuplicates("aaaa");
            // "a"
            Console.WriteLine($"Input: {"aaaa"} Output: {output}");

            output = RemoveDuplicates("AaA");
            // "Aa"
            Console.WriteLine($"Input: {"AaA"} Output: {output}");

            output = RemoveDuplicates("");
            Console.WriteLine($"Input: {""} Output: {output}");
        }
    }
}