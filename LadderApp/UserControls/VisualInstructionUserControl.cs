using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using LadderApp.Model.Instructions;
using LadderApp.Model;

namespace LadderApp
{
    public delegate void ChangeLineEventHandler(VisualInstructionUserControl sender, System.Windows.Forms.Keys e);
    public delegate void DeleteLineEventHandler(VisualLine sender);
    public delegate void VisualInstructionSelectedEventHandler(VisualInstructionUserControl sender, VisualLine visualLine);
    public delegate void AskToChangeAddressEventHandler(VisualInstructionUserControl sender);

    public partial class VisualInstructionUserControl : UserControl, IInstruction, IDigitalAddressable

    {
        public event ChangeLineEventHandler ChangeLineEvent;
        public event DeleteLineEventHandler DeleteLineEvent;
        public event VisualInstructionSelectedEventHandler VisualInstructionSelectedEvent;
        public event AskToChangeAddressEventHandler AskToChangeAddressEvent;

        private Graphics graphics;

        private List<Panel> insertionIndicatorList = new List<Panel>();

        private Color penColor = Color.Blue;

        private Pen linePen = new Pen(Color.Blue, 1);
        private Pen textPen = new Pen(Color.Blue, 1);
        private Pen selectionPen = new Pen(Color.Black, 3);
        private Pen pen = new Pen(Color.Blue, 1);

        private SolidBrush energizedBrush = new SolidBrush(Color.FromArgb(0, 255, 0));
        private SolidBrush commentBrush = new SolidBrush(Color.Yellow);
        private SolidBrush selectionBrush = new SolidBrush(Color.Red);
        private SolidBrush addressTextBrush = new SolidBrush(Color.Black);
        private SolidBrush symbolTextBrush = new SolidBrush(Color.Blue);
        private SolidBrush commentTextBrush = new SolidBrush(Color.Black);

        private Font textFont = new Font(FontFamily.GenericSansSerif.Name, 10, FontStyle.Regular, GraphicsUnit.Pixel);

        private List<VisualInstructionUserControl> parallelBranchList;

        public VisualLine VisualLine { get; set; }

        public Instruction Instruction { get; private set; }
        public bool Selected { get; set; } = false;
        public Size XYSize { get; set; }
        public Point XYPosition { get; set; }
        private bool lastParallelBranchEnd = false;
        public bool LastParallelBranchEnd { get => lastParallelBranchEnd; set { if (OpCode == OperationCode.ParallelBranchNext) { lastParallelBranchEnd = value; } } }
        public Point XYConnection { get; private set; }

        public OperationCode OpCode
        {
            get
            {
                if (Instruction != null)
                {
                    return Instruction.OpCode;
                }
                else
                {
                    return OperationCode.None;
                }

            }

            set => Instruction.OpCode = value;
        }

        public Object GetOperand(int posicao)
        {
            return Instruction.GetOperand(posicao);
        }

        public void SetOperand(int operandIndex, Object value)
        {
            Instruction.SetOperand(operandIndex, value);
        }

        public bool IsAllOperandsOk()
        {
            return ((IInstruction)Instruction).IsAllOperandsOk();
        }

        [FlagsAttribute]
        private enum SelectionType : short
        {
            Background = 1,
            Border = 2
        };


        private Rectangle rectSelection;
        private Rectangle rectInstruction;
        private Rectangle rectEnergized;

        private int xTotalHorizontal = 0;
        private int yTotalVertical = 0;

        private int xHorizontalHalf = 0;
        private int yVerticalHalf = 0;

        private int xInstructionHorizotalBegin = 0;
        private int xInstructionHorizotalEnd = 0;

        private int yInstructionVerticalBegin = 0;
        private int yInstructionVerticalEnd = 0;


        private int xCoilInstructionHorizontalBegin = 0;
        private int xCoilInstructionHorizontalEnd = 0;

        private int yThirdVertical = 0;
        private int yFourthVertical = 0;
        private int yFifthVertical = 0;
        private int ySixthVertical = 0;

        private int xFifthHorizontal = 0;
        private int xSixthHorizontal = 0;
        private int xTenthHorizontal = 0;

        private Point xy1;
        private Point xy2;
        private Point xy3;

        private VisualInstructionUserControl pointToParallelBegin = null;
        public VisualInstructionUserControl PointToParallelBegin
        {
            get
            {
                switch (OpCode)
                {
                    case OperationCode.ParallelBranchBegin:
                    case OperationCode.ParallelBranchNext:
                    case OperationCode.ParallelBranchEnd:
                        return pointToParallelBegin;
                    default:
                        return null;
                }
            }
            set
            {
                pointToParallelBegin = value;
            }
        }

        private VisualInstructionUserControl pointToParallelEnd = null;
        public VisualInstructionUserControl PointToParallelEnd
        {
            get
            {
                switch (OpCode)
                {
                    case OperationCode.ParallelBranchBegin:
                    case OperationCode.ParallelBranchNext:
                    case OperationCode.ParallelBranchEnd:
                        return pointToParallelEnd;
                    default:
                        return null;
                }
            }
            set
            {
                pointToParallelEnd = value;
            }
        }

        private VisualInstructionUserControl pointToNextParallelPoint = null;

