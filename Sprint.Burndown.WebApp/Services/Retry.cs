using System;
using System.Collections.Generic;
using System.Threading;

namespace Sprint.Burndown.WebApp.Services
{
    public static class Retry
    {
        private static readonly TimeSpan DefaultRetryInterval = TimeSpan.FromMilliseconds(150);

        public static TResult Do<TResult>(Func<TResult> func, TimeSpan? retryInterval = null, int maxAttemptCount = 3)
        {
            retryInterval = retryInterval ?? DefaultRetryInterval;
            var exceptions = new List<Exception>();

            for (var attempted = 0; attempted < maxAttemptCount; attempted++)
            {
                try
                {
                    if (attempted > 0)
                    {
                        Thread.Sleep(retryInterval.Value);
                    }

                    return func();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            throw new AggregateException(exceptions);

            /*Do<object>(() =>
            {
                action();
                return null;
            }, retryInterval, maxAttemptCount);

            public static T Do<T>(
                Func<T> action,
                TimeSpan retryInterval,
                int maxAttemptCount = 3)
            {
                var exceptions = new List<Exception>();

                for (int attempted = 0; attempted < maxAttemptCount; attempted++)
                {
                    try
                    {
                        if (attempted > 0)
                        {
                            Thread.Sleep(retryInterval);
                        }
                        return action();
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }
                throw new AggregateException(exceptions);
            }*/
        }
    }
}
