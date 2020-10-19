using LadderApp.Model;
using LadderApp.Model.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadderApp.Services
{
    class LadderSimulatorServices
    {
        private LadderVerificationServices verificationServices;

        public LadderSimulatorServices() : this(new LadderVerificationServices())
        {
        }

        public LadderSimulatorServices(LadderVerificationServices verificationServices)
        {
            this.verificationServices = verificationServices;
        }

        private class LineStretchSummary
        {
            public bool Initiated { get; private set; } = false;

            private bool lineValue = true;
            public bool LineValue { get => lineValue; set { lineValue = value; Initiated = true; } }
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

                foreach (FirstOperandAddressDigitalInstruction instruction in line.Outputs.FindAll(i => i is IAddressable))
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
                                instruction.GetAddress().Counter.Enable = lineStretchSummary[lineStretchSummary.Count - 1].LineValue;
                                ExecuteCountersSimulator(instruction, instruction.GetAddress());
                            }
                            
                            if (instruction.OpCode == OperationCode.Reset)
                            {
                                if (lineStretchSummary[lineStretchSummary.Count - 1].LineValue)
                                {
                                    switch (instruction.GetAddress().AddressType)
                                    {
                                        case AddressTypeEnum.DigitalMemoryCounter:
                                            instruction.GetAddress().Counter.Reset = true;
                                            ExecuteCountersSimulator(instruction, instruction.GetAddress());
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

        public void ExecuteCountersSimulator(Instruction instruction, Address counterAddress)
        {

            switch (counterAddress.Counter.Type)
            {
                case 0: // Counter acending
                    if (counterAddress.Counter.Reset == true)
                    {
                        counterAddress.Value = false;
                        counterAddress.Counter.Accumulated = 0;
                        counterAddress.Counter.Reset = false;
                    }
                    if (counterAddress.Counter.Enable == true && counterAddress.Counter.Pulse == true)
                    {
                        counterAddress.Counter.Pulse = false;

                        if (counterAddress.Counter.Accumulated <= Int32.MaxValue)
                        {
                            counterAddress.Counter.Accumulated++;
                            if (counterAddress.Counter.Accumulated >= counterAddress.Counter.Preset)
                            {
                                counterAddress.Value = true;
                            }
                            else
                            {
                                counterAddress.Value = false;
                            }
                        }
                    }
                    break;

                case 1: // Counter descending
                    if (counterAddress.Counter.Reset == true)
                    {
                        counterAddress.Counter.Accumulated = counterAddress.Counter.Preset;
                        counterAddress.Value = false;
                        counterAddress.Counter.Reset = false;
                    }
                    if (counterAddress.Counter.Enable == true && counterAddress.Counter.Pulse == true)
                    {
                        counterAddress.Counter.Pulse = false;
                        if (counterAddress.Counter.Accumulated > 0)
                        {
                            counterAddress.Counter.Accumulated--;
                            if (counterAddress.Counter.Accumulated == 0)
                            {
                                counterAddress.Value = true;
                            }
                            else
                            {
                                counterAddress.Value = false;
                            }
                        }
                    }
                    break;

                default:
                    break;
            }
            if (counterAddress.Counter.Enable == false)
            {
                counterAddress.Counter.Pulse = true;
            }
        }
    }
}
