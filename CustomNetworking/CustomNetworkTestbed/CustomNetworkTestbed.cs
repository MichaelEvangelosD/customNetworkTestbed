using System;
using System.Net;

namespace CustomNetworkTestbed
{
    class CustomNetworkTestbed
    {
        public static int Main(string[] args)
        {
            string passedIPAddress = "127.0.0.1";
            string passedPort = "50000";

            //Parsing passed arguments to respected types.
            IPAddress parsedIPAddress = IPAddress.Parse(passedIPAddress);
            int parsedPort = int.Parse(passedPort);

            IPEndPoint serverEndpoint = new IPEndPoint(parsedIPAddress, parsedPort);

            Server server = new Server(serverEndpoint);

            server.RunServer();

            return 0;
        }
    }
}
