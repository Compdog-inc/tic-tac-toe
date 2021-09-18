using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compdog.TicTacToe.Plugins;
using Compdog.TicTacToe.Utils;

namespace Compdog.TicTacToe
{
    public class PCUE
    {
        public Game Game { get; }
        public EventManager EventManager { get; }
        public Arguments Arguments { get; }

        private Internal.PCUE.PluginManager pluginManager;

        public PCUE(Game _game, EventManager _eventManager, Arguments _args)
        {
            Game = _game;
            EventManager = _eventManager;
            Arguments = _args;

            pluginManager = new Internal.PCUE.PluginManager(this);
        }

        public bool LoadPlugins(bool allowDuplicates)
        {
            return pluginManager.Load(allowDuplicates);
        }

        public bool UnloadPlugins()
        {
            return pluginManager.Unload();
        }

        public bool TrySearchPlugins(string query, out IPlugin[] plugins)
        {
            return pluginManager.TrySearch(query, out plugins);
        }

        public IPlugin[] SearchPlugins(string query)
        {
            return pluginManager.Search(query);
        }

        public IPlugin[] ListPlugins()
        {
            return pluginManager.List();
        }
    }
}
