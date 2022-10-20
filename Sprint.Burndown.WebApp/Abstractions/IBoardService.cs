using System.Collections.Generic;
using Sprint.Burndown.WebApp.ViewModels;

namespace Sprint.Burndown.WebApp.Abstractions
{
    public interface IBoardService
    {
        BoardViewModel GetDefaultBoard();

        void SaveDefaultBoard(BoardViewModel board);

        IList<BoardViewModel> GetBoards();

        UserPreferencesViewModel GetUserPreferences();

        void UpdatePreferences(UserPreferencesViewModel preferencesModel);
    }
}
