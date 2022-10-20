using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using Sprint.Burndown.WebApp.Core;

namespace Sprint.Burndown.WebApp.Attributes
{
    public class InvalidateCacheAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var controller = (Controller)context.Controller;
            var cacheHeaders = controller.Request.Headers["Cache-Control"];
            var invalidateCache = cacheHeaders.Any(s => s.Trim().Equals("no-cache", StringComparison.InvariantCultureIgnoreCase));

            var cacheUpdateHeaders = controller.Request.Headers["X-Cache-Update"];
            var allowPartialUpdate = cacheUpdateHeaders.Any(s => s.Trim().Equals("partial", StringComparison.InvariantCultureIgnoreCase));

            var cacheContext = new CacheContext()
            {
                InvalidateCache = invalidateCache,
                AllowPartialUpdate = allowPartialUpdate

            };

            controller.Request.HttpContext.Items[nameof(CacheContext)] = cacheContext;
        }
    }
}
