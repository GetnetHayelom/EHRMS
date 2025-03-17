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

    public DateOnly workSiteEstablishDate { get; set; }

    public mainStatus workSiteStatus { get; set; }
   
    public int addressID { get; set; }

    public virtual addressModel? addressModel { get; set; } = null!;
    public virtual ICollection<workSiteHistoryModel>? WorkSiteHistories { get; set; }
    public virtual ICollection<jobPlacementModel>? JobPlacements { get; set; }

    public workSiteModel() { }
}
