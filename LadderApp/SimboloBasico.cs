using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml.Serialization;

namespace LadderApp
{
    [XmlInclude(typeof(EnderecamentoLadder))]
    [XmlType(TypeName="Simbolo")]
    [Serializable]
    public class SimboloBasico : ISimbolo
    {
        public SimboloBasico()
        {
        }

        public SimboloBasico(CodigosInterpretaveis _ci)
        {
            codigoInterpretavel = _ci;
        }
        
        [XmlIgnore]
        public int iNumOperandos;

        [XmlElement(Order = 2, IsNullable = true, ElementName = "Operando")]
        public Object[] Operandos = new Object[5];


        private CodigosInterpretaveis codigoInterpretavelAtual = CodigosInterpretaveis.NENHUM;
        [XmlElement(Order = 1, ElementName = "Instrucao")]
        public CodigosInterpretaveis codigoInterpretavel
        {
            get { return codigoInterpretavelAtual; }
            set {
                codigoInterpretavelAtual = value;
                RealocaOperandos();
            }
        }
        public void setCI(CodigosInterpretaveis codigo)
        {
            codigoInterpretavel = codigo;
            RealocaOperandos();
        }

        public CodigosInterpretaveis getCI()
        {
            return codigoInterpretavel;
        }

        public Object[] getOperandos()
        {
            return Operandos;
        }

        public Object getOperandos(int posicao)
        {
            if (Operandos.Length > 0)
                return Operandos[posicao];
            else
                return null;
        }

        public void setOperando(int iNumOperando, Object valor)
        {
            if (iNumOperando < iNumOperandos)
            {
                if (ValidaEndereco(iNumOperando, valor))
                    Operandos[iNumOperando] = valor;
                //else
                    // gerar excecao
            }
        }


        public void setOperando(Object[] operandos)
        {
            Operandos = operandos;
        }


        public int RealocaOperandos()
        {
            Operandos = null;
            GC.Collect();
            Operandos = new Object[5];

            /// Aloca espaco para a quantidade de operandos de cada codigo interpretavel
            switch (getCI())
            {
                case CodigosInterpretaveis.NENHUM:
                case CodigosInterpretaveis.FIM_DA_LINHA:
                    iNumOperandos = 0;
                    break;
                case CodigosInterpretaveis.INICIO_DA_LINHA:
                case CodigosInterpretaveis.CONTATO_NA:
                case CodigosInterpretaveis.CONTATO_NF:
                case CodigosInterpretaveis.BOBINA_SAIDA:
                case CodigosInterpretaveis.RESET:
                    iNumOperandos = 1;
                    break;
                case CodigosInterpretaveis.PARALELO_INICIAL:
                case CodigosInterpretaveis.PARALELO_FINAL:
                case CodigosInterpretaveis.PARALELO_PROXIMO:
                    iNumOperandos = 0;
                    break;
                case CodigosInterpretaveis.CONTADOR:
                    iNumOperandos = 4;

                    //((EnderecamentoLadder)getOperandos(0)).Contador.Tipo = 0;
                    //((EnderecamentoLadder)getOperandos(0)).Contador.Preset = 0;
                    //((EnderecamentoLadder)getOperandos(0)).Contador.Acumulado = 0;
                    setOperando(1, (Int32)0); // tipo
                    setOperando(2, (Int32)0); // preset
                    setOperando(3, (Int32)0); // acum

                    break;
                case CodigosInterpretaveis.TEMPORIZADOR:
                    iNumOperandos = 5;

                    //((EnderecamentoLadder)getOperandos(0)).Temporizador.Tipo = 0;
                    //((EnderecamentoLadder)getOperandos(0)).Temporizador.Preset = 0;
                    //((EnderecamentoLadder)getOperandos(0)).Temporizador.Acumulado = 0;
                    //((EnderecamentoLadder)getOperandos(0)).Temporizador.BaseTempo = 0;
                    setOperando(1, (Int32)0); // tipo
                    setOperando(2, (Int32)0); // preset
                    setOperando(3, (Int32)0); // acum
                    setOperando(4, (Int32)0); // Base tempo

                    break;
            }
            return iNumOperandos;
        }

        private Size TamanhoXY;
        [XmlIgnore]
        public Size tamanhoXY
        {
            get { return TamanhoXY; }
            set { TamanhoXY = new Size(0, 0); }
        }

        private Point PosicaoXY;
        [XmlIgnore]
        public Point posicaoXY
        {
            get { return PosicaoXY; }
            set { PosicaoXY = new Point(0, 0); }
        }

        //private Point xyConexao;
        [XmlIgnore]
        public Point XYConexao
        {
            get { return new Point(0, 0); }
        }

        public void Dispose()
        {
            setCI(CodigosInterpretaveis.NENHUM);
        }

