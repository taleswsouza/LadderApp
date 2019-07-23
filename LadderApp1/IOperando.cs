using System;
using System.Collections.Generic;
using System.Text;

namespace LadderApp1
{
    public delegate void MudouOperandoEventHandler(Object sender);
    public interface IOperando
    {
        event MudouOperandoEventHandler MudouOperando;
        //Object RetornaOperando();
    }
}
