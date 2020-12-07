using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.Models
{
    public class MCQquestions
    {
        public string Que_No { get; set; }
        public string Question { get; set; }
        public string OP1 { get; set; }
        public string OP2 { get; set; }
        public string OP3 { get; set; }
        public string OP4 { get; set; }
        public string Ans { get; set; }
    }
}