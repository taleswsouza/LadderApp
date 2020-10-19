using LadderApp.Model;
using LadderApp.Services;
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
        private AddressingServices addressingServices;

        public enum ProgramStatus
        {
            NotInitialized,
            New,
            Open,
            Saved
        }
        public ProgramStatus Status { get; set; } = ProgramStatus.NotInitialized;

        public ProjectForm(LadderProgram program, AddressingServices addressingServices) : this()
        {
            this.Program = program;
            this.addressingServices = addressingServices;
            this.addressingServices.SetAddressing(program.addressing);

            if (Status == ProgramStatus.NotInitialized && Program.device == null)
            {
                Program.device = DeviceFactory.CreateNewDevice();

                addressingServices.AlocateIOAddressing(Program.device);
                addressingServices.AlocateAddressingMemoryAndTimerAndCounter(Program, Program.addressing.ListMemoryAddress, AddressTypeEnum.DigitalMemory, 10);

                addressingServices.AlocateAddressingMemoryAndTimerAndCounter(Program, Program.addressing.ListTimerAddress, AddressTypeEnum.DigitalMemoryTimer, 10);

                addressingServices.AlocateAddressingMemoryAndTimerAndCounter(Program, Program.addressing.ListCounterAddress, AddressTypeEnum.DigitalMemoryCounter, 10);
            }
            else
            {
                Status = ProgramStatus.Open;
                if (Program.device == null)
                {
                    Program.device = DeviceFactory.CreateNewDevice();
                }

                addressingServices.AlocateIOAddressing(Program.device);

                addressingServices.AlocateAddressingMemoryAndTimerAndCounter(Program, Program.addressing.ListMemoryAddress, AddressTypeEnum.DigitalMemory, Program.addressing.ListMemoryAddress.Count);

                addressingServices.AlocateAddressingMemoryAndTimerAndCounter(Program, Program.addressing.ListTimerAddress, AddressTypeEnum.DigitalMemoryTimer, Program.addressing.ListTimerAddress.Count);

                addressingServices.AlocateAddressingMemoryAndTimerAndCounter(Program, Program.addressing.ListCounterAddress, AddressTypeEnum.DigitalMemoryCounter, Program.addressing.ListCounterAddress.Count);

                addressingServices.ReindexAddresses(Program);
            }

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

        private void ProjectForm_Resize(object sender, EventArgs e)
        {
            tvnProjectTree.Location = new Point(0, 0);
            tvnProjectTree.Size = new Size(this.Size.Width, this.Size.Height);
        }

        private void ProjectForm_Load(object sender, EventArgs e)
        {
            tvnProjectTree.TopNode.Expand();

            AlocateIOAddressingProjectTree();
            AlocateAddressingMemoryAndTimerAndCounterToProjectTree(Program.addressing.ListMemoryAddress, AddressTypeEnum.DigitalMemory);
            AlocateAddressingMemoryAndTimerAndCounterToProjectTree(Program.addressing.ListTimerAddress, AddressTypeEnum.DigitalMemoryTimer);
            AlocateAddressingMemoryAndTimerAndCounterToProjectTree(Program.addressing.ListCounterAddress, AddressTypeEnum.DigitalMemoryCounter);

            OpenLadderForm();
        }


        private void ProjectForm_Shown(object sender, EventArgs e)
        {
            MainWindowForm mainWindowForm = (MainWindowForm)this.MdiParent;
            mainWindowForm.ResetWindowLayout();
        }

        private void ProjectForm_FormClosing(object sender, FormClosingEventArgs e)
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
                        addressingServices.RealocatePinAddressesByPinTypesFromDevice(Program.device);
                        addressingServices.AlocateIOAddressing(Program.device);

                        AlocateIOAddressingProjectTree();
                    }
                    break;
                case "tvnAddressingConfigurationNode":
                    AddressingForm addressingForm = new AddressingForm(Program.addressing);
                    addressingForm.Owner = this;
                    if (addressingForm.ShowDialog() == DialogResult.OK)
                    {
                        addressingServices.AlocateAddressingMemoryAndTimerAndCounter(Program, Program.addressing.ListMemoryAddress, AddressTypeEnum.DigitalMemory, addressingForm.NumberOfMemoryAddresses);
                        addressingServices.AlocateAddressingMemoryAndTimerAndCounter(Program, Program.addressing.ListTimerAddress, AddressTypeEnum.DigitalMemoryTimer, addressingForm.NumberOfTimerAddresses);
                        addressingServices.AlocateAddressingMemoryAndTimerAndCounter(Program, Program.addressing.ListCounterAddress, AddressTypeEnum.DigitalMemoryCounter, addressingForm.NumberOfCounterAddresses);

                        AlocateAddressingMemoryAndTimerAndCounterToProjectTree(Program.addressing.ListMemoryAddress, AddressTypeEnum.DigitalMemory);
                        AlocateAddressingMemoryAndTimerAndCounterToProjectTree(Program.addressing.ListTimerAddress, AddressTypeEnum.DigitalMemoryTimer);
                        AlocateAddressingMemoryAndTimerAndCounterToProjectTree(Program.addressing.ListCounterAddress, AddressTypeEnum.DigitalMemoryCounter);

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

        public void AlocateIOAddressingProjectTree()
        {
            tvnProjectTree.BeginUpdate();

            InputsNodes.Clear();
            OutputsNodes.Clear();

            foreach (Address address in Program.device.PinAddresses)
            {
                switch (address.AddressType)
                {
                    case AddressTypeEnum.DigitalInput:
                        InputsNodes.Add(address.Name, address.GetNameAndComment());
                        InputsNodes[address.Name].Tag = address;
                        break;
                    case AddressTypeEnum.DigitalOutput:
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
        private void AlocateAddressingMemoryAndTimerAndCounterToProjectTree(List<Address> addresses, AddressTypeEnum type)
        {
            TreeNodeCollection nodeToUpdate = GetAddressingNodesByAddressType(type);
            nodeToUpdate.Clear();
            foreach (Address address in addresses)
            {
                address.SetDevice(Program.device);
                nodeToUpdate.Add(address.Name, address.GetNameAndComment());
                nodeToUpdate[address.Name].Tag = address;
                address.EditedCommentEvent += new EditedCommentEventHandler(Address_EditedComment);
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