using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Referring.Core;
using LAIR.Collections.Generic;
using Referring.WordNet;
using System.Diagnostics;
using System.Threading;

namespace Referring.Client
{
    public delegate void ReferringProgressChangeDelegate(double percent);
    public delegate void ReferringWorkCompleteDelegate(string elapsedTime);

    public class ReferringProcess
    {
        private List<string> sentenceList = new List<string>();
        private List<string> wordList = new List<string>();
        private List<string> sentenceListOriginalCase = new List<string>();
        private List<string> wordListUpperOriginalCase = new List<string>();

        private List<Word> goodWordList = new List<Word>();
        private List<Sentence> goodSentenceList = new List<Sentence>();

        private List<string> gooodPOSesList;
        private List<string> restrictedWordsList;

        private WordNetManager wordNetManager = new WordNetManager();

        private SynchronizationContext context;

        public event ReferringProgressChangeDelegate ProgressChanged;
        public event ReferringWorkCompleteDelegate WorkCompleted;

        public void RunReferrengProcess(object param)
        {
            try
            {
                context = (SynchronizationContext)param;

                SetProgressPercentage(0);

                Logger.LogInfo("Starting referring process...");

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                ReferringManager.Instance.IsReferringRunning = true;

                Logger.LogInfo("Initialing restricted words and good POSes.");
                InitializeRestrictedWords();
                InitializeGoodPOSes();

                Logger.LogInfo("Getting sentence list.");
                sentenceList = ReferringManager.Instance.OriginalText.ClearUnnecessarySymbolsInText()
                    .DivideTextToSentences()
                    .ClearWhiteSpacesInList()
                    .RemoveEmptyItemsInList()
                    .ToLower();

                Logger.LogInfo("Getting word list.");
                wordList = ReferringManager.Instance.OriginalText.ClearUnnecessarySymbolsInText()
                    .DivideTextToWords()
                    .RemoveEmptyItemsInList()
                    .ToLower();

                Logger.LogInfo(string.Format("Text contains {0} sentences and {1} words.", sentenceList.Count, wordList.Count));

                Logger.LogInfo("Calculate word weights.");
                CalculateWordWeights();

                Logger.LogInfo("Calculating sentence weights.");
                CalculateSentenceWeights();

                Logger.LogInfo("Calculating required sentence count.");
                int sentenceCount = goodSentenceList.Count;
                int requiredSentenceCount = (int)(sentenceCount * ReferringManager.Instance.ReferringCoefficient);
                Logger.LogInfo(string.Format("Required sentences: {0}.", requiredSentenceCount));

                ReferringManager.Instance.OriginalTextSentenceCount = sentenceCount;
                ReferringManager.Instance.ReferredTextSentenceCount = requiredSentenceCount;

                Logger.LogInfo("Taking required sentences with biggest weight.");
                var requiredSentences = goodSentenceList.OrderByDescending(c => c.Weight).Take(requiredSentenceCount).ToList();

                Logger.LogInfo("Using original cases in essay.");
                sentenceListOriginalCase = ReferringManager.Instance.OriginalText.ClearUnnecessarySymbolsInText()
                    .DivideTextToSentences()
                    .ClearWhiteSpacesInList()
                    .RemoveEmptyItemsInList();

                Logger.LogInfo("Building the essay.");
                string essay = BuildEssay(requiredSentences);

                ReferringManager.Instance.ReferredText = essay;
                ReferringManager.Instance.IsReferringCompete = true;
                ReferringManager.Instance.OrderedWordList = goodWordList.TransformPOSToRussian().OrderByDescending(c => c.Weight).ToList();

                //order words and weights by weight only for comfortable view
                var showGoodWordList = ReferringManager.Instance.OrderedWordList;
                var showGoodSentenceList = goodSentenceList.OrderByDescending(c => c.Weight).ToList();

                stopwatch.Stop();

                var ts = stopwatch.Elapsed;
                var elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:000}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);

                Logger.LogInfo("Referring completed. Elapsed time: " + elapsedTime);

