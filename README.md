# Tic Tac Toe
Examples and docs for bot api

## Getting started (in Visual Studio 2019)

### Initial setup
1. Open Visual Studio and create a new project with `Class Library (.NET Framework)` template. (make sure it uses .NET Framework 4.7.2).
2. Download latest release of engine.dll and add it to project references.

### Running example
1. Delete `Class1.cs` and replace it with Bot.cs.
2. Build the project and copy the compiled .dll file to `/bots/` directory in tictactoe.
3. Run `bot list` command. You should see `[examplebot]: Compdog.TicTacToe.Bots.ExampleBot.Bot v1.0.0.0` in the list.
4. Run `bot run examplebot`. You should see:
```
Starting examplebot...
This is an example.
randombot ended with result True.
```
5. Now you can create your own bot!

## Tic Tac Toe client commands
All commands are in the `command arg0 arg1 arg2...` format

- `exit` - Exits client
- `cls` - Clears the screen
- `clear [cell?]` - Clears the board. Optionally clears a cell.
- `print [player?]` - Prints the current global (or player if specified) board.
- `player [player?]` - Sets or gets the current player.
- `move [cell]` - Sets `cell` to current player
- `bot`
  - `list` - Lists all currently loaded bots.
  - `info [query]` - Finds all bots using `query` and shows detailed info.
  - `run [query] \[args]` - Finds all bots using `query` and runs with args `\arg`
  - `load` - Reloads bots.
  - `unload` - Unloads currently loaded bots.
 
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

### Base library
TODO: write stuff