        public VisualInstructionUserControl PointToNextParallelPoint
        {
            get
            {
                switch (OpCode)
                {
                    case OperationCode.ParallelBranchBegin:
                    case OperationCode.ParallelBranchNext:
                    case OperationCode.ParallelBranchEnd:
                        return pointToNextParallelPoint;
                    default:
                        return null;
                }
            }
            set
            {
                pointToNextParallelPoint = value;
            }
        }

        public object[] Operands { get => Instruction.Operands; set => Instruction.Operands = value; }

        public VisualInstructionUserControl()
        {
            InitializeComponent();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            //Instruction = new Instruction();
        }

        public VisualInstructionUserControl(Instruction intruction) : this()
        {
            this.Instruction = intruction;
        }

        public VisualInstructionUserControl(OperationCode opCode) : this()
        {
            Instruction = InstructionFactory.createInstruction(opCode);
        }


        private void UpdateVariables()
        {
            xTotalHorizontal = Size.Width;
            yTotalVertical = Size.Height;

            xHorizontalHalf = xTotalHorizontal / 2;
            yVerticalHalf = yTotalVertical / 2;

            xFifthHorizontal = xTotalHorizontal / 5;
            xSixthHorizontal = xTotalHorizontal / 6;

            xInstructionHorizotalBegin = xFifthHorizontal * 2;
            xInstructionHorizotalEnd = 3 * xFifthHorizontal;

            yThirdVertical = yTotalVertical / 3;
            yFourthVertical = yTotalVertical / 4;
            yFifthVertical = yTotalVertical / 5;
            ySixthVertical = yTotalVertical / 6;

            yInstructionVerticalBegin = yThirdVertical;
            yInstructionVerticalEnd = 2 * yThirdVertical;

            xCoilInstructionHorizontalBegin = xSixthHorizontal * 2;
            xCoilInstructionHorizontalEnd = 4 * xSixthHorizontal;

            xTenthHorizontal = xTotalHorizontal / 10;

            rectSelection = new Rectangle(1, 1, xTotalHorizontal - 3, yTotalVertical - 3);

            switch (OpCode)
            {
                case OperationCode.None:
                    break;
                case OperationCode.LineBegin:
                    XYConnection = new Point(xTenthHorizontal * 7, (VisualLine.YSize / 2));
                    rectSelection = new Rectangle(1, 1, xTotalHorizontal - 3, yTotalVertical - 3);
                    break;
                case OperationCode.LineEnd:
                    XYConnection = new Point(0, (VisualLine.YSize / 2));
                    break;
                case OperationCode.NormallyOpenContact:
                    //selection
                    rectSelection = new Rectangle(1, 1, xTotalHorizontal - 3, yTotalVertical - 3);
                    break;
                case OperationCode.NormallyClosedContact:
                    break;
                case OperationCode.OutputCoil:
                    break;
                case OperationCode.Timer:
                    rectInstruction = new Rectangle(2, 2, xTotalHorizontal - 4, yTotalVertical - 4);
                    break;
                case OperationCode.Counter:
                    rectInstruction = new Rectangle(1, 1, xTotalHorizontal - 3, yTotalVertical - 3);
                    break;
                case OperationCode.ParallelBranchBegin:
                    rectSelection = new Rectangle(1, (VisualLine.YSize / 4) + 3, xTotalHorizontal - 3, (VisualLine.YSize / 2) - 3);

                    XYConnection = new Point(xHorizontalHalf, (VisualLine.YSize / 2));
                    break;
                case OperationCode.ParallelBranchEnd:
                    rectSelection = new Rectangle(1, (VisualLine.YSize / 4) + 3, xTotalHorizontal - 3, yTotalVertical - (VisualLine.YSize / 2) - 3);
                    XYConnection = new Point(xHorizontalHalf, (VisualLine.YSize / 2));
                    break;
                case OperationCode.ParallelBranchNext:
                    rectSelection = new Rectangle(1, (VisualLine.YSize / 4) + 3, xTotalHorizontal - 3, (VisualLine.YSize / 2) - 3);
                    if (LastParallelBranchEnd)
                    {
                        XYConnection = new Point(xHorizontalHalf, yVerticalHalf);
                    }
                    else
                    {
                        XYConnection = new Point(xHorizontalHalf, (VisualLine.YSize / 2));
                    }
                    break;
                case OperationCode.BackgroundLine:
                    break;
                default:
                    break;
            }
        }

        public void AssociateParallelBranchList(List<VisualInstructionUserControl> parallelBranchList)
        {
            if (OpCode == OperationCode.ParallelBranchEnd)
            {
                this.parallelBranchList = parallelBranchList;
            }
        }

