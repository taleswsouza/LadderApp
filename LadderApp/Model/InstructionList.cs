using System;
using System.Collections.Generic;
using System.Text;

namespace LadderApp
{
    public class InstructionList : List<Instruction>, IList<Instruction>
    {
        /// <summary>
        /// suporte - utilizado na funcao "ProcuraCodigoInterpretavel()" - carrega
        /// o codigointerpretavel para as iteracoes da funcao
        /// </summary>
        private OperationCode operationCode = OperationCode.None;

        /// <summary>
        /// Verifica se a lista contem o codigointerpretavel do parametro
        /// </summary>
        /// <param name="opCode">codigo interpretavel a ser verificado na lista</param>
        /// <returns>true - codigo encontrado na lista / false - codigno nao encontrado na lista</returns>
        public bool Contains(OperationCode opCode)
        {
            operationCode = opCode;
            bool _bResult = this.Exists(ProcuraCodigoInterpretavel);
            operationCode = OperationCode.None;
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


        public bool HasDuplicatedTimers(List<Address> usedTimers)
        {
            foreach (Instruction instruction in this)
            {
                if (instruction.OpCode== OperationCode.Timer)
                {
                    if (usedTimers.Contains((Address)instruction.GetOperand(0)))
                        return false;
                    else
                        usedTimers.Add((Address)instruction.GetOperand(0));
                }
            }
            return true;
        }

        public bool HasDuplicatedCounters(List<Address> usedCounters)
        {
            foreach (Instruction instruction in this)
            {
                if (instruction.OpCode== OperationCode.Counter)
                {
                    if (usedCounters.Contains((Address)instruction.GetOperand(0)))
                        return false;
                    else
                        usedCounters.Add((Address)instruction.GetOperand(0));
                }
            }
            return true;
        }
        
        
        public InstructionList InsertAllWithClearBefore(List<Instruction> instructions)
        {
            this.Clear();

            if (instructions.Count > 0)
                foreach (Instruction instruction in instructions)
                {
                    this.Add(instruction);
                }

            return this;
        }


        public InstructionList InsertAllWithClearBefore(OperationCode [] _arrayCI)
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
                this.Insert(i, new Instruction(OperationCode.ParallelBranchNext));
            }
        }


        public void InsereParalelo(TipoInsercaoParalelo _tIP)
        {
            InsereParaleloProximo();

            switch (_tIP)
            {
                case TipoInsercaoParalelo.PARALELO_INICIADO:
                    this[0].OpCode = OperationCode.ParallelBranchBegin;
                    break;
                case TipoInsercaoParalelo.PARALELO_FINALIZADO:
                    this.Add(new Instruction(OperationCode.ParallelBranchEnd));
                    break;
                case TipoInsercaoParalelo.PARALELO_COMPLETO:
                    this[0].OpCode = OperationCode.ParallelBranchBegin;
                    this.Add(new Instruction(OperationCode.ParallelBranchEnd));
                    break;
            }
        }

        public void ValidaOperandos(Addressing _ep)
        {
            foreach(Instruction instruction in this)
                instruction.ValidateInstructionOperands(_ep);
        }


        private bool ProcuraCodigoInterpretavel(Instruction instruction)
        {
            if (instruction.OpCode== operationCode)
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
      
        public enum TipoInsercaoParalelo
        {
            PARALELO_INICIADO,
            PARALELO_FINALIZADO,
            PARALELO_COMPLETO
        }
    }
}
