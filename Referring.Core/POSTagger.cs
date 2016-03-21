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
    }
}
