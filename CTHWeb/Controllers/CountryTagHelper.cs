using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Primitives;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CTHWeb.Controllers
{
    /// <summary>
    /// TODO: localized
    /// add select2 with image flags
    /// NuGet package
    /// app hb deploy
    /// TODO: provider / dependency injection for FromIP 
    /// readme.md / wiki on github
    /// tests
    /// </summary>
    [HtmlTargetElement("select", Attributes = ASPCountryAttributeName)]
    public class CountryTagHelper:TagHelper
    {
        [ViewContext]
        public ViewContext ViewContext { get; set; }


        public CountryTagHelper()
        {
            
            ASPCountryEmpty = true;
        }
        private const string ASPCountryAttributeName = "asp-country";
        private const string ASPCountrySelectedAttributeName = "asp-country-selected";
        private const string ASPCountryEmptyAttributeName = "asp-country-empty";
        private const string ASPCountryFromIPAttributeName = "asp-country-fromIP";
        [HtmlAttributeName(ASPCountryAttributeName)]
        public bool ASPCountry { get; set; }

        [HtmlAttributeName(ASPCountryEmptyAttributeName)]
        public bool ASPCountryEmpty { get; set; }

        [HtmlAttributeName(ASPCountrySelectedAttributeName)]
        public string ASPCountrySelected{ get; set; }

        [HtmlAttributeName(ASPCountryFromIPAttributeName)]
        public bool ASPCountryFromIP{ get; set; }

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
        private async Task<string> GetCountryCodeFromIP(string ip)
        {
            //ip= "188.25.145.65";
            var url = "http://freegeoip.net/csv/"+ip;
            var request = WebRequest.Create(url);
            request.Method = "GET";
            using (var wr = await request.GetResponseAsync())
            {
                using (var receiveStream = wr.GetResponseStream())
                {
                    using (var reader = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        string content = reader.ReadToEnd();
                        return content.Split(',')[1];
                    }
                        
                }
                    
            }


        }
        string GetUserIP(ViewContext vc)
        {
            StringValues headerFwd;
            if ((ViewContext.HttpContext?.Request?.Headers?.TryGetValue("X-Forwarded-For", out headerFwd) ?? false){
                string rawValues = headerFwd.ToString();  
                if (!string.IsNullOrWhiteSpace(rawValues))
                {
                    return rawValues.Split(',')[0];
                }

            }
            return ViewContext.HttpContext?.Connection?.RemoteIpAddress?.ToString();

        }
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
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
                if(ASPCountryFromIP && !existSelected)
                {
                    string ip;
                    
                    try
                    {
                        ip = GetUserIP(ViewContext);
                        ASPCountrySelected = await GetCountryCodeFromIP(ip);
                        existSelected = true;
                    }
                    catch(Exception ex)
                    {
                        //do nothing...
                    }
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
            await base.ProcessAsync(context, output);
        }
    }
}
