using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace LadderApp
{
    public delegate void ChangeLineEventHandler(VisualInstructionUserControl sender, System.Windows.Forms.Keys e);
    public delegate void DeleteLineEventHandler(VisualLine sender);
    public delegate void VisualInstructionSelectedEventHandler(VisualInstructionUserControl sender, VisualLine lCL);
    public delegate void AskToChangeAddressEventHandler(VisualInstructionUserControl sender, Rectangle rect, Type tipo, Int32 valorMax, Int32 valorMin, params Object[] faixa);

    public partial class VisualInstructionUserControl : UserControl

    {
        public event ChangeLineEventHandler ChangeLineEvent;
        public event DeleteLineEventHandler DeleteLineEvent;
        public event VisualInstructionSelectedEventHandler VisualInstructionSelectedEvent;
        public event AskToChangeAddressEventHandler AskToChangeAddressEvent;

        public Instruction Instruction { get; private set; }
        public bool Selected { get; set; } = false;
        public Size XYSize { get; set; }
        public Point XYPosition { get; set; }

        public OperationCode OpCode
        {
            get
            {
                if (Instruction != null)
                    return Instruction.OpCode;
                else
                    return OperationCode.None;

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

        Font textFont = new Font(FontFamily.GenericSansSerif.Name, 10, FontStyle.Regular, GraphicsUnit.Pixel);

        [FlagsAttribute]
        private enum TipoSelecaoFlag : short
        {
            Fundo = 1,
            Borda = 2
        };

        private Graphics graphics;

        private VisualLine visualLine;
        public VisualLine VisualLine { get => visualLine; set => visualLine = value; }


        List<VisualInstructionUserControl> visualInstructions;

        private bool ultimoVPI = false;
        public bool UltimoVPI
        {
            get { return ultimoVPI; }
            set
            {
                if (OpCode == OperationCode.ParallelBranchNext)
                    ultimoVPI = value;
            }
        }
        public Point XYConnection { get; private set; }

        private Rectangle rectSelecao;
        private Rectangle rectSimbolo;
        private Rectangle rectEnergizado;

        public int yTamSimboloPadrao = 0;

        // Posicionamento
        int xTotalHorizontal = 0;
        int yTotalVertical = 0;

        int xMeioHorizontal = 0;
        int yMeioVertical = 0;

        int xInicioHSimbolo = 0;
        int xFimHSimbolo = 0;

        int yInicioVSimbolo = 0;
        int yFimVSimbolo = 0;


        int xInicioHSimboloBobina = 0;
        int xFimHSimboloBobina = 0;

        int yTercoVertical = 0;
        int yQuartoVertical = 0;
        int yQuintoVertical = 0;
        int ySextoVertical = 0;

        int xQuintoHorizontal = 0;
        int xSextoHorizontal = 0;
        int xDecimoHorizontal = 0;

        Point xy1;
        Point xy2;
        Point xy3;

        private VisualInstructionUserControl aponta2PI = null;
        public VisualInstructionUserControl Aponta2PI
        {
            get
            {
                switch (OpCode)
                {
                    case OperationCode.ParallelBranchBegin:
                    case OperationCode.ParallelBranchNext:
                    case OperationCode.ParallelBranchEnd:
                        return aponta2PI;
                    default:
                        return null;
                }
            }
            set
            {
                aponta2PI = value;
            }
        }

        private VisualInstructionUserControl aponta2PF = null;
        public VisualInstructionUserControl Aponta2PF
        {
            get
            {
                switch (OpCode)
                {
                    case OperationCode.ParallelBranchBegin:
                    case OperationCode.ParallelBranchNext:
                    case OperationCode.ParallelBranchEnd:
                        return aponta2PF;
                    default:
                        return null;
                }
            }
            set
            {
                aponta2PF = value;
            }
        }

        /// <summary>
        /// aponta para o proximo PP ponto do paralelo
        /// se e PI o proximo e o VPI se e VPI o proximo e
        /// VPI ou PF
        /// </summary>
        private VisualInstructionUserControl aponta2proxPP = null;
        public VisualInstructionUserControl Aponta2proxPP
        {
            get
            {
                switch (OpCode)
                {
                    case OperationCode.ParallelBranchBegin:
                    case OperationCode.ParallelBranchNext:
                    case OperationCode.ParallelBranchEnd:
                        return aponta2proxPP;
                    default:
                        return null;
                }
            }
            set
            {
                aponta2proxPP = value;
            }
        }

        public VisualInstructionUserControl()
        {
            InitializeComponent();
            Instruction = new Instruction();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;
        }

        public VisualInstructionUserControl(Instruction intruction)
        {
            InitializeComponent();
            this.Instruction = intruction;
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;
        }

        public VisualInstructionUserControl(OperationCode _ci)
        {
            InitializeComponent();
            Instruction = new Instruction(_ci);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;
        }


        ///<sumary>
        /// <para>AtualizaVariaveisDesenho()</para>
        /// <para>Atualiza todas as variaveis do objeto relativas a:</para>
        /// <para> - desenho</para>
        /// <para> - selecao</para>
        /// <para> - ponto de conexao (para desenho de fundo)</para>
        ///</sumary>
        private void AtualizaVariaveisDesenho()
        {
            xTotalHorizontal = this.Size.Width;
            yTotalVertical = this.Size.Height;

            xMeioHorizontal = xTotalHorizontal / 2;
            yMeioVertical = yTotalVertical / 2;

            xQuintoHorizontal = xTotalHorizontal / 5;
            xSextoHorizontal = xTotalHorizontal / 6;

            xInicioHSimbolo = xQuintoHorizontal * 2;
            xFimHSimbolo = 3 * xQuintoHorizontal;

            yTercoVertical = yTotalVertical / 3;
            yQuartoVertical = yTotalVertical / 4;
            yQuintoVertical = yTotalVertical / 5;
            ySextoVertical = yTotalVertical / 6;

            yInicioVSimbolo = yTercoVertical;
            yFimVSimbolo = 2 * yTercoVertical;

            xInicioHSimboloBobina = xSextoHorizontal * 2;
            xFimHSimboloBobina = 4 * xSextoHorizontal;

            xDecimoHorizontal = xTotalHorizontal / 10;

            // Geral selecao
            rectSelecao = new Rectangle(1, 1, xTotalHorizontal - 3, yTotalVertical - 3);

            switch (OpCode)
            {
                case OperationCode.None:
                    break;
                case OperationCode.LineBegin:
                    XYConnection = new Point(xDecimoHorizontal * 7, (VisualLine.YSize / 2));
                    //Selecao
                    rectSelecao = new Rectangle(1, 1, xTotalHorizontal - 3, yTotalVertical - 3);
                    break;
                case OperationCode.LineEnd:
                    XYConnection = new Point(0, (VisualLine.YSize / 2));
                    break;
                case OperationCode.NormallyOpenContact:
                    //Selecao
                    rectSelecao = new Rectangle(1, 1, xTotalHorizontal - 3, yTotalVertical - 3);
                    break;
                case OperationCode.NormallyClosedContact:
                    break;
                case OperationCode.OutputCoil:
                    break;
                case OperationCode.Timer:
                    rectSimbolo = new Rectangle(2, 2, xTotalHorizontal - 4, yTotalVertical - 4);
                    break;
                case OperationCode.Counter:
                    rectSimbolo = new Rectangle(1, 1, xTotalHorizontal - 3, yTotalVertical - 3);
                    break;
                case OperationCode.ParallelBranchBegin:
                    // selecao em torno do ponto de conexao
                    rectSelecao = new Rectangle(1, (VisualLine.YSize / 4) + 3, xTotalHorizontal - 3, (VisualLine.YSize / 2) - 3);

                    XYConnection = new Point(xMeioHorizontal, (VisualLine.YSize / 2));
                    break;
                case OperationCode.ParallelBranchEnd:
                    rectSelecao = new Rectangle(1, (VisualLine.YSize / 4) + 3, xTotalHorizontal - 3, yTotalVertical - (VisualLine.YSize / 2) - 3);
                    XYConnection = new Point(xMeioHorizontal, (VisualLine.YSize / 2));
                    break;
                case OperationCode.ParallelBranchNext:
                    // selecao em torno do ponto de conexao
                    rectSelecao = new Rectangle(1, (VisualLine.YSize / 4) + 3, xTotalHorizontal - 3, (VisualLine.YSize / 2) - 3);
                    if (ultimoVPI)
                        XYConnection = new Point(xMeioHorizontal, yMeioVertical);
                    else
                        XYConnection = new Point(xMeioHorizontal, (VisualLine.YSize / 2));
                    break;
                case OperationCode.BackgroundLine:
                    break;
                default:
                    break;
            }
        }

        public void SalvaVPI2PF(List<VisualInstructionUserControl> _lstVPI)
        {
            if (OpCode == OperationCode.ParallelBranchEnd)
            {
                if (visualInstructions == null)
                    visualInstructions = new List<VisualInstructionUserControl>();
                else
                    visualInstructions.Clear();


                foreach (VisualInstructionUserControl _simbAux in _lstVPI)
                {
                    visualInstructions.Add(_simbAux);
                }
            }
        }

        private void DesenhaContatoNA()
        {
            if (Instruction.IsAllOperandsOk())
            {
                if (this.OpCode == OperationCode.NormallyOpenContact &&
                ((Address)GetOperand(0)).Value == true)
                    DrawEnergized();
            }

            // -| Linha horizontal anterior ao simbolo
            xy1 = new Point(0, yMeioVertical);
            xy2 = new Point(xInicioHSimbolo, yMeioVertical);
            graphics.DrawLine(linePen, xy1, xy2);

            // |- Linha horizontal posterior ao simbolo
            xy1 = new Point(xFimHSimbolo, yMeioVertical);
            xy2 = new Point(this.Width, yMeioVertical);
            graphics.DrawLine(linePen, xy1, xy2);

            ///////////////////////////////
            // -| Primeiro barra do simbolo
            xy1 = new Point(xInicioHSimbolo, yInicioVSimbolo);
            xy2 = new Point(xInicioHSimbolo, yFimVSimbolo);
            graphics.DrawLine(linePen, xy1, xy2);

            // para compor a primeira barra estilo colchete (barra horizontal superior)
            xy1 = new Point(xInicioHSimbolo - xDecimoHorizontal, yInicioVSimbolo);
            xy2 = new Point(xInicioHSimbolo, yInicioVSimbolo);
            graphics.DrawLine(linePen, xy1, xy2);

            // para compor a primeira barra estilo colchete (barra horizontal inferior)
            xy1 = new Point(xInicioHSimbolo - xDecimoHorizontal, yFimVSimbolo);
            xy2 = new Point(xInicioHSimbolo, yFimVSimbolo);
            graphics.DrawLine(linePen, xy1, xy2);

            //////////////////////////////
            // |- Segunda barra do simbolo
            xy1 = new Point(xFimHSimbolo, yInicioVSimbolo);
            xy2 = new Point(xFimHSimbolo, yFimVSimbolo);
            graphics.DrawLine(linePen, xy1, xy2);

            // para compor a segunda barra estilo colchete (barra horizontal superior)
            xy1 = new Point(xFimHSimbolo + xDecimoHorizontal, yInicioVSimbolo);
            xy2 = new Point(xFimHSimbolo, yInicioVSimbolo);
            graphics.DrawLine(linePen, xy1, xy2);

            // para compor a segunda barra estilo colchete (barra horizontal inferior)
            xy1 = new Point(xFimHSimbolo + xDecimoHorizontal, yFimVSimbolo);
            xy2 = new Point(xFimHSimbolo, yFimVSimbolo);
            graphics.DrawLine(linePen, xy1, xy2);

            DrawAddress();
            DrawComment();
        }

        private void DesenhaContatoNF()
        {
            if (IsAllOperandsOk())
            {
                if (this.OpCode == OperationCode.NormallyClosedContact &&
                    ((Address)GetOperand(0)).Value == false)
                    DrawEnergized();
            }
            else
                DrawEnergized();


            DesenhaContatoNA();


            // barra do NF
            xy1 = new Point(xInicioHSimbolo - xDecimoHorizontal, yFimVSimbolo);
            xy2 = new Point(xFimHSimbolo + xDecimoHorizontal, yInicioVSimbolo);
            graphics.DrawLine(linePen, xy1, xy2);
        }

        private void DrawOutputCoil()
        {
            if (IsAllOperandsOk())
            {

                if (this.OpCode == OperationCode.OutputCoil &&
                    ((Address)GetOperand(0)).Value == true)
                    DrawEnergized();
            }

            Point[] pontosCurva = new Point[3];

            // Linha horizontal anterior ao simbolo
            xy1 = new Point(0, yMeioVertical);
            xy2 = new Point(xInicioHSimboloBobina, yMeioVertical);
            graphics.DrawLine(linePen, xy1, xy2);

            // Linha horizontal posterior ao simbolo
            xy1 = new Point(xFimHSimboloBobina, yMeioVertical);
            xy2 = new Point(this.Width, yMeioVertical);
            graphics.DrawLine(linePen, xy1, xy2);

            // ( do simbolo
            xy1 = new Point(xInicioHSimboloBobina + xDecimoHorizontal, yInicioVSimbolo);
            xy2 = new Point(xInicioHSimboloBobina, yMeioVertical);
            xy3 = new Point(xInicioHSimboloBobina + xDecimoHorizontal, yFimVSimbolo);
            pontosCurva[0] = xy1;
            pontosCurva[1] = xy2;
            pontosCurva[2] = xy3;
            graphics.DrawCurve(linePen, pontosCurva, 0.7F);

            // ) do simbolo
            xy1 = new Point(xFimHSimboloBobina - xDecimoHorizontal, yInicioVSimbolo);
            xy2 = new Point(xFimHSimboloBobina, yMeioVertical);
            xy3 = new Point(xFimHSimboloBobina - xDecimoHorizontal, yFimVSimbolo);
            pontosCurva[0] = xy1;
            pontosCurva[1] = xy2;
            pontosCurva[2] = xy3;
            graphics.DrawCurve(linePen, pontosCurva, 0.7F);

            DrawAddress();
            DrawComment();
        }

        private void DrawOutputCoilWithText(string text)
        {
            DrawOutputCoil();

            xy1 = new Point(xMeioHorizontal - (int)(textFont.Size / 2.0F), yMeioVertical - (int)(textFont.Size / 2.0F));
            graphics.DrawString(text, textFont, symbolTextBrush, xy1.X, xy1.Y);
        }


        private void DrawParallelBranchBegin()
        {
            // Linha vertical
            xy1 = new Point(xMeioHorizontal, (VisualLine.YSize / 2));
            xy2 = new Point(xMeioHorizontal, yTotalVertical);
            graphics.DrawLine(linePen, xy1, xy2);

            // Linha horizontal
            xy1 = new Point(0, (VisualLine.YSize / 2));
            xy2 = new Point(xTotalHorizontal, (VisualLine.YSize / 2));
            graphics.DrawLine(linePen, xy1, xy2);
        }

        private void DrawParallelBranchEnd()
        {
            // Linha vertical
            xy1 = new Point(xMeioHorizontal, (VisualLine.YSize / 2));
            xy2 = new Point(xMeioHorizontal, (yTotalVertical - (VisualLine.YSize / 2)));
            graphics.DrawLine(linePen, xy1, xy2);

            // Linha horizontal 1
            xy1 = new Point(0, (VisualLine.YSize / 2));
            xy2 = new Point(xTotalHorizontal, (VisualLine.YSize / 2));
            graphics.DrawLine(linePen, xy1, xy2);

            // Linha horizontal 2
            xy1 = new Point(0, (yTotalVertical - (VisualLine.YSize / 2)));
            xy2 = new Point(xMeioHorizontal, (yTotalVertical - (VisualLine.YSize / 2)));
            graphics.DrawLine(linePen, xy1, xy2);

            if (visualInstructions != null)
            {
                foreach (VisualInstructionUserControl visualInstruction in visualInstructions)
                {

                    //?? talvez passar xy3 para _xyConexao
                    xy3 = visualInstruction.XYConnection;
                    xy1 = new Point(0, ((visualInstruction.XYPosition.Y + xy3.Y) - this.XYPosition.Y));
                    xy2 = new Point(xMeioHorizontal, ((visualInstruction.XYPosition.Y + xy3.Y) - this.XYPosition.Y));
                    graphics.DrawLine(linePen, xy1, xy2);
                }
            }
        }

        private void DrawParallelBranchNext()
        {
            if (ultimoVPI)
            {
                // Linha | do L
                xy1 = new Point(xMeioHorizontal, 0);
                xy2 = new Point(xMeioHorizontal, yMeioVertical);
                graphics.DrawLine(linePen, xy1, xy2);

                // Linha _ do L
                xy1 = new Point(xMeioHorizontal, yMeioVertical);
                xy2 = new Point(xTotalHorizontal, yMeioVertical);
                graphics.DrawLine(linePen, xy1, xy2);

            }
            else
            {
                // Linha | do L
                xy1 = new Point(xMeioHorizontal, 0);
                xy2 = new Point(xMeioHorizontal, yTotalVertical);
                graphics.DrawLine(linePen, xy1, xy2);

                // Linha horizontal
                xy1 = new Point(xMeioHorizontal, (VisualLine.YSize / 2));
                xy2 = new Point(xTotalHorizontal, (VisualLine.YSize / 2));
                graphics.DrawLine(linePen, xy1, xy2);
            }
        }

        private void DrawLineBegin()
        {
            Font fontTexto = new Font(FontFamily.GenericSansSerif.Name, 12, FontStyle.Regular, GraphicsUnit.Pixel);

            // Linha vertical
            xy1 = new Point(xDecimoHorizontal * 7, 0);
            xy2 = new Point(xDecimoHorizontal * 7, yTotalVertical);
            graphics.DrawLine(linePen, xy1, xy2);

            // Linha |-
            xy1 = new Point(xDecimoHorizontal * 7, (VisualLine.YSize / 2));
            xy2 = new Point(xTotalHorizontal, (VisualLine.YSize / 2));
            graphics.DrawLine(linePen, xy1, xy2);

            // Posicao numero da linha
            xy1 = new Point(xMeioHorizontal, yMeioVertical);

            if (IsAllOperandsOk())
            {
                // Endereco
                RectangleF _recTxtLinha = new RectangleF(0F, (float)((VisualLine.YSize / 2) - textFont.Height), (float)(xDecimoHorizontal * 7), (float)(textFont.Height));
                StringFormat _stringFormat = new StringFormat();
                _stringFormat.Alignment = StringAlignment.Center;
                _stringFormat.LineAlignment = StringAlignment.Center;

                if (GetOperand(0) != null)
                    graphics.DrawString(GetOperand(0).ToString().PadLeft(4, '0'), textFont, symbolTextBrush, _recTxtLinha, _stringFormat);
            }
        }

        private void DrawLineEnd()
        {
            // Linha vertical
            xy1 = new Point(xDecimoHorizontal, 0);
            xy2 = new Point(xDecimoHorizontal, yTotalVertical);
            graphics.DrawLine(linePen, xy1, xy2);

            // Linha |-
            xy1 = new Point(0, (VisualLine.YSize / 2));
            xy2 = new Point(xDecimoHorizontal, (VisualLine.YSize / 2));
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

                    DrawTimeBase();
                    break;
                case OperationCode.Counter:
                    title = "C";
                    if (IsAllOperandsOk())
                        switch ((Int32)((Address)GetOperand(0)).Counter.Type)
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
                    break;
            }



            StringFormat format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            RectangleF titleRectangle = new RectangleF(1F, 3F, (float)xTotalHorizontal, (float)(textFont.Height));
            graphics.DrawString(title, textFont, symbolTextBrush, titleRectangle, format);
            graphics.DrawRectangle(linePen, rectSimbolo);

            /// para indicar comentário no quadro de saida
            if (IsAllOperandsOk())
                if (((Address)GetOperand(0)).Comment.Trim() != "")
                {
                    /// será usado para apresenta um tooltip
                    this.Tag = ((Address)GetOperand(0)).Comment;
                    /// para indicar comentário - desenha uma elipse no canto sup. dir.
                    graphics.DrawEllipse(new Pen(Color.Black), (xTotalHorizontal - 8 - 3), 3, 7, 7);
                    graphics.FillEllipse(new SolidBrush(Color.Yellow), (xTotalHorizontal - 8 - 3), 3, 7, 7);
                }
                else
                {
                    this.Tag = null;
                }
            else
            {
                this.Tag = null;
            }

        }

        private void DrawBackground()
        {
            // Variaveis auxiliares para posicionamento
            //  dos controles
            int _posX = 0; // linhaAtual.posX;  // auxiliar para posX
            int _posY = 0; // linhaAtual.posY;  // auxiliar para posY
            int _posYOriginal = VisualLine.YPosition;


            VisualParallelBranch visualParallelBranch = null;
            List<VisualParallelBranch> visualParallelBranchList = new List<VisualParallelBranch>();

            VisualInstructionUserControl _simbAntAux = null;
            VisualInstructionUserControl _simbAnt2DesenhoAux = null;

            _simbAnt2DesenhoAux = VisualLine.LineBegin;

            graphics.Clear(Color.White);
            if (VisualLine.visualInstructions.Count > 0)
            {
                foreach (VisualInstructionUserControl simbAux in VisualLine.visualInstructions)
                {

                    if (simbAux.OpCode == OperationCode.ParallelBranchBegin ||
                        simbAux.OpCode == OperationCode.ParallelBranchNext ||
                        simbAux.OpCode == OperationCode.ParallelBranchEnd)
                    {
                        switch (simbAux.OpCode)
                        {
                            case OperationCode.ParallelBranchBegin:
                                visualParallelBranch = new VisualParallelBranch();
                                visualParallelBranch.par = simbAux;

                                visualParallelBranchList.Add(visualParallelBranch);
                                break;
                            case OperationCode.ParallelBranchEnd:

                                simbAux.Refresh();

                                visualParallelBranch.lstVPI.Insert(0, visualParallelBranch.par);

                                foreach (VisualInstructionUserControl _simb2PF in visualParallelBranch.lstVPI)
                                {
                                    _posX = _simb2PF.XYPosition.X + _simb2PF.XYConnection.X;
                                    _posY = _simb2PF.XYPosition.Y + _simb2PF.XYConnection.Y;
                                    _posY -= _posYOriginal;
                                    xy1 = new Point(_posX, _posY);

                                    _posX = simbAux.XYPosition.X + simbAux.XYConnection.X;
                                    _posY = _simb2PF.XYPosition.Y + _simb2PF.XYConnection.Y;
                                    _posY -= _posYOriginal;
                                    xy2 = new Point(_posX, _posY);

                                    graphics.DrawLine(pen, xy1, xy2);
                                }

                                _posX = simbAux.XYPosition.X + simbAux.XYConnection.X;
                                _posY = visualParallelBranch.lstVPI[visualParallelBranch.lstVPI.Count - 1].XYPosition.Y + visualParallelBranch.lstVPI[visualParallelBranch.lstVPI.Count - 1].XYConnection.Y;
                                _posY -= _posYOriginal;
                                xy1 = new Point(_posX, _posY);

                                _posX = simbAux.XYPosition.X + simbAux.XYConnection.X;
                                _posY = simbAux.XYPosition.Y + simbAux.XYConnection.Y;
                                _posY -= _posYOriginal;
                                xy2 = new Point(_posX, _posY);

                                graphics.DrawLine(pen, xy1, xy2);

                                visualParallelBranch = null;
                                visualParallelBranchList.RemoveAt(visualParallelBranchList.Count - 1);

                                if (visualParallelBranchList.Count > 0)
                                    visualParallelBranch = visualParallelBranchList[visualParallelBranchList.Count - 1];
                                break;
                            case OperationCode.ParallelBranchNext:
                                visualParallelBranch.lstVPI.Add(simbAux);
                                break;
                            default:
                                break;
                        }

                        if (simbAux.OpCode != OperationCode.ParallelBranchEnd)
                        {
                            _posX = _simbAnt2DesenhoAux.XYPosition.X + _simbAnt2DesenhoAux.XYConnection.X;
                            _posY = _simbAnt2DesenhoAux.XYPosition.Y + _simbAnt2DesenhoAux.XYConnection.Y;
                            _posY -= _posYOriginal;
                            xy1 = new Point(_posX, _posY);

                            _posX = simbAux.XYPosition.X + simbAux.XYConnection.X;
                            _posY = simbAux.XYPosition.Y + simbAux.XYConnection.Y;
                            _posY -= _posYOriginal;
                            xy2 = new Point(_posX, _posY);

                            graphics.DrawLine(pen, xy1, xy2);
                        }


                        if (visualParallelBranchList.Count > 0 && simbAux.OpCode == OperationCode.ParallelBranchEnd)
                            if (visualParallelBranch.lstVPI.Count > 0)
                                _simbAnt2DesenhoAux = visualParallelBranch.lstVPI[visualParallelBranch.lstVPI.Count - 1];
                            else
                                _simbAnt2DesenhoAux = visualParallelBranch.par;
                        else
                            _simbAnt2DesenhoAux = simbAux;

                    }
                    _simbAntAux = simbAux;
                }
            }

            _posX = _simbAnt2DesenhoAux.XYPosition.X + _simbAnt2DesenhoAux.XYConnection.X;
            _posY = _simbAnt2DesenhoAux.XYPosition.Y + _simbAnt2DesenhoAux.XYConnection.Y;
            _posY -= _posYOriginal;


            xy1 = new Point(_posX, _posY);

            _posX = VisualLine.LineEnd.XYPosition.X + VisualLine.LineEnd.XYConnection.X;
            _posY = VisualLine.LineEnd.XYPosition.Y + VisualLine.LineEnd.XYConnection.Y;
            _posY -= _posYOriginal;
            xy2 = new Point(_posX, _posY);

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
            RectangleF commentRectangle = new RectangleF(new PointF((float)(xDecimoHorizontal / 2), (float)(yFimVSimbolo + 2)), new SizeF((float)(xTotalHorizontal - (xDecimoHorizontal / 2) - 3), (float)neededSize.Height));
            if (neededSize.Width > ((xTotalHorizontal - (xDecimoHorizontal / 2) - 3)) + 1)
            {
                commentLinesNeeded = 2;
                for (int i = 0; i < 20; i++)
                {
                    secondLineComment = comment.Substring(comment.Length - 1, 1) + secondLineComment;
                    comment = comment.Remove(comment.Length - 1, 1);
                    neededSize = TextRenderer.MeasureText(comment, textFont);
                    if (neededSize.Width < (xTotalHorizontal - (xDecimoHorizontal / 2) - 3))
                    {
                        neededSize = TextRenderer.MeasureText(secondLineComment, textFont);
                        if (neededSize.Width > (xTotalHorizontal - (xDecimoHorizontal / 2) - 3))
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
            RectangleF _recTxtEnd;

            switch (this.OpCode)
            {
                case OperationCode.Timer:
                case OperationCode.Counter:
                    _recTxtEnd = new RectangleF((float)(xMeioHorizontal / 2), (float)this.yQuintoVertical, (float)xMeioHorizontal, (float)textFont.GetHeight());
                    break;
                default:
                    _recTxtEnd = new RectangleF(new PointF((float)(xDecimoHorizontal / 2), (float)(3)), new SizeF((float)(xTotalHorizontal - (xDecimoHorizontal / 2) - 3), (float)textFont.GetHeight()));
                    break;
            }

            // Endereco
            StringFormat _stringFormat = new StringFormat();
            _stringFormat.Alignment = StringAlignment.Center;
            _stringFormat.LineAlignment = StringAlignment.Center;


            if (GetOperand(0) != null)
            {
                addressName = ((Address)GetOperand(0)).Name;
            }
            else
                addressName = "?";


            graphics.DrawString(addressName, textFont, symbolTextBrush, _recTxtEnd, _stringFormat);

        }

        private void DrawPreset()
        {
            String _txtPreset = "";
            Int32 _intPreset = -1;
            RectangleF _recTxtPreset;

            switch (this.OpCode)
            {
                case OperationCode.Counter:
                    if (IsAllOperandsOk())
                        _intPreset = (Int32)((Address)GetOperand(0)).Counter.Preset;
                    _recTxtPreset = new RectangleF((float)(0), (float)(2 * this.yQuintoVertical + 2), xTotalHorizontal, (float)(textFont.Height));
                    break;
                case OperationCode.Timer:
                    if (IsAllOperandsOk())
                        _intPreset = (Int32)((Address)GetOperand(0)).Timer.Preset;
                    _recTxtPreset = new RectangleF((float)(0), (float)(3 * this.yQuintoVertical + 2), xTotalHorizontal, (float)(textFont.Height));
                    break;
                default:
                    _recTxtPreset = new RectangleF((float)(0), (float)(2 * this.yQuintoVertical + 2), xTotalHorizontal, (float)(textFont.Height));
                    break;
            }

            // Endereco
            StringFormat _stringFormat = new StringFormat();
            _stringFormat.Alignment = StringAlignment.Center;
            _stringFormat.LineAlignment = StringAlignment.Center;


            if (_intPreset != -1)
            {
                _txtPreset = "PR: " + _intPreset.ToString();
            }
            else
                _txtPreset = "PR: ?";

            graphics.DrawString(_txtPreset, textFont, symbolTextBrush, _recTxtPreset, _stringFormat);

        }

        private void DrawTimeBase()
        {
            String _txtBaseTempo = "";
            Int32 _intBaseTempo = -1;
            RectangleF _recTxtBaseTempo;

            switch (this.OpCode)
            {
                case OperationCode.Counter:
                    return;
                case OperationCode.Timer:
                    if (IsAllOperandsOk())
                        _intBaseTempo = (Int32)((Address)GetOperand(0)).Timer.TimeBase;
                    _recTxtBaseTempo = new RectangleF((float)(0), (float)(2 * this.yQuintoVertical + 2), xTotalHorizontal, (float)(textFont.Height));
                    break;
                default:
                    _recTxtBaseTempo = new RectangleF((float)(0), (float)(2 * this.yQuintoVertical + 2), xTotalHorizontal, (float)(textFont.Height));
                    break;
            }

            // Endereco
            StringFormat _stringFormat = new StringFormat();
            _stringFormat.Alignment = StringAlignment.Center;
            _stringFormat.LineAlignment = StringAlignment.Center;


            if (_intBaseTempo != -1)
            {
                _txtBaseTempo = "BT: ";
                switch (_intBaseTempo)
                {
                    case 0:
                        _txtBaseTempo += "10 ms";
                        break;
                    case 1:
                        _txtBaseTempo += "100 ms";
                        break;
                    case 2:
                        _txtBaseTempo += "1 s";
                        break;
                    case 3:
                        _txtBaseTempo += "1 m";
                        break;
                    default:
                        break;
                }

            }
            else
                _txtBaseTempo = "BT: ?";

            graphics.DrawString(_txtBaseTempo, textFont, symbolTextBrush, _recTxtBaseTempo, _stringFormat);
        }

        /// <summary>
        /// Desenha o valor de acumulado para os quadros de saida
        ///<remarks>incluido modificação para uso da classe SimulaContador</remarks>
        /// </summary>
        private void DrawAccumulated()
        {
            String _txtAcum = "AC: ?";
            Int32 _intAcum = -1;
            RectangleF _recTxtAcum;

            switch (this.OpCode)
            {
                case OperationCode.Counter:
                    if (IsAllOperandsOk())
                        _intAcum = (Int32)((Address)GetOperand(0)).Counter.Accumulated;
                    _recTxtAcum = new RectangleF((float)(0), (float)(3 * this.yQuintoVertical + 2), xTotalHorizontal, (float)(textFont.Height));
                    break;
                case OperationCode.Timer:
                    if (IsAllOperandsOk())
                        _intAcum = (Int32)((Address)GetOperand(0)).Timer.Accumulated;
                    _recTxtAcum = new RectangleF((float)(0), (float)(4 * this.yQuintoVertical + 2), xTotalHorizontal, (float)(textFont.Height));
                    break;
                default:
                    _recTxtAcum = new RectangleF((float)(xMeioHorizontal / 2), (float)(3 * this.yQuintoVertical + 2), xMeioHorizontal, (float)(textFont.Height));
                    break;
            }

            // Endereco
            StringFormat _stringFormat = new StringFormat();
            _stringFormat.Alignment = StringAlignment.Center;
            _stringFormat.LineAlignment = StringAlignment.Center;


            if (_intAcum != -1)
            {
                _txtAcum = "AC: " + _intAcum.ToString();
            }
            else
                _txtAcum = "AC: ?";

            graphics.DrawString(_txtAcum, textFont, symbolTextBrush, _recTxtAcum, _stringFormat);

        }

        private void DrawSelectedVisualInstruction(TipoSelecaoFlag tipoSelecao)
        {
            //borda no objeto inteiro
            switch (tipoSelecao)
            {
                case TipoSelecaoFlag.Borda:
                    graphics.DrawRectangle(selectionPen, rectSelecao);
                    break;
                case TipoSelecaoFlag.Fundo:
                    graphics.FillRectangle(selectionBrush, rectSelecao);
                    break;
                default:
                    graphics.FillRectangle(selectionBrush, rectSelecao);
                    graphics.DrawRectangle(selectionPen, rectSelecao);
                    break;
            }
        }


        private void DrawEnergized()
        {
            // Sobre a linha horizontal antes do simbolo
            rectEnergizado = new Rectangle(Convert.ToInt32(selectionPen.Width), (yTotalVertical - ySextoVertical) / 2, xInicioHSimbolo - Convert.ToInt32(selectionPen.Width), ySextoVertical);
            graphics.FillRectangle(energizedBrush, rectEnergizado);

            // Sobre a linha horizontal depois do simbolo
            rectEnergizado = new Rectangle(xFimHSimbolo, (yTotalVertical - ySextoVertical) / 2, xTotalHorizontal - xFimHSimbolo - Convert.ToInt32(selectionPen.Width), ySextoVertical);
            graphics.FillRectangle(energizedBrush, rectEnergizado);
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
                DrawSelectedVisualInstruction(TipoSelecaoFlag.Fundo);
            else
                graphics.Clear(Color.White);

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
                    DesenhaContatoNA();
                    break;
                case OperationCode.NormallyClosedContact:
                    DesenhaContatoNF();
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
                DrawSelectedVisualInstruction(TipoSelecaoFlag.Borda);

        }

        private void VisualInstruction_Load(object sender, EventArgs e)
        {
            graphics = Graphics.FromHwnd(Handle);
            AtualizaVariaveisDesenho();
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
                    VisualInstructionSelectedEvent(this, VisualLine);

                LadderForm ladderForm = (LadderForm)this.Parent;
                ladderForm.SetMessage(this.Location.ToString() + " - " + this.Size.ToString() + " - " + this.TabStop.ToString() + " - " + this.TabIndex.ToString() + " - " + this.OpCode.ToString());

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
            AtualizaVariaveisDesenho();
        }

        private void VisualInstruction_SizeChanged(object sender, EventArgs e)
        {
            AtualizaVariaveisDesenho();
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if (keyData == Keys.Down ||
                keyData == Keys.Up)
                return true;

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

        ///// <summary>
        ///// Retorna o simbolobasico do objeto
        ///// </summary>
        //public Instruction Instruction
        //{
        //    get { return instruction; }
        //}

        //private void txtGeral_Enter(object sender, EventArgs e)
        //{
        //    ((Label)sender).BackColor = Color.Black;
        //    ((Label)sender).ForeColor = Color.White;
        //}

        //private void txtGeral_Leave(object sender, EventArgs e)
        //{
        //    ((Label)sender).BackColor = Color.Transparent;
        //    ((Label)sender).ForeColor = Color.Black;
        //}

        //private void txtEndereco_DoubleClick(object sender, EventArgs e)
        //{
        //    /// caso o evento nao este alocado, nao executa
        //    if (AskToChangeAddressEvent == null)
        //        return;

        //    switch (OpCode)
        //    {
        //        case OperationCode.NormallyOpenContact:
        //        case OperationCode.NormallyClosedContact:
        //            AskToChangeAddressEvent(this, new Rectangle(0, 0, 0, 0), (new Address()).GetType(), 0, 0, null);
        //            break;
        //        case OperationCode.OutputCoil:
        //            AskToChangeAddressEvent(this, new Rectangle(0, 0, 0, 0), (new Address()).GetType(), 0, 0, null);
        //            break;
        //        case OperationCode.Timer:
        //        case OperationCode.Counter:
        //            AskToChangeAddressEvent(this, new Rectangle(0, 0, 0, 0), (new Address()).GetType(), 0, 0, null);
        //            break;
        //    }
        //}

        //private void txtGeral_Click(object sender, EventArgs e)
        //{
        //    ((Label)sender).Select();
        //}

        private void VisualInstruction_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            switch (OpCode)
            {
                case OperationCode.NormallyOpenContact:
                case OperationCode.NormallyClosedContact:
                    //                    SolicitaMudarEndereco(this, new Rectangle(0, 0, 0, 0), (new EnderecamentoLadder()).GetType(), 0, 0, null);
                    break;
                case OperationCode.OutputCoil:
                    break;
                case OperationCode.Timer:
                case OperationCode.Counter:
                    AskToChangeAddressEvent(this, new Rectangle(0, 0, 0, 0), (new Address()).GetType(), 0, 0, null);
                    break;
            }
        }
    }
}
