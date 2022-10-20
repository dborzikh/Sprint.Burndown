using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sprint.Burndown.WebApp.Abstractions
{
    public interface IHasAdditions: IHasIdentifier
    {
        IHasIdentifier GetAdditions();
    }

    public interface IHasAdditions<T> : IHasAdditions where T: class, IHasIdentifier
    {
        T Additions { get; set; }
    }
}
