using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Referring.Core
{
    public class Sentence
    {
        public int Index { get; set; }
        public string Value { get; set; }
        public int Weight { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}", Index, Value,  Weight);
        }
    }
}
