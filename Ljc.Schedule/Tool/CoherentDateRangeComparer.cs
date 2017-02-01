using System;
using System.Collections.Generic;
using Ljc.Schedule.Models;

namespace Ljc.Schedule.Tool
{
    /// <summary>
    /// CoherentDateRange类型日期比较器（起始与截止日期相等就认为二者相等）
    /// </summary>
    public class CoherentDateRangeComparer : IEqualityComparer<CoherentDateRange>
    {
        public bool Equals(CoherentDateRange x, CoherentDateRange y)
        {
            return x.StartDate == y.StartDate && x.EndDate == y.EndDate;
        }

        public int GetHashCode(CoherentDateRange obj)
        {
            if (Object.ReferenceEquals(obj, null)) return 0;

            int hashStartDate = obj.StartDate.GetHashCode();

            int hashEndDate = obj.EndDate.GetHashCode();

            return hashStartDate ^ hashEndDate;
        }
    }
}
