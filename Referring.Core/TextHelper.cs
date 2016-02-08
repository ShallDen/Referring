using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Referring.Core
{
    public static class TextHelper
    {
        private static string[] sentenceSeparator = { ".", "!", "?", "!?", "?!" };
        private static string[] wordsSeparator = { " ", ".", "!", "?", "!?", "?!" };

        public static List<string> DivideToSentences(this string text)
        {
            return new List<string>(text.Split(sentenceSeparator, StringSplitOptions.RemoveEmptyEntries));
        }
        public static List<string> DivideToWords(this string text)
        {
            return new List<string>(text.Split(wordsSeparator, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
