using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace LadderApp
{
    public delegate void MudaLinhaEventHandler(FreeUserControl sender, System.Windows.Forms.Keys e);
    public delegate void DeletaLinhaEventHandler(VisualLine sender);
    public delegate void ControleSelecionadoEventHandler(FreeUserControl sender, VisualLine lCL);
    public delegate void SolicitaMudarEnderecoEventHandler(FreeUserControl sender, Rectangle rect, Type tipo, Int32 valorMax, Int32 valorMin, params Object[] faixa);

    public partial class FreeUserControl : BasicUserControl

    {
        public event MudaLinhaEventHandler MudaLinha;
        public event DeletaLinhaEventHandler DeletaLinha;
        public event ControleSelecionadoEventHandler ControleSelecionado;
        public event SolicitaMudarEnderecoEventHandler SolicitaMudarEndereco;

        private Boolean usoTxtIndicado = false;

        // indicador de insercao
        List<System.Windows.Forms.Panel> listaIndicadores = new List<System.Windows.Forms.Panel>();
        System.Windows.Forms.Panel indicadorInsercao = null;

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

        private Graphics e;

        private VisualLine visualLine;
        public VisualLine VisualLine { get => visualLine; set => visualLine = value; }


        List<FreeUserControl> lstVPI;

        private bool ultimoVPI = false;
        public bool UltimoVPI
        {
            get { return ultimoVPI; }
            set
            {
                if (OpCode== OperationCode.PARALELO_PROXIMO)
                    ultimoVPI = value;
            }
        }

        private new Point xyConexao;
        public new Point XYConexao
        {
            get { return xyConexao; }
        }

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

        private FreeUserControl aponta2PI = null;
        public FreeUserControl Aponta2PI
        {
            get
            {
                switch (OpCode)
                {
                    case OperationCode.PARALELO_INICIAL:
                    case OperationCode.PARALELO_PROXIMO:
                    case OperationCode.PARALELO_FINAL:
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

        private FreeUserControl aponta2PF = null;
        public FreeUserControl Aponta2PF
        {
            get
            {
                switch (OpCode)
                {
                    case OperationCode.PARALELO_INICIAL:
                    case OperationCode.PARALELO_PROXIMO:
                    case OperationCode.PARALELO_FINAL:
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
        private FreeUserControl aponta2proxPP = null;
        public FreeUserControl Aponta2proxPP
        {
            get
            {
                switch (OpCode)
                {
                    case OperationCode.PARALELO_INICIAL:
                    case OperationCode.PARALELO_PROXIMO:
                    case OperationCode.PARALELO_FINAL:
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

        public FreeUserControl()
        {
            InitializeComponent();
            codigoInterpretavel = new Symbol();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;
        }

        public FreeUserControl(Symbol _sb)
        {
            InitializeComponent();
            codigoInterpretavel = _sb;
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;
        }

        public FreeUserControl(OperationCode _ci)
        {
            InitializeComponent();
            codigoInterpretavel = new Symbol(_ci);
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
                case OperationCode.INICIO_DA_LINHA:
                    xyConexao = new Point(xDecimoHorizontal * 7, (VisualLine.tamY / 2));
                    //Selecao
                    rectSelecao = new Rectangle(1, 1, xTotalHorizontal - 3, yTotalVertical - 3);
                    break;
                case OperationCode.FIM_DA_LINHA:
                    xyConexao = new Point(0, (VisualLine.tamY / 2));
                    break;
                case OperationCode.CONTATO_NA:
                    //Selecao
                    rectSelecao = new Rectangle(1, 1, xTotalHorizontal - 3, yTotalVertical - 3);
                    break;
                case OperationCode.CONTATO_NF:
                    break;
                case OperationCode.BOBINA_SAIDA:
                    break;
                case OperationCode.TEMPORIZADOR:
                    rectSimbolo = new Rectangle(2, 2, xTotalHorizontal - 4, yTotalVertical - 4);
                    break;
                case OperationCode.CONTADOR:
                    rectSimbolo = new Rectangle(1, 1, xTotalHorizontal - 3, yTotalVertical - 3);
                    break;
                case OperationCode.PARALELO_INICIAL:
                    // selecao em torno do ponto de conexao
                    rectSelecao = new Rectangle(1, (VisualLine.tamY / 4) + 3, xTotalHorizontal - 3, (VisualLine.tamY / 2) - 3);

                    xyConexao = new Point(xMeioHorizontal, (VisualLine.tamY / 2));
                    break;
                case OperationCode.PARALELO_FINAL:
                    rectSelecao = new Rectangle(1, (VisualLine.tamY / 4) + 3, xTotalHorizontal - 3, yTotalVertical - (VisualLine.tamY / 2) - 3);
                    xyConexao = new Point(xMeioHorizontal, (VisualLine.tamY / 2));
                    break;
                case OperationCode.PARALELO_PROXIMO:
                    // selecao em torno do ponto de conexao
                    rectSelecao = new Rectangle(1, (VisualLine.tamY / 4) + 3, xTotalHorizontal - 3, (VisualLine.tamY / 2) - 3);
                    if (ultimoVPI)
                        xyConexao = new Point(xMeioHorizontal, yMeioVertical);
                    else
                        xyConexao = new Point(xMeioHorizontal, (VisualLine.tamY / 2));
                    break;
                case OperationCode.LINHA_DE_FUNDO:
                    break;
                default:
                    break;
            }
        }

        public void SalvaVPI2PF(List<FreeUserControl> _lstVPI)
        {
            if (OpCode== OperationCode.PARALELO_FINAL)
            {
                if (lstVPI == null)
                    lstVPI = new List<FreeUserControl>();
                else
                    lstVPI.Clear();


                foreach (FreeUserControl _simbAux in _lstVPI)
                {
                    lstVPI.Add(_simbAux);
                }
            }
        }

        private void DesenhaContatoNA()
        {
            if (getOperandos(0) != null)
            {

                if (this.OpCode== OperationCode.CONTATO_NA &&
                    ((Address)getOperandos(0)).Valor == true)
                    Energizado();
            }

            // -| Linha horizontal anterior ao simbolo
            xy1 = new Point(0, yMeioVertical);
            xy2 = new Point(xInicioHSimbolo, yMeioVertical);
            e.DrawLine(linePen, xy1, xy2);

            // |- Linha horizontal posterior ao simbolo
            xy1 = new Point(xFimHSimbolo, yMeioVertical);
            xy2 = new Point(this.Width, yMeioVertical);
            e.DrawLine(linePen, xy1, xy2);

            ///////////////////////////////
            // -| Primeiro barra do simbolo
            xy1 = new Point(xInicioHSimbolo, yInicioVSimbolo);
            xy2 = new Point(xInicioHSimbolo, yFimVSimbolo);
            e.DrawLine(linePen, xy1, xy2);

            // para compor a primeira barra estilo colchete (barra horizontal superior)
            xy1 = new Point(xInicioHSimbolo - xDecimoHorizontal, yInicioVSimbolo);
            xy2 = new Point(xInicioHSimbolo, yInicioVSimbolo);
            e.DrawLine(linePen, xy1, xy2);

            // para compor a primeira barra estilo colchete (barra horizontal inferior)
            xy1 = new Point(xInicioHSimbolo - xDecimoHorizontal, yFimVSimbolo);
            xy2 = new Point(xInicioHSimbolo, yFimVSimbolo);
            e.DrawLine(linePen, xy1, xy2);

            //////////////////////////////
            // |- Segunda barra do simbolo
            xy1 = new Point(xFimHSimbolo, yInicioVSimbolo);
            xy2 = new Point(xFimHSimbolo, yFimVSimbolo);
            e.DrawLine(linePen, xy1, xy2);

            // para compor a segunda barra estilo colchete (barra horizontal superior)
            xy1 = new Point(xFimHSimbolo + xDecimoHorizontal, yInicioVSimbolo);
            xy2 = new Point(xFimHSimbolo, yInicioVSimbolo);
            e.DrawLine(linePen, xy1, xy2);

            // para compor a segunda barra estilo colchete (barra horizontal inferior)
            xy1 = new Point(xFimHSimbolo + xDecimoHorizontal, yFimVSimbolo);
            xy2 = new Point(xFimHSimbolo, yFimVSimbolo);
            e.DrawLine(linePen, xy1, xy2);

            DesenhaEndereco();

            DesenhaComentario();
        }

        private void DesenhaContatoNF()
        {
            if (getOperandos(0) != null)
            {
                if (this.OpCode== OperationCode.CONTATO_NF &&
                    ((Address)getOperandos(0)).Valor == false)
                    Energizado();
            }
            else
                Energizado();


            DesenhaContatoNA();


            // barra do NF
            xy1 = new Point(xInicioHSimbolo - xDecimoHorizontal, yFimVSimbolo);
            xy2 = new Point(xFimHSimbolo + xDecimoHorizontal, yInicioVSimbolo);
            e.DrawLine(linePen, xy1, xy2);
        }

        private void DesenhaBobinaSaida()
        {
            if (getOperandos(0) != null)
            {

                if (this.OpCode== OperationCode.BOBINA_SAIDA &&
                    ((Address)getOperandos(0)).Valor == true)
                    Energizado();
            }

            Point[] pontosCurva = new Point[3];

            // Linha horizontal anterior ao simbolo
            xy1 = new Point(0, yMeioVertical);
            xy2 = new Point(xInicioHSimboloBobina, yMeioVertical);
            e.DrawLine(linePen, xy1, xy2);

            // Linha horizontal posterior ao simbolo
            xy1 = new Point(xFimHSimboloBobina, yMeioVertical);
            xy2 = new Point(this.Width, yMeioVertical);
            e.DrawLine(linePen, xy1, xy2);

            // ( do simbolo
            xy1 = new Point(xInicioHSimboloBobina + xDecimoHorizontal, yInicioVSimbolo);
            xy2 = new Point(xInicioHSimboloBobina, yMeioVertical);
            xy3 = new Point(xInicioHSimboloBobina + xDecimoHorizontal, yFimVSimbolo);
            pontosCurva[0] = xy1;
            pontosCurva[1] = xy2;
            pontosCurva[2] = xy3;
            e.DrawCurve(linePen, pontosCurva, 0.7F);

            // ) do simbolo
            xy1 = new Point(xFimHSimboloBobina - xDecimoHorizontal, yInicioVSimbolo);
            xy2 = new Point(xFimHSimboloBobina, yMeioVertical);
            xy3 = new Point(xFimHSimboloBobina - xDecimoHorizontal, yFimVSimbolo);
            pontosCurva[0] = xy1;
            pontosCurva[1] = xy2;
            pontosCurva[2] = xy3;
            e.DrawCurve(linePen, pontosCurva, 0.7F);

            DesenhaEndereco();
            DesenhaComentario();
        }

        private void DesenhaBobinaComString(string Texto)
        {
            DesenhaBobinaSaida();

            xy1 = new Point(xMeioHorizontal - (int)(textFont.Size / 2.0F), yMeioVertical - (int)(textFont.Size / 2.0F));
            e.DrawString(Texto, textFont, symbolTextBrush, xy1.X, xy1.Y);
        }


        private void DesenhaParaleloInicial()
        {
            // Linha vertical
            xy1 = new Point(xMeioHorizontal, (VisualLine.tamY / 2));
            xy2 = new Point(xMeioHorizontal, yTotalVertical);
            e.DrawLine(linePen, xy1, xy2);

            // Linha horizontal
            xy1 = new Point(0, (VisualLine.tamY / 2));
            xy2 = new Point(xTotalHorizontal, (VisualLine.tamY / 2));
            e.DrawLine(linePen, xy1, xy2);
        }

        private void DesenhaParaleloFinal()
        {
            // Linha vertical
            xy1 = new Point(xMeioHorizontal, (VisualLine.tamY / 2));
            xy2 = new Point(xMeioHorizontal, (yTotalVertical - (VisualLine.tamY / 2)));
            e.DrawLine(linePen, xy1, xy2);

            // Linha horizontal 1
            xy1 = new Point(0, (VisualLine.tamY / 2));
            xy2 = new Point(xTotalHorizontal, (VisualLine.tamY / 2));
            e.DrawLine(linePen, xy1, xy2);

            // Linha horizontal 2
            xy1 = new Point(0, (yTotalVertical - (VisualLine.tamY / 2)));
            xy2 = new Point(xMeioHorizontal, (yTotalVertical - (VisualLine.tamY / 2)));
            e.DrawLine(linePen, xy1, xy2);

            if (lstVPI != null)
            {
                foreach (FreeUserControl _simbAux in lstVPI)
                {

                    //?? talvez passar xy3 para _xyConexao
                    xy3 = _simbAux.xyConexao;
                    xy1 = new Point(0, ((_simbAux.posicaoXY.Y + xy3.Y) - this.posicaoXY.Y));
                    xy2 = new Point(xMeioHorizontal, ((_simbAux.posicaoXY.Y + xy3.Y) - this.posicaoXY.Y));
                    e.DrawLine(linePen, xy1, xy2);
                }
            }
        }

        private void DesenhaParaleloProximo()
        {
            if (ultimoVPI)
            {
                // Linha | do L
                xy1 = new Point(xMeioHorizontal, 0);
                xy2 = new Point(xMeioHorizontal, yMeioVertical);
                e.DrawLine(linePen, xy1, xy2);

                // Linha _ do L
                xy1 = new Point(xMeioHorizontal, yMeioVertical);
                xy2 = new Point(xTotalHorizontal, yMeioVertical);
                e.DrawLine(linePen, xy1, xy2);

            }
            else
            {
                // Linha | do L
                xy1 = new Point(xMeioHorizontal, 0);
                xy2 = new Point(xMeioHorizontal, yTotalVertical);
                e.DrawLine(linePen, xy1, xy2);

                // Linha horizontal
                xy1 = new Point(xMeioHorizontal, (VisualLine.tamY / 2));
                xy2 = new Point(xTotalHorizontal, (VisualLine.tamY / 2));
                e.DrawLine(linePen, xy1, xy2);
            }
        }

        private void DesenhaInicioLinha()
        {
            Font fontTexto = new Font(FontFamily.GenericSansSerif.Name, 12, FontStyle.Regular, GraphicsUnit.Pixel);

            // Linha vertical
            xy1 = new Point(xDecimoHorizontal * 7, 0);
            xy2 = new Point(xDecimoHorizontal * 7, yTotalVertical);
            e.DrawLine(linePen, xy1, xy2);

            // Linha |-
            xy1 = new Point(xDecimoHorizontal * 7, (VisualLine.tamY / 2));
            xy2 = new Point(xTotalHorizontal, (VisualLine.tamY / 2));
            e.DrawLine(linePen, xy1, xy2);

            // Posicao numero da linha
            xy1 = new Point(xMeioHorizontal, yMeioVertical);

            if (getOperandos(0) != null)
            {
                // Endereco
                RectangleF _recTxtLinha = new RectangleF(0F, (float)((VisualLine.tamY / 2) - textFont.Height), (float)(xDecimoHorizontal * 7), (float)(textFont.Height));
                StringFormat _stringFormat = new StringFormat();
                _stringFormat.Alignment = StringAlignment.Center;
                _stringFormat.LineAlignment = StringAlignment.Center;

                if (getOperandos(0) != null)
                    e.DrawString(getOperandos(0).ToString().PadLeft(4, '0'), textFont, symbolTextBrush, _recTxtLinha, _stringFormat);
            }
        }

        private void DesenhaFimLinha()
        {
            // Linha vertical
            xy1 = new Point(xDecimoHorizontal, 0);
            xy2 = new Point(xDecimoHorizontal, yTotalVertical);
            e.DrawLine(linePen, xy1, xy2);

            // Linha |-
            xy1 = new Point(0, (VisualLine.tamY / 2));
            xy2 = new Point(xDecimoHorizontal, (VisualLine.tamY / 2));
            e.DrawLine(linePen, xy1, xy2);
        }


        private void DesenhaQuadroSaida()
        {
            RectangleF _recTxtTitulo;
            String _txtTitulo = "";

            _recTxtTitulo = new RectangleF(1F, 3F, (float)xTotalHorizontal, (float)(textFont.Height));

            StringFormat _stringFormat = new StringFormat();
            _stringFormat.Alignment = StringAlignment.Center;
            _stringFormat.LineAlignment = StringAlignment.Center;

            DesenhaEndereco();

            DesenhaPreset();

            DesenhaAcum();

            switch (this.OpCode)
            {
                case OperationCode.TEMPORIZADOR:
                    _txtTitulo = "T";
                    if (getOperandos(0) != null)
                        switch ((Int32)((Address)getOperandos(0)).Temporizador.Tipo)
                        {
                            case 0:
                                _txtTitulo = "TON";
                                break;
                            case 1:
                                _txtTitulo = "TOF";
                                break;
                            case 2:
                                _txtTitulo = "RTO";
                                break;
                            default:
                                _txtTitulo = "T?";
                                break;
                        }

                    DesenhaBaseTempo();
                    break;
                case OperationCode.CONTADOR:
                    _txtTitulo = "C";
                    if (getOperandos(0) != null)
                        switch ((Int32)((Address)getOperandos(0)).Contador.Tipo)
                        {
                            case 0:
                                _txtTitulo = "CTU";
                                break;
                            case 1:
                                _txtTitulo = "CTD";
                                break;
                            default:
                                _txtTitulo = "C?";
                                break;
                        }
                    break;
            }



            e.DrawString(_txtTitulo, textFont, symbolTextBrush, _recTxtTitulo, _stringFormat);
            e.DrawRectangle(linePen, rectSimbolo);

            /// para indicar coment�rio no quadro de saida
            if (getOperandos(0) != null)
                if (((Address)getOperandos(0)).Apelido.Trim() != "")
                {
                    /// ser� usado para apresenta um tooltip
                    this.Tag = ((Address)getOperandos(0)).Apelido;
                    /// para indicar coment�rio - desenha uma elipse no canto sup. dir.
                    e.DrawEllipse(new Pen(Color.Black), (xTotalHorizontal - 8 - 3), 3, 7, 7);
                    e.FillEllipse(new SolidBrush(Color.Yellow), (xTotalHorizontal - 8 - 3), 3, 7, 7);
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

        private void DesenhaFundo()
        {
            // Variaveis auxiliares para posicionamento
            //  dos controles
            int _posX = 0; // linhaAtual.posX;  // auxiliar para posX
            int _posY = 0; // linhaAtual.posY;  // auxiliar para posY
            int _posYOriginal = VisualLine.posY;


            VisualParallelBranch _par = null;
            List<VisualParallelBranch> _lst_par = new List<VisualParallelBranch>();

            FreeUserControl _simbAntAux = null;
            FreeUserControl _simbAnt2DesenhoAux = null;

            _simbAnt2DesenhoAux = VisualLine.simboloInicioLinha;

            e.Clear(Color.White);
            if (VisualLine.simbolos.Count > 0)
            {
                foreach (FreeUserControl simbAux in VisualLine.simbolos)
                {

                    if (simbAux.OpCode== OperationCode.PARALELO_INICIAL ||
                        simbAux.OpCode== OperationCode.PARALELO_PROXIMO ||
                        simbAux.OpCode== OperationCode.PARALELO_FINAL)
                    {
                        switch (simbAux.OpCode)
                        {
                            case OperationCode.PARALELO_INICIAL:
                                _par = new VisualParallelBranch();
                                _par.par = simbAux;

                                _lst_par.Add(_par);
                                break;
                            case OperationCode.PARALELO_FINAL:

                                simbAux.Refresh();

                                _par.lstVPI.Insert(0, _par.par);

                                foreach (FreeUserControl _simb2PF in _par.lstVPI)
                                {
                                    _posX = _simb2PF.posicaoXY.X + _simb2PF.XYConexao.X;
                                    _posY = _simb2PF.posicaoXY.Y + _simb2PF.XYConexao.Y;
                                    _posY -= _posYOriginal;
                                    xy1 = new Point(_posX, _posY);

                                    _posX = simbAux.posicaoXY.X + simbAux.XYConexao.X;
                                    _posY = _simb2PF.posicaoXY.Y + _simb2PF.XYConexao.Y;
                                    _posY -= _posYOriginal;
                                    xy2 = new Point(_posX, _posY);

                                    e.DrawLine(pen, xy1, xy2);
                                }

                                _posX = simbAux.posicaoXY.X + simbAux.XYConexao.X;
                                _posY = _par.lstVPI[_par.lstVPI.Count - 1].posicaoXY.Y + _par.lstVPI[_par.lstVPI.Count - 1].XYConexao.Y;
                                _posY -= _posYOriginal;
                                xy1 = new Point(_posX, _posY);

                                _posX = simbAux.posicaoXY.X + simbAux.XYConexao.X;
                                _posY = simbAux.posicaoXY.Y + simbAux.XYConexao.Y;
                                _posY -= _posYOriginal;
                                xy2 = new Point(_posX, _posY);

                                e.DrawLine(pen, xy1, xy2);

                                _par = null;
                                _lst_par.RemoveAt(_lst_par.Count - 1);

                                if (_lst_par.Count > 0)
                                    _par = _lst_par[_lst_par.Count - 1];
                                break;
                            case OperationCode.PARALELO_PROXIMO:
                                _par.lstVPI.Add(simbAux);
                                break;
                            default:
                                break;
                        }

                        if (simbAux.OpCode!= OperationCode.PARALELO_FINAL)
                        {
                            _posX = _simbAnt2DesenhoAux.posicaoXY.X + _simbAnt2DesenhoAux.XYConexao.X;
                            _posY = _simbAnt2DesenhoAux.posicaoXY.Y + _simbAnt2DesenhoAux.XYConexao.Y;
                            _posY -= _posYOriginal;
                            xy1 = new Point(_posX, _posY);

                            _posX = simbAux.posicaoXY.X + simbAux.XYConexao.X;
                            _posY = simbAux.posicaoXY.Y + simbAux.XYConexao.Y;
                            _posY -= _posYOriginal;
                            xy2 = new Point(_posX, _posY);

                            e.DrawLine(pen, xy1, xy2);
                        }


                        if (_lst_par.Count > 0 && simbAux.OpCode== OperationCode.PARALELO_FINAL)
                            if (_par.lstVPI.Count > 0)
                                _simbAnt2DesenhoAux = _par.lstVPI[_par.lstVPI.Count - 1];
                            else
                                _simbAnt2DesenhoAux = _par.par;
                        else
                            _simbAnt2DesenhoAux = simbAux;

                    }
                    _simbAntAux = simbAux;
                }
            }

            _posX = _simbAnt2DesenhoAux.posicaoXY.X + _simbAnt2DesenhoAux.XYConexao.X;
            _posY = _simbAnt2DesenhoAux.posicaoXY.Y + _simbAnt2DesenhoAux.XYConexao.Y;
            _posY -= _posYOriginal;


            xy1 = new Point(_posX, _posY);

            _posX = VisualLine.simboloFimLinha.posicaoXY.X + VisualLine.simboloFimLinha.XYConexao.X;
            _posY = VisualLine.simboloFimLinha.posicaoXY.Y + VisualLine.simboloFimLinha.XYConexao.Y;
            _posY -= _posYOriginal;
            xy2 = new Point(_posX, _posY);

            e.DrawLine(pen, xy1, xy2);
        }

        private void DesenhaComentario()
        {
            String _txtComent = "";
            RectangleF _recTxtComent;

            if (getOperandos(0) != null)
            {
                _txtComent = ((Address)getOperandos(0)).Apelido;
                if (_txtComent.Trim() != "")
                {
                    switch (this.OpCode)
                    {
                        default:
                            break;
                    }

                    Size _propostoSize;
                    _propostoSize = TextRenderer.MeasureText(_txtComent, textFont);
                    _recTxtComent = new RectangleF(new PointF((float)(xDecimoHorizontal / 2), (float)(yFimVSimbolo + 2)), new SizeF((float)(xTotalHorizontal - (xDecimoHorizontal / 2) - 3), (float)_propostoSize.Height));

                    StringFormat _stringFormat = new StringFormat();
                    _stringFormat.Alignment = StringAlignment.Center;
                    _stringFormat.LineAlignment = StringAlignment.Center;

                    int intNumLinhas = 1;
                    String strSegundaLinha = "";
                    if (_propostoSize.Width > ((xTotalHorizontal - (xDecimoHorizontal / 2) - 3)) + 1)
                    {
                        intNumLinhas = 2;
                        for (int i = 0; i < 20; i++)
                        {
                            strSegundaLinha = _txtComent.Substring(_txtComent.Length - 1, 1) + strSegundaLinha;
                            _txtComent = _txtComent.Remove(_txtComent.Length - 1, 1);
                            _propostoSize = TextRenderer.MeasureText(_txtComent, textFont);
                            if (_propostoSize.Width < (xTotalHorizontal - (xDecimoHorizontal / 2) - 3))
                            {
                                i = 20; // sai do loop
                                /// caso a segundo linha n�o permitiu coloca *** no final.
                                _propostoSize = TextRenderer.MeasureText(strSegundaLinha, textFont);
                                if (_propostoSize.Width > (xTotalHorizontal - (xDecimoHorizontal / 2) - 3))
                                {
                                    strSegundaLinha = strSegundaLinha.Substring(0, _txtComent.Length - 3) + " ...";
                                }
                            }
                        }
                        _recTxtComent.Height = (float)(textFont.Size * textFont.FontFamily.GetCellAscent(FontStyle.Regular) / textFont.FontFamily.GetEmHeight(FontStyle.Regular));
                    }

                    for (int i = 1; i <= intNumLinhas; i++)
                    {
                        e.FillRectangle(commentBrush, _recTxtComent);
                        e.DrawString(_txtComent, textFont, commentTextBrush, _recTxtComent, _stringFormat);

                        _recTxtComent.Y += _recTxtComent.Height;
                        _txtComent = strSegundaLinha;
                    }
                }
            }


        }


        private void DesenhaEndereco()
        {
            String _txtEndereco = "";
            RectangleF _recTxtEnd;

            switch (this.OpCode)
            {
                case OperationCode.TEMPORIZADOR:
                case OperationCode.CONTADOR:
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


            if (getOperandos(0) != null)
            {
                _txtEndereco = ((Address)getOperandos(0)).Nome;
            }
            else
                _txtEndereco = "?";


            e.DrawString(_txtEndereco, textFont, symbolTextBrush, _recTxtEnd, _stringFormat);

        }

        private void DesenhaPreset()
        {
            String _txtPreset = "";
            Int32 _intPreset = -1;
            RectangleF _recTxtPreset;

            switch (this.OpCode)
            {
                case OperationCode.CONTADOR:
                    if (getOperandos(0) != null)
                        _intPreset = (Int32)((Address)getOperandos(0)).Contador.Preset;
                    _recTxtPreset = new RectangleF((float)(0), (float)(2 * this.yQuintoVertical + 2), xTotalHorizontal, (float)(textFont.Height));
                    break;
                case OperationCode.TEMPORIZADOR:
                    if (getOperandos(0) != null)
                        _intPreset = (Int32)((Address)getOperandos(0)).Temporizador.Preset;
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

            e.DrawString(_txtPreset, textFont, symbolTextBrush, _recTxtPreset, _stringFormat);

        }

        private void DesenhaBaseTempo()
        {
            String _txtBaseTempo = "";
            Int32 _intBaseTempo = -1;
            RectangleF _recTxtBaseTempo;

            switch (this.OpCode)
            {
                case OperationCode.CONTADOR:
                    return;
                case OperationCode.TEMPORIZADOR:
                    if (getOperandos(0) != null)
                        _intBaseTempo = (Int32)((Address)getOperandos(0)).Temporizador.BaseTempo;
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

            e.DrawString(_txtBaseTempo, textFont, symbolTextBrush, _recTxtBaseTempo, _stringFormat);
        }

        /// <summary>
        /// Desenha o valor de acumulado para os quadros de saida
        ///<remarks>incluido modifica��o para uso da classe SimulaContador</remarks>
        /// </summary>
        private void DesenhaAcum()
        {
            String _txtAcum = "AC: ?";
            Int32 _intAcum = -1;
            RectangleF _recTxtAcum;

            switch (this.OpCode)
            {
                case OperationCode.CONTADOR:
                    if (getOperandos(0) != null)
                        _intAcum = (Int32)((Address)getOperandos(0)).Contador.Acumulado;
                    _recTxtAcum = new RectangleF((float)(0), (float)(3 * this.yQuintoVertical + 2), xTotalHorizontal, (float)(textFont.Height));
                    break;
                case OperationCode.TEMPORIZADOR:
                    if (getOperandos(0) != null)
                        _intAcum = (Int32)((Address)getOperandos(0)).Temporizador.Acumulado;
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

            e.DrawString(_txtAcum, textFont, symbolTextBrush, _recTxtAcum, _stringFormat);

        }

        private void SelecaoSimbolo(TipoSelecaoFlag tipoSelecao)
        {
            //borda no objeto inteiro
            switch (tipoSelecao)
            {
                case TipoSelecaoFlag.Borda:
                    e.DrawRectangle(selectionPen, rectSelecao);
                    break;
                case TipoSelecaoFlag.Fundo:
                    e.FillRectangle(selectionBrush, rectSelecao);
                    break;
                default:
                    e.FillRectangle(selectionBrush, rectSelecao);
                    e.DrawRectangle(selectionPen, rectSelecao);
                    break;
            }
        }


        private void Energizado()
        {
            // Sobre a linha horizontal antes do simbolo
            rectEnergizado = new Rectangle(Convert.ToInt32(selectionPen.Width), (yTotalVertical - ySextoVertical) / 2, xInicioHSimbolo - Convert.ToInt32(selectionPen.Width), ySextoVertical);
            e.FillRectangle(energizedBrush, rectEnergizado);

            // Sobre a linha horizontal depois do simbolo
            rectEnergizado = new Rectangle(xFimHSimbolo, (yTotalVertical - ySextoVertical) / 2, xTotalHorizontal - xFimHSimbolo - Convert.ToInt32(selectionPen.Width), ySextoVertical);
            e.FillRectangle(energizedBrush, rectEnergizado);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        public override void DesenhaSimbolo()
        {
            if (this.e != null)
            {
                if (this.selecionado)
                    SelecaoSimbolo(TipoSelecaoFlag.Fundo);
                else
                    e.Clear(Color.White);

                if (!usoTxtIndicado)
                    ApresentaCamposTxt();

                switch (OpCode)
                {
                    case OperationCode.None:
                        e.Clear(Color.White);
                        break;
                    case OperationCode.INICIO_DA_LINHA:
                        DesenhaInicioLinha();
                        break;
                    case OperationCode.FIM_DA_LINHA:
                        DesenhaFimLinha();
                        break;
                    case OperationCode.CONTATO_NA:
                        DesenhaContatoNA();
                        break;
                    case OperationCode.CONTATO_NF:
                        DesenhaContatoNF();
                        break;
                    case OperationCode.BOBINA_SAIDA:
                        DesenhaBobinaSaida();
                        break;
                    case OperationCode.TEMPORIZADOR:
                        DesenhaQuadroSaida();
                        break;
                    case OperationCode.CONTADOR:
                        DesenhaQuadroSaida();
                        break;
                    case OperationCode.PARALELO_INICIAL:
                        DesenhaParaleloInicial();
                        break;
                    case OperationCode.PARALELO_FINAL:
                        DesenhaParaleloFinal();
                        break;
                    case OperationCode.PARALELO_PROXIMO:
                        DesenhaParaleloProximo();
                        break;
                    case OperationCode.LINHA_DE_FUNDO:
                        DesenhaFundo();
                        break;
                    case OperationCode.RESET:
                        DesenhaBobinaComString("R");
                        break;
                    default:
                        break;
                }

                if (this.selecionado)
                    SelecaoSimbolo(TipoSelecaoFlag.Borda);

            }
        }

        private void ControleLivre_Load(object sender, EventArgs e)
        {
            this.e = Graphics.FromHwnd(Handle);
            if (OpCode!= null)
                AtualizaVariaveisDesenho();
        }

        private void ControleLivre_Paint(object sender, PaintEventArgs e)
        {
            if (this.e != null && VisualLine.tamY > 0)
            {
                this.e = e.Graphics;
                DesenhaSimbolo();
            }
        }

        private void ControleLivre_Enter(object sender, EventArgs e)
        {
            if (OpCode!= OperationCode.LINHA_DE_FUNDO &&
                OpCode!= OperationCode.FIM_DA_LINHA)
            {
                this.selecionado = true;

                if (ControleSelecionado != null)
                    ControleSelecionado(this, VisualLine);

                LadderForm _frmDL;
                _frmDL = (LadderForm)this.Parent;
                _frmDL.SetMessage(this.Location.ToString() + " - " + this.Size.ToString() + " - " + this.TabStop.ToString() + " - " + this.TabIndex.ToString() + " - " + this.OpCode.ToString());

                this.Refresh();
            }
        }

        private void ControleLivre_Leave(object sender, EventArgs e)
        {
            if (OpCode!= OperationCode.LINHA_DE_FUNDO &&
                OpCode!= OperationCode.FIM_DA_LINHA)
            {
                this.Refresh();
            }
        }

        public void NovasInsercoesLinhaHorizontal(bool bHabilita)
        {
            if (bHabilita == true && OpCode!= OperationCode.FIM_DA_LINHA)
            {
                indicadorInsercao = new System.Windows.Forms.Panel();
                indicadorInsercao.BackColor = Color.Transparent;
                indicadorInsercao.Size = new Size(15, 15);
                indicadorInsercao.Location = new Point(this.Size.Width - 15, (this.Location.Y + (this.Size.Height / 2) - (indicadorInsercao.Size.Height / 2)));
                indicadorInsercao.Parent = this;
                indicadorInsercao.BringToFront();
                indicadorInsercao.CreateControl();

                listaIndicadores.Add(indicadorInsercao);
            }
            else if (OpCode!= OperationCode.FIM_DA_LINHA)
            {
                foreach (System.Windows.Forms.Panel ii in listaIndicadores)
                {
                    ii.Dispose();
                }
                listaIndicadores.Clear();
            }
        }

        private void ControleLivre_MouseEnter(object sender, EventArgs e)
        {
            if (OpCode!= OperationCode.FIM_DA_LINHA)
                NovasInsercoesLinhaHorizontal(false); // era true
        }

        private void ControleLivre_MouseLeave(object sender, EventArgs e)
        {
            if (OpCode!= OperationCode.FIM_DA_LINHA)
                NovasInsercoesLinhaHorizontal(false);
        }

        private void ControleLivre_Resize(object sender, EventArgs e)
        {
            if (OpCode!= null)
                AtualizaVariaveisDesenho();
        }

        private void ControleLivre_SizeChanged(object sender, EventArgs e)
        {
            if (OpCode!= null)
                AtualizaVariaveisDesenho();
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if (keyData == Keys.Down ||
                keyData == Keys.Up)
                return true;

            return base.IsInputKey(keyData);
        }

        private void ControleLivre_KeyDown(object sender, KeyEventArgs e)
        {

            if (this.OpCode== OperationCode.INICIO_DA_LINHA)
            {
                if (MudaLinha != null &&
                    (e.KeyData == Keys.Down || e.KeyData == Keys.Up))
                {
                    MudaLinha(this, e.KeyData);
                }

                if (DeletaLinha != null &&
                    (e.KeyData == Keys.Delete))
                {
                    DeletaLinha(VisualLine);
                }
            }
        }

        /// <summary>
        /// Retorna o simbolobasico do objeto
        /// </summary>
        public Symbol SimboloBasico
        {
            get { return codigoInterpretavel; }
        }

        private void ApresentaCamposTxt()
        {
        }

        private void txtGeral_Enter(object sender, EventArgs e)
        {
            ((Label)sender).BackColor = Color.Black;
            ((Label)sender).ForeColor = Color.White;
        }

        private void txtGeral_Leave(object sender, EventArgs e)
        {
            ((Label)sender).BackColor = Color.Transparent;
            ((Label)sender).ForeColor = Color.Black;
        }

        private void txtEndereco_DoubleClick(object sender, EventArgs e)
        {
            /// caso o evento nao este alocado, nao executa
            if (SolicitaMudarEndereco == null)
                return;

            switch (OpCode)
            {
                case OperationCode.CONTATO_NA:
                case OperationCode.CONTATO_NF:
                    SolicitaMudarEndereco(this, new Rectangle(0, 0, 0, 0), (new Address()).GetType(), 0, 0, null);
                    break;
                case OperationCode.BOBINA_SAIDA:
                    SolicitaMudarEndereco(this, new Rectangle(0, 0, 0, 0), (new Address()).GetType(), 0, 0, null);
                    break;
                case OperationCode.TEMPORIZADOR:
                case OperationCode.CONTADOR:
                    SolicitaMudarEndereco(this, new Rectangle(0, 0, 0, 0), (new Address()).GetType(), 0, 0, null);
                    break;
            }
        }

        private void txtGeral_Click(object sender, EventArgs e)
        {
            ((Label)sender).Select();
        }

        private void ControleLivre_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            switch (OpCode)
            {
                case OperationCode.CONTATO_NA:
                case OperationCode.CONTATO_NF:
                    //                    SolicitaMudarEndereco(this, new Rectangle(0, 0, 0, 0), (new EnderecamentoLadder()).GetType(), 0, 0, null);
                    break;
                case OperationCode.BOBINA_SAIDA:
                    break;
                case OperationCode.TEMPORIZADOR:
                case OperationCode.CONTADOR:
                    SolicitaMudarEndereco(this, new Rectangle(0, 0, 0, 0), (new Address()).GetType(), 0, 0, null);
                    break;
            }
        }
    }
}