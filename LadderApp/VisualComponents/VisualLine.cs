using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using LadderApp.Model;
using LadderApp.Model.Instructions;

namespace LadderApp
{
    public partial class VisualLine
    {
        private int tabStop = 0;
        private Line line;


        public VisualLine()
        {
        }

        public VisualLine(LadderForm ladderForm, Line line)
        {
            this.ladderForm = ladderForm;
            this.line = line;

            InitializeFixedVisualInstructionsToInitialVisualLine();
        }

        // links
        private LadderForm ladderForm;
        public VisualLine PreviousLine { get; set; }
        public VisualLine NextLine { get; set; }

        // fixed visual instructions
        public VisualInstructionUserControl LineBegin = new VisualInstructionUserControl(OperationCode.LineBegin);
        public VisualInstructionUserControl LineEnd = new VisualInstructionUserControl(OperationCode.LineEnd);
        public VisualInstructionUserControl BackgroundLine = new VisualInstructionUserControl(OperationCode.BackgroundLine);

        // dinamic visual instructions
        public List<VisualInstructionUserControl> visualInstructions = new List<VisualInstructionUserControl>();
        public List<VisualInstructionUserControl> visualOutputInstructions = new List<VisualInstructionUserControl>();

        public enum LocalToInsertInstruction
        {
            ConditionsAtLeft,
            OutputsAtRight,
            Undefined
        }

        private int lineNumber = 0;
        public int LineNumber
        {
            get { return lineNumber; }
            set
            {
                LineBegin.SetOperand(0, value);
                lineNumber = value;
            }
        }

        public int XPosition { get; set; } = 0;
        public int YPosition { get; set; } = 0;

        public int XSize = 70;
        public int YSize = 80;

        private int XPositionToFirstOutputVisualInstruction = 0;

        public double percentageVisualInstructionReduction = 1D;

        private int xTotalHorizontal = 0;
        public int xTotal
        {
            get { return xTotalHorizontal; }
            set { xTotalHorizontal = value - 10; }
        }


        private int yTotalVertical = 0;
        public int yTotal
        {
            get { return yTotalVertical; }
            set { yTotalVertical = value - 10; }
        }

