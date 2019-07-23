using System;
using System.Collections.Generic;
using System.Text;



using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.ComponentModel;
using LadderApp.Formularios;
namespace LadderApp
{
    [XmlInclude(typeof(EnderecamentoPrograma))]
    [Serializable]

    public class ProgramaBasico
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
        public String Nome{
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

        public ProgramaBasico()
        {
        }

        public EnderecamentoPrograma endereco = new EnderecamentoPrograma();

        public DispositivoLadder dispositivo = null;

        private List<LinhaCompleta> linhasPrograma = new List<LinhaCompleta>();
        public List<LinhaCompleta> linhas
        {
            get { return linhasPrograma; }
        }

        [XmlIgnore]
        public List<EnderecamentoLadder> lstTemporizadoresUtilizados = new List<EnderecamentoLadder>();
        [XmlIgnore]
        public List<EnderecamentoLadder> lstContadoresUtilizados = new List<EnderecamentoLadder>();


        /// <summary>
        /// Insere uma linha no programa no final das linhas
        /// </summary>
        /// <param name="_lc">nova linha a ser inserida</param>
        /// <returns>indice da linha inserida</returns>
        public int InsereLinhaNoFinal(LinhaCompleta _lc)
        {
            linhasPrograma.Add(_lc);
            return (linhasPrograma.Count - 1);
        }

        /// <summary>
        /// Insere uma linha no programa na primeira linha (antes de todas)
        /// </summary>
        /// <param name="_lc">nova linha a ser inserida</param>
        /// <returns></returns>
        public int InsereLinhaNoInicio(LinhaCompleta _lc)
        {
            return InsereLinhaNoIndice(0, _lc);
        }

        public int InsereLinhaNoIndice(int linha, LinhaCompleta _lc)
        {
            if (linha > linhasPrograma.Count)
                linha = linhasPrograma.Count;

            if (linha < 0)
                linha = 0;

            linhasPrograma.Insert(linha, _lc);
            return linha;
        }

        public void ApagaLinha(int linha)
        {
            linhasPrograma[linha].ApagaLinha();
            linhasPrograma.RemoveAt(linha);
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
            foreach (EnderecamentoLadder _tmp in endereco.lstTemporizador)
            {
                if (_tmp.Temporizador.Reset == true)
                {
                    _tmp.Temporizador.Acumulado = 0;
                    _tmp.Valor = false;
                    _tmp.Temporizador.Reset = false;
                }

                switch (_tmp.Temporizador.Tipo)
                {
                    case 0: // TON - Contador Crescente
                        if (_tmp.Temporizador.EN && !_tmp.Temporizador.Reset)
                        {
                            _tmp.Temporizador.AcumuladoParcial++;
                            if (_tmp.Temporizador.AcumuladoParcial >= _tmp.Temporizador.PresetParcial)
                            {
                                _tmp.Temporizador.AcumuladoParcial = 0;
                                _tmp.Temporizador.Acumulado++;

                                if (_tmp.Temporizador.Acumulado >= _tmp.Temporizador.Preset)
                                {
                                    _tmp.Valor = true; /// DONE = true
                                    _tmp.Temporizador.Acumulado = _tmp.Temporizador.Preset;
                                }
                            }
                        }
                        else
                        {
                            _tmp.Valor = false; /// DONE = false
                            _tmp.Temporizador.Acumulado = 0;
                            _tmp.Temporizador.AcumuladoParcial = 0;
                            _tmp.Temporizador.Reset = false;
                        }
                        break;

                    case 1: // TOF - Contador Decrescente
                        if (_tmp.Temporizador.EN || _tmp.Temporizador.Reset)
                        {
                            _tmp.Valor = true; /// DONE = true
                            _tmp.Temporizador.Acumulado = 0;
                            _tmp.Temporizador.AcumuladoParcial = 0;
                            _tmp.Temporizador.Reset = false;
                        }
                        else
                        {
                            if (_tmp.Valor) // DN habilitado - temporizador contando
                                _tmp.Temporizador.AcumuladoParcial++;

                            if (_tmp.Temporizador.AcumuladoParcial >= _tmp.Temporizador.PresetParcial)
                            {
                                _tmp.Temporizador.AcumuladoParcial = 0;
                                _tmp.Temporizador.Acumulado++;
                            }

                            if (_tmp.Temporizador.Acumulado >= _tmp.Temporizador.Preset)
                            {
                                _tmp.Valor = false; /// DONE = false
                                _tmp.Temporizador.Acumulado = 0;
                                _tmp.Temporizador.AcumuladoParcial = 0;
                            }
                        }

                        break;

                    case 2: // RTO
                        if (_tmp.Temporizador.Reset)
                        {
                            _tmp.Valor = false; /// DONE = false
                            _tmp.Temporizador.Acumulado = 0;
                            _tmp.Temporizador.AcumuladoParcial = 0;
                        }

                        if (_tmp.Temporizador.EN)
                        {
                            _tmp.Temporizador.AcumuladoParcial++;
                            if (_tmp.Temporizador.AcumuladoParcial == _intPresetParcial)
                            {
                                _tmp.Temporizador.AcumuladoParcial = 0;

                                if (_tmp.Temporizador.Acumulado <= Int32.MaxValue)
                                {
                                    if (_tmp.Temporizador.Acumulado < _tmp.Temporizador.Preset)
                                        _tmp.Temporizador.Acumulado++;
                                    else
                                        _tmp.Temporizador.Acumulado = _tmp.Temporizador.Preset;

                                    if (_tmp.Temporizador.Acumulado >= _tmp.Temporizador.Preset)
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
        public EnderecamentoLadder auxToggleBitPulse = null;


        public void ExecutaSimuladoContadores(SimboloBasico _sb, EnderecamentoLadder _endContador)
        {

            switch (_endContador.Contador.Tipo)
	        {
	            case 0: // Contador Crescente
                    if (_endContador.Contador.Reset == true)
		            {
                        //_sb.setOperando(3, (Int32)0);
                        _endContador.Valor = false;
                        _endContador.Contador.Acumulado = 0;
                        _endContador.Contador.Reset = false;
                    }
                    if (_endContador.Contador.EN == true && _endContador.Contador.Pulso == true)
		            {
                        _endContador.Contador.Pulso = false;

                        if (_endContador.Contador.Acumulado <= Int32.MaxValue)
                        {
                            _endContador.Contador.Acumulado++;
                            if (_endContador.Contador.Acumulado >= _endContador.Contador.Preset)
                                _endContador.Valor = true;
                            else
                                _endContador.Valor = false;
                        }
		            }
		            break;

	            case 1: // Contador Decrescente
                    if (_endContador.Contador.Reset == true)
		            {
                        _endContador.Contador.Acumulado = _endContador.Contador.Preset;
                        _endContador.Valor = false;
                        _endContador.Contador.Reset = false;
                    }
                    if (_endContador.Contador.EN == true && _endContador.Contador.Pulso == true)
		            {
                        _endContador.Contador.Pulso = false;
                        if (_endContador.Contador.Acumulado > 0)
                        {
                            _endContador.Contador.Acumulado--;

                            if (_endContador.Contador.Acumulado == 0)
                                _endContador.Valor = true;
                            else
                                _endContador.Valor = false;
                        }
		            }
		            break;

	            default:
		            break;
	        }
            if (_endContador.Contador.EN == false)
                _endContador.Contador.Pulso = true;

        }


        public Boolean ExecutaLadderSimulado()
        {
            if (!VerificaPrograma())
                return false;

            Boolean bAuxValor = false;
            List<SuporteConjunto> op = new List<SuporteConjunto>();

            foreach (LinhaCompleta _lc in this.linhasPrograma)
            {
                /// cria a linha
                op.Add(new SuporteConjunto());
                foreach (SimboloBasico _sb in _lc.simbolos)
                {
                    switch (_sb.getCI())
                    {
                        case CodigosInterpretaveis.PARALELO_INICIAL:
                            /// cria o paralelo
                            op.Add(new SuporteConjunto());
                            /// cria o paralelo inicial
                            op.Add(new SuporteConjunto());

                            break;
                        case CodigosInterpretaveis.PARALELO_FINAL:
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
                        case CodigosInterpretaveis.PARALELO_PROXIMO:
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
                            switch (_sb.getCI())
                            {
                                case CodigosInterpretaveis.CONTATO_NA:
                                    bAuxValor = ((EnderecamentoLadder)_sb.getOperandos(0)).Valor;
                                    break;
                                case CodigosInterpretaveis.CONTATO_NF:
                                    bAuxValor = !((EnderecamentoLadder)_sb.getOperandos(0)).Valor;
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
                foreach (SimboloBasico _sb in _lc.saida)
                {
                    switch (_sb.getCI())
                    {
                        case CodigosInterpretaveis.BOBINA_SAIDA:
                        case CodigosInterpretaveis.TEMPORIZADOR:
                        case CodigosInterpretaveis.CONTADOR:
                        case CodigosInterpretaveis.RESET:

                            if (_sb.getCI() == CodigosInterpretaveis.BOBINA_SAIDA)
                                ((EnderecamentoLadder)_sb.getOperandos(0)).Valor = (bool)op[op.Count - 1].valor;
                            else if (_sb.getCI() == CodigosInterpretaveis.TEMPORIZADOR)
                            {
                                ((EnderecamentoLadder)_sb.getOperandos(0)).Temporizador.EN = (bool)op[op.Count - 1].valor;
                            }
                            else if (_sb.getCI() == CodigosInterpretaveis.CONTADOR)
                            {
                                ((EnderecamentoLadder)_sb.getOperandos(0)).Contador.EN = (bool)op[op.Count - 1].valor;
                                ExecutaSimuladoContadores(_sb, ((EnderecamentoLadder)_sb.getOperandos(0)));
                            }
                            else if (_sb.getCI() == CodigosInterpretaveis.RESET)
                            {
                                if ((bool)op[op.Count - 1].valor)
                                {
                                    switch (((EnderecamentoLadder)_sb.getOperandos(0)).TpEnderecamento)
                                    {
                                        case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_CONTADOR:
                                            ((EnderecamentoLadder)_sb.getOperandos(0)).Contador.Reset = true;
                                            ExecutaSimuladoContadores(_sb, ((EnderecamentoLadder)_sb.getOperandos(0)));
                                            break;

                                        case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_TEMPORIZADOR:
                                            ((EnderecamentoLadder)_sb.getOperandos(0)).Temporizador.Reset = true;
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

            GC.Collect();

            if (auxToggleBitPulse != null)
            {
                auxToggleBitPulse.Valor = auxToggleBitPulse.Valor == true ? false : true;
                auxToggleBitPulse = null;
            }

            return true;
        }

        private bool GravaSenhaNoLadder(ref CodigosInterpretaveis2Txt txtCodigoInterpretavel)
        {
            DialogResult _result;
            String _strSenha = "";
            frmSenha _frmSenha = new frmSenha();

            _frmSenha.Text = "Digite a nova senha:";
            _frmSenha.lblSenhaAtual.Text = "Nova senha:";

            for (int i = 0; i < 2; i++)
            {
                _result = _frmSenha.ShowDialog();

                if (_result == DialogResult.OK)
                {
                    _strSenha = _frmSenha.txtSenha.Text;
                    _frmSenha.txtSenha.Text = "";
                    _frmSenha.Text = "Confirme a nova senha:";
                    _frmSenha.lblSenhaAtual.Text = "Confirme a nova senha:";
                    _frmSenha.btnOK.DialogResult = DialogResult.Yes;
                }
                else if (_result != DialogResult.Yes)
                {
                    MessageBox.Show("Operação cancelada!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                else
                {
                    if (_strSenha != _frmSenha.txtSenha.Text)
                    {
                        MessageBox.Show("Operação cancelada!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                    else
                    {
                        txtCodigoInterpretavel.AddCabecalho();
                        txtCodigoInterpretavel.txtCabecalho.Add(CodigosInterpretaveis.CABECALHO_SENHA_0);
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
            CodigosInterpretaveis2Txt txtCodigoInterpretavel = new CodigosInterpretaveis2Txt();

            bool bIniciado = false;

            if (!VerificaPrograma())
                return false;

            //txtCodigoInterpretavel.Add("@laddermic.com");
            txtCodigoInterpretavel.Add(CodigosInterpretaveis.NENHUM);

            /// caso senha para inserir senha
            /// realiza recuperação da senha
            if (bGravarLadderNoExecutavel && bGravarSenha)
            {
                _result = MessageBox.Show("Tem certeza que deseja gravar uma senha no executável a ser gerado?", "Solicitar senha:", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                if (_result == DialogResult.Yes)
                {
                    if (!GravaSenhaNoLadder(ref txtCodigoInterpretavel))
                        return false;
                }
                else
                    return false;

            }


            endereco.LimpaIndicacaoEmUso();

            linha += Environment.NewLine;
            doc += linha;


            foreach (LinhaCompleta _lc in this.linhasPrograma)
            {
                linha = "";
                foreach(SimboloBasico _sb in _lc.simbolos)
                {
                    switch (_sb.getCI())
                    {
                        case CodigosInterpretaveis.PARALELO_INICIAL:
                            if (bIniciado)
                                linha += " && ";

                            bIniciado = false;
                            linha += "((";
                            break;
                        case CodigosInterpretaveis.PARALELO_FINAL:
                            linha += "))";
                            break;
                        case CodigosInterpretaveis.PARALELO_PROXIMO:
                            bIniciado = false;
                            linha += ") || (";
                            break;
                        default:
                            if (bIniciado)
                                linha += " && ";
                            switch(_sb.getCI())
                            {
                                case CodigosInterpretaveis.CONTATO_NA:
                                    linha += ((EnderecamentoLadder)_sb.getOperandos(0)).Acesso;
                                    ((EnderecamentoLadder)_sb.getOperandos(0)).EmUso = true;
                                    break;
                                case CodigosInterpretaveis.CONTATO_NF:
                                    linha += "!" + ((EnderecamentoLadder)_sb.getOperandos(0)).Acesso;
                                    ((EnderecamentoLadder)_sb.getOperandos(0)).EmUso = true;
                                    break;
                            }
                            bIniciado = true;
                            break;
                    }

                    txtCodigoInterpretavel.Add(_sb);
                }
                bIniciado = false;

                if (linha == "")
                    linha = "1";

                linha = "(" + linha + ")";

                linhaTeste = linha;

                OperandosLinha.Clear();
                OperandosSELinha.Clear();
                foreach (SimboloBasico _sb in _lc.saida)
                {
                    switch (_sb.getCI())
                    {
                        case CodigosInterpretaveis.BOBINA_SAIDA:
                        case CodigosInterpretaveis.TEMPORIZADOR:
                        case CodigosInterpretaveis.CONTADOR:
                        case CodigosInterpretaveis.RESET:
                            txtCodigoInterpretavel.Add(_sb);

                            if (_sb.getCI() == CodigosInterpretaveis.BOBINA_SAIDA)
                            {
                                OperandosLinha.Add(((EnderecamentoLadder)_sb.getOperandos(0)).Acesso);
                                ((EnderecamentoLadder)_sb.getOperandos(0)).EmUso = true;
                            }
                            else if (_sb.getCI() == CodigosInterpretaveis.TEMPORIZADOR)
                            {
                                OperandosLinha.Add(((EnderecamentoLadder)_sb.getOperandos(0)).Acesso2);
                                ((EnderecamentoLadder)_sb.getOperandos(0)).EmUso = true;
                            }
                            else if (_sb.getCI() == CodigosInterpretaveis.CONTADOR)
                            {
                                OperandosLinha.Add(((EnderecamentoLadder)_sb.getOperandos(0)).Acesso2);
                                FuncoesAposLinha += " ExecContador(&" + ((EnderecamentoLadder)_sb.getOperandos(0)).Nome + ");";
                                ((EnderecamentoLadder)_sb.getOperandos(0)).EmUso = true;
                            }
                            else if (_sb.getCI() == CodigosInterpretaveis.RESET)
                            {
                                OperandosSELinha.Add(((EnderecamentoLadder)_sb.getOperandos(0)).Nome + ".Reset = 1;");
                                ((EnderecamentoLadder)_sb.getOperandos(0)).EmUso = true;

                                switch (((EnderecamentoLadder)_sb.getOperandos(0)).TpEnderecamento)
                                {
                                    case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_CONTADOR:
                                        OperandosSELinha.Add("ExecContador(&" + ((EnderecamentoLadder)_sb.getOperandos(0)).Nome + ");");
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

                txtCodigoInterpretavel.Add(CodigosInterpretaveis.FIM_DA_LINHA);
            }

            txtCodigoInterpretavel.Add(CodigosInterpretaveis.NENHUM);

            _result = MessageBox.Show("Deseja gerar o arquivo .C a abaixo? " + Environment.NewLine + doc, "Gerar Arquivo .c ?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

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
                foreach (EnderecamentoLadder _endCada in endereco.lstIOEntrada)
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
                foreach (EnderecamentoLadder _endCada in endereco.lstIOSaida)
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
                    foreach (EnderecamentoLadder _endCada in endereco.lstMemoria)
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
                DadosParametros += "// parametros temporizadores" + Environment.NewLine;
                foreach (EnderecamentoLadder _endCada in endereco.lstTemporizador)
                {
                    if (_endCada.EmUso)
                    {
                        bIndicaTemporizadorNoPrograma = true;
                        DadosParametros += "\t" + _endCada.Nome + ".Tipo = " + _endCada.Temporizador.Tipo.ToString() + ";" + Environment.NewLine;
                        DadosParametros += "\t" + _endCada.Nome + ".Base = " + _endCada.Temporizador.BaseTempo.ToString() + ";" + Environment.NewLine;
                        DadosParametros += "\t" + _endCada.Nome + ".Preset = " + _endCada.Temporizador.Preset.ToString() + ";" + Environment.NewLine;
                        DadosParametros += "\t" + _endCada.Nome + ".Acumulado = 0;" + Environment.NewLine;
                        DadosParametros += Environment.NewLine;

                        /// prerapara a declaração dos endereços
                        if (!_lstEndUsados.Contains(_endCada.EnderecoRaiz))
                            _lstEndUsados.Add(_endCada.EnderecoRaiz);

                        /// prerapara verificação dos tipos de temporizadores usados
                        if (!_lstTiposTemporizadoresUsados.Contains(_endCada.Temporizador.Tipo))
                            _lstTiposTemporizadoresUsados.Add(_endCada.Temporizador.Tipo);
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
                foreach (EnderecamentoLadder _endCada in endereco.lstContador)
                {
                    if (_endCada.EmUso)
                    {
                        bIndicaContadorNoPrograma = true;
                        DadosParametros += "\t" + _endCada.Nome + ".Tipo = " + _endCada.Contador.Tipo.ToString() + ";" + Environment.NewLine;
                        DadosParametros += "\t" + _endCada.Nome + ".Preset = " + _endCada.Contador.Preset.ToString() + ";" + Environment.NewLine;
                        DadosParametros += "\t" + _endCada.Nome + ".Acumulado = 0;" + Environment.NewLine;
                        DadosParametros += Environment.NewLine;

                        /// prerapara a declaração dos endereços
                        if (!_lstEndUsados.Contains(_endCada.EnderecoRaiz))
                            _lstEndUsados.Add(_endCada.EnderecoRaiz);

                        /// prerapara verificação dos tipos de temporizadores usados
                        if (!_lstTiposContadoresUsados.Contains(_endCada.Contador.Tipo))
                            _lstTiposContadoresUsados.Add(_endCada.Contador.Tipo);
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

                ModuloIntegracaoMSP430 msp430gcc = new ModuloIntegracaoMSP430(false);

                /// Prepara ENDERECOS
                DadosArquivoEnderecosH = ArquivosFontesC.enderecosH;
                DadosArquivoEnderecosH = DadosArquivoEnderecosH.Replace("#ENDEREÇOS#", DadosEnderecos);
                DadosArquivoEnderecosH.Trim();

                msp430gcc.CriaArquivo("enderecos.h", DadosArquivoEnderecosH);


                /// Prepara DEFINICAO
                msp430gcc.CriaArquivo("definicao.h", ArquivosFontesC.definicaoH);


                /// Prepara SETUPHARDWARE
                msp430gcc.CriaArquivo("setuphardware.h", ArquivosFontesC.setupHardwareH);


                /// Prepara FUNCOES
                if (bIndicaContadorNoPrograma) /// CONTADOR
                    DadosArquivoFuncoesH = ArquivosFontesC.funcoesH.Replace("#EXECCONTADOR_H#", ArquivosFontesC.ExecContador_funcoesH);
                else
                    DadosArquivoFuncoesH = ArquivosFontesC.funcoesH.Replace("#EXECCONTADOR_H#", "");


                if (bIndicaTemporizadorNoPrograma) /// TEMPORIZADOR
                    DadosArquivoFuncoesH = DadosArquivoFuncoesH.Replace("#EXECTEMPORIZADOR_H#", ArquivosFontesC.ExecTemporizador_funcoesH);
                else
                    DadosArquivoFuncoesH = DadosArquivoFuncoesH.Replace("#EXECTEMPORIZADOR_H#", "");

                msp430gcc.CriaArquivo("funcoes.h", DadosArquivoFuncoesH);


                /// Prepara USUARIO
                if (bIndicaTemporizadorNoPrograma)
                    DadosArquivoUsuarioH = ArquivosFontesC.usuarioH.Replace("#EXECTEMPORIZADORES_H#", ArquivosFontesC.ExecTemporizadores_usuarioH);
                else
                    DadosArquivoUsuarioH = ArquivosFontesC.usuarioH.Replace("#EXECTEMPORIZADORES_H#", "");

                msp430gcc.CriaArquivo("usuario.h", DadosArquivoUsuarioH);

                DadosArquivoUsuarioC = ArquivosFontesC.usuarioC;
                if (bIndicaTemporizadorNoPrograma)
                {
                    DadosArquivoUsuarioC = DadosArquivoUsuarioC.Replace("#EXECTEMPORIZADORES_C#", ArquivosFontesC.ExecTemporizadores_usuarioC);
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
                DadosArquivoMainC = ArquivosFontesC.mainC;

                if (bGravarLadderNoExecutavel)
                {
                    txtCodigoInterpretavel.FinalizaCabecalho();
                    DadosCodigosInterpretaveis = "const unsigned char codigosInterpretaveis[" + txtCodigoInterpretavel.Length.ToString().Trim() + "] = {" + txtCodigoInterpretavel.ToString() + "};";
                    //DadosCodigosInterpretaveis += Environment.NewLine + "codigosInterpretaveis[" + (txtCodigoInterpretavel.Length - 1).ToString().Trim() + "] = (char)0;";
                    DadosArquivoMainC = DadosArquivoMainC.Replace("#CODIGOSINTERPRETAVEIS#", DadosCodigosInterpretaveis);
                    //DadosArquivo = DadosArquivo.Replace("#CODIGOSINTERPRETAVEIS#", "");
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
                    DadosArquivoFuncoesC = ArquivosFontesC.funcoesC.Replace("#EXECCONTADOR_C#", ArquivosFontesC.ExecContador_funcoesC);

                    /// TIPOS DE TEMPORIZADORES USADOS
                    if (_lstTiposContadoresUsados.Contains(0))
                        DadosArquivoFuncoesC = DadosArquivoFuncoesC.Replace("#EXECCONTADOR_TIPO0#", ArquivosFontesC.ExecContador_Tipo0_funcoesC);
                    else
                        DadosArquivoFuncoesC = DadosArquivoFuncoesC.Replace("#EXECCONTADOR_TIPO0#", "");

                    if (_lstTiposContadoresUsados.Contains(1))
                        DadosArquivoFuncoesC = DadosArquivoFuncoesC.Replace("#EXECCONTADOR_TIPO1#", ArquivosFontesC.ExecContador_Tipo1_funcoesC);
                    else
                        DadosArquivoFuncoesC = DadosArquivoFuncoesC.Replace("#EXECCONTADOR_TIPO1#", "");
                }
                else
                    DadosArquivoFuncoesC = ArquivosFontesC.funcoesC.Replace("#EXECCONTADOR_C#", "");


                if (bIndicaTemporizadorNoPrograma) /// TEMPORIZADOR
                {
                    DadosArquivoFuncoesC = DadosArquivoFuncoesC.Replace("#EXECTEMPORIZADOR_C#", ArquivosFontesC.ExecTemporizador_funcoesC);

                    /// TIPOS DE TEMPORIZADORES USADOS
                    if (_lstTiposTemporizadoresUsados.Contains(0))
                        DadosArquivoFuncoesC = DadosArquivoFuncoesC.Replace("#EXECTEMPORIZADOR_TIPO0#", ArquivosFontesC.ExecTemporizador_Tipo0_funcoes);
                    else
                        DadosArquivoFuncoesC = DadosArquivoFuncoesC.Replace("#EXECTEMPORIZADOR_TIPO0#", "");
                    
                    if (_lstTiposTemporizadoresUsados.Contains(1))
                        DadosArquivoFuncoesC = DadosArquivoFuncoesC.Replace("#EXECTEMPORIZADOR_TIPO1#", ArquivosFontesC.ExecTemporizador_Tipo1_funcoes);
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
                DadosArquivoSetupHardwareC = ArquivosFontesC.setupHardwareC.Replace("#SETUPIO#", DadosSetupIO);
                DadosArquivoSetupHardwareC = DadosArquivoSetupHardwareC.Replace("#LEENTRADAS#", DadosLeEntradas);
                DadosArquivoSetupHardwareC = DadosArquivoSetupHardwareC.Replace("#ESCREVESAIDAS#", DadosEscreveSaidas);
                msp430gcc.CriaArquivo("setuphardware.c", DadosArquivoSetupHardwareC);
                msp430gcc.CompilaMSP430gcc("setuphardware");

                
                /// Prepara INTERRUPCAO
                msp430gcc.CriaArquivo("interrupcao.c", ArquivosFontesC.interrupcaoC);
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
            foreach (LinhaCompleta _lc in this.linhasPrograma)
            {
                if (!this.VerificaLinha(_lc))
                    _bResult = false;
            }

            /// verifica todo o programa de forma interdependente

            return _bResult;
        }

        private bool VerificaLinha(LinhaCompleta _linha)
        {
            /// Verifica se a linha solicitada e valida
            //if (iLinha > 0 && iLinha < this.linhas.Count)
            //    return false;
            ListaSimbolo _lst = new ListaSimbolo();

            _lst.InsertAllWithClearBefore(_linha.saida);

            if (_lst.Count > 0)
            {
                if (!(_lst.Contains(CodigosInterpretaveis.BOBINA_SAIDA) ||
                    _lst.Contains(CodigosInterpretaveis.TEMPORIZADOR) ||
                    _lst.Contains(CodigosInterpretaveis.CONTADOR) ||
                    _lst.Contains(CodigosInterpretaveis.RESET)))
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

            _lst.InsertAllWithClearBefore(_linha.simbolos);

            /// 1.1 - Verifica se a linha tem simbolos validos
            if (_lst.Count > 0)
                if (_lst.Contains(CodigosInterpretaveis.BOBINA_SAIDA) ||
                    _lst.Contains(CodigosInterpretaveis.TEMPORIZADOR) ||
                    _lst.Contains(CodigosInterpretaveis.CONTADOR) ||
                    _lst.Contains(CodigosInterpretaveis.RESET))
                    return false;


            /// 2.2 - Verifica se todos os simbolos tem os operandos minimos atribuidos
            if (!_lst.ContainsAllOperandos())
                return false;
            
            return true;
        }

        // Reindexa endrereços da logica ladder
        public bool ReindexaEnderecos()
        {
            foreach (LinhaCompleta _lc in this.linhasPrograma)
            {
                foreach (SimboloBasico _sb in _lc.simbolos)
                {
                    switch (_sb.getCI())
                    {
                        case CodigosInterpretaveis.NENHUM:
                        case CodigosInterpretaveis.INICIO_DA_LINHA:
                        case CodigosInterpretaveis.FIM_DA_LINHA:
                        case CodigosInterpretaveis.PARALELO_INICIAL:
                        case CodigosInterpretaveis.PARALELO_FINAL:
                        case CodigosInterpretaveis.PARALELO_PROXIMO:
                            break;
                        default:
                            _sb.setOperando(0, endereco.Find((EnderecamentoLadder)_sb.getOperandos(0)));
                            break;
                    }
                }
                foreach (SimboloBasico _sb in _lc.saida)
                {
                    switch (_sb.getCI())
                    {
                        case CodigosInterpretaveis.NENHUM:
                        case CodigosInterpretaveis.INICIO_DA_LINHA:
                        case CodigosInterpretaveis.FIM_DA_LINHA:
                        case CodigosInterpretaveis.PARALELO_INICIAL:
                        case CodigosInterpretaveis.PARALELO_FINAL:
                        case CodigosInterpretaveis.PARALELO_PROXIMO:
                            break;
                        default:
                            _sb.setOperando(0, endereco.Find((EnderecamentoLadder)_sb.getOperandos(0)));

                            if (_sb.getOperandos(0) != null)
                            {
                                if (_sb.getCI() == CodigosInterpretaveis.CONTADOR)
                                {
                                    ((EnderecamentoLadder)_sb.getOperandos(0)).Contador.Tipo = (Int32)_sb.getOperandos(1);
                                    ((EnderecamentoLadder)_sb.getOperandos(0)).Contador.Preset = (Int32)_sb.getOperandos(2);
                                    ((EnderecamentoLadder)_sb.getOperandos(0)).Contador.Acumulado = (Int32)_sb.getOperandos(3);
                                }
                                else if (_sb.getCI() == CodigosInterpretaveis.TEMPORIZADOR)
                                {
                                    ((EnderecamentoLadder)_sb.getOperandos(0)).Temporizador.Tipo = (Int32)_sb.getOperandos(1);
                                    ((EnderecamentoLadder)_sb.getOperandos(0)).Temporizador.Preset = (Int32)_sb.getOperandos(2);
                                    ((EnderecamentoLadder)_sb.getOperandos(0)).Temporizador.Acumulado = (Int32)_sb.getOperandos(3);
                                    ((EnderecamentoLadder)_sb.getOperandos(0)).Temporizador.BaseTempo = (Int32)_sb.getOperandos(4);
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
