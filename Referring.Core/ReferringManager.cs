using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Referring.Core
{
    public static class ReferringManager
    {
        public static string OriginalText { get; set; }
        public static string ReferredText { get; set; }
        public static double ReferringCoefficient { get; set; }
        public static bool IsReferringCompete { get; set; }
    }
}
