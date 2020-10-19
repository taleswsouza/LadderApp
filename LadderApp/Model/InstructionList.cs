using LadderApp.Model;
using LadderApp.Model.Instructions;
using System;
using System.Collections.Generic;
using System.Text;

namespace LadderApp
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
                    if (opCode.Equals(OperationCode.NormallyOpenContact))
                    {
                        this.Add(new NormallyOpenContact());
                    }
                    else if (opCode.Equals(OperationCode.NormallyClosedContact))
                    {
                        this.Add(new NormallyClosedContact());
                    }
                    else if (opCode.Equals(OperationCode.OutputCoil))
                    {
                        this.Add(new OutputCoil());
                    }
                    else if (opCode.Equals(OperationCode.Reset))
                    {
                        this.Add(new ResetOutputInstruction());
                    }
                    else if (opCode.Equals(OperationCode.Timer))
                    {
                        this.Add(new TimerInstruction());
                    }
                    else if (opCode.Equals(OperationCode.Counter))
                    {
                        this.Add(new CounterInstruction());
                    }
                    else
                    {
                        this.Add(new Instruction(opCode));
                    }
                }
            }
            return this;
        }


        public void InsertParallelBranchNext()
        {
            for (int i = (this.Count - 1); i >= 0; i--)
            {
                this.Insert(i, new Instruction(OperationCode.ParallelBranchNext));
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
                    this.Add(new Instruction(OperationCode.ParallelBranchEnd));
                    break;
                case ParallelBranchInsertionType.ParallelBranchCompleted:
                    this[0].OpCode = OperationCode.ParallelBranchBegin;
                    this.Add(new Instruction(OperationCode.ParallelBranchEnd));
                    break;
            }
        }

        public void ValidateOperands()
        {
            foreach (Instruction instruction in this)
                instruction.ValidateInstructionOperands();
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
