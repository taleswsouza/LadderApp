using System;
using System.Collections.Generic;
using System.Text;

namespace LadderApp.Exceções
{
    public class NaoECodigoInterpretavelException : Exception
    {
        public override string Message
        {
            get
            {
                return "Não é um CodigosInterpretaveis";
            }
        }
    }
}
