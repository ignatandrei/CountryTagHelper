using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CTHWeb.Controllers
{
    /// <summary>
    /// TODO: from IP
    /// TODO: localized
    /// TODO: first item empty
    /// </summary>
    [HtmlTargetElement("select", Attributes = ASPCountryAttributeName)]
    public class CountryTagHelper:TagHelper
    {
        private const string ASPCountryAttributeName = "asp-country";
        private const string ASPCountrySelectedAttributeName = "asp-country-selected";

        [HtmlAttributeName(ASPCountryAttributeName)]
        public bool ASPCountry { get; set; }

        [HtmlAttributeName(ASPCountrySelectedAttributeName)]
        public string ASPCountrySelected{ get; set; }

        static string[] CountryISO;
        //static PropertyInfo[] properties;
        static CountryTagHelper()
        {
            var t =typeof( ResCTH);
            var properties= t.GetProperties(
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
            if (ASPCountry)
            {
                bool existSelected = !string.IsNullOrWhiteSpace(ASPCountrySelected);
                string option = "<option value='{0}' {2}>{1}</option>";
                foreach (var item in CountryISO)
                {
                    string selected = "";
                    string localizedName = ResCTH.ResourceManager.GetString(item);
                    if (existSelected)
                    {
                        bool currentItem = string.Equals(item, ASPCountrySelected, StringComparison.CurrentCultureIgnoreCase);
                        currentItem = currentItem || string.Equals(localizedName, ASPCountrySelected, StringComparison.CurrentCultureIgnoreCase);
                        if (currentItem)
                            selected = "selected";
                    }
                    output.Content.AppendFormat(option, item, localizedName,selected);
                }
            }
            return base.ProcessAsync(context, output);
        }
    }
}
