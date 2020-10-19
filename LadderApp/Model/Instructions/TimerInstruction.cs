using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadderApp.Model.Instructions
{
    public class TimerInstruction : OutputBoxInstruction
    {
        public TimerInstruction() : base(OperationCode.Timer)
        {
            Operands = new Object[5];
            setBoxType(0);
            setPreset(0);
            setAccumulated(0);
            SetTimeBase(0);
        }

        public void SetTimeBase(int value)
        {
            SetOperand(4, (int)value);
        }

        public int GetTimeBase()
        {
            return (int)GetOperand(4);
        }
    }
}
