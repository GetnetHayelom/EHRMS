namespace PIS2.Views
{
    public class CertificationView
    {
        public string CertificationType { get; set; }
        public int? CategoryID { get; set; }
        public int MaleCount { get; set; }
        public int FemaleCount { get; set; }
        public int TotalCount { get; set; }
        public CertificationView() { }
    }

    public class CertificationSummaryView
    {
        public string CertificationCategory { get; set; }
        public int Male { get; set; }
        public int Female { get; set; }
        public int Total { get; set; }

    }
}
