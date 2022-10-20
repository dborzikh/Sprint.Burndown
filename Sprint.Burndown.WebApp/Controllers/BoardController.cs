using System.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Sprint.Burndown.WebApp.Abstractions;
using Sprint.Burndown.WebApp.Attributes;
using Sprint.Burndown.WebApp.ViewModels;

namespace Sprint.Burndown.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/board")]
    [Authorize]
    public class BoardController : Controller
    {
        private IBoardService BoardService { get; }

        private IGlobalContext Context { get; }

        private ISprintService SprintService { get; }

        public BoardController(IGlobalContext context, IBoardService boardService, ISprintService sprintService)
        {
            Context = context;
            BoardService = boardService;
            SprintService = sprintService;
        }

        [HttpGet]
        [Route("[action]")]
        [InvalidateCache]
        public ActionResult<BoardViewModel> Default()
        {
            var board = BoardService.GetDefaultBoard();
            if (board == null)
            {
                return NotFound();
            }

            return board;
        }

        [HttpGet]
        [Route("[action]")]
        [InvalidateCache]
        public ActionResult<BoardViewModel[]> GetAll()
        {
            var boards = BoardService
                .GetBoards()
                .ToArray();

            if (!boards.Any())
            {
                return NotFound();
            }

            return boards;
        }

        [HttpGet("{id}", Order = 3)]
        [InvalidateCache]
        public ActionResult<object> Sprints(BoardViewModel board)
        {
            var boards = BoardService.GetBoards();
            var selectedBoard = boards.FirstOrDefault(p => p.Id == board.Id);
            var sprints = SprintService.GetSprints(selectedBoard);

            return new
            {
                Context.HasIncompleteData,
                Boards = boards, // TODO: Fix SRP violation
                Sprints = sprints,
            };
        }

        [HttpPost("{id}")]
        public ActionResult UpdateDefaultBoard(BoardViewModel board)
        {
            BoardService.SaveDefaultBoard(board);

            return Ok();
        }

        [HttpGet]
        [Route("[action]")]
        public ActionResult<UserPreferencesViewModel> GetPreferences()
        {
            return BoardService.GetUserPreferences();
        }

        [HttpPost]
        [Route("[action]")]
        public ActionResult<UserPreferencesViewModel> SavePreferences([FromBody]UserPreferencesViewModel preferences)
        {
            BoardService.UpdatePreferences(preferences);

            return Ok();
        }
    }
}