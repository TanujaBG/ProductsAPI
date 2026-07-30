using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Algos
{
    public static class ReverseWord
    {
        public static string Reverse(string input)
        {
            if(string.IsNullOrEmpty(input))
            {
                return input;
            }

            char[] chars = input.ToCharArray();
            revers(chars, 0, chars.Length - 1);
            for(int i = 0; i< chars.Length; i++)
            {
                if(chars[i] != ' ')
                {
                    int start = i;
                    while(i < chars.Length && chars[i] != ' ')
                    {
                        i++;
                    }

                    int end = i - 1;
                    revers(chars, start, end);
                }
            }

            return new string(chars);
        }

        private static void revers(char[] chars, int left, int right)
        {
            while(left < right)
            {
                char temp = chars[left];
                chars[left] = chars[right];
                chars[right] = temp;
                left++;
                right--;
            }
        }
    }
}
