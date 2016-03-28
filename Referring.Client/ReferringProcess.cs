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
       // List<string> goodWordList = new List<string>();
        List<Word> goodWordList = new List<Word>();

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


            //choose sentence
            foreach (var sentence in sentenceList)
            {
                var wordsInSentence = sentence.DivideTextToWords();

                //choose word
                foreach (var wordVariable in wordsInSentence)
                {
                    string word = wordVariable;
                    string wordPOS = string.Empty;
                    Set<SynSet> synsets = new Set<SynSet>();
                    Set<SynSet> synsetsWithPOS = new Set<SynSet>();

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

                    //goodWordList.Add(new Word { Value = word, UsingCount = 0, Weight = 0 });

                    if (ReferringManager.Instance.IsStemmingForAllTextActivated)
                    {
                        word = Stemm(word);
                    }

                    //synsets search
                    synsets = GetSynsets(word, wordPOS);

                    //try to find synsets another one if there no synsets found
                    if (synsets.Count == 0)
                    {
                        if (!ReferringManager.Instance.IsStemmingForAllTextActivated)
                        {
                            word = Stemm(word);
                            synsets = GetSynsets(word, wordPOS);
                        }

                        //if there no synsets found
                        if (synsets.Count == 0)
                        {
                            AddWordWithCalculation(word, wordPOS);
                            continue;
                        }
                    }

                    foreach (var synset in synsets)
                    {
                        var words = synset.Words;
                    }
                    

                }
            }

            var test = goodWordList.Distinct().ToList();

            MessageManager.ShowWarning("This feature isn't implemented yet!");
            Logger.LogWarning("This feature isn't implemented yet!");
        }

        private void AddWordWithCalculation(string word, string pos)
        {
            int usingCount = CalculateUsingCount(word);
            int weight = usingCount;

            if (!goodWordList.Select(c=>c.Value).Contains(word))
            {
                goodWordList.Add(new Word { Value = word, POS = pos, UsingCount = usingCount, Weight = weight });
            }
        }

        private int CalculateUsingCount(string word)
        {
            return wordList.Where(c => c == word).Count();
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

        private Set<SynSet> GetSynsets(string word, params string[] pos)
        {
            if (ReferringManager.Instance.IsPOSDetectionActivated)
            {
                return wordNetManager.GetSynSets(word, pos);
            }
            else
            {
                return wordNetManager.GetSynSets(word);
            }
        }
    }
}