using System;
using asshcii.game.components;
using asshcii.game.resources;

namespace asshcii.game {
    public static class Program {
        public static void Main() {
            Planet neptune = new Planet("Neptune");

            Ascii refineryAscii = new Ascii(new[,] { { 'X', ' ', '=' },
                                                        { 'X', ' ', 'X' }});

            // TODO: Make pre-defined buildings that automatically add all the components when they're constructed
            Building refinery = new Building("Refinery", refineryAscii);
            refinery.AddComponent(new Produces<IronResource>(100));
            refinery.AddComponent(new Produces<PowerResource>(100));

            refinery.AddComponent(new UpgradeCost<IronResource>(100));
            refinery.AddComponent(new UpgradeCost<PowerResource>(100));
            
            refinery.AddComponent(new Consumes<PowerResource>(100));

            PlayerBase playerBase = new PlayerBase("TestBase", neptune);
            playerBase.AddComponent(new Storage<IronResource>(1000));
            playerBase.AddComponent(new Storage<PowerResource>(1000));
            playerBase.Buildings.Add(refinery);

            while(true){
                Console.WriteLine(playerBase);
                Console.WriteLine();
                if(!playerBase.TryBuild(refinery)){
                    break;
                }
            }

            Attack kestrelAttack = new Attack(20);
            Health kestrelHealth = new Health(200);
            Ship kestrel = new Ship("Kestrel", kestrelAttack, kestrelHealth);

            Attack vultureAttack = new Attack(15);
            Health vultureHealth = new Health(250);
            Ship vulture = new Ship("Vulture", vultureAttack, vultureHealth);

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