        private void DrawNormallyOpenContact()
        {
            if (Instruction.IsAllOperandsOk())
            {
                if (this.OpCode == OperationCode.NormallyOpenContact &&
                ((Address)GetOperand(0)).Value == true)
                {
                    DrawEnergized();
                }
            }

            // -| Horizontal line, before the instruction
            xy1 = new Point(0, yVerticalHalf);
            xy2 = new Point(xInstructionHorizotalBegin, yVerticalHalf);
            graphics.DrawLine(linePen, xy1, xy2);

            // |- Horizontal line, after the instruction
            xy1 = new Point(xInstructionHorizotalEnd, yVerticalHalf);
            xy2 = new Point(this.Width, yVerticalHalf);
            graphics.DrawLine(linePen, xy1, xy2);

            // -| The right vertical bar of instruction 
            xy1 = new Point(xInstructionHorizotalBegin, yInstructionVerticalBegin);
            xy2 = new Point(xInstructionHorizotalBegin, yInstructionVerticalEnd);
            graphics.DrawLine(linePen, xy1, xy2);

            // small horizontal bar for the inverted-bracket-style bar at rigth (top horizontal bar)
            xy1 = new Point(xInstructionHorizotalBegin - xTenthHorizontal, yInstructionVerticalBegin);
            xy2 = new Point(xInstructionHorizotalBegin, yInstructionVerticalBegin);
            graphics.DrawLine(linePen, xy1, xy2);

            // small horizontal bar for the inverted-bracket-style bar at rigth (bottom horizontal bar)
            xy1 = new Point(xInstructionHorizotalBegin - xTenthHorizontal, yInstructionVerticalEnd);
            xy2 = new Point(xInstructionHorizotalBegin, yInstructionVerticalEnd);
            graphics.DrawLine(linePen, xy1, xy2);

            // |- The left vertical bar of instruction 
            xy1 = new Point(xInstructionHorizotalEnd, yInstructionVerticalBegin);
            xy2 = new Point(xInstructionHorizotalEnd, yInstructionVerticalEnd);
            graphics.DrawLine(linePen, xy1, xy2);

            // small horizontal bar for the inverted-bracket-style bar at left (top horizontal bar)
            xy1 = new Point(xInstructionHorizotalEnd + xTenthHorizontal, yInstructionVerticalBegin);
            xy2 = new Point(xInstructionHorizotalEnd, yInstructionVerticalBegin);
            graphics.DrawLine(linePen, xy1, xy2);

            // small horizontal bar for the inverted-bracket-style bar at rigth (bottom horizontal bar)
            xy1 = new Point(xInstructionHorizotalEnd + xTenthHorizontal, yInstructionVerticalEnd);
            xy2 = new Point(xInstructionHorizotalEnd, yInstructionVerticalEnd);
            graphics.DrawLine(linePen, xy1, xy2);

            DrawAddress();
            DrawComment();
        }

        private void DrawNormallyClosedContact()
        {
            if (IsAllOperandsOk())
            {
                if (this.OpCode == OperationCode.NormallyClosedContact &&
                    ((Address)GetOperand(0)).Value == false)
                {
                    DrawEnergized();
                }
            }
            else
            {
                DrawEnergized();
            }


            DrawNormallyOpenContact();


            // |/| closing bar to normally closed contact instruction
            xy1 = new Point(xInstructionHorizotalBegin - xTenthHorizontal, yInstructionVerticalEnd);
            xy2 = new Point(xInstructionHorizotalEnd + xTenthHorizontal, yInstructionVerticalBegin);
            graphics.DrawLine(linePen, xy1, xy2);
        }

        private void DrawOutputCoil()
        {
            if (IsAllOperandsOk())
            {
                if (this.OpCode == OperationCode.OutputCoil &&
                    ((Address)GetOperand(0)).Value == true)
                {
                    DrawEnergized();
                }
            }

            Point[] semiEllipse = new Point[3];

            // The horizontal line before instruction
            xy1 = new Point(0, yVerticalHalf);
            xy2 = new Point(xCoilInstructionHorizontalBegin, yVerticalHalf);
            graphics.DrawLine(linePen, xy1, xy2);

            // The horizontal line after instruction
            xy1 = new Point(xCoilInstructionHorizontalEnd, yVerticalHalf);
            xy2 = new Point(this.Width, yVerticalHalf);
            graphics.DrawLine(linePen, xy1, xy2);

            // ( - right parentheses of coil instruction
            xy1 = new Point(xCoilInstructionHorizontalBegin + xTenthHorizontal, yInstructionVerticalBegin);
            xy2 = new Point(xCoilInstructionHorizontalBegin, yVerticalHalf);
            xy3 = new Point(xCoilInstructionHorizontalBegin + xTenthHorizontal, yInstructionVerticalEnd);
            semiEllipse[0] = xy1;
            semiEllipse[1] = xy2;
            semiEllipse[2] = xy3;
            graphics.DrawCurve(linePen, semiEllipse, 0.7F);

            // ) - left parentheses of coil instruction
            xy1 = new Point(xCoilInstructionHorizontalEnd - xTenthHorizontal, yInstructionVerticalBegin);
            xy2 = new Point(xCoilInstructionHorizontalEnd, yVerticalHalf);
            xy3 = new Point(xCoilInstructionHorizontalEnd - xTenthHorizontal, yInstructionVerticalEnd);
            semiEllipse[0] = xy1;
            semiEllipse[1] = xy2;
            semiEllipse[2] = xy3;
            graphics.DrawCurve(linePen, semiEllipse, 0.7F);

            DrawAddress();
            DrawComment();
        }

        private void DrawOutputCoilWithText(string text)
        {
            DrawOutputCoil();

            xy1 = new Point(xHorizontalHalf - (int)(textFont.Size / 2.0F), yVerticalHalf - (int)(textFont.Size / 2.0F));
            graphics.DrawString(text, textFont, symbolTextBrush, xy1.X, xy1.Y);
        }