        public void AdjustPositioning()
        {
            VisualInstructionUserControl visualInstructionAux = null;

            int biggerX = 0;

            int countInstructions = 0;
            int accumulatedXAtFirstOutputInstruction = 0;
            int accumulatedXAtBiggerOutputInstruction = 0;
            int outputSize = 0;

            VisualParallelBranch parallelBranch = null;
            List<VisualParallelBranch> visualParallelBranches = new List<VisualParallelBranch>();

            int lastSizeOfYToFinalParallelBranch = 0;

            int sizeOfYToBackgroundDraw = 0;

            int xPosition = XPosition;
            int yPosition = YPosition;

            int xSize = XSize;
            int ySize = YSize;

            int xSizeAccumulated = XSize;
            int ySizeAccumulated = YSize;

            visualInstructions.AddRange(visualOutputInstructions);

            foreach (VisualInstructionUserControl visualInstruction in visualInstructions)
            {
                countInstructions++;

                if (visualParallelBranches.Count == 0)
                {
                    if (biggerX > 0)
                    {
                        xPosition = biggerX;
                    }
                    biggerX = 0;
                }
                else
                {
                    if (biggerX < (xPosition + xSize))
                    {
                        biggerX = xPosition + xSize;
                    }
                }

                switch (visualInstruction.OpCode)
                {
                    case OperationCode.ParallelBranchBegin:
                        ySize = YSize; // restaura tamanho Y base
                        xPosition = xSizeAccumulated;
                        xSize = this.XSize / 3;

                        parallelBranch = new VisualParallelBranch();
                        visualParallelBranches.Add(parallelBranch);

                        parallelBranch.accumulatedY = yPosition;
                        parallelBranch.biggerY = yPosition;
                        parallelBranch.biggerX = biggerX;
                        parallelBranch.parallelBranchBegin = visualInstruction;

                        xSizeAccumulated = xPosition + xSize;
                        break;
                    case OperationCode.ParallelBranchEnd:
                        xSize = this.XSize / 3;
                        ySize = parallelBranch.lastParallelBranchNext.XYPosition.Y - parallelBranch.parallelBranchBegin.XYPosition.Y + parallelBranch.lastParallelBranchNext.XYSize.Height; // _ultTamY2ParaleloFinal;
                        yPosition = parallelBranch.parallelBranchBegin.XYPosition.Y;

                        if (biggerX > parallelBranch.biggerX)
                            parallelBranch.biggerX = biggerX;

                        xPosition = parallelBranch.biggerX;

                        if (lastSizeOfYToFinalParallelBranch > sizeOfYToBackgroundDraw)
                        {
                            sizeOfYToBackgroundDraw = lastSizeOfYToFinalParallelBranch;
                        }

                        visualInstruction.AssociateParallelBranchList(parallelBranch.parallelBranchList);

                        visualInstruction.PointToParallelBegin = parallelBranch.parallelBranchBegin;
                        parallelBranch.parallelBranchBegin.PointToParallelEnd = visualInstruction;
                        parallelBranch.lastParallelBranchNext.PointToNextParallelPoint = visualInstruction;

                        parallelBranch.lastParallelBranchNext.LastParallelBranchEnd = true;

                        parallelBranch = null;
                        visualParallelBranches.RemoveAt(visualParallelBranches.Count - 1);

                        if (visualParallelBranches.Count > 0)
                        {
                            parallelBranch = visualParallelBranches[visualParallelBranches.Count - 1];
                        }


                        if (visualParallelBranches.Count > 0)
                        {
                            if (lastSizeOfYToFinalParallelBranch > parallelBranch.biggerY)
                            {
                                parallelBranch.biggerY = lastSizeOfYToFinalParallelBranch;
                            }
                        }

                        xSizeAccumulated = xPosition + xSize;
                        break;
                    case OperationCode.ParallelBranchNext:
                        ySize = YSize;
                        xSize = this.XSize / 3;

                        if (biggerX > parallelBranch.biggerX)
                            parallelBranch.biggerX = biggerX;
                        biggerX = 0;

                        xPosition = parallelBranch.parallelBranchBegin.XYPosition.X;

                        parallelBranch.accumulatedY = parallelBranch.biggerY;
                        yPosition = parallelBranch.biggerY + (YSize * (parallelBranch.numberOfParallelBranchNext + 1));

                        if (parallelBranch.numberOfParallelBranchNext > 0)
                        {
                            visualInstructionAux = parallelBranch.lastParallelBranchNext;
                            visualInstructionAux.LastParallelBranchEnd = false;
                        }
                        else
                        {
                            visualInstructionAux = parallelBranch.parallelBranchBegin;
                        }

                        visualInstructionAux.XYSize = new Size(visualInstructionAux.XYSize.Width, yPosition - visualInstructionAux.XYPosition.Y);

                        visualInstructionAux.PointToNextParallelPoint = visualInstruction;

                        parallelBranch.numberOfParallelBranchNext += 1;
                        parallelBranch.lastParallelBranchNext = visualInstruction;
                        lastSizeOfYToFinalParallelBranch = yPosition;

                        parallelBranch.parallelBranchList.Add(visualInstruction);

                        xSizeAccumulated = xPosition + xSize;
                        break;
                    default:
                        ySize = YSize;
                        xSize = XSize;
                        xPosition = xSizeAccumulated;
                        xSizeAccumulated += xSize;
                        break;
                }

                visualInstruction.XYPosition = new Point(xPosition, yPosition);
                visualInstruction.XYSize = new Size(xSize, ySize);

                if ((visualOutputInstructions.Count > 0) && (countInstructions >= (visualInstructions.Count - visualOutputInstructions.Count)))
                {
                    if (countInstructions == (visualInstructions.Count - visualOutputInstructions.Count))
                    {
                        accumulatedXAtFirstOutputInstruction = xSizeAccumulated;
                    }

                    if (accumulatedXAtBiggerOutputInstruction < xSizeAccumulated)
                    {
                        accumulatedXAtBiggerOutputInstruction = xSizeAccumulated;
                    }
                }
            }

            outputSize = (accumulatedXAtBiggerOutputInstruction - accumulatedXAtFirstOutputInstruction);// -this.tamX;
            if (outputSize == 0)
            {
                outputSize = XSize;
            }

            visualInstructions.RemoveRange((visualInstructions.Count - visualOutputInstructions.Count), visualOutputInstructions.Count);

            sizeOfYToBackgroundDraw += YSize;

            if ((YPosition > YSize) && (sizeOfYToBackgroundDraw > YPosition))
            {
                sizeOfYToBackgroundDraw -= YPosition;
            }

            LineBegin.XYPosition = new Point(XPosition, YPosition);
            LineBegin.XYSize = new Size(this.XSize, sizeOfYToBackgroundDraw);
            LineBegin.Location = new Point(XPosition, YPosition);
            LineBegin.Size = new Size(this.XSize, sizeOfYToBackgroundDraw);

            if (xSizeAccumulated < (ladderForm.Width - this.XSize))
            {
                xSizeAccumulated = (ladderForm.Width - this.XSize);
            }

            if (PreviousLine != null)
            {
                if (xSizeAccumulated < PreviousLine.LineEnd.XYPosition.X)
                {
                    xSizeAccumulated = PreviousLine.LineEnd.XYPosition.X;
                }
            }

            XPositionToFirstOutputVisualInstruction = xSizeAccumulated - accumulatedXAtFirstOutputInstruction - (outputSize);

            LineEnd.XYPosition = new Point(xSizeAccumulated, YPosition);
            LineEnd.XYSize = new Size(this.XSize, sizeOfYToBackgroundDraw);
            LineEnd.Location = new Point(xSizeAccumulated, YPosition);
            LineEnd.Size = new Size(this.XSize, sizeOfYToBackgroundDraw);

            BackgroundLine.XYPosition = new Point(XPosition, YPosition);
            BackgroundLine.XYSize = new Size(xSizeAccumulated, sizeOfYToBackgroundDraw);
            BackgroundLine.Location = new Point(XPosition, YPosition);
            BackgroundLine.Size = new Size(xSizeAccumulated, sizeOfYToBackgroundDraw);

            ResizeVisualInstructions();
        }

