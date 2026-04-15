using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PIS2.TagHelpers
{
    // This allows to use <excel-export> in Razor pages
    [HtmlTargetElement("excel-export")]
    public class ExcelExportTagHelper : TagHelper
    {
        // Table ID to export
        [HtmlAttributeName("table")]
        public string Table { get; set; }

        // Filename for Excel file
        [HtmlAttributeName("filename")]
        public string FileName { get; set; } = "export.xlsx";

        // Optional CSS class
        [HtmlAttributeName("class")]
        public string CssClass { get; set; } = "btn btn-outline-primary m-2 d-print-none d-flex";
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Render a <button> element
            output.TagName = "button";
            output.TagMode = TagMode.StartTagAndEndTag;

            // Set button attributes
            output.Attributes.SetAttribute("type", "button");
            output.Attributes.SetAttribute("class", CssClass);
            output.Attributes.SetAttribute("onclick", $"exportTableToExcel('{Table}', '{FileName}')");

            // Set inner text
            output.Content.SetContent("To Excel");
        }
    }
}
