namespace Sprint.Burndown.WebApp.Utils
{
    public class IoC
    {
        public static TService Resolve<TService>() where TService : class
        {
            return (TService)Startup.ServiceProvider.GetService(typeof(TService));
        }
    }
}