        private void ResizeVisualInstructions()
        {
            tabStop++;
            LineBegin.TabIndex = tabStop;
            LineBegin.TabStop = true;

            int outputToInsertIndex = 0;
            if (visualOutputInstructions.Count > 0)
            {
                outputToInsertIndex = visualInstructions.Count;
                visualInstructions.AddRange(visualOutputInstructions);
            }

            int index = 0;
            foreach (VisualInstructionUserControl visualInstruction in visualInstructions)
            {
                tabStop++;
                visualInstruction.TabIndex = tabStop;
                visualInstruction.TabStop = true;

                visualInstruction.Size = new Size(Convert.ToInt32(Convert.ToDouble(visualInstruction.XYSize.Width) * percentageVisualInstructionReduction), visualInstruction.XYSize.Height);

                if (index >= outputToInsertIndex && (visualOutputInstructions.Count > 0))
                {
                    visualInstruction.Location = new Point(visualInstruction.XYPosition.X + (visualInstruction.XYSize.Width - (Convert.ToInt32(Convert.ToDouble(visualInstruction.XYSize.Width) * percentageVisualInstructionReduction))) / 2 + XPositionToFirstOutputVisualInstruction, visualInstruction.XYPosition.Y);
                }
                else
                {
                    visualInstruction.Location = new Point(visualInstruction.XYPosition.X + (visualInstruction.XYSize.Width - (Convert.ToInt32(Convert.ToDouble(visualInstruction.XYSize.Width) * percentageVisualInstructionReduction))) / 2, visualInstruction.XYPosition.Y);
                }

                visualInstruction.Visible = true;
                visualInstruction.Invalidate();
                index++;
            }

            if (visualOutputInstructions.Count > 0)
            {
                visualInstructions.RemoveRange(outputToInsertIndex, visualOutputInstructions.Count);
            }

            LineEnd.TabIndex = 0;
            LineEnd.TabStop = false;

            BackgroundLine.TabIndex = 0;
            BackgroundLine.TabStop = false;
            BackgroundLine.Invalidate();

        }


