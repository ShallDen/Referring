using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Referring.Core
{
    public class Word
    {
        public string Value { get; set; }
        public string POS { get; set; }
        public int UsingCount { get; set; }
        public int Weight { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}", Value, POS, UsingCount, Weight);
        }
    }
}
