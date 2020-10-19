using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadderApp.Model.Instructions
{
    public class OutputBoxInstruction : Instruction, IOutput
    {
        public OutputBoxInstruction(OperationCode opCode) : base(opCode)
        {
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
