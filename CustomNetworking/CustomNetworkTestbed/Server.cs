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
        List<Client> clients = new List<Client>();

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

            Console.WriteLine("Server running...");

            while (serverRunning)
            {
                string serverSideInput = Console.ReadLine();

                if (serverSideInput.Equals("/quit"))
                {
                    serverRunning = false;
                    Console.WriteLine("Stopping...");
                    break;
                }

                foreach (Client clientObj in clients)
                {
                    clientObj.clientWriter.Write("Server message: ");
                    clientObj.clientWriter.WriteLine(serverSideInput);
                    clientObj.clientWriter.Flush();
                }
            }

            Console.WriteLine("Stopped!");

            serverListenerSocket.Close();
        }

        void ClientListenerWorker()
        {
            Socket activeSocket;

            while (serverRunning)
            {
                try
                {
                    activeSocket = serverListenerSocket.Accept();
                }
                catch (ObjectDisposedException)
                {
                    DisconnectAllUsers();
                    break;
                }

                Console.WriteLine("Client connected!");

                NetworkStream netStream = new NetworkStream(activeSocket);

                //Open data streams to the active socket (client)
                StreamReader streamReader = new StreamReader(netStream);
                StreamWriter streamWriter = new StreamWriter(netStream);

                streamWriter.WriteLine("Hello! Thanks for connecting!");
                streamWriter.Flush();

                Client newClient = new Client(this, activeSocket, netStream, streamReader, streamWriter);

                clients.Add(newClient);

                newClient.StartClientThread();
            }
        }

        public bool IsServerRunning()
        {
            return serverRunning;
        }

        public List<Client> GetClientsList()
        {
            return clients;
        }

        public void RemoveClientFromList(int clientID)
        {
            foreach (Client client in clients)
            {
                if (client.ClientID == clientID)
                {
                    clients.Remove(client);
                    break;
                }
            }
        }

        public void DisconnectAllUsers()
        {
            foreach (Client client in GetClientsList())
            {
                client.clientWriter.WriteLine("Server message: Server shutting down");
                client.ForceServerDisconnect();
            }
        }
    }
}
