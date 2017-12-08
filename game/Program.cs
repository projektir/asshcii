using System;
using asshcii.game.actions;
using asshcii.game.buildings;
using asshcii.game.components;
using asshcii.game.resources;

namespace asshcii.game
{
    public static class Program
    {
        public static void Main()
        {
            var neptune = new Planet("Neptune");

            var ironMine = new IronMine();

            var playerBase = new PlayerBase("TestBase", neptune);
            playerBase.AddComponent(new Storage<IronResource>(1000));
            playerBase.AddComponent(new Storage<PowerResource>(1000));
            playerBase.Buildings.Add(ironMine);

            for(int i = 0; i < 10; i++)
            {
                Console.WriteLine(playerBase);
                Console.WriteLine();
                var result = new UpgradeBuildingAction(ironMine).Execute(playerBase);
                if (!result.Success)
                {
                    Console.WriteLine("Could not upgrade");
                    Console.WriteLine(result.Message);
                    break;
                }
            }

            var kestrelAttack = new Attack(20);
            var kestrelHealth = new Health(200);
            var kestrel = new Ship("Kestrel", kestrelAttack, kestrelHealth);

            var vultureAttack = new Attack(15);
            var vultureHealth = new Health(250);
            var vulture = new Ship("Vulture", vultureAttack, vultureHealth);

            Console.WriteLine("Before Attack:");
            Console.WriteLine(kestrel);
            Console.WriteLine(vulture);

            kestrel.Attack(vulture);

            Console.WriteLine();
            Console.WriteLine("After Attack:");
            Console.WriteLine(kestrel);
            Console.WriteLine(vulture);
        }
    }
}
