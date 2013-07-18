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

                insertInstrumentToTable(e.Instrument);
            }
            else if (e.IsFinal)
            {
                // Instrument was not found and TT API has given up looking for it
                Console.WriteLine("Instrument was NOT found: {0}", e.Error);
            }
        }



        private void priceSub_FieldsUpdated(object sender, FieldsUpdatedEventArgs e)
        {
            if (e.Error == null)
            {

                //TODO debug only change to preopen for release
                if (Equals(e.Fields.GetSeriesStatusField().Value, TradingStatus.Trading))
                {
                    releaseOrders(ready);
                }
                else
                { 
                    Console.WriteLine("{0} Status: {1}",
                        DateTime.Now.TimeOfDay, e.Fields.GetSeriesStatusField().Value);
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


        public void TEST_instrumentLookup()
        {
            InstrumentLookupSubscription req = new InstrumentLookupSubscription(
                        apiInstance.Session,
                        Dispatcher.Current,
                        new ProductKey(MarketKey.Cme, ProductType.Future, "CLT"),
                        "AUG13");
            req.Update += new EventHandler<InstrumentLookupSubscriptionEventArgs>(instrumentLookupSub);
            req.Start();

        }



    }
}
