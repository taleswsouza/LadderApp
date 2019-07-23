using System;
using System.Collections.Generic;
using System.Text;
using LadderApp1.Exceções;

namespace LadderApp1.CodigoInterpretavel
{
    public class Txt2CodigosInterpretaveis
    {
        private String DadosConvertidosChar = "";

        /// <summary>
        /// Flags
        /// </summary>
        private bool bCabecalhoValidado = false;
        public bool bSolicitarSenha = false;
        public String strSenha = "";

        private Int32 posAtualLeitura = -1;
        public Int32 PosAtual
        {
            get
            {
                if (ExisteCodigoInterpretavel())
                {
                    //if (posAtualLeitura <= 0)
                    //    posAtualLeitura = PosInicial;
                    //else
                    //    posAtualLeitura++;
                }
                return posAtualLeitura;
            }
        }

        private Int32 intTamanhoCabecalho = -1;

        const String IdCodigo = "@laddermic.com";

        public Txt2CodigosInterpretaveis(String DadosConvertidosChar)
        {
            this.DadosConvertidosChar = DadosConvertidosChar;
        }

        public Int32 PosInicial
        {
            get {
                if (this.ExisteCodigoInterpretavel())
                {
                    Int32 posIncial = DadosConvertidosChar.IndexOf(IdCodigo) + IdCodigo.Length;

                    if (bCabecalhoValidado)
                        posIncial = posIncial + intTamanhoCabecalho + 2;

                    return posIncial;
                }
                else
                    return -1;
            }
        }

        public bool ExisteCodigoInterpretavel()
        {
            if (DadosConvertidosChar.IndexOf(IdCodigo) != -1)
                return true;
            else
                return false;
        }

        public bool ExisteCabecalho()
        {
            if (ExisteCodigoInterpretavel())
                if (LeCodigoInterpretavel(PosInicial) == CodigosInterpretaveis.CABECALHO_TAMANHO)
                {
                    intTamanhoCabecalho = (Int32)Convert.ToChar(DadosConvertidosChar.Substring(PosInicial + 1, 1));
                    return true;
                }

            return false;
        }

        public void ObtemInformacoesCabecalho()
        {
            int intTamanhoSenha = 0;
            if (ExisteCodigoInterpretavel())
                if (ExisteCabecalho())
                {
                    for (int i = PosInicial + 2; i <= PosInicial + intTamanhoCabecalho + 2; i++)
                        switch (LeCodigoInterpretavel(i))
                        {
                            case CodigosInterpretaveis.CABECALHO_SENHA_0:
                                bSolicitarSenha = true;
                                i++;
                                intTamanhoSenha = LeInteiro(i);
                                i++;

                                strSenha = DadosConvertidosChar.Substring(i, intTamanhoSenha);
                                i += intTamanhoSenha;
                                break;
                            default:
                                break;
                        }
                }

            bCabecalhoValidado = true;
        }

        public CodigosInterpretaveis LeCodigoInterpretavel(Int32 _pos)
        {
            //if (ExisteCodigoInterpretavel())
            {
                try
                {
                    return (CodigosInterpretaveis)Convert.ToChar(DadosConvertidosChar.Substring(_pos, 1));
                }
                catch
                {
                    throw new NaoECodigoInterpretavelException();
                }
            }

            //return CodigosInterpretaveis.NENHUM;
        }

        public TipoEnderecamentoDispositivo LeTipoEnderecamento(Int32 _pos)
        {
            if (ExisteCodigoInterpretavel())
            {
                try
                {
                    return (TipoEnderecamentoDispositivo)Convert.ToChar(DadosConvertidosChar.Substring(_pos, 1));
                }
                catch
                {
                    throw new Exception();
                }
            }

            return TipoEnderecamentoDispositivo.NENHUM;
        }

        public Int32 LeInteiro(Int32 _pos)
        {
            if (ExisteCodigoInterpretavel())
            {
                try
                {
                    return (Int32)Convert.ToChar(DadosConvertidosChar.Substring(_pos, 1));
                }
                catch
                {
                    throw new Exception();
                }
            }

            return -1;
        }
        
