using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Referring.Core;

namespace Referring.Client
{
    public class EssayComparer
    {
        private string autoEssay = string.Empty;
        private string manualEssay = string.Empty;
        private List<string> autoEssaySentences = new List<string>();
        private List<string> manualEssaySentences = new List<string>();

        public EssayComparer(string autoEssayPath, string manualEssayPath)
        {
            autoEssay = ReferringManager.Instance.ReferredText;
            manualEssay = ReferringManager.Instance.ManualEssayText;
        }

        public double Compare()
        {
            Logger.LogInfo("Using original cases in essay.");

            autoEssaySentences = autoEssay.DivideTextToSentences()
                .ClearWhiteSpacesInList();
            manualEssaySentences = manualEssay.ClearUnnecessarySymbolsInText()
                .DivideTextToSentences()
                .ClearWhiteSpacesInList()
                .RemoveEmptyItemsInList();

            double requiredHits = manualEssaySentences.Count;
            double hit = 0;

            foreach (var sentence in manualEssaySentences)
            {
                if (autoEssaySentences.Contains(sentence))
                {
                    ++hit;
                }
            }

            double percentage = hit / requiredHits * 100;

            ReferringManager.Instance.IsComparisonCompete = true;
            return percentage;
        }

        public List<Word> GetWordStatistics(string essay)
        {
            var referringProcess = new ReferringProcess();
            referringProcess.UsePercentage = false;

            referringProcess.SentenceList = essay.ClearUnnecessarySymbolsInText()
                    .DivideTextToSentences()
                    .ClearWhiteSpacesInList()
                    .RemoveEmptyItemsInList()
                    .ToLower(); 

            referringProcess.WordList = essay.ClearUnnecessarySymbolsInText()
                    .DivideTextToWords()
                    .RemoveEmptyItemsInList()
                    .ToLower(); 

            referringProcess.CalculateWordWeights();

            return referringProcess.GoodWordList;
        }
    }
}
