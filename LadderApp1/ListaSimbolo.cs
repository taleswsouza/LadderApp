using System;
using System.Collections.Generic;
using System.Text;

namespace LadderApp1
{
    public class ListaSimbolo : List<SimboloBasico>, IList<SimboloBasico>
    {
        /// <summary>
        /// suporte - utilizado na funcao "ProcuraCodigoInterpretavel()" - carrega
        /// o codigointerpretavel para as iteracoes da funcao
        /// </summary>
        private CodigosInterpretaveis ciLocal = CodigosInterpretaveis.NENHUM;

        /// <summary>
        /// Verifica se a lista contem o codigointerpretavel do parametro
        /// </summary>
        /// <param name="value">codigo interpretavel a ser verificado na lista</param>
        /// <returns>true - codigo encontrado na lista / false - codigno nao encontrado na lista</returns>
        public bool Contains(CodigosInterpretaveis value)
        {
            ciLocal = value;
            bool _bResult = this.Exists(ProcuraCodigoInterpretavel);
            ciLocal = CodigosInterpretaveis.NENHUM;
            return _bResult;
        }

        /// <summary>
        /// Verifica se todos os operandos especificados por instrucao
        /// (codigointerpretave) estao atribuidos
        /// </summary>
        /// <returns>true - se todos os operandos estao atribuidos /
        /// false - se algum operanto estiver null</returns>
        public bool ContainsAllOperandos()
        {
            return this.TrueForAll(VerificaSeSimboloTemTodosOperandos);
        }


        public bool ExisteTemporizadorDuplicado(List<EnderecamentoLadder> lstTemporizadoresUtilizados)
        {
            //lstTemporizadoresUtilizados.Clear();
            foreach (SimboloBasico _sb in this)
            {
                if (_sb.getCI() == CodigosInterpretaveis.TEMPORIZADOR)
                {
                    if (lstTemporizadoresUtilizados.Contains((EnderecamentoLadder)_sb.getOperandos(0)))
                        return false;
                    else
                        lstTemporizadoresUtilizados.Add((EnderecamentoLadder)_sb.getOperandos(0));
                }
            }
            return true;
        }

        public bool ExisteContadorDuplicado(List<EnderecamentoLadder> lstContadoresUtilizados)
        {
            //lstContadoresUtilizados.Clear();
            foreach (SimboloBasico _sb in this)
            {
                if (_sb.getCI() == CodigosInterpretaveis.CONTADOR)
                {
                    if (lstContadoresUtilizados.Contains((EnderecamentoLadder)_sb.getOperandos(0)))
                        return false;
                    else
                        lstContadoresUtilizados.Add((EnderecamentoLadder)_sb.getOperandos(0));
                }
            }
            return true;
        }
        
        
        /// <summary>
        /// InsertAllWithClearBefore() - Insere todos os itens SimboloBasico da lista 
        /// do parametro de entrada na lista atual, caso ja haja itens na lista atual,
        /// serao apagados.
        /// </summary>
        /// <param name="_lstSB">Lista de entrada com os simbolos que serao
        /// inseridos no objeto</param>
        /// <returns>objeto atual</returns>
        public ListaSimbolo InsertAllWithClearBefore(List<SimboloBasico> _lstSB)
        {
            this.Clear();

            if (_lstSB.Count > 0)
                foreach (SimboloBasico _cadaSB in _lstSB)
                {
                    this.Add(_cadaSB);
                    //if (_cadaSB.codigoInterpretavel == CodigosInterpretaveis.TEMPORIZADOR)
                    //{
                    //    if (_cadaSB.getOperandos(0) != null)
                    //        this.lstTemporizadoresUtilizados.Add((EnderecamentoLadder)_cadaSB.getOperandos(0));
                    //}
                    //else if (_cadaSB.codigoInterpretavel == CodigosInterpretaveis.CONTADOR)
                    //{
                    //    if (_cadaSB.getOperandos(0) != null)
                    //        this.lstContadoresUtilizados.Add((EnderecamentoLadder)_cadaSB.getOperandos(0));
                    //}
                }

            return this;
        }


        /// <summary>
        /// InsertAllWithClearBefore() - Insere todos os itens CodigosInterpretaveis da lista 
        /// do parametro de entrada na lista atual, caso ja haja itens na lista atual,
        /// serao apagados.
        /// </summary>
        /// <param name="_lstSB">Lista de entrada com os CodigosInterpretaveis que serao
        /// inseridos no objeto</param>
        /// <returns>objeto atual</returns>
        public ListaSimbolo InsertAllWithClearBefore(CodigosInterpretaveis [] _arrayCI)
        {
            this.Clear();

            if (_arrayCI.Length > 0)
                foreach (CodigosInterpretaveis _cadaCI in _arrayCI)
                    this.Add(new SimboloBasico(_cadaCI));

            return this;
        }


        public void InsereParaleloProximo()//bool _bAposPrimeiro)
        {
            //int _inicio = 0;
            //if (_bAposPrimeiro)
            //    _inicio = 
            for(int i = (this.Count - 1); i >= 0; i--)
            {
                this.Insert(i, new SimboloBasico(CodigosInterpretaveis.PARALELO_PROXIMO));
            }
        }


        public void InsereParalelo(TipoInsercaoParalelo _tIP)
        {
            InsereParaleloProximo();

            switch (_tIP)
            {
                case TipoInsercaoParalelo.PARALELO_INICIADO:
                    this[0].setCI(CodigosInterpretaveis.PARALELO_INICIAL);
                    break;
                case TipoInsercaoParalelo.PARALELO_FINALIZADO:
                    this.Add(new SimboloBasico(CodigosInterpretaveis.PARALELO_FINAL));
                    break;
                case TipoInsercaoParalelo.PARALELO_COMPLETO:
                    this[0].setCI(CodigosInterpretaveis.PARALELO_INICIAL);
                    this.Add(new SimboloBasico(CodigosInterpretaveis.PARALELO_FINAL));
                    break;
            }
        }

        public void ValidaOperandos(EnderecamentoPrograma _ep)
        {
            foreach(SimboloBasico _sb in this)
                _sb.ValidaOperandosSimbolo(_ep);
        }


        private bool ProcuraCodigoInterpretavel(SimboloBasico _sb)
        {
            if (_sb.getCI() == ciLocal)
                return true;
            else
                return false;
        }

        private static bool VerificaSeSimboloTemTodosOperandos(SimboloBasico _sb)
        {
            bool _bResult = true;
            for (int i = 0; i < _sb.iNumOperandos; i++)
            {
                if (_sb.getOperandos(i) == null)
                {
                    _bResult = false;
                    break;
                }
            }
            return _bResult;
        }
      
        
        private static SimboloBasico SimboloBasicoToSimboloBasico(SimboloBasico _sb)
        {
            return (_sb);
        }

        public enum TipoInsercaoParalelo
        {
            PARALELO_INICIADO,
            PARALELO_FINALIZADO,
            PARALELO_COMPLETO
        }
    }
}