        private void InitializeFixedVisualInstructionsToInitialVisualLine()
        {
            //LineBegin.OpCode = OperationCode.LineBegin;
            LineBegin.TabStop = true;
            LineBegin.VisualLine = this;
            LineBegin.Parent = this.ladderForm;
            LineBegin.CreateControl();
            LineBegin.BringToFront();

            LineEnd.OpCode = OperationCode.LineEnd;
            LineEnd.TabStop = false;
            LineEnd.VisualLine = this;
            LineEnd.Parent = this.ladderForm;
            LineEnd.CreateControl();
            LineEnd.BringToFront();

            InsertInstructionDirect(this.visualInstructions, this.LineBegin, line.Instructions);
            InsertInstructionDirect(this.visualOutputInstructions, this.LineBegin, line.Outputs);

            BackgroundLine.Parent = ladderForm;
            BackgroundLine.OpCode = OperationCode.BackgroundLine;
            BackgroundLine.TabStop = false;
            BackgroundLine.VisualLine = this;
            BackgroundLine.SendToBack();
            BackgroundLine.Parent = this.ladderForm;
            BackgroundLine.CreateControl();
            BackgroundLine.SendToBack();
            BackgroundLine.Enabled = false;

            SetFunctionsToFixedVisualInstructions();
        }

        public void DeleteLine()
        {
            visualOutputInstructions.Reverse();
            foreach (VisualInstructionUserControl visualInstruction in visualOutputInstructions)
            {
                visualInstruction.Dispose();
            }
            visualOutputInstructions.Clear();

            visualInstructions.Reverse();
            foreach (VisualInstructionUserControl visualInstruction in visualInstructions)
            {
                visualInstruction.Dispose();
            }
            visualInstructions.Clear();

            LineBegin.Dispose();
            LineEnd.Dispose();
            BackgroundLine.Dispose();
        }


        private void SetFunctionsToFixedVisualInstructions()
        {
            LineBegin.VisualInstructionSelectedEvent += new VisualInstructionSelectedEventHandler(ladderForm.VisualInstruction_Selected);
            LineBegin.MouseClick += new MouseEventHandler(LineBeginVisualInstruction_Click);
            LineBegin.DeleteLineEvent += new DeleteLineEventHandler(ladderForm.LineBeginVisualInstruction_DeleteLine);

            int insertOutputIndex = 0;
            if (visualOutputInstructions.Count > 0)
            {
                insertOutputIndex = visualInstructions.Count;
                visualInstructions.AddRange(visualOutputInstructions);
            }

            foreach (VisualInstructionUserControl visualInstruction in visualInstructions)
            {
                visualInstruction.VisualInstructionSelectedEvent += new VisualInstructionSelectedEventHandler(ladderForm.VisualInstruction_Selected);
                visualInstruction.MouseClick += new MouseEventHandler(VisualInstruction_Click);
                visualInstruction.KeyDown += new KeyEventHandler(ladderForm.VisualInstruction_KeyDown);
            }

            if (visualOutputInstructions.Count > 0)
            {
                visualInstructions.RemoveRange(insertOutputIndex, visualOutputInstructions.Count);
            }

        }

        public VisualInstructionUserControl InsertInstruction(LocalToInsertInstruction localToInsertInstruction, VisualInstructionUserControl visualInstruction, params OperationCode[] opCodes)
        {
            return InsertInstruction(true, localToInsertInstruction, visualInstruction, opCodes);
        }

        public VisualInstructionUserControl InsertInstruction(bool after, LocalToInsertInstruction localToInsertInstruction, VisualInstructionUserControl visualInstruction, params OperationCode[] opCodes)
        {
            InstructionList instructions = new InstructionList();
            instructions.InsertAllWithClearBefore(opCodes);

            switch (localToInsertInstruction)
            {
                case LocalToInsertInstruction.Undefined:
                    visualInstruction = InsertInstructionAtLocalToBeDefined(after, visualInstruction, instructions);
                    break;
                case LocalToInsertInstruction.ConditionsAtLeft:
                    visualInstruction = InsertConditionInstructionAtLeft(after, visualInstruction, instructions);
                    break;
                case LocalToInsertInstruction.OutputsAtRight:
                    visualInstruction = InsertOutputInstructionAtRight(after, visualInstruction, instructions);
                    break;
            }
            return visualInstruction;
        }


