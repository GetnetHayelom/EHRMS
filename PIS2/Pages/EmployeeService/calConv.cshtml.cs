using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PIS2.Pages.EmployeeService
{
    public class calConvModel : PageModel
    {
        [BindProperty]
        public string EthiopianDate { get; set; }  // format: yyyy-MM-dd

        [BindProperty]
        public DateTime? GregorianDate { get; set; }

        public string Result { get; set; }


        public void OnGet()
        {
        }

        public IActionResult OnPostConvertToGregorian()
        {
            if (!string.IsNullOrEmpty(EthiopianDate))
            {
                var parts = EthiopianDate.Split('-');
                if (parts.Length == 3 &&
                    int.TryParse(parts[0], out int year) &&
                    int.TryParse(parts[1], out int month) &&
                    int.TryParse(parts[2], out int day))
                {
                    try
                    {
                        var ethDate = new EthiopianDateTime(year, month, day);
                        GregorianDate = ethDate.ToGregorianDate();
                        Result = $"{EthiopianDate} (Ethiopian) → {GregorianDate:yyyy-MM-dd} (Gregorian)";
                    }
                    catch
                    {
                        Result = "Invalid Ethiopian date!";
                    }
                }
            }
            return Page();
        }

        public IActionResult OnPostConvertToEthiopian()
        {
            if (GregorianDate.HasValue)
            {
                try
                {
                    var ethDate = new EthiopianDateTime(GregorianDate.Value);
                    Result = $"{GregorianDate:yyyy-MM-dd} (Gregorian) -- {ethDate.Year}-{ethDate.Month:D2}-{ethDate.Day:D2} (Ethiopian)";
                }
                catch
                {
                    Result = "Invalid Gregorian date!";
                }
            }
            return Page();
        }
    }

}
public class EthiopianDateTime
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }

    public EthiopianDateTime(int year, int month, int day)
    {
        Year = year;
        Month = month;
        Day = day;
    }

    public EthiopianDateTime(DateTime gregorianDate)
    {
        GregorianToEthiopian(gregorianDate.Year, gregorianDate.Month, gregorianDate.Day, out int y, out int m, out int d);
        Year = y;
        Month = m;
        Day = d;
    }

    public DateTime ToGregorianDate()
    {
        EthiopianToGregorian(Year, Month, Day, out int gYear, out int gMonth, out int gDay);
        return new DateTime(gYear, gMonth, gDay);
    }

    public override string ToString()
    {
        return $"{Year:D4}-{Month:D2}-{Day:D2}";
    }

    // --- Conversion Logic ---

    private static void GregorianToEthiopian(int gYear, int gMonth, int gDay, out int eYear, out int eMonth, out int eDay)
    {
        int[] monthDays = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        bool isLeap = (gYear % 4 == 0 && gYear % 100 != 0) || (gYear % 400 == 0);
        if (isLeap) monthDays[1] = 29;

        int daysOfYear = 0;
        for (int i = 0; i < gMonth - 1; i++) daysOfYear += monthDays[i];
        daysOfYear += gDay;

        int newYearOffset = isLeap ? 12 : 11;
        int ethDayOfYear = daysOfYear - newYearOffset;

        if (ethDayOfYear <= 0)
        {
            ethDayOfYear += isLeap ? 366 : 365;
            eYear = gYear - 8;
        }
        else
        {
            eYear = gYear - 7;
        }

        eMonth = (ethDayOfYear - 1) / 30 + 1;
        eDay = (ethDayOfYear - 1) % 30 + 1;
    }

    private static void EthiopianToGregorian(int eYear, int eMonth, int eDay, out int gYear, out int gMonth, out int gDay)
    {
        // Approximation of Ethiopian → Gregorian
        gYear = eYear + 7;
        int newYearOffset = ((gYear % 4 == 0 && gYear % 100 != 0) || (gYear % 400 == 0)) ? 12 : 11;

        int days = (eMonth - 1) * 30 + eDay + newYearOffset;

        int[] monthDays = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        if ((gYear % 4 == 0 && gYear % 100 != 0) || (gYear % 400 == 0)) monthDays[1] = 29;

        gMonth = 1;
        while (days > monthDays[gMonth - 1])
        {
            days -= monthDays[gMonth - 1];
            gMonth++;
        }
        gDay = days;
    }
}
