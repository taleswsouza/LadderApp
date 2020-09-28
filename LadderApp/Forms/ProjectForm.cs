using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LadderApp
{
    public partial class ProjectForm : Form
    {
        public LadderForm ladderForm;
        public LadderProgram program = new LadderProgram();

        public ProjectForm(LadderProgram program) : this()
        {
            this.program = program;
        }

        public ProjectForm()
        {
            InitializeComponent();
        }

        public void SetText()
        {
            this.Text = program.Nome;
        }

        private void ProjetoLadder_Resize(object sender, EventArgs e)
        {
            ArvoreProjeto.Location = new Point(0, 0);
            ArvoreProjeto.Size = new Size(this.Size.Width, this.Size.Height);
        }

        private void ProjetoLadder_Load(object sender, EventArgs e)
        {
            ArvoreProjeto.TopNode.Expand();

            if (program.Status == LadderProgram.ProgramStatus.NotInitialized)
            {
                program.device = new Device(1);

                AlocaEnderecamentoIO();

                AlocaEnderecamentoMemoria(program.addressing.ListMemoryAddress, AddressTypeEnum.DigitalMemory, 10);
                AlocaEnderecamentoMemoria(program.addressing.ListTimerAddress, AddressTypeEnum.DigitalMemoryTimer, 10);
                AlocaEnderecamentoMemoria(program.addressing.ListCounterAddress, AddressTypeEnum.DigitalMemoryCounter, 10);
            }
            else
            {
                if (program.device == null)
                    program.device = new Device(1);
                AlocaEnderecamentoIO();

                AlocaEnderecamentoMemoria(program.addressing.ListMemoryAddress, AddressTypeEnum.DigitalMemory, program.addressing.ListMemoryAddress.Count);
                AlocaEnderecamentoMemoria(program.addressing.ListTimerAddress, AddressTypeEnum.DigitalMemoryTimer, program.addressing.ListTimerAddress.Count);
                AlocaEnderecamentoMemoria(program.addressing.ListCounterAddress, AddressTypeEnum.DigitalMemoryCounter, program.addressing.ListCounterAddress.Count);

                program.ReindexAddresses();
            }

            //if (!CheckLadderFormIsNotNull())
                OpenLadderForm();
        }


        private void ProjetoLadder_Shown(object sender, EventArgs e)
        {
            MainWindowForm mainWindowForm = (MainWindowForm)this.MdiParent;
            mainWindowForm.ResetWindowLayout();
        }

        private void ProjetoLadder_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show(String.Format("Do you want to save the project {0}?", Text), "LadderApp", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            switch (result)
            {
                case DialogResult.Cancel:
                    e.Cancel = true;
                    break;
                case DialogResult.Yes:
                    ladderForm.Close();
                    break;
                case DialogResult.No:
                    ladderForm.Close();
                    break;
            }
        }

        private void ArvoreProjeto_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            switch (e.Node.Name)
            {
                case "NoProgramaLadder":
                    //if (!CheckLadderFormIsNotNull())
                    //{
                        OpenLadderForm();
                    //}
                    break;
                case "NoDispositivo":
                    DeviceForm frmDisp = new DeviceForm(program.device);

                    if (frmDisp.ShowDialog() == DialogResult.OK)
                    {
                        int i = 0;
                        foreach (Pin pd in program.device.lstBitPorta)
                        {
                            pd.TipoDefinido = frmDisp.lstEndModificado[i];
                            i++;
                        }
                        program.device.RealocaEnderecoDispositivo();

                        AlocaEnderecamentoIO();
                    }
                    break;
                case "NoConfMemoria":
                    AddressingForm addressingForm = new AddressingForm(program.addressing);
                    addressingForm.Owner = this;

                    if (addressingForm.ShowDialog() == DialogResult.OK)
                    {
                        AlocaEnderecamentoMemoria(program.addressing.ListMemoryAddress, AddressTypeEnum.DigitalMemory, addressingForm.NumberOfMemoryAddresses);
                        AlocaEnderecamentoMemoria(program.addressing.ListTimerAddress, AddressTypeEnum.DigitalMemoryTimer, addressingForm.NumberOfTimerAddresses);
                        AlocaEnderecamentoMemoria(program.addressing.ListCounterAddress, AddressTypeEnum.DigitalMemoryCounter, addressingForm.NumberOfCounterAddresses);
                    }
                    break;
                default:
                    if (e.Node.Tag != null)
                        if (this.ladderForm.ControleSelecionado != null)
                        {
                            InsereEnderecoNoSimbolo(this.ladderForm.ControleSelecionado, (Address)e.Node.Tag);
                            this.ladderForm.ActiveControl = this.ladderForm.ControleSelecionado;
                        }
                    break;
            }
        }

        public void OpenLadderForm()
        {
            ladderForm = new LadderForm(program);
            ladderForm.projectForm = this;
            ladderForm.MdiParent = this.MdiParent;
            ladderForm.ReorganizeLines();
            ladderForm.Show();
        }

        public void AlocaEnderecamentoIO()
        {
            /// Atalho para o No de enderecamento
            TreeNode _NoEnderecamento = ArvoreProjeto.Nodes["NoProjeto"].Nodes["NoEnderecamento"];
            _NoEnderecamento.Nodes["NoEntradas"].Nodes.Clear();
            _NoEnderecamento.Nodes["NoSaidas"].Nodes.Clear();
            program.addressing.ListInputAddress.Clear();
            program.addressing.ListOutputAddress.Clear();
            foreach (Address el in program.device.lstEndBitPorta)
            {
                el.ApontaDispositivo(program.device);
                switch (el.AddressType)
                {
                    case AddressTypeEnum.DigitalInput:
                        program.addressing.ListInputAddress.Add(el);
                        //if (!_NoEnderecamento.Nodes["NoEntradas"].Nodes.ContainsKey(el.Nome))
                        //{
                        _NoEnderecamento.Nodes["NoEntradas"].Nodes.Add(el.Name, el.Name + (el.Comment == "" ? "" : " - " + el.Comment));
                        _NoEnderecamento.Nodes["NoEntradas"].Nodes[el.Name].Tag = el;
                        //el.MudouComentario += new MudouComentarioEventHandler(Endereco_MudouComentario);
                        //}
                        break;
                    case AddressTypeEnum.DigitalOutput:
                        program.addressing.ListOutputAddress.Add(el);
                        //if (!_NoEnderecamento.Nodes["NoSaidas"].Nodes.ContainsKey(el.Nome))
                        //{
                        _NoEnderecamento.Nodes["NoSaidas"].Nodes.Add(el.Name, el.Name + (el.Comment == "" ? "" : " - " + el.Comment));
                        _NoEnderecamento.Nodes["NoSaidas"].Nodes[el.Name].Tag = el;
                        //el.MudouComentario += new MudouComentarioEventHandler(Endereco_MudouComentario);
                        //}
                        break;
                }
                el.EditedCommentEvent += new EditedCommentEventHandler(Endereco_MudouComentario);
            }

        }

        void Endereco_MudouComentario(Address sender)
        {
            TreeNode _NoEnderecamento = ArvoreProjeto.Nodes["NoProjeto"].Nodes["NoEnderecamento"];
            int _pos = 0;
            switch (sender.AddressType)
            {
                case AddressTypeEnum.DigitalInput:
                    _pos = _NoEnderecamento.Nodes["NoEntradas"].Nodes.IndexOfKey(sender.Name);

                    if (_pos >= 0)
                        _NoEnderecamento.Nodes["NoEntradas"].Nodes[_pos].Text = sender.Name + (sender.Comment == "" ? "" : " - " + sender.Comment);
                    break;
                case AddressTypeEnum.DigitalOutput:
                    _pos = _NoEnderecamento.Nodes["NoSaidas"].Nodes.IndexOfKey(sender.Name);

                    if (_pos >= 0)
                        _NoEnderecamento.Nodes["NoSaidas"].Nodes[_pos].Text = sender.Name + (sender.Comment == "" ? "" : " - " + sender.Comment);
                    break;
                case AddressTypeEnum.DigitalMemory:
                    _pos = _NoEnderecamento.Nodes["NoMemoria"].Nodes.IndexOfKey(sender.Name);

                    if (_pos >= 0)
                        _NoEnderecamento.Nodes["NoMemoria"].Nodes[_pos].Text = sender.Name + (sender.Comment == "" ? "" : " - " + sender.Comment);
                    break;
                case AddressTypeEnum.DigitalMemoryCounter:
                    _pos = _NoEnderecamento.Nodes["NoContadores"].Nodes.IndexOfKey(sender.Name);

                    if (_pos >= 0)
                        _NoEnderecamento.Nodes["NoContadores"].Nodes[_pos].Text = sender.Name + (sender.Comment == "" ? "" : " - " + sender.Comment);
                    break;
                case AddressTypeEnum.DigitalMemoryTimer:
                    _pos = _NoEnderecamento.Nodes["NoTemporizadores"].Nodes.IndexOfKey(sender.Name);

                    if (_pos >= 0)
                        _NoEnderecamento.Nodes["NoTemporizadores"].Nodes[_pos].Text = sender.Name + (sender.Comment == "" ? "" : " - " + sender.Comment);
                    break;
            }

        }

        /// <summary>
        /// Aloca e realoca na No objeto de enderecamento do programa atual
        /// uma quantidade especificada do tipo de memoria solicitado
        /// </summary>
        /// <param name="e">Enderecamento do programa</param>
        /// <param name="tp">tipo de memoria a ser realocada</param>
        /// <param name="qtdEnd">Quantidade do tipo desejada</param>
        public int AlocaEnderecamentoMemoria(List<Address> _lstE, AddressTypeEnum tp, int qtdEnd)
        {
            /// Atalho para o No de enderecamento
            TreeNode _NoEnderecamento = ArvoreProjeto.Nodes["NoProjeto"].Nodes["NoEnderecamento"];
            String _txtNoEndereco = "";

            int _qtdAtual = 1;
            switch (tp)
            {
                case AddressTypeEnum.DigitalMemory:
                    _txtNoEndereco = "NoMemoria";
                    break;
                case AddressTypeEnum.DigitalMemoryCounter:
                    _txtNoEndereco = "NoContadores";
                    break;
                case AddressTypeEnum.DigitalMemoryTimer:
                    _txtNoEndereco = "NoTemporizadores";
                    break;
            }

            IndicaEnderecoEmUso(this.program, tp);

            _NoEnderecamento.Nodes[_txtNoEndereco].Nodes.Clear();
            _qtdAtual = _lstE.Count;
            if ((_qtdAtual == 0) || (_qtdAtual < qtdEnd))
            {
                for (int i = _qtdAtual + 1; i <= qtdEnd; i++)
                    _lstE.Add(new Address(tp, i, program.device));
            }
            else if (_qtdAtual > qtdEnd)
            {
                for (int i = (_qtdAtual - 1); i >= qtdEnd; i--)
                {
                    if (!_lstE[i].Used)
                    {
                        _lstE[i] = null;
                        _lstE.RemoveAt(i);
                    }
                    else
                        break;
                }
            }

            foreach (Address el in _lstE)
            {
                _NoEnderecamento.Nodes[_txtNoEndereco].Nodes.Add(el.Name, el.Name + (el.Comment == "" ? "" : " - " + el.Comment));
                _NoEnderecamento.Nodes[_txtNoEndereco].Nodes[el.Name].Tag = el;
                el.EditedCommentEvent += new EditedCommentEventHandler(Endereco_MudouComentario);
            }

            return 0;
        }

        private void IndicaEnderecoEmUso(LadderProgram program, AddressTypeEnum addressType)
        {
            program.addressing.LimpaIndicacaoEmUso();
            foreach (Line line in program.Lines)
            {
                line.instructions.AddRange(line.outputs);
                foreach (Instruction instruction in line.instructions)
                {
                    switch (instruction.OpCode)
                    {
                        /// pporque disso aqui
                        case OperationCode.NormallyOpenContact:
                        case OperationCode.NormallyClosedContact:
                        case OperationCode.OutputCoil:
                        case OperationCode.Timer:
                        case OperationCode.Counter:
                        case OperationCode.Reset:
                            if (instruction.IsAllOperandsOk())
                            {
                                Address _el = (Address)instruction.GetOperand(0);
                                if (_el.AddressType == addressType)
                                    _el.Used = true;
                            }
                            break;
                    }
                }
                line.instructions.RemoveRange(line.instructions.Count - line.outputs.Count, line.outputs.Count);
            }
        }

        public void InsereEnderecoNoSimbolo(VisualInstructionUserControl _cL, Address _end)
        {
            if (!_cL.IsDisposed)
            {
                _cL.SetOperand(0, _end);
                _cL.Refresh();
            }
        }
    }
}