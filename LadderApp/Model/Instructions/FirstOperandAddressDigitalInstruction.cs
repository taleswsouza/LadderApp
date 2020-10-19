using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadderApp.Model.Instructions
{
    public abstract class FirstOperandAddressDigitalInstruction : Instruction, IAddressable
    {
        protected FirstOperandAddressDigitalInstruction(OperationCode operationCode) : base(operationCode)
        {
        }

        protected override bool IsOperandOk(int index, object value)
        {
            if (!base.IsOperandOk(index, value))
            {
                return false;
            }
            if (index == 0 && value is Address address)
            {
                return CheckFirstOperandHasTheCorrectAddressType(address);
            }
            return true;
        }
        protected abstract bool CheckFirstOperandHasTheCorrectAddressType(Address address);

        protected override bool ValidateAddress(int index, Object newOperand)
        {
            return index == 0 && IsOperandOk(index, newOperand);
        }

        public Address GetAddress()
        {
            return (Address)GetOperand(0);
        }

        public override bool GetValue()
        {
            return GetAddress().Value;
        }

        public void SetUsed()
        {
            GetAddress().Used = true;
        }

    }
}
