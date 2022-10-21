using System;
using System.IO;
using System.Threading;
using System.Net.Sockets;

namespace CustomNetworkTestbed
{
    public static class ClientIDs
    {
        static int currentID = 0;

        public static int GetID()
        {
            currentID++;
            return currentID;
        }
    }

    public class Client
    {
        public int ClientID { get; private set; }

        public Server Server { get; private set; }

        public Socket clientSock { get; private set; }
        public NetworkStream networkStream { get; private set; }
        public StreamReader clientReader { get; private set; }
        public StreamWriter clientWriter { get; private set; }

        Thread clientThread;

        private Client(Server clientServer)
        {
            this.Server = clientServer;
            ClientID = ClientIDs.GetID();
        }

        public Client(Server server, Socket clientSock, NetworkStream networkStream, StreamReader clientReader, StreamWriter clientWriter) : this(server)
        {
            this.clientSock = clientSock;
            this.networkStream = networkStream;
            this.clientReader = clientReader;
            this.clientWriter = clientWriter;
        }

        public void StartClientThread()
        {
            clientThread = new Thread(ClientSideThread);
            clientThread.Start();
        }

        void ClientSideThread()
        {
            while (Server.IsServerRunning())
            {
                try
                {
                    string tempStr = clientReader.ReadLine();

                    foreach (Client client in Server.GetClientsList())
                    {
                        if (client.ClientID == ClientID)
                        {
                            client.clientWriter.Write("You: ");
                            client.clientWriter.Write(tempStr);
                            client.clientWriter.WriteLine();
                            client.clientWriter.Flush();
                            continue;
                        }

                        client.clientWriter.Write($"User#{ClientID}: ");
                        client.clientWriter.WriteLine(tempStr);
                        client.clientWriter.Flush();
                    }
                }
                catch (IOException)
                {
                    break;
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
            }

            ForceServerDisconnect();
        }

        public void ForceServerDisconnect()
        {
            Console.WriteLine($"Client {ClientID} disconected");

            Server.RemoveClientFromList(ClientID);

            clientReader.Close();
            clientWriter.Close();
            networkStream.Close();
            clientSock.Close();
        }
    }
}
