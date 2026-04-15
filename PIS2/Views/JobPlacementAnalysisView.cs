namespace PIS2.Views
{
    public class JobPlacementAnalysisView
    {
        public List<PlacementGap> UnderStaffed { get; set; } = new();
        public List<PlacementGap> OverStaffed { get; set; } = new();
        public List<UnstructuredPlacement> Unstructured { get; set; } = new();

        public class PlacementGap
        {
            public string CompanyName { get; set; } = "";
            public string DepartmentName { get; set; } = "";
            public string JobTitle { get; set; } = "";
            public int Required { get; set; }
            public int Actual { get; set; }
            public int Difference => Math.Abs(Required - Actual);
        }

        public class UnstructuredPlacement
        {
            public int JobPlacementID { get; set; }
            public int EmploymentID { get; set; }
            public string GivenID { get; set; } = "";
            public string EmployeeName { get; set; } = "";
            public string DepartmentName { get; set; } = "";
            public string JobTitle { get; set; } = "";
            public string CompanyName { get; set; } = "";
        }

        
    }
    public class JobGapAnalysis
    {
        public string JobTitle { get; set; } = "";
        public string DepartmentName { get; set; } = "";
        public int DeptID { get; set; }
        public int JobID { get; set; }
        public int CompanyID { get; set; }
        public string CompanyName { get; set; } = "";
        public int TargetCount { get; set; }
        public int ActualCount { get; set; }
        public int Variance { get; set; }
        public string PolicyStatus { get; set; } = "";
    }
}
