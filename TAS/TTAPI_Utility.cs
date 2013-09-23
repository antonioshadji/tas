using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TAS
{
    using TradingTechnologies.TTAPI;

    public static class TTAPI_Utility
    {


        public static MarketKey market(string mkt)
        {
            switch (mkt.ToUpper())
            {
                case "CME":
                    return MarketKey.Cme;
                case "ICE":
                    return MarketKey.Ice;
                default:
                    return MarketKey.Invalid;
            }
        }


        public static ProductType prodtype(string pt)
        {
            switch (pt.ToUpper())
            {
                case "FUTURE":
                    return ProductType.Future;
                case "SPREAD":
                    return ProductType.Spread;
                default:
                    return ProductType.Invalid;
            }
        }

        public static AccountType acctType(string atype)
        {

            switch (atype.ToUpper())
            {
                case "A1":
                    return AccountType.Agent1;
                case "M1":
                    return AccountType.MarketMaker1;
                case "P1":
                    return AccountType.Principal1;
                case "G1":
                    return AccountType.GiveUp1;
                case "U1":
                    return AccountType.Unallocated1;
                default:
                    return AccountType.None;
                 
            }

        }

    }
}
