using System.Collections.Generic;
using Ljc.WebApi.Models;

namespace Ljc.WebApi.Interface
{
    public interface ITimeStatisticRepository
    {
        void Add(TimeStatistic item);
        IEnumerable<TimeStatistic> GetAll();
        TimeStatistic Find(string key);
        TimeStatistic Remove(string key);
        void Update(TimeStatistic item);
    }
}
