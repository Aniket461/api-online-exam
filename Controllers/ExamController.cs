using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OnlineExam.Models;

namespace OnlineExam.Controllers
{
    public class ExamController : ApiController
    {
        OnlineExamEntities1 oe = new OnlineExamEntities1();
        // GET: api/Exam
        public IEnumerable<Exam> Get()
        {
            return oe.Exams;
        }

        // GET: api/Exam/5
        public HttpResponseMessage Get(int id)
        {
            Exam exam = oe.Exams.Find(id);
            if (exam == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "No Data Found");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, exam);
            }
        }

        // POST: api/Exam
        public void Post([FromBody] Exam value)
        {
            oe.Exams.Add(value);
            oe.SaveChanges();
        }

        // PUT: api/Exam/5
        public void Put(int id, [FromBody] Exam value)
        {
            Exam e = oe.Exams.Find(id);
            e.ExamID = value.ExamID;
            e.CompanyID = value.CompanyID;
            e.ExamName = value.ExamName;
            e.Subject = value.Subject;
            oe.SaveChanges();
        }

        // DELETE: api/Exam/5
        public HttpResponseMessage Delete(int id)
        {
            Exam exam = oe.Exams.Where(e => e.ExamID == id).FirstOrDefault();
            if (exam == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "No Data Found");
            }
            else
            {
                oe.Exams.Remove(exam);
                oe.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, exam);
            }

        }


        [Route("Select")]
        public IQueryable<Object> GetByCompany(string id)
        {
            var result = (from e in oe.Exams
                          join c in oe.Companies on e.CompanyID equals c.CompanyID
                          where c.CompanyName == id
                          select new
                          {
                              CompanyID = e.CompanyID,
                              CompanyName = c.CompanyName,
                              ExamID = e.ExamID,
                              ExamName = e.ExamName,
                              Subject = e.Subject
                          }

                    );
            return result;
        }

        //
        [Route("GetUniqueT")]
        [HttpGet]
        public IEnumerable<string> Get1()
        {
            var t = oe.Exams.Select(e => e.Subject).Distinct();
            return t;

        }

        //

        [Route("getuniquee")]
        [HttpGet]
        public IEnumerable<String> Get1(string name)
        {
            var e1 = from e in oe.Exams join c in oe.Companies on e.CompanyID equals c.CompanyID where c.CompanyName == name select e.ExamName;
            return e1;
        }


        //
        [Route("getuniqueeid")]
        [HttpGet]
        public IEnumerable<Object> Get2(string name)
        {
            var e1 = from e in oe.Exams join c in oe.Companies on e.CompanyID equals c.CompanyID where c.CompanyName == name select new { e.ExamName, e.ExamID };
            return e1;
        }

        //


    }

}
