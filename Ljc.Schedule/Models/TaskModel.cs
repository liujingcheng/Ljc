namespace Ljc.Schedule.Models
{
    public class TaskModel
    {
        public string TaskName { get; set; }
        public string PlanSpentDays { get; set; }
        public string PlanStartTime { get; set; }
        public string PlanEndTime { get; set; }
        public string TaskMember { get; set; }
        public string CompleteRatio { get; set; }
        public string Output { get; set; }
        public string Remark { get; set; }
        public string HolidayRemark { get; set; }
    }
}
