using System;
using Compdog.TicTacToe.Utils;

namespace Compdog.TicTacToe.Bots.ExampleBot
{
    public class Bot : IBot
    {
        public string ShortDescription => "Example bot";
        public string Description => "A detailed description of what your bot can do.";
        
        public bool Load(Loader loader, Arguments args)
        {
            if (!loader.Subscribe(this, "move")) return false;
            return true;    // Return false to not load this bot
        }
        
        public bool Unload(Loader loader)
        {
            if(!loader.UnSubscribe(this, "move")) return false;
            return true;    // Return false to show warning
        }
        
        public bool Event(Loader loader, SubscribeEventArgs e)
        {
            switch(e.Event)
            {
                case "MOVE":
                    Console.WriteLine("Moved!");
                    return true;
            }
            
            return false;   // Return true to terminate
        }
        
        public bool Run(Game game, string[] args)
        {
            Console.WriteLine("This is an example.");
            return true;    // Return status (true - ok, false - failed)
        }
    }
}
