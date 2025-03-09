using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Constants
{
    public static class Type
    {
        public const string NodeLocked = "node-locked";
        public const string HostedFloating = "hosted-floating";
        public const string OnPremiseFloating = "on-premise-floating";

        public static readonly string[] AllValues = { NodeLocked };
    }
}
