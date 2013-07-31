using System;

namespace TAS
{
    class Program
    {
        //args contains only command line parameters if any.
        static void Main(string[] args)
        {
            Console.WriteLine("{0}", Environment.GetCommandLineArgs()[0]);
            Console.WriteLine("started at: {0}",DateTime.Now);
        
            string ttUserId = string.Empty;
            string ttPassword = string.Empty;
            string ttAccount = string.Empty;

            //string ttExchange = string.Empty; 
            string ttProduct = string.Empty;
            //string ttProductType = string.Empty;
            string ttContractMonth = string.Empty;

            int qty = 0;
            int BuyPrice = 0;
            int SellPrice = 1;

            if (args.Length == 8)
            {
                ttUserId = args[0];
                ttPassword = args[1];
                ttAccount = args[2];

                ttProduct = args[3];
                ttContractMonth = args[4];

                qty = Convert.ToInt16(args[5]);
                BuyPrice = Convert.ToInt16(args[6]);
                SellPrice = Convert.ToInt16(args[7]);

                using (TTAPIEvents tt = new TTAPIEvents(ttUserId, ttPassword, ttAccount,
                    ttProduct, ttContractMonth,
                    qty, BuyPrice, SellPrice))
                {
                    tt.Start();
                }
            }
            else
            { 
                Console.WriteLine("Parameters Required: Username, Password, Account, Product Code, Contract Month MMMYY, Quantity, Buy Price, Sell Price");
                Console.WriteLine("Nothing done; program exiting!  ---  Hit any key to exit.");
                Console.ReadKey();
            }
        }
    }
}
