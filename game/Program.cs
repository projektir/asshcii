using System;
using asshcii.game.components;

namespace asshcii.game
{
    class Program
    {
        static void Main(string[] args)
        {
            var neptune = new Planet("Neptune");

            var refineryAscii = new Ascii(new char[,] { { 'X', ' ', '=' },
                                                        { 'X', ' ', 'X' }});

            var refinery = new Building("Refinery", refineryAscii);

            var kestrelAttack = new Attack(20);
            var kestrelHealth = new Health(200);
            var kestrel = new Ship("Kestrel", kestrelAttack, kestrelHealth);

            var vultureAttack = new Attack(15);
            var vultureHealth = new Health(250);
            var vulture = new Ship("Vulture", vultureAttack, vultureHealth);

            Console.WriteLine(refinery);
            Console.WriteLine();

            Console.WriteLine("Before Attack:");
            Console.WriteLine(kestrel);
            Console.WriteLine(vulture);
            Console.WriteLine();

            kestrel.Attack(vulture);

            Console.WriteLine("After Attack:");
            Console.WriteLine(kestrel);
            Console.WriteLine(vulture);
        }
    }
}
