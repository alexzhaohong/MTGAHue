﻿using MTGADispatcher;
using MTGADispatcher.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagicLights
{
    public class Demo : IMagicService
    {
        private static Dictionary<char, MagicColor> colorMap = new Dictionary<char, MagicColor>
        {
            ['R'] = MagicColor.Red,
            ['G'] = MagicColor.Green,
            ['U'] = MagicColor.Blue,
            ['B'] = MagicColor.Black,
            ['W'] = MagicColor.White
        };

        private readonly Game game;

        private int instanceId = 0;

        public Demo(Game game)
        {
            this.game = game;
        }

        public void Start()
        {
            Console.WriteLine("Q quits. Multiple colors ok (Eg: RG)");
            while (true)
            {
                Console.WriteLine("Select color [R]ed/[G]reen/bl[U]e/[B]lack/[W]hite]:");
                var input = Console.ReadLine()?.ToUpperInvariant();
                if (input == null || input == "Q")
                {
                    return;
                }

                var colors = ParseColors(input).ToArray();
                instanceId++;

                var instance = new Instance(instanceId, instanceId, 1, colors);
                game.Events.Dispatch(new CastSpell(instance));
            }
        }

        public Task Stop()
        {
            return Task.CompletedTask;
        }

        private IEnumerable<MagicColor> ParseColors(string input)
        {
            foreach (var c in input)
            {
                if (colorMap.TryGetValue(c, out var color))
                {
                    yield return color;
                }
            }
        }
    }
}
