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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public string IsAnyTaskGoing()
        {
            using (var context = LjcDbContextFactory.Create())
            {
                try
                {
                    return
                        context.TimeStatisticDbSet.Any(
                            p => p.Status == "Started" && p.UserId == "a829bdd0186e4324a21f5be3b2c2998d").ToString();
                }
                catch (Exception ex)
                {
                    return "got exception:" + ex.Message + "\r\n" + ex.StackTrace;
                }
            }

        }

    }
}
