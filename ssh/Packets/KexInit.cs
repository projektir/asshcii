using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using SSH.Algorithms;
using SSH.Algorithms.Ciper;
using SSH.Algorithms.Compression;
using SSH.Algorithms.HostKey;
using SSH.Algorithms.Kex;
using SSH.Algorithms.MAC;

namespace SSH.Packets
{
    public class KexInit : Packet
    {
        public override PacketType PacketType => PacketType.SSH_MSG_KEXINIT;
        public byte[] Cookie { get; set; } = new byte[16];
        public List<string> KexAlgorithms { get; private set; } = new List<string>();
        public List<string> ServerHostKeyAlgorithms { get; private set; } = new List<string>();
        public List<string> EncryptionAlgorithmsClientToServer { get; private set; } = new List<string>();
        public List<string> EncryptionAlgorithmsServerToClient { get; private set; } = new List<string>();
        public List<string> MacAlgorithmsClientToServer { get; private set; } = new List<string>();
        public List<string> MacAlgorithmsServerToClient { get; private set; } = new List<string>();
        public List<string> CompressionAlgorithmsClientToServer { get; private set; } = new List<string>();
        public List<string> CompressionAlgorithmsServerToClient { get; private set; } = new List<string>();
        public List<string> LanguagesClientToServer { get; private set; } = new List<string>();
        public List<string> LanguagesServerToClient { get; private set; } = new List<string>();

        internal static KexInit FromAlgorithmProvider(AlgorithmProvider algorithmProvider)
        {
            KexInit kexInit = new KexInit();
            kexInit.KexAlgorithms.AddRange(
                algorithmProvider.GetNames(
                    algorithmProvider.SupportedKexAlgorithms
                )
            );
            kexInit.ServerHostKeyAlgorithms.AddRange(
                algorithmProvider.GetNames(
                    algorithmProvider.SupportedHostKeyAlgorithms
                )
            );

            kexInit.EncryptionAlgorithmsClientToServer.AddRange(
                algorithmProvider.GetNames(algorithmProvider.SupportedCiphers)
            );
            kexInit.EncryptionAlgorithmsServerToClient.AddRange(
                algorithmProvider.GetNames(algorithmProvider.SupportedCiphers)
            );

            kexInit.MacAlgorithmsClientToServer.AddRange(
                algorithmProvider.GetNames(algorithmProvider.SupportedMACAlgorithms)
            );
            kexInit.MacAlgorithmsServerToClient.AddRange(
                algorithmProvider.GetNames(algorithmProvider.SupportedMACAlgorithms)
            );
            kexInit.CompressionAlgorithmsClientToServer.AddRange(
                algorithmProvider.GetNames(algorithmProvider.SupportedCompressions)
            );
            kexInit.CompressionAlgorithmsServerToClient.AddRange(
                algorithmProvider.GetNames(algorithmProvider.SupportedCompressions)
            );
            return kexInit;
        }

        public bool FirstKexPacketFollows { get; set; }

        public KexInit()
        {
            RandomNumberGenerator.Create().GetBytes(Cookie);
        }

        protected override void InternalGetBytes(ByteWriter writer)
        {
            writer.WriteRawBytes(Cookie);
            writer.WriteStringList(KexAlgorithms);
            writer.WriteStringList(ServerHostKeyAlgorithms);
            writer.WriteStringList(EncryptionAlgorithmsClientToServer);
            writer.WriteStringList(EncryptionAlgorithmsServerToClient);
            writer.WriteStringList(MacAlgorithmsClientToServer);
            writer.WriteStringList(MacAlgorithmsServerToClient);
            writer.WriteStringList(CompressionAlgorithmsClientToServer);
            writer.WriteStringList(CompressionAlgorithmsServerToClient);
            writer.WriteStringList(LanguagesClientToServer);
            writer.WriteStringList(LanguagesServerToClient);
            writer.WriteByte(FirstKexPacketFollows ? (byte)0x01 : (byte)0x00);
            writer.WriteUInt32(0);
        }

        protected override void Load(ByteReader reader)
        {
            Cookie = reader.GetBytes(16);
            KexAlgorithms = reader.GetNameList();
            ServerHostKeyAlgorithms = reader.GetNameList();
            EncryptionAlgorithmsClientToServer = reader.GetNameList();
            EncryptionAlgorithmsServerToClient = reader.GetNameList();
            MacAlgorithmsClientToServer = reader.GetNameList();
            MacAlgorithmsServerToClient = reader.GetNameList();
            CompressionAlgorithmsClientToServer = reader.GetNameList();
            CompressionAlgorithmsServerToClient = reader.GetNameList();
            LanguagesClientToServer = reader.GetNameList();
            LanguagesServerToClient = reader.GetNameList();
            FirstKexPacketFollows = reader.GetBoolean();
            /*
              uint32       0 (reserved for future extension)
            */
            uint reserved = reader.GetUInt32();
        }

