using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTMUtil
{
    public class XMLReaderException : Exception
    {
        public XMLReaderException() : base() { }
        public XMLReaderException(string message) : base(message) { }
    }
}
