using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OnlineExam.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Http.Description;
using Microsoft.VisualBasic.FileIO;
using System.Text;

namespace OnlineExam.Controllers
{
    public class QuestionController : ApiController
    {
        static bool a;
        OnlineExamEntities1 oe = new OnlineExamEntities1();


        #region getallfiles
        // GET: api/Question
        [Route("GetAllFiles")]
        public IEnumerable<Object> Get()
        {
            var f = from q in oe.Questions where q.Del == 1 select new { q.FileName, q.FileID };
            return f;
        }

        #endregion


        // GET: api/Question/5
        [ResponseType(typeof(Question))]
        public IHttpActionResult Get(int id)
        {
            Question question = oe.Questions.Find(id);
            if (question == null)
            {
                return NotFound();
            }
            return Ok(question);
        }


        
        // POST: api/Question
        [Route("NewFile")]

        public void Post([FromBody] Question question)
        {
            oe.Questions.Add(question);
            oe.SaveChanges();
        }

        // PUT: api/Question/5
        public void Put(int id, [FromBody] Question question)
        {
            Question q = oe.Questions.Find(id);
            q.FileName = question.FileName;
            q.ExamID = question.ExamID;
            q.Level = question.Level;
            oe.SaveChanges();

        }


        #region RemoveFileApi
        // DELETE: api/Question/5
        [Route("RemoveFile")]
        [ResponseType(typeof(Question))]
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                Question question = oe.Questions.Where(q => q.FileID == id).FirstOrDefault();
                question.Del = 0;
                oe.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Successful Deletion");
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Unsuccessful");
            }
        }


        #endregion


        #region ReadingfilesforQuestionPage
        //
        [AcceptVerbs("GET", "POST")]
        [Route("Read/{id}")]

        public List<MCQquestions> ReadQuestionFile(int id)
        {

            List<MCQquestions> mcq = new List<MCQquestions>();

            try
            {

                List<string[]> data = new List<string[]>();
                string path = (from q in oe.Questions
                               where q.FileID == id
                               select q.FileName
                                   ).FirstOrDefault();
                TextFieldParser parser = new TextFieldParser(path, Encoding.Default);
                parser.HasFieldsEnclosedInQuotes = true;
                parser.SetDelimiters(",");
                string[] fields;
                while (!parser.EndOfData)
                {
                    fields = parser.ReadFields();
                    if (fields[0] == "")
                        continue;
                    data.Add(fields);
                }
                parser.Close();
                int[] arr = new int[data.Count];           //for Random Questions
                arr[0] = 0;
                Random rand = new Random();
                for (int i = 1; i <= 10; i++)
                {
                    int number;
                    do
                    {
                        number = rand.Next(1, data.Count);
                    } while (arr.Contains(number));
                    arr[i] = number;
                }
                int k = 0;
                mcq.Add(new MCQquestions
                {
                    Que_No = data[0][0],
                    Question = data[0][1],
                    OP1 = data[0][2],
                    OP2 = data[0][3],
                    OP3 = data[0][4],
                    OP4 = data[0][5],
                    Ans = data[0][6]
                });

                for (int i = 1; i <= 10; i++)
                {
                    foreach (var rows in data)
                    {
                        if (rows[0] == arr[i].ToString())
                        {
                            mcq.Add(new MCQquestions
                            {
                                Que_No = i.ToString(),
                                Question = rows[1],
                                OP1 = rows[2],
                                OP2 = rows[3],
                                OP3 = rows[4],
                                OP4 = rows[5],
                                Ans = rows[6]
                            });
                            break;
                        }
                    }
                }
                //IEnumerable<MCQquestions> m = mcq as IEnumerable<MCQquestions>;
                return mcq;
            }

            catch(Exception e)
            {
                return mcq;
            }
        }
        #endregion

        //

        #region posttoexistingexam
        [HttpPost]
        [Route("try")]
        public IHttpActionResult Post2(Question q)
        {
            try
            {
                List<int> res = (from q1 in oe.Questions where q1.ExamID == q.ExamID select q1.Level).ToList();
                foreach (int i in res)
                {
                    if (i == q.Level)
                    {
                        a = true;
                        break;

                    }

                }
                if (a == true)
                {
                    Question question = (from q1 in oe.Questions where q1.ExamID == q.ExamID & q1.Level == q.Level select q1).Single();
                    question.FileName = q.FileName;
                    question.Del = q.Del;
                    oe.SaveChanges();
                    return Ok("update");
                }
                else
                {
                    oe.Questions.Add(q);
                    oe.SaveChanges();
                    return Ok("new");
                }
            }
            catch(Exception e)
            {
                return Ok(e);
            }

        }


        #endregion



        #region PosttoNewExam

        [HttpPost]
        [Route("try2")]
        public IHttpActionResult Post2(Post2 p)
        {
            try
            {
                Exam exam = new Exam();
                exam.ExamName = p.ExamName;
                exam.CompanyID = (from c in oe.Companies where c.CompanyName == p.CompanyName select c.CompanyID).Single();
                exam.Subject = p.Subject;
                oe.Exams.Add(exam);
                oe.SaveChanges();
                Question question = new Question();
                question.FileName = p.FileName;
                question.ExamID = (from e in oe.Exams where e.ExamName == p.ExamName select e.ExamID).Single();
                question.Level = p.Level;
                question.Del = p.Del;
                oe.Questions.Add(question);
                oe.SaveChanges();
                return Ok(question.ExamID);
            }
            catch
            {
                return Ok("error");
            }
        }

        #endregion
        //

        #region GetFileID

        [HttpGet]
        [Route("getfileid")]
        public HttpResponseMessage getfileids(int id)
        {

            var d = from cm in oe.Questions where cm.ExamID == id && cm.Del == 1 select cm;

            if(d.Count() == 3)
            {

                return Request.CreateResponse(HttpStatusCode.OK, d);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, "Notthree");
            }


        }

        #endregion


    }
}