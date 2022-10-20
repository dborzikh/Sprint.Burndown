using System;
using System.Collections.Generic;
using System.Linq;

using Sprint.Burndown.WebApp.Abstractions;
using Sprint.Burndown.WebApp.Attributes;

namespace Sprint.Burndown.WebApp.Models
{
    [CacheEntity("UserPreferences")]
    public class UserPreferences : IHasIdentifier
    {
        public string Id { get; set; } = "1";

        public string DefaultBoardId { get; set; } = "-1";

        public IList<SubViewType> PreferredSubViews { get; set; }

        public UserPreferences()
        {
            PreferredSubViews = Enum.GetValues(typeof(SubViewType))
                .OfType<SubViewType>()
                .ToList();
        }
    }
}
