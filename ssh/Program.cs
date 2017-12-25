using System;

namespace ssh
{
    class Program
    {
        private static bool running = true;
        public static void Main(string[] args)
        {
            Console.CancelKeyPress += Console_CancelKeyPress;

            var server = new SshServer<TestState>("Test server");
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
    }

    public class TestState : ClientState {

    }
}