        private void DrawParallelBranchBegin()
        {
            // vertical line
            xy1 = new Point(xHorizontalHalf, (VisualLine.YSize / 2));
            xy2 = new Point(xHorizontalHalf, yTotalVertical);
            graphics.DrawLine(linePen, xy1, xy2);

            // horizontal line
            xy1 = new Point(0, (VisualLine.YSize / 2));
            xy2 = new Point(xTotalHorizontal, (VisualLine.YSize / 2));
            graphics.DrawLine(linePen, xy1, xy2);
        }

        private void DrawParallelBranchEnd()
        {
            // vertical line
            xy1 = new Point(xHorizontalHalf, (VisualLine.YSize / 2));
            xy2 = new Point(xHorizontalHalf, (yTotalVertical - (VisualLine.YSize / 2)));
            graphics.DrawLine(linePen, xy1, xy2);

            // horizontal line 1
            xy1 = new Point(0, (VisualLine.YSize / 2));
            xy2 = new Point(xTotalHorizontal, (VisualLine.YSize / 2));
            graphics.DrawLine(linePen, xy1, xy2);

            // horizontal line 2
            xy1 = new Point(0, (yTotalVertical - (VisualLine.YSize / 2)));
            xy2 = new Point(xHorizontalHalf, (yTotalVertical - (VisualLine.YSize / 2)));
            graphics.DrawLine(linePen, xy1, xy2);

            if (parallelBranchList != null)
            {
                foreach (VisualInstructionUserControl visualInstruction in parallelBranchList)
                {
                    xy3 = visualInstruction.XYConnection;
                    xy1 = new Point(0, ((visualInstruction.XYPosition.Y + xy3.Y) - this.XYPosition.Y));
                    xy2 = new Point(xHorizontalHalf, ((visualInstruction.XYPosition.Y + xy3.Y) - this.XYPosition.Y));
                    graphics.DrawLine(linePen, xy1, xy2);
                }
            }
        }

        private void DrawParallelBranchNext()
        {
            if (LastParallelBranchEnd)
            {
                // | - Vertical line of the 'L' from parallel
                xy1 = new Point(xHorizontalHalf, 0);
                xy2 = new Point(xHorizontalHalf, yVerticalHalf);
                graphics.DrawLine(linePen, xy1, xy2);

                // _ - Horizontal line of the 'L' from parallel
                xy1 = new Point(xHorizontalHalf, yVerticalHalf);
                xy2 = new Point(xTotalHorizontal, yVerticalHalf);
                graphics.DrawLine(linePen, xy1, xy2);

            }
            else
            {
                // | - Vertical line of the 'L' from parallel
                xy1 = new Point(xHorizontalHalf, 0);
                xy2 = new Point(xHorizontalHalf, yTotalVertical);
                graphics.DrawLine(linePen, xy1, xy2);

                // horizontal line
                xy1 = new Point(xHorizontalHalf, (VisualLine.YSize / 2));
                xy2 = new Point(xTotalHorizontal, (VisualLine.YSize / 2));
                graphics.DrawLine(linePen, xy1, xy2);
            }
        }

        private void DrawLineBegin()
        {
            Font textFont = new Font(FontFamily.GenericSansSerif.Name, 12, FontStyle.Regular, GraphicsUnit.Pixel);

            // vertical line
            xy1 = new Point(xTenthHorizontal * 7, 0);
            xy2 = new Point(xTenthHorizontal * 7, yTotalVertical);
            graphics.DrawLine(linePen, xy1, xy2);

            // line |-
            xy1 = new Point(xTenthHorizontal * 7, (VisualLine.YSize / 2));
            xy2 = new Point(xTotalHorizontal, (VisualLine.YSize / 2));
            graphics.DrawLine(linePen, xy1, xy2);

            // number of line position
            xy1 = new Point(xHorizontalHalf, yVerticalHalf);

            if (IsAllOperandsOk())
            {
                // Address
                RectangleF lineTextRectangle = new RectangleF(0F, (float)((VisualLine.YSize / 2) - this.textFont.Height), (float)(xTenthHorizontal * 7), (float)(this.textFont.Height));
                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;

                if (GetOperand(0) != null)
                {
                    //graphics.DrawString(GetOperand(0).ToString().PadLeft(4, '0'), this.textFont, symbolTextBrush, lineTextRectangle, format);
                    graphics.DrawString(VisualLine.LineNumber.ToString().PadLeft(4, '0'), this.textFont, symbolTextBrush, lineTextRectangle, format);
                }
            }
        }

        private void DrawLineEnd()
        {
            // vertical line
            xy1 = new Point(xTenthHorizontal, 0);
            xy2 = new Point(xTenthHorizontal, yTotalVertical);
            graphics.DrawLine(linePen, xy1, xy2);

            // horizontal line
            xy1 = new Point(0, (VisualLine.YSize / 2));
            xy2 = new Point(xTenthHorizontal, (VisualLine.YSize / 2));
            graphics.DrawLine(linePen, xy1, xy2);
        }


