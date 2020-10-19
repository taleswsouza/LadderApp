using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadderApp.Model.Instructions
{
    public class InstructionFactory
    {
        public static Instruction createInstruction(OperationCode opCode)
        {
            switch(opCode)
            {
                case OperationCode.NormallyOpenContact:
                    return new NormallyOpenContact();
                case OperationCode.NormallyClosedContact:
                    return new NormallyClosedContact();
                case OperationCode.OutputCoil:
                    return new OutputCoil();
                case OperationCode.Reset:
                    return new ResetOutputInstruction();
                case OperationCode.Timer:
                    return new TimerInstruction();
                case OperationCode.Counter:
                    return new CounterInstruction();
                case OperationCode.LineBegin:
                    return new LineBeginInstruction();
                default:
                    return new Instruction(opCode);
            }
        }
    }
}
