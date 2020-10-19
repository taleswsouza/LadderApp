using System;

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

        protected override bool CheckFirstOperandHasTheCorrectAddressType(Address address)
        {
            if (address.AddressType.Equals(AddressTypeEnum.DigitalMemoryCounter))
            {
                return true;
            }
            return false;
        }
    }
}
