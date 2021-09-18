# Tic Tac Toe
A simple client with plugins

## Getting started (in Visual Studio 2019)

### Initial setup
1. Open Visual Studio and create a new project with `Class Library (.NET Framework)` template.
![Choose C# Library project template]  
make sure it uses `.NET Framework 4.7.2`
![Make sure you use .NET Framework 4.7.2]

2. Download the latest release of [engine.dll].
3. Right click on `References` and press `Add Reference...`  
![Right click on References and press Add Reference...]  
4. In the window that opens, press the `Browse` tab  
![Press the browse tab]  
5. Press `Browse` button  
![Press the browse button]  
6. Locate the downloaded `engine.dll` and press `Add`
7. You should see `engine.dll` in the list  
![engine.dll in the list]  
8. Click on `engine` in `References`  
![engine selected in references list]  
9. Set `Copy Local` to `False` in the properties  
![Copy Local set to False in properties]


### Running example
This example shows how you can use the Plugin API to make bots.  

1. Follow the steps in [Initial setup](#initial-setup)
2. Delete `Class1.cs` and replace it with [Bot.cs].  
![Bot.cs in project files]  
4. Build the project (`Ctrl+B`) and copy the compiled .dll file to `/plugins/` directory in tictactoe.
5. Run `plugin list` command. You should see `Compdog.TicTacToe.Bots.ExampleBot.Bot v1.0.0.0` in the list.
6. Run `plugin run examplebot`. You should see:
```
This is an example.
```
5. Now you can create your own plugin! Don't forget to look at [base library](#base-library)

## Tic Tac Toe client arguments
- `(/-)d` - Allows duplicate plugins
- `(/-)h` - Disables hot reloading of plugins
- `(/-)c` - If set, clears the console on hot reload
- `(/-)s [int]` - Sets the seed of `Compdog.TicTacToe.Game.Random`

## Tic Tac Toe client commands
All commands are in the `command arg0 arg1 arg2...` format

- `exit` - Exits client
- `args` - Displays the process arguments
- `cls` - Clears the screen
- `clear [cell?]` - Clears the board or cell
- `print [player?]` - Prints the current global (or player if specified) board
- `player [player?]` - Sets or gets the current player.
- `move [cell]` - Sets `cell` to current player
- `plugin`
  - `list` - Lists all currently loaded plugins
  - `info [query]` - Finds all plugins using `query` and shows detailed info
  - `event [event]` - Calls global event `event`
  - `run [query] \[args]` - Finds all plugins using `query` and runs with args `\arg`
  - `load` - Reloads plugins
  - `unload` - Unloads currently loaded plugins
 
 ### Argument formats
 `player` - A number representing the player index.  
 Example: `0`, `1`  
 
 `query` - Each segment separated by a `space` matches the name. If a segment starts with `v` and is a valid version (`v1.2.3.4`) then it matches to that version. (you can have multiple version queries).  
 Example: `mybot`, `My.Company.Bot`, `mybot v0.1.5.3`, `My.Company.Bot v1.0.2.1 v1.0.0.0`  
 
 `cell` - `[row][column]`. `row` can be `A`, `B` or `C`. `column` can be `1`, `2` or `3`.  
 Example: `A1`, `B2`, `C3`  
 
### Examples
Playing with a friend:
```
move a1
player 1
move b2
player 0
move a3
player 1
move a2
player 0
move b1
player 1
move c2
```

Playing with a bot:
```
move a1
plugin run mybot \move \1
move a3
plugin run mybot \move \1
move b1
plugin run mybot \move \1
```

Playing with a bot with events:
```
move a1
move a3
move b1
```

## Base library
- `Compdog.TicTacToe.Cell` - Enum for cell values
- `Compdog.TicTacToe.Arg`
  - `static EmptyArray` - Returns an empty `Arg` array
  - `static Empty` - Returns an empty `Arg` instance
  - `Name` - The argument's name
  - `IsNull` - Returns `true` if the raw value is `null`
  - `Raw` - Returns the raw value
  - `implicit operator T(Arg)` - Returns the raw value casted as `T` (valid options are the same as in constructors)
- `Compdog.TicTacToe.SubscribedEventArgs`
  - `Event` - Event name
  - `Args` - Event args
  - `bool HasArg(string name)` - Returns `true` if arg `name` exists
  - `bool GetArg(string name)` - Returns the arg with name `name`
- `Compdog.TicTacToe.BoardUpdateEventArgs`
  - `Board` - Changed board index
  - `Cell` - Changed cell index. `-1` if multiple changed (like when calling `Clear(board)`)
  - `OldValue` - Changed cell value before change
  - `NewValue` - Changed cell value after change
- `Compdog.TicTacToe.Game` - Main game logic
  - `static CELL_COUNT` - Number of cells in the game
  - `Random` - Random seeded with `-s` argument or random
  - `RandomSeed` - `Random` seed
  - `BoardCount` - Number of boards (players) in the game
  - `event BoardUpdate` - Called when a board changes
  - `UInt32 GetBoardRaw(int board)` - Returns raw board value
  - `void Clear(int board)` - Clears the whole board
  - `void Clear(int board, Cell(int) cell)` - Clears the cell
  - `bool Taken(int board, Cell(int) cell)` - Returns true if the cell is not empty
  - `void Move(int board, Cell(int) cell)` - Sets `cell` to `board`. If already taken throws an `InvalidOperationException`
  - `bool TryMove(int board, Cell(int) cell, out string errorMessage)` - Sets `cell` to `board`. If error, returns `false` and sets `errorMessage` to the error
  - `int GetWin(int board)` - Returns win move or `-1` if none
- `Compdog.TicTacToe.PCUE`
  - `Game` - The game
  - `EventManager` - The event manager
  - `Arguments` - The command line arguments
  - `bool LoadPlugins(bool allowDuplicates)` - Loads plugins
  - `bool UnloadPlugins()` - Unloads plugins
- `Compdog.TicTacToe.EventManager`
  - `bool Subscribe(IPlugin sender, string evt)` - Subscribes `sender` to event `evt`. Returns `false` if already subscribed
  - `bool UnSubscribe(IPlugin sender, string evt)` - Unsubscribes `sender` from event `evt`. Returns `false` if not subscribed
  - `bool Call(string evt, params Arg[] args)` - Calls event `evt` with args `args`. Returns `false` if event doesn't exist or if plugin terminated it
- `Compdog.TicTacToe.Plugins.IPlugin` - Interface for all plugins
  - `Description` - Long description of the plugin. Shown on `plugin info`. If null or empty `ShortDescription` is shown
  - `ShortDescription` - Short description of the plugin. Shown on `plugin list`
  - `bool Load(PCUE pcue)` - Called on plugin load. Return `false` if it shouldn't load it
  - `bool Unload()` - Called on plugin unload
  - `bool Event(SubscribedEventArgs e)` - Called on global event
  - `bool Run(string[] args)` - Main function. `args` contains all args that start with `\` passed in with `plugin run`
- `Compdog.TicTacToe.Utils.Arguments` - Command line arguments
  - `string GetFlag(string name)` - Returns the flag or `string.Empty` if none
  - `bool HasFlag(string name)` - Returns `true` if flag exists
  - `bool HasWord(string word, bool ignoreCase = true)` - Returns `true` if word exists
  - `string[] GetWords()` - Returns all words
- `Compdog.TicTacToe.Utils.CommandLine` - Command line parser
  - `static Arguments ParseArguments(string[] args, params char[] flagIndicators)` - Parses command line arguments


[engine.dll]: https://github.com/Compdog-inc/tic-tac-toe/releases/latest
[Bot.cs]: https://github.com/Compdog-inc/tic-tac-toe/blob/main/examples/Bot.cs

[Choose C# Library project template]: https://user-images.githubusercontent.com/66779418/133610967-cb1d3ef1-1a9c-454e-adaa-36c048ce5bd7.png
[Make sure you use .NET Framework 4.7.2]: https://user-images.githubusercontent.com/66779418/133611204-95efa064-58c3-4416-bdc9-67ba96177455.png
[Right click on References and press Add Reference...]: https://user-images.githubusercontent.com/66779418/133611693-49886db7-dc62-402d-a166-8257288e55c6.png
[Press the browse tab]: https://user-images.githubusercontent.com/66779418/133612454-1ce752a6-4c25-4068-bce1-888c93d1c94a.png
[Press the browse button]: https://user-images.githubusercontent.com/66779418/133612364-8694d81b-3600-454f-a840-5bb8524546d0.png
[engine.dll in the list]: https://user-images.githubusercontent.com/66779418/133612588-f82c34cc-5cd4-4930-aead-b7d6e6c2e8ee.png
[engine selected in references list]: https://user-images.githubusercontent.com/66779418/133612620-5a2004d2-a59d-4427-ae86-36cc41ca982a.png
[Copy Local set to False in properties]: https://user-images.githubusercontent.com/66779418/133612646-75afe3ba-4b4f-4412-bfb0-dd04dde1e3ac.png
[Bot.cs in project files]: https://user-images.githubusercontent.com/66779418/133613953-60929397-739c-4091-b65e-eeeddf610a65.png
