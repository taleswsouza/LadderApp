using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadderApp.Model
{
    public class OutputCoil : OneAddressDigitalInstruction, IOutput
    {
        public OutputCoil() : base(OperationCode.OutputCoil)
        {
        }

        protected override bool CheckOperand(Object operand)
        {
            if (operand is Address address)
            {
                if (!(address.AddressType.Equals(AddressTypeEnum.DigitalMemory)
                    || address.AddressType.Equals(AddressTypeEnum.DigitalOutput)))
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public string GetOutputDeclaration()
        {
            return GetAddress().GetVariableBitValueName();
        }

    }
}
