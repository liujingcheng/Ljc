using System;
using System.Collections.Generic;
using System.Linq;
using Ljc.WebApi.Interface;
using Ljc.WebApi.Models;

namespace Ljc.WebApi.Repository
{
    public class TimeStatisticRepository : ITimeStatisticRepository
    {
        public IEnumerable<TimeStatistic> GetAll()
        {
            using (var context = new LjcDbContext())
            {
                // Create database
                context.Database.EnsureCreated();
                return context.Timestatistic.ToList();

            }
        }

        public TimeStatistic Find(string key)
        {
            throw new NotImplementedException();
        }

        public TimeStatistic Remove(string key)
        {
            throw new NotImplementedException();
        }

        public void Update(TimeStatistic item)
        {
            throw new NotImplementedException();
        }

        public void Add(TimeStatistic item)
        {
            using (var context = new LjcDbContext())
            {
                // Create database
                context.Database.EnsureCreated();

            }
        }

    }
}
