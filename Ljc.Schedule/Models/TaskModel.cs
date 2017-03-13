using System;

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

        /// <summary>
        /// PlanStartTime的时间类型，用于排序
        /// </summary>
        public DateTime? PlanStartTimeDateTime
        {
            get
            {
                DateTime d;
                if (DateTime.TryParse(PlanStartTime, out d))
                {
                    return d;
                }
                return null;
            }
        }
    }
}


