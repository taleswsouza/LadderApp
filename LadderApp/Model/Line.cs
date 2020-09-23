using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace LadderApp
{
    public class Line
    {
        [XmlIgnore]
        public Symbol simboloInicioLinha = new Symbol(OperationCode.INICIO_DA_LINHA);
        [XmlElement(ElementName = "Simbolo")]
        public List<Symbol> simbolos = new List<Symbol>();
        public List<Symbol> saida = new List<Symbol>();
        [XmlIgnore]
        public Symbol simboloFimLinha = new Symbol(OperationCode.FIM_DA_LINHA);

        [XmlIgnore]
        public Addressing endereco = null;

        public Line(int quallinha, Addressing _endereco)
        {
            endereco = _endereco;
            
            switch (quallinha)
            {
                case 1:
                    InicializaDesenhosSimbolos1();
                    break;
                case 2:
                    InicializaDesenhosSimbolos2();
                    break;
                case 3:
                    InicializaDesenhosSimbolos3();
                    break;
                case 4:
                    InicializaDesenhosSimbolos4();
                    break;
                case 5:
                    InicializaDesenhosSimbolos5();
                    break;
                case 6:
                    InicializaDesenhosSimbolosVazio();
                    break;
                default:
                    break;
            }
        }

        public Line()
        {
        }
        
        private void InicializaDesenhosSimbolos1()
        {
            // Inicio de Linha
            simboloInicioLinha.OpCode = OperationCode.INICIO_DA_LINHA;

            Symbol ex1 = null;

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_INICIAL;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_INICIAL;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_INICIAL;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_PROXIMO;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NF;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_FINAL;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_PROXIMO;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NF;
            simbolos.Add(ex1);
            //
            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_INICIAL;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_PROXIMO;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NF;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_FINAL;
            simbolos.Add(ex1);
            //
            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_INICIAL;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_PROXIMO;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_FINAL;
            simbolos.Add(ex1);

            //teste inicio
            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_PROXIMO;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_INICIAL;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_PROXIMO;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_PROXIMO;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_FINAL;
            simbolos.Add(ex1);

            // teste fim
            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_FINAL;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_PROXIMO;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_INICIAL;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_PROXIMO;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_FINAL;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_INICIAL;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_PROXIMO;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_FINAL;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_FINAL;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_INICIAL;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_PROXIMO;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_FINAL;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            //saida
            ex1 = new Symbol();
            ex1.OpCode = OperationCode.BOBINA_SAIDA;
            saida.Add(ex1);

            // Fim de Linha
            simboloFimLinha.OpCode = OperationCode.FIM_DA_LINHA;
        }

        private void InicializaDesenhosSimbolos2()
        {
            // Inicio de Linha
            simboloInicioLinha.OpCode = OperationCode.INICIO_DA_LINHA;

            Symbol ex1 = null;

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_INICIAL;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_INICIAL;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_INICIAL;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_PROXIMO;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NF;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_FINAL;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_PROXIMO;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NF;
            simbolos.Add(ex1);
            //
            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_INICIAL;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_PROXIMO;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NF;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_FINAL;
            simbolos.Add(ex1);
            //
            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_INICIAL;
            simbolos.Add(ex1);

            //ex1 = new SimboloBasico();
            ////ex1.TabStop = true;
            //ex1.setCI(CodigosInterpretaveis.CONTATO_NA);
            //simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_PROXIMO;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_FINAL;
            simbolos.Add(ex1);

            //teste inicio
            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_PROXIMO;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_INICIAL;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_PROXIMO;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_PROXIMO;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_FINAL;
            simbolos.Add(ex1);

            // teste fim
            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_FINAL;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_PROXIMO;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_INICIAL;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_PROXIMO;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_FINAL;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_INICIAL;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_PROXIMO;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_FINAL;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_FINAL;
            simbolos.Add(ex1);

            //saida
            ex1 = new Symbol();
            ex1.OpCode = OperationCode.BOBINA_SAIDA;
            saida.Add(ex1);

            // Fim de Linha
            simboloFimLinha.OpCode = OperationCode.FIM_DA_LINHA;
        }

        private void InicializaDesenhosSimbolos3()
        {
            // Inicio de Linha
            simboloInicioLinha.OpCode = OperationCode.INICIO_DA_LINHA;

            Symbol ex1 = null;

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            ex1.setOperando(0, endereco.lstMemoria[1]);
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NF;
            ex1.setOperando(0, endereco.lstTemporizador[2]);
            simbolos.Add(ex1);

            
            //saida
            ex1 = new Symbol();
            ex1.OpCode = OperationCode.BOBINA_SAIDA;
            ex1.setOperando(0, endereco.lstMemoria[1]);
            saida.Add(ex1);

            // Fim de Linha
            simboloFimLinha.OpCode = OperationCode.FIM_DA_LINHA;
        }

        private void InicializaDesenhosSimbolos4()
        {
            // Inicio de Linha
            simboloInicioLinha.OpCode = OperationCode.INICIO_DA_LINHA;

            Symbol ex1 = null;

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_INICIAL;
            saida.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.BOBINA_SAIDA;
            saida.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_PROXIMO;
            saida.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.BOBINA_SAIDA;
            saida.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_FINAL;
            saida.Add(ex1);

            // Fim de Linha
            simboloFimLinha.OpCode = OperationCode.FIM_DA_LINHA;
        }

        private void InicializaDesenhosSimbolos5()
        {
            // Inicio de Linha
            simboloInicioLinha.OpCode = OperationCode.INICIO_DA_LINHA;

            Symbol ex1 = null;

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.CONTATO_NA;
            ex1.setOperando(0, endereco.lstMemoria[0]);
            endereco.lstMemoria[0].EmUso = true;
            simbolos.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_INICIAL;
            saida.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.BOBINA_SAIDA;
            saida.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_PROXIMO;
            saida.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.BOBINA_SAIDA;
            saida.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_PROXIMO;
            saida.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.BOBINA_SAIDA;
            saida.Add(ex1);

            ex1 = new Symbol();
            ex1.OpCode = OperationCode.PARALELO_FINAL;
            saida.Add(ex1);

            // Fim de Linha
            simboloFimLinha.OpCode = OperationCode.FIM_DA_LINHA;
            //simboloFimLinha.TabStop = false;

        }

        private void InicializaDesenhosSimbolosVazio()
        {
            // Inicio de Linha
            simboloInicioLinha.OpCode = OperationCode.INICIO_DA_LINHA;

            // Fim de Linha
            simboloFimLinha.OpCode = OperationCode.FIM_DA_LINHA;
        }

        public void ApagaLinha()
        {
            saida.Reverse();
            foreach (Symbol cl in saida)
            {
                cl.Dispose();
            }
            saida.Clear();

            simbolos.Reverse();
            foreach (Symbol cl in simbolos)
            {
                cl.Dispose();
            }
            simbolos.Clear();

            simboloInicioLinha.Dispose();
            simboloFimLinha.Dispose();
        }

        public Symbol Insere2Saida(SymbolList _lstSB)
        {
            int _indiceSimbolo = 0;
            int _subt2posicionaSimboloInserido = 0;

            switch (saida.Count)
            {
                case 0:
                    /// case 0: Primeiro simbolo na saida, adiciona apenas um
                    /// simbolo na saida
                    _indiceSimbolo = 0;

                    if (_lstSB.Count > 1)
                    {
                        _lstSB.InsereParalelo(SymbolList.TipoInsercaoParalelo.PARALELO_COMPLETO);
                        _subt2posicionaSimboloInserido = -1;
                    }

                    break;
                case 1:
                    /// case 1: Caso ja exista 1 (um) simbolo de saida, insere um 
                    /// paralelo de forma automatica


                    // aqui 0=antes, 1=depois
                    if (_indiceSimbolo == 0)
                    {
                        /// prepara para inserir antes do objeto atual
                        _lstSB.InsereParalelo(SymbolList.TipoInsercaoParalelo.PARALELO_INICIADO);

                        /// insere PP antes do objeto atual na linha
                        this.saida.Insert(0, new Symbol(OperationCode.PARALELO_PROXIMO));
                        /// insere PF depois do objeto atual da linha
                        this.saida.Insert(this.saida.Count, new Symbol(OperationCode.PARALELO_FINAL));
                    }
                    else
                    {
                        _lstSB.InsereParalelo(SymbolList.TipoInsercaoParalelo.PARALELO_FINALIZADO);
                        _subt2posicionaSimboloInserido = -1;

                        this.saida.Insert(0, new Symbol(OperationCode.PARALELO_INICIAL));
                        _indiceSimbolo++;
                    }

                    break;
                default:
                    switch (this.saida[_indiceSimbolo].OpCode)
                    {
                        case OperationCode.PARALELO_INICIAL:
                            _lstSB.InsereParalelo(SymbolList.TipoInsercaoParalelo.PARALELO_INICIADO);

                            this.saida[0].OpCode = OperationCode.PARALELO_PROXIMO;
                            break;
                        case OperationCode.PARALELO_PROXIMO:
                            _lstSB.InsereParaleloProximo();
                            break;
                        case OperationCode.PARALELO_FINAL:
                            _lstSB.InsereParaleloProximo();
                            break;
                        default:
                            _lstSB.InsereParaleloProximo();
                            _indiceSimbolo++;
                            break;
                    }
                    break;
            }

            foreach (Symbol _sb in _lstSB)
            {
                this.saida.Insert(_indiceSimbolo, _sb);
                _indiceSimbolo++;
            }

            /// retorna o ultimo objeto inserido
            return this.saida[_indiceSimbolo - 1 + _subt2posicionaSimboloInserido];
        }


    }
}