        /// <summary>
        /// Valida o operando de acordo com o codigo interpretavel
        /// </summary>
        /// <param name="_novoOperando">Caso operando ainda nao atribuido verifica antes</param>
        /// <returns></returns>
        private bool ValidaEndereco(int _indice, Object _novoOperando)
        {
            EnderecamentoLadder _end = null;
            EnderecamentoLadder _atualOperando = null;
            bool _bValido = true;
            bool _bEndereco = false;

            if (_novoOperando != null)
            {
                if (_novoOperando.GetType().Name == "EnderecamentoLadder")
                {
                    if (_novoOperando == null)
                        _end = (EnderecamentoLadder)getOperandos(0);
                    else
                        _end = (EnderecamentoLadder)_novoOperando;
                }
                else if (_novoOperando.GetType().Name == "Int32")
                {
                }
                
            }

            switch (getCI())
            {
                case CodigosInterpretaveis.NENHUM:
                case CodigosInterpretaveis.FIM_DA_LINHA:
                    _bValido = false;
                    break;
                case CodigosInterpretaveis.INICIO_DA_LINHA:
                    if (_novoOperando != null)
                        if (_novoOperando.GetType().ToString() != "System.Int32")
                            _bValido = false;
                    break;
                case CodigosInterpretaveis.CONTATO_NA:
                case CodigosInterpretaveis.CONTATO_NF:
                    _bEndereco = true;
                    if (_end != null)
                    {
                        switch (_end.TpEnderecamento)
                        {
                            case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA:
                            case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_CONTADOR:
                            case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_TEMPORIZADOR:
                            case TipoEnderecamentoDispositivo.DIGITAL_ENTRADA:
                            case TipoEnderecamentoDispositivo.DIGITAL_SAIDA:
                                _bValido = true;
                                break;
                            default:
                                _bValido = false;
                                break;
                        }
                    }
                    else
                        _bValido = false;
                    break;
                case CodigosInterpretaveis.BOBINA_SAIDA:
                    _bEndereco = true;
                    if (_end != null)
                    {
                        switch (_end.TpEnderecamento)
                        {
                            case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA:
                            case TipoEnderecamentoDispositivo.DIGITAL_SAIDA:
                                _bValido = true;
                                break;
                            default:
                                _bValido = false;
                                break;
                        }
                    }
                    else
                        _bValido = false;
                    break;
                case CodigosInterpretaveis.TEMPORIZADOR:
                    if (_end != null && _indice == 0)
                    {
                        _bEndereco = true;
                        switch (_end.TpEnderecamento)
                        {
                            case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_TEMPORIZADOR:
                                _bValido = true;
                                break;
                            default:
                                _bValido = false;
                                break;
                        }
                    }
                    else if (_indice > 0)
                    {
                        switch (_indice)
                        {
                            case 1:
                                break;
                            case 2:
                                break;
                            default:
                                break;
                        }
                    }
                    else
                        _bValido = false;
                    break;
                case CodigosInterpretaveis.CONTADOR:
                    if (_end != null && _indice == 0)
                    {
                        _bEndereco = true;
                        switch (_end.TpEnderecamento)
                        {
                            case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_CONTADOR:
                                _bValido = true;
                                break;
                            default:
                                _bValido = false;
                                break;
                        }
                    }
                    else if (_indice > 0)
                    {
                        switch (_indice)
                        {
                            case 1:
                                break;
                            case 2:
                                break;
                            default:
                                break;
                        }
                    }
                    else
                        _bValido = false;
                    break;
                case CodigosInterpretaveis.PARALELO_INICIAL:
                case CodigosInterpretaveis.PARALELO_FINAL:
                case CodigosInterpretaveis.PARALELO_PROXIMO:
                    _bValido = false;
                    break;
                case CodigosInterpretaveis.RESET:
                    _bEndereco = true;
                    if (_end != null)
                    {
                        switch (_end.TpEnderecamento)
                        {
                            case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_CONTADOR:
                            case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_TEMPORIZADOR:
                                _bValido = true;
                                break;
                            default:
                                _bValido = false;
                                break;
                        }
                    }
                    else
                        _bValido = false;
                    break;
                default:
                    _bValido = false;
                    break;
            }

            if (_bEndereco)
            {
                if (getOperandos(0) != null)
                    _atualOperando = (EnderecamentoLadder)getOperandos(0);

                if (_bValido)
                {
                    if (_atualOperando != null)
                        _atualOperando.MudouOperando -= new MudouOperandoEventHandler(Ocorreu_MudouOperando);

                    _end.MudouOperando += new MudouOperandoEventHandler(Ocorreu_MudouOperando);
                }
                else
                    if (_novoOperando == null)
                    {
                        if (_atualOperando != null)
                        {
                            _atualOperando.MudouOperando -= new MudouOperandoEventHandler(Ocorreu_MudouOperando);
                            Operandos[0] = null;
                        }
                    }
            }
            //else
            //    _bValido = true;

            return _bValido;
        }

        void Ocorreu_MudouOperando(object sender)
        {
            ValidaEndereco(0, null);
        }

        public void ValidaOperandosSimbolo(EnderecamentoPrograma _EndDisp)
        {
            for(int i = 0; i < iNumOperandos; i++)
                if (Operandos[i] != null)
                    if (Operandos[i].GetType().Name == "EnderecamentoLadder")
                    {
                        /// Verifica se o endereco atual existe na lista de enderecos
                        /// do programa atual, se existir recupera o opontamento corrigido
                        /// para o endereco
                        Object oper = _EndDisp.Find(((EnderecamentoLadder)Operandos[i]));
                        /// recebido o endereco corrigido valida o endereco a faz as atribui
                        /// coes necessarias
                        if (oper != null)
                        {
                            if (ValidaEndereco(i, oper))
                                Operandos[i] = oper;
                            else
                            {
                                Operandos[i] = null;
                                GC.Collect();
                            }
                        }
                        else
                        {
                            Operandos[i] = null;
                            GC.Collect();
                        }



                    }
        }
    }
}