        public EnderecamentoLadder LeEndereco(ref Int32 _pos, EnderecamentoPrograma enderecamento)
        {
            EnderecamentoLadder _endLido = null;
            TipoEnderecamentoDispositivo _tpEndLido;
            Int32 _iIndiceEndLido = 0;
            SimboloBasico _sb = null;

            _sb = new SimboloBasico(LeCodigoInterpretavel(_pos));

            switch (_sb.getCI())
            {
                case CodigosInterpretaveis.NENHUM:
                    break;
                case CodigosInterpretaveis.FIM_DA_LINHA:
                    break;
                case CodigosInterpretaveis.CONTATO_NA:
                case CodigosInterpretaveis.CONTATO_NF:
                    _tpEndLido = LeTipoEnderecamento(_pos); _pos++;
                    _iIndiceEndLido = LeInteiro(_pos); _pos++;

                    _endLido = enderecamento.Find(_tpEndLido, _iIndiceEndLido);

                    //if (_endLido == null)
                    //{
                    //    programa.dispositivo.lstBitPorta[_iIndiceEndLido - 1].TipoDefinido = _tpEndLido;
                    //    programa.dispositivo.RealocaEnderecoDispositivo();
                    //    programa.endereco.AlocaEnderecamentoIO(programa.dispositivo);
                    //    _endLido = programa.endereco.Find(_tpEndLido, _iIndiceEndLido);
                    //}
                    //_sb.setOperando(0, _endLido);
                    break;
                case CodigosInterpretaveis.BOBINA_SAIDA:
                case CodigosInterpretaveis.RESET:
                    break;
                case CodigosInterpretaveis.PARALELO_INICIAL:
                case CodigosInterpretaveis.PARALELO_FINAL:
                case CodigosInterpretaveis.PARALELO_PROXIMO:
                    break;
                case CodigosInterpretaveis.CONTADOR:
                    break;
                case CodigosInterpretaveis.TEMPORIZADOR:
                    break;
            }

            return _endLido;
        }

        public Int32 NumeroOperandos(CodigosInterpretaveis _ci)
        {
            int iNumOperandos = -1;
            switch (_ci)
            {
                case CodigosInterpretaveis.NENHUM:
                    iNumOperandos = 0;
                    break;
                case CodigosInterpretaveis.FIM_DA_LINHA:
                    iNumOperandos = 0;
                    break;
                case CodigosInterpretaveis.CONTATO_NA:
                case CodigosInterpretaveis.CONTATO_NF:
                    iNumOperandos = 2;
                    break;
                case CodigosInterpretaveis.BOBINA_SAIDA:
                case CodigosInterpretaveis.RESET:
                    iNumOperandos = 2;
                    break;
                case CodigosInterpretaveis.PARALELO_INICIAL:
                case CodigosInterpretaveis.PARALELO_FINAL:
                case CodigosInterpretaveis.PARALELO_PROXIMO:
                    iNumOperandos = 0;
                    break;
                case CodigosInterpretaveis.CONTADOR:
                    iNumOperandos = 3;
                    break;
                case CodigosInterpretaveis.TEMPORIZADOR:
                    iNumOperandos = 4;
                    break;
                default:
                    iNumOperandos = 0;
                    break;
            }

            return iNumOperandos;
        }
        
