using System;

namespace Sprint.Burndown.WebApp.Attributes
{
    public class CacheEntityAttribute : Attribute
    {
        public string Name { get; }

        public CacheEntityAttribute(string name)
        {
            Name = name;
        }
    }
}
