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
        public enum ProgramStatus
        {
            NotInitialized,
            New,
            Open,
            Saved
        }
        public ProgramStatus Status { get; set; } = ProgramStatus.NotInitialized;

        public ProjectForm(LadderProgram program) : this()
        {
            this.Program = program;
        }

        public ProjectForm()
        {
            InitializeComponent();
        }

        public LadderForm LadderForm { get; set; }
        public LadderProgram Program { get; set; } = new LadderProgram();
        public string PathFile { get; set; } = "";


        public void SetText()
        {
            this.Text = Program.Name;
        }

        private void ProjetoLadder_Resize(object sender, EventArgs e)
        {
            tvnProjectTree.Location = new Point(0, 0);
            tvnProjectTree.Size = new Size(this.Size.Width, this.Size.Height);
        }

        private void ProjetoLadder_Load(object sender, EventArgs e)
        {
            tvnProjectTree.TopNode.Expand();

            if (Status == ProgramStatus.NotInitialized)
            {
                Program.device = new Device();

                AlocateIOAddressing();

                AlocateAddressingMemoryAndTimerAndCounter(Program.addressing.ListMemoryAddress, AddressTypeEnum.DigitalMemory, 10);
                AlocateAddressingMemoryAndTimerAndCounter(Program.addressing.ListTimerAddress, AddressTypeEnum.DigitalMemoryTimer, 10);
                AlocateAddressingMemoryAndTimerAndCounter(Program.addressing.ListCounterAddress, AddressTypeEnum.DigitalMemoryCounter, 10);
            }
            else
            {
                if (Program.device == null)
                    Program.device = new Device();
                AlocateIOAddressing();

                AlocateAddressingMemoryAndTimerAndCounter(Program.addressing.ListMemoryAddress, AddressTypeEnum.DigitalMemory, Program.addressing.ListMemoryAddress.Count);
                AlocateAddressingMemoryAndTimerAndCounter(Program.addressing.ListTimerAddress, AddressTypeEnum.DigitalMemoryTimer, Program.addressing.ListTimerAddress.Count);
                AlocateAddressingMemoryAndTimerAndCounter(Program.addressing.ListCounterAddress, AddressTypeEnum.DigitalMemoryCounter, Program.addressing.ListCounterAddress.Count);

                Program.ReindexAddresses();
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
                    LadderForm.Close();
                    break;
                case DialogResult.No:
                    LadderForm.Close();
                    break;
            }
        }

        private void tvnProjectTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            switch (e.Node.Name)
            {
                case "tvnLadderProgramNode":
                    OpenLadderForm();
                    break;
                case "tvnDeviceNode":
                    DeviceForm deviceForm = new DeviceForm(Program.device);
                    if (deviceForm.ShowDialog() == DialogResult.OK)
                    {
                        int i = 0;
                        foreach (Pin pin in Program.device.Pins)
                        {
                            pin.Type = deviceForm.PinTypeList[i];
                            i++;
                        }
                        Program.device.RealocatePinAddresses();
                        AlocateIOAddressing();
                    }
                    break;
                case "tvnAddressingConfigurationNode":
                    AddressingForm addressingForm = new AddressingForm(Program.addressing);
                    addressingForm.Owner = this;
                    if (addressingForm.ShowDialog() == DialogResult.OK)
                    {
                        AlocateAddressingMemoryAndTimerAndCounter(Program.addressing.ListMemoryAddress, AddressTypeEnum.DigitalMemory, addressingForm.NumberOfMemoryAddresses);
                        AlocateAddressingMemoryAndTimerAndCounter(Program.addressing.ListTimerAddress, AddressTypeEnum.DigitalMemoryTimer, addressingForm.NumberOfTimerAddresses);
                        AlocateAddressingMemoryAndTimerAndCounter(Program.addressing.ListCounterAddress, AddressTypeEnum.DigitalMemoryCounter, addressingForm.NumberOfCounterAddresses);
                    }
                    break;
                default:
                    if (e.Node.Tag != null)
                        if (this.LadderForm.VisualInstruction != null)
                        {
                            InsertAddressAtInstruction(this.LadderForm.VisualInstruction, (Address)e.Node.Tag);
                            this.LadderForm.ActiveControl = this.LadderForm.VisualInstruction;
                        }
                    break;
            }
        }

        public void OpenLadderForm()
        {
            LadderForm = new LadderForm(Program);
            LadderForm.projectForm = this;
            LadderForm.MdiParent = this.MdiParent;
            LadderForm.ReorganizeLines();
            LadderForm.Show();
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

            Program.addressing.ListInputAddress.Clear();
            Program.addressing.ListOutputAddress.Clear();
            foreach (Address address in Program.device.PinAddresses)
            {
                address.SetDevice(Program.device);
                switch (address.AddressType)
                {
                    case AddressTypeEnum.DigitalInput:
                        Program.addressing.ListInputAddress.Add(address);
                        InputsNodes.Add(address.Name, address.GetNameAndComment());
                        InputsNodes[address.Name].Tag = address;
                        break;
                    case AddressTypeEnum.DigitalOutput:
                        Program.addressing.ListOutputAddress.Add(address);
                        OutputsNodes.Add(address.Name, address.GetNameAndComment());
                        OutputsNodes[address.Name].Tag = address;
                        break;
                }
                address.EditedCommentEvent += new EditedCommentEventHandler(Address_EditedComment);
            }
            tvnProjectTree.EndUpdate();
        }

        void Address_EditedComment(Address sender)
        {
            tvnProjectTree.BeginUpdate();
            TreeNodeCollection nodeToUpdateAddress = GetAddressingNodesByAddressType(sender.AddressType);
            int position = nodeToUpdateAddress.IndexOfKey(sender.Name);
            if (position >= 0)
                nodeToUpdateAddress[position].Text = sender.GetNameAndComment();
            tvnProjectTree.EndUpdate();
        }

        public int AlocateAddressingMemoryAndTimerAndCounter(List<Address> adresses, AddressTypeEnum type, int numberOfAddresses)
        {
            IndicateAddressUsed(this.Program, type);

            TreeNodeCollection nodeToUpdate = GetAddressingNodesByAddressType(type);
            nodeToUpdate.Clear();


            int currentNumber = adresses.Count;
            if (currentNumber == 0 || currentNumber < numberOfAddresses)
            {
                for (int i = currentNumber + 1; i <= numberOfAddresses; i++)
                    adresses.Add(new Address(type, i, Program.device));
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
            program.addressing.CleanUsedIndication();
            foreach (Line line in program.Lines)
            {
                line.Instructions.AddRange(line.Outputs);
                foreach (Instruction instruction in line.Instructions)
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
                line.Instructions.RemoveRange(line.Instructions.Count - line.Outputs.Count, line.Outputs.Count);
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