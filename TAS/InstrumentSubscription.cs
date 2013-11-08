using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TAS
{
    using TradingTechnologies.TTAPI;

    class InstrumentSubscription
    {
        private List<string> api_products = new List<string>();

        private TradingStatus market_state = TradingStatus.Unknown;

        private ProductKey key = ProductKey.Empty;
        private string contract = string.Empty;

        private Session session = null;

        internal Instrument instrument = null;
        internal OrderFeed orderfeed = null;
        public bool ready = false;
        public List<OrderProfile> orders = new List<OrderProfile>();

        private PopulateParameters data = null;

        //constructor
        public InstrumentSubscription(PopulateParameters pp, Session s)
        {
            key = pp.product;
            contract = pp.contract;
            data = pp;
            session = s;

            subscribeContracts(key, contract);
        }

        private void subscribeContracts(ProductKey pk, string contract)
        {
            Console.WriteLine("Subscribe to {0} {1}", pk.Name, contract);

            InstrumentLookupSubscription req = new InstrumentLookupSubscription(
                    session,
                    Dispatcher.Current,
                    pk, contract);

            string inst = string.Concat(pk.Name, contract);

            if (!api_products.Contains(inst))
            {
                req.Update += new EventHandler<InstrumentLookupSubscriptionEventArgs>(instrumentLookupSub);
                req.Start();
                Console.WriteLine("Instrument Lookup event handler added");
                api_products.Add(inst);
            }
        }


        private void instrumentLookupSub(object sender, InstrumentLookupSubscriptionEventArgs e)
        {
            if (e.Instrument != null && e.Error == null)
            {
                // Instrument was found
                Console.WriteLine("Instrument was found: {0}",
                    e.Instrument.GetFormattedName(InstrumentNameFormat.Full));

                instrument = e.Instrument;
                orderfeed = e.Instrument.GetValidOrderFeeds()[0];

                //CreateOrders(e.Instrument, e.Instrument.GetValidOrderFeeds()[0]);

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
            if (e.Error == null)
            {
                if (Equals(e.Fields.GetSeriesStatusField().Value, TradingStatus.PreOpen))
                {
                    //Stopwatch sw = new Stopwatch();
                    //Stopwatch sw2 = new Stopwatch();
                    //sw.Start();
                    //sw2.Start();
                    if (ready)
                    {
                        SubmitOrderList();
                        //sw2.Stop();
                        //Console.WriteLine(sw2.Elapsed);

                        ready = false;
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
            else
            {
                Console.WriteLine(e.Error);
            }
        }

        private void SubmitOrderList()
        {
            foreach (OrderProfile prof in this.orders)
            {
                if (session.SendOrder(prof))
                {
                    //sw.Stop();
                    Console.WriteLine("Send Order Success : {0}", prof.SiteOrderKey);
                    //Console.WriteLine(sw.Elapsed);
                }
                else
                {
                    Console.WriteLine("Send Order failed : {0}", prof.RoutingStatus.Message);
                }
            }
        }



    }
}
