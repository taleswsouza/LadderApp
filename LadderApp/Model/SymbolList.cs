using System;
using System.Collections.Generic;
using System.Text;

namespace LadderApp
{
    public class SymbolList : List<Symbol>, IList<Symbol>
    {
        /// <summary>
        /// suporte - utilizado na funcao "ProcuraCodigoInterpretavel()" - carrega
        /// o codigointerpretavel para as iteracoes da funcao
        /// </summary>
        private OpCode ciLocal = OpCode.NENHUM;

        /// <summary>
        /// Verifica se a lista contem o codigointerpretavel do parametro
        /// </summary>
        /// <param name="value">codigo interpretavel a ser verificado na lista</param>
        /// <returns>true - codigo encontrado na lista / false - codigno nao encontrado na lista</returns>
        public bool Contains(OpCode value)
        {
            ciLocal = value;
            bool _bResult = this.Exists(ProcuraCodigoInterpretavel);
            ciLocal = OpCode.NENHUM;
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


        public bool ExisteTemporizadorDuplicado(List<Address> lstTemporizadoresUtilizados)
        {
            foreach (Symbol _sb in this)
            {
                if (_sb.getCI() == OpCode.TEMPORIZADOR)
                {
                    if (lstTemporizadoresUtilizados.Contains((Address)_sb.getOperandos(0)))
                        return false;
                    else
                        lstTemporizadoresUtilizados.Add((Address)_sb.getOperandos(0));
                }
            }
            return true;
        }

        public bool ExisteContadorDuplicado(List<Address> lstContadoresUtilizados)
        {
            foreach (Symbol _sb in this)
            {
                if (_sb.getCI() == OpCode.CONTADOR)
                {
                    if (lstContadoresUtilizados.Contains((Address)_sb.getOperandos(0)))
                        return false;
                    else
                        lstContadoresUtilizados.Add((Address)_sb.getOperandos(0));
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
        public SymbolList InsertAllWithClearBefore(List<Symbol> _lstSB)
        {
            this.Clear();

            if (_lstSB.Count > 0)
                foreach (Symbol _cadaSB in _lstSB)
                {
                    this.Add(_cadaSB);
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
        public SymbolList InsertAllWithClearBefore(OpCode [] _arrayCI)
        {
            this.Clear();

            if (_arrayCI.Length > 0)
                foreach (OpCode _cadaCI in _arrayCI)
                    this.Add(new Symbol(_cadaCI));

            return this;
        }


        public void InsereParaleloProximo()//bool _bAposPrimeiro)
        {
            for(int i = (this.Count - 1); i >= 0; i--)
            {
                this.Insert(i, new Symbol(OpCode.PARALELO_PROXIMO));
            }
        }


        public void InsereParalelo(TipoInsercaoParalelo _tIP)
        {
            InsereParaleloProximo();

            switch (_tIP)
            {
                case TipoInsercaoParalelo.PARALELO_INICIADO:
                    this[0].setCI(OpCode.PARALELO_INICIAL);
                    break;
                case TipoInsercaoParalelo.PARALELO_FINALIZADO:
                    this.Add(new Symbol(OpCode.PARALELO_FINAL));
                    break;
                case TipoInsercaoParalelo.PARALELO_COMPLETO:
                    this[0].setCI(OpCode.PARALELO_INICIAL);
                    this.Add(new Symbol(OpCode.PARALELO_FINAL));
                    break;
            }
        }

        public void ValidaOperandos(Addressing _ep)
        {
            foreach(Symbol _sb in this)
                _sb.ValidaOperandosSimbolo(_ep);
        }


        private bool ProcuraCodigoInterpretavel(Symbol _sb)
        {
            if (_sb.getCI() == ciLocal)
                return true;
            else
                return false;
        }

        private static bool VerificaSeSimboloTemTodosOperandos(Symbol _sb)
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
      
        
        private static Symbol SimboloBasicoToSimboloBasico(Symbol _sb)
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
