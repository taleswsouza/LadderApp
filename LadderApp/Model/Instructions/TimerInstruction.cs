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
            Operands = new Object[5];
            setBoxType(0);
            setPreset(0);
            setAccumulated(0);
            SetTimeBase(0);
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


        public void SetTimeBase(int value)
        {
            SetOperand(4, (int)value);
        }

        public int GetTimeBase()
        {
            return (int)GetOperand(4);
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
