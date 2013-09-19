using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace TAS
{
    using TradingTechnologies.TTAPI;

    public partial class TTAPIEvents
    {
        public string account_number = string.Empty;
        public string account_type = string.Empty;
        public List<string> order_instructions = new List<string>();


        public void readParameters(string iniFileName)
        {
            string line;

            // Read the file and display it line by line.
            if (File.Exists(iniFileName))
            {
                char[] delimiter = {' '};
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
                            subscribeContracts(mkt(p[1]), p[2], prodtype(p[3]) , p[4]);
                            while ((line = file.ReadLine()) != null)
                            {
                                order_instructions.Add(line);
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
                Console.WriteLine(iniFileName +" is not found! no orders application will exit");
                Console.ReadKey();
                Environment.Exit(1);
            }
            
        }


      
    }
}
