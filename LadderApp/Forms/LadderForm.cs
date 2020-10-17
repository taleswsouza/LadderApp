
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using LadderApp.Formularios;
using LadderApp.Services;

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

        public void ShowMessageInStatus(String message)
        {
            lblstatusMessage.Text = message;
        }


        public void LineBeginVisualInstruction_DeleteLine(VisualLine sender)
        {
            int lineIndex = visualProgram.Lines.IndexOf(sender);

            if (lineIndex >= 0)
            {
                SuspendLayout();

                visualProgram.DeleteLine(lineIndex);

                if (lineIndex == 0)
                    lineIndex = 1;

                if (visualProgram.Lines.Count > 0)
                    visualProgram.Lines[lineIndex - 1].LineBegin.Select();

                ReorganizeLines();

                if (visualProgram.Lines.Count > 0)
                    visualProgram.Lines[lineIndex - 1].LineBegin.Refresh();

                ResumeLayout();
            }
        }

        public void ReorganizeLines()
        {
            ReorganizeLines(0);
        }


        public void ReorganizeLines(int numberOfTimes)
        {
            int auxY = 0;
            int lineNumber = 0;
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
                        ShowMessageInStatus(ex.Message);
                    }
                    VisualLine previsousVisualLine = visualProgram.Lines[visualProgram.Lines.Count - 1];

                    if (numberOfTimes == 0)
                        visualProgram.Lines[visualProgram.Lines.Count - 1].LineEnd.XYPosition = new Point(auxX, 0);

                    foreach (VisualLine visualLine in visualProgram.Lines)
                    {
                        if (previsousVisualLine != null)
                            previsousVisualLine.NextLine = visualLine;
                        visualLine.PreviousLine = previsousVisualLine;
                        visualLine.YPosition = auxY;
                        visualLine.LineNumber = lineNumber;
                        visualLine.LineBegin.Invalidate();
                        visualLine.AdjustPositioning();
                        auxY += visualLine.BackgroundLine.XYSize.Height;
                        lineNumber++;
                        previsousVisualLine = visualLine;

                        visualLine.BackgroundLine.Invalidate();

                        if (auxX < visualLine.LineEnd.XYPosition.X)
                            auxX = visualLine.LineEnd.XYPosition.X;

                    }
                    previsousVisualLine.NextLine = visualProgram.Lines[0];

                    if (tempVertical > 0 || tempHorizontal > 0)
                    {
                        VerticalScroll.Value = Math.Min(tempVertical, VerticalScroll.Maximum);
                        HorizontalScroll.Value = Math.Min(tempHorizontal, HorizontalScroll.Maximum);
                    }
                    this.AutoScroll = true;
                }

                if (numberOfTimes == 0 && visualProgram.Lines.Count > 0)
                {
                    visualProgram.Lines[visualProgram.Lines.Count - 1].LineEnd.XYPosition = new Point(auxX, visualProgram.Lines[0].LineEnd.XYPosition.Y);
                    ReorganizeLines(1);
                }

                if (numberOfTimes == 1)
                {
                    if (visualProgram.Lines.Count > 0)
                    {
                        this.VerticalScroll.Maximum = visualProgram.Lines[visualProgram.Lines.Count - 1].LineEnd.XYPosition.Y + visualProgram.Lines[visualProgram.Lines.Count - 1].LineEnd.XYSize.Height;
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
                this.mnuAddressing.Enabled = false;
                this.mnuAddressing.Visible = false;

                this.mnuInsertLadderLine.Enabled = true;

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
                                SuspendLayout();
                                SelectNextControl(visualInstruction, false, true, false, false);
                                SelectedVisualLine.RemoveVisualInstruction(visualInstruction);
                                ReorganizeLines();
                                ResumeLayout();
                                SelectedVisualLine.BackgroundLine.Invalidate();
                            }
                    }
                    break;
            }
        }

        public List<Instruction> VariosSelecionados(VisualInstructionUserControl visualInstruction)
        {
            OperationCode opCode = visualInstruction.OpCode;
            List<Instruction> instructions = new List<Instruction>();
            List<VisualInstructionUserControl> visualInstructions;

            switch (opCode)
            {
                case OperationCode.ParallelBranchBegin:
                case OperationCode.ParallelBranchNext:
                case OperationCode.ParallelBranchEnd:
                    int initialPositionIndex;
                    int finalPositionIndex;

                    if (SelectedVisualLine.visualInstructions.Contains(visualInstruction))
                        visualInstructions = SelectedVisualLine.visualInstructions;
                    else if (SelectedVisualLine.visualOutputInstructions.Contains(visualInstruction))
                        visualInstructions = SelectedVisualLine.visualOutputInstructions;
                    else
                        return instructions;

                    initialPositionIndex = visualInstructions.IndexOf(visualInstruction);

                    if (opCode == OperationCode.ParallelBranchEnd)
                    {
                        finalPositionIndex = initialPositionIndex;
                        initialPositionIndex = visualInstructions.IndexOf(visualInstruction.PointToParallelBegin);
                    }
                    else
                        finalPositionIndex = visualInstructions.IndexOf(visualInstruction.PointToNextParallelPoint);

                    for (int i = initialPositionIndex; i <= finalPositionIndex; i++)
                    {
                        instructions.Add(visualInstructions[i].Instruction);
                    }
                    break;
                default:
                    instructions.Add(visualInstruction.Instruction);
                    break;
            }
            return instructions;
        }

        private void mnuClearAddress_Click(object sender, EventArgs e)
        {
            OperationCode opCode = VisualInstruction.OpCode;
            if ((!VisualInstruction.IsDisposed) &&
                 (opCode != OperationCode.LineBegin &&
                 opCode != OperationCode.ParallelBranchBegin &&
                 opCode != OperationCode.ParallelBranchEnd))
            {
                VisualInstruction.SetOperand(0, null);
                VisualInstruction.Refresh();
            }
        }

        private void mnuToggleBit_Click(object sender, EventArgs e)
        {
            ((Address)VisualInstruction.GetOperand(0)).Value = ((Address)VisualInstruction.GetOperand(0)).Value == true ? false : true;

            LadderSimulatorServices simultatorService = new LadderSimulatorServices();
            simultatorService.SimulateLadder(projectForm.Program);
            //projectForm.Program.SimulateLadder();
            this.Invalidate(true);
        }

        public void VisualInstruction_AskToChangeAddress(VisualInstructionUserControl sender)
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
            LadderSimulatorServices simultatorService = new LadderSimulatorServices();

            Address toggleBitPulse = (Address)VisualInstruction.GetOperand(0);
            toggleBitPulse.Value = toggleBitPulse.Value != true;
            simultatorService.SimulateLadder(projectForm.Program, toggleBitPulse);

            this.Invalidate(true);
        }
    }
}