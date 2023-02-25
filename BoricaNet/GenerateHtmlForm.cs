using Newtonsoft.Json.Linq;

namespace BoricaNet;

internal static class GenerateHtmlForm
{
    public static string GenerateHTMLForm(string json, string id = "borica-form", bool isDev = false)
    {
        var obj = JObject.Parse(json);

        string html;
        if (isDev)
        {
            html = $"<form id={id} method=\"post\" action=\"https://3dsgate-dev.borica.bg/cgi-bin/cgi_link\">";
        }
        else
        {
            html = $"<form id={id} method=\"post\" action=\"https://3dsgate.borica.bg/cgi-bin/cgi_link\">";
        }

        foreach (var property in obj.Properties())
        {
            if(!string.IsNullOrEmpty(property.Value.ToString()))
            {
                //html += $"<input type='hidden' name='" + property.Name + "size='" + "' value='" + property.Value + "'>";
                html += $"<input type=\"hidden\" name=\"{property.Name}\" size=\"{property.Value.ToString().Length}\" value=\"{property.Value}\">";
            }
        }

        html += "<input type='submit' value='Submit'>";

        html += "</form>";

        return html;
    }
}