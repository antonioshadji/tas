using System;
using System.Data;

namespace TAS
{
    using TradingTechnologies.TTAPI;

    public partial class TTAPIEvents
    {
        public void submitOrder(
            Instrument instrument, 
            BuySell bs,  
            int qty,
            int prc,
            string acct, AccountType acctType)
        {
            OrderFeed oFeed = instrument.GetValidOrderFeeds()[0]; 

            if (oFeed.IsTradingEnabled)
            {
                OrderProfile prof = new OrderProfile(oFeed, instrument);
                prof.BuySell = bs;
                prof.AccountType = acctType;
                prof.AccountName = acct;
                prof.OrderQuantity = Quantity.FromInt(instrument, qty);
                prof.OrderType = OrderType.Limit;
                prof.LimitPrice = Price.FromInt(instrument, prc);

                if (!instrument.Session.SendOrder(prof))
                {
                    Console.WriteLine(
                        string.Format("Send Order failed : {0}",prof.RoutingStatus.Message));
                }
                else
                {
                    Console.WriteLine(
                        string.Format("Send Order Success : {0}", prof.SiteOrderKey));

                }
            }
            else
            {
                Console.WriteLine("Order Feed {0} Is NOT Enabled for Trading",
                oFeed.ToString());
            }
            
        }

        private AccountType act(string type)
        {
            if (string.Equals(type.ToUpper(), "A1"))
            { return AccountType.Agent1; }
            else if (string.Equals(type.ToUpper(), "M1"))
            { return AccountType.MarketMaker1; }
            else if (string.Equals(type.ToUpper(), "P1"))
            { return AccountType.Principal1; }
            else if (string.Equals(type.ToUpper(), "G1"))
            { return AccountType.GiveUp1; }
            else if (string.Equals(type.ToUpper(), "U1"))
            { return AccountType.Unallocated1; }
            else
            { return AccountType.None; }


        }

    }
}
