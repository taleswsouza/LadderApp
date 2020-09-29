using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace LadderApp
{
    public interface IInstruction
    {
        OperationCode OpCode { get; set; }
        Object[] Operands { get; set; }
        int GetNumberOfOperands();
        bool IsAllOperandsOk();
        Object GetOperand(int position);
        void SetOperand(int position, Object value);
    }
}
