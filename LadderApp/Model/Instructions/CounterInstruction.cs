using System;
using System.Xml.Serialization;

namespace LadderApp.Model.Instructions
{
    public class CounterInstruction : OutputBoxInstruction
    {
        public CounterInstruction() : base(OperationCode.Counter)
        {
            Operands = new Object[1];
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
