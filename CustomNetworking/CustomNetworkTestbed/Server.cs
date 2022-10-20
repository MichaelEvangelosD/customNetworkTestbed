using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace CustomNetworkTestbed
{
    public class Server
    {
        IPEndPoint ipEndPoint;
        Socket serverListenerSocket;

        bool serverRunning = false;
        List<StreamWriter> clientWriters = new List<StreamWriter>();

        public Server(IPEndPoint iPEndPoint)
        {
            this.ipEndPoint = iPEndPoint;
        }

        public void RunServer()
        {
            serverListenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverListenerSocket.Bind(ipEndPoint);
            serverListenerSocket.Listen(10);

            serverRunning = true;

            //Create the listener thread
            Thread serverClientListener = new Thread(ClientListenerWorker);
            serverClientListener.Start();

            while (serverRunning)
            {
                string serverSideInput = Console.ReadLine();

                if (serverSideInput.Equals("/quit"))
                {
                    serverRunning = false;
                    Console.WriteLine("Stopping...");
                    break;
                }

                foreach (StreamWriter clientWriter in clientWriters)
                {
                    clientWriter.WriteLine(serverSideInput);
                    clientWriter.Flush();
                }
            }

            Console.WriteLine("Stopped!");

            serverListenerSocket.Close();
        }

        void ClientListenerWorker()
        {
            while (serverRunning)
            {
                Socket activeSocket = serverListenerSocket.Accept();
                Console.WriteLine("Client connected!");

                NetworkStream netStream = new NetworkStream(activeSocket);

                //Open data streams to the active socket (client)
                StreamReader streamReader = new StreamReader(netStream);
                StreamWriter streamWriter = new StreamWriter(netStream);

                streamWriter.WriteLine("Hello! Thanks for connecting!");
                streamWriter.Flush();

                clientWriters.Add(streamWriter);

                Thread clientThread = new Thread(ClientSide);
                clientThread.Start(streamReader);
            }
        }

        void ClientSide(object obj)
        {
            StreamReader reader = (StreamReader)obj;

            while (serverRunning)
            {
                string tempStr = reader.ReadLine();

                foreach (StreamWriter writer in clientWriters)
                {
                    writer.WriteLine(tempStr);
                    writer.Flush();
                }
            }
        }
    }
}
