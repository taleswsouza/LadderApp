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
                //return base.Message;
                //return "Could not initialize the library (port: TIUSB)";
                return "Porta TIUSB não encontrou o microcontrolador.";
            }
        }
    }
}
