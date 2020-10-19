using LadderApp.Factorys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadderApp.Services
{
    class ExecutableReaderService
    {
        private AddressingServices addressingServices = AddressingServices.Instance;
        private LadderLineServices lineServices = new LadderLineServices();

        public LadderProgram ReadExecutable(string content, string projectName)
        {
            if (content.IndexOf("@laddermic.com") == -1)
            {
                throw new Exception("Executable has no Ladder content!");
            }

            OperationCode opCode = OperationCode.None;
            int countOfEnds = 0;
            int lineIndex = 0;
            Address address;
            AddressTypeEnum addressType;
            int index = 0;


            LadderProgram program = new LadderProgram();
            program.Name = projectName;
            program.device = DeviceFactory.CreateNewDevice();
            addressingServices.AlocateIOAddressing(program.device);
            addressingServices.AlocateMemoryAddressing(program.device, program.addressing.ListMemoryAddress, AddressTypeEnum.DigitalMemory, 10);
            addressingServices.AlocateMemoryAddressing(program.device, program.addressing.ListTimerAddress, AddressTypeEnum.DigitalMemoryTimer, 10);
            addressingServices.AlocateMemoryAddressing(program.device, program.addressing.ListCounterAddress, AddressTypeEnum.DigitalMemoryCounter, 10);
            lineIndex = program.InsertLineAtEnd(new Line());

            for (int position = content.IndexOf("@laddermic.com") + 15; position < content.Length; position++)
            {
                opCode = (OperationCode)Convert.ToChar(content.Substring(position, 1));

                switch (opCode)
                {
                    case OperationCode.None:
                        countOfEnds++;
                        break;
                    case OperationCode.LineEnd:
                        countOfEnds++;
                        if ((OperationCode)Convert.ToChar(content.Substring(position + 1, 1)) != OperationCode.None)
                            lineIndex = program.InsertLineAtEnd(new Line());
                        break;
                    case OperationCode.NormallyOpenContact:
                    case OperationCode.NormallyClosedContact:
                        countOfEnds = 0;
                        Instruction instruction = new Instruction((OperationCode)opCode);

                        addressType = (AddressTypeEnum)Convert.ToChar(content.Substring(position + 1, 1));
                        index = (Int32)Convert.ToChar(content.Substring(position + 2, 1));
                        address = addressingServices.Find(addressType, index);
                        if (address == null)
                        {
                            program.device.Pins[index - 1].Type = addressType;
                            addressingServices.RealocatePinAddressesByPinTypesFromDevice(program.device);
                            addressingServices.AlocateIOAddressing(program.device);
                            address = addressingServices.Find(addressType, index);
                        }
                        instruction.SetOperand(0, address);

                        position += 2;
                        program.Lines[lineIndex].Instructions.Add(instruction);
                        break;
                    case OperationCode.OutputCoil:
                    case OperationCode.Reset:
                        countOfEnds = 0;
                        {
                            InstructionList instructions = new InstructionList();
                            instructions.Add(new Instruction((OperationCode)opCode));
                            addressType = (AddressTypeEnum)Convert.ToChar(content.Substring(position + 1, 1));
                            index = (Int32)Convert.ToChar(content.Substring(position + 2, 1));
                            address = addressingServices.Find(addressType, index);
                            if (address == null)
                            {
                                program.device.Pins[index - 1].Type = addressType;
                                addressingServices.RealocatePinAddressesByPinTypesFromDevice(program.device);
                                addressingServices.AlocateIOAddressing(program.device);
                                address = addressingServices.Find(addressType, index);
                            }
                            instructions[instructions.Count - 1].SetOperand(0, address);
                            position += 2;
                            lineServices.InsertToOutputs(program.Lines[lineIndex], instructions);
                            instructions.Clear();
                        }
                        break;
                    case OperationCode.ParallelBranchBegin:
                    case OperationCode.ParallelBranchEnd:
                    case OperationCode.ParallelBranchNext:
                        countOfEnds = 0;
                        program.Lines[lineIndex].Instructions.Add(new Instruction((OperationCode)opCode));
                        break;
                    case OperationCode.Counter:
                        countOfEnds = 0;
                        {
                            InstructionList instructions = new InstructionList();
                            instructions.Add(new Instruction((OperationCode)opCode));
                            instructions[instructions.Count - 1].SetOperand(0, addressingServices.Find(AddressTypeEnum.DigitalMemoryCounter, (Int32)Convert.ToChar(content.Substring(position + 1, 1))));
                            ((Address)instructions[instructions.Count - 1].GetOperand(0)).Counter.Type = (Int32)Convert.ToChar(content.Substring(position + 2, 1));
                            ((Address)instructions[instructions.Count - 1].GetOperand(0)).Counter.Preset = (Int32)Convert.ToChar(content.Substring(position + 3, 1));

                            instructions[instructions.Count - 1].SetOperand(1, ((Address)instructions[instructions.Count - 1].GetOperand(0)).Counter.Type);
                            instructions[instructions.Count - 1].SetOperand(2, ((Address)instructions[instructions.Count - 1].GetOperand(0)).Counter.Preset);
                            position += 3;
                            lineServices.InsertToOutputs(program.Lines[lineIndex], instructions);
                            instructions.Clear();
                        }
                        break;
                    case OperationCode.Timer:
                        countOfEnds = 0;
                        {
                            InstructionList instructions = new InstructionList();
                            instructions.Add(new Instruction((OperationCode)opCode));
                            instructions[instructions.Count - 1].SetOperand(0, addressingServices.Find(AddressTypeEnum.DigitalMemoryTimer, (Int32)Convert.ToChar(content.Substring(position + 1, 1))));
                            ((Address)instructions[instructions.Count - 1].GetOperand(0)).Timer.Type = (Int32)Convert.ToChar(content.Substring(position + 2, 1));
                            ((Address)instructions[instructions.Count - 1].GetOperand(0)).Timer.TimeBase = (Int32)Convert.ToChar(content.Substring(position + 3, 1));
                            ((Address)instructions[instructions.Count - 1].GetOperand(0)).Timer.Preset = (Int32)Convert.ToChar(content.Substring(position + 4, 1));

                            instructions[instructions.Count - 1].SetOperand(1, ((Address)instructions[instructions.Count - 1].GetOperand(0)).Timer.Type);
                            instructions[instructions.Count - 1].SetOperand(2, ((Address)instructions[instructions.Count - 1].GetOperand(0)).Timer.Preset);
                            instructions[instructions.Count - 1].SetOperand(4, ((Address)instructions[instructions.Count - 1].GetOperand(0)).Timer.TimeBase);

                            position += 4;
                            lineServices.InsertToOutputs(program.Lines[lineIndex], instructions);
                            instructions.Clear();
                        }
                        break;
                }

                /// end of codes
                if (countOfEnds >= 2)
                {
                    MicIntegrationServices p = new MicIntegrationServices();
                    //p.CreateFile("opcode.txt", content.Substring(content.IndexOf("@laddermic.com"), i - content.IndexOf("@laddermic.com") + 1));
                    //position = content.Length;
                    break;
                }

            }
            return program;
        }


    }
}
