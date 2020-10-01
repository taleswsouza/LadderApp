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
                    address.Timer.Acumulado = 0;
                    address.Value = false;
                    address.Timer.Reset = false;
                }

                switch (address.Timer.Tipo)
                {
                    case 0: // TON - Contador Crescente
                        if (address.Timer.EN && !address.Timer.Reset)
                        {
                            address.Timer.AcumuladoParcial++;
                            if (address.Timer.AcumuladoParcial >= address.Timer.PresetParcial)
                            {
                                address.Timer.AcumuladoParcial = 0;
                                address.Timer.Acumulado++;

                                if (address.Timer.Acumulado >= address.Timer.Preset)
                                {
                                    address.Value = true; /// DONE = true
                                    address.Timer.Acumulado = address.Timer.Preset;
                                }
                            }
                        }
                        else
                        {
                            address.Value = false; /// DONE = false
                            address.Timer.Acumulado = 0;
                            address.Timer.AcumuladoParcial = 0;
                            address.Timer.Reset = false;
                        }
                        break;

                    case 1: // TOF - Contador Decrescente
                        if (address.Timer.EN || address.Timer.Reset)
                        {
                            address.Value = true; /// DONE = true
                            address.Timer.Acumulado = 0;
                            address.Timer.AcumuladoParcial = 0;
                            address.Timer.Reset = false;
                        }
                        else
                        {
                            if (address.Value) // DN habilitado - temporizador contando
                                address.Timer.AcumuladoParcial++;

                            if (address.Timer.AcumuladoParcial >= address.Timer.PresetParcial)
                            {
                                address.Timer.AcumuladoParcial = 0;
                                address.Timer.Acumulado++;
                            }

                            if (address.Timer.Acumulado >= address.Timer.Preset)
                            {
                                address.Value = false; /// DONE = false
                                address.Timer.Acumulado = 0;
                                address.Timer.AcumuladoParcial = 0;
                            }
                        }

                        break;

                    case 2: // RTO
                        if (address.Timer.Reset)
                        {
                            address.Value = false; /// DONE = false
                            address.Timer.Acumulado = 0;
                            address.Timer.AcumuladoParcial = 0;
                        }

                        if (address.Timer.EN)
                        {
                            address.Timer.AcumuladoParcial++;
                            if (address.Timer.AcumuladoParcial == parcialPreset)
                            {
                                address.Timer.AcumuladoParcial = 0;

                                if (address.Timer.Acumulado <= Int32.MaxValue)
                                {
                                    if (address.Timer.Acumulado < address.Timer.Preset)
                                        address.Timer.Acumulado++;
                                    else
                                        address.Timer.Acumulado = address.Timer.Preset;

                                    if (address.Timer.Acumulado >= address.Timer.Preset)
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

            switch (counterAddress.Counter.Tipo)
            {
                case 0: // Contador Crescente
                    if (counterAddress.Counter.Reset == true)
                    {
                        counterAddress.Value = false;
                        counterAddress.Counter.Acumulado = 0;
                        counterAddress.Counter.Reset = false;
                    }
                    if (counterAddress.Counter.EN == true && counterAddress.Counter.Pulso == true)
                    {
                        counterAddress.Counter.Pulso = false;

                        if (counterAddress.Counter.Acumulado <= Int32.MaxValue)
                        {
                            counterAddress.Counter.Acumulado++;
                            if (counterAddress.Counter.Acumulado >= counterAddress.Counter.Preset)
                                counterAddress.Value = true;
                            else
                                counterAddress.Value = false;
                        }
                    }
                    break;

                case 1: // Contador Decrescente
                    if (counterAddress.Counter.Reset == true)
                    {
                        counterAddress.Counter.Acumulado = counterAddress.Counter.Preset;
                        counterAddress.Value = false;
                        counterAddress.Counter.Reset = false;
                    }
                    if (counterAddress.Counter.EN == true && counterAddress.Counter.Pulso == true)
                    {
                        counterAddress.Counter.Pulso = false;
                        if (counterAddress.Counter.Acumulado > 0)
                        {
                            counterAddress.Counter.Acumulado--;

                            if (counterAddress.Counter.Acumulado == 0)
                                counterAddress.Value = true;
                            else
                                counterAddress.Value = false;
                        }
                    }
                    break;

                default:
                    break;
            }
            if (counterAddress.Counter.EN == false)
                counterAddress.Counter.Pulso = true;

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
                                ((Address)instruction.GetOperand(0)).Timer.EN = (bool)lineStretchSummary[lineStretchSummary.Count - 1].Value;
                            }
                            else if (instruction.OpCode == OperationCode.Counter)
                            {
                                ((Address)instruction.GetOperand(0)).Counter.EN = (bool)lineStretchSummary[lineStretchSummary.Count - 1].Value;
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


            addressing.LimpaIndicacaoEmUso();

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
                string DadosArquivoUsuarioC = "";
                string DadosArquivoUsuarioH = "";
                string DadosArquivoFuncoesC = "";
                string DadosArquivoFuncoesH = "";
                string DadosArquivoEnderecosH = "";
                string DadosArquivoSetupHardwareC = "";
                string DadosArquivoMainC = "";
                string DadosSetupIO = "";
                string DadosLeEntradas = "";
                string DadosEscreveSaidas = "";
                string DadosParametros = "";
                string DadosEnderecos = "";
                string DadosCodigosInterpretaveis = "";
                string DadosTemporizadores = "";
                bool bIndicaContadorNoPrograma = false;
                bool bIndicaTemporizadorNoPrograma = false;
                bool bIndicaEntradaUsadaNoPrograma = false;
                bool bIndicaSaidaUsadaNoPrograma = false;

                List<String> _lstEndUsados = new List<String>();
                List<String> usedPorts = new List<String>();
                List<Int32> _lstTiposTemporizadoresUsados = new List<Int32>();
                List<Int32> _lstTiposContadoresUsados = new List<Int32>();

                /// 1. prepara a configuração para as portas de entrada.
                /// 2. levantamento das portas que foram usadas no programa
                foreach (Address address in addressing.ListInputAddress)
                    if (address.Parametro != "" && address.Used == true)
                    {
                        bIndicaEntradaUsadaNoPrograma = true;
                        /// 1.1. Adiciona os parametros dos endereços usados no programa
                        DadosParametros += "\t" + address.Parametro + ";" + Environment.NewLine;

                        /// 2.1. prerapara a declaração dos endereços
                        if (!usedPorts.Contains(address.EnderecoRaiz))
                            usedPorts.Add(address.EnderecoRaiz);
                    }
                DadosParametros += Environment.NewLine;

                /// 1. prepara a configuração para as portas de saida.
                /// 2. levantamento das portas que foram usadas no programa
                foreach (Address _endCada in addressing.ListOutputAddress)
                    if (_endCada.Parametro != "" && _endCada.Used == true)
                    {
                        bIndicaSaidaUsadaNoPrograma = true;
                        /// 1.1. Adiciona os parametros dos endereços usados no programa
                        DadosParametros += "\t" + _endCada.Parametro + ";" + Environment.NewLine;

                        /// 2.1. prerapara a declaração dos endereços
                        if (!usedPorts.Contains(_endCada.EnderecoRaiz))
                            usedPorts.Add(_endCada.EnderecoRaiz);
                    }
                DadosParametros += Environment.NewLine;

                /// prepara a declaração das portas que foram usadas no programa
                for (int i = 0; i < usedPorts.Count; i++)
                    _lstEndUsados.Add(usedPorts[i] + "_IN, " + usedPorts[i] + "_OUT, " + usedPorts[i] + "_DIR");

                for (int i = 0; i < usedPorts.Count; i++)
                {
                    /// Escreve rotina SetupIO(void)
                    DadosSetupIO += usedPorts[i] + "OUT = 0; // Init Output data of port" + Environment.NewLine;
                    DadosSetupIO += usedPorts[i] + "DIR = " + usedPorts[i] + "_DIR.Byte; // Init of Port1 Data-Direction Reg (Out=1 / Inp=0)" + Environment.NewLine;
                    DadosSetupIO += usedPorts[i] + "SEL = 0; // Port-Modules:" + Environment.NewLine;
                    DadosSetupIO += usedPorts[i] + "IE = 0; // Interrupt Enable (0=dis 1=enabled)" + Environment.NewLine;
                    DadosSetupIO += usedPorts[i] + "IES = 0; // Interrupt Edge Select (0=pos 1=neg)" + Environment.NewLine;
                    DadosSetupIO += Environment.NewLine;

                    if (bIndicaEntradaUsadaNoPrograma)
                    {
                        /// Escreve rotina LeEntradas(void)
                        DadosLeEntradas += usedPorts[i] + "_IN.Byte = " + usedPorts[i] + "IN;" + Environment.NewLine;
                    }

                    if (bIndicaSaidaUsadaNoPrograma)
                    {
                        /// Escreve rotina LEscreveSaidas(void)
                        DadosEscreveSaidas += usedPorts[i] + "OUT = " + usedPorts[i] + "_OUT.Byte; // Write Output data of port1" + Environment.NewLine;
                    }
                }

                /// prepara composição de parametros e declaração de variáveis
                foreach (Address _endCada in addressing.ListMemoryAddress)
                    if (_endCada.Used)
                    {
                        /// prerapara a declaração dos endereços
                        if (!_lstEndUsados.Contains(_endCada.EnderecoRaiz))
                            _lstEndUsados.Add(_endCada.EnderecoRaiz);
                    }

                /// Prepara a lista de endereços do tipo TPort - que será declarada
                if (_lstEndUsados.Count > 0)
                {
                    DadosEnderecos += "TPort ";
                    foreach (String _strDeclaraVariavel in _lstEndUsados)
                        DadosEnderecos += _strDeclaraVariavel + ", ";
                    DadosEnderecos = DadosEnderecos.Substring(0, DadosEnderecos.Length - 2) + ";" + Environment.NewLine;
                    _lstEndUsados.Clear();
                }

                /// Adiciona os parametros dos endereços usados no programa
                DadosParametros += "// timer parameters" + Environment.NewLine;
                foreach (Address _endCada in addressing.ListTimerAddress)
                {
                    if (_endCada.Used)
                    {
                        bIndicaTemporizadorNoPrograma = true;
                        DadosParametros += "\t" + _endCada.Name + ".Tipo = " + _endCada.Timer.Tipo.ToString() + ";" + Environment.NewLine;
                        DadosParametros += "\t" + _endCada.Name + ".Base = " + _endCada.Timer.BaseTempo.ToString() + ";" + Environment.NewLine;
                        DadosParametros += "\t" + _endCada.Name + ".Preset = " + _endCada.Timer.Preset.ToString() + ";" + Environment.NewLine;
                        DadosParametros += "\t" + _endCada.Name + ".Acumulado = 0;" + Environment.NewLine;
                        DadosParametros += Environment.NewLine;

                        /// prerapara a declaração dos endereços
                        if (!_lstEndUsados.Contains(_endCada.EnderecoRaiz))
                            _lstEndUsados.Add(_endCada.EnderecoRaiz);

                        /// prerapara verificação dos tipos de temporizadores usados
                        if (!_lstTiposTemporizadoresUsados.Contains(_endCada.Timer.Tipo))
                            _lstTiposTemporizadoresUsados.Add(_endCada.Timer.Tipo);
                    }
                }

                /// Prepara a lista de endereços do tipo TTemporizador  - que será declarada
                if (_lstEndUsados.Count > 0)
                {
                    DadosEnderecos += "TTemporizador ";
                    foreach (String _strDeclaraVariavel in _lstEndUsados)
                    {
                        DadosEnderecos += _strDeclaraVariavel + ", ";
                        DadosTemporizadores += "ExecTemporizador(&" + _strDeclaraVariavel + ");" + Environment.NewLine;
                    }
                    DadosEnderecos = DadosEnderecos.Substring(0, DadosEnderecos.Length - 2) + ";" + Environment.NewLine;
                    _lstEndUsados.Clear();
                }


                /// Adiciona os parametros dos endereços usados no programa
                foreach (Address _endCada in addressing.ListCounterAddress)
                {
                    if (_endCada.Used)
                    {
                        bIndicaContadorNoPrograma = true;
                        DadosParametros += "\t" + _endCada.Name + ".Tipo = " + _endCada.Counter.Tipo.ToString() + ";" + Environment.NewLine;
                        DadosParametros += "\t" + _endCada.Name + ".Preset = " + _endCada.Counter.Preset.ToString() + ";" + Environment.NewLine;
                        DadosParametros += "\t" + _endCada.Name + ".Acumulado = 0;" + Environment.NewLine;
                        DadosParametros += Environment.NewLine;

                        /// prerapara a declaração dos endereços
                        if (!_lstEndUsados.Contains(_endCada.EnderecoRaiz))
                            _lstEndUsados.Add(_endCada.EnderecoRaiz);

                        /// prerapara verificação dos tipos de temporizadores usados
                        if (!_lstTiposContadoresUsados.Contains(_endCada.Counter.Tipo))
                            _lstTiposContadoresUsados.Add(_endCada.Counter.Tipo);
                    }
                }

                /// Prepara a lista de endereços do tipo TContador  - que será declarada
                if (_lstEndUsados.Count > 0)
                {
                    DadosEnderecos += "TContador ";
                    foreach (String _strDeclaraVariavel in _lstEndUsados)
                        DadosEnderecos += _strDeclaraVariavel + ", ";
                    DadosEnderecos = DadosEnderecos.Substring(0, DadosEnderecos.Length - 2) + ";" + Environment.NewLine;
                    _lstEndUsados.Clear();
                }

                MSP430IntegrationServices msp430gcc = new MSP430IntegrationServices(false);

                /// Prepara ENDERECOS
                DadosArquivoEnderecosH = MicrocontrollersBaseCodeFilesResource.enderecosH;
                DadosArquivoEnderecosH = DadosArquivoEnderecosH.Replace("#ENDEREÇOS#", DadosEnderecos);
                DadosArquivoEnderecosH.Trim();

                msp430gcc.CreateFile("enderecos.h", DadosArquivoEnderecosH);


                /// Prepara DEFINICAO
                msp430gcc.CreateFile("definicao.h", MicrocontrollersBaseCodeFilesResource.definicaoH);


                /// Prepara SETUPHARDWARE
                msp430gcc.CreateFile("setuphardware.h", MicrocontrollersBaseCodeFilesResource.setupHardwareH);


                /// Prepara FUNCOES
                if (bIndicaContadorNoPrograma) /// CONTADOR
                    DadosArquivoFuncoesH = MicrocontrollersBaseCodeFilesResource.funcoesH.Replace("#EXECCONTADOR_H#", MicrocontrollersBaseCodeFilesResource.ExecContador_funcoesH);
                else
                    DadosArquivoFuncoesH = MicrocontrollersBaseCodeFilesResource.funcoesH.Replace("#EXECCONTADOR_H#", "");


                if (bIndicaTemporizadorNoPrograma) /// TEMPORIZADOR
                    DadosArquivoFuncoesH = DadosArquivoFuncoesH.Replace("#EXECTEMPORIZADOR_H#", MicrocontrollersBaseCodeFilesResource.ExecTemporizador_funcoesH);
                else
                    DadosArquivoFuncoesH = DadosArquivoFuncoesH.Replace("#EXECTEMPORIZADOR_H#", "");

                msp430gcc.CreateFile("funcoes.h", DadosArquivoFuncoesH);


                /// Prepara USUARIO
                if (bIndicaTemporizadorNoPrograma)
                    DadosArquivoUsuarioH = MicrocontrollersBaseCodeFilesResource.usuarioH.Replace("#EXECTEMPORIZADORES_H#", MicrocontrollersBaseCodeFilesResource.ExecTemporizadores_usuarioH);
                else
                    DadosArquivoUsuarioH = MicrocontrollersBaseCodeFilesResource.usuarioH.Replace("#EXECTEMPORIZADORES_H#", "");

                msp430gcc.CreateFile("usuario.h", DadosArquivoUsuarioH);

                DadosArquivoUsuarioC = MicrocontrollersBaseCodeFilesResource.usuarioC;
                if (bIndicaTemporizadorNoPrograma)
                {
                    DadosArquivoUsuarioC = DadosArquivoUsuarioC.Replace("#EXECTEMPORIZADORES_C#", MicrocontrollersBaseCodeFilesResource.ExecTemporizadores_usuarioC);
                    DadosArquivoUsuarioC = DadosArquivoUsuarioC.Replace("#TEMPORIZADORES#", DadosTemporizadores);
                }
                else
                    DadosArquivoUsuarioC = DadosArquivoUsuarioC.Replace("#EXECTEMPORIZADORES_C#", "");

                DadosArquivoUsuarioC = DadosArquivoUsuarioC.Replace("#LADDER#", doc);
                DadosArquivoUsuarioC = DadosArquivoUsuarioC.Replace("#PARAMETROS#", DadosParametros);
                DadosArquivoUsuarioC.Trim();

                msp430gcc.CreateFile("usuario.c", DadosArquivoUsuarioC);
                msp430gcc.CompilesMsp430ViaGcc("usuario");


                /// Prepara MAIN
                DadosArquivoMainC = MicrocontrollersBaseCodeFilesResource.mainC;

                if (bGravarLadderNoExecutavel)
                {
                    txtCodigoInterpretavel.FinalizaCabecalho();
                    DadosCodigosInterpretaveis = "const unsigned char codigosInterpretaveis[" + txtCodigoInterpretavel.Length.ToString().Trim() + "] = {" + txtCodigoInterpretavel.ToString() + "};";
                    DadosArquivoMainC = DadosArquivoMainC.Replace("#CODIGOSINTERPRETAVEIS#", DadosCodigosInterpretaveis);
                }
                else
                    DadosArquivoMainC = DadosArquivoMainC.Replace("#CODIGOSINTERPRETAVEIS#", "");
                DadosArquivoMainC.Trim();

                /// criar classe para tratar codigos interpretaveis
                msp430gcc.CreateFile("codigos.txt", DadosCodigosInterpretaveis);


                if (bIndicaTemporizadorNoPrograma)
                {
                    DadosArquivoMainC = DadosArquivoMainC.Replace("#EXECTEMPORIZADORES_CHAMADA#", "ExecTemporizadores();");
                }
                else
                {
                    DadosArquivoMainC = DadosArquivoMainC.Replace("#EXECTEMPORIZADORES_CHAMADA#", "");
                }

                msp430gcc.CreateFile("main.c", DadosArquivoMainC);
                msp430gcc.CompilesMsp430ViaGcc("main");


                /// Prepara FUNCOES
                if (bIndicaContadorNoPrograma) /// CONTADOR
                {
                    DadosArquivoFuncoesC = MicrocontrollersBaseCodeFilesResource.funcoesC.Replace("#EXECCONTADOR_C#", MicrocontrollersBaseCodeFilesResource.ExecContador_funcoesC);

                    /// TIPOS DE TEMPORIZADORES USADOS
                    if (_lstTiposContadoresUsados.Contains(0))
                        DadosArquivoFuncoesC = DadosArquivoFuncoesC.Replace("#EXECCONTADOR_TIPO0#", MicrocontrollersBaseCodeFilesResource.ExecContador_Tipo0_funcoesC);
                    else
                        DadosArquivoFuncoesC = DadosArquivoFuncoesC.Replace("#EXECCONTADOR_TIPO0#", "");

                    if (_lstTiposContadoresUsados.Contains(1))
                        DadosArquivoFuncoesC = DadosArquivoFuncoesC.Replace("#EXECCONTADOR_TIPO1#", MicrocontrollersBaseCodeFilesResource.ExecContador_Tipo1_funcoesC);
                    else
                        DadosArquivoFuncoesC = DadosArquivoFuncoesC.Replace("#EXECCONTADOR_TIPO1#", "");
                }
                else
                    DadosArquivoFuncoesC = MicrocontrollersBaseCodeFilesResource.funcoesC.Replace("#EXECCONTADOR_C#", "");


                if (bIndicaTemporizadorNoPrograma) /// TEMPORIZADOR
                {
                    DadosArquivoFuncoesC = DadosArquivoFuncoesC.Replace("#EXECTEMPORIZADOR_C#", MicrocontrollersBaseCodeFilesResource.ExecTemporizador_funcoesC);

                    /// TIPOS DE TEMPORIZADORES USADOS
                    if (_lstTiposTemporizadoresUsados.Contains(0))
                        DadosArquivoFuncoesC = DadosArquivoFuncoesC.Replace("#EXECTEMPORIZADOR_TIPO0#", MicrocontrollersBaseCodeFilesResource.ExecTemporizador_Tipo0_funcoes);
                    else
                        DadosArquivoFuncoesC = DadosArquivoFuncoesC.Replace("#EXECTEMPORIZADOR_TIPO0#", "");

                    if (_lstTiposTemporizadoresUsados.Contains(1))
                        DadosArquivoFuncoesC = DadosArquivoFuncoesC.Replace("#EXECTEMPORIZADOR_TIPO1#", MicrocontrollersBaseCodeFilesResource.ExecTemporizador_Tipo1_funcoes);
                    else
                        DadosArquivoFuncoesC = DadosArquivoFuncoesC.Replace("#EXECTEMPORIZADOR_TIPO1#", "");
                }
                else
                {
                    DadosArquivoFuncoesC = DadosArquivoFuncoesC.Replace("#EXECTEMPORIZADOR_C#", "");
                }
                msp430gcc.CreateFile("funcoes.c", DadosArquivoFuncoesC);
                msp430gcc.CompilesMsp430ViaGcc("funcoes");


                /// Prepara SETUPHARDARE
                DadosArquivoSetupHardwareC = MicrocontrollersBaseCodeFilesResource.setupHardwareC.Replace("#SETUPIO#", DadosSetupIO);
                DadosArquivoSetupHardwareC = DadosArquivoSetupHardwareC.Replace("#LEENTRADAS#", DadosLeEntradas);
                DadosArquivoSetupHardwareC = DadosArquivoSetupHardwareC.Replace("#ESCREVESAIDAS#", DadosEscreveSaidas);
                msp430gcc.CreateFile("setuphardware.c", DadosArquivoSetupHardwareC);
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
                                    ((Address)instruction.GetOperand(0)).Counter.Tipo = (Int32)instruction.GetOperand(1);
                                    ((Address)instruction.GetOperand(0)).Counter.Preset = (Int32)instruction.GetOperand(2);
                                    ((Address)instruction.GetOperand(0)).Counter.Acumulado = (Int32)instruction.GetOperand(3);
                                }
                                else if (instruction.OpCode == OperationCode.Timer)
                                {
                                    ((Address)instruction.GetOperand(0)).Timer.Tipo = (Int32)instruction.GetOperand(1);
                                    ((Address)instruction.GetOperand(0)).Timer.Preset = (Int32)instruction.GetOperand(2);
                                    ((Address)instruction.GetOperand(0)).Timer.Acumulado = (Int32)instruction.GetOperand(3);
                                    ((Address)instruction.GetOperand(0)).Timer.BaseTempo = (Int32)instruction.GetOperand(4);
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
