using System;
using System.Collections.Generic;
using System.Text;
using LadderApp.Exce��es;

namespace LadderApp.CodigoInterpretavel
{
    public class Text2OpCodeServices
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
                return posAtualLeitura;
            }
        }

        private Int32 intTamanhoCabecalho = -1;

        const String IdCodigo = "@laddermic.com";

        public Text2OpCodeServices(String DadosConvertidosChar)
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
                if (LeCodigoInterpretavel(PosInicial) == OperationCode.CABECALHO_TAMANHO)
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
                            case OperationCode.CABECALHO_SENHA_0:
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

        public OperationCode LeCodigoInterpretavel(Int32 _pos)
        {
            {
                try
                {
                    return (OperationCode)Convert.ToChar(DadosConvertidosChar.Substring(_pos, 1));
                }
                catch
                {
                    throw new NotValidOpCodeException();
                }
            }
        }

        public AddressTypeEnum LeTipoEnderecamento(Int32 _pos)
        {
            if (ExisteCodigoInterpretavel())
            {
                try
                {
                    return (AddressTypeEnum)Convert.ToChar(DadosConvertidosChar.Substring(_pos, 1));
                }
                catch
                {
                    throw new Exception();
                }
            }

            return AddressTypeEnum.NENHUM;
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
        
        public Address LeEndereco(ref Int32 _pos, Addressing enderecamento)
        {
            Address _endLido = null;
            AddressTypeEnum _tpEndLido;
            Int32 _iIndiceEndLido = 0;
            Symbol _sb = null;

            _sb = new Symbol(LeCodigoInterpretavel(_pos));

            switch (_sb.OpCode)
            {
                case OperationCode.None:
                    break;
                case OperationCode.FIM_DA_LINHA:
                    break;
                case OperationCode.CONTATO_NA:
                case OperationCode.CONTATO_NF:
                    _tpEndLido = LeTipoEnderecamento(_pos); _pos++;
                    _iIndiceEndLido = LeInteiro(_pos); _pos++;

                    _endLido = enderecamento.Find(_tpEndLido, _iIndiceEndLido);
                    break;
                case OperationCode.BOBINA_SAIDA:
                case OperationCode.RESET:
                    break;
                case OperationCode.PARALELO_INICIAL:
                case OperationCode.PARALELO_FINAL:
                case OperationCode.PARALELO_PROXIMO:
                    break;
                case OperationCode.CONTADOR:
                    break;
                case OperationCode.TEMPORIZADOR:
                    break;
            }

            return _endLido;
        }

        public Int32 NumeroOperandos(OperationCode _ci)
        {
            int iNumOperandos = -1;
            switch (_ci)
            {
                case OperationCode.None:
                    iNumOperandos = 0;
                    break;
                case OperationCode.FIM_DA_LINHA:
                    iNumOperandos = 0;
                    break;
                case OperationCode.CONTATO_NA:
                case OperationCode.CONTATO_NF:
                    iNumOperandos = 2;
                    break;
                case OperationCode.BOBINA_SAIDA:
                case OperationCode.RESET:
                    iNumOperandos = 2;
                    break;
                case OperationCode.PARALELO_INICIAL:
                case OperationCode.PARALELO_FINAL:
                case OperationCode.PARALELO_PROXIMO:
                    iNumOperandos = 0;
                    break;
                case OperationCode.CONTADOR:
                    iNumOperandos = 3;
                    break;
                case OperationCode.TEMPORIZADOR:
                    iNumOperandos = 4;
                    break;
                default:
                    iNumOperandos = 0;
                    break;
            }

            return iNumOperandos;
        }
        
        private LadderProgram LerExecutavel(String strNomeProjeto)
        {
            List<int> lstCodigosLidos = new List<int>();
            OperationCode guarda = OperationCode.None;

            if (this.ExisteCodigoInterpretavel())
            {
                Int32 intContaFim = 0;
                Int32 intIndiceLinha = 0;
                Address _endLido;
                AddressTypeEnum _tpEndLido;
                Int32 _iIndiceEndLido = 0;


                /// Cria um programa novo vazio
                LadderProgram programa = new LadderProgram();
                programa.StsPrograma = LadderProgram.StatusPrograma.NOVO;
                programa.Nome = strNomeProjeto;
                programa.dispositivo = new Device(1);
                programa.endereco.AlocaEnderecamentoIO(programa.dispositivo);
                programa.endereco.AlocaEnderecamentoMemoria(programa.dispositivo, programa.endereco.lstMemoria, AddressTypeEnum.DIGITAL_MEMORIA, 10);
                programa.endereco.AlocaEnderecamentoMemoria(programa.dispositivo, programa.endereco.lstTemporizador, AddressTypeEnum.DIGITAL_MEMORIA_TEMPORIZADOR, 10);
                programa.endereco.AlocaEnderecamentoMemoria(programa.dispositivo, programa.endereco.lstContador, AddressTypeEnum.DIGITAL_MEMORIA_CONTADOR, 10);
                intIndiceLinha = programa.InsereLinhaNoFinal(new Line());

                for (int i = this.PosInicial; i < DadosConvertidosChar.Length; i++)
                {
                    guarda = LeCodigoInterpretavel(i); i++;

                    switch (guarda)
                    {
                        case OperationCode.None:
                            intContaFim++;
                            break;
                        case OperationCode.FIM_DA_LINHA:
                            intContaFim++;
                            if (LeCodigoInterpretavel(i + 1) != OperationCode.None)
                                intIndiceLinha = programa.InsereLinhaNoFinal(new Line());
                            break;
                        case OperationCode.CONTATO_NA:
                        case OperationCode.CONTATO_NF:
                            intContaFim = 0;
                            {
                                Symbol _sb = new Symbol((OperationCode)guarda);
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
                        case OperationCode.BOBINA_SAIDA:
                        case OperationCode.RESET:
                            intContaFim = 0;
                            {
                                SymbolList _lstSB = new SymbolList();
                                _lstSB.Add(new Symbol((OperationCode)guarda));
                                _tpEndLido = (AddressTypeEnum)Convert.ToChar(DadosConvertidosChar.Substring(i + 1, 1));
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
                        case OperationCode.PARALELO_INICIAL:
                        case OperationCode.PARALELO_FINAL:
                        case OperationCode.PARALELO_PROXIMO:
                            intContaFim = 0;
                            programa.linhas[intIndiceLinha].simbolos.Add(new Symbol((OperationCode)guarda));
                            break;
                        case OperationCode.CONTADOR:
                            intContaFim = 0;
                            {
                                SymbolList _lstSB = new SymbolList();
                                _lstSB.Add(new Symbol((OperationCode)guarda));
                                _lstSB[_lstSB.Count - 1].setOperando(0, programa.endereco.Find(AddressTypeEnum.DIGITAL_MEMORIA_CONTADOR, (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 1, 1))));
                                ((Address)_lstSB[_lstSB.Count - 1].getOperandos(0)).Contador.Tipo = (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 2, 1));
                                ((Address)_lstSB[_lstSB.Count - 1].getOperandos(0)).Contador.Preset = (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 3, 1));

                                _lstSB[_lstSB.Count - 1].setOperando(1, ((Address)_lstSB[_lstSB.Count - 1].getOperandos(0)).Contador.Tipo);
                                _lstSB[_lstSB.Count - 1].setOperando(2, ((Address)_lstSB[_lstSB.Count - 1].getOperandos(0)).Contador.Preset);
                                i += 3;
                                programa.linhas[intIndiceLinha].Insere2Saida(_lstSB);
                                _lstSB.Clear();
                            }
                            break;
                        case OperationCode.TEMPORIZADOR:
                            intContaFim = 0;
                            {
                                SymbolList _lstSB = new SymbolList();
                                _lstSB.Add(new Symbol((OperationCode)guarda));
                                _lstSB[_lstSB.Count - 1].setOperando(0, programa.endereco.Find(AddressTypeEnum.DIGITAL_MEMORIA_TEMPORIZADOR, (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 1, 1))));
                                ((Address)_lstSB[_lstSB.Count - 1].getOperandos(0)).Temporizador.Tipo = (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 2, 1));
                                ((Address)_lstSB[_lstSB.Count - 1].getOperandos(0)).Temporizador.BaseTempo = (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 3, 1));
                                ((Address)_lstSB[_lstSB.Count - 1].getOperandos(0)).Temporizador.Preset = (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 4, 1));

                                _lstSB[_lstSB.Count - 1].setOperando(1, ((Address)_lstSB[_lstSB.Count - 1].getOperandos(0)).Temporizador.Tipo);
                                _lstSB[_lstSB.Count - 1].setOperando(2, ((Address)_lstSB[_lstSB.Count - 1].getOperandos(0)).Temporizador.Preset);
                                _lstSB[_lstSB.Count - 1].setOperando(4, ((Address)_lstSB[_lstSB.Count - 1].getOperandos(0)).Temporizador.BaseTempo);

                                i += 4;
                                programa.linhas[intIndiceLinha].Insere2Saida(_lstSB);
                                _lstSB.Clear();
                            }
                            break;
                    }

                    /// fim dos c�digos
                    if (intContaFim >= 2)
                    {
                        /// grava os dados lidos do codigo intepretavel
                        MSP430IntegrationServices p = new MSP430IntegrationServices();
                        p.CriaArquivo("codigosinterpretaveis.txt", DadosConvertidosChar.Substring(DadosConvertidosChar.IndexOf("@laddermic.com"), i - DadosConvertidosChar.IndexOf("@laddermic.com") + 1));

                        /// for�a sa�da do loop
                        i = DadosConvertidosChar.Length;
                    }
                }
                return programa;

            }
            return null;
        }
    
    }
}