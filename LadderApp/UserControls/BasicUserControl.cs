using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace LadderApp
{
    public class BasicUserControl : UserControl, IInstruction
    {
        protected Instruction instruction;
        public bool Selected { get; set; } = false;

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
        public Point PositionXY
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
                if (instruction != null)
                    return instruction.OpCode;
                else
                    return OperationCode.None;

            }

            set => instruction.OpCode = value;
        }

        public object[] Operands { get => ((IInstruction)instruction).Operands; set => ((IInstruction)instruction).Operands = value; }

        public Object GetOperand(int posicao)
        {
            return instruction.GetOperand(posicao);
        }

        public void SetOperand(int iNumOperando, Object valor)
        {
            instruction.SetOperand(iNumOperando, valor);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.BackColor = System.Drawing.Color.Transparent;
            this.Name = "ControleBasico";
            this.ResumeLayout(false);

        }

        public int GetNumberOfOperands()
        {
            return ((IInstruction)instruction).GetNumberOfOperands();
        }

        public bool IsAllOperandsOk()
        {
            return ((IInstruction)instruction).IsAllOperandsOk();
        }
    }
}
