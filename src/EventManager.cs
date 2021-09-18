using Compdog.TicTacToe.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compdog.TicTacToe
{
    public readonly struct Arg
    {
        public static Arg[] EmptyArray => new Arg[0];
        public static Arg Empty => new Arg();

        private readonly string _name;
        private readonly object _raw;

        public Arg(string name)
        {
            _name = name;
            _raw = null;
        }

        public Arg(string name, object raw)
        {
            _name = name;
            _raw = raw;
        }

        public Arg(string name, string str)
        {
            _name = name;
			_raw = str;
        }

        public Arg(string name, bool b)
        {
            _name = name;
			_raw = b;
        }

        public Arg(string name, byte b)
        {
            _name = name;
			_raw = b;
        }

        public Arg(string name, short s)
        {
            _name = name;
			_raw = s;
        }

        public Arg(string name, ushort us)
        {
            _name = name;
			_raw = us;
        }

        public Arg(string name, int i)
        {
            _name = name;
			_raw = i;
        }

        public Arg(string name, uint ui)
        {
            _name = name;
			_raw = ui;
        }

        public Arg(string name, long l)
        {
            _name = name;
			_raw = l;
        }

        public Arg(string name, ulong ul)
        {
            _name = name;
			_raw = ul;
        }

        public string Name => _name;
        public bool IsNull => _raw == null;
        public object Raw => _raw;
        
        public static implicit operator string(Arg a) => (string)a.Raw;
        public static implicit operator bool(Arg a) => (bool)a.Raw;
        public static implicit operator byte(Arg a) => (byte)a.Raw;
        public static implicit operator short(Arg a) => (short)a.Raw;
        public static implicit operator ushort(Arg a) => (ushort)a.Raw;
        public static implicit operator int(Arg a) => (int)a.Raw;
        public static implicit operator uint(Arg a) => (uint)a.Raw;
        public static implicit operator long(Arg a) => (long)a.Raw;
        public static implicit operator ulong(Arg a) => (ulong)a.Raw;
    }

    public class SubscribedEventArgs : EventArgs
    {
        public string Event { get; }
        public Arg[] Args { get; }

        public SubscribedEventArgs(string _event, params Arg[] _args)
        {
            Event = _event;
            Args = _args;
        }

        public bool HasArg(string name)
        {
            return Args.Any(a => !a.IsNull && a.Name.Equals(name));
        }

        public bool HasArg(string name, StringComparison comparisonType)
        {
            return Args.Any(a => !a.IsNull && a.Name.Equals(name, comparisonType));
        }

        public Arg GetArg(string name)
        {
            return Args.FirstOrDefault(a => !a.IsNull && a.Name.Equals(name));
        }

        public Arg GetArg(string name, StringComparison comparisonType)
        {
            return Args.FirstOrDefault(a => !a.IsNull && a.Name.Equals(name, comparisonType));
        }
    }

    public class EventManager
    {
        private Dictionary<string, List<IPlugin>> subscribedEvents = new Dictionary<string, List<IPlugin>>();

        public bool Subscribe(IPlugin sender, string evt)
        {
            if (!subscribedEvents.ContainsKey(evt.ToUpperInvariant())) subscribedEvents.Add(evt.ToUpperInvariant(), new List<IPlugin>());
            if (subscribedEvents[evt.ToUpperInvariant()].Contains(sender)) return false;
            subscribedEvents[evt.ToUpperInvariant()].Add(sender);
            return true;
        }

        public bool UnSubscribe(IPlugin sender, string evt)
        {
            if (!subscribedEvents.ContainsKey(evt.ToUpperInvariant())) return false;
            if (subscribedEvents[evt.ToUpperInvariant()].Contains(sender))
            {
                subscribedEvents[evt.ToUpperInvariant()].Remove(sender);
                return true;
            }
            return false;
        }

        public bool Call(string evt, params Arg[] args)
        {
            if (!subscribedEvents.ContainsKey(evt.ToUpperInvariant())) return false;
            foreach (IPlugin plugin in subscribedEvents[evt.ToUpperInvariant()])
            {
                if (plugin.Event(new SubscribedEventArgs(evt.ToUpperInvariant(), args))) return false;
            }
            return true;
        }
    }
}
