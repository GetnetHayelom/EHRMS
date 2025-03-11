using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PIS2.Models;

public class overtimeHistoryModel
{
    [Key] public int overtimeHistoryID { get; set; }
    public int overtimeRecordID { get; set; }
    public virtual overtimeRecordModel? overtimeRecordModel { get; set; }
    public DateTime overtimeHistoryDate { get; set; } = DateTime.Now;
    public overtimeStatus overtimeHistoryAction { get; set; }
    public string overtimeUser { get; set; }

    public overtimeHistoryModel()
    {

    }

}
public enum overtimeStatus
{
    Hold,
    Approved,
    Posted,
    Released,
    Completed,
    Cancelled,
    Void
}