        private void DrawOutputBox()
        {
            DrawAddress();

            DrawPreset();

            DrawAccumulated();

            String title = "";
            switch (this.OpCode)
            {
                case OperationCode.Timer:
                    title = "T";
                    if (IsAllOperandsOk())
                    {
                        switch ((Int32)((Address)GetOperand(0)).Timer.Type)
                        {
                            case 0:
                                title = "TON";
                                break;
                            case 1:
                                title = "TOF";
                                break;
                            case 2:
                                title = "RTO";
                                break;
                            default:
                                title = "T?";
                                break;
                        }
                    }

                    DrawTimeBase();
                    break;
                case OperationCode.Counter:
                    title = "C";
                    if (IsAllOperandsOk())
                    {
                        //switch ((Int32)((Address)GetOperand(0)).Counter.Type)
                        switch (((CounterInstruction)Instruction).GetBoxType())
                        {
                            case 0:
                                title = "CTU";
                                break;
                            case 1:
                                title = "CTD";
                                break;
                            default:
                                title = "C?";
                                break;
                        }
                    }
                    break;
            }



            StringFormat format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            RectangleF titleRectangle = new RectangleF(1F, 3F, (float)xTotalHorizontal, (float)(textFont.Height));
            graphics.DrawString(title, textFont, symbolTextBrush, titleRectangle, format);
            graphics.DrawRectangle(linePen, rectInstruction);

            if (IsAllOperandsOk())
            {
                if (((Address)GetOperand(0)).Comment.Trim() != "")
                {
                    this.Tag = ((Address)GetOperand(0)).Comment;

                    graphics.DrawEllipse(new Pen(Color.Black), (xTotalHorizontal - 8 - 3), 3, 7, 7);
                    graphics.FillEllipse(new SolidBrush(Color.Yellow), (xTotalHorizontal - 8 - 3), 3, 7, 7);
                }
                else
                {
                    this.Tag = null;
                }
            }
            else
            {
                this.Tag = null;
            }

        }

        private void DrawBackground()
        {
            int xPosition = 0;
            int yPosition = 0;
            int originalYPosition = VisualLine.YPosition;


            VisualParallelBranch visualParallelBranch = null;
            List<VisualParallelBranch> visualParallelBranchList = new List<VisualParallelBranch>();

            VisualInstructionUserControl previousInstruction = null;
            VisualInstructionUserControl previousInstructionToDraw = VisualLine.LineBegin;

            graphics.Clear(Color.White);
            if (VisualLine.visualInstructions.Count > 0)
            {
                foreach (VisualInstructionUserControl visualInstruction in VisualLine.visualInstructions)
                {

                    if (visualInstruction.OpCode == OperationCode.ParallelBranchBegin ||
                        visualInstruction.OpCode == OperationCode.ParallelBranchNext ||
                        visualInstruction.OpCode == OperationCode.ParallelBranchEnd)
                    {
                        switch (visualInstruction.OpCode)
                        {
                            case OperationCode.ParallelBranchBegin:
                                visualParallelBranch = new VisualParallelBranch();
                                visualParallelBranch.parallelBranchBegin = visualInstruction;

                                visualParallelBranchList.Add(visualParallelBranch);
                                break;
                            case OperationCode.ParallelBranchEnd:

                                visualInstruction.Refresh();

                                visualParallelBranch.parallelBranchList.Insert(0, visualParallelBranch.parallelBranchBegin);

                                foreach (VisualInstructionUserControl _simb2PF in visualParallelBranch.parallelBranchList)
                                {
                                    xPosition = _simb2PF.XYPosition.X + _simb2PF.XYConnection.X;
                                    yPosition = _simb2PF.XYPosition.Y + _simb2PF.XYConnection.Y;
                                    yPosition -= originalYPosition;
                                    xy1 = new Point(xPosition, yPosition);

                                    xPosition = visualInstruction.XYPosition.X + visualInstruction.XYConnection.X;
                                    yPosition = _simb2PF.XYPosition.Y + _simb2PF.XYConnection.Y;
                                    yPosition -= originalYPosition;
                                    xy2 = new Point(xPosition, yPosition);

                                    graphics.DrawLine(pen, xy1, xy2);
                                }

                                xPosition = visualInstruction.XYPosition.X + visualInstruction.XYConnection.X;
                                yPosition = visualParallelBranch.parallelBranchList[visualParallelBranch.parallelBranchList.Count - 1].XYPosition.Y + visualParallelBranch.parallelBranchList[visualParallelBranch.parallelBranchList.Count - 1].XYConnection.Y;
                                yPosition -= originalYPosition;
                                xy1 = new Point(xPosition, yPosition);

                                xPosition = visualInstruction.XYPosition.X + visualInstruction.XYConnection.X;
                                yPosition = visualInstruction.XYPosition.Y + visualInstruction.XYConnection.Y;
                                yPosition -= originalYPosition;
                                xy2 = new Point(xPosition, yPosition);

                                graphics.DrawLine(pen, xy1, xy2);

                                visualParallelBranch = null;
                                visualParallelBranchList.RemoveAt(visualParallelBranchList.Count - 1);

                                if (visualParallelBranchList.Count > 0)
                                    visualParallelBranch = visualParallelBranchList[visualParallelBranchList.Count - 1];
                                break;
                            case OperationCode.ParallelBranchNext:
                                visualParallelBranch.parallelBranchList.Add(visualInstruction);
                                break;
                            default:
                                break;
                        }

                        if (visualInstruction.OpCode != OperationCode.ParallelBranchEnd)
                        {
                            xPosition = previousInstructionToDraw.XYPosition.X + previousInstructionToDraw.XYConnection.X;
                            yPosition = previousInstructionToDraw.XYPosition.Y + previousInstructionToDraw.XYConnection.Y;
                            yPosition -= originalYPosition;
                            xy1 = new Point(xPosition, yPosition);

                            xPosition = visualInstruction.XYPosition.X + visualInstruction.XYConnection.X;
                            yPosition = visualInstruction.XYPosition.Y + visualInstruction.XYConnection.Y;
                            yPosition -= originalYPosition;
                            xy2 = new Point(xPosition, yPosition);

                            graphics.DrawLine(pen, xy1, xy2);
                        }


                        if (visualParallelBranchList.Count > 0 && visualInstruction.OpCode == OperationCode.ParallelBranchEnd)
                        {
                            if (visualParallelBranch.parallelBranchList.Count > 0)
                            {
                                previousInstructionToDraw = visualParallelBranch.parallelBranchList[visualParallelBranch.parallelBranchList.Count - 1];
                            }
                            else
                            {
                                previousInstructionToDraw = visualParallelBranch.parallelBranchBegin;
                            }
                        }
                        else
                        {
                            previousInstructionToDraw = visualInstruction;
                        }

                    }
                    previousInstruction = visualInstruction;
                }
            }

            xPosition = previousInstructionToDraw.XYPosition.X + previousInstructionToDraw.XYConnection.X;
            yPosition = previousInstructionToDraw.XYPosition.Y + previousInstructionToDraw.XYConnection.Y;
            yPosition -= originalYPosition;


            xy1 = new Point(xPosition, yPosition);

            xPosition = VisualLine.LineEnd.XYPosition.X + VisualLine.LineEnd.XYConnection.X;
            yPosition = VisualLine.LineEnd.XYPosition.Y + VisualLine.LineEnd.XYConnection.Y;
            yPosition -= originalYPosition;
            xy2 = new Point(xPosition, yPosition);

            graphics.DrawLine(pen, xy1, xy2);
        }

