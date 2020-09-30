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

                AlocateAddressingMemoryAndTimerAndCounter(program.addressing.ListMemoryAddress, AddressTypeEnum.DigitalMemory, 10);
                AlocateAddressingMemoryAndTimerAndCounter(program.addressing.ListTimerAddress, AddressTypeEnum.DigitalMemoryTimer, 10);
                AlocateAddressingMemoryAndTimerAndCounter(program.addressing.ListCounterAddress, AddressTypeEnum.DigitalMemoryCounter, 10);
            }
            else
            {
                if (program.device == null)
                    program.device = new Device(1);
                AlocateIOAddressing();

                AlocateAddressingMemoryAndTimerAndCounter(program.addressing.ListMemoryAddress, AddressTypeEnum.DigitalMemory, program.addressing.ListMemoryAddress.Count);
                AlocateAddressingMemoryAndTimerAndCounter(program.addressing.ListTimerAddress, AddressTypeEnum.DigitalMemoryTimer, program.addressing.ListTimerAddress.Count);
                AlocateAddressingMemoryAndTimerAndCounter(program.addressing.ListCounterAddress, AddressTypeEnum.DigitalMemoryCounter, program.addressing.ListCounterAddress.Count);

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
                    DeviceForm deviceForm = new DeviceForm(program.device);
                    if (deviceForm.ShowDialog() == DialogResult.OK)
                    {
                        int i = 0;
                        foreach (Pin pin in program.device.pins)
                        {
                            pin.Type = deviceForm.lstEndModificado[i];
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
                        AlocateAddressingMemoryAndTimerAndCounter(program.addressing.ListMemoryAddress, AddressTypeEnum.DigitalMemory, addressingForm.NumberOfMemoryAddresses);
                        AlocateAddressingMemoryAndTimerAndCounter(program.addressing.ListTimerAddress, AddressTypeEnum.DigitalMemoryTimer, addressingForm.NumberOfTimerAddresses);
                        AlocateAddressingMemoryAndTimerAndCounter(program.addressing.ListCounterAddress, AddressTypeEnum.DigitalMemoryCounter, addressingForm.NumberOfCounterAddresses);
                    }
                    break;
                default:
                    if (e.Node.Tag != null)
                        if (this.ladderForm.VisualInstruction != null)
                        {
                            InsertAddressAtInstruction(this.ladderForm.VisualInstruction, (Address)e.Node.Tag);
                            this.ladderForm.ActiveControl = this.ladderForm.VisualInstruction;
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

        private TreeNodeCollection GetAddressingNodesByAddressType(AddressTypeEnum type)
        {
            switch (type)
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
            foreach (Address address in program.device.addressesToEachPinList)
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
            TreeNodeCollection nodeToUpdateAddress = GetAddressingNodesByAddressType(editedAddress.AddressType);
            int position = nodeToUpdateAddress.IndexOfKey(editedAddress.Name);
            if (position >= 0)
                nodeToUpdateAddress[position].Text = editedAddress.GetNameAndComment();
            tvnProjectTree.EndUpdate();
        }

        public int AlocateAddressingMemoryAndTimerAndCounter(List<Address> adresses, AddressTypeEnum type, int numberOfAddresses)
        {
            //String _txtNoEndereco = "";

            //switch (type)
            //{
            //    case AddressTypeEnum.DigitalMemory:
            //        _txtNoEndereco = "tvnMemoriesNode";
            //        break;
            //    case AddressTypeEnum.DigitalMemoryCounter:
            //        _txtNoEndereco = "tvnCountersNode";
            //        break;
            //    case AddressTypeEnum.DigitalMemoryTimer:
            //        _txtNoEndereco = "tvnTimersNode";
            //        break;
            //}

            IndicateAddressUsed(this.program, type);

            //AddressingNode.Nodes[_txtNoEndereco].Nodes.Clear();
            TreeNodeCollection nodeToUpdate = GetAddressingNodesByAddressType(type);
            nodeToUpdate.Clear();


            int currentNumber = adresses.Count;
            if (currentNumber == 0 || currentNumber < numberOfAddresses)
            {
                for (int i = currentNumber + 1; i <= numberOfAddresses; i++)
                    adresses.Add(new Address(type, i, program.device));
            }
            else if (currentNumber > numberOfAddresses)
            {
                for (int i = (currentNumber - 1); i >= numberOfAddresses; i--)
                {
                    if (!adresses[i].Used)
                    {
                        adresses[i] = null;
                        adresses.RemoveAt(i);
                    }
                    else
                        break;
                }
            }

            foreach (Address address in adresses)
            {
                nodeToUpdate.Add(address.Name, address.GetNameAndComment());
                nodeToUpdate[address.Name].Tag = address;
                address.EditedCommentEvent += new EditedCommentEventHandler(Address_EditedComment);
            }
            return 0;
        }

        private void IndicateAddressUsed(LadderProgram program, AddressTypeEnum addressType)
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
                                Address address = (Address)instruction.GetOperand(0);
                                if (address.AddressType == addressType)
                                    address.Used = true;
                            }
                            break;
                    }
                }
                line.instructions.RemoveRange(line.instructions.Count - line.outputs.Count, line.outputs.Count);
            }
        }

        public void InsertAddressAtInstruction(VisualInstructionUserControl visualInstruction, Address address)
        {
            if (!visualInstruction.IsDisposed)
            {
                visualInstruction.SetOperand(0, address);
                visualInstruction.Refresh();
            }
        }
    }
}