using System;

namespace Practice
{
    /// <summary>
    /// Day 1 — Nullable Reference Types exercise.
    /// Predict each numbered output BEFORE running, implement the Part 3 challenge,
    /// and answer the Part 4 warning-spotting question in chat.
    /// Covers: ?. ?? ??= operators, flow analysis, and null-safe handling.
    /// </summary>
    public static class NullableRefTypes
    {
        public static void Run()
        {
            // ---------------------------------------------------------------
            // Part 1 — null-safe operators (predict each output)
            // ---------------------------------------------------------------
            Console.WriteLine("Part 1 — operators:");
            string? name = null;
            Console.WriteLine($"(1) name?.Length       -> [{name?.Length}]");     // ?  (name is null)
            Console.WriteLine($"(2) name?.Length ?? -1 -> {name?.Length ?? -1}");  // ?
            name ??= "Alice";
            Console.WriteLine($"(3) after ??= 'Alice'  -> {name}");               // ?
            Console.WriteLine($"(4) name?.Length ?? -1 -> {name?.Length ?? -1}");  // ?

            // ---------------------------------------------------------------
            // Part 2 — flow analysis (predict each output)
            // ---------------------------------------------------------------
            Console.WriteLine("\nPart 2 — flow analysis:");
            Console.WriteLine($"(5) Describe(null)     -> {Describe(null)}");     // ?
            Console.WriteLine($"(6) Describe(\"hello\")  -> {Describe("hello")}");  // ?

            // ---------------------------------------------------------------
            // Part 3 — CHALLENGE (your turn):
            // Implement Initial(string?) below so that:
            //Initial("Bob") => "B";
            //   Initial("")    => "?";
            //   Initial(null)  => "?";
            // Then UNCOMMENT the three test lines.
            // ---------------------------------------------------------------
             Console.WriteLine("\nPart 3 — Initial(name):");
            Console.WriteLine($"(7) Initial(\"Bob\") -> {Initial("Bob")}");
            Console.WriteLine($"(8) Initial(\"\")    -> {Initial("")}");
            Console.WriteLine($"(9) Initial(null)  -> {Initial(null)}");

            // ---------------------------------------------------------------
            // Part 4 — WARNING SPOT (answer in chat, no code change):
            // Assume:  string? maybe = something;   string sure = "x";
            // Which of these produce a NULLABLE compiler warning, and why?
            //   (A) int a = maybe.Length;
            //   (B) int b = maybe?.Length ?? 0;
            //   (C) string s = maybe;
            //   (D) if (maybe != null) { int d = maybe.Length; }
            // ---------------------------------------------------------------
        }

        /// <summary>
        /// Uses flow analysis: after the null check returns, 'text' is known to be non-null,
        /// so 'text.Length' produces no warning.
        /// </summary>
        private static string Describe(string? text)
        {
            if (text is null) return "was null";
            return $"length {text.Length}";
        }

        /// <summary>
        /// CHALLENGE: return the uppercase first letter of 'name', or "?" if name is null or empty.
        /// Hint: string.IsNullOrEmpty(name) covers both the null and empty cases in one check;
        /// after that guard, name[0] is safe to access.
        /// </summary>
        private static string Initial(string? name)
        {
            if(string.IsNullOrWhiteSpace(name))  return "?";
            return name[0].ToString().ToUpper();    
        }
    }
}
