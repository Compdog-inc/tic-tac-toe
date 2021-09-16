using Compdog.TicTacToe.Utils;

namespace Compdog.TicTacToe.Bots
{
    public interface IBot
    {
        string Description { get; }
        string ShortDescription { get; }

        bool Load(Loader loader, Arguments args);
        bool Unload(Loader loader);
        bool Event(Loader loader, SubscribedEventArgs e);

        bool Run(Game game, Loader loader, string[] args);
    }
}
