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

                            //if (instruction.IsAllOperandsOk())
                            //{
                            if (instruction.OpCode == OperationCode.Counter)
                            {
                                CounterInstruction counter = (CounterInstruction)instruction;
                                counter.Counter.Type = (int)instruction.GetOperand(1);
                                counter.Counter.Preset = (int)instruction.GetOperand(2);
                                counter.Counter.Accumulated = (int)instruction.GetOperand(3);
                            }

                            if (instruction.OpCode == OperationCode.Timer)
                            {
                                ((Address)instruction.GetOperand(0)).Timer.Type = (Int32)instruction.GetOperand(1);
                                ((Address)instruction.GetOperand(0)).Timer.Preset = (Int32)instruction.GetOperand(2);
                                ((Address)instruction.GetOperand(0)).Timer.Accumulated = (Int32)instruction.GetOperand(3);
                                ((Address)instruction.GetOperand(0)).Timer.TimeBase = (Int32)instruction.GetOperand(4);
                            }
                            //}
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

        //public int AlocateMemoryAddressing(Device device, List<Address> addresses, AddressTypeEnum addressType, int numberOfAddress)
        //{
        //    int currentNumberOfAddress = addresses.Count;
        //    if ((currentNumberOfAddress == 0) || (currentNumberOfAddress < numberOfAddress))
        //    {
        //        for (int i = currentNumberOfAddress + 1; i <= numberOfAddress; i++)
        //        {
        //            addresses.Add(new Address(addressType, i, device.NumberBitsByPort));
        //        }
        //    }
        //    else if (currentNumberOfAddress > numberOfAddress)
        //    {
        //        for (int i = (currentNumberOfAddress - 1); i >= numberOfAddress; i--)
        //        {
        //            if (!addresses[i].Used)
        //            {
        //                addresses[i] = null;
        //                addresses.RemoveAt(i);
        //            }
        //            else
        //            {
        //                break;
        //            }
        //        }
        //    }
        //    return 0;
        //}

        public int AlocateAddressingMemoryAndTimerAndCounter(LadderProgram program, List<Address> addresses, AddressTypeEnum type, int numberOfAddresses)
        {
            IndicateAddressUsed(program, type);

            int currentNumberOfAddress = addresses.Count;
            if (currentNumberOfAddress == 0 || currentNumberOfAddress < numberOfAddresses)
            {
                for (int i = currentNumberOfAddress + 1; i <= numberOfAddresses; i++)
                {
                    //if (type.Equals(AddressTypeEnum.DigitalMemoryCounter))
                    //{
                    //    addresses.Add(new InternalCounter(i, program.device.NumberBitsByPort));
                    //}
                    //else
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

        private void IndicateAddressUsed(LadderProgram program, AddressTypeEnum addressType)
        {
            CleanUsedIndication();
            foreach (Line line in program.Lines)
            {
                line.Instructions.AddRange(line.Outputs);
                foreach (Instruction instruction in line.Instructions)
                {
                    switch (instruction.OpCode)
                    {
                        /// TODO: Why this is this way?
                        case OperationCode.NormallyOpenContact:
                        case OperationCode.NormallyClosedContact:
                        case OperationCode.OutputCoil:
                        case OperationCode.Timer:
                        case OperationCode.Counter:
                        case OperationCode.Reset:
                            if (instruction.IsAllOperandsOk())
                            {
                                Address address = (Address)instruction.GetOperand(0);
                                if (address.AddressType == addressType)
                                {
                                    address.Used = true;
                                }
                            }
                            break;
                    }
                }
                line.Instructions.RemoveRange(line.Instructions.Count - line.Outputs.Count, line.Outputs.Count);
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
