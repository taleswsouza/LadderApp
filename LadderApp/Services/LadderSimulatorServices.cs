using LadderApp.Model;
using LadderApp.Model.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LadderApp.Services
{
    internal class LadderSimulatorServices
    {
        private AddressingServices addressingServices;
        private LadderVerificationServices verificationServices;
        public LadderSimulatorServices() : this(new LadderVerificationServices(), AddressingServices.Instance)
        {
        }

        public LadderSimulatorServices(LadderVerificationServices verificationServices, AddressingServices addressingServices)
        {
            this.verificationServices = verificationServices;
            this.addressingServices = addressingServices;
        }

        public void ExecuteCountersSimulator(CounterAddress counter)
        {
            switch (counter.Type)
            {
                case 0: // Counter acending
                    if (counter.Reset == true)
                    {
                        counter.Reset = false;
                        counter.Done = false;
                        counter.Accumulated = 0;
                    }
                    if (counter.Enable == true && counter.Pulse == true)
                    {
                        counter.Pulse = false;

                        if (counter.Accumulated <= Int32.MaxValue)
                        {
                            counter.Accumulated++;
                            if (counter.Accumulated >= counter.Preset)
                            {
                                counter.Done = true;
                            }
                            else
                            {
                                counter.Done = false;
                            }
                        }
                    }
                    break;

                case 1: // Counter descending
                    if (counter.Reset == true)
                    {
                        counter.Reset = false;
                        counter.Done = false;
                        counter.Accumulated = counter.Preset;
                    }
                    if (counter.Enable == true && counter.Pulse == true)
                    {
                        counter.Pulse = false;
                        if (counter.Accumulated > 0)
                        {
                            counter.Accumulated--;
                            if (counter.Accumulated == 0)
                            {
                                counter.Done = true;
                            }
                            else
                            {
                                counter.Done = false;
                            }
                        }
                    }
                    break;

                default:
                    break;
            }
            if (counter.Enable == false)
            {
                counter.Pulse = true;
            }
        }

        public bool SimulateLadder(LadderProgram program)
        {
            return SimulateLadder(program, null);
        }

        public bool SimulateLadder(LadderProgram program, Address auxToggleBitPulse)
        {
            if (!verificationServices.VerifyProgram(program))
            {
                return false;
            }

            SimulateTimers(program);

            List<LineStretchSummary> lineStretchSummary = new List<LineStretchSummary>();
            foreach (Line line in program.Lines)
            {
                lineStretchSummary.Add(new LineStretchSummary());
                foreach (Instruction instruction in line.Instructions)
                {
                    switch (instruction.OpCode)
                    {
                        case OperationCode.ParallelBranchBegin:
                            lineStretchSummary.Add(new LineStretchSummary());
                            break;

                        case OperationCode.ParallelBranchEnd:
                            MakeOrLogicOverParallelBranchTwoLastLineStretchAndRemoveLastOne(lineStretchSummary);

                            MakeAndLogicOverTwoLastLineStreatchAndRemoveLastOne(lineStretchSummary);
                            break;

                        case OperationCode.ParallelBranchNext:
                            lineStretchSummary.Add(new LineStretchSummary());
                            break;

                        default:
                            if (lineStretchSummary[lineStretchSummary.Count - 1].Initiated)
                            {
                                lineStretchSummary[lineStretchSummary.Count - 1].LineValue = lineStretchSummary[lineStretchSummary.Count - 1].LineValue && instruction.GetValue();
                            }
                            else
                            {
                                lineStretchSummary[lineStretchSummary.Count - 1].LineValue = instruction.GetValue();
                            }
                            break;
                    }
                }

                foreach (FirstOperandAddressDigitalInstruction instruction in line.Outputs.FindAll(i => i is IDigitalAddressable))
                {
                    switch (instruction.OpCode)
                    {
                        case OperationCode.OutputCoil:
                        case OperationCode.Timer:
                        case OperationCode.Counter:
                        case OperationCode.Reset:

                            if (instruction.OpCode == OperationCode.OutputCoil)
                            {
                                instruction.GetAddress().Value = lineStretchSummary[lineStretchSummary.Count - 1].LineValue;
                            }

                            if (instruction.OpCode == OperationCode.Timer)
                            {
                                TimerAddress timer = (TimerAddress)instruction.GetAddress();
                                timer.Enable = lineStretchSummary[lineStretchSummary.Count - 1].LineValue;
                            }

                            if (instruction.OpCode == OperationCode.Counter)
                            {
                                CounterAddress counter = (CounterAddress)instruction.GetAddress();
                                counter.Enable = lineStretchSummary[lineStretchSummary.Count - 1].LineValue;
                                ExecuteCountersSimulator(counter);
                            }

                            if (instruction.OpCode == OperationCode.Reset)
                            {
                                if (lineStretchSummary[lineStretchSummary.Count - 1].LineValue)
                                {
                                    OutputBoxAddress resetAddress = (OutputBoxAddress)instruction.GetAddress();
                                    resetAddress.Reset = true;
                                    switch (instruction.GetAddress().AddressType)
                                    {
                                        case AddressTypeEnum.DigitalMemoryCounter:
                                            ExecuteCountersSimulator((CounterAddress)resetAddress);
                                            break;

                                        case AddressTypeEnum.DigitalMemoryTimer:
                                            break;

                                        default:
                                            break;
                                    }
                                }
                            }
                            break;

                        default:
                            break;
                    }
                }
                lineStretchSummary.RemoveAt(lineStretchSummary.Count - 1);
            }

            if (auxToggleBitPulse != null)
            {
                auxToggleBitPulse.Value = auxToggleBitPulse.Value == true ? false : true;
                auxToggleBitPulse = null;
            }
            return true;
        }

        public void SimulateTimers(LadderProgram program)
        {
            int parcialPreset = -1;

            foreach (TimerAddress address in program.addressing.ListTimerAddress.Cast<TimerAddress>())
            {

                if (address.Reset == true)
                {
                    address.Accumulated = 0;
                    address.Done = false;
                    address.Reset = false;
                }

                switch (address.Type)
                {
                    case 0: // TON
                        if (address.Enable && !address.Reset)
                        {
                            address.ParcialAccumulated++;
                            if (address.ParcialAccumulated >= address.ParcialPreset)
                            {
                                address.ParcialAccumulated = 0;
                                address.Accumulated++;

                                if (address.Accumulated >= address.Preset)
                                {
                                    address.Done = true; /// DONE = true
                                    address.Accumulated = address.Preset;
                                }
                            }
                        }
                        else
                        {
                            address.Done = false;
                            address.Accumulated = 0;
                            address.ParcialAccumulated = 0;
                            address.Reset = false;
                        }
                        break;

                    case 1: // TOF
                        if (address.Enable || address.Reset)
                        {
                            address.Done = true; /// DONE = true
                            address.Accumulated = 0;
                            address.ParcialAccumulated = 0;
                            address.Reset = false;
                        }
                        else
                        {
                            if (address.Value) // Done enabled - timer counting
                                address.ParcialAccumulated++;

                            if (address.ParcialAccumulated >= address.ParcialPreset)
                            {
                                address.ParcialAccumulated = 0;
                                address.Accumulated++;
                            }

                            if (address.Accumulated >= address.Preset)
                            {
                                address.Done = false; /// DONE = false
                                address.Accumulated = 0;
                                address.ParcialAccumulated = 0;
                            }
                        }

                        break;

                    case 2: // RTO
                        if (address.Reset)
                        {
                            address.Done = false; /// DONE = false
                            address.Accumulated = 0;
                            address.ParcialAccumulated = 0;
                        }

                        if (address.Enable)
                        {
                            address.ParcialAccumulated++;
                            if (address.ParcialAccumulated == parcialPreset)
                            {
                                address.ParcialAccumulated = 0;

                                if (address.Accumulated <= Int32.MaxValue)
                                {
                                    if (address.Accumulated < address.Preset)
                                        address.Accumulated++;
                                    else
                                        address.Accumulated = address.Preset;

                                    if (address.Accumulated >= address.Preset)
                                    {
                                        address.Done = true; /// DONE = true
                                    }
                                    else
                                    {
                                        address.Done = false; /// DONE = false
                                    }
                                }
                            }
                        }
                        break;

                    default:
                        break;
                } /// switch
            }
        }

        private static void MakeAndLogicOverTwoLastLineStreatchAndRemoveLastOne(List<LineStretchSummary> lineStretchSummary)
        {
            if (lineStretchSummary[lineStretchSummary.Count - 2].Initiated)
            {
                lineStretchSummary[lineStretchSummary.Count - 2].LineValue = lineStretchSummary[lineStretchSummary.Count - 2].LineValue && lineStretchSummary[lineStretchSummary.Count - 1].LineValue;
            }
            else
            {
                lineStretchSummary[lineStretchSummary.Count - 2].LineValue = lineStretchSummary[lineStretchSummary.Count - 1].LineValue;
            }
            lineStretchSummary.RemoveAt(lineStretchSummary.Count - 1);
        }

        private static void MakeOrLogicOverParallelBranchTwoLastLineStretchAndRemoveLastOne(List<LineStretchSummary> lineStretchSummary)
        {
            if (lineStretchSummary[lineStretchSummary.Count - 2].Initiated)
            {
                lineStretchSummary[lineStretchSummary.Count - 2].LineValue = lineStretchSummary[lineStretchSummary.Count - 2].LineValue || lineStretchSummary[lineStretchSummary.Count - 1].LineValue;
            }
            else
            {
                lineStretchSummary[lineStretchSummary.Count - 2].LineValue = lineStretchSummary[lineStretchSummary.Count - 1].LineValue;
            }
            lineStretchSummary.RemoveAt(lineStretchSummary.Count - 1);
        }

        private class LineStretchSummary
        {
            private bool lineValue = true;
            public bool Initiated { get; private set; } = false;
            public bool LineValue { get => lineValue; set { lineValue = value; Initiated = true; } }
        }
    }
}