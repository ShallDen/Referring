using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Referring.WordNet;
using LAIR.Collections.Generic;

namespace Referring.Core
{
    public class WordNetManager
    {
        private static WordNetEngine wordNetEngine;

        static WordNetManager()
        {
            wordNetEngine = new WordNetEngine(@"C:\Program Files (x86)\WordNet\2.1\dict", false);
        }

        private WordNetEngine.POS TransformPOS(string pos)
        {
            var translatedPOS = WordNetEngine.POS.None;

            switch (pos)
            {
                case "JJ":   translatedPOS = WordNetEngine.POS.Adjective; break;
                case "JJR":  translatedPOS = WordNetEngine.POS.Adjective; break;
                case "JJS":  translatedPOS = WordNetEngine.POS.Adjective; break;

                case "NN":   translatedPOS = WordNetEngine.POS.Noun; break;
                case "NNP":  translatedPOS = WordNetEngine.POS.Noun; break;
                case "NNPS": translatedPOS = WordNetEngine.POS.Noun; break;
                case "NNS":  translatedPOS = WordNetEngine.POS.Noun; break;

                case "RB":   translatedPOS = WordNetEngine.POS.Adverb; break;
                case "RBR":  translatedPOS = WordNetEngine.POS.Adverb; break;
                case "RBS":  translatedPOS = WordNetEngine.POS.Adverb; break;

                case "VB":   translatedPOS = WordNetEngine.POS.Verb; break;
                case "VBD":  translatedPOS = WordNetEngine.POS.Verb; break;
                case "VBG":  translatedPOS = WordNetEngine.POS.Verb; break;
                case "VBN":  translatedPOS = WordNetEngine.POS.Verb; break;
                case "VBP":  translatedPOS = WordNetEngine.POS.Verb; break;
                case "VBZ":  translatedPOS = WordNetEngine.POS.Verb; break;

                default:     translatedPOS = WordNetEngine.POS.Noun; break;
            }

            return translatedPOS;
        }

        public Set<SynSet> GetSynSets(string word, params string[] pos)
        {
            try
            {
                if (pos == null || pos.Length == 0)
                {
                    //Get synset with all parts of speech
                    return wordNetEngine.GetSynSets(word);
                }
                else
                {
                    //Get synsets with restriction of part of speech
                    var translatedPOS = TransformPOS(pos.First());
                    return new Set<SynSet>();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Unable to get synset. " + ex);
                return new Set<SynSet>();
            }
        }
    }
}
