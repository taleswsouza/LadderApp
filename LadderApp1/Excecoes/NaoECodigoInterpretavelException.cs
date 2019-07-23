using System;
using System.Collections.Generic;
using System.Text;

namespace LadderApp1.Exceções
{
    public class NaoECodigoInterpretavelException : Exception
    {
        public override string Message
        {
            get
            {
                //return base.Message;
                return "Não é um CodigosInterpretaveis";
            }
        }
    }
}
