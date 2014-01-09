using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TAS
{
    using TradingTechnologies.TTAPI;

    public partial class TTAPIEvents
    {
        List<string> api_products = new List<string>();

        TradingStatus market_state = TradingStatus.Unknown;
  

        public void subscribeContracts(MarketKey market, string product, ProductType type, string contract)
        {
            Console.WriteLine("Subscribe to {0} {1} {2} {3}", market, product, type, contract);

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

                OrderFeed order_feed=null;
                foreach (OrderFeed of in e.Instrument.GetValidOrderFeeds())
                {
                    if (of.IsTradingEnabled)
                    {
                        order_feed = of;
                        break;
                    }
                }
                if (!Equals(order_feed, null))
                { CreateOrders(e.Instrument, order_feed); }
                else
                { Console.WriteLine("Order creation failed for {0}", e.Instrument.Name); }

                // Subscribe for Inside Market Data
                PriceSubscription priceSub = new PriceSubscription(e.Instrument,
                    Dispatcher.Current);
                priceSub.Settings = new PriceSubscriptionSettings(
                    PriceSubscriptionType.InsideMarket);
                priceSub.FieldsUpdated += new FieldsUpdatedEventHandler(priceSub_FieldsUpdated);
                priceSub.Start();


                Console.WriteLine("Price Subcription started: Waiting for Pre-Open...");
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
            
            try
            {
                
                if (Equals(e.Fields.GetSeriesStatusField().Value, TradingStatus.PreOpen  ))
                {
                    //Stopwatch sw = new Stopwatch();
                    //Stopwatch sw2 = new Stopwatch();
                    //sw.Start();
                    //sw2.Start();
                    if (ready)
                    {
                        
                        foreach (OrderProfile prof in orders)
                        {
                            if (apiInstance.Session.SendOrder(prof))
                            {
                                //sw.Stop();
                                Console.WriteLine("Send Order Success : {0}", prof.SiteOrderKey);
                                //Console.WriteLine(sw.Elapsed);
                                ready = false;
                            }
                            else
                            {
                                Console.WriteLine("Send Order failed : {0}", prof.RoutingStatus.Message); 
                            }
                        }
                        //sw2.Stop();
                        //Console.WriteLine(sw2.Elapsed);
                        
                    }
                }

                if (!Equals(e.Fields.GetSeriesStatusField().Value, market_state))
                {
                    market_state = e.Fields.GetSeriesStatusField().Value;
                    Console.WriteLine("{0}: Current Market State is : {1}", 
                        DateTime.Now.TimeOfDay,
                        market_state);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); 
            }
        }

        private MarketKey mkt(string market)
        {
            switch (market.ToUpper())
            {
                case "CME":
                    return MarketKey.Cme; 
                case "ICE":
                    return MarketKey.Ice;
                default:
                    return MarketKey.Invalid; 
            }
        }


        public ProductType prodtype(string pt)
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

        public void writeLog(object value)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"stopwatch.txt"))
            {
                file.WriteLine(value);
            }
        }




    }
}
