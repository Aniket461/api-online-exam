using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using OnlineExam.Models;
namespace OnlineExam.Controllers
{
    public class EmailController : ApiController
    {

        [HttpGet]
        [Route("getemail")]
        public HttpResponseMessage GetEmail(string email)
        {
            try
            {
                var hasher = new HMACSHA256(Encoding.UTF8.GetBytes(email));
                var h = Convert.ToBase64String(hasher.ComputeHash(Encoding.UTF8.GetBytes(email)));
                //return Request.CreateResponse(HttpStatusCode.OK, h);

                Email e = new Email() { message = h, subject = "Password Change", toemail = email, toname = "Mark" };
                var client = new HttpClient();
                var posttalk = client.PostAsJsonAsync<Email>("https://localhost:44399/sendemail", e);
                posttalk.Wait();

                var result = posttalk.Result;
                if (result.IsSuccessStatusCode)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Email Sent");
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

        [HttpPost]
        [Route("sendemail")]
        public async Task SendEmail(Email e)
        {
            try
            {
                var message = new MailMessage();
                message.To.Add(new MailAddress(e.toname + " <" + e.toemail + ">"));
                message.From = new MailAddress("Surve Aniket <ltiranj461@gmail.com>");
                message.Bcc.Add(new MailAddress("Surve Aniket <ltiranj461@gmail.com>"));
                message.Subject = e.subject;
                message.Body = createEmailBody(e.toname, e.message);
                message.IsBodyHtml = true;
                using (var smtp = new SmtpClient())
                {
                    await smtp.SendMailAsync(message);
                    await Task.FromResult(0);
                }
            }
            catch(Exception ec)
            {
                Console.WriteLine(ec);
            }
        }

        private string createEmailBody(string userName, string message)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("/template.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{username}", userName);
            body = body.Replace("{message}", message);
            return body;
        }
    }
}
