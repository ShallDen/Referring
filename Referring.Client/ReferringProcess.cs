﻿using System;
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

                    Set<SynSet> synsets = new Set<SynSet>();
                    Set<SynSet> synsetsWithPOS = new Set<SynSet>();

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

                    //add current word with using-count characteristics
                    AddWordWithCalculation(word, wordPOS);

                    if (ReferringManager.Instance.IsStemmingForAllTextActivated)
                    {
                        word = stemmedWord;
                    }

                    //synsets search
                    synsets = GetSynsets(word, wordPOS);

                    //try to find synsets another one if there no synsets found
                    if (synsets.Count == 0)
                    {
                        if (!ReferringManager.Instance.IsStemmingForAllTextActivated)
                        {
                            word = stemmedWord;
                            synsets = GetSynsets(word, wordPOS);
                        }

                        //no synsets are found, go to next word
                        if (synsets.Count == 0)
                        {
                            continue;
                        }
                    }

                    //synsets are founded, begin processing

                    //take last synset

                    var lastSynset = synsets.Last();

                    //go to next word if there no synonyms in synset
                    if (lastSynset.Words.Count == 1)
                    {
                        continue;
                    }

                    foreach (var synword in lastSynset.Words)
                    {
                        string stemmedSynword = Stemm(synword);

                        if (stemmedSynword == stemmedWord)
                        {
                            continue;
                        }

                        if (ReferringManager.Instance.OriginalText.Contains(stemmedSynword))
                        {
                            //TODO: think how to fix related weight problem //says, tells
                            int usingCount = CalculateUsingCount(stemmedSynword);
                            UpdateWordWeight(word, usingCount);
                        }
                    }
                }

                goodSentenceList.Add(new Sentence { Value = sentence, Weight = 0 });
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

            var showGoodSentenceList = goodSentenceList.OrderByDescending(c => c.Weight).ToList();

            int sentenceCount = goodSentenceList.Count;
            int requiredSentenceCount = (int)(sentenceCount * ReferringManager.Instance.ReferringCoefficient);

            var requiredSentences = showGoodSentenceList.Take(requiredSentenceCount).ToList();

            string essay = string.Empty;

            sentenceListOriginalCase = ReferringManager.Instance.OriginalText.ClearUnnecessarySymbolsInText()
                .DivideTextToSentences()
                .ClearWhiteSpacesInList()
                .RemoveEmptyItemsInList();


            foreach (var sentence in goodSentenceList)
            {
                if(requiredSentences.Contains(sentence))
                {
                    if (!string.IsNullOrEmpty(essay))
                    {
                        essay = string.Format("{0} {1}. ", essay, sentence.Value);
                    }
                    else
                    {
                        essay = string.Format("{0}.", sentence.Value);
                    }
                }
               // sentence.Value
            }

            ReferringManager.Instance.ReferredText = essay;
            ReferringManager.Instance.IsReferringCompete = true;

            MessageManager.ShowInformation("Referring complete! You can save essay to file.");
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
               // goodWordList.Weight += usingCount;
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

////////////////////////

//Take the biggest counted synset without words wit underscore
//var temp1 = synsets.Select(c => c.Words.Where(a=>!a.Contains("_")));
//var temp2 = temp1.OrderByDescending(c => c.Count()).First();

//var temp1 = synsets.Select(c => c.Words.Where(a => !a.Contains("_"))).OrderByDescending(c => c.Count()).First();

///////////////////////