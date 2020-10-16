using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace LadderApp
{
    public class Line
    {
        public Line()
        {
        }

        [XmlElement(ElementName = "Instruction")]
        public List<Instruction> Instructions { get; set; } = new List<Instruction>();
        public List<Instruction> Outputs { get; set; } = new List<Instruction>();

        public void DeleteLine()
        {
            Outputs.Reverse();
            foreach (Instruction instruction in Outputs)
            {
                instruction.Dispose();
            }
            Outputs.Clear();

            Instructions.Reverse();
            foreach (Instruction instruction in Instructions)
            {
                instruction.Dispose();
            }
            Instructions.Clear();
        }

        public Instruction InsertToOutputs(InstructionList instructions)
        {
            int index = 0;
            int auxToPositionInsertedInstruciton = 0;

            switch (Outputs.Count)
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
                        this.Outputs.Insert(0, new Instruction(OperationCode.ParallelBranchNext));
                        /// insert parallel branch end after the actual object
                        this.Outputs.Insert(this.Outputs.Count, new Instruction(OperationCode.ParallelBranchEnd));
                    }
                    else
                    {
                        instructions.InsertParallelBranch(InstructionList.ParallelBranchInsertionType.ParallelBranchFinalized);
                        auxToPositionInsertedInstruciton = -1;

                        this.Outputs.Insert(0, new Instruction(OperationCode.ParallelBranchBegin));
                        index++;
                    }

                    break;
                default:
                    switch (this.Outputs[index].OpCode)
                    {
                        case OperationCode.ParallelBranchBegin:
                            instructions.InsertParallelBranch(InstructionList.ParallelBranchInsertionType.ParallelBranchInitialized);

                            this.Outputs[0].OpCode = OperationCode.ParallelBranchNext;
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
                this.Outputs.Insert(index, instruction);
                index++;
            }
            return this.Outputs[index - 1 + auxToPositionInsertedInstruciton];
        }
    }
}
