using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Views
{
    public class EvaluationViews
    {
    }

    /// <summary>
    /// Evaluation Summary View
    /// </summary>
    // Primary Evaluation Data
    public class EvaluationSummaryView
    {
        public int evaluationID { get; set; }
        public string evaluationName { get; set; }
        public int evaluationStatus { get; set; } // Matches your WHERE filter
        public DateTime evaluationStartDate { get; set; }
        public DateTime evaluationEndDate { get; set; }

        // Employee Data (Using the CONCAT_WS alias)
        public int employmentID { get; set; }
        public string personFullName { get; set; }
        public int? jobPlacementID { get; set; }
        // Type Data
        public int evaluationTypeID { get; set; }
        public string evaluationTypeName { get; set; }
        [Precision(18, 2)]
        public decimal evaluationTypeWeight { get; set; }
        public bool isFixed { get; set; }

        // Task Data
        public int evaluationTaskID { get; set; }
        public string evaluationTaskName { get; set; }
        [Precision(18, 2)]
        public decimal evaluationTaskWeight { get; set; }

        // SubTask Data
        public int evaluationSubTaskID { get; set; }
        public string evaluationSubTaskName { get; set; }
        [Precision(18, 2)]
        public decimal evaluationSubTaskWeight { get; set; }

        // Valuation Metrics
        [Precision(18, 2)]
        public decimal timeValuation { get; set; }
        [Precision(18, 2)]
        public decimal resourceValuation { get; set; }
        [Precision(18, 2)]
        public decimal performanceValuation { get; set; }

        // Calculated Columns from SQL View
        [Precision(18, 2)]
        public decimal SubTaskAvgScore { get; set; }
        [Precision(18, 2)]
        public decimal WeightedSubTaskScore { get; set; }

    }

    // The top-level object for the report
    public class EvalSingleEmployeeReport
    {
        public int evaluationID { get; set; }
        public string evaluationName { get; set; }
        public string personFullName { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        [Precision(18, 2)]
        public decimal FinalGrandTotal { get; set; }
        [Precision(18, 2)]
        public decimal? PreviousGrandTotal { get; set; }

        // Grouped by Evaluation Type
        public List<EvalTypeSummary> Types { get; set; } = new();
    }

    public class EvalTypeSummary
    {
        public string typeName { get; set; }
        public decimal typeWeight { get; set; }
        // Actual score earned for this type

        // Summarized by Task
        public List<EvalTaskSummary> Tasks { get; set; } = new();
        public decimal typeContribution
        {
            get
            {
                if (Tasks.Sum(t => t.taskWeight) == 0) return 0;
                return Tasks.Sum(t => t.taskScore) * typeWeight / Tasks.Sum(t => t.taskWeight);
            }
        }
    }

    public class EvalTaskSummary
    {
        public string taskName { get; set; }
        [Precision(18, 2)]
        public decimal taskWeight { get; set; }
        [Precision(18, 2)]
        public decimal avgTime { get; set; }
        [Precision(18, 2)]
        public decimal avgResource { get; set; }
        [Precision(18, 2)]
        public decimal avgPerformance { get; set; }
        public List<EvalSubTaskSummary> SubTasks { get; set; } = new();
        public decimal taskScore
        {
            get
            {
                decimal totalWeight = SubTasks.Sum(s => s.subtaskWeight) * 4;
                if (totalWeight == 0) return 0; // Prevent DivideByZeroException

                decimal scoreSum = SubTasks.Sum(s => s.subtaskScore);
                return (taskWeight * scoreSum) / totalWeight;
            }
        }

    }

    public class EvalSubTaskSummary
    {
        public string subtaskName { get; set; }
        [Precision(18, 2)]
        public decimal subtaskWeight { get; set; }
        [Precision(18, 2)]
        public decimal subTime { get; set; }
        [Precision(18, 2)]
        public decimal subResource { get; set; }
        [Precision(18, 2)]
        public decimal subPerformance { get; set; }
        [Precision(18, 2)]
        public decimal subtaskScore
        {
            get
            {
                return ((subTime + subResource + subPerformance) / 3) * subtaskWeight;
            }
        }
    }

    public class EvalGrandView
    {
        public int employmentID { get; set; }
        public string givenID { get; set; }
        public mainStatus employmentStatus { get; set; }
        public int evaluationID { get; set; }
        public string evaluationName { get; set; }
        public string personFullName { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public evaluationStatus evaluationStatus { get; set; }
        public int? jobPlacementID { get; set; }
        public int? departmentID { get; set; }
        public string? departmentName { get; set; }
        public int? companyID { get; set; }
        public string? companyName { get; set; }
        public int evaluationTypeID { get; set; }
        public string evaluationTypeName { get; set; }
        [Precision(18, 2)]
        public decimal TaskScoreSum { get; set; }
        [Precision(18, 2)]
        public decimal TypeScore { get; set; }
        [Precision(18, 2)]
        public decimal FinalScore { get; set; }
    }
}
