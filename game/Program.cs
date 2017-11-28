using System;
using asshcii.game.components;
using asshcii.game.resources;

namespace asshcii.game {
    class Program {
        static void Main(string[] args) {
            var neptuneResources = new AvailableResources(true, true);
            var neptune = new Planet("Neptune", neptuneResources);

            var refineryAscii = new Ascii(new char[,] { { 'X', ' ', '=' },
                                                        { 'X', ' ', 'X' }});

            //var resources = new Resources(50, 100);
            var refinery = new Building("Refinery", refineryAscii);
            refinery.AddComponent(new Produces<IronResource>(100));
            refinery.AddComponent(new Consumes<PowerResource>(100));
            refinery.AddComponent(new Produces<PowerResource>(100));

            var playerResources = new Resources(400, 200);
            var playerBase = new PlayerBase("TestBase", playerResources, neptune);

            Console.WriteLine(playerBase);
            Console.WriteLine();

            playerBase.TryBuild(refinery);

            Console.WriteLine(playerBase);
            Console.WriteLine();

            playerBase.TryBuild(refinery);

            Console.WriteLine(playerBase);
            Console.WriteLine();

            playerBase.TryBuild(refinery);

            Console.WriteLine(playerBase);
            Console.WriteLine();

            var kestrelAttack = new Attack(20);
            var kestrelHealth = new Health(200);
            var kestrel = new Ship("Kestrel", kestrelAttack, kestrelHealth);

            var vultureAttack = new Attack(15);
            var vultureHealth = new Health(250);
            var vulture = new Ship("Vulture", vultureAttack, vultureHealth);

            Console.WriteLine("Before Attack:\n");
            Console.WriteLine(kestrel);
            Console.WriteLine(vulture);

            kestrel.Attack(vulture);

            Console.WriteLine("After Attack:");
            Console.WriteLine(kestrel);
            Console.WriteLine(vulture);
        }
    }
}
