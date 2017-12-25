namespace SSH.Algorithms.Compression
{
    public interface ICompression : IAlgorithm
    {
        byte[] Compress(byte[] data);
        byte[] Decompress(byte[] data);
    }
}