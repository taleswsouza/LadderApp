using System;
using System.Collections.Generic;
using System.Text;



using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.ComponentModel;
using LadderApp.Formularios;
using LadderApp.Resources;

namespace LadderApp
{
    [XmlInclude(typeof(Addressing))]
    [Serializable]
    public class LadderProgram
    {
        public LadderProgram()
        {
        }

        public string Name { get; set; } = "NoName";

        public Addressing addressing = new Addressing();

        public Device device;
        public List<Line> Lines { get; } = new List<Line>();

        [XmlIgnore]
        public List<Address> usedTimers = new List<Address>();
        [XmlIgnore]
        public List<Address> usedCounters = new List<Address>();


        public int InsertLineAtEnd(Line line)
        {
            Lines.Add(line);
            return (Lines.Count - 1);
        }

        public int InsertLineAtBegin(Line line)
        {
            return InsertLineAt(0, line);
        }

        public int InsertLineAt(int index, Line line)
        {
            if (index > Lines.Count)
                index = Lines.Count;

            if (index < 0)
                index = 0;

            Lines.Insert(index, line);
            return index;
        }

        public void RemoveLineAt(int index)
        {
            Lines[index].DeleteLine();
            Lines.RemoveAt(index);
        }

        public void SimulateTimers()
        {
            int parcialPreset = -1;

            foreach (Address address in addressing.ListTimerAddress)
            {
                if (address.Timer.Reset == true)
                {
                    address.Timer.Accumulated = 0;
                    address.Value = false;
                    address.Timer.Reset = false;
                }

                switch (address.Timer.Type)
                {
                    case 0: // TON - Contador Crescente
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
                            address.Value = false; /// DONE = false
                            address.Timer.Accumulated = 0;
                            address.Timer.ParcialAccumulated = 0;
                            address.Timer.Reset = false;
                        }
                        break;

                    case 1: // TOF - Contador Decrescente
                        if (address.Timer.Enable || address.Timer.Reset)
                        {
                            address.Value = true; /// DONE = true
                            address.Timer.Accumulated = 0;
                            address.Timer.ParcialAccumulated = 0;
                            address.Timer.Reset = false;
                        }
                        else
                        {
                            if (address.Value) // DN habilitado - temporizador contando
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
                                        address.Value = true; /// DONE = true
                                    else
                                        address.Value = false; /// DONE = false
                                }
                            }
                        }
                        break;
                    case 3:
                        break;

