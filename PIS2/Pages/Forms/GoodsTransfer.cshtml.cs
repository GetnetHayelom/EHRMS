using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using System.ComponentModel.DataAnnotations;

namespace PIS2.Pages.Forms
{
    public class GoodsTransferModel : PageModel
    {

       
        public void OnGet()
        {
        }
    

            public TransferSlipModel TransferSlip { get; set; } = new TransferSlipModel();
    }

        // A model to hold all the form data
        public class TransferSlipModel
        {
            [DataType(DataType.Date)]
            public DateTime Date { get; set; } = DateTime.Now;

            // From
            public string FromName { get; set; }
            public string FromIDNo { get; set; }
            public string FromDept { get; set; }
            public string FromSign { get; set; }

            // To
            public string ToName { get; set; }
            public string ToIDNo { get; set; }
            public string ToDept { get; set; }
            public string ToSign { get; set; }

            // Authorization
            public string AuthorizedBy { get; set; }
            public string AuthJobTitle { get; set; }
            public string AuthSign { get; set; }

            // Posting
            public string PostedBy { get; set; }
            public string PostJobTitle { get; set; }
            public string PostSign { get; set; }

       
        public List<TransferItem> Items { get; set; } = new List<TransferItem> { new TransferItem() };
      }
    // 1. New model for a single item row
    public class TransferItem
    {
        public string AssetCode { get; set; }
        public string Description { get; set; }
        public string Condition { get; set; }
        public string InspectorSign { get; set; } // Use 'Sign' for simplicity
        public string Remark { get; set; }
    }


}
