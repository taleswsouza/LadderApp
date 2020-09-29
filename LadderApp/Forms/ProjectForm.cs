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
            tvnProjectTree.Location = new Point(0, 0);
            tvnProjectTree.Size = new Size(this.Size.Width, this.Size.Height);
        }

        private void ProjetoLadder_Load(object sender, EventArgs e)
        {
            tvnProjectTree.TopNode.Expand();

            if (program.Status == LadderProgram.ProgramStatus.NotInitialized)
            {
                program.device = new Device(1);

                AlocateIOAddressing();

                AlocaEnderecamentoMemoria(program.addressing.ListMemoryAddress, AddressTypeEnum.DigitalMemory, 10);
                AlocaEnderecamentoMemoria(program.addressing.ListTimerAddress, AddressTypeEnum.DigitalMemoryTimer, 10);
                AlocaEnderecamentoMemoria(program.addressing.ListCounterAddress, AddressTypeEnum.DigitalMemoryCounter, 10);
            }
            else
            {
                if (program.device == null)
                    program.device = new Device(1);
                AlocateIOAddressing();

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
                case "tvnLadderProgramNode":
                        OpenLadderForm();
                    break;
                case "tvnDeviceNode":
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

                        AlocateIOAddressing();
                    }
                    break;
                case "tvnAddressingConfigurationNode":
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

        private TreeNode AddressingNode => tvnProjectTree.Nodes["tvnProjectNode"].Nodes["tvnAddressingNode"];
        private TreeNodeCollection InputsNodes => AddressingNode.Nodes["tvnInputsNode"].Nodes;
        private TreeNodeCollection OutputsNodes => AddressingNode.Nodes["tvnOutputsNode"].Nodes;
        private TreeNodeCollection MemoriesNodes => AddressingNode.Nodes["tvnMemoriesNode"].Nodes;
        private TreeNodeCollection CountersNodes => AddressingNode.Nodes["tvnCountersNode"].Nodes;
        private TreeNodeCollection TimersNodes => AddressingNode.Nodes["tvnTimersNode"].Nodes;

        private TreeNodeCollection GetAddressingNodesByAddress(Address address)
        {
            switch (address.AddressType)
            {
                case AddressTypeEnum.DigitalInput:
                    return InputsNodes;
                case AddressTypeEnum.DigitalOutput:
                    return OutputsNodes;
                case AddressTypeEnum.DigitalMemory:
                    return MemoriesNodes;
                case AddressTypeEnum.DigitalMemoryCounter:
                    return CountersNodes;
                case AddressTypeEnum.DigitalMemoryTimer:
                    return TimersNodes;
                default:
                    return null;
            }
        }

        public void AlocateIOAddressing()
        {
            tvnProjectTree.BeginUpdate();

            InputsNodes.Clear();
            OutputsNodes.Clear();

            program.addressing.ListInputAddress.Clear();
            program.addressing.ListOutputAddress.Clear();
            foreach (Address address in program.device.lstEndBitPorta)
            {
                address.SetDevice(program.device);
                switch (address.AddressType)
                {
                    case AddressTypeEnum.DigitalInput:
                        program.addressing.ListInputAddress.Add(address);
                        InputsNodes.Add(address.Name, address.GetNameAndComment());
                        InputsNodes[address.Name].Tag = address;
                        break;
                    case AddressTypeEnum.DigitalOutput:
                        program.addressing.ListOutputAddress.Add(address);
                        OutputsNodes.Add(address.Name, address.GetNameAndComment());
                        OutputsNodes[address.Name].Tag = address;
                        break;
                }
                address.EditedCommentEvent += new EditedCommentEventHandler(Address_EditedComment);
            }
            tvnProjectTree.EndUpdate();
        }

        void Address_EditedComment(Address editedAddress)
        {
            tvnProjectTree.BeginUpdate();
            TreeNodeCollection nodeToUpdateAddress = GetAddressingNodesByAddress(editedAddress);
            int position = nodeToUpdateAddress.IndexOfKey(editedAddress.Name);
            if (position >= 0)
                nodeToUpdateAddress[position].Text = editedAddress.GetNameAndComment();
            tvnProjectTree.EndUpdate();
        }

        public int AlocaEnderecamentoMemoria(List<Address> _lstE, AddressTypeEnum tp, int qtdEnd)
        {
            TreeNode _NoEnderecamento = tvnProjectTree.Nodes["tvnProjectNode"].Nodes["tvnAddressingNode"];
            String _txtNoEndereco = "";

            int _qtdAtual = 1;
            switch (tp)
            {
                case AddressTypeEnum.DigitalMemory:
                    _txtNoEndereco = "tvnMemoriesNode";
                    break;
                case AddressTypeEnum.DigitalMemoryCounter:
                    _txtNoEndereco = "tvnCountersNode";
                    break;
                case AddressTypeEnum.DigitalMemoryTimer:
                    _txtNoEndereco = "tvnTimersNode";
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
                el.EditedCommentEvent += new EditedCommentEventHandler(Address_EditedComment);
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