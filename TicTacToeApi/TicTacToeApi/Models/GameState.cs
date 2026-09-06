namespace TicTacToeApi.Models
{
    public enum GameStatus { InProgress, Won, Draw }

    //public class Move
    //{
        
    //}

    public class GameState
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string[] Board { get; set; } = new string[9];
        public string CurrentPlayer { get; set; } = "X";
        public GameStatus Status { get; set; } = GameStatus.InProgress;
        public string? Winner { get; set; }
        public List<int> WinningCells { get; set; } = new();
        public List<Move> History { get; set; } = new();
    }
}
