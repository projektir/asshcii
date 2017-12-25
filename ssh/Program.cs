using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace SSH
{
    class Program
    {
        private static bool running = true;
        public static void Main(string[] args)
        {
            if (args.Any(a => a == "--generate-keys"))
            {
                GenerateRsa("private.pem", "public.pem", 2048);
                Console.WriteLine("Generated `private.pem` and `public.pem`");
                return;
            }
            Console.CancelKeyPress += Console_CancelKeyPress;
            var settings = new Settings
            {
                ProtocolVersionExchangeName = "34c3",
                ProtocolVersionExchangeComment = "Hackers! Hackers everywhere"
            };
            if (File.Exists("private.pem"))
            {
                settings.SshRsaPrivateKeyFile = "private.pem";
            }
            else
            {
                Console.WriteLine("Warning: No private key found. SSH-RSA will be disabled");
                Console.WriteLine("To generate a key, run with --generate-keys");
            }

            var server = new Server<TestState>("Test server", settings);
            server.Start();

            while (running)
            {
                server.Poll();
                System.Threading.Thread.Sleep(25);
            }

            server.Stop();
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            running = false;
        }

        /// <summary>
        /// Generates 2 XML files (public and private key) 
        /// </summary> 
        /// <param name="privateKeyPath">RSA private key file path</param> 
        /// <param name="publicKeyPath">RSA private key file path</param> /
        // <param name="size">secure size must be above 512</param> 
        public static void GenerateRsa(string privateKeyPath, string publicKeyPath, int size)
        {
            //stream to save the keys
            FileStream fs = null;
            StreamWriter sw = null;

            //create RSA provider
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(size);
            try
            {
                //save private key
                fs = new FileStream(privateKeyPath, FileMode.Create, FileAccess.Write);
                sw = new StreamWriter(fs);
                sw.Write(ToXmlString(rsa, true));
                sw.Flush();
            }
            finally
            {
                if (sw != null) sw.Close();
                if (fs != null) fs.Close();
            }

            try
            {
                //save public key
                fs = new FileStream(publicKeyPath, FileMode.Create, FileAccess.Write);
                sw = new StreamWriter(fs);
                sw.Write(ToXmlString(rsa, false));
                sw.Flush();
            }
            finally
            {
                if (sw != null) sw.Close();
                if (fs != null) fs.Close();
            }
            rsa.Clear();
        }

        public static string ToXmlString(RSA rsa, bool includePrivateParameters)
        {
            RSAParameters parameters = rsa.ExportParameters(includePrivateParameters);

            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
                  parameters.Modulus != null ? Convert.ToBase64String(parameters.Modulus) : null,
                  parameters.Exponent != null ? Convert.ToBase64String(parameters.Exponent) : null,
                  parameters.P != null ? Convert.ToBase64String(parameters.P) : null,
                  parameters.Q != null ? Convert.ToBase64String(parameters.Q) : null,
                  parameters.DP != null ? Convert.ToBase64String(parameters.DP) : null,
                  parameters.DQ != null ? Convert.ToBase64String(parameters.DQ) : null,
                  parameters.InverseQ != null ? Convert.ToBase64String(parameters.InverseQ) : null,
                  parameters.D != null ? Convert.ToBase64String(parameters.D) : null);
        }
    }

    public class TestState : ClientState
    {

    }
}
