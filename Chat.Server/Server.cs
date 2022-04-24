using System.Collections.Generic;

using System;

using System.Text;

using System.Net;

using System.Net.Sockets;

using System.IO;

using System.Threading;

using System.Collections;

namespace ConsoleApplication1

{

    public class Serv

    {

        public static Dictionary<string, ClientHandler> clientHandlers = new Dictionary<string, ClientHandler>();

        public void Start()

        {

            try

            {

                IPAddress ipAd = IPAddress.Parse("127.0.0.1"); //localhost IP address,

                TcpListener myList = new TcpListener(ipAd, 8001); //and use the same at

                myList.Start(); //the client side

                Console.WriteLine("The server is running at port 8001...");

                Console.WriteLine("The local End point is :" + myList.LocalEndpoint);

                while (true)

                {

                    Console.WriteLine("Waiting for a connection.....");

                    Socket s = myList.AcceptSocket();

                    ClientHandler ch = new ClientHandler(s);

                    lock (clientHandlers)

                    {

                        NetworkStream socketStream = new NetworkStream(s);

                        BinaryReader reader = new BinaryReader(socketStream);

                        string txt = reader.ReadString(); // get nickname

                        string nick = txt.Substring(4);

                        clientHandlers[nick] = ch;

                    }

                    Thread t = new Thread(new ThreadStart(ch.HandleClient));

                    t.Start();

                }

                // myList.Stop();

            }

            catch (Exception e)

            {

                Console.WriteLine("Error..... " + e.StackTrace);

            }

        }

    }

    public class ClientHandler

    {

        private Socket s;

        private NetworkStream socketStream;

        public BinaryWriter writer;

        private BinaryReader reader;

        public ClientHandler(Socket s)

        {

            this.s = s;

            socketStream = new NetworkStream(s);

            writer = new BinaryWriter(socketStream);

            reader = new BinaryReader(socketStream);

        }

        public void HandleClient()

        {

            Console.WriteLine("Connection accepted from " + s.RemoteEndPoint);

            string txt = "retur ";

            while (true)

            {

                txt = reader.ReadString();

                lock (Serv.clientHandlers)

                {

                    // forudsætter ALL:

                    foreach (var pair in Serv.clientHandlers)

                    {

                        (pair.Value.writer).Write("TO ALL: " + txt );

                    }

                }

            }

        }

    }

}