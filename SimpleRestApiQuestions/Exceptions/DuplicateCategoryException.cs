using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleRestApiQuestions.Exceptions
{
    public class DuplicateCategoryException : Exception
    {
        public DuplicateCategoryException()
        {
        }

        public DuplicateCategoryException(string message) : base(message)
        {
        }

        public DuplicateCategoryException(string message, Exception inner) : base(message, inner)
        {
        }   
    }
}
