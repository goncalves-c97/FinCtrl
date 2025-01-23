using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebLibrary.Exceptions
{
    internal class UnsuccessfulResponseException : Exception
    {
        public UnsuccessfulResponseException() : base() { }
        public UnsuccessfulResponseException(string message) : base(message) { }
        public UnsuccessfulResponseException(string message, Exception e) : base(message, e) { }
    }
}
