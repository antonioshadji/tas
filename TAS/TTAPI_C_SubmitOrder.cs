using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data;

namespace TAS
{
    using TradingTechnologies.TTAPI;
    using TradingTechnologies.TTAPI.Tradebook;

    public partial class TTAPIEvents
    {

        private void releaseOrders(bool trade_ready)
        {
            if (trade_ready)
            {
                foreach (DataRow r in orders.Rows)
                {

                    if (!(bool)r["sent"])
                    {
                        submitOrder(
                            (Instrument)r["Instrument"],
                            (BuySell)r["BS"],
                            (int)r["qty"],
                            (int)r["price"],
                            (string)r["Account"],
                            (AccountType)r["AccoutnType"]);
                        r["sent"] = true;
                    }
                    else
                    { Console.WriteLine("already sent"); }
                }
            }
            else
            { Console.WriteLine(DateTime.Now.TimeOfDay.ToString() +  " Waiting..."); }
        }


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
