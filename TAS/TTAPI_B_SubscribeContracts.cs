using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TAS
{
    using TradingTechnologies.TTAPI;

    public partial class TTAPIEvents
    {
        Stopwatch sw = new Stopwatch();
        List<string> api_products = new List<string>();

        public void subscribeContracts(MarketKey market, ProductType type, string product, string contract)
        {
            InstrumentLookupSubscription req = new InstrumentLookupSubscription(
                    apiInstance.Session,
                    Dispatcher.Current,
                    new ProductKey(market, type, product),
                    contract);
            
            string inst = string.Concat(market, type, product, contract);

            if (!api_products.Contains(inst))
            {
                req.Update += new EventHandler<InstrumentLookupSubscriptionEventArgs>(instrumentLookupSub);
                req.Start(); 
                api_products.Add(inst);
            }


            //TODO
            //Cleaning Up When you shut down your application, you should detach all event handlers and 
            //call the Dispose method for each instance of the InstrumentLookupSubscription
            //class as follows:
            //req.Update -+ reqUpdate;
            //req.Dispose();

        }

        private void instrumentLookupSub(object sender, InstrumentLookupSubscriptionEventArgs e)
        {
            if (e.Instrument != null && e.Error == null)
            {
                // Instrument was found
                Console.WriteLine("Instrument was found: {0}",
                    e.Instrument.GetFormattedName(InstrumentNameFormat.Full));

                // Subscribe for Inside Market Data
                PriceSubscription priceSub = new PriceSubscription(e.Instrument,
                    Dispatcher.Current);
                priceSub.Settings = new PriceSubscriptionSettings(
                    PriceSubscriptionType.InsideMarket);
                priceSub.FieldsUpdated += new FieldsUpdatedEventHandler(priceSub_FieldsUpdated);
                priceSub.Start();

                Console.WriteLine("Orders to be submitted on trading status Pre-Open:");
                Console.WriteLine("Contract: {0}", e.Instrument.Name);
                Console.WriteLine("Buy {0} for {1}", BQ, BP);
                Console.WriteLine("Sell {0} at {1}", SQ, SP);
                Console.WriteLine("Waiting for Pre-Open...");
            }
            else if (e.IsFinal)
            {
                // Instrument was not found and TT API has given up looking for it
                Console.WriteLine("Instrument was NOT found: {0}", e.Error);
                Console.WriteLine("WARNING!!!!! NO ORDERS WILL BE SENT!!!!");
            }
        }



        private void priceSub_FieldsUpdated(object sender, FieldsUpdatedEventArgs e)
        {
            if (e.Error == null)
            {
                if (Equals(e.Fields.GetSeriesStatusField().Value, TradingStatus.PreOpen ))
                {
                    if (ready)
                    {
                        submitOrder(e.Fields.Instrument, BuySell.Buy, BQ, BP, ttAccount, AccountType.Agent1);
                        submitOrder(e.Fields.Instrument, BuySell.Sell, SQ, SP, ttAccount, AccountType.Agent1);
                        ready = false;
                    }
                   
                }

            }
            else
            {
                Console.WriteLine(e.Error); 
            }
        }

        private MarketKey mkt(string market)
        {
            if (string.Equals(market.ToUpper(), "CME"))
            { return MarketKey.Cme; }
            else
            { return MarketKey.Invalid; }
        }

        private ProductType pt(string type)
        {
            if (string.Equals(type.ToUpper(), "FUTURE"))
            { return ProductType.Future; }
            else if (string.Equals(type.ToUpper(), "SPREAD"))
            { return ProductType.Spread; }
            else
            { return ProductType.Invalid; }
        }

        public void writeLog(object value)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"stopwatch.txt"))
            {
                file.WriteLine(value);
            }
        }




    }
}
