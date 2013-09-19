using System;

namespace TAS
{
    class Program
    {
        //args contains only command line parameters if any.
        static void Main(string[] args)
        {
            //Environment.GetCommandLineArgs starts with executable name, then command line args
            Console.WriteLine("{0}", Environment.GetCommandLineArgs()[0]);
            Console.WriteLine("started at: {0}",DateTime.Now);
        
            string ttUserId = string.Empty;
            string ttPassword = string.Empty;
            string ttConfig = string.Empty;           

            if (args.Length == 3)
            {
                ttUserId = args[0];
                ttPassword = args[1];
                ttConfig = args[2];


                using (TTAPIEvents tt = new TTAPIEvents(
                    ttUserId, 
                    ttPassword, 
                    ttConfig))
                {
                    tt.Start();
                }
            }
            else
            { 
                Console.WriteLine("Parameters Required: Username, Password, Filename of text configuration file.");
                Console.WriteLine("Nothing done; program exiting!  ---  Hit any key to exit.");
                Console.ReadKey();
            }
        }
    }
}
