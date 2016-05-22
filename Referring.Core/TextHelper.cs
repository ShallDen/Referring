using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Referring.Core
{
    public static class TextHelper
    {
        private static string[] sentenceSeparator = { ". ", "! ", "? ", "!? ", "?! ", ".\" ", "!\" ", "?\" " };
        private static string[] wordsSeparator = { " ", ".", "!", "?", "!?", "?!" };

        public static List<string> DivideTextToSentences(this string text)
        {
            return new List<string>(text.Split(sentenceSeparator, StringSplitOptions.RemoveEmptyEntries));
        }

        public static List<string> DivideTextToWords(this string text)
        {
            return new List<string>(text.Split(wordsSeparator, StringSplitOptions.RemoveEmptyEntries));
        }

        public static string ClearUnnecessarySymbolsInText(this string text)
        {
            text = text.Replace(",", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace(":", "")
                .Replace(";", "")
                .Replace(",", "")
                .Replace("-", " ")
                .Replace("—", "")
                .Replace("\"", "")
                .Replace("“", "")
                .Replace("”", "")
                .Replace("»", "")
                .Replace("\r", " ")
                .Replace("\n", " ");
            
            return text;
        }

        public static List<string> ClearUnnecessarySymbolsInList(this List<string> list)
        {
            return list.Select(c => c.ClearUnnecessarySymbolsInText()).ToList();
        }

        public static List<string> ClearWhiteSpacesInList(this List<string> list)
        {
            return list.Select(c => c.Trim()).ToList();
        }

        public static List<string> RemoveEmptyItemsInList(this List<string> list)
        {
            var deletedItemsCount = list.RemoveAll(c=>string.IsNullOrWhiteSpace(c));
            return list;
        }

        public static List<string> ToLower(this List<string> list)
        {
            return list.Select(c => c.ToLower()).ToList();
        }

        public static List<Word> TransformPOSToRussian(this List<Word> list)
        {
            foreach (var word in list)
            {
                word.POS = Tagger.TransformPOSToRussian(word.POS);
            }
            return list;
        }
    }
}
