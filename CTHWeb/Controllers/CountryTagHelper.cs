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
    /// add select2 with image flags
    /// NuGet package
    /// readme.md / wiki on github
    /// tests
    /// </summary>
    [HtmlTargetElement("select", Attributes = ASPCountryAttributeName)]
    public class CountryTagHelper:TagHelper
    {
        public CountryTagHelper()
        {
            ASPCountryEmpty = true;
        }
        private const string ASPCountryAttributeName = "asp-country";
        private const string ASPCountrySelectedAttributeName = "asp-country-selected";
        private const string ASPCountryEmptyAttributeName = "asp-country-empty";
        [HtmlAttributeName(ASPCountryAttributeName)]
        public bool ASPCountry { get; set; }

        [HtmlAttributeName(ASPCountryEmptyAttributeName)]
        public bool ASPCountryEmpty { get; set; }

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
                if (ASPCountryEmpty)
                {
                    string empty = "<option selected style='display: none' value=''></option>";
                    output.Content.AppendHtml(empty);
                }
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
