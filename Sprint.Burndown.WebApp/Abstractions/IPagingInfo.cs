using System;

namespace Sprint.Burndown.WebApp.Abstractions
{
    public interface IPagingInfo
    {
        int MaxResults { get; set; }

        int StartAt { get; set; }

        int? Total { get; set; }

        bool? IsLast { get; set; }
    }
}