using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PIS2.Pages.Forms
{
    public class GoodsGatePassModel : PageModel
    {
        [BindProperty]
        public GatePassModel GatePass { get; set; } = new GatePassModel();
        public void OnGet()
        {
        }
    }
    public class GatePassModel
    {
        public string? To { get; set; }
        public string? From { get; set; }
        public DateTime? Date { get; set; }
        public string? RequestedBy { get; set; }
        public string? Signature { get; set; }
        public string? Items { get; set; }
        public string? Reason { get; set; }
        public string? ApprovedBy { get; set; }
        public string? Title { get; set; }
        public string? VehicleNo { get; set; }
        public string? Destination { get; set; }
    }
}
