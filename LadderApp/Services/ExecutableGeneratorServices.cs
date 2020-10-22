using LadderApp.Formularios;
using LadderApp.Model;
using LadderApp.Model.Instructions;
using LadderApp.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LadderApp.Services
{
    class ExecutableGeneratorServices
    {

        private MicIntegrationServices micIntegrationServices;
        private OpCode2TextServices opCode2TextServices;
        private LadderVerificationServices verificationServices;
        private AddressingServices addressingServices;


        private LadderAddressing addressing;

        public LadderAddressing GetAddressing()
        {
            return addressing;
        }

        public void SetAddressing(LadderAddressing value)
        {
            addressing = value;
        }

        public ExecutableGeneratorServices() : this(new LadderVerificationServices(), new OpCode2TextServices(), new MicIntegrationServices(false), AddressingServices.Instance)
        {
        }

        public ExecutableGeneratorServices(LadderVerificationServices verificationServices, OpCode2TextServices opCode2TextServices, MicIntegrationServices micIntegrationServices, AddressingServices addressingServices)
        {
            this.verificationServices = verificationServices;
            this.opCode2TextServices = opCode2TextServices;
            this.micIntegrationServices = micIntegrationServices;
            this.addressingServices = addressingServices;
        }

        public bool GenerateExecutable(LadderProgram program, bool saveProgramInsideExecutable, bool savePassword, string password, bool writeProgram)
        {
            SetAddressing(program.addressing);
            addressingServices.SetAddressing(program.addressing);

            String doc = "", lineText = "", lineTestText = "", outputLastOperand = "";
            bool operandInLine = false;
            List<String> outputOperands = new List<string>();
            List<String> operandsToReset = new List<string>();
            String functionsAfterLine = "";
            DialogResult result;

            bool initiated = false;

            if (!verificationServices.VerifyProgram(program))
                return false;

            opCode2TextServices.Add(OperationCode.None);

            if (saveProgramInsideExecutable && savePassword && !String.IsNullOrEmpty(password))
            {
                opCode2TextServices.AddHeader();
                opCode2TextServices.Header.Add(OperationCode.HeadPassword0);
                opCode2TextServices.Header.Add(password.Length);
                opCode2TextServices.Header.Add(password);
            }

            addressingServices.CleanUsedIndication();

            lineText += Environment.NewLine;
            doc += lineText;


            foreach (Line line in program.Lines)
            {
                lineText = "";
                foreach (Instruction instruction in line.Instructions)
                {
                    switch (instruction.OpCode)
                    {
                        case OperationCode.ParallelBranchBegin:
                            if (initiated)
                                lineText += " && ";

                            initiated = false;
                            lineText += "((";
                            break;
                        case OperationCode.ParallelBranchEnd:
                            lineText += "))";
                            break;
                        case OperationCode.ParallelBranchNext:
                            initiated = false;
                            lineText += ") || (";
                            break;
                        default:
                            if (initiated)
                            {
                                lineText += " && ";
                            }
                            switch (instruction.OpCode)
                            {
                                case OperationCode.NormallyOpenContact:
                                    lineText += ((Address)instruction.GetOperand(0)).GetBitVariableName();
                                    ((Address)instruction.GetOperand(0)).Used = true;
                                    break;
                                case OperationCode.NormallyClosedContact:
                                    lineText += "!" + ((Address)instruction.GetOperand(0)).GetBitVariableName();
                                    ((Address)instruction.GetOperand(0)).Used = true;
                                    break;
                            }
                            initiated = true;
                            break;
                    }

                    opCode2TextServices.Add(instruction);
                }
                initiated = false;

                if (lineText == "")
                {
                    lineText = "1";
                }

                lineText = "(" + lineText + ")";

                lineTestText = lineText;

                outputOperands.Clear();
                operandsToReset.Clear();
                foreach (FirstOperandAddressDigitalInstruction instruction in line.Outputs.FindAll(i => i is IDigitalAddressable))
                {
                    switch (instruction.OpCode)
                    {
                        case OperationCode.OutputCoil:
                        case OperationCode.Timer:
                        case OperationCode.Counter:
                            outputOperands.Add(((IOutput)instruction).GetOutputDeclaration());
                            goto case OperationCode.Reset;
                        case OperationCode.Reset:
                            opCode2TextServices.Add((Instruction)instruction);
                            instruction.SetUsed();

                            if (instruction.OpCode == OperationCode.Counter)
                            {
                                functionsAfterLine += " ExecuteCounter(&" + instruction.GetAddress().GetName() + ");";
                            }

                            if (instruction.OpCode == OperationCode.Reset)
                            {
                                operandsToReset.Add(((IOutput)instruction).GetOutputDeclaration());

                                switch (instruction.GetAddress().AddressType)
                                {
                                    case AddressTypeEnum.DigitalMemoryCounter:
                                        operandsToReset.Add("ExecuteCounter(&" + instruction.GetAddress().GetName() + ");");
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }

                if (outputOperands.Count > 0)
                {
                    operandInLine = true;
                    lineText = "";
                    foreach (String outputOperand in outputOperands)
                    {
                        lineText += outputOperand + " = ";
                        outputLastOperand = outputOperand;
                    }
                    lineText += lineTestText + ";";

                    if (functionsAfterLine != "")
                    {
                        lineText += Environment.NewLine + functionsAfterLine;
                    }
                    functionsAfterLine = "";

                    doc += lineText + Environment.NewLine;
                }


                if (operandsToReset.Count > 0)
                {
                    if (operandInLine)
                    {
                        lineTestText = outputLastOperand;
                    }

                    lineText = "if (" + lineTestText + ") {" + Environment.NewLine;
                    foreach (String maybeOutpustOfLine in operandsToReset)
                        lineText += maybeOutpustOfLine + Environment.NewLine;
                    lineText += "}";
                    doc += lineText + Environment.NewLine;
                }
                operandInLine = false;

                doc += Environment.NewLine;

                opCode2TextServices.Add(OperationCode.LineEnd);
            }

            opCode2TextServices.Add(OperationCode.None);

            result = MessageBox.Show("Do you want to generate the .C file below? " + Environment.NewLine + doc, "Confirmation: Generate .C file?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (result == DialogResult.OK)
            {
                string contentLadderProgramDotHFile = "";
                string contentLadderProgramDotCFile = "";
                string contentFunctionsDotHFile = "";
                string contentFunctionsDotCFile = "";
                string contentAddressesDotHFile = "";
                string contentHardwareSetupDotCFile = "";
                string contentMainDotCFile = "";
                string contentIOSetup = "";
                string contentReadInputs = "";
                string contentWriteOutputs = "";
                string contentParameterization = "";
                string contentVariableDeclarations = "";
                string contentOpCodes = "";
                string contentTimers = "";
                bool counterPresent = false;
                bool timerPresent = false;
                bool inputsPresent = false;
                bool outputsPresent = false;

                List<string> usedVariableNames = new List<string>();
                List<string> usedPorts = new List<string>();
                List<int> usedTimerTypes = new List<int>();
                List<int> usedCounterTypes = new List<int>();

                PrepareParameterizationForInputPorts(ref contentParameterization, ref inputsPresent, usedPorts);

                PrepareParameterizationForOutputAddress(ref contentParameterization, ref outputsPresent, usedPorts);

                PrepareVariableDeclarationForInputOrOutPutUsedInProgram(usedVariableNames, usedPorts);

                for (int i = 0; i < usedPorts.Count; i++)
                {
                    contentIOSetup += usedPorts[i] + "OUT = 0; // Init Output data of port" + Environment.NewLine;
                    contentIOSetup += usedPorts[i] + "DIR = " + usedPorts[i] + "_DIR.Byte; // Init of Port1 Data-Direction Reg (Out=1 / Inp=0)" + Environment.NewLine;
                    contentIOSetup += usedPorts[i] + "SEL = 0; // Port-Modules:" + Environment.NewLine;
                    contentIOSetup += usedPorts[i] + "IE = 0; // Interrupt Enable (0=dis 1=enabled)" + Environment.NewLine;
                    contentIOSetup += usedPorts[i] + "IES = 0; // Interrupt Edge Select (0=pos 1=neg)" + Environment.NewLine;
                    contentIOSetup += Environment.NewLine;

                    if (inputsPresent)
                    {
                        contentReadInputs += usedPorts[i] + "_IN.Byte = " + usedPorts[i] + "IN;" + Environment.NewLine;
                    }

                    if (outputsPresent)
                    {
                        contentWriteOutputs += usedPorts[i] + "OUT = " + usedPorts[i] + "_OUT.Byte; // Write Output data of port1" + Environment.NewLine;
                    }
                }

                foreach (Address address in GetAddressing().ListMemoryAddress)
                {
                    if (address.Used)
                    {
                        if (!usedVariableNames.Contains(address.GetStructVariable()))
                        {
                            usedVariableNames.Add(address.GetStructVariable());
                        }
                    }
                }

                if (usedVariableNames.Count > 0)
                {
                    contentVariableDeclarations += "TPort ";
                    foreach (String variableName in usedVariableNames)
                    {
                        contentVariableDeclarations += variableName + ", ";
                    }
                    contentVariableDeclarations = contentVariableDeclarations.Substring(0, contentVariableDeclarations.Length - 2) + ";" + Environment.NewLine;
                    usedVariableNames.Clear();
                }

                /// timer
                contentParameterization += "// timer parameters" + Environment.NewLine;
                foreach (Address address in GetAddressing().ListTimerAddress)
                {
                    if (address.Used)
                    {
                        timerPresent = true;
                        contentParameterization += "\t" + address.GetName() + ".Type = " + address.Timer.Type.ToString() + ";" + Environment.NewLine;
                        contentParameterization += "\t" + address.GetName() + ".TimeBase = " + address.Timer.TimeBase.ToString() + ";" + Environment.NewLine;
                        contentParameterization += "\t" + address.GetName() + ".Preset = " + address.Timer.Preset.ToString() + ";" + Environment.NewLine;
                        contentParameterization += "\t" + address.GetName() + ".Accumulated = 0;" + Environment.NewLine;
                        contentParameterization += Environment.NewLine;

                        /// prepare variable declaration
                        if (!usedVariableNames.Contains(address.GetStructVariable()))
                        {
                            usedVariableNames.Add(address.GetStructVariable());
                        }

                        if (!usedTimerTypes.Contains(address.Timer.Type))
                        {
                            usedTimerTypes.Add(address.Timer.Type);
                        }
                    }
                }

                /// timers
                if (usedVariableNames.Count > 0)
                {
                    contentVariableDeclarations += "TTimer ";
                    foreach (String variableName in usedVariableNames)
                    {
                        contentVariableDeclarations += variableName + ", ";
                        contentTimers += "ExecuteTimer(&" + variableName + ");" + Environment.NewLine;
                    }
                    contentVariableDeclarations = contentVariableDeclarations.Substring(0, contentVariableDeclarations.Length - 2) + ";" + Environment.NewLine;
                    usedVariableNames.Clear();
                }


                /// counters
                foreach (CounterInstruction counter in program.Lines.SelectMany(l => l.Outputs).Where(i => i is CounterInstruction))
                {
                    if (counter.GetAddress().Used)
                    {
                        counterPresent = true;
                        contentParameterization += $"\t{counter.GetName()}.Type = {counter.GetBoxType()};{Environment.NewLine}";
                        contentParameterization += $"\t{counter.GetName()}.Preset = {counter.GetPreset()};{Environment.NewLine}";
                        contentParameterization += $"\t{counter.GetName()}.Accumulated = 0;{Environment.NewLine}";
                        contentParameterization += Environment.NewLine;

                        /// prepare variable declaration
                        if (!usedVariableNames.Contains(counter.GetAddress().GetStructVariable()))
                        {
                            usedVariableNames.Add(counter.GetAddress().GetStructVariable());
                        }

                        if (!usedCounterTypes.Contains(counter.GetBoxType()))
                        {
                            usedCounterTypes.Add(counter.GetBoxType());
                        }
                    }
                }

                /// Prepare TCounter to be declared
                if (usedVariableNames.Count > 0)
                {
                    contentVariableDeclarations += "TCounter ";
                    foreach (String variableDeclaration in usedVariableNames)
                    {
                        contentVariableDeclarations += variableDeclaration + ", ";
                    }
                    contentVariableDeclarations = contentVariableDeclarations.Substring(0, contentVariableDeclarations.Length - 2) + ";" + Environment.NewLine;
                    usedVariableNames.Clear();
                }

                //MSP430IntegrationServices msp430gcc = new MSP430IntegrationServices(false);

                /// Prepara ENDERECOS
                contentAddressesDotHFile = MicrocontrollersBaseCodeFilesResource.addressesH;
                contentAddressesDotHFile = contentAddressesDotHFile.Replace("#VARIABLE_DECLARATIONS#", contentVariableDeclarations);
                contentAddressesDotHFile.Trim();

                micIntegrationServices.CreateFile("addresses.h", contentAddressesDotHFile);


                micIntegrationServices.CreateFile("definitions.h", MicrocontrollersBaseCodeFilesResource.definitionsH);


                micIntegrationServices.CreateFile("hardwaresetup.h", MicrocontrollersBaseCodeFilesResource.hardwaresetupH);


                if (counterPresent)
                {
                    contentFunctionsDotHFile = MicrocontrollersBaseCodeFilesResource.functionsH.Replace("#EXECCOUNTER_FUNCTION_H#", MicrocontrollersBaseCodeFilesResource.execcounter_functionsH);
                }
                else
                {
                    contentFunctionsDotHFile = MicrocontrollersBaseCodeFilesResource.functionsH.Replace("#EXECCOUNTER_FUNCTION_H#", "");
                }


                if (timerPresent)
                {
                    contentFunctionsDotHFile = contentFunctionsDotHFile.Replace("#EXECTIMER_FUNCTION_H#", MicrocontrollersBaseCodeFilesResource.exectimer_functionsH);
                }
                else
                {
                    contentFunctionsDotHFile = contentFunctionsDotHFile.Replace("#EXECTIMER_FUNCTION_H#", "");
                }

                micIntegrationServices.CreateFile("functions.h", contentFunctionsDotHFile);


                if (timerPresent)
                {
                    contentLadderProgramDotHFile = MicrocontrollersBaseCodeFilesResource.ladderprogramH.Replace("#EXEC_TIMERS_LADDERPROGRAM_H#", MicrocontrollersBaseCodeFilesResource.exectimer_functions_ladderprogramH);
                }
                else
                {
                    contentLadderProgramDotHFile = MicrocontrollersBaseCodeFilesResource.ladderprogramH.Replace("#EXEC_TIMERS_LADDERPROGRAM_H#", "");
                }

                micIntegrationServices.CreateFile("ladderprogram.h", contentLadderProgramDotHFile);

                contentLadderProgramDotCFile = MicrocontrollersBaseCodeFilesResource.ladderprogramC;
                if (timerPresent)
                {
                    contentLadderProgramDotCFile = contentLadderProgramDotCFile.Replace("#EXEC_TIMERS_LADDERPROGRAM_C#", MicrocontrollersBaseCodeFilesResource.exectimer_functions_ladderprogramC);
                    contentLadderProgramDotCFile = contentLadderProgramDotCFile.Replace("#TIMERS_LADDERPROGRAM_C#", contentTimers);
                }
                else
                {
                    contentLadderProgramDotCFile = contentLadderProgramDotCFile.Replace("#EXEC_TIMERS_LADDERPROGRAM_C#", "");
                }

                contentLadderProgramDotCFile = contentLadderProgramDotCFile.Replace("#LADDER#", doc);
                contentLadderProgramDotCFile = contentLadderProgramDotCFile.Replace("#PARAMETERIZATION#", contentParameterization);
                contentLadderProgramDotCFile.Trim();

                micIntegrationServices.CreateFile("ladderprogram.c", contentLadderProgramDotCFile);
                micIntegrationServices.CompilesMsp430ViaGcc("ladderprogram");


                contentMainDotCFile = MicrocontrollersBaseCodeFilesResource.mainC;

                if (saveProgramInsideExecutable)
                {
                    opCode2TextServices.FinalizeHeader();
                    contentOpCodes = "const unsigned char ladderInstructions[" + opCode2TextServices.Length+ "] = {" + opCode2TextServices + "};";
                    contentMainDotCFile = contentMainDotCFile.Replace("#LADDER_INSTRUCTIONS#", contentOpCodes);
                }
                else
                {
                    contentMainDotCFile = contentMainDotCFile.Replace("#LADDER_INSTRUCTIONS#", "");
                }
                contentMainDotCFile.Trim();

                //msp430gcc.CreateFile("opcodes.txt", contentOpCodes);


                if (timerPresent)
                {
                    contentMainDotCFile = contentMainDotCFile.Replace("#EXEC_TIMERS_CALL#", "ExecuteTimers();");
                }
                else
                {
                    contentMainDotCFile = contentMainDotCFile.Replace("#EXEC_TIMERS_CALL#", "");
                }

                micIntegrationServices.CreateFile("main.c", contentMainDotCFile);
                micIntegrationServices.CompilesMsp430ViaGcc("main");


                if (counterPresent)
                {
                    contentFunctionsDotCFile = MicrocontrollersBaseCodeFilesResource.functionsC.Replace("#EXEC_COUNTER_FUNCTION_C#", MicrocontrollersBaseCodeFilesResource.execcounter_functionsC);

                    if (usedCounterTypes.Contains(0))
                    {
                        contentFunctionsDotCFile = contentFunctionsDotCFile.Replace("#EXEC_COUNTER_TYPE_0_FUNCTION_C#", MicrocontrollersBaseCodeFilesResource.execcounter_ctu_type0_functionsC);
                    }
                    else
                    {
                        contentFunctionsDotCFile = contentFunctionsDotCFile.Replace("#EXEC_COUNTER_TYPE_0_FUNCTION_C#", "");
                    }

                    if (usedCounterTypes.Contains(1))
                    {
                        contentFunctionsDotCFile = contentFunctionsDotCFile.Replace("#EXEC_COUNTER_TYPE_1_FUNCTION_C#", MicrocontrollersBaseCodeFilesResource.execcounter_ctd_type1_functionsC);
                    }
                    else
                    {
                        contentFunctionsDotCFile = contentFunctionsDotCFile.Replace("#EXEC_COUNTER_TYPE_1_FUNCTION_C#", "");
                    }
                }
                else
                {
                    contentFunctionsDotCFile = MicrocontrollersBaseCodeFilesResource.functionsC.Replace("#EXEC_COUNTER_FUNCTION_C#", "");
                }


                if (timerPresent)
                {
                    contentFunctionsDotCFile = contentFunctionsDotCFile.Replace("#EXEC_TIMER_FUNCTION_C#", MicrocontrollersBaseCodeFilesResource.exectimer_functionsC);

                    if (usedTimerTypes.Contains(0))
                    {
                        contentFunctionsDotCFile = contentFunctionsDotCFile.Replace("#EXEC_TIMER_TYPE_0_FUNCTION_C#", MicrocontrollersBaseCodeFilesResource.exectimer_ton_type0_functions);
                    }
                    else
                    {
                        contentFunctionsDotCFile = contentFunctionsDotCFile.Replace("#EXEC_TIMER_TYPE_0_FUNCTION_C#", "");
                    }

                    if (usedTimerTypes.Contains(1))
                    {
                        contentFunctionsDotCFile = contentFunctionsDotCFile.Replace("#EXEC_TIMER_TYPE_1_FUNCTION_C#", MicrocontrollersBaseCodeFilesResource.exectimer_tof_type1_functions);
                    }
                    else
                    {
                        contentFunctionsDotCFile = contentFunctionsDotCFile.Replace("#EXEC_TIMER_TYPE_1_FUNCTION_C#", "");
                    }
                }
                else
                {
                    contentFunctionsDotCFile = contentFunctionsDotCFile.Replace("#EXEC_TIMER_FUNCTION_C#", "");
                }
                micIntegrationServices.CreateFile("functions.c", contentFunctionsDotCFile);
                micIntegrationServices.CompilesMsp430ViaGcc("functions");


                contentHardwareSetupDotCFile = MicrocontrollersBaseCodeFilesResource.hardwaresetupC.Replace("#IO_HARDWARE_SETUP_C#", contentIOSetup);
                contentHardwareSetupDotCFile = contentHardwareSetupDotCFile.Replace("#READ_INPUTS#", contentReadInputs);
                contentHardwareSetupDotCFile = contentHardwareSetupDotCFile.Replace("#WRITE_OUTPUTS#", contentWriteOutputs);
                micIntegrationServices.CreateFile("hardwaresetup.c", contentHardwareSetupDotCFile);
                micIntegrationServices.CompilesMsp430ViaGcc("hardwaresetup");


                micIntegrationServices.CreateFile("interruption.c", MicrocontrollersBaseCodeFilesResource.interruptionC);
                micIntegrationServices.CompilesMsp430ViaGcc("interruption");

                micIntegrationServices.CompileELF(program.Name);

                micIntegrationServices.CompilationStepMergeAllDotOFilesAndGenerateElfFile(program.Name);

                if (writeProgram)
                {
                    micIntegrationServices.DownloadViaUSB(program.Name);
                }
            }

            return true;
        }

        private static void PrepareVariableDeclarationForInputOrOutPutUsedInProgram(List<string> usedVariableNames, List<string> usedPorts)
        {
            for (int i = 0; i < usedPorts.Count; i++)
            {
                usedVariableNames.Add(usedPorts[i] + "_IN, " + usedPorts[i] + "_OUT, " + usedPorts[i] + "_DIR");
            }
        }

        private void PrepareParameterizationForOutputAddress(ref string contentParameterization, ref bool outputsPresent, List<string> usedPorts)
        {
            /// 1. prepare to setup output ports
            foreach (Address address in GetAddressing().ListOutputAddress)
            {
                if (address.GetIOParameterization() != "" && address.Used == true)
                {
                    outputsPresent = true;
                    /// 1.1. add input ports to parameterization
                    contentParameterization += "\t" + address.GetIOParameterization() + ";" + Environment.NewLine;

                    /// 2.1. prepare input ports to variable declaration
                    if (!usedPorts.Contains(address.GetStructVariable()))
                    {
                        usedPorts.Add(address.GetStructVariable());
                    }
                }
            }
            contentParameterization += Environment.NewLine;
        }

        private void PrepareParameterizationForInputPorts(ref string contentParameterization, ref bool inputsPresent, List<string> usedPorts)
        {
            /// 1. Prepare Parameterization For Input ports.
            /// 2. include these ports in usedPorts list
            foreach (Address address in GetAddressing().ListInputAddress)
            {
                if (address.GetIOParameterization() != "" && address.Used == true)
                {
                    inputsPresent = true;
                    /// 1.1. add parameterization for each input address
                    contentParameterization += "\t" + address.GetIOParameterization() + ";" + Environment.NewLine;

                    /// 2.1. prepare variable declaration to each input address
                    if (!usedPorts.Contains(address.GetStructVariable()))
                        usedPorts.Add(address.GetStructVariable());
                }
            }
            contentParameterization += Environment.NewLine;
        }
    }
}