        private void DrawComment()
        {

            if (!IsAllOperandsOk() || !(GetOperand(0) is Address))
            {
                return;
            }
            String comment = ((Address)GetOperand(0)).Comment.Trim();
            if (comment == "")
            {
                return;
            }


            int commentLinesNeeded = 1;
            String secondLineComment = "";

            Size neededSize = TextRenderer.MeasureText(comment, textFont);
            RectangleF commentRectangle = new RectangleF(new PointF((float)(xTenthHorizontal / 2), (float)(yInstructionVerticalEnd + 2)), new SizeF((float)(xTotalHorizontal - (xTenthHorizontal / 2) - 3), (float)neededSize.Height));
            if (neededSize.Width > ((xTotalHorizontal - (xTenthHorizontal / 2) - 3)) + 1)
            {
                commentLinesNeeded = 2;
                for (int i = 0; i < 20; i++)
                {
                    secondLineComment = comment.Substring(comment.Length - 1, 1) + secondLineComment;
                    comment = comment.Remove(comment.Length - 1, 1);
                    neededSize = TextRenderer.MeasureText(comment, textFont);
                    if (neededSize.Width < (xTotalHorizontal - (xTenthHorizontal / 2) - 3))
                    {
                        neededSize = TextRenderer.MeasureText(secondLineComment, textFont);
                        if (neededSize.Width > (xTotalHorizontal - (xTenthHorizontal / 2) - 3))
                        {
                            secondLineComment = secondLineComment.Substring(0, comment.Length - 3) + " ...";
                        }
                        break;
                    }
                }
                commentRectangle.Height = (float)(textFont.Size * textFont.FontFamily.GetCellAscent(FontStyle.Regular) / textFont.FontFamily.GetEmHeight(FontStyle.Regular));
            }

            StringFormat commentAlignmentFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            for (int line = 1; line <= commentLinesNeeded; line++)
            {
                graphics.FillRectangle(commentBrush, commentRectangle);
                graphics.DrawString(comment, textFont, commentTextBrush, commentRectangle, commentAlignmentFormat);

                commentRectangle.Y += commentRectangle.Height;
                comment = secondLineComment;
            }
        }


        private void DrawAddress()
        {
            String addressName = "";
            RectangleF addressTextRectangle;

            switch (this.OpCode)
            {
                case OperationCode.Timer:
                case OperationCode.Counter:
                    addressTextRectangle = new RectangleF((float)(xHorizontalHalf / 2), (float)this.yFifthVertical, (float)xHorizontalHalf, (float)textFont.GetHeight());
                    break;
                default:
                    addressTextRectangle = new RectangleF(new PointF((float)(xTenthHorizontal / 2), (float)(3)), new SizeF((float)(xTotalHorizontal - (xTenthHorizontal / 2) - 3), (float)textFont.GetHeight()));
                    break;
            }

            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;


            if (GetOperand(0) != null)
            {
                addressName = ((Address)GetOperand(0)).GetName();
            }
            else
            {
                addressName = "?";
            }


            graphics.DrawString(addressName, textFont, symbolTextBrush, addressTextRectangle, format);

        }

