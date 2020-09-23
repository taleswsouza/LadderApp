
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

        public ProjectForm linkProjeto = null;

        public VisualProgram prgLivre = null;

        private Boolean RedimencionandoLinhas = false;

        private FreeUserControl controleSelecionado = null;
        public FreeUserControl ControleSelecionado
        {
            get { return controleSelecionado; }
        }

        private VisualLine linhaSelecionada = null;
        public VisualLine LinhaSelecionada
        {
            get { return linhaSelecionada; }
        }


        public List<Instruction> lstVariosSelecionados = new List<Instruction>();

        public LadderForm(LadderProgram _prgBasico)
        {
            InitializeComponent();

            this.VScroll = false;
            this.HScroll = false;

            prgLivre = new VisualProgram(_prgBasico, this);
        }

        private void DiagramaLadder_Resize(object sender, EventArgs e)
        {
            if (RedimencionandoLinhas == false)
            {
                RedimencionandoLinhas = true;
                ReorganizandoLinhas();
                RedimencionandoLinhas = false;
            }
        }

        public void SetMessage(String Texto)
        {
            //lblstatusMensagem.Text = Texto;
        }

        public void ApagaLinhasSimbolos()
        {
            foreach (VisualLine lcl in prgLivre.linhas)
            {
            }
        }

        public void DeletaLinha(VisualLine sender)
        {
            int _indiceLinhaDeletar = this.prgLivre.linhas.IndexOf(sender);
            int _indiceLinha = _indiceLinhaDeletar;

            if (_indiceLinha >= 0)
            {
                this.SuspendLayout();

                /// Faz todos os tratamentos para
                /// desconsiderar a linha a ser deleta
                /// OBS.: Isto garante um melhor agilidade
                ///     no desenho da tela.
                this.prgLivre.ApagaLinha(_indiceLinha);

                if (_indiceLinha == 0)
                    _indiceLinha = 1;

                if (this.prgLivre.linhas.Count > 0)
                    this.prgLivre.linhas[_indiceLinha - 1].simboloInicioLinha.Select();

                this.ReorganizandoLinhas();

                if (this.prgLivre.linhas.Count > 0)
                    this.prgLivre.linhas[_indiceLinha - 1].simboloInicioLinha.Refresh();

                this.ResumeLayout();
            }
        }

        public void ReorganizandoLinhas()
        {
            ReorganizandoLinhas(0);
        }


        public void ReorganizandoLinhas(int _numVezes)
        {
            int auxY = 0;
            int iTabStop = 0;
            int iLinha = 0;
            int auxX = 0;

            if (prgLivre != null)
            {
                if (prgLivre.linhas.Count > 0)
                {
                    if (_numVezes == 0)
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
                    VisualLine _LinhaAnterior = prgLivre.linhas[prgLivre.linhas.Count - 1];

                    if (_numVezes == 0)
                        prgLivre.linhas[prgLivre.linhas.Count - 1].simboloFimLinha.posicaoXY = new Point(auxX, 0);

                    foreach (VisualLine _linhasDL in prgLivre.linhas)
                    {
                        if (_LinhaAnterior != null)
                            _LinhaAnterior.linhaProxima = _linhasDL;
                        _linhasDL.linhaAnterior = _LinhaAnterior;
                        _linhasDL.iTabStop = iTabStop;
                        _linhasDL.posY = auxY;
                        _linhasDL.NumLinha = iLinha;
                        _linhasDL.simboloInicioLinha.Invalidate();
                        _linhasDL.AjustaPosicionamento();
                        auxY += _linhasDL.simboloDesenhoFundo.tamanhoXY.Height;
                        iTabStop = _linhasDL.iTabStop;
                        iLinha++;
                        _LinhaAnterior = _linhasDL;

                        _linhasDL.simboloDesenhoFundo.Invalidate();

                        if (auxX < _linhasDL.simboloFimLinha.posicaoXY.X)
                            auxX = _linhasDL.simboloFimLinha.posicaoXY.X;

                    }
                    _LinhaAnterior.linhaProxima = prgLivre.linhas[0];

                    if (tempVertical > 0 || tempHorizontal > 0)
                    {
                        this.VerticalScroll.Value = tempVertical;
                        this.HorizontalScroll.Value = tempHorizontal;
                    }
                    this.AutoScroll = true;
                }

                if (_numVezes == 0 && prgLivre.linhas.Count > 0)
                {
                    prgLivre.linhas[prgLivre.linhas.Count - 1].simboloFimLinha.posicaoXY = new Point(auxX, prgLivre.linhas[0].simboloFimLinha.posicaoXY.Y);
                    ReorganizandoLinhas(1);
                }

                if (_numVezes == 1)
                {
                    if (prgLivre.linhas.Count > 0)
                    {
                        this.VerticalScroll.Maximum = prgLivre.linhas[prgLivre.linhas.Count - 1].simboloFimLinha.posicaoXY.Y + prgLivre.linhas[prgLivre.linhas.Count - 1].simboloFimLinha.tamanhoXY.Height;
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

        /// <summary>
        /// Insere linha abaixo da linha selecionada
        /// </summary>
        public int InsereLinha()
        {
            return InsereLinha(false);
        }

        /// <summary>
        /// Insere linha abaixo ou acima da linha selecionada
        /// </summary>
        /// <param name="_acima">true - acima / false - abaixo</param>
        public int InsereLinha(Boolean _acima)
        {
            int _linha = 0;
            if (LinhaSelecionada != null)
            {
                if (!LinhaSelecionada.simboloInicioLinha.IsDisposed)
                {
                    _linha = (int)LinhaSelecionada.simboloInicioLinha.GetOperand(0);

                    if (_acima == false)
                        _linha++;
                }
                else
                    _linha = 0;
            }

            this.SuspendLayout();

            _linha = prgLivre.InsereLinhaNoIndice(_linha);
            this.ReorganizandoLinhas();

            prgLivre.linhas[_linha].simboloInicioLinha.Select();
            prgLivre.linhas[_linha].simboloInicioLinha.Refresh();


            this.ResumeLayout();

            return _linha;
        }

        private void menuInsereLinhaAbaixo_Click(object sender, EventArgs e)
        {
            this.InsereLinha();
        }

        private void menuInsereLinhaAcima_Click(object sender, EventArgs e)
        {
            this.InsereLinha(true);
        }

        private void DiagramaLadder_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                /// Enderecamentos e somente sobre controleslivres
                this.menuEnderecamento.Enabled = false;
                this.menuEnderecamento.Visible = false;

                /// Manipular linhas
                this.menuInsereLinha.Enabled = true;

                /// Extensao de paralelo - acima/abaixo
                ///    somente sobre simbolos de paralelo
                this.menuEstenderParaleloAcima.Enabled = false;
                this.menuEstenderParaleloAcima.Visible = false;
                this.menuEstenderParaleloAbaixo.Enabled = false;
                this.menuEstenderParaleloAbaixo.Visible = false;

                this.menuToggleBit.Enabled = false;

                this.menuControle.Show(this.PointToScreen(e.Location));
            }
        }

        public void simboloInicioLinha_MudaLinha(FreeUserControl sender, Keys e)
        {
            if (e == Keys.Up)
            {
                if ((int)sender.VisualLine.simboloInicioLinha.GetOperand(0) != 0)
                    sender.VisualLine.linhaAnterior.simboloInicioLinha.Select();
            }
            else if (e == Keys.Down)
            {
                if ((int)sender.VisualLine.linhaProxima.simboloInicioLinha.GetOperand(0) != 0)
                    sender.VisualLine.linhaProxima.simboloInicioLinha.Select();
            }
        }

        public void Simbolo_ControleSelecionado(FreeUserControl sender, VisualLine lCL)
        {
            if (controleSelecionado != null)
                if (!controleSelecionado.IsDisposed)
                    if(!controleSelecionado.Equals(sender))
                    {
                        controleSelecionado.selecionado = false;
                        controleSelecionado.Refresh();
                    }
            //cmbOpcoes.Visible = false;
            controleSelecionado = sender;
            linhaSelecionada = lCL;
        }

        /// <summary>
        /// Tratamento de evento para indicar comentário nos quadros de saida
        /// temporizador / contador via tooltip.
        /// </summary>
        public void SimboloQuadroSaida_MouseHover(object sender, EventArgs e)
        {
            /// o objeto .Tag e atribuido na função DesenhaQuadroSaida() da
            /// ControleLivre
            if (((FreeUserControl)sender).Tag != null)
                if (((String)((FreeUserControl)sender).Tag) != "")
                    toolTipQuadrosSaida.Show((String)((FreeUserControl)sender).Tag, ((FreeUserControl)sender), 3000);
        }


        public void Simbolo_KeyDown(object sender, KeyEventArgs e)
        {
            FreeUserControl _cL = (FreeUserControl)sender;
            switch (_cL.OpCode)
            {
                case OperationCode.None:
                    /// Tive que colocar aqui esta opcao de NENHUM para evitar que
                    /// a execucao passasse duas vezes em apagar
                    break;
                case OperationCode.INICIO_DA_LINHA:
                    break;
                default:
                    if (e.KeyCode == Keys.Delete)
                    {
                        if (_cL != null && linhaSelecionada != null)
                            if (!_cL.IsDisposed)
                            {
                                switch (_cL.OpCode)
                                {
                                    case OperationCode.PARALELO_INICIAL:
                                        break;
                                    case OperationCode.PARALELO_PROXIMO:
                                        break;
                                    case OperationCode.PARALELO_FINAL:
                                        break;
                                    default:
                                        break;
                                }

                                this.SuspendLayout();
                                this.SelectNextControl(_cL, false, true, false, false);
                                linhaSelecionada.ApagaSimbolos(_cL);
                                this.ReorganizandoLinhas();
                                this.ResumeLayout();
                                linhaSelecionada.simboloDesenhoFundo.Invalidate();
                            }
                    }
                    break;
            }
        }

        public List<Instruction> VariosSelecionados(FreeUserControl _cL, VisualLine _lCL)
        {
            OperationCode _cI = _cL.OpCode;
            List<Instruction> _lstSB = new List<Instruction>();
            List<FreeUserControl> _lstCL = null;

            switch (_cI)
            {
                case OperationCode.PARALELO_INICIAL:
                case OperationCode.PARALELO_PROXIMO:
                case OperationCode.PARALELO_FINAL:
                    int _indicePosInicial = 0;
                    int _indicePosFinal = 0;

                    /// verifica em qual lista o controle esta presente (simbolos/saida)
                    if (LinhaSelecionada.simbolos.Contains(_cL))
                        _lstCL = LinhaSelecionada.simbolos;
                    else if (LinhaSelecionada.saida.Contains(_cL))
                        _lstCL = LinhaSelecionada.saida;
                    else
                        return _lstSB;

                    /// define a posicao inicial a partir da posicao
                    /// do controle na lista
                    _indicePosInicial = _lstCL.IndexOf(_cL);

                    if (_cI == OperationCode.PARALELO_FINAL)
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
                        _lstSB.Add(_lstCL[i].SimboloBasico);
                    }
                    break;
                default:
                    _lstSB.Add(_cL.SimboloBasico);
                    break;
            }
            return _lstSB;
        }

        private void menuLimparEndereco_Click(object sender, EventArgs e)
        {
            OperationCode _cI = controleSelecionado.OpCode;
            if ((!controleSelecionado.IsDisposed) &&
                 (_cI != OperationCode.INICIO_DA_LINHA &&
                 _cI != OperationCode.PARALELO_INICIAL &&
                 _cI != OperationCode.PARALELO_FINAL))
            {
                controleSelecionado.SetOperand(0, null);
                controleSelecionado.Refresh();
            }
        }

        private void menuEstenderParaleloAcima_Click(object sender, EventArgs e)
        {
            switch (controleSelecionado.OpCode)
            {
                case OperationCode.PARALELO_INICIAL:
                    break;
                case OperationCode.PARALELO_FINAL:
                    break;
                case OperationCode.PARALELO_PROXIMO:
                    break;
            }
        }

        private void DiagramaLadder_Scroll(object sender, ScrollEventArgs e)
        {
        }

        private void menuToggleBit_Click(object sender, EventArgs e)
        {
            ((Address)controleSelecionado.GetOperand(0)).Valor = ((Address)controleSelecionado.GetOperand(0)).Valor == true ? false : true;
            linkProjeto.programa.ExecutaLadderSimulado();
            this.Invalidate(true);
        }

        public void ControleSelecionado_SolicitaMudarEndereco(FreeUserControl sender, Rectangle rect, Type tipo, int valorMax, int valorMin, params object[] faixa)
        {
            ChangeTimerCounterParametersForm Altera = new ChangeTimerCounterParametersForm(sender.OpCode);

            if (!sender.IsAllOperandsOk())
            {
                MessageBox.Show("Please, assign an address first!", "Change configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            switch (sender.OpCode)
            {
                case OperationCode.TEMPORIZADOR:
                    Altera.Tipo = (Int32)((Address)sender.GetOperand(0)).Temporizador.Tipo;
                    Altera.Preset = (Int32)((Address)sender.GetOperand(0)).Temporizador.Preset;
                    Altera.Acumulado = (Int32)((Address)sender.GetOperand(0)).Temporizador.Acumulado;
                    Altera.BaseTempo = (Int32)((Address)sender.GetOperand(0)).Temporizador.BaseTempo;
                    break;
                case OperationCode.CONTADOR:
                    Altera.Tipo = (Int32)((Address)sender.GetOperand(0)).Contador.Tipo;
                    Altera.Preset = (Int32)((Address)sender.GetOperand(0)).Contador.Preset;
                    Altera.Acumulado = (Int32)((Address)sender.GetOperand(0)).Contador.Acumulado;
                    break;
                default:
                    break;
            }

            DialogResult _result = Altera.ShowDialog();

            if (_result == DialogResult.OK)
            {
                /// mantem os parametros do ci atualizados
                sender.SetOperand(1, Altera.Tipo);
                sender.SetOperand(2, Altera.Preset);
                sender.SetOperand(3, Altera.Acumulado);
                switch (sender.OpCode)
                {
                    case OperationCode.TEMPORIZADOR:
                        /// mantem os parametros do ci atualizados
                        sender.SetOperand(4, Altera.BaseTempo);

                        ((Address)sender.GetOperand(0)).Temporizador.Tipo = Altera.Tipo;
                        ((Address)sender.GetOperand(0)).Temporizador.Preset = Altera.Preset;
                        ((Address)sender.GetOperand(0)).Temporizador.Acumulado = Altera.Acumulado;
                        ((Address)sender.GetOperand(0)).Temporizador.BaseTempo = Altera.BaseTempo;

                        sender.SetOperand(1, ((Address)sender.GetOperand(0)).Temporizador.Tipo);
                        sender.SetOperand(2, ((Address)sender.GetOperand(0)).Temporizador.Preset);
                        sender.SetOperand(3, ((Address)sender.GetOperand(0)).Temporizador.Acumulado);
                        sender.SetOperand(4, ((Address)sender.GetOperand(0)).Temporizador.BaseTempo);

                        break;
                    case OperationCode.CONTADOR:
                        ((Address)sender.GetOperand(0)).Contador.Tipo = Altera.Tipo;
                        ((Address)sender.GetOperand(0)).Contador.Preset = Altera.Preset;
                        ((Address)sender.GetOperand(0)).Contador.Acumulado = Altera.Acumulado;

                        sender.SetOperand(1, ((Address)sender.GetOperand(0)).Contador.Tipo);
                        sender.SetOperand(2, ((Address)sender.GetOperand(0)).Contador.Preset);
                        sender.SetOperand(3, ((Address)sender.GetOperand(0)).Contador.Acumulado);
                        break;
                    default:
                        break;
                }

                sender.Invalidate();
            }
            
        }

        private void menuToggleBitPulse_Click(object sender, EventArgs e)
        {
            linkProjeto.programa.auxToggleBitPulse = ((Address)controleSelecionado.GetOperand(0));
            linkProjeto.programa.auxToggleBitPulse.Valor = linkProjeto.programa.auxToggleBitPulse.Valor == true ? false : true;
            linkProjeto.programa.ExecutaLadderSimulado();
            this.Invalidate(true);
        }

    }
}