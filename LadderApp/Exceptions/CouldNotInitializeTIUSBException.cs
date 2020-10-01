using System;
using System.Collections.Generic;
using System.Text;

namespace LadderApp.Exceções
{
    public class CouldNotInitializeTIUSBException : Exception
    {
        public override string Message
        {
            get
            {
                return "TIUSB port do not found the microcontroller.";
            }
        }
    }
}
