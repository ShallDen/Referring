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

        List<string> gooodPOSesList = new List<string>();
        List<string> restrictedWordsList = new List<string>();

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
            restrictedWordsList.Add("is");
            restrictedWordsList.Add("am");
            restrictedWordsList.Add("are");
            restrictedWordsList.Add("was");
            restrictedWordsList.Add("has");
            restrictedWordsList.Add("it");
            restrictedWordsList.Add("it's");
            restrictedWordsList.Add("two");
        }

        private void InitializeGoodPOSes()
        {
            gooodPOSesList.Add("JJ");  //adjectives
            gooodPOSesList.Add("JJR");
            gooodPOSesList.Add("JJS");

            gooodPOSesList.Add("NN");  //nouns
            gooodPOSesList.Add("NNP");
            gooodPOSesList.Add("NNPS");
            gooodPOSesList.Add("NNS");

            gooodPOSesList.Add("RB");  //adverbs
            gooodPOSesList.Add("RBR");
            gooodPOSesList.Add("RBS");

            gooodPOSesList.Add("VB");  //verbs
            gooodPOSesList.Add("VBD");
            gooodPOSesList.Add("VBG");
            gooodPOSesList.Add("VBN");
            gooodPOSesList.Add("VBP");
            gooodPOSesList.Add("VBZ");
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

  //if (!wordDictionary.ContainsKey(word))
  //{
  //    wordDictionary.Add(word, 0);
  //}
  //else
  //{
  //    wordDictionary[word] += 1;
  //}


//var testwords = ReferringManager.OriginalText.DivideTextToWords()
//    .ClearUnnecessarySymbolsInList()
//    .RemoveEmptyItemsInList()
//    .ToLower();

////var testCleartext = ReferringManager.OriginalText.ClearUnnecessarySymbolsInText();

//string test = Tagger.DetectPOS("feature");
//string stemTest = Stemmer.Stemm("feature");

////  WordNetManager wm = new WordNetManager();