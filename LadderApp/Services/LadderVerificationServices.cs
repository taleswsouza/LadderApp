using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadderApp.Services
{
    class LadderVerificationServices
    {
        public List<Address> usedTimers = new List<Address>();
        public List<Address> usedCounters = new List<Address>();
        public bool VerifyProgram(LadderProgram program)
        {
            usedCounters.Clear();
            usedTimers.Clear();
            foreach (Line line in program.Lines)
            {
                if (!VerifyLine(line))
                    return false;
            }
            return true;
        }

        private bool VerifyLine(Line line)
        {
            InstructionList instructions = new InstructionList();
            instructions.InsertAllWithClearBefore(line.Outputs);
            if (instructions.Count > 0)
            {
                if (!(instructions.Contains(OperationCode.OutputCoil) ||
                    instructions.Contains(OperationCode.Timer) ||
                    instructions.Contains(OperationCode.Counter) ||
                    instructions.Contains(OperationCode.Reset)))
                    return false;
            }
            else
                return false;


            if (!instructions.ContainsAllOperandos())
                return false;

            if (!instructions.HasDuplicatedTimers(usedTimers))
                return false;

            if (!instructions.HasDuplicatedCounters(usedCounters))
                return false;

            instructions.InsertAllWithClearBefore(line.Instructions);

            if (instructions.Count > 0)
                if (instructions.Contains(OperationCode.OutputCoil) ||
                    instructions.Contains(OperationCode.Timer) ||
                    instructions.Contains(OperationCode.Counter) ||
                    instructions.Contains(OperationCode.Reset))
                    return false;


            if (!instructions.ContainsAllOperandos())
                return false;

            return true;
        }

    }
}
