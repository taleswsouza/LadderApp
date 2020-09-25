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
        public enum StatusPrograma
        {
            NAOINICIADO,
            NOVO,
            ABERTO,
            SALVO
        }

        private class SuporteConjunto
        {
            public bool valor = true;
            public bool bIniciado = false;
        }

        private String nomePrograma = "";
        public String Nome
        {
            get
            {
                if (PathFile == "")
                {
                    if (nomePrograma != "")
                        return nomePrograma;
                    else
                        return "Sem nome";
                }
                else
                    return PathFile.Substring(PathFile.LastIndexOf(@"\") + 1, PathFile.Length - PathFile.LastIndexOf(@"\") - 1);
            }
            set { nomePrograma = value; }
        }
        [XmlIgnore]
        public String PathFile = "";

        private StatusPrograma stsprg = StatusPrograma.NAOINICIADO;
        public StatusPrograma StsPrograma
        {
            get { return stsprg; }
            set { stsprg = value; }
        }

        public LadderProgram()
        {
        }

        public Addressing addressing = new Addressing();

        public Device device;

        private List<Line> lines = new List<Line>();
        public List<Line> Lines
        {
            get { return lines; }
        }

        [XmlIgnore]
        public List<Address> lstTemporizadoresUtilizados = new List<Address>();
        [XmlIgnore]
        public List<Address> lstContadoresUtilizados = new List<Address>();


        /// <summary>
        /// Insere uma linha no programa no final das linhas
        /// </summary>
        /// <param name="_lc">nova linha a ser inserida</param>
        /// <returns>indice da linha inserida</returns>
        public int InsereLinhaNoFinal(Line _lc)
        {
            lines.Add(_lc);
            return (lines.Count - 1);
        }

        /// <summary>
        /// Insere uma linha no programa na primeira linha (antes de todas)
        /// </summary>
        /// <param name="_lc">nova linha a ser inserida</param>
        /// <returns></returns>
        public int InsereLinhaNoInicio(Line _lc)
        {
            return InsereLinhaNoIndice(0, _lc);
        }

        public int InsereLinhaNoIndice(int linha, Line _lc)
        {
            if (linha > lines.Count)
                linha = lines.Count;

            if (linha < 0)
                linha = 0;

            lines.Insert(linha, _lc);
            return linha;
        }

        public void ApagaLinha(int linha)
        {
            lines[linha].ApagaLinha();
            lines.RemoveAt(linha);
        }

        /// <summary>
        /// Lógica dos temporizadores
        /// </summary>
        public void ExecutaSimuladoTemporizadores()
        {
            /// faz a função de um preset parcial para acumular na base de tempo
            /// programada para o temporizador, utilizando a base de tempo da thread (100ms)
            Int32 _intPresetParcial = -1;

            /// executa a rotina para cada temporizador
            foreach (Address _tmp in addressing.ListTimerAddress)
            {
                if (_tmp.Timer.Reset == true)
                {
                    _tmp.Timer.Acumulado = 0;
                    _tmp.Valor = false;
                    _tmp.Timer.Reset = false;
                }

                switch (_tmp.Timer.Tipo)
                {
                    case 0: // TON - Contador Crescente
                        if (_tmp.Timer.EN && !_tmp.Timer.Reset)
                        {
                            _tmp.Timer.AcumuladoParcial++;
                            if (_tmp.Timer.AcumuladoParcial >= _tmp.Timer.PresetParcial)
                            {
                                _tmp.Timer.AcumuladoParcial = 0;
                                _tmp.Timer.Acumulado++;

                                if (_tmp.Timer.Acumulado >= _tmp.Timer.Preset)
                                {
                                    _tmp.Valor = true; /// DONE = true
                                    _tmp.Timer.Acumulado = _tmp.Timer.Preset;
                                }
                            }
                        }
                        else
                        {
                            _tmp.Valor = false; /// DONE = false
                            _tmp.Timer.Acumulado = 0;
                            _tmp.Timer.AcumuladoParcial = 0;
                            _tmp.Timer.Reset = false;
                        }
                        break;

                    case 1: // TOF - Contador Decrescente
                        if (_tmp.Timer.EN || _tmp.Timer.Reset)
                        {
                            _tmp.Valor = true; /// DONE = true
                            _tmp.Timer.Acumulado = 0;
                            _tmp.Timer.AcumuladoParcial = 0;
                            _tmp.Timer.Reset = false;
                        }
                        else
                        {
                            if (_tmp.Valor) // DN habilitado - temporizador contando
                                _tmp.Timer.AcumuladoParcial++;

                            if (_tmp.Timer.AcumuladoParcial >= _tmp.Timer.PresetParcial)
                            {
                                _tmp.Timer.AcumuladoParcial = 0;
                                _tmp.Timer.Acumulado++;
                            }

                            if (_tmp.Timer.Acumulado >= _tmp.Timer.Preset)
                            {
                                _tmp.Valor = false; /// DONE = false
                                _tmp.Timer.Acumulado = 0;
                                _tmp.Timer.AcumuladoParcial = 0;
                            }
                        }

                        break;

                    case 2: // RTO
                        if (_tmp.Timer.Reset)
                        {
                            _tmp.Valor = false; /// DONE = false
                            _tmp.Timer.Acumulado = 0;
                            _tmp.Timer.AcumuladoParcial = 0;
                        }

                        if (_tmp.Timer.EN)
                        {
                            _tmp.Timer.AcumuladoParcial++;
                            if (_tmp.Timer.AcumuladoParcial == _intPresetParcial)
                            {
                                _tmp.Timer.AcumuladoParcial = 0;

                                if (_tmp.Timer.Acumulado <= Int32.MaxValue)
                                {
                                    if (_tmp.Timer.Acumulado < _tmp.Timer.Preset)
                                        _tmp.Timer.Acumulado++;
                                    else
                                        _tmp.Timer.Acumulado = _tmp.Timer.Preset;

                                    if (_tmp.Timer.Acumulado >= _tmp.Timer.Preset)
                                        _tmp.Valor = true; /// DONE = true
                                    else
                                        _tmp.Valor = false; /// DONE = false
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


        public void ExecutaSimuladoContadores(Instruction instruction, Address _endContador)
        {

            switch (_endContador.Counter.Tipo)
            {
                case 0: // Contador Crescente
                    if (_endContador.Counter.Reset == true)
                    {
                        _endContador.Valor = false;
                        _endContador.Counter.Acumulado = 0;
                        _endContador.Counter.Reset = false;
                    }
                    if (_endContador.Counter.EN == true && _endContador.Counter.Pulso == true)
                    {
                        _endContador.Counter.Pulso = false;

                        if (_endContador.Counter.Acumulado <= Int32.MaxValue)
                        {
                            _endContador.Counter.Acumulado++;
                            if (_endContador.Counter.Acumulado >= _endContador.Counter.Preset)
                                _endContador.Valor = true;
                            else
                                _endContador.Valor = false;
                        }
                    }
                    break;

                case 1: // Contador Decrescente
                    if (_endContador.Counter.Reset == true)
                    {
                        _endContador.Counter.Acumulado = _endContador.Counter.Preset;
                        _endContador.Valor = false;
                        _endContador.Counter.Reset = false;
                    }
                    if (_endContador.Counter.EN == true && _endContador.Counter.Pulso == true)
                    {
                        _endContador.Counter.Pulso = false;
                        if (_endContador.Counter.Acumulado > 0)
                        {
                            _endContador.Counter.Acumulado--;

                            if (_endContador.Counter.Acumulado == 0)
                                _endContador.Valor = true;
                            else
                                _endContador.Valor = false;
                        }
                    }
                    break;

                default:
                    break;
            }
            if (_endContador.Counter.EN == false)
                _endContador.Counter.Pulso = true;

        }


        public Boolean ExecutaLadderSimulado()
        {
            if (!VerificaPrograma())
                return false;

            Boolean bAuxValor = false;
            List<SuporteConjunto> op = new List<SuporteConjunto>();

            foreach (Line _lc in this.lines)
            {
                /// cria a linha
                op.Add(new SuporteConjunto());
                foreach (Instruction instruction in _lc.instructions)
                {
                    switch (instruction.OpCode)
                    {
                        case OperationCode.ParallelBranchBegin:
                            /// cria o paralelo
                            op.Add(new SuporteConjunto());
                            /// cria o paralelo inicial
                            op.Add(new SuporteConjunto());

                            break;
                        case OperationCode.ParallelBranchEnd:
                            /// Atualiza o paralelo
                            if (op[op.Count - 2].bIniciado)
                                op[op.Count - 2].valor = op[op.Count - 2].valor || op[op.Count - 1].valor;
                            else
                                op[op.Count - 2].valor = op[op.Count - 1].valor;

                            /// remove o utlimo paralelo proximo
                            op.RemoveAt(op.Count - 1);

                            /// Atualiza o anterior ao paralelo
                            if (op[op.Count - 2].bIniciado)
                                op[op.Count - 2].valor = op[op.Count - 2].valor && op[op.Count - 1].valor;
                            else
                                op[op.Count - 2].valor = op[op.Count - 1].valor;

                            op[op.Count - 2].bIniciado = true;

                            /// remove o paralelo
                            op.RemoveAt(op.Count - 1);


                            break;
                        case OperationCode.ParallelBranchNext:
                            /// Atualiza o paralelo
                            if (op[op.Count - 2].bIniciado)
                                op[op.Count - 2].valor = op[op.Count - 2].valor || op[op.Count - 1].valor;
                            else
                                op[op.Count - 2].valor = op[op.Count - 1].valor;

                            op[op.Count - 2].bIniciado = true;

                            /// remove o paralelo inicial ou paralelo proximo anterior
                            op.RemoveAt(op.Count - 1);

                            /// cria novo paralelo proximo
                            op.Add(new SuporteConjunto());

                            break;
                        default:
                            switch (instruction.OpCode)
                            {
                                case OperationCode.NormallyOpenContact:
                                    bAuxValor = ((Address)instruction.GetOperand(0)).Valor;
                                    break;
                                case OperationCode.NormallyClosedContact:
                                    bAuxValor = !((Address)instruction.GetOperand(0)).Valor;
                                    break;
                            }

                            if (op[op.Count - 1].bIniciado)
                                op[op.Count - 1].valor = op[op.Count - 1].valor && bAuxValor;
                            else
                                op[op.Count - 1].valor = bAuxValor;

                            op[op.Count - 1].bIniciado = true;
                            break;
                    }
                }

                /// atribui o resultado final da linha nas saidas
                foreach (Instruction instruction in _lc.outputs)
                {
                    switch (instruction.OpCode)
                    {
                        case OperationCode.OutputCoil:
                        case OperationCode.Timer:
                        case OperationCode.Counter:
                        case OperationCode.Reset:

                            if (instruction.OpCode == OperationCode.OutputCoil)
                                ((Address)instruction.GetOperand(0)).Valor = (bool)op[op.Count - 1].valor;
                            else if (instruction.OpCode == OperationCode.Timer)
                            {
                                ((Address)instruction.GetOperand(0)).Timer.EN = (bool)op[op.Count - 1].valor;
                            }
                            else if (instruction.OpCode == OperationCode.Counter)
                            {
                                ((Address)instruction.GetOperand(0)).Counter.EN = (bool)op[op.Count - 1].valor;
                                ExecutaSimuladoContadores(instruction, ((Address)instruction.GetOperand(0)));
                            }
                            else if (instruction.OpCode == OperationCode.Reset)
                            {
                                if ((bool)op[op.Count - 1].valor)
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

                op.RemoveAt(op.Count - 1);
            }

            if (auxToggleBitPulse != null)
            {
                auxToggleBitPulse.Valor = auxToggleBitPulse.Valor == true ? false : true;
                auxToggleBitPulse = null;
            }

            return true;
        }

        private bool GravaSenhaNoLadder(ref OpCode2TextServices txtCodigoInterpretavel)
        {
            DialogResult _result;
            String _strSenha = "";
            PasswordForm _frmSenha = new PasswordForm();

            _frmSenha.Text = "Enter the new password:";
            _frmSenha.lblSenhaAtual.Text = "New password:";

            for (int i = 0; i < 2; i++)
            {
                _result = _frmSenha.ShowDialog();

                if (_result == DialogResult.OK)
                {
                    _strSenha = _frmSenha.txtSenha.Text;
                    _frmSenha.txtSenha.Text = "";
                    _frmSenha.Text = "Confirm the new password:";
                    _frmSenha.lblSenhaAtual.Text = "Confirm the new password:";
                    _frmSenha.btnOK.DialogResult = DialogResult.Yes;
                }
                else if (_result != DialogResult.Yes)
                {
                    MessageBox.Show("Operation canceled!", "LadderApp", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                else
                {
                    if (_strSenha != _frmSenha.txtSenha.Text)
                    {
                        MessageBox.Show("Operation canceled!", "LadderApp", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                    else
                    {
                        txtCodigoInterpretavel.AddCabecalho();
                        txtCodigoInterpretavel.txtCabecalho.Add(OperationCode.HeadPassword0);
                        txtCodigoInterpretavel.txtCabecalho.Add(_strSenha.Length);
                        txtCodigoInterpretavel.txtCabecalho.Add(_strSenha);
                    }
                }
            }
            return true;
        }

        public bool GeraExecutavel(bool bGravarLadderNoExecutavel, bool bGravarSenha, bool bEscreverPrograma)
        {
            String doc = "", linha = "", linhaTeste = "", saidaUltimoOperando = "";
            //int numCodigosInterpretaveis = 0;
            bool bOperandosLinha = false;
            List<String> OperandosLinha = new List<string>();
            List<String> OperandosSELinha = new List<string>();
            String FuncoesAposLinha = "";
            DialogResult _result;
            OpCode2TextServices txtCodigoInterpretavel = new OpCode2TextServices();

            bool bIniciado = false;

            if (!VerificaPrograma())
                return false;

            //txtCodigoInterpretavel.Add("@laddermic.com");
            txtCodigoInterpretavel.Add(OperationCode.None);

            /// caso senha para inserir senha
            /// realiza recuperação da senha
            if (bGravarLadderNoExecutavel && bGravarSenha)
            {
                _result = MessageBox.Show("Are you sure you want to write a password to the executable to be generated?", "Request password", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                if (_result == DialogResult.Yes)
                {
                    if (!GravaSenhaNoLadder(ref txtCodigoInterpretavel))
                        return false;
                }
                else
                    return false;

            }


            addressing.LimpaIndicacaoEmUso();

            linha += Environment.NewLine;
            doc += linha;


            foreach (Line _lc in this.lines)
            {
                linha = "";
                foreach (Instruction instruction in _lc.instructions)
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
                                    ((Address)instruction.GetOperand(0)).EmUso = true;
                                    break;
                                case OperationCode.NormallyClosedContact:
                                    linha += "!" + ((Address)instruction.GetOperand(0)).Acesso;
                                    ((Address)instruction.GetOperand(0)).EmUso = true;
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
                foreach (Instruction instruction in _lc.outputs)
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
                                ((Address)instruction.GetOperand(0)).EmUso = true;
                            }
                            else if (instruction.OpCode == OperationCode.Timer)
                            {
                                OperandosLinha.Add(((Address)instruction.GetOperand(0)).Acesso2);
                                ((Address)instruction.GetOperand(0)).EmUso = true;
                            }
                            else if (instruction.OpCode == OperationCode.Counter)
                            {
                                OperandosLinha.Add(((Address)instruction.GetOperand(0)).Acesso2);
                                FuncoesAposLinha += " ExecContador(&" + ((Address)instruction.GetOperand(0)).Name + ");";
                                ((Address)instruction.GetOperand(0)).EmUso = true;
                            }
                            else if (instruction.OpCode == OperationCode.Reset)
                            {
                                OperandosSELinha.Add(((Address)instruction.GetOperand(0)).Name + ".Reset = 1;");
                                ((Address)instruction.GetOperand(0)).EmUso = true;

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

            _result = MessageBox.Show("Do you want to generate the .C file below? " + Environment.NewLine + doc, "Confirmation: Generate .C file?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (_result == DialogResult.OK)
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
                List<String> _lstPortasUsadas = new List<String>();
                List<Int32> _lstTiposTemporizadoresUsados = new List<Int32>();
                List<Int32> _lstTiposContadoresUsados = new List<Int32>();

                /// 1. prepara a configuração para as portas de entrada.
                /// 2. levantamento das portas que foram usadas no programa
                foreach (Address _endCada in addressing.ListInputAddress)
                    if (_endCada.Parametro != "" && _endCada.EmUso == true)
                    {
                        bIndicaEntradaUsadaNoPrograma = true;
                        /// 1.1. Adiciona os parametros dos endereços usados no programa
                        DadosParametros += "\t" + _endCada.Parametro + ";" + Environment.NewLine;

                        /// 2.1. prerapara a declaração dos endereços
                        if (!_lstPortasUsadas.Contains(_endCada.EnderecoRaiz))
                            _lstPortasUsadas.Add(_endCada.EnderecoRaiz);
                    }
                DadosParametros += Environment.NewLine;

                /// 1. prepara a configuração para as portas de saida.
                /// 2. levantamento das portas que foram usadas no programa
                foreach (Address _endCada in addressing.ListOutputAddress)
                    if (_endCada.Parametro != "" && _endCada.EmUso == true)
                    {
                        bIndicaSaidaUsadaNoPrograma = true;
                        /// 1.1. Adiciona os parametros dos endereços usados no programa
                        DadosParametros += "\t" + _endCada.Parametro + ";" + Environment.NewLine;

                        /// 2.1. prerapara a declaração dos endereços
                        if (!_lstPortasUsadas.Contains(_endCada.EnderecoRaiz))
                            _lstPortasUsadas.Add(_endCada.EnderecoRaiz);
                    }
                DadosParametros += Environment.NewLine;

                /// prepara a declaração das portas que foram usadas no programa
                for (int i = 0; i < _lstPortasUsadas.Count; i++)
                    _lstEndUsados.Add(_lstPortasUsadas[i] + "_IN, " + _lstPortasUsadas[i] + "_OUT, " + _lstPortasUsadas[i] + "_DIR");

                for (int i = 0; i < _lstPortasUsadas.Count; i++)
                {
                    /// Escreve rotina SetupIO(void)
                    DadosSetupIO += _lstPortasUsadas[i] + "OUT = 0; // Init Output data of port" + Environment.NewLine;
                    DadosSetupIO += _lstPortasUsadas[i] + "DIR = " + _lstPortasUsadas[i] + "_DIR.Byte; // Init of Port1 Data-Direction Reg (Out=1 / Inp=0)" + Environment.NewLine;
                    DadosSetupIO += _lstPortasUsadas[i] + "SEL = 0; // Port-Modules:" + Environment.NewLine;
                    DadosSetupIO += _lstPortasUsadas[i] + "IE = 0; // Interrupt Enable (0=dis 1=enabled)" + Environment.NewLine;
                    DadosSetupIO += _lstPortasUsadas[i] + "IES = 0; // Interrupt Edge Select (0=pos 1=neg)" + Environment.NewLine;
                    DadosSetupIO += Environment.NewLine;

                    if (bIndicaEntradaUsadaNoPrograma)
                    {
                        /// Escreve rotina LeEntradas(void)
                        DadosLeEntradas += _lstPortasUsadas[i] + "_IN.Byte = " + _lstPortasUsadas[i] + "IN;" + Environment.NewLine;
                    }

                    if (bIndicaSaidaUsadaNoPrograma)
                    {
                        /// Escreve rotina LEscreveSaidas(void)
                        DadosEscreveSaidas += _lstPortasUsadas[i] + "OUT = " + _lstPortasUsadas[i] + "_OUT.Byte; // Write Output data of port1" + Environment.NewLine;
                    }
                }

                /// prepara composição de parametros e declaração de variáveis
                foreach (Address _endCada in addressing.ListMemoryAddress)
                    if (_endCada.EmUso)
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
                    if (_endCada.EmUso)
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
                    if (_endCada.EmUso)
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

                msp430gcc.CriaArquivo("enderecos.h", DadosArquivoEnderecosH);


                /// Prepara DEFINICAO
                msp430gcc.CriaArquivo("definicao.h", MicrocontrollersBaseCodeFilesResource.definicaoH);


                /// Prepara SETUPHARDWARE
                msp430gcc.CriaArquivo("setuphardware.h", MicrocontrollersBaseCodeFilesResource.setupHardwareH);


                /// Prepara FUNCOES
                if (bIndicaContadorNoPrograma) /// CONTADOR
                    DadosArquivoFuncoesH = MicrocontrollersBaseCodeFilesResource.funcoesH.Replace("#EXECCONTADOR_H#", MicrocontrollersBaseCodeFilesResource.ExecContador_funcoesH);
                else
                    DadosArquivoFuncoesH = MicrocontrollersBaseCodeFilesResource.funcoesH.Replace("#EXECCONTADOR_H#", "");


                if (bIndicaTemporizadorNoPrograma) /// TEMPORIZADOR
                    DadosArquivoFuncoesH = DadosArquivoFuncoesH.Replace("#EXECTEMPORIZADOR_H#", MicrocontrollersBaseCodeFilesResource.ExecTemporizador_funcoesH);
                else
                    DadosArquivoFuncoesH = DadosArquivoFuncoesH.Replace("#EXECTEMPORIZADOR_H#", "");

                msp430gcc.CriaArquivo("funcoes.h", DadosArquivoFuncoesH);


                /// Prepara USUARIO
                if (bIndicaTemporizadorNoPrograma)
                    DadosArquivoUsuarioH = MicrocontrollersBaseCodeFilesResource.usuarioH.Replace("#EXECTEMPORIZADORES_H#", MicrocontrollersBaseCodeFilesResource.ExecTemporizadores_usuarioH);
                else
                    DadosArquivoUsuarioH = MicrocontrollersBaseCodeFilesResource.usuarioH.Replace("#EXECTEMPORIZADORES_H#", "");

                msp430gcc.CriaArquivo("usuario.h", DadosArquivoUsuarioH);

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

                msp430gcc.CriaArquivo("usuario.c", DadosArquivoUsuarioC);
                msp430gcc.CompilaMSP430gcc("usuario");


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
                msp430gcc.CriaArquivo("codigos.txt", DadosCodigosInterpretaveis);


                if (bIndicaTemporizadorNoPrograma)
                {
                    DadosArquivoMainC = DadosArquivoMainC.Replace("#EXECTEMPORIZADORES_CHAMADA#", "ExecTemporizadores();");
                }
                else
                {
                    DadosArquivoMainC = DadosArquivoMainC.Replace("#EXECTEMPORIZADORES_CHAMADA#", "");
                }

                msp430gcc.CriaArquivo("main.c", DadosArquivoMainC);
                msp430gcc.CompilaMSP430gcc("main");


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
                msp430gcc.CriaArquivo("funcoes.c", DadosArquivoFuncoesC);
                msp430gcc.CompilaMSP430gcc("funcoes");


                /// Prepara SETUPHARDARE
                DadosArquivoSetupHardwareC = MicrocontrollersBaseCodeFilesResource.setupHardwareC.Replace("#SETUPIO#", DadosSetupIO);
                DadosArquivoSetupHardwareC = DadosArquivoSetupHardwareC.Replace("#LEENTRADAS#", DadosLeEntradas);
                DadosArquivoSetupHardwareC = DadosArquivoSetupHardwareC.Replace("#ESCREVESAIDAS#", DadosEscreveSaidas);
                msp430gcc.CriaArquivo("setuphardware.c", DadosArquivoSetupHardwareC);
                msp430gcc.CompilaMSP430gcc("setuphardware");


                /// Prepara INTERRUPCAO
                msp430gcc.CriaArquivo("interrupcao.c", MicrocontrollersBaseCodeFilesResource.interrupcaoC);
                msp430gcc.CompilaMSP430gcc("interrupcao");

                /// CRIA ELF
                msp430gcc.CompilaELF(this.Nome);

                /// CRIA EXECUTAVE E GRAVA NO DISPOSITIVO
                msp430gcc.CompilaA43(this.Nome);

                if (bEscreverPrograma)
                    msp430gcc.GravaViaUSB(this.Nome);
            }

            return true;
        }

        public bool VerificaPrograma()
        {
            bool _bResult = true;
            lstContadoresUtilizados.Clear();
            lstTemporizadoresUtilizados.Clear();

            /// Verifica cada linha de forma independente
            foreach (Line _lc in this.lines)
            {
                if (!this.VerificaLinha(_lc))
                    _bResult = false;
            }

            return _bResult;
        }

        private bool VerificaLinha(Line _linha)
        {
            InstructionList _lst = new InstructionList();

            _lst.InsertAllWithClearBefore(_linha.outputs);

            if (_lst.Count > 0)
            {
                if (!(_lst.Contains(OperationCode.OutputCoil) ||
                    _lst.Contains(OperationCode.Timer) ||
                    _lst.Contains(OperationCode.Counter) ||
                    _lst.Contains(OperationCode.Reset)))
                    return false;
            }
            else
                return false;


            /// 2.1 - Verifica se todos os simbolos tem os operandos minimos atribuidos
            if (!_lst.ContainsAllOperandos())
                return false;

            if (!_lst.ExisteTemporizadorDuplicado(lstTemporizadoresUtilizados))
                return false;

            if (!_lst.ExisteContadorDuplicado(lstContadoresUtilizados))
                return false;

            _lst.InsertAllWithClearBefore(_linha.instructions);

            /// 1.1 - Verifica se a linha tem simbolos validos
            if (_lst.Count > 0)
                if (_lst.Contains(OperationCode.OutputCoil) ||
                    _lst.Contains(OperationCode.Timer) ||
                    _lst.Contains(OperationCode.Counter) ||
                    _lst.Contains(OperationCode.Reset))
                    return false;


            /// 2.2 - Verifica se todos os simbolos tem os operandos minimos atribuidos
            if (!_lst.ContainsAllOperandos())
                return false;

            return true;
        }

        // Reindexa endrereços da logica ladder
        public bool ReindexaEnderecos()
        {
            foreach (Line _lc in this.lines)
            {
                foreach (Instruction instruction in _lc.instructions)
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
                foreach (Instruction instruction in _lc.outputs)
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
