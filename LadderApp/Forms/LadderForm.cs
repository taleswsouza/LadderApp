
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using LadderApp.Formularios;

namespace LadderApp
{
    public partial class LadderForm : Form
    {
        private int tempVertical = 0;
        private int tempHorizontal = 0;

        public ProjectForm projectForm;

        public VisualProgram visualProgram;

        private Boolean ResizingLines = false;

        public VisualInstructionUserControl VisualInstruction { get; private set; } = null;
        public VisualLine SelectedVisualLine { get; private set; } = null;


        public LadderForm(LadderProgram program)
        {
            InitializeComponent();

            this.VScroll = false;
            this.HScroll = false;

            visualProgram = new VisualProgram(program, this);
        }

        private void DiagramaLadder_Resize(object sender, EventArgs e)
        {
            if (ResizingLines == false)
            {
                ResizingLines = true;
                ReorganizeLines();
                ResizingLines = false;
            }
        }

        public void SetMessage(String Texto)
        {
            //lblstatusMensagem.Text = Texto;
        }

        public void ApagaLinhasSimbolos()
        {
            foreach (VisualLine lcl in visualProgram.Lines)
            {
            }
        }

        public void LineBeginVisualInstruction_DeleteLine(VisualLine sender)
        {
            int _indiceLinhaDeletar = this.visualProgram.Lines.IndexOf(sender);
            int _indiceLinha = _indiceLinhaDeletar;

            if (_indiceLinha >= 0)
            {
                this.SuspendLayout();

                /// Faz todos os tratamentos para
                /// desconsiderar a linha a ser deleta
                /// OBS.: Isto garante um melhor agilidade
                ///     no desenho da tela.
                this.visualProgram.ApagaLinha(_indiceLinha);

                if (_indiceLinha == 0)
                    _indiceLinha = 1;

                if (this.visualProgram.Lines.Count > 0)
                    this.visualProgram.Lines[_indiceLinha - 1].LineBegin.Select();

                this.ReorganizeLines();

                if (this.visualProgram.Lines.Count > 0)
                    this.visualProgram.Lines[_indiceLinha - 1].LineBegin.Refresh();

                this.ResumeLayout();
            }
        }

        public void ReorganizeLines()
        {
            ReorganizeLines(0);
        }


        public void ReorganizeLines(int numberOfTimes)
        {
            int auxY = 0;
            int iTabStop = 0;
            int iLinha = 0;
            int auxX = 0;

            if (visualProgram != null)
            {
                if (visualProgram.Lines.Count > 0)
                {
                    if (numberOfTimes == 0)
                    {
                        tempVertical = this.VerticalScroll.Value;
                        tempHorizontal = this.HorizontalScroll.Value;
                    }

                    try
                    {
                        this.AutoScroll = false;
                        this.VerticalScroll.Value = 0;
                        this.HorizontalScroll.Value = 0;
                    }
                    catch (Exception ex)
                    {
                        SetMessage(ex.Message);
                    }
                    VisualLine previsousVisualLine = visualProgram.Lines[visualProgram.Lines.Count - 1];

                    if (numberOfTimes == 0)
                        visualProgram.Lines[visualProgram.Lines.Count - 1].LineEnd.PositionXY = new Point(auxX, 0);

                    foreach (VisualLine visualLine in visualProgram.Lines)
                    {
                        if (previsousVisualLine != null)
                            previsousVisualLine.NextLine = visualLine;
                        visualLine.PreviousLine = previsousVisualLine;
                        visualLine.tabStop = iTabStop;
                        visualLine.YPosition = auxY;
                        visualLine.NumLinha = iLinha;
                        visualLine.LineBegin.Invalidate();
                        visualLine.AdjustPositioning();
                        auxY += visualLine.BackgroundLine.tamanhoXY.Height;
                        iTabStop = visualLine.tabStop;
                        iLinha++;
                        previsousVisualLine = visualLine;

                        visualLine.BackgroundLine.Invalidate();

                        if (auxX < visualLine.LineEnd.PositionXY.X)
                            auxX = visualLine.LineEnd.PositionXY.X;

                    }
                    previsousVisualLine.NextLine = visualProgram.Lines[0];

                    if (tempVertical > 0 || tempHorizontal > 0)
                    {
                        this.VerticalScroll.Value = tempVertical;
                        this.HorizontalScroll.Value = tempHorizontal;
                    }
                    this.AutoScroll = true;
                }

                if (numberOfTimes == 0 && visualProgram.Lines.Count > 0)
                {
                    visualProgram.Lines[visualProgram.Lines.Count - 1].LineEnd.PositionXY = new Point(auxX, visualProgram.Lines[0].LineEnd.PositionXY.Y);
                    ReorganizeLines(1);
                }

                if (numberOfTimes == 1)
                {
                    if (visualProgram.Lines.Count > 0)
                    {
                        this.VerticalScroll.Maximum = visualProgram.Lines[visualProgram.Lines.Count - 1].LineEnd.PositionXY.Y + visualProgram.Lines[visualProgram.Lines.Count - 1].LineEnd.tamanhoXY.Height;
                        this.HorizontalScroll.Maximum = auxX;
                    }
                    else
                    {
                        this.VerticalScroll.Maximum = this.Height;
                        this.HorizontalScroll.Maximum = this.Width;
                    }
                }
            }
        }

