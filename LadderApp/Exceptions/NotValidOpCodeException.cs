using System;
using System.Collections.Generic;
using System.Text;

namespace LadderApp.Exceções
{
    public class NotValidOpCodeException : Exception
    {
        public override string Message
        {
            get
            {
                return "It's not an valid OpCode!";
            }
        }
    }
}
