using LadderApp.Model;
using System;

namespace LadderApp.Model.Instructions
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
