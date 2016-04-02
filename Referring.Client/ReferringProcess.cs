using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Referring.Core;
using LAIR.Collections.Generic;
using Referring.WordNet;

namespace Referring.Client
{
    public class ReferringProcess
    {
        List<string> sentenceList = new List<string>();
        List<string> wordList = new List<string>();
        List<string> sentenceListOriginalCase = new List<string>();
        List<string> wordListUpperOriginalCase = new List<string>();

        List<Word> goodWordList = new List<Word>();
        List<Sentence> goodSentenceList = new List<Sentence>();

        List<string> gooodPOSesList;
        List<string> restrictedWordsList;

        WordNetManager wordNetManager = new WordNetManager();

        public void RunReferrengProcess()
        {
            Logger.LogInfo("Starting referring process...");

            InitializeRestrictedWords();
            InitializeGoodPOSes();

            sentenceList = ReferringManager.Instance.OriginalText.ClearUnnecessarySymbolsInText()
                .DivideTextToSentences()
                .ClearWhiteSpacesInList()
                .RemoveEmptyItemsInList()
                .ToLower();

            wordList = ReferringManager.Instance.OriginalText.ClearUnnecessarySymbolsInText()
                .DivideTextToWords()
                .RemoveEmptyItemsInList()
                .ToLower();

            int sentenceIndex = 0;

            //choose sentence
            foreach (var sentence in sentenceList)
            {
                var wordsInSentence = sentence.DivideTextToWords();

                //choose word
                foreach (var wordVariable in wordsInSentence)
                {
                    string word = string.Empty;
                    string stemmedWord = string.Empty;
                    string wordPOS = string.Empty;

                    var synsets = new List<SynSet>();
                    var synsetsWithPOS = new List<SynSet>();

                    word = wordVariable;
                    stemmedWord = Stemm(word);

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

                    ////////////
                    //add current word with using-count characteristics
                    ////////////

                    AddWordWithCalculation(word, wordPOS);

                    if (ReferringManager.Instance.IsStemmingForAllTextActivated)
                    {
                        word = stemmedWord;
                    }

                    ////////////
                    //synsets search
                    ////////////
                    synsets = GetSynsets(word, wordPOS);

                    //try to find synsets another one if there no synsets found
                    if (synsets.Count == 0)
                    {
                        if (!ReferringManager.Instance.IsStemmingForAllTextActivated)
                        {
                            word = stemmedWord;
                            synsets = GetSynsets(word, wordPOS);
                        }

                        //no synsets are found even after stemming, go to next word
                        if (synsets.Count == 0)
                        {
                            continue;
                        }
                    }

                    ////////////
                    //synsets are founded, begin processing
                    ////////////

                    //take last synset
                    var requiredSynset = GetRequiredSynset(synsets);

                    foreach (var synword in requiredSynset.Words)
                    {
                        string stemmedSynword = string.Empty;

                        //delete underscore symbols from synword if it consists of several words and not to stemm it
                        stemmedSynword = !synword.Contains("_") ? Stemm(synword) : synword.Replace("_", " ");

                        if (stemmedSynword == stemmedWord)
                        {
                            continue;
                        }

                        if (ReferringManager.Instance.OriginalText.Contains(stemmedSynword))
                        {
                            //TODO: think how to fix related weight problem //says, tells

                            //add weight to word from synwords
                            int usingCount = CalculateUsingCount(stemmedSynword);
                            UpdateWordWeight(word, usingCount);
                        }
                    }
                }

                ++sentenceIndex;
                goodSentenceList.Add(new Sentence { Index = sentenceIndex, Value = sentence, Weight = 0 });
            }

            var showGoodList = goodWordList.OrderByDescending(c => c.Weight).ToList();

            //Calculate sentences weight
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
            }

            //order sentences by weight
            var showGoodSentenceList = goodSentenceList.OrderByDescending(c => c.Weight).ToList();

            //calculate required sentence count
            int sentenceCount = goodSentenceList.Count;
            int requiredSentenceCount = (int)(sentenceCount * ReferringManager.Instance.ReferringCoefficient);

            //take required sentences with biggest weight 
            var requiredSentences = showGoodSentenceList.Take(requiredSentenceCount).ToList();

            //for good looking
            sentenceListOriginalCase = ReferringManager.Instance.OriginalText.ClearUnnecessarySymbolsInText()
                .DivideTextToSentences()
                .ClearWhiteSpacesInList()
                .RemoveEmptyItemsInList();

            string essay = string.Empty;

            //build essay
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

            ReferringManager.Instance.ReferredText = essay;
            ReferringManager.Instance.IsReferringCompete = true;

            MessageManager.ShowInformation("Referring complete! You can save essay to file.");
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

        private void CalculateSentenceWeight(Sentence sentence)
        {
            
        }

        private void AddWord(string word, string pos, int usingCount, int weight)
        {
            string stemmedWord = Stemm(word);

            if (!goodWordList.Select(c => Stemm(c.Value)).Contains(stemmedWord))
            {
                goodWordList.Add(new Word { Value = word, POS = pos, UsingCount = usingCount, Weight = weight });
            }
        }

        private void UpdateWordWeight(string word, int usingCount)
        {
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
        private int CalculateWeight(string word)
        {
            return CalculateUsingCount(word);
        }

        private void InitializeRestrictedWords()
        {
            restrictedWordsList = new List<string> { "is", "am", "are", "was", "has", "it", "it's", "two"};
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