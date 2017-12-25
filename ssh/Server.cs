using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SSH.Algorithms;

namespace SSH
{
    public class Server<TClientState> where TClientState : ClientState, new()
    {
        public List<Client<TClientState>> Clients { get; } = new List<Client<TClientState>>();

        public delegate void ClientConnected(Server<TClientState> sender, TClientState state);
        public delegate void ClientDisconnected(Server<TClientState> sender, TClientState state);
        public delegate void ClientMessage(Server<TClientState> sender, TClientState state);

        public AlgorithmProvider AlgorithmProvider { get; } = new AlgorithmProvider();
        private IConfigurationRoot configuration;
        public string ProtocolVersionExchange;
        private LoggerFactory loggerFactory;
        private ILogger logger;
        private TcpListener listener;
        public Settings Settings { get; }
        private const int DefaultPort = 222;
        private const int ConnectionBacklog = 64;

        public Server(string name, Settings settings = null)
        {
            Settings = settings ?? new Settings();

            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: false)
                .Build();

            loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole(configuration.GetSection("Logging"));
            logger = loggerFactory.CreateLogger(name);
            ProtocolVersionExchange = "SSH-2.0-";

            if (string.IsNullOrEmpty(Settings.ProtocolVersionExchangeName))
            {
                var version = typeof(Server<TClientState>).Assembly.GetName().Version.ToString();
                ProtocolVersionExchange += name.Replace(" ", "_") + "_" + version;
            }
            else
            {
                ProtocolVersionExchange += Settings.ProtocolVersionExchangeName.Replace(" ", "_");
            }

            if (!string.IsNullOrEmpty(Settings.ProtocolVersionExchangeComment))
            {
                ProtocolVersionExchange += " " + Settings.ProtocolVersionExchangeComment;
            }

            if (!string.IsNullOrEmpty(settings.SshRsaPrivateKeyFile))
            {
                var file = File.ReadAllText(settings.SshRsaPrivateKeyFile);
                AlgorithmProvider.HostKeys.Add("ssh-rsa", file);
            }
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

                var client = new Client<TClientState>(this, socket, loggerFactory.CreateLogger(socket.RemoteEndPoint.ToString()));
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
