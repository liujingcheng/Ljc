using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ljc.WebApi.Interface;
using Ljc.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Ljc.WebApi.Areas.TimeStat.Controllers
{
    //[Route("api/[controller]")]
    public class TimeStatController : Controller
    {
        private ITimeStatisticRepository _repository;

        public TimeStatController(ITimeStatisticRepository repository)
        {
            _repository = repository;
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<TimeStatistic> Get()
        {
            return new List<TimeStatistic>();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [Route("time/anygoing")]
        public bool IsAnyTaskGoing()
        {
            return _repository.IsAnyTaskGoing();
        }
    }
}
