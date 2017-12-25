using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SSH.Algorithms;
using SSH.Algorithms.Kex;
using SSH.Packets;

namespace SSH
{
    public class Client<TClientState> where TClientState : ClientState, new()
    {
        public Server<TClientState> Server { get; set; }
        public TClientState State { get; set; }
        public Socket Socket { get; set; }
        public ILogger Logger { get; set; }
        public bool IsConnected { get { return Socket != null; } }
        public byte[] SessionID { get; private set; }

        private bool hasCompletedProtocolVersionExchange = false;
        private string protocolVersionExchange;
        private KexInit KexInitServerToClient;
        private KexInit KexInitClientToServer;
        private ExchangeContext ActiveExchangeContext = new ExchangeContext();
        private ExchangeContext PendingExchangeContext = new ExchangeContext();

        public Client(Server<TClientState> server, Socket socket, ILogger logger)
        {
            this.Socket = socket;
            this.Logger = logger;

            this.Socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            this.Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
            this.Server = server;

            KexInitServerToClient = KexInit.FromAlgorithmProvider(Server.AlgorithmProvider);

            const int socketBufferSize = 2 * Packet.MaxPacketSize;
            Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, socketBufferSize);
            Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, socketBufferSize);
            Socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);

            // 4.2.Protocol Version Exchange - https://tools.ietf.org/html/rfc4253#section-4.2
            Send($"{Server.ProtocolVersionExchange}\r\n");

            // 7.1.  Algorithm Negotiation - https://tools.ietf.org/html/rfc4253#section-7.1
            Send(KexInitServerToClient);
        }

        public void Poll()
        {
            if (!IsConnected)
                return;

            bool dataAvailable = Socket.Poll(0, SelectMode.SelectRead);
            if (dataAvailable)
            {
                int read = Socket.Available;
                if (read < 1)
                {
                    Disconnect();
                    return;
                }

                if (!hasCompletedProtocolVersionExchange)
                {
                    // Wait for CRLF
                    try
                    {
                        ReadProtocolVersionExchange();
                        if (hasCompletedProtocolVersionExchange)
                        {
                            // TODO: Consider processing Protocol Version Exchange for validity
                            Logger.LogDebug($"Received ProtocolVersionExchange: {protocolVersionExchange}");
                        }
                    }
                    catch (Exception)
                    {
                        Disconnect();
                        return;
                    }
                }

                if (hasCompletedProtocolVersionExchange)
                {
                    try
                    {
                        Packet packet = Packet.ReadPacket(this, Socket);
                        while (packet != null)
                        {
                            Logger.LogDebug($"Received Packet: {packet.PacketType}");
                            HandlePacket(packet);

                            packet = Packet.ReadPacket(this, Socket);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex.Message);
                        Disconnect();
                        return;
                    }
                }
            }
        }

        private void HandlePacket(Packet packet)
        {
            try
            {
                HandleSpecificPacket((dynamic)packet);
            }
            catch (RuntimeBinderException)
            {
                Logger.LogError($"Could not handle packet {packet.PacketType}");
                // TODO: Send an SSH_MSG_UNIMPLEMENTED if we get here
            }
        }

        private void HandleSpecificPacket(KexInit packet)
        {
            Logger.LogDebug("Received KexInit");

            if (PendingExchangeContext == null)
            {
                Logger.LogDebug("Re-exchanging keys!");
                PendingExchangeContext = new ExchangeContext();
                Send(KexInitServerToClient);
            }

            KexInitClientToServer = packet;

            PendingExchangeContext.KexAlgorithm = packet.PickKexAlgorithm(Server.AlgorithmProvider);
            PendingExchangeContext.HostKeyAlgorithm = packet.PickHostKeyAlgorithm(Server.AlgorithmProvider);
            PendingExchangeContext.CipherClientToServer = packet.PickCipherClientToServer(Server.AlgorithmProvider);
            PendingExchangeContext.CipherServerToClient = packet.PickCipherServerToClient(Server.AlgorithmProvider);
            PendingExchangeContext.MACAlgorithmClientToServer = packet.PickMACAlgorithmClientToServer(Server.AlgorithmProvider);
            PendingExchangeContext.MACAlgorithmServerToClient = packet.PickMACAlgorithmServerToClient(Server.AlgorithmProvider);
            PendingExchangeContext.CompressionClientToServer = packet.PickCompressionAlgorithmClientToServer(Server.AlgorithmProvider);
            PendingExchangeContext.CompressionServerToClient = packet.PickCompressionAlgorithmServerToClient(Server.AlgorithmProvider);

            Logger.LogDebug($"Selected KexAlgorithm: {PendingExchangeContext.KexAlgorithm.Name}");
            Logger.LogDebug($"Selected HostKeyAlgorithm: {PendingExchangeContext.HostKeyAlgorithm.Name}");
            Logger.LogDebug($"Selected CipherClientToServer: {PendingExchangeContext.CipherClientToServer.Name}");
            Logger.LogDebug($"Selected CipherServerToClient: {PendingExchangeContext.CipherServerToClient.Name}");
            Logger.LogDebug($"Selected MACAlgorithmClientToServer: {PendingExchangeContext.MACAlgorithmClientToServer.Name}");
            Logger.LogDebug($"Selected MACAlgorithmServerToClient: {PendingExchangeContext.MACAlgorithmServerToClient.Name}");
            Logger.LogDebug($"Selected CompressionClientToServer: {PendingExchangeContext.CompressionClientToServer.Name}");
            Logger.LogDebug($"Selected CompressionServerToClient: {PendingExchangeContext.CompressionServerToClient.Name}");
        }

        private void HandleSpecificPacket(KexDHInit packet)
        {
            Logger.LogDebug("Received KexDHInit");

            if ((PendingExchangeContext == null) || (PendingExchangeContext.KexAlgorithm == null))
            {
                throw new InvalidOperationException("Server did not receive SSH_MSG_KEX_INIT as expected.");
            }

            // 1. C generates a random number x (1 &lt x &lt q) and computes e = g ^ x mod p.  C sends e to S.
            // 2. S receives e.  It computes K = e^y mod p
            byte[] sharedSecret = PendingExchangeContext.KexAlgorithm.DecryptKeyExchange(packet.ClientValue);

            // 2. S generates a random number y (0 < y < q) and computes f = g ^ y mod p.
            byte[] serverKeyExchange = PendingExchangeContext.KexAlgorithm.CreateKeyExchange();

            byte[] hostKey = PendingExchangeContext.HostKeyAlgorithm.CreateKeyAndCertificatesData();

            // H = hash(V_C || V_S || I_C || I_S || K_S || e || f || K)
            byte[] exchangeHash = ComputeExchangeHash(
                PendingExchangeContext.KexAlgorithm,
                hostKey,
                packet.ClientValue,
                serverKeyExchange,
                sharedSecret);

            if (SessionID == null)
                SessionID = exchangeHash;

            // https://tools.ietf.org/html/rfc4253#section-7.2

            // Initial IV client to server: HASH(K || H || "A" || session_id)
            // (Here K is encoded as mpint and "A" as byte and session_id as raw
            // data.  "A" means the single character A, ASCII 65).
            byte[] clientCipherIV = ComputeEncryptionKey(
                PendingExchangeContext.KexAlgorithm,
                exchangeHash,
                PendingExchangeContext.CipherClientToServer.BlockSize,
                sharedSecret, 'A');

            // Initial IV server to client: HASH(K || H || "B" || session_id)
            byte[] serverCipherIV = ComputeEncryptionKey(
                PendingExchangeContext.KexAlgorithm,
                exchangeHash,
                PendingExchangeContext.CipherServerToClient.BlockSize,
                sharedSecret, 'B');

            // Encryption key client to server: HASH(K || H || "C" || session_id)
            byte[] clientCipherKey = ComputeEncryptionKey(
                PendingExchangeContext.KexAlgorithm,
                exchangeHash,
                PendingExchangeContext.CipherClientToServer.KeySize,
                sharedSecret, 'C');

            // Encryption key server to client: HASH(K || H || "D" || session_id)
            byte[] serverCipherKey = ComputeEncryptionKey(
                PendingExchangeContext.KexAlgorithm,
                exchangeHash,
                PendingExchangeContext.CipherServerToClient.KeySize,
                sharedSecret, 'D');

            // Integrity key client to server: HASH(K || H || "E" || session_id)
            byte[] clientHmacKey = ComputeEncryptionKey(
                PendingExchangeContext.KexAlgorithm,
                exchangeHash,
                PendingExchangeContext.MACAlgorithmClientToServer.KeySize,
                sharedSecret, 'E');

            // Integrity key server to client: HASH(K || H || "F" || session_id)
            byte[] serverHmacKey = ComputeEncryptionKey(
                PendingExchangeContext.KexAlgorithm,
                exchangeHash,
                PendingExchangeContext.MACAlgorithmServerToClient.KeySize,
                sharedSecret, 'F');

            // Set all keys we just generated
            PendingExchangeContext.CipherClientToServer.SetKey(clientCipherKey, clientCipherIV);
            PendingExchangeContext.CipherServerToClient.SetKey(serverCipherKey, serverCipherIV);
            PendingExchangeContext.MACAlgorithmClientToServer.SetKey(clientHmacKey);
            PendingExchangeContext.MACAlgorithmServerToClient.SetKey(serverHmacKey);

            // Send reply to client!
            KexDHReply reply = new KexDHReply()
            {
                ServerHostKey = hostKey,
                ServerValue = serverKeyExchange,
                Signature = PendingExchangeContext.HostKeyAlgorithm.CreateSignatureData(exchangeHash)
            };

            Send(reply);
            Send(new NewKeys());
        }

        private void HandleSpecificPacket(NewKeys packet)
        {
            Logger.LogDebug("Received NewKeys, activating ExchangeContext");

            ActiveExchangeContext = PendingExchangeContext;
            PendingExchangeContext = null;
        }

        private byte[] ComputeEncryptionKey(IKexAlgorithm kexAlgorithm, byte[] exchangeHash, uint keySize, byte[] sharedSecret, char letter)
        {
            // K(X) = HASH(K || H || X || session_id)

            // Prepare the buffer
            byte[] keyBuffer = new byte[keySize];
            int keyBufferIndex = 0;
            int currentHashLength = 0;
            byte[] currentHash = null;

            // We can stop once we fill the key buffer
            while (keyBufferIndex < keySize)
            {
                using (ByteWriter writer = new ByteWriter())
                {
                    // Write "K"
                    writer.WriteMPInt(sharedSecret);

                    // Write "H"
                    writer.WriteRawBytes(exchangeHash);

                    if (currentHash == null)
                    {
                        // If we haven't done this yet, add the "X" and session_id
                        writer.WriteByte((byte)letter);
                        writer.WriteRawBytes(SessionID);
                    }
                    else
                    {
                        // If the key isn't long enough after the first pass, we need to
                        // write the current hash as described here:
                        //      K1 = HASH(K || H || X || session_id)   (X is e.g., "A")
                        //      K2 = HASH(K || H || K1)
                        //      K3 = HASH(K || H || K1 || K2)
                        //      ...
                        //      key = K1 || K2 || K3 || ...
                        writer.WriteRawBytes(currentHash);
                    }

                    currentHash = kexAlgorithm.ComputeHash(writer.ToByteArray());
                }

                currentHashLength = Math.Min(currentHash.Length, (int)(keySize - keyBufferIndex));
                Array.Copy(currentHash, 0, keyBuffer, keyBufferIndex, currentHashLength);

                keyBufferIndex += currentHashLength;
            }

            return keyBuffer;
        }
        private byte[] ComputeExchangeHash(IKexAlgorithm kexAlgorithm, byte[] hostKeyAndCerts, byte[] clientExchangeValue, byte[] serverExchangeValue, byte[] sharedSecret)
        {
            // H = hash(V_C || V_S || I_C || I_S || K_S || e || f || K)
            using (ByteWriter writer = new ByteWriter())
            {
                writer.WriteString(protocolVersionExchange);
                writer.WriteString(Server.ProtocolVersionExchange);

                writer.WriteBytes(KexInitClientToServer.GetBytes());
                writer.WriteBytes(KexInitServerToClient.GetBytes());
                writer.WriteBytes(hostKeyAndCerts);

                writer.WriteMPInt(clientExchangeValue);
                writer.WriteMPInt(serverExchangeValue);
                writer.WriteMPInt(sharedSecret);

                return kexAlgorithm.ComputeHash(writer.ToByteArray());
            }
        }

        // Read 1 byte from the socket until we find "\r\n"
        private void ReadProtocolVersionExchange()
        {
            NetworkStream stream = new NetworkStream(Socket, false);
            string result = null;

            List<byte> data = new List<byte>();

            bool foundCR = false;
            int value = stream.ReadByte();
            while (value != -1)
            {
                if (foundCR && (value == '\n'))
                {
                    // DONE
                    result = Encoding.UTF8.GetString(data.ToArray());
                    hasCompletedProtocolVersionExchange = true;
                    break;
                }

                if (value == '\r')
                    foundCR = true;
                else
                {
                    foundCR = false;
                    data.Add((byte)value);
                }

                value = stream.ReadByte();
            }

            protocolVersionExchange += result;
        }
        public void Send(Packet packet)
        {
            Send(packet.ToByteArray());
        }
        private void Send(string message)
        {
            Logger.LogDebug($"Sending raw string: {message.Trim()}");
            Send(Encoding.UTF8.GetBytes(message));
        }

        private void Send(byte[] data)
        {
            if (!IsConnected)
                return;

            Socket.Send(data);
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
