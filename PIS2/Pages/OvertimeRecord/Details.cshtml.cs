using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models.HR;

namespace PIS2.Pages.OvertimeRecord
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

        public overtimeRecordModel overtimeRecordModel { get; set; } = default!;
        public List<overtimeHistoryModel> OvertimeHistories { get; set; } = default!;
        public double otCost = 0;
        public bool isSelf { get; set; } = false;
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var overtimerecordmodel = await _context.OvertimeRecords.Include(otr => otr.employmentModel).ThenInclude(e => e.personModel)
                .Include(otr => otr.overtimeModel)
                .Include(otr => otr.OvertimeHistories)
                .FirstOrDefaultAsync(m => m.overtimeRecordID == id);
            OvertimeHistories = await _context.OvertimeHistories.Where(oh =>oh.overtimeRecordID == id).ToListAsync();
            //otCost = overtimeRecordModel.GetOTCost();
            if (overtimerecordmodel == null)
            {
                return NotFound();
            }
            else
            {
                overtimeRecordModel = overtimerecordmodel;
                var currentUser = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

                if (currentUser != null && currentUser.personID == overtimeRecordModel.employmentModel?.personID)
                {
                    isSelf = true;
                }
            }
            return Page();
        }
        [BindProperty]
        public int overtimeRecordID { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            var overtimeRecord = await _context.OvertimeRecords.FindAsync(overtimeRecordID);
            if (overtimeRecord == null)
            {
                return NotFound();
            }

            overtimeRecord.modifiedBy = User.Identity.Name;
            overtimeRecord.overtimeRecordStatus = overtimeStatus.Posted;

            _context.Attach(overtimeRecord).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

            return RedirectToPage("./Details", new { id = overtimeRecordID });
        }
    }
}
