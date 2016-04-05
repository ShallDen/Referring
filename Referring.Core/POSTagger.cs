using System.IO;
using System.Linq;
using Referring.POSTagger;

namespace Referring.Core
{
    public static class Tagger
    {
        static string lexicon;
        static FastTag fastTag;

        static Tagger()
        {
            lexicon = File.ReadAllText("Grammar\\lexicon.txt");
            fastTag = new FastTag(lexicon);
        }

        public static string DetectPOS(string word)
        {
            var result = fastTag.Tag(word);
            return result.First().PosTag;
        }

        public static string TransformPOSToRussian(string pos)
        {
            string translatedPOS = string.Empty;

            switch (pos)
            {
                case "JJ": translatedPOS = "прилагательное"; break;
                case "JJR": translatedPOS = "прилагательное"; break;
                case "JJS": translatedPOS = "прилагательное"; break;

                case "NN": translatedPOS = "существительное"; break;
                case "NNP": translatedPOS = "существительное"; break;
                case "NNPS": translatedPOS = "существительное"; break;
                case "NNS": translatedPOS = "существительное"; break;

                case "RB": translatedPOS = "наречие"; break;
                case "RBR": translatedPOS = "наречие"; break;
                case "RBS": translatedPOS = "наречие"; break;

                case "VB": translatedPOS = "глагол"; break;
                case "VBD": translatedPOS = "глагол"; break;
                case "VBG": translatedPOS = "глагол"; break;
                case "VBN": translatedPOS = "глагол"; break;
                case "VBP": translatedPOS = "глагол"; break;
                case "VBZ": translatedPOS = "глагол"; break;
                case "-": translatedPOS = "-"; break;

                default: translatedPOS = "существительное"; break;
            }

            return translatedPOS;
        }
    }
}
