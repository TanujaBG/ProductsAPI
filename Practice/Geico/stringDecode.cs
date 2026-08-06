namespace Practice.Geico
{
    /// <summary>
    /// Decodes an encoded string where digits followed by brackets represent repetition counts.
    /// For example: "3[a]" → "aaa", "2[3[a]b]" → "aaabaaab".
    /// </summary>
    public class StringDecode
    {
        /// <summary>
        /// Decodes the entire encoded string by parsing letters and digit-bracket pairs recursively.
        /// </summary>
        /// <param name="encoded">The encoded string to decode.</param>
        /// <returns>The decoded string.</returns>
        public static string Decode(string encoded)
        {
            if (string.IsNullOrEmpty(encoded))
            {
                return string.Empty;
            }

            int position = 0;
            return DecodeRange(encoded, ref position, terminator: null).ToString();
        }

        /// <summary>
        /// Parses a run of characters from the current position, handling letters and
        /// digit-bracket pairs, until the terminator (or end of string) is reached.
        /// This single loop is shared by both the root level and nested bracket sections,
        /// which is what removes the previously duplicated parsing logic.
        /// </summary>
        /// <param name="encoded">The encoded string being parsed.</param>
        /// <param name="position">The current position (passed by reference and updated).</param>
        /// <param name="terminator">Character that ends this range (']' for a nested section, null at the root).</param>
        /// <returns>A <see cref="System.Text.StringBuilder"/> containing the decoded content for this range.</returns>
        private static System.Text.StringBuilder DecodeRange(string encoded, ref int position, char? terminator)
        {
            var decoded = new System.Text.StringBuilder();

            while (position < encoded.Length)
            {
                char currentChar = encoded[position];

                // Stop when we reach the terminator (closing bracket for a nested section).
                if (terminator.HasValue && currentChar == terminator.Value)
                {
                    position++; // Skip the terminator.
                    break;
                }

                if (char.IsLetter(currentChar))
                {
                    decoded.Append(currentChar);
                    position++;
                }
                else if (char.IsDigit(currentChar))
                {
                    // Parse the repeat count, decode the bracketed section, then repeat it.
                    int repeatCount = ParseDigitSequence(encoded, ref position);
                    string decodedSection = DecodeBracketedSection(encoded, ref position);

                    for (int repetition = 0; repetition < repeatCount; repetition++)
                    {
                        decoded.Append(decodedSection);
                    }
                }
                else
                {
                    position++; // Skip any unexpected characters.
                }
            }

            return decoded;
        }

        /// <summary>
        /// Parses a sequence of consecutive digits starting at the current position and returns the numeric value.
        /// Updates position to point to the first non-digit character.
        /// </summary>
        /// <param name="encoded">The encoded string being parsed.</param>
        /// <param name="position">The current position (passed by reference and updated).</param>
        /// <returns>The numeric value parsed from the digits.</returns>
        private static int ParseDigitSequence(string encoded, ref int position)
        {
            int number = 0;
            while (position < encoded.Length && char.IsDigit(encoded[position]))
            {
                number = number * 10 + (encoded[position] - '0');
                position++;
            }
            return number;
        }

        /// <summary>
        /// Decodes the content within brackets, starting at the opening '[' and ending past the closing ']'.
        /// Delegates the inner parsing to <see cref="DecodeRange"/> with ']' as the terminator.
        /// </summary>
        /// <param name="encoded">The encoded string being parsed.</param>
        /// <param name="position">The current position (passed by reference and updated).</param>
        /// <returns>The decoded content of the bracketed section.</returns>
        private static string DecodeBracketedSection(string encoded, ref int position)
        {
            if (position >= encoded.Length || encoded[position] != '[')
            {
                return string.Empty;
            }

            position++; // Skip the opening bracket '['.
            return DecodeRange(encoded, ref position, terminator: ']').ToString();
        }
    
        /// <summary>
        /// Runs a comprehensive set of test cases to validate the Decode method.
        /// </summary>
        public static void Run()
        {
            // Each case pairs an encoded input with the string it should decode to.
            var testCases = new (string Encoded, string Expected)[]
            {
                ("abc", "abc"),                       // only letters
                ("3[a]", "aaa"),                      // a single encoded section
                ("2[3[a]b]", "aaabaaab"),             // nested encoded sections
                ("2[a]3[b]", "aabbb"),                // multiple adjacent sections
                ("12[a]", "aaaaaaaaaaaa"),            // multi-digit repeat count
                ("x2[a]y", "xaay"),                   // letters before and after a section
                ("100[a]", new string('a', 100)),     // a large decoded output
                ("[]", ""),                           // minimum valid bracket input
                ("2[a2[b]]", "abbabb"),               // a section with letters inside
                ("2[]", ""),                          // empty encoded section
                ("", ""),                             // empty string
            };

            foreach (var (encoded, expected) in testCases)
            {
                string decoded = Decode(encoded);
                Console.WriteLine(decoded == expected ? "Pass" : "Fail");
            }
        }

    }
}