        public VisualInstructionUserControl InsertInstructionAtLocalToBeDefined(bool after, VisualInstructionUserControl visualInstruction, InstructionList instructions)
        {
            instructions.ValidateOperands();

            if (!instructions.Contains(OperationCode.OutputCoil) &&
                !instructions.Contains(OperationCode.Timer) &&
                !instructions.Contains(OperationCode.Counter))
            {
                return InsertConditionInstructionAtLeft(after, visualInstruction, instructions);
            }
            else
            {
                return InsertOutputInstructionAtRight(after, visualInstruction, instructions);
            }
        }

        private VisualInstructionUserControl InsertConditionInstructionAtLeft(bool after, VisualInstructionUserControl visualInstruction, InstructionList instructions)
        {
            int index = VerifyPositionToInsert(after, visualInstruction, this.visualInstructions);
            foreach (Instruction instruction in instructions)
            {
                line.Instructions.Insert(index, instruction);
                InsertVisualInstructionAt(index, this.visualInstructions, instruction);
                index++;
            }
            return this.visualInstructions[index - 1];
        }

        private VisualInstructionUserControl InsertOutputInstructionAtRight(bool after, VisualInstructionUserControl visualInstruction, InstructionList instructions)
        {
            int index = 0;
            int autToRetornInsertedInstruction = 0;

            switch (visualOutputInstructions.Count)
            {
                case 0:
                    index = 0;

                    if (instructions.Count > 1)
                    {
                        instructions.InsertParallelBranch(InstructionList.ParallelBranchInsertionType.ParallelBranchCompleted);
                        autToRetornInsertedInstruction = -1;
                    }

                    break;
                case 1:
                    index = VerifyPositionToInsert(after, visualInstruction, this.visualOutputInstructions);

                    if (index == 0)
                    {
                        instructions.InsertParallelBranch(InstructionList.ParallelBranchInsertionType.ParallelBranchInitialized);

                        line.Outputs.Insert(0, InstructionFactory.createInstruction(OperationCode.ParallelBranchNext));
                        InsertVisualInstructionAt(0, this.visualOutputInstructions, InstructionFactory.createInstruction(OperationCode.ParallelBranchNext));
                        line.Outputs.Insert(this.visualOutputInstructions.Count, InstructionFactory.createInstruction(OperationCode.ParallelBranchEnd));
                        InsertVisualInstructionAt(this.visualOutputInstructions.Count, this.visualOutputInstructions, InstructionFactory.createInstruction(OperationCode.ParallelBranchEnd));
                    }
                    else
                    {
                        instructions.InsertParallelBranch(InstructionList.ParallelBranchInsertionType.ParallelBranchFinalized);
                        autToRetornInsertedInstruction = -1;

                        line.Outputs.Insert(0, InstructionFactory.createInstruction(OperationCode.ParallelBranchBegin));
                        InsertVisualInstructionAt(0, this.visualOutputInstructions, InstructionFactory.createInstruction(OperationCode.ParallelBranchBegin));
                        index++;
                    }

                    break;
                default:
                    index = VerifyPositionToInsert(false, visualInstruction, this.visualOutputInstructions);

                    switch (this.visualOutputInstructions[index].OpCode)
                    {
                        case OperationCode.ParallelBranchBegin:
                            instructions.InsertParallelBranch(InstructionList.ParallelBranchInsertionType.ParallelBranchInitialized);

                            line.Outputs[0].OpCode = OperationCode.ParallelBranchNext;
                            this.visualOutputInstructions[0].OpCode = OperationCode.ParallelBranchNext;
                            break;
                        case OperationCode.ParallelBranchNext:
                            instructions.InsertParallelBranchNext();
                            break;
                        case OperationCode.ParallelBranchEnd:
                            instructions.InsertParallelBranchNext();
                            break;
                        default:
                            instructions.InsertParallelBranchNext();
                            index++;
                            break;
                    }
                    break;
            }

            foreach (Instruction instruction in instructions)
            {
                line.Outputs.Insert(index, instruction);
                InsertVisualInstructionAt(index, this.visualOutputInstructions, instruction);
                index++;
            }

            return this.visualOutputInstructions[index - 1 + autToRetornInsertedInstruction];
        }

