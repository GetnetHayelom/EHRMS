using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using static System.Formats.Asn1.AsnWriter;

namespace PIS2.Pages.AJAX
{
    public class CallMe : PageModel
    {
        private readonly PISContext _context;
        public CallMe(PISContext ctx)
        {
            _context = ctx;
            
        }
        [HttpPost]
        public JsonResult OnPostCalculateWorkingDays(DateTime startDate, DateTime endDate)
        {

            // Example: List of holidays – ideally from a database or config
            var holidays = _context.Holidays.Where(h => h.holidayStatus == mainStatus.Active).ToList();

            double workingDays = 0;

            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                if (IsHoliday(date))
                    continue;

                if (date.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                if (date.DayOfWeek == DayOfWeek.Saturday)
                    workingDays += 0.5;
                else
                    workingDays += 1;
            }

            return new JsonResult(workingDays);
        }
        public bool IsHoliday(DateTime date)
        {
            List<holidayModel> holidays = _context.Holidays.Where(h => h.holidayStatus == mainStatus.Active).ToList();
            return holidays.Any(h =>
                date.Date >= h.holidayStart.Date &&
                date.Date <= (h.holidayEnd == default ? h.holidayStart.Date : h.holidayEnd.Date)
            );
        }

    }
}
