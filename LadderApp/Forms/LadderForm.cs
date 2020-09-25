
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

        private VisualInstructionUserControl controleSelecionado = null;
        public VisualInstructionUserControl ControleSelecionado
        {
            get { return controleSelecionado; }
        }

        private VisualLine linhaSelecionada = null;
        public VisualLine LinhaSelecionada
        {
            get { return linhaSelecionada; }
        }


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
                ReorganizandoLinhas();
                ResizingLines = false;
            }
        }

        public void SetMessage(String Texto)
        {
            //lblstatusMensagem.Text = Texto;
        }

        public void ApagaLinhasSimbolos()
        {
            foreach (VisualLine lcl in visualProgram.linhas)
            {
            }
        }

        public void DeletaLinha(VisualLine sender)
        {
            int _indiceLinhaDeletar = this.visualProgram.linhas.IndexOf(sender);
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

                if (this.visualProgram.linhas.Count > 0)
                    this.visualProgram.linhas[_indiceLinha - 1].LineBegin.Select();

                this.ReorganizandoLinhas();

                if (this.visualProgram.linhas.Count > 0)
                    this.visualProgram.linhas[_indiceLinha - 1].LineBegin.Refresh();

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

            if (visualProgram != null)
            {
                if (visualProgram.linhas.Count > 0)
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
                    VisualLine _LinhaAnterior = visualProgram.linhas[visualProgram.linhas.Count - 1];

                    if (_numVezes == 0)
                        visualProgram.linhas[visualProgram.linhas.Count - 1].LineEnd.posicaoXY = new Point(auxX, 0);

                    foreach (VisualLine _linhasDL in visualProgram.linhas)
                    {
                        if (_LinhaAnterior != null)
                            _LinhaAnterior.NextLine = _linhasDL;
                        _linhasDL.PreviousLine = _LinhaAnterior;
                        _linhasDL.iTabStop = iTabStop;
                        _linhasDL.posY = auxY;
                        _linhasDL.NumLinha = iLinha;
                        _linhasDL.LineBegin.Invalidate();
                        _linhasDL.AjustaPosicionamento();
                        auxY += _linhasDL.BackgroundLine.tamanhoXY.Height;
                        iTabStop = _linhasDL.iTabStop;
                        iLinha++;
                        _LinhaAnterior = _linhasDL;

                        _linhasDL.BackgroundLine.Invalidate();

                        if (auxX < _linhasDL.LineEnd.posicaoXY.X)
                            auxX = _linhasDL.LineEnd.posicaoXY.X;

                    }
                    _LinhaAnterior.NextLine = visualProgram.linhas[0];

                    if (tempVertical > 0 || tempHorizontal > 0)
                    {
                        this.VerticalScroll.Value = tempVertical;
                        this.HorizontalScroll.Value = tempHorizontal;
                    }
                    this.AutoScroll = true;
                }

                if (_numVezes == 0 && visualProgram.linhas.Count > 0)
                {
                    visualProgram.linhas[visualProgram.linhas.Count - 1].LineEnd.posicaoXY = new Point(auxX, visualProgram.linhas[0].LineEnd.posicaoXY.Y);
                    ReorganizandoLinhas(1);
                }

                if (_numVezes == 1)
                {
                    if (visualProgram.linhas.Count > 0)
                    {
                        this.VerticalScroll.Maximum = visualProgram.linhas[visualProgram.linhas.Count - 1].LineEnd.posicaoXY.Y + visualProgram.linhas[visualProgram.linhas.Count - 1].LineEnd.tamanhoXY.Height;
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
                if (!LinhaSelecionada.LineBegin.IsDisposed)
                {
                    _linha = (int)LinhaSelecionada.LineBegin.GetOperand(0);

                    if (_acima == false)
                        _linha++;
                }
                else
                    _linha = 0;
            }

            this.SuspendLayout();

            _linha = visualProgram.InsereLinhaNoIndice(_linha);
            this.ReorganizandoLinhas();

            visualProgram.linhas[_linha].LineBegin.Select();
            visualProgram.linhas[_linha].LineBegin.Refresh();


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

        public void simboloInicioLinha_MudaLinha(VisualInstructionUserControl sender, Keys e)
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

        public void Simbolo_ControleSelecionado(VisualInstructionUserControl sender, VisualLine lCL)
        {
            if (controleSelecionado != null)
                if (!controleSelecionado.IsDisposed)
                    if(!controleSelecionado.Equals(sender))
                    {
                        controleSelecionado.Selected = false;
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
            if (((VisualInstructionUserControl)sender).Tag != null)
                if (((String)((VisualInstructionUserControl)sender).Tag) != "")
                    toolTipQuadrosSaida.Show((String)((VisualInstructionUserControl)sender).Tag, ((VisualInstructionUserControl)sender), 3000);
        }


        public void Simbolo_KeyDown(object sender, KeyEventArgs e)
        {
            VisualInstructionUserControl _cL = (VisualInstructionUserControl)sender;
            switch (_cL.OpCode)
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
                        if (_cL != null && linhaSelecionada != null)
                            if (!_cL.IsDisposed)
                            {
                                switch (_cL.OpCode)
                                {
                                    case OperationCode.ParallelBranchBegin:
                                        break;
                                    case OperationCode.ParallelBranchNext:
                                        break;
                                    case OperationCode.ParallelBranchEnd:
                                        break;
                                    default:
                                        break;
                                }

                                this.SuspendLayout();
                                this.SelectNextControl(_cL, false, true, false, false);
                                linhaSelecionada.ApagaSimbolos(_cL);
                                this.ReorganizandoLinhas();
                                this.ResumeLayout();
                                linhaSelecionada.BackgroundLine.Invalidate();
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
                    if (LinhaSelecionada.visualInstructions.Contains(_cL))
                        _lstCL = LinhaSelecionada.visualInstructions;
                    else if (LinhaSelecionada.visualOutputInstructions.Contains(_cL))
                        _lstCL = LinhaSelecionada.visualOutputInstructions;
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

        private void menuLimparEndereco_Click(object sender, EventArgs e)
        {
            OperationCode _cI = controleSelecionado.OpCode;
            if ((!controleSelecionado.IsDisposed) &&
                 (_cI != OperationCode.LineBegin &&
                 _cI != OperationCode.ParallelBranchBegin &&
                 _cI != OperationCode.ParallelBranchEnd))
            {
                controleSelecionado.SetOperand(0, null);
                controleSelecionado.Refresh();
            }
        }

        private void menuEstenderParaleloAcima_Click(object sender, EventArgs e)
        {
            switch (controleSelecionado.OpCode)
            {
                case OperationCode.ParallelBranchBegin:
                    break;
                case OperationCode.ParallelBranchEnd:
                    break;
                case OperationCode.ParallelBranchNext:
                    break;
            }
        }

        private void DiagramaLadder_Scroll(object sender, ScrollEventArgs e)
        {
        }

        private void menuToggleBit_Click(object sender, EventArgs e)
        {
            ((Address)controleSelecionado.GetOperand(0)).Valor = ((Address)controleSelecionado.GetOperand(0)).Valor == true ? false : true;
            projectForm.program.ExecutaLadderSimulado();
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
                    changeTimerCounterParametersForm.Type = (Int32)((Address)sender.GetOperand(0)).Timer.Tipo;
                    changeTimerCounterParametersForm.Preset = (Int32)((Address)sender.GetOperand(0)).Timer.Preset;
                    changeTimerCounterParametersForm.Accumulated = (Int32)((Address)sender.GetOperand(0)).Timer.Acumulado;
                    changeTimerCounterParametersForm.TimeBase = (Int32)((Address)sender.GetOperand(0)).Timer.BaseTempo;
                    break;
                case OperationCode.Counter:
                    changeTimerCounterParametersForm.Type = (Int32)((Address)sender.GetOperand(0)).Counter.Tipo;
                    changeTimerCounterParametersForm.Preset = (Int32)((Address)sender.GetOperand(0)).Counter.Preset;
                    changeTimerCounterParametersForm.Accumulated = (Int32)((Address)sender.GetOperand(0)).Counter.Acumulado;
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

                        ((Address)sender.GetOperand(0)).Timer.Tipo = changeTimerCounterParametersForm.Type;
                        ((Address)sender.GetOperand(0)).Timer.Preset = changeTimerCounterParametersForm.Preset;
                        ((Address)sender.GetOperand(0)).Timer.Acumulado = changeTimerCounterParametersForm.Accumulated;
                        ((Address)sender.GetOperand(0)).Timer.BaseTempo = changeTimerCounterParametersForm.TimeBase;

                        sender.SetOperand(1, ((Address)sender.GetOperand(0)).Timer.Tipo);
                        sender.SetOperand(2, ((Address)sender.GetOperand(0)).Timer.Preset);
                        sender.SetOperand(3, ((Address)sender.GetOperand(0)).Timer.Acumulado);
                        sender.SetOperand(4, ((Address)sender.GetOperand(0)).Timer.BaseTempo);

                        break;
                    case OperationCode.Counter:
                        ((Address)sender.GetOperand(0)).Counter.Tipo = changeTimerCounterParametersForm.Type;
                        ((Address)sender.GetOperand(0)).Counter.Preset = changeTimerCounterParametersForm.Preset;
                        ((Address)sender.GetOperand(0)).Counter.Acumulado = changeTimerCounterParametersForm.Accumulated;

                        sender.SetOperand(1, ((Address)sender.GetOperand(0)).Counter.Tipo);
                        sender.SetOperand(2, ((Address)sender.GetOperand(0)).Counter.Preset);
                        sender.SetOperand(3, ((Address)sender.GetOperand(0)).Counter.Acumulado);
                        break;
                    default:
                        break;
                }

                sender.Invalidate();
            }
            
        }

        private void menuToggleBitPulse_Click(object sender, EventArgs e)
        {
            projectForm.program.auxToggleBitPulse = ((Address)controleSelecionado.GetOperand(0));
            projectForm.program.auxToggleBitPulse.Valor = projectForm.program.auxToggleBitPulse.Valor == true ? false : true;
            projectForm.program.ExecutaLadderSimulado();
            this.Invalidate(true);
        }

    }
}