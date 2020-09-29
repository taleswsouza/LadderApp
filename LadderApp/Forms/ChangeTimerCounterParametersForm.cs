using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LadderApp.Formularios
{
    public partial class ChangeTimerCounterParametersForm : Form
    {
        private OperationCode opCode = OperationCode.None;

        public OperationCode OpCode { get => opCode; set => opCode = value; }
        public int Type { get; set; } = 0;
        public int TimeBase { get; set; } = 0;
        public int Preset { get; set; } = 0;
        public int Accumulated { get; set; } = 0;

        public ChangeTimerCounterParametersForm(OperationCode opCode)
        {
            InitializeComponent();
            switch (opCode)
            {
                case OperationCode.Counter:
                case OperationCode.Timer:
                    OpCode = opCode;
                    break;
                default:
                    throw new Exception("Not a valid opCode!");
            }
        }

        private void AlteraTemporizadorContador_Load(object sender, EventArgs e)
        {
            switch (this.OpCode)
            {
                case OperationCode.Counter:
                    Text = "Change Counter";
                    cmbTimeBase.Visible = false;
                    lblTimeBase.Visible = false;
                    cmbType.Items.Clear();
                    cmbType.Items.Add("CTU");
                    cmbType.Items.Add("CTD");
                    break;
                case OperationCode.Timer:
                    Text = "Change Timer";
                    cmbType.Items.Clear();
                    cmbType.Items.Add("TON");
                    cmbType.Items.Add("TOF");
                    cmbType.Items.Add("RTO");
                    break;
            }

            cmbType.SelectedIndex = Type;
            cmbTimeBase.SelectedIndex = TimeBase;
            txtPreset.Value = Preset;
            txtAccumulated.Value = Accumulated;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Type = cmbType.SelectedIndex;
            TimeBase = cmbTimeBase.SelectedIndex;
            Preset = decimal.ToInt32(txtPreset.Value);
            Accumulated = decimal.ToInt32(txtAccumulated.Value);
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}