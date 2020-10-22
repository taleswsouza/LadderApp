using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadderApp.Model.Instructions
{
    class LineBeginInstruction : Instruction
    {
        public LineBeginInstruction() : base(OperationCode.LineBegin)
        {
            Operands = new Object[1];
            SetOperand(0, 0);
        }

        protected override bool IsOperandOk(int index, object value)
        {
            if (index != 0 || !base.IsOperandOk(index, value))
            {
                return false;
            }
            if (value is int lineNumber)
            {
                return lineNumber >= 0;
            }
            return false;
        }
    }
}
