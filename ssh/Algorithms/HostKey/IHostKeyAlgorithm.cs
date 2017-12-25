namespace SSH.Algorithms.HostKey
{
    public interface IHostKeyAlgorithm : IAlgorithm 
    {
        void ImportKey(string keyXml);
        byte[] CreateKeyAndCertificatesData();
        byte[] CreateSignatureData(byte[] hash);
    }
}