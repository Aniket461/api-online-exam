using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using OnlineExam.Models;
namespace OnlineExam.Controllers
{
    public class StudentController : ApiController
    {
        OnlineExamEntities1 db = new OnlineExamEntities1();


        [Route("student")]
        //GET
        public IQueryable<Student> GetStudents()
        {
            return db.Students;
        }

        //GET by id
        [Route("student/{id}")]
        public IHttpActionResult GetStudent(int id)
        {
            Student st = db.Students.Find(id);

            return Ok(st);
        }
        [Route("student/delete/{id}")]
        public HttpResponseMessage Delete(int id)
        {
            Student st = db.Students.Find(id);

            if(st != null)
            {
                db.Students.Remove(st);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK,db.Students);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "No Student with this ID exist");
            }
        }

        [HttpPost]
        [Route("student/post")]
        public HttpResponseMessage Post(Student s)

        {
            try
            {
                if (s != null)
                {
                var hasher = new HMACSHA256(Encoding.UTF8.GetBytes(s.Pwd));
                var h = Convert.ToBase64String(hasher.ComputeHash(Encoding.UTF8.GetBytes(s.Pwd)));
                s.Pwd = h;
                    db.Students.Add(s);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, "succesfully Posted");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Error");
                }
            }
            catch(Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "Error");
            }
        }

        [Route("student/{id}")]
        //put for password change
        public HttpResponseMessage Put(int id, [FromBody] Student c)
        {
            try
            {

                Student st = db.Students.Find(id);
                if (st != null)
                {
                    var hasher = new HMACSHA256(Encoding.UTF8.GetBytes(c.Pwd));
                    var h = Convert.ToBase64String(hasher.ComputeHash(Encoding.UTF8.GetBytes(c.Pwd)));

                    st.Pwd = h;
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, "Password Updated succesfully");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "No such student exists");
                }
            }
            catch(Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "Error");
            }

        }


        
        [HttpPost]
        [Route("student/login")]
        public HttpResponseMessage login(Login l)
        {

            try
            {
                var hasher = new HMACSHA256(Encoding.UTF8.GetBytes(l.password));
                var h = Convert.ToBase64String(hasher.ComputeHash(Encoding.UTF8.GetBytes(l.password)));

                Student s = (from cm in db.Students where cm.Email == l.email && cm.Pwd == h select cm).FirstOrDefault();

                if (s != null)
                {

                    return Request.CreateResponse(HttpStatusCode.OK, s);
                }

                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "NoLogin");
                }

            }
            catch(Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "Error");
            }
        }


        [Route("getuniques")]
        public IEnumerable<string> Get(string state)
        {
            return (from s in db.Students where s.State == state select s.City).Distinct();
        }

        //
        [HttpGet]
        [Route("reportquery1")]
        public IEnumerable<Object> Get2(string cname, string ename, int level, int marks)
        {
            if (level == 1)
            {
                return from c in db.Companies
                       join e in db.Exams on c.CompanyID equals e.CompanyID
                       join r in db.Reports on e.ExamID equals r.ExamID
                       join s in db.Students on r.StudentID equals s.StudentID

                       where c.CompanyName == cname & e.ExamName == ename & r.Level1_Score >= marks
                       select new
                       {
                           ID = r.StudentID,
                           score1 = r.Level1_Score,
                           sname = s.StudentName,
                           scolg = s.College,
                           squal = s.Qualification,
                           syoc = s.Year_of_Completion

                       };
            }
            else if (level == 2)
            {
                return from c in db.Companies
                       join e in db.Exams on c.CompanyID equals e.CompanyID
                       join r in db.Reports on e.ExamID equals r.ExamID
                       join s in db.Students on r.StudentID equals s.StudentID
                       where c.CompanyName == cname & e.ExamName == ename & r.Level2_Score >= marks
                       select new
                       {
                           ID = r.StudentID,
                           score1 = r.Level1_Score,
                           score2 = r.Level2_Score,
                           sname = s.StudentName,
                           scolg = s.College,
                           squal = s.Qualification,
                           syoc = s.Year_of_Completion
                       };
            }
            else
            {
                return from c in db.Companies
                       join e in db.Exams on c.CompanyID equals e.CompanyID
                       join r in db.Reports on e.ExamID equals r.ExamID
                       join s in db.Students on r.StudentID equals s.StudentID
                       where c.CompanyName == cname & e.ExamName == ename & r.Level3_Score >= marks
                       select new
                       {
                           ID = r.StudentID,
                           score1 = r.Level1_Score,
                           score2 = r.Level2_Score,
                           score3 = r.Level3_Score,
                           sname = s.StudentName,
                           scolg = s.College,
                           squal = s.Qualification,
                           syoc = s.Year_of_Completion
                       };
            }


        }
        [HttpGet]
        [Route("reportquery2")]
        public IEnumerable<Object> Get3(string tname, int level, int marks)
        {
            if (level == 1)
            {
                return from c in db.Companies
                       join e in db.Exams on c.CompanyID equals e.CompanyID
                       join r in db.Reports on e.ExamID equals r.ExamID
                       join s in db.Students on r.StudentID equals s.StudentID

                       where e.Subject == tname & r.Level1_Score >= marks
                       select new
                       {
                           ID = r.StudentID,
                           score1 = r.Level1_Score,
                           sname = s.StudentName,
                           scolg = s.College,
                           squal = s.Qualification,
                           syoc = s.Year_of_Completion
                       };
            }
            else if (level == 2)
            {
                return from c in db.Companies
                       join e in db.Exams on c.CompanyID equals e.CompanyID
                       join r in db.Reports on e.ExamID equals r.ExamID
                       join s in db.Students on r.StudentID equals s.StudentID
                       where e.Subject == tname & r.Level2_Score >= marks
                       select new
                       {
                           ID = r.StudentID,
                           score1 = r.Level1_Score,
                           score2 = r.Level2_Score,
                           sname = s.StudentName,
                           scolg = s.College,
                           squal = s.Qualification,
                           syoc = s.Year_of_Completion
                       };
            }
            else
            {
                return from c in db.Companies
                       join e in db.Exams on c.CompanyID equals e.CompanyID
                       join r in db.Reports on e.ExamID equals r.ExamID
                       join s in db.Students on r.StudentID equals s.StudentID
                       where e.Subject == tname & r.Level3_Score >= marks
                       select new
                       {
                           ID = r.StudentID,
                           score1 = r.Level1_Score,
                           score2 = r.Level2_Score,
                           score3 = r.Level3_Score,
                           sname = s.StudentName,
                           scolg = s.College,
                           squal = s.Qualification,
                           syoc = s.Year_of_Completion
                       };
            }


        }
        [HttpGet]
        [Route("reportquery3")]
        public IEnumerable<Object> Get4(string state, string city, int level, int marks)
        {
            if (level == 1)
            {
                return from c in db.Companies
                       join e in db.Exams on c.CompanyID equals e.CompanyID
                       join r in db.Reports on e.ExamID equals r.ExamID
                       join s in db.Students on r.StudentID equals s.StudentID

                       where s.City == city & s.State == state & r.Level1_Score >= marks
                       select new
                       {
                           ID = r.StudentID,
                           score1 = r.Level1_Score,
                           sname = s.StudentName,
                           scolg = s.College,
                           squal = s.Qualification,
                           syoc = s.Year_of_Completion
                       };
            }
            else if (level == 2)
            {
                return from c in db.Companies
                       join e in db.Exams on c.CompanyID equals e.CompanyID
                       join r in db.Reports on e.ExamID equals r.ExamID
                       join s in db.Students on r.StudentID equals s.StudentID
                       where s.City == city & s.State == state & r.Level2_Score >= marks
                       select new
                       {
                           ID = r.StudentID,
                           score1 = r.Level1_Score,
                           score2 = r.Level2_Score,
                           sname = s.StudentName,
                           scolg = s.College,
                           squal = s.Qualification,
                           syoc = s.Year_of_Completion
                       };
            }
            else
            {
                return from c in db.Companies
                       join e in db.Exams on c.CompanyID equals e.CompanyID
                       join r in db.Reports on e.ExamID equals r.ExamID
                       join s in db.Students on r.StudentID equals s.StudentID
                       where s.City == city & s.State == state & r.Level3_Score >= marks
                       select new
                       {
                           ID = r.StudentID,
                           score1 = r.Level1_Score,
                           score2 = r.Level2_Score,
                           score3 = r.Level3_Score,
                           sname = s.StudentName,
                           scolg = s.College,
                           squal = s.Qualification,
                           syoc = s.Year_of_Completion
                       };
            }


        }


    }

}
