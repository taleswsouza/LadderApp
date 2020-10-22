using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadderApp.Model.Instructions
{
    public class OutputCoil : FirstOperandAddressDigitalInstruction, IOutput
    {
        public OutputCoil() : base(OperationCode.OutputCoil)
        {
            Operands = new object[1];
        }

        protected override bool IsOperandOk(int index, object value)
        {
            return index == 0 && base.IsOperandOk(index, value);
        }

        protected override bool CheckFirstOperandHasTheCorrectAddressType(Address address)
        {
            if (address.AddressType.Equals(AddressTypeEnum.DigitalMemory)
                || address.AddressType.Equals(AddressTypeEnum.DigitalOutput))
            {
                return true;
            }
            return false;
        }

        public string GetOutputDeclaration()
        {
            return GetAddress().GetBitVariableName();
        }

    }
}
