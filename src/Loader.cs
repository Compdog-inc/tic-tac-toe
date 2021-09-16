using Compdog.TicTacToe.Bots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compdog.TicTacToe
{
    public class SubscribedEventArgs : EventArgs
    {
        public string Event { get; }
        public Game Game { get; }

        public SubscribedEventArgs(string _event, Game _game)
        {
            Event = _event;
            Game = _game;
        }
    }

    public class Loader
    {
        private Dictionary<string, List<IBot>> subscribedEvents = new Dictionary<string, List<IBot>>();

        public bool Subscribe(IBot sender, string evt)
        {
            if (!subscribedEvents.ContainsKey(evt.ToUpperInvariant())) subscribedEvents.Add(evt.ToUpperInvariant(), new List<IBot>());
            if (subscribedEvents[evt.ToUpperInvariant()].Contains(sender)) return false;
            subscribedEvents[evt.ToUpperInvariant()].Add(sender);
            return true;
        }

        public bool UnSubscribe(IBot sender, string evt)
        {
            if (!subscribedEvents.ContainsKey(evt.ToUpperInvariant())) return false;
            if (subscribedEvents[evt.ToUpperInvariant()].Contains(sender))
            {
                subscribedEvents[evt.ToUpperInvariant()].Remove(sender);
                return true;
            }
            return false;
        }

        public bool Call(string evt, Game game)
        {
            if (!subscribedEvents.ContainsKey(evt.ToUpperInvariant())) return false;
            foreach (IBot bot in subscribedEvents[evt.ToUpperInvariant()])
            {
                if (bot.Event(this, new SubscribedEventArgs(evt.ToUpperInvariant(), game))) return false;
            }
            return true;
        }
    }
}
