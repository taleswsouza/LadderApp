using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadderApp.Model
{
    public class NormallyOpenContact : OneAddressDigitalInstruction
    {
        public NormallyOpenContact() : base(OperationCode.NormallyOpenContact)
        {
        }

        protected NormallyOpenContact(OperationCode operationCode) : base(operationCode)
        {
        }

        protected override bool CheckOperand(Object operand)
        {
            if (operand is Address address)
            {
                if (address.AddressType.Equals(AddressTypeEnum.None))
                {
                    return false;
                }
                return true;
            }
            return false;
        }
    }
}
