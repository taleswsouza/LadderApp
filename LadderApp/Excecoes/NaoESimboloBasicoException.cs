using System;
using System.Collections.Generic;
using System.Text;

namespace LadderApp.Exceções
{
    public class NaoESimboloBasicoException : Exception
    {
        public override string Message
        {
            get
            {
                return "Não é um SimboloBasico";
            }
        }
    }
}
