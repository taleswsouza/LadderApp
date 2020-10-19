using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadderApp.Model.Instructions
{
    public abstract class OutputBoxInstruction : FirstOperandAddressDigitalInstruction, IOutput
    {
        public OutputBoxInstruction(OperationCode opCode) : base(opCode)
        {
        }

        protected override bool IsOperandOk(int index, object value)
        {
            if (!base.IsOperandOk(index, value))
            {
                return false;
            }
            if ((index >= 1) && (value is int number))
            {
                if (!(number >= 0 && number <= 255))
                {
                    return false;
                }
                return IsNumberParametersOk(index, number);
            }
            return true;
        }

        protected virtual bool IsNumberParametersOk(int index, int number)
        {
            switch (index)
            {
                case 1:
                    return (number >= 0 && number <= 1);
                case 2:
                    return (number >= 0 && number <= 255);
                case 3:
                    return (number >= 0 && number <= 255);
                default:
                    return false;
            };
        }

        public void setBoxType(int value)
        {
            SetOperand(1, (int)value);
        }

        public int GetBoxType()
        {
            return (int)GetOperand(1);
        }

        public void setPreset(int value)
        {
            SetOperand(2, (int)value);
        }

        public int GetPreset()
        {
            return (int)GetOperand(2);
        }

        public void setAccumulated(int value)
        {
            SetOperand(3, (int)value);
        }

        public int GetAccumulated()
        {
            return (int)GetOperand(3);
        }

        public string GetOutputDeclaration()
        {
            return GetAddress().GetEnableBit();
        }
    }
}
