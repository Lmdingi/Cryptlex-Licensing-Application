using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Constants
{
    public static class ExpirationStrategy
    {
        public const string Immediate = "immediate";
        public const string Delayed = "delayed";
        public const string Rolling = "rolling";

        public static readonly string[] AllValues = { Immediate, Delayed, Rolling };
    }
}
