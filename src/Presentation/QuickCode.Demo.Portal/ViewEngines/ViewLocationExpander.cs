using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickCode.Demo.Portal.ViewEngines
{
    public class ViewLocationExpander : IViewLocationExpander
    {

        /// <summary>
        /// Used to specify the locations that the view engine should search to 
        /// locate views.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="viewLocations"></param>
        /// <returns></returns>
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            //{2} is area, {1} is controller,{0} is the action
            var locations = new[]
            {
                "/Views/{1}/{0}.cshtml",
                "/Views/Generated/{1}/{0}.cshtml",
                "/Views/Generated/{2}/{1}/{0}.cshtml",
                "/Views/{2}/{1}/{0}.cshtml",
                "/Views/Defaults/{1}/{0}.cshtml",
                "/Views/UserManagerModule/{1}/{0}.cshtml"
            };

            return locations.Union(viewLocations);
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            context.Values["customviewlocation"] = nameof(ViewLocationExpander);
        }
    }
}
