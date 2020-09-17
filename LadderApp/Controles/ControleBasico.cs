using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace LadderApp
{
    public class ControleBasico : UserControl, ISimbolo
    {
        protected SimboloBasico codigoInterpretavel = null;

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

        public ControleBasico()
        {
        }

        public virtual void setCI(CodigosInterpretaveis codInterpretavelNovo)
        {
                codigoInterpretavel.setCI(codInterpretavelNovo);
        }

        virtual public void DesenhaSimbolo()
        {
        }

        public CodigosInterpretaveis getCI()
        {
            if (codigoInterpretavel != null)
                return codigoInterpretavel.getCI();
            else
                return CodigosInterpretaveis.NENHUM;

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
