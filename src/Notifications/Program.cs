using System;
using System.Collections.Generic;
using System.Threading;

namespace Notifications
{
    class Program
    {
        static bool acceptRequests = true;

        static bool lastAcceptRequests = false;

        static List<byte[]> data = new List<byte[]>();

        public static void Main(string[] args)
        {
			// Register for GC notifications
            GC.RegisterForFullGCNotification(10, 10);

			// Start a thread that will monitor the GC
            Thread monitorGC = new Thread(new ThreadStart(MonitorGC));
            monitorGC.Start();

			// Do actual work
            while (true)
            {
                var newAcceptRequests = acceptRequests;
                if (newAcceptRequests)
                {
                    if (newAcceptRequests != lastAcceptRequests)
                    {
                        Console.WriteLine("Accepting requests");
                    }

					// Allocate more memory to trigger the GC notification
                    data.Add(new byte[1000]);
                }
                else
                {
                    if (newAcceptRequests != lastAcceptRequests)
                    {
                        Console.WriteLine("Redirecting requests");
                    }
                }

                lastAcceptRequests = newAcceptRequests;
            }
        }

        public static void MonitorGC()
        {
            while (true)
            {
				// Wait for full GC approaching notification
                GCNotificationStatus s = GC.WaitForFullGCApproach();
                if (s == GCNotificationStatus.Succeeded)
                {
					// Disable requests
                    acceptRequests = false;

					// Clear existing requests
                    data.Clear();

					// Force a full collection
                    GC.Collect();

					// Enable requests
                    acceptRequests = true;
                }
            }

        }
    }
}
