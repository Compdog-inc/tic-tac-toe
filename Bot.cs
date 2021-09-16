using System;

namespace Compdog.TicTacToe.Bots.ExampleBot
{
    public class Bot : IBot
    {
        public string ShortDescription => "Example bot";
        public string Description => "A detailed description of what your bot can do.";
        
        public bool Run(Game game, string[] args)
        {
            Console.WriteLine("This is an example.");
            return true;    // Return status (true - ok, false - failed)
        }
    }
}
