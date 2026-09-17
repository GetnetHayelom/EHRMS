namespace PIS2.Models.Foundation
{
    public static class NumberToWordConverter
    {
        public static string Convert(decimal number)
        {
            if (number == 0) return "Zero";

            // Separate the decimal for currency/formatting if needed
            long absoluteNumber = (long)Math.Floor(number);

            return NumberToText(absoluteNumber).Trim();
        }

        private static string NumberToText(long n)
        {
            if (n < 0) return "Negative " + NumberToText(Math.Abs(n));
            if (n < 20) return new[] { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" }[n];
            if (n < 100) return new[] { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" }[n / 10] + (n % 10 != 0 ? " " + NumberToText(n % 10) : "");
            if (n < 1000) return NumberToText(n / 100) + " Hundred" + (n % 100 != 0 ? " " + NumberToText(n % 100) : "");
            if (n < 1000000) return NumberToText(n / 1000) + " Thousand" + (n % 1000 != 0 ? " " + NumberToText(n % 1000) : "");
            if (n < 1000000000) return NumberToText(n / 1000000) + " Million" + (n % 1000000 != 0 ? " " + NumberToText(n % 1000000) : "");

            return "Number too large";
        }
    }
}