        private static int VerifyPositionToInsert(bool after, VisualInstructionUserControl visualInstruction, List<VisualInstructionUserControl> visualInstructions)
        {
            int index = visualInstructions.IndexOf(visualInstruction);

            if (index < 0)
                index = 0;
            else
                if (after)
                index++;
            return index;
        }

        private VisualInstructionUserControl InsertVisualInstructionAt(int index, List<VisualInstructionUserControl> visualInstructions, Instruction instruction)
        {
            visualInstructions.Insert(index, new VisualInstructionUserControl(instruction));
            visualInstructions[index].VisualLine = this;
            visualInstructions[index].TabStop = true;
            visualInstructions[index].VisualInstructionSelectedEvent += new VisualInstructionSelectedEventHandler(ladderForm.VisualInstruction_Selected);
            visualInstructions[index].MouseClick += new MouseEventHandler(VisualInstruction_Click);
            visualInstructions[index].KeyDown += new KeyEventHandler(ladderForm.VisualInstruction_KeyDown);
            visualInstructions[index].AskToChangeAddressEvent += new AskToChangeAddressEventHandler(ladderForm.VisualInstruction_AskToChangeAddress);
            visualInstructions[index].Parent = this.ladderForm;
            visualInstructions[index].Visible = false;
            visualInstructions[index].CreateControl();
            visualInstructions[index].BringToFront();

            if (instruction.OpCode == OperationCode.Timer ||
                instruction.OpCode == OperationCode.Counter)
            {
                visualInstructions[index].MouseHover += new EventHandler(ladderForm.OutputTypeBox_MouseHover);
            }

            return visualInstructions[index];
        }



        public VisualInstructionUserControl InsertInstructionDirect(List<VisualInstructionUserControl> visualInstructions, VisualInstructionUserControl visualInstruction, List<Instruction> instructions)
        {
            int index = visualInstructions.IndexOf(visualInstruction);

            if (index < 0)
                index = 0;
            else
                index++;

            if (instructions.Count > 0)
            {
                foreach (Instruction instruction in instructions)
                {
                    InsertVisualInstructionAt(index, visualInstructions, instruction);
                    index++;
                }

                return visualInstructions[index - 1];
            }
            return null;
        }


