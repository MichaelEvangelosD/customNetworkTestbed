using System;
using System.Net;
using System.Net.Sockets;

namespace CustomNetworkTestbed
{
    class CustomNetworkTestbed
    {
        public static int Main(string[] args)
        {
            if(args.Length < 2)
            {
                Console.WriteLine("Less passed arguments than needed!");
                return 1;
            }

            string passedIPAddress = args[0];
            string passedPort = args[1];

            //Parsing passed arguments to respected types.
            IPAddress parsedIPAddress = IPAddress.Parse(passedIPAddress);
            int parsedPort = int.Parse(passedPort);

            //TODO: RUN()

            return 0;
        }
    }
}
