using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Referring.Core;
using System.ComponentModel;

namespace Referring.Client
{
    public enum ComparisonType
    {
        FullSentences,
        MainWordFulness,
        MainWordAccuracyWithError,
        MainWordAccuracyWithSignificanceKoefficient
    }

    public class EssayComparer : INotifyPropertyChanged
    {
        private string autoEssay = string.Empty;
        private string manualEssay = string.Empty;
        private List<string> autoEssaySentences = new List<string>();
        private List<string> manualEssaySentences = new List<string>();

        private double mEssayComparisonPercentage;
        private bool mIsUseCurrentEssayAsFirstFile;

        private static EssayComparer instance;

        public event PropertyChangedEventHandler PropertyChanged;

        public static EssayComparer Instance
        {
            get { return instance ?? (instance = new EssayComparer()); }
        }

        public ComparisonType ComparisonType { get; set; }

        public bool IsComparisonCompete { get; set; }
        public bool IsUseCurrentEssayAsFirstFile
        {
            get { return mIsUseCurrentEssayAsFirstFile; }
            set
            {
                if (mIsUseCurrentEssayAsFirstFile != value)
                {
                    mIsUseCurrentEssayAsFirstFile = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("IsUseCurrentEssayAsFirstFile"));
                }
            }
        }

        public string FisrtEssayPath { get; set; }
        public string SecondEssayPath { get; set; }

        public double EssayComparisonPercentage
        {
            get { return mEssayComparisonPercentage; }
            set
            {
                if (mEssayComparisonPercentage != value)
                {
                    mEssayComparisonPercentage = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("EssayComparisonPercentage"));
                }
            }
        }

        public EssayComparer()
        {

        }

        public double Compare(ComparisonType comparisonType, List<Word> autoEssayStatistics, List<Word> manualEssayStatistics)
        {
            double info = 0;
            switch (comparisonType)
            {
                case ComparisonType.FullSentences:
                    info = CompareFullSentences();
                    break;
                case ComparisonType.MainWordFulness:
                    info = CompareMainWordFulness(autoEssayStatistics, manualEssayStatistics);
                    break;
                case ComparisonType.MainWordAccuracyWithError:
                    CompareMainWordAccuracyWithError(autoEssayStatistics, manualEssayStatistics);
                    break;
                case ComparisonType.MainWordAccuracyWithSignificanceKoefficient:
                    info = CompareMainWordAccuracyWithSignificanceKoefficient(autoEssayStatistics, manualEssayStatistics);
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }

            IsComparisonCompete = true;

            return info;
        }

        private double CompareFullSentences()
        {
            //Сравнение по полным предложениям
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

            return percentage;
        }

        public double CompareMainWordFulness(List<Word> autoEssayStatistics, List<Word> manualEssayStatistics)
        {
            //Сравнение по полноте
            double requiredHits = manualEssayStatistics.Count;
            double hits = 0;

            foreach (var word in manualEssayStatistics)
            {
                if (autoEssayStatistics.Where(c=>c.Value == word.Value).Any())
                {
                    ++hits;
                }
            }

            double percentage = hits / requiredHits * 100;

            return percentage;
        }

        public double CompareMainWordAccuracyWithError(List<Word> autoEssayStatistics, List<Word> manualEssayStatistics)
        {
            //Сравнение по точности с погрешностью
            double percentage = 0;
            return percentage;
        }

        public double CompareMainWordAccuracyWithSignificanceKoefficient(List<Word> autoEssayStatistics, List<Word> manualEssayStatistics)
        {
            //Сравнение по точности с использованием коэф. значимости
            double hits = 0;
            double weightRate = 0;
            List<Word> autoWinsList = new List<Word>();

            foreach (var word in manualEssayStatistics)
            {
                var temp = autoEssayStatistics.Where(c => c.Value == word.Value);
                if (temp.Any())
                {
                    double autoValue = temp.Select(c => c.Weight).First();
                    double manualValue = word.Weight;

                    if (autoValue > manualValue)
                        autoWinsList.Add(word);

                    weightRate += autoValue / manualValue;
                    ++hits;
                }
            }

            double percentage = weightRate / hits * 100;

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