        private void DrawPreset()
        {
            String presetText = "";
            RectangleF presetTextRectangle;

            Int32 preset = -1;
            switch (this.OpCode)
            {
                case OperationCode.Counter:
                    if (IsAllOperandsOk())
                    {
                        //preset = (Int32)((Address)GetOperand(0)).Counter.Preset;
                        preset = ((CounterInstruction)Instruction).GetPreset();
                    }
                    presetTextRectangle = new RectangleF((float)(0), (float)(2 * this.yFifthVertical + 2), xTotalHorizontal, (float)(textFont.Height));
                    break;
                case OperationCode.Timer:
                    if (IsAllOperandsOk())
                    {
                        preset = (Int32)((Address)GetOperand(0)).Timer.Preset;
                    }
                    presetTextRectangle = new RectangleF((float)(0), (float)(3 * this.yFifthVertical + 2), xTotalHorizontal, (float)(textFont.Height));
                    break;
                default:
                    presetTextRectangle = new RectangleF((float)(0), (float)(2 * this.yFifthVertical + 2), xTotalHorizontal, (float)(textFont.Height));
                    break;
            }

            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;


            if (preset != -1)
            {
                presetText = "PR: " + preset.ToString();
            }
            else
            {
                presetText = "PR: ?";
            }

            graphics.DrawString(presetText, textFont, symbolTextBrush, presetTextRectangle, format);

        }

        private void DrawTimeBase()
        {
            String timeBaseText = "";
            Int32 timeBase = -1;
            RectangleF timeBaseTextRectangle;

            switch (this.OpCode)
            {
                case OperationCode.Counter:
                    return;
                case OperationCode.Timer:
                    if (IsAllOperandsOk())
                        timeBase = (Int32)((Address)GetOperand(0)).Timer.TimeBase;
                    timeBaseTextRectangle = new RectangleF((float)(0), (float)(2 * this.yFifthVertical + 2), xTotalHorizontal, (float)(textFont.Height));
                    break;
                default:
                    timeBaseTextRectangle = new RectangleF((float)(0), (float)(2 * this.yFifthVertical + 2), xTotalHorizontal, (float)(textFont.Height));
                    break;
            }

            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;


            if (timeBase != -1)
            {
                timeBaseText = "BT: ";
                switch (timeBase)
                {
                    case 0:
                        timeBaseText += "10 ms";
                        break;
                    case 1:
                        timeBaseText += "100 ms";
                        break;
                    case 2:
                        timeBaseText += "1 s";
                        break;
                    case 3:
                        timeBaseText += "1 m";
                        break;
                    default:
                        break;
                }

            }
            else
            {
                timeBaseText = "BT: ?";
            }

            graphics.DrawString(timeBaseText, textFont, symbolTextBrush, timeBaseTextRectangle, format);
        }

        private void DrawAccumulated()
        {
            String accumulatedText = "AC: ?";
            Int32 accumulated = -1;
            RectangleF accumulatedTextRectangle;

            switch (this.OpCode)
            {
                case OperationCode.Counter:
                    if (IsAllOperandsOk())
                    {
                        //accumulated = (Int32)((Address)GetOperand(0)).Counter.Accumulated;
                        accumulated = ((CounterInstruction)Instruction).GetAccumulated();
                    }
                    accumulatedTextRectangle = new RectangleF((float)(0), (float)(3 * this.yFifthVertical + 2), xTotalHorizontal, (float)(textFont.Height));
                    break;
                case OperationCode.Timer:
                    if (IsAllOperandsOk())
                    {
                        accumulated = (Int32)((Address)GetOperand(0)).Timer.Accumulated;
                    }
                    accumulatedTextRectangle = new RectangleF((float)(0), (float)(4 * this.yFifthVertical + 2), xTotalHorizontal, (float)(textFont.Height));
                    break;
                default:
                    accumulatedTextRectangle = new RectangleF((float)(xHorizontalHalf / 2), (float)(3 * this.yFifthVertical + 2), xHorizontalHalf, (float)(textFont.Height));
                    break;
            }

            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;


            if (accumulated != -1)
            {
                accumulatedText = "AC: " + accumulated.ToString();
            }
            else
            {
                accumulatedText = "AC: ?";
            }

            graphics.DrawString(accumulatedText, textFont, symbolTextBrush, accumulatedTextRectangle, format);

        }

        private void DrawSelectedVisualInstruction(SelectionType selectionTime)
        {
            switch (selectionTime)
            {
                case SelectionType.Border:
                    graphics.DrawRectangle(selectionPen, rectSelection);
                    break;
                case SelectionType.Background:
                    graphics.FillRectangle(selectionBrush, rectSelection);
                    break;
                default:
                    graphics.FillRectangle(selectionBrush, rectSelection);
                    graphics.DrawRectangle(selectionPen, rectSelection);
                    break;
            }
        }


