using System;

namespace LadderApp.Model.Instructions
{
    public abstract class FirstOperandAddressDigitalInstruction : Instruction, IDigitalAddressable
    {
        protected FirstOperandAddressDigitalInstruction(OperationCode operationCode) : base(operationCode)
        {
        }

        public virtual Address GetAddress()
        {
            return (Address)GetOperand(0);
        }

        public override bool GetValue()
        {
            return GetAddress().Value;
        }

        public virtual void SetAddress(Address address)
        {
            SetOperand(0, address);
        }

        public void SetAddressUsed()
        {
            GetAddress().Used = true;
        }

        public void SetValue(bool value)
        {
            GetAddress().Value = value;
        }

        protected abstract bool CheckFirstOperandHasTheCorrectAddressType(Address address);

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
        protected override bool ValidateAddress(int index, Object newOperand)
        {
            return index == 0 && IsOperandOk(index, newOperand);
        }
    }
}