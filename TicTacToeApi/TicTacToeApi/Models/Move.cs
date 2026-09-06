namespace TicTacToeApi.Models
{
    public class Move
    {
        public int MoveNumber { get; set; }
        public string Player { get; set; } = String.Empty;
        public int Row { get; set; }
        public int Col { get; set; }
    }
}
