using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LadderApp
{
    public partial class DeviceForm : Form
    {
        public List<AddressType> lstEndModificado = new List<AddressType>();

        Color corTextoPadrao = Color.Black;
        Color corPinoIndefinida = Color.Red;
        Color corPinoDefinida = Color.Green;

        public DeviceForm()
        {
            InitializeComponent();
        }

        public DeviceForm(Device dl)
        {
            String _txtPino = "";
            Color _cor = corTextoPadrao;
            InitializeComponent();
            lblFabricante.Text = "Manufacturer: " + dl.Fabricante;
            lblSerie.Text = "Series: " + dl.Serie;
            lblModelo.Text = "Model: " + dl.Modelo;
            lblQtdPortas.Text = "Number of ports: " + dl.QtdPortas.ToString();
            lblQtdBitsPorta.Text = "Number of bits per port: " + dl.QtdBitsPorta.ToString();

            int i = 1;
            int j = 0;
            foreach(BitPortasDispositivo pd in dl.lstBitPorta)
            {
                //_txtPino = "Pino " + i.ToString().PadLeft(2,'0');
                _txtPino = "(P" + (((i - 1) / dl.QtdBitsPorta) + 1) + "." + ((i - 1) - ((Int16)((i - 1) / dl.QtdBitsPorta) * dl.QtdBitsPorta)) + ")";
                switch (pd.TipoPino)
                {
                    case PinType.IO_DIGITAL_ENTRADA_OU_SAIDA:
                        if (pd.TipoDefinido == AddressType.NENHUM)
                        {
                            _txtPino += "-Not Used";
                            _cor = corPinoIndefinida;
                        }
                        else if (pd.TipoDefinido == AddressType.DIGITAL_ENTRADA)
                        {
                            _txtPino += "-Input";
                            _cor = corPinoDefinida;
                        }
                        else if (pd.TipoDefinido == AddressType.DIGITAL_SAIDA)
                        {
                            _txtPino += "-Output";
                            _cor = corPinoDefinida;
                        }
                        break;
                    case PinType.IO_DIGITAL_ENTRADA:
                        _txtPino += "-Input";
                        break;
                    case PinType.IO_DIGITAL_SAIDA:
                        _txtPino += "-Output";
                        break;
                    default:
                        _txtPino += "-Unavailable";
                        break;
                }
                //if (pd.TipoPino != TiposPinosDispositivo.NENHUM)
                //{
                    ArvorePinos.Nodes[0].Nodes.Add(_txtPino);
                    ArvorePinos.Nodes[0].Nodes[j].ForeColor = _cor;
                    ArvorePinos.Nodes[0].Nodes[j].Tag = pd.TipoPino;
                    lstEndModificado.Add(pd.TipoDefinido);
                    _cor = corTextoPadrao;
                    j++;
                //}
                i++;
                
            }
            grpConfiguraPino.Visible = false;
        }

        private void ArvorePortas_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            PinoSelecionado(e.Node);
        }

        private void ArvorePortas_AfterSelect(object sender, TreeViewEventArgs e)
        {
            PinoSelecionado(e.Node);
        }

        private void PinoSelecionado(TreeNode e)
        {

            if (e.Text.StartsWith("(P"))
            {
                grpConfiguraPino.Visible = true;
                grpConfiguraPino.Text = "Bit Configuration : " + e.Text.Substring(1, e.Text.IndexOf(")-")- 1);

                rbEntradaOuSaida.Enabled = true;
                rbEntrada.Enabled = true;
                rbSaida.Enabled = true;
                rbOutro.Enabled = true;

                switch ((PinType)e.Tag)
                {
                    case PinType.IO_DIGITAL_ENTRADA_OU_SAIDA:
                        rbEntradaOuSaida.Enabled = true;
                        rbEntrada.Enabled = true;
                        rbSaida.Enabled = true;
                        rbOutro.Enabled = false;

                        switch (lstEndModificado[e.Index])
                        {
                            case AddressType.NENHUM:
                                rbEntradaOuSaida.Checked = true;
                                break;
                            case AddressType.DIGITAL_ENTRADA:
                                rbEntrada.Checked = true;
                                break;
                            case AddressType.DIGITAL_SAIDA:
                                rbSaida.Checked = true;
                                break;
                        }

                        break;
                    case PinType.IO_DIGITAL_ENTRADA:
                        rbEntradaOuSaida.Enabled = false;
                        rbEntrada.Checked = true;
                        rbSaida.Enabled = false;
                        rbOutro.Enabled = false;
                        break;
                    case PinType.IO_DIGITAL_SAIDA:
                        rbEntradaOuSaida.Enabled = false;
                        rbEntrada.Enabled = false;
                        rbSaida.Checked = true;
                        rbOutro.Enabled = false;
                        break;
                    default:
                        rbEntradaOuSaida.Enabled = false;
                        rbEntrada.Enabled = false;
                        rbSaida.Enabled = false;
                        rbOutro.Checked = true;
                        break;
                }
            }
            else
                grpConfiguraPino.Visible = false;
        }

        private void btnAplicar_Click(object sender, EventArgs e)
        {
            String _txtPino = ArvorePinos.SelectedNode.Text.Substring(0, ArvorePinos.SelectedNode.Text.IndexOf(")-")+1);
            //String _txtPino = "(P"; //+ ((i / dl.QtdBitsPorta) + 1) + "." + ((i - 1) - ((Int16)((i - 1) / dl.QtdBitsPorta) * dl.QtdBitsPorta)) + ")";

            Color _cor = corTextoPadrao;

            if (ArvorePinos.SelectedNode.Text.StartsWith("(P"))
            {
                switch ((PinType)ArvorePinos.SelectedNode.Tag)
                {
                    case PinType.IO_DIGITAL_ENTRADA_OU_SAIDA:
                        _cor = corPinoDefinida;
                        if (rbEntradaOuSaida.Checked == true)
                        {
                            lstEndModificado[ArvorePinos.SelectedNode.Index] = AddressType.NENHUM;
                            _txtPino += "-Not Used";
                            _cor = corPinoIndefinida;
                        }
                        else if (rbEntrada.Checked == true)
                        {
                            lstEndModificado[ArvorePinos.SelectedNode.Index] = AddressType.DIGITAL_ENTRADA;
                            _txtPino += "-Input";
                        }
                        else if (rbSaida.Checked == true)
                        {
                            lstEndModificado[ArvorePinos.SelectedNode.Index] = AddressType.DIGITAL_SAIDA;
                            _txtPino += "-Output";
                        }
                        ///rbEntradaOuSaida.Checked = true;
                        break;
                    case PinType.IO_DIGITAL_ENTRADA:
                        _txtPino += "-Input";
                        _cor = corPinoDefinida;
                        lstEndModificado[ArvorePinos.SelectedNode.Index] = AddressType.DIGITAL_ENTRADA;
                        //rbEntrada.Checked = true;
                        break;
                    case PinType.IO_DIGITAL_SAIDA:
                        _txtPino += "-Output";
                        _cor = corPinoDefinida;
                        lstEndModificado[ArvorePinos.SelectedNode.Index] = AddressType.DIGITAL_SAIDA;
                        //rbSaida.Checked = true;
                        break;
                    default:
                        lstEndModificado[ArvorePinos.SelectedNode.Index] = AddressType.NENHUM;
                        //rbOutro.Checked = true;
                        break;
                }
                ArvorePinos.SelectedNode.Text = _txtPino;
                ArvorePinos.SelectedNode.ForeColor = _cor;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    
    }
}