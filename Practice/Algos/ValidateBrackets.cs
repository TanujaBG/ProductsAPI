namespace Algos
{
    public static class ValidateBrackets
    {
        public static bool IsValid(string s)
        {
            if(s is null)
            { return false;}

            if(s == string.Empty)
            {
                return true;
            }

            if(s.Length %2 != 0)
            {
                return false;
            }

            Dictionary<char, char> brackets = new()
            {
                { '(', ')' },
                { '{', '}' },
                { '[', ']' }
            };

            Stack<char> stack = new();

            foreach(char c in s)
            {
                if(brackets.ContainsKey(c))
                {
                    stack.Push(c);
                }
                else
                {
                    if(stack.Count == 0 || brackets[stack.Pop()] != c)
                    {
                        return false;
                    }
                }
            }


            return stack.Count == 0;

        }
    }
}