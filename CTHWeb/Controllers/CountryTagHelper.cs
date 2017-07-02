using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;

namespace CTHWeb.Controllers
{
    public class CountryTagHelper:TagHelper
    {
        static string[] CountryISO;
        static PropertyInfo[] properties;
        static CountryTagHelper()
        {
            var t =typeof( ResCTH);
            properties= t.GetProperties(
                BindingFlags.Public |
                BindingFlags.Static |
                BindingFlags.GetProperty);
            CountryISO = properties
                .Where(it=>it.Name.Length==2)
                .Select(it => it.Name)
                .ToArray();
        }
        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            
            return base.ProcessAsync(context, output);
        }
    }
}
