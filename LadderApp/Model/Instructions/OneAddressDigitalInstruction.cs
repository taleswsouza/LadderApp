using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadderApp.Model
{
    public abstract class OneAddressDigitalInstruction : Instruction
    {
        protected OneAddressDigitalInstruction(OperationCode operationCode) : base(operationCode)
        {
            Operands = new Object[1];
        }

        public override bool IsAllOperandsOk()
        {
            return CheckOperand(Operands[0]);
        }

        protected abstract bool CheckOperand(Object operand);

        protected override bool ValidateAddress(int index, Object newOperand)
        {
            return index == 0 && CheckOperand(newOperand);
        }

        public override bool GetValue()
        {
            return GetAddress().Value;
        }
    }
}
