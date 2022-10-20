using Sprint.Burndown.WebApp.Attributes;

namespace Sprint.Burndown.WebApp.ViewModels
{
    public enum IssueGroup
    {
        [ShortName("U")]
        Undefined = 0,

        [ShortName("D")]
        Debt = 1,

        [ShortName("S")]
        Scope = 2,

        [ShortName("B")]
        Bonus = 3
    }
}
