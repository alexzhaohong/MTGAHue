﻿using MTGADispatcher.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace MTGADispatcher.Integration.Features
{
    public class TestFixture : IDisposable
    {
        private MtgaService service;

        private string path;

        private StreamWriter streamWriter;

        public List<CastSpell> SpellsCast = new List<CastSpell>();

        public Game Game;

        public Game CreateGame()
        {
            path = Path.GetTempFileName();
            streamWriter = new StreamWriter(path);

            Game = new Game();

            service = new MtgaService(
                new BlockDispatcher(
                    new BlockBuilder(),
                    new Dispatcher<Block>(),
                    () => new FileLineReader(path)),
                new BlockProcessor(
                    Game,
                    new IGameUpdater[]
                    {
                        new ServerToClientBlockHandler(new InstanceBuilder()),
                        new GameEndedBlockHandler()
                    }));

            service.Start();

            Game.Events.Subscriptions.Subscribe<CastSpell>(OnCastSpell);

            return Game;
        }

        public void WriteContents(string resourceName)
        {
            var assembly = Assembly.GetAssembly(GetType());
            var assemblyName = assembly.GetName().Name;

            using (var stream = 
                assembly.GetManifestResourceStream($"{assemblyName}.Games.{resourceName}"))
            {
                using (var reader = new StreamReader(stream))
                {
                    var contents = reader.ReadToEnd();
                    streamWriter.Write(contents);
                    streamWriter.Flush();
                }
            }
        }

        public void WaitForGameEnd()
        {
            var manualReset = new ManualResetEvent(false);

            var action = new Action<GameEnded>(g => { manualReset.Set(); });

            Game.Events.Subscriptions.Subscribe(action);
            streamWriter.WriteLine(@"[Client GRE]5/4/2019 1:50:05 PM: Match to X: GreToClientEvent
{
'Game': 'Over'
}");

            streamWriter.Flush();

            var succeeded = manualReset.WaitOne(TimeSpan.FromSeconds(3));
            Game.Events.Subscriptions.Unsubscribe(action);

            if (!succeeded)
            {
                throw new InvalidOperationException("Game didn't end after waiting");
            }
        }

        private void OnCastSpell(CastSpell spell)
        {
            SpellsCast.Add(spell);
        }

        public void Dispose()
        {
            if (Game == null)
            {
                return;
            }

            Game.Events.Subscriptions.Unsubscribe<CastSpell>(OnCastSpell);
            service?.Dispose();
            streamWriter?.Dispose();

            if (path != null && File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
