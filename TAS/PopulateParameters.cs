using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TAS
{
    using TradingTechnologies.TTAPI;

    class PopulateParameters
    {
        public string account_number = string.Empty;
        public string account_type = string.Empty;

        public ProductKey product = ProductKey.Empty;
        public string contract = string.Empty;
        public InstrumentKey instrument = InstrumentKey.Empty;



        public List<string> order_instructions = new List<string>();

        private string config_file = string.Empty;
        
        //constructor
        public PopulateParameters(string fn)
        {
            config_file = fn;

            readParameters(config_file);
        }

        
        private void readParameters(string iniFileName)
        {
            string line;

            // Read the file and display it line by line.
            if (File.Exists(iniFileName))
            {
                char[] delimiter = { ' ' };
                System.IO.StreamReader file = new System.IO.StreamReader(iniFileName);


                while ((line = file.ReadLine()) != null)
                {
                    string[] p = line.Split(delimiter);

                    switch (p[0].ToUpper())
                    {
                        case "ACCOUNT:":
                            account_number = p[1];
                            account_type = p[2];
                            break;
                        case "CONTRACT:":
                            //CONTRACT: CME CLT FUTURE NOV13
                            product = new ProductKey(p[1], p[3], p[2]);
                            contract = p[4];
                            instrument = new InstrumentKey(p[1], p[3], p[2], p[4]);
                            Console.WriteLine("Instrument: {0}",instrument.ToString());

                            while ((line = file.ReadLine()) != null)
                            {
                                if (line.Length > 0)
                                { order_instructions.Add(line); }
                            }
                            break;
                        default:
                            break;
                    }
                }
                file.Close();
            }
            else
            {
                Console.WriteLine(iniFileName + " is not found! no orders application will exit");
                Console.ReadKey();
                Environment.Exit(1);
            }

        }


    }
}
