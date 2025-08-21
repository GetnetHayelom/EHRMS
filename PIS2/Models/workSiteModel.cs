using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PIS2.Models;

public partial class workSiteModel
{
    [Key]
    public int workSiteID { get; set; }

    public string workSiteName { get; set; } = null!;
    public string workSiteCode { get; set; } = null!;
    public string workSiteNature { get; set; } = null!;
    public string? mapLink { get; set; } = null!;
    public int? employmentID { get; set; }//Site Manager
    public virtual employmentModel? employmentModel { get; set; }
    public int? subAccountID { get; set; }
    public virtual subAccountModel? subAccountModel {get; set;}
    public DateOnly workSiteEstablishDate { get; set; }

    public mainStatus workSiteStatus { get; set; }
   
    public int addressID { get; set; }

    public virtual addressModel? addressModel { get; set; } = null!;
    public virtual ICollection<workSiteHistoryModel>? WorkSiteHistories { get; set; }
    public virtual ICollection<siteAssignmentModel>? SiteAssignments { get; set; }
    public string modifiedBy { get; set; }
    public workSiteModel() { }
}
