using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Referring.Core
{
    public class EssayBuilder
    {
        public static string BuildEssay(List<Sentence> requiredSentences, List<Sentence> goodSentenceList)
        {
            Logger.LogInfo("Using original cases in essay.");
            List<string> sentenceListOriginalCase = ReferringManager.Instance.OriginalText.ClearUnnecessarySymbolsInText()
                .DivideTextToSentences()
                .ClearWhiteSpacesInList()
                .RemoveEmptyItemsInList();

            string essay = string.Empty;

            foreach (var sentence in goodSentenceList)
            {
                if (requiredSentences.Contains(sentence))
                {
                    if (!string.IsNullOrEmpty(essay))
                    {
                        essay = string.Format("{0} {1}. ", essay, sentenceListOriginalCase[sentence.Index - 1]);
                    }
                    else
                    {
                        essay = string.Format("{0}.", sentenceListOriginalCase[sentence.Index - 1]);
                    }
                }
            }

            Logger.LogInfo("Essay was built.");
            return essay;
        }
    }
}
