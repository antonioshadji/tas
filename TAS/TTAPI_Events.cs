using System;
using System.Diagnostics;

namespace TAS
{

    using TradingTechnologies.TTAPI;

    public class TTAPI_Events : IDisposable 
    {
        /// <summary>
        /// Declare the TTAPI objects
        /// </summary>
        private UniversalLoginTTAPI apiInstance = null;
        private WorkerDispatcher workDispatch = null;
        private bool disposed = false;
        private object m_lock = new object();
        private string username = string.Empty;
        private string password = string.Empty;
        private string ttConfig = "orders.ini";

        #region Constructors
   
        public TTAPI_Events(string u, string p, string c)
        {
            username = u;
            password = p;
            ttConfig = c;
        } 
        #endregion
        
        
        /// <summary>
        /// Create and start the Dispatcher
        /// </summary>
        public void Start()
        {
            // Attach a WorkerDispatcher to the current thread
            workDispatch = Dispatcher.AttachWorkerDispatcher();
            workDispatch.BeginInvoke(new Action(Init));
            workDispatch.Run();
        }

        /// <summary>
        /// Initialize TT API
        /// </summary>
        public void Init()
        {
            // Use "Universal Login" Login Mode
            ApiInitializeHandler d = new ApiInitializeHandler(ttApiInitHandler);
            TTAPI.CreateUniversalLoginTTAPI(Dispatcher.Current, username, password, d);
        }


        /// <summary>
        /// Event notification for status of TT API initialization
        /// </summary>
        public void ttApiInitHandler(TTAPI api, Exception ex)
        {
            if (ex == null)
            {
                Console.WriteLine("TT API Initialization Succeeded");
                // Authenticate your credentials
                apiInstance = (UniversalLoginTTAPI)api;
                apiInstance.AuthenticationStatusUpdate += new
                EventHandler<AuthenticationStatusUpdateEventArgs>(
                apiInstance_AuthenticationStatusUpdate);
                apiInstance.Start();
            }
            else
            {
                Console.WriteLine("TT API Initialization Failed: {0}", ex.Message);
                Console.WriteLine("press any key to exit");
                Console.ReadKey();
                Dispose();
            }
        }

        /// <summary>
        /// Event notification for status of authentication
        /// </summary>
        public void apiInstance_AuthenticationStatusUpdate(object sender,
            AuthenticationStatusUpdateEventArgs e)
        {
            if (e.Status.IsSuccess)
            {
                Console.WriteLine("TT API User Authentication Succeeded: {0}", e.Status.StatusMessage);
                
                // Add code here to begin working with the TT API
                PopulateParameters data = new PopulateParameters(ttConfig);
                OrderHandler OrderManager = new OrderHandler(data.account_number, data.account_type);
                InstrumentSubscription sub = new InstrumentSubscription(data, apiInstance.Session);


                Console.ReadKey();
                OrderManager.CreateOrders(sub.instrument, sub.orderfeed, data.order_instructions);
                sub.orders = OrderManager.orders;
                sub.ready = true;

            }
            else
            {
                Console.WriteLine("TT API User Authentication failed: {0}", e.Status.StatusMessage);
                Dispose();
            }
        }



 

        /// <summary>
        /// Shuts down the TT API
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {

                    //// Shutdown all subscriptions
                    //if (data != null)
                    //{
                    //    req.Dispose();
                    //    req = null;
                    //}
                    //if (ps != null)
                    //{
                    //    ps.Dispose();
                    //    ps = null;
                    //}
                    //if (ts != null)
                    //{
                    //    ts.Dispose();
                    //    ts = null;
                    //}

                    // Shutdown the Dispatcher
                    if (workDispatch != null)
                    {
                        workDispatch.BeginInvokeShutdown();
                        workDispatch = null;
                    }

                    // Shutdown the TT API
                    TTAPI.ShutdownCompleted += new EventHandler(TTAPI_ShutdownCompleted);
                    TTAPI.Shutdown();
                }
            }

            disposed = true;
        }


        public void TTAPI_ShutdownCompleted(object sender, EventArgs e)
        {
            // Dispose of your other objects / resources
            Console.WriteLine("exit application");
        }





        }
    }
