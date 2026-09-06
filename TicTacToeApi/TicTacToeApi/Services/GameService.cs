using System.Collections.Concurrent;
using TicTacToeApi.Models;

namespace TicTacToeApi.Services
{
    public class GameService
    {
        private readonly ConcurrentDictionary<Guid, GameState> _games = new();
        private readonly object _scoreboardLock = new();
        private readonly Scoreboard _scoreboard = new();

        public GameState CreateGame()
        {
            var game = new GameState
            {
                Id = Guid.NewGuid(),
                Board = new string[9],
                CurrentPlayer = "X",
                Status = GameStatus.InProgress
            };
            _games[game.Id] = game;
            return game;
        }

        public GameState? GetGame(Guid id) =>
            _games.TryGetValue(id, out var game) ? game : null;

        public GameState? MakeMove(Guid id, Move move, bool vsComputer = false)
        {
            if (!_games.TryGetValue(id, out var game)) return null;

            lock (game)
            {
                int index = move.Row * 3 + move.Col;
                if (string.IsNullOrEmpty(game.Board[index]) &&
                    game.CurrentPlayer == move.Player &&
                    game.Status == GameStatus.InProgress)
                {
                    ApplyMove(game, move, isComputer: false);
                }

                if (vsComputer && game.Status == GameStatus.InProgress && game.CurrentPlayer == "O")
                {
                    MakeComputerMove(game);
                }
            }

            return game;
        }

        public GameState ResetGame(Guid id)
        {
            var game = new GameState
            {
                Id = id,
                Board = new string[9],
                CurrentPlayer = "X",
                Status = GameStatus.InProgress
            };
            _games[id] = game;
            return game;
        }

        public Scoreboard GetScoreboard() => _scoreboard;

        public void ResetScoreboard()
        {
            lock (_scoreboardLock)
            {
                _scoreboard.XWins = 0;
                _scoreboard.OWins = 0;
                _scoreboard.Draws = 0;
            }
        }

        public GameState? UndoLastMove(Guid id, bool vsComputer = false)
        {
            if (!_games.TryGetValue(id, out var game)) return null;
            if (game.History.Count == 0) return game;

            lock (game)
            {
                if (vsComputer && game.History.Count >= 2)
                {
                    var last = game.History[^1];
                    var prev = game.History[^2];

                    game.Board[last.Row * 3 + last.Col] = string.Empty;
                    game.Board[prev.Row * 3 + prev.Col] = string.Empty;

                    game.History.RemoveRange(game.History.Count - 2, 2);
                    game.CurrentPlayer = "X";
                }
                else
                {
                    var last = game.History[^1];
                    game.Board[last.Row * 3 + last.Col] = string.Empty;
                    game.History.RemoveAt(game.History.Count - 1);
                    game.CurrentPlayer = last.Player;
                }

                game.Status = GameStatus.InProgress;
                game.Winner = null;
                game.WinningCells.Clear();
                CheckGameStatus(game);
            }

            return game;
        }

        public GameState? MakeComputerMove(Guid id)
        {
            if (!_games.TryGetValue(id, out var game)) return null;

            lock (game)
            {
                if (game.Status == GameStatus.InProgress && game.CurrentPlayer == "O")
                {
                    MakeComputerMove(game);
                }
            }

            return game;
        }

        private void ApplyMove(GameState game, Move move, bool isComputer)
        {
            int index = move.Row * 3 + move.Col;
            game.Board[index] = move.Player;
            move.MoveNumber = game.History.Count + 1;
            game.History.Add(move);

            CheckGameStatus(game);

            if (game.Status == GameStatus.InProgress)
            {
                if (!isComputer)
                {
                    // Human move: flip turn
                    game.CurrentPlayer = game.CurrentPlayer == "X" ? "O" : "X";
                }
                else
                {
                    // Computer always plays O, so give turn back to X
                    game.CurrentPlayer = "X";
                }
            }
            else if (game.Status == GameStatus.Won)
            {
                lock (_scoreboardLock)
                {
                    if (game.Winner == "X") _scoreboard.XWins++;
                    else _scoreboard.OWins++;
                }
            }
            else if (game.Status == GameStatus.Draw)
            {
                lock (_scoreboardLock) { _scoreboard.Draws++; }
            }
        }

        private void MakeComputerMove(GameState game)
        {
            // 1. Computer tries to win
            if (TryFindWinningMove(game, "O", out var winMove))
            {
                winMove.Player = "O";
                ApplyMove(game, winMove, isComputer: true);
                return;
            }

            // 2. Computer tries to block X
            if (TryFindWinningMove(game, "X", out var blockMove))
            {
                // IMPORTANT:
                // We searched for X's winning position,
                // but the actual move belongs to the computer = O.
                blockMove.Player = "O";

                ApplyMove(game, blockMove, isComputer: true);
                return;
            }

            // 3. Take center
            if (string.IsNullOrEmpty(game.Board[4]))
            {
                ApplyMove(
                    game,
                    new Move
                    {
                        Player = "O",
                        Row = 1,
                        Col = 1
                    },
                    isComputer: true);

                return;
            }

            // 4. Take a corner
            var corners = new (int, int)[]
            {
        (0, 0),
        (0, 2),
        (2, 0),
        (2, 2)
            };

            foreach (var (r, c) in corners)
            {
                int idx = r * 3 + c;

                if (string.IsNullOrEmpty(game.Board[idx]))
                {
                    ApplyMove(
                        game,
                        new Move
                        {
                            Player = "O",
                            Row = r,
                            Col = c
                        },
                        isComputer: true);

                    return;
                }
            }

            // 5. Take any remaining cell
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    int idx = r * 3 + c;

                    if (string.IsNullOrEmpty(game.Board[idx]))
                    {
                        ApplyMove(
                            game,
                            new Move
                            {
                                Player = "O",
                                Row = r,
                                Col = c
                            },
                            isComputer: true);

                        return;
                    }
                }
            }
        }

        private bool TryFindWinningMove(GameState game, string player, out Move move)
        {
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                {
                    int idx = r * 3 + c;
                    if (string.IsNullOrEmpty(game.Board[idx]))
                    {
                        game.Board[idx] = player;
                        CheckGameStatus(game);
                        if (game.Status == GameStatus.Won && game.Winner == player)
                        {
                            game.Board[idx] = string.Empty;
                            game.Status = GameStatus.InProgress;
                            game.Winner = null;
                            game.WinningCells.Clear();
                            move = new Move { Player = player, Row = r, Col = c };
                            return true;
                        }
                        game.Board[idx] = string.Empty;
                        game.Status = GameStatus.InProgress;
                        game.Winner = null;
                        game.WinningCells.Clear();
                    }
                }
            move = null!;
            return false;
        }

        private void CheckGameStatus(GameState game)
        {
            string[] b = game.Board;
            var lines = new List<List<int>> {
                new(){0,1,2}, new(){3,4,5}, new(){6,7,8},
                new(){0,3,6}, new(){1,4,7}, new(){2,5,8},
                new(){0,4,8}, new(){2,4,6}
            };

            foreach (var line in lines)
            {
                var values = line.Select(i => b[i]).ToList();
                if (values.All(v => v == "X") || values.All(v => v == "O"))
                {
                    game.Status = GameStatus.Won;
                    game.Winner = values.First();
                    game.WinningCells = line.Select(i => (i / 3, i % 3)).ToList();
                    return;
                }
            }

            if (game.History.Count == 9) game.Status = GameStatus.Draw;
        }
    }
}
