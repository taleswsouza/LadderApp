using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadderApp.Model.Instructions
{
    public class TimerInstruction : OutputBoxInstruction
    {
        public TimerInstruction() : base(OperationCode.Timer)
        {
            Operands = new Object[1];
        }

        protected override bool IsNumberParametersOk(int index, int number)
        {
            switch (index)
            {
                case 1:
                    return (number >= 0 && number <= 2);
                case 2:
                    return (number >= 0 && number <= 255);
                case 3:
                    return (number >= 0 && number <= 255);
                case 4:
                    // timebase
                    return (number >= 0 && number <= 3);
                default:
                    return false;
            };
        }

        public virtual void SetTimeBase(int value)
        {
            ((TimerAddress)GetOutputBoxAddress()).TimeBase = value;
        }

        public virtual int GetTimeBase()
        {
            return ((TimerAddress)GetOutputBoxAddress()).TimeBase;
        }

        protected override bool CheckFirstOperandHasTheCorrectAddressType(Address address)
        {
            if (address.AddressType.Equals(AddressTypeEnum.DigitalMemoryTimer))
            {
                return true;
            }
            return false;
        }
    }
}
