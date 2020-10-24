using LadderApp.Model;
using LadderApp.Model.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadderApp.Services
{
    public class AddressingServices
    {
        private LadderAddressing addressing;

        private static readonly AddressingServices instance = new AddressingServices();

        static AddressingServices()
        {
        }

        private AddressingServices()
        {
        }

        public static AddressingServices Instance
        {
            get
            {
                return instance;
            }
        }

        public void SetAddressing(LadderAddressing addressing)
        {
            this.addressing = addressing;
        }

        public Address Find(AddressTypeEnum addressType, int index)
        {
            Address fakeAddress = new Address(addressType, index);
            return Find(fakeAddress);
        }

        public Address Find(Address address)
        {
            int addressingIndex = addressing.GetAllAddresses().IndexOf(address);

            if (addressingIndex == -1)
            {
                return null;
            }

            return addressing.GetAllAddresses()[addressingIndex];
        }

        public bool ReindexAddresses(LadderProgram program)
        {
            foreach (Line line in program.Lines)
            {
                foreach (Instruction instruction in line.Instructions)
                {
                    switch (instruction.OpCode)
                    {
                        case OperationCode.None:
                        case OperationCode.LineBegin:
                        case OperationCode.LineEnd:
                        case OperationCode.ParallelBranchBegin:
                        case OperationCode.ParallelBranchEnd:
                        case OperationCode.ParallelBranchNext:
                            break;
                        default:
                            instruction.SetOperand(0, Find((Address)instruction.GetOperand(0)));
                            break;
                    }
                }
                foreach (Instruction instruction in line.Outputs)
                {
                    switch (instruction.OpCode)
                    {
                        case OperationCode.None:
                        case OperationCode.LineBegin:
                        case OperationCode.LineEnd:
                        case OperationCode.ParallelBranchBegin:
                        case OperationCode.ParallelBranchEnd:
                        case OperationCode.ParallelBranchNext:
                            break;
                        default:
                            instruction.SetOperand(0, Find((Address)instruction.GetOperand(0)));

                            if (instruction.OpCode == OperationCode.Counter)
                            {
                                CounterAddress counter = (CounterAddress)instruction.GetOperand(0);
                                //counter.Type = (int)instruction.GetOperand(1);
                                //counter.Preset = (int)instruction.GetOperand(2);
                                //counter.Accumulated = (int)instruction.GetOperand(3);
                            }

                            if (instruction.OpCode == OperationCode.Timer)
                            {
                                TimerAddress timer = (TimerAddress)instruction.GetOperand(0);
                                //timer.Type = (Int32)instruction.GetOperand(1);
                                //timer.Preset = (Int32)instruction.GetOperand(2);
                                //timer.Accumulated = (Int32)instruction.GetOperand(3);
                                //timer.TimeBase = (Int32)instruction.GetOperand(4);
                            }
                            break;
                    }
                }
            }
            return true;
        }

        public void CleanUsedIndication()
        {
            addressing.GetAllAddresses().ForEach(a => a.Used = false);
        }

        public void AlocateIOAddressing(Device device)
        {
            addressing.ListInputAddress.Clear();
            addressing.ListOutputAddress.Clear();
            foreach (Address address in device.PinAddresses)
            {
                address.SetNumberOfBitsByPort(device.NumberBitsByPort);
                switch (address.AddressType)
                {
                    case AddressTypeEnum.DigitalInput:
                        addressing.ListInputAddress.Add(address);
                        break;
                    case AddressTypeEnum.DigitalOutput:
                        addressing.ListOutputAddress.Add(address);
                        break;
                }
            }
        }

        public int AlocateAddressingMemoryAndTimerAndCounter(LadderProgram program, List<Address> addresses, AddressTypeEnum type, int numberOfAddresses)
        {
            IndicateAddressUsed(program);

            int currentNumberOfAddress = addresses.Count;
            if (currentNumberOfAddress == 0 || currentNumberOfAddress < numberOfAddresses)
            {
                for (int i = currentNumberOfAddress + 1; i <= numberOfAddresses; i++)
                {
                    if (type.Equals(AddressTypeEnum.DigitalMemoryCounter))
                    {
                        addresses.Add(new CounterAddress(i, program.device.NumberBitsByPort));
                        continue;
                    }
                    if (type.Equals(AddressTypeEnum.DigitalMemoryTimer))
                    {
                        addresses.Add(new TimerAddress(i, program.device.NumberBitsByPort));
                        continue;
                    }
                    addresses.Add(new Address(type, i, program.device.NumberBitsByPort));
                }
            }
            else if (currentNumberOfAddress > numberOfAddresses)
            {
                for (int i = (currentNumberOfAddress - 1); i >= numberOfAddresses; i--)
                {
                    if (!addresses[i].Used)
                    {
                        addresses[i] = null;
                        addresses.RemoveAt(i);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return 0;
        }

        private void IndicateAddressUsed(LadderProgram program)
        {
            CleanUsedIndication();
            foreach (FirstOperandAddressDigitalInstruction instruction in program.Lines.SelectMany(l => l.Instructions)
                .Union(program.Lines.SelectMany(l => l.Outputs))
                .Where(i => i is FirstOperandAddressDigitalInstruction)
                .Cast<FirstOperandAddressDigitalInstruction>())
            {
                instruction.SetAddressUsed();
            }
        }

        public void RealocatePinAddressesByPinTypesFromDevice(Device device)
        {
            for (int i = 0; i < (device.NumberOfPorts * device.NumberBitsByPort); i++)
            {
                device.PinAddresses[i].AddressType = device.Pins[i].Type;
            }
        }
    }
}
