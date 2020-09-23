using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace LadderApp
{
    public interface ISymbol
    {
        OperationCode OpCode { get; set; }

        Object[] getOperandos();
        Object getOperandos(int posicao);
        void setOperando(int iNumOperando, Object valor);
        void setOperando(Object[] operandos);

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
