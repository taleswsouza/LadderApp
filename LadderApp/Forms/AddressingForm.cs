using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LadderApp
{
    public partial class AddressingForm : Form
    {
        public AddressingForm()
        {
            InitializeComponent();
        }

        public AddressingForm(Addressing addressing)
        {
            InitializeComponent();

            txtInput.Value = addressing.ListInputAddress.Count;
            txtInput.Enabled = false;

            txtOutput.Value = addressing.ListOutputAddress.Count;
            txtOutput.Enabled = false;

            txtMemory.Value = addressing.ListMemoryAddress.Count;
            txtMemory.Enabled = true;

            txtTimer.Value = addressing.ListTimerAddress.Count;
            txtTimer.Enabled = true;

            txtCounter.Value = addressing.ListCounterAddress.Count;
            txtCounter.Enabled = true;
        }

        public int NumberOfMemoryAddresses { get; private set; } = 0;
        public int NumberOfTimerAddresses { get; private set; } = 0;
        public int NumberOfCounterAddresses { get; private set; } = 0;

        private void CloseWindow(object sender, EventArgs e)
        {
            NumberOfMemoryAddresses = Decimal.ToInt32(txtMemory.Value);
            NumberOfTimerAddresses = Decimal.ToInt32(txtTimer.Value);
            NumberOfCounterAddresses = Decimal.ToInt32(txtCounter.Value);

            this.Close();
        }

        private void Cancel(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}