using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadderApp.Model.Instructions
{
    public class NormallyOpenContact : FirstOperandAddressDigitalInstruction
    {
        public NormallyOpenContact() : base(OperationCode.NormallyOpenContact)
        {
            Operands = new object[1];
        }

        protected NormallyOpenContact(OperationCode operationCode) : base(operationCode)
        {
        }

        protected override bool IsOperandOk(int index, object value)
        {
            return index == 0 && base.IsOperandOk(index, value);
        }

        protected override bool CheckFirstOperandHasTheCorrectAddressType(Address address)
        {
            if (address.AddressType.Equals(AddressTypeEnum.None))
            {
                return false;
            }
            return true;
        }
    }
}
