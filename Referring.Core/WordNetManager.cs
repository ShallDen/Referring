using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Referring.WordNet;

namespace Referring.Core
{
    public class WordNetManager
    {
        public WordNetManager()
        {
            WordNetEngine we = new WordNetEngine("", false);
            we.GetSynSet("");
        }
    }
}
