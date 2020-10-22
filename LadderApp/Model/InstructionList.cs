using LadderApp.Model;
using LadderApp.Model.Instructions;
using System;
using System.Collections.Generic;
using System.Text;

namespace LadderApp.Model
{
    public class InstructionList : List<Instruction>, IList<Instruction>
    {
        private OperationCode operationCode = OperationCode.None;

        public bool Contains(OperationCode opCode)
        {
            operationCode = opCode;
            bool result = this.Exists(FindInstruction);
            operationCode = OperationCode.None;
            return result;
        }

        public bool ContainsAllOperandos()
        {
            return this.TrueForAll(VerifyAllInstructionsHasNotNullOperands);
        }

        public bool HasDuplicatedTimers(List<Address> usedTimers)
        {
            foreach (Instruction instruction in this)
            {
                if (instruction.OpCode == OperationCode.Timer)
                {
                    if (usedTimers.Contains((Address)instruction.GetOperand(0)))
                        return false;
                    else
                        usedTimers.Add((Address)instruction.GetOperand(0));
                }
            }
            return true;
        }

        public bool HasDuplicatedCounters(List<Address> usedCounters)
        {
            foreach (Instruction instruction in this)
            {
                if (instruction.OpCode == OperationCode.Counter)
                {
                    if (usedCounters.Contains((Address)instruction.GetOperand(0)))
                        return false;
                    else
                        usedCounters.Add((Address)instruction.GetOperand(0));
                }
            }
            return true;
        }


        public InstructionList InsertAllWithClearBefore(List<Instruction> instructions)
        {
            this.Clear();

            if (instructions.Count > 0)
                foreach (Instruction instruction in instructions)
                {
                    this.Add(instruction);
                }

            return this;
        }


        public InstructionList InsertAllWithClearBefore(OperationCode[] opCodes)
        {
            this.Clear();

            if (opCodes.Length > 0)
            {
                foreach (OperationCode opCode in opCodes)
                {
                    this.Add(InstructionFactory.createInstruction(opCode));
                }
            }
            return this;
        }


        public void InsertParallelBranchNext()
        {
            for (int i = (this.Count - 1); i >= 0; i--)
            {
                this.Insert(i, InstructionFactory.createInstruction(OperationCode.ParallelBranchNext));
            }
        }


        public void InsertParallelBranch(ParallelBranchInsertionType parallelBranchInsertionType)
        {
            InsertParallelBranchNext();

            switch (parallelBranchInsertionType)
            {
                case ParallelBranchInsertionType.ParallelBranchInitialized:
                    this[0].OpCode = OperationCode.ParallelBranchBegin;
                    break;
                case ParallelBranchInsertionType.ParallelBranchFinalized:
                    this.Add(InstructionFactory.createInstruction(OperationCode.ParallelBranchEnd));
                    break;
                case ParallelBranchInsertionType.ParallelBranchCompleted:
                    this[0].OpCode = OperationCode.ParallelBranchBegin;
                    this.Add(InstructionFactory.createInstruction(OperationCode.ParallelBranchEnd));
                    break;
            }
        }

        public void ValidateOperands()
        {
            foreach (Instruction instruction in this)
            {
                instruction.ValidateInstructionOperands();
            }
        }


        private bool FindInstruction(Instruction instruction)
        {
            if (instruction.OpCode == operationCode)
                return true;
            else
                return false;
        }

        private static bool VerifyAllInstructionsHasNotNullOperands(Instruction instruction)
        {
            for (int i = 0; i < instruction.GetNumberOfOperands(); i++)
            {
                if (instruction.GetOperand(i) == null)
                {
                    return false;
                }
            }
            return true;
        }

        public enum ParallelBranchInsertionType
        {
            ParallelBranchInitialized,
            ParallelBranchFinalized,
            ParallelBranchCompleted
        }
    }
}
