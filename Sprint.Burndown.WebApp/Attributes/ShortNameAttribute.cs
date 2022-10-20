using System;

namespace Sprint.Burndown.WebApp.Attributes
{
    public class ShortNameAttribute : Attribute
    {
        public string ShortName { get; }

        public ShortNameAttribute(string shortName)
        {
            ShortName = shortName;
        }
    }
}