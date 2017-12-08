using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ssh
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }

    public abstract class ClientState
    {
        public IPAddress IpAddress { get; set; }
    }

    public class SshServer<TClientState> where TClientState : ClientState, new()
    {
        public List<Client<TClientState>> Clients { get; set; }
        public delegate void ClientConnected(SshServer<TClientState> sender, TClientState state);
        public delegate void ClientDisconnected(SshServer<TClientState> sender, TClientState state);
        public delegate void ClientMessage(SshServer<TClientState> sender, TClientState state);
    }

    public class Client<TClientState> where TClientState : ClientState
    {
        public TClientState State { get; set; }
        public TcpClient Socket { get; set; }
    }
}
