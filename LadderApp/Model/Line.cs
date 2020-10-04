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
                    /// case 0: Primeiro simbolo na saida, adiciona apenas um
                    /// simbolo na saida
                    index = 0;

                    if (instructions.Count > 1)
                    {
                        instructions.InsertParallelBranch(InstructionList.ParallelBranchInsertionType.ParallelBranchCompleted);
                        auxToPositionInsertedInstruciton = -1;
                    }

                    break;
                case 1:
                    /// case 1: Caso ja exista 1 (um) simbolo de saida, insere um 
                    /// paralelo de forma automatica


                    // aqui 0=antes, 1=depois
                    if (index == 0)
                    {
                        /// prepara para inserir antes do objeto atual
                        instructions.InsertParallelBranch(InstructionList.ParallelBranchInsertionType.ParallelBranchInitialized);

                        /// insere PP antes do objeto atual na linha
                        this.Outputs.Insert(0, new Instruction(OperationCode.ParallelBranchNext));
                        /// insere PF depois do objeto atual da linha
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
