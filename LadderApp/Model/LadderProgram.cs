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
            Lines[index].ApagaLinha();
            Lines.RemoveAt(index);
        }

        public void SimulateTimers()
        {
            /// faz a função de um preset parcial para acumular na base de tempo
            /// programada para o temporizador, utilizando a base de tempo da thread (100ms)
            int parcialPreset = -1;

            /// executa a rotina para cada temporizador
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
                foreach (Instruction instruction in line.instructions)
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

                foreach (Instruction instruction in line.outputs)
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

        private bool SavePasswordIntoLadder(ref OpCode2TextServices txtCodigoInterpretavel)
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
                    password = passwordForm.txtSenha.Text;
                    passwordForm.txtSenha.Text = "";
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
                    if (password != passwordForm.txtSenha.Text)
                    {
                        MessageBox.Show("Operation canceled!", "LadderApp", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                    else
                    {
                        txtCodigoInterpretavel.AddCabecalho();
                        txtCodigoInterpretavel.txtCabecalho.Add(OperationCode.HeadPassword0);
                        txtCodigoInterpretavel.txtCabecalho.Add(password.Length);
                        txtCodigoInterpretavel.txtCabecalho.Add(password);
                    }
                }
            }
            return true;
        }

        public bool GeraExecutavel(bool bGravarLadderNoExecutavel, bool bGravarSenha, bool bEscreverPrograma)
        {
            String doc = "", linha = "", linhaTeste = "", saidaUltimoOperando = "";
            bool bOperandosLinha = false;
            List<String> OperandosLinha = new List<string>();
            List<String> OperandosSELinha = new List<string>();
            String FuncoesAposLinha = "";
            DialogResult result;
            OpCode2TextServices txtCodigoInterpretavel = new OpCode2TextServices();

            bool bIniciado = false;

            if (!VerifyProgram())
                return false;

            txtCodigoInterpretavel.Add(OperationCode.None);

            /// caso senha para inserir senha
            /// realiza recuperação da senha
            if (bGravarLadderNoExecutavel && bGravarSenha)
            {
                result = MessageBox.Show("Are you sure you want to write a password to the executable to be generated?", "Request password", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (result == DialogResult.Yes)
                {
                    if (!SavePasswordIntoLadder(ref txtCodigoInterpretavel))
                        return false;
                }
                else
                    return false;

            }


            addressing.CleanUsedIndication();

            linha += Environment.NewLine;
            doc += linha;


            foreach (Line line in this.Lines)
            {
                linha = "";
                foreach (Instruction instruction in line.instructions)
                {
                    switch (instruction.OpCode)
                    {
                        case OperationCode.ParallelBranchBegin:
                            if (bIniciado)
                                linha += " && ";

                            bIniciado = false;
                            linha += "((";
                            break;
                        case OperationCode.ParallelBranchEnd:
                            linha += "))";
                            break;
                        case OperationCode.ParallelBranchNext:
                            bIniciado = false;
                            linha += ") || (";
                            break;
                        default:
                            if (bIniciado)
                                linha += " && ";
                            switch (instruction.OpCode)
                            {
                                case OperationCode.NormallyOpenContact:
                                    linha += ((Address)instruction.GetOperand(0)).Acesso;
                                    ((Address)instruction.GetOperand(0)).Used = true;
                                    break;
                                case OperationCode.NormallyClosedContact:
                                    linha += "!" + ((Address)instruction.GetOperand(0)).Acesso;
                                    ((Address)instruction.GetOperand(0)).Used = true;
                                    break;
                            }
                            bIniciado = true;
                            break;
                    }

                    txtCodigoInterpretavel.Add(instruction);
                }
                bIniciado = false;

                if (linha == "")
                    linha = "1";

                linha = "(" + linha + ")";

                linhaTeste = linha;

                OperandosLinha.Clear();
                OperandosSELinha.Clear();
                foreach (Instruction instruction in line.outputs)
                {
                    switch (instruction.OpCode)
                    {
                        case OperationCode.OutputCoil:
                        case OperationCode.Timer:
                        case OperationCode.Counter:
                        case OperationCode.Reset:
                            txtCodigoInterpretavel.Add(instruction);

                            if (instruction.OpCode == OperationCode.OutputCoil)
                            {
                                OperandosLinha.Add(((Address)instruction.GetOperand(0)).Acesso);
                                ((Address)instruction.GetOperand(0)).Used = true;
                            }
                            else if (instruction.OpCode == OperationCode.Timer)
                            {
                                OperandosLinha.Add(((Address)instruction.GetOperand(0)).Acesso2);
                                ((Address)instruction.GetOperand(0)).Used = true;
                            }
                            else if (instruction.OpCode == OperationCode.Counter)
                            {
                                OperandosLinha.Add(((Address)instruction.GetOperand(0)).Acesso2);
                                FuncoesAposLinha += " ExecContador(&" + ((Address)instruction.GetOperand(0)).Name + ");";
                                ((Address)instruction.GetOperand(0)).Used = true;
                            }
                            else if (instruction.OpCode == OperationCode.Reset)
                            {
                                OperandosSELinha.Add(((Address)instruction.GetOperand(0)).Name + ".Reset = 1;");
                                ((Address)instruction.GetOperand(0)).Used = true;

                                switch (((Address)instruction.GetOperand(0)).AddressType)
                                {
                                    case AddressTypeEnum.DigitalMemoryCounter:
                                        OperandosSELinha.Add("ExecContador(&" + ((Address)instruction.GetOperand(0)).Name + ");");
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

                if (OperandosLinha.Count > 0)
                {
                    bOperandosLinha = true;
                    linha = "";
                    foreach (String saidalinha in OperandosLinha)
                    {
                        linha += saidalinha + " = ";
                        saidaUltimoOperando = saidalinha;
                    }
                    linha += linhaTeste + ";";

                    if (FuncoesAposLinha != "")
                        linha += Environment.NewLine + FuncoesAposLinha;
                    FuncoesAposLinha = "";

                    doc += linha + Environment.NewLine;
                }


                if (OperandosSELinha.Count > 0)
                {
                    if (bOperandosLinha)
                        linhaTeste = saidaUltimoOperando;

                    linha = "if (" + linhaTeste + ") {" + Environment.NewLine;
                    foreach (String saidalinha in OperandosSELinha)
                        linha += saidalinha + Environment.NewLine;
                    linha += "}";
                    doc += linha + Environment.NewLine;
                }
                bOperandosLinha = false;

                doc += Environment.NewLine;

                txtCodigoInterpretavel.Add(OperationCode.LineEnd);
            }

            txtCodigoInterpretavel.Add(OperationCode.None);

            result = MessageBox.Show("Do you want to generate the .C file below? " + Environment.NewLine + doc, "Confirmation: Generate .C file?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (result == DialogResult.OK)
            {
                /// declarações
                string contentUserDotHFile = "";
                string contentUserDotCFile = "";
                string contentFunctionsDotHFile = "";
                string contentFunctionsDotCFile = "";
                string contentAddressesDotHFile = "";
                string contentHardwareSetupDotCFile = "";
                string contentMainDotCFile = "";
                string contentIOSetup = "";
                string contentReadInputs = "";
                string contentWriteOutputs = "";
                string contentParameters = "";
                string contentAddress = "";
                string contentOpCodes = "";
                string contentTimers = "";
                bool counterPresent = false;
                bool timerPresent = false;
                bool inputsPresent = false;
                bool outputsPresent = false;

                List<String> usedAddresses = new List<String>();
                List<String> usedPorts = new List<String>();
                List<Int32> usedTimerTypes = new List<Int32>();
                List<Int32> usedCounterTypes = new List<Int32>();

                /// 1. prepara a configuração para as portas de entrada.
                /// 2. levantamento das portas que foram usadas no programa
                foreach (Address address in addressing.ListInputAddress)
                    if (address.Parametro != "" && address.Used == true)
                    {
                        inputsPresent = true;
                        /// 1.1. Adiciona os parametros dos endereços usados no programa
                        contentParameters += "\t" + address.Parametro + ";" + Environment.NewLine;

                        /// 2.1. prerapara a declaração dos endereços
                        if (!usedPorts.Contains(address.EnderecoRaiz))
                            usedPorts.Add(address.EnderecoRaiz);
                    }
                contentParameters += Environment.NewLine;

                /// 1. prepara a configuração para as portas de saida.
                /// 2. levantamento das portas que foram usadas no programa
                foreach (Address address in addressing.ListOutputAddress)
                    if (address.Parametro != "" && address.Used == true)
                    {
                        outputsPresent = true;
                        /// 1.1. Adiciona os parametros dos endereços usados no programa
                        contentParameters += "\t" + address.Parametro + ";" + Environment.NewLine;

                        /// 2.1. prerapara a declaração dos endereços
                        if (!usedPorts.Contains(address.EnderecoRaiz))
                            usedPorts.Add(address.EnderecoRaiz);
                    }
                contentParameters += Environment.NewLine;

                /// prepara a declaração das portas que foram usadas no programa
                for (int i = 0; i < usedPorts.Count; i++)
                    usedAddresses.Add(usedPorts[i] + "_IN, " + usedPorts[i] + "_OUT, " + usedPorts[i] + "_DIR");

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
                        if (!usedAddresses.Contains(address.EnderecoRaiz))
                            usedAddresses.Add(address.EnderecoRaiz);
                    }

                /// Prepara a lista de endereços do tipo TPort - que será declarada
                if (usedAddresses.Count > 0)
                {
                    contentAddress += "TPort ";
                    foreach (String _strDeclaraVariavel in usedAddresses)
                        contentAddress += _strDeclaraVariavel + ", ";
                    contentAddress = contentAddress.Substring(0, contentAddress.Length - 2) + ";" + Environment.NewLine;
                    usedAddresses.Clear();
                }

                /// Adiciona os parametros dos endereços usados no programa
                contentParameters += "// timer parameters" + Environment.NewLine;
                foreach (Address address in addressing.ListTimerAddress)
                {
                    if (address.Used)
                    {
                        timerPresent = true;
                        contentParameters += "\t" + address.Name + ".Tipo = " + address.Timer.Type.ToString() + ";" + Environment.NewLine;
                        contentParameters += "\t" + address.Name + ".Base = " + address.Timer.TimeBase.ToString() + ";" + Environment.NewLine;
                        contentParameters += "\t" + address.Name + ".Preset = " + address.Timer.Preset.ToString() + ";" + Environment.NewLine;
                        contentParameters += "\t" + address.Name + ".Acumulado = 0;" + Environment.NewLine;
                        contentParameters += Environment.NewLine;

                        /// prerapara a declaração dos endereços
                        if (!usedAddresses.Contains(address.EnderecoRaiz))
                            usedAddresses.Add(address.EnderecoRaiz);

                        /// prerapara verificação dos tipos de temporizadores usados
                        if (!usedTimerTypes.Contains(address.Timer.Type))
                            usedTimerTypes.Add(address.Timer.Type);
                    }
                }

                /// Prepara a lista de endereços do tipo TTemporizador  - que será declarada
                if (usedAddresses.Count > 0)
                {
                    contentAddress += "TTemporizador ";
                    foreach (String variableDeclaration in usedAddresses)
                    {
                        contentAddress += variableDeclaration + ", ";
                        contentTimers += "ExecTemporizador(&" + variableDeclaration + ");" + Environment.NewLine;
                    }
                    contentAddress = contentAddress.Substring(0, contentAddress.Length - 2) + ";" + Environment.NewLine;
                    usedAddresses.Clear();
                }


                /// Adiciona os parametros dos endereços usados no programa
                foreach (Address address in addressing.ListCounterAddress)
                {
                    if (address.Used)
                    {
                        counterPresent = true;
                        contentParameters += "\t" + address.Name + ".Tipo = " + address.Counter.Type.ToString() + ";" + Environment.NewLine;
                        contentParameters += "\t" + address.Name + ".Preset = " + address.Counter.Preset.ToString() + ";" + Environment.NewLine;
                        contentParameters += "\t" + address.Name + ".Acumulado = 0;" + Environment.NewLine;
                        contentParameters += Environment.NewLine;

                        /// prerapara a declaração dos endereços
                        if (!usedAddresses.Contains(address.EnderecoRaiz))
                            usedAddresses.Add(address.EnderecoRaiz);

                        /// prerapara verificação dos tipos de temporizadores usados
                        if (!usedCounterTypes.Contains(address.Counter.Type))
                            usedCounterTypes.Add(address.Counter.Type);
                    }
                }

                /// Prepara a lista de endereços do tipo TContador  - que será declarada
                if (usedAddresses.Count > 0)
                {
                    contentAddress += "TContador ";
                    foreach (String variableDeclaration in usedAddresses)
                        contentAddress += variableDeclaration + ", ";
                    contentAddress = contentAddress.Substring(0, contentAddress.Length - 2) + ";" + Environment.NewLine;
                    usedAddresses.Clear();
                }

                MSP430IntegrationServices msp430gcc = new MSP430IntegrationServices(false);

                /// Prepara ENDERECOS
                contentAddressesDotHFile = MicrocontrollersBaseCodeFilesResource.enderecosH;
                contentAddressesDotHFile = contentAddressesDotHFile.Replace("#ENDEREÇOS#", contentAddress);
                contentAddressesDotHFile.Trim();

                msp430gcc.CreateFile("enderecos.h", contentAddressesDotHFile);


                /// Prepara DEFINICAO
                msp430gcc.CreateFile("definicao.h", MicrocontrollersBaseCodeFilesResource.definicaoH);


                /// Prepara SETUPHARDWARE
                msp430gcc.CreateFile("setuphardware.h", MicrocontrollersBaseCodeFilesResource.setupHardwareH);


                /// Prepara FUNCOES
                if (counterPresent) /// CONTADOR
                    contentFunctionsDotHFile = MicrocontrollersBaseCodeFilesResource.funcoesH.Replace("#EXECCONTADOR_H#", MicrocontrollersBaseCodeFilesResource.ExecContador_funcoesH);
                else
                    contentFunctionsDotHFile = MicrocontrollersBaseCodeFilesResource.funcoesH.Replace("#EXECCONTADOR_H#", "");


                if (timerPresent) /// TEMPORIZADOR
                    contentFunctionsDotHFile = contentFunctionsDotHFile.Replace("#EXECTEMPORIZADOR_H#", MicrocontrollersBaseCodeFilesResource.ExecTemporizador_funcoesH);
                else
                    contentFunctionsDotHFile = contentFunctionsDotHFile.Replace("#EXECTEMPORIZADOR_H#", "");

                msp430gcc.CreateFile("funcoes.h", contentFunctionsDotHFile);


                /// Prepara USUARIO
                if (timerPresent)
                    contentUserDotHFile = MicrocontrollersBaseCodeFilesResource.usuarioH.Replace("#EXECTEMPORIZADORES_H#", MicrocontrollersBaseCodeFilesResource.ExecTemporizadores_usuarioH);
                else
                    contentUserDotHFile = MicrocontrollersBaseCodeFilesResource.usuarioH.Replace("#EXECTEMPORIZADORES_H#", "");

                msp430gcc.CreateFile("usuario.h", contentUserDotHFile);

                contentUserDotCFile = MicrocontrollersBaseCodeFilesResource.usuarioC;
                if (timerPresent)
                {
                    contentUserDotCFile = contentUserDotCFile.Replace("#EXECTEMPORIZADORES_C#", MicrocontrollersBaseCodeFilesResource.ExecTemporizadores_usuarioC);
                    contentUserDotCFile = contentUserDotCFile.Replace("#TEMPORIZADORES#", contentTimers);
                }
                else
                    contentUserDotCFile = contentUserDotCFile.Replace("#EXECTEMPORIZADORES_C#", "");

                contentUserDotCFile = contentUserDotCFile.Replace("#LADDER#", doc);
                contentUserDotCFile = contentUserDotCFile.Replace("#PARAMETROS#", contentParameters);
                contentUserDotCFile.Trim();

                msp430gcc.CreateFile("usuario.c", contentUserDotCFile);
                msp430gcc.CompilesMsp430ViaGcc("usuario");


                /// Prepara MAIN
                contentMainDotCFile = MicrocontrollersBaseCodeFilesResource.mainC;

                if (bGravarLadderNoExecutavel)
                {
                    txtCodigoInterpretavel.FinalizaCabecalho();
                    contentOpCodes = "const unsigned char codigosInterpretaveis[" + txtCodigoInterpretavel.Length.ToString().Trim() + "] = {" + txtCodigoInterpretavel.ToString() + "};";
                    contentMainDotCFile = contentMainDotCFile.Replace("#CODIGOSINTERPRETAVEIS#", contentOpCodes);
                }
                else
                    contentMainDotCFile = contentMainDotCFile.Replace("#CODIGOSINTERPRETAVEIS#", "");
                contentMainDotCFile.Trim();

                /// criar classe para tratar codigos interpretaveis
                msp430gcc.CreateFile("codigos.txt", contentOpCodes);


                if (timerPresent)
                {
                    contentMainDotCFile = contentMainDotCFile.Replace("#EXECTEMPORIZADORES_CHAMADA#", "ExecTemporizadores();");
                }
                else
                {
                    contentMainDotCFile = contentMainDotCFile.Replace("#EXECTEMPORIZADORES_CHAMADA#", "");
                }

                msp430gcc.CreateFile("main.c", contentMainDotCFile);
                msp430gcc.CompilesMsp430ViaGcc("main");


                /// Prepara FUNCOES
                if (counterPresent) /// CONTADOR
                {
                    contentFunctionsDotCFile = MicrocontrollersBaseCodeFilesResource.funcoesC.Replace("#EXECCONTADOR_C#", MicrocontrollersBaseCodeFilesResource.ExecContador_funcoesC);

                    /// TIPOS DE TEMPORIZADORES USADOS
                    if (usedCounterTypes.Contains(0))
                        contentFunctionsDotCFile = contentFunctionsDotCFile.Replace("#EXECCONTADOR_TIPO0#", MicrocontrollersBaseCodeFilesResource.ExecContador_Tipo0_funcoesC);
                    else
                        contentFunctionsDotCFile = contentFunctionsDotCFile.Replace("#EXECCONTADOR_TIPO0#", "");

                    if (usedCounterTypes.Contains(1))
                        contentFunctionsDotCFile = contentFunctionsDotCFile.Replace("#EXECCONTADOR_TIPO1#", MicrocontrollersBaseCodeFilesResource.ExecContador_Tipo1_funcoesC);
                    else
                        contentFunctionsDotCFile = contentFunctionsDotCFile.Replace("#EXECCONTADOR_TIPO1#", "");
                }
                else
                    contentFunctionsDotCFile = MicrocontrollersBaseCodeFilesResource.funcoesC.Replace("#EXECCONTADOR_C#", "");


                if (timerPresent) /// TEMPORIZADOR
                {
                    contentFunctionsDotCFile = contentFunctionsDotCFile.Replace("#EXECTEMPORIZADOR_C#", MicrocontrollersBaseCodeFilesResource.ExecTemporizador_funcoesC);

                    /// TIPOS DE TEMPORIZADORES USADOS
                    if (usedTimerTypes.Contains(0))
                        contentFunctionsDotCFile = contentFunctionsDotCFile.Replace("#EXECTEMPORIZADOR_TIPO0#", MicrocontrollersBaseCodeFilesResource.ExecTemporizador_Tipo0_funcoes);
                    else
                        contentFunctionsDotCFile = contentFunctionsDotCFile.Replace("#EXECTEMPORIZADOR_TIPO0#", "");

                    if (usedTimerTypes.Contains(1))
                        contentFunctionsDotCFile = contentFunctionsDotCFile.Replace("#EXECTEMPORIZADOR_TIPO1#", MicrocontrollersBaseCodeFilesResource.ExecTemporizador_Tipo1_funcoes);
                    else
                        contentFunctionsDotCFile = contentFunctionsDotCFile.Replace("#EXECTEMPORIZADOR_TIPO1#", "");
                }
                else
                {
                    contentFunctionsDotCFile = contentFunctionsDotCFile.Replace("#EXECTEMPORIZADOR_C#", "");
                }
                msp430gcc.CreateFile("funcoes.c", contentFunctionsDotCFile);
                msp430gcc.CompilesMsp430ViaGcc("funcoes");


                /// Prepara SETUPHARDARE
                contentHardwareSetupDotCFile = MicrocontrollersBaseCodeFilesResource.setupHardwareC.Replace("#SETUPIO#", contentIOSetup);
                contentHardwareSetupDotCFile = contentHardwareSetupDotCFile.Replace("#LEENTRADAS#", contentReadInputs);
                contentHardwareSetupDotCFile = contentHardwareSetupDotCFile.Replace("#ESCREVESAIDAS#", contentWriteOutputs);
                msp430gcc.CreateFile("setuphardware.c", contentHardwareSetupDotCFile);
                msp430gcc.CompilesMsp430ViaGcc("setuphardware");


                /// Prepara INTERRUPCAO
                msp430gcc.CreateFile("interrupcao.c", MicrocontrollersBaseCodeFilesResource.interrupcaoC);
                msp430gcc.CompilesMsp430ViaGcc("interrupcao");

                /// CRIA ELF
                msp430gcc.CompileELF(this.Name);

                /// CRIA EXECUTAVE E GRAVA NO DISPOSITIVO
                msp430gcc.CompileA43(this.Name);

                if (bEscreverPrograma)
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
            instructions.InsertAllWithClearBefore(line.outputs);
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

            instructions.InsertAllWithClearBefore(line.instructions);

            /// 1.1 - Verifica se a linha tem simbolos validos
            if (instructions.Count > 0)
                if (instructions.Contains(OperationCode.OutputCoil) ||
                    instructions.Contains(OperationCode.Timer) ||
                    instructions.Contains(OperationCode.Counter) ||
                    instructions.Contains(OperationCode.Reset))
                    return false;


            /// 2.2 - Verifica se todos os simbolos tem os operandos minimos atribuidos
            if (!instructions.ContainsAllOperandos())
                return false;

            return true;
        }

        public bool ReindexAddresses()
        {
            foreach (Line line in this.Lines)
            {
                foreach (Instruction instruction in line.instructions)
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
                foreach (Instruction instruction in line.outputs)
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