        void VisualInstruction_Click(object sender, MouseEventArgs e)
        {
            if (!e.Button.Equals(MouseButtons.Right))
            {
                return;
            }

            VisualInstructionUserControl visualInstruction = (VisualInstructionUserControl)sender;
            OperationCode opCode = visualInstruction.OpCode;
            if (opCode != OperationCode.LineBegin)
            {
                ladderForm.mnuInsertLadderLine.Enabled = false;

                ladderForm.mnuToggleBit.Enabled = false;
                if (opCode == OperationCode.ParallelBranchBegin ||
                    opCode == OperationCode.ParallelBranchEnd ||
                    opCode == OperationCode.ParallelBranchNext)
                {
                    ladderForm.mnuAddressing.Enabled = false;
                    ladderForm.mnuAddressing.Visible = false;

                    ladderForm.mnuExtendParallelBranchAbove.Enabled = true;
                    ladderForm.mnuExtendParallelBranchAbove.Visible = true;
                    ladderForm.mnuExtendParallelBranchBelow.Enabled = true;
                    ladderForm.mnuExtendParallelBranchBelow.Visible = true;

                }
                else
                {
                    ladderForm.mnuAddressing.Enabled = true;
                    ladderForm.mnuAddressing.Visible = true;

                    if (visualInstruction.IsAllOperandsOk())
                    {
                        ladderForm.mnuToggleBit.Enabled = true;
                    }
                    else
                    {
                        ladderForm.mnuToggleBit.Enabled = false;
                    }

                    ProjectForm projectForm = ladderForm.projectForm;
                    TreeNode addressingNode = projectForm.tvnProjectTree.Nodes["tvnProjectNode"].Nodes["tvnAddressingNode"];
                    foreach (TreeNode eachAddressTypeNode in addressingNode.Nodes)
                    {
                        ToolStripMenuItem menu = null;
                        switch (eachAddressTypeNode.Text)
                        {
                            case "Memories":
                                menu = ladderForm.mnuMemory;
                                break;
                            case "Timer":
                                menu = ladderForm.mnuTimer;
                                break;
                            case "Counter":
                                menu = ladderForm.mnuCounter;
                                break;
                            case "Input":
                                menu = ladderForm.mnuInput;
                                break;
                            case "Output":
                                menu = ladderForm.mnuOutput;
                                break;
                        }

                        Address address = null;
                        if (visualInstruction.IsAllOperandsOk())
                        {
                            Object obj = visualInstruction.GetOperand(0);
                            if (obj is Address)
                            {
                                address = (Address)obj;
                            }
                        }

                        menu.DropDownItems.Clear();
                        foreach (TreeNode eachAddressNode in eachAddressTypeNode.Nodes)
                        {
                            menu.DropDownItems.Add(eachAddressNode.Text);

                            if (address != null)
                            {
                                if (address.Name == eachAddressNode.Text)
                                {
                                    menu.DropDownItems[menu.DropDownItems.Count - 1].Select();
                                }
                            }

                            menu.DropDownItems[menu.DropDownItems.Count - 1].Name = eachAddressNode.Text;
                            menu.DropDownItems[menu.DropDownItems.Count - 1].Tag = eachAddressNode.Tag;
                            menu.DropDownItems[menu.DropDownItems.Count - 1].Click += new EventHandler(MenuContextAddress_Click);
                        }
                    }

                }
            }
            ladderForm.mnuContextAtInstruction.Show(visualInstruction.PointToScreen(e.Location));
        }

        void MenuContextAddress_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem clickedMenuItem = (ToolStripMenuItem)sender;
            ladderForm.projectForm.InsertAddressAtInstruction(ladderForm.VisualInstruction, (Address)clickedMenuItem.Tag);
        }

        void LineBeginVisualInstruction_Click(object sender, MouseEventArgs e)
        {
            if (!e.Button.Equals(MouseButtons.Right))
            {
                return;
            }

            VisualInstructionUserControl visualInstruction = (VisualInstructionUserControl)sender;
            if (visualInstruction.OpCode == OperationCode.LineBegin)
            {
                ladderForm.mnuAddressing.Enabled = false;
                ladderForm.mnuInsertLadderLine.Enabled = true;

                ladderForm.mnuExtendParallelBranchAbove.Enabled = false;
                ladderForm.mnuExtendParallelBranchAbove.Visible = false;
                ladderForm.mnuExtendParallelBranchBelow.Enabled = false;
                ladderForm.mnuExtendParallelBranchBelow.Visible = false;

                ladderForm.mnuContextAtInstruction.Show(visualInstruction.PointToScreen(e.Location));
            }
        }