                    default:
                        break;
                } /// switch
            }

        }

        [XmlIgnore]
        public Address auxToggleBitPulse = null;


        public void ExecutaSimuladoContadores(Instruction instruction, Address counterAddress)
        {

            switch (counterAddress.Counter.Type)
            {
                case 0: // Contador Crescente
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
                                counterAddress.Value = true;
                            else
                                counterAddress.Value = false;
                        }
                    }
                    break;

                case 1: // Contador Decrescente
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
                                counterAddress.Value = true;
                            else
                                counterAddress.Value = false;
                        }
                    }
                    break;

                default:
                    break;
            }
            if (counterAddress.Counter.Enable == false)
                counterAddress.Counter.Pulse = true;

        }

        private class LineStretchSummary
        {
            public bool Initiated { get; set; } = false;
            public bool Value { get; set; } = true;
        }
        public bool SimulateLadder()
        {
            if (!VerifyProgram())
                return false;

            List<LineStretchSummary> lineStretchSummary = new List<LineStretchSummary>();
            foreach (Line line in this.Lines)
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
                            bool value = false;
                            switch (instruction.OpCode)
                            {
                                case OperationCode.NormallyOpenContact:
                                    value = ((Address)instruction.GetOperand(0)).Value;
                                    break;
                                case OperationCode.NormallyClosedContact:
                                    value = !((Address)instruction.GetOperand(0)).Value;
                                    break;
                            }

                            if (lineStretchSummary[lineStretchSummary.Count - 1].Initiated)
                                lineStretchSummary[lineStretchSummary.Count - 1].Value = lineStretchSummary[lineStretchSummary.Count - 1].Value && value;
                            else
                                lineStretchSummary[lineStretchSummary.Count - 1].Value = value;

                            lineStretchSummary[lineStretchSummary.Count - 1].Initiated = true;
                            break;
                    }
                }

                foreach (Instruction instruction in line.Outputs)
                {
                    switch (instruction.OpCode)
                    {
                        case OperationCode.OutputCoil:
                        case OperationCode.Timer:
                        case OperationCode.Counter:
                        case OperationCode.Reset:

                            if (instruction.OpCode == OperationCode.OutputCoil)
                                ((Address)instruction.GetOperand(0)).Value = (bool)lineStretchSummary[lineStretchSummary.Count - 1].Value;
                            else if (instruction.OpCode == OperationCode.Timer)
                            {
                                ((Address)instruction.GetOperand(0)).Timer.Enable = (bool)lineStretchSummary[lineStretchSummary.Count - 1].Value;
                            }
                            else if (instruction.OpCode == OperationCode.Counter)
                            {
                                ((Address)instruction.GetOperand(0)).Counter.Enable = (bool)lineStretchSummary[lineStretchSummary.Count - 1].Value;
                                ExecutaSimuladoContadores(instruction, ((Address)instruction.GetOperand(0)));
                            }
                            else if (instruction.OpCode == OperationCode.Reset)
                            {
                                if ((bool)lineStretchSummary[lineStretchSummary.Count - 1].Value)
                                {
                                    switch (((Address)instruction.GetOperand(0)).AddressType)
                                    {
                                        case AddressTypeEnum.DigitalMemoryCounter:
                                            ((Address)instruction.GetOperand(0)).Counter.Reset = true;
                                            ExecutaSimuladoContadores(instruction, ((Address)instruction.GetOperand(0)));
                                            break;

                                        case AddressTypeEnum.DigitalMemoryTimer:
                                            ((Address)instruction.GetOperand(0)).Timer.Reset = true;
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

        private static void MakeAndLogicOverTwoLastLineStreatchAndRemoveLastOne(List<LineStretchSummary> lineStretchSummary)
        {
            if (lineStretchSummary[lineStretchSummary.Count - 2].Initiated)
                lineStretchSummary[lineStretchSummary.Count - 2].Value = lineStretchSummary[lineStretchSummary.Count - 2].Value && lineStretchSummary[lineStretchSummary.Count - 1].Value;
            else
                lineStretchSummary[lineStretchSummary.Count - 2].Value = lineStretchSummary[lineStretchSummary.Count - 1].Value;
            lineStretchSummary[lineStretchSummary.Count - 2].Initiated = true;
            lineStretchSummary.RemoveAt(lineStretchSummary.Count - 1);
        }

        private static void MakeOrLogicOverParallelBranchTwoLastLineStretchAndRemoveLastOne(List<LineStretchSummary> lineStretchSummary)
        {
            if (lineStretchSummary[lineStretchSummary.Count - 2].Initiated)
                lineStretchSummary[lineStretchSummary.Count - 2].Value = lineStretchSummary[lineStretchSummary.Count - 2].Value || lineStretchSummary[lineStretchSummary.Count - 1].Value;
            else
                lineStretchSummary[lineStretchSummary.Count - 2].Value = lineStretchSummary[lineStretchSummary.Count - 1].Value;
            lineStretchSummary.RemoveAt(lineStretchSummary.Count - 1);
        }

        private bool SavePasswordIntoLadder(ref OpCode2TextServices opCode2TextServices)
        {
            String password = "";
            PasswordForm passwordForm = new PasswordForm();
            passwordForm.Text = "Enter the new password:";
            passwordForm.lblPassword.Text = "New password:";

            for (int i = 0; i < 2; i++)
            {
                DialogResult result = passwordForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    password = passwordForm.txtPassword.Text;
                    passwordForm.txtPassword.Text = "";
                    passwordForm.Text = "Confirm the new password:";
                    passwordForm.lblPassword.Text = "Confirm the new password:";
                    passwordForm.btnOK.DialogResult = DialogResult.Yes;
                }
                else if (result != DialogResult.Yes)
                {
                    MessageBox.Show("Operation canceled!", "LadderApp", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                else
                {
                    if (password != passwordForm.txtPassword.Text)
                    {
                        MessageBox.Show("Operation canceled!", "LadderApp", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                    else
                    {
                        opCode2TextServices.AddHeader();
                        opCode2TextServices.Header.Add(OperationCode.HeadPassword0);
                        opCode2TextServices.Header.Add(password.Length);
                        opCode2TextServices.Header.Add(password);
                    }
                }
            }
            return true;
        }

        public bool GenerateExecutable(bool savePrograInsideExecutable, bool savePassword, bool writeProgram)
        {
            String doc = "", lineText = "", lineTestText = "", outputLastOperand = "";
            bool operandInLine = false;
            List<String> operandsInLine = new List<string>();
            List<String> operandsInLine2MaybeWithAddress = new List<string>();
            String functionsAfterLine = "";
            DialogResult result;
            OpCode2TextServices opCode2TextServices = new OpCode2TextServices();

            bool initiated = false;

            if (!VerifyProgram())
                return false;

            opCode2TextServices.Add(OperationCode.None);

            /// caso senha para inserir senha
            /// realiza recuperação da senha
            if (savePrograInsideExecutable && savePassword)
            {
                result = MessageBox.Show("Are you sure you want to write a password to the executable to be generated?", "Request password", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (result == DialogResult.Yes)
                {
                    if (!SavePasswordIntoLadder(ref opCode2TextServices))
                        return false;
                }
                else
                    return false;

            }


            addressing.CleanUsedIndication();

            lineText += Environment.NewLine;
            doc += lineText;


            foreach (Line line in this.Lines)
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
                                lineText += " && ";
                            switch (instruction.OpCode)
                            {
                                case OperationCode.NormallyOpenContact:
                                    lineText += ((Address)instruction.GetOperand(0)).GetVariableBitValueName();
                                    ((Address)instruction.GetOperand(0)).Used = true;
                                    break;
                                case OperationCode.NormallyClosedContact:
                                    lineText += "!" + ((Address)instruction.GetOperand(0)).GetVariableBitValueName();
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
                    lineText = "1";

                lineText = "(" + lineText + ")";

                lineTestText = lineText;

                operandsInLine.Clear();
                operandsInLine2MaybeWithAddress.Clear();
                foreach (Instruction instruction in line.Outputs)
                {
                    switch (instruction.OpCode)
                    {
                        case OperationCode.OutputCoil:
                        case OperationCode.Timer:
                        case OperationCode.Counter:
                        case OperationCode.Reset:
                            opCode2TextServices.Add(instruction);

                            if (instruction.OpCode == OperationCode.OutputCoil)
                            {
                                operandsInLine.Add(((Address)instruction.GetOperand(0)).GetVariableBitValueName());
                                ((Address)instruction.GetOperand(0)).Used = true;
                            }
                            else if (instruction.OpCode == OperationCode.Timer)
                            {
                                operandsInLine.Add(((Address)instruction.GetOperand(0)).GetEnableBit());
                                ((Address)instruction.GetOperand(0)).Used = true;
                            }
                            else if (instruction.OpCode == OperationCode.Counter)
                            {
                                operandsInLine.Add(((Address)instruction.GetOperand(0)).GetEnableBit());
                                functionsAfterLine += " ExecContador(&" + ((Address)instruction.GetOperand(0)).Name + ");";
                                ((Address)instruction.GetOperand(0)).Used = true;
                            }
                            else if (instruction.OpCode == OperationCode.Reset)
                            {
                                operandsInLine2MaybeWithAddress.Add(((Address)instruction.GetOperand(0)).Name + ".Reset = 1;");
                                ((Address)instruction.GetOperand(0)).Used = true;

                                switch (((Address)instruction.GetOperand(0)).AddressType)
                                {
                                    case AddressTypeEnum.DigitalMemoryCounter:
                                        operandsInLine2MaybeWithAddress.Add("ExecContador(&" + ((Address)instruction.GetOperand(0)).Name + ");");
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

                if (operandsInLine.Count > 0)
                {
                    operandInLine = true;
                    lineText = "";
                    foreach (String saidalinha in operandsInLine)
                    {
                        lineText += saidalinha + " = ";
                        outputLastOperand = saidalinha;
                    }
                    lineText += lineTestText + ";";

                    if (functionsAfterLine != "")
                        lineText += Environment.NewLine + functionsAfterLine;
                    functionsAfterLine = "";

                    doc += lineText + Environment.NewLine;
                }


                if (operandsInLine2MaybeWithAddress.Count > 0)
                {
                    if (operandInLine)
                        lineTestText = outputLastOperand;

                    lineText = "if (" + lineTestText + ") {" + Environment.NewLine;
                    foreach (String maybeOutpustOfLine in operandsInLine2MaybeWithAddress)
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
                /// declarações
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

                /// 1. prepara a configuração para as portas de entrada.
                /// 2. levantamento das portas que foram usadas no programa
                foreach (Address address in addressing.ListInputAddress)
                    if (address.GetPortParameterization() != "" && address.Used == true)
                    {
                        inputsPresent = true;
                        /// 1.1. Adiciona os parametros dos endereços usados no programa
                        contentParameterization += "\t" + address.GetPortParameterization() + ";" + Environment.NewLine;

                        /// 2.1. prerapara a declaração dos endereços
                        if (!usedPorts.Contains(address.GetVariableName()))
                            usedPorts.Add(address.GetVariableName());
                    }
                contentParameterization += Environment.NewLine;

                /// 1. prepara a configuração para as portas de saida.
                /// 2. levantamento das portas que foram usadas no programa
                foreach (Address address in addressing.ListOutputAddress)
                    if (address.GetPortParameterization() != "" && address.Used == true)
                    {
                        outputsPresent = true;
                        /// 1.1. Adiciona os parametros dos endereços usados no programa
                        contentParameterization += "\t" + address.GetPortParameterization() + ";" + Environment.NewLine;

                        /// 2.1. prerapara a declaração dos endereços
                        if (!usedPorts.Contains(address.GetVariableName()))
                            usedPorts.Add(address.GetVariableName());
                    }
                contentParameterization += Environment.NewLine;

                /// prepara a declaração das portas que foram usadas no programa
                for (int i = 0; i < usedPorts.Count; i++)
                    usedVariableNames.Add(usedPorts[i] + "_IN, " + usedPorts[i] + "_OUT, " + usedPorts[i] + "_DIR");

                for (int i = 0; i < usedPorts.Count; i++)
                {
                    /// Escreve rotina SetupIO(void)
                    contentIOSetup += usedPorts[i] + "OUT = 0; // Init Output data of port" + Environment.NewLine;
                    contentIOSetup += usedPorts[i] + "DIR = " + usedPorts[i] + "_DIR.Byte; // Init of Port1 Data-Direction Reg (Out=1 / Inp=0)" + Environment.NewLine;
                    contentIOSetup += usedPorts[i] + "SEL = 0; // Port-Modules:" + Environment.NewLine;
                    contentIOSetup += usedPorts[i] + "IE = 0; // Interrupt Enable (0=dis 1=enabled)" + Environment.NewLine;
                    contentIOSetup += usedPorts[i] + "IES = 0; // Interrupt Edge Select (0=pos 1=neg)" + Environment.NewLine;
                    contentIOSetup += Environment.NewLine;

                    if (inputsPresent)
                    {
                        /// Escreve rotina LeEntradas(void)
                        contentReadInputs += usedPorts[i] + "_IN.Byte = " + usedPorts[i] + "IN;" + Environment.NewLine;
                    }

                    if (outputsPresent)
                    {
                        /// Escreve rotina LEscreveSaidas(void)
                        contentWriteOutputs += usedPorts[i] + "OUT = " + usedPorts[i] + "_OUT.Byte; // Write Output data of port1" + Environment.NewLine;
                    }
                }

                /// prepara composição de parametros e declaração de variáveis
                foreach (Address address in addressing.ListMemoryAddress)
                    if (address.Used)
                    {
                        /// prerapara a declaração dos endereços
                        if (!usedVariableNames.Contains(address.GetVariableName()))
                            usedVariableNames.Add(address.GetVariableName());
                    }

                /// Prepara a lista de endereços do tipo TPort - que será declarada
                if (usedVariableNames.Count > 0)
                {
                    contentVariableDeclarations += "TPort ";
                    foreach (String variableName in usedVariableNames)
                        contentVariableDeclarations += variableName + ", ";
                    contentVariableDeclarations = contentVariableDeclarations.Substring(0, contentVariableDeclarations.Length - 2) + ";" + Environment.NewLine;
                    usedVariableNames.Clear();
                }

                /// Adiciona os parametros dos endereços usados no programa
                contentParameterization += "// timer parameters" + Environment.NewLine;
                foreach (Address address in addressing.ListTimerAddress)
                {
                    if (address.Used)
                    {
                        timerPresent = true;
                        contentParameterization += "\t" + address.Name + ".Tipo = " + address.Timer.Type.ToString() + ";" + Environment.NewLine;
                        contentParameterization += "\t" + address.Name + ".Base = " + address.Timer.TimeBase.ToString() + ";" + Environment.NewLine;
                        contentParameterization += "\t" + address.Name + ".Preset = " + address.Timer.Preset.ToString() + ";" + Environment.NewLine;
                        contentParameterization += "\t" + address.Name + ".Acumulado = 0;" + Environment.NewLine;
                        contentParameterization += Environment.NewLine;

                        /// prerapara a declaração dos endereços
                        if (!usedVariableNames.Contains(address.GetVariableName()))
                            usedVariableNames.Add(address.GetVariableName());

                        /// prerapara verificação dos tipos de temporizadores usados
                        if (!usedTimerTypes.Contains(address.Timer.Type))
                            usedTimerTypes.Add(address.Timer.Type);
                    }
                }

                /// Prepara a lista de endereços do tipo TTemporizador  - que será declarada
                if (usedVariableNames.Count > 0)
                {
                    contentVariableDeclarations += "TTemporizador ";
                    foreach (String variableName in usedVariableNames)
                    {
                        contentVariableDeclarations += variableName + ", ";
                        contentTimers += "ExecTemporizador(&" + variableName + ");" + Environment.NewLine;
                    }
                    contentVariableDeclarations = contentVariableDeclarations.Substring(0, contentVariableDeclarations.Length - 2) + ";" + Environment.NewLine;
                    usedVariableNames.Clear();
                }


                /// Adiciona os parametros dos endereços usados no programa
                foreach (Address address in addressing.ListCounterAddress)
                {
                    if (address.Used)
                    {
                        counterPresent = true;
                        contentParameterization += "\t" + address.Name + ".Tipo = " + address.Counter.Type.ToString() + ";" + Environment.NewLine;
                        contentParameterization += "\t" + address.Name + ".Preset = " + address.Counter.Preset.ToString() + ";" + Environment.NewLine;
                        contentParameterization += "\t" + address.Name + ".Acumulado = 0;" + Environment.NewLine;
                        contentParameterization += Environment.NewLine;

                        /// prerapara a declaração dos endereços
                        if (!usedVariableNames.Contains(address.GetVariableName()))
                            usedVariableNames.Add(address.GetVariableName());

                        /// prerapara verificação dos tipos de temporizadores usados
                        if (!usedCounterTypes.Contains(address.Counter.Type))
                            usedCounterTypes.Add(address.Counter.Type);
                    }
                }

                /// Prepara a lista de endereços do tipo TContador  - que será declarada
                if (usedVariableNames.Count > 0)
                {
                    contentVariableDeclarations += "TContador ";
                    foreach (String variableDeclaration in usedVariableNames)
                        contentVariableDeclarations += variableDeclaration + ", ";
                    contentVariableDeclarations = contentVariableDeclarations.Substring(0, contentVariableDeclarations.Length - 2) + ";" + Environment.NewLine;
                    usedVariableNames.Clear();
                }

                MSP430IntegrationServices msp430gcc = new MSP430IntegrationServices(false);

                /// Prepara ENDERECOS
                contentAddressesDotHFile = MicrocontrollersBaseCodeFilesResource.enderecosH;
                contentAddressesDotHFile = contentAddressesDotHFile.Replace("#VARIABLE_DECLARATIONS#", contentVariableDeclarations);
                contentAddressesDotHFile.Trim();

                msp430gcc.CreateFile("enderecos.h", contentAddressesDotHFile);


                /// Prepara DEFINICAO
                msp430gcc.CreateFile("definicao.h", MicrocontrollersBaseCodeFilesResource.definicaoH);


                /// Prepara SETUPHARDWARE
                msp430gcc.CreateFile("setuphardware.h", MicrocontrollersBaseCodeFilesResource.setupHardwareH);


                /// Prepara FUNCOES
                if (counterPresent) /// CONTADOR
                    contentFunctionsDotHFile = MicrocontrollersBaseCodeFilesResource.funcoesH.Replace("#EXECCOUNTER_FUNCTION_H#", MicrocontrollersBaseCodeFilesResource.ExecContador_funcoesH);
                else
                    contentFunctionsDotHFile = MicrocontrollersBaseCodeFilesResource.funcoesH.Replace("#EXECCOUNTER_FUNCTION_H#", "");


                if (timerPresent) /// TEMPORIZADOR
                    contentFunctionsDotHFile = contentFunctionsDotHFile.Replace("#EXECTIMER_FUNCTION_H#", MicrocontrollersBaseCodeFilesResource.ExecTemporizador_funcoesH);
                else
                    contentFunctionsDotHFile = contentFunctionsDotHFile.Replace("#EXECTIMER_FUNCTION_H#", "");

                msp430gcc.CreateFile("functions.h", contentFunctionsDotHFile);


                /// Prepara USUARIO
                if (timerPresent)
                    contentLadderProgramDotHFile = MicrocontrollersBaseCodeFilesResource.ladderprogramH.Replace("#EXEC_TIMERS_USER_H#", MicrocontrollersBaseCodeFilesResource.ExecTemporizadores_usuarioH);
                else
                    contentLadderProgramDotHFile = MicrocontrollersBaseCodeFilesResource.ladderprogramH.Replace("#EXEC_TIMERS_USER_H#", "");

                msp430gcc.CreateFile("ladderprogram.h", contentLadderProgramDotHFile);

                contentLadderProgramDotCFile = MicrocontrollersBaseCodeFilesResource.ladderprogramC;
                if (timerPresent)
                {
                    contentLadderProgramDotCFile = contentLadderProgramDotCFile.Replace("#EXEC_TIMERS_USER_C#", MicrocontrollersBaseCodeFilesResource.ExecTemporizadores_usuarioC);
                    contentLadderProgramDotCFile = contentLadderProgramDotCFile.Replace("#TIMERS_USER_C#", contentTimers);
                }
                else
                    contentLadderProgramDotCFile = contentLadderProgramDotCFile.Replace("#EXEC_TIMERS_USER_C#", "");

                contentLadderProgramDotCFile = contentLadderProgramDotCFile.Replace("#LADDER#", doc);
                contentLadderProgramDotCFile = contentLadderProgramDotCFile.Replace("#PARAMETERIZATION#", contentParameterization);
                contentLadderProgramDotCFile.Trim();

                msp430gcc.CreateFile("ladderprogram.c", contentLadderProgramDotCFile);
                msp430gcc.CompilesMsp430ViaGcc("ladderprogram");


                /// Prepara MAIN
                contentMainDotCFile = MicrocontrollersBaseCodeFilesResource.mainC;

                if (savePrograInsideExecutable)
                {
                    opCode2TextServices.FinalizeHeader();
                    contentOpCodes = "const unsigned char ladderInstructions[" + opCode2TextServices.Length.ToString().Trim() + "] = {" + opCode2TextServices.ToString() + "};";
                    contentMainDotCFile = contentMainDotCFile.Replace("#LADDER_INSTRUCTIONS#", contentOpCodes);
                }
                else
                    contentMainDotCFile = contentMainDotCFile.Replace("#LADDER_INSTRUCTIONS#", "");
                contentMainDotCFile.Trim();

                ///// criar classe para tratar codigos interpretaveis
                //msp430gcc.CreateFile("codigos.txt", contentOpCodes);


                if (timerPresent)
                {
                    contentMainDotCFile = contentMainDotCFile.Replace("#EXEC_TIMERS_CALL#", "ExecTemporizadores();");
                }
                else
                {
                    contentMainDotCFile = contentMainDotCFile.Replace("#EXEC_TIMERS_CALL#", "");
                }

                msp430gcc.CreateFile("main.c", contentMainDotCFile);
                msp430gcc.CompilesMsp430ViaGcc("main");


                /// Prepara FUNCOES
                if (counterPresent) /// CONTADOR
                {
                    contentFunctionsDotCFile = MicrocontrollersBaseCodeFilesResource.funcoesC.Replace("#EXEC_COUNTER_FUNCTION_C#", MicrocontrollersBaseCodeFilesResource.ExecContador_funcoesC);

                    /// TIPOS DE TEMPORIZADORES USADOS
                    if (usedCounterTypes.Contains(0))
                        contentFunctionsDotCFile = contentFunctionsDotCFile.Replace("#EXEC_COUNTER_TYPE_0_FUNCTION_C#", MicrocontrollersBaseCodeFilesResource.ExecContador_Tipo0_funcoesC);
                    else
                        contentFunctionsDotCFile = contentFunctionsDotCFile.Replace("#EXEC_COUNTER_TYPE_0_FUNCTION_C#", "");

                    if (usedCounterTypes.Contains(1))
                        contentFunctionsDotCFile = contentFunctionsDotCFile.Replace("#EXEC_COUNTER_TYPE_1_FUNCTION_C#", MicrocontrollersBaseCodeFilesResource.ExecContador_Tipo1_funcoesC);
                    else
                        contentFunctionsDotCFile = contentFunctionsDotCFile.Replace("#EXEC_COUNTER_TYPE_1_FUNCTION_C#", "");
                }
                else
                    contentFunctionsDotCFile = MicrocontrollersBaseCodeFilesResource.funcoesC.Replace("#EXEC_COUNTER_FUNCTION_C#", "");


                if (timerPresent) /// TEMPORIZADOR
                {
                    contentFunctionsDotCFile = contentFunctionsDotCFile.Replace("#EXEC_TIMER_FUNCTION_C#", MicrocontrollersBaseCodeFilesResource.ExecTemporizador_funcoesC);

                    /// TIPOS DE TEMPORIZADORES USADOS
                    if (usedTimerTypes.Contains(0))
                        contentFunctionsDotCFile = contentFunctionsDotCFile.Replace("#EXEC_TIMER_TYPE_0_FUNCTION_C#", MicrocontrollersBaseCodeFilesResource.ExecTemporizador_Tipo0_funcoes);
                    else
                        contentFunctionsDotCFile = contentFunctionsDotCFile.Replace("#EXEC_TIMER_TYPE_0_FUNCTION_C#", "");

                    if (usedTimerTypes.Contains(1))
                        contentFunctionsDotCFile = contentFunctionsDotCFile.Replace("#EXEC_TIMER_TYPE_1_FUNCTION_C#", MicrocontrollersBaseCodeFilesResource.ExecTemporizador_Tipo1_funcoes);
                    else
                        contentFunctionsDotCFile = contentFunctionsDotCFile.Replace("#EXEC_TIMER_TYPE_1_FUNCTION_C#", "");
                }
                else
                {
                    contentFunctionsDotCFile = contentFunctionsDotCFile.Replace("#EXEC_TIMER_FUNCTION_C#", "");
                }
                msp430gcc.CreateFile("functions.c", contentFunctionsDotCFile);
                msp430gcc.CompilesMsp430ViaGcc("funcoes");


                /// Prepara SETUPHARDARE
                contentHardwareSetupDotCFile = MicrocontrollersBaseCodeFilesResource.setupHardwareC.Replace("#IO_HARDWARE_SETUP_C#", contentIOSetup);
                contentHardwareSetupDotCFile = contentHardwareSetupDotCFile.Replace("#READ_INPUTS#", contentReadInputs);
                contentHardwareSetupDotCFile = contentHardwareSetupDotCFile.Replace("#WRITE_OUTPUTS#", contentWriteOutputs);
                msp430gcc.CreateFile("setuphardware.c", contentHardwareSetupDotCFile);
                msp430gcc.CompilesMsp430ViaGcc("setuphardware");


                /// Prepara INTERRUPCAO
                msp430gcc.CreateFile("interrupcao.c", MicrocontrollersBaseCodeFilesResource.interrupcaoC);
                msp430gcc.CompilesMsp430ViaGcc("interrupcao");

                /// CRIA ELF
                msp430gcc.CompileELF(this.Name);

                /// CRIA EXECUTAVE E GRAVA NO DISPOSITIVO
                msp430gcc.CompilationStepMergeAllDotOFilesAndGenerateElfFile(this.Name);

                if (writeProgram)
                    msp430gcc.DownloadViaUSB(this.Name);
            }

            return true;
        }

        public bool VerifyProgram()
        {
            usedCounters.Clear();
            usedTimers.Clear();
            foreach (Line line in this.Lines)
            {
                if (!VerifyLine(line))
                    return false;
            }
            return true;
        }

        private bool VerifyLine(Line line)
        {
            InstructionList instructions = new InstructionList();
            instructions.InsertAllWithClearBefore(line.Outputs);
            if (instructions.Count > 0)
            {
                if (!(instructions.Contains(OperationCode.OutputCoil) ||
                    instructions.Contains(OperationCode.Timer) ||
                    instructions.Contains(OperationCode.Counter) ||
                    instructions.Contains(OperationCode.Reset)))
                    return false;
            }
            else
                return false;


            /// 2.1 - Verifica se todos os simbolos tem os operandos minimos atribuidos
            if (!instructions.ContainsAllOperandos())
                return false;

            if (!instructions.HasDuplicatedTimers(usedTimers))
                return false;

            if (!instructions.HasDuplicatedCounters(usedCounters))
                return false;

            instructions.InsertAllWithClearBefore(line.Instructions);

            if (instructions.Count > 0)
                if (instructions.Contains(OperationCode.OutputCoil) ||
                    instructions.Contains(OperationCode.Timer) ||
                    instructions.Contains(OperationCode.Counter) ||
                    instructions.Contains(OperationCode.Reset))
                    return false;


            if (!instructions.ContainsAllOperandos())
                return false;

            return true;
        }

        public bool ReindexAddresses()
        {
            foreach (Line line in this.Lines)
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
                            instruction.SetOperand(0, addressing.Find((Address)instruction.GetOperand(0)));
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
                            instruction.SetOperand(0, addressing.Find((Address)instruction.GetOperand(0)));

                            if (instruction.IsAllOperandsOk())
                            {
                                if (instruction.OpCode == OperationCode.Counter)
                                {
                                    ((Address)instruction.GetOperand(0)).Counter.Type = (Int32)instruction.GetOperand(1);
                                    ((Address)instruction.GetOperand(0)).Counter.Preset = (Int32)instruction.GetOperand(2);
                                    ((Address)instruction.GetOperand(0)).Counter.Accumulated = (Int32)instruction.GetOperand(3);
                                }
                                else if (instruction.OpCode == OperationCode.Timer)
                                {
                                    ((Address)instruction.GetOperand(0)).Timer.Type = (Int32)instruction.GetOperand(1);
                                    ((Address)instruction.GetOperand(0)).Timer.Preset = (Int32)instruction.GetOperand(2);
                                    ((Address)instruction.GetOperand(0)).Timer.Accumulated = (Int32)instruction.GetOperand(3);
                                    ((Address)instruction.GetOperand(0)).Timer.TimeBase = (Int32)instruction.GetOperand(4);
                                }
                            }
                            break;
                    }
                }
            }
            return true;
        }
    }
}
