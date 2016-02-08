using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Referring.Core
{
    public interface IReferringManager
    {
        bool IsReferringCompete { get; set; }
    }

    public class ReferringManager : IReferringManager
    {
        public ReferringManager()
        {
            IsReferringCompete = false;
        }

        public bool IsReferringCompete { get; set; }
    }
}
