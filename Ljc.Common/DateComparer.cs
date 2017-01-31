using System;
using System.Collections.Generic;

namespace Ljc.Common
{
    /// <summary>
    /// DateTime类型日期比较器（只要日期相等就认为二者相等）
    /// </summary>
    public class DateComparer : IEqualityComparer<DateTime>
    {
        public bool Equals(DateTime x, DateTime y)
        {
            return x.Date == y.Date;
        }

        public int GetHashCode(DateTime obj)
        {
            return obj.GetHashCode();
        }
    }
}
