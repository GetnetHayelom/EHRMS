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
}
