using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LadderApp
{
    public partial class DeviceForm : Form
    {
        private Color DefaultTextColor = Color.Black;
        Color DefaultUndefinedColor = Color.Red;
        Color DefaultDefinedColor = Color.Green;

        public List<AddressTypeEnum> PinTypeList { get; set; } = new List<AddressTypeEnum>();

        public DeviceForm()
        {
            InitializeComponent();
        }

        public DeviceForm(Device device) : this()
        {
            lblManufacturer.Text = "Manufacturer: " + device.Manufacturer;
            lblSeries.Text = "Series: " + device.Series;
            lblModel.Text = "Model: " + device.Model;
            lblNumberOfPorts.Text = "Number of ports: " + device.NumberOfPorts.ToString();
            lblNumberOfBitsPerPort.Text = "Number of bits per port: " + device.NumberBitsByPort.ToString();

            int i = 1;
            foreach (Pin pin in device.Pins)
            {
                Color color = DefaultTextColor;
                String pinText = "(P" + (((i - 1) / device.NumberBitsByPort) + 1) + "." + ((i - 1) - ((Int16)((i - 1) / device.NumberBitsByPort) * device.NumberBitsByPort)) + ")";
                switch (pin.PinType)
                {
                    case PinTypeEnum.IODigitalInputOrOutput:
                        if (pin.Type == AddressTypeEnum.None)
                        {
                            pinText += "-Not Used";
                            color = DefaultUndefinedColor;
                        }
                        else if (pin.Type == AddressTypeEnum.DigitalInput)
                        {
                            pinText += "-Input";
                            color = DefaultDefinedColor;
                        }
                        else if (pin.Type == AddressTypeEnum.DigitalOutput)
                        {
                            pinText += "-Output";
                            color = DefaultDefinedColor;
                        }
                        break;
                    case PinTypeEnum.IODigitalInput:
                        pinText += "-Input";
                        break;
                    case PinTypeEnum.IODigitalOutput:
                        pinText += "-Output";
                        break;
                    default:
                        pinText += "-Unavailable";
                        break;
                }
                tvnPinsTree.Nodes[0].Nodes.Add(pinText);
                tvnPinsTree.Nodes[0].Nodes[i-1].ForeColor = color;
                tvnPinsTree.Nodes[0].Nodes[i-1].Tag = pin.PinType;
                PinTypeList.Add(pin.Type);
                color = DefaultTextColor;
                i++;
            }
            grpPinConfiguration.Visible = false;
        }

        private void tvnPinsTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            PinSelectedEvent(e.Node);
        }

        private void tvnPinsTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            PinSelectedEvent(e.Node);
        }

        private void PinSelectedEvent(TreeNode e)
        {

            if (e.Text.StartsWith("(P"))
            {
                grpPinConfiguration.Visible = true;
                grpPinConfiguration.Text = "Bit Configuration : " + e.Text.Substring(1, e.Text.IndexOf(")-") - 1);

                rbNotUsed.Enabled = true;
                rbInput.Enabled = true;
                rbOutput.Enabled = true;
                rbUnavailable.Enabled = true;

                switch ((PinTypeEnum)e.Tag)
                {
                    case PinTypeEnum.IODigitalInputOrOutput:
                        rbNotUsed.Enabled = true;
                        rbInput.Enabled = true;
                        rbOutput.Enabled = true;
                        rbUnavailable.Enabled = false;

                        switch (PinTypeList[e.Index])
                        {
                            case AddressTypeEnum.None:
                                rbNotUsed.Checked = true;
                                break;
                            case AddressTypeEnum.DigitalInput:
                                rbInput.Checked = true;
                                break;
                            case AddressTypeEnum.DigitalOutput:
                                rbOutput.Checked = true;
                                break;
                        }

                        break;
                    case PinTypeEnum.IODigitalInput:
                        rbNotUsed.Enabled = false;
                        rbInput.Checked = true;
                        rbOutput.Enabled = false;
                        rbUnavailable.Enabled = false;
                        break;
                    case PinTypeEnum.IODigitalOutput:
                        rbNotUsed.Enabled = false;
                        rbInput.Enabled = false;
                        rbOutput.Checked = true;
                        rbUnavailable.Enabled = false;
                        break;
                    default:
                        rbNotUsed.Enabled = false;
                        rbInput.Enabled = false;
                        rbOutput.Enabled = false;
                        rbUnavailable.Checked = true;
                        break;
                }
            }
            else
                grpPinConfiguration.Visible = false;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            string pinText = tvnPinsTree.SelectedNode.Text.Substring(0, tvnPinsTree.SelectedNode.Text.IndexOf(")-") + 1);

            Color color = DefaultTextColor;

            if (tvnPinsTree.SelectedNode.Text.StartsWith("(P"))
            {
                switch ((PinTypeEnum)tvnPinsTree.SelectedNode.Tag)
                {
                    case PinTypeEnum.IODigitalInputOrOutput:
                        color = DefaultDefinedColor;
                        if (rbNotUsed.Checked == true)
                        {
                            PinTypeList[tvnPinsTree.SelectedNode.Index] = AddressTypeEnum.None;
                            pinText += "-Not Used";
                            color = DefaultUndefinedColor;
                        }
                        else if (rbInput.Checked == true)
                        {
                            PinTypeList[tvnPinsTree.SelectedNode.Index] = AddressTypeEnum.DigitalInput;
                            pinText += "-Input";
                        }
                        else if (rbOutput.Checked == true)
                        {
                            PinTypeList[tvnPinsTree.SelectedNode.Index] = AddressTypeEnum.DigitalOutput;
                            pinText += "-Output";
                        }
                        break;
                    case PinTypeEnum.IODigitalInput:
                        pinText += "-Input";
                        color = DefaultDefinedColor;
                        PinTypeList[tvnPinsTree.SelectedNode.Index] = AddressTypeEnum.DigitalInput;
                        break;
                    case PinTypeEnum.IODigitalOutput:
                        pinText += "-Output";
                        color = DefaultDefinedColor;
                        PinTypeList[tvnPinsTree.SelectedNode.Index] = AddressTypeEnum.DigitalOutput;
                        break;
                    default:
                        PinTypeList[tvnPinsTree.SelectedNode.Index] = AddressTypeEnum.None;
                        break;
                }
                tvnPinsTree.SelectedNode.Text = pinText;
                tvnPinsTree.SelectedNode.ForeColor = color;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}