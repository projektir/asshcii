using System.Net.Sockets;
using Microsoft.Extensions.Logging;

namespace ssh
{
    public class Client<TClientState> where TClientState : ClientState
    {
        public TClientState State { get; set; }
        public Socket Socket { get; set; }
        public ILogger Logger { get; set; }

        public bool IsConnected { get { return Socket != null; }}

        public void Poll()
        {
        }

        public void Disconnect()
        {
            Logger.LogDebug("Disconnected");
            if (Socket != null)
            {
                try
                {
                    Socket.Shutdown(SocketShutdown.Both);
                }
                catch { }
                Socket = null;
            }
        }
    }
}