                SetProgressPercentage(100);
                context.Send(OnWorkCompleted, elapsedTime);
            }
            catch (Exception ex)
            {
                Logger.LogError("Here is some problem..." + ex);
                MessageManager.ShowError("Возникла проблема..." + ex);

                ReferringManager.Instance.IsReferringRunning = false;
            }
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
            ReferringManager.Instance.ProgressPercentageCurrent = value;
            context.Send(OnProgressChanged, ReferringManager.Instance.ProgressPercentageCurrent);
        }

        private void AddProgressPercentage(double value)
        {
            ReferringManager.Instance.ProgressPercentageCurrent += value;
            context.Send(OnProgressChanged, ReferringManager.Instance.ProgressPercentageCurrent);
        }

        private void CalculateWordWeights()
        {
            double percentageMax = 60;
            double step = (percentageMax - ReferringManager.Instance.ProgressPercentageCurrent) / sentenceList.Count;

            int sentenceIndex = 0;

            //choose sentence
            foreach (var sentence in sentenceList)
            {
                var wordsInSentence = sentence.DivideTextToWords();

                //choose word
                foreach (var wordVariable in wordsInSentence)
                {
                    var word = string.Empty;
                    var stemmedWord = string.Empty;
                    var wordPOS = "-";

                    var synsets = new List<SynSet>();
                    var synsetsWithPOS = new List<SynSet>();

                    word = wordVariable;

                    //check word length
                    if (ReferringManager.Instance.IsWordCutActivated)
                    {
                        if (word.Length < ReferringManager.Instance.WordCutLength)
                            continue;
                    }

                    //stemm the word
                    stemmedWord = Stemm(word);

                    //check if word already in list
                    if (goodWordList.Select(c => Stemm(c.Value)).Contains(stemmedWord))
                        continue;

                    //skip for restricted words
                    if (IsWordRestricted(word))
                        continue;

                    if (ReferringManager.Instance.IsPOSDetectionActivated)
                    {
                        //detect part of speech
                        wordPOS = DetectPOS(word);

                        //is word noun, verb, adjective or adverb?
                        if (!IsPOSGood(wordPOS))
                            continue;
                    }

                    //add current word with using-count characteristics
                    AddWordWithCalculation(word, wordPOS);

                    //Skip next steps if WordNet isn't activated
                    if (!ReferringManager.Instance.IsWordNetActivated)
                        continue;

                    if (ReferringManager.Instance.IsStemmingActivated)
                    {
                        word = stemmedWord;
                    }

                    //search synsets
                    synsets = GetSynsets(word, wordPOS);

                    //try to find synsets another one if there no synsets found
                    if (synsets.Count == 0)
                    {
                        if (!ReferringManager.Instance.IsStemmingActivated)
                        {
                            word = stemmedWord;
                            synsets = GetSynsets(word, wordPOS);
                        }

                        //no synsets are found even after stemming, go to next word
                        if (synsets.Count == 0)
                            continue;
                    }

                    //synsets are found, begin updating

                    //take required synset
                    var requiredSynset = GetRequiredSynset(synsets);

                    //update word weight from synword's using
                    UpdateWordWeight(word, stemmedWord, requiredSynset);
                }

                ++sentenceIndex;
                goodSentenceList.Add(new Sentence { Index = sentenceIndex, Value = sentence, Weight = 0 });
                AddProgressPercentage(step);
            }

            Logger.LogInfo("Word weights are calculated.");
        }

        private void CalculateSentenceWeights()
        {
            double percentageMax = 99;
            double step = (percentageMax - ReferringManager.Instance.ProgressPercentageCurrent) / sentenceList.Count;

            //choose sentence
            foreach (var sentence in goodSentenceList)
            {
                var wordsInSentence = sentence.Value.DivideTextToWords();

                //choose word
                foreach (var wordVariable in wordsInSentence)
                {
                    string stemmedWord = Stemm(wordVariable);

                    if (goodWordList.Select(c => Stemm(c.Value)).Contains(stemmedWord))
                    {
                        var searchWord = goodWordList.Where(c => Stemm(c.Value) == stemmedWord).FirstOrDefault();
                        var searchSentence = goodSentenceList.Where(c => c.Value == sentence.Value).FirstOrDefault();
                        searchSentence.Weight += searchWord.Weight;
                    }
                }

                AddProgressPercentage(step);
            }

            Logger.LogInfo("Sentence weights are calculated.");
        }

        private void AddWord(string word, string pos, int usingCount, int weight)
        {
            string stemmedWord = Stemm(word);

            if (!goodWordList.Select(c => Stemm(c.Value)).Contains(stemmedWord))
            {
                goodWordList.Add(new Word { Value = word, POS = pos, UsingCount = usingCount, Weight = weight });
            }
        }

        private void AddWordWeight(string word, int usingCount)
        {
            Logger.LogInfo(string.Format("Adding {0} weight to word '{1}'.", usingCount, word));

            string stemmedWord = Stemm(word);

            if (goodWordList.Select(c => Stemm(c.Value)).Contains(stemmedWord))
            {
                var searchWord = goodWordList.Where(c=>Stemm(c.Value) == stemmedWord).FirstOrDefault();
                searchWord.Weight += usingCount;
            }
        }

        private void AddWordWithCalculation(string word, string pos)
        {
            int usingCount = CalculateUsingCount(word);
            int weight = usingCount;

            AddWord(word, pos, usingCount, weight);
        }

        private int CalculateUsingCount(string word)
        {
            string stemmedWord = Stemm(word);
            return wordList.Where(c => Stemm(c) == stemmedWord).Count();
        }

        private SynSet GetRequiredSynset(List<SynSet> synsets)
        {
            var requiredSynset = synsets.Last();

            for (int i = 1; i <= synsets.Count; i++)
            {
                int index = synsets.Count - i;
                requiredSynset = synsets[index];

                if (requiredSynset.Words.Count != 1)
                {
                    break;
                }
            }

            return requiredSynset;
        }

        private void UpdateWordWeight(string word, string stemmedWord, SynSet requiredSynset)
        {
            foreach (var synword in requiredSynset.Words)
            {
                string stemmedSynword = string.Empty;

                //delete underscore symbols from synword if it consists of several words and not stemm it
                stemmedSynword = !synword.Contains("_") ? Stemm(synword) : synword.Replace("_", " ");

                if (stemmedSynword == stemmedWord)
                    continue;

                if (ReferringManager.Instance.OriginalText.ToLower().Contains(stemmedSynword))
                {
                    //add weight to word from synword
                    var usingCount = CalculateUsingCount(synword);

                    if (usingCount > 0)
                    {
                        AddWordWeight(word, usingCount);
                    }
                }
            }
        }

        private string BuildEssay(List<Sentence> requiredSentences)
        {
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

        private void InitializeRestrictedWords()
        {
            restrictedWordsList = new List<string> { "is", "am", "are", "was", "has", "it", "it's", "one", "two", "the", "a", "in", "at", "on"};
        }

        private void InitializeGoodPOSes()
        {
            //only adjectives, nouns, adverbs, verbs
            gooodPOSesList = new List<string> { "JJ", "JJR", "JJS", "NN", "NNP", "NNPS", "NNS", "RB", "RBR", "RBS", "VB", "VBD", "VBG", "VBN", "VBP", "VBZ" };
        }
        
        private bool IsWordRestricted(string word)
        {
            if (restrictedWordsList.Contains(word))
                return true;
            else
                return false;
        }

        private bool IsPOSGood(string pos)
        {
            if (gooodPOSesList.Contains(pos))
                return true;
            else
                return false;
        }

        private string DetectPOS(string word)
        {
            return Tagger.DetectPOS(word);
        }

        private string Stemm(string word)
        {
            return Stemmer.Stemm(word);
        }

        private List<SynSet> GetSynsets(string word, params string[] pos)
        {
            if (ReferringManager.Instance.IsPOSDetectionActivated)
            {
                return wordNetManager.GetSynSets(word, pos).ToList();
            }
            else
            {
                return wordNetManager.GetSynSets(word).ToList();
            }
        }
    }
}

////////////////////////

//Take the biggest counted synset without words wit underscore
//var temp1 = synsets.Select(c => c.Words.Where(a=>!a.Contains("_")));
//var temp2 = temp1.OrderByDescending(c => c.Count()).First();

//var temp1 = synsets.Select(c => c.Words.Where(a => !a.Contains("_"))).OrderByDescending(c => c.Count()).First();

///////////////////////