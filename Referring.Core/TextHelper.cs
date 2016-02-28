﻿using System;
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
                .Replace("-", "")
                .Replace("\"", "")
                .Replace("\r", " ")
                .Replace("\n", " ");

            return text;
        }

        public static List<string> ClearUnnecessarySymbolsInList(this List<string> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = list[i].ClearUnnecessarySymbolsInText();
            }

            return list;
        }

        public static List<string> RemoveEmptyItemsInList(this List<string> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(list[i]))
                {
                    list.Remove(list[i]);
                    i--;
                }
            }

            return list;
        }
    }
}
