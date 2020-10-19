using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadderApp.Services
{
    class LadderLineServices
    {
        public Instruction InsertToOutputs(Line line, InstructionList instructions)
        {
            int index = 0;
            int auxToPositionInsertedInstruciton = 0;

            switch (line.Outputs.Count)
            {
                case 0:
                    /// case 0: First instruction to output, add only one 
                    /// instruction at output
                    index = 0;

                    if (instructions.Count > 1)
                    {
                        instructions.InsertParallelBranch(InstructionList.ParallelBranchInsertionType.ParallelBranchCompleted);
                        auxToPositionInsertedInstruciton = -1;
                    }

                    break;
                case 1:
                    /// case 1: If exists one output instruction, insert parallel branch 
                    /// automatically


                    // here 0=before, 1=after
                    if (index == 0)
                    {
                        /// prepare to insert before the actual object
                        instructions.InsertParallelBranch(InstructionList.ParallelBranchInsertionType.ParallelBranchInitialized);

                        /// insert parallel branch next before the actual object
                        line.Outputs.Insert(0, new Instruction(OperationCode.ParallelBranchNext));
                        /// insert parallel branch end after the actual object
                        line.Outputs.Insert(line.Outputs.Count, new Instruction(OperationCode.ParallelBranchEnd));
                    }
                    else
                    {
                        instructions.InsertParallelBranch(InstructionList.ParallelBranchInsertionType.ParallelBranchFinalized);
                        auxToPositionInsertedInstruciton = -1;

                        line.Outputs.Insert(0, new Instruction(OperationCode.ParallelBranchBegin));
                        index++;
                    }

                    break;
                default:
                    switch (line.Outputs[index].OpCode)
                    {
                        case OperationCode.ParallelBranchBegin:
                            instructions.InsertParallelBranch(InstructionList.ParallelBranchInsertionType.ParallelBranchInitialized);

                            line.Outputs[0].OpCode = OperationCode.ParallelBranchNext;
                            break;
                        case OperationCode.ParallelBranchNext:
                            instructions.InsertParallelBranchNext();
                            break;
                        case OperationCode.ParallelBranchEnd:
                            instructions.InsertParallelBranchNext();
                            break;
                        default:
                            instructions.InsertParallelBranchNext();
                            index++;
                            break;
                    }
                    break;
            }

            foreach (Instruction instruction in instructions)
            {
                line.Outputs.Insert(index, instruction);
                index++;
            }
            return line.Outputs[index - 1 + auxToPositionInsertedInstruciton];
        }

        public void DeleteLine(Line line)
        {
            line.Outputs.Reverse();
            foreach (Instruction instruction in line.Outputs)
            {
                instruction.Dispose();
            }
            line.Outputs.Clear();

            line.Instructions.Reverse();
            foreach (Instruction instruction in line.Instructions)
            {
                instruction.Dispose();
            }
            line.Instructions.Clear();
        }


        public void RemoveLineAt(List<Line> lines, int index)
        {
            lines[index].DeleteLine();
            lines.RemoveAt(index);
        }

    }
}
