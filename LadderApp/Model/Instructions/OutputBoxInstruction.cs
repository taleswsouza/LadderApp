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

        protected OutputBoxAddress GetOutputBoxAddress()
        {
            return (OutputBoxAddress)GetAddress();
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

        public virtual void setBoxType(int value)
        {
            GetOutputBoxAddress().Type = value;
        }

        public virtual int GetBoxType()
        {
            return GetOutputBoxAddress().Type;
        }

        public virtual void setPreset(int value)
        {
            GetOutputBoxAddress().Preset = value;
        }

        public virtual int GetPreset()
        {
            return GetOutputBoxAddress().Preset;
        }

        public virtual void setAccumulated(int value)
        {
            GetOutputBoxAddress().Accumulated = value;
        }

        public virtual int GetAccumulated()
        {
            return GetOutputBoxAddress().Accumulated;
        }

        public string GetOutputDeclaration()
        {
            return GetAddress().GetEnableBit();
        }

        public string GetName()
        {
            return GetAddress().GetName();
        }
    }
}
