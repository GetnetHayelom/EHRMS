namespace PIS2.Enums
{
    public class TrainingEnums
    {
    }
    public enum trainingCategory
    {
        Technical,
        HR,
        Safety,
        Compliance,
        Management,
        Quality,
        Other
    }

    public enum trainingDeliveryMode
    {
        Online,
        Onsite,
        Hybrid
    }

    public enum trainingStatus
    {
        Planned,
        Ongoing,
        Completed,
        Cancelled
    }

    public enum trainingResult
    {
        InProgress,
        Passed,
        Failed,
        Absent
    }
}
