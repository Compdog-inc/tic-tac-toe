# Changelog

## [Unreleased]

### Added
 - Plugin Controlled User Environment (PCUE)
 - PluginManager
 
### Changed

- `TicTacToe.Bots.IBot` is now `TicTacToe.Plugins.IPlugin`
- Removed repeating function arguments from `IPlugin` and replaced with `PCUE` in `Load`

## [1.1.0.0] - 2021-09-16

### Added

- CommandLine utils to base library
- Global events
- `bot event` client command

### Changed

- Upgraded base library to v1.1.0.0

## [1.0.0.1] - 2021-09-16

### Added

- Main game logic
- Bot interface

[unreleased]: https://github.com/Compdog-inc/tic-tac-toe/compare/v1.1.0.0...main
[1.1.0.0]: https://github.com/Compdog-inc/tic-tac-toe/releases/tag/v1.1.0.0
[1.0.0.1]: https://github.com/Compdog-inc/tic-tac-toe/releases/tag/v1.0.0.1
