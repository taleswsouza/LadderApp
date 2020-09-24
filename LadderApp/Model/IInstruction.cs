using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace LadderApp
{
    public interface IInstruction
    {
        OperationCode OpCode { get; set; }
        Object[] Operands { get; set; }
        int GetNumberOfOperands();
        bool IsAllOperandsOk();
        Object GetOperand(int posicao);
        void SetOperand(int position, Object value);

        Size tamanhoXY
        {
            get;
            set;
        }

        Point posicaoXY
        {
            get;
            set;
        }

        Point XYConexao
        {
            get;
        }
    }
}
