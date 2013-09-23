using System;
using System.Collections.Generic;

namespace TAS
{
    using TradingTechnologies.TTAPI;

    public class OrderHandler
    {
        public List<OrderProfile> orders = new List<OrderProfile>();

        private string account_number = string.Empty;
        private string account_type = string.Empty;

        //constructor
        public OrderHandler(string acct, string acctType)
        {
            
            account_number = acct;
            account_type = acctType;
            
        }

        public void CreateOrders(Instrument instrument, OrderFeed orderfeed, List<string> order_instructions)
        {
            char[] delimiter = { ' ' };

            Console.WriteLine("Contract: {0}", instrument.Name);
            foreach (string order in order_instructions )
            {
                OrderProfile profile = new OrderProfile(orderfeed, instrument);
                profile.OrderType = OrderType.Limit;
                profile.AccountName = account_number;
                profile.AccountType = TTAPI_Utility.acctType(account_type);
                
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
        
     

    }
}
