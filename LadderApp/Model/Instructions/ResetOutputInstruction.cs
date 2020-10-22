using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadderApp.Model.Instructions
{
    public class ResetOutputInstruction : FirstOperandAddressDigitalInstruction, IOutput
    {
        public ResetOutputInstruction()  : base(OperationCode.Reset)
        {
            Operands = new object[1];
        }

        protected override bool IsOperandOk(int index, object value)
        {
            return index == 0 && base.IsOperandOk(index, value);
        }

        protected override bool CheckFirstOperandHasTheCorrectAddressType(Address address)
        {
                if (address.AddressType.Equals(AddressTypeEnum.DigitalMemoryCounter)
                    || address.AddressType.Equals(AddressTypeEnum.DigitalMemoryTimer))
                {
                    return true;
                }
            return false;
        }

        public string GetOutputDeclaration()
        {
            return $"{GetAddress().GetName()}.Reset = 1;";
        }
    }
}
