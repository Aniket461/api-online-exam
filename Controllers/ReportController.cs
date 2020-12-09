using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OnlineExam.Models;
using System.Data.Entity;
using System.Globalization;
using System.Data.Entity.Core.Objects;


namespace OnlineExam.Controllers
{
    public class ReportController : ApiController
    {

        OnlineExamEntities1 oe1 = new OnlineExamEntities1();


        #region Dashboard

        [Route("dashboard")]
        [HttpGet]
        public IQueryable<Object> Dashboard(int sid)
        {
            
            var report = from r in oe1.Reports
                         join e in oe1.Exams on r.ExamID equals e.ExamID
                         join c in oe1.Companies on e.CompanyID equals c.CompanyID
                         where r.StudentID == sid
                         select new
                         {
                             ExamID = r.ExamID,
                             CompanyName = c.CompanyName,
                             ExamName = e.ExamName,
                             Subject = e.Subject,
                             Level1_Score = r.Level1_Score,
                             Level2_Score = r.Level2_Score,
                             Level3_Score = r.Level3_Score,
                             Date = r.Date
        };

       // DateTime dt = DateTime.ParseExact(report[7] ,"MM/dd/yyyy T hh:mm:ss", CultureInfo.InvariantCulture);


            if (report == null)
            {
                return null;
            }
            return report;
        }
        [Route("CheckReport")]
        [AcceptVerbs("GET", "POST")]
        public HttpResponseMessage CheckReport(int sid, int eid)
        {
            var str = "";
            DateTime date = DateTime.Now.AddDays(-180);
            try
            {
                var report = (from n in oe1.Reports
                              where n.StudentID == sid && n.ExamID == eid
                              select new { Date = n.Date }).OrderByDescending(t => t.Date).FirstOrDefault();
                TimeSpan comp = new TimeSpan(180, 0, 0, 0);

                if (report == null) { 
                
                    return Request.CreateResponse(HttpStatusCode.OK, new { Message = "T" });

                }
                else
                {

                    TimeSpan diff = (TimeSpan)(date.Date - report.Date);
                    if (diff < comp)
                    {
                        str = "F";
                        return Request.CreateResponse(HttpStatusCode.OK, new { Message = "F" });
                    }
                    str = "T";
                    return Request.CreateResponse(HttpStatusCode.OK, new { Message = "T" });
                }
            }

            catch (Exception e)
            {
                str = "T";
                return Request.CreateResponse(HttpStatusCode.OK, new { Message = "Error" });
            }

        }


        #endregion

        //


        #region GetlevelwiseReport

        [Route("GetLevelReport")]
        public Object GetReport(int sid, int eid, int lid)
        {
            dynamic report = null;
            try
            {
                if (lid == 1)
                {
                    report = (from n in oe1.Reports
                              where n.StudentID == sid && n.ExamID == eid
                              select new { Level_Score = n.Level1_Score, Date = n.Date }).OrderByDescending(t => t.Date).FirstOrDefault();
                }
                if (lid == 2)
                {
                    report = (from n in oe1.Reports
                              where n.StudentID == sid && n.ExamID == eid
                              select new { Level_Score = n.Level2_Score, Date = n.Date }).OrderByDescending(t => t.Date).FirstOrDefault();
                }
                if (lid == 3)
                {
                    report = (from n in oe1.Reports
                              where n.StudentID == sid && n.ExamID == eid
                              select new { Level_Score = n.Level3_Score, Date = n.Date }).OrderByDescending(t => t.Date).FirstOrDefault();
                }


                return report;
            }
            catch (NullReferenceException e)
            {
                return "Not Appeared";
            }
        }

        #endregion
        //



        #region getFinalReport

        [Route("GetReport")]
        [HttpGet]
        public Object GetReport(int sid, int eid)
        {
            DateTime date = DateTime.Now.AddDays(-1);
            //var report = from r in oe1.Reports
            //         where r.StudentID == sid && r.ExamID == eid && r.Date>date
            //         select r;
            try
            {
                var report = (from n in oe1.Reports
                              join e in oe1.Exams on n.ExamID equals e.ExamID
                              join c in oe1.Companies on e.CompanyID equals c.CompanyID
                              where n.StudentID == sid && n.ExamID == eid
                              select new
                              {
                                  ReportID = n.ReportID,
                                  ExamID = n.ExamID,
                                  StudentID = n.StudentID,
                                  Level1_Score = n.Level1_Score,
                                  Level2_Score = n.Level2_Score,
                                  Level3_Score = n.Level3_Score,
                                  Date = n.Date,
                                  CompanyName = c.CompanyName
                              }

                              ).OrderByDescending(t => t.Date).FirstOrDefault();
                return report;
            }

            catch (NullReferenceException e)
            {
                return e;
            }

        }

        #endregion




        #region PostReports
        [HttpPost]
        [Route("postreport")]

        public HttpResponseMessage Postreport(Report r)
        {
            try
            {
                if (r != null) {

                    oe1.Reports.Add(r);
                    oe1.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, "Done");
                
                }
                else
                {
                   return Request.CreateResponse(HttpStatusCode.OK, "NotDone");
                }

               }
            catch(Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "Error");
            }
        }

        #endregion

    }
}
