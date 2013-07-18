using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace TAS
{
    class Program
    {
        //args contains only command line parameters if any.
        static void Main(string[] args)
        {
            Console.WriteLine("{0}", Environment.GetCommandLineArgs()[0]);
            Console.WriteLine("started at: {0} on {1}",
               DateTime.Now.TimeOfDay,
               DateTime.Now.Date);
        
            string ttUserId = string.Empty;
            string ttPassword = string.Empty; 

            if (args.Length >= 2)
            {
                ttUserId = args[0];
                ttPassword = args[1];
            }

           using (TTAPIEvents tt = new TTAPIEvents(ttUserId, ttPassword))
           {
               tt.Start(); 
           }
        }
    }
}
