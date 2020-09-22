using System;
using System.Collections.Generic;
using System.Text;

namespace LadderApp
{
    public delegate void MudouOperandoEventHandler(Object sender);
    public interface IOperand
    {
        event MudouOperandoEventHandler MudouOperando;
    }
}
