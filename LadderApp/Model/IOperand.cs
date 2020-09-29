using System;
using System.Collections.Generic;
using System.Text;

namespace LadderApp
{
    public delegate void ChangedOperandEventHandler(Object sender);
    public interface IOperand
    {
        event ChangedOperandEventHandler ChangedOperandEvent;
    }
}
