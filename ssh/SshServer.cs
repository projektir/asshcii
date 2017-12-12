using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ssh
{
    public class SshServer<TClientState> where TClientState : ClientState, new()
    {
        public List<Client<TClientState>> Clients { get; set; }
        public delegate void ClientConnected(SshServer<TClientState> sender, TClientState state);
        public delegate void ClientDisconnected(SshServer<TClientState> sender, TClientState state);
        public delegate void ClientMessage(SshServer<TClientState> sender, TClientState state);

        private IConfigurationRoot configuration;
        private LoggerFactory loggerFactory;
        private ILogger logger;
        private TcpListener listener;
        private const int DefaultPort = 222;
        private const int ConnectionBacklog = 64;

        public SshServer(string name)
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: false)
                .Build();

            loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole(configuration.GetSection("Logging"));
            logger = loggerFactory.CreateLogger(name);
        }

        public void Start()
        {
            // Ensure we are stopped before we start listening
            Stop();

            logger.LogInformation("Starting up...");

            // Create a listener on the required port
            int port = configuration.GetValue<int>("port", DefaultPort);
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start(ConnectionBacklog);

            logger.LogInformation($"Listening on port: {port}");
        }

        public void Poll()
        {
            // Check for new connections
            while (listener.Pending())
            {
                Task<Socket> acceptTask = listener.AcceptSocketAsync();
                acceptTask.Wait();

                Socket socket = acceptTask.Result;
                logger.LogDebug($"New Client: {socket.RemoteEndPoint.ToString()}");

                var client = new Client<TClientState>();
                client.Socket = socket;
                client.Logger = loggerFactory.CreateLogger(socket.RemoteEndPoint.ToString());
                Clients.Add(client);
            }

            Clients.ForEach(c => c.Poll());

            Clients.RemoveAll(c => c.IsConnected == false);
        }

        public void Stop()
        {
            if (listener != null)
            {
                logger.LogInformation("Shutting down...");

                Clients.ForEach(c => c.Disconnect());
                Clients.Clear();

                listener.Stop();
                listener = null;

                logger.LogInformation("Stopped!");
            }
        }

    }
}