        public IKexAlgorithm PickKexAlgorithm(AlgorithmProvider provider)
        {
            foreach (string algo in this.KexAlgorithms)
            {
                IKexAlgorithm selectedAlgo = provider.GetType<IKexAlgorithm>(provider.SupportedKexAlgorithms, algo);
                if (selectedAlgo != null)
                {
                    return selectedAlgo;
                }
            }

            // If no algorithm satisfying all these conditions can be found, the
            // connection fails, and both sides MUST disconnect.
            throw new NotSupportedException("Could not find a shared Kex Algorithm");
        }

        public IHostKeyAlgorithm PickHostKeyAlgorithm(AlgorithmProvider provider)
        {
            foreach (string algo in this.ServerHostKeyAlgorithms)
            {
                IHostKeyAlgorithm selectedAlgo = provider.GetType<IHostKeyAlgorithm>(provider.SupportedHostKeyAlgorithms, algo);
                if (selectedAlgo != null)
                {
                    return selectedAlgo;
                }
            }

            // If no algorithm satisfying all these conditions can be found, the
            // connection fails, and both sides MUST disconnect.
            throw new NotSupportedException("Could not find a shared Host Key Algorithm");
        }

        public ICipher PickCipherClientToServer(AlgorithmProvider provider)
        {
            foreach (string algo in this.EncryptionAlgorithmsClientToServer)
            {
                ICipher selectedCipher = provider.GetType<ICipher>(provider.SupportedCiphers, algo);
                if (selectedCipher != null)
                {
                    return selectedCipher;
                }
            }

            // If no algorithm satisfying all these conditions can be found, the
            // connection fails, and both sides MUST disconnect.
            throw new NotSupportedException("Could not find a shared Client-To-Server Cipher Algorithm");
        }

        public ICipher PickCipherServerToClient(AlgorithmProvider provider)
        {
            foreach (string algo in this.EncryptionAlgorithmsServerToClient)
            {
                ICipher selectedCipher = provider.GetType<ICipher>(provider.SupportedCiphers, algo);
                if (selectedCipher != null)
                {
                    return selectedCipher;
                }
            }

            // If no algorithm satisfying all these conditions can be found, the
            // connection fails, and both sides MUST disconnect.
            throw new NotSupportedException("Could not find a shared Server-To-Client Cipher Algorithm");
        }

        public IMACAlgorithm PickMACAlgorithmClientToServer(AlgorithmProvider provider)
        {
            foreach (string algo in this.MacAlgorithmsClientToServer)
            {
                IMACAlgorithm selectedAlgo = provider.GetType<IMACAlgorithm>(provider.SupportedMACAlgorithms, algo);
                if (selectedAlgo != null)
                {
                    return selectedAlgo;
                }
            }

            // If no algorithm satisfying all these conditions can be found, the
            // connection fails, and both sides MUST disconnect.
            throw new NotSupportedException("Could not find a shared Client-To-Server MAC Algorithm");
        }

        public IMACAlgorithm PickMACAlgorithmServerToClient(AlgorithmProvider provider)
        {
            foreach (string algo in this.MacAlgorithmsServerToClient)
            {
                IMACAlgorithm selectedAlgo = provider.GetType<IMACAlgorithm>(provider.SupportedMACAlgorithms, algo);
                if (selectedAlgo != null)
                {
                    return selectedAlgo;
                }
            }

            // If no algorithm satisfying all these conditions can be found, the
            // connection fails, and both sides MUST disconnect.
            throw new NotSupportedException("Could not find a shared Server-To-Client MAC Algorithm");
        }

        public ICompression PickCompressionAlgorithmClientToServer(AlgorithmProvider provider)
        {
            foreach (string algo in this.CompressionAlgorithmsClientToServer)
            {
                ICompression selectedAlgo = provider.GetType<ICompression>(provider.SupportedCompressions, algo);
                if (selectedAlgo != null)
                {
                    return selectedAlgo;
                }
            }

            // If no algorithm satisfying all these conditions can be found, the
            // connection fails, and both sides MUST disconnect.
            throw new NotSupportedException("Could not find a shared Client-To-Server Compresion Algorithm");
        }

        public ICompression PickCompressionAlgorithmServerToClient(AlgorithmProvider provider)
        {
            foreach (string algo in this.CompressionAlgorithmsServerToClient)
            {
                ICompression selectedAlgo = provider.GetType<ICompression>(provider.SupportedCompressions, algo);
                if (selectedAlgo != null)
                {
                    return selectedAlgo;
                }
            }

            // If no algorithm satisfying all these conditions can be found, the
            // connection fails, and both sides MUST disconnect.
            throw new NotSupportedException("Could not find a shared Server-To-Client Compresion Algorithm");
        }
    }
}