        private void DrawEnergized()
        {
            rectEnergized = new Rectangle(Convert.ToInt32(selectionPen.Width), (yTotalVertical - ySixthVertical) / 2, xInstructionHorizotalBegin - Convert.ToInt32(selectionPen.Width), ySixthVertical);
            graphics.FillRectangle(energizedBrush, rectEnergized);

            rectEnergized = new Rectangle(xInstructionHorizotalEnd, (yTotalVertical - ySixthVertical) / 2, xTotalHorizontal - xInstructionHorizotalEnd - Convert.ToInt32(selectionPen.Width), ySixthVertical);
            graphics.FillRectangle(energizedBrush, rectEnergized);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        public void DrawInstruction()
        {
            if (graphics == null)
            {
                return;
            }

            if (Selected)
            {
                DrawSelectedVisualInstruction(SelectionType.Background);
            }
            else
            {
                graphics.Clear(Color.White);
            }

            switch (OpCode)
            {
                case OperationCode.None:
                    graphics.Clear(Color.White);
                    break;
                case OperationCode.LineBegin:
                    DrawLineBegin();
                    break;
                case OperationCode.LineEnd:
                    DrawLineEnd();
                    break;
                case OperationCode.NormallyOpenContact:
                    DrawNormallyOpenContact();
                    break;
                case OperationCode.NormallyClosedContact:
                    DrawNormallyClosedContact();
                    break;
                case OperationCode.OutputCoil:
                    DrawOutputCoil();
                    break;
                case OperationCode.Timer:
                    DrawOutputBox();
                    break;
                case OperationCode.Counter:
                    DrawOutputBox();
                    break;
                case OperationCode.ParallelBranchBegin:
                    DrawParallelBranchBegin();
                    break;
                case OperationCode.ParallelBranchEnd:
                    DrawParallelBranchEnd();
                    break;
                case OperationCode.ParallelBranchNext:
                    DrawParallelBranchNext();
                    break;
                case OperationCode.BackgroundLine:
                    DrawBackground();
                    break;
                case OperationCode.Reset:
                    DrawOutputCoilWithText("R");
                    break;
                default:
                    break;
            }

            if (this.Selected)
            {
                DrawSelectedVisualInstruction(SelectionType.Border);
            }

        }

        private void VisualInstruction_Load(object sender, EventArgs e)
        {
            graphics = Graphics.FromHwnd(Handle);
            UpdateVariables();
        }

        private void VisualInstruction_Paint(object sender, PaintEventArgs e)
        {
            if (this.graphics != null && VisualLine.YSize > 0)
            {
                this.graphics = e.Graphics;
                DrawInstruction();
            }
        }

        private void VisualInstruction_Enter(object sender, EventArgs e)
        {
            if (OpCode != OperationCode.BackgroundLine &&
                OpCode != OperationCode.LineEnd)
            {
                this.Selected = true;

                if (VisualInstructionSelectedEvent != null)
                {
                    VisualInstructionSelectedEvent(this, VisualLine);
                }

                LadderForm ladderForm = (LadderForm)this.Parent;
                ladderForm.ShowMessageInStatus(this.Location.ToString() + " - " + this.Size.ToString() + " - " + this.TabStop.ToString() + " - " + this.TabIndex.ToString() + " - " + this.OpCode.ToString());

                this.Refresh();
            }
        }

        private void VisualInstruction_Leave(object sender, EventArgs e)
        {
            if (OpCode != OperationCode.BackgroundLine &&
                OpCode != OperationCode.LineEnd)
            {
                this.Refresh();
            }
        }

        private void VisualInstruction_Resize(object sender, EventArgs e)
        {
            UpdateVariables();
        }

        private void VisualInstruction_SizeChanged(object sender, EventArgs e)
        {
            UpdateVariables();
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if (keyData == Keys.Down ||
                keyData == Keys.Up)
            {
                return true;
            }

            return base.IsInputKey(keyData);
        }

        private void VisualInstruction_KeyDown(object sender, KeyEventArgs e)
        {

            if (this.OpCode == OperationCode.LineBegin)
            {
                if (ChangeLineEvent != null &&
                    (e.KeyData == Keys.Down || e.KeyData == Keys.Up))
                {
                    ChangeLineEvent(this, e.KeyData);
                }

                if (DeleteLineEvent != null &&
                    (e.KeyData == Keys.Delete))
                {
                    DeleteLineEvent(VisualLine);
                }
            }
        }

        private void VisualInstruction_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            switch (OpCode)
            {
                //case OperationCode.NormallyOpenContact:
                //case OperationCode.NormallyClosedContact:
                //    break;
                //case OperationCode.OutputCoil:
                //    break;
                case OperationCode.Timer:
                case OperationCode.Counter:
                    AskToChangeAddressEvent(this);
                    break;
            }
        }

        public int GetNumberOfOperands()
        {
            return Instruction.GetNumberOfOperands();
        }

        public void SetAddress(Address address)
        {
            ((IDigitalAddressable)Instruction).SetAddress(address);
        }

        public Address GetAddress()
        {
            return ((IDigitalAddressable)Instruction).GetAddress();
        }

        public void SetUsed()
        {
            ((IDigitalAddressable)Instruction).SetUsed();
        }

        public bool GetValue()
        {
            throw new NotImplementedException();
        }

        public void SetValue(bool value)
        {
            throw new NotImplementedException();
        }
    }
}
