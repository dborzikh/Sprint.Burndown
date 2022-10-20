using Sprint.Burndown.WebApp.Core;

namespace Sprint.Burndown.WebApp.Abstractions
{
    public interface ICredentialsStorage
    {
        void Add(string token, UserCredentials credentials);

        bool Exists(string token);

        void Remove(string token);

        UserCredentials GetCurrentUserCredentials();
    }
}
