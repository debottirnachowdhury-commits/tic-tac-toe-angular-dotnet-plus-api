using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicTacToeApi.Models;
using TicTacToeApi.Services;

namespace TicTacToeApi.Controllers
{
    [ApiController]
    [Route("api/games")]
    public class GamesController : ControllerBase
    {
        private readonly GameService _service;
        public GamesController(GameService service)
        {
            _service = service;
        }

        [HttpPost]
        public ActionResult<GameState> CreateGame() => Ok(_service.CreateGame());

        [HttpGet("{id}")]
        public ActionResult<GameState> GetGame(Guid id)
        {
            var game = _service.GetGame(id);
            return game == null ? NotFound() : Ok(game);
        }

        [HttpPost("{id}/moves")]
        public ActionResult<GameState> MakeMove(Guid id, [FromBody] Move move, [FromQuery] bool vsComputer = false)
        {
            var game = _service.MakeMove(id, move, vsComputer);
            return game == null ? NotFound() : Ok(game);
        }

        [HttpPost("{id}/reset")]
        public ActionResult<GameState> ResetGame(Guid id) => Ok(_service.ResetGame(id));

        [HttpGet("/api/scoreboard")]
        public ActionResult<Scoreboard> GetScoreboard() => Ok(_service.GetScoreboard());

        [HttpPost("/api/scoreboard/reset")]
        public ActionResult ResetScoreboard()
        {
            _service.ResetScoreboard();
            return Ok(_service.GetScoreboard());
        }

        [HttpPost("{id}/undo")]
        public ActionResult<GameState> Undo(Guid id, [FromQuery] bool vsComputer = false)
        {
            var game = _service.UndoLastMove(id, vsComputer);
            return game == null ? NotFound() : Ok(game);
        }

        [HttpPost("{id}/computer-move")]
        public ActionResult<GameState> ComputerMove(Guid id)
        {
            var game = _service.MakeComputerMove(id);
            return game == null ? NotFound() : Ok(game);
        }

    }
}