        private ProgramaBasico LerExecutavel(String strNomeProjeto)
        {
            List<int> lstCodigosLidos = new List<int>();
            CodigosInterpretaveis guarda = CodigosInterpretaveis.NENHUM;

            if (this.ExisteCodigoInterpretavel())
            {
                Int32 intContaFim = 0;
                Int32 intIndiceLinha = 0;
                Int32 iNumOperandos = 0;
                EnderecamentoLadder _endLido;
                TipoEnderecamentoDispositivo _tpEndLido;
                Int32 _iIndiceEndLido = 0;


                //if (ExisteCabecalho())
                //{
                //    /// trata informações do cabecalho
                //    if (bSolicitarSenha)

                //}

                /// Cria um programa novo vazio
                ProgramaBasico programa = new ProgramaBasico();
                programa.StsPrograma = ProgramaBasico.StatusPrograma.NOVO;
                programa.Nome = strNomeProjeto;
                programa.dispositivo = new DispositivoLadder(1);
                programa.endereco.AlocaEnderecamentoIO(programa.dispositivo);
                programa.endereco.AlocaEnderecamentoMemoria(programa.dispositivo, programa.endereco.lstMemoria, TipoEnderecamentoDispositivo.DIGITAL_MEMORIA, 10);
                programa.endereco.AlocaEnderecamentoMemoria(programa.dispositivo, programa.endereco.lstTemporizador, TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_TEMPORIZADOR, 10);
                programa.endereco.AlocaEnderecamentoMemoria(programa.dispositivo, programa.endereco.lstContador, TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_CONTADOR, 10);
                intIndiceLinha = programa.InsereLinhaNoFinal(new LinhaCompleta());

                for (int i = this.PosInicial; i < DadosConvertidosChar.Length; i++)
                {
                    //guarda = (CodigosInterpretaveis)Convert.ToChar(DadosConvertidosChar.Substring(i, 1));
                    guarda = LeCodigoInterpretavel(i); i++;

                    switch (guarda)
                    {
                        case CodigosInterpretaveis.NENHUM:
                            intContaFim++;
                            iNumOperandos = 0;
                            break;
                        case CodigosInterpretaveis.FIM_DA_LINHA:
                            intContaFim++;
                            iNumOperandos = 0;
                            if (LeCodigoInterpretavel(i + 1) != CodigosInterpretaveis.NENHUM)
                                intIndiceLinha = programa.InsereLinhaNoFinal(new LinhaCompleta());
                            break;
                        //case CodigosInterpretaveis.INICIO_DA_LINHA:
                        case CodigosInterpretaveis.CONTATO_NA:
                        case CodigosInterpretaveis.CONTATO_NF:
                            intContaFim = 0;
                            iNumOperandos = 2;
                            {
                                SimboloBasico _sb = new SimboloBasico((CodigosInterpretaveis)guarda);
                                //_sb.setOperando(0, programa.endereco.Find((TipoEnderecamentoDispositivo)Convert.ToChar(DadosConvertidosChar.Substring(i + 1, 1)), (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 2, 1))));

                                _tpEndLido = LeTipoEnderecamento(i); i++;
                                _iIndiceEndLido = LeInteiro(i); i++;
                                _endLido = programa.endereco.Find(_tpEndLido, _iIndiceEndLido);
                                if (_endLido == null)
                                {
                                    programa.dispositivo.lstBitPorta[_iIndiceEndLido - 1].TipoDefinido = _tpEndLido;
                                    programa.dispositivo.RealocaEnderecoDispositivo();
                                    programa.endereco.AlocaEnderecamentoIO(programa.dispositivo);
                                    _endLido = programa.endereco.Find(_tpEndLido, _iIndiceEndLido);
                                }
                                _sb.setOperando(0, _endLido);

                                //i += 2;
                                programa.linhas[intIndiceLinha].simbolos.Add(_sb);
                            }
                            break;
                        case CodigosInterpretaveis.BOBINA_SAIDA:
                        case CodigosInterpretaveis.RESET:
                            intContaFim = 0;
                            iNumOperandos = 2;
                            {
                                ListaSimbolo _lstSB = new ListaSimbolo();
                                _lstSB.Add(new SimboloBasico((CodigosInterpretaveis)guarda));
                                _tpEndLido = (TipoEnderecamentoDispositivo)Convert.ToChar(DadosConvertidosChar.Substring(i + 1, 1));
                                _iIndiceEndLido = (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 2, 1));
                                _endLido = programa.endereco.Find(_tpEndLido, _iIndiceEndLido);
                                if (_endLido == null)
                                {
                                    programa.dispositivo.lstBitPorta[_iIndiceEndLido - 1].TipoDefinido = _tpEndLido;
                                    programa.dispositivo.RealocaEnderecoDispositivo();
                                    programa.endereco.AlocaEnderecamentoIO(programa.dispositivo);
                                    _endLido = programa.endereco.Find(_tpEndLido, _iIndiceEndLido);
                                }
                                _lstSB[_lstSB.Count - 1].setOperando(0, _endLido);
                                i += 2;
                                programa.linhas[intIndiceLinha].Insere2Saida(_lstSB);
                                _lstSB.Clear();
                            }
                            break;
                        case CodigosInterpretaveis.PARALELO_INICIAL:
                        case CodigosInterpretaveis.PARALELO_FINAL:
                        case CodigosInterpretaveis.PARALELO_PROXIMO:
                            intContaFim = 0;
                            iNumOperandos = 0;
                            programa.linhas[intIndiceLinha].simbolos.Add(new SimboloBasico((CodigosInterpretaveis)guarda));
                            break;
                        case CodigosInterpretaveis.CONTADOR:
                            intContaFim = 0;
                            iNumOperandos = 3;
                            {
                                ListaSimbolo _lstSB = new ListaSimbolo();
                                _lstSB.Add(new SimboloBasico((CodigosInterpretaveis)guarda));
                                _lstSB[_lstSB.Count - 1].setOperando(0, programa.endereco.Find(TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_CONTADOR, (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 1, 1))));
                                ((EnderecamentoLadder)_lstSB[_lstSB.Count - 1].getOperandos(0)).Contador.Tipo = (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 2, 1));
                                ((EnderecamentoLadder)_lstSB[_lstSB.Count - 1].getOperandos(0)).Contador.Preset = (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 3, 1));

                                _lstSB[_lstSB.Count - 1].setOperando(1, ((EnderecamentoLadder)_lstSB[_lstSB.Count - 1].getOperandos(0)).Contador.Tipo);
                                _lstSB[_lstSB.Count - 1].setOperando(2, ((EnderecamentoLadder)_lstSB[_lstSB.Count - 1].getOperandos(0)).Contador.Preset);
                                i += 3;
                                programa.linhas[intIndiceLinha].Insere2Saida(_lstSB);
                                _lstSB.Clear();
                            }
                            break;
                        case CodigosInterpretaveis.TEMPORIZADOR:
                            intContaFim = 0;
                            iNumOperandos = 4;
                            {
                                ListaSimbolo _lstSB = new ListaSimbolo();
                                _lstSB.Add(new SimboloBasico((CodigosInterpretaveis)guarda));
                                _lstSB[_lstSB.Count - 1].setOperando(0, programa.endereco.Find(TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_TEMPORIZADOR, (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 1, 1))));
                                ((EnderecamentoLadder)_lstSB[_lstSB.Count - 1].getOperandos(0)).Temporizador.Tipo = (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 2, 1));
                                ((EnderecamentoLadder)_lstSB[_lstSB.Count - 1].getOperandos(0)).Temporizador.BaseTempo = (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 3, 1));
                                ((EnderecamentoLadder)_lstSB[_lstSB.Count - 1].getOperandos(0)).Temporizador.Preset = (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 4, 1));

                                _lstSB[_lstSB.Count - 1].setOperando(1, ((EnderecamentoLadder)_lstSB[_lstSB.Count - 1].getOperandos(0)).Temporizador.Tipo);
                                _lstSB[_lstSB.Count - 1].setOperando(2, ((EnderecamentoLadder)_lstSB[_lstSB.Count - 1].getOperandos(0)).Temporizador.Preset);
                                _lstSB[_lstSB.Count - 1].setOperando(4, ((EnderecamentoLadder)_lstSB[_lstSB.Count - 1].getOperandos(0)).Temporizador.BaseTempo);

                                i += 4;
                                programa.linhas[intIndiceLinha].Insere2Saida(_lstSB);
                                _lstSB.Clear();
                            }
                            break;
                    }

                    /// fim dos códigos
                    if (intContaFim >= 2)
                    {
                        /// grava os dados lidos do codigo intepretavel
                        ModuloIntegracaoMSP430 p = new ModuloIntegracaoMSP430();
                        p.CriaArquivo("codigosinterpretaveis.txt", DadosConvertidosChar.Substring(DadosConvertidosChar.IndexOf("@laddermic.com"), i - DadosConvertidosChar.IndexOf("@laddermic.com") + 1));

                        /// força saída do loop
                        i = DadosConvertidosChar.Length;
                    }
                }

                //frmProj = new ProjetoLadder(programa);
                //frmProj.MdiParent = this;
                //frmProj.Show();
                //frmProj.SetText();
                return programa;

            }
            //else
            //    MessageBox.Show("O arquivo não foi reconhecido pelo sistema!", "Abrir Arquivos ...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return null;
        }
    
    }
}
