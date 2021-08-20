using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Dto
{
    public class QuestionModelRequest
    {
        public string Question { get; set; }

        public string[] WrongAnswers { get; set; }

        public string CorrectAnswer { get; set; }
    }
}
