using System;
using System.Collections.Generic;

namespace TAS
{
    using TradingTechnologies.TTAPI;

    public partial class TTAPIEvents
    {
        public List<OrderProfile> orders = new List<OrderProfile>();

        private void CreateOrders(Instrument instrument, OrderFeed orderfeed)
        {
            char[] delimiter = { ' ' };

            Console.WriteLine("Contract: {0}", instrument.Name);
            foreach (string order in order_instructions )
            {
                OrderProfile profile = new OrderProfile(orderfeed, instrument);
                profile.OrderType = OrderType.Limit;
                profile.AccountName = account_number;
                profile.AccountType = acctType(account_type);
                
                string[] p = order.Split(delimiter);
                if (p.Length == 3)
                {
                    if (String.Equals(p[0], "B"))
                    {
                        profile.BuySell = BuySell.Buy;
                    }
                    else if (String.Equals(p[0], "S"))
                    {
                        profile.BuySell = BuySell.Sell;
                    }

                    profile.OrderQuantity = Quantity.FromString(instrument, p[1]);
                    profile.LimitPrice = Price.FromString(instrument, p[2]);
                    orders.Add(profile);

                    Console.WriteLine("{0} QTY:{1} PRICE:{2}", profile.BuySell, profile.OrderQuantity, profile.LimitPrice);    
                }
            }

  
            Console.WriteLine("order count: {0}", orders.Count);
        }
        
        private AccountType acctType(string type)
        {

            switch (type)
            {
                case "A1":
                    return AccountType.A1;
                case "M1":
                    return AccountType.M1; 
                case "P1":
                    return AccountType.P1; 
                case "G1":
                    return AccountType.G1; 
                case "U1":
                    return AccountType.U1; 
                default:
                    return AccountType.None;
            }

        }

    }
}
