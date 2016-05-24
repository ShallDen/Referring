using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Referring.WordNet;
using LAIR.Collections.Generic;
using System.IO;

namespace Referring.Core
{
    public class WordNetManager
    {
        private static WordNetEngine wordNetEngine;

        public WordNetManager()
        {
            wordNetEngine = new WordNetEngine(ReferringManager.Instance.WordNetDirectory, false);
        }

        public static void CheckWordNetPaths(string wordNetDirectory)
        {
            if (!Directory.Exists(wordNetDirectory))
                throw new DirectoryNotFoundException("Отсутствует WordNet директория: \n" + wordNetDirectory);

            // get data and index paths
            string[] dataPaths = new string[]
            {
                Path.Combine(wordNetDirectory, "data.adj"),
                Path.Combine(wordNetDirectory, "data.adv"),
                Path.Combine(wordNetDirectory, "data.noun"),
                Path.Combine(wordNetDirectory, "data.verb")
            };

            string[] indexPaths = new string[]
            {
                Path.Combine(wordNetDirectory, "index.adj"),
                Path.Combine(wordNetDirectory, "index.adv"),
                Path.Combine(wordNetDirectory, "index.noun"),
                Path.Combine(wordNetDirectory, "index.verb")
            };

            // make sure all files exist
            foreach (string path in dataPaths.Union(indexPaths))
                if (!File.Exists(path))
                    throw new FileNotFoundException("Отсутствует WordNet файл: \n" + path);
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
                    return wordNetEngine.GetSynSets(word, translatedPOS);
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
