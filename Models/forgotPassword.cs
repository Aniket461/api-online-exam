using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class forgotPassword
    {
        public string email { get; set; }
        public string passcode { get; set; }
        public string newpassword { get; set; }
    }
}