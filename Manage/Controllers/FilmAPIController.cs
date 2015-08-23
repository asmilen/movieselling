using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Manage.Controllers
{
    public class FilmAPIController : ApiController
    {
        // GET api/filmapi
        public IEnumerable<string> Get(string text)
        {
            return new string[] { text, "value2" };
        }

        // GET api/filmapi/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/filmapi
        public void Post([FromBody]string value)
        {
        }

        // PUT api/filmapi/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/filmapi/5
        public void Delete(int id)
        {
        }
    }
}
