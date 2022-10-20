using Sprint.Burndown.WebApp.Extensions;

namespace Sprint.Burndown.WebApp.Const
{
    public class CacheKeys
    {
        private const string RootKey = "Sprint.Burndown";

        private static string AllBoardsKey => CacheKeyFor("Boards:All");

        private static string AllSprintsKey => CacheKeyFor("Sprints:All");

        private static string AllIssuesKey => CacheKeyFor("Issues:All");

        public static CacheKeyInfo AllBoards()
        {
            return new CacheKeyInfo(AllBoardsKey);
        }

        public static CacheKeyInfo AllSprints => new CacheKeyInfo(AllSprintsKey);

        public static CacheKeyInfo AllIssues => new CacheKeyInfo(AllIssuesKey);

        public static CacheKeyInfo SprintsOf(string boardId) => new CacheKeyInfo(CacheKeyFor($"Sprints:{boardId}"));

        public static CacheKeyInfo IssuesOf(string sprintId)
        {
            return new CacheKeyInfo(CacheKeyFor($"Issues:{sprintId}"));
        }

        public static CacheKeyInfo AllHolidays()
        {
            return new CacheKeyInfo(CacheKeyFor("Holidays:All"));
        }

        private static string CacheKeyFor(string keyName)
        {
            return $"{RootKey}:Entities:{keyName}";
        }
    }
}
