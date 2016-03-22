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
        List<string> goodWordList = new List<string>();

        //Dictionary<string, int> sentenceDictionary = new Dictionary<string, int>();
        //Dictionary<string, int> wordDictionary = new Dictionary<string, int>();

        List<string> gooodPOSesList;
        List<string> restrictedWordsList;

        public void RunReferrengProcess()
        {
            Logger.LogInfo("Starting referring process...");

            WordNetManager wordNetManager = new WordNetManager();

            InitializeRestrictedWords();
            InitializeGoodPOSes();

            sentenceList = ReferringManager.OriginalText.ClearUnnecessarySymbolsInText()
                .DivideTextToSentences()
                .ClearWhiteSpacesInList()
                .RemoveEmptyItemsInList()
                .ToLower();
             
            wordList = ReferringManager.OriginalText.ClearUnnecessarySymbolsInText()
                .DivideTextToWords()
                .RemoveEmptyItemsInList()
                .ToLower();

            
            //choose sentence
            foreach (var sentence in sentenceList)
            {
                var wordsInSentence = sentence.DivideTextToWords();

                //choose word
                foreach (var word in wordsInSentence)
                {
                    //skip for restricted words
                    if (IsWordRestricted(word))
                        continue;

                    //detect part of speech
                    var wordPOS = Tagger.DetectPOS(word);
                    
                    //is word noun, verb, adjective or adverb?
                    if(IsPOSGood(wordPOS))
                    {
                        goodWordList.Add(word);

                        //synsets search
                        var synsets = wordNetManager.GetSynSets(word);
                        var synsets0 = wordNetManager.GetSynSets(word, wordPOS);

                        if (!synsets.Any())
                        {
                            var stemmedWord = Stemmer.Stemm(word);
                            synsets = wordNetManager.GetSynSets(stemmedWord);
                            synsets0 = wordNetManager.GetSynSets(stemmedWord, wordPOS);

                            if (!synsets.Any())
                                continue;
                        }

                        foreach (var synset in synsets)
                        {
                            var words = synset.Words;
                        }

                    }
                }
            }

            var test = goodWordList.Distinct().ToList();
            MessageManager.ShowWarning("This feature isn't implemented yet!");
            Logger.LogWarning("This feature isn't implemented yet!");
        }

       
        private void InitializeRestrictedWords()
        {
            restrictedWordsList = new List<string> { "is", "am", "are", "was", "has", "it", "it's", "two"};
        }

        private void InitializeGoodPOSes()
        {
            //choose only adjectives, nouns, adverbs, verbs
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
    }
}