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

        public void ExecuteCountersSimulator(Instruction instruction)
        {
            CounterInstruction counter = (CounterInstruction)instruction;
            switch (counter.Counter.Type)
            {
                case 0: // Counter acending
                    if (counter.Counter.Reset == true)
                    {
                        counter.GetAddress().Value = false;
                        counter.Counter.Accumulated = 0;
                        counter.Counter.Reset = false;
                    }
                    if (counter.Counter.Enable == true && counter.Counter.Pulse == true)
                    {
                        counter.Counter.Pulse = false;

                        if (counter.Counter.Accumulated <= Int32.MaxValue)
                        {
                            counter.Counter.Accumulated++;
                            if (counter.Counter.Accumulated >= counter.Counter.Preset)
                            {
                                counter.GetAddress().Value = true;
                            }
                            else
                            {
                                counter.GetAddress().Value = false;
                            }
                        }
                    }
                    break;

                case 1: // Counter descending
                    if (counter.Counter.Reset == true)
                    {
                        counter.Counter.Accumulated = counter.Counter.Preset;
                        counter.GetAddress().Value = false;
                        counter.Counter.Reset = false;
                    }
                    if (counter.Counter.Enable == true && counter.Counter.Pulse == true)
                    {
                        counter.Counter.Pulse = false;
                        if (counter.Counter.Accumulated > 0)
                        {
                            counter.Counter.Accumulated--;
                            if (counter.Counter.Accumulated == 0)
                            {
                                counter.GetAddress().Value = true;
                            }
                            else
                            {
                                counter.GetAddress().Value = false;
                            }
                        }
                    }
                    break;

                default:
                    break;
            }
            if (counter.Counter.Enable == false)
            {
                counter.Counter.Pulse = true;
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
                                instruction.GetAddress().Timer.Enable = lineStretchSummary[lineStretchSummary.Count - 1].LineValue;
                            }

                            if (instruction.OpCode == OperationCode.Counter)
                            {
                                CounterInstruction counter = (CounterInstruction)instruction;
                                counter.Counter.Enable = lineStretchSummary[lineStretchSummary.Count - 1].LineValue;
                                ExecuteCountersSimulator(instruction);
                            }

                            if (instruction.OpCode == OperationCode.Reset)
                            {
                                if (lineStretchSummary[lineStretchSummary.Count - 1].LineValue)
                                {
                                    switch (instruction.GetAddress().AddressType)
                                    {
                                        case AddressTypeEnum.DigitalMemoryCounter:
                                            FirstOperandAddressDigitalInstruction reset = (FirstOperandAddressDigitalInstruction)instruction;
                                            // TODO RETIRAR ESTA LINGUIÇA DEPOIS
                                            CounterInstruction counter = program.Lines.SelectMany(l => l.Outputs).Where(o => o is CounterInstruction).Cast<CounterInstruction>().Where(c => c.GetAddress().Equals(reset.GetAddress())).First();
                                            counter.Counter.Reset = true;
                                            ExecuteCountersSimulator(counter);
                                            break;

                                        case AddressTypeEnum.DigitalMemoryTimer:
                                            instruction.GetAddress().Timer.Reset = true;
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

            foreach (Address address in program.addressing.ListTimerAddress)
            {
                if (address.Timer.Reset == true)
                {
                    address.Timer.Accumulated = 0;
                    address.Value = false;
                    address.Timer.Reset = false;
                }

                switch (address.Timer.Type)
                {
                    case 0: // TON
                        if (address.Timer.Enable && !address.Timer.Reset)
                        {
                            address.Timer.ParcialAccumulated++;
                            if (address.Timer.ParcialAccumulated >= address.Timer.ParcialPreset)
                            {
                                address.Timer.ParcialAccumulated = 0;
                                address.Timer.Accumulated++;

                                if (address.Timer.Accumulated >= address.Timer.Preset)
                                {
                                    address.Value = true; /// DONE = true
                                    address.Timer.Accumulated = address.Timer.Preset;
                                }
                            }
                        }
                        else
                        {
                            address.Value = false;
                            address.Timer.Accumulated = 0;
                            address.Timer.ParcialAccumulated = 0;
                            address.Timer.Reset = false;
                        }
                        break;

                    case 1: // TOF
                        if (address.Timer.Enable || address.Timer.Reset)
                        {
                            address.Value = true; /// DONE = true
                            address.Timer.Accumulated = 0;
                            address.Timer.ParcialAccumulated = 0;
                            address.Timer.Reset = false;
                        }
                        else
                        {
                            if (address.Value) // Done enabled - timer counting
                                address.Timer.ParcialAccumulated++;

                            if (address.Timer.ParcialAccumulated >= address.Timer.ParcialPreset)
                            {
                                address.Timer.ParcialAccumulated = 0;
                                address.Timer.Accumulated++;
                            }

                            if (address.Timer.Accumulated >= address.Timer.Preset)
                            {
                                address.Value = false; /// DONE = false
                                address.Timer.Accumulated = 0;
                                address.Timer.ParcialAccumulated = 0;
                            }
                        }

                        break;

                    case 2: // RTO
                        if (address.Timer.Reset)
                        {
                            address.Value = false; /// DONE = false
                            address.Timer.Accumulated = 0;
                            address.Timer.ParcialAccumulated = 0;
                        }

                        if (address.Timer.Enable)
                        {
                            address.Timer.ParcialAccumulated++;
                            if (address.Timer.ParcialAccumulated == parcialPreset)
                            {
                                address.Timer.ParcialAccumulated = 0;

                                if (address.Timer.Accumulated <= Int32.MaxValue)
                                {
                                    if (address.Timer.Accumulated < address.Timer.Preset)
                                        address.Timer.Accumulated++;
                                    else
                                        address.Timer.Accumulated = address.Timer.Preset;

                                    if (address.Timer.Accumulated >= address.Timer.Preset)
                                    {
                                        address.Value = true; /// DONE = true
                                    }
                                    else
                                    {
                                        address.Value = false; /// DONE = false
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