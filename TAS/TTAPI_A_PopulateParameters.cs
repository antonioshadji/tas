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

        DataSet xmlData = new DataSet("OrderSet");
        DataTable orders;

        public void loadOrders()
        {
            try
            {
                xmlData.ReadXmlSchema("ORDERS.xsd");
                xmlData.ReadXml("ORDERS.XML");
            }
            catch (Exception ex)
            { Console.WriteLine(ex.ToString()); }

            orders = xmlData.Tables["Orders"];
            try
            {
                orders.Columns.Add("Instrument", typeof(Instrument));
                orders.Columns.Add("sent", typeof(bool));
                orders.Columns.Add("BS", typeof(BuySell));
                orders.AcceptChanges();
            }
            catch (Exception ex)
            { Console.WriteLine(ex.ToString()); }


            foreach (DataRow r in orders.Rows)
            {
                r["sent"] = false;
                string display_order = "";
                foreach (DataColumn c in orders.Columns)
                {
                    display_order += (r[c] + " ");
                }
                Console.WriteLine(display_order);

                //<Exchange>CME</Exchange>
                //<ProductType>FUTURE</ProductType>
                //<ProductName>CL</ProductName>
                //<Contract>AUG13</Contract>
                subscribeContracts(
                    mkt((string)r["Exchange"]),
                    pt((string)r["ProductType"]),
                    (string)r["ProductName"],
                    (string)r["Contract"]);

            }

        }

        public void insertInstrumentToTable(Instrument inst)
        {
            string[] contractDef = extractDataFromInstrument(inst);

            foreach (DataRow r in orders.Rows)
            {
                if (string.Equals(r["Exchange"].ToString().ToLower(), contractDef[0].ToLower()))
                {
                    if (string.Equals(r["ProductName"].ToString().ToLower(), contractDef[1].ToLower()))
                    {
                        if (string.Equals(r["Contract"].ToString().ToLower(), contractDef[2].ToLower()))
                        {
                            try
                            {
                                r["Instrument"] = inst;
                                if (string.Equals(r["BuySell"], "BUY"))
                                { r["BS"] = BuySell.Buy; }
                                else if (string.Equals(r["BuySell"], "SELL"))
                                { r["BS"] = BuySell.Sell; }
                                r.AcceptChanges();
                                Console.WriteLine("Instrument saved: {0}", inst.Name);
                            }
                            catch (Exception ex)
                            { Console.WriteLine("ERROR: {0}", ex.ToString()); }
                        }
                    }
                }
            }



            //Console.WriteLine("Product: {0}", inst.Product);
            //Console.WriteLine("Key: {0}", inst.Key);
            //Console.WriteLine("Full Name: {0}", inst.GetFormattedName(InstrumentNameFormat.Full));
            //Console.WriteLine("Name: {0}", inst.Name);
          
        }

        public string[] extractDataFromInstrument(Instrument inst)
        {
            string[] words = inst.Name.Split(' ');

            return words;
        }

    }
}
