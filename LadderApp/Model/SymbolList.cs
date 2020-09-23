using System;
using System.Collections.Generic;
using System.Text;

namespace LadderApp
{
    public class SymbolList : List<Instruction>, IList<Instruction>
    {
        /// <summary>
        /// suporte - utilizado na funcao "ProcuraCodigoInterpretavel()" - carrega
        /// o codigointerpretavel para as iteracoes da funcao
        /// </summary>
        private OperationCode ciLocal = OperationCode.None;

        /// <summary>
        /// Verifica se a lista contem o codigointerpretavel do parametro
        /// </summary>
        /// <param name="value">codigo interpretavel a ser verificado na lista</param>
        /// <returns>true - codigo encontrado na lista / false - codigno nao encontrado na lista</returns>
        public bool Contains(OperationCode value)
        {
            ciLocal = value;
            bool _bResult = this.Exists(ProcuraCodigoInterpretavel);
            ciLocal = OperationCode.None;
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
            foreach (Instruction _sb in this)
            {
                if (_sb.OpCode== OperationCode.TEMPORIZADOR)
                {
                    if (lstTemporizadoresUtilizados.Contains((Address)_sb.GetOperand(0)))
                        return false;
                    else
                        lstTemporizadoresUtilizados.Add((Address)_sb.GetOperand(0));
                }
            }
            return true;
        }

        public bool ExisteContadorDuplicado(List<Address> lstContadoresUtilizados)
        {
            foreach (Instruction _sb in this)
            {
                if (_sb.OpCode== OperationCode.CONTADOR)
                {
                    if (lstContadoresUtilizados.Contains((Address)_sb.GetOperand(0)))
                        return false;
                    else
                        lstContadoresUtilizados.Add((Address)_sb.GetOperand(0));
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
        public SymbolList InsertAllWithClearBefore(List<Instruction> _lstSB)
        {
            this.Clear();

            if (_lstSB.Count > 0)
                foreach (Instruction _cadaSB in _lstSB)
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
        public SymbolList InsertAllWithClearBefore(OperationCode [] _arrayCI)
        {
            this.Clear();

            if (_arrayCI.Length > 0)
                foreach (OperationCode _cadaCI in _arrayCI)
                    this.Add(new Instruction(_cadaCI));

            return this;
        }


        public void InsereParaleloProximo()//bool _bAposPrimeiro)
        {
            for(int i = (this.Count - 1); i >= 0; i--)
            {
                this.Insert(i, new Instruction(OperationCode.PARALELO_PROXIMO));
            }
        }


        public void InsereParalelo(TipoInsercaoParalelo _tIP)
        {
            InsereParaleloProximo();

            switch (_tIP)
            {
                case TipoInsercaoParalelo.PARALELO_INICIADO:
                    this[0].OpCode = OperationCode.PARALELO_INICIAL;
                    break;
                case TipoInsercaoParalelo.PARALELO_FINALIZADO:
                    this.Add(new Instruction(OperationCode.PARALELO_FINAL));
                    break;
                case TipoInsercaoParalelo.PARALELO_COMPLETO:
                    this[0].OpCode = OperationCode.PARALELO_INICIAL;
                    this.Add(new Instruction(OperationCode.PARALELO_FINAL));
                    break;
            }
        }

        public void ValidaOperandos(Addressing _ep)
        {
            foreach(Instruction _sb in this)
                _sb.ValidaOperandosSimbolo(_ep);
        }


        private bool ProcuraCodigoInterpretavel(Instruction _sb)
        {
            if (_sb.OpCode== ciLocal)
                return true;
            else
                return false;
        }

        private static bool VerificaSeSimboloTemTodosOperandos(Instruction instruction)
        {
            bool _bResult = true;
            for (int i = 0; i < instruction.GetNumberOfOperands(); i++)
            {
                if (instruction.GetOperand(i) == null)
                {
                    _bResult = false;
                    break;
                }
            }
            return _bResult;
        }
      
        
        private static Instruction SimboloBasicoToSimboloBasico(Instruction _sb)
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
