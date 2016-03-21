using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Referring.Core;

namespace Referring.Client
{
    public class ReferringProcess
    {
        List<string> sentenceList = new List<string>();
        List<string> wordList = new List<string>();

        Dictionary<string, int> sentenceDictionary = new Dictionary<string, int>();
        Dictionary<string, int> wordDictionary = new Dictionary<string, int>();

        List<string> gooodPOSesList = new List<string>();
        List<string> restrictedWordsList = new List<string>();

        public void RunReferrengProcess()
        {
            Logger.LogInfo("Starting referring process...");

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

            var list = new List<string>();

            foreach (var sentence in sentenceList)
            {
               // sentenceDictionary.Add(sentence, 0);

                var wordsInSentence = sentence.DivideTextToWords();

                foreach (var word in wordsInSentence)
                {
                    if (IsWordRestricted(word))
                        continue;

                    var wordPOS = Tagger.DetectPOS(word);
                    
                    if(IsPOSGood(wordPOS))
                    {
                        list.Add(word);


                    }
                }
            }

            var test = list.Distinct().ToList();
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