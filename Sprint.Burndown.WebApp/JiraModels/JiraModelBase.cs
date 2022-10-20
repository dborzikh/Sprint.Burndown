using System;

using Sprint.Burndown.WebApp.Abstractions;

namespace Sprint.Burndown.WebApp.JiraModels
{
    public abstract class JiraModelBase : IHasIdentifier
    {
        public string Id { get; set; }

        public string Self { get; set; }

        public string Name { get; set; }
    }
}
