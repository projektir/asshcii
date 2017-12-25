using SSH.Algorithms.Ciper;
using SSH.Algorithms.Compression;
using SSH.Algorithms.HostKey;
using SSH.Algorithms.Kex;
using SSH.Algorithms.MAC;

namespace SSH
{
    public class ExchangeContext
    {
        public IKexAlgorithm KexAlgorithm { get; set; } = null;
        public IHostKeyAlgorithm HostKeyAlgorithm { get; set; } = null;
        public ICipher CipherClientToServer { get; set; } = new NoCipher();
        public ICipher CipherServerToClient { get; set; } = new NoCipher();
        public IMACAlgorithm MACAlgorithmClientToServer { get; set; } = null;
        public IMACAlgorithm MACAlgorithmServerToClient { get; set; } = null;
        public ICompression CompressionClientToServer { get; set; } = new NoCompression();
        public ICompression CompressionServerToClient { get; set; } = new NoCompression();
    }
}
