using Referring.Stemming;

namespace Referring.Core
{
    public static class Stemmer
    {
        static EnglishStemmer stemmer;

        static Stemmer()
        {
            stemmer = new EnglishStemmer();
        }

        public static string Stemm(string word)
        {
            var result = stemmer.Stem(word);
            return result;
        }
    }
}