        public bool RemoveVisualInstruction(VisualInstructionUserControl visualInstructionToBeRemoved)
        {
            int initialPositionIndex = 0;
            int finalPositionIndex = 0;
            int outputIndex = 0;
            List<VisualInstructionUserControl> visualInstructionsToRemove = new List<VisualInstructionUserControl>();
            List<VisualInstructionUserControl> visualInstructionsAux = null;
            List<Instruction> instructions = null;
            VisualInstructionUserControl visualInstructionToChangeOpCode = null;

            if (!visualInstructions.Contains(visualInstructionToBeRemoved))
            {
                if (!visualOutputInstructions.Contains(visualInstructionToBeRemoved))
                {
                    return false;
                }
                else
                {
                    visualInstructionsAux = visualOutputInstructions;
                    instructions = line.Outputs;

                    switch (visualInstructionToBeRemoved.OpCode)
                    {
                        case OperationCode.ParallelBranchBegin:
                        case OperationCode.ParallelBranchEnd:
                        case OperationCode.ParallelBranchNext:
                            break;
                        default:
                            if (this.visualOutputInstructions.Count > 1)
                            {
                                outputIndex = this.visualOutputInstructions.IndexOf(visualInstructionToBeRemoved);
                                visualInstructionToBeRemoved = this.visualOutputInstructions[outputIndex - 1];
                            }
                            break;
                    }
                }
            }
            else
            {
                visualInstructionsAux = visualInstructions;
                instructions = line.Instructions;
            }

            switch (visualInstructionToBeRemoved.OpCode)
            {
                case OperationCode.ParallelBranchBegin:
                case OperationCode.ParallelBranchNext:
                    initialPositionIndex = visualInstructionsAux.IndexOf(visualInstructionToBeRemoved);
                    finalPositionIndex = visualInstructionsAux.IndexOf(visualInstructionToBeRemoved.PointToNextParallelPoint);

                    finalPositionIndex--;

                    switch (visualInstructionToBeRemoved.OpCode)
                    {
                        case OperationCode.ParallelBranchBegin:
                            if (visualInstructionToBeRemoved.PointToNextParallelPoint.PointToNextParallelPoint.PointToParallelBegin != null)
                            {
                                visualInstructionsToRemove.Add(visualInstructionToBeRemoved.PointToNextParallelPoint.PointToNextParallelPoint);
                                finalPositionIndex++;
                            }
                            else
                                visualInstructionToChangeOpCode = visualInstructionToBeRemoved.PointToNextParallelPoint;
                            break;
                        case OperationCode.ParallelBranchNext:
                            if (visualInstructionToBeRemoved.PointToNextParallelPoint.PointToParallelBegin != null)
                            {
                                if (visualInstructionToBeRemoved.PointToNextParallelPoint.PointToParallelBegin.PointToNextParallelPoint.Equals(visualInstructionToBeRemoved))
                                {
                                    visualInstructionsToRemove.Add(visualInstructionToBeRemoved.PointToNextParallelPoint.PointToParallelBegin);
                                    finalPositionIndex++;
                                }
                            }
                            break;
                    }
                    break;
                case OperationCode.ParallelBranchEnd:
                    finalPositionIndex = visualInstructionsAux.IndexOf(visualInstructionToBeRemoved);
                    initialPositionIndex = visualInstructionsAux.IndexOf(visualInstructionToBeRemoved.PointToParallelBegin);
                    break;
                default:
                    initialPositionIndex = visualInstructionsAux.IndexOf(visualInstructionToBeRemoved);
                    finalPositionIndex = initialPositionIndex;
                    break;
            }

            for (int i = initialPositionIndex; i <= finalPositionIndex; i++)
            {
                visualInstructionsToRemove.Add(visualInstructionsAux[i]);
            }

            foreach (VisualInstructionUserControl eachVisualInstructionToBeRemoved in visualInstructionsToRemove)
            {
                RemoveInstruction(visualInstructionsAux, instructions, eachVisualInstructionToBeRemoved);
            }

            if (visualInstructionToChangeOpCode != null)
            {
                visualInstructionToChangeOpCode.OpCode = OperationCode.ParallelBranchBegin;
            }

            return true;
        }

        public bool RemoveInstruction(List<VisualInstructionUserControl> visualInstructions, List<Instruction> instructions, VisualInstructionUserControl visulInstructionToBeRemoved)
        {
            int index = visualInstructions.IndexOf(visulInstructionToBeRemoved);
            visualInstructions.RemoveAt(index);

            instructions.RemoveAt(index);

            visulInstructionToBeRemoved.VisualInstructionSelectedEvent -= new VisualInstructionSelectedEventHandler(ladderForm.VisualInstruction_Selected);
            visulInstructionToBeRemoved.MouseClick -= new MouseEventHandler(VisualInstruction_Click);
            visulInstructionToBeRemoved.KeyDown -= new KeyEventHandler(ladderForm.VisualInstruction_KeyDown);
            visulInstructionToBeRemoved.Instruction.Dispose();
            visulInstructionToBeRemoved.Dispose();

            return true;
        }

    }
}
