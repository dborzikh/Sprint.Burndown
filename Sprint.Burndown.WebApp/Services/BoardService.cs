using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Sprint.Burndown.WebApp.Abstractions;
using Sprint.Burndown.WebApp.Extensions;
using Sprint.Burndown.WebApp.Models;
using Sprint.Burndown.WebApp.ViewModels;

namespace Sprint.Burndown.WebApp.Services
{
    public class BoardService : IBoardService
    {
        private ICacheStorage CacheStorage { get; }

        private ICredentialsStorage CredentialsStorage { get; }

        private IRepository Repository { get; }

        public BoardService(ICacheStorage cacheStorage, IRepository repository, ICredentialsStorage credentialsStorage)
        {
            CacheStorage = cacheStorage;
            CredentialsStorage = credentialsStorage;
            Repository = repository;
        }

        public IList<BoardViewModel> GetBoards()
        {
            return Repository
                .GetBoards()
                .ProjectTo<BoardViewModel>()
                .ToList();
        }

        public BoardViewModel GetDefaultBoard()
        {
            if (!HasCredentials())
            {
                return null;
            }

            var preferences = GetUserPreferencesOrDefault();
            var boards = Repository
                .GetBoards()
                .ProjectTo<BoardViewModel>()
                .ToList();

            return boards.FirstOrDefault(board => board.Id == preferences.DefaultBoardId) ?? boards.FirstOrDefault();
        }

        public void SaveDefaultBoard(BoardViewModel board)
        {
            var preferences = GetUserPreferencesOrDefault();
            preferences.DefaultBoardId = board.Id;

            CacheStorage.Put(preferences);
        }

        public UserPreferencesViewModel GetUserPreferences()
        {
            return Mapper.Map<UserPreferencesViewModel>(GetUserPreferencesOrDefault());
        }

        public void UpdatePreferences(UserPreferencesViewModel preferencesModel)
        {
            if (preferencesModel?.PreferredSubViews == null)
            {
                return;
            }

            var preferences = GetUserPreferencesOrDefault();
            preferences.PreferredSubViews = preferencesModel.PreferredSubViews
                .Distinct()
                .ToList();

            CacheStorage.Put(preferences);
        }

        private UserPreferences GetUserPreferencesOrDefault()
        {
            var credentials = CredentialsStorage.GetCurrentUserCredentials();
            var currentUserPreferences = CacheStorage.Get<UserPreferences>(credentials.UserId);
            return currentUserPreferences ?? new UserPreferences { Id = credentials.UserId };
        }

        private bool HasCredentials()
        {
            return CredentialsStorage.GetCurrentUserCredentials() != null;
        }
    }
}
