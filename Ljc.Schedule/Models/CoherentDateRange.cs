using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ljc.Schedule.Models
{
    /// <summary>
    /// 连续的日期范围(日期段)
    /// </summary>
    public class CoherentDateRange
    {
        /// <summary>
        /// 起始日期
        /// </summary>
        public DateTime StartDate { get; }

        /// <summary>
        /// 截止日期
        /// </summary>
        public DateTime EndDate { get; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public int TotalDays
        {
            get { return (EndDate - StartDate).Days + 1; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="startDate">起始日期</param>
        /// <param name="endDate">截止日期</param>
        /// <param name="remark">备注</param>
        public CoherentDateRange(DateTime startDate, DateTime endDate, string remark = null)
        {
            StartDate = startDate.Date;
            EndDate = endDate.Date;
            Remark = remark;

            if (StartDate > EndDate)
            {
                var tempDate = StartDate;
                StartDate = EndDate;
                EndDate = tempDate;
            }

            if (StartDate.DayOfWeek == DayOfWeek.Saturday || StartDate.DayOfWeek == DayOfWeek.Sunday)
            {
                Remark += "周末";
            }

        }
    }
}
