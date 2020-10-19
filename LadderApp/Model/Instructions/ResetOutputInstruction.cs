using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadderApp.Model
{
    public class ResetOutputInstruction : OneAddressDigitalInstruction, IOutput
    {
        public ResetOutputInstruction()  : base(OperationCode.Reset)
        {
        }

        protected override bool CheckOperand(object operand)
        {
            if (operand is Address address)
            {
                if (!(address.AddressType.Equals(AddressTypeEnum.DigitalMemoryCounter)
                    || address.AddressType.Equals(AddressTypeEnum.DigitalMemoryTimer)))
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public string GetOutputDeclaration()
        {
            return $"{GetAddress().Name}.Reset = 1;";
        }
    }
}
