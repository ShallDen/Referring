using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Referring.Core;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace Referring.Core
{
    public enum ComparisonType
    {
        FullSentences,
        MainWordFulness,
        MainWordAccuracyWithError,
        MainWordAccuracyWithSignificanceKoefficient
    }

    public delegate void EssayComparisonProgressChangeDelegate(double percent);
    public delegate void EssayComparisonCompleteDelegate(string elapsedTime);

    public class EssayComparer : INotifyPropertyChanged
    {
        private double mEssayComparisonPercentage;
        private bool mIsUseCurrentEssayAsFirstFile;
        private SynchronizationContext context;

        private static EssayComparer instance;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EssayComparisonProgressChangeDelegate ProgressChanged;
        public event EssayComparisonCompleteDelegate WorkCompleted;

        public static EssayComparer Instance
        {
            get { return instance ?? (instance = new EssayComparer()); }
        }

        public ComparisonType ComparisonType { get; set; }
        public bool IsComparisonCompete { get; set; }
        public string FisrtEssay { get; set; }
        public string FisrtEssayPath { get; set; }
        public string SecondEssay { get; set; }
        public string SecondEssayPath { get; set; }
        public double ProgressPercentageCurrent { get; set; }

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

        public void RunComparison(object param)
        {
            context = (SynchronizationContext)param;

            SetProgressPercentage(0);

            Logger.LogInfo("Starting comparison process...");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            EssayComparer.Instance.IsComparisonCompete = false;
            EssayComparer.Instance.EssayComparisonPercentage = Compare();

            stopwatch.Stop();

            var ts = stopwatch.Elapsed;
            var elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:000}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);

            Logger.LogInfo("Comparison completed. Elapsed time: " + elapsedTime);

            SetProgressPercentage(100);
            context.Send(OnWorkCompleted, elapsedTime);
        }

        private double Compare()
        {
            ComparisonType comparisonType = EssayComparer.Instance.ComparisonType;
            string firstEssay = EssayComparer.Instance.FisrtEssay;
            string secondtEssay = EssayComparer.Instance.SecondEssay;

            var firstEssayStatistics = GetWordStatistics(firstEssay);
            var secondEssayStatistics = GetWordStatistics(secondtEssay);

            Logger.LogInfo("Comparison type is '" + comparisonType.ToString() + "'");
            Logger.LogInfo("Fisrt essay contains " + firstEssayStatistics.Count  + " words.");
            Logger.LogInfo("Second essay contains " + secondEssayStatistics.Count + " words.");

            double info = 0;
            switch (comparisonType)
            {
                case ComparisonType.FullSentences:
                    info = CompareFullSentences(firstEssay, secondtEssay);
                    break;
                case ComparisonType.MainWordFulness:
                    info = CompareMainWordFulness(firstEssayStatistics, secondEssayStatistics);
                    break;
                case ComparisonType.MainWordAccuracyWithError:
                    info = CompareMainWordAccuracyWithError(firstEssayStatistics, secondEssayStatistics);
                    break;
                case ComparisonType.MainWordAccuracyWithSignificanceKoefficient:
                    info = CompareMainWordAccuracyWithSignificanceCoefficient(firstEssayStatistics, secondEssayStatistics);
                    break;
                default:
                    info = CompareMainWordFulness(firstEssayStatistics, secondEssayStatistics);
                    break;
            }

            return info;
        }

        private double CompareFullSentences(string firstEssay, string secondEssay)
        {
            //Сравнение по полным предложениям
            Logger.LogInfo("Using original cases in essay.");

            var firstEssaySentences = firstEssay.DivideTextToSentences()
                .ClearWhiteSpacesInList();
            var secondEssaySentences = secondEssay.ClearUnnecessarySymbolsInText()
                .DivideTextToSentences()
                .ClearWhiteSpacesInList()
                .RemoveEmptyItemsInList();

            double progressPercentageMax = 100;
            double step = (progressPercentageMax - EssayComparer.Instance.ProgressPercentageCurrent) / secondEssaySentences.Count;

            double requiredHits = secondEssaySentences.Count;
            double comparisonPercentage = 0;
            double hits = 0;

            foreach (var sentence in secondEssaySentences)
            {
                if (firstEssaySentences.Contains(sentence))
                    ++hits;
                AddProgressPercentage(step);
            }

            if (requiredHits > 0)
                comparisonPercentage = hits / requiredHits * 100;
            else
                comparisonPercentage = 0;

            return comparisonPercentage;
        }

        private double CompareMainWordFulness(List<Word> fisrtEssayStatistics, List<Word> secondEssayStatistics)
        {
            //Сравнение по полноте
            double progressPercentageMax = 100;
            double step = (progressPercentageMax - EssayComparer.Instance.ProgressPercentageCurrent) / secondEssayStatistics.Count;

            double requiredHits = secondEssayStatistics.Count;
            double comparisonPercentage = 0;
            double hits = 0;

            foreach (var word in secondEssayStatistics)
            {
                if (fisrtEssayStatistics.Where(c=>c.Value == word.Value).Any())
                    ++hits;
                AddProgressPercentage(step);
            }

            if (requiredHits > 0)
                comparisonPercentage = hits / requiredHits * 100;
            else
                comparisonPercentage = 0;

            return comparisonPercentage;
        }

        private double CompareMainWordAccuracyWithError(List<Word> fisrtEssayStatistics, List<Word> secondEssayStatistics)
        {
            //Сравнение по точности с погрешностью
            double progressPercentageMax = 100;
            double step = (progressPercentageMax - EssayComparer.Instance.ProgressPercentageCurrent) / secondEssayStatistics.Count;

            double error = 0.2;
            double requiredHits = secondEssayStatistics.Count;
            double comparisonPercentage = 0;
            double hits = 0;

            foreach (var word in secondEssayStatistics)
            {
                if (fisrtEssayStatistics.Where(c => c.Value == word.Value).Any())
                {
                    var firstEssayWord = fisrtEssayStatistics.Where(c => c.Value == word.Value).First();
                    var possibleWeightMin = firstEssayWord.Weight * (1 - error);
                    var possibleWeightMax = firstEssayWord.Weight * (1 + error);

                    //Round minimum value
                    var integeterPartMin = Math.Truncate(possibleWeightMin);
                    var fractionalPartMin = possibleWeightMin - integeterPartMin;

                    if (fractionalPartMin > 0)
                        possibleWeightMin -= fractionalPartMin;

                    //Round maximum value
                    var integeterPartMax = Math.Truncate(possibleWeightMax);
                    var fractionalPartMax = possibleWeightMax - integeterPartMax;

                    if (fractionalPartMax > 0)
                        possibleWeightMax = integeterPartMax + 1;

                    // min <= weight <= max
                    bool isHit = word.Weight >= possibleWeightMin && word.Weight <= possibleWeightMax;

                    if (isHit)
                        ++hits;
                }

                AddProgressPercentage(step);
            }

            if (requiredHits > 0)
                comparisonPercentage = hits / requiredHits * 100;
            else
                comparisonPercentage = 0;

            return comparisonPercentage;
        }

        private double CompareMainWordAccuracyWithSignificanceCoefficient(List<Word> fisrtEssayStatistics, List<Word> secondEssayStatistics)
        {
            //Сравнение по точности с использованием коэф. значимости
            double progressPercentageMax = 100;
            double step = (progressPercentageMax - EssayComparer.Instance.ProgressPercentageCurrent) / secondEssayStatistics.Count;

            double hits = 0;
            double weightRate = 0;
            double comparisonPercentage = 0;

            var statistics = new StringBuilder();

            var header = string.Format("Слово;Вес #1;Вес #2;Отношение");
            statistics.AppendLine(header);

            foreach (var word in secondEssayStatistics)
            {
                if (fisrtEssayStatistics.Where(c => c.Value == word.Value).Any())
                {
                    var firstEssayWord = fisrtEssayStatistics.Where(c => c.Value == word.Value).First();

                    double wordRate = (double)firstEssayWord.Weight / word.Weight;

                    var newLine = string.Format("{0};{1};{2};{3}", word.Value, firstEssayWord.Weight, word.Weight, wordRate);
                    statistics.AppendLine(newLine);

                    weightRate += wordRate;
                    ++hits;
                }

                AddProgressPercentage(step);
            }

            if (FileManager.IsExist(FileManager.FileFullPath))
            {
                int index = FileManager.FileName.LastIndexOf(".");
                string name = FileManager.FileName.Substring(0, index);
                string fileName = string.Concat(FileManager.FileDirectory + "ComparisonStatistics_" + name + ".csv");

                Logger.LogInfo("Saving comparison statistics: " + fileName);

                if (FileManager.IsExist(fileName))
                    FileManager.Delete(fileName);

                FileManager.SaveContent(statistics.ToString(), fileName);

                Logger.LogInfo("Comparison statistics was saved.");
            }
            else
            {
                Logger.LogError(string.Format("Path: {0} isn't valid", FileManager.FileFullPath));
            }

            if (hits > 0)
                comparisonPercentage = weightRate / hits * 100;
            else
                comparisonPercentage = 0;

            return comparisonPercentage;
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

        private void OnProgressChanged(object i)
        {
            double percent = Convert.ToDouble(i);

            if (ProgressChanged != null)
                ProgressChanged(percent);
        }

        private void OnWorkCompleted(object elapsedTime)
        {
            if (WorkCompleted != null)
                WorkCompleted(elapsedTime.ToString());
        }

        private void SetProgressPercentage(double value)
        {
            ProgressPercentageCurrent = value;
            context.Send(OnProgressChanged, ProgressPercentageCurrent);
        }

        private void AddProgressPercentage(double value)
        {
            ProgressPercentageCurrent += value;
            context.Send(OnProgressChanged, ProgressPercentageCurrent);
        }   
    }
}
