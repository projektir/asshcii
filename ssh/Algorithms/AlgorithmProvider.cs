using System;
using System.Collections.Generic;
using SSH.Algorithms.Ciper;
using SSH.Algorithms.Compression;
using SSH.Algorithms.HostKey;
using SSH.Algorithms.Kex;
using SSH.Algorithms.MAC;

namespace SSH.Algorithms
{
    public class AlgorithmProvider
    {
        public IReadOnlyList<Type> SupportedKexAlgorithms { get; } = new List<Type>()
        {
            typeof(DiffieHellmanGroup14SHA1)
        };
        public Dictionary<string, string> HostKeys = new Dictionary<string, string>();
        public IReadOnlyList<Type> SupportedHostKeyAlgorithms { get; } = new List<Type>()
        {
            typeof(SSHRSA)
        };
        public IEnumerable<string> GetNames(IReadOnlyList<Type> types)
        {
            foreach (Type type in types)
            {
                IAlgorithm algo = Activator.CreateInstance(type) as IAlgorithm;
                yield return algo.Name;
            }
        }
        public T GetType<T>(IReadOnlyList<Type> types, string selected) where T : class
        {
            foreach (Type type in types)
            {
                IAlgorithm algo = Activator.CreateInstance(type) as IAlgorithm;
                if (algo.Name.Equals(selected, StringComparison.OrdinalIgnoreCase))
                {
                    if (algo is IHostKeyAlgorithm)
                    {
                        ((IHostKeyAlgorithm)algo).ImportKey(HostKeys[algo.Name]);
                    }

                    return algo as T;
                }
            }

            return default(T);
        }

        public IReadOnlyList<Type> SupportedCiphers { get; private set; } = new List<Type>()
        {
            typeof(TripleDESCBC)
        };

        public IReadOnlyList<Type> SupportedMACAlgorithms { get; private set; } = new List<Type>()
        {
            typeof(HMACSHA1)
        };

        public IReadOnlyList<Type> SupportedCompressions { get; private set; } = new List<Type>()
        {
            typeof(NoCompression)
        };
    }
}