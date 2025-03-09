using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Constants
{
    public static class FingerprintMatchingStrategy
    {
        public const string Fuzzy = "fuzzy";
        public const string Exact = "exact";
        public const string Loose = "loose";

        public static readonly string[] AllValues = { Fuzzy, Exact, Loose };
    }
}
