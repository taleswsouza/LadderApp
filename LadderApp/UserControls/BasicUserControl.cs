using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace LadderApp
{
    public class BasicUserControl : UserControl, ISymbol
    {
        protected Symbol codigoInterpretavel = null;

        public bool selecionado = false;

        protected Point xyConexao;
        public Point XYConexao
        {
            get { return xyConexao; }
        }

        protected Size TamanhoXY;
        public Size tamanhoXY
        {
            get { return TamanhoXY; }
            set { TamanhoXY = value; }
        }

        protected Point PosicaoXY;
        public Point posicaoXY
        {
            get { return PosicaoXY; }
            set { PosicaoXY = value; }
        }

        public BasicUserControl()
        {
        }

        virtual public void DesenhaSimbolo()
        {
        }

        public OperationCode OpCode
        {
            get
            {
                if (codigoInterpretavel != null)
                    return codigoInterpretavel.OpCode;
                else
                    return OperationCode.None;

            }

            set => codigoInterpretavel.OpCode = value;
        }

        public Object[] getOperandos()
        {
            return codigoInterpretavel.getOperandos();
        }

        public Object getOperandos(int posicao)
        {
            return codigoInterpretavel.getOperandos(posicao);
        }

        public void setOperando(int iNumOperando, Object valor)
        {
            codigoInterpretavel.setOperando(iNumOperando, valor);
        }

        public void setOperando(Object[] operandos)
        {
            codigoInterpretavel.setOperando(operandos);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.BackColor = System.Drawing.Color.Transparent;
            this.Name = "ControleBasico";
            this.ResumeLayout(false);

        }
    }
}
