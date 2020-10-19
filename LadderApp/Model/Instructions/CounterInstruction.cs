using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadderApp.Model.Instructions
{
    public class CounterInstruction : OutputBoxInstruction
    {
        public CounterInstruction() : base(OperationCode.Counter)
        {
            Operands = new Object[4];
            setBoxType(0);
            setPreset(0);
            setAccumulated(0);
        }
    }
}
