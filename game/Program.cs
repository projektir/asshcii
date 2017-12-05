﻿using System;
using asshcii.game.buildings;
using asshcii.game.components;
using asshcii.game.resources;

namespace asshcii.game
{
    class Program
    {
        static void Main(string[] args)
        {
            var neptune = new Planet("Neptune");

            var refineryAscii = new Ascii(new char[,] { { 'X', ' ', '=' },
                                                        { 'X', ' ', 'X' }});

            // TODO: Make pre-defined buildings that automatically add all the components when they're constructed
            var ironMine = new IronMine();

            var playerBase = new PlayerBase("TestBase", neptune);
            playerBase.AddComponent(new Storage<IronResource>(1000));
            playerBase.AddComponent(new Storage<PowerResource>(1000));
            playerBase.Buildings.Add(ironMine);

            while (true)
            {
                Console.WriteLine(playerBase);
                Console.WriteLine();
                if (!playerBase.TryBuild(ironMine))
                {
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
