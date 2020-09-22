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

        public AddressingForm(Addressing ep)
        {
            InitializeComponent();

            txtEntrada.Value = ep.lstIOEntrada.Count;
            txtEntrada.Enabled = false;

            txtSaida.Value = ep.lstIOSaida.Count;
            txtSaida.Enabled = false;

            txtMemoria.Value = ep.lstMemoria.Count;
            txtMemoria.Enabled = true;

            txtTemporizador.Value = ep.lstTemporizador.Count;
            txtTemporizador.Enabled = true;

            txtContador.Value = ep.lstContador.Count;
            txtContador.Enabled = true;
        }

        private int iQtdMemoria = 0;
        public int QtdMemoria
        {
            get { return iQtdMemoria; }
        }

        private int iQtdTemporizador = 0;
        public int QtdTemporizador
        {
            get { return iQtdTemporizador; }
        }

        private int iQtdContador = 0;
        public int QtdContador
        {
            get { return iQtdContador; }
        }

        private void FechaJanela(object sender, EventArgs e)
        {
            iQtdMemoria = (int)txtMemoria.Value;
            iQtdTemporizador = (int)txtTemporizador.Value;
            iQtdContador = (int)txtContador.Value;

            this.Close();
        }
    }
}