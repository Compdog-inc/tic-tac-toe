using Compdog.TicTacToe.Utils;

namespace Compdog.TicTacToe.Plugins
{
    public interface IPlugin
    {
        string Description { get; }
        string ShortDescription { get; }

        bool Load(PCUE pcue);
        bool Unload();
        bool Event(SubscribedEventArgs e);

        bool Run(string[] args);
    }
}
