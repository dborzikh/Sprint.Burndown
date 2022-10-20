namespace Sprint.Burndown.WebApp.Abstractions
{
    public interface IGlobalContext
    {
        bool HasIncompleteData { get; set;  }
    }
}
