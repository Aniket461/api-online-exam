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
    public class ForgotPasswordController : ApiController
    {
        OnlineExamEntities1 db = new OnlineExamEntities1();

        [HttpPost]
        [Route("forgot")]

        public HttpResponseMessage ForgotPassWord(forgotPassword fp)
        {

            Student s = (from cm in db.Students where cm.Email == fp.email select cm).FirstOrDefault();

            if(s!= null)
            {
                var hasher = new HMACSHA256(Encoding.UTF8.GetBytes(fp.email));
                var h = Convert.ToBase64String(hasher.ComputeHash(Encoding.UTF8.GetBytes(fp.email)));

                if(h == fp.passcode)
                {

                    var hasher2 = new HMACSHA256(Encoding.UTF8.GetBytes(fp.newpassword));
                    var h2 = Convert.ToBase64String(hasher2.ComputeHash(Encoding.UTF8.GetBytes(fp.newpassword)));


                    s.Pwd = h2;
                    db.SaveChanges();

                    return Request.CreateResponse(HttpStatusCode.OK, "Match");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "NotMatch");
                }
            }

            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "NotDone");
            }



        }
        
    }
}
