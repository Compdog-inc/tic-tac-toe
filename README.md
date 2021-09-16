# Tic Tac Toe
Examples and docs for tictatoe

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
1. Follow the steps in [Initial setup](#initial-setup)
2. Delete `Class1.cs` and replace it with [Bot.cs].  
![Bot.cs in project files]  
4. Build the project (`Ctrl+B`) and copy the compiled .dll file to `/bots/` directory in tictactoe.
5. Run `bot list` command. You should see `Compdog.TicTacToe.Bots.ExampleBot.Bot v1.0.0.0` in the list.
6. Run `bot run examplebot`. You should see:
```
Starting assemblyname...
This is an example.
assemblyname ended with result True.
```
(with `assemblyname` replaced with your project name)  
5. Now you can create your own bot! Don't forget to look at [base library](#base-library)

## Tic Tac Toe client arguments
- `(/-)d` - Allows duplicate bots
- `(/-)h` - Disables hot reloading of bots
- `(/-)c` - If set, clears the console on hot reload
- `(/-)s [int]` - Sets the seed of `Compdog.TicTacToe.Game.Random`

## Tic Tac Toe client commands
All commands are in the `command arg0 arg1 arg2...` format

- `exit` - Exits client
- `args` - Displays the process arguments
- `cls` - Clears the screen
- `clear [cell?]` - Clears the board. Optionally clears a cell
- `print [player?]` - Prints the current global (or player if specified) board
- `player [player?]` - Sets or gets the current player.
- `move [cell]` - Sets `cell` to current player
- `bot`
  - `list` - Lists all currently loaded bots
  - `info [query]` - Finds all bots using `query` and shows detailed info
  - `event [event]` - Calls global event `event`
  - `run [query] \[args]` - Finds all bots using `query` and runs with args `\arg`
  - `load` - Reloads bots
  - `unload` - Unloads currently loaded bots
 
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
bot run mybot \move \1
move a3
bot run mybot \move \1
move b1
bot run mybot \move \1
```

## Base library
- `Compdog.TicTacToe.Cell` - Enum for cell values
- `Compdog.TicTacToe.SubscribedEventArgs`
  - `Event` - Event name
  - `Game` - Current `Game` instance
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
- `Compdog.TicTacToe.Loader` - Bot controller
  - `bool Subscribe(IBot sender, string evt)` - Subscribes `sender` to event `evt`. Returns `false` if already subscribed
  - `bool UnSubscribe(IBot sender, string evt)` - Unsubscribes `sender` from event `evt`. Returns `false` if not subscribed
  - `bool Call(string evt, Game game)` - Calls event `evt`. Returns `false` if event doesn't exist or if bot terminated it
- `Compdog.TicTacToe.Bots.IBot` - Interface for all bots
  - `Description` - Long description of the bot. Shown on `bot info`. If null or empty `ShortDescription` is shown
  - `ShortDescription` - Short description of the bot. Shown on `bot list`
  - `bool Load(Loader loader, Arguments args)` - Called on bot load. Return `false` if it shouldn't load it
  - `bool Unload(Loader loader)` - Called on bot unload
  - `bool Event(Loader loader, SubscribedEventArgs e)` - Called on global event
  - `bool Run(Game game, Loader loader, string[] args)` - Main code of the bot. `args` contains all args that start with `\` passed in with `bot run`


[engine.dll]: https://github.com/Compdog-inc/tic-tac-toe/releases/latest
[Bot.cs]: https://github.com/Compdog-inc/tic-tac-toe/blob/main/Bot.cs

[Choose C# Library project template]: https://user-images.githubusercontent.com/66779418/133610967-cb1d3ef1-1a9c-454e-adaa-36c048ce5bd7.png
[Make sure you use .NET Framework 4.7.2]: https://user-images.githubusercontent.com/66779418/133611204-95efa064-58c3-4416-bdc9-67ba96177455.png
[Right click on References and press Add Reference...]: https://user-images.githubusercontent.com/66779418/133611693-49886db7-dc62-402d-a166-8257288e55c6.png
[Press the browse tab]: https://user-images.githubusercontent.com/66779418/133612454-1ce752a6-4c25-4068-bce1-888c93d1c94a.png
[Press the browse button]: https://user-images.githubusercontent.com/66779418/133612364-8694d81b-3600-454f-a840-5bb8524546d0.png
[engine.dll in the list]: https://user-images.githubusercontent.com/66779418/133612588-f82c34cc-5cd4-4930-aead-b7d6e6c2e8ee.png
[engine selected in references list]: https://user-images.githubusercontent.com/66779418/133612620-5a2004d2-a59d-4427-ae86-36cc41ca982a.png
[Copy Local set to False in properties]: https://user-images.githubusercontent.com/66779418/133612646-75afe3ba-4b4f-4412-bfb0-dd04dde1e3ac.png
[Bot.cs in project files]: https://user-images.githubusercontent.com/66779418/133613953-60929397-739c-4091-b65e-eeeddf610a65.png