        public int InsertLine()
        {
            return InsertLine(false);
        }

        public int InsertLine(bool above)
        {
            int lineIndex = 0;
            if (SelectedVisualLine != null)
            {
                if (!SelectedVisualLine.LineBegin.IsDisposed)
                {
                    lineIndex = (int)SelectedVisualLine.LineBegin.GetOperand(0);

                    if (above == false)
                        lineIndex++;
                }
                else
                    lineIndex = 0;
            }

            this.SuspendLayout();

            lineIndex = visualProgram.InsertLineAt(lineIndex);
            this.ReorganizeLines();

            visualProgram.Lines[lineIndex].LineBegin.Select();
            visualProgram.Lines[lineIndex].LineBegin.Refresh();


            this.ResumeLayout();

            return lineIndex;
        }

        private void mnuInsertLadderLineBelow_Click(object sender, EventArgs e)
        {
            this.InsertLine();
        }

        private void mnuInsertLadderLineAbove_Click(object sender, EventArgs e)
        {
            this.InsertLine(true);
        }

        private void LadderForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                /// Enderecamentos e somente sobre controleslivres
                this.mnuAddressing.Enabled = false;
                this.mnuAddressing.Visible = false;

                /// Manipular linhas
                this.mnuInsertLadderLine.Enabled = true;

                /// Extensao de paralelo - acima/abaixo
                ///    somente sobre simbolos de paralelo
                this.mnuExtendParallelBranchAbove.Enabled = false;
                this.mnuExtendParallelBranchAbove.Visible = false;
                this.mnuExtendParallelBranchBelow.Enabled = false;
                this.mnuExtendParallelBranchBelow.Visible = false;

                this.mnuToggleBit.Enabled = false;

