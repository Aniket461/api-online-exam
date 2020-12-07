using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OnlineExam.Models;
namespace OnlineExam.Controllers
{
    public class CompanyController : ApiController
    {

        OnlineExamEntities1 db = new OnlineExamEntities1();

        [Route("GetCompany")]
        public HttpResponseMessage GetCompany()
        {
            var com = db.Companies.ToList();
            if (com.Count > 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, com);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "No data found");
            }
        }

        //
        [Route("GetUniqueC")]
        public IEnumerable<string> Get2()
        {
            var com1 = db.Companies.Select(c => c.CompanyName).Distinct();
            return com1;

        }

        [Route("addcompany")]
        public IHttpActionResult Post(Company c)
        {

            db.Companies.Add(c);
            db.SaveChanges();
            return Ok(c);
        }

        [Route("GetUniqueCompany")]
        public IEnumerable<object> Get3()
        {
            var comp = from c in db.Companies select new { c.CompanyName, c.State, c.City };
            return comp;

        }
    }
}
