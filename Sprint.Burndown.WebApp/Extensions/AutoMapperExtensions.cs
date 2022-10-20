using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace Sprint.Burndown.WebApp.Extensions
{
    public static class AutoMapperExtensions
    {
        public static IEnumerable<TDest> ProjectTo<TDest>(this IEnumerable<Object> query)
        {
            if (query == null)
            {
                return null;
            }

            return query.Select(Mapper.Map<TDest>);
        }
    }
}