                this.mnuContextAtInstruction.Show(this.PointToScreen(e.Location));
            }
        }

        public void LineBeginVisualInstruction_ChangeLine(VisualInstructionUserControl sender, Keys e)
        {
            if (e == Keys.Up)
            {
                if ((int)sender.VisualLine.LineBegin.GetOperand(0) != 0)
                    sender.VisualLine.PreviousLine.LineBegin.Select();
            }
            else if (e == Keys.Down)
            {
                if ((int)sender.VisualLine.NextLine.LineBegin.GetOperand(0) != 0)
                    sender.VisualLine.NextLine.LineBegin.Select();
            }
        }

        public void VisualInstruction_Selected(VisualInstructionUserControl visualInstruction, VisualLine visualLine)
        {
            if (VisualInstruction != null)
                if (!VisualInstruction.IsDisposed)
                    if (!VisualInstruction.Equals(visualInstruction))
                    {
                        VisualInstruction.Selected = false;
                        VisualInstruction.Refresh();
                    }
            VisualInstruction = visualInstruction;
            SelectedVisualLine = visualLine;
        }

        public void OutputTypeBox_MouseHover(object sender, EventArgs e)
        {
            VisualInstructionUserControl visualInstruction = (VisualInstructionUserControl)sender;
            if (visualInstruction.Tag != null && (String)visualInstruction.Tag != "")
                toolTipOutputBox.Show((String)visualInstruction.Tag, visualInstruction, 3000);
        }


        public void VisualInstruction_KeyDown(object sender, KeyEventArgs e)
        {
            VisualInstructionUserControl visualInstruction = (VisualInstructionUserControl)sender;
            switch (visualInstruction.OpCode)
            {
                case OperationCode.None:
                    /// Tive que colocar aqui esta opcao de NENHUM para evitar que
                    /// a execucao passasse duas vezes em apagar
                    break;
                case OperationCode.LineBegin:
                    break;
                default:
                    if (e.KeyCode == Keys.Delete)
                    {
                        if (visualInstruction != null && SelectedVisualLine != null)
                            if (!visualInstruction.IsDisposed)
                            {
                                this.SuspendLayout();
                                this.SelectNextControl(visualInstruction, false, true, false, false);
                                SelectedVisualLine.RemoveVisualInstruction(visualInstruction);
                                this.ReorganizeLines();
                                this.ResumeLayout();
                                SelectedVisualLine.BackgroundLine.Invalidate();
                            }
                    }
                    break;
            }
        }

        public List<Instruction> VariosSelecionados(VisualInstructionUserControl _cL, VisualLine _lCL)
        {
            OperationCode _cI = _cL.OpCode;
            List<Instruction> instructions = new List<Instruction>();
            List<VisualInstructionUserControl> _lstCL = null;

            switch (_cI)
            {
                case OperationCode.ParallelBranchBegin:
                case OperationCode.ParallelBranchNext:
                case OperationCode.ParallelBranchEnd:
                    int _indicePosInicial = 0;
                    int _indicePosFinal = 0;

                    /// verifica em qual lista o controle esta presente (simbolos/saida)
                    if (SelectedVisualLine.visualInstructions.Contains(_cL))
                        _lstCL = SelectedVisualLine.visualInstructions;
                    else if (SelectedVisualLine.visualOutputInstructions.Contains(_cL))
                        _lstCL = SelectedVisualLine.visualOutputInstructions;
                    else
                        return instructions;

                    /// define a posicao inicial a partir da posicao
                    /// do controle na lista
                    _indicePosInicial = _lstCL.IndexOf(_cL);

                    if (_cI == OperationCode.ParallelBranchEnd)
                    {
                        /// se for paralelo final, inverte a posicial inicial/final
                        _indicePosFinal = _indicePosInicial;
                        /// se for paralelo final, a posicao inicial e
                        /// o paralelo inical
                        _indicePosInicial = _lstCL.IndexOf(_cL.Aponta2PI);
                    }
                    else
                        /// senao for final aponta para o proximo item de paralelo
                        _indicePosFinal = _lstCL.IndexOf(_cL.Aponta2proxPP);

                    /// pega todos os controles dentro da faixa inicial / final
                    for (int i = _indicePosInicial; i <= _indicePosFinal; i++)
                    {
                        instructions.Add(_lstCL[i].Instruction);
                    }
                    break;
                default:
                    instructions.Add(_cL.Instruction);
                    break;
            }
            return instructions;
        }

        private void mnuClearAddress_Click(object sender, EventArgs e)
        {
            OperationCode _cI = VisualInstruction.OpCode;
            if ((!VisualInstruction.IsDisposed) &&
                 (_cI != OperationCode.LineBegin &&
                 _cI != OperationCode.ParallelBranchBegin &&
                 _cI != OperationCode.ParallelBranchEnd))
            {
                VisualInstruction.SetOperand(0, null);
                VisualInstruction.Refresh();
            }
        }

        private void LadderForm_Scroll(object sender, ScrollEventArgs e)
        {
        }

        private void mnuToggleBit_Click(object sender, EventArgs e)
        {
            ((Address)VisualInstruction.GetOperand(0)).Value = ((Address)VisualInstruction.GetOperand(0)).Value == true ? false : true;
            projectForm.Program.SimulateLadder();
            this.Invalidate(true);
        }

        public void ControleSelecionado_SolicitaMudarEndereco(VisualInstructionUserControl sender, Rectangle rect, Type tipo, int valorMax, int valorMin, params object[] faixa)
        {
            if (!sender.IsAllOperandsOk())
            {
                MessageBox.Show("Please, assign an address first!", "Change configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            ChangeTimerCounterParametersForm changeTimerCounterParametersForm = new ChangeTimerCounterParametersForm(sender.OpCode);
            switch (sender.OpCode)
            {
                case OperationCode.Timer:
                    changeTimerCounterParametersForm.Type = (Int32)((Address)sender.GetOperand(0)).Timer.Type;
                    changeTimerCounterParametersForm.Preset = (Int32)((Address)sender.GetOperand(0)).Timer.Preset;
                    changeTimerCounterParametersForm.Accumulated = (Int32)((Address)sender.GetOperand(0)).Timer.Accumulated;
                    changeTimerCounterParametersForm.TimeBase = (Int32)((Address)sender.GetOperand(0)).Timer.TimeBase;
                    break;
                case OperationCode.Counter:
                    changeTimerCounterParametersForm.Type = (Int32)((Address)sender.GetOperand(0)).Counter.Type;
                    changeTimerCounterParametersForm.Preset = (Int32)((Address)sender.GetOperand(0)).Counter.Preset;
                    changeTimerCounterParametersForm.Accumulated = (Int32)((Address)sender.GetOperand(0)).Counter.Accumulated;
                    break;
                default:
                    break;
            }
            DialogResult result = changeTimerCounterParametersForm.ShowDialog();

            if (result == DialogResult.OK)
            {
                /// mantem os parametros do ci atualizados
                sender.SetOperand(1, changeTimerCounterParametersForm.Type);
                sender.SetOperand(2, changeTimerCounterParametersForm.Preset);
                sender.SetOperand(3, changeTimerCounterParametersForm.Accumulated);
                switch (sender.OpCode)
                {
                    case OperationCode.Timer:
                        /// mantem os parametros do ci atualizados
                        sender.SetOperand(4, changeTimerCounterParametersForm.TimeBase);

                        ((Address)sender.GetOperand(0)).Timer.Type = changeTimerCounterParametersForm.Type;
                        ((Address)sender.GetOperand(0)).Timer.Preset = changeTimerCounterParametersForm.Preset;
                        ((Address)sender.GetOperand(0)).Timer.Accumulated = changeTimerCounterParametersForm.Accumulated;
                        ((Address)sender.GetOperand(0)).Timer.TimeBase = changeTimerCounterParametersForm.TimeBase;

                        sender.SetOperand(1, ((Address)sender.GetOperand(0)).Timer.Type);
                        sender.SetOperand(2, ((Address)sender.GetOperand(0)).Timer.Preset);
                        sender.SetOperand(3, ((Address)sender.GetOperand(0)).Timer.Accumulated);
                        sender.SetOperand(4, ((Address)sender.GetOperand(0)).Timer.TimeBase);

                        break;
                    case OperationCode.Counter:
                        ((Address)sender.GetOperand(0)).Counter.Type = changeTimerCounterParametersForm.Type;
                        ((Address)sender.GetOperand(0)).Counter.Preset = changeTimerCounterParametersForm.Preset;
                        ((Address)sender.GetOperand(0)).Counter.Accumulated = changeTimerCounterParametersForm.Accumulated;

                        sender.SetOperand(1, ((Address)sender.GetOperand(0)).Counter.Type);
                        sender.SetOperand(2, ((Address)sender.GetOperand(0)).Counter.Preset);
                        sender.SetOperand(3, ((Address)sender.GetOperand(0)).Counter.Accumulated);
                        break;
                    default:
                        break;
                }

                sender.Invalidate();
            }

        }

        private void mnuToggleBitPulse_Click(object sender, EventArgs e)
        {
            projectForm.Program.auxToggleBitPulse = ((Address)VisualInstruction.GetOperand(0));
            projectForm.Program.auxToggleBitPulse.Value = projectForm.Program.auxToggleBitPulse.Value == true ? false : true;
            projectForm.Program.SimulateLadder();
            this.Invalidate(true);
        }

    }
}