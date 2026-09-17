using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PIS2.Enums;

namespace PIS2.Models.HR;

public class overtimeHistoryModel
{
    [Key] public int overtimeHistoryID { get; set; }
    public int overtimeRecordID { get; set; }
    public virtual overtimeRecordModel? overtimeRecordModel { get; set; }
    public DateTime modifiedDate { get; set; } = DateTime.Now;
    public overtimeStatus overtimeHistoryAction { get; set; }
    public string modifiedBy { get; set; }

    public overtimeHistoryModel()
    {

    }

}
