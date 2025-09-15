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
        public string Gender { get; set; }
        public string CertificationName { get; set; }
        public int Count { get; set; }

    }
}
