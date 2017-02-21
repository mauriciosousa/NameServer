using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace NameServer
{
    public delegate void NotificationEventHandler(object sender, string message);

    public class NSTcpListener
    {

        public event NotificationEventHandler NewNotification; 

        int port;
        private List<Host> _database;

        private TcpListener _server;
        private int BUFFER = 1024;

        public bool running = true;

        public NSTcpListener(int port, List<Host> database)
        {
            this.port = port;
            _database = database;

            _server = new TcpListener(IPAddress.Any, port);
            _server.Start();

            Thread acceptLoop = new Thread(new ParameterizedThreadStart(AcceptClients));
            acceptLoop.Start();
        }

        private void AcceptClients(object o)
        {
            OnNewNotification("Accepting clients on port " + port);
            while (running)
            {
                try
                {
                    TcpClient client = _server.AcceptTcpClient();
                    Thread clientThread = new Thread(new ParameterizedThreadStart(clientHandler));
                    clientThread.Start(client);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Close();
                    OnNewNotification("Please restart the server");
                }
            }
        }


        private void clientHandler(object o)
        {
            OnNewNotification("New connection established");

            TcpClient client = (TcpClient)o;

            using (NetworkStream ns = client.GetStream())
            {
                while (running)
                {

                    byte[] message = new byte[BUFFER];
                    int bytesRead = 0;

                    try
                    {
                        bytesRead = ns.Read(message, 0, BUFFER);
                    }
                    catch
                    {
                        break;
                    }

                    if (bytesRead == 0)
                    {
                        break;
                    }
                    else
                    {
                        //   get/id
                        string ret = "none";
                        string s = System.Text.Encoding.Default.GetString(message);
                        s = Regex.Replace(s, @"\s+ ", "");



                        string[] l = s.Split('/');

                        Console.WriteLine(s.Length);
                        if (l.Length == 3 && l[0] == "get")
                        {
                            OnNewNotification("GET");
                            foreach (Host h in _database)
                            {
                                if (h.id == l[1])
                                {
                                    ret = h.id + "/" + h.address + "/" + h.port;

                                    OnNewNotification("-- Request found for " + h.id);
                                }
                            }
                            if (ret == "none") OnNewNotification("-- No request found for " + l[1]);
                        }
                        else
                        {
                            OnNewNotification("--Invalid Request");
                        }

                        byte[] retb = System.Text.Encoding.Default.GetBytes(ret);
                        ns.Write(retb, 0, retb.Length);
                    }
                }
                OnNewNotification("Connection terminated");
                client.Close();
            }
        }

        internal void Close()
        {
            if (_server != null)
            {
                this._server.Stop();
                running = false;
            }
        }

        protected void OnNewNotification(string s)
        {
            if (NewNotification != null)
            {
                NewNotification(this, s);
            }
        }
    }
}