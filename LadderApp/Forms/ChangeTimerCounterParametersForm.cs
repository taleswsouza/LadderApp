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
        public CodigosInterpretaveis Funcao = CodigosInterpretaveis.NENHUM;
        public Int32 Tipo = 0;
        public Int32 BaseTempo = 0;
        public Int32 Preset = 0;
        public Int32 Acumulado = 0;

        public ChangeTimerCounterParametersForm(CodigosInterpretaveis funcao)
        {
            InitializeComponent();

            switch (funcao)
            {
                case CodigosInterpretaveis.CONTADOR:
                case CodigosInterpretaveis.TEMPORIZADOR:
                    Funcao = funcao;
                    break;
            }
        }

        private void AlteraTemporizadorContador_Load(object sender, EventArgs e)
        {
            switch (this.Funcao)
            {
                case CodigosInterpretaveis.CONTADOR:
                    this.Text = this.Text.Replace("<%>", "Counter");
                    cmbBaseTempo.Visible = false;
                    lblBaseTempo.Visible = false;
                    cmbTipo.Items.Clear();
                    cmbTipo.Items.Add("CTU");
                    cmbTipo.Items.Add("CTD");
                    break;
                case CodigosInterpretaveis.TEMPORIZADOR:
                    this.Text = this.Text.Replace("<%>", "Timer");
                    cmbTipo.Items.Clear();
                    cmbTipo.Items.Add("TON");
                    cmbTipo.Items.Add("TOF");
                    cmbTipo.Items.Add("RTO");
                    break;
                default:
                    break;
            }

            cmbTipo.SelectedIndex = this.Tipo;
            cmbBaseTempo.SelectedIndex = this.BaseTempo;
            txtPreset.Value = this.Preset;
            txtAcumulado.Value = this.Acumulado;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Tipo = (int)cmbTipo.SelectedIndex;
            this.BaseTempo = (int)cmbBaseTempo.SelectedIndex;
            this.Preset = (int)txtPreset.Value;
            this.Acumulado = (int)txtAcumulado.Value